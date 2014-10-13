using System;
using System.Runtime.InteropServices;

namespace Codeer.Friendly.Core.Inside
{
    public struct Message
	{
		public IntPtr HWnd { get; set; }
		public IntPtr LParam { get; set; }
		public int Msg { get; set; }
		public IntPtr Result { get; set; }
		public IntPtr WParam { get; set; }
		internal object GetLParam(Type type)
		{
			return Marshal.PtrToStructure(LParam, type);
		}
	}
}
