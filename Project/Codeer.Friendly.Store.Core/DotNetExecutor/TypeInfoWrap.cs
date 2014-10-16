using System;
using System.Collections.Generic;
using System.Reflection;

namespace Codeer.Friendly.DotNetExecutor
{
    /// <summary>
    /// RTのタイプ情報をラップ
    /// </summary>
    class TypeInfoWrap
    {
        object _core;

        internal TypeInfoWrap(object core)
        {
            _core = core;
        }

        internal IEnumerable<Type> ImplementedInterfaces
        {
            get
            {
                return (IEnumerable<Type>)_core.GetType().GetProperty("ImplementedInterfaces").GetGetMethod().Invoke(_core, new object[0]);
            }
        }

        internal MethodInfo[] GetDeclaredMethods(string operation)
        {
            return new List<MethodInfo>(
                (IEnumerable<MethodInfo>)_core.GetType().GetMethod("GetDeclaredMethods").Invoke(_core, new object[] { operation })).ToArray();
        }
    }
}
