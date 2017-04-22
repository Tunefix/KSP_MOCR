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
		ReferenceFrame refsmmat;
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
		Tuple<double, double, double> burnVector;
		Tuple<double, double, double> burnVelocityVector;
		Tuple<double, double, double> burnPositionVecotr;
		Tuple<double, double, double> burnAnglularMomentumVector;
		Tuple<double, double, double> burnEccentricityVector;
		Tuple<double, double, double> burnNVector;
		double burn_eccentricity;
		double burn_longitudeOfAscendingNode;
		double burn_argumentOfPeriapsis;
		double burn_trueAnomaly;


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
			burn_radiusAtTIG = vectorMagnitude(positionVector);
			burn_trueAnomalyAtTIG = trueAnomaly;
			burn_velocityAtTIG = vectorMagnitude(velocityVector);
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
			}
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
			Tuple<double, double, double> angularMomentum = crossProduct(burnVelocityVector, burnPositionVecotr);
			Tuple<double, double, double> nVector = getNVector(angularMomentum);
			Tuple<double, double, double> eVector = getEccentricityVector(burnVelocityVector, burnPositionVecotr);
			double burn_semiMajorAxis = getSemiMajorAxis(burn_radiusAtTIG, burn_velocityAtTIG);
			double burn_inc = getInclination(angularMomentum);
			double burn_eccentricity = getEccentricity(eVector);
			double burn_periapsis = getPeriapsis(burn_eccentricity, burn_semiMajorAxis);
			double burn_loan = getLongitudeOfAscendingNode(nVector);
			double burn_argumentOfPeriapsis = getArgumentOfPeriapsis(eVector, nVector);
			double burn_semiMinorAxis = getSemiMinorAxis(burn_semiMajorAxis, burn_eccentricity);

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

		private double getInclination(Tuple<double, double, double> angularMomentum)
		{
			Tuple<double, double, double> kVector = new Tuple<double, double, double>(0, 0, 1);
			return angleBetweenVecotrs(angularMomentum, kVector);
		}

		private Tuple<double, double, double> getNVector(Tuple<double, double, double> angularMomentum)
		{
			return new Tuple<double, double, double>(angularMomentum.Item2 * -1, angularMomentum.Item1, 0);
		}

		private double angleBetweenVecotrs(Tuple<double, double, double> a, Tuple<double, double, double> b)
		{
			return Math.Acos(dotProduct(a,b)/(vectorMagnitude(a) * vectorMagnitude(b)));
		}

		private double getSemiMinorAxis(double semiMajorAxis, double eccentricity)
		{
			return semiMajorAxis * Math.Sqrt(1 - (eccentricity * eccentricity));
		}

		private double getPeriapsis(double eccentricity, double semiMajorAxis)
		{
			return (1 - eccentricity) * semiMajorAxis;
		}

		private double getApoapsis(double eccentricity, double semiMajorAxis)
		{
			return (1 + eccentricity) * semiMajorAxis;
		}

		private double getLongitudeOfAscendingNode(Tuple<double, double, double> nVector)
		{
			Tuple<double, double, double> xVector = new Tuple<double, double, double>(1, 0, 0);
			double absoluteN = vectorAbsolute(nVector);
			double loan;

			if (nVector.Item2 >= 0)
			{
				//loan = Math.Acos(nVector.Item1 / absoluteN);
				loan = angleBetweenVecotrs(nVector, xVector);
			}
			else
			{
				//loan = (2 * Math.PI) - Math.Acos(nVector.Item1 / absoluteN);
				loan = (2 * Math.PI) - angleBetweenVecotrs(nVector, xVector);
			}
			return loan;
		}

		private double getSemiMajorAxis(double radius, double velocity)
		{
			return 1 / ((2 / radius) - ((velocity * velocity) / gravParam));
		}

		private double getEccentricity(double semiMajorAxis, Tuple<double, double, double> angularMomentum)
		{
			return Math.Sqrt(1 - ((gravParam * semiMajorAxis) / (squareVector(angularMomentum))));
		}

		private double getEccentricity(Tuple<double, double, double> eccentricityVector)
		{
			return vectorMagnitude(eccentricityVector);
		}

		private double getTrueAnomaly(Tuple<double, double, double> eccentricityVector, Tuple<double, double, double> positionVector)
		{
			double absoluteEV = vectorAbsolute(eccentricityVector);
			double absolutePV = vectorAbsolute(positionVector);
			double step1 = (eccentricityVector.Item1 * positionVector.Item1)
			+ (eccentricityVector.Item2 * positionVector.Item2)
			+ (eccentricityVector.Item3 * positionVector.Item3);
			double step2 = step1 / (absoluteEV * absolutePV);
			return Math.Acos(step2);
		}

		private double getArgumentOfPeriapsis(Tuple<double, double, double> eccentricityVector, Tuple<double, double, double> nVector)
		{
			double tmp = Math.Acos(dotProduct(eccentricityVector, nVector) / (vectorMagnitude(eccentricityVector)*vectorMagnitude(nVector)));
			
			if (eccentricityVector.Item3 <= 0)
			{
				return tmp;
			}
			return (2 * Math.PI) - tmp;
		}

		private Tuple<double, double, double> getAngularMomentum(Tuple<double, double, double> positionVector, Tuple<double, double, double> velocityVector)
		{
			return crossProduct(positionVector, velocityVector);
		}

		private Tuple<double, double, double> getEccentricityVector(
			Tuple<double, double, double> veloctiyVector,
			Tuple<double, double, double> angularMomentum,
			Tuple<double, double, double> positionVector)
		{
			Tuple<double, double, double> step1 = crossProduct(veloctiyVector, angularMomentum);
			Tuple<double, double, double> step2 = vectorDivision(step1, gravParam);
			Tuple<double, double, double> step3 = vectorDivision(positionVector, vectorMagnitude(positionVector));
			Tuple<double, double, double> step4 = vectorSubtrackt(step2, step3);
			return step4;
		}

		private Tuple<double, double, double> getEccentricityVector(
			Tuple<double, double, double> veloctiyVector,
			Tuple<double, double, double> positionVector
			)
		{
			double step1 = Math.Pow(burn_velocityAtTIG,2);
			double step2 = step1 / gravParam;
			double step3 = 1 / burn_radiusAtTIG;
			double step4 = step2 - step3;

			Tuple<double, double, double> step5 = vectorMultiply(positionVector, step4);

			double step6 = dotProduct(veloctiyVector, positionVector);
			double step7 = step6 / gravParam;
			
			Tuple<double, double, double> step8 = vectorMultiply(veloctiyVector, step7);

			Tuple<double, double, double> step9 = vectorAdd(step5, step8);

			return step9;
		}
		
		private Tuple<double, double, double> crossProduct(Tuple<double, double, double> a, Tuple<double, double, double> b)
		{
			double s1 = (a.Item2 * b.Item3) - (a.Item3 * b.Item2);
			double s2 = (a.Item3 * b.Item1) - (a.Item1 * b.Item3);
			double s3 = (a.Item1 * b.Item2) - (a.Item2 * b.Item1);
			return new Tuple<double, double, double>(s1, s2, s3);
		}

		private double dotProduct(Tuple<double, double, double> a, Tuple<double, double, double> b)
		{
			return (a.Item1 * b.Item1) + (a.Item2 * b.Item2) + (a.Item3 * b.Item3);
		}
		
		private Tuple<double, double, double> vectorMultiply(Tuple<double, double, double> v, double s)
		{
			return new Tuple<double, double, double>(v.Item1 * s, v.Item2 * s, v.Item3 * s);
		}

		private double vectorAbsolute(Tuple<double, double, double> v)
		{
			return Math.Sqrt(Math.Pow(v.Item1, 2) + Math.Pow(v.Item2, 2) + Math.Pow(v.Item3, 2));
		}

		private double vectorMagnitude(Tuple<double, double, double> v)
		{
			return Math.Sqrt(Math.Pow(v.Item1, 2) + Math.Pow(v.Item2, 2) + Math.Pow(v.Item3, 2));
		}

		private Tuple<double, double, double> vectorDivision(Tuple<double, double, double> v, double d)
		{
			double s1 = v.Item1 / d;
			double s2 = v.Item2 / d;
			double s3 = v.Item3 / d;
			return new Tuple<double, double, double>(s1, s2, s3);
		}

		private Tuple<double, double, double> vectorAdd(Tuple<double, double, double> a, Tuple<double, double, double> b)
		{
			double s1 = a.Item1 + b.Item1;
			double s2 = a.Item2 + b.Item2;
			double s3 = a.Item3 + b.Item3;
			return new Tuple<double, double, double>(s1, s2, s3);
		}

		private Tuple<double, double, double> vectorSubtrackt(Tuple<double, double, double> a, Tuple<double, double, double> b)
		{
			double s1 = a.Item1 - b.Item1;
			double s2 = a.Item2 - b.Item2;
			double s3 = a.Item3 - b.Item3;
			return new Tuple<double, double, double>(s1, s2, s3);
		}

		private double squareVector(Tuple<double, double, double> v)
		{
			return (v.Item1 * v.Item1) + (v.Item2 * v.Item2) + (v.Item3 * v.Item3);
		}
		
		
	}
}
