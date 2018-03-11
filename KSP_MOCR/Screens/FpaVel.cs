using KRPC.Client.Services.KRPC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class FpaVel : MocrScreen
	{
		public FpaVel(Screen form)
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

			int steps = 200;

			string tgtapo = form.dataStorage.getData("TGTAPO");
			if (tgtapo == "")
			{
				tgtapo = "120000";
				form.dataStorage.setData("TGTAPO", "120000");
			}
			int.TryParse(tgtapo, out int apo);
			double mass = form.streamCollection.GetData(DataType.body_mass);
			if (mass == 0) { mass = 5.2915158e22; }
			double bodyrad = form.streamCollection.GetData(DataType.body_radius);
			if (bodyrad == 0) { bodyrad = 600000; }
			double distance = bodyrad + apo;
			double speed = Math.Sqrt((6.67408e-11 * mass) / distance);

			// Find inertial speed at the pad
			float rotPeriod = form.streamCollection.GetData(DataType.body_rotPeriod);
			if(rotPeriod == 0) { rotPeriod = 21600; } // If no period found, set to Kerbin-period
			double lat = 0; // TODO: Set to 0 now, add button to calibrate later
			
			// MAKE GUIDE(S)

			// TODO: THIS SHOULD MAYBE BE CALCULATED BY SOME METHOD
			double StaticSpeed = (2 * Math.PI * bodyrad * Math.Cos(Helper.deg2rad(lat))) / rotPeriod;
			double speedStep = (speed - StaticSpeed) / steps;

			double maxAngle = 40;
			double maxAnglePosition = 0.1; // position on the velocity axis of max angle. 0.0 - 1.0
			double maxAngleSpeed = ((speed - StaticSpeed) * maxAnglePosition) + StaticSpeed;

			double a = (speed - StaticSpeed) * maxAnglePosition; // X-axis length
			double b = maxAngle; // Y-axis height
			double c = 0; // Y-axis offset
			double d = maxAngleSpeed; // X-axis offset


			List<KeyValuePair<double, double?>> middle = new List<KeyValuePair<double, double?>>();
			List<KeyValuePair<double, double?>> middle2 = new List<KeyValuePair<double, double?>>();
			List<KeyValuePair<double, double?>> tgt = new List<KeyValuePair<double, double?>>();

			List<KeyValuePair<double, double?>> low = new List<KeyValuePair<double, double?>>();
			List<KeyValuePair<double, double?>> low2 = new List<KeyValuePair<double, double?>>();

			List<KeyValuePair<double, double?>> high = new List<KeyValuePair<double, double?>>();
			List<KeyValuePair<double, double?>> high2 = new List<KeyValuePair<double, double?>>();


			for (int i = 0; i <= steps; i++)
			{
				// ELLIPTIC CURVE
				int x = (int)(Math.Round(i * ((maxAngleSpeed - StaticSpeed) / steps)) + StaticSpeed);
				double? y = Math.Sqrt(Math.Pow(b, 2) * (1 - (Math.Pow(x - d, 2) / Math.Pow(a, 2)))) + c;
				if (i == 0) y = 0;
				middle.Add(new KeyValuePair<double, double?>(x, y));
				low.Add(new KeyValuePair<double, double?>(x, y * 0.8));
				high.Add(new KeyValuePair<double, double?>(x, y * 1.2));


				// SMOOTHSTEP DOWN
				x = (int)(speed - (i * ((speed - maxAngleSpeed) / steps)));
				
				double t = (x - maxAngleSpeed) / (speed - maxAngleSpeed);
				double m0 = 1.5;
				double m1 = 0.5;
				double sx = ((Math.Pow(t, 3) - (2 * Math.Pow(t, 2)) + t) * m0)
					+
					((-2 * Math.Pow(t, 3)) + (3 * Math.Pow(t, 2)))
					+
					((Math.Pow(t, 3) - Math.Pow(t, 2)) * m1);
				double sy = (-2 * Math.Pow(t, 3)) + (3 * Math.Pow(t, 2));

				x = (int)(speed - Math.Round(sx * (speed - maxAngleSpeed)));
				y = sy * maxAngle;

				middle2.Add(new KeyValuePair<double, double?>(x, y));
				low2.Add(new KeyValuePair<double, double?>(x, y * 0.8));
				high2.Add(new KeyValuePair<double, double?>(x, y * 1.2));
			}

			// TARGET LINE
			tgt.Add(new KeyValuePair<double, double?>(speed, -2));
			tgt.Add(new KeyValuePair<double, double?>(speed, 2));


			data.Add(middle);
			types.Add(Plot.Type.LINE);

			data.Add(middle2);
			types.Add(Plot.Type.LINE);

			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{

				data.Add(chartData["FlightangleVelocity"]);
				types.Add(Plot.Type.CROSS);
			}
			else
			{
				// ADD EMPTY DATA
				data.Add(new List<KeyValuePair<double, double?>>());
				types.Add(Plot.Type.CROSS);
			}

			data.Add(tgt);
			types.Add(Plot.Type.LINE);

			data.Add(low);
			types.Add(Plot.Type.LINE);

			data.Add(low2);
			types.Add(Plot.Type.LINE);

			data.Add(high);
			types.Add(Plot.Type.LINE);

			data.Add(high2);
			types.Add(Plot.Type.LINE);

			screenCharts[0].setData(data, types, false);
			screenCharts[0].maxX = (int)Math.Round(speed * 1.1f);
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenCharts.Add(null); // Initialize Charts
			for (int i = 0; i < 4; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs

			screenInputs[0] = Helper.CreateInput(-1, -1, 1, 1); // Every page must have an input to capture keypresses on Unix
			//screenInputs[0].Text = maxHDOT.ToString();
			//screenInputs[0].TextChanged += (sender, e) => this.setmaxHDOT(sender, e);

			screenLabels[0] = Helper.CreateLabel(30, 0, 60, 1, "======= FLIGHT PATH ANGLE / INERTIAL VELOCITY =======");

			screenCharts[0] = Helper.CreatePlot(0, 1, 120, 30, 0, 3000, -4, 60);
			screenCharts[0].setSeriesColor(0, Color.FromArgb(100, 251, 0, 0));
			screenCharts[0].setSeriesColor(1, Color.FromArgb(100, 251, 0, 0));
			screenCharts[0].setSeriesColor(2, Color.FromArgb(100, 251, 251, 251));
			screenCharts[0].setSeriesColor(3, Color.FromArgb(200, 0, 251, 0));
			screenCharts[0].setSeriesColor(4, Color.FromArgb(100, 0, 251, 0));
			screenCharts[0].setSeriesColor(5, Color.FromArgb(100, 0, 251, 0));
			screenCharts[0].setSeriesColor(6, Color.FromArgb(100, 0, 251, 0));
			screenCharts[0].setSeriesColor(7, Color.FromArgb(100, 0, 251, 0));
		}

		public override void resize()
		{

		}
	}
}
