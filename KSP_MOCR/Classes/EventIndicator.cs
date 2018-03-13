using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace KSP_MOCR
{
	public class EventIndicator : Control
	{
		public string upperText = "";
		public string lowerText = "";

		public bool small = false;

		public enum color { OFF, WHITE, AMBER, RED, GREEN, BLUE, WHITE_LIT, AMBER_LIT, RED_LIT, GREEN_LIT, BLUE_LIT}

		public color upperColor = color.WHITE;
		public color lowerColor = color.WHITE;

		enum type { UPPER, LOWER, FULL}

		readonly Brush background = new SolidBrush(Color.FromArgb(255, 128, 128, 128));
		readonly Brush black = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
		readonly Pen border = new Pen(Color.FromArgb(255, 128, 128, 128), 4f);

		readonly Brush textBrushLight = new SolidBrush(Color.FromArgb(200, 0, 0, 0));

		readonly Brush Off = new SolidBrush(Color.FromArgb(255, 80, 80, 80));
		readonly Brush BlankDim = new SolidBrush(Color.FromArgb(255, 200, 200, 198));
		readonly Brush BlankLit = new SolidBrush(Color.FromArgb(255, 255, 255, 253));
		readonly Brush AmberDim = new SolidBrush(Color.FromArgb(255, 128, 100, 32));
		readonly Brush AmberLit = new SolidBrush(Color.FromArgb(255, 255, 200, 32));
		readonly Brush RedDim = new SolidBrush(Color.FromArgb(255, 128, 32, 32));
		readonly Brush RedLit = new SolidBrush(Color.FromArgb(255, 255, 32, 32));
		readonly Brush GreenDim = new SolidBrush(Color.FromArgb(255, 32, 128, 32));
		readonly Brush GreenLit = new SolidBrush(Color.FromArgb(255, 32, 255, 32));
		readonly Brush BlueDim = new SolidBrush(Color.FromArgb(255, 32, 32, 128));
		readonly Brush BlueLit = new SolidBrush(Color.FromArgb(255, 64, 64, 255));

		public EventIndicator()
		{
			this.DoubleBuffered = true;
		}

		public void updateState(bool lower, color c)
		{
			if(lower)
			{
				lowerColor = c;
			}
			else
			{
				upperColor = c;
			}
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.InterpolationMode = InterpolationMode.HighQualityBilinear;
			g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

			if(small)
			{
				drawSmall(g);
			}
			else
			{
				drawNormal(g);
			}
		}

		void drawSmall(Graphics g)
		{
			// FILL
			drawFill(type.FULL, upperColor, g);
			// SHADE
			drawLightShade(type.FULL, g);

			drawText(type.FULL, upperText, g);

			// OUTER BORDER
			g.DrawRectangle(border, 0, 0, this.Width, this.Height);
		}

		void drawNormal(Graphics g)
		{ 

			// FILL
			drawFill(type.UPPER, upperColor, g);
			drawFill(type.LOWER, lowerColor, g);

			// SHADE
			drawLightShade(type.UPPER, g);
			drawLightShade(type.LOWER, g);

			// TEXT
			drawText(type.UPPER, upperText, g);
			drawText(type.LOWER, lowerText, g);

			// OUTER BORDER
			g.DrawRectangle(border, 0, 0, this.Width, this.Height);

			// BORDERS AND MIDDLE SEP LINE
			g.FillRectangle(black, 4, 4, this.Width - 8, 4);
			g.FillRectangle(black, 4, this.Height - 8, this.Width - 8, 4);
			g.FillRectangle(black, 4, 4, 2, this.Height - 8);
			g.FillRectangle(black, this.Width - 6, 4, 2, this.Height - 8);
			g.FillRectangle(black, 6, (this.Height / 2) - 0.5f, this.Width - 12, 1);

		}

		void drawLightShade(type t, Graphics g)
		{
			float x = 6;
			float y = 8;
			float w = this.Width - 12;
			float h = (this.Height / 2) - 8;

			if (t == type.FULL) { x = 2; y = 2; h = this.Height - 4; w = this.Width - 4; }
			if (t == type.LOWER) y += h;

			// LIGHT SHADE
			RectangleF mask = new RectangleF(x - (w / 2f), y - (h / 2f), w * 2, h * 2);
			GraphicsPath ellipsePath = new GraphicsPath();
			ellipsePath.AddEllipse(mask);

			using (var maskBrush = new PathGradientBrush(ellipsePath))
			{
				maskBrush.CenterPoint = new PointF(x + (w / 2f), y + (h / 2f));
				maskBrush.CenterColor = Color.FromArgb(0, 0, 0, 0);
				maskBrush.SurroundColors = new[] { Color.FromArgb(160, 0, 0, 0) };
				maskBrush.FocusScales = new PointF(0.2f, 0.2f);

				g.FillRectangle(maskBrush, new RectangleF(x, y, w, h));
			}
		}

		void drawFill(type t, color c, Graphics g)
		{
			int x = 6;
			int y = 8;
			int w = this.Width - 12;
			int h = (this.Height / 2) - 8;

			if (t == type.FULL) { x = 2; y = 2; h = this.Height - 4; w = this.Width - 4; }

			if (t == type.LOWER) y = this.Height / 2;

			switch(c)
			{
				case color.OFF:
					g.FillRectangle(Off, x, y, w, h);
					break;
				case color.AMBER:
					g.FillRectangle(AmberDim, x, y, w, h);
					break;
				case color.AMBER_LIT:
					g.FillRectangle(AmberLit, x, y, w, h);
					break;
				case color.RED:
					g.FillRectangle(RedDim, x, y, w, h);
					break;
				case color.RED_LIT:
					g.FillRectangle(RedLit, x, y, w, h);
					break;
				case color.GREEN:
					g.FillRectangle(GreenDim, x, y, w, h);
					break;
				case color.GREEN_LIT:
					g.FillRectangle(GreenLit, x, y, w, h);
					break;
				case color.BLUE:
					g.FillRectangle(BlueDim, x, y, w, h);
					break;
				case color.BLUE_LIT:
					g.FillRectangle(BlueLit, x, y, w, h);
					break;
				case color.WHITE_LIT:
					g.FillRectangle(BlankLit, x, y, w, h);
					break;

				case color.WHITE:
				default:
					g.FillRectangle(BlankDim, x, y, w, h);
					break;
			}
		}

		void drawText(type t, string text, Graphics g)
		{
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;

			int x = 5;
			float y = 8;
			float w = this.Width - 10;
			int h = (this.Height / 2) - 8;

			if (t == type.FULL) { x = 2; y = 2; h = this.Height - 4; w = this.Width - 4; }
			if (t == type.LOWER) y = (this.Height / 2) + 1f;

			GraphicsState state = g.Save();
			if(t == type.FULL) { g.ScaleTransform(0.80f, 1.0f); w = w * 1.25f; }

			// SPLIT BY NEWLIE
			string[] lines = text.Split('\n');

			if (lines.Length == 1)
			{
				g.DrawString(text, this.Font, textBrushLight, new RectangleF(x, y, w, h), format);
			}
			else
			{
				
				if (t == type.FULL) { y += 2.5f; h = 7; } else { y += 1; h = 9; }
				g.DrawString(lines[0], this.Font, textBrushLight, new RectangleF(x, y, w, h), format);
				if (t == type.FULL) { y += 7.5f; } else { y+= 9f; }
				g.DrawString(lines[1], this.Font, textBrushLight, new RectangleF(x, y, w, h), format);
			}

			g.Restore(state);
		}
	}
}
