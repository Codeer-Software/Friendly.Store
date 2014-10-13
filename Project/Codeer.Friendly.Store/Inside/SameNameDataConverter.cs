using System;
using System.Reflection;
using Codeer.Friendly.Inside.Protocol;
using Codeer.Friendly.Store.Core.CopyDataProtocol;
using Codeer.Friendly.Store.Core;

namespace Codeer.Friendly.Store.Inside
{
    static class SameNameDataConverter
    {
        public static object Cast(object src)
        {
            if (src == null)
            {
                return null;
            }
            string name = src.GetType().FullName;
            if (name.IndexOf("Codeer") == 0)
            {
                return Cast(name, src);
            }
            return src;
        }

        private static object Cast(string name, object src)
        {
            object obj = CreateCodeerInstance(name);
            foreach (var p in src.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var set = obj.GetType().GetField(p.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                set.SetValue(obj, SameNameDataConverter.Cast(p.GetValue(src)));
            }
            return obj;
        }

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
            else if (name == typeof(SystemControlType).FullName)
            {
                return SystemControlType.EndFriendlyConnectorWindowInApp;
            }
            else if (name == typeof(ContextOrderProtocolInfo).FullName)
            {
                return new ContextOrderProtocolInfo(
                    new ProtocolInfo(ProtocolType.AsyncOperation, new OperationTypeInfo("a"), new VarAddress(0), "a", "a", new object[0]),
                    IntPtr.Zero);
            }
            else if (name == typeof(SystemControlInfo).FullName)
            {
                return new SystemControlInfo(SystemControlType.EndFriendlyConnectorWindowInApp, null);
            }
            else if (name == typeof(CopyDataProtocolInfo).FullName)
            {
                return new CopyDataProtocolInfo(IntPtr.Zero, null);
            }
            throw new NotSupportedException("未実装");
        }
    }
}
