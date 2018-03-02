using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace KSP_MOCR
{
	public partial class Form1 : Form
	{
		//public Dictionary<String, Dictionary<int, Nullable<double>>> chartData = new Dictionary<String, Dictionary<int, Nullable<double>>>();
		public Dictionary<String, List<KeyValuePair<double, double?>>> chartData = new Dictionary<String, List<KeyValuePair<double, double?>>>();

		private StreamCollection graphStreams;

		public double? originLat;
		public double originLon;

		private KeyValuePair<double, double?> prevAlt = new KeyValuePair<double, double?>(0,0); // Storage of previous MET and altitude for calculation of Hdot (HDOT = m/s)

		public void setupChartData(StreamCollection streamCollection)
		{
			graphStreams = streamCollection;

			chartData.Add("altitudeTime", new List<KeyValuePair<double, double?>>());
			for (int i = 0; i < 600; i++) chartData["altitudeTime"].Add(new KeyValuePair<double, double?>(i, null));

			chartData.Add("apoapsisTime", new List<KeyValuePair<double, double?>>());
			for (int i = 0; i < 600; i++) chartData["apoapsisTime"].Add(new KeyValuePair<double, double?>(i, null));

			chartData.Add("periapsisTime", new List<KeyValuePair<double, double?>>());
			for (int i = 0; i < 600; i++) chartData["periapsisTime"].Add(new KeyValuePair<double, double?>(i, null));

			chartData.Add("geeTime", new List<KeyValuePair<double, double?>>());
			for (int i = 0; i < 600; i++) chartData["geeTime"].Add(new KeyValuePair<double, double?>(i, null));

			chartData.Add("terrainTime", new List<KeyValuePair<double, double?>>());
			for (int i = 0; i < 600; i++) chartData["terrainTime"].Add(new KeyValuePair<double, double?>(i, null));

			chartData.Add("dynPresTime", new List<KeyValuePair<double, double?>>());
			for (int i = 0; i < 600; i++) chartData["dynPresTime"].Add(new KeyValuePair<double, double?>(i, null));
			
			chartData.Add("rollTime", new List<KeyValuePair<double, double?>>());
			for (int i = 0; i < 600; i++) chartData["rollTime"].Add(new KeyValuePair<double, double?>(i, null));
			
			chartData.Add("pitchTime", new List<KeyValuePair<double, double?>>());
			for (int i = 0; i < 600; i++) chartData["pitchTime"].Add(new KeyValuePair<double, double?>(i, null));
			
			chartData.Add("yawTime", new List<KeyValuePair<double, double?>>());
			for (int i = 0; i < 600; i++) chartData["yawTime"].Add(new KeyValuePair<double, double?>(i, null));

			chartData.Add("altitudeSpeed", new List<KeyValuePair<double, double?>>());
			for (int i = 0; i< 3000; i++) chartData["altitudeSpeed"].Add(new KeyValuePair<double, double?>(i, null));

			chartData.Add("timeToApoSpeed", new List<KeyValuePair<double, double?>>());
			for (int i = 0; i < 3000; i++) chartData["timeToApoSpeed"].Add(new KeyValuePair<double, double?>(i, null));

			chartData.Add("altitudeRange", new List<KeyValuePair<double, double?>>());

			chartData.Add("lat", new List<KeyValuePair<double, double?>>());
			chartData.Add("lon", new List<KeyValuePair<double, double?>>());

			chartData.Add("hdot", new List<KeyValuePair<double, double?>>());
			chartData.Add("hhdot", new List<KeyValuePair<double, double?>>());
		}

		public void updateChartData(object sender, EventArgs e)
		{
			if (connected && krpc.CurrentGameScene == KRPC.Client.Services.KRPC.GameScene.Flight && graphStreams != null)
			{
				double altitude = graphStreams.GetData(DataType.flight_meanAltitude);
				double apoapsis = graphStreams.GetData(DataType.orbit_apoapsisAltitude);
				double periapsis = graphStreams.GetData(DataType.orbit_periapsisAltitude);
				double speed = graphStreams.GetData(DataType.orbit_speed);
				double MET = graphStreams.GetData(DataType.vessel_MET);
				double elevation = graphStreams.GetData(DataType.flight_elevation);
				float gee = graphStreams.GetData(DataType.flight_gForce);
				float dynPress = graphStreams.GetData(DataType.flight_dynamicPressure);
				float roll = graphStreams.GetData(DataType.flight_inertial_roll);
				float pitch = graphStreams.GetData(DataType.flight_inertial_pitch);
				float yaw = graphStreams.GetData(DataType.flight_inertial_yaw);
				double tapo = graphStreams.GetData(DataType.orbit_timeToApoapsis);
				double lat = graphStreams.GetData(DataType.flight_map_latitude);
				double lon = graphStreams.GetData(DataType.flight_map_longitude);

				// Limit taop to 120 minutes
				if(tapo > 120 * 60)
				{
					tapo = 120 * 60;
				}

				// Calculate Range
				double RANGE = 0;
				if (originLat != null)
				{
					float R = graphStreams.GetData(DataType.body_radius); // body radius in metres					
					double lat1 = (double)originLat;
					double lon1 = originLon;

					double rad1 = Helper.deg2rad(lat);
					double rad2 = Helper.deg2rad(lat1);
					double deltaRad = Helper.deg2rad((lat1 - lat));
					double deltaLambda = Helper.deg2rad((lon1 - lon));

					double a = Math.Sin(deltaRad / 2) * Math.Sin(deltaRad / 2) +
							Math.Cos(rad1) * Math.Cos(rad2) *
							Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
					double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

					RANGE = R * c;
				}

				addValueToChart(chartData["lat"], MET, lat, 10000);
				addValueToChart(chartData["lon"], MET, lon, 10000);

				// HDOT
				if(prevAlt.Key != 0)
				{
					double HDOT = (double)((altitude - prevAlt.Value) / (MET - prevAlt.Key));
					addValueToChart(chartData["hdot"], MET, HDOT, 3000);
					addValueToChart(chartData["hhdot"], HDOT, altitude, 3000);
				}
				prevAlt = new KeyValuePair<double, double?>(MET, altitude);



				addValueToChart(chartData["altitudeTime"], MET, altitude);
				addValueToChart(chartData["apoapsisTime"], MET, apoapsis);
				addValueToChart(chartData["periapsisTime"], (int)MET, periapsis);
				addValueToChart(chartData["geeTime"], (int)MET, gee);
				addValueToChart(chartData["terrainTime"], (int)MET, elevation);
				addValueToChart(chartData["dynPresTime"], (int)MET, dynPress);
				addValueToChart(chartData["rollTime"], (int)MET, roll);
				addValueToChart(chartData["pitchTime"], (int)MET, pitch);
				addValueToChart(chartData["yawTime"], (int)MET, yaw);
				
				
				addValueToChart(chartData["altitudeSpeed"], (int)speed, altitude, 3000);
				addValueToChart(chartData["timeToApoSpeed"], (int)speed, tapo, 3000);
				
				addValueToChart(chartData["altitudeRange"], (int)RANGE, altitude, 3000);
			}
		}

		public void addValueToChart(List<KeyValuePair<double, double?>> chart, double x, double? y)
		{
			addValueToChart(chart, x, y, 600);
		}

		public void addValueToChart(List<KeyValuePair<double, double?>> chart, double x, double? y, int maxCount)
		{
			if (chart.Count >= maxCount)
			{
				for (int i = 1; i < maxCount; i++)
				{
					chart[i - 1] = chart[i];
				}
				chart[maxCount - 1] = new KeyValuePair<double, double?>(x, y);
			}
			else
			{
				chart.Add(new KeyValuePair<double, double?>(x, y));
			}
		}

		public void setOrigin(double lat, double lon)
		{
			originLat = lat;
			originLon = lon;
		}

		
	}
}
