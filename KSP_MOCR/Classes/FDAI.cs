using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace KSP_MOCR
{
	public partial class FDAI : Label
	{
		/**
		 * The rotation order is: Yaw, Pitch, Roll (YXZ)
		 *
		 * The axis are:
		 *   X: left/right
		 *   Y: up/down
		 *   Z: in/out
		 */
		public double pitch = 0;
		public double roll = 0;
		public double yaw = 0;

		public double offsetR = 0;
		public double offsetP = 0;
		public double offsetY = 0;

		double errorR = 0;
		double errorP = 0;
		double errorY = 0;
		double errorScale = 5; // Degrees at full deflection

		double rollRateR = 0;
		double rollRateP = 0;
		double rollRateY = 0;
		double rollRateScale = 5;

		double zeroR = 0;
		double zeroP = 0;
		double zeroY = 0;

		Tuple<double, double, double, double> rotation;
		readonly Tuple<double, double, double, double> baseDirection = new Tuple<double, double, double, double>(0, 1, 0, 0); // w, x, y, z

		List<FDAILine> lineCollection = new List<FDAILine>();
		List<FDAILine> numberCollection = new List<FDAILine>();
		List<FDAIPolygon> polygonCollection = new List<FDAIPolygon>();
		List<FDAIPolygon> shieldCollection = new List<FDAIPolygon>();
		KeyValuePair<List<FDAILine>, List<FDAIPolygon>> comboCollection;


		float FDAI_size;
		float FDAI_left;
		float FDAI_top;
		double FDAI_centerX;
		double FDAI_centerY;

		readonly Pen borderPen = new Pen(Color.FromArgb(255, 96, 96, 96), 1.0f);
		readonly Pen borderPenLight = new Pen(Color.FromArgb(200, 96, 96, 96), 1.0f);
		readonly Pen FDAIPen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.0f);
		
		
		readonly Pen grayCenterPen = new Pen(Color.FromArgb(255, 64, 64, 64), 10f);
		readonly Pen blackCenterPen = new Pen(Color.FromArgb(255, 32, 32, 21), 8f);
		readonly Pen whiteCenterPen = new Pen(Color.FromArgb(255, 253, 250, 250), 2f);
		
		readonly Brush skyPen = new SolidBrush(Color.FromArgb(255, 0, 101, 204));
		readonly Brush groundPen = new SolidBrush(Color.FromArgb(255, 101, 51, 0));
		readonly Brush arrowPen = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
		
		
		
		readonly Brush whiteBrush = new SolidBrush(Color.FromArgb(255, 200, 204, 194));
		readonly Brush pureWhiteBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
		readonly Brush blackBrush = new SolidBrush(Color.FromArgb(255, 16, 16, 16));
		readonly Brush redBrush = new SolidBrush(Color.FromArgb(255, 180, 28, 25));
		
		readonly Pen whiteNumberPen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.0f);
		readonly Pen blackNumberPen = new Pen(Color.FromArgb(200, 32, 32, 32), 1.0f);

		readonly static Color maskColor = Color.FromArgb(255, 16, 16, 16); // Inner "black" rings
		readonly static Color mask2Color = Color.FromArgb(255, 32, 32, 32); // Outer grey panel
		readonly Pen maskPen = new Pen(maskColor, 200);
		readonly Pen mask2Pen = new Pen(mask2Color, 200);
		
		readonly Pen outerBorderPen = new Pen(Color.FromArgb(255, 96, 96, 96), 2.0f);

		readonly Color yawNumbers = Color.FromArgb(200, 255, 255, 255);

		readonly Pen ShadowPen1 = new Pen(Color.FromArgb(55, 0, 0, 0), 1.0f);
		readonly Pen ShadowPen2 = new Pen(Color.FromArgb(45, 0, 0, 0), 1.0f);
		readonly Pen ShadowPen3 = new Pen(Color.FromArgb(35, 0, 0, 0), 1.0f);
		readonly Pen ShadowPen4 = new Pen(Color.FromArgb(25, 0, 0, 0), 1.0f);
		readonly Pen ShadowPen5 = new Pen(Color.FromArgb(15, 0, 0, 0), 1.0f);

		public FDAI()
		{
            this.DoubleBuffered = true;
		}

		/*
		public void setAttitude(float roll, float pitch, float yaw)
		{
			this.pitch = pitch;
			this.roll = roll;
			this.yaw = yaw;
		}*/

		public void setRotation(Tuple<double, double, double, double> q)
		{
			this.rotation = q;
		}

		public void setError(double roll, double pitch, double yaw, double scale)
		{
			errorR = roll;
			errorP = pitch;
			errorY = yaw;
			errorScale = scale;
		}

		public void setRates(double roll, double pitch, double yaw, double scale)
		{
			rollRateR = roll;
			rollRateP = pitch;
			rollRateY = yaw;
			rollRateScale = scale;
		}

		public void setOffset(double roll, double pitch, double yaw)
		{
			offsetR = roll;
			offsetP = pitch;
			offsetY = yaw;
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			if (this.rotation != null)
			{
				setRotFromQuaternion(this.rotation);
			}

			/*Console.WriteLine("FDAI R: " + Helper.prtlen(Helper.toFixed(roll, 2), 8, Helper.Align.RIGHT)
				+ "  P: " + Helper.prtlen(Helper.toFixed(pitch, 2), 8, Helper.Align.RIGHT)
				+ "  Y: " + Helper.prtlen(Helper.toFixed(yaw, 2), 8, Helper.Align.RIGHT)
				);*/

			paintFDAI(e, g);
		}

		private void paintFDAI(PaintEventArgs e, Graphics g)
		{
			/**
			 * THIS IS THE FLIGHT DIRECTOR ATTITUDE INDICATOR (FDAI) TYPE
			 **/

			if (this.Width >= this.Height)
			{
				FDAI_size = this.Height * 0.6f;
				FDAI_left = ((this.Width - FDAI_size) / 2);
				FDAI_top = this.Height * 0.2f;
			}
			else
			{
				FDAI_size = this.Width * 0.6f;
				FDAI_top = ((this.Height - FDAI_size) / 2);
				FDAI_left = this.Width * 0.2f;
			}

			int lowerLimit = (int)Math.Round(yaw - 40);
			int upperLimit = (int)Math.Round(yaw + 40);
			double oneDegSize = FDAI_size / 70;
			FDAI_centerX = ((FDAI_size / 2) + FDAI_left);
			FDAI_centerY = ((FDAI_size / 2) + FDAI_top);
			double bigLineLength = FDAI_size / 7;
			double shortLineLength = FDAI_size / 14;
			

			double rotX = -yaw - offsetY + zeroY;
			double rotY = -pitch + offsetP + zeroP;
			double rotZ = roll - offsetR + zeroR;
			//Console.WriteLine("R: " + roll + ", P: " + pitch + ", Y:" + yaw);

			float midPointX = (float)FDAI_centerX;
			float midPointY = (float)FDAI_centerY;
			double radius = FDAI_size / 1.7f;

			lineCollection.Clear();
			numberCollection.Clear();
			polygonCollection.Clear();
			shieldCollection.Clear();
			comboCollection = new KeyValuePair<List<FDAILine>, List<FDAIPolygon>>();

			polygonCollection = addSphere(radius, polygonCollection);

			lineCollection = addLongitudes(radius, lineCollection);
			lineCollection = addLatitudes(radius, lineCollection);

			comboCollection = addNumbers(radius, numberCollection, shieldCollection);
			numberCollection = comboCollection.Key;
			shieldCollection = comboCollection.Value;

			// BACKGROUND
			g.FillRectangle(new SolidBrush(Color.FromArgb(255, 32, 32, 32)), 0, 0, Width, Height);



			// DRAW THE FDAI BALL
			drawPolygonCollection(g, polygonCollection, rotX, rotY, rotZ, midPointX, midPointY);
			drawLineCollection(g, lineCollection, rotX, rotY, rotZ, midPointX, midPointY);
			
			drawPolygonCollection(g, shieldCollection, rotX, rotY, rotZ, midPointX, midPointY);
			drawLineCollection(g, numberCollection, rotX, rotY, rotZ, midPointX, midPointY);



			/*
			 * DRAW FIXED STUFF
			 */

			

			// HOLE SHADE
			RectangleF mask = new RectangleF(FDAI_left, FDAI_top, FDAI_size, FDAI_size);
			GraphicsPath ellipsePath = new GraphicsPath();
			ellipsePath.AddEllipse(mask);

			using (var maskBrush = new PathGradientBrush(ellipsePath))
			{
				maskBrush.CenterPoint = new PointF(mask.Width / 2f, mask.Height / 2f);
				maskBrush.CenterColor = Color.FromArgb(0, 0, 0, 0);
				maskBrush.SurroundColors = new[] { Color.FromArgb(160, 0, 0, 0) };
				maskBrush.FocusScales = new PointF(0.75f, 0.75f);
	
				g.FillRectangle(maskBrush, mask);
			}


			// CrossHairs
			drawCrosshairs(g);






			// Draw mask and outline
			float rimOffset = (maskPen.Width / 2f);
			g.DrawEllipse(maskPen, FDAI_left - rimOffset, FDAI_top - rimOffset, FDAI_size + (2 * rimOffset), FDAI_size + (2 * rimOffset));
			g.DrawEllipse(borderPen, FDAI_left, FDAI_top, FDAI_size, FDAI_size);

			drawRollScale(g, radius);
			drawRollIndicator(g, roll);

			// Error Needles
			drawErrorNeedles(g);

			// Draw 2nd mask and outline
			float rimSize = (FDAI_size * 0.08f) + (mask2Pen.Width / 2f);
			g.DrawEllipse(maskPen, FDAI_left - rimSize, FDAI_top - rimSize, FDAI_size + (2 * rimSize), FDAI_size + (2 * rimSize));
			rimSize = (FDAI_size * 0.08f);
			g.DrawEllipse(borderPen, FDAI_left - rimSize, FDAI_top - rimSize, FDAI_size + (2 * rimSize), FDAI_size + (2 * rimSize));

			// ERROR SCALES
			drawErrorScales(g);

			// Draw 3rd mask and outline
			rimSize = (FDAI_size * 0.08f * 2) + (mask2Pen.Width / 2f);
			g.DrawEllipse(mask2Pen, FDAI_left - rimSize, FDAI_top - rimSize, FDAI_size + (2 * rimSize), FDAI_size + (2 * rimSize));
			rimSize = (FDAI_size * 0.08f * 2);
			g.DrawEllipse(borderPen, FDAI_left - rimSize, FDAI_top - rimSize, FDAI_size + (2 * rimSize), FDAI_size + (2 * rimSize));


			Point[] border = new Point[5];
			border[0] = new Point(1, 1);
			border[1] = new Point(1, this.Height - 1);
			border[2] = new Point(this.Width - 1, this.Height - 1);
			border[3] = new Point(this.Width - 1, 1);
			border[4] = new Point(1, 1);

			g.DrawLines(outerBorderPen, border);

			// ROLL RATES
			drawRollRates(g);

			//g.FillPath(whiteBrush, text);
		}

		private void drawPolygonCollection(Graphics g, List<FDAIPolygon> collection, double rotX, double rotY, double rotZ, float midPointX, float midPointY)
		{
			// Rotate, clip, project, transform and Draw Polygons
			for (int i = 0; i < collection.Count; i++)
			{
				collection[i] = rotPolygon(collection[i], rotX, rotY, rotZ);
				collection[i] = clipPolygonAtZPoint(collection[i], 0);

				if (collection[i] != null)
				{
					PointF[] points2D = projectPoints(collection[i].points);
					points2D = transformPoints(points2D, midPointX, midPointY);
					if (points2D.Length > 1)
					{
						g.FillPolygon(collection[i].brush, points2D);
					}
				}
			}
		}

		private void drawLineCollection(Graphics g, List<FDAILine> collection, double rotX, double rotY, double rotZ, float midPointX, float midPointY)
		{
			// Rotate, clip, project, transform and Draw Lines
			for (int i = 0; i < collection.Count; i++)
			{
				collection[i] = rotPoints(collection[i], rotX, rotY, rotZ);
				collection[i] = clipLineAtZPoint(collection[i], 10);

				if (collection[i].points.Length > 0)
				{
					PointF[] points2D = projectPoints(collection[i].points);
					points2D = transformPoints(points2D, midPointX, midPointY);
					if (points2D.Length > 1)
					{
						g.DrawLines(collection[i].pen, points2D);
					}
				}
			}
		}
		
		private void drawCrosshairs(Graphics g)
		{
			double crosshairLength = FDAI_size * 0.075;
			double whiteHorizontalLength = FDAI_size * 0.173;
			Pen crosshairPen = new Pen(Color.FromArgb(255, 203, 147, 62), 1.75f);
			float whitePenThickness = FDAI_size * 0.020f;
			Pen whitePen = new Pen(Color.FromArgb(255, 255, 255, 255), whitePenThickness);

			// Horizontal Line
			PointF[] line = new PointF[2];

			float x1 = (float)(FDAI_centerX - crosshairLength);
			float y1 = (float)(FDAI_centerY);

			float x2 = (float)(FDAI_centerX + crosshairLength);
			float y2 = (float)(FDAI_centerY);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(crosshairPen, line);

			// Vertical Line
			line = new PointF[2];

			x1 = (float)(FDAI_centerX);
			y1 = (float)(FDAI_centerY);

			x2 = (float)(FDAI_centerX);
			y2 = (float)(FDAI_centerY + crosshairLength);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(crosshairPen, line);


			// WHITE BARS (HOLDER)
			line = new PointF[2];

			x1 = (float)(FDAI_centerX - crosshairLength + (whitePenThickness / 2));
			y1 = (float)(FDAI_centerY);

			x2 = (float)(FDAI_centerX - crosshairLength - whiteHorizontalLength);
			y2 = (float)(FDAI_centerY);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);


			line = new PointF[2];

			x1 = (float)(FDAI_centerX + crosshairLength - (whitePenThickness / 2));
			y1 = (float)(FDAI_centerY);

			x2 = (float)(FDAI_centerX + crosshairLength + whiteHorizontalLength);
			y2 = (float)(FDAI_centerY);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			// WHITE VERTICAL STUB
			line = new PointF[2];

			x1 = (float)(FDAI_centerX);
			y1 = (float)(FDAI_centerY + crosshairLength);

			x2 = (float)(FDAI_centerX);
			y2 = (float)(FDAI_centerY + crosshairLength + whitePenThickness);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			// WHITE SEMI-CIRCLE
			float x = (float)(FDAI_centerX - crosshairLength);
			float y = (float)(FDAI_centerY - crosshairLength);
			float w = (float)(crosshairLength * 2);
			float h = w;

			g.DrawArc(whitePen, x, y, w, h, 0, 180);

		}

		private void drawErrorNeedles(Graphics g)
		{
			float length = FDAI_size * 0.75f;
			float centerOffset = FDAI_size * 0.075f;
			float maxDeflection = FDAI_size * 0.337f;

			float deflectionR = (float)normalizeDeflection(errorR, errorScale);
			float deflectionP = (float)normalizeDeflection(errorP, errorScale);
			float deflectionY = (float)normalizeDeflection(errorY, errorScale);

			float needleThickness = FDAI_size * 0.0125f;
			Pen needlePen = new Pen(Color.FromArgb(255, 203, 147, 62), needleThickness);
			Pen shadowPen = new Pen(Color.FromArgb(55, 0, 0, 0), needleThickness * 2);

			// YAW NEEDLE (Bottom needle)
			PointF[] line = new PointF[2];

			float x1 = (float)(FDAI_centerX - (deflectionY * maxDeflection));
			float y1 = (float)(FDAI_centerY + centerOffset);

			float x2 = (float)(FDAI_centerX - (deflectionY * maxDeflection));
			float y2 = (float)(FDAI_centerY + length);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(shadowPen, line);
			g.DrawLines(needlePen, line);
			

			// ROLL NEEDLE (Top needle)
			line = new PointF[2];

			x1 = (float)(FDAI_centerX - (deflectionR * maxDeflection));
			y1 = (float)(FDAI_centerY - centerOffset);

			x2 = (float)(FDAI_centerX - (deflectionR * maxDeflection));
			y2 = (float)(FDAI_centerY - length);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(shadowPen, line);
			g.DrawLines(needlePen, line);

			// PITCH NEEDLE (Right needle)
			line = new PointF[2];

			x1 = (float)(FDAI_centerX + centerOffset);
			y1 = (float)(FDAI_centerY - (deflectionP * maxDeflection));

			x2 = (float)(FDAI_centerX + length);
			y2 = (float)(FDAI_centerY - (deflectionP * maxDeflection));

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(shadowPen, line);
			g.DrawLines(needlePen, line);
		}

		private double normalizeDeflection(double error, double scale)
		{
			double result = 0;
			if (Math.Abs(error) > scale)
			{
				if (error > 0)
				{
					result = 1.0f;
				}
				else
				{
					result = -1.0f;
				}
			}
			else
			{
				result = error / scale;
			}

			return result;
		}

		private void drawErrorScales(Graphics g)
		{
			float offset = (FDAI_size * 0.08f * 1);
			float scaleWidth = FDAI_size * 0.337f; // Half Scale (0 to max deflection)
			float whitePenThickness = FDAI_size * 0.020f;
			Pen whitePen = new Pen(Color.FromArgb(255, 255, 255, 255), whitePenThickness);

			float length1 = FDAI_size * 0.52f; // Full deflection
			float length2 = FDAI_size * 0.56f; // 2/3 deflection
			float length3 = FDAI_size * 0.595f; // 1/3 deflection
			float length4 = FDAI_size * 0.635f; // 0 deflection
			float length5 = FDAI_size * 0.58f; // 1/2 deflection

			GraphicsPath clipPath = new GraphicsPath();
			clipPath.AddEllipse(FDAI_left - offset, FDAI_top - offset, FDAI_size + (2 * offset), FDAI_size + (2 * offset));
			g.SetClip(clipPath, CombineMode.Exclude);

			// YAW SCALE (Bottom Scale)
			// Full deflection
			PointF[] line = new PointF[2];

			float x1 = (float)(FDAI_centerX + (1 * scaleWidth));
			float y1 = (float)(FDAI_centerY);

			float x2 = (float)(FDAI_centerX + (1 * scaleWidth));
			float y2 = (float)(FDAI_centerY + length1);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			line = new PointF[2];

			x1 = (float)(FDAI_centerX - (1 * scaleWidth));
			y1 = (float)(FDAI_centerY);

			x2 = (float)(FDAI_centerX - (1 * scaleWidth));
			y2 = (float)(FDAI_centerY + length1);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			// 2/3rds deflection
			line = new PointF[2];

			x1 = (float)(FDAI_centerX - ((2f/3f) * scaleWidth));
			y1 = (float)(FDAI_centerY);

			x2 = (float)(FDAI_centerX - ((2f/3f) * scaleWidth));
			y2 = (float)(FDAI_centerY + length2);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			line = new PointF[2];

			x1 = (float)(FDAI_centerX + ((2f / 3f) * scaleWidth));
			y1 = (float)(FDAI_centerY);

			x2 = (float)(FDAI_centerX + ((2f / 3f) * scaleWidth));
			y2 = (float)(FDAI_centerY + length2);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			// 1/3rds deflection
			line = new PointF[2];

			x1 = (float)(FDAI_centerX - ((1f / 3f) * scaleWidth));
			y1 = (float)(FDAI_centerY);

			x2 = (float)(FDAI_centerX - ((1f / 3f) * scaleWidth));
			y2 = (float)(FDAI_centerY + length3);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			line = new PointF[2];

			x1 = (float)(FDAI_centerX + ((1f / 3f) * scaleWidth));
			y1 = (float)(FDAI_centerY);

			x2 = (float)(FDAI_centerX + ((1f / 3f) * scaleWidth));
			y2 = (float)(FDAI_centerY + length3);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			// 0 deflection
			line = new PointF[2];

			x1 = (float)(FDAI_centerX);
			y1 = (float)(FDAI_centerY);

			x2 = (float)(FDAI_centerX);
			y2 = (float)(FDAI_centerY + length4);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);



			// PITCH SCALE (Right Scale)
			// Full deflection
			line = new PointF[2];

			x1 = (float)(FDAI_centerX);
			y1 = (float)(FDAI_centerY - (1 * scaleWidth));

			x2 = (float)(FDAI_centerX + length1);
			y2 = (float)(FDAI_centerY - (1 * scaleWidth));

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			line = new PointF[2];

			x1 = (float)(FDAI_centerX);
			y1 = (float)(FDAI_centerY + (1 * scaleWidth));

			x2 = (float)(FDAI_centerX + length1);
			y2 = (float)(FDAI_centerY + (1 * scaleWidth));

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			// 2/3rds deflection
			line = new PointF[2];

			x1 = (float)(FDAI_centerX);
			y1 = (float)(FDAI_centerY - ((2f / 3f) * scaleWidth));

			x2 = (float)(FDAI_centerX + length2);
			y2 = (float)(FDAI_centerY - ((2f / 3f) * scaleWidth));

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			line = new PointF[2];

			x1 = (float)(FDAI_centerX);
			y1 = (float)(FDAI_centerY + ((2f / 3f) * scaleWidth));

			x2 = (float)(FDAI_centerX + length2);
			y2 = (float)(FDAI_centerY + ((2f / 3f) * scaleWidth));

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			// 1/3rds deflection
			line = new PointF[2];

			x1 = (float)(FDAI_centerX);
			y1 = (float)(FDAI_centerY - ((1f / 3f) * scaleWidth));

			x2 = (float)(FDAI_centerX + length3);
			y2 = (float)(FDAI_centerY - ((1f / 3f) * scaleWidth));

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			line = new PointF[2];

			x1 = (float)(FDAI_centerX);
			y1 = (float)(FDAI_centerY + ((1f / 3f) * scaleWidth));

			x2 = (float)(FDAI_centerX + length3);
			y2 = (float)(FDAI_centerY + ((1f / 3f) * scaleWidth));

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			// 0 deflection
			line = new PointF[2];

			x1 = (float)(FDAI_centerX);
			y1 = (float)(FDAI_centerY);

			x2 = (float)(FDAI_centerX + length4);
			y2 = (float)(FDAI_centerY);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);



			// ROLL SCALE (Top Scale)
			// Full deflection
			line = new PointF[2];

			x1 = (float)(FDAI_centerX - (1 * scaleWidth));
			y1 = (float)(FDAI_centerY);

			x2 = (float)(FDAI_centerX - (1 * scaleWidth));
			y2 = (float)(FDAI_centerY - length1);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			line = new PointF[2];

			x1 = (float)(FDAI_centerX + (1 * scaleWidth));
			y1 = (float)(FDAI_centerY);

			x2 = (float)(FDAI_centerX + (1 * scaleWidth));
			y2 = (float)(FDAI_centerY - length1);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			// 1/2 deflection
			line = new PointF[2];

			x1 = (float)(FDAI_centerX - (0.5 * scaleWidth));
			y1 = (float)(FDAI_centerY);

			x2 = (float)(FDAI_centerX - (0.5 * scaleWidth));
			y2 = (float)(FDAI_centerY - length5);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			line = new PointF[2];

			x1 = (float)(FDAI_centerX + (0.5 * scaleWidth));
			y1 = (float)(FDAI_centerY);

			x2 = (float)(FDAI_centerX + (0.5 * scaleWidth));
			y2 = (float)(FDAI_centerY - length5);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);

			// 0 deflection
			line = new PointF[2];

			x1 = (float)(FDAI_centerX);
			y1 = (float)(FDAI_centerY);

			x2 = (float)(FDAI_centerX);
			y2 = (float)(FDAI_centerY - length4);

			line[0] = new PointF(x1, y1);
			line[1] = new PointF(x2, y2);

			g.DrawLines(whitePen, line);


			g.ResetClip();
		}
		
		private void drawRollScale(Graphics g, double radius)
		{
			float redPenWidth = (float)(2 * radius * Math.Sin(Helper.deg2rad(2.5f)));
			float rollPenWidth = (redPenWidth / 5);
			Pen rollPen = new Pen(Color.White, rollPenWidth);
			Pen redPen = new Pen(Color.FromArgb(255, 187, 0, 0), redPenWidth);

			float longLength = FDAI_size * 0.06f;
			float shortLength = FDAI_size * 0.04f;
			float redLength = FDAI_size * 0.02f;
			
			for (int i = 0; i < 360; i += 5)
			{
				GraphicsState state = g.Save();
				g.ResetTransform();
				g.TranslateTransform((float)FDAI_centerX, (float)FDAI_centerY);
				g.RotateTransform(i);

				PointF[] line = new PointF[2];
				
				// RED ZONES AROUND 90/270(0/180)
				if ((i >= 345 || i <= 10) || (i >= 165 && i <= 190))
				{
					g.RotateTransform(2.5f);
					
					line[0] = new PointF(FDAI_size / 2f, 0);
					line[1] = new PointF(FDAI_size / 2f + redLength, 0);
					
					g.DrawLines(redPen, line);
					
					g.RotateTransform(-2.5f); // just in case, keeping things in order never hurts.
				}
				
				
				line[0] = new PointF(FDAI_size / 2f, 0);
				if (i % 30 == 0)
				{
					line[1] = new PointF((FDAI_size / 2f) + longLength, 0);
				}
				else if (i % 10 == 0)
				{
					line[1] = new PointF((FDAI_size / 2f) + shortLength, 0);
				}
				else
				{
					line[1] = new PointF((FDAI_size / 2f) + redLength, 0);
				}
				
				g.DrawLines(rollPen, line);
				
				g.Restore(state);
			}
		}

		private void drawRollIndicator(Graphics g, double roll)
		{
			// DRAW ROLL ARROW
			int legAngle = 60;
			double legAngleRad = (legAngle * Math.PI)/ 180;
			double yOffset = Math.Round(FDAI_size / 12f);
			double xOffset = Math.Round(yOffset / Math.Tan(legAngleRad));
			
			GraphicsState state = g.Save();
			g.ResetTransform();
			g.TranslateTransform((float)FDAI_centerX, (float)FDAI_centerY);
			g.RotateTransform((float)(roll + 90 - offsetR));

			PointF[] arrow = new PointF[4];
			arrow[0] = new PointF(0, 0 - (FDAI_size / 2));
			arrow[1] = new PointF((int)xOffset, (int)(0 - (FDAI_size / 2) + yOffset));
			arrow[2] = new PointF((int)(xOffset * -1), (int)(0 - (FDAI_size / 2) + yOffset));
			arrow[3] = new PointF(0, 0 - (FDAI_size / 2));

			g.FillPolygon(arrowPen, arrow);

			g.Restore(state);
		}

		private void drawRollRates(Graphics g)
		{
			float length = FDAI_size;
			float height = FDAI_size * 0.125f;
			float maxDeflection = length * 0.4f;
			float whitePenThickness = FDAI_size * 0.020f;
			Pen whitePen = new Pen(Color.FromArgb(255, 255, 255, 255), whitePenThickness);

			PointF[] line;
			float x1, x2, y1, y2;

			// Normalize the rates
			float rR = (float)normalizeDeflection(rollRateR, rollRateScale);
			float rP = (float)normalizeDeflection(rollRateP, rollRateScale);
			float rY = (float)normalizeDeflection(rollRateY, rollRateScale);


			// YAW RATES (Bottom indicator)
			float x = (float)(FDAI_centerX - (length / 2f));
			float center = x + (length / 2f);
			float y = (float)(FDAI_centerY + (FDAI_size * 0.68f));
			g.FillRectangle(new SolidBrush(maskColor), x, y, length, height); // Black background
			g.FillRectangle(pureWhiteBrush, x, y + (0.4f * height), length, height * 0.6f); // White field

			

			for (int i = 5; i > 0; i--)
			{
				line = new PointF[2];
				x1 = center - ((i / 5f) * maxDeflection);
				y1 = y + height;
				x2 = center - ((i / 5f) * maxDeflection);
				y2 = y + (height * 0.3f);
				if (i == 5) y2 = y + (height * 0.15f);
				line[0] = new PointF(x1, y1);
				line[1] = new PointF(x2, y2);
				g.DrawLines(whitePen, line);

				line = new PointF[2];
				x1 = center + ((i / 5f) * maxDeflection);
				x2 = center + ((i / 5f) * maxDeflection);
				line[0] = new PointF(x1, y1);
				line[1] = new PointF(x2, y2);
				g.DrawLines(whitePen, line);
			}

			
			line = new PointF[3];
			line[0] = new PointF(center, y + (height * 0.20f));
			line[1] = new PointF(center + (length * 0.03f), y + (height * 0.42f));
			line[2] = new PointF(center - (length * 0.03f), y + (height * 0.42f));
			GraphicsPath path = new GraphicsPath();
			path.AddLines(line);
			g.FillPath(pureWhiteBrush, path);

			line = new PointF[8];
			line[0] = new PointF((float)(center - (maxDeflection * rY) + (length * 0.0075f)), y + (height * 0.42f));
			line[1] = new PointF((float)(center - (maxDeflection * rY) + (length * 0.04f)), y + (height * 0.85f));
			line[2] = new PointF((float)(center - (maxDeflection * rY) + (length * 0.01f)), y + (height * 0.85f));
			line[3] = new PointF((float)(center - (maxDeflection * rY) + (length * 0.01f)), y + (height * 1.00f));
			line[4] = new PointF((float)(center - (maxDeflection * rY) - (length * 0.01f)), y + (height * 1.00f));
			line[5] = new PointF((float)(center - (maxDeflection * rY) - (length * 0.01f)), y + (height * 0.85f));
			line[6] = new PointF((float)(center - (maxDeflection * rY) - (length * 0.04f)), y + (height * 0.85f));
			line[7] = new PointF((float)(center - (maxDeflection * rY) - (length * 0.0075f)), y + (height * 0.42f));
			path = new GraphicsPath();
			path.AddLines(line);
			g.FillPath(blackBrush, path);

			//BORDER
			g.DrawRectangle(borderPenLight, x, y, length, height);


			// ROLL RATES (Top Indicator)
			x = (float)(FDAI_centerX - (length / 2f));
			center = x + (length / 2f);
			y = (float)(FDAI_centerY - (FDAI_size * 0.68f) - height);
			g.FillRectangle(new SolidBrush(maskColor), x, y, length, height); // Black background
			g.FillRectangle(pureWhiteBrush, x, y, length, height * 0.6f); // White field



			for (int i = 5; i > 0; i--)
			{
				line = new PointF[2];
				x1 = center - ((i / 5f) * maxDeflection);
				y1 = y;
				x2 = center - ((i / 5f) * maxDeflection);
				y2 = y + (height * 0.7f);
				if (i == 5) y2 = y + (height * 0.85f);
				line[0] = new PointF(x1, y1);
				line[1] = new PointF(x2, y2);
				g.DrawLines(whitePen, line);

				line = new PointF[2];
				x1 = center + ((i / 5f) * maxDeflection);
				x2 = center + ((i / 5f) * maxDeflection);
				line[0] = new PointF(x1, y1);
				line[1] = new PointF(x2, y2);
				g.DrawLines(whitePen, line);
			}


			line = new PointF[3];
			line[0] = new PointF(center, y + (height * 0.80f));
			line[1] = new PointF(center + (length * 0.03f), y + (height * 0.58f));
			line[2] = new PointF(center - (length * 0.03f), y + (height * 0.58f));
			path = new GraphicsPath();
			path.AddLines(line);
			g.FillPath(pureWhiteBrush, path);

			line = new PointF[8];
			line[0] = new PointF((float)(center + (maxDeflection * rR) + (length * 0.0075f)), y + (height * 0.58f));
			line[1] = new PointF((float)(center + (maxDeflection * rR) + (length * 0.04f)), y + (height * 0.15f));
			line[2] = new PointF((float)(center + (maxDeflection * rR) + (length * 0.01f)), y + (height * 0.15f));
			line[3] = new PointF((float)(center + (maxDeflection * rR) + (length * 0.01f)), y + (height * 0.00f));
			line[4] = new PointF((float)(center + (maxDeflection * rR) - (length * 0.01f)), y + (height * 0.00f));
			line[5] = new PointF((float)(center + (maxDeflection * rR) - (length * 0.01f)), y + (height * 0.15f));
			line[6] = new PointF((float)(center + (maxDeflection * rR) - (length * 0.04f)), y + (height * 0.15f));
			line[7] = new PointF((float)(center + (maxDeflection * rR) - (length * 0.0075f)), y + (height * 0.58f));
			path = new GraphicsPath();
			path.AddLines(line);
			g.FillPath(blackBrush, path);

			//BORDER
			g.DrawRectangle(borderPenLight, x, y, length, height);


			// PITCH RATE (Right Indicator)
			GraphicsState state = g.Save();
			g.TranslateTransform(Width / 2f, Height / 2f);
			g.RotateTransform(90);
			g.TranslateTransform(-Width / 2f, -Height / 2f);

			x = (float)(FDAI_centerX - (length / 2f));
			center = x + (length / 2f);
			y = (float)(FDAI_centerY - (FDAI_size * 0.68f) - height);
			g.FillRectangle(new SolidBrush(maskColor), x, y, length, height); // Black background
			g.FillRectangle(pureWhiteBrush, x, y, length, height * 0.6f); // White field



			for (int i = 5; i > 0; i--)
			{
				line = new PointF[2];
				x1 = center - ((i / 5f) * maxDeflection);
				y1 = y;
				x2 = center - ((i / 5f) * maxDeflection);
				y2 = y + (height * 0.7f);
				if (i == 5) y2 = y + (height * 0.85f);
				line[0] = new PointF(x1, y1);
				line[1] = new PointF(x2, y2);
				g.DrawLines(whitePen, line);

				line = new PointF[2];
				x1 = center + ((i / 5f) * maxDeflection);
				x2 = center + ((i / 5f) * maxDeflection);
				line[0] = new PointF(x1, y1);
				line[1] = new PointF(x2, y2);
				g.DrawLines(whitePen, line);
			}


			line = new PointF[3];
			line[0] = new PointF(center, y + (height * 0.80f));
			line[1] = new PointF(center + (length * 0.03f), y + (height * 0.58f));
			line[2] = new PointF(center - (length * 0.03f), y + (height * 0.58f));
			path = new GraphicsPath();
			path.AddLines(line);
			g.FillPath(pureWhiteBrush, path);

			line = new PointF[8];
			line[0] = new PointF((float)(center + (maxDeflection * rP) + (length * 0.0075f)), y + (height * 0.58f));
			line[1] = new PointF((float)(center + (maxDeflection * rP) + (length * 0.04f)), y + (height * 0.15f));
			line[2] = new PointF((float)(center + (maxDeflection * rP) + (length * 0.01f)), y + (height * 0.15f));
			line[3] = new PointF((float)(center + (maxDeflection * rP) + (length * 0.01f)), y + (height * 0.00f));
			line[4] = new PointF((float)(center + (maxDeflection * rP) - (length * 0.01f)), y + (height * 0.00f));
			line[5] = new PointF((float)(center + (maxDeflection * rP) - (length * 0.01f)), y + (height * 0.15f));
			line[6] = new PointF((float)(center + (maxDeflection * rP) - (length * 0.04f)), y + (height * 0.15f));
			line[7] = new PointF((float)(center + (maxDeflection * rP) - (length * 0.0075f)), y + (height * 0.58f));
			path = new GraphicsPath();
			path.AddLines(line);
			g.FillPath(blackBrush, path);

			//BORDER
			g.DrawRectangle(borderPenLight, x, y, length, height);

			g.Restore(state);
		}

		private KeyValuePair<List<FDAILine>, List<FDAIPolygon>> addNumbers(double radius, List<FDAILine> lineCollection, List<FDAIPolygon> polyCollection)
		{
			KeyValuePair<List<FDAILine>, List<FDAIPolygon>> output = new KeyValuePair<List<FDAILine>, List<FDAIPolygon>>(lineCollection, polyCollection);
			output = addLatNumbers(radius, output);
			output = addLonNumbers(radius, output);
			return output;
		}
		
		private KeyValuePair<List<FDAILine>, List<FDAIPolygon>> addLatNumbers(double radius, KeyValuePair<List<FDAILine>, List<FDAIPolygon>> collection)
		{
			KeyValuePair<List<FDAILine>, List<FDAIPolygon>> output = collection;

			// Draw Latitude numbers
			int[] latDeg = { -60, -30, 30, 60 };
			List<int[]> latNum = new List<int[]>{new int[]{ 3, 0 }, new int[]{ 3, 3 }, new int[]{ 3 }, new int[]{ 6 } };
			double xDeg = 2.5;
			double yDeg = 5;
			double xSize = (Math.PI * xDeg * radius) / 180f;
			double ySize = (Math.PI * yDeg * radius) / 180f;
			double rX;
			double rY;
			
			for (int i = 0; i < latDeg.Length; i++)
			{
				for (int j = 15; j < 360; j += 30)
				{
					
					for (int n = 0; n < latNum[i].Length; n++)
					{
						FDAILine digit = new FDAILine(0);
						digit.points = getDigitPoints(latNum[i][n], xSize, ySize, radius);
						
						digit = rotPoints(digit, 0, 90, 0);

						rY = j + (yDeg / 2f);
						rX = latDeg[i] - (xDeg * (latNum[i].Length / 2f)) + (n * xDeg);
						
						digit = rotPoints(digit, rX, rY, 0);
						
						if (j > 180)
						{
							digit.pen = whiteNumberPen;
						}
						else
						{
							digit.pen = blackNumberPen;
						}

						output.Key.Add(digit);
					}

					// Make background polygon
					FDAIPolygon shield = new FDAIPolygon(4);
					shield.points[0] = new Tuple<double, double, double>(-(xSize * 1.25 * (latNum[i].Length / 2f)), 1.25 * (ySize / 2f), radius);
					shield.points[1] = new Tuple<double, double, double>((xSize * 1.25 * (latNum[i].Length / 2f)), 1.25 * (ySize / 2f), radius);
					shield.points[2] = new Tuple<double, double, double>((xSize * 1.25 * (latNum[i].Length / 2f)), -1.25 * (ySize / 2f), radius);
					shield.points[3] = new Tuple<double, double, double>(-(xSize * 1.25 * (latNum[i].Length / 2f)), -1.25 * (ySize / 2f), radius);
					
					shield = rotPolygon(shield, 0, 90, 0);

					rY = j;
					rX = latDeg[i];// - (xDeg * (latNum[i].Length / 2f));
					
					shield = rotPolygon(shield, rX, rY, 0);

					if (j > 180)
					{
						shield.brush = blackBrush;
					}
					else
					{
						shield.brush = whiteBrush;
					}

					output.Value.Add(shield);
				}
				
			}

			return output;
		}
		
		private KeyValuePair<List<FDAILine>, List<FDAIPolygon>> addLonNumbers(double radius, KeyValuePair<List<FDAILine>, List<FDAIPolygon>> collection)
		{
			KeyValuePair<List<FDAILine>, List<FDAIPolygon>> output = collection;

			// Draw Longitude numbers
			int[] latDeg = { -45, -15, 15, 45 };
			int[] lonDeg = { 0, 30, 60, 90, 120, 150, 180, 210, 240, 270, 300, 330 };
			List<int[]> lonNum = new List<int[]>{
				new int[]{ 0 },
				new int[]{ 3, 0 },
				new int[]{ 6, 0 },
				new int[]{ 9, 0 },
				new int[]{ 1, 2, 0 },
				new int[]{ 1, 5, 0 },
				new int[]{ 1, 8, 0 },
				new int[]{ 2, 1, 0 },
				new int[]{ 2, 4, 0 },
				new int[]{ 2, 7, 0 },
				new int[]{ 3, 0, 0 },
				new int[]{ 3, 3, 0 }
			};
			double xDeg = 2.5;
			double yDeg = 5;
			double xSize = (Math.PI * xDeg * radius) / 180f;
			double ySize = (Math.PI * yDeg * radius) / 180f;
			double rX;
			double rY;
			
			for (int i = 0; i < latDeg.Length; i++)
			{
				for (int j = 0; j < lonDeg.Length; j ++)
				{

					for (int n = 0; n < lonNum[j].Length; n++)
					{
						FDAILine digit = new FDAILine(0);
						digit.points = getDigitPoints(lonNum[j][n], xSize, ySize, radius);

						digit = rotPoints(digit, 0, 90, 0);

						rY = lonDeg[j] + (yDeg / 2f);
						rX = latDeg[i] - (xDeg * (lonNum[j].Length / 2f)) + (n * xDeg);

						digit = rotPoints(digit, rX, rY, 0);

						if (lonDeg[j] > 180)
						{
							digit.pen = whiteNumberPen;
						}
						else
						{
							digit.pen = blackNumberPen;
						}

						output.Key.Add(digit);
					}
					
					// Make background polygon
					FDAIPolygon shield = new FDAIPolygon(4);
					shield.points[0] = new Tuple<double, double, double>(-(xSize * 1.25 * (lonNum[j].Length / 2f)), 1.25 * (ySize / 2f), radius);
					shield.points[1] = new Tuple<double, double, double>((xSize * 1.25 * (lonNum[j].Length / 2f)), 1.25 * (ySize / 2f), radius);
					shield.points[2] = new Tuple<double, double, double>((xSize * 1.25 * (lonNum[j].Length / 2f)), -1.25 * (ySize / 2f), radius);
					shield.points[3] = new Tuple<double, double, double>(-(xSize * 1.25 * (lonNum[j].Length / 2f)), -1.25 * (ySize / 2f), radius);
					
					shield = rotPolygon(shield, 0, 90, 0);

					rY = lonDeg[j];
					rX = latDeg[i];// - (xDeg * (latNum[i].Length / 2f));
					
					shield = rotPolygon(shield, rX, rY, 0);

					if (lonDeg[j] > 180)
					{
						shield.brush = blackBrush;
					}
					else
					{
						shield.brush = whiteBrush;
					}

					output.Value.Add(shield);
				}
				
			}

			return output;
		}

		private List<FDAIPolygon> addSphere(double radius, List<FDAIPolygon> collection)
		{
			List<FDAIPolygon> output = collection;
			radius = radius * 0.999;

			// Fill sphere
			double polygonBleed = 0.15; 
			for (int i = 70; i >= -75; i -= 5)
			{
				for (int j = 0; j < 360; j += 5)
				{
					FDAIPolygon polygon = new FDAIPolygon(4);
					if (j >= 180)
					{
						polygon.brush = blackBrush;
					}
					else
					{
						polygon.brush = whiteBrush;
					}

					double r1 = i + polygonBleed;
					double r2 = i - polygonBleed;
					double p1 = j + polygonBleed;
					double p2 = j - polygonBleed;

					double x = (radius * Math.Sin(Helper.deg2rad(p2))) * Math.Cos(Helper.deg2rad(r2));
					double y = radius * Math.Sin(Helper.deg2rad(r2));
					double z = (radius * Math.Cos(Helper.deg2rad(r2))) * Math.Cos(Helper.deg2rad(p2));
					polygon.points[0] = new Tuple<double, double, double>(x, y, z);

					x = (radius * Math.Sin(Helper.deg2rad(p2)))  * Math.Cos(Helper.deg2rad(r1 + 5));
					y = radius * Math.Sin(Helper.deg2rad(r1 + 5));
					z = (radius * Math.Cos(Helper.deg2rad(r1 + 5))) * Math.Cos(Helper.deg2rad(p2));
					polygon.points[1] = new Tuple<double, double, double>(x, y, z);

					x = (radius * Math.Sin(Helper.deg2rad(p1 + 5))) * Math.Cos(Helper.deg2rad(r1 + 5));
					y = radius * Math.Sin(Helper.deg2rad(r1 + 5));
					z = (radius * Math.Cos(Helper.deg2rad(r2 + 5))) * Math.Cos(Helper.deg2rad(p1 + 5));
					polygon.points[2] = new Tuple<double, double, double>(x, y, z);

					x = (radius * Math.Sin(Helper.deg2rad(p1 + 5))) * Math.Cos(Helper.deg2rad(r2));
					y = radius * Math.Sin(Helper.deg2rad(r2));
					z = (radius * Math.Cos(Helper.deg2rad(r2))) * Math.Cos(Helper.deg2rad(p1 + 5));
					polygon.points[3] = new Tuple<double, double, double>(x, y, z);

					output.Add(polygon);
				}
			}

			// Create the Gimbal-Lock areas
			for (int i = 75; i <= 85; i += 5)
			{
				for (int j = 0; j < 360; j += 5)
				{
					FDAIPolygon polygon = new FDAIPolygon(4);
					polygon.brush = redBrush;
					

					double r1 = i + polygonBleed;
					double r2 = i - polygonBleed;
					double p1 = j + polygonBleed;
					double p2 = j - polygonBleed;

					double x = (radius * Math.Sin(Helper.deg2rad(p2))) * Math.Cos(Helper.deg2rad(r2));
					double y = radius * Math.Sin(Helper.deg2rad(r2));
					double z = (radius * Math.Cos(Helper.deg2rad(r2))) * Math.Cos(Helper.deg2rad(p2));
					polygon.points[0] = new Tuple<double, double, double>(x, y, z);

					x = (radius * Math.Sin(Helper.deg2rad(p2)))  * Math.Cos(Helper.deg2rad(r1 + 5));
					y = radius * Math.Sin(Helper.deg2rad(r1 + 5));
					z = (radius * Math.Cos(Helper.deg2rad(r1 + 5))) * Math.Cos(Helper.deg2rad(p2));
					polygon.points[1] = new Tuple<double, double, double>(x, y, z);

					x = (radius * Math.Sin(Helper.deg2rad(p1 + 5))) * Math.Cos(Helper.deg2rad(r1 + 5));
					y = radius * Math.Sin(Helper.deg2rad(r1 + 5));
					z = (radius * Math.Cos(Helper.deg2rad(r2 + 5))) * Math.Cos(Helper.deg2rad(p1 + 5));
					polygon.points[2] = new Tuple<double, double, double>(x, y, z);

					x = (radius * Math.Sin(Helper.deg2rad(p1 + 5))) * Math.Cos(Helper.deg2rad(r2));
					y = radius * Math.Sin(Helper.deg2rad(r2));
					z = (radius * Math.Cos(Helper.deg2rad(r2))) * Math.Cos(Helper.deg2rad(p1 + 5));
					polygon.points[3] = new Tuple<double, double, double>(x, y, z);

					output.Add(polygon);
				}
			}
			
			for (int i = -80; i >= -90; i -= 5)
			{
				for (int j = 0; j < 360; j += 5)
				{
					FDAIPolygon polygon = new FDAIPolygon(4);
					polygon.brush = redBrush;
					

					double r1 = i + polygonBleed;
					double r2 = i - polygonBleed;
					double p1 = j + polygonBleed;
					double p2 = j - polygonBleed;

					double x = (radius * Math.Sin(Helper.deg2rad(p2))) * Math.Cos(Helper.deg2rad(r2));
					double y = radius * Math.Sin(Helper.deg2rad(r2));
					double z = (radius * Math.Cos(Helper.deg2rad(r2))) * Math.Cos(Helper.deg2rad(p2));
					polygon.points[0] = new Tuple<double, double, double>(x, y, z);

					x = (radius * Math.Sin(Helper.deg2rad(p2)))  * Math.Cos(Helper.deg2rad(r1 + 5));
					y = radius * Math.Sin(Helper.deg2rad(r1 + 5));
					z = (radius * Math.Cos(Helper.deg2rad(r1 + 5))) * Math.Cos(Helper.deg2rad(p2));
					polygon.points[1] = new Tuple<double, double, double>(x, y, z);

					x = (radius * Math.Sin(Helper.deg2rad(p1 + 5))) * Math.Cos(Helper.deg2rad(r1 + 5));
					y = radius * Math.Sin(Helper.deg2rad(r1 + 5));
					z = (radius * Math.Cos(Helper.deg2rad(r2 + 5))) * Math.Cos(Helper.deg2rad(p1 + 5));
					polygon.points[2] = new Tuple<double, double, double>(x, y, z);

					x = (radius * Math.Sin(Helper.deg2rad(p1 + 5))) * Math.Cos(Helper.deg2rad(r2));
					y = radius * Math.Sin(Helper.deg2rad(r2));
					z = (radius * Math.Cos(Helper.deg2rad(r2))) * Math.Cos(Helper.deg2rad(p1 + 5));
					polygon.points[3] = new Tuple<double, double, double>(x, y, z);

					output.Add(polygon);
				}
			}

			return output;
		}

		private List<FDAILine> addLongitudes(double radius, List<FDAILine> collection)
		{
			List<FDAILine> output = collection;
			
			int[] lonLines = { 0, 30, 60, 90, 120, 150, 180, 210, 240, 270, 300, 330};
			int[] intLines = { 15, 45, 75, 105, 135, 165, 195, 225, 255, 285, 315, 345 };
			int[] intLinesLat = { 5, 10, 15, 20, 25, 35, 40, 45, 50, 55, 65, 70, 75, 80, 85,
								95, 100, 105, 110, 115, 125, 130, 135, 140, 145, 155, 160, 165, 170, 175,
								185, 190, 195, 200, 205, 215, 220, 225, 230, 235, 245, 250, 255, 260, 265,
								275, 280, 285, 290, 295, 305, 310, 315, 320, 325, 335, 340, 345, 350, 355 };
			double intLinesSize = (Math.PI * 2 * radius) / 180f;
			
			double resolution = 5; // degress between points on the line
			int numberOfPointsPrLine = (int)Math.Floor(90 / resolution);

			for (int j = 0; j < lonLines.Length; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					FDAILine line = new FDAILine(numberOfPointsPrLine + 1);
					for (int i = 0; i <= numberOfPointsPrLine; i++)
					{
						double x = radius * Math.Sin(Helper.deg2rad((resolution * i) + (k * 90)));
						double y = radius * Math.Cos(Helper.deg2rad((resolution * i) + (k * 90)));
						double z = 0;
						line.points[i] = new Tuple<double, double, double>(x, y, z);
					}

					line = rotPoints(line, 90, lonLines[j], 0);

					if (lonLines[j] >= 90 && lonLines[j] < 270)
					{
						line.pen = new Pen(Color.FromArgb(255, 255, 255, 255), 1f);
					}
					else
					{
						line.pen = new Pen(Color.FromArgb(255, 32, 32, 32), 1f);
					}

					collection.Add(line);
				}
			}
			
			// INTERMIDIARY LINE STUBS
			for (int i = 0; i < intLines.Length; i++)
			{
				for (int j = 0; j < intLinesLat.Length; j++)
				{
					FDAILine line = new FDAILine(2);
					double y1 = 0 - intLinesSize / 2;
					double y2 = 0 + intLinesSize / 2;
					double x = 0;
					double z = radius * Math.Cos(Helper.deg2rad(1));
					line.points[0] = new Tuple<double, double, double>(x, y1, z);
					line.points[1] = new Tuple<double, double, double>(x, y2, z);

					// Rotate intermidiary line into place
					line = rotPoints(line, intLines[i], intLinesLat[j], 0);

					if (intLinesLat[j] > 180)
					{
						line.pen = new Pen(Color.FromArgb(255, 255, 255, 255), 1f);
					}
					else
					{
						line.pen = new Pen(Color.FromArgb(255, 32, 32, 32), 1f);
					}
					
					collection.Add(line);
				}
			}

			return collection;
		}
		
		private List<FDAILine> addLatitudes(double radius, List<FDAILine> collection)
		{
			List<FDAILine> output = collection;
			int[] latLines = { -85, -75, -60, -30, 0, 30, 60, 75, 85};
			int[] intLines = { -80, -70, -65, -55, -50, -45, -40, -35, -25, -20, -15, -10, -5, 5, 10, 15, 20, 25, 35, 40, 45, 50, 55, 65, 70, 80 };
			int[] intLinesRad = { 15, 45, 75, 105, 135, 165, 195, 225, 255, 285, 315, 345 };
			double intLinesSize = (Math.PI * 2 * radius) / 180f;
			double resolution = 5; // degress between points on the line
			int numberOfPointsPrLine = (int)Math.Floor(180 / resolution);
			// We draw the lines as two semi-circles to get better result later after z-cut


			for (int j = 0; j < latLines.Length; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					double currentRadius = radius * Math.Cos(Helper.deg2rad(latLines[j]));
					FDAILine line = new FDAILine(numberOfPointsPrLine + 1);
					for (int i = 0; i <= numberOfPointsPrLine; i++)
					{
						double x = currentRadius * Math.Sin(Helper.deg2rad((resolution * i) + (k * 180)));
						double y = currentRadius * Math.Cos(Helper.deg2rad((resolution * i) + (k * 180)));
						double z = 0;
						line.points[i] = new Tuple<double, double, double>(x, y, z);
					}

					line = rotPoints(line, 0, 0, 0);
					float yTranform = (float)(radius * Math.Sin(Helper.deg2rad(latLines[j])));
					line = transform3DLine(line, 0, yTranform, 0);

					if (latLines[j] == 0)
					{
						FDAILine line0 = new FDAILine(line);
						line0.pen = blackCenterPen;
						collection.Add(line0);
						
						line0 = new FDAILine(line);
						line0.pen = grayCenterPen;
						collection.Add(line0);

						line.pen = whiteCenterPen;
					}
					else if (k != 0)
					{
						line.pen = whiteNumberPen;
					}
					else
					{
						line.pen = blackNumberPen;
					}

					collection.Add(line);
				}
			}

			// INTERMIDIARY LINE STUBS
			for (int i = 0; i < intLinesRad.Length; i++)
			{
				for (int j = 0; j < intLines.Length; j++)
				{
					FDAILine line = new FDAILine(2);
					double x1 = 0 - intLinesSize / 2;
					double x2 = 0 + intLinesSize / 2;
					double y = 0;
					double z = radius;
					line.points[0] = new Tuple<double, double, double>(x1, y, z);
					line.points[1] = new Tuple<double, double, double>(x2, y, z);

					// Rotate intermidiary line into place
					line = rotPoints(line, intLines[j] + 90, intLinesRad[i], 0);
					
					if (intLinesRad[i] <= 180)
					{
						line.pen = new Pen(Color.FromArgb(255, 255, 255, 255), 1f);
					}
					else
					{
						line.pen = new Pen(Color.FromArgb(255, 0, 0, 0), 1f);
					}
					
					collection.Add(line);
				}
			}
			
			return collection;
		}
		
		private FDAIPolygon clipPolygonAtZPoint(FDAIPolygon polygon, float z)
		{
			for (int i = 0; i < polygon.points.Length; i++)
			{
				if (polygon.points[i].Item3 >= z)
				{
					return polygon;
				}
			}
			return null;
		}

		private FDAILine clipLineAtZPoint(FDAILine line, float z)
		{
			line.points = clipAtZPoint(line.points, 0);
			return line;
		}

		private Tuple<double, double, double>[] clipAtZPoint(Tuple<double, double, double>[] points, float z)
		{
			List<Tuple<double, double, double>> tmp = new List<Tuple<double, double, double>>();
			for (int i = 0; i < points.Length; i++)
			{
				if (points[i].Item3 >= 0)
				{
					tmp.Add(points[i]);
				}
			}

			Tuple<double, double, double>[] output = tmp.ToArray();
			return output;
		}

		private PointF[] transformPoints(PointF[] points, float x, float y)
		{
			PointF[] output = new PointF[points.Length];
			for (int i = 0; i < points.Length; i++)
			{
				output[i].X = points[i].X + x;
				output[i].Y = points[i].Y + y;
			}
			return output;
		}

		private FDAILine transform3DLine(FDAILine line, float x, float y, float z)
		{
			line.points = transform3DPoints(line.points, x, y, z);
			return line;
		}
		
		private Tuple<double, double, double>[] transform3DPoints(Tuple<double, double, double>[] points, float x, float y, float z)
		{
			Tuple<double, double, double>[] output = new Tuple<double, double, double>[points.Length];
			for (int i = 0; i < points.Length; i++)
			{
				double nX = points[i].Item1 + x;
				double nY = points[i].Item2 + y;
				double nZ = points[i].Item3 + z;
				output[i] = new Tuple<double, double, double>(nX, nY, nZ);
			}
			return output;
		}

		private FDAIPolygon rotPolygon(FDAIPolygon polygon, double rotX, double rotY, double rotZ)
		{
			FDAIPolygon output = new FDAIPolygon(polygon.points.Length);
			output.brush = polygon.brush;

			for (int i = 0; i < polygon.points.Length; i++)
			{
				output.points[i] = rotPoint(polygon.points[i], rotX, rotY, rotZ);
			}

			return output;
		}

		private FDAILine rotPoints(FDAILine points, double rotX, double rotY, double rotZ)
		{
			FDAILine output = new FDAILine(points.points.Length);

			for (int i = 0; i < points.points.Length; i++)
			{
				output.points[i] = rotPoint(points.points[i], rotX, rotY, rotZ);
			}
			output.pen = points.pen;

			return output;
		}

		/**
		 * Rotation order is X,Y,Z
		 */
		private Tuple<double, double, double> rotPoint(Tuple<double, double, double> point, double rotX, double rotY, double rotZ)
		{
			double x;
			double y;
			double z;
			
			// Rotate around X-axis
			x = point.Item1;
			y = (point.Item2 * Math.Sin(Helper.deg2rad(rotX))) - (point.Item3 * Math.Cos(Helper.deg2rad(rotX)));
			z = (point.Item2 * Math.Cos(Helper.deg2rad(rotX))) + (point.Item3 * Math.Sin(Helper.deg2rad(rotX)));
			point = new Tuple<double, double, double>(x, y, z);
			
			// Rotate around Y-axis
			x = (point.Item1 * Math.Cos(Helper.deg2rad(rotY))) - (point.Item3 * Math.Sin(Helper.deg2rad(rotY)));
			y = point.Item2;
			z = (point.Item1 * Math.Sin(Helper.deg2rad(rotY))) + (point.Item3 * Math.Cos(Helper.deg2rad(rotY)));
			point = new Tuple<double, double, double>(x, y, z);

			// Rotate around Z-axis
			x = (point.Item1 * Math.Cos(Helper.deg2rad(rotZ))) - (point.Item2 * Math.Sin(Helper.deg2rad(rotZ)));
			y = (point.Item1 * Math.Sin(Helper.deg2rad(rotZ))) + (point.Item2 * Math.Cos(Helper.deg2rad(rotZ)));
			z = point.Item3;
			point = new Tuple<double, double, double>(x, y, z);

			return point;
		}

		private PointF[] projectPoints(Tuple<double, double, double>[] points)
		{
			PointF[] output = new PointF[points.Length];
			for (int i = 0; i < points.Length; i++)
			{
				output[i] = projectPoint(points[i]);
			}
			return output;
		}
		
		private PointF projectPoint(Tuple<double, double, double> point)
		{
			// Convert 3D-points to 2D-points
			double x = point.Item1;
			double y = point.Item2;
			PointF point2D = new PointF((float)x, (float)y);			
			return point2D;
		}
		

		private Tuple<double, double, double> Q2V(Tuple<double, double, double, double> q)
		{
			Tuple<double, double, double, double> R = new Tuple<double, double, double, double>(q.Item4, q.Item1, q.Item2, q.Item3); // Rotation to be applied
			Tuple<double, double, double, double> P = baseDirection;
 			Tuple<double, double, double, double> Rc = new Tuple<double, double, double, double>(q.Item4, -q.Item1, -q.Item2, -q.Item3); // R-conjugate R'

			// The new vector
			Tuple<double, double, double, double> Pc = H(H(R, P), Rc);

			return new Tuple<double, double, double>(Pc.Item2, Pc.Item3, Pc.Item4);
		}

		private Tuple<double, double, double, double> H(Tuple<double, double, double, double> a, Tuple<double, double, double, double> b)
		{
			return HamiltonProduct(a, b);
		}
		
		private Tuple<double, double, double, double> HamiltonProduct(Tuple<double, double, double, double> a, Tuple<double, double, double, double> b)
		{
			// r = b, q = a
			double w = b.Item1 * a.Item1 - b.Item2 * a.Item2 - b.Item3 * a.Item3 - b.Item4 * a.Item4;
			double x = b.Item1 * a.Item2 + b.Item2 * a.Item1 - b.Item3 * a.Item4 + b.Item4 * a.Item3;
			double y = b.Item1 * a.Item3 + b.Item2 * a.Item4 + b.Item3 * a.Item1 - b.Item4 * a.Item2;
			double z = b.Item1 * a.Item4 - b.Item2 * a.Item3 + b.Item3 * a.Item2 + b.Item4 * a.Item1;

			return new Tuple<double, double, double, double>(w, x, y, z);
			
			/*
		return [r[0]*q[0]-r[1]*q[1]-r[2]*q[2]-r[3]*q[3],
            r[0]*q[1]+r[1]*q[0]-r[2]*q[3]+r[3]*q[2],
            r[0]*q[2]+r[1]*q[3]+r[2]*q[0]-r[3]*q[1],
            r[0]*q[3]-r[1]*q[2]+r[2]*q[1]+r[3]*q[0]]
            */
		}

		private void setRotFromVector(Tuple<double, double, double, double> q)
		{
			double w = q.Item1;
			double x = q.Item2;
			double y = q.Item3;
			double z = q.Item4;
			
			roll  = (float)Helper.rad2deg(Math.Atan2(2*y*w + 2*x*z, 1 - 2*y*y - 2*z*z));
			pitch = (float)Helper.rad2deg(Math.Atan2(2*x*w + 2*y*z, 1 - 2*x*x - 2*z*z));
			yaw   = (float)Helper.rad2deg(Math.Asin(2*x*y + 2*z*w));
		}

		private void setRotFromQuaternion(Tuple<double, double, double, double> q)
		{
			// 1:X, 2:Y, 3:Z, 4:W
			/*
			 * kRPC-angles are XYZW, but reference frame is different.
			 * 
			 *  Euler   kRPC
			 *    X      Y
			 *    Y      Z
			 *    Z      X
			 */
			double x = q.Item2;
			double y = q.Item3;
			double z = q.Item1;
			double w = q.Item4;

			double a = 0;
			double b = 0;
			double c = 0;
			double d = 0;
			double e = 0;

			

			String order = "wiki";

			switch (order)
			{

				case "ZYX":
					a = 2 * (x * y + w * z);
					b = w * w + x * x - y * y - z * z;
					c = -2 * (x * z - w * y);
					d = 2 * (y * z + w * x);
					e = w * w - x * x - y * y + z * z;
					break;
				case "ZXY":
					a = -2 * (x * y - w * z);
					b = w * w - x * x + y * y - z * z;
					c = 2 * (y * z + w * x);
					d = -2 * (x * z - w * y);
					e = w * w - x * x - y * y + z * z;
					break;
				case "XYZ":
					a = -2 * (y * z - w * x);
					b = w * w - x * x - y * y + z * z;
					c = 2 * (x * z + w * y);
					d = -2 * (x * y - w * z);
					e = w * w + x * x - y * y - z * z;
					break;
				case "XZY":
					a = 2 * (y * z + w * x);
					b = w * w - x * x + y * y - z * z;
					c = -2 * (x * y - w * z);
					d = 2 * (x * z + w * y);
					e = w * w + x * x - y * y - z * z;
					break;
				case "YXZ":
					a = 2 * (x * z + w * y);
					b = w * w - x * x - y * y + z * z;
					c = -2 * (y * z - w * x);
					d = 2 * (x * y + w * z);
					e = w * w - x * x + y * y - z * z;
					break;
				case "wiki":
					a = 2 * ((w * x) + (y * z));
					b = 1 - (2 * ((x * x) + (y * y)));
					c = 2 * ((w * y) - (z * x));
					d = 2 * ((w * z) + (x * y));
					e = 1 - (2 * ((y * y) + (z * z)));
					break;
			}

			// OLD STYLE
			/*
			roll = Helper.rad2deg(Math.Atan2(d, e)); // Z
			pitch = Helper.rad2deg(Math.Asin(c)); // Y
			yaw = Helper.rad2deg(Math.Atan2(a, b)); // X
			/**/

			// WIKI STYLE
			/**/
			roll = Helper.rad2deg(Math.Atan2(a, b)); // X
			pitch = Helper.rad2deg(Math.Asin(c)); // Y
			yaw = Helper.rad2deg(Math.Atan2(d, e)); // Z
			/**/

			// CHECK FOR OUT OF BOUNDS PITCH
			if(Double.IsNaN(pitch))
			{
				if(c < -1)
				{
					pitch = -90f;
				}
				else
				{
					pitch = 90f;
				}
			}


			/*
			yaw = Helper.rad2deg(Math.Atan2(d, e)); // X
			pitch = Helper.rad2deg(Math.Asin(c)); // Y
			roll = Helper.rad2deg(Math.Atan2(a, b)); // Z
			/*
			threeaxisrot( 2*(q.x*q.y + q.w*q.z),
                     q.w*q.w + q.x*q.x - q.y*q.y - q.z*q.z,
                    -2*(q.x*q.z - q.w*q.y),
                     2*(q.y*q.z + q.w*q.x),
                     q.w*q.w - q.x*q.x - q.y*q.y + q.z*q.z,
                     res);
			res[0] = atan2( r31, r32 );
  			res[1] = asin ( r21 );
  			res[2] = atan2( r11, r12 );
			*/
		}
	}
}