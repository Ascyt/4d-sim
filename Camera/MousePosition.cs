using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Utility class to get and set the mouse cursor position on Windows.
/// </summary>
public class MousePosition : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

#if UNITY_STANDALONE_WIN
    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out Point lpPoint);

    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);
#endif

    public static Point GetCursorPosition()
    {
#if UNITY_STANDALONE_WIN
        if (GetCursorPos(out Point lpPoint))
            return lpPoint;
        else
            return new Point(0, 0);
#else
            return new Point(0, 0);
#endif
    }

    public static bool SetCursorPosition(Point point)
    {
#if UNITY_STANDALONE_WIN
        return SetCursorPos(point.X, point.Y);
#else
            return false;
#endif
    }
}
