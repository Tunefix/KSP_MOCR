using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using KRPC.Client.Services.SpaceCenter;

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
		Tuple<double, double, double> burnPositionVecotr;
		//Tuple<double, double, double> burnAnglularMomentumVector;
		//Tuple<double, double, double> burnEccentricityVector;
		//Tuple<double, double, double> burnNVector;
		//double burn_eccentricity;
		//double burn_longitudeOfAscendingNode;
		//double burn_argumentOfPeriapsis;
		//double burn_trueAnomaly;


		static Color orbitColor = Color.FromArgb(200, 255, 255, 255);
		static Color bodyColor = Color.FromArgb(200, 0, 0, 200);
		static Pen orbitPen = new Pen(orbitColor, 1.0f);
		static Pen bodyPen = new Pen(bodyColor, 1.0f);
		static Pen burnPen = new Pen(Color.FromArgb(200, 200, 0, 0), 1.0f);
		static Brush apoBrush = new SolidBrush(Color.FromArgb(200, 200, 100, 0));
		static Brush periBrush = new SolidBrush(Color.FromArgb(200, 0, 200, 0));
		static Brush textBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
		static Brush burnBrush = new SolidBrush(Color.FromArgb(200, 200, 0, 0));

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

		public void setOrbit(double Apo, double Peri, double sMajorA, double sMinorA, double argOP, double lOAN, double radius, double tA)
		{
			orbitApoapsis = Apo;
			orbitPeriapsis = Peri;
			semiMajorAxis = sMajorA;
			semiMinorAxis = sMinorA;
			argumentOfPeriapsis = argOP;
			longitudeOfAscendingNode = lOAN;
			currentRadius = radius;
			trueAnomaly = tA;
		}

		public void setBurnData(double trueAnomaly, Tuple<double, double, double> velocityVector, Tuple<double, double, double> positionVector, float gravParam)
		{
			burn_radiusAtTIG = OrbitFunctions.vectorMagnitude(positionVector);
			burn_trueAnomalyAtTIG = trueAnomaly;
			burn_velocityAtTIG = OrbitFunctions.vectorMagnitude(velocityVector);
			burnVelocityVector = velocityVector;
			burnPositionVecotr = positionVector;
			this.gravParam = gravParam;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			// Check for body and orbitset
			if (body != null && orbitApoapsis != 0)
			{
				// Determine scaler
				int maxSizePx;
				if (this.Width > this.Height)
				{
					maxSizePx = this.Width;
				}
				else
				{
					maxSizePx = this.Height;
				}

				// Get biggest apopapsis of sattelites
				double satMaxApo = 0;
				lock(bodySatellites)
				{
					foreach (CelestialBody sat in bodySatellites)
					{
						if (sat.Orbit.Apoapsis > satMaxApo)
						{
							satMaxApo = sat.Orbit.Apoapsis;
						}
					}
				}

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
				g.DrawEllipse(bodyPen, rect);
				
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
				

				// Draw Orbit
				float xOffset = (float)(semiMajorAxis - orbitPeriapsis);
				double orbitRotation = Helper.rad2deg(longitudeOfAscendingNode + argumentOfPeriapsis);
				double totRot = longitudeOfAscendingNode + argumentOfPeriapsis + trueAnomaly;

				
				//g.RotateTransform((float)orbitRotation);

				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Near;

				g.RotateTransform(-(float)Helper.rad2deg(longitudeOfAscendingNode));

				//g.DrawLine(orbitPen, new PointF(0, 0), new PointF(250, 0));
				//String str = "Longitude of ascending node: " + Helper.toFixed(Helper.rad2deg(longitudeOfAscendingNode), 2);
				//g.DrawString(str, font, textBrush, 0, 0, stringFormat);

				g.RotateTransform(-(float)Helper.rad2deg(argumentOfPeriapsis));

				//g.DrawLine(orbitPen, new PointF(0, 0), new PointF(250, 0));
				//str = "Argument of Periapsis: " + Helper.toFixed(Helper.rad2deg(argumentOfPeriapsis), 2);
				//g.DrawString(str, font, textBrush, 0, 0, stringFormat);




				left = 0 - (float)(((semiMajorAxis * 2) - orbitPeriapsis) * scaler);
				top = 0 - (float)(semiMinorAxis * scaler);
				width = (float)((semiMajorAxis * 2) * scaler);
				height = (float)((semiMinorAxis * 2) * scaler);

				rect = new RectangleF(left, top, width, height);
				g.DrawEllipse(orbitPen, rect);

				//Console.WriteLine("TA: " + trueAnomaly.ToString());

				// Draw position of vessel
				width = (float)(maxSizePx * 0.02);
				height = (float)(maxSizePx * 0.02);
				left = 0 + ((float)(currentRadius * Math.Cos(trueAnomaly) * scaler));
				top = 0 - ((float)(currentRadius * Math.Sin(trueAnomaly) * scaler));

				rect = new RectangleF(left - (width / 2), top - (height / 2), width, height);
				g.FillEllipse(new SolidBrush(orbitColor), rect);

				// Draw vessel altitude
				GraphicsState stateT = g.Save();
				g.TranslateTransform((float)left, (float)top);
				g.RotateTransform((float)Helper.rad2deg(longitudeOfAscendingNode + argumentOfPeriapsis));

				stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Near;
				g.DrawString(Math.Round(currentRadius - bodyRadius).ToString(), font, textBrush, 0, 2);

				g.Restore(stateT);

				// Draw position of Periapsis
				width = (float)(maxSizePx * 0.015);
				height = (float)(maxSizePx * 0.015);
				left = (float)(orbitPeriapsis * scaler);
				top = (float)(0);

				rect = new RectangleF(left - (width / 2), top - (height / 2), width, height);
				g.FillEllipse(periBrush, rect);

				stateT = g.Save();
				g.TranslateTransform((float)left, (float)top);
				g.RotateTransform((float)Helper.rad2deg(longitudeOfAscendingNode + argumentOfPeriapsis));

				String str = Math.Round(orbitPeriapsis - bodyRadius).ToString();

				stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Near;
				g.DrawString(str, font, textBrush, 0, 2, stringFormat);

				g.Restore(stateT);

				// Draw position of Apoapsis
				width = (float)(maxSizePx * 0.015);
				height = (float)(maxSizePx * 0.015);
				left = 0 - (float)(orbitApoapsis * scaler);
				top = 0;

				rect = new RectangleF(left - (width / 2), top - (height / 2), width, height);
				g.FillEllipse(apoBrush, rect);

				stateT = g.Save();
				g.TranslateTransform((float)left, (float)top);
				g.RotateTransform((float)Helper.rad2deg(longitudeOfAscendingNode + argumentOfPeriapsis));

				str = Math.Round(orbitApoapsis - bodyRadius).ToString();

				stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Near;
				g.DrawString(str, font, textBrush, 0, 2, stringFormat);

				g.Restore(stateT);

				// Draw Burn Point
				width = (float)(maxSizePx * 0.02);
				height = (float)(maxSizePx * 0.02);
				left = 0 + ((float)(burn_radiusAtTIG * Math.Cos(burn_trueAnomalyAtTIG) * scaler));
				top = 0 - ((float)(burn_radiusAtTIG * Math.Sin(burn_trueAnomalyAtTIG) * scaler));

				rect = new RectangleF(left - (width / 2), top - (height / 2), width, height);
				g.FillEllipse(burnBrush, rect);

				// Restore orientation
				g.Restore(state);

				drawBurnOrbit(g, scaler);


				// Draw Positional data (if exists)
				if (positionalData != null)
				{
					foreach(Tuple<List<Tuple<double?, double?>>, Color> points in positionalData)
					plotPositionalData(g, points);
				}
			}
		}

		private void plotPositionalData(Graphics g, Tuple<List<Tuple<double?, double?>>, Color> points)
		{
			Brush brush = new SolidBrush(points.Item2);
			Brush brush2 = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
			GraphicsState state = g.Save();
			g.TranslateTransform((float)graphCenterX, (float)graphCenterY);

			foreach (Tuple<double?, double?> point in points.Item1)
			{
				float x = (float)((point.Item1 * scaler) - 1.5);
				float y = (float)((point.Item2 * scaler) - 1.5);
				g.FillEllipse(brush, new RectangleF(x, y, 3, 3));
			}

			// DRAW point1 and point2
			if (point1 != null)
			{
				float x = (float)((point1.Item1 * scaler) - 2);
				float y = (float)((point1.Item2 * scaler) - 2);
				g.FillEllipse(brush2, new RectangleF(x, y, 4, 4));
			}
			
			if (point2 != null)
			{
				float x = (float)((point2.Item1 * scaler) - 2);
				float y = (float)((point2.Item2 * scaler) - 2);
				g.FillEllipse(brush2, new RectangleF(x, y, 4, 4));
			}
			
			
			// Restore orientation
			g.Restore(state);
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

		private void drawBurnOrbit(Graphics g, double scaler)
		{
			// Calculate orbit data
			if (!double.IsNaN(burnVelocityVector.Item1))
			{
				Tuple<double, double, double> angularMomentum = OrbitFunctions.crossProduct(burnVelocityVector, burnPositionVecotr);
				Tuple<double, double, double> nVector = OrbitFunctions.getNVector(angularMomentum);
				Tuple<double, double, double> eVector = OrbitFunctions.getEccentricityVector(burnVelocityVector, burnPositionVecotr, burn_velocityAtTIG, gravParam, burn_radiusAtTIG);
				double burn_semiMajorAxis = OrbitFunctions.getSemiMajorAxis(burn_radiusAtTIG, burn_velocityAtTIG, gravParam);
				double burn_inc = OrbitFunctions.getInclination(angularMomentum);
				double burn_eccentricity = OrbitFunctions.getEccentricity(eVector);
				double burn_periapsis = OrbitFunctions.getPeriapsis(burn_eccentricity, burn_semiMajorAxis);
				double burn_loan = OrbitFunctions.getLongitudeOfAscendingNode(nVector);
				double burn_argumentOfPeriapsis = OrbitFunctions.getArgumentOfPeriapsis(eVector, nVector);
				double burn_semiMinorAxis = OrbitFunctions.getSemiMinorAxis(burn_semiMajorAxis, burn_eccentricity);

				// Draw Orbit
				float xOffset = (float)(burn_semiMajorAxis - burn_periapsis);
				double orbitRotation = Helper.rad2deg(burn_loan + burn_argumentOfPeriapsis);

				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Near;

				GraphicsState state = g.Save();
				g.TranslateTransform((float)graphCenterX, (float)graphCenterY);
				g.RotateTransform((float)-Helper.rad2deg(burn_loan));

				//g.DrawLine(burnPen, new PointF(0, 0), new PointF(250, 0));
				//String str = "Longitude of ascending node: " + Helper.toFixed(Helper.rad2deg(burn_loan), 2);
				//g.DrawString(str, font, burnBrush, 0, 0, stringFormat);

				g.RotateTransform((float)-Helper.rad2deg(burn_argumentOfPeriapsis));

				//g.DrawLine(burnPen, new PointF(0, 0), new PointF(250, 0));
				//str = "Argument of Periapsis: " + Helper.toFixed(Helper.rad2deg(burn_argumentOfPeriapsis), 2);
				//g.DrawString(str, font, burnBrush, 0, 0, stringFormat);

				float left = 0 - (float)(((burn_semiMajorAxis * 2) - burn_periapsis) * scaler);
				float top = 0 - (float)(burn_semiMinorAxis * scaler);
				float width = (float)((burn_semiMajorAxis * 2) * scaler);
				float height = (float)((burn_semiMinorAxis * 2) * scaler);

				RectangleF rect = new RectangleF(left, top, width, height);
				g.DrawEllipse(burnPen, rect);

				// Restore orientation
				g.Restore(state);


				String str = "P: " + Math.Round(burn_periapsis - bodyRadius).ToString();
				g.DrawString(str, font, textBrush, 10, 2, stringFormat);

				str = "A: " + Math.Round((burn_semiMajorAxis * 2) - burn_periapsis - bodyRadius).ToString();
				g.DrawString(str, font, textBrush, 12, 20, stringFormat);
			}
		}

		
		
		
	}
}
