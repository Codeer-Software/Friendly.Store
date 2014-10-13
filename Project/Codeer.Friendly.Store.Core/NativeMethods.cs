#define CODE_ANALYSIS
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Codeer.Friendly.Store.Core
{
	/// <summary>
	/// WindowsApi。
	/// </summary>
	static class NativeMethods
	{
		/// <summary>
		/// 終了メッセージ。
		/// </summary>
		internal const int WM_QUIT = 0x12;

		/// <summary>
		/// データコピー。
		/// </summary>
		internal const int WM_COPYDATA = 0x004A;

        /// <summary>
        /// タイマー
        /// </summary>
        internal const int WM_TIMER = 0x0113;

		/// <summary>
		/// コピーデータ構造体。
		/// </summary>
		internal struct COPYDATASTRUCT
		{
			public IntPtr dwData;
			public UInt32 cbData;
			public IntPtr lpData;
		}

		/// <summary>
		/// メッセージ送信。
		/// </summary>
		/// <param name="hWnd">ウィンドウハンドル。</param>
        /// <param name="msg">メッセージ。</param>
        /// <param name="wParam">wParam。</param>
        /// <param name="lParam">lParam。</param>
        /// <returns>結果。</returns>
        [DllImport("user32.dll")]
		internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

		/// <summary>
        /// メッセージ送信。
		/// </summary>
        /// <param name="hWnd">ウィンドウハンドル。</param>
        /// <param name="msg">メッセージ。</param>
        /// <param name="wParam">wParam。</param>
        /// <param name="lParam">lParam。</param>
        /// <returns>結果。</returns>
        [DllImport("user32.dll")]
		internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// メッセージ送信。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル。</param>
        /// <param name="msg">メッセージ。</param>
        /// <param name="wParam">wParam。</param>
        /// <param name="lParam">lParam。</param>
        /// <returns>結果。</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		/// <summary>
        /// 指定のウィンドウハンドルの所属するスレッドとプロセスの取得。
		/// </summary>
        /// <param name="hWnd">ウィンドウハンドル。</param>
        /// <param name="lpdwProcessId">プロセスID。</param>
        /// <returns>スレッドID。</returns>
		[DllImport("user32.dll")]
		internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

		/// <summary>
        /// メッセージ構造体。
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		internal struct MSG
		{
            public IntPtr HWnd;
			public Int32 Msg;
            public IntPtr WParam;
            public IntPtr LParam;
			public Int32 Time;
			public POINTAPI Pt;
		}

		/// <summary>
        /// 位置情報。
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		internal struct POINTAPI
		{
			public Int32 x;
			public Int32 y;
		}

		/// <summary>
        /// メッセージ取得。
		/// </summary>
        /// <param name="msg">メッセージ。</param>
        /// <param name="hWnd">ウィンドウハンドル。</param>
        /// <param name="wMsgFilterMin">フィルタ最小値。</param>
        /// <param name="wMsgFilterMax">フィルタ最大値。</param>
        /// <returns>結果。</returns>
		[DllImport("user32.dll")]
		internal extern static int GetMessage(ref MSG msg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
		
		/// <summary>
        /// メッセージ変換。
		/// </summary>
        /// <param name="msg">メッセージ。</param>
        /// <returns>結果。</returns>
		[DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool TranslateMessage(ref MSG msg);
		
		/// <summary>
        /// メッセージディスパッチ。
		/// </summary>
        /// <param name="msg">メッセージ。</param>
        /// <returns>結果。</returns>
		[DllImport("user32.dll")]
		internal static extern IntPtr DispatchMessage(ref MSG msg);

		/// <summary>
        /// 割り当てのタイプ。
		/// </summary>
		[Flags]
		internal enum AllocationType
		{
			Commit = 0x1000,
			Reserve = 0x2000,
			Decommit = 0x4000,
			Release = 0x8000,
			Reset = 0x80000,
			Physical = 0x400000,
			TopDown = 0x100000,
			WriteWatch = 0x200000,
			LargePages = 0x20000000
		}

		/// <summary>
        /// アクセス保護のタイプ。
		/// </summary>
		[Flags]
		internal enum MemoryProtection
		{
			Execute = 0x10,
			ExecuteRead = 0x20,
			ExecuteReadWrite = 0x40,
			ExecuteWriteCopy = 0x80,
			NoAccess = 0x01,
			ReadOnly = 0x02,
			ReadWrite = 0x04,
			WriteCopy = 0x08,
			GuardModifierflag = 0x100,
			NoCacheModifierflag = 0x200,
			WriteCombineModifierflag = 0x400
		}

		/// <summary>
		/// 解放操作のタイプ
		/// </summary>
		[Flags]
		internal enum FreeType
		{
			Decommit = 0x4000,
			Release = 0x8000,
		}

        /// <summary>
        /// 指定のウィンドウハンドルが存在するか。
        /// </summary>
        /// <param name="hWnd">ハンドル。</param>
        /// <returns>存在するか。</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindow(IntPtr hWnd);

		/// <summary>
        /// アクセスフラグ。
		/// </summary>
		[Flags]
		internal enum ProcessAccessFlags : uint
		{
			All = 0x001F0FFF,
			Terminate = 0x00000001,
			CreateThread = 0x00000002,
			VMOperation = 0x00000008,
			VMRead = 0x00000010,
			VMWrite = 0x00000020,
			DupHandle = 0x00000040,
			SetInformation = 0x00000200,
			QueryInformation = 0x00000400,
			Synchronize = 0x00100000
		}

        [DllImport("kernel32.dll")]
        internal static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hObject);

		/// <summary>
        /// DLLロード。
		/// </summary>
        /// <param name="lpFileName">ファイル名称。</param>
        /// <returns>DLLハンドル。</returns>
		[DllImport("kernel32", CharSet = CharSet.Auto)]
		internal static extern IntPtr LoadLibrary(string lpFileName);

		/// <summary>
		/// ダイナミックリンクライブラリ（DLL）が持つ、指定されたエクスポート済み関数のアドレスを取得します。
		/// </summary>
        /// <param name="hModule">DLL モジュールのハンドル。</param>
        /// <param name="procName">関数名。</param>
		/// <returns></returns>
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true)]
		internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		/// <summary>
        /// モジュールハンドルの取得。
		/// </summary>
        /// <param name="lpModuleName">モジュール名称。</param>
		/// <returns>ハンドル</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr GetModuleHandle(string lpModuleName);

		/// <summary>
        /// メッセージ処理関数用デリゲート。
		/// </summary>
        /// <param name="hwnd">ウィンドウハンドル。</param>
        /// <param name="msg">メッセージ。</param>
        /// <param name="wParam">wParam。</param>
        /// <param name="lParam">lParam。</param>
        /// <returns>実行結果。</returns>
        internal delegate IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

		/// <summary>
        /// ウィンドウプロシージャへのポインタ、またはウィンドウプロシージャへのポインタを表すハンドルへのアクセス。
		/// </summary>
        internal static int GWL_WNDPROC = -4;

        /// <summary>
        /// 指定されたウィンドウに関する情報を取得。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル。</param>
        /// <param name="nIndex">操作種別。</param>
        /// <returns>結果。</returns>
        internal static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
            {
                return GetWindowLongPtr64(hWnd, nIndex);
            }
            else
            {
                return new IntPtr(GetWindowLongPtr32(hWnd, nIndex));
            }
        }

        /// <summary>
        /// 指定されたウィンドウに関する情報を取得。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル。</param>
        /// <param name="nIndex">操作種別。</param>
        /// <returns>結果</returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        /// <summary>
        /// 指定されたウィンドウに関する情報を取得。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル。</param>
        /// <param name="nIndex">操作種別。</param>
        /// <returns>結果</returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern int GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        /// <summary>
        /// 指定されたウィンドウに関する情報を設定。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル。</param>
        /// <param name="nIndex">操作種別。</param>
        /// <param name="dwNewLong">情報。</param>
        /// <returns>設定前の情報。</returns>
        internal static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            }
            else
            {
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
            }
        }

        /// <summary>
        /// 指定されたウィンドウに関する情報を設定。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル。</param>
        /// <param name="nIndex">操作種別。</param>
        /// <param name="dwNewLong">情報。</param>
        /// <returns>設定前の情報。</returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// 指定されたウィンドウに関する情報を設定。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル。</param>
        /// <param name="nIndex">操作種別。</param>
        /// <param name="dwNewLong">情報。</param>
        /// <returns>設定前の情報。</returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        /// <summary>
        /// 指定されたウィンドウプロシージャに、メッセージ情報を渡す。
        /// </summary>
        /// <param name="lpPrevWndFunc">ウィンドウプロシージャ。</param>
        /// <param name="hWnd">ウィンドウハンドル。</param>
        /// <param name="msg">メッセージ。</param>
        /// <param name="wParam">wParam。</param>
        /// <param name="lParam">lParam。</param>
        /// <returns>結果。</returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// ウィンドウ登録構造体
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct WNDCLASSEX
        {
            public uint cbSize;
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        /// <summary>
        /// デフォルトウィンドウハンドラ
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="msg">メッセージ</param>
        /// <param name="wParam">WParam</param>
        /// <param name="lParam">LParam</param>
        /// <returns>結果</returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// ウィンドウクラス登録
        /// </summary>
        /// <param name="lpwcx">ウィンドウクラス情報</param>
        /// <returns>ウィンドウ識別子</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern ushort RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        /// <summary>
        /// メッセージ用ウィンドウにする場合の識別子
        /// </summary>
        internal static IntPtr HWND_MESSAGE = new IntPtr(-3);

        /// <summary>
        /// ウィンドウ生成
        /// </summary>
        /// <param name="dwExStyle">拡張スタイル</param>
        /// <param name="lpClassName">ウィンドウクラス名称</param>
        /// <param name="lpWindowName">ウィンドウタイトル</param>
        /// <param name="dwStyle">スタイル</param>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        /// <param name="nWidth">幅</param>
        /// <param name="nHeight">高さ</param>
        /// <param name="hWndParent">親</param>
        /// <param name="hMenu">メニュー</param>
        /// <param name="hInstance">インスタンス</param>
        /// <param name="lpParam">パラメータ</param>
        /// <returns>ウィンドウハンドル</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle,
            int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        /// <summary>
        /// ウィンドウ破棄
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <returns>成否</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DestroyWindow(IntPtr hWnd);

        /// <summary>
        /// タイマー設定
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="nIDEvent">ID</param>
        /// <param name="uElapse">感覚</param>
        /// <param name="lpTimerFunc">タイマー関数</param>
        /// <returns>ID</returns>
        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern IntPtr SetTimer(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, IntPtr lpTimerFunc);

        /// <summary>
        /// タイマー削除
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="uIDEvent">イベントID</param>
        /// <returns>成否</returns>
        [DllImport("user32.dll", ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool KillTimer(IntPtr hWnd, IntPtr uIDEvent);
    }
}

