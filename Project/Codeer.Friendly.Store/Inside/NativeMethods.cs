#define CODE_ANALYSIS
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Codeer.Friendly.Store.Inside
{
	/// <summary>
	/// WindowsApi。
	/// </summary>
	static class NativeMethods
    {
        /// <summary>
        /// データコピー。
        /// </summary>
        internal const int WM_COPYDATA = 0x004A;

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

        // メッセージの処理方法オプション。
		internal enum PeekMsgOption
		{
			PM_NOREMOVE = 0,
			PM_REMOVE
		}

		/// <summary>
        /// メッセージ取得。
		/// </summary>
        /// <param name="lpMsg">メッセージ。</param>
        /// <param name="hwnd">ウィンドウハンドル。</param>
        /// <param name="wMsgFilterMin">フィルタ。</param>
        /// <param name="wMsgFilterMax">フィルタ。</param>
        /// <param name="wRemoveMsg">メッセージ取得オプション。</param>
        /// <returns>成否。</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool PeekMessage(ref MSG lpMsg, IntPtr hwnd, uint wMsgFilterMin, uint wMsgFilterMax, PeekMsgOption wRemoveMsg);

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
		/// 指定されたプロセスの仮想アドレス空間内のメモリ領域の予約とコミットの一方または両方を行います。
		/// この関数は AllocationType.Rest フラグがセットされていない限り、確保されるメモリが自動的に 0 で初期化されます。
		/// </summary>
        /// <param name="hProcess">割り当てたいメモリを保持するプロセス。</param>
        /// <param name="lpAddress">割り当てたい開始アドレス。</param>
        /// <param name="dwSize">割り当てたい領域のバイト単位のサイズ。</param>
        /// <param name="flAllocationType">割り当てのタイプ。</param>
        /// <param name="flProtect">アクセス保護のタイプ。</param>
        /// <returns>確保されたアドレス。</returns>
		[DllImport("kernel32.dll")]
		internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
           IntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

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
		/// 指定されたプロセスの仮想アドレス空間内のメモリ領域を解放またはコミット解除します。
		/// </summary>
        /// <param name="hProcess">解放したいメモリを保持するプロセス。</param>
        /// <param name="lpAddress">解放したいメモリ領域の開始アドレス。</param>
        /// <param name="dwSize">解放したいメモリ領域のバイト単位のサイズ。</param>
        /// <param name="dwFreeType">解放操作のタイプ。</param>
		/// <returns>成否</returns>
		[DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
           IntPtr dwSize, FreeType dwFreeType);

		/// <summary>
		/// 指定されたプロセスのメモリ領域にデータを書き込みます。
		/// 書き込みたい領域全体がアクセス可能でなければなりません。さもないと、関数は失敗します。
		/// </summary>
        /// <param name="hProcess">プロセスのハンドル。</param>
        /// <param name="lpBaseAddress">書き込み開始アドレス。</param>
        /// <param name="lpBuffer">データバッファ。</param>
        /// <param name="nSize">書き込みたいバイト数。</param>
        /// <param name="lpNumberOfBytesWritten">実際に書き込まれたバイト数。</param>
        /// <returns>成否。</returns>
		[DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
			byte[] lpBuffer, IntPtr nSize, IntPtr lpNumberOfBytesWritten);

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
		/// 別のプロセスのアドレス空間で稼働するスレッドを作成します。
		/// </summary>
        /// <param name="hProcess">新しいスレッドを稼働させるプロセスを識別するハンドル。</param>
        /// <param name="lpThreadAttributes">スレッドのセキュリティ属性へのポインタ。</param>
        /// <param name="dwStackSize"> 初期のスタックサイズ (バイト数)。</param>
        /// <param name="lpStartAddress">スレッド関数へのポインタ。</param>
        /// <param name="lpParameter">新しいスレッドの引数へのポインタ。</param>
        /// <param name="dwCreationFlags">作成フラグ。</param>
        /// <param name="lpThreadId">取得したスレッド識別子へのポインタ。</param>
        /// <returns>スレッドハンドル。</returns>
		[DllImport("kernel32")]
        internal static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, IntPtr dwStackSize,
		  IntPtr lpStartAddress, IntPtr lpParameter, int dwCreationFlags, out IntPtr lpThreadId);

		/// <summary>
        /// モジュールハンドルの取得。
		/// </summary>
        /// <param name="lpModuleName">モジュール名称。</param>
		/// <returns>ハンドル</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr GetModuleHandle(string lpModuleName);

		/// <summary>
        /// WaitForSingleObject無限待ち時間。
		/// </summary>
		internal const UInt32 INFINITE = 0xFFFFFFFF;

		/// <summary>
        /// シグナル待ち。
		/// </summary>
        /// <param name="hHandle">ハンドル。</param>
        /// <param name="dwMilliseconds">待ち時間。</param>
        /// <returns>制御を返した原因。</returns>
		[DllImport("kernel32.dll")]
		internal static extern uint WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

		/// <summary>
        /// ハンドルクローズ。
		/// </summary>
        /// <param name="hObject">オブジェクトハンドル。</param>
        /// <returns>成否。</returns>
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// Wow64上で動作するプロセスであるか調べる
        /// </summary>
        /// <param name="hProcess">プロセスハンドル</param>
        /// <param name="isWow64">Wow64上で動作するプロセスであるか情報</param>
        /// <returns>成否</returns>
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWow64Process([In] IntPtr hProcess, [Out][MarshalAs(UnmanagedType.Bool)] out bool isWow64);

        /// <summary>
        /// プロセス内のロードされているモジュールを全取得
        /// </summary>
        /// <param name="hProcess">プロセスハンドル</param>
        /// <param name="modules">モジュール</param>
        /// <param name="arrayCount">配列数</param>
        /// <param name="trueCount">本当のモジュール数</param>
        /// <returns>成否</returns>
        [DllImport("psapi.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumProcessModules(IntPtr hProcess, [In][Out]IntPtr[] modules, uint arrayCount, out uint trueCount);

        /// <summary>
        /// モジュール名称取得
        /// </summary>
        /// <param name="hProcess">プロセスハンドル</param>
        /// <param name="hModule">モジュールハンドル</param>
        /// <param name="lpBaseName">モジュール名称</param>
        /// <param name="nSize">名称格納バッファ</param>
        /// <returns>モジュール名称バッファサイズ</returns>
        [DllImport("psapi.dll", CharSet = CharSet.Auto)]
        internal static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out]StringBuilder lpBaseName, int nSize);

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
        /// 
        /// </summary>
        /// <param name="hwndParent"></param>
        /// <param name="hwndChildAfter"></param>
        /// <param name="lpszClass"></param>
        /// <param name="lpszWindow"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, IntPtr lpszClass, IntPtr lpszWindow);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpClassName"></param>
        /// <param name="nMaxCount"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
    }
}

