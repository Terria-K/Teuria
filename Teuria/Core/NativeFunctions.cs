using System;
using System.Runtime.InteropServices;

namespace Teuria.Unsafe; 

internal static class NativeFunctions 
{
    internal const string RFD_SHARP = "rfd_sharp.dll";


    [DllImport(RFD_SHARP, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static unsafe extern PathHandler open_file(string directory);

    [DllImport(RFD_SHARP, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static unsafe extern PathHandler open_file_with_filter(string directory, string[] filters, UIntPtr length);
    [DllImport(RFD_SHARP, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static unsafe extern PathHandler save_file(string directory);

    [DllImport(RFD_SHARP, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static unsafe extern PathHandler save_file_with_filter(string directory, string[] filters, UIntPtr length);

}

[StructLayout(LayoutKind.Sequential)]
internal struct PathHandler 
{
    public byte error;
    public IntPtr path;
}