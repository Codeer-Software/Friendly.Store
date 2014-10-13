#define CODE_ANALYSIS
using System;
using System.Collections.Generic;
using Codeer.Friendly.Inside.Protocol;
using Codeer.Friendly.DotNetExecutor;
using Codeer.Friendly.Store.Core.CopyDataProtocol;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using Codeer.Friendly.Store.Core;
using Codeer.Friendly.Core.Inside;

namespace Codeer.Friendly.Store.Core
{
	/// <summary>
	/// アプリケーション側システムコントロールウィンドウ。
	/// </summary>
	class SystemControlWindowInApp : ReceiveForm
	{
        FriendlyConnectorWindowInAppManager _handleAndWindow = new FriendlyConnectorWindowInAppManager();
        DotNetFriendlyControl _dotNetFriendlyControl = new DotNetFriendlyControl();
        int _operationProcess;
        bool _hasThread;

        internal SystemControlWindowInApp(bool hasThread, int operationProcess)
        {
            _hasThread = hasThread;
            _operationProcess = operationProcess;
        }

        //@@@プロセス系のAPIが使えない？
        //相手プロセスの生存確認が未
        /*
        protected override void WndProc(ref Message message)
        {
            switch (message.Msg)
            {
                case NativeMethods.WM_TIMER:
                    if (!_hasThread)
                    {
                        var handle = NativeMethods.OpenProcess
                            (NativeMethods.ProcessAccessFlags.Synchronize, false, _operationProcess);
                        if (handle == IntPtr.Zero)
                        {
                            Dispose();
                            return;
                        }
                        NativeMethods.CloseHandle(handle);
                    }
                    break;
                default:
                    break;
                    
            }
            base.WndProc(ref message);
        }
        */

        /// <summary>
        /// データ受信時の処理。
        /// </summary>
        /// <param name="communicationNo">通信番号。</param>
        /// <param name="recieveData">受信データ。</param>
        /// <param name="senderWindow">送信元ウィンドウ。</param>
        protected override void OnRecieveData(int communicationNo, object recieveData, IntPtr senderWindow)
        {
            SystemControlInfo controlInfo = (SystemControlInfo)recieveData;
			object ret = null;
			switch (controlInfo.SystemControlType)
			{
                case SystemControlType.StartFriendlyConnectorWindowInApp:
                    ret = StartFriendlyConnectorWindowInApp(controlInfo);
                    break;
				case SystemControlType.EndFriendlyConnectorWindowInApp:
                    EndFriendlyConnectorWindowInApp(controlInfo);
					break;
				case SystemControlType.EndSystem:
                    EndSystem();
					break;
			}
            SendReturnData(communicationNo, senderWindow, ret);
		}

        /// <summary>
        /// フレンドリー操作ウィンドウ開始。
        /// </summary>
        /// <param name="controlInfo">コントロール情報。</param>
        /// <returns>FriendlyConnectorWindowInAppのハンドル。</returns>
        IntPtr StartFriendlyConnectorWindowInApp(SystemControlInfo controlInfo)
        {
            IntPtr targetThreadWindowHandle = (IntPtr)controlInfo.Data;
            FriendlyConnectorWindowInApp window = null;
            IntPtr executeWindowHandle = IntPtr.Zero;

            //現在のウィンドウプロックを取得
            IntPtr currentProc = NativeMethods.GetWindowLongPtr(targetThreadWindowHandle, NativeMethods.GWL_WNDPROC);
            //InvokeWindowを起動するためのプロックを設定
            NativeMethods.WndProc proc = delegate(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
            {
                switch (msg)
                {
                    case 0:
                        if (executeWindowHandle == IntPtr.Zero)
                        {
                            window = new FriendlyConnectorWindowInApp(_handleAndWindow, _dotNetFriendlyControl);
                            executeWindowHandle = window.Handle;
                        }
                        break;
                    default:
                        break;
                }
                return NativeMethods.CallWindowProc(currentProc, hwnd, msg, wParam, lParam);
            };
            NativeMethods.SetWindowLongPtr(targetThreadWindowHandle, NativeMethods.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(proc));

            //起動するまで待つ
            while (executeWindowHandle == IntPtr.Zero)
            {
                //指定のウィンドウが消えていたら終了
                if (!NativeMethods.IsWindow(targetThreadWindowHandle))
                {
                    return IntPtr.Zero;
                }
                NativeMethods.SendMessage(targetThreadWindowHandle, 0, IntPtr.Zero, IntPtr.Zero);
                Thread.Sleep(10);
            }
            GC.KeepAlive(proc);

            //元に戻す
            NativeMethods.SetWindowLongPtr(targetThreadWindowHandle, NativeMethods.GWL_WNDPROC, currentProc);

            //登録
            _handleAndWindow.Add(executeWindowHandle, window);
            return executeWindowHandle;
        }

        /// <summary>
        /// フレンドリー操作ウィンドウ終了。
        /// </summary>
        /// <param name="controlInfo">コントロール情報。</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void EndFriendlyConnectorWindowInApp(SystemControlInfo controlInfo)
        {
            IntPtr handle = (IntPtr)controlInfo.Data;
            FriendlyConnectorWindowInApp invokeWindow = _handleAndWindow.FromHandle(handle);
            _handleAndWindow.Remove(handle);

            //スレッドが異なるので、Invokeによって終了処理を実施する。
            if (invokeWindow != null)
            {
                try
                {
                    invokeWindow.Invoke((MethodInvoker)delegate
                    {
                        try
                        {
                            invokeWindow.RequestDispose();
                        }
                        catch { }
                    });
                }
                catch { }
            }
        }

        /// <summary>
        /// システム終了
        /// </summary>
        void EndSystem()
        {
            if (_hasThread)
            {
                //自身のスレッドを終了させる
                NativeMethods.PostMessage(Handle, NativeMethods.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
            }
            else
            {
                Dispose();
            }
        }

        /// <summary>
        /// 破棄
        /// </summary>
        /// <param name="disposing">Disposeメソッドから呼び出されたか</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_handleAndWindow != null)
                {
                    //ウィンドウ全終了
                    foreach (KeyValuePair<IntPtr, FriendlyConnectorWindowInApp> element in _handleAndWindow.Clone())
                    {
                        //何らかの都合で終了に失敗するものが出ても消せるだけ消しておく
                        //スレッドが異なるので、Invokeによって終了処理を実施する
                        try
                        {
                            element.Value.Invoke((MethodInvoker)delegate
                            {
                                try
                                {
                                    element.Value.RequestDispose();
                                }
                                catch { }
                            });
                        }
                        catch { }
                    }
                    _handleAndWindow = null;
                }
                _dotNetFriendlyControl = null;
            }
            catch { }
            base.Dispose(disposing);
        }
    }
}
