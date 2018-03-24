using System;
using System.Collections.Generic;
using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;

namespace KSP_MOCR
{
	class ResourcesScreen : MocrScreen
	{
		Parts parts;
		List<Tuple<int, string>> partStrings = new List<Tuple<int, string>>();

		public ResourcesScreen(Screen form)
		{
			this.form = form;

			screenStreams = form.streamCollection;

			this.charSize = false;
			this.width = 656;
			this.height = 494;
			this.updateRate = 1000;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			screenLabels[2].Text = "LT: " + Helper.timeString(DateTime.Now.TimeOfDay.TotalSeconds);

			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				double MET = screenStreams.GetData(DataType.vessel_MET);

				screenLabels[3].Text = "MET: " + Helper.timeString(MET, 3);

				parts = screenStreams.GetData(DataType.vessel_parts);

				partStrings.Clear();

				double totFuel = 0;
				double totLox = 0;
				double totMono = 0;

				if (parts != null)
				{
					IList<Part> partList = parts.All;
					foreach (Part p in partList)
					{
						Resources r = p.Resources;

						if (r.HasResource("MonoPropellant") || r.HasResource("LiquidFuel") || r.HasResource("Oxidizer"))
						{
							float fuelA = r.Amount("LiquidFuel");
							float loxA = r.Amount("Oxidizer");
							float monoA = r.Amount("MonoPropellant");

							totFuel += fuelA;
							totLox += loxA;
							totMono += monoA;

							float fuelM = r.Max("LiquidFuel");
							float loxM = r.Max("Oxidizer");
							float monoM = r.Max("MonoPropellant");

							string name = Helper.prtlen(p.Title, 40).ToUpper();
							int stage = p.DecoupleStage;
							string stageStr = Helper.prtlen(stage.ToString(), 3, Helper.Align.RIGHT);
							string fuel = Helper.prtlen(Helper.toFixed(fuelA, 1), 8, Helper.Align.RIGHT);
							string lox = Helper.prtlen(Helper.toFixed(loxA, 1), 8, Helper.Align.RIGHT);
							string mono = Helper.prtlen(Helper.toFixed(monoA, 2), 7, Helper.Align.RIGHT);

							float fuelP = (fuelA / fuelM) * 100f;
							if (fuelM == 0) fuelP = 0;
							float loxP = (loxA / loxM) * 100f;
							if (loxM == 0) loxP = 0;
							float monoP = (monoA / monoM) * 100f;
							if (monoM == 0) monoP = 0;

							string fuelS = Helper.prtlen(Helper.toFixed(fuelP, 2), 7, Helper.Align.RIGHT) + "%";
							string loxS = Helper.prtlen(Helper.toFixed(loxP, 2), 7, Helper.Align.RIGHT) + "%";
							string monoS = Helper.prtlen(Helper.toFixed(monoP, 2), 7, Helper.Align.RIGHT) + "%";

							partStrings.Add(new Tuple<int, string>(stage, stageStr + " " + name + fuel + fuelS + lox + loxS + mono + monoS));
						}
					}
				}

				// SORT THE LIST
				partStrings.Sort((x, y) => y.Item1.CompareTo(x.Item1));

				for (int i = 5; i < 31; i++)
				{
					if (partStrings.Count > i - 5)
					{
						screenLabels[i].Text = partStrings[i - 5].Item2;
					}
					else
					{
						screenLabels[i].Text = "";
					}
				}

				// PRINT TOTALS (123456.0   123456.0   12345.00)
				string totF = Helper.prtlen(Helper.toFixed(totFuel, 1), 11, Helper.Align.RIGHT);
				string totL = Helper.prtlen(Helper.toFixed(totLox, 1), 11, Helper.Align.RIGHT);
				string totM = Helper.prtlen(Helper.toFixed(totMono, 2), 11, Helper.Align.RIGHT);
				screenLabels[52].Text = totF + totL + totM;

				screenLabels[55].Text = "CURRENT STAGE: " + screenStreams.GetData(DataType.control_currentStage).ToString();

			}
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenCharts.Add(null); // Initialize Charts
			for (int i = 0; i < 100; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs

			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix


			screenLabels[0] = Helper.CreateCRTLabel(0, 0, 5, 1, "SCR 3");
			screenLabels[1] = Helper.CreateCRTLabel(27, 0, 30, 1, "CONSUMABLES", 4);
			screenLabels[2] = Helper.CreateCRTLabel(0, 1.5, 12, 1, "LT: XX:XX:XX");
			screenLabels[3] = Helper.CreateCRTLabel(29, 1.5, 14, 1, "MET: XXX:XX:XX");

			screenLabels[4] = Helper.CreateCRTLabel(0, 3, 72, 1, " STG ──────────── PART ─────────────── FUEL ─────── LOX ─────── MONO ──");

			for(int i = 5; i < 37; i++)
			{
				screenLabels[i] = Helper.CreateCRTLabel(0, i + 0.5f, 96, 1, "", 2);
			}

			screenLabels[50] = Helper.CreateCRTLabel(0, 30, 72, 1, "─────────────────────────────────────── FUEL ────── LOX ──── MONO ─────");
			screenLabels[51] = Helper.CreateCRTLabel(25, 32, 8, 1, "TOTALS:");
			screenLabels[52] = Helper.CreateCRTLabel(34, 32, 38, 1, "");

			screenLabels[55] = Helper.CreateCRTLabel(0, 32, 25, 1, "");
		}

		public override void resize() { }
	}
}
