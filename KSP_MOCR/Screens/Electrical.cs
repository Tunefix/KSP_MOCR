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
	class Electrical : MocrScreen
	{
		double MET = 0;
		Parts parts;

		Dictionary<ulong, double> storagePrevLvl = new Dictionary<ulong, double>();

		public Electrical(Screen form)
		{
			this.form = form;
			this.screenStreams = form.streamCollection;

			this.charSize = false;
			this.width = 674;
			this.height = 508;
			this.updateRate = 1000;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 80; i++) screenLabels.Add(null); // Initialize Labels

			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateCRTLabel(0, 0, 5, 1, "SCR 6");
			screenLabels[1] = Helper.CreateCRTLabel(21, 0, 30, 1, "SPACECRAFT ELECTRICAL SYSTEMS");
			screenLabels[2] = Helper.CreateCRTLabel(0, 1, 12, 1, "LT: XX:XX:XX");
			screenLabels[3] = Helper.CreateCRTLabel(29, 1, 14, 1, "MET: XXX:XX:XX");

			screenLabels[4] = Helper.CreateCRTLabel(0, 3, 72, 1, "─── SOLAR PANELS ──── GEN ─── SUN ─┬────────────── TOTALS ──────────────");
			screenLabels[5] = Helper.CreateCRTLabel(0, 18, 72, 1, "─────────────── STORAGE ──────────────── CAP ───── LVL ───── FLW ──────");
			screenLabels[6] = Helper.CreateCRTLabel(36, 4, 36, 1, "                GEN     SUN");
			screenLabels[7] = Helper.CreateCRTLabel(36, 5, 36, 1, "     SOLAR:   XXXX.XX    X.XX");
			screenLabels[8] = Helper.CreateCRTLabel(36, 7, 36, 1, "                CAP     LVL     FLW");
			screenLabels[9] = Helper.CreateCRTLabel(36, 8, 36, 1, "   STORAGE:  XXXX.XX XXXX.XX XXXX.XX");
			//screenLabels[7] = Helper.CreateCRTLabel(1, 15, 58, 1, "──────────── STORAGE ───────────── CAP ────── LVL ──────");

			for (int i = 4; i < 17; i++)
			{
				// FROM 10
				screenLabels[i + 6] = Helper.CreateCRTLabel(0, i, 35, 1, "");

				// FROM 50
				screenLabels[i + 46] = Helper.CreateCRTLabel(35, i, 1, 1, "│");

				// FROM 30
				screenLabels[i + 26] = Helper.CreateCRTLabel(0, i + 15, 72, 1, "");
			}

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
					Dictionary<ulong, string> storage = new Dictionary<ulong, string>();
					List<string> source = new List<string>();

					double solarGenSum = 0;
					double solarSunSum = 0;
					double storageCapSum = 0;
					double storageLvlSum = 0;
					double storageFlwSum = 0;


					IList<Part> partList = parts.All;

					

					/*Console.WriteLine("*************");
					Console.WriteLine("* PART LIST *");
					Console.WriteLine("*************");
					*/

					foreach (Part p in partList)
					{
						Resources r = p.Resources;

						/*Console.WriteLine("");
						Console.WriteLine("  === " + p.Title + " ===");
						Console.WriteLine("    STAGE : " + p.Stage.ToString());

						Console.WriteLine("    == MODULES ==");
						IList<Module> m = p.Modules;
						foreach(Module mod in m)
						{
							Console.WriteLine("    " + mod.Name);
							IDictionary<string, string> f =  mod.Fields;
							foreach(KeyValuePair<string, string> kvp in f)
							{
								Console.WriteLine("      " + kvp.Key + ": " + kvp.Value);
							}
						}

						Console.WriteLine("    == RESOURCES ==");
						
						IList<Resource> ra = r.All;
						foreach (Resource res in ra)
						{
							Console.WriteLine("    " + res.Name);
						}
						*/




						if (r.HasResource("ElectricCharge"))
						{
							ulong id = p.id;
							double flow = 0;
							float curAmount = r.Amount("ElectricCharge");
							float maxAmount = r.Max("ElectricCharge");

							// FIND FLOW AND STORE CURRENT LEVEL
							if (storagePrevLvl.ContainsKey(id))
							{
								flow = curAmount - storagePrevLvl[id];
								storagePrevLvl[id] = curAmount;
							}
							else
							{
								storagePrevLvl.Add(id, curAmount);
							}

							// Add to totalt
							storageCapSum += maxAmount;
							storageLvlSum += curAmount;
							storageFlwSum += flow;

							string s = "";
							s += Helper.prtlen(p.Title, 35, Helper.Align.RIGHT);
							s += Helper.prtlen(Helper.toFixed(maxAmount, 2), 10, Helper.Align.RIGHT);
							s += Helper.prtlen(Helper.toFixed(curAmount, 2), 10, Helper.Align.RIGHT);
							s += Helper.prtlen(Helper.toFixed(flow, 2), 10, Helper.Align.RIGHT);
							
							storage.Add(id, s);
						}

						SolarPanel panel = p.SolarPanel;
						if (panel != null)
						{
							float flow = panel.EnergyFlow;
							float sun = panel.SunExposure;

							// Add to totalt
							solarGenSum += flow;
							solarSunSum += sun;


							string s = "";
							s += Helper.prtlen(p.Title, 18, Helper.Align.RIGHT);
							s += Helper.prtlen(Helper.toFixed(flow, 2), 8, Helper.Align.RIGHT);
							s += Helper.prtlen(Helper.toFixed(sun, 2), 8, Helper.Align.RIGHT);
							source.Add(s);
						}
					}

					// PRINT FIRST 13 OF EACH
					for (int i = 0; i < 13; i++)
					{
						if (source.Count > i) screenLabels[10 + i].Text = source[i];
					}

					int n = 0;
					foreach(KeyValuePair<ulong, string> kvp in storage)
					{
						if(n < 13)
						{
							if(storage.Count > n) screenLabels[30 + n].Text = kvp.Value;
							n++;
						}
						else
						{
							break;
						}
					}

					// PRINT TOTALS
					screenLabels[7].Text = "     SOLAR: " + Helper.prtlen(Helper.toFixed(solarGenSum, 2), 8, Helper.Align.RIGHT) + Helper.prtlen(Helper.toFixed(solarSunSum, 2), 8, Helper.Align.RIGHT);

					string storSums = "   STORAGE: ";
					storSums += Helper.prtlen(Helper.toFixed(storageCapSum, 2), 8, Helper.Align.RIGHT);
					storSums += Helper.prtlen(Helper.toFixed(storageLvlSum, 2), 8, Helper.Align.RIGHT);
					storSums += Helper.prtlen(Helper.toFixed(storageFlwSum, 2), 8, Helper.Align.RIGHT);
					screenLabels[9].Text = storSums;
				}
			}
		}
	}
}
