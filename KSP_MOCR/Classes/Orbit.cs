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
		ReferenceFrame bodyNonRotFrame;
		float bodyRadius;
		String bodyName;
		IList<CelestialBody> bodySatellites;

		CelestialBody bodyTarget;

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

		static Brush ExitSoiBrush = new SolidBrush(Color.FromArgb(200, 255, 0, 0));
		static Brush EnterSoiBrush = new SolidBrush(Color.FromArgb(200, 0, 255, 0));

		static Pen[] orbitPenBurn = new Pen[]
			{
		new Pen(Color.FromArgb(200, 200, 0, 0), 2.0f),
		new Pen(Color.FromArgb(200, 0, 200, 0), 2.0f),
		new Pen(Color.FromArgb(200, 0, 0, 200), 2.0f)
		};

		static Color[] burnOrbitColor = new Color[]
		{
			Color.FromArgb(200, 200, 0, 0),
			Color.FromArgb(200, 0, 200, 0),
			Color.FromArgb(200, 0, 0, 200)
		};

		readonly Font font;

		public List<Tuple<List<Tuple<double?, double?>>, Color>> positionalData { get; set; }
		
		public Tuple<double, double> point1;
		public Tuple<double, double> point2;

		// Orbit Constants
		double thetaZeroNoREF;
		double thetaZero;
		double alphaZero;
		double directrix;
		double eccentricity;
		double inclination;
		double trueAnomaly;
		PointF P1;
		PointF P2;
		Orbit bodyOrbit;

		double relPeriapse = -1;
		float relPeriapseX;
		float relPeriapseY;


		IList<Node> burnNodes;

		double satX = 0;
		double satY = 0;

		// Manual offset/zoom
		double slideX = 0;
		double slideY = 0;
		double slideXtmp = 0;
		double slideYtmp = 0;
		double zoom = 1;

		/**
		 * SINGLE PLACE TO TURN ALL DEBUG LINES/TEXT ON/OFF
		 **/
		bool debug = false;

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
			if (b != null)
			{
				bodyRadius = b.EquatorialRadius;
				bodyName = b.Name;
				bodySatellites = b.Satellites;
				bodyNonRotFrame = b.NonRotatingReferenceFrame;
			}
		}

		public void setBodyTarget(CelestialBody t)
		{
			bodyTarget = t;
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
					MakeOrbitConstants(g, orbit);
					drawPatchedConic(g, orbit, 0, 360, orbitColor);
					drawCurrentPosition(g, orbit);
				}

				MakeOrbitConstants(g, currentOrbit, false, "CurrentOrbit");
				drawPatchedConic(g, currentOrbit, 0, 360, orbitColor);

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

							// Swap thetaStart and thetaEnd is orbit is retrograde
							if (burnNode.Orbit.Inclination > Helper.deg2rad(90) && burnNode.Orbit.Inclination <= Helper.deg2rad(275))
							{
								double tmp = burnTA;
								burnTA = TAatSOI;
								TAatSOI = tmp;
							}

							MakeOrbitConstants(g, burnNode.Orbit, false, "Node " + i.ToString());
							drawPatchedConic(g, burnNode.Orbit, Helper.rad2deg(burnTA) + 180, Helper.rad2deg(TAatSOI) + 180, burnOrbitColor[i + s % 3]);

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
								if (SOIorbit.Inclination > Helper.deg2rad(90) && SOIorbit.Inclination <= Helper.deg2rad(275))
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
									double CloseTime = SOIorbit.TimeToPeriapsis + burnNode.Orbit.TimeToSOIChange + UT;
									MakeOrbitConstants(g, SOIorbit.Body.Orbit, false, "SOI Orbit Blob");
									drawBlob(g, SOIorbit.Body.Orbit, CloseTime); // This need to be done with satX/Y at 0, becouse it is relative to main body

									satX = SOIorbit.Body.Orbit.PositionAt(UT, bodyNonRotFrame).Item1 * scaler;
									satY = SOIorbit.Body.Orbit.PositionAt(UT, bodyNonRotFrame).Item3 * scaler;
									MakeOrbitConstants(g, SOIorbit, false, -1, "SOI Orbit local", true);
									drawPatchedConic(g, SOIorbit, Helper.rad2deg(entryTA) + 180, Helper.rad2deg(TAatSOI) + 180, burnOrbitColor[(i + s) % 3]);
									drawPeriapse(g, SOIorbit);

									// Main body relative orbit
									satX = SOIorbit.Body.Orbit.PositionAt(CloseTime, bodyNonRotFrame).Item1 * scaler;
									satY = SOIorbit.Body.Orbit.PositionAt(CloseTime, bodyNonRotFrame).Item3 * scaler;
									relPeriapse = -1;
									MakeOrbitConstants(g, SOIorbit, true, CloseTime, "SOI Orbit Relative", true);
									drawPatchedConic(g, SOIorbit, Helper.rad2deg(entryTA) + 180, Helper.rad2deg(TAatSOI) + 180, burnOrbitColor[(i + s) % 3], true);
									drawPeriapse(g, relPeriapseX, relPeriapseY);
								}
								else
								{
									MakeOrbitConstants(g, SOIorbit, false, "SOI Orbit " + s.ToString());
									drawPatchedConic(g, SOIorbit, Helper.rad2deg(entryTA) + 180, Helper.rad2deg(TAatSOI) + 180, burnOrbitColor[(i + s) % 3]);
									drawPeriapse(g, SOIorbit);
								}

								if (SOIorbit.Eccentricity < 1 && double.IsNaN(SOIorbit.TimeToSOIChange))
								{
									drawApoapse(g, SOIorbit);
									drawAscDescNodes(g, SOIorbit);
								}

								SOIorbit = SOIorbit.NextOrbit;
							}

						}
						else
						{
							satX = 0;
							satY = 0;
							MakeOrbitConstants(g, burnNode.Orbit, false, "Burn Orbit");
							drawPatchedConic(g, burnNode.Orbit, 0, 360, burnOrbitColor[(i + s) % 3]);

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
		private void drawPatchedConic(Graphics g, Orbit orbit, double thetaStart, double thetaEnd, Color orbitColor) { drawPatchedConic(g, orbit, thetaStart, thetaEnd, orbitColor, false); }
		private void drawPatchedConic(Graphics g, Orbit orbit, double thetaStart, double thetaEnd, Color orbitColor, bool relativeToParent)
		{
			int steps = 60; // Number of line segments
			double totTheta = thetaEnd - thetaStart;
			double thetaStep = totTheta / steps;

			double theta;
			
			PointF? lastPoint = null;
			PointF? point = null;

			PointF center;

			Pen localPen = new Pen(orbitColor, 2f);
			Brush localBrush = new SolidBrush(orbitColor);

			for (int i = 0; i <= steps; i++)
			{
				// IF relativeToParent, recalculate satX/Y and thetaZero each time.
				if (relativeToParent)
				{
					theta = Helper.deg2rad(180 - ((i * thetaStep) + thetaStart)) + thetaZeroNoREF;
					double time = orbit.UTAtTrueAnomaly(theta - thetaZeroNoREF);
					Tuple<double, double, double> pos = bodyOrbit.PositionAt(time, bodyNonRotFrame);
					satX = pos.Item1 * scaler;
					satY = pos.Item3 * scaler;

					if(pos.Item3 <= 0)
					{
						thetaZero = Math.Acos(pos.Item1 / Math.Sqrt(Math.Pow(pos.Item1, 2) + Math.Pow(pos.Item3, 2))) + thetaZeroNoREF;
					}
					else
					{
						thetaZero = (Helper.deg2rad(360) - Math.Acos(pos.Item1 / Math.Sqrt(Math.Pow(pos.Item1, 2) + Math.Pow(pos.Item3, 2)))) + thetaZeroNoREF;
					}
				}

				theta = Helper.deg2rad(180 - ((i * thetaStep) + thetaStart)) + thetaZero;
				Tuple<double, double> XY = getXYfromTheta(theta, orbit, relativeToParent);

				point = new PointF((float)XY.Item1, (float)XY.Item2);

				if (lastPoint != null)
				{
					
					g.DrawLine(localPen, (PointF)lastPoint, (PointF)point);
					g.DrawLine(orbitPen3, (PointF)lastPoint, (PointF)point);
					lastPoint = point;
				}
				else
				{
					lastPoint = point;
				}

				if (i % 10 == 0 && debug)
				{
					center = new PointF((float)(graphCenterX + satX), (float)(graphCenterY - satY));
					g.DrawLine(orbitPen, (PointF)point, center);

					float x = (float)((300 * Math.Cos(thetaZero)) + center.X);
					float y = (float)((300 * Math.Sin(thetaZero)) + center.Y);
					g.DrawLine(burnPen, center, new PointF(x, y));
					g.DrawString("ThetaZero ", font, textBrush, x, y);
				}

				if (i == 0 && eccentricity >= 1)
				{
					float size = 10;
					center = (PointF)point;
					PointF[] shape = new PointF[3];
					shape[0] = new PointF(center.X + (size * -1f), center.Y + (size * -1.0f));
					shape[1] = new PointF(center.X + (size * 1f), center.Y + (size * -1.0f));
					shape[2] = new PointF(center.X + (size * 0f), center.Y + (size * 0f));
					//g.FillPolygon(EnterSoiBrush, shape);
					g.FillEllipse(EnterSoiBrush, center.X - (size / 2), center.Y - (size / 2), size, size);
				}
				else if(i == steps && !double.IsNaN(orbit.TimeToSOIChange))
				{
					float size = 10;
					center = (PointF)point;
					PointF[] shape = new PointF[3];
					shape[0] = new PointF(center.X + (size * -1f), center.Y + (size * 1f));
					shape[1] = new PointF(center.X + (size * 1f), center.Y + (size * 1f));
					shape[2] = new PointF(center.X + (size * 0f), center.Y + (size * 0f));
					//g.FillPolygon(ExitSoiBrush, shape);
					g.FillEllipse(ExitSoiBrush, center.X - (size / 2), center.Y - (size / 2), size, size);
				}
			}


		}

		private void MakeOrbitConstants(Graphics g, Orbit orbit) { MakeOrbitConstants(g, orbit, false, ""); }
		private void MakeOrbitConstants(Graphics g, Orbit orbit, bool relativeToParent) { MakeOrbitConstants(g, orbit, relativeToParent, ""); }
		private void MakeOrbitConstants(Graphics g, Orbit orbit, bool relativeToParent, string debug) { MakeOrbitConstants(g, orbit, relativeToParent, -1, debug, false); }
		private void MakeOrbitConstants(Graphics g, Orbit orbit, bool relativeToParent, double time, string debugstr, bool relativeREF)
		{
			float x, y;
			double alpha;
			double r;
			double REF = 0; // Degress from x-right
			double bodyInc;

			if (orbit.Body.Name == "Sun")
			{
				bodyInc = 0;
			}
			else
			{
				bodyInc = orbit.Body.Orbit.Inclination;
			

				if(relativeREF)
				{
					if (time != -1)
					{
						if (bodyInc > Helper.deg2rad(90) && bodyInc <= Helper.deg2rad(270))
						{
							REF = 0 - orbit.Body.Orbit.LongitudeOfAscendingNode + orbit.Body.Orbit.ArgumentOfPeriapsis + orbit.Body.Orbit.TrueAnomalyAtUT(time);
						}
						else
						{
							REF = 0 - orbit.Body.Orbit.LongitudeOfAscendingNode - orbit.Body.Orbit.ArgumentOfPeriapsis - orbit.Body.Orbit.TrueAnomalyAtUT(time);
						}
					}
					else
					{
						if (bodyInc > Helper.deg2rad(90) && bodyInc <= Helper.deg2rad(270))
						{
							REF = 0 - orbit.Body.Orbit.LongitudeOfAscendingNode + orbit.Body.Orbit.ArgumentOfPeriapsis + orbit.Body.Orbit.TrueAnomaly;
						}
						else
						{
							REF = 0 - orbit.Body.Orbit.LongitudeOfAscendingNode - orbit.Body.Orbit.ArgumentOfPeriapsis - orbit.Body.Orbit.TrueAnomaly;
						}
					}
				}
			}

			eccentricity = orbit.Eccentricity;
			inclination = orbit.Inclination;
			trueAnomaly = orbit.TrueAnomaly;
			bodyOrbit = orbit.Body.Orbit;

			if (eccentricity <= 0.00001)
			{
				// SET EVERYTHING TO 0
				thetaZero = 0;
				thetaZeroNoREF = 0;
				alphaZero = 0;
				directrix = orbit.Periapsis;

				x = (float)((orbit.Periapsis * scaler) + graphCenterX);
				P1 = new PointF(x, (float)(graphCenterY + satY));
				P2 = new PointF(-x, (float)(graphCenterY - satY));
			}
			else
			{
				if (inclination > Helper.deg2rad(90) && inclination <= Helper.deg2rad(270))
				{
					thetaZeroNoREF =  0 - orbit.LongitudeOfAscendingNode + orbit.ArgumentOfPeriapsis; // Rotation clockwise from x-direction (Right) of Apoapse
					thetaZero = REF + thetaZeroNoREF;
					alphaZero = REF - orbit.LongitudeOfAscendingNode; // Decending Node
				}
				else
				{
					thetaZeroNoREF = 0 - orbit.LongitudeOfAscendingNode - orbit.ArgumentOfPeriapsis; // Rotation counterclockwise from x-direction (Right) of Apoapse
					thetaZero = REF + thetaZeroNoREF; 
					alphaZero = REF - orbit.LongitudeOfAscendingNode; // Decending Node
				}

				directrix = (orbit.Periapsis / eccentricity) + orbit.Periapsis;


				//Inclination point 1
				alpha = alphaZero;
				r = (eccentricity * directrix) / (1 + (eccentricity * Math.Cos(alpha - thetaZero)));
				x = (float)((r * Math.Cos(alpha) * scaler) + graphCenterX + satX);
				y = (float)((r * Math.Sin(alpha) * scaler) + graphCenterY - satY);
				P1 = new PointF(x, y);
				//g.FillEllipse(burnBrush, x - 5, y - 5, 10, 10);

				//Inclination point 2
				alpha = Helper.deg2rad(180) + alphaZero;
				r = (eccentricity * directrix) / (1 + (eccentricity * Math.Cos(alpha - thetaZero)));
				x = (float)((r * Math.Cos(alpha) * scaler) + graphCenterX + satX);
				y = (float)((r * Math.Sin(alpha) * scaler) + graphCenterY - satY);
				P2 = new PointF(x, y);
				//g.FillEllipse(burnBrush, x - 5, y - 5, 10, 10);
			}

			// DEBUG LINES
			if (debug)
			{
				PointF center = new PointF((float)(graphCenterX + satX), (float)(graphCenterY - satY));
				x = (float)((200 * Math.Cos(thetaZero)) + center.X);
				y = (float)((200 * Math.Sin(thetaZero)) + center.Y);
				g.DrawLine(burnPen, center, new PointF(x, y));
				g.DrawString("ThetaZero " + debugstr, font, textBrush, x, y);

				x = (float)((200 * Math.Cos(alphaZero)) + center.X);
				y = (float)((200 * Math.Sin(alphaZero)) + center.Y);
				g.DrawLine(burnPen, center, new PointF(x, y));
				g.DrawString("AlphaZero " + debugstr, font, textBrush, x, y);

				x = (float)((200 * Math.Cos(REF)) + center.X);
				y = (float)((200 * Math.Sin(REF)) + center.Y);
				g.DrawLine(burnPen, center, new PointF(x, y));
				g.DrawString("REF " + debugstr, font, textBrush, x, y);

				g.DrawArc(burnPen, center.X - 180, center.Y - 180, 360, 360, (float)Helper.rad2deg(REF), (float)Helper.rad2deg(alphaZero - REF));
				g.DrawArc(burnPen, center.X - 160, center.Y - 160, 320, 320, (float)Helper.rad2deg(alphaZero), (float)Helper.rad2deg(thetaZero - alphaZero));
			}

		}

		private Tuple<double, double> getXYfromTheta(double theta, Orbit orbit) { return getXYfromTheta(theta, orbit, false); }
		private Tuple<double, double> getXYfromTheta(double theta, Orbit orbit, bool relativeToParent)
		{
			double alpha;
			double r;
			double x, y;
			double dx = 0;
			double dy = 0;

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
			if (inclination != 0) // If inclination is 0 keep dx and dy at 0
			{
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
				dx = d * Math.Sin(alphaZero) * scaler;
				dy = d * Math.Cos(alphaZero) * scaler;

				// Scale with angle
				dx = dx * Math.Abs(Math.Sin(inclination));
				dy = dy * Math.Abs(Math.Sin(inclination));
			}

			// Get Point coordinates
			x = (x * scaler) + graphCenterX + satX + dx;
			y = (y * scaler) + graphCenterY - satY - dy;

			// Store location for Relative Periapse
			if(r < relPeriapse || relPeriapse == -1)
			{
				relPeriapse = r;
				relPeriapseX = (float)x;
				relPeriapseY = (float)y;
			}

			return new Tuple<double, double>(x, y);
		}

		private void drawBlob(Graphics g, Orbit orbit, double time) { drawBlob(g, orbit, time, false);  }
		private void drawBlob(Graphics g, Orbit orbit, double time, bool relativeToParent)
		{
			float x, y;
			double ta = orbit.TrueAnomalyAtUT(time);
			Tuple<double, double> XY = getXYfromTheta(thetaZero - ta, orbit, relativeToParent);
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

		private void drawPeriapse(Graphics g, Orbit orbit) { drawPeriapse(g, orbit, false);  }
		private void drawPeriapse(Graphics g, Orbit orbit, bool relativeToParent)
		{
			float x, y;
			Tuple<double, double> XY = getXYfromTheta(thetaZero, orbit, relativeToParent);
			x = (float)XY.Item1;
			y = (float)XY.Item2;

			g.FillEllipse(apoBrush, x - 5, y - 5, 10, 10);
			g.DrawString(Math.Round(orbit.PeriapsisAltitude).ToString(), font, textBrush, x, y);
		}

		private void drawPeriapse(Graphics g, float x, float y)
		{
			g.FillEllipse(apoBrush, x - 5, y - 5, 10, 10);
			g.DrawString(Math.Round(relPeriapse).ToString(), font, textBrush, x, y);
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
