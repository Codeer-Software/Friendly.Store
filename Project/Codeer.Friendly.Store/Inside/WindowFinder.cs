using System;
using System.Diagnostics;
using System.Text;

namespace Codeer.Friendly.Store.Inside
{
    static class WindowFinder
    {
        internal static IntPtr Get(Process pr, string name)
        {
            IntPtr handle = IntPtr.Zero;
            while (true)
            {
                handle = NativeMethods.FindWindowEx(IntPtr.Zero, handle, IntPtr.Zero, IntPtr.Zero);
                if (handle == IntPtr.Zero)
                {
                    break;
                }
                int id;
                int thread = NativeMethods.GetWindowThreadProcessId(handle, out id);
                if (id == pr.Id)
                {
                    StringBuilder bb = new StringBuilder(1024);
                    NativeMethods.GetClassName(handle, bb, 1022);
                    if (bb.ToString().IndexOf(name) != -1)
                    {
                        return handle;
                    }
                }
            }
            return IntPtr.Zero;
        }
    }
}
