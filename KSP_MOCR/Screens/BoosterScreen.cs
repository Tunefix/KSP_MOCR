using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using KRPC.Client;
using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;


namespace KSP_MOCR
{
	class BoosterScreen : MocrScreen
	{
		KRPC.Client.Services.SpaceCenter.Vessel vessel;
		KRPC.Client.Services.SpaceCenter.Flight flight;
		//KRPC.Client.Services.SpaceCenter.Orbit orbit;

		public BoosterScreen(Form1 form)
		{
			this.connection = form.connection;
			this.krpc = this.connection.KRPC();
			this.spaceCenter = this.connection.SpaceCenter();
			this.help = new KSP_MOCR.helper(form);
			this.form = form;
			this.chartData = form.chartData;

			this.width = 120;
			this.height = 30;
		}

		public override void destroyStreams()
		{

		}

		public override void makeElements()
		{
			for (int i = 0; i < 100; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 12; i++) screenIndicators.Add(null); // Initialize Indicators

			screenLabels[0] = help.CreateLabel(16, 1, 13); // Local Time
			screenLabels[1] = help.CreateLabel(0, 1, 14); // MET Time
			screenLabels[2] = help.CreateLabel(39, 0, 42, 1, "============= BOOSTER MODULE ============="); // Screen Title
			screenLabels[3] = help.CreateLabel(84, 0, 39, 1, "├───────────── STATUS ─────────────┤"); // Status Headline
			screenLabels[4] = help.CreateLabel(84, 1, 1, 1, "│");
			screenLabels[5] = help.CreateLabel(0, 2, 85, 1, "───────────────── ENGINES ──────────────────────────────────────────────────────────┤"); // Obrit/Position headline
			screenLabels[6] = help.CreateLabel(44, 3, 1, 1, "│");
			screenLabels[7] = help.CreateLabel(44, 4, 1, 1, "│");
			screenLabels[8] = help.CreateLabel(44, 5, 1, 1, "│");
			screenLabels[9] = help.CreateLabel(44, 6, 1, 1, "│");
			screenLabels[10] = help.CreateLabel(44, 7, 1, 1, "│");
			screenLabels[11] = help.CreateLabel(44, 8, 1, 1, "│");
			screenLabels[12] = help.CreateLabel(44, 9, 1, 1, "│");
			screenLabels[13] = help.CreateLabel(44, 10, 1, 1, "│");
			screenLabels[14] = help.CreateLabel(44, 11, 1, 1, "│");
			screenLabels[15] = help.CreateLabel(44, 12, 1, 1, "│");
			screenLabels[16] = help.CreateLabel(84, 3, 1, 1, "│");
			screenLabels[17] = help.CreateLabel(84, 4, 1, 1, "│");
			screenLabels[18] = help.CreateLabel(84, 5, 1, 1, "│");
			screenLabels[19] = help.CreateLabel(84, 6, 1, 1, "│");
			screenLabels[20] = help.CreateLabel(84, 7, 36, 1, "│┌─────────── SUPPLIES ────────────┐");
			screenLabels[21] = help.CreateLabel(84, 8, 1, 1, "│");
			screenLabels[22] = help.CreateLabel(84, 9, 1, 1, "│");
			screenLabels[23] = help.CreateLabel(84, 10, 1, 1, "│");
			screenLabels[24] = help.CreateLabel(84, 11, 1, 1, "│");
			screenLabels[25] = help.CreateLabel(84, 12, 1, 1, "│");
			screenLabels[26] = help.CreateLabel(44, 13, 1, 1, "│");
			screenLabels[27] = help.CreateLabel(44, 14, 1, 1, "│");
			screenLabels[28] = help.CreateLabel(84, 13, 1, 1, "│");
			screenLabels[29] = help.CreateLabel(84, 14, 1, 1, "│");



			// Engine data
			screenLabels[40] = help.CreateLabel(45, 3, 30, 1, "      THRUST  MAX THR  vISP"); // Headline
			screenLabels[41] = help.CreateLabel(45, 4, 30, 1, ""); // Engine 1
			screenLabels[42] = help.CreateLabel(45, 5, 30, 1, ""); // Engine 2
			screenLabels[43] = help.CreateLabel(45, 6, 30, 1, ""); // Engine 3
			screenLabels[44] = help.CreateLabel(45, 7, 30, 1, ""); // Engine 4
			screenLabels[45] = help.CreateLabel(45, 8, 30, 1, ""); // Engine 5
			screenLabels[46] = help.CreateLabel(45, 9, 30, 1, ""); // Engine 6
			screenLabels[47] = help.CreateLabel(45, 10, 30, 1, ""); // Engine 7
			screenLabels[48] = help.CreateLabel(45, 11, 30, 1, ""); // Engine 8
			screenLabels[49] = help.CreateLabel(45, 12, 30, 1, ""); // Engine 9

			// Weight and TWR
			screenLabels[30] = help.CreateLabel(45, 13, 30, 1, ""); // Weight
			screenLabels[31] = help.CreateLabel(45, 14, 30, 1, ""); // TWR

			// Supplies
			screenLabels[50] = help.CreateLabel(85, 8, 35, 1, "                     "); // Supply line 1
			screenLabels[51] = help.CreateLabel(85, 9, 35, 1, "                     "); // Supply line 2
			screenLabels[52] = help.CreateLabel(85, 10, 35, 1, "                     "); // Supply line 3
			screenLabels[53] = help.CreateLabel(85, 11, 35, 1, "                     "); // Supply line 4
			screenLabels[54] = help.CreateLabel(85, 12, 35, 1, "                     "); // Supply line 5
			screenLabels[55] = help.CreateLabel(85, 13, 35, 1, "                     "); // Supply line 5
			screenLabels[58] = help.CreateLabel(85, 14, 35, 1, "└─────────────────────────────────┘"); // Supply line 6

			// Status
			screenIndicators[0] = help.CreateIndicator(86, 1, 10, 1, "SAS");
			screenIndicators[1] = help.CreateIndicator(97, 1, 10, 1, "RCS");
			screenIndicators[2] = help.CreateIndicator(108, 1, 10, 1, "GEAR");
			screenIndicators[3] = help.CreateIndicator(86, 2, 10, 1, "BRAKES");
			screenIndicators[4] = help.CreateIndicator(97, 2, 10, 1, "LIGHTS");
			screenIndicators[5] = help.CreateIndicator(108, 2, 10, 1, "ABORT");
			screenIndicators[6] = help.CreateIndicator(86, 4, 10, 1, "POWER HI");
			screenIndicators[7] = help.CreateIndicator(97, 4, 10, 1, "G HIGH");
			screenIndicators[8] = help.CreateIndicator(108, 4, 10, 1, "LOX LOW");
			screenIndicators[9] = help.CreateIndicator(86, 5, 10, 1, "POWER LOW");
			screenIndicators[10] = help.CreateIndicator(97, 5, 10, 1, "MONO LOW");
			screenIndicators[11] = help.CreateIndicator(108, 5, 10, 1, "FUEL LOW");


			for (int i = 0; i < 1; i++) form.screenCharts.Add(null); // Initialize Charts

			// Gee-Force vs. Time Graph
			form.screenCharts[0] = help.CreateChart(60, 15, 60, 15, 0, 600);
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			// Re-usable data variable for graph data
			List<Dictionary<int, Nullable<double>>> data = new List<Dictionary<int, Nullable<double>>>();


			if (form.connected && krpc.CurrentGameScene == GameScene.Flight)
			{
				vessel = spaceCenter.ActiveVessel;
				flight = vessel.Flight();
				//orbit = vessel.Orbit;


				screenLabels[0].Text = " LT: " + help.timeString(DateTime.Now.TimeOfDay.TotalSeconds);
				screenLabels[1].Text = "MET: " + help.timeString(vessel.MET, 3);

				/** 
				 * Engines
				 **/

				//  Get parts in current stage
				screenLabels[50].Text = "Stage: " + vessel.Control.CurrentStage.ToString();
				int stage = vessel.Control.CurrentStage;
				bool foundEngine = false;
				double multiplier = 91;
				double maxDev = 0;

				double stageCurThr = 0;
				double stageMaxThr = 0;

				int n = 0;

				for (int i = stage; i >= 0; i--)
				{
					IList<Part> parts = vessel.Parts.InStage(i);
					foreach (Part part in parts)
					{
						Engine eng = part.Engine;
						if (eng != null)
						{
							foundEngine = true;
							Tuple<double, double, double> pos = part.Position(vessel.ReferenceFrame);

							double left = pos.Item1;
							double top = pos.Item3;

							double devX = Math.Abs(left);
							if(devX > maxDev) { maxDev = devX; }
							double devY = Math.Abs(top);
							if (devY > maxDev) { maxDev = devY; }

							if (screenEngines.Count < (n + 1))
							{
								screenEngines.Add(null);
								screenEngines[n] = help.CreateEngine(2, 3, 6, 3, (n + 1).ToString());
							}

							screenEngines[n].offsetX = left;
							screenEngines[n].offsetY = top;

							if (eng.Thrust > 0) { screenEngines[n].setStatus(true); }
							else { screenEngines[n].setStatus(false); }

							// Engine data
							screenLabels[41 + n].Text = help.prtlen((n + 1).ToString(), 2, helper.Align.RIGHT) + ":"
								+ "  " + help.prtlen(help.toFixed(eng.Thrust/1000, 1), 7, helper.Align.RIGHT)
								+ "  " + help.prtlen(help.toFixed(eng.MaxThrust/1000, 1), 7, helper.Align.RIGHT)
								+ "  " + help.prtlen(help.toFixed(eng.VacuumSpecificImpulse, 1), 5, helper.Align.RIGHT);

							stageCurThr += eng.Thrust;
							stageMaxThr += eng.MaxThrust;

							n++;
						}
					}

					if (foundEngine)
					{ 
						if (maxDev > 1)
						{
							multiplier = 91 / maxDev;
						}

						// position indicators
						for(int j = 0; j < n; j++)
						{
							int x = (int)Math.Round(167 + (screenEngines[j].offsetX * multiplier));
							int y = (int)Math.Round(157 + (screenEngines[j].offsetY * multiplier));
							screenEngines[j].Location = new Point(x, y);
						}

						break;
					}
				}

				// Disable other engineIndicators
				while(n < screenEngines.Count)
				{
					screenEngines[n].Dispose();
					screenLabels[41 + n].Text = "";
					n++;
				}

				// Weight and TWR
				double weight = vessel.Mass / 1000;
				double TWRc = (stageCurThr / 1000) / (weight * 9.81);
				double TWRm = (stageMaxThr / 1000) / (weight * 9.81); ;
				screenLabels[30].Text = "Weight: " + help.prtlen(help.toFixed(weight, 1), 5, helper.Align.RIGHT) + "t";
				screenLabels[31].Text = "   TWR: " + help.prtlen(help.toFixed(TWRc, 2), 4, helper.Align.RIGHT)
					+ "  " + help.prtlen(help.toFixed(TWRm, 2), 4, helper.Align.RIGHT);


				// Supplies
				/*
				double mF = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("LiquidFuel");
				double cF = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("LiquidFuel");

				double mO = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("Oxidizer");
				double cO = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("Oxidizer");

				double mM = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("MonoPropellant");
				double cM = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("MonoPropellant");

				double mE = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("ElectricCharge");
				double cE = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("ElectricCharge");

				//screenLabels[50].Text = "         LF     LO     MP     EC";
				/*screenLabels[51].Text = "STAGE:"
										+ help.prtlen(Math.Round(cF).ToString(), 7, helper.Align.RIGHT)
										+ help.prtlen(Math.Round(cO).ToString(), 7, helper.Align.RIGHT)
										+ help.prtlen(help.toFixed(cM, 2), 7, helper.Align.RIGHT)
										+ help.prtlen(Math.Round(cE).ToString(), 7, helper.Align.RIGHT);
				screenLabels[52].Text = "    %:"
										+ help.prtlen(Math.Round((cF / mF) * 100).ToString(), 7, helper.Align.RIGHT)
										+ help.prtlen(Math.Round((cO / mO) * 100).ToString(), 7, helper.Align.RIGHT)
										+ help.prtlen(Math.Round((cM / mM) * 100).ToString(), 7, helper.Align.RIGHT)
										+ help.prtlen(Math.Round((cE / mE) * 100).ToString(), 7, helper.Align.RIGHT);

				mF = vessel.Resources.Max("LiquidFuel");
				cF = vessel.Resources.Amount("LiquidFuel");

				mO = vessel.Resources.Max("Oxidizer");
				cO = vessel.Resources.Amount("Oxidizer");

				mM = vessel.Resources.Max("MonoPropellant");
				cM = vessel.Resources.Amount("MonoPropellant");

				mE = vessel.Resources.Max("ElectricCharge");
				cE = vessel.Resources.Amount("ElectricCharge");

				screenLabels[54].Text = "  TOT:"
										+ help.prtlen(Math.Round(cF).ToString(), 7, helper.Align.RIGHT)
										+ help.prtlen(Math.Round(cO).ToString(), 7, helper.Align.RIGHT)
										+ help.prtlen(help.toFixed(cM, 2), 7, helper.Align.RIGHT)
										+ help.prtlen(Math.Round(cE).ToString(), 7, helper.Align.RIGHT);
				screenLabels[55].Text = "    %:"
										+ help.prtlen(Math.Round((cF / mF) * 100).ToString(), 7, helper.Align.RIGHT)
										+ help.prtlen(Math.Round((cO / mO) * 100).ToString(), 7, helper.Align.RIGHT)
										+ help.prtlen(Math.Round((cM / mM) * 100).ToString(), 7, helper.Align.RIGHT)
										+ help.prtlen(Math.Round((cE / mE) * 100).ToString(), 7, helper.Align.RIGHT);
				*/

				// Status
				if (vessel.Control.SAS) { screenIndicators[0].setStatus(1); } else { screenIndicators[0].setStatus(0); } // SAS
				if (vessel.Control.RCS) { screenIndicators[1].setStatus(1); } else { screenIndicators[1].setStatus(0); } // RCS
				if (vessel.Control.Gear) { screenIndicators[2].setStatus(1); } else { screenIndicators[2].setStatus(0); } // GEAR
				if (vessel.Control.Brakes) { screenIndicators[3].setStatus(2); } else { screenIndicators[3].setStatus(0); } // Break
				if (vessel.Control.Lights) { screenIndicators[4].setStatus(4); } else { screenIndicators[4].setStatus(0); } // Lights
				if (vessel.Control.Abort) { screenIndicators[5].setStatus(2); } else { screenIndicators[5].setStatus(0); } // Abort

				if (flight.GForce > 4) { screenIndicators[7].setStatus(4); } else { screenIndicators[7].setStatus(0); } // G High

				float maxR = vessel.Resources.Max("ElectricCharge");
				float curR = vessel.Resources.Amount("ElectricCharge");
				if (curR / maxR > 0.95) { screenIndicators[6].setStatus(1); } else { screenIndicators[6].setStatus(0); } // Power High
				if (curR / maxR < 0.1) { screenIndicators[9].setStatus(2); } else { screenIndicators[9].setStatus(0); } // Power Low

				maxR = vessel.Resources.Max("MonoPropellant");
				curR = vessel.Resources.Amount("MonoPropellant");
				if (curR / maxR < 0.1) { screenIndicators[10].setStatus(2); } else { screenIndicators[10].setStatus(0); } // Monopropellant Low

				maxR = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("LiquidFuel");
				curR = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("LiquidFuel");
				if (curR / maxR < 0.1) { screenIndicators[11].setStatus(2); } else { screenIndicators[11].setStatus(0); } // Fuel Low

				maxR = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("Oxidizer");
				curR = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("Oxidizer");
				if (curR / maxR < 0.1) { screenIndicators[8].setStatus(2); } else { screenIndicators[8].setStatus(0); } // LOW Low


				// Graphs
				data = new List<Dictionary<int, Nullable<double>>>();
				data.Add(chartData["geeTime"]);
				form.showData(0, data, false);
			}
		}
	}
}
