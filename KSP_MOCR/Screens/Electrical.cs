using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class Electrical : MocrScreen
	{
		double MET = 0;
		Parts parts;

		public Electrical(Screen form)
		{
			this.form = form;
			this.screenStreams = form.streamCollection;

			this.charSize = true;
			this.width = 60;
			this.height = 32;
			this.updateRate = 1000;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 80; i++) screenLabels.Add(null); // Initialize Labels

			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateLabel(0, 0, 5, 1, "SCR 6");
			screenLabels[1] = Helper.CreateLabel(16, 0, 30, 1, "SPACECRAFT ELECTRICAL SYSTEMS");
			screenLabels[2] = Helper.CreateLabel(0, 1, 12, 1, "LT: XX:XX:XX");
			screenLabels[3] = Helper.CreateLabel(23, 1, 14, 1, "MET: XXX:XX:XX");

			screenLabels[4] = Helper.CreateLabel(1, 3, 58, 1,  "──────────── SOURCES ───────────── GEN ────── SUN ──────");
			//screenLabels[5] = Helper.CreateLabel(31, 3, 28, 1, "────────── DRAINS ──────────");
			screenLabels[6] = Helper.CreateLabel(1, 15, 58, 1, "──────────── STORAGE ───────────── CAP ────── LVL ──────");

			screenLabels[10] = Helper.CreateLabel(0, 4, 60, 1, ""); // SOURCE #1
			screenLabels[11] = Helper.CreateLabel(0, 5, 60, 1, "");
			screenLabels[12] = Helper.CreateLabel(0, 6, 60, 1, "");
			screenLabels[13] = Helper.CreateLabel(0, 7, 60, 1, "");
			screenLabels[14] = Helper.CreateLabel(0, 8, 60, 1, "");
			screenLabels[15] = Helper.CreateLabel(0, 9, 60, 1, "");
			screenLabels[16] = Helper.CreateLabel(0, 10, 60, 1, "");
			screenLabels[17] = Helper.CreateLabel(0, 11, 60, 1, "");
			screenLabels[18] = Helper.CreateLabel(0, 12, 60, 1, "");
			screenLabels[19] = Helper.CreateLabel(0, 13, 60, 1, "");

			/*screenLabels[20] = Helper.CreateLabel(30, 4, 30, 1, ""); // DRAIN #1
			screenLabels[21] = Helper.CreateLabel(30, 5, 30, 1, "");
			screenLabels[22] = Helper.CreateLabel(30, 6, 30, 1, "");
			screenLabels[23] = Helper.CreateLabel(30, 7, 30, 1, "");
			screenLabels[24] = Helper.CreateLabel(30, 8, 30, 1, "");
			screenLabels[25] = Helper.CreateLabel(30, 9, 30, 1, "");
			screenLabels[26] = Helper.CreateLabel(30, 10, 30, 1, "");
			screenLabels[27] = Helper.CreateLabel(30, 11, 30, 1, "");
			screenLabels[28] = Helper.CreateLabel(30, 12, 30, 1, "");
			screenLabels[29] = Helper.CreateLabel(30, 13, 30, 1, "");*/

			
			screenLabels[60] = Helper.CreateLabel(0, 16, 58, 1, ""); // BATTERY #1
			screenLabels[61] = Helper.CreateLabel(0, 16, 58, 1, "");
			screenLabels[62] = Helper.CreateLabel(0, 16, 58, 1, "");
			screenLabels[63] = Helper.CreateLabel(0, 16, 58, 1, "");
			screenLabels[64] = Helper.CreateLabel(0, 16, 58, 1, "");
			screenLabels[65] = Helper.CreateLabel(0, 16, 58, 1, "");
			screenLabels[66] = Helper.CreateLabel(0, 16, 58, 1, "");
			screenLabels[67] = Helper.CreateLabel(0, 16, 58, 1, "");
			screenLabels[68] = Helper.CreateLabel(0, 16, 58, 1, "");
			screenLabels[69] = Helper.CreateLabel(0, 16, 58, 1, "");

		}

		public override void resize()
		{
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				MET = screenStreams.GetData(DataType.vessel_MET);
				parts = screenStreams.GetData(DataType.vessel_parts);

				screenLabels[2].Text = "LT: " + Helper.timeString(DateTime.Now.TimeOfDay.TotalSeconds);
				screenLabels[3].Text = "MET: " + Helper.timeString(MET, 3);

				// FIND SOURCES, DRAINS, AND STORAGE
				if (parts != null)
				{
					List<string> storage = new List<string>();
					List<string> source = new List<string>();
					IList<Part> partList = parts.All;
					foreach (Part p in partList)
					{
						Resources r = p.Resources;

						if (r.HasResource("ElectricCharge"))
						{
							string s = "";
							s += Helper.prtlen(p.Title, 30, Helper.Align.RIGHT);
							s += Helper.prtlen(Helper.toFixed(r.Max("ElectricCharge"), 2), 10, Helper.Align.RIGHT);
							s += Helper.prtlen(Helper.toFixed(r.Amount("ElectricCharge"), 2), 10, Helper.Align.RIGHT);
							storage.Add(s);
						}

						SolarPanel panel = p.SolarPanel;
						if (panel != null)
						{
							string s = "";
							s += Helper.prtlen(p.Title, 30, Helper.Align.RIGHT);
							s += Helper.prtlen(Helper.toFixed(panel.EnergyFlow, 2), 10, Helper.Align.RIGHT);
							s += Helper.prtlen(Helper.toFixed(panel.SunExposure, 2), 10, Helper.Align.RIGHT);
							source.Add(s);
						}
					}

					// PRINT FIRST 10 OF EACH
					for (int i = 0; i < 10; i++)
					{
						if (storage.Count > i) screenLabels[60 + i].Text = storage[i];
						if (source.Count > i) screenLabels[10 + i].Text = source[i];
					}
				}
			}
		}
	}
}
