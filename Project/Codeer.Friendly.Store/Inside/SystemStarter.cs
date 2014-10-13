using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Codeer.Friendly;
using Codeer.Friendly.Store.Properties;
using System.Threading;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Codeer.Friendly.Store.Core;
using Codeer.Friendly.Core.Inside;

namespace Codeer.Friendly.Store.Inside
{
	/// <summary>
	/// システム起動。
	/// </summary>
	static class SystemStarter
	{
		/// <summary>
		/// 起動。
		/// </summary>
		/// <param name="process">対象プロセス。</param>
        /// <param name="mainHandle">メインウィンドウのハンドル。</param>
        /// <param name="clrVersion">CLRバージョン名称。</param>
        /// <returns>システムコントローラー。</returns>
        internal static SystemController Start(Process process, IntPtr mainHandle, string clrVersion)
		{
            clrVersion = Codeer.Friendly.Store.Core.Debug.ReadDebugMark(clrVersion);
            if (!CpuTargetCheckUtility.IsSameCpu(process))
            {
                throw new FriendlyOperationException(Resources.ErrorTargetCpuDifference);
            }

            string dllName = GetCodeerFriendlyWindows(clrVersion);

            Codeer.Friendly.Store.Core.Debug.Trace("I want to be your friend.");

            LoadDll(process, dllName);
            return StartInApp(process, mainHandle, dllName);
		}

        /// <summary>
        /// CodeerFriendlyWindows.dllを対象プロセスに読み込ませる。
        /// </summary>
        /// <param name="process">対象プロセス。</param>
        /// <param name="dllName">dll名称。</param>
        static void LoadDll(Process process, string dllName)
		{
			IntPtr pLibRemote = IntPtr.Zero;
			IntPtr hThreadLoader = IntPtr.Zero;
			try
			{
				//ロードさせるDLL名称を対象プロセス内にメモリを確保して書き込む
                List<byte> szLibPathTmp = new List<byte>(Encoding.Unicode.GetBytes(dllName));
                szLibPathTmp.Add(0);//NULL終端を書き足す。
                byte[] szLibPath = szLibPathTmp.ToArray();
				pLibRemote = NativeMethods.VirtualAllocEx(process.Handle, IntPtr.Zero, new IntPtr(szLibPath.Length),
											  NativeMethods.AllocationType.Commit, NativeMethods.MemoryProtection.ReadWrite);
				if (pLibRemote == IntPtr.Zero ||
					!NativeMethods.WriteProcessMemory(process.Handle, pLibRemote, szLibPath, new IntPtr((int)szLibPath.Length), IntPtr.Zero))
				{
					throw new FriendlyOperationException(Resources.ErrorProcessAcess);
				}

                Codeer.Friendly.Store.Core.Debug.Trace("I write first letter.");

				//実行関数取得
				IntPtr pFunc = NativeMethods.GetProcAddress(NativeMethods.GetModuleHandle("Kernel32"), "LoadLibraryW");
				if (pFunc == IntPtr.Zero)
				{
					throw new FriendlyOperationException(Resources.ErrorDllLoad);
				}

				//対象プロセスにDLLをロードさせる
				IntPtr tid;
                hThreadLoader = NativeMethods.CreateRemoteThread(process.Handle, IntPtr.Zero, IntPtr.Zero,
							pFunc, pLibRemote, 0, out tid);
				if (hThreadLoader == IntPtr.Zero)
				{
					throw new FriendlyOperationException(Resources.ErrorProcessAcess);
				}
				NativeMethods.WaitForSingleObject(hThreadLoader, NativeMethods.INFINITE);

                Codeer.Friendly.Store.Core.Debug.Trace("You read first letter.");
            }
			finally
			{
				if (hThreadLoader != IntPtr.Zero)
				{
					NativeMethods.CloseHandle(hThreadLoader);
				}
				if (pLibRemote != IntPtr.Zero)
				{
					NativeMethods.VirtualFreeEx(process.Handle, pLibRemote,
							 IntPtr.Zero, NativeMethods.FreeType.Release);
				}
			}
		}

        /// <summary>
        /// プロセスでシステムを起動させる。
        /// </summary>
        /// <param name="process">対象プロセス。</param>
        /// <param name="mainHandle">メインウィンドウのハンドル。</param>
        /// <param name="dllName">サーバー側で動作させるDLL名称。</param>
        /// <returns>システムコントローラー。</returns>
        static SystemController StartInApp(Process process, IntPtr mainHandle, string dllName)
		{
            //ウィンドウハンドル受信スレッド起動
            object sync = new object();
            IntPtr recieveWindowHandle = IntPtr.Zero;
            IntPtr systemControlWindowHandle = IntPtr.Zero;
            int targetProcessId = process.Id;
            Thread recieveThread = new Thread((ThreadStart)delegate
            {
                using (SystemControlWindowInAppHandleRecieveWindow window = new SystemControlWindowInAppHandleRecieveWindow())
                {
                    lock (sync)
                    {
                        recieveWindowHandle = window.Handle;
                    }

                    //受信待ち
                    while (true)
                    {
                        NativeMethods.MSG msg = new NativeMethods.MSG();
                        if (NativeMethods.PeekMessage(ref msg, window.Handle, 0, 0, NativeMethods.PeekMsgOption.PM_REMOVE))
                        {
                            NativeMethods.TranslateMessage(ref msg);
                            NativeMethods.DispatchMessage(ref msg);
                        }
                        if (window.SystemControlWindowHandle != IntPtr.Zero)
                        {
                            lock (sync)
                            {
                                systemControlWindowHandle = window.SystemControlWindowHandle;
                            }
                            break;
                        }
                        Thread.Sleep(10);

                        //通信プロセスが消えたら終わり
                        try
                        {
                            if (Process.GetProcessById(targetProcessId) == null)
                            {
                                break;
                            }
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
            });
            recieveThread.Start();

            Codeer.Friendly.Store.Core.Debug.Trace("I make post.");

            //受信ウィンドウ生成待ち
            while (true)
            {
                lock (sync)
                {
                    if (recieveWindowHandle != IntPtr.Zero)
                    {
                        break;
                    }
                }
            }

            Codeer.Friendly.Store.Core.Debug.Trace("post maked.");

			//ロードさせたDLLのメソッドを呼び出させる
			IntPtr pServerName = IntPtr.Zero;
			IntPtr hThreadServerOpen = IntPtr.Zero;
			try
			{
                //起動情報
                string startupInfo = recieveWindowHandle.ToInt64().ToString(CultureInfo.CurrentCulture) + "+"
                    + mainHandle.ToInt64().ToString(CultureInfo.CurrentCulture) + Codeer.Friendly.Store.Core.Debug.DebugMark + "###" + typeof(SystemStarterInApp).Assembly.FullName;

				//受信ウィンドウハンドルを対象プロセス内にメモリを確保して書き込む longを文字列化して書き込む
                List<byte> startInfoTmp = new List<byte>(Encoding.Unicode.GetBytes(startupInfo));
                startInfoTmp.Add(0);//null終端を足す
                byte[] startInfo = startInfoTmp.ToArray();
				pServerName = NativeMethods.VirtualAllocEx(process.Handle, IntPtr.Zero, new IntPtr(startInfo.Length),
											  NativeMethods.AllocationType.Commit, NativeMethods.MemoryProtection.ReadWrite);
				if (pServerName == IntPtr.Zero ||
					!NativeMethods.WriteProcessMemory(process.Handle, pServerName, startInfo, new IntPtr((int)startInfo.Length), IntPtr.Zero))
				{
					throw new FriendlyOperationException(Resources.ErrorProcessAcess);
				}

                Codeer.Friendly.Store.Core.Debug.Trace("I write second letter.");

				//実行関数取得
                IntPtr pFunc = GetTargetProcAddress(process, dllName, "Initialize");
				if (pFunc == IntPtr.Zero)
				{
					throw new FriendlyOperationException(Resources.ErrorCLR);
				}

				//対象プロセスでサーバー開始メソッドを実行
				IntPtr tid;
                hThreadServerOpen = NativeMethods.CreateRemoteThread(process.Handle, IntPtr.Zero, IntPtr.Zero,
							pFunc, pServerName, 0, out tid);
				if (hThreadServerOpen == IntPtr.Zero)
				{
					throw new FriendlyOperationException(Resources.ErrorProcessAcess);
				}
				NativeMethods.WaitForSingleObject(hThreadServerOpen, NativeMethods.INFINITE);

                Codeer.Friendly.Store.Core.Debug.Trace("You read second letter.");

                //受信スレッドの終了待ち（返信待ち）
                while (recieveThread.IsAlive)
                {
                    Thread.Sleep(10);
                }

                Codeer.Friendly.Store.Core.Debug.Trace("I got address.");
			}
			finally
			{
				if (hThreadServerOpen != IntPtr.Zero)
				{
					NativeMethods.CloseHandle(hThreadServerOpen);
				}
				if (pServerName != IntPtr.Zero)
				{
					NativeMethods.VirtualFreeEx(process.Handle, pServerName,
							 IntPtr.Zero, NativeMethods.FreeType.Release);
				}
			}

            //nullなら失敗
            if (systemControlWindowHandle == IntPtr.Zero)
            {
                throw new FriendlyOperationException(Resources.ErrorProcessAcess);
            }

            //システムコントロールウィンドウのハンドルを返す
            Codeer.Friendly.Store.Core.Debug.Trace("We are friends.");
            return new SystemController(systemControlWindowHandle);
		}
        
        /// <summary>
        /// CLRバージョン名称を調整。
        /// </summary>
        /// <param name="clrVersion">CLRバージョン。</param>
        /// <returns>CLRバージョン名称。</returns>
        static string AdjustClrVersionName(string clrVersion)
        {
            return clrVersion.Replace(".", string.Empty);
        }

        /// <summary>
        /// CodeerFriendlyWindows.dllの名前。
        /// </summary>
        /// <param name="clrVersion">CLRバージョン。</param>
        /// <returns>CodeerFriendlyWindows.dllの名前。</returns>
        static string GetCodeerFriendlyWindows(string clrVersion)
        {
            clrVersion = AdjustClrVersionName(clrVersion);
            if (IntPtr.Size == 4)
            {
                return "CodeerFriendlyStore_x86_" + clrVersion;
            }
            else
            {
                return "CodeerFriendlyStore_x64_" + clrVersion;
            }
        }

        /// <summary>
        /// システムコントロールウィンドウハンドル受信。
        /// </summary>
        class SystemControlWindowInAppHandleRecieveWindow : CommunicationWindow
        {
            IntPtr _systemControlWindowHandle;

            /// <summary>
            /// システムコントロールウィンドウハンドル。
            /// </summary>
            internal IntPtr SystemControlWindowHandle { get { return _systemControlWindowHandle; } }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            internal SystemControlWindowInAppHandleRecieveWindow()
            {
                CreateHandle();
            }

            /// <summary>
            /// ウィンドウプロック。
            /// </summary>
            /// <param name="m">メッセージ。</param>
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == SystemStarterInApp.WM_NOTIFY_SYSTEM_CONTROL_WINDOW_HANDLE)
                {
                    _systemControlWindowHandle = m.WParam;
                    return;
                }
                base.WndProc(ref m);
            }
        }

        /// <summary>
        /// 対象プロセスのDLL関数アドレスの取得
        /// </summary>
        /// <param name="process">対象プロセス</param>
        /// <param name="dllName">DLL名称</param>
        /// <param name="procName">関数名称</param>
        /// <returns>アドレス</returns>
        static IntPtr GetTargetProcAddress(Process process, string dllName, string procName)
        {
            //自分のプロセスにロードして距離を計測する
            IntPtr mod = NativeMethods.LoadLibrary(dllName);
            IntPtr proc = NativeMethods.GetProcAddress(mod, procName);
            if (proc == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            //対象プロセス内のベースアドレスを取得
            IntPtr targetBase = GetModuleBase(process, dllName);
            if (targetBase == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            //対象プロセスの関数アドレスを計算
            return (IntPtr.Size == 4) ? CalcProcAddressInt(mod, proc, targetBase) : CalcProcAddressLong(mod, proc, targetBase);
        }

        /// <summary>
        /// 対象プロセスのDLL関数アドレスを計算
        /// </summary>
        /// <param name="mod">自プロセスのモジュールハンドル</param>
        /// <param name="proc">自プロセスの関数アドレス</param>
        /// <param name="targetModBase">対象プロセスのDLLベースアドレス</param>
        /// <returns>対象プロセスのDLL関数アドレス</returns>
        static IntPtr CalcProcAddressLong(IntPtr mod, IntPtr proc, IntPtr targetModBase)
        {
            ulong distance = (ulong)proc.ToInt64() - (ulong)mod.ToInt64();
            return new IntPtr((long)((ulong)targetModBase.ToInt64() + distance));
        }
                
        /// <summary>
        /// 対象プロセスのDLL関数アドレスを計算
        /// </summary>
        /// <param name="mod">自プロセスのモジュールハンドル</param>
        /// <param name="proc">自プロセスの関数アドレス</param>
        /// <param name="targetModBase">対象プロセスのDLLベースアドレス</param>
        /// <returns>対象プロセスのDLL関数アドレス</returns>
        static IntPtr CalcProcAddressInt(IntPtr mod, IntPtr proc, IntPtr targetModBase)
        {
            uint distance = (uint)proc.ToInt32() - (uint)mod.ToInt32();
            return new IntPtr((int)((uint)targetModBase.ToInt32() + distance));
        }

        /// <summary>
        /// 指定プロセスのDLLベースアドレスを取得
        /// </summary>
        /// <param name="process">プロセス</param>
        /// <param name="dllName">DLL名称</param>
        /// <returns>指定プロセスのDLLベースアドレス</returns>
        private static IntPtr GetModuleBase(Process process, string dllName)
        {
            //指定プロセスのロードしているモジュールを全取得
            IntPtr[] lphModule = new IntPtr[0];
            while (true)
            {
                uint binSize;
                if (!NativeMethods.EnumProcessModules(process.Handle, lphModule, (uint)(lphModule.Length * IntPtr.Size), out binSize))
                {
                    return IntPtr.Zero;
                }
                int modCount = (int)(binSize / IntPtr.Size);
                if (modCount == lphModule.Length)
                {
                    break;
                }
                lphModule = new IntPtr[modCount];
            }

            //指定のdllのベースアドレスを取得し、そこから距離を利用して目的の関数アドレスを取得
            for (int i = 0; i < lphModule.Length; i++)
            {
                StringBuilder filePath = new StringBuilder((1024 + 1) * 8);
                NativeMethods.GetModuleFileNameEx(process.Handle, lphModule[i], filePath, 1024 * 8);
                if (string.Compare(Path.GetFileNameWithoutExtension(filePath.ToString()), dllName, true, CultureInfo.CurrentCulture) == 0)
                {
                    return lphModule[i];
                }
            }
            return IntPtr.Zero;
        }
	}
}
