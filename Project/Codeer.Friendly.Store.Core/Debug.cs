#define CODE_ANALYSIS
using System.Diagnostics.CodeAnalysis;

namespace Codeer.Friendly.Store.Core
{
    /// <summary>
    /// デバッグ用クラス
    /// </summary>
    public static class Debug
    {
        static bool _isDebug;

        /// <summary>
        /// デバッグモードマーク
        /// </summary>
        public static string DebugMark { get { return _isDebug ? ("???") : (string.Empty); } }

        /// <summary>
        /// トレース
        /// </summary>
        /// <param name="msg">メッセージ</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static void Trace(string msg)
        {
            if (!_isDebug)
            {
                return;
            }
        }

        /// <summary>
        /// バッグモードか否かを読み取る
        /// </summary>
        /// <param name="info">情報文字列</param>
        /// <returns>情報コア</returns>
        public static string ReadDebugMark(string info)
        {
            string infoCore = info.Replace("???", string.Empty);
            _isDebug = (info != infoCore);
            return infoCore;
        }
    }
}
