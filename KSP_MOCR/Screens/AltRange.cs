using System;
using System.Collections.Generic;
using System.Drawing;
using KRPC.Client.Services.KRPC;

namespace KSP_MOCR
{
	class AltRange : MocrScreen
	{
		double range = 100000; // Meter downrange

		public AltRange(Screen form)
		{
			this.form = form;
			this.chartData = form.form.chartData;
			this.screenStreams = form.streamCollection;

			this.width = 120;
			this.height = 30;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{

			// Re-usable data variable for graph data
			List<List<KeyValuePair<int, double?>>> data = new List<List<KeyValuePair<int, double?>>>();
			List<Plot.Type> types = new List<Plot.Type>();


			// Calculate trajectory bounds
			int steps = 50;

			string tgtapo = form.dataStorage.getData("TGTAPO");
			if (tgtapo == "")
			{
				tgtapo = "120000";
				form.dataStorage.setData("TGTAPO", "120000");
			}

			int.TryParse(tgtapo, out int apo);

			List<KeyValuePair<int, double?>> middle = new List<KeyValuePair<int, double?>>();
			List<KeyValuePair<int, double?>> upper = new List<KeyValuePair<int, double?>>();
			List<KeyValuePair<int, double?>> lower = new List<KeyValuePair<int, double?>>();
			//Dictionary<int, double?> lower = new Dictionary<int, double?>();



			for (int i = 0; i <= steps; i++)
			{
				int x = (int)Math.Round(i * (range / steps));
				double? y = Math.Sqrt(Math.Pow(apo, 2) * (1 - (Math.Pow(x - range, 2) / Math.Pow(range, 2))));
				middle.Add(new KeyValuePair<int, double?>(x, y));

				double c = range / 2.5f;

				y = Math.Sqrt(Math.Pow(apo + c, 2) * (1 - (Math.Pow(x - range, 2) / Math.Pow(range, 2)))) - c;
				lower.Add(new KeyValuePair<int, double?>(x, y));

				y = Math.Sqrt(Math.Pow(apo - c, 2) * (1 - (Math.Pow(x - range, 2) / Math.Pow(range, 2)))) + c;
				upper.Add(new KeyValuePair<int, double?>(x, y));
			}



			data = new List<List<KeyValuePair<int, double?>>>();
			types = new List<Plot.Type>();

			data.Add(middle);
			types.Add(Plot.Type.LINE);

			data.Add(lower);
			types.Add(Plot.Type.LINE);

			data.Add(upper);
			types.Add(Plot.Type.LINE);

			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				data.Add(chartData["altitudeRange"]);
				types.Add(Plot.Type.CROSS);
			}

			screenCharts[0].setData(data, types, false);

			if (form.form.originLat != null)
			{
				string lat = Helper.toFixed(form.form.originLat, 4);
				string lon = Helper.toFixed(form.form.originLon, 4);
				screenLabels[1].Text = "ORIGIN: " + lat + "  " + lon;
			}
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenCharts.Add(null); // Initialize Charts
			for (int i = 0; i < 3; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			for (int i = 0; i < 1; i++) screenButtons.Add(null); // Initialize Buttons

			screenInputs[0] = Helper.CreateInput(109, 0, 10, 1); // Every page must have an input to capture keypresses on Unix
			screenInputs[0].Text = range.ToString();
			screenInputs[0].TextChanged += (sender, e) => this.setRange(sender, e);

			screenButtons[0] = Helper.CreateButton(0, 0, 12, 1, "SET ORIGIN");
			screenButtons[0].Font = form.buttonFont;
			screenButtons[0].Click += (sender, e) => this.setOrigin(sender, e);

			screenLabels[0] = Helper.CreateLabel(39, 0, 42, 1, "======= ALTITUDE/RANGE FROM ORIGIN =======");
			screenLabels[1] = Helper.CreateLabel(13, 0, 42, 1, "ORIGIN NOT SET");
			screenLabels[2] = Helper.CreateLabel(93, 0, 16, 1, "INSERTION RANGE");

			// Altitude vs. Orbital Speed
			screenCharts[0] = Helper.CreatePlot(0, 1, 120, 30, 0, -1, 0, -1);
			screenCharts[0].setSeriesColor(0, Color.FromArgb(100, 251, 0, 0));
			screenCharts[0].setSeriesColor(1, Color.FromArgb(100, 0, 251, 0));
		}

		/// <summary>
		/// Get smoothstep value (0-1) at position t, where 0 <= t <= 1
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public double smootstep(double t)
		{
			return ((3 * Math.Pow(t, 2)) - (2 * Math.Pow(t, 3)));
		}

		public void setOrigin(object sender, EventArgs e)
		{
			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				form.form.originLat = screenStreams.GetData(DataType.flight_map_latitude);
				form.form.originLon = screenStreams.GetData(DataType.flight_map_longitude);
			}
		}

		public void setRange(object sender, EventArgs e)
		{
			if(int.TryParse(screenInputs[0].Text, out int r))
			{
				range = r;
			}
		}
	}
}
