using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KSP_MOCR
{
	partial class FDAI : Label
	{
		private float pitch = 0;
		private float roll = -10;
		private float yaw = 0;
		
		double zeroX = 0;
		double zeroY = 90;
		double zeroZ = 0;
		
		
		int FDAI_size;
		int FDAI_left;
		int FDAI_top;
		double FDAI_centerX;
		double FDAI_centerY;

		readonly Pen borderPen = new Pen(Color.FromArgb(255, 96, 96, 96), 1.0f);
		readonly Pen FDAIPen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.0f);
		readonly Pen crosshairPen = new Pen(Color.FromArgb(255, 203, 147, 62), 1.75f);
		
		readonly Pen grayCenterPen = new Pen(Color.FromArgb(255, 64, 64, 64), 10f);
		readonly Pen blackCenterPen = new Pen(Color.FromArgb(255, 32, 32, 21), 8f);
		readonly Pen whiteCenterPen = new Pen(Color.FromArgb(255, 253, 250, 250), 2f);
		
		readonly Brush skyPen = new SolidBrush(Color.FromArgb(255, 0, 101, 204));
		readonly Brush groundPen = new SolidBrush(Color.FromArgb(255, 101, 51, 0));
		readonly Brush arrowPen = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
		
		
		
		readonly Brush whiteBrush = new SolidBrush(Color.FromArgb(255, 200, 204, 194));
		readonly Brush blackBrush = new SolidBrush(Color.FromArgb(255, 16, 16, 16));
		readonly Brush redBrush = new SolidBrush(Color.FromArgb(255, 180, 28, 25));
		
		readonly Pen whiteNumberPen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.0f);
		readonly Pen blackNumberPen = new Pen(Color.FromArgb(200, 32, 32, 32), 1.0f);

		
		readonly Pen maskPen = new Pen(Color.FromArgb(255, 0, 0, 0), 200);
		readonly Pen mask2Pen = new Pen(Color.FromArgb(255, 32, 32, 32), 200);
		
		readonly Pen outerBorderPen = new Pen(Color.FromArgb(255, 96, 96, 96), 2.0f);

		readonly Color yawNumbers = Color.FromArgb(200, 255, 255, 255);

		public FDAI()
		{
            this.DoubleBuffered = true;
		}

		public void setAttitude(float roll, float pitch, float yaw)
		{
			this.pitch = pitch;
			this.roll = roll;
			this.yaw = yaw;
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			paintFDAI(e, g);
			//paintPFD(e, g);
		}

		private void paintFDAI(PaintEventArgs e, Graphics g)
		{
			/**
			 * THIS IS THE FLIGHT DIRECTOR ATTITUDE INDICATOR (FDAI) TYPE
			 **/

			if (this.Width > this.Height)
			{
				FDAI_size = this.Height - 50;
				FDAI_left = ((this.Width - this.Height) / 2) + 25;
				FDAI_top = 25;
			}
			else
			{
				FDAI_size = this.Width - 50;
				FDAI_left = ((this.Height - this.Height) / 2) + 25;
				FDAI_top = 25;
			}

			int lowerLimit = (int)Math.Round(yaw - 40);
			int upperLimit = (int)Math.Round(yaw + 40);
			double oneDegSize = FDAI_size / 70;
			FDAI_centerX = ((FDAI_size / 2) + FDAI_left);
			FDAI_centerY = ((FDAI_size / 2) + FDAI_top);
			double bigLineLength = FDAI_size / 7;
			double shortLineLength = FDAI_size / 14;
			double crosshairLength = FDAI_size / 2;

			double rotX = pitch;
			double rotY = yaw;
			double rotZ = -roll;

			float midPointX = (float)FDAI_centerX;
			float midPointY = (float)FDAI_centerY;
			double radius = FDAI_size / 1.7f;

			List<FDAILine> lineCollection = new List<FDAILine>();
			List<FDAILine> numberCollection = new List<FDAILine>();
			List<FDAIPolygon> polygonCollection = new List<FDAIPolygon>();
			List<FDAIPolygon> shieldCollection = new List<FDAIPolygon>();
			KeyValuePair<List<FDAILine>, List<FDAIPolygon>> comboCollection = new KeyValuePair<List<FDAILine>, List<FDAIPolygon>>();

			polygonCollection = addSphere(radius, polygonCollection);

			lineCollection = addLongitudes(radius, lineCollection);
			lineCollection = addLatitudes(radius, lineCollection);

			comboCollection = addNumbers(radius, numberCollection, shieldCollection);
			numberCollection = comboCollection.Key;
			shieldCollection = comboCollection.Value;

			// DRAW THE FDAI BALL
			drawPolygonCollection(g, polygonCollection, rotX, rotY, rotZ, midPointX, midPointY);
			drawLineCollection(g, lineCollection, rotX, rotY, rotZ, midPointX, midPointY);
			
			drawPolygonCollection(g, shieldCollection, rotX, rotY, rotZ, midPointX, midPointY);
			drawLineCollection(g, numberCollection, rotX, rotY, rotZ, midPointX, midPointY);

			

			/*
			 * DRAW FIXED STUFF
			 */

			// HOLE SHADE
			Rectangle mask = new Rectangle(FDAI_left, FDAI_top, FDAI_size, FDAI_size);
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
			Point[] line = new Point[2];

			int x1 = (int)Math.Round(FDAI_centerX - (crosshairLength / 2));
			int y1 = (int)Math.Round(FDAI_centerY);

			int x2 = (int)Math.Round(FDAI_centerX + (crosshairLength / 2));
			int y2 = (int)Math.Round(FDAI_centerY);

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


			drawRollScale(g, radius);
			drawRollIndicator(g, roll);
			
			// Draw 2nd mask and outline
			g.DrawEllipse(mask2Pen, FDAI_left - 130, FDAI_top - 130, FDAI_size + 260, FDAI_size + 260);
			g.DrawEllipse(borderPen, FDAI_left - 30, FDAI_top - 30, FDAI_size + 60, FDAI_size + 60);

			
			Point[] border = new Point[5];
			border[0] = new Point(0, 0);
			border[1] = new Point(0, this.Height);
			border[2] = new Point(this.Width, this.Height);
			border[3] = new Point(this.Width, 0);
			border[4] = new Point(0, 0);

			g.DrawLines(outerBorderPen, border);

			//g.FillPath(whiteBrush, text);
		}

		private void drawPolygonCollection(Graphics g, List<FDAIPolygon> collection, double rotX, double rotY, double rotZ, float midPointX, float midPointY)
		{
			// Rotate, clip, project, transform and Draw Polygons
			for (int i = 0; i < collection.Count; i++)
			{
				// Initial rotaion to zero FDAI
				collection[i] = rotPolygon(collection[i], zeroX, zeroY, zeroZ);
				
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
				// Initial rotaion to zero FDAI
				collection[i] = rotPoints(collection[i], zeroX, zeroY, zeroZ);
				
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
		
		
		private void drawRollScale(Graphics g, double radius)
		{
			float redPenWidth = (float)(2 * radius * Math.Sin(Helper.deg2rad(2.5f)));
			Pen rollPen = new Pen(Color.White, 3f);
			Pen redPen = new Pen(Color.FromArgb(255, 187, 0, 0), redPenWidth);
			
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
					line[1] = new PointF(FDAI_size / 2f + 5, 0);
					
					g.DrawLines(redPen, line);
					
					g.RotateTransform(-2.5f); // just in case, keeping things in order never hurts.
				}
				
				
				line[0] = new PointF(FDAI_size / 2f, 0);
				if (i % 30 == 0)
				{
					line[1] = new PointF((FDAI_size / 2f) + 15, 0);
				}
				else if (i % 10 == 0)
				{
					line[1] = new PointF((FDAI_size / 2f) + 10, 0);
				}
				else
				{
					line[1] = new PointF((FDAI_size / 2f) + 5, 0);
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
			g.RotateTransform((float)-roll);

			Point[] arrow = new Point[4];
			arrow[0] = new Point(0, 0 - (FDAI_size / 2));
			arrow[1] = new Point((int)xOffset, (int)(0 - (FDAI_size / 2) + yOffset));
			arrow[2] = new Point((int)(xOffset * -1), (int)(0 - (FDAI_size / 2) + yOffset));
			arrow[3] = new Point(0, 0 - (FDAI_size / 2));

			g.FillPolygon(arrowPen, arrow);

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
		
		private void paintPFD(PaintEventArgs e, Graphics g)
		{
			/**
			 * THIS IS THE PRIMARY FLIGHT DISPLAY (PFD) TYPE 
			 **/

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