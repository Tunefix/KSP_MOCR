using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KSP_MOCR
{
	public class EngineIndicator : Control
	{
		bool status = false;

		public double offsetX;
		public double offsetY;
		private int x;
		private int y;
		private bool disp = false;
		
		public readonly int maxDiameter = 90;
		public readonly int minDiameter = 36;
		public readonly double maxThrust = 2000000;
		private double thrust;

		readonly Pen whitePen = new Pen(Color.FromArgb(255, 200, 200, 200), 1f);
		readonly Pen borderPen = new Pen(Color.FromArgb(255, 64, 64, 64), 1f);
		
		readonly Brush blackBrush = new SolidBrush(Color.FromArgb(255, 16, 16, 16));
		readonly Brush whiteBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));

		public EngineIndicator()
		{
			this.DoubleBuffered = true;
		}

		public void setStatus(bool on)
		{
			status = on;
			this.Invalidate();
		}

		public void setThrust(double t)
		{
			thrust = t;
		}

		public void setCenterPoint(int x, int y)
		{
			this.x = x;
			this.y = y;
			int offset = (int)Math.Round(Width / 2f);
			Location = new Point(x - offset, y - offset);
		}

		public void display(bool b)
		{
			this.disp = b;
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (disp)
			{
				Graphics g = e.Graphics;
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

				StringFormat format = new StringFormat();
				format.Alignment = StringAlignment.Center;
				format.LineAlignment = StringAlignment.Center;


			
				//float size = (float)((thrust / maxThrust) * maxDiameter);
				float size = (float)(2 * Math.Sqrt((0.0032 * thrust)/Math.PI));

				if (size < minDiameter) size = minDiameter;
				if (size > maxDiameter) size = maxDiameter;



				int left = (int)Math.Round(x - (size / 2f));
				int top = (int)Math.Round(y - (size / 2f));
				Location = new Point(Left, Top);
				Size = new Size((int)Math.Ceiling(size), (int)Math.Ceiling(size));

				RectangleF rect = new RectangleF(1, 1, size - 2, size - 2);

				// Fill background, draw outer border and number
				if (status)
				{
					g.FillEllipse(whiteBrush, rect);
					g.DrawEllipse(borderPen, rect);
					g.DrawString(this.Text, this.Font, blackBrush, this.Width / 2f, this.Height / 2f, format);
				}
				else
				{
					g.FillEllipse(blackBrush, rect);
					g.DrawEllipse(borderPen, rect);
					g.DrawString(this.Text, this.Font, whiteBrush, this.Width / 2f, this.Height / 2f, format);
				}
			}
		}
	}
}
