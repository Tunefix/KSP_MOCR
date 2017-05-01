using System;
using System.Collections.Generic;
using System.Drawing;
using KRPC.Client.Services.KRPC;

namespace KSP_MOCR
{
	class AltVel : MocrScreen
	{
		public AltVel(Form1 form)
		{
            this.form = form;
			this.chartData = form.chartData;

			this.width = 120;
			this.height = 30;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{

			// Re-usable data variable for graph data
			List<Dictionary<int, double?>> data = new List<Dictionary<int, double?>>();
			List<Plot.Type> types = new List<Plot.Type>();

			double fps2ms = 0.3048 * 0.2743;
			double nm2m = 1852;

			if (form.connected && form.krpc.CurrentGameScene == GameScene.Flight)
			{
				Dictionary<int, double?> target = new Dictionary<int, double?>();
				target.Add((int)(1341 * fps2ms), 0 * nm2m);
				target.Add((int)(1400 * fps2ms), 0.6 * nm2m);
				target.Add((int)(1883 * fps2ms), 3.3 * nm2m);
				target.Add((int)(3044 * fps2ms), 9.0 * nm2m);
				target.Add((int)(5087 * fps2ms), 18.2 * nm2m);
				target.Add((int)(6783 * fps2ms), 25.4 * nm2m);
				target.Add((int)(7902 * fps2ms), 31.1 * nm2m);
				target.Add((int)(8976 * fps2ms), 36.3 * nm2m);
				target.Add((int)(9164 * fps2ms), 46.0 * nm2m);
				target.Add((int)(9702 * fps2ms), 58.4 * nm2m);
				target.Add((int)(10341 * fps2ms), 68.7 * nm2m);
				target.Add((int)(11079 * fps2ms), 76.9 * nm2m);
				target.Add((int)(11914 * fps2ms), 83.2 * nm2m);
				target.Add((int)(12851 * fps2ms), 87.8 * nm2m);
				target.Add((int)(13899 * fps2ms), 90.09 * nm2m);
				target.Add((int)(15067 * fps2ms), 92.8 * nm2m);
				target.Add((int)(16374 * fps2ms), 93.6 * nm2m);
				target.Add((int)(17842 * fps2ms), 93.7 * nm2m);
				target.Add((int)(19262 * fps2ms), 93.5 * nm2m);
				target.Add((int)(20618 * fps2ms), 93.3 * nm2m);
				target.Add((int)(22003 * fps2ms), 93.2 * nm2m);
				target.Add((int)(22869 * fps2ms), 93.4 * nm2m);
				target.Add((int)(22998 * fps2ms), 93.5 * nm2m);
				target.Add((int)(23535 * fps2ms), 93.5 * nm2m);
				target.Add((int)(24100 * fps2ms), 93.3 * nm2m);
				target.Add((int)(24690 * fps2ms), 92.6 * nm2m);
				target.Add((int)(25306 * fps2ms), 92.6 * nm2m);
				target.Add((int)(25599 * fps2ms), 92.6 * nm2m);
			
				data = new List<Dictionary<int, double?>>();
				types = new List<Plot.Type>();
				data.Add(chartData["altitudeSpeed"]);
				types.Add(Plot.Type.CROSS);
				
				data.Add(target);
				types.Add(Plot.Type.LINE);
				screenCharts[0].setData(data, types, false);
			}
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenCharts.Add(null); // Initialize Charts
			for (int i = 0; i < 1; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs

			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateLabel(39, 0, 42, 1, "======= ALTITUDE/INERTIAL VELOCITY =======");

			// Altitude vs. Orbital Speed
			screenCharts[0] = Helper.CreatePlot(0, 1, 120, 30, 0, 3000, 0, -1);
			screenCharts[0].setSeriesColor(0, Color.FromArgb(100, 251, 0, 0));
			screenCharts[0].setSeriesColor(1, Color.FromArgb(100, 0, 251, 0));
		}
	}
}
