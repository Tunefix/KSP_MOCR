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

		int currentStage;
		int oldStage;
		ReferenceFrame vesselReferenceFrame;

		float LiquidFuelMax = 0;
		float LiquidFuelAmount = 0;
		float OxidizerMax = 0;
		float OxidizerAmount = 0;
		float MonoPropellantMax = 0;
		float MonoPropellantAmount = 0;
		float TotLiquidFuelMax = 0;
		float TotLiquidFuelAmount = 0;
		float TotOxidizerMax = 0;
		float TotOxidizerAmount = 0;
		float TotMonoPropellantMax = 0;
		float TotMonoPropellantAmount = 0;

		double mass = 0;

		double MET = 0;
		Parts parts;

		double g = 8.80556;

		public BoosterScreen(Screen form)
		{
			this.form = form;
			this.chartData = form.form.chartData;
			
			screenStreams = form.streamCollection;

			this.charSize = false;
			this.width = 656;
			this.height = 494;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 100; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 12; i++) screenIndicators.Add(null); // Initialize Indicators
			for (int i = 0; i < 2; i++) screenInputs.Add(null); // Initialize Inputs

			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix



			screenLabels[0] = Helper.CreateCRTLabel(0, 0, 5, 1, "SCR 2");
			screenLabels[1] = Helper.CreateCRTLabel(25, 0, 42, 1, "BOOSTER SYSTEMS", 4); // Screen Title
			screenLabels[2] = Helper.CreateCRTLabel(0, 1.5, 12, 1, "LT: XX:XX:XX");
			screenLabels[3] = Helper.CreateCRTLabel(29, 1.5, 14, 1, "MET: XXX:XX:XX");
			
			screenLabels[4] = Helper.CreateCRTLabel(84, 1, 1, 1, "│");
			screenLabels[5] = Helper.CreateCRTLabel(0, 3, 85, 1, "───────────────── ENGINES ─────────────────────────────────────────────"); // Obrit/Position headline

			screenLabels[6] = Helper.CreateCRTLabel(0, 4, 20, 1, ""); // CURRENT STAGE

			screenLabels[7] = Helper.CreateCRTLabel(40, 4, 1, 1, "│");
			screenLabels[8] = Helper.CreateCRTLabel(40, 5, 1, 1, "│");
			screenLabels[9] = Helper.CreateCRTLabel(40, 6, 1, 1, "│");
			screenLabels[10] = Helper.CreateCRTLabel(40, 7, 1, 1, "│");
			screenLabels[11] = Helper.CreateCRTLabel(40, 8, 1, 1, "│");
			screenLabels[12] = Helper.CreateCRTLabel(40, 9, 1, 1, "│");
			screenLabels[13] = Helper.CreateCRTLabel(40, 10, 1, 1, "│");
			screenLabels[14] = Helper.CreateCRTLabel(40, 11, 1, 1, "│");
			screenLabels[15] = Helper.CreateCRTLabel(40, 12, 1, 1, "│");
			screenLabels[16] = Helper.CreateCRTLabel(40, 13, 1, 1, "│");
			screenLabels[17] = Helper.CreateCRTLabel(40, 14, 1, 1, "│");
			screenLabels[18] = Helper.CreateCRTLabel(40, 15, 1, 1, "│");
			screenLabels[19] = Helper.CreateCRTLabel(40, 16, 1, 1, "│");
			screenLabels[20] = Helper.CreateCRTLabel(40, 17, 1, 1, "│");
			screenLabels[21] = Helper.CreateCRTLabel(40, 18, 1, 1, "│");
			screenLabels[22] = Helper.CreateCRTLabel(40, 19, 1, 1, "│");
			screenLabels[23] = Helper.CreateCRTLabel(40, 20, 1, 1, "│");
			screenLabels[24] = Helper.CreateCRTLabel(40, 21, 1, 1, "│");
			screenLabels[25] = Helper.CreateCRTLabel(40, 22, 1, 1, "│");
			screenLabels[26] = Helper.CreateCRTLabel(40, 23, 1, 1, "│");
			screenLabels[27] = Helper.CreateCRTLabel(40, 24, 1, 1, "│");
			screenLabels[28] = Helper.CreateCRTLabel(40, 25, 1, 1, "│");
			screenLabels[29] = Helper.CreateCRTLabel(40, 26, 1, 1, "│");
			screenLabels[30] = Helper.CreateCRTLabel(40, 27, 1, 1, "│");
			screenLabels[31] = Helper.CreateCRTLabel(40, 28, 1, 1, "│");
			screenLabels[32] = Helper.CreateCRTLabel(40, 29, 1, 1, "│");
			screenLabels[33] = Helper.CreateCRTLabel(40, 30, 1, 1, "│");



			// Engine data
			screenLabels[40] = Helper.CreateCRTLabel(41, 4, 30, 1, "      THRUST  MAX THR   vISP"); // Headline
			screenLabels[41] = Helper.CreateCRTLabel(41, 5, 30, 1, ""); // Engine 1
			screenLabels[42] = Helper.CreateCRTLabel(41, 6, 30, 1, ""); // Engine 2
			screenLabels[43] = Helper.CreateCRTLabel(41, 7, 30, 1, ""); // Engine 3
			screenLabels[44] = Helper.CreateCRTLabel(41, 8, 30, 1, ""); // Engine 4
			screenLabels[45] = Helper.CreateCRTLabel(41, 9, 30, 1, ""); // Engine 5
			screenLabels[46] = Helper.CreateCRTLabel(41, 10, 30, 1, ""); // Engine 6
			screenLabels[47] = Helper.CreateCRTLabel(41, 11, 30, 1, ""); // Engine 7
			screenLabels[48] = Helper.CreateCRTLabel(41, 12, 30, 1, ""); // Engine 8
			screenLabels[49] = Helper.CreateCRTLabel(41, 13, 30, 1, ""); // Engine 9
			screenLabels[50] = Helper.CreateCRTLabel(41, 14, 30, 1, "");
			screenLabels[51] = Helper.CreateCRTLabel(41, 14, 30, 1, "");
			screenLabels[52] = Helper.CreateCRTLabel(41, 15, 30, 1, "");
			screenLabels[53] = Helper.CreateCRTLabel(41, 16, 30, 1, "");
			screenLabels[54] = Helper.CreateCRTLabel(41, 17, 30, 1, "");
			screenLabels[55] = Helper.CreateCRTLabel(41, 18, 30, 1, "");
			screenLabels[56] = Helper.CreateCRTLabel(41, 19, 30, 1, "");
			screenLabels[57] = Helper.CreateCRTLabel(41, 20, 30, 1, "");
			screenLabels[58] = Helper.CreateCRTLabel(41, 21, 30, 1, "");
			screenLabels[59] = Helper.CreateCRTLabel(41, 22, 30, 1, "");
			screenLabels[60] = Helper.CreateCRTLabel(41, 23, 30, 1, ""); // Engine 20

			// Weight and TWR
			screenLabels[61] = Helper.CreateCRTLabel(0, 26, 41, 1, "────────────────────────────────────────");
			screenLabels[62] = Helper.CreateCRTLabel(0, 28, 14, 1, " MASS: "); // Weight
			screenLabels[63] = Helper.CreateCRTLabel(14, 27, 30, 1, "         CUR   LOW   MAX");
			screenLabels[64] = Helper.CreateCRTLabel(14, 28, 30, 1, ""); // TWR

			screenLabels[40] = Helper.CreateCRTLabel(20, 31, 1, 1, "│");
			screenLabels[40] = Helper.CreateCRTLabel(20, 32, 1, 1, "│");
			screenLabels[40] = Helper.CreateCRTLabel(20, 33, 1, 1, "│");
			screenLabels[40] = Helper.CreateCRTLabel(20, 34, 1, 1, "│");


			// Supplies
			screenLabels[69] = Helper.CreateCRTLabel(40, 24, 35, 1, "├─────────── SUPPLIES ────────────┐");
			screenLabels[70] = Helper.CreateCRTLabel(42, 25, 35, 1, "                     "); // Supply line 1
			screenLabels[71] = Helper.CreateCRTLabel(42, 26, 35, 1, "                     "); // Supply line 2
			screenLabels[72] = Helper.CreateCRTLabel(42, 27, 35, 1, "                     "); // Supply line 3
			screenLabels[73] = Helper.CreateCRTLabel(42, 28, 35, 1, "                     "); // Supply line 4
			screenLabels[74] = Helper.CreateCRTLabel(42, 29, 35, 1, "                     "); // Supply line 5
			screenLabels[75] = Helper.CreateCRTLabel(42, 30, 35, 1, "                     "); // Supply line 5
			screenLabels[78] = Helper.CreateCRTLabel(40, 31, 35, 1, "└─────────────────────────────────┘"); // Supply line 6
			

			// DELTA V BUDGET
			//screenLabels[80] = Helper.CreateCRTLabel(19, 30, 23, 1, "┬── DELTA V AVAIL. ──");
			screenLabels[81] = Helper.CreateCRTLabel(22, 31.5, 23, 1, "STAGE: xxxxx.x m/s");
			screenLabels[82] = Helper.CreateCRTLabel(22, 32.5, 23, 1, "TOTAL: xxxxx.x m/s");
			
			screenLabels[83] = Helper.CreateCRTLabel(23, 26, 15, 1, "");
			
			screenLabels[85] = Helper.CreateCRTLabel(0, 30, 52, 1, "── BURN DURATION ───┬── DELTA V AVAIL. ─");
			screenLabels[86] = Helper.CreateCRTLabel(0, 31.5, 10, 1, " REQ'D ΔV:");
			screenLabels[84] = Helper.CreateCRTLabel(0, 33, 20, 1, "BURN TIME:");
			
			screenInputs[1] = Helper.CreateInput(99, 444, 72, 17, HorizontalAlignment.Right,true);
			screenInputs[1].Text = "0";
			
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			// Re-usable data variable for graph data
			List<List<KeyValuePair<double, double?>>> data = new List<List<KeyValuePair<double, double?>>>();
			List<Plot.Type> types = new List<Plot.Type>();


			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				currentStage = screenStreams.GetData(DataType.control_currentStage);
				MET = screenStreams.GetData(DataType.vessel_MET);
				parts = screenStreams.GetData(DataType.vessel_parts);
				vesselReferenceFrame = screenStreams.GetData(DataType.vessel_referenceFrame);

				bool force = false;
				if (currentStage != oldStage)
				{
					force = true;
					screenStreams.setStage(currentStage);
					Console.WriteLine(DateTime.Now.ToString() + "." + DateTime.Now.Millisecond.ToString() + " NEW STAGE: " + currentStage.ToString());
				}

				getStageSupplies(force);
				if(LiquidFuelMax == 0)
				{
					// TRY TO WALK TROUGH THE STAGES UNTIL FUEL IS FOUND OR STAGES RUN OUT
					for(int s = currentStage - 1; s >= 0; s--)
					{
						screenStreams.setStage(s);
						Console.WriteLine(DateTime.Now.ToString() + "." + DateTime.Now.Millisecond.ToString() + " SET STREAM STAGE: " + s.ToString());
						getStageSupplies(true);
						if(LiquidFuelMax != 0)
						{
							oldStage = currentStage;
							break;
						}
					}
				}


				TotLiquidFuelMax = screenStreams.GetData(DataType.resource_total_max_liquidFuel);
				TotLiquidFuelAmount = screenStreams.GetData(DataType.resource_total_amount_liquidFuel);
				TotOxidizerMax = screenStreams.GetData(DataType.resource_total_max_oxidizer);
				TotOxidizerAmount = screenStreams.GetData(DataType.resource_total_amount_oxidizer);
				TotMonoPropellantMax = screenStreams.GetData(DataType.resource_total_max_monoPropellant);
				TotMonoPropellantAmount = screenStreams.GetData(DataType.resource_total_amount_monoPropellant);

				mass = screenStreams.GetData(DataType.vessel_mass);

				

				screenLabels[2].Text = "LT: " + Helper.timeString(DateTime.Now.TimeOfDay.TotalSeconds);
				screenLabels[3].Text = "MET: " + Helper.timeString(MET, 3); // 0 RPC

				/** 
				 * Engines
				 **/

				//  Get parts in current stage
				screenLabels[6].Text = "STAGE: " + currentStage.ToString(); // 0 RPC

				bool foundEngine = false;
				double multiplier = 91;
				double maxDev = 0;
				

				double stageCurThr = 0;
				double stageMaxThr = 0;

				int n = 0;



				if (parts != null)
				{
					// CLEAR OLD ENGINES
					foreach(EngineIndicator ei in screenEngines)
					{
						ei.display(false);
					}

					for (int i = currentStage; i >= 0; i--)
					{
						IList<Part> stageParts = parts.InStage(i);
						foreach (Part part in stageParts)
						{
							Engine eng = part.Engine;
							
							if (eng != null)
							{
								foundEngine = true;
								Tuple<double, double, double> pos = part.Position(vesselReferenceFrame);

								double left = pos.Item1;
								double top = pos.Item3;

								double devX = Math.Abs(left);
								if (devX > maxDev) { maxDev = devX; }
								double devY = Math.Abs(top);
								if (devY > maxDev) { maxDev = devY; }

								if (screenEngines.Count < (n + 1))
								{
									screenEngines.Add(null);
									screenEngines[n] = Helper.CreateEngine(0, 0, (n + 1).ToString(), eng.MaxVacuumThrust);
								}
									screenEngines[n].display(true);
									screenEngines[n].setThrust(eng.MaxVacuumThrust);
								

								screenEngines[n].offsetX = left;
								screenEngines[n].offsetY = top;

								if (eng.Thrust > 0) { screenEngines[n].setStatus(true); }
								else { screenEngines[n].setStatus(false); }

								// Engine data
								screenLabels[41 + n].Text = Helper.prtlen((n + 1).ToString(), 4, Helper.Align.RIGHT) + ":"
									+ "" + Helper.prtlen(Helper.toFixed(eng.Thrust / 1000, 1), 7, Helper.Align.RIGHT)
									+ "  " + Helper.prtlen(Helper.toFixed(eng.MaxThrust / 1000, 1), 7, Helper.Align.RIGHT)
									+ "  " + Helper.prtlen(Helper.toFixed(eng.VacuumSpecificImpulse, 1), 5, Helper.Align.RIGHT);

								stageCurThr += eng.Thrust;
								stageMaxThr += eng.MaxThrust;

								n++;
							}
						}

						if(foundEngine)
						{ 
							int engNum = n;

							// TOTAL THUST AND STUFF
							screenLabels[41 + n].Text = "─────────────────────────────────";
							n++;
							screenLabels[41 + n].Text = " TOT:"
										+ "" + Helper.prtlen(Helper.toFixed(stageCurThr / 1000, 1), 7, Helper.Align.RIGHT)
										+ "  " + Helper.prtlen(Helper.toFixed(stageMaxThr / 1000, 1), 7, Helper.Align.RIGHT);
							n++;

							// CLEAR OUT OLD LABLED
							while(n < 20)
							{
								screenLabels[41 + n].Text = "";
								n++;
							}


							int maxSpread = 96;
							multiplier = maxSpread / maxDev;

							if (engNum == 1)
							{
								multiplier = 0;
							}

							// CENTER POINT
							int centerX = 184;
							int centerY = 214;

							// position indicators
							for (int j = 0; j < engNum; j++)
							{
								int x = (int)Math.Round(centerX + (screenEngines[j].offsetX * multiplier));
								int y = (int)Math.Round(centerY + (screenEngines[j].offsetY * multiplier));
								screenEngines[j].setCenterPoint(x, y);
							}

							break;
						}
					}
				}

				// Disable other engineIndicators
				while(n < screenEngines.Count)
				{
					screenEngines[n].Dispose();
					screenEngines.RemoveAt(n);
					screenLabels[41 + n].Text = "";
					n++;
				}
				screenEngines.TrimExcess();
				
				// Weight and TWR
				double weight = mass / 1000;
				double TWRc = (stageCurThr / 1000) / (weight * 9.81);
				double TWRm = (stageMaxThr / 1000) / (weight * 9.81); ;
				screenLabels[62].Text = "  MASS: " + Helper.prtlen(Helper.toFixed(weight, 1), 5, Helper.Align.RIGHT) + "t";
				screenLabels[64].Text = "   TWR: " + Helper.prtlen(Helper.toFixed(TWRc, 2), 4, Helper.Align.RIGHT)
					+ "  " + Helper.prtlen(Helper.toFixed(TWRm, 2), 4, Helper.Align.RIGHT);


				// Supplies
				double mF = LiquidFuelMax;
				double cF = LiquidFuelAmount;

				double mO = OxidizerMax;
				double cO = OxidizerAmount;

				double mM = MonoPropellantMax;
				double cM = MonoPropellantAmount;

				screenLabels[70].Text = "         LF     LO     MP";
				screenLabels[71].Text = "STAGE:"
										+ Helper.prtlen(Math.Round(cF).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round(cO).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Helper.toFixed(cM, 2), 7, Helper.Align.RIGHT);
				screenLabels[72].Text = "    %:"
										+ Helper.prtlen(Math.Round((cF / mF) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cO / mO) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cM / mM) * 100).ToString(), 7, Helper.Align.RIGHT);

				mF = TotLiquidFuelMax;
				cF = TotLiquidFuelAmount;

				mO = TotOxidizerMax;
				cO = TotOxidizerAmount;

				mM = TotMonoPropellantMax;
				cM = TotMonoPropellantAmount;
				
				screenLabels[74].Text = "  TOT:"
										+ Helper.prtlen(Math.Round(cF).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round(cO).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Helper.toFixed(cM, 2), 7, Helper.Align.RIGHT);
				screenLabels[75].Text = "    %:"
										+ Helper.prtlen(Math.Round((cF / mF) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cO / mO) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cM / mM) * 100).ToString(), 7, Helper.Align.RIGHT);
				
				// Delta V
				// TOTO: Maybe buttonifize this, so the calculations are not done every refresh.
				updateDeltaVStats(currentStage, parts);
			}
		}

		public override void resize() { }

		private void updateDeltaVStats(int currentStage, Parts parts)
		{
			
			double[] stageWetMass = new double[currentStage + 1];
			double[] stageDryMass = new double[currentStage + 1];
			double[] stageISP = new double[currentStage + 1];
			double[] stageDV = new double[currentStage + 1];
			double[] stageThrust = new double[currentStage + 1];

			// Iterate all (remaining) stages
			if (parts != null)
			{
				for (int i = currentStage; i >= 0; i--)
				{
					IList<Part> stageParts = parts.InDecoupleStage(i);
					double wetMass = 0;
					double dryMass = 0;
					List<double> engineThrust = new List<double>();
					List<double> engineISP = new List<double>();

					// Iterate all parts
					foreach (Part part in stageParts)
					{
						wetMass += part.Mass;
						dryMass += part.DryMass;

						Engine engine = part.Engine;
						if (engine != null)
						{
							engineThrust.Add(engine.MaxThrust);
							engineISP.Add(engine.VacuumSpecificImpulse);
						}
					}

					stageWetMass[i] = wetMass;
					stageDryMass[i] = dryMass;

					// Sum the engines
					double totalThrust = 0;
					double totalDivisor = 0;

					for (int j = 0; j < engineThrust.Count; j++)
					{
						totalThrust += engineThrust[j];
						totalDivisor += engineThrust[j] / engineISP[j];
					}
					stageThrust[i] = totalThrust;
					stageISP[i] = totalThrust / totalDivisor;
				}
			}

			// Go through and calulate stage Delta-V
			String stageDeltas = "";
			bool curStage = false;
			for (int i = stageISP.Length - 1; i >= 0; i--)
			{
				double totalDryMass = stageDryMass[i];
				double totalWetMass = stageWetMass[i];
				
				for (int j = i - 1; j >= 0; j--)
				{
					totalDryMass += stageWetMass[j];
					totalWetMass += stageWetMass[j];
				}
				stageDV[i] = Math.Log(totalWetMass / totalDryMass) * stageISP[i] * g;

				if (curStage == false && !double.IsNaN(stageDV[i]))
				{
					stageDeltas += Helper.prtlen(Helper.toFixed(stageDV[i], 2), 9, Helper.Align.RIGHT);
					curStage = true;
				}
			}

			screenLabels[81].Text = "STG: " + stageDeltas + " m/s";

			// SUM TOTAL
			double totalDV = 0;
			foreach (double dv in stageDV)
			{
				if (!double.IsNaN(dv)) totalDV += dv; 
			}
			screenLabels[82].Text = "TOT: " + Helper.prtlen(Helper.toFixed(totalDV, 2), 9, Helper.Align.RIGHT) + " m/s";

			// CALCULATE BURN TIME
			double sDv = 0;
			double sTh = 0;
			for (int i = stageDV.Length - 1; i >= 0; i--)
			{
				if (!double.IsNaN(stageDV[i]))
				{
					sDv = stageDV[i];
					sTh = stageThrust[i];
					break;
				}
			}

			double deltaV;
			try
			{
				deltaV = double.Parse(screenInputs[1].Text);
			}
			catch (Exception)
			{
				deltaV = 0;
			}
			double burnSecs = burnTime(deltaV, sDv, sTh);
			screenLabels[84].Text = "BURN TIME: " + Helper.timeString(burnSecs, false);
		}

		private double burnTime(double dV, double ISP, double thrust)
		{
			double massInit = mass;
			double massFine = massInit * Math.Exp(-dV / (ISP * g));
			double massProp = massInit - massFine;
			double massDot = thrust / (ISP * g);
			return massProp / massDot;
		}

		private void getStageSupplies(bool force)
		{
			LiquidFuelMax = screenStreams.GetData(DataType.resource_stage_max_liquidFuel, force);
			LiquidFuelAmount = screenStreams.GetData(DataType.resource_stage_amount_liquidFuel, force);
			OxidizerMax = screenStreams.GetData(DataType.resource_stage_max_oxidizer, force);
			OxidizerAmount = screenStreams.GetData(DataType.resource_stage_amount_oxidizer, force);
			MonoPropellantMax = screenStreams.GetData(DataType.resource_stage_max_monoPropellant, force);
			MonoPropellantAmount = screenStreams.GetData(DataType.resource_stage_amount_monoPropellant, force);
		}
	}
}
