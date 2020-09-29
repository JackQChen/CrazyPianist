using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrazyPianist.Common;

namespace CrazyPianist
{
    public partial class FrmMain : Form
    {
        bool isRun = false;
        AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var hWnd = AppInvoke.Init();
                MouseInvoke.InitApp(hWnd);
                var rect = AppInvoke.Rectangle;
                var g = Graphics.FromHwnd(hWnd);
                var pArray = new Point[4];
                var w = rect.Width / 4;
                for (int i = 0; i < pArray.Length; i++)
                    pArray[i] = new Point(w / 2 + i * w, rect.Height * 2 / 3);
                while (true)
                {
                    if (!isRun)
                        autoResetEvent.WaitOne();
                    Process(g, pArray);
                    ShowGrid(g, w, pArray);
                    Thread.Sleep(1);
                }
            });
        }

        void ShowGrid(Graphics g, int w, Point[] pArr)
        {
            for (int i = 0; i < pArr.Length; i++)
            {
                g.DrawCross(pArr[i].X - w / 2, pArr[i].Y - w / 2, w, w);
            }
        }

        void Process(Graphics g, Point[] pArr)
        {
            for (int i = 0; i < pArr.Length; i++)
            {
                var c = AppInvoke.GetColor(pArr[i].X - 10, pArr[i].Y - 10);
                var isHit = c.R < 50 && c.G < 50 && c.B < 50;
                g.DrawString(isHit.ToString(), this.Font, Brushes.White, pArr[i].X + 5, pArr[i].Y + 5);
                if (isHit)
                    MouseInvoke.AppClick(pArr[i].X, pArr[i].Y);
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            isRun = !isRun;
            if (isRun)
            {
                autoResetEvent.Set();
                this.btnRun.Text = "停止";
                this.btnRun.ImageIndex = 1;
            }
            else
            {
                this.btnRun.Text = "启动";
                this.btnRun.ImageIndex = 0;
            }
        }
    }
}

