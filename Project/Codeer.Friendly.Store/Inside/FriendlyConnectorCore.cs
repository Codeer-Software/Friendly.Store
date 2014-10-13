using System;
using System.Threading;
using System.Collections.Generic;
using Codeer.Friendly.Inside.Protocol;
using Codeer.Friendly.Store.Core.CopyDataProtocol;
using Codeer.Friendly.Store.Properties;
using System.Reflection;
using Codeer.Friendly.Store.Core;

namespace Codeer.Friendly.Store.Inside
{
    /// <summary>
    /// Friendly処理の接続。
    /// </summary>
    class FriendlyConnectorCore
    {
        readonly IntPtr _friendlyConnectorWindowInAppHandle;
        readonly IntPtr _friendlyConnectorWindowInAppHandleAsync;
        readonly ReceiveAfterSend _receiveWindow;
        internal IntPtr FriendlyConnectorWindowInAppHandle { get { return _friendlyConnectorWindowInAppHandle; } }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="friendlyConnectorWindowInAppHandleAsync">非同期通信用App側Friendly通信接続ウィンドウ。</param>
        /// <param name="friendlyConnectorWindowInAppHandle">App側Friendly通信接続ウィンドウ。</param>
        /// <param name="receiveWindow">受信ウィンドウ。</param>
        internal FriendlyConnectorCore(IntPtr friendlyConnectorWindowInAppHandleAsync, IntPtr friendlyConnectorWindowInAppHandle, ReceiveAfterSend receiveWindow)
        {
            _friendlyConnectorWindowInAppHandleAsync = friendlyConnectorWindowInAppHandleAsync;
            _friendlyConnectorWindowInAppHandle = friendlyConnectorWindowInAppHandle;
            _receiveWindow = receiveWindow;
        }

        /// <summary>
        /// 実行。
        /// </summary>
        /// <param name="info">呼び出し情報。</param>
        /// <returns>戻り値。</returns>
        public ReturnInfo SendAndReceive(ProtocolInfo info)
        {
            switch (info.ProtocolType)
            {
                case ProtocolType.IsEmptyVar:
                case ProtocolType.AsyncResultVarInitialize:
                    return AsyncState(info);
                case ProtocolType.AsyncOperation:
                    return AsyncOperation(info);
                case ProtocolType.Operation:
                    return Operation(info);
                case ProtocolType.BinOff:
                    return BinOff(info);
                default:
                    return SendForExecuteContext(info);
            }
        }

        /// <summary>
        /// 非同期状態に関する通。
        /// 非同期結果バッファの初期化と、完了の問い合わせ。
        /// 対象アプリケーション内でコントロールスレッドで実行される。
        /// </summary>
        /// <param name="info">呼び出し情報。</param>
        /// <returns>戻り値。</returns>
        private ReturnInfo AsyncState(ProtocolInfo info)
        {
            object obj = CopyDataProtocolTalker.SendAndRecieve(_friendlyConnectorWindowInAppHandleAsync, info, _receiveWindow);
            ReturnInfo ret = SameNameDataConverter.Cast(obj) as ReturnInfo;
            if (ret == null)
            {
                throw new FriendlyOperationException(Resources.ErrorAppCommunication);
            }
            return ret;
        }

        /// <summary>
        /// 非同期操作実行通信。
        /// 非同期実行のトリガをコントロールスレッドでかけて、実行は対象スレッドに任せる。
        /// </summary>
        /// <param name="info">呼び出し情報。</param>
        /// <returns>戻り値。</returns>
        private ReturnInfo AsyncOperation(ProtocolInfo info)
        {
            ContextOrderProtocolInfo contextOrder = new ContextOrderProtocolInfo(info, _friendlyConnectorWindowInAppHandle);
            object obj = CopyDataProtocolTalker.SendAndRecieve(_friendlyConnectorWindowInAppHandleAsync, contextOrder, _receiveWindow);
            ReturnInfo ret = SameNameDataConverter.Cast(obj) as ReturnInfo;
            if (ret == null)
            {
                throw new FriendlyOperationException(Resources.ErrorAppCommunication);
            }
            return ret;
        }

        /// <summary>
        /// 同期実行。
        /// しかし、Windowsの場合、操作実行は非同期で実行しないと、稀にに操作中にSendMessageが失敗してしまう操作がある。
        /// そのため、非同期操作のプロトコルを使って、実行させ、終了するのを待つ。
        /// </summary>
        /// <param name="info">呼び出し情報。</param>
        /// <returns>戻り値。</returns>
        private ReturnInfo Operation(ProtocolInfo info)
        {
            //完了の成否確認用
            ReturnInfo isComplete = SendForExecuteContext(new ProtocolInfo(ProtocolType.VarInitialize, null, null, string.Empty, string.Empty, new object[] { null }));
            if (isComplete.Exception != null)
            {
                return isComplete;
            }

            //引数の先頭に存在確認フラグを挿入
            List<object> arg = new List<object>();
            arg.Add(isComplete.ReturnValue);
            arg.AddRange(info.Arguments);

            //非同期実行
            ReturnInfo retValue = SendForExecuteContext(new ProtocolInfo(ProtocolType.AsyncOperation, info.OperationTypeInfo, info.VarAddress, info.TypeFullName, info.Operation, arg.ToArray()));
            if (retValue.Exception != null)
            {
                return retValue;
            }

            //処理が完了するのを待つ
            VarAddress complateCheckHandle = (VarAddress)isComplete.ReturnValue;
            int sleepTime = 1;
            while (true)
            {
                //結果の確認は実行対象スレッド以外で実施する。
                //処理が完了するまで、そのスレッドには割り込まない。
                object obj = CopyDataProtocolTalker.SendAndRecieve(_friendlyConnectorWindowInAppHandleAsync,
                    new ProtocolInfo(ProtocolType.IsEmptyVar, null, null, string.Empty, string.Empty, new object[] { complateCheckHandle })
                    , _receiveWindow);
                ReturnInfo ret = SameNameDataConverter.Cast(obj) as ReturnInfo;
                if (ret == null)
                {
                    throw new FriendlyOperationException(Resources.ErrorAppCommunication);
                }
                if (!(bool)ret.ReturnValue)
                {
                    break;
                }
                Thread.Sleep(sleepTime);
                sleepTime++;
                if (100 < sleepTime)
                {
                    sleepTime = 100;
                }
            }

            //結果を取得
            ReturnInfo checkComplate = SendForExecuteContext(new ProtocolInfo(ProtocolType.GetValue, null, complateCheckHandle, string.Empty, string.Empty, new object[0]));
            if (checkComplate.Exception != null)
            {
                return checkComplate;
            }
            ReturnInfo checkComplateCore = checkComplate.ReturnValue as ReturnInfo;
            if (checkComplateCore == null)
            {
                throw new FriendlyOperationException(Resources.ErrorAppCommunication);
            }
            if (checkComplateCore.Exception != null)
            {
                return checkComplateCore;
            }

            //解放
            NativeMethods.SendMessage(_friendlyConnectorWindowInAppHandle, /*FriendlyConnectorWindowInApp.WM_BINOFF*/0x8000, new IntPtr(complateCheckHandle.Core), IntPtr.Zero);

            //戻り値を返す
            return retValue;
        }

        /// <summary>
        /// BinOffはGCのスレッドからコールされるので、SendMessageのみで通信する（受信しない）
        /// </summary>
        /// <param name="info">呼び出し情報。</param>
        /// <returns>戻り値。</returns>
        private ReturnInfo BinOff(ProtocolInfo info)
        {
            NativeMethods.SendMessage(_friendlyConnectorWindowInAppHandle, /*FriendlyConnectorWindowInApp.WM_BINOFF*/0x8000, new IntPtr(info.VarAddress.Core), IntPtr.Zero);
            return new ReturnInfo();
        }

        /// <summary>
        /// 実行スレッドに送信。
        /// </summary>
        /// <param name="info">呼び出し情報。</param>
        /// <returns>戻り値。</returns>
        ReturnInfo SendForExecuteContext(ProtocolInfo info)
        {
            object obj = CopyDataProtocolTalker.SendAndRecieve(_friendlyConnectorWindowInAppHandle, info, _receiveWindow);
            ReturnInfo ret = SameNameDataConverter.Cast(obj) as ReturnInfo;
            if (ret == null)
            {
                throw new FriendlyOperationException(Resources.ErrorAppCommunication);
            }
            return ret;
        }
    }
}
