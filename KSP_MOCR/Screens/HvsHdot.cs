using System;
using System.Collections.Generic;
using System.Drawing;
using KRPC.Client.Services.KRPC;
using System.Globalization;

namespace KSP_MOCR
{
	class HvsHdot : MocrScreen
	{

		double maxHDOT = 500;
		double apo = 120000;

		public HvsHdot(Screen form)
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
			List<List<KeyValuePair<double, double?>>> data = new List<List<KeyValuePair<double, double?>>>();
			List<Plot.Type> types = new List<Plot.Type>();

			// CHECT FOR NEW APO
			string tgtapo = form.dataStorage.getData("TGTAPO");
			if (apo.ToString() != tgtapo) calcApoMax();

			screenInputs[0].Text = Math.Round(maxHDOT).ToString();

			// SATURN V LINE
			List<KeyValuePair<double, double?>> sat = new List<KeyValuePair<double, double?>>();
			sat.Add(new KeyValuePair<double, double?>(0, 0));
			sat.Add(new KeyValuePair<double, double?>(ms(299), m(0.6)));
			sat.Add(new KeyValuePair<double, double?>(ms(817), m(3.3)));
			sat.Add(new KeyValuePair<double, double?>(ms(1504), m(9.0)));
			sat.Add(new KeyValuePair<double, double?>(ms(2234), m(18.2)));
			sat.Add(new KeyValuePair<double, double?>(ms(2726), m(25.4)));
			sat.Add(new KeyValuePair<double, double?>(ms(2961), m(31.1)));
			sat.Add(new KeyValuePair<double, double?>(ms(3174), m(36.3)));
			sat.Add(new KeyValuePair<double, double?>(ms(2777), m(46.0)));
			sat.Add(new KeyValuePair<double, double?>(ms(2290), m(58.4)));
			sat.Add(new KeyValuePair<double, double?>(ms(1861), m(68.7)));
			sat.Add(new KeyValuePair<double, double?>(ms(1464), m(76.9)));
			sat.Add(new KeyValuePair<double, double?>(ms(1101), m(83.2)));
			sat.Add(new KeyValuePair<double, double?>(ms(778), m(87.8)));
			sat.Add(new KeyValuePair<double, double?>(ms(497), m(90.9)));
			sat.Add(new KeyValuePair<double, double?>(ms(263), m(92.8)));
			sat.Add(new KeyValuePair<double, double?>(ms(86), m(93.6)));
			sat.Add(new KeyValuePair<double, double?>(ms(-23), m(93.7)));
			sat.Add(new KeyValuePair<double, double?>(ms(-58), m(93.5)));
			sat.Add(new KeyValuePair<double, double?>(ms(-28), m(93.3)));
			sat.Add(new KeyValuePair<double, double?>(ms(86), m(93.4)));
			sat.Add(new KeyValuePair<double, double?>(ms(36), m(93.5)));
			sat.Add(new KeyValuePair<double, double?>(ms(-40), m(93.5)));
			sat.Add(new KeyValuePair<double, double?>(ms(-66), m(93.3)));
			sat.Add(new KeyValuePair<double, double?>(ms(-69), m(92.6)));
			sat.Add(new KeyValuePair<double, double?>(ms(-36), m(92.6)));
			sat.Add(new KeyValuePair<double, double?>(ms(-1), m(92.6)));


			// MAKE GUIDE(S)
			int steps = 200;
			List<KeyValuePair<double, double?>> middle = new List<KeyValuePair<double, double?>>();
			List<KeyValuePair<double, double?>> middle2 = new List<KeyValuePair<double, double?>>();
			for (int i = 0; i <= steps; i++)
			{
				int x = (int)Math.Round(i * (maxHDOT / steps));
				double? y = (-1 * Math.Sqrt(Math.Pow((apo/2), 2) * (1 - (Math.Pow(x, 2) / Math.Pow(maxHDOT, 2))))) + (apo/2);
				middle.Add(new KeyValuePair<double, double?>(x, y));

				x = (int)Math.Round(i * (maxHDOT / steps));
				y = (Math.Sqrt(Math.Pow((apo / 2), 2) * (1 - (Math.Pow(x, 2) / Math.Pow(maxHDOT, 2))))) + (apo / 2);
				middle2.Add(new KeyValuePair<double, double?>(x, y));

				/*
				double c = maxHDOT / 2.5f;

				y = Math.Sqrt(Math.Pow(apo + c, 2) * (1 - (Math.Pow(x - maxHDOT, 2) / Math.Pow(maxHDOT, 2)))) - c;
				lower.Add(new KeyValuePair<double, double?>(x, y));

				y = Math.Sqrt(Math.Pow(apo - c, 2) * (1 - (Math.Pow(x - maxHDOT, 2) / Math.Pow(maxHDOT, 2)))) + c;
				upper.Add(new KeyValuePair<double, double?>(x, y));
				*/
			}

			data.Add(middle);
			types.Add(Plot.Type.LINE);

			data.Add(middle2);
			types.Add(Plot.Type.LINE);

			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{

				data.Add(chartData["hhdot"]);
				types.Add(Plot.Type.CROSS);
			}

			screenCharts[0].maxY = (int)Math.Round(apo * 1.1f);
			screenCharts[0].maxX = (int)Math.Round(maxHDOT * 1.1f);
			screenCharts[0].setData(data, types, false);
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenCharts.Add(null); // Initialize Charts
			for (int i = 0; i < 3; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs

			screenInputs[0] = Helper.CreateInput(10, 0, 10, 1); // Every page must have an input to capture keypresses on Unix
			screenInputs[0].Text = maxHDOT.ToString();
			screenInputs[0].TextChanged += (sender, e) => this.setmaxHDOT(sender, e);

			screenLabels[0] = Helper.CreateLabel(39, 0, 42, 1, "======= H / HDOT =======");
			screenLabels[1] = Helper.CreateLabel(0, 0, 10, 1, "MAX HDOT:");
			
			screenCharts[0] = Helper.CreatePlot(0, 1, 120, 30, -100, 1200, 0, -1);
			screenCharts[0].setSeriesColor(0, Color.FromArgb(100, 251, 0, 0));
			screenCharts[0].setSeriesColor(1, Color.FromArgb(100, 251, 0, 0));
			screenCharts[0].setSeriesColor(2, Color.FromArgb(100, 251, 251, 251));
			screenCharts[0].setSeriesColor(3, Color.FromArgb(100, 251, 251, 251));
			screenCharts[0].setSeriesColor(4, Color.FromArgb(200, 0, 169, 51));
			screenCharts[0].setSeriesColor(5, Color.FromArgb(200, 0, 51, 204));
			screenCharts[0].setSeriesColor(6, Color.FromArgb(200, 204, 51, 0));


			calcApoMax();
			
		}

		public void calcApoMax()
		{
			string tgtapo = form.dataStorage.getData("TGTAPO");
			if (tgtapo == "")
			{
				tgtapo = "120000";
				form.dataStorage.setData("TGTAPO", "120000");
			}
			int.TryParse(tgtapo, out int tapo);
			if (tapo != -1) apo = tapo;

			// FIND ORBITAL SPEED (CIRCULAR ORBIT)
			double mass = form.streamCollection.GetData(DataType.body_mass);
			if (mass == 0) { mass = 5.2915158e22; }
			double bodyrad = form.streamCollection.GetData(DataType.body_radius);
			if (bodyrad == 0) { bodyrad = 600000; }
			double distance = bodyrad + apo;
			double speed = Math.Sqrt((6.67408e-11 * mass) / distance);

			maxHDOT = (apo / speed) * 20;
		}

		public override void resize()
		{

		}

		private void setmaxHDOT(object sender, EventArgs e)
		{
			if (int.TryParse(screenInputs[0].Text, out int hdot))
			{
				maxHDOT = hdot;
			}
		}

		private double ms(double fs)
		{
			return fs * 0.3048f;
		}

		private double m (double nm)
		{
			return nm * 1852;
		}
	}
}
