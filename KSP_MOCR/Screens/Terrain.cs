using KRPC.Client;
using KRPC.Client.Services.KRPC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class Terrain : MocrScreen
	{

		public Terrain(Form1 form)
		{
			this.connection = form.connection;
			this.krpc = this.connection.KRPC();
			this.form = form;
			this.help = new KSP_MOCR.helper(form);
			this.chartData = form.chartData;

			this.width = 120;
			this.height = 30;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			// Re-usable data variable for graph data
			List<Dictionary<int, Nullable<double>>> data = new List<Dictionary<int, Nullable<double>>>();

			if (form.connected && krpc.CurrentGameScene == GameScene.Flight)
			{
				data = new List<Dictionary<int, Nullable<double>>>();
				data.Add(chartData["altitudeTime"]);
				data.Add(chartData["terrainTime"]);
				form.showData(0, data, false);
			}
		}

		public override void destroyStreams()
		{

		}


		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) form.screenCharts.Add(null); // Initialize Charts

			// Altitude vs. Time Graph
			form.screenCharts[0] = help.CreateChart(0, 0, 120, 30, 0, 600,-100,3000);
		}
	}
}
