using System;
using System.IO;
using System.Runtime.InteropServices;
using Teuria.Unsafe;

namespace Teuria;

public class RustyFileDialog 
{
    public static string Open(string directory = "/") 
    {
        var strPath = NativeFunctions.open_file(directory);
        if (strPath.error == 64)
            return Marshal.PtrToStringAnsi(strPath.path);

        return null;
    }

    public static string Open(string directory = "/", string[] filters = null) 
    {
        if (filters == null) 
            return Open(directory);
        
        var strPath = NativeFunctions.open_file_with_filter(directory, filters, ((uint)filters.Length));
        if (strPath.error == 64)
            return Marshal.PtrToStringAnsi(strPath.path);

        return null;
    }

    public static string Save(string directory = "/") 
    {
        var strPath = NativeFunctions.save_file(directory);
        if (strPath.error == 64)
            return Marshal.PtrToStringAnsi(strPath.path);
        return null;
    }

    public static string Save(string directory = "/", string[] filters = null) 
    {
        if (filters == null)  
            return Save(directory);
        
        var strPath = NativeFunctions.save_file_with_filter(directory, filters, ((uint)filters.Length));
        if (strPath.error == 64)
            return Marshal.PtrToStringAnsi(strPath.path);
        return null;
    }
}

