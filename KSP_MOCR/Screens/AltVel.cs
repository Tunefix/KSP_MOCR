using System;
using System.Collections.Generic;
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

			if (form.connected && form.krpc.CurrentGameScene == GameScene.Flight)
			{
				data = new List<Dictionary<int, double?>>();
				data.Add(chartData["altitudeSpeed"]);
				screenCharts[0].setData(data, false);
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
		}
	}
}
