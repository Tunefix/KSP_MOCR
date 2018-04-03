using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using KRPC.Client.Services.SpaceCenter;
using System.Drawing.Text;

namespace KSP_MOCR
{
	public class OrbitGraph : System.Windows.Forms.Control
	{
		CelestialBody body;
		float bodyRadius;
		String bodyName;
		IList<CelestialBody> bodySatellites;
		//ReferenceFrame refsmmat;
		double orbitApoapsis;
		double orbitPeriapsis;
		double currentRadius;
		double semiMajorAxis;
		double semiMinorAxis;
		double argumentOfPeriapsis;
		double trueAnomaly;
		double longitudeOfAscendingNode;
		double inclination;

		double scaler;
		double zoom = 1;

		float graphCenterX;
		float graphCenterY;

		// BURN ORBIT
		float gravParam;
		double burn_trueAnomalyAtTIG;
		double burn_radiusAtTIG;
		double burn_velocityAtTIG;
		//Tuple<double, double, double> burnVector;
		Tuple<double, double, double> burnVelocityVector;
		Tuple<double, double, double> burnPositionVector;
		//Tuple<double, double, double> burnAnglularMomentumVector;
		//Tuple<double, double, double> burnEccentricityVector;
		//Tuple<double, double, double> burnNVector;
		//double burn_eccentricity;
		//double burn_longitudeOfAscendingNode;
		//double burn_argumentOfPeriapsis;
		//double burn_trueAnomaly;


		static Color orbitColor = Color.FromArgb(200, 255, 255, 255);
		static Color orbitColor2 = Color.FromArgb(128, 0, 128, 255);
		static Color bodyColor = Color.FromArgb(200, 0, 0, 200);
		static Pen orbitPen = new Pen(orbitColor, 1.0f);
		static Pen orbitPen2 = new Pen(orbitColor2, 2.0f);
		static Pen bodyPen = new Pen(bodyColor, 1.0f);
		static Pen burnPen = new Pen(Color.FromArgb(200, 200, 0, 0), 1.0f);
		static Brush apoBrush = new SolidBrush(Color.FromArgb(200, 0, 200, 0));
		static Brush periBrush = new SolidBrush(Color.FromArgb(200, 200, 100, 0));
		static Brush textBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
		static Brush burnBrush = new SolidBrush(Color.FromArgb(200, 200, 0, 0));
		static Brush bodyBrush = new SolidBrush(bodyColor);

		readonly Font font;

		public List<Tuple<List<Tuple<double?, double?>>, Color>> positionalData { get; set; }
		
		public Tuple<double, double> point1;
		public Tuple<double, double> point2;

		public OrbitGraph(Font f)
		{
			this.DoubleBuffered = true;
			font = f;
		}

		public void setZoom(double zoomFactor)
		{
			this.zoom = zoomFactor;
		}
		
		public double getZoom()
		{
			return this.zoom;
		}

		public void setBody(CelestialBody b, float br, String n, IList<CelestialBody> bodySatellites)
		{
			body = b;
			bodyRadius = br;
			bodyName = n;
			this.bodySatellites = bodySatellites;
		}

		public void setOrbit(double Apo, double Peri, double sMajorA, double sMinorA, double argOP, double lOAN, double radius, double tA, double inc)
		{
			orbitApoapsis = Apo;
			orbitPeriapsis = Peri;
			semiMajorAxis = sMajorA;
			semiMinorAxis = sMinorA;
			argumentOfPeriapsis = argOP;
			longitudeOfAscendingNode = lOAN;
			currentRadius = radius;
			trueAnomaly = tA;
			inclination = inc;
		}

		public void setBurnData(double trueAnomaly, Tuple<double, double, double> velocityVector, Tuple<double, double, double> positionVector, float gravParam)
		{
			burn_radiusAtTIG = OrbitFunctions.vectorMagnitude(positionVector);
			burn_trueAnomalyAtTIG = trueAnomaly;
			burn_velocityAtTIG = OrbitFunctions.vectorMagnitude(velocityVector);
			burnVelocityVector = velocityVector;
			burnPositionVector = positionVector;
			this.gravParam = gravParam;
		}

		public void setBurnData(Tuple<double, double, double> velocityVector, Tuple<double, double, double> positionVector)
		{
			burn_radiusAtTIG = OrbitFunctions.vectorMagnitude(positionVector);
			burn_velocityAtTIG = OrbitFunctions.vectorMagnitude(velocityVector);
			burnVelocityVector = velocityVector;
			burnPositionVector = positionVector;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

			// DEBUG TEST
			//g.FillRectangle(new SolidBrush(Color.Maroon), 0, 0, this.Width, this.Height);

			// Check for body and orbitset
			if (body != null && orbitApoapsis != 0)
			{
				// Determine scaler
				int maxSizePx;
				if (this.Width < this.Height)
				{
					maxSizePx = this.Width;
				}
				else
				{
					maxSizePx = this.Height;
				}

				// Get biggest apopapsis of satellites
				double satMaxApo = 0;
				/*lock(bodySatellites)
				{
					foreach (CelestialBody sat in bodySatellites)
					{
						if (sat.Orbit.Apoapsis > satMaxApo)
						{
							satMaxApo = sat.Orbit.Apoapsis;
						}
					}
				}*/

				// Check of vessel orbit is biggest
				if (orbitApoapsis > satMaxApo)
				{
					double maxDist = (orbitApoapsis * 2.25) / zoom; // The .25 is to have some margin to the edges of the graph
					scaler = maxSizePx / maxDist;
				}
				else
				{
					scaler = maxSizePx / ((satMaxApo * 2.25) / zoom);
				}

				// Set Center of graph
				graphCenterX = Width / 2;
				graphCenterY = Height / 2;

				// Draw Body
				float left = graphCenterX - (float)(bodyRadius * scaler);
				float top = graphCenterY - (float)(bodyRadius * scaler);
				float width = (float)(bodyRadius * 2 * scaler);
				float height = (float)(bodyRadius * 2 * scaler);
				RectangleF rect = new RectangleF(left, top, width, height);
				g.FillEllipse(bodyBrush, rect);
				
				GraphicsState state = g.Save();
				g.TranslateTransform((float)graphCenterX, (float)graphCenterY);
								
				// Draw satellites
				lock(bodySatellites)
				{
					foreach (CelestialBody sat in bodySatellites)
					{
						Orbit orbit = sat.Orbit;
						drawOrbit(g,
							orbit.SemiMajorAxis,
							orbit.SemiMinorAxis,
							orbit.Periapsis,
							orbit.LongitudeOfAscendingNode,
							orbit.ArgumentOfPeriapsis,
							orbit.TrueAnomaly,
							orbit.Radius,
							sat.EquatorialRadius,
							sat.Name);
					}
				}
				g.Restore(state);
				

				//drawBurnOrbit(g, scaler);

				drawCurrentOrbit(g, scaler);
				drawPeriapse(g, scaler);
				drawApoapse(g, scaler);
				drawCurrentPosition(g, scaler);

				drawBurnPoint(g, scaler);
			}
		}

		private void drawCurrentPosition(Graphics g, double scaler)
		{
			double offset = (orbitApoapsis - orbitPeriapsis) / 2d;
			double angle = longitudeOfAscendingNode + argumentOfPeriapsis + trueAnomaly;
			double locInc = Math.Sin(argumentOfPeriapsis + trueAnomaly) * inclination;
			double offsetX = offset * Math.Cos(longitudeOfAscendingNode + argumentOfPeriapsis) * -1; // For some reason;
			double offsetY = offset * Math.Sin(longitudeOfAscendingNode + argumentOfPeriapsis) * -1; // Times -1 because 0Y is at top, not bottom;

			double radius = Math.Sqrt(Math.Pow(semiMajorAxis * Math.Cos(angle), 2) + Math.Pow(semiMinorAxis * Math.Sin(angle), 2));

			PointF offsetPoint = new PointF((float)(offsetX * scaler), (float)(offsetY * scaler));
			PointF point = sphericalCoordinate(radius, locInc, angle, scaler, offsetPoint);

			g.FillEllipse(textBrush, point.X - 5, point.Y - 5, 10, 10);
		}

		private void drawPeriapse(Graphics g, double scaler)
		{
			double offset = (orbitApoapsis - orbitPeriapsis) / 2d;
			double angle = longitudeOfAscendingNode + argumentOfPeriapsis;
			double locInc = Math.Sin(longitudeOfAscendingNode + argumentOfPeriapsis) * inclination;
			double offsetX = offset * Math.Cos(longitudeOfAscendingNode + argumentOfPeriapsis) * -1; // For some reason;
			double offsetY = offset * Math.Sin(longitudeOfAscendingNode + argumentOfPeriapsis) * -1; // Times -1 because 0Y is at top, not bottom;

			double radius = Math.Sqrt(Math.Pow(semiMajorAxis * Math.Cos(angle), 2) + Math.Pow(semiMinorAxis * Math.Sin(angle), 2));

			PointF offsetPoint = new PointF((float)(offsetX * scaler), (float)(offsetY * scaler));
			PointF point = sphericalCoordinate(radius, locInc, angle, scaler, offsetPoint);

			g.FillEllipse(periBrush, point.X - 5, point.Y - 5, 10, 10);
		}

		private void drawApoapse(Graphics g, double scaler)
		{
			double offset = (orbitApoapsis - orbitPeriapsis) / 2d;
			double angle = longitudeOfAscendingNode + argumentOfPeriapsis + Math.PI;
			double locInc = Math.Sin(longitudeOfAscendingNode + argumentOfPeriapsis) * inclination;
			double offsetX = offset * Math.Cos(longitudeOfAscendingNode + argumentOfPeriapsis) * -1; // For some reason;
			double offsetY = offset * Math.Sin(longitudeOfAscendingNode + argumentOfPeriapsis) * -1; // Times -1 because 0Y is at top, not bottom;

			double radius = Math.Sqrt(Math.Pow(semiMajorAxis * Math.Cos(angle), 2) + Math.Pow(semiMinorAxis * Math.Sin(angle), 2));

			PointF offsetPoint = new PointF((float)(offsetX * scaler), (float)(offsetY * scaler));
			PointF point = sphericalCoordinate(radius, locInc, angle, scaler, offsetPoint);

			g.FillEllipse(apoBrush, point.X - 5, point.Y - 5, 10, 10);
		}

		private void drawCurrentOrbit(Graphics g, double scaler)
		{
			double theta = 0;
			double eccentricity = semiMajorAxis / semiMinorAxis;

			double radius;
			double locInc;

			double offset = (orbitApoapsis - orbitPeriapsis) / 2d;
			double offsetX = offset * Math.Cos(longitudeOfAscendingNode + argumentOfPeriapsis) * -1; // For some reason
			double offsetY = offset * Math.Sin(longitudeOfAscendingNode + argumentOfPeriapsis) * -1; // Times -1 because 0Y is at top, not bottom

			PointF offsetPoint = new PointF((float)(offsetX * scaler), (float)(offsetY * scaler));

			PointF? lastPoint = null;
			PointF? point = null;

			while(theta <= 2 * Math.PI)
			{
				radius = Math.Sqrt(Math.Pow(semiMajorAxis * Math.Cos(theta),2) + Math.Pow(semiMinorAxis * Math.Sin(theta),2));
				locInc = Math.Sin(longitudeOfAscendingNode + theta) * inclination;
				point = sphericalCoordinate(radius, locInc, theta, scaler, offsetPoint);

				if(lastPoint != null)
				{
					g.DrawLine(orbitPen, (PointF)lastPoint, (PointF)point);
					g.DrawLine(orbitPen2, (PointF)lastPoint, (PointF)point);
					lastPoint = point;
				}
				else
				{
					lastPoint = point;
				}

				theta += Math.PI / 180;
			}
			g.DrawLine(orbitPen, (PointF)lastPoint, (PointF)point);
			g.DrawLine(orbitPen2, (PointF)lastPoint, (PointF)point); // Close the orbit
		}

		private void drawBurnPoint(Graphics g, double scaler)
		{
			float x = (float)(graphCenterX + (burnPositionVector.Item1 * scaler));
			float y = (float)(graphCenterY - (burnPositionVector.Item3 * scaler));

			g.FillEllipse(burnBrush, x - 5, y - 5, 10, 10);
		}

		private PointF sphericalCoordinate(double radius, double inclination, double asimuth, double scaler, PointF offset)
		{
			double x = radius * Math.Cos(inclination) * Math.Cos(asimuth) * scaler;
			double y = radius * Math.Cos(inclination) * Math.Sin(asimuth) * scaler * -1; // -1 because screenY is inverted
			double z = radius * Math.Sin(inclination) * scaler;

			return new PointF((float)x + graphCenterX + offset.X, (float)y + graphCenterY + offset.Y);
		}

		

		private void drawOrbit(Graphics g,
				double semiMajorAxis,
				double semiMinorAxis,
				double orbitPeriapsis,
				double longitudeOfAscendingNode,
				double argumentOfPeriapsis,
				double trueAnomaly,
				double currentRadius,
				double radius,
				String name)
		{
			// PROPERITES
			float left, top, width, height;
			RectangleF rect;
			
			GraphicsState state = g.Save();
			
			// Draw Orbit
			float xOffset = (float)(semiMajorAxis - orbitPeriapsis);
			double orbitRotation = Helper.rad2deg(longitudeOfAscendingNode + argumentOfPeriapsis);
			double totRot = longitudeOfAscendingNode + argumentOfPeriapsis + trueAnomaly;

			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Near;
			stringFormat.LineAlignment = StringAlignment.Near;

			g.RotateTransform(-(float)Helper.rad2deg(longitudeOfAscendingNode));
			g.RotateTransform(-(float)Helper.rad2deg(argumentOfPeriapsis));

			left = 0 - (float)(((semiMajorAxis * 2) - orbitPeriapsis) * scaler);
			top = 0 - (float)(semiMinorAxis * scaler);
			width = (float)((semiMajorAxis * 2) * scaler);
			height = (float)((semiMinorAxis * 2) * scaler);

			rect = new RectangleF(left, top, width, height);
			g.DrawEllipse(orbitPen, rect);

			//Console.WriteLine("TA: " + trueAnomaly.ToString());

			// Draw position of vessel
			width = (float)(scaler * radius);
			height = (float)(scaler * radius);
			left = 0 + ((float)(currentRadius * Math.Cos(trueAnomaly) * scaler));
			top = 0 - ((float)(currentRadius * Math.Sin(trueAnomaly) * scaler));

			rect = new RectangleF(left - (width / 2), top - (height / 2), width, height);
			g.FillEllipse(new SolidBrush(orbitColor), rect);

			// Draw Name
			GraphicsState stateT = g.Save();
			g.TranslateTransform((float)left, (float)top);
			g.RotateTransform((float)Helper.rad2deg(longitudeOfAscendingNode + argumentOfPeriapsis));

			stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Near;
			g.DrawString(name, font, textBrush, 0, 2);

			g.Restore(stateT);
			g.Restore(state);
		}
	}
}
