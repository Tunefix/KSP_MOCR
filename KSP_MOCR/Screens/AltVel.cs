using System;
using System.Collections.Generic;
using System.Drawing;
using KRPC.Client.Services.KRPC;

namespace KSP_MOCR
{
	class AltVel : MocrScreen
	{
		public AltVel(Screen form)
		{
            this.form = form;
			this.chartData = form.form.chartData;

			this.width = 120;
			this.height = 30;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{

			// Re-usable data variable for graph data
			List<List<KeyValuePair<int, double?>>> data = new List<List<KeyValuePair<int, double?>>>();
			List<Plot.Type> types = new List<Plot.Type>();

			double fps2ms = 0.3048 * 0.2743;
			double nm2m = 1852;


			List<KeyValuePair<int, double?>> target = new List<KeyValuePair< int, double?>>();
			target.Add(new KeyValuePair<int, double?>((int)(1341 * fps2ms), 0 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(1400 * fps2ms), 0.6 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(1883 * fps2ms), 3.3 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(3044 * fps2ms), 9.0 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(5087 * fps2ms), 18.2 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(6783 * fps2ms), 25.4 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(7902 * fps2ms), 31.1 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(8976 * fps2ms), 36.3 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(9164 * fps2ms), 46.0 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(9702 * fps2ms), 58.4 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(10341 * fps2ms), 68.7 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(11079 * fps2ms), 76.9 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(11914 * fps2ms), 83.2 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(12851 * fps2ms), 87.8 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(13899 * fps2ms), 90.09 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(15067 * fps2ms), 92.8 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(16374 * fps2ms), 93.6 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(17842 * fps2ms), 93.7 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(19262 * fps2ms), 93.5 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(20618 * fps2ms), 93.3 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(22003 * fps2ms), 93.2 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(22869 * fps2ms), 93.4 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(22998 * fps2ms), 93.5 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(23535 * fps2ms), 93.5 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(24100 * fps2ms), 93.3 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(24690 * fps2ms), 92.6 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(25306 * fps2ms), 92.6 * nm2m));
			target.Add(new KeyValuePair<int, double?>((int)(25599 * fps2ms), 92.6 * nm2m));

			// Calculate trajectory bounds
			int steps = 50;

			string tgtapo = form.dataStorage.getData("TGTAPO");
			if(tgtapo == "")
			{
				tgtapo = "120000";
				form.dataStorage.setData("TGTAPO", "120000");
			}

			int.TryParse(tgtapo, out int apo);
			double mass = form.streamCollection.GetData(DataType.body_mass);
			if(mass == 0) { mass = 5.2915158e22; }
			double bodyrad = form.streamCollection.GetData(DataType.body_radius);
			if (bodyrad == 0) { bodyrad = 600000; }
			double distance = bodyrad + apo;
			double speed = Math.Sqrt((6.67408e-11 * mass)/distance);
			double speedStep = speed / steps;

			List<KeyValuePair<int, double?>> middle = new List<KeyValuePair<int, double?>>();
			List<KeyValuePair<int, double?>> upper = new List<KeyValuePair<int, double?>>();
			//Dictionary<int, double?> lower = new Dictionary<int, double?>();

			for (int i = 0; i <= steps; i++)
			{
				int a = 0;
				int x = (int)(i * speedStep);
				double b = Math.Pow(apo - a, 2);
				//double y = Math.Sqrt(b * (1 - (Math.Pow(x - speed, 2) / Math.Pow(speed, 2)))) + a;
				double y = (Math.Sin((Math.PI * (x - (speed / 2))) / speed) * (apo / 2)) + (apo / 2);
				upper.Add(new KeyValuePair<int, double?>(x, y));

				//double xa = 0;
				//y = ((3 * Math.Pow(xa, 2)) - (2 * Math.Pow(xa, 3))) * apo; // Smoothstep
				//y = ((6 * Math.Pow(xa, 5)) - (15 * Math.Pow(xa, 4)) + (10 * Math.Pow(xa, 3))) * apo; // Smootherstep
				//y = smootstep(smootstep(smootstep(xa))) * apo;

				//x(t) = (t3−2t2 + t)m0 + (−2t3 + 3t2)+(t3−t2)m1y(t) =−2t3 + 3t2

				double t = x / speed;
				double m0 = 2;
				double m1 = 3;
				double sx = ((Math.Pow(t, 3) - (2 * Math.Pow(t, 2)) + t) * m0)
					+
					((-2 * Math.Pow(t,3)) + (3 * Math.Pow(t,2)))
					+
					((Math.Pow(t,3) - Math.Pow(t,2)) * m1);
				double sy = (-2 * Math.Pow(t, 3)) + (3 * Math.Pow(t, 2));

				x = (int)Math.Round(sx * speed);
				y = sy * apo;

				middle.Add(new KeyValuePair<int, double?>(x, y));
			}

			List<KeyValuePair<int, double?>> orbit = new List<KeyValuePair<int, double?>>();
			for(int i = 10000; i < 200000; i += 2000)
			{
				int x = (int)Math.Round(Math.Sqrt((6.67408e-11 * mass) / (bodyrad + i)));
				orbit.Add(new KeyValuePair<int, double?>(x, i));
			}



			data = new List<List<KeyValuePair<int, double?>>>();
			types = new List<Plot.Type>();
				

			data.Add(middle);
			types.Add(Plot.Type.LINE);

			data.Add(orbit);
			types.Add(Plot.Type.LINE);

			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				data.Add(chartData["altitudeSpeed"]);
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

			screenLabels[0] = Helper.CreateLabel(39, 0, 42, 1, "======= ALTITUDE/INERTIAL VELOCITY =======");

			// Altitude vs. Orbital Speed
			screenCharts[0] = Helper.CreatePlot(0, 1, 120, 30, 0, 3000, 0, -1);
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
	}
}
