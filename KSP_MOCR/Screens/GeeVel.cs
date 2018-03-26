using System;
using System.Collections.Generic;
using System.Drawing;
using KRPC.Client.Services.KRPC;

namespace KSP_MOCR
{
	class GeeVel : MocrScreen
	{
		public GeeVel(Screen form)
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

			List<KeyValuePair<double, double?>> target = new List<KeyValuePair<double, double?>>();
			target.Add(new KeyValuePair<double, double?>(3000, 0));
			target.Add(new KeyValuePair<double, double?>(2000, 1));
			target.Add(new KeyValuePair<double, double?>(1500, 2));
			target.Add(new KeyValuePair<double, double?>(700, 3));
			target.Add(new KeyValuePair<double, double?>(600, 4));
			target.Add(new KeyValuePair<double, double?>(500, 20));

			

			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				data.Add(chartData["geeVel"]);
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

			screenLabels[0] = Helper.CreateLabel(39, 0, 42, 1, "======= G-FORCE / INERTIAL VELOCITY =======");

			// Altitude vs. Orbital Speed
			screenCharts[0] = Helper.CreatePlot(0, 1, 120, 30, 3000, 0, 12, 0);
			screenCharts[0].setSeriesColor(0, Color.FromArgb(100, 251, 251, 251));
			screenCharts[0].setSeriesColor(1, Color.FromArgb(100, 251, 0, 0));
			screenCharts[0].setSeriesColor(2, Color.FromArgb(100, 0, 251, 0));
			
		}

		public override void resize()
		{

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
