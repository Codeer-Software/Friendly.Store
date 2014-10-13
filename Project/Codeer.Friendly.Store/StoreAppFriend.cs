#define CODE_ANALYSIS
using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using Codeer.Friendly.Store.Inside;
using Codeer.Friendly.Inside.Protocol;
using Codeer.Friendly.Store.Properties;
using Codeer.Friendly.Store.Core.CopyDataProtocol;
using System.Diagnostics.CodeAnalysis;

namespace Codeer.Friendly.Store
{
	/// <summary>
	/// ストアアプリケーションを操作するためのクラスです。
    /// AppFriendを継承しています。
	/// </summary>
    public class StoreAppFriend : AppFriend, IDisposable
	{
        int _processId;
        SystemController _systemController;
        ReceiveAfterSend _receiveWindow;
        FriendlyConnectorCore _currentConnector;

		/// <summary>
		/// 接続者。
		/// </summary>
        protected override IFriendlyConnector FriendlyConnector { get { return new FriendlyConnectorWrap(this); } }

		/// <summary>
        /// 操作対象アプリケーションのプロセスIDを取得できます。
		/// </summary>
		public int ProcessId { get { return _processId; } }

        /// <summary>
        /// システムコントローラー。
        /// </summary>
        internal SystemController SystemController { get { return _systemController; } }

        /// <summary>
        /// 受信ウィンドウ
        /// </summary>
        internal ReceiveAfterSend ReceiveWindow { get { return _receiveWindow; } }

		/// <summary>
		/// コンストラクタです。
		/// 指定のプロセスに接続します。
        /// この指定の場合、接続時のメインウィンドウのスレッドで処理が実行されます。
        /// </summary>
        /// <param name="process">接続対象プロセス。</param>
		public StoreAppFriend(Process process)
		{
            //@@@一旦CLRのバージョンは4で固定
            string clrVersion = "4.0";

			//メインウィンドウ取得待ち。
            IntPtr executeContextWindowHandle = IntPtr.Zero;
			while (executeContextWindowHandle == IntPtr.Zero)
			{
				process = Process.GetProcessById(process.Id);
                if (process == null)
                {
                    break;
                }
                executeContextWindowHandle = WindowFinder.Get(process, "Windows.UI.Core.CoreWindow");
				Thread.Sleep(10);
			}
			if (process == null)
			{
				throw new FriendlyOperationException(Resources.ErrorAppConnection);
			}
            Initialize(clrVersion, executeContextWindowHandle);
        }

        private void Initialize(string clrVersion, IntPtr executeContextWindowHandle)
        {

            ProtocolMessageManager.Initialize();

            //受信用ウィンドウ生成
            _receiveWindow = new ReceiveAfterSend();

            //プロセスの取得
            NativeMethods.GetWindowThreadProcessId(executeContextWindowHandle, out _processId);

            //サーバーを開始させる。
            _systemController = SystemStarter.Start(Process.GetProcessById(_processId), executeContextWindowHandle, clrVersion);

            //メインの実行ウィンドウハンドル生成。
            _currentConnector = _systemController.StartFriendlyConnector(executeContextWindowHandle, _receiveWindow);
        }

		/// <summary>
		/// ファイナライザ。
		/// </summary>
		~StoreAppFriend()
		{
			Dispose(false);
		}

		/// <summary>
		/// 破棄します。
		/// このメソッドが呼び出されるとアプリケーションとの通信が切断され、管理していた変数が解放されます。
        /// ただし、管理領域から解放されるだけで、メモリの解放はガベージコレクションに委ねられます。
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// 破棄。
		/// </summary>
		/// <param name="disposing">破棄フラグ。</param>
		protected virtual void Dispose(bool disposing)
		{
            if (disposing && _receiveWindow != null)
            {
                _receiveWindow.Dispose();
            }
            _receiveWindow = null;
            if (_systemController != null)
			{
                _systemController.EndSystem();
                _systemController = null;
			}
            lock (this)
            {
                _currentConnector = null;
            }
            GC.Collect();
        }

        /// <summary>
        /// 送受信
        /// </summary>
        /// <param name="info">通信情報</param>
        /// <returns>戻り値</returns>
        ReturnInfo SendAndReceive(ProtocolInfo info)
        {
            lock (this)
            {
                if (_currentConnector == null)
                {
                    return new ReturnInfo();
                }
                return _currentConnector.SendAndReceive(info);
            }
        }

        /// <summary>
        /// 接続者。
        /// </summary>
        class FriendlyConnectorWrap : IFriendlyConnector
        {
            StoreAppFriend _app;

            /// <summary>
            /// コンストラクタ。
            /// </summary>
            /// <param name="app">アプリケーション操作クラス。</param>
            public FriendlyConnectorWrap(StoreAppFriend app)
            {
                _app = app;
            }

            /// <summary>
            /// 接続者を区別するためのユニークなオブジェクト。
            /// </summary>
            public object Identity { get { return _app; } }

            /// <summary>
            /// 送受信。
            /// </summary>
            /// <param name="info">通信情報。</param>
            /// <returns>戻り値。</returns>
            public ReturnInfo SendAndReceive(ProtocolInfo info)
            {
                return _app.SendAndReceive(info);
            }

            public AppFriend App
            {
                get { return _app; }
            }
        }
	}
}
