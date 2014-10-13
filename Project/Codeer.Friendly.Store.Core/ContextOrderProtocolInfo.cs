using System;
using Codeer.Friendly.Inside.Protocol;

namespace Codeer.Friendly.Store.Core
{
    /// <summary>
    /// 実行コンテキスト指定プロトコル情報
    /// </summary>
    [Serializable]
    public class ContextOrderProtocolInfo
    {
        object _protocolInfo;
        IntPtr _executeWindowHandle;

        /// <summary>
        /// プロトコル情報
        /// </summary>
        internal ProtocolInfo ProtocolInfo { get { return (ProtocolInfo)_protocolInfo; } }

        /// <summary>
        /// 実行ウィンドウ
        /// </summary>
        public IntPtr ExecuteWindowHandle { get { return _executeWindowHandle; } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="protocolInfo">プロトコル情報</param>
        /// <param name="executeWindowHandle">実行ウィンドウハンドル</param>
        public ContextOrderProtocolInfo(object protocolInfo, IntPtr executeWindowHandle)
        {
            _protocolInfo = protocolInfo;
            _executeWindowHandle = executeWindowHandle;
        }
    }
}
