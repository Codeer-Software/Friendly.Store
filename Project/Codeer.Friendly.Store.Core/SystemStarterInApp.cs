using System;
using System.Threading;
using System.Diagnostics;
using Codeer.Friendly.Store.Core.Properties;
using Codeer.Friendly;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Reflection;
using Codeer.Friendly.Store.Core;

namespace Codeer.Friendly.Store.Core
{
	/// <summary>
	/// Windowsアプリケーション操作開始クラス。
	/// </summary>
	public static class SystemStarterInApp
	{
        /// <summary>
        /// ウィンドウハンドル通知メッセージ。
        /// </summary>
        public const int WM_NOTIFY_SYSTEM_CONTROL_WINDOW_HANDLE = 0x8100;

		/// <summary>
		/// 開始。
		/// </summary>
		/// <param name="startInfo">開始情報。</param>
        public static void Start(string startInfo)
        {
            startInfo = Debug.ReadDebugMark(startInfo);
            int index = startInfo.IndexOf("+");
            IntPtr mainHandle = IntPtr.Zero;
            if (index != -1)
            {
                string tmp = startInfo.Substring(index + 1);
                mainHandle = new IntPtr(long.Parse(tmp, CultureInfo.CurrentCulture));
                startInfo = startInfo.Substring(0, index);
            }
            Debug.Trace("I read second letter.");
            if (mainHandle == IntPtr.Zero)
            {
                return;
            }

            //対象プロセスのIDを取得
            int windowProcessId = 0;
            IntPtr terminalWindow = new IntPtr(long.Parse(startInfo, CultureInfo.CurrentCulture));
            NativeMethods.GetWindowThreadProcessId(terminalWindow, out windowProcessId);
            IntPtr controlWindowHandle = StartFriendlyConnectorWindowInApp(mainHandle, windowProcessId);
            NativeMethods.SendMessage(terminalWindow, WM_NOTIFY_SYSTEM_CONTROL_WINDOW_HANDLE, controlWindowHandle, IntPtr.Zero);
        }

        static IntPtr StartFriendlyConnectorWindowInApp(IntPtr targetThreadWindowHandle, int processId)
        {
            IntPtr executeWindowHandle = IntPtr.Zero;

            //現在のウィンドウプロックを取得
            IntPtr currentProc = NativeMethods.GetWindowLongPtr(targetThreadWindowHandle, NativeMethods.GWL_WNDPROC);
            //InvokeWindowを起動するためのプロックを設定
            NativeMethods.WndProc proc = delegate(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
            {
                SystemControlWindowInApp window = new SystemControlWindowInApp(false, processId);
                window.ToString();
                executeWindowHandle = window.Handle;
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
            return executeWindowHandle;
        }
	}
}
