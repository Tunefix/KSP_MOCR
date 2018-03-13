using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KSP_MOCR
{
	public class Screw : Control
	{
		public float angle = 0;
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.InterpolationMode = InterpolationMode.HighQualityBilinear;

			float size = this.Width - 6;
			
			

			Brush backest = new SolidBrush(Color.FromArgb(16, 0, 0, 0));
			Brush groove = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
			Color color1 = Color.FromArgb(255, 200, 200, 200);
			Color color2 = Color.FromArgb(255, 96, 96, 96);
			Brush screw = new LinearGradientBrush(new RectangleF(3, 3, size, size), color1, color2, angle);
			Pen rim = new Pen(Color.FromArgb(200, 64, 64, 64), 1f);
			Pen highlight = new Pen(Color.FromArgb(64, 255, 255, 255), 1f);
			

			
			g.FillEllipse(backest, 1, 1, size + 4, size + 4);
			g.FillEllipse(screw, 3, 3, size, size);
			g.DrawEllipse(rim, 2, 2, size + 1, size + 1);
			g.DrawEllipse(highlight, 3, 3, size, size);

			GraphicsState state = g.Save();

			g.TranslateTransform(Width / 2, Height / 2);
			g.RotateTransform(angle);
			g.TranslateTransform(-Width / 2, -Height / 2);


			g.FillRectangle(groove, 3, (Height / 2) - 2, size, 4);
			g.DrawLine(highlight, 3, (Height / 2) - 2, Width - 3, (Height / 2) - 2);
			g.DrawLine(highlight, 3, (Height / 2) + 2, Width - 3, (Height / 2) + 2);

			g.Restore(state);
		}
	}
}
