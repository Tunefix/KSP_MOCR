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

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;

			float size;
			if (this.Width > this.Height)
			{
				size = this.Height;
			}
			else
			{
				size = this.Width;
			}
	
			RectangleF rect = new RectangleF(1, 1, size-2, size-2);

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
