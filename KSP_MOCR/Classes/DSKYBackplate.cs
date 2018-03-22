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
	class DSKYBackplate : CustomLabel
	{
		readonly Brush blackBrush = new SolidBrush(Color.FromArgb(255, 16, 16, 16));
		readonly Brush backBrush = new SolidBrush(Color.FromArgb(255, 32, 32, 32));
		readonly Brush backgroundBrush = new SolidBrush(Color.FromArgb(255, 96, 96, 96));

		readonly Brush whiteBrush = new SolidBrush(Color.FromArgb(64, 200, 204, 194));

		readonly Brush greenBrush = new SolidBrush(Color.FromArgb(255, 0, 200, 0));

		readonly Brush screwBrush1 = new SolidBrush(Color.FromArgb(255, 160, 160, 160));
		readonly Brush screwBrush2 = new SolidBrush(Color.FromArgb(255, 96, 96, 96));
		readonly Pen screwPen = new Pen(Color.FromArgb(255, 64, 64, 64), 1f);

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.InterpolationMode = InterpolationMode.HighQualityBilinear;

			drawBackground(g);
		}

		private void drawBackground(Graphics g)
		{

			// FILL BACKGROUND
			g.FillRectangle(backgroundBrush, 0, 0, Width, Height);

			// Add screwholes
			g.FillEllipse(backBrush, -2, -2, 20, 20);
			g.FillEllipse(backBrush, 175, -2, 20, 20);
			g.FillEllipse(backBrush, Width - 18, -2, 20, 20);
			
			g.FillRectangle(backBrush, 0, 0, 8, 18);
			g.FillRectangle(backBrush, 8, 0, 10, 8);
			g.FillRectangle(backBrush, 175, 0, 20, 8);
			g.FillRectangle(backBrush, Width - 8, 0, 8, 18);
			g.FillRectangle(backBrush, Width - 18, 0, 10, 8);

			g.FillEllipse(backBrush, -2, 251, 20, 20);
			g.FillEllipse(backBrush, 175, 251, 20, 20);
			g.FillEllipse(backBrush, Width - 18, 251, 20, 20);

			g.FillRectangle(backBrush, 0, 251, 8, 20);
			g.FillRectangle(backBrush, Width - 8, 251, 8, 20);

			g.FillEllipse(backBrush, -2, Height - 18, 20, 20);
			g.FillEllipse(backBrush, Width - 18, Height - 18, 20, 20);

			g.FillRectangle(backBrush, 0, Height - 18, 8, 18);
			g.FillRectangle(backBrush, 8, Height - 8, 10, 8);
			g.FillRectangle(backBrush, Width - 8, Height - 18, 8, 18);
			g.FillRectangle(backBrush, Width - 18, Height - 8, 10, 8);

			// ADD SCREWS
			addScrew(1, 1, g);
			addScrew(178, 1, g);
			addScrew(355, 1, g);

			addScrew(1, 254, g);
			addScrew(178, 254, g);
			addScrew(355, 254, g);

			addScrew(1, 421, g);
			addScrew(355, 421, g);

			// ADD HOLES FOR LIGHTS AND 7-SIGS
			g.FillRectangle(backBrush, 23, 15, 150, 240);
			g.FillRectangle(blackBrush, 201, 15, 150, 240);

			// DSKY BARS AND WHITE SPOTS
			g.FillRectangle(greenBrush, 225, 210, 113, 2.5f);
			g.FillRectangle(greenBrush, 225, 168, 113, 2.5f);
			g.FillRectangle(greenBrush, 225, 120, 113, 2.5f);
			g.FillRectangle(whiteBrush, 225, 124, 113, 2.5f);
			g.FillEllipse(whiteBrush, 206, 210, 5f, 5f);
			g.FillEllipse(whiteBrush, 343, 210, 5f, 5f);
			g.FillEllipse(whiteBrush, 206, 168, 5f, 5f);
			g.FillEllipse(whiteBrush, 343, 168, 5f, 5f);
			g.FillEllipse(whiteBrush, 206, 122, 5f, 5f);
			g.FillEllipse(whiteBrush, 343, 122, 5f, 5f);

			g.FillEllipse(whiteBrush, 275f, 110, 5f, 5f);
			g.FillEllipse(whiteBrush, 275f, 65, 5f, 5f);
			g.FillEllipse(whiteBrush, 275f, 20, 5f, 5f);
		}

		private void addScrew(float x, float y, Graphics g)
		{
			g.FillEllipse(screwBrush1, x, y, 14, 14);
			g.FillPolygon(screwBrush2, screwInfillPoints(x + 2.5f, y + 2.5f, 9));
			g.DrawPolygon(screwPen, screwInfillPoints(x + 2.5f, y + 2.5f, 9));
			g.DrawEllipse(screwPen, x, y, 14, 14);
		}

		private PointF[] screwInfillPoints(float x, float y, float size)
		{
			PointF[] path = new PointF[6];
			float s = (float)((size / 2f) * Math.Sin(Helper.deg2rad(30)));
			float l = (float)((size / 2f) * Math.Cos(Helper.deg2rad(30)));

			path[0] = new PointF(x, y + (size / 2f));
			path[1] = new PointF(x + s, y + (size / 2f) - l);
			path[2] = new PointF(x + size - s, y + (size / 2f) - l);

			path[3] = new PointF(x + size, y + (size / 2f));
			path[4] = new PointF(x + size - s, y + (size / 2f) + l);
			path[5] = new PointF(x + s, y + (size / 2f) + l);
			

			return path;
		}
	}
}
