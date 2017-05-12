using System;
using System.Collections.Generic;
using System.Drawing;

namespace KSP_MOCR
{
	public class Attitude : MocrScreen
	{
		public Attitude(Screen form)
		{
			this.form = form;
			this.chartData = form.form.chartData;
			screenStreams = new StreamCollection(form.connection);

			this.width = 60;
			this.height = 30;
			
			this.updateRate = 500;
		}
		
		public override void makeElements()
		{
			for (int i = 0; i < 3; i++) screenCharts.Add(null); // Initialize Charts
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix
			
			screenCharts[0] = Helper.CreatePlot(0, 0, 60, 10,0,600,-180,180);
			screenCharts[0].setSeriesColor(0, Color.FromArgb(200, 255, 255, 255));
			
			screenCharts[1] = Helper.CreatePlot(0, 10, 60, 10, 0, 600, -90, 90);
			screenCharts[1].setSeriesColor(0, Color.FromArgb(200, 255, 255, 255));
			
			screenCharts[2] = Helper.CreatePlot(0, 20, 60, 10, 0, 600, 0, 360);
			screenCharts[2].setSeriesColor(0, Color.FromArgb(200, 255, 255, 255));
		}
		
		public override void updateLocalElements(object sender, EventArgs e)
		{
			List<Dictionary<int, double?>> data = new List<Dictionary<int, double?>>();
			List<Plot.Type> types = new List<Plot.Type>();
			
			data = new List<Dictionary<int, double?>>();
			types = new List<Plot.Type>();
			data.Add(chartData["rollTime"]);
			types.Add(Plot.Type.LINE);
			screenCharts[0].setData(data, types, false);
			
			data = new List<Dictionary<int, double?>>();
			types = new List<Plot.Type>();
			data.Add(chartData["pitchTime"]);
			types.Add(Plot.Type.LINE);
			screenCharts[1].setData(data, types, false);
			
			data = new List<Dictionary<int, double?>>();
			types = new List<Plot.Type>();
			data.Add(chartData["yawTime"]);
			types.Add(Plot.Type.LINE);
			screenCharts[2].setData(data, types, false);
		}
	}
}
