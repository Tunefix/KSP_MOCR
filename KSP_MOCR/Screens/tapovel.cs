using System;
using System.Collections.Generic;
using System.Drawing;
using KRPC.Client.Services.KRPC;

namespace KSP_MOCR
{
	class TApoVel : MocrScreen
	{
		public TApoVel(Screen form)
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

			// Calculate trajectory target
			int steps = 50;
			int hsteps = 25;

			string tgtapo = "60";

			int.TryParse(tgtapo, out int apo);
			double mass = form.streamCollection.GetData(DataType.body_mass);
			if (mass == 0) { mass = 5.2915158e22; }
			double bodyrad = form.streamCollection.GetData(DataType.body_radius);
			if (bodyrad == 0) { bodyrad = 600000; }
			double distance = bodyrad + apo;
			double speed = Math.Sqrt((6.67408e-11 * mass) / distance);
			double speedStep = speed / steps;

			List<KeyValuePair<double, double?>> target = new List<KeyValuePair<double, double?>>();

			for (int i = 0; i <= hsteps; i++)
			{
				int x = (int)(i * speedStep);
				double? y;

				double t = x / (speed / 2);
				double m0 = 1;
				double m1 = 1;
				double sx = ((Math.Pow(t, 3) - (2 * Math.Pow(t, 2)) + t) * m0)
					+
					((-2 * Math.Pow(t, 3)) + (3 * Math.Pow(t, 2)))
					+
					((Math.Pow(t, 3) - Math.Pow(t, 2)) * m1);
				double sy = (-2 * Math.Pow(t, 3)) + (3 * Math.Pow(t, 2));

				x = (int)Math.Round(sx * (speed / 2));
				y = sy * apo;

				target.Add(new KeyValuePair<double, double?>(x, y));
			}

			for (int i = hsteps; i <= steps; i++)
			{
				int x = (int)(speed - (i * speedStep));
				double? y;

				double t = x / (speed / 2);
				double m0 = 1;
				double m1 = 1;
				double sx = ((Math.Pow(t, 3) - (2 * Math.Pow(t, 2)) + t) * m0)
					+
					((-2 * Math.Pow(t, 3)) + (3 * Math.Pow(t, 2)))
					+
					((Math.Pow(t, 3) - Math.Pow(t, 2)) * m1);
				double sy = (-2 * Math.Pow(t, 3)) + (3 * Math.Pow(t, 2));

				x = (int)(speed - Math.Round(sx * (speed / 2)));
				y = sy * apo;

				target.Add(new KeyValuePair<double, double?>(x, y));
			}



			data = new List<List<KeyValuePair<double, double?>>>();
			types = new List<Plot.Type>();


			data.Add(target);
			types.Add(Plot.Type.LINE);

			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				data.Add(chartData["timeToApoSpeed"]);
				types.Add(Plot.Type.CROSS);
			}

			screenCharts[0].setData(data, types, false);

		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenCharts.Add(null); // Initialize Charts
			for (int i = 0; i < 1; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs

			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateLabel(0, 0, 120, 1, Helper.prtlen("======= TIME TO APOAPSIS / INERTIAL VELOCITY =======", 120, Helper.Align.CENTER));

			// Altitude vs. Orbital Speed
			screenCharts[0] = Helper.CreatePlot(0, 1, 120, 30, 0, 3000, 0, -1);
			screenCharts[0].setSeriesColor(0, Color.FromArgb(100, 251, 0, 0));
			screenCharts[0].setSeriesColor(1, Color.FromArgb(100, 251, 251, 251));
		}

		public override void resize()
		{

		}

		/// <summary>
		/// Get smoothstep value (0-1) at position t, where 0 <= t <= 1
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public double smootstep(double t, double m0, double m1)
		{
			return ((3 * Math.Pow(t, 2)) - (2 * Math.Pow(t, 3)));
		}
	}
}
