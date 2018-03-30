using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KRPC.Client.Services.SpaceCenter;
using System.Globalization;

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
		public string bodyName;

		public bool tail = false;
		public bool fade = false;
		public int taillength = 3000;

		readonly Pen CoordinatePen = new Pen(Color.FromArgb(32, 78, 128, 118), 1.5f);
		readonly Brush craftBrush = new SolidBrush(Color.FromArgb(230, 255, 255, 255));

		public List<KeyValuePair<double, double?>> trackHistory;
		public List<KeyValuePair<double, double?>> trackHistoryLat;
		public List<KeyValuePair<double, double?>> trackHistoryLon;

		public Map()
		{
			this.DoubleBuffered = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			// Get the backgound a.k.a the map
			loadMap();
			if(map != null) g.DrawImage(map, 0, 0, this.Width, this.Height);

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
			if(trackHistoryLat != null && trackHistoryLat.Count > 1 && tail == true) drawTrackHistory(g);

			// DRAW FUTURE GROUNDTRACK
			drawGroundTrack(g);

			// DRAW SPACECRAFT
			if (trackHistoryLat != null && trackHistoryLat.Count > 1)
			{
				int lastPosition = trackHistoryLat.Count - 1;
				double lat = (double)trackHistoryLat[lastPosition].Value;
				double lon = (double)trackHistoryLon[lastPosition].Value;

				//Console.WriteLine("LL: " + lat.ToString() + " :: " + lon.ToString());

				float relativeLongitude = (float)((this.Width / 360f) * (lon + 180));
				float relativeLatitude = (float)(this.Height - ((this.Height / 180f) * (lat + 90)));
				PointF[] craftPoly = getVesselPath(vesselType, 10f, relativeLongitude, relativeLatitude);
				g.FillPolygon(craftBrush, craftPoly);
			}
		}

		private void loadMap()
		{
			if (body != null)
			{
				if(bodyName != body.Name)
				{
					if (map != null) map.Dispose();
					bodyName = body.Name;
					loadMapFile(bodyName);
				}
			}
			else if(bodyName != null && bodyName != "")
			{
				if(map == null)
				{
					loadMapFile(bodyName);
				}
			}
			else
			{
				if (bodyName == "" || bodyName == null)
				{
					bodyName = "Kerbin";
					loadMapFile(bodyName);
				}
			}
		}

		private void loadMapFile(String file)
		{
			try
			{
				if (Environment.OSVersion.Platform == PlatformID.Unix)
				{
					map = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/" + file + "Map.png");
				}
				else
				{
					map = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\" + file + "Map.png");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.GetType().ToString() + ":" + ex.Message + "\n" + ex.StackTrace);
			}
		}

		private void drawTrackHistory(Graphics g)
		{
			PointF? prevPoint = null;
			PointF newPoint;

			Pen trackHistoryPen = new Pen(Color.FromArgb(200, 255, 255, 255), 2f);

			float rightLimit = (float)((this.Width / 360f) * (270));
			float leftLimit = (float)((this.Width / 360f) * (90));
			
			if (trackHistoryLat != null)
			{
				int count = trackHistoryLat.Count;
				int offset = 0;
				if (taillength < count) offset = count - taillength;

				for (int n = offset; n < count; n++)
				{
					if (trackHistoryLat[n].Value != null)
					{
						if(fade)
						{
							int A = (int)(Math.Round((200f / (count / 1.5f)) * n));
							if (A > 200) A = 200;
							trackHistoryPen = new Pen(Color.FromArgb(A, 255, 255, 255), 2f);
						}
						
						double lat = (double)trackHistoryLat[n].Value;
						double lon = (double)trackHistoryLon[n].Value;

						float relativeLongitude = (float)((this.Width / 360f) * (lon + 180));
						float relativeLatitude = (float)(this.Height - ((this.Height / 180f) * (lat + 90)));
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
					n++;
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
					//break;
					
				case VesselType.Ship:
				default:
					// TODO: DRAW SHIP HERE
					points.Add(new PointF((0.2f * size) + offsetX, 0 + offsetY));
					points.Add(new PointF((0.8f * size) + offsetX, 0 + offsetY));
					points.Add(new PointF((1 * size) + offsetX, (1 * size) + offsetY));
					points.Add(new PointF((0 * size) + offsetX, (1 * size) + offsetY));
					break;
			}
			
			return points.ToArray();
		}
	}
}
