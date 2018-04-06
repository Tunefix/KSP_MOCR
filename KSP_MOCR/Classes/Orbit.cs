using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using KRPC.Client.Services.SpaceCenter;
using System.Drawing.Text;
using System.Threading;

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
		
		double longitudeOfAscendingNode;
		

		Orbit currentOrbit;

		double scaler = 1;
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
		static Pen orbitPenBurn = new Pen(Color.FromArgb(200, 200, 0, 0), 2.0f);
		static Pen bodyPen = new Pen(bodyColor, 1.0f);
		static Pen burnPen = new Pen(Color.FromArgb(200, 200, 0, 0), 1.0f);
		static Brush apoBrush = new SolidBrush(Color.FromArgb(200, 0, 87, 91));
		static Brush periBrush = new SolidBrush(Color.FromArgb(200, 0, 87, 91));
		static Brush textBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
		static Brush burnBrush = new SolidBrush(Color.FromArgb(200, 200, 0, 0));
		static Brush bodyBrush = new SolidBrush(bodyColor);
		static Brush NodeBrush = new SolidBrush(Color.FromArgb(200, 109, 167, 3));

		readonly Font font;

		public List<Tuple<List<Tuple<double?, double?>>, Color>> positionalData { get; set; }
		
		public Tuple<double, double> point1;
		public Tuple<double, double> point2;

		// Orbit Constants
		double thetaZero;
		double alphaZero;
		double directrix;
		double eccentricity;
		double inclination;
		double trueAnomaly;
		PointF P1;
		PointF P2;

		Node burnNode;

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

		public void setOrbit(Orbit orbit)
		{
			currentOrbit = orbit;
		}

		public void setBurnNode(Node node)
		{
			burnNode = node;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

			// DEBUG TEST
			//g.FillRectangle(new SolidBrush(Color.Maroon), 0, 0, this.Width, this.Height);

			// Check for body and orbitset
			if (currentOrbit != null)
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
				if (currentOrbit.Apoapsis > satMaxApo)
				{
					double maxDist = (currentOrbit.Apoapsis * 2.25) / zoom; // The .25 is to have some margin to the edges of the graph
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
				float left = graphCenterX - (float)(currentOrbit.Body.EquatorialRadius * scaler);
				float top = graphCenterY - (float)(currentOrbit.Body.EquatorialRadius * scaler);
				float width = (float)(currentOrbit.Body.EquatorialRadius * 2 * scaler);
				float height = (float)(currentOrbit.Body.EquatorialRadius * 2 * scaler);
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

				//drawCurrentOrbit(g, scaler);

				MakeOrbitConstants(currentOrbit);
				drawPatchedConic(g, currentOrbit, 0, orbitPen2);

				drawPeriapse(g, currentOrbit);
				drawApoapse(g, currentOrbit);

				drawAscDescNodes(g, currentOrbit);

				drawCurrentPosition(g, currentOrbit);

				if (burnNode != null)
				{
					drawBurnPoint(g, burnNode);
					MakeOrbitConstants(burnNode.Orbit);
					drawPatchedConic(g, burnNode.Orbit, 0, orbitPenBurn);

					drawPeriapse(g, burnNode.Orbit);
					drawApoapse(g, burnNode.Orbit);

					drawAscDescNodes(g, burnNode.Orbit);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="scaler"></param>
		/// <param name="orbit"></param>
		/// <param name="thetaLimit">Where to start drawing the orbit, in degrees counter-clockwise from the apoapse direction. Set to 0 if orbit eccentricity is < 1. Else set to 180 - true Anomaly at SOI.</param>
		private void drawPatchedConic(Graphics g, Orbit orbit, double thetaLimit, Pen orbitPen2)
		{
			int steps = 360; // Number of line segments
			double totTheta = 360 - (2 * thetaLimit);
			double thetaStep = totTheta / steps;

			double theta;
			
			PointF? lastPoint = null;
			PointF? point = null;


			for (int i = 0; i <= steps; i++)
			{
				theta = Helper.deg2rad(180 - ((i * thetaStep) + thetaLimit)) + thetaZero;
				Tuple<double, double> XY = getXYfromTheta(theta, orbit);

				point = new PointF((float)XY.Item1, (float)XY.Item2);

				if (lastPoint != null)
				{
					g.DrawLine(orbitPen, (PointF)lastPoint, (PointF)point);
					g.DrawLine(orbitPen2, (PointF)lastPoint, (PointF)point);
					lastPoint = point;
				}
				else
				{
					lastPoint = point;
				}
			}
		}

		private void MakeOrbitConstants(Orbit orbit)
		{
			float x, y;
			double alpha;
			double r;

			eccentricity = orbit.Eccentricity;
			inclination = orbit.Inclination;
			trueAnomaly = orbit.TrueAnomaly;

			thetaZero = 0 - orbit.LongitudeOfAscendingNode - orbit.ArgumentOfPeriapsis; // Rotation clockwise from x-direction (Right) of Apoapse
			alphaZero = 0 - orbit.LongitudeOfAscendingNode; // Decending Node
			directrix = (orbit.Periapsis / eccentricity) + orbit.Periapsis;

			//Inclination point 1
			alpha = alphaZero;
			r = (eccentricity * directrix) / (1 + (eccentricity * Math.Cos(alpha - thetaZero)));
			x = (float)(r * Math.Cos(alpha) * scaler) + graphCenterX;
			y = (float)(r * Math.Sin(alpha) * scaler) + graphCenterY;
			P1 = new PointF(x, y);
			//g.FillEllipse(burnBrush, x - 5, y - 5, 10, 10);

			//Inclination point 2
			alpha = Helper.deg2rad(180) + alphaZero;
			r = (eccentricity * directrix) / (1 + (eccentricity * Math.Cos(alpha - thetaZero)));
			x = (float)(r * Math.Cos(alpha) * scaler) + graphCenterX;
			y = (float)(r * Math.Sin(alpha) * scaler) + graphCenterY;
			P2 = new PointF(x, y);
			//g.FillEllipse(burnBrush, x - 5, y - 5, 10, 10);
		}

		private Tuple<double, double> getXYfromTheta(double theta, Orbit orbit)
		{
			double alpha;
			double r;
			float x, y;



			alpha = alphaZero - theta;

			r = (eccentricity * directrix) / (1 + (eccentricity * Math.Cos(theta - thetaZero)));


			// Get Point coordinates
			x = (float)(r * Math.Cos(theta));
			y = (float)(r * Math.Sin(theta));

			// Adjust for inclination
			double d = 0; // Distance from point to Asc/Desc-node-line
						  // Formula:
						  // d = |ex1 - ex2 + ex3 - ex4 |
						  //     ------------------------
						  //         sqrt(ex5 + ex6)
			double ex1 = (P2.Y - P1.Y) * x;
			double ex2 = (P2.X - P1.X) * y;
			double ex3 = P2.X * P1.Y;
			double ex4 = P2.Y * P1.X;
			double ex5 = Math.Pow(P2.Y - P1.Y, 2);
			double ex6 = Math.Pow(P2.X - P1.X, 2);
			d = (ex1 - ex2 + ex3 - ex4) / Math.Sqrt(ex5 + ex6);



			// Split into x,y
			double dx = d * Math.Sin(alphaZero) * scaler;
			double dy = d * Math.Cos(alphaZero) * scaler;

			// Scale with angle
			dx = dx * Math.Sin(inclination);
			dy = dy * Math.Sin(inclination);

			// Get Point coordinates
			x = (float)((x * scaler) + graphCenterX + dx);
			y = (float)((y * scaler) + graphCenterY - dy);

			return new Tuple<double, double>(x, y);
		}

		private void drawCurrentPosition(Graphics g, Orbit orbit)
		{
			float x, y;
			Tuple<double, double> XY = getXYfromTheta(thetaZero - trueAnomaly, orbit);
			x = (float)XY.Item1;
			y = (float)XY.Item2;

			g.FillEllipse(textBrush, x - 5, y - 5, 10, 10);
		}

		private void drawPeriapse(Graphics g, Orbit orbit)
		{
			float x, y;
			Tuple<double, double> XY = getXYfromTheta(thetaZero, orbit);
			x = (float)XY.Item1;
			y = (float)XY.Item2;

			g.FillEllipse(apoBrush, x - 5, y - 5, 10, 10);
			g.DrawString(Math.Round(orbit.PeriapsisAltitude).ToString(), font, textBrush, x, y);
		}

		private void drawApoapse(Graphics g, Orbit orbit)
		{
			float x, y;
			Tuple<double, double> XY = getXYfromTheta(Helper.deg2rad(180) + thetaZero, orbit);
			x = (float)XY.Item1;
			y = (float)XY.Item2;

			g.FillEllipse(apoBrush, x - 5, y - 5, 10, 10);
			g.DrawString(Math.Round(orbit.ApoapsisAltitude).ToString(), font, textBrush, x, y);
		}

		private void drawAscDescNodes(Graphics g, Orbit orbit)
		{
			float x, y;
			Tuple<double, double> XY = getXYfromTheta(alphaZero, orbit);
			x = (float)XY.Item1;
			y = (float)XY.Item2;

			g.FillEllipse(NodeBrush, x - 5, y - 5, 10, 10);

			XY = getXYfromTheta(Helper.deg2rad(180) + alphaZero, orbit);
			x = (float)XY.Item1;
			y = (float)XY.Item2;

			g.FillEllipse(NodeBrush, x - 5, y - 5, 10, 10);
		}

		

		private void drawBurnPoint(Graphics g, Node node)
		{
			float x = (float)(graphCenterX + (node.Position(body.NonRotatingReferenceFrame).Item1 * scaler));
			float y = (float)(graphCenterY - (node.Position(body.NonRotatingReferenceFrame).Item3 * scaler));

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
