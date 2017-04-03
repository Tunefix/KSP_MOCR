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
	class AscentScreen : MocrScreen
	{
		KRPC.Client.Services.SpaceCenter.Vessel vessel;
		KRPC.Client.Services.SpaceCenter.Flight flight;
		KRPC.Client.Services.SpaceCenter.Orbit orbit;


		public AscentScreen(Form1 form)
		{
			this.connection = form.connection;
			this.krpc = this.connection.KRPC();
			this.spaceCenter = this.connection.SpaceCenter();
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

			screenLabels[0] = Helper.CreateLabel(16, 1, 13); // Local Time
			screenLabels[1] = Helper.CreateLabel(0, 1, 14); // MET Time
			screenLabels[2] = Helper.CreateLabel(39, 0, 42, 1, "============ ASCENSION MODULE ============"); // Screen Title
			screenLabels[3] = Helper.CreateLabel(84, 0, 39, 1, "├───────────── STATUS ─────────────┤"); // Status Headline
			screenLabels[4] = Helper.CreateLabel(84, 1, 1, 1, "│");
			screenLabels[5] = Helper.CreateLabel(0, 2, 85, 1, "────────────────── ORBIT ──────────────────┬─────────────── POSITION ───────────────┤"); // Obrit/Position headline
			screenLabels[6] = Helper.CreateLabel(0, 3, 44, 1, "                 CUR       TGT       DTA   │"); // Orbit subheadlines
			screenLabels[7] = Helper.CreateLabel(43, 4, 1, 1, "│");
			screenLabels[8] = Helper.CreateLabel(43, 5, 1, 1, "│");
			screenLabels[9] = Helper.CreateLabel(43, 6, 1, 1, "│");
			screenLabels[10] = Helper.CreateLabel(43, 7, 1, 1, "│");
			screenLabels[11] = Helper.CreateLabel(43, 8, 1, 1, "│");
			screenLabels[12] = Helper.CreateLabel(43, 9, 1, 1, "│");
			screenLabels[13] = Helper.CreateLabel(43, 10, 1, 1, "│");
			screenLabels[14] = Helper.CreateLabel(43, 11, 1, 1, "│");
			screenLabels[15] = Helper.CreateLabel(43, 12, 1, 1, "│");
			screenLabels[16] = Helper.CreateLabel(84, 3, 1, 1, "│");
			screenLabels[17] = Helper.CreateLabel(84, 4, 1, 1, "│");
			screenLabels[18] = Helper.CreateLabel(84, 5, 1, 1, "│");
			screenLabels[19] = Helper.CreateLabel(84, 6, 1, 1, "│");
			screenLabels[20] = Helper.CreateLabel(84, 7, 36, 1, "│┌─────────── SUPPLIES ────────────┐");
			screenLabels[21] = Helper.CreateLabel(84, 8, 1, 1, "│");
			screenLabels[22] = Helper.CreateLabel(84, 9, 1, 1, "│");
			screenLabels[23] = Helper.CreateLabel(84, 10, 1, 1, "│");
			screenLabels[24] = Helper.CreateLabel(84, 11, 1, 1, "│");
			screenLabels[25] = Helper.CreateLabel(84, 12, 1, 1, "│");
			screenLabels[26] = Helper.CreateLabel(43, 13, 1, 1, "│");
			screenLabels[27] = Helper.CreateLabel(43, 14, 1, 1, "│");
			screenLabels[28] = Helper.CreateLabel(84, 13, 1, 1, "│");
			screenLabels[29] = Helper.CreateLabel(84, 14, 1, 1, "│");

			// Orbit data
			screenLabels[30] = Helper.CreateLabel(2, 4, 20, 1, "      Alt: "); // Altitude
			screenLabels[31] = Helper.CreateLabel(2, 5, 20, 1, " Apoapsis: "); // Apoapsis
			screenLabels[32] = Helper.CreateLabel(2, 6, 20, 1, "Periapsis: "); // Periapasis
			screenLabels[33] = Helper.CreateLabel(2, 7, 20, 1, "      TtA: "); // Time to Apoapsis
			screenLabels[34] = Helper.CreateLabel(2, 8, 20, 1, "      TtP: "); // Time to Periapsis
			screenLabels[35] = Helper.CreateLabel(2, 9, 20, 1, "      Inc: "); // Inclination
			screenLabels[36] = Helper.CreateLabel(2, 10, 20, 1, "      Ecc: "); // Eccentricity
			screenLabels[37] = Helper.CreateLabel(2, 12, 20, 1, " Orb. Vel: "); // Orbit Velocity
			screenLabels[38] = Helper.CreateLabel(2, 13, 20, 1, " Sur. Vel: "); // Surface Velocity

			// Orbit Targets and Deltas
			screenLabels[60] = Helper.CreateLabel(24, 10, 8, 1, ""); // Target Eccentricity
			screenLabels[61] = Helper.CreateLabel(24, 12, 8, 1, ""); // Target Orbital Velocity

			screenLabels[65] = Helper.CreateLabel(34, 5, 8, 1, ""); // Delta Apoapsis
			screenLabels[66] = Helper.CreateLabel(34, 6, 8, 1, ""); // Delta Periapsis
			screenLabels[67] = Helper.CreateLabel(34, 10, 8, 1, ""); // Delta Eccentricity
			screenLabels[68] = Helper.CreateLabel(34, 12, 8, 1, ""); // Delta Orbital Velocty

			// Position data
			screenLabels[40] = Helper.CreateLabel(44, 4, 20, 1, "  Body: "); // Sphere of Influence
			screenLabels[41] = Helper.CreateLabel(44, 5, 20, 1, "   Lat: "); // Latitude
			screenLabels[42] = Helper.CreateLabel(44, 6, 20, 1, "   Lon: "); // Longitude
			screenLabels[43] = Helper.CreateLabel(44, 9, 39, 1, " Atm.Den: "); // Atmosphere Density AND Radar Alt
			screenLabels[44] = Helper.CreateLabel(44, 10, 20, 1, " Atm.Pre: "); // Atmosphere Pressure
			screenLabels[45] = Helper.CreateLabel(44, 11, 20, 1, " Dyn.Pre: "); // Dynamic Pressure
			screenLabels[46] = Helper.CreateLabel(64, 5, 19, 1, "R: "); // Roll
			screenLabels[47] = Helper.CreateLabel(64, 6, 19, 1, "P: "); // Pitch
			screenLabels[48] = Helper.CreateLabel(64, 7, 19, 1, "Y: "); // Yaw
			screenLabels[49] = Helper.CreateLabel(64, 4, 19, 1, "     SURF     ORBT"); // RPY Headlines

			// Supplies
			screenLabels[50] = Helper.CreateLabel(85, 8, 35, 1, "                     "); // Supply line 1
			screenLabels[51] = Helper.CreateLabel(85, 9, 35, 1, "                     "); // Supply line 2
			screenLabels[52] = Helper.CreateLabel(85, 10, 35, 1, "                     "); // Supply line 3
			screenLabels[53] = Helper.CreateLabel(85, 11, 35, 1, "                     "); // Supply line 4
			screenLabels[54] = Helper.CreateLabel(85, 12, 35, 1, "                     "); // Supply line 5
			screenLabels[55] = Helper.CreateLabel(85, 13, 35, 1, "                     "); // Supply line 5
			screenLabels[58] = Helper.CreateLabel(85, 14, 35, 1, "└─────────────────────────────────┘"); // Supply line 6

			// Status
			screenIndicators[0] = Helper.CreateIndicator(86, 1, 10, 1, "SAS");
			screenIndicators[1] = Helper.CreateIndicator(97, 1, 10, 1, "RCS");
			screenIndicators[2] = Helper.CreateIndicator(108, 1, 10, 1, "GEAR");
			screenIndicators[3] = Helper.CreateIndicator(86, 2, 10, 1, "BRAKES");
			screenIndicators[4] = Helper.CreateIndicator(97, 2, 10, 1, "LIGHTS");
			screenIndicators[5] = Helper.CreateIndicator(108, 2, 10, 1, "ABORT");
			screenIndicators[6] = Helper.CreateIndicator(86, 4, 10, 1, "POWER HI");
			screenIndicators[7] = Helper.CreateIndicator(97, 4, 10, 1, "G HIGH");
			screenIndicators[8] = Helper.CreateIndicator(108, 4, 10, 1, "LOX LOW");
			screenIndicators[9] = Helper.CreateIndicator(86, 5, 10, 1, "POWER LOW");
			screenIndicators[10] = Helper.CreateIndicator(97, 5, 10, 1, "MONO LOW");
			screenIndicators[11] = Helper.CreateIndicator(108, 5, 10, 1, "FUEL LOW");


			for (int i = 0; i < 5; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(24, 5, 8, 1, HorizontalAlignment.Right); // Target Apoapsis
			screenInputs[0].Text = "120000";
			screenInputs[1] = Helper.CreateInput(24, 6, 8, 1, HorizontalAlignment.Right); // Target Periapsis
			screenInputs[1].Text = "120000";



			for (int i = 0; i < 2; i++) form.screenCharts.Add(null); // Initialize Charts
																		// Altitude vs. Time Graph
			form.screenCharts[0] = Helper.CreateChart(0, 15, 60, 15, 0, 600);

			// Gee-Force vs. Time Graph
			form.screenCharts[1] = Helper.CreateChart(60, 15, 60, 15, 0, 600);
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			// Re-usable data variable for graph data
			List<Dictionary<int, double?>> data = new List<Dictionary<int, double?>>();


			if (form.connected && krpc.CurrentGameScene == GameScene.Flight)
			{
				vessel = spaceCenter.ActiveVessel;
				flight = vessel.Flight();
				orbit = vessel.Orbit;


				screenLabels[0].Text = " LT: " + Helper.timeString(DateTime.Now.TimeOfDay.TotalSeconds);
				screenLabels[1].Text = "MET: " + Helper.timeString(vessel.MET, 3);

				// Orbit info
				screenLabels[30].Text = "      Alt: " + Helper.prtlen(Math.Round(flight.MeanAltitude).ToString(), 9, Helper.Align.RIGHT); // Altitude
				screenLabels[31].Text = " Apoapsis: " + Helper.prtlen(Math.Round(orbit.ApoapsisAltitude).ToString(), 9, Helper.Align.RIGHT); // Apoapsis
				screenLabels[32].Text = "Periapsis: " + Helper.prtlen(Math.Round(orbit.PeriapsisAltitude).ToString(), 9, Helper.Align.RIGHT); // Periapasis
				screenLabels[33].Text = "      TtA: " + Helper.prtlen(Helper.timeString(orbit.TimeToApoapsis), 9, Helper.Align.RIGHT); // Time to Apoapsis
				screenLabels[34].Text = "      TtP: " + Helper.prtlen(Helper.timeString(orbit.TimeToPeriapsis), 9, Helper.Align.RIGHT); // Time to Periapsis
				screenLabels[35].Text = "      Inc: " + Helper.prtlen(Helper.toFixed(orbit.Inclination, 3), 9, Helper.Align.RIGHT); // Inclination
				screenLabels[36].Text = "      Ecc: " + Helper.prtlen(Helper.toFixed(orbit.Eccentricity, 3), 9, Helper.Align.RIGHT); // Eccentricity
				screenLabels[37].Text = " Orb. Vel: " + Helper.prtlen(Helper.toFixed(orbit.Speed, 1), 9, Helper.Align.RIGHT); // Orbit Velocity
				screenLabels[38].Text = " Sur. Vel: " + Helper.prtlen(Helper.toFixed(vessel.Flight(vessel.Orbit.Body.ReferenceFrame).Speed, 1), 9, Helper.Align.RIGHT); // Surface Velocity

				// Orbit Targets and Deltas
				double tgtA = 0, tgtP = 0;
				if (double.TryParse(screenInputs[0].Text, out tgtA) && double.TryParse(screenInputs[1].Text, out tgtP))
				{
					double sMa = (tgtA + tgtP + (orbit.Body.EquatorialRadius * 2)) / 2;
					double tgtEcc = (sMa - (tgtP + orbit.Body.EquatorialRadius)) / sMa;
					screenLabels[60].Text = Helper.prtlen(Helper.toFixed(tgtEcc, 3), 8, Helper.Align.RIGHT); // Target Eccentricity
					screenLabels[67].Text = Helper.prtlen(Helper.toFixed(orbit.Eccentricity - tgtEcc, 3), 8, Helper.Align.RIGHT); // Delta Eccentricity
				}

				if (double.TryParse(screenInputs[0].Text, out tgtA))
				{ screenLabels[65].Text = Helper.prtlen(Math.Round(tgtA - orbit.ApoapsisAltitude).ToString(), 8, Helper.Align.RIGHT); } // Delta Apoapsis
				if (double.TryParse(screenInputs[1].Text, out tgtP))
				{ screenLabels[66].Text = Helper.prtlen(Math.Round(tgtP - orbit.PeriapsisAltitude).ToString(), 8, Helper.Align.RIGHT); } // Delta Periapsis

				double u = orbit.Body.GravitationalParameter;
				double a = orbit.Body.EquatorialRadius + tgtA;
				double r = orbit.Body.EquatorialRadius + tgtA;
				double v = Math.Sqrt(u * ((2 / r) - (1 / a)));
				screenLabels[61].Text = Helper.prtlen(Helper.toFixed(v, 1), 8, Helper.Align.RIGHT); // Target Obital Velocity at Apoapsis


				// POSITION INFO
				screenLabels[46].Text = "R: " + Helper.prtlen(Helper.toFixed(flight.Roll, 2), 7, Helper.Align.RIGHT) + "  " + Helper.prtlen(Helper.toFixed(flight.Roll, 2), 7, Helper.Align.RIGHT);
				screenLabels[47].Text = "P: " + Helper.prtlen(Helper.toFixed(flight.Pitch, 2), 7, Helper.Align.RIGHT);
				screenLabels[48].Text = "Y: " + Helper.prtlen(Helper.toFixed(flight.Heading, 2), 7, Helper.Align.RIGHT);

				screenLabels[40].Text = "  Body: " + Helper.prtlen(orbit.Body.Name, 9, Helper.Align.RIGHT);
				screenLabels[41].Text = "   Lat: " + Helper.prtlen(Helper.toFixed(flight.Latitude, 5), 9, Helper.Align.RIGHT);
				screenLabels[42].Text = "   Lon: " + Helper.prtlen(Helper.toFixed(flight.Longitude, 5), 9, Helper.Align.RIGHT);

				screenLabels[43].Text = " Atm.Den: " + Helper.prtlen(Helper.toFixed(flight.AtmosphereDensity, 1), 9, Helper.Align.RIGHT) + "  Radar Alt: " + Helper.prtlen(Math.Round(flight.SurfaceAltitude).ToString(), 7, Helper.Align.RIGHT);
				screenLabels[44].Text = " Atm.Pre: " + Helper.prtlen(Math.Round(flight.StaticPressure).ToString(), 9, Helper.Align.RIGHT);
				screenLabels[45].Text = " Dyn.Pre: " + Helper.prtlen(Math.Round(flight.DynamicPressure).ToString(), 9, Helper.Align.RIGHT);

				// Supplies
				double mF = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("LiquidFuel");
				double cF = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("LiquidFuel");

				double mO = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("Oxidizer");
				double cO = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("Oxidizer");

				double mM = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("MonoPropellant");
				double cM = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("MonoPropellant");

				double mE = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Max("ElectricCharge");
				double cE = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage, false).Amount("ElectricCharge");

				screenLabels[50].Text = "         LF     LO     MP     EC";
				screenLabels[51].Text = "STAGE:"
										+ Helper.prtlen(Math.Round(cF).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round(cO).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Helper.toFixed(cM, 2), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round(cE).ToString(), 7, Helper.Align.RIGHT);
				screenLabels[52].Text = "    %:"
										+ Helper.prtlen(Math.Round((cF / mF) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cO / mO) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cM / mM) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cE / mE) * 100).ToString(), 7, Helper.Align.RIGHT);

				mF = vessel.Resources.Max("LiquidFuel");
				cF = vessel.Resources.Amount("LiquidFuel");

				mO = vessel.Resources.Max("Oxidizer");
				cO = vessel.Resources.Amount("Oxidizer");

				mM = vessel.Resources.Max("MonoPropellant");
				cM = vessel.Resources.Amount("MonoPropellant");

				mE = vessel.Resources.Max("ElectricCharge");
				cE = vessel.Resources.Amount("ElectricCharge");

				screenLabels[54].Text = "  TOT:"
										+ Helper.prtlen(Math.Round(cF).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round(cO).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Helper.toFixed(cM, 2), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round(cE).ToString(), 7, Helper.Align.RIGHT);
				screenLabels[55].Text = "    %:"
										+ Helper.prtlen(Math.Round((cF / mF) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cO / mO) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cM / mM) * 100).ToString(), 7, Helper.Align.RIGHT)
										+ Helper.prtlen(Math.Round((cE / mE) * 100).ToString(), 7, Helper.Align.RIGHT);

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
				Dictionary<int, Nullable<double>> targetA = new Dictionary<int, Nullable<double>>();
				targetA[0] = tgtA;
				targetA[600] = tgtA;

				Dictionary<int, Nullable<double>> targetP = new Dictionary<int, Nullable<double>>();
				targetP[0] = tgtP;
				targetP[600] = tgtP;

				data = new List<Dictionary<int, Nullable<double>>>();
				data.Add(chartData["altitudeTime"]);
				data.Add(chartData["apoapsisTime"]);
				data.Add(chartData["periapsisTime"]);
				data.Add(targetA);
				data.Add(targetP);
				form.showData(0, data, false);

				data = new List<Dictionary<int, Nullable<double>>>();
				data.Add(chartData["geeTime"]); 
				data.Add(chartData["dynPresTime"]);
				form.showData(1, data, true);
			}
		}
	}
}
