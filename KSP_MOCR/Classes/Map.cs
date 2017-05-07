using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KRPC.Client.Services.SpaceCenter;

namespace KSP_MOCR
{
	public class Map : System.Windows.Forms.Control
	{
		Bitmap map;

		public double longitude;
		public double latitude;
		public Tuple<double, double, double> positionVector;
		public Tuple<double, double, double> velocityVector;
		public float bodyRadius; //[m]
		public float bodyGravityParam; //[km^3/s^2]
		public float bodyRotSpeed; //[rad/s]

		public VesselType vesselType;
		public CelestialBody body;

		readonly Pen CoordinatePen = new Pen(Color.FromArgb(100, 78, 128, 118), 2f);
		readonly Pen trackHistoryPen = new Pen(Color.FromArgb(200, 255, 255, 255), 2f);
		readonly Brush craftBrush = new SolidBrush(Color.FromArgb(230, 255, 255, 255));

		public Dictionary<int, PointF?> trackHistory;
		
		public Map()
		{
			this.DoubleBuffered = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			// Get the backgound a.k.a the map
			map = loadMap(body);
			g.DrawImage(map, 0, 0, this.Width, this.Height);

			// DRAW COORDINATE SYSTEM
			double stepSize = this.Width / 360f;
			for (int i = 0; i <= 360; i += 10)
			{
				g.DrawLine(CoordinatePen, (float)(stepSize * i), 0, (float)(stepSize * i), this.Height);
			}

			stepSize = this.Height / 180f;
			for (int i = 0; i <= 180; i += 10)
			{
				g.DrawLine(CoordinatePen, 0, (float)(stepSize * i), this.Width, (float)(stepSize * i));
			}

			// DRAW TRACK HISTORY
			drawTrackHistory(g);

			// DRAW FUTURE GROUNDTRACK
			drawGroundTrack(g);

			// DRAW SPACECRAFT
			float relativeLongitude = (float)((this.Width / 360) * (longitude + 180));
			float relativeLatitude = (float)(this.Height - ((this.Height / 180f) * (latitude + 90)));
			PointF[] craftPoly = getVesselPath(vesselType, 10f, relativeLongitude, relativeLatitude);
			g.FillPolygon(craftBrush, craftPoly);
			
			
			this.Invalidate();
		}

		private Bitmap loadMap(CelestialBody body)
		{
			Bitmap map;
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				map = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/KerbinMap.png");
			}
			else
			{
				map = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\KerbinMap.png");
			}
			return map;
		}

		private void drawTrackHistory(Graphics g)
		{
			PointF? prevPoint = null;
			PointF newPoint;

			float rightLimit = (float)((this.Width / 360f) * (270));
			float leftLimit = (float)((this.Width / 360f) * (90));
			
			if (trackHistory != null)
			{
				foreach (KeyValuePair<int, PointF?> pair in trackHistory)
				{
					if (pair.Value != null)
					{
						float relativeLongitude = (float)((this.Width / 360f) * (pair.Value.Value.X + 180));
						float relativeLatitude = (float)(this.Height - ((this.Height / 180f) * (pair.Value.Value.Y + 90)));
						newPoint = new PointF(relativeLongitude, relativeLatitude);
						
						if (prevPoint != null)
						{
							if (prevPoint.Value.X > rightLimit && newPoint.X < leftLimit)
							{
								g.DrawLine(trackHistoryPen, (PointF)prevPoint, new PointF(this.Width, relativeLatitude));
								g.DrawLine(trackHistoryPen, newPoint, new PointF(0, prevPoint.Value.Y));
							}
							else if (prevPoint.Value.X < leftLimit && newPoint.X > rightLimit)
							{
								g.DrawLine(trackHistoryPen, (PointF)prevPoint, new PointF(0, relativeLatitude));
								g.DrawLine(trackHistoryPen, newPoint, new PointF(this.Width, prevPoint.Value.Y));
							}
							else
							{
								g.DrawLine(trackHistoryPen, (PointF)prevPoint, newPoint);
							}
							
						}
						
						prevPoint = newPoint;
					}
				}
			}
		}

		private void drawGroundTrack(Graphics g)
		{
			
		}

		private PointF[] getVesselPath(VesselType type, float size, float x, float y)
		{
			List<PointF> points = new List<PointF>();

			float offsetX = x - (size / 2f);
			float offsetY = y - (size / 2f);

			switch (type)
			{
				case VesselType.Probe:
					// TODO: DRAW SATTELLITE HERE
					break;
					
				case VesselType.Ship:
				default:
					// TODO: DRAW SHIP HERE
					points.Add(new PointF((0.2f * size) + offsetX, 0 + offsetY));
					points.Add(new PointF((0.8f * size) + offsetX, 0 + offsetY));
					points.Add(new PointF((1 * size) + offsetX, (1 * size) + offsetY));
					points.Add(new PointF((0 * size) + offsetX, (1 * size) + offsetY));
					//points.Add(new PointF(0 + offsetX, 0 + offsetY));
					break;
			}
			
			return points.ToArray();
		}
	}
}
