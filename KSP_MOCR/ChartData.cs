using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace KSP_MOCR
{
	public partial class Form1 : Form
	{
		public Dictionary<String, Dictionary<int, Nullable<double>>> chartData = new Dictionary<String, Dictionary<int, Nullable<double>>>();

		private KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Flight> flight_stream;
		private KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Orbit> orbit_stream;
		private KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Vessel> vessel_stream;

		private KRPC.Client.Services.SpaceCenter.Flight flight;
		private KRPC.Client.Services.SpaceCenter.Orbit orbit;
		private KRPC.Client.Services.SpaceCenter.Vessel vessel;

		public void setupChartData()
		{
			chartData.Add("altitudeTime", new Dictionary<int, double?>());
			for (int i = 0; i < 600; i++) chartData["altitudeTime"].Add(i, null);

			chartData.Add("apoapsisTime", new Dictionary<int, double?>());
			for (int i = 0; i < 600; i++) chartData["apoapsisTime"].Add(i, null);

			chartData.Add("periapsisTime", new Dictionary<int, double?>());
			for (int i = 0; i < 600; i++) chartData["periapsisTime"].Add(i, null);

			chartData.Add("geeTime", new Dictionary<int, double?>());
			for (int i = 0; i < 600; i++) chartData["geeTime"].Add(i, null);

			chartData.Add("terrainTime", new Dictionary<int, double?>());
			for (int i = 0; i < 600; i++) chartData["terrainTime"].Add(i, null);

			chartData.Add("dynPresTime", new Dictionary<int, double?>());
			for (int i = 0; i < 600; i++) chartData["dynPresTime"].Add(i, null);
		}

		public void showData(int chartID, List<Dictionary<int, Nullable<double>>> data, bool multiAxis)
		{
			/*
			Chart chart = screenCharts[chartID];
			// Clear out old lines
			chart.Series.Clear();

			if (data.Count > 1 && multiAxis == true)
			{
				int i = 0;
				foreach (Dictionary<int, Nullable<double>> line in data)
				{
					Series serie = new Series();
					serie.ChartType = SeriesChartType.Line;
					serie.XValueType = ChartValueType.Auto;
					serie.YValueType = ChartValueType.Auto;
					serie.Color = chartLineColors[i];
					serie.BorderWidth = 2;

					foreach (KeyValuePair<int, Nullable<double>> point in line)
					{
						if (point.Value != null) serie.Points.AddXY(point.Key, point.Value);
					}

					chart.Series.Add(serie);
					if(i == 0)
					{
						chart.Series[i].YAxisType = AxisType.Primary;
					}
					else
					{
						chart.Series[i].YAxisType = AxisType.Secondary;
					}
					i++;
				}
			}
			else if (data.Count > 0)
			{
				int i = 0;
				foreach (Dictionary<int, Nullable<double>> line in data)
				{
					Series serie = new Series();
					serie.ChartType = SeriesChartType.Line;
					serie.XValueType = ChartValueType.Auto;
					serie.Color = chartLineColors[i];
					serie.BorderWidth = 2;

					foreach (KeyValuePair<int, Nullable<double>> point in line)
					{
						if (point.Value != null) serie.Points.AddXY(point.Key, point.Value);
					}

					chart.Series.Add(serie);
					i++;
				}
			}
			*/
		}

		public void updateChartData(object sender, EventArgs e)
		{
			if (connected && krpc.CurrentGameScene == KRPC.Client.Services.KRPC.GameScene.Flight)
			{
				if (this.vessel_stream == null) this.vessel_stream = connection.AddStream(() => spaceCenter.ActiveVessel);
				this.vessel = this.vessel_stream.Get();

				if (this.flight_stream == null) this.flight_stream = connection.AddStream(() => vessel.Flight(vessel.SurfaceReferenceFrame));
                this.flight = this.flight_stream.Get();

				if (this.orbit_stream == null) this.orbit_stream = connection.AddStream(() => vessel.Orbit);
				this.orbit = this.orbit_stream.Get();


				Console.WriteLine(flight.MeanAltitude);
				Console.WriteLine(flight.BedrockAltitude);
				Console.WriteLine(flight.SurfaceAltitude);
				Console.WriteLine((flight.MeanAltitude - flight.BedrockAltitude).ToString() );

				if (vessel.MET > 600)
				{
					for (int i = 1; i < 600; i++)
					{
						chartData["altitudeTime"][i - 1] = chartData["altitudeTime"][i];
						chartData["apoapsisTime"][i - 1] = chartData["apoapsisTime"][i];
						chartData["periapsisTime"][i - 1] = chartData["periapsisTime"][i];
						chartData["geeTime"][i - 1] = chartData["geeTime"][i];
						chartData["terrainTime"][i - 1] = chartData["terrainTime"][i];
						chartData["dynPresTime"][i - 1] = chartData["dynPresTime"][i];
					}
					chartData["altitudeTime"][599] = this.flight.MeanAltitude;
					chartData["apoapsisTime"][599] = this.orbit.ApoapsisAltitude;
					chartData["periapsisTime"][599] = this.orbit.PeriapsisAltitude;
					chartData["geeTime"][599] = this.flight.GForce;
					chartData["terrainTime"][599] = this.flight.Elevation;
					chartData["dynPresTime"][599] = this.flight.DynamicPressure;
				}
				else
				{
					chartData["altitudeTime"][(int)vessel.MET] = this.flight.MeanAltitude;
					chartData["apoapsisTime"][(int)vessel.MET] = this.orbit.ApoapsisAltitude;
					chartData["periapsisTime"][(int)vessel.MET] = this.orbit.PeriapsisAltitude;
					chartData["geeTime"][(int)vessel.MET] = this.flight.GForce;
					chartData["terrainTime"][(int)vessel.MET] = this.flight.Elevation;
					chartData["dynPresTime"][(int)vessel.MET] = this.flight.DynamicPressure;
				}
			}
		}
	}
}
