using System;
using System.Collections.Generic;
using System.Drawing;
using KRPC.Client.Services.KRPC;

namespace KSP_MOCR
{
	class ApoPeriAlt : MocrScreen
	{
		public ApoPeriAlt(Screen form)
		{
			this.form = form;
			this.chartData = form.form.chartData;

			this.width = 120;
			this.height = 30;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{

			// Re-usable data variable for graph data
			List<List<KeyValuePair<double, double?>>> data = new List<List<KeyValuePair<double, double?>>>();
			List<Plot.Type> types = new List<Plot.Type>();


			string tgtapo = form.dataStorage.getData("TGTAPO");
			string tgtperi = form.dataStorage.getData("TGTPERI");

			if (tgtapo == "")
			{
				tgtapo = "120000";
				form.dataStorage.setData("TGTAPO", tgtapo);
			}

			if (tgtperi == "")
			{
				tgtperi = "120000";
				form.dataStorage.setData("TGTPERI", tgtperi);
			}

			double.TryParse(tgtapo, out double tgtA);
			double.TryParse(tgtperi, out double tgtP);

			

			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				int xMin = screenCharts[0].findMinX(chartData["altitudeTime"]);
				int xMax = screenCharts[0].findMaxX(chartData["altitudeTime"]);

				List<KeyValuePair<double, double?>> targetA = new List<KeyValuePair<double, double?>>();
				targetA.Add(new KeyValuePair<double, double?>(xMin, tgtA));
				targetA.Add(new KeyValuePair<double, double?>(xMax, tgtA));

				List<KeyValuePair<double, double?>> targetP = new List<KeyValuePair<double, double?>>();
				targetP.Add(new KeyValuePair<double, double?>(xMin, tgtP));
				targetP.Add(new KeyValuePair<double, double?>(xMax, tgtP));

				data.Add(targetA);
				types.Add(Plot.Type.LINE);
				data.Add(targetP);
				types.Add(Plot.Type.LINE);
				data.Add(chartData["apoapsisTime"]);
				types.Add(Plot.Type.LINE);
				data.Add(chartData["periapsisTime"]);
				types.Add(Plot.Type.LINE);
				data.Add(chartData["altitudeTime"]);
				types.Add(Plot.Type.LINE);
			}

			//data.Add(target);
			//types.Add(Plot.Type.LINE);

			screenCharts[0].setData(data, types, false);

		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenCharts.Add(null); // Initialize Charts
			for (int i = 0; i < 1; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs

			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateLabel(39, 0, 42, 1, "======= ALTITUDES =======");

			// Altitude vs. Orbital Speed
			screenCharts[0] = Helper.CreatePlot(0, 1, 120, 30, -1, -1, 0, -1);
			screenCharts[0].fixedXwidth = 600;
			screenCharts[0].setSeriesColor(0, Color.FromArgb(100, 251, 251, 251));
			screenCharts[0].setSeriesColor(1, Color.FromArgb(100, 251, 251, 251));
			screenCharts[0].setSeriesColor(2, Color.FromArgb(200, 0, 169, 51));
			screenCharts[0].setSeriesColor(3, Color.FromArgb(200, 0, 51, 204));
			screenCharts[0].setSeriesColor(4, Color.FromArgb(200, 204, 51, 0));

		}

		public override void resize()
		{
			if (screenCharts.Count > 0 && screenCharts[0] != null)
			{
				screenCharts[0].Size = new Size(form.ClientSize.Width, form.ClientSize.Height - 19);
			}
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

	}
}
