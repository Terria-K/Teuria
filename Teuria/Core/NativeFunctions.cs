using System;
using System.Runtime.InteropServices;

namespace Teuria.Unsafe; 

internal static class NativeFunctions 
{
    internal const string RFD_SHARP = "rfd_sharp.dll";
    internal const string KERNEL32 = "kernel32.dll";
    internal const string SDL2 = "SDL2";


    [DllImport(RFD_SHARP, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static unsafe extern PathHandler open_file(string directory);

    [DllImport(RFD_SHARP, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static unsafe extern PathHandler open_file_with_filter(string directory, string[] filters, nuint length);
    [DllImport(RFD_SHARP, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static unsafe extern PathHandler save_file(string directory);

    [DllImport(RFD_SHARP, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static unsafe extern PathHandler save_file_with_filter(string directory, string[] filters, nuint length);
    [DllImport(KERNEL32)]
    internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
    [DllImport(KERNEL32)]
    internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
    [DllImport(KERNEL32, SetLastError = true)]
    internal static extern IntPtr GetStdHandle(int nStdHandle);
    [DllImport(SDL2, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint SDL_GetWindowFlags(IntPtr window);


}

[Flags]
public enum SDL_WindowFlags : uint
{
    SDL_WINDOW_FULLSCREEN =		0x00000001,
    SDL_WINDOW_OPENGL =		0x00000002,
    SDL_WINDOW_SHOWN =		0x00000004,
    SDL_WINDOW_HIDDEN =		0x00000008,
    SDL_WINDOW_BORDERLESS =		0x00000010,
    SDL_WINDOW_RESIZABLE =		0x00000020,
    SDL_WINDOW_MINIMIZED =		0x00000040,
    SDL_WINDOW_MAXIMIZED =		0x00000080,
    SDL_WINDOW_MOUSE_GRABBED =	0x00000100,
    SDL_WINDOW_INPUT_FOCUS =	0x00000200,
    SDL_WINDOW_MOUSE_FOCUS =	0x00000400,
    SDL_WINDOW_FULLSCREEN_DESKTOP = (SDL_WINDOW_FULLSCREEN | 0x00001000),
    SDL_WINDOW_FOREIGN =		0x00000800,
    SDL_WINDOW_ALLOW_HIGHDPI =	0x00002000,	/* Requires >= 2.0.1 */
    SDL_WINDOW_MOUSE_CAPTURE =	0x00004000,	/* Requires >= 2.0.4 */
    SDL_WINDOW_ALWAYS_ON_TOP =	0x00008000,	/* Requires >= 2.0.5 */
    SDL_WINDOW_SKIP_TASKBAR =	0x00010000,	/* Requires >= 2.0.5 */
    SDL_WINDOW_UTILITY =		0x00020000,	/* Requires >= 2.0.5 */
    SDL_WINDOW_TOOLTIP =		0x00040000,	/* Requires >= 2.0.5 */
    SDL_WINDOW_POPUP_MENU =		0x00080000,	/* Requires >= 2.0.5 */
    SDL_WINDOW_KEYBOARD_GRABBED =	0x00100000,	/* Requires >= 2.0.16 */
    SDL_WINDOW_VULKAN =		0x10000000,	/* Requires >= 2.0.6 */
    SDL_WINDOW_METAL =		0x2000000,	/* Requires >= 2.0.14 */
    SDL_WINDOW_INPUT_GRABBED = SDL_WINDOW_MOUSE_GRABBED,
}

[StructLayout(LayoutKind.Sequential)]
internal struct PathHandler 
{
    public byte error;
    public nint path;
}
