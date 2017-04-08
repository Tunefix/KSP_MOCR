using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KSP_MOCR
{
	public class FDAI : Label
	{
		private float pitch = 0;
		private float roll = -10;
		private float yaw = 0;

		readonly Pen borderPen = new Pen(Color.FromArgb(255, 255, 255, 255), 1.0f);
		readonly Pen FDAIPen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.0f);
		readonly Pen crosshairPen = new Pen(Color.FromArgb(255, 255, 255, 0), 1.5f);
		readonly Brush skyPen = new SolidBrush(Color.FromArgb(255, 0, 101, 204));
		readonly Brush groundPen = new SolidBrush(Color.FromArgb(255, 101, 51, 0));
		readonly Brush arrowPen = new SolidBrush(Color.FromArgb(255, 255, 255, 255));

		readonly Pen maskPen = new Pen(Color.FromArgb(255, 0, 0, 0), 200);

		readonly Color yawNumbers = Color.FromArgb(200, 255, 255, 255);

		public FDAI()
		{
		}

		public void setAttitude(float pitch, float roll, float yaw)
		{
			this.pitch = pitch;
			this.roll = roll;
			this.yaw = yaw;
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			Console.WriteLine("Painting FDAI");
			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			int FDAI_size;
			int FDAI_left;
			int FDAI_top;

			bool horizon = false;
			float horizonY = 0;

			if (this.Width > this.Height)
			{
				FDAI_size = this.Height - 8;
				FDAI_left = ((this.Width - this.Height) / 2) + 4;
				FDAI_top = 4;
			}
			else
			{
				FDAI_size = this.Width - 8;
				FDAI_left = ((this.Height - this.Height) / 2) + 4;
				FDAI_top = 4;
			}

			int lowerLimit = (int)Math.Round(yaw - 40);
			int upperLimit = (int)Math.Round(yaw + 40);
			double oneDegSize = FDAI_size / 70;
			double FDAI_centerX = ((FDAI_size / 2) + FDAI_left);
			double FDAI_centerY = ((FDAI_size / 2) + FDAI_top);
			double bigLineLength = FDAI_size / 7;
			double shortLineLength = FDAI_size / 14;
			double crosshairLength = FDAI_size / 2;

			int x1, y1, x2, y2;
			Point[] line;

			Dictionary<Double, Point[]> yawBars = new Dictionary<Double, Point[]>();
			Dictionary<Double, Point[]> pitchBars = new Dictionary<Double, Point[]>();

			StringFormat stringFormat;


			// Draw Yaw bars
			for (int i = lowerLimit; i <= upperLimit; i++)
			{
				if (i % 10 == 0)
				{
					// Draw big line with numbers
					line = new Point[2];

					double diff = i - yaw;
					double centerX = oneDegSize * diff;
					double centerY = 0;

					x1 = (int)Math.Round(centerX);
					y1 = (int)Math.Round(centerY - (bigLineLength / 2));

					x2 = (int)Math.Round(centerX);
					y2 = (int)Math.Round(centerY + (bigLineLength / 2));

					line[0] = new Point(x1, y1);
					line[1] = new Point(x2, y2);

					yawBars.Add(i, line);
				}
				else if(i % 5 == 0)
				{
					// Draw shot line
					line = new Point[2];

					double diff = i - yaw;
					double centerX = oneDegSize * diff;
					double centerY = 0;

					x1 = (int)Math.Round(centerX);
					y1 = (int)Math.Round(centerY - (shortLineLength / 2));

					x2 = (int)Math.Round(centerX);
					y2 = (int)Math.Round(centerY + (shortLineLength / 2));

					line[0] = new Point(x1, y1);
					line[1] = new Point(x2, y2);

					yawBars.Add(i, line);

					//g.DrawLines(FDAIPen, line);
				}
			}

			/*
			 * DRAW PITCH BARS
			 */
			lowerLimit = (int)Math.Round(pitch - 40);
			upperLimit = (int)Math.Round(pitch + 40);
			bigLineLength = FDAI_size / 5;
			shortLineLength = FDAI_size / 10;

			for (int i = lowerLimit; i <= upperLimit; i++)
			{
				if (i % 10 == 0)
				{
					// Draw big line with numbers
					line = new Point[2];

					double diff = i - pitch;
					double centerX = 0;
					double centerY = (oneDegSize * diff) * -1;

					x1 = (int)Math.Round(centerX - (bigLineLength / 2));
					y1 = (int)Math.Round(centerY);

					x2 = (int)Math.Round(centerX + (bigLineLength / 2));
					y2 = (int)Math.Round(centerY);

					line[0] = new Point(x1, y1);
					line[1] = new Point(x2, y2);

					pitchBars.Add(i, line);

					// DRAW SKY AND GROUND
					if (i == 0)
					{
						horizon = true;
						horizonY = (float)centerY;
					}
				}
				else if(i % 5 == 0)
				{
					// Draw short line
					line = new Point[2];

					double diff = i - pitch;
					double centerX = 0;
					double centerY = (oneDegSize * diff) * -1;

					x1 = (int)Math.Round(centerX - (shortLineLength / 2));
					y1 = (int)Math.Round(centerY);

					x2 = (int)Math.Round(centerX + (shortLineLength / 2));
					y2 = (int)Math.Round(centerY);

					line[0] = new Point(x1, y1);
					line[1] = new Point(x2, y2);

					pitchBars.Add(i, line);
				}
			}

			/*
			 * NOW, actually rotate the canvas, and draw stuff, in the right order, back to front
			 */

			// Rotate canvas
			GraphicsState state = g.Save();
			g.ResetTransform();
			g.TranslateTransform((float)FDAI_centerX, (float)FDAI_centerY);
			g.RotateTransform(-roll);

			// If 0 degrees pitch was not found, draw either all sky or all ground.
			if (!horizon)
			{
				if (lowerLimit > 0)
				{
					g.FillRectangle(skyPen, (float)(0 - (FDAI_size / 2)), (float)(0 - (FDAI_size / 2)), FDAI_size, FDAI_size);
				}
				else
				{
					g.FillRectangle(groundPen, (float)(0 - (FDAI_size / 2)), (float)(0 - (FDAI_size / 2)), FDAI_size, FDAI_size);
				}
			}
			else
			{
				g.FillRectangle(skyPen, (float)(0 - (FDAI_size / 2)), (float)(horizonY - FDAI_size), FDAI_size, FDAI_size);

				g.FillRectangle(groundPen, (float)(0 - (FDAI_size / 2)), (float)(horizonY), FDAI_size, FDAI_size);
			}

			// DRAW PITCH AND ROLL LINES
			foreach(KeyValuePair<Double, Point[]> bar in pitchBars)
			{
				g.DrawLines(FDAIPen, bar.Value);

				String s = "";
				double i = bar.Key;
				if (i > 90) { s = (i - ((i - 90) * 2)).ToString(); }
				else if (i < -90) { s = (i - ((i + 90) * 2)).ToString(); }
				else { s = i.ToString(); }

				if (s.Substring(s.Length - 1, 1) == "5") { s = ""; }

				stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Far;
				stringFormat.LineAlignment = StringAlignment.Center;

				g.DrawString(s, Font, new SolidBrush(yawNumbers), bar.Value[0], stringFormat);

				stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Center;
				g.DrawString(s, Font, new SolidBrush(yawNumbers), bar.Value[1], stringFormat);
			}

			foreach (KeyValuePair<Double, Point[]> bar in yawBars)
			{
				g.DrawLines(FDAIPen, bar.Value);

				stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Far;

				String s = "";
				double i = bar.Key;
				if (i >= 360) { s = (i - 360).ToString(); }
				else if (i < 0) { s = (i + 360).ToString(); }
				else { s = i.ToString(); }

				if (s.Substring(s.Length - 1, 1) == "5") { s = "";}

				g.DrawString(s, Font, new SolidBrush(yawNumbers), bar.Value[0], stringFormat);

				stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Near;
				g.DrawString(s, Font, new SolidBrush(yawNumbers), bar.Value[1], stringFormat);
			}

			// DRAW ROLL ARROW
			int legAngle = 60;
			double legAngleRad = (legAngle * Math.PI)/ 180;
			double yOffset = Math.Round(FDAI_size / 12f);
			double xOffset = Math.Round(yOffset / Math.Tan(legAngleRad));

			Point[] arrow = new Point[4];
			arrow[0] = new Point(0, 0 - (FDAI_size / 2));
			arrow[1] = new Point((int)xOffset, (int)(0 - (FDAI_size / 2) + yOffset));
			arrow[2] = new Point((int)(xOffset * -1), (int)(0 - (FDAI_size / 2) + yOffset));
			arrow[3] = new Point(0, 0 - (FDAI_size / 2));

			g.FillPolygon(arrowPen, arrow);


			g.Restore(state);


			/*
			 * DRAW FIXED POINT
			 */

			// CrossHairs
			line = new Point[2];

			x1 = (int)Math.Round(FDAI_centerX - (crosshairLength / 2));
			y1 = (int)Math.Round(FDAI_centerY);

			x2 = (int)Math.Round(FDAI_centerX + (crosshairLength / 2));
			y2 = (int)Math.Round(FDAI_centerY);

			line[0] = new Point(x1, y1);
			line[1] = new Point(x2, y2);

			g.DrawLines(crosshairPen, line);

			line = new Point[2];

			x1 = (int)Math.Round(FDAI_centerX);
			y1 = (int)Math.Round(FDAI_centerY - (crosshairLength / 2));

			x2 = (int)Math.Round(FDAI_centerX);
			y2 = (int)Math.Round(FDAI_centerY + (crosshairLength / 2));

			line[0] = new Point(x1, y1);
			line[1] = new Point(x2, y2);

			g.DrawLines(crosshairPen, line);


			// Draw mask and outline
			g.DrawEllipse(maskPen, FDAI_left - 100, FDAI_top - 100, FDAI_size + 200, FDAI_size + 200);
			g.DrawEllipse(borderPen, FDAI_left, FDAI_top, FDAI_size, FDAI_size);

			Point[] border = new Point[5];
			border[0] = new Point(1, 1);
			border[1] = new Point(1, this.Height - 1);
			border[2] = new Point(this.Width - 1, this.Height - 1);
			border[3] = new Point(this.Width - 1, 1);
			border[4] = new Point(1, 1);

			g.DrawLines(borderPen, border);

		}
	}
}