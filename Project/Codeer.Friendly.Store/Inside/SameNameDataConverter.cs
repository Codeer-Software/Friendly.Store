using System;
using System.Reflection;
using Codeer.Friendly.Inside.Protocol;
using Codeer.Friendly.Store.Core.CopyDataProtocol;
using Codeer.Friendly.Store.Core;

namespace Codeer.Friendly.Store.Inside
{
    /// <summary>
    /// Codeer.Friendly以下のデータは全くの同名でStore.Coreにも定義されている
    /// その変換
    /// </summary>
    static class SameNameDataConverter
    {
        /// <summary>
        /// 変換
        /// </summary>
        /// <param name="src">元</param>
        /// <returns>変換結果</returns>
        public static object Cast(object src)
        {
            if (src == null)
            {
                return null;
            }
            object obj = CreateCodeerInstance(src.GetType().FullName);
            if (obj == null)
            {
                return src;
            }
            foreach (var p in src.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var set = obj.GetType().GetField(p.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                set.SetValue(obj, Cast(p.GetValue(src)));
            }
            return obj;
        }

        /// <summary>
        /// Codeer.Friendly以下のデータ生成
        /// </summary>
        /// <param name="name">名前</param>
        /// <returns>オブジェクト</returns>
        private static object CreateCodeerInstance(string name)
        {
            if (name == typeof(ProtocolInfo).FullName)
            {
                return new ProtocolInfo(ProtocolType.AsyncOperation, new OperationTypeInfo("a"), new VarAddress(0), "a", "a", new object[0]);
            }
            else if (name == typeof(ReturnInfo).FullName)
            {
                return new ReturnInfo();
            }

            else if (name == typeof(ExceptionInfo).FullName)
            {
                return new ExceptionInfo(new Exception());
            }
            else if (name == typeof(VarAddress).FullName)
            {
                return new VarAddress(0);
            }
            else if (name == typeof(OperationTypeInfo).FullName)
            {
                return new OperationTypeInfo("a");
            }
            else if (name == typeof(ProtocolType).FullName)
            {
                return ProtocolType.AsyncOperation;
            }
            return null;
        }
    }
}
