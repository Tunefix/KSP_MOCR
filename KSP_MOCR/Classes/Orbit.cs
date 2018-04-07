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

		Orbit currentOrbit;
		double UT;

		double scaler = 1;

		float graphCenterX;
		float graphCenterY;

		static Color orbitColor = Color.FromArgb(200, 255, 255, 255);
		static Color orbitColor3 = Color.FromArgb(128, 255, 255, 255);
		static Color orbitColor2 = Color.FromArgb(128, 0, 128, 255);
		static Color bodyColor = Color.FromArgb(200, 0, 0, 200);
		static Pen orbitPen = new Pen(orbitColor, 1.0f);
		static Pen orbitPen2 = new Pen(orbitColor2, 2.0f);
		static Pen orbitPen3 = new Pen(orbitColor3, 1.0f);

		static Pen bodyPen = new Pen(bodyColor, 1.0f);
		static Pen burnPen = new Pen(Color.FromArgb(200, 200, 0, 0), 1.0f);
		static Brush apoBrush = new SolidBrush(Color.FromArgb(200, 0, 87, 91));
		static Brush periBrush = new SolidBrush(Color.FromArgb(200, 0, 87, 91));
		static Brush textBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
		static Brush burnBrush = new SolidBrush(Color.FromArgb(200, 200, 0, 0));
		static Brush bodyBrush = new SolidBrush(bodyColor);
		static Brush NodeBrush = new SolidBrush(Color.FromArgb(200, 109, 167, 3));

		static Pen[] orbitPenBurn = new Pen[]
			{
		new Pen(Color.FromArgb(200, 200, 0, 0), 2.0f),
		new Pen(Color.FromArgb(200, 0, 200, 0), 2.0f),
		new Pen(Color.FromArgb(200, 0, 0, 200), 2.0f)
		};

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


		IList<Node> burnNodes;

		double satX = 0;
		double satY = 0;

		// Manual offset/zoom
		double slideX = 0;
		double slideY = 0;
		double slideXtmp = 0;
		double slideYtmp = 0;
		double zoom = 1;

		public OrbitGraph(Font f)
		{
			this.DoubleBuffered = true;
			font = f;
		}

		public void setUT(double ut)
		{
			UT = ut;
		}

		public void setZoom(double zoomFactor)
		{
			this.zoom = zoomFactor;
		}

		public void addZoom(double zoomFactor)
		{
			zoom = zoom * Math.Pow(1.2, zoomFactor);
			Console.WriteLine(zoom.ToString());
		}
		
		public double getZoom()
		{
			return this.zoom;
		}

		public void setSlideXY(double x, double y)
		{
			slideX = x;
			slideY = y;
		}

		public void setSlideTmpXY(double x, double y)
		{
			slideXtmp = x;
			slideYtmp = y;
		}

		public void addSlideXY(double x, double y)
		{
			slideX += x;
			slideY += y;
		}

		public void setBody(CelestialBody b)
		{
			body = b;
			bodyRadius = b.EquatorialRadius;
			bodyName = b.Name;
			bodySatellites = b.Satellites;
		}

		public void setOrbit(Orbit orbit)
		{
			currentOrbit = orbit;
		}

		public void setBurnNodes(IList<Node> nodes)
		{
			burnNodes = nodes;
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
				graphCenterX = (Width / 2);
				graphCenterY = (Height / 2);

				// Add manual offset
				graphCenterX += (float)(slideX + slideXtmp);
				graphCenterY += (float)(slideY + slideYtmp);

				// Draw Body
				float left = graphCenterX - (float)(currentOrbit.Body.EquatorialRadius * scaler);
				float top = graphCenterY - (float)(currentOrbit.Body.EquatorialRadius * scaler);
				float width = (float)(currentOrbit.Body.EquatorialRadius * 2 * scaler);
				float height = (float)(currentOrbit.Body.EquatorialRadius * 2 * scaler);
				RectangleF rect = new RectangleF(left, top, width, height);
				g.FillEllipse(bodyBrush, rect);
								
				// Draw satellites
				foreach (CelestialBody sat in bodySatellites)
				{
					Orbit orbit = sat.Orbit;
					MakeOrbitConstants(orbit);
					drawPatchedConic(g, orbit, 0, 360, orbitPen);
					drawCurrentPosition(g, orbit);
				}

				MakeOrbitConstants(currentOrbit);
				drawPatchedConic(g, currentOrbit, 0, 360, orbitPen2);

				drawPeriapse(g, currentOrbit);
				drawApoapse(g, currentOrbit);

				drawAscDescNodes(g, currentOrbit);

				drawCurrentPosition(g, currentOrbit);

				if (burnNodes != null)
				{
					int s = 0;
					for (int i = 0; i < burnNodes.Count; i++)
					{
						Node burnNode = burnNodes[i];

						drawBurnPoint(g, burnNode);

						if (!double.IsNaN(burnNode.Orbit.TimeToSOIChange))
						{
							double SOIUT = burnNode.Orbit.TimeToSOIChange + UT;
							double TAatSOI = burnNode.Orbit.TrueAnomalyAtUT(SOIUT);
							double burnTA = burnNode.Orbit.TrueAnomalyAtUT(burnNode.UT);

							MakeOrbitConstants(burnNode.Orbit);
							drawPatchedConic(g, burnNode.Orbit, Helper.rad2deg(burnTA) + 180, Helper.rad2deg(TAatSOI) + 180, orbitPenBurn[i + s % 3]);

							Orbit SOIorbit = burnNode.Orbit.NextOrbit;
							while(SOIorbit != null)
							{
								s++;
								double entryTA = SOIorbit.TrueAnomalyAtUT(SOIUT);
								SOIUT = SOIorbit.TimeToSOIChange + UT;
								if (!double.IsNaN(SOIUT))
								{
									TAatSOI = SOIorbit.TrueAnomalyAtUT(SOIUT);
								}
								else
								{
									TAatSOI = Helper.deg2rad(360);
									entryTA = 0;
								}

								// Swap thetaStart and thetaEnd is orbit is retrograde
								if(SOIorbit.Inclination > Helper.deg2rad(90) && SOIorbit.Inclination <= Helper.deg2rad(275))
								{
									double tmp = entryTA;
									entryTA = TAatSOI;
									TAatSOI = tmp;
								}

								satX = 0;
								satY = 0;

								if (SOIorbit.Body.Name != body.Name && SOIorbit.Body.Name != "Sun")
								{
									// ORBIT AROUND ANTOHER BODY
									double CloseTime = SOIorbit.TimeToPeriapsis + UT;
									satX = SOIorbit.Body.Orbit.PositionAt(CloseTime, body.NonRotatingReferenceFrame).Item1 * scaler;
									satY = SOIorbit.Body.Orbit.PositionAt(CloseTime, body.NonRotatingReferenceFrame).Item3 * scaler;
									MakeOrbitConstants(SOIorbit.Body.Orbit); // FIXME: Wrong position of satellite at periapse of passing orbit
									drawBlob(g, SOIorbit.Body.Orbit, CloseTime);
								}

								MakeOrbitConstants(SOIorbit);
								drawPatchedConic(g, SOIorbit, Helper.rad2deg(entryTA) + 180, Helper.rad2deg(TAatSOI) + 180, orbitPenBurn[(i + s) % 3]);

								drawPeriapse(g, SOIorbit);
								if (SOIorbit.Eccentricity < 1)
								{
									drawApoapse(g, SOIorbit);
									drawAscDescNodes(g, SOIorbit);
								}

								SOIorbit = SOIorbit.NextOrbit;
							}

						}
						else
						{
							MakeOrbitConstants(burnNode.Orbit);
							drawPatchedConic(g, burnNode.Orbit, 0, 360, orbitPenBurn[(i + s) % 3]);

							drawPeriapse(g, burnNode.Orbit);
							drawApoapse(g, burnNode.Orbit);

							drawAscDescNodes(g, burnNode.Orbit);
						}

						
						

						
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void drawPatchedConic(Graphics g, Orbit orbit, double thetaStart, double thetaEnd, Pen orbitPen2)
		{
			int steps = 360; // Number of line segments
			double totTheta = thetaEnd - thetaStart;
			double thetaStep = totTheta / steps;

			double theta;
			
			PointF? lastPoint = null;
			PointF? point = null;


			for (int i = 0; i <= steps; i++)
			{
				theta = Helper.deg2rad(180 - ((i * thetaStep) + thetaStart)) + thetaZero;
				Tuple<double, double> XY = getXYfromTheta(theta, orbit);

				point = new PointF((float)XY.Item1, (float)XY.Item2);

				if (lastPoint != null)
				{
					
					g.DrawLine(orbitPen2, (PointF)lastPoint, (PointF)point);
					g.DrawLine(orbitPen3, (PointF)lastPoint, (PointF)point);
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

			if (eccentricity == 0)
			{
				// SET EVERYTHING TO 0
				thetaZero = 0;
				alphaZero = 0;
				directrix = orbit.Periapsis;

				x = (float)((orbit.Periapsis * scaler) + graphCenterX);
				P1 = new PointF(x, graphCenterY);
				P2 = new PointF(-x, graphCenterY);
			}
			else
			{
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
		}

		private Tuple<double, double> getXYfromTheta(double theta, Orbit orbit)
		{
			double alpha;
			double r;
			float x, y;



			alpha = alphaZero - theta;

			if (eccentricity == 0)
			{
				r = directrix; // Periapse is stored here for ecc = 0
			}
			else
			{
				r = (eccentricity * directrix) / (1 + (eccentricity * Math.Cos(theta - thetaZero)));
			}


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
			x = (float)((x * scaler) + graphCenterX + satX + dx);
			y = (float)((y * scaler) + graphCenterY - satY - dy);

			return new Tuple<double, double>(x, y);
		}

		private void drawBlob(Graphics g, Orbit orbit, double time)
		{
			float x, y;
			double ta = orbit.TrueAnomalyAtUT(time);
			Tuple<double, double> XY = getXYfromTheta(thetaZero - ta, orbit);
			x = (float)XY.Item1;
			y = (float)XY.Item2;

			g.FillEllipse(textBrush, x - 5, y - 5, 10, 10);
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
	}
}
