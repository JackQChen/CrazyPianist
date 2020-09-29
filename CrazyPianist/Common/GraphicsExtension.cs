using System.Drawing;

namespace CrazyPianist.Common
{
    public static class GraphicsExtension
    {
        public static void DrawCross(this Graphics g, int x, int y, int w, int h)
        {
            int l1 = x, t1 = y + h / 2, l2 = x + w / 2, t2 = y;
            g.FillRectangle(Brushes.White, l1, t1, w, 2);
            g.FillRectangle(Brushes.White, l2, t2, 2, h);
        }
    }
}
