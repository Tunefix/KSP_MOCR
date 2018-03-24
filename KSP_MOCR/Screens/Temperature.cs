using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class Temperature : MocrScreen
	{
		Parts parts;
		List<Part> hotParts = new List<Part>();
		List<Part> ablators = new List<Part>();
		List<Part> radiators = new List<Part>();

		public Temperature(Screen form)
		{
			this.form = form;
			this.screenStreams = form.streamCollection;

			this.charSize = false;
			this.width = 656;
			this.height = 494;
			this.updateRate = 1000;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 125; i++) screenLabels.Add(null); // Initialize Labels

			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateCRTLabel(0, 0, 5, 1, "SCR 7");
			screenLabels[1] = Helper.CreateCRTLabel(15, 0, 34, 1, "TEMPERATURE / ABLATORS / RADIATORS", 4);
			screenLabels[2] = Helper.CreateCRTLabel(0, 1.5, 12, 1, "LT: XX:XX:XX");
			screenLabels[3] = Helper.CreateCRTLabel(29, 1.5, 14, 1, "MET: XXX:XX:XX");

			screenLabels[4] = Helper.CreateCRTLabel(0, 3, 72, 1, "──────────── HOT PARTS ────────────┬────────── ABLATOR PARTS ──────────");
			screenLabels[6] = Helper.CreateCRTLabel(36, 18, 72, 1, "────────── RADIATOR PARTS ─────────");

			// CENTER DIVISOR
			for(int i = 4; i < 34; i++)
			{
				screenLabels[80 + i] = Helper.CreateCRTLabel(35, i, 1, 1, "│");
			}

			// HOT PARTS LABLES
			for (int i = 0; i < 29; i++)
			{
				screenLabels[10 + i] = Helper.CreateCRTLabel(0, 4 + i, 35, 1, "");
			}

			// ABLATOR PARTS LABLES
			for (int i = 0; i < 14; i++)
			{
				screenLabels[40 + i] = Helper.CreateCRTLabel(36, 4 + i, 35, 1, "");
			}

			// RADIATOR PARTS LABLES
			for (int i = 0; i < 14; i++)
			{
				screenLabels[60 + i] = Helper.CreateCRTLabel(36, 19 + i, 35, 1, "");
			}
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			screenLabels[2].Text = "LT: " + Helper.timeString(DateTime.Now.TimeOfDay.TotalSeconds);

			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				double MET = screenStreams.GetData(DataType.vessel_MET);

				screenLabels[3].Text = "MET: " + Helper.timeString(MET, 3);

				parts = screenStreams.GetData(DataType.vessel_parts);

				hotParts.Clear();
				ablators.Clear();
				radiators.Clear();

				

				if (parts != null)
				{
					IList<Part> partList = parts.All;
					foreach (Part p in partList)
					{
						addHotPart(p);

						Radiator rad = p.Radiator;
						if(rad != null)
						{
							radiators.Add(p);
						}

						Resources r = p.Resources;
						if(r.HasResource("Ablator"))
						{
							ablators.Add(p);
						}
					}
				}

				printHotParts();
				printAblators();
				printRadiators();
			}
		}

		public override void resize()
		{
		}

		private void addHotPart(Part part)
		{
			// Find place to insert
			int index = 0;
			foreach(Part p in hotParts)
			{
				double temp = part.Temperature;
				double skinTemp = part.SkinTemperature;
				if (skinTemp > temp) temp = skinTemp;

				double Ptemp = p.Temperature;
				double PskinTemp = p.SkinTemperature;
				if (PskinTemp > Ptemp) Ptemp = PskinTemp;

				if (Ptemp < temp)
				{
					break;
				}
				index++;
			}
			hotParts.Insert(index, part);
		}

		private void printHotParts()
		{
			for(int i = 0; i < 29; i++)
			{
				if (hotParts.Count > i)
				{
					Part part = hotParts[i];
					string name = Helper.prtlen(part.Title, 26).ToUpper();

					double temp = part.Temperature;
					double maxTemp = part.MaxTemperature;
					double skinTemp = part.SkinTemperature;
					double maxSkinTemp = part.MaxSkinTemperature;
					if (skinTemp > temp)
					{
						temp = skinTemp;
						maxTemp = maxSkinTemp;
					}

					string tempS = Helper.prtlen(Helper.toFixed(temp, 1), 7, Helper.Align.RIGHT);
					string star = " ";
					if (temp > maxTemp * 0.5f) star = "*";
					screenLabels[10 + i].Text = name + tempS + star;
				}
				else
				{
					screenLabels[10 + i].Text = "";
				}
			}
		}

		private void printAblators()
		{
			for (int i = 0; i < 7; i++)
			{
				if (ablators.Count > i)
				{
					Part part = ablators[i];
					string name = Helper.prtlen(part.Title, 35, Helper.Align.CENTER).ToUpper();
					string temp = Helper.prtlen(Helper.toFixed(part.Temperature, 1), 6, Helper.Align.RIGHT);
					string star = " ";

					Resources r = part.Resources;
					float a = r.Amount("Ablator");
					string abl = Helper.prtlen(Helper.toFixed(a, 1), 6, Helper.Align.RIGHT);

					if (part.Temperature > part.MaxTemperature * 0.9f) star = "*";
					screenLabels[40 + (i * 2)].Text = name;
					screenLabels[41 + (i * 2)].Text = "   TEMP: " + temp + star + "  ABL: " + abl;
				}
				else
				{
					screenLabels[40 + (i * 2)].Text = "";
					screenLabels[41 + (i * 2)].Text = "";
				}
			}
		}

		private void printRadiators()
		{
			for (int i = 0; i < 7; i++)
			{
				if (radiators.Count > i)
				{
					Part part = radiators[i];
					Radiator rad = part.Radiator;
					string name = Helper.prtlen(part.Title, 35, Helper.Align.CENTER).ToUpper();
					string pos = Helper.prtlen(rad.State.ToString(), 12, Helper.Align.LEFT); // EXTENDED/RETRACTED/EXTENDING/RETRACTING/BROKEN
					string state = "";

					IList<Module> modules = part.Modules;
					foreach (Module m in modules)
					{
						if (m.Name == "ModuleActiveRadiator")
						{
							IDictionary<string, string> fields = m.Fields;
							state = fields["Cooling"];
						}
					}

					state = Helper.prtlen(state, 8, Helper.Align.RIGHT);

					screenLabels[60 + (i * 2)].Text = name;
					screenLabels[61 + (i * 2)].Text = pos + state;
				}
				else
				{
					screenLabels[60 + (i * 2)].Text = "";
					screenLabels[61 + (i * 2)].Text = "";
				}
			}
		}
	}
}
