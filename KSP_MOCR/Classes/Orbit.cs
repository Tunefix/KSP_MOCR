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
		List<CelestialBody> bodySatellites;
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

		float graphCenterX;
		float graphCenterY;


		static Color orbitColor = Color.FromArgb(200, 255, 255, 255);
		static Color bodyColor = Color.FromArgb(200, 0, 0, 200);
		static Pen orbitPen = new Pen(orbitColor, 1.0f);
		static Pen bodyPen = new Pen(bodyColor, 1.0f);
		static Brush apoBrush = new SolidBrush(Color.FromArgb(200, 200, 0, 0));
		static Brush periBrush = new SolidBrush(Color.FromArgb(200, 0, 200, 0));
		static Brush textBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));

		readonly Font font;

		public OrbitGraph(Font f)
		{
            this.DoubleBuffered = true;
			font = f;
		}

		public void setBody(CelestialBody b, float br, String n)
		{
			body = b;
			bodyRadius = br;
			bodyName = n;
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

				double maxDist = orbitApoapsis * 2.25; // The .25 is to have some margin to the edges of the graph
				scaler = maxSizePx / maxDist;

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

				// Draw Orbit
				float xOffset = (float)(semiMajorAxis - orbitPeriapsis);
				double orbitRotation = Helper.rad2deg(longitudeOfAscendingNode + argumentOfPeriapsis);

				GraphicsState state = g.Save();
				g.TranslateTransform((float)graphCenterX, (float)graphCenterY);
				g.RotateTransform((float)orbitRotation);

				left = 0 - (float)(orbitPeriapsis * scaler);
				top = 0 - (float)(semiMinorAxis * scaler);
				width = (float)((semiMajorAxis * 2) * scaler);
				height = (float)((semiMinorAxis * 2) * scaler);

				rect = new RectangleF(left, top, width, height);
				g.DrawEllipse(orbitPen, rect);

				Console.WriteLine("TA: " + trueAnomaly.ToString());

				// Draw position of vessel
				width = (float)(maxSizePx * 0.02);
				height = (float)(maxSizePx * 0.02);
				left = 0 - ((float)(currentRadius * Math.Cos(trueAnomaly) * scaler));
				top = 0 + ((float)(currentRadius * Math.Sin(trueAnomaly) * scaler));

				rect = new RectangleF(left - (width / 2), top - (height / 2), width, height);
				g.FillEllipse(new SolidBrush(orbitColor), rect);

				GraphicsState stateT = g.Save();
				g.TranslateTransform((float)left, (float)top);
				g.RotateTransform((float)-orbitRotation);

				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Near;
				g.DrawString(Math.Round(currentRadius - bodyRadius).ToString(), font, textBrush, 0, 2);

				g.Restore(stateT);

				// Draw position of Periapsis
				width = (float)(maxSizePx * 0.015);
				height = (float)(maxSizePx * 0.015);
				left = -(float)(orbitPeriapsis * scaler) - (width / 2);
				top = (float)(0 - (height / 2));

				rect = new RectangleF(left, top, width, height);
				g.FillEllipse(periBrush, rect);

				stateT = g.Save();
				g.TranslateTransform((float)left, (float)top);
				g.RotateTransform((float)-orbitRotation);

				String str = Math.Round(orbitPeriapsis - bodyRadius).ToString();

				stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Near;
				g.DrawString(str, font, textBrush, 0, 2, stringFormat);

				g.Restore(stateT);

				// Draw position of Apoapsis
				width = (float)(maxSizePx * 0.015);
				height = (float)(maxSizePx * 0.015);
				left = (float)(orbitApoapsis * scaler);
				top = 0;

				rect = new RectangleF(left - (width / 2), top - (height / 2), width, height);
				g.FillEllipse(apoBrush, rect);

				stateT = g.Save();
				g.TranslateTransform((float)left, (float)top);
				g.RotateTransform((float)-orbitRotation);

				str = Math.Round(orbitApoapsis - bodyRadius).ToString();

				stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Near;
				g.DrawString(str, font, textBrush, 0, 2, stringFormat);

				g.Restore(stateT);

				// Restore orientation
				g.Restore(state);
			}


		}
	}
}
