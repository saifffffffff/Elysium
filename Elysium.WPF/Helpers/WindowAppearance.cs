using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Elysium.WPF.Helpers;

internal static class WindowAppearance
{
    private const int DwmwaWindowCornerPreference = 33;
    private const int DwmWindowCornerPreferenceRoundSmall = 3;

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(
        IntPtr hwnd,
        int attribute,
        ref int value,
        int valueSize);

    public static void ApplySmallCornerRadius(Window window)
    {
        var handle = new WindowInteropHelper(window).Handle;

        if (handle == IntPtr.Zero)
            return;

        var preference = DwmWindowCornerPreferenceRoundSmall;
        DwmSetWindowAttribute(
            handle,
            DwmwaWindowCornerPreference,
            ref preference,
            sizeof(int));
    }
}