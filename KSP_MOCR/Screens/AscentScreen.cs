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

		// Stream Data //
		float inertRoll = 0;
		float inertPitch = 0;
		float inertYaw = 0;
		double MET = 0;
		double MeanAltitude = 0;
		double SurfaceAltitude = 0;
		double ApoapsisAltitude = 0;
		double PeriapsisAltitude = 0;
		double TimeToApoapsis = 0;
		double TimeToPeriapsis = 0;
		double Inclination = 0;
		double Eccentricity = 0;
		double OrbSpeed = 0;
		double Speed = 0;
		double EquatorialRadius = 0;
		double GravitationalParameter = 0;
		float Roll = 0;
		float Pitch = 0;
		float Yaw = 0;
		String BodyName = "";
		double lat = 0;
		double lon = 0;
		double AtmosphereDensity = 0;
		double StaticPressure = 0;
		double DynamicPressure = 0;
		float LiquidFuelMax = 0;
		float LiquidFuelAmount = 0;
		float OxidizerMax = 0;
		float OxidizerAmount = 0;
		float MonoPropellantMax = 0;
		float MonoPropellantAmount = 0;
		float ElectricChargeMax = 0;
		float ElectricChargeAmount = 0;
		bool SAS, RCS, Gear, Brakes, Lights, Abort;
		float GForce;
		Tuple<double, double, double> inertDirection;


		public AscentScreen(Screen form)
		{
			this.form = form;
			this.screenStreams = form.streamCollection;
			this.chartData = form.chartData;

			this.width = 120;
			this.height = 30;
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

			// Orbit Targets and Deltas
			screenLabels[60] = Helper.CreateLabel(24, 10, 8, 1, ""); // Target Eccentricity
			screenLabels[61] = Helper.CreateLabel(24, 12, 8, 1, ""); // Target Orbital Velocity

			screenLabels[65] = Helper.CreateLabel(34, 5, 8, 1, ""); // Delta Apoapsis
			screenLabels[66] = Helper.CreateLabel(34, 6, 8, 1, ""); // Delta Periapsis
			screenLabels[67] = Helper.CreateLabel(34, 10, 8, 1, ""); // Delta Eccentricity
			screenLabels[68] = Helper.CreateLabel(34, 12, 8, 1, ""); // Delta Orbital Velocty

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

			// Get target apo/peri from PySSSMQ if they exsists there. If they don't write them in.
			string tgtapo = form.dataStorage.getData("TGTAPO");
			string tgtperi = form.dataStorage.getData("TGTPERI");

			if(tgtapo == "")
			{
				tgtapo = "120000";
				form.dataStorage.setData("TGTAPO", tgtapo);
			}

			if (tgtperi == "")
			{
				tgtperi = "120000";
				form.dataStorage.setData("TGTPERI", tgtperi);
			}

			for (int i = 0; i < 5; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(24, 5, 8, 1, HorizontalAlignment.Right); // Target Apoapsis
			screenInputs[0].Text = tgtapo;
			screenInputs[0].TextChanged += (sender, e) => updateApo(sender, e, screenInputs[0].Text);
			screenInputs[1] = Helper.CreateInput(24, 6, 8, 1, HorizontalAlignment.Right); // Target Periapsis
			screenInputs[1].Text = tgtperi;
			screenInputs[1].TextChanged += (sender, e) => updatePeri(sender, e, screenInputs[1].Text);


			for (int i = 0; i < 2; i++) screenCharts.Add(null); // Initialize Charts

			// Altitude vs. Time Graph
			screenCharts[0] = Helper.CreatePlot(0, 15, 60, 15, -1, -1, 0, -1);
			screenCharts[0].fixedXwidth = 600;
			screenCharts[0].setSeriesColor(0, Color.FromArgb(100, 251, 251, 251));
			screenCharts[0].setSeriesColor(1, Color.FromArgb(100, 251, 251, 251));
			screenCharts[0].setSeriesColor(2, Color.FromArgb(200, 0, 169, 51));
			screenCharts[0].setSeriesColor(3, Color.FromArgb(200, 0, 51, 204));
			screenCharts[0].setSeriesColor(4, Color.FromArgb(200, 204, 51, 0));

			// Gee-Force vs. Time Graph
			screenCharts[1] = Helper.CreatePlot(60, 15, 60, 15, -1, -1);
			screenCharts[1].fixedXwidth = 600;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			// Re-usable data variable for graph data
			List<List<KeyValuePair<int, double?>>> data = new List<List<KeyValuePair<int, double?>>>();
			List<Plot.Type> types = new List<Plot.Type>();

		
			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				inertDirection = screenStreams.GetData(DataType.flight_inertial_direction);
				inertRoll = screenStreams.GetData(DataType.flight_inertial_roll);
				inertPitch = (float)Helper.rad2deg(Math.Asin(inertDirection.Item2));
				inertYaw = (float)Helper.rad2deg(Math.Atan(inertDirection.Item3 / inertDirection.Item1));
				MET = screenStreams.GetData(DataType.vessel_MET);
				MeanAltitude = screenStreams.GetData(DataType.flight_meanAltitude);
				SurfaceAltitude = screenStreams.GetData(DataType.flight_surfaceAltitude);
				ApoapsisAltitude = screenStreams.GetData(DataType.orbit_apoapsisAltitude);
				PeriapsisAltitude = screenStreams.GetData(DataType.orbit_periapsisAltitude);
				TimeToApoapsis = screenStreams.GetData(DataType.orbit_timeToApoapsis);
				TimeToPeriapsis = screenStreams.GetData(DataType.orbit_timeToPeriapsis);
				Inclination = screenStreams.GetData(DataType.orbit_inclination);
				Eccentricity = screenStreams.GetData(DataType.orbit_eccentricity);
				OrbSpeed = screenStreams.GetData(DataType.orbit_speed);
				Speed = screenStreams.GetData(DataType.flight_speed);
				EquatorialRadius = screenStreams.GetData(DataType.body_radius);
				GravitationalParameter = screenStreams.GetData(DataType.body_gravityParameter);
				Roll = screenStreams.GetData(DataType.flight_roll);
				Pitch = screenStreams.GetData(DataType.flight_pitch);
				Yaw = screenStreams.GetData(DataType.flight_heading);
				BodyName = screenStreams.GetData(DataType.body_name);
				lat = screenStreams.GetData(DataType.flight_latitude);
				lon = screenStreams.GetData(DataType.flight_longitude);
				AtmosphereDensity = screenStreams.GetData(DataType.flight_atmosphereDensity);
				StaticPressure = screenStreams.GetData(DataType.flight_staticPressure);
				DynamicPressure = screenStreams.GetData(DataType.flight_dynamicPressure);
				LiquidFuelMax = screenStreams.GetData(DataType.resource_stage_max_liquidFuel);
				LiquidFuelAmount = screenStreams.GetData(DataType.resource_stage_amount_liquidFuel);
				OxidizerMax = screenStreams.GetData(DataType.resource_stage_max_oxidizer);
				OxidizerAmount = screenStreams.GetData(DataType.resource_stage_amount_oxidizer);
				MonoPropellantMax = screenStreams.GetData(DataType.resource_stage_max_monoPropellant);
				MonoPropellantAmount = screenStreams.GetData(DataType.resource_stage_amount_monoPropellant);
				ElectricChargeMax = screenStreams.GetData(DataType.resource_stage_max_electricCharge);
				ElectricChargeAmount = screenStreams.GetData(DataType.resource_stage_amount_electricCharge);
				SAS = screenStreams.GetData(DataType.control_SAS);
				RCS = screenStreams.GetData(DataType.control_RCS);
				Gear = screenStreams.GetData(DataType.control_gear);
				Brakes = screenStreams.GetData(DataType.control_brakes);
				Lights = screenStreams.GetData(DataType.control_lights);
				Abort = screenStreams.GetData(DataType.control_abort);
				GForce = screenStreams.GetData(DataType.flight_gForce);

				screenLabels[0].Text = " LT: " + Helper.timeString(DateTime.Now.TimeOfDay.TotalSeconds);
				screenLabels[1].Text = "MET: " + Helper.timeString(MET, 3);

				// Orbit info
				screenLabels[30].Text = "      Alt: " + Helper.prtlen(Math.Round(MeanAltitude).ToString(), 9, Helper.Align.RIGHT); // Altitude
				screenLabels[31].Text = " Apoapsis: " + Helper.prtlen(Math.Round(ApoapsisAltitude).ToString(), 9, Helper.Align.RIGHT); // Apoapsis
				screenLabels[32].Text = "Periapsis: " + Helper.prtlen(Math.Round(PeriapsisAltitude).ToString(), 9, Helper.Align.RIGHT); // Periapasis
				screenLabels[33].Text = "      TtA: " + Helper.prtlen(Helper.timeString(TimeToApoapsis), 9, Helper.Align.RIGHT); // Time to Apoapsis
				screenLabels[34].Text = "      TtP: " + Helper.prtlen(Helper.timeString(TimeToPeriapsis), 9, Helper.Align.RIGHT); // Time to Periapsis
				screenLabels[35].Text = "      Inc: " + Helper.prtlen(Helper.toFixed(Helper.rad2deg(Inclination), 3), 9, Helper.Align.RIGHT); // Inclination
				screenLabels[36].Text = "      Ecc: " + Helper.prtlen(Helper.toFixed(Eccentricity, 3), 9, Helper.Align.RIGHT); // Eccentricity
				screenLabels[37].Text = " Orb. Vel: " + Helper.prtlen(Helper.toFixed(OrbSpeed, 1), 9, Helper.Align.RIGHT); // Orbit Velocity
				screenLabels[38].Text = " Sur. Vel: " + Helper.prtlen(Helper.toFixed(Speed, 1), 9, Helper.Align.RIGHT); // Surface Velocity

				// Orbit Targets and Deltas
				double tgtA = 0, tgtP = 0;
				if (double.TryParse(screenInputs[0].Text, out tgtA) && double.TryParse(screenInputs[1].Text, out tgtP))
				{
					double sMa = (tgtA + tgtP + (EquatorialRadius * 2)) / 2;
					double tgtEcc = (sMa - (tgtP + EquatorialRadius)) / sMa;
					screenLabels[60].Text = Helper.prtlen(Helper.toFixed(tgtEcc, 3), 8, Helper.Align.RIGHT); // Target Eccentricity
					screenLabels[67].Text = Helper.prtlen(Helper.toFixed(Eccentricity - tgtEcc, 3), 8, Helper.Align.RIGHT); // Delta Eccentricity
				}

				if (double.TryParse(screenInputs[0].Text, out tgtA))
				{ screenLabels[65].Text = Helper.prtlen(Math.Round(tgtA - ApoapsisAltitude).ToString(), 8, Helper.Align.RIGHT); } // Delta Apoapsis
				if (double.TryParse(screenInputs[1].Text, out tgtP))
				{ screenLabels[66].Text = Helper.prtlen(Math.Round(tgtP - PeriapsisAltitude).ToString(), 8, Helper.Align.RIGHT); } // Delta Periapsis

				double u = GravitationalParameter;
				double a = EquatorialRadius + tgtA;
				double r = EquatorialRadius + tgtA;
				double v = Math.Sqrt(u * ((2 / r) - (1 / a)));
				screenLabels[61].Text = Helper.prtlen(Helper.toFixed(v, 1), 8, Helper.Align.RIGHT); // Target Obital Velocity at Apoapsis


				// POSITION INFO
				screenLabels[46].Text = "R: " + Helper.prtlen(Helper.toFixed(Roll, 2), 7, Helper.Align.RIGHT) + "  " + Helper.prtlen(Helper.toFixed(inertRoll, 2), 7, Helper.Align.RIGHT);
				screenLabels[47].Text = "P: " + Helper.prtlen(Helper.toFixed(Pitch, 2), 7, Helper.Align.RIGHT) + "  " + Helper.prtlen(Helper.toFixed(inertPitch, 2), 7, Helper.Align.RIGHT);
				screenLabels[48].Text = "Y: " + Helper.prtlen(Helper.toFixed(Yaw, 2), 7, Helper.Align.RIGHT) + "  " + Helper.prtlen(Helper.toFixed(inertYaw, 2), 7, Helper.Align.RIGHT);

				screenLabels[40].Text = "  Body: " + Helper.prtlen(BodyName, 9, Helper.Align.RIGHT);
				screenLabels[41].Text = "   Lat: " + Helper.prtlen(Helper.toFixed(lat, 5), 9, Helper.Align.RIGHT);
				screenLabels[42].Text = "   Lon: " + Helper.prtlen(Helper.toFixed(lon, 5), 9, Helper.Align.RIGHT);

				screenLabels[43].Text = " Atm.Den: " + Helper.prtlen(Helper.toFixed(AtmosphereDensity, 1), 9, Helper.Align.RIGHT) + "  Radar Alt: " + Helper.prtlen(Math.Round(SurfaceAltitude).ToString(), 7, Helper.Align.RIGHT);
				screenLabels[44].Text = " Atm.Pre: " + Helper.prtlen(Math.Round(StaticPressure).ToString(), 9, Helper.Align.RIGHT);
				screenLabels[45].Text = " Dyn.Pre: " + Helper.prtlen(Math.Round(DynamicPressure).ToString(), 9, Helper.Align.RIGHT) + "    G-Force: " + Helper.prtlen(Helper.toFixed(GForce, 2), 7, Helper.Align.RIGHT);

				// Supplies
				double mF = LiquidFuelMax;
				double cF = LiquidFuelAmount;

				double mO = OxidizerMax;
				double cO = OxidizerAmount;

				double mM = MonoPropellantMax;
				double cM = MonoPropellantAmount;

				double mE = ElectricChargeMax;
				double cE = ElectricChargeAmount;

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

				mF = screenStreams.GetData(DataType.resource_total_max_liquidFuel);
				cF = screenStreams.GetData(DataType.resource_total_amount_liquidFuel);

				mO = screenStreams.GetData(DataType.resource_total_max_oxidizer);
				cO = screenStreams.GetData(DataType.resource_total_amount_oxidizer);

				mM = screenStreams.GetData(DataType.resource_total_max_monoPropellant);
				cM = screenStreams.GetData(DataType.resource_total_amount_monoPropellant);

				mE = screenStreams.GetData(DataType.resource_total_max_electricCharge);
				cE = screenStreams.GetData(DataType.resource_total_amount_electricCharge);

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
				if (SAS) { screenIndicators[0].setStatus(Indicator.status.GREEN); } else { screenIndicators[0].setStatus(Indicator.status.OFF); } // SAS
				if (RCS) { screenIndicators[1].setStatus(Indicator.status.GREEN); } else { screenIndicators[1].setStatus(Indicator.status.OFF); } // RCS
				if (Gear) { screenIndicators[2].setStatus(Indicator.status.GREEN); } else { screenIndicators[2].setStatus(Indicator.status.OFF); } // GEAR
				if (Brakes) { screenIndicators[3].setStatus(Indicator.status.RED); } else { screenIndicators[3].setStatus(Indicator.status.OFF); } // Break
				if (Lights) { screenIndicators[4].setStatus(Indicator.status.AMBER); } else { screenIndicators[4].setStatus(Indicator.status.OFF); } // Lights
				if (Abort) { screenIndicators[5].setStatus(Indicator.status.RED); } else { screenIndicators[5].setStatus(Indicator.status.OFF); } // Abort

				if (GForce > 4) { screenIndicators[7].setStatus(Indicator.status.AMBER); } else { screenIndicators[7].setStatus(Indicator.status.OFF); } // G High

				double maxR = mE;
				double curR = cE;
				if (curR / maxR > 0.95) { screenIndicators[6].setStatus(Indicator.status.GREEN); } else { screenIndicators[6].setStatus(Indicator.status.OFF); } // Power High
				if (curR / maxR < 0.1) { screenIndicators[9].setStatus(Indicator.status.RED); } else { screenIndicators[9].setStatus(Indicator.status.OFF); } // Power Low

				maxR = mM;
				curR = cM;
				if (curR / maxR < 0.1) { screenIndicators[10].setStatus(Indicator.status.RED); } else { screenIndicators[10].setStatus(Indicator.status.OFF); } // Monopropellant Low

				maxR = mF;
				curR = cF;
				if (curR / maxR < 0.1) { screenIndicators[11].setStatus(Indicator.status.RED); } else { screenIndicators[11].setStatus(Indicator.status.OFF); } // Fuel Low

				maxR = mO;
				curR = cO;
				if (curR / maxR < 0.1) { screenIndicators[8].setStatus(Indicator.status.RED); } else { screenIndicators[8].setStatus(Indicator.status.OFF); } // LOW Low


				// Graphs
				int xMin = screenCharts[0].findMinX(chartData["altitudeTime"]);
				int xMax = screenCharts[0].findMaxX(chartData["altitudeTime"]);

				List<KeyValuePair<int, double?>> targetA = new List<KeyValuePair<int, double?>>();
				targetA.Add(new KeyValuePair<int, double?>(xMin, tgtA));
				targetA.Add(new KeyValuePair<int, double?>(xMax, tgtA));

				List<KeyValuePair<int, double?>> targetP = new List<KeyValuePair<int, double?>>();
				targetP.Add(new KeyValuePair<int, double?>(xMin, tgtP));
				targetP.Add(new KeyValuePair<int, double?>(xMax, tgtP));

				data = new List<List<KeyValuePair<int, double?>>>();
				types = new List<Plot.Type>();
				data.Add(targetA);
				types.Add(Plot.Type.LINE);
				data.Add(targetP);
				types.Add(Plot.Type.LINE);
				data.Add(chartData["apoapsisTime"]);
				types.Add(Plot.Type.LINE);
				data.Add(chartData["periapsisTime"]);
				types.Add(Plot.Type.LINE);
				data.Add(chartData["altitudeTime"]);
				types.Add(Plot.Type.LINE);
				screenCharts[0].setData(data, types, false);

				data = new List<List<KeyValuePair<int, double?>>>();
				types = new List<Plot.Type>();
				data.Add(chartData["geeTime"]);
				types.Add(Plot.Type.LINE);
				data.Add(chartData["dynPresTime"]);
				types.Add(Plot.Type.LINE);
				screenCharts[1].setData(data, types, true);
			}
		}

		public void updateApo(object sender, EventArgs e, string data)
		{
			form.dataStorage.setData("TGTAPO", data);
		}

		public void updatePeri(object sender, EventArgs e, string data)
		{
			form.dataStorage.setData("TGTPERI", data);
		}
	}
}
