using System;
using System.Reflection;

namespace Codeer.Friendly.DotNetExecutor
{
    /// <summary>
    /// RTのIntrospectionをラップ
    /// </summary>
    class IntrospectionWrap
    {
        MethodInfo _getTypeInfo;

        internal IntrospectionWrap(TypeFinder typeFinder)
        {

            Type rtRef = typeFinder.GetType("System.Reflection.IntrospectionExtensions");
            _getTypeInfo = rtRef.GetMethod("GetTypeInfo", BindingFlags.Static | BindingFlags.Public);

        }

        internal TypeInfoWrap GetTypeInfo(Type type)
        {
            try
            {
                return new TypeInfoWrap(_getTypeInfo.Invoke(null, new object[] { type }));
            }
            catch { }
            return null;
        }
    }
}
