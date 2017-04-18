using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KSP_MOCR
{
	public class VerticalMeter : Control
	{
		public bool doubleMeter = false;
		float value1 = 0;
		float value2 = 0;

		int scale1min = 0;
		int scale1max = 100;
		int scale2min = 0;
		int scale2max = 100;

		Font font;

		readonly Pen borderPen = new Pen(Color.FromArgb(255, 32, 32, 32), 3f);
		readonly Pen scalePen = new Pen(Color.FromArgb(240, 255, 255, 255), 1.5f);
		readonly Brush scaleBrush = new SolidBrush(Color.FromArgb(240, 255, 255, 255));
		readonly Brush pointerBrush = new SolidBrush(Color.FromArgb(255, 16, 16, 16));
		
		public VerticalMeter(Font font)
		{
			this.DoubleBuffered = true;
			this.font = font;
		}

		public void setScale(int min, int max) { setScale1(min, max); setScale2(min, max);}
		
		public void setScale1(int min, int max)
		{
			scale1min = min;
			scale1max = max;
		}
		
		public void setScale2(int min, int max)
		{
			scale2min = min;
			scale2max = max;
		}

		public void setValue(float value) { setValue1(value); setValue2(value);}
		
		public void setValue1(float value)
		{
			value1 = value;
			this.Invalidate();
		}
		
		public void setValue2(float value)
		{
			value2 = value;
			this.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			RectangleF Rect = new Rectangle(0, 0, Width, Height);
			Rectangle Rectb = new Rectangle(0, 0, Width, Height);

			// Black background
			g.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, 0, 0)), Rect);

			// Peg the arrows
			if (value1 > scale1max) { value1 = scale1max; }
			if (value1 < scale1min) { value1 = scale1min; }
			if (value2 > scale2max) { value2 = scale2max; }
			if (value2 < scale2min) { value2 = scale2min; }

			// Set nan to scaleMinimum
			if (double.IsNaN((double)value1)){ value1 = scale1min; }
			if (double.IsNaN((double)value2)){ value2 = scale2min; }

			if (doubleMeter)
			{
				drawDoubleWhite(g);
				drawDoubleScale(g);
				drawDoublePointer(g);
			}
			else
			{
				drawSingleWhite(g);
				drawSingleScale(g);
				drawSinglePointer(g);
			}
			
			// Border Line			
			g.DrawRectangle(borderPen, Rectb);
		}

		private void drawSingleWhite(Graphics g)
		{
			// Arrow-Lane (The white(glowy) bit
			RectangleF Rect = new Rectangle(3, 0, (Width / 3), Height / 2);
			Brush whiteGrad = new LinearGradientBrush(Rect, Color.FromArgb(200, 255, 255, 255), Color.FromArgb(255, 255, 255, 255), 90);
			g.FillRectangle(whiteGrad, Rect);

			Rect = new Rectangle(3, (Height / 2) - 1, (Width / 3), Height / 2);
			whiteGrad = new LinearGradientBrush(Rect, Color.FromArgb(255, 255, 255, 255), Color.FromArgb(200, 255, 255, 255), 90);
			g.FillRectangle(whiteGrad, Rect);

			// Arrow-Lane vertical drop-shadows
			Rect = new Rectangle(2, 0, 3, Height);
			whiteGrad = new LinearGradientBrush(Rect, Color.FromArgb(200, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), 0f);
			g.FillRectangle(whiteGrad, Rect);

			Rect = new Rectangle(2 + (Width / 3), 0, 3, Height);
			whiteGrad = new LinearGradientBrush(Rect, Color.FromArgb(200, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), 180f);
			g.FillRectangle(whiteGrad, Rect);
		}

		private void drawDoubleWhite(Graphics g)
		{
			// Arrow-Lane (The white(glowy) bit
			RectangleF Rect = new Rectangle(3, 0, (Width / 5), Height / 2);
			Brush whiteGrad = new LinearGradientBrush(Rect, Color.FromArgb(200, 255, 255, 255), Color.FromArgb(255, 255, 255, 255), 90);
			g.FillRectangle(whiteGrad, Rect);

			Rect = new Rectangle(3, (Height / 2) - 1, (Width / 5), Height / 2);
			whiteGrad = new LinearGradientBrush(Rect, Color.FromArgb(255, 255, 255, 255), Color.FromArgb(200, 255, 255, 255), 90);
			g.FillRectangle(whiteGrad, Rect);

			// Arrow-Lane vertical drop-shadows
			Rect = new Rectangle(2, 0, 3, Height);
			whiteGrad = new LinearGradientBrush(Rect, Color.FromArgb(200, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), 0f);
			g.FillRectangle(whiteGrad, Rect);

			Rect = new Rectangle(2 + (Width / 5), 0, 3, Height);
			whiteGrad = new LinearGradientBrush(Rect, Color.FromArgb(200, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), 180f);
			g.FillRectangle(whiteGrad, Rect);


			float x = Width - (Width / 5) - 3;
			// Arrow-Lane (The white(glowy) bit
			Rect = new RectangleF(x, 0, (Width / 5), Height / 2);
			whiteGrad = new LinearGradientBrush(Rect, Color.FromArgb(200, 255, 255, 255), Color.FromArgb(255, 255, 255, 255), 90);
			g.FillRectangle(whiteGrad, Rect);

			Rect = new RectangleF(x, (Height / 2) - 1, (Width / 5), Height / 2);
			whiteGrad = new LinearGradientBrush(Rect, Color.FromArgb(255, 255, 255, 255), Color.FromArgb(200, 255, 255, 255), 90);
			g.FillRectangle(whiteGrad, Rect);

			// Arrow-Lane vertical drop-shadows
			Rect = new RectangleF(x - 1, 0, 3, Height);
			whiteGrad = new LinearGradientBrush(Rect, Color.FromArgb(200, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), 0f);
			g.FillRectangle(whiteGrad, Rect);

			Rect = new RectangleF(x - 1 + (Width / 5), 0, 3, Height);
			whiteGrad = new LinearGradientBrush(Rect, Color.FromArgb(200, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), 180f);
			g.FillRectangle(whiteGrad, Rect);
		}

		private void drawSingleScale(Graphics g)
		{
			// Scale
			float x1 = (Width / 3) + 3;
			float x2 = (Width / 3) + 8;
			float[] scaleData = getScaleData(scale1max, scale1min);
			StringFormat format = new StringFormat();
			format.LineAlignment = StringAlignment.Center;

			for (int i = 0; i < scaleData[2] + 1; i++)
			{
				float y1 = (Height - 10) - (scaleData[1] * i);
				float y2 = y1;
				g.DrawLine(scalePen, x1, y1, x2, y2);

				g.DrawString(Math.Round(i * scaleData[0]).ToString(), font, scaleBrush, x2 + 2, y2, format);
			}
		}
		
		
		private void drawDoubleScale(Graphics g)
		{
			// Scale 1
			float x1 = (Width / 5) + 3;
			float x2 = (Width / 5) + 6;
			float[] scaleData = getScaleData(scale1max, scale1min);
			StringFormat format = new StringFormat();
			format.LineAlignment = StringAlignment.Center;

			for (int i = 0; i < scaleData[2] + 1; i++)
			{
				float y1 = (Height - 10) - (scaleData[1] * i);
				float y2 = y1;
				g.DrawLine(scalePen, x1, y1, x2, y2);

				g.DrawString(Math.Round(i * scaleData[0]).ToString(), font, scaleBrush, x2 + 1, y2, format);
			}
			
			// Scale 2
			x1 = Width - (Width / 5) - 3;
			x2 = Width - (Width / 5) - 6;
			scaleData = getScaleData(scale1max, scale1min);
			format.LineAlignment = StringAlignment.Center;
			format.Alignment = StringAlignment.Far;

			for (int i = 0; i < scaleData[2] + 1; i++)
			{
				float y1 = (Height - 10) - (scaleData[1] * i);
				float y2 = y1;
				g.DrawLine(scalePen, x1, y1, x2, y2);

				g.DrawString(Math.Round(i * scaleData[0]).ToString(), font, scaleBrush, x2 - 1, y2, format);
			}
		}

		private void drawSinglePointer(Graphics g)
		{
			// Pointer
			double scaler = (Height - 20) / (double)Math.Abs(scale1max - scale1min);
			float y = (float)((Height - 10) - (value1 * scaler));
			float x = 3;
			float w = Width / 3;
			float a = 50; // Angle at "pointy" end


			PointF[] points = new PointF[4];
			points[0] = new PointF(x + w, y);
			points[1] = new PointF(x, (float)(y + (w * Math.Sin(Helper.deg2rad(a / 2)))));
			points[2] = new PointF(x, (float)(y - (w * Math.Sin(Helper.deg2rad(a / 2)))));
			points[3] = new PointF(x + w, y);
			g.FillPolygon(pointerBrush, points);

			RectangleF Rect = new RectangleF(0, y - 1, 4, 2);
			g.FillRectangle(pointerBrush, Rect);
		}
		
		
		private void drawDoublePointer(Graphics g)
		{
			// Pointer 1
			double scaler = (Height - 20) / (double)Math.Abs(scale1max - scale1min);
			float y = (float)((Height - 10) - (value1 * scaler));
			float x = 3;
			float w = Width / 5;
			float a = 50; // Angle at "pointy" end


			PointF[] points = new PointF[4];
			points[0] = new PointF(x + w, y);
			points[1] = new PointF(x, (float)(y + (w * Math.Sin(Helper.deg2rad(a / 2)))));
			points[2] = new PointF(x, (float)(y - (w * Math.Sin(Helper.deg2rad(a / 2)))));
			points[3] = new PointF(x + w, y);
			g.FillPolygon(pointerBrush, points);

			RectangleF Rect = new RectangleF(0, y - 1, 4, 2);
			g.FillRectangle(pointerBrush, Rect);
			
			
			// Pointer 2
			scaler = (Height - 20) / (double)Math.Abs(scale2max - scale2min);
			y = (float)((Height - 10) - (value2 * scaler));
			x = Width - 3;
			w = Width / 5;
			a = 50; // Angle at "pointy" end


			points = new PointF[4];
			points[0] = new PointF(x - w, y);
			points[1] = new PointF(x, (float)(y + (w * Math.Sin(Helper.deg2rad(a / 2)))));
			points[2] = new PointF(x, (float)(y - (w * Math.Sin(Helper.deg2rad(a / 2)))));
			points[3] = new PointF(x - w, y);
			g.FillPolygon(pointerBrush, points);

			Rect = new RectangleF(Width - 4, y - 1, 4, 2);
			g.FillRectangle(pointerBrush, Rect);
		}


		private float[] getScaleData(int max, int min)
		{
			float[] ret = new float[4];
			
			int split = max - min;
			float xPrPx = (float)(split / (Height - 20f));
			int maxLabels = (int)Math.Floor((Height - 20) / 25f);
			int numLabels = (int)(Math.Ceiling(maxLabels / 5f) * 5);
			float gridStep = split / (float)numLabels;
			
			int gridLines = (int)Math.Floor(split / gridStep);
			float gridStepPx = (float)(gridStep / xPrPx);

			ret[0] = gridStep;
			ret[1] = gridStepPx;
			ret[2] = gridLines;
			ret[3] = xPrPx;

			return ret;
		}
	}
}
