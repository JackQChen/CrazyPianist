using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CrazyPianist.Common
{
    public class AppInvoke
    {
        //user32
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpWindowClass, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);
        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out RECT lpRect);
        [DllImport("User32.dll")]
        private extern static IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")]
        public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool BRePaint);

        //gdi32
        private const int SRCCOPY = 0x00CC0020;
        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        [DllImport("gdi32.dll")]
        private static extern int GetPixel(IntPtr hDC, int x, int y);

        static IntPtr hWnd = IntPtr.Zero;
        static RECT rect = new RECT();

        public static Rectangle Rectangle { get; private set; }

        private struct RECT
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
        }

        public static IntPtr Init()
        {
            hWnd = FindWindow(null, "钢琴块2");
            var hWndChild = FindWindowEx(hWnd, IntPtr.Zero, null, "Chrome Legacy Window");
            GetWindowRect(hWndChild, out rect);
            Rectangle = new Rectangle(0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top);
            return hWnd;
        }

        public static Color GetColor(int x, int y)
        {
            IntPtr hdc = GetDC(hWnd);
            int c = GetPixel(hdc, x, y);
            ReleaseDC(hWnd, hdc);
            return Color.FromArgb(c & 0xFF, (c & 0xFF00) / 256, (c & 0xFF0000) / 65536);
        }

        public static Image GetImage()
        {
            IntPtr hdcSrc = GetDC(hWnd);
            IntPtr hdcDest = CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = CreateCompatibleBitmap(hdcSrc, Rectangle.Width, Rectangle.Height);
            IntPtr hOld = SelectObject(hdcDest, hBitmap);
            BitBlt(hdcDest, 0, 0, Rectangle.Width, Rectangle.Height, hdcSrc, 0, 0, SRCCOPY);
            SelectObject(hdcDest, hOld);
            DeleteDC(hdcDest);
            ReleaseDC(hWnd, hdcSrc);
            Image img = Image.FromHbitmap(hBitmap);
            DeleteObject(hBitmap);
            return img;
        }
    }
}
