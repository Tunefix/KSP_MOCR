using KRPC.Client;
using KRPC.Client.Services;
using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class TestScreen : MocrScreen
	{

		public TestScreen(Screen form)
		{
			this.screenStreams = form.streamCollection;
			this.form = form;

			this.width = 120;
			this.height = 30;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight) // krpc.CurrentGameScene is 1 RPC
			{
				// GET DATA
				double meanAltitude = screenStreams.GetData(DataType.flight_meanAltitude); // 0 RPC
				Tuple<double,double,double> vel = screenStreams.GetData(DataType.flight_velocity);


				//flight = GetData.getFlight(); // 7 RPC
				//screenLabels[1].Text = flight.MeanAltitude.ToString(); // 7 RPC

				screenLabels[1].Text = meanAltitude.ToString();
				screenLabels[2].Text = vel.Item1.ToString();
			}
		}

		public override void makeElements()
		{
			for (int i = 0; i < 32; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i< 1; i++) screenInputs.Add(null); // Initialize Inputs


			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix



			screenLabels[0] = Helper.CreateLabel(1, 4, 58, 1); // Connection Status
			screenLabels[0].Text = "NOT CONNECTED";

			screenLabels[1] = Helper.CreateLabel(61, 4, 58, 2);

			screenLabels[4] = Helper.CreateLabel(0, 0, 60, 2, "──────────────────── CONNECTION DETAILS ────────────────────"); // Connection Header}

			screenLabels[2] = Helper.CreateLabel(1, 6, 58, 2, "Label 2");

			screenLabels[3] = Helper.CreateLabel(0, 10, 120, 1, "┼─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ┼");
			screenLabels[5] = Helper.CreateLabel(0, 11, 1, 1, "│"); // TEST
			screenLabels[6] = Helper.CreateLabel(119, 11, 1, 1, "│"); // TEST
			screenLabels[7] = Helper.CreateLabel(0, 12, 1, 2, "│\n││"); // TEST
			screenLabels[8] = Helper.CreateLabel(1, 14, 1, 1, "│"); // TEST

			screenLabels[9] = Helper.CreateLabel(1, 5, 58, 2, "CTRL STATUS:");

			screenLabels[10] = Helper.CreateLabel(10, 11, 1, 1, "│"); // TEST
			screenLabels[11] = Helper.CreateLabel(20, 11, 1, 1, "│"); // TEST
			screenLabels[12] = Helper.CreateLabel(30, 11, 1, 1, "│"); // TEST
			screenLabels[13] = Helper.CreateLabel(40, 11, 1, 1, "│"); // TEST
			screenLabels[14] = Helper.CreateLabel(50, 11, 1, 1, "│"); // TEST
			screenLabels[15] = Helper.CreateLabel(60, 11, 1, 1, "│"); // TEST
			screenLabels[16] = Helper.CreateLabel(70, 11, 1, 1, "│"); // TEST

			screenLabels[17] = Helper.CreateLabel(20, 15, 6, 1, "HHgåfy"); // TEST
			screenLabels[18] = Helper.CreateLabel(20, 16, 6, 1, "HHåygf"); // TEST
			screenLabels[19] = Helper.CreateLabel(20, 17, 6, 1, "HHgåfy"); // TEST

			screenLabels[20] = Helper.CreateLabel(5, 15, 1, 1, "┼");
			screenLabels[21] = Helper.CreateLabel(6, 15, 1, 1, "┼");
			screenLabels[22] = Helper.CreateLabel(5, 16, 1, 1, "┼");
			screenLabels[23] = Helper.CreateLabel(6, 16, 1, 1, "┼");

			screenLabels[24] = Helper.CreateLabel(10, 14, 1, 1, "┼");

			screenLabels[25] = Helper.CreateLabel(0, 10, 120, 1);
			screenLabels[25].Location = new Point(4, 350);
			screenLabels[25].Size = new Size((int)(120 * form.pxPrChar), (int)form.pxPrLine);
			screenLabels[25].Text = "┼─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ─┬─ ─ ─ ─ ┼";
			screenLabels[25].ForeColor = Color.FromArgb(255, 255, 255);
			screenLabels[25].BackColor = Color.Maroon;

			screenLabels[26] = Helper.CreateLabel(60, 7, 25, 1, "THIS IS STANDARD TEXT", false);
			screenLabels[27] = Helper.CreateLabel(60, 8, 25, 1, "     THIS IS BIG TEXT", true);
			screenLabels[28] = Helper.CreateLabel(85, 7, 10, 1, "432.67", false);
			screenLabels[29] = Helper.CreateLabel(85, 8, 10, 1, "432.67", true);
			screenLabels[30] = Helper.CreateLabel(85, 6, 10, 1, "432.67", false);
			screenLabels[31] = Helper.CreateLabel(85, 9, 10, 1, "432.67", true);

		}

		public override void resize() { }
	}
}
