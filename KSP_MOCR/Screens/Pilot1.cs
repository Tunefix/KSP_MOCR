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
using System.Threading;
using System.Diagnostics;

namespace KSP_MOCR
{
	class Pilot1 : MocrScreen
	{
		private KRPC.Client.Services.SpaceCenter.Vessel vessel;
		private KRPC.Client.Services.SpaceCenter.Flight flight;
		private KRPC.Client.Services.SpaceCenter.Resources vessel_resources;
		private KRPC.Client.Services.SpaceCenter.Control vessel_control;
		private KRPC.Client.Services.SpaceCenter.Orbit orbit;
		private KRPC.Client.Services.SpaceCenter.Resources vessel_resources_stage;


		private KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Vessel> vessel_stream;
		private KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Flight> flight_stream;
		private KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Resources> resources_stream;
		private KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Control> control_stream;
		private KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Orbit> orbit_stream;
		private KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Resources> vessel_resources_stage_stream;
		
		private int setRotR = 0;
		private int setRotP = 90;
		private int setRotY = 90;
		private int rotStep = 5;

		private int lockRotR;
		private int lockRotP;
		private int lockRotY;

		private double rollRate = 1;
		private double pitchRate = 1;

		private int controlMode = 0; // 0: Free, 1: Autopilot, 2: Lock, 3: Roll program, 4: pitch program

		public Pilot1(Form1 form)
		{
			this.connection = form.connection;
			this.krpc = this.connection.KRPC();
			this.spaceCenter = this.connection.SpaceCenter();
			this.help = new KSP_MOCR.helper(form);
			this.form = form;
			this.chartData = form.chartData;

			this.updateRate = 250;

			this.width = 120;
			this.height = 30;
		}

		public override void destroyStreams()
		{
			if (this.vessel_stream != null) this.vessel_stream.Remove();
			if (this.flight_stream != null) this.flight_stream.Remove();
			if (this.resources_stream != null) this.resources_stream.Remove();
			if (this.control_stream != null) this.control_stream.Remove();
			if (this.orbit_stream != null) this.orbit_stream.Remove();
			if (this.vessel_resources_stage_stream != null) this.vessel_resources_stage_stream.Remove();
		}

		public override void makeElements()
		{
			for (int i = 0; i < 100; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 50; i++) screenIndicators.Add(null); // Initialize Indicators
			for (int i = 0; i < 50; i++) screenButtons.Add(null); // Initialize Buttons


			// LABELS
			screenLabels[0] = help.CreateLabel(16, 1, 13); // Local Time
			screenLabels[1] = help.CreateLabel(0, 1, 14); // MET Time
			screenLabels[2] = help.CreateLabel(39, 0, 42, 1, "============= PILOT MODULE #1 ============"); // Screen Title
			screenLabels[3] = help.CreateLabel(84, 0, 39, 1, "├───────────── STATUS ─────────────┤"); // Status Headline
			screenLabels[4] = help.CreateLabel(84, 1, 1, 1, "│");
			screenLabels[5] = help.CreateLabel(0, 2, 85, 1, "────────────────────────────────────────────────────────────────────────────────────┤"); // Obrit/Position headline
			screenLabels[6] = help.CreateLabel(0, 3, 38, 1, "┌───── TRANS ─────┐┌────── ROT ──────┐");
			screenLabels[7] = help.CreateLabel(0, 10, 19, 1, "┌──── THROTTLE ───┐");
			screenLabels[8] = help.CreateLabel(0, 14, 38, 1, "┌───────── READOUT ─────────┐┌ STAGE ┐");
			screenLabels[9] = help.CreateLabel(0, 15, 25, 1, "      SET     CUR    LOCK");
			screenLabels[10] = help.CreateLabel(0, 20, 80, 1, "───────── PROGRAMS ─────┬─── ACTION GROUPS ───┬─────── SAS SETTING ───────┐");
			screenLabels[11] = help.CreateLabel(0, 21, 24, 1, "┌── ROLL ──┐┌── PITCH ─┐");
			screenLabels[12] = help.CreateLabel(0, 29, 70, 1, "────────────────────────┴────────────┴────────────────────────────────────────┘");

			// THROTTLE CONTROLS
			screenButtons[0] = help.CreateButton(1, 12, 5, 1, "-10");
			screenButtons[0].Font = form.buttonFont;
			screenButtons[0].Click += (sender, e) => this.changeThrottle(sender, e, -0.1);
			screenButtons[1] = help.CreateButton(1, 11, 5, 1, "-1");
			screenButtons[1].Font = form.buttonFont;
			screenButtons[1].Click += (sender, e) => this.changeThrottle(sender, e, -0.01);
			screenButtons[2] = help.CreateButton(7, 11, 5, 1, "+1");
			screenButtons[2].Font = form.buttonFont;
			screenButtons[2].Click += (sender, e) => this.changeThrottle(sender, e, 0.01);
			screenButtons[3] = help.CreateButton(7, 12, 5, 1, "+10");
			screenButtons[3].Font = form.buttonFont;
			screenButtons[3].Click += (sender, e) => this.changeThrottle(sender, e, 0.1);
			screenButtons[16] = help.CreateButton(1, 13, 5, 1, "-100");
			screenButtons[16].Font = form.buttonFont;
			screenButtons[16].Click += (sender, e) => this.changeThrottle(sender, e, -1);
			screenButtons[17] = help.CreateButton(7, 13, 5, 1, "+100");
			screenButtons[17].Font = form.buttonFont;
			screenButtons[17].Click += (sender, e) => this.changeThrottle(sender, e, 1);

			// THROTTLE LABLES
			screenLabels[15] = help.CreateLabel(13, 11, 4, 1, "CUR:");
			screenLabels[16] = help.CreateLabel(13, 12, 4, 1, "100%");

			// ROTATIONAL LABELS
			screenLabels[17] = help.CreateLabel(33, 4, 4, 1, "RATE");

			// ROTATIONAL INDICATORS
			screenIndicators[22] = help.CreateIndicator(33, 5, 2, 2, "");
			screenIndicators[23] = help.CreateIndicator(33, 7, 2, 2, "");

			// ROTATIONAL CONTROLS
			screenButtons[4] = help.CreateButton(25, 4, 3, 2, "⭡");
			screenButtons[4].Click += this.pitchUp;
			screenButtons[5] = help.CreateButton(25, 7, 3, 2, "⭣");
			screenButtons[5].Click += this.pitchDown;
			screenButtons[6] = help.CreateButton(21, 7, 3, 2, "⭠");
			screenButtons[6].Click += this.yawLeft;
			screenButtons[7] = help.CreateButton(29, 7, 3, 2, "⭢");
			screenButtons[7].Click += this.yawRight;
			screenButtons[8] = help.CreateButton(21, 4, 3, 2, "⟲");
			screenButtons[8].Click += this.rollLeft;
			screenButtons[9] = help.CreateButton(29, 4, 3, 2, "⟳");
			screenButtons[9].Click += this.rollRight;

			screenButtons[10] = help.CreateButton(31, 10, 6, 1, "AUTO");
			screenButtons[10].Font = form.buttonFont;
			screenButtons[10].Click += (sender, e) => this.setMode(sender, e, 1);
			screenButtons[11] = help.CreateButton(31, 11, 6, 1, "FREE");
			screenButtons[11].Font = form.buttonFont;
			screenButtons[11].Click += (sender, e) => this.setMode(sender, e, 0);
			screenButtons[12] = help.CreateButton(31, 12, 6, 1, "LOCK");
			screenButtons[12].Font = form.buttonFont;
			screenButtons[12].Click += (sender, e) => this.setMode(sender, e, 2);

			screenButtons[13] = help.CreateButton(35, 5, 2, 2, "1");
			screenButtons[13].Font = form.buttonFont;
			screenButtons[13].Click += (sender, e) => this.setRotRate(sender, e, 1);

			screenButtons[14] = help.CreateButton(35, 7, 2, 2, "5");
			screenButtons[14].Font = form.buttonFont;
			screenButtons[14].Click += (sender, e) => this.setRotRate(sender, e, 5);

			// READOUT LABELS
			screenLabels[20] = help.CreateLabel(1, 16, 25, 1, "R: ±XXX.X  ±XXX.X  ±XXX.X");
			screenLabels[21] = help.CreateLabel(1, 17, 25, 1, "P: ±XXX.X  ±XXX.X  ±XXX.X");
			screenLabels[22] = help.CreateLabel(1, 18, 25, 1, "Y: ±XXX.X  ±XXX.X  ±XXX.X");

			// STAGE LABEL
			screenLabels[25] = help.CreateLabel(30, 15, 7, 1, "CUR: XX");

			// STAGE BUTTON
			screenButtons[15] = help.CreateButton(30, 17, 7, 2, "STAGE");
			screenButtons[15].Font = form.buttonFont;
			screenButtons[15].Click += this.stage;

			// PROGRAM LABEL
			screenLabels[30] = help.CreateLabel(1, 22, 23, 1, "RAT: X.X/s   RAT X.X/s");

			// PROGRAM BUTTONS
			screenButtons[20] = help.CreateButton(2, 23, 3, 1, "-");
			screenButtons[20].Font = form.buttonFont;
			screenButtons[20].Click += this.rollRateMinus;
			screenButtons[21] = help.CreateButton(7, 23, 3, 1, "+");
			screenButtons[21].Font = form.buttonFont;
			screenButtons[21].Click += this.rollRatePlus;
			screenButtons[22] = help.CreateButton(14, 23, 3, 1, "-");
			screenButtons[22].Font = form.buttonFont;
			screenButtons[22].Click += this.pitchRateMinus;
			screenButtons[23] = help.CreateButton(19, 23, 3, 1, "+");
			screenButtons[23].Font = form.buttonFont;
			screenButtons[23].Click += this.pitchRatePlus;

			screenButtons[24] = help.CreateButton(1, 27, 10, 1, "RUN");
			screenButtons[24].Font = form.buttonFont;
			screenButtons[24].Click += this.rollProgramRun;

			screenButtons[25] = help.CreateButton(13, 27, 10, 1, "RUN");
			screenButtons[25].Font = form.buttonFont;
			screenButtons[25].Click += this.pitchProgramRun;

			// PROGRAM INDICATORS
			screenIndicators[20] = help.CreateIndicator(1, 25, 10, 1, "ENGAGED");
			screenIndicators[21] = help.CreateIndicator(13, 25, 10, 1, "ENGAGED");

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

			screenIndicators[25] = help.CreateIndicator(20, 10, 10, 1, "AUTO");
			screenIndicators[26] = help.CreateIndicator(20, 11, 10, 1, "FREE/SAS");
			screenIndicators[27] = help.CreateIndicator(20, 12, 10, 1, "LOCK");
			screenIndicators[28] = help.CreateIndicator(20, 13, 10, 1, "PROGRAM");


			// SAS BUTTONS
			screenButtons[30] = help.CreateButton(48, 22, 7, 1, "PRO");
			screenButtons[30].Font = form.buttonFont;
			screenButtons[30].Click += (sender, e) => this.setSAS(sender, e, 1);

			screenButtons[31] = help.CreateButton(57, 22, 7, 1, "RETRO");
			screenButtons[31].Font = form.buttonFont;
			screenButtons[31].Click += (sender, e) => this.setSAS(sender, e, 2);

			screenButtons[32] = help.CreateButton(66, 22, 7, 1, "STAB");
			screenButtons[32].Font = form.buttonFont;
			screenButtons[32].Click += (sender, e) => this.setSAS(sender, e, 0);

			screenButtons[33] = help.CreateButton(48, 25, 7, 1, "NORM");
			screenButtons[33].Font = form.buttonFont;
			screenButtons[33].Click += (sender, e) => this.setSAS(sender, e, 3);

			screenButtons[34] = help.CreateButton(57, 25, 7, 1, "A NOR");
			screenButtons[34].Font = form.buttonFont;
			screenButtons[34].Click += (sender, e) => this.setSAS(sender, e, 4);

			screenButtons[35] = help.CreateButton(66, 25, 7, 1, "SAS");
			screenButtons[35].Font = form.buttonFont;
			screenButtons[35].Click += (sender, e) => this.setSAS(sender, e, 11);

			screenButtons[36] = help.CreateButton(48, 28, 7, 1, "R IN");
			screenButtons[36].Font = form.buttonFont;
			screenButtons[36].Click += (sender, e) => this.setSAS(sender, e, 5);

			screenButtons[37] = help.CreateButton(57, 28, 7, 1, "R OUT");
			screenButtons[37].Font = form.buttonFont;
			screenButtons[37].Click += (sender, e) => this.setSAS(sender, e, 6);

			screenButtons[38] = help.CreateButton(66, 28, 7, 1, "RCS");
			screenButtons[38].Font = form.buttonFont;
			screenButtons[38].Click += (sender, e) => this.setSAS(sender, e, 10);

			screenButtons[39] = help.CreateButton(39, 28, 7, 1, "NODE");
			screenButtons[39].Font = form.buttonFont;
			screenButtons[39].Click += (sender, e) => this.setSAS(sender, e, 7);


			// SAS INDICATORS
			screenIndicators[30] = help.CreateIndicator(48, 21, 7, 1, "PRO");
			screenIndicators[31] = help.CreateIndicator(57, 21, 7, 1, "RETRO");
			screenIndicators[32] = help.CreateIndicator(66, 21, 7, 1, "STAB");
			screenIndicators[33] = help.CreateIndicator(48, 24, 7, 1, "NORM");
			screenIndicators[34] = help.CreateIndicator(57, 24, 7, 1, "A NOR");
			screenIndicators[35] = help.CreateIndicator(66, 24, 7, 1, "SAS");
			screenIndicators[36] = help.CreateIndicator(48, 27, 7, 1, "R IN");
			screenIndicators[37] = help.CreateIndicator(57, 27, 7, 1, "R OUT");
			screenIndicators[38] = help.CreateIndicator(66, 27, 7, 1, "RCS");
			screenIndicators[39] = help.CreateIndicator(39, 27, 7, 1, "NODE");

			// ACTION GROUP LABELS RIGHT
			screenLabels[40] = help.CreateLabel(46, 21, 1, 1, "│");
			screenLabels[41] = help.CreateLabel(46, 22, 1, 1, "│");
			screenLabels[42] = help.CreateLabel(46, 23, 1, 1, "│");
			screenLabels[43] = help.CreateLabel(46, 24, 1, 1, "│");
			screenLabels[44] = help.CreateLabel(46, 25, 1, 1, "│");
			screenLabels[45] = help.CreateLabel(37, 26, 10, 1, "┌────────┘");
			screenLabels[46] = help.CreateLabel(37, 27, 1, 1, "│");
			screenLabels[47] = help.CreateLabel(37, 28, 1, 1, "│");

			// ActionGroup INDICATORS
			screenIndicators[40] = help.CreateIndicator(26, 21, 4, 1, "");
			screenIndicators[41] = help.CreateIndicator(31, 21, 4, 1, "");
			screenIndicators[42] = help.CreateIndicator(36, 21, 4, 1, "");
			screenIndicators[43] = help.CreateIndicator(41, 21, 4, 1, "");
			screenIndicators[44] = help.CreateIndicator(26, 24, 4, 1, "");
			screenIndicators[45] = help.CreateIndicator(31, 24, 4, 1, "");
			screenIndicators[46] = help.CreateIndicator(36, 24, 4, 1, "");
			screenIndicators[47] = help.CreateIndicator(41, 24, 4, 1, "");
			screenIndicators[48] = help.CreateIndicator(26, 27, 4, 1, "");
			screenIndicators[49] = help.CreateIndicator(31, 27, 4, 1, "");


			// ACTION GROUP BUTTONS
			screenButtons[40] = help.CreateButton(26, 22, 4, 1, "1");
			screenButtons[40].Font = form.buttonFont;
			screenButtons[40].Click += (sender, e) => toggleActionGroup(sender, e, 1);
			screenButtons[41] = help.CreateButton(31, 22, 4, 1, "2");
			screenButtons[41].Font = form.buttonFont;
			screenButtons[41].Click += (sender, e) => toggleActionGroup(sender, e, 2);
			screenButtons[42] = help.CreateButton(36, 22, 4, 1, "3");
			screenButtons[42].Font = form.buttonFont;
			screenButtons[42].Click += (sender, e) => toggleActionGroup(sender, e, 3);
			screenButtons[43] = help.CreateButton(41, 22, 4, 1, "4");
			screenButtons[43].Font = form.buttonFont;
			screenButtons[43].Click += (sender, e) => toggleActionGroup(sender, e, 4);
			screenButtons[44] = help.CreateButton(26, 25, 4, 1, "5");
			screenButtons[44].Font = form.buttonFont;
			screenButtons[44].Click += (sender, e) => toggleActionGroup(sender, e, 5);
			screenButtons[45] = help.CreateButton(31, 25, 4, 1, "6");
			screenButtons[45].Font = form.buttonFont;
			screenButtons[45].Click += (sender, e) => toggleActionGroup(sender, e, 6);
			screenButtons[46] = help.CreateButton(36, 25, 4, 1, "7");
			screenButtons[46].Font = form.buttonFont;
			screenButtons[46].Click += (sender, e) => toggleActionGroup(sender, e, 7);
			screenButtons[47] = help.CreateButton(41, 25, 4, 1, "8");
			screenButtons[47].Font = form.buttonFont;
			screenButtons[47].Click += (sender, e) => toggleActionGroup(sender, e, 8);
			screenButtons[48] = help.CreateButton(26, 28, 4, 1, "9");
			screenButtons[48].Font = form.buttonFont;
			screenButtons[48].Click += (sender, e) => toggleActionGroup(sender, e, 9);
			screenButtons[49] = help.CreateButton(31, 28, 4, 1, "0");
			screenButtons[49].Font = form.buttonFont;
			screenButtons[49].Click += (sender, e) => toggleActionGroup(sender, e, 0);

			// ACTION GROUP LABELS LEFT
			screenLabels[51] = help.CreateLabel(24, 21, 1, 1, "│");
			screenLabels[52] = help.CreateLabel(24, 22, 1, 1, "│");
			screenLabels[53] = help.CreateLabel(24, 23, 1, 1, "│");
			screenLabels[54] = help.CreateLabel(24, 24, 1, 1, "│");
			screenLabels[55] = help.CreateLabel(24, 25, 1, 1, "│");
			screenLabels[56] = help.CreateLabel(24, 26, 1, 1, "│");
			screenLabels[57] = help.CreateLabel(24, 27, 1, 1, "│");
			screenLabels[58] = help.CreateLabel(24, 28, 1, 1, "│");

			//for (int i = 0; i < 1; i++) form.screenCharts.Add(null); // Initialize Charts

			// Gee-Force vs. Time Graph
			//form.screenCharts[0] = help.CreateChart(60, 15, 60, 15, 0, 600);
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			// Re-usable data variable for graph data
			//List<Dictionary<int, Nullable<double>>> data = new List<Dictionary<int, Nullable<double>>>();

			// Always update Local Time
			screenLabels[0].Text = " LT: " + help.timeString(DateTime.Now.TimeOfDay.TotalSeconds);

			// ROTATIONAL INDICATORS Are local, so always update these also
			if (rotStep == 1)
			{
				screenIndicators[22].setStatus(4);
				screenIndicators[23].setStatus(0);
			}
			else if (rotStep == 5)
			{
				screenIndicators[22].setStatus(0);
				screenIndicators[23].setStatus(4);
			}

			if (form.connected && krpc.CurrentGameScene == GameScene.Flight)
			{
				// INITIALIZE STREAMS
				if (this.vessel_stream == null) this.vessel_stream = connection.AddStream(() => spaceCenter.ActiveVessel);
				if (this.flight_stream == null) this.flight_stream = connection.AddStream(() => spaceCenter.ActiveVessel.Flight(spaceCenter.ActiveVessel.Orbit.Body.ReferenceFrame));
				if (this.resources_stream == null) this.resources_stream = connection.AddStream(() => spaceCenter.ActiveVessel.Resources);
				if (this.control_stream == null) this.control_stream = connection.AddStream(() => spaceCenter.ActiveVessel.Control);
				if (this.orbit_stream == null) this.orbit_stream = connection.AddStream(() => spaceCenter.ActiveVessel.Orbit);
				if (this.vessel_resources_stage_stream == null) this.vessel_resources_stage_stream = connection.AddStream(() => spaceCenter.ActiveVessel.ResourcesInDecoupleStage(spaceCenter.ActiveVessel.Control.CurrentStage, false));

				// GET STREAM DATA
				this.vessel = this.vessel_stream.Get();
				this.flight = this.flight_stream.Get();
				this.vessel_control = this.control_stream.Get();
				this.vessel_resources = this.resources_stream.Get();
				this.orbit = this.orbit_stream.Get();
				this.vessel_resources_stage = this.vessel_resources_stage_stream.Get();

				/*
				screenLabels[1].Text = "MET: " + help.timeString(vessel.MET, 3);

				// THROTTLE
				screenLabels[16].Text = help.prtlen(Math.Ceiling(vessel_control.Throttle * 100).ToString() + "%", 4, helper.Align.RIGHT);

				// ROTATION READOUT
				double cR = flight.Roll;
				double cP = flight.Pitch;
				double cY = flight.Heading;

				double sR = this.setRotR;
				double sP = this.setRotP;
				double sY = this.setRotY;

				double? nR;
				double? nP;
				double? nY;
				

				// IF IN LOCK MODE SHOW LOCK ANGLES
				if(controlMode == 2)
				{
					nR = lockRotR;
					nP = lockRotP;
					nY = lockRotY;
				}
				else
				{
					nR = null;
					nP = null;
					nY = null;
				}
				screenLabels[20].Text = "R: " + help.prtlen(help.toFixed(sR, 1), 6) + "  " + help.prtlen(help.toFixed(cR, 1), 6) + "  " + help.prtlen(help.toFixed(nR, 1), 6);
				screenLabels[21].Text = "P: " + help.prtlen(help.toFixed(sP, 1), 6) + "  " + help.prtlen(help.toFixed(cP, 1), 6) + "  " + help.prtlen(help.toFixed(nP, 1), 6);
				screenLabels[22].Text = "Y: " + help.prtlen(help.toFixed(sY, 1), 6) + "  " + help.prtlen(help.toFixed(cY, 1), 6) + "  " + help.prtlen(help.toFixed(nY, 1), 6);

				// STAGE LABEL
				screenLabels[25].Text = "CUR: " + help.prtlen(vessel_control.CurrentStage.ToString(), 2);

				// Status
				if (vessel_control.SAS) { screenIndicators[0].setStatus(1); } else { screenIndicators[0].setStatus(0); } // SAS
				if (vessel_control.RCS) { screenIndicators[1].setStatus(1); } else { screenIndicators[1].setStatus(0); } // RCS
				if (vessel_control.Gear) { screenIndicators[2].setStatus(1); } else { screenIndicators[2].setStatus(0); } // GEAR
				if (vessel_control.Brakes) { screenIndicators[3].setStatus(2); } else { screenIndicators[3].setStatus(0); } // Break
				if (vessel_control.Lights) { screenIndicators[4].setStatus(4); } else { screenIndicators[4].setStatus(0); } // Lights
				if (vessel_control.Abort) { screenIndicators[5].setStatus(2); } else { screenIndicators[5].setStatus(0); } // Abort

				if (flight.GForce > 4) { screenIndicators[7].setStatus(4); } else { screenIndicators[7].setStatus(0); } // G High

				float maxR = vessel_resources.Max("ElectricCharge");
				float curR = vessel_resources.Amount("ElectricCharge");
				if (curR / maxR > 0.95) { screenIndicators[6].setStatus(1); } else { screenIndicators[6].setStatus(0); } // Power High
				if (curR / maxR < 0.1 && curR / maxR > 0) { screenIndicators[9].setStatus(2); } else { screenIndicators[9].setStatus(0); } // Power Low

				maxR = vessel_resources.Max("MonoPropellant");
				curR = vessel_resources.Amount("MonoPropellant");
				if (curR / maxR < 0.1 && curR / maxR > 0) { screenIndicators[10].setStatus(2); } else { screenIndicators[10].setStatus(0); } // Monopropellant Low

				maxR = vessel_resources_stage.Max("LiquidFuel");
				curR = vessel_resources_stage.Amount("LiquidFuel");
				if (curR / maxR < 0.1 && curR / maxR > 0) { screenIndicators[11].setStatus(2); } else { screenIndicators[11].setStatus(0); } // Fuel Low

				maxR = vessel_resources_stage.Max("Oxidizer");
				curR = vessel_resources_stage.Amount("Oxidizer");
				if (curR / maxR < 0.1 && curR / maxR > 0) { screenIndicators[8].setStatus(2); } else { screenIndicators[8].setStatus(0); } // LOW Low
				
				// Check for autopilot
				switch(controlMode)
				{
					case 0:
						screenIndicators[25].setStatus(0);
						screenIndicators[26].setStatus(4);
						screenIndicators[27].setStatus(0);
						screenIndicators[28].setStatus(0);
						screenIndicators[20].setStatus(0);
						screenIndicators[21].setStatus(0);
						break;
					case 1:
						screenIndicators[25].setStatus(1);
						screenIndicators[26].setStatus(0);
						screenIndicators[27].setStatus(0);
						screenIndicators[28].setStatus(0);
						screenIndicators[20].setStatus(0);
						screenIndicators[21].setStatus(0);
						break;
					case 2:
						screenIndicators[25].setStatus(0);
						screenIndicators[26].setStatus(0);
						screenIndicators[27].setStatus(2);
						screenIndicators[28].setStatus(0);
						screenIndicators[20].setStatus(0);
						screenIndicators[21].setStatus(0);
						break;
					case 3:
						screenIndicators[25].setStatus(0);
						screenIndicators[26].setStatus(0);
						screenIndicators[27].setStatus(0);
						screenIndicators[28].setStatus(4);
						screenIndicators[20].setStatus(1);
						screenIndicators[21].setStatus(0);
						break;
					case 4:
						screenIndicators[25].setStatus(0);
						screenIndicators[26].setStatus(0);
						screenIndicators[27].setStatus(0);
						screenIndicators[28].setStatus(4);
						screenIndicators[20].setStatus(0);
						screenIndicators[21].setStatus(1);
						break;
				}
				// PROGRAM LABEL
				screenLabels[30].Text = "RAT: " + help.toFixed(rollRate, 1) + "/s  RAT: " + help.toFixed(pitchRate, 1) + "/s";

				// SAS INDICATORS
				for(int i = 30; i < 40; i++)
				{
					screenIndicators[i].setStatus(0);
				}

				switch (vessel_control.SASMode)
				{
					case SASMode.Prograde:
						screenIndicators[30].setStatus(4);
						break;
					case SASMode.Retrograde:
						screenIndicators[31].setStatus(4);
						break;
					case SASMode.StabilityAssist:
						screenIndicators[32].setStatus(4);
						break;
					case SASMode.Normal:
						screenIndicators[33].setStatus(4);
						break;
					case SASMode.AntiNormal:
						screenIndicators[34].setStatus(4);
						break;
					case SASMode.Radial:
						screenIndicators[36].setStatus(4);
						break;
					case SASMode.AntiRadial:
						screenIndicators[37].setStatus(4);
						break;
					case SASMode.Maneuver:
						screenIndicators[39].setStatus(4);
						break;
				}

				if (vessel_control.SAS) { screenIndicators[35].setStatus(4); } else { screenIndicators[35].setStatus(0); }
				if (vessel_control.RCS) { screenIndicators[38].setStatus(4); } else { screenIndicators[38].setStatus(0); }


				// Action Group Indicators
				for (uint i = 0; i < 10; i++)
				{
					uint g = i;
					if (i == 0) { g = 10; }

					if (vessel_control.GetActionGroup(i))
					{
						screenIndicators[(int)(39 + g)].setStatus(4);
					}
					else
					{
						screenIndicators[(int)(39 + g)].setStatus(0);
					}
				}


				// Graphs
				//data = new List<Dictionary<int, Nullable<double>>>();
				//data.Add(chartData["geeTime"]);
				//form.showData(0, data, false);

				// SET TARGET FOR AUTOPILOT IF MODE IS AUTO
				if (controlMode == 1)
				{
					vessel.AutoPilot.TargetPitch = this.setRotP;
					vessel.AutoPilot.TargetRoll = this.setRotR;
					vessel.AutoPilot.TargetHeading = this.setRotY;
				}
				else if (controlMode == 2)
				{
					vessel.AutoPilot.TargetPitch = this.lockRotP;
					vessel.AutoPilot.TargetRoll = this.lockRotR;
					vessel.AutoPilot.TargetHeading = this.lockRotY;
				}
				/**/
			}
		}

		private void pitchUp(object sender, EventArgs e){ this.setRotP += this.rotStep; }
		private void pitchDown(object sender, EventArgs e) { this.setRotP -= this.rotStep; }
		private void yawLeft(object sender, EventArgs e) { this.setRotY += this.rotStep; }
		private void yawRight(object sender, EventArgs e) { this.setRotY -= this.rotStep; }
		private void rollLeft(object sender, EventArgs e) { this.setRotR += this.rotStep; }
		private void rollRight(object sender, EventArgs e) { this.setRotR -= this.rotStep; }

		private void setMode(object sender, EventArgs e, int mode)
		{
			switch(mode)
			{
				case 0: // FREE
					vessel.AutoPilot.Disengage();
					this.controlMode = 0;
					break;
				case 1: // AUTOPILOT
					vessel.AutoPilot.Engage();
					this.controlMode = 1;
					break;
				case 2: // LOCK
					lockRotR = setRotR;
					lockRotP = setRotP;
					lockRotY = setRotY;

					vessel.AutoPilot.Engage();
					this.controlMode = 2;
					break;
				case 3: // Roll Program
					vessel.AutoPilot.Engage();
					this.controlMode = 3;
					break;
				case 4: // Pitch Program
					vessel.AutoPilot.Engage();
					this.controlMode = 4;
					break;
			}
		}

		private void setRotRate(object sender, EventArgs e, int rate)
		{
			rotStep = rate;
		}

		private void stage(object sender, EventArgs e)
		{
			vessel.Control.ActivateNextStage();
		}

		private void toggleActionGroup(object sender, EventArgs e, uint group)
		{
			// Get action group status
			bool active = vessel.Control.GetActionGroup(group);

			if (active) { vessel.Control.SetActionGroup(group, false); }
			else { vessel.Control.SetActionGroup(group, true); }
		}

		private void rollRateMinus(object sender, EventArgs e)
		{
			rollRate -= 0.1;
			if(rollRate < 0) { rollRate = 0; }
		}

		private void rollRatePlus(object sender, EventArgs e)
		{
			rollRate += 0.1;
		}

		private void pitchRateMinus(object sender, EventArgs e)
		{
			pitchRate -= 0.1;
			if (pitchRate < 0) { pitchRate = 0; }
		}

		private void pitchRatePlus(object sender, EventArgs e)
		{
			pitchRate += 0.1;
		}

		private void rollProgramRun(object sender, EventArgs e)
		{
			controlMode = 3;
			Thread thread = new Thread(rollProgram);
			thread.Start();
		}

		private void pitchProgramRun(object sender, EventArgs e)
		{
			controlMode = 4;
			Thread thread = new Thread(pitchProgram);
			thread.Start();
		}

		private void changeThrottle(object sender, EventArgs e, double change)
		{
			double cThr = vessel.Control.Throttle;
			double nThr = cThr + change;
			if(nThr > 100){ nThr = 100; }
			if (nThr < 0) { nThr = 0; }
			vessel.Control.Throttle = (float)nThr;
		}

		private void setSAS(object sender, EventArgs e, int mode)
		{
			switch(mode)
			{
				case 0:
					vessel.Control.SASMode = SASMode.StabilityAssist;
					break;
				case 1:
					vessel.Control.SASMode = SASMode.Prograde;
					break;
				case 2:
					vessel.Control.SASMode = SASMode.Retrograde;
					break;
				case 3:
					vessel.Control.SASMode = SASMode.Normal;
					break;
				case 4:
					vessel.Control.SASMode = SASMode.AntiNormal;
					break;
				case 5:
					vessel.Control.SASMode = SASMode.Radial;
					break;
				case 6:
					vessel.Control.SASMode = SASMode.AntiRadial;
					break;
				case 7:
					if (vessel.Control.Nodes.Count > 0)
					{
						vessel.Control.SASMode = SASMode.Maneuver;
					}
					break;
				case 10:
					if (vessel.Control.RCS)
					{
						vessel.Control.RCS = false;
					}
					else
					{
						vessel.Control.RCS = true;
					}
					break;
				case 11:
					if (vessel.Control.SAS)
					{
						vessel.Control.SAS = false;
					}
					else
					{
						vessel.Control.SAS = true;
						vessel.AutoPilot.Disengage();
						controlMode = 0;
					}
					break;
			}
		}

		private void rollProgram()
		{
			//Console.WriteLine("ROLL PROGRAM");
			double curRoll = Math.Round(flight.Roll);
			double dstRoll = setRotR;
			double rate;
			bool done = false;

			if (dstRoll > curRoll)
			{
				rate = rollRate;
			}
			else
			{
				rate = rollRate * -1;
			}

			vessel.AutoPilot.Engage();
			Stopwatch loopTime = new Stopwatch();
			int sleepTime;

			while (curRoll != dstRoll && controlMode == 3)
			{
				//Console.WriteLine("ROLLING " + dstRoll.ToString() + ":" + curRoll.ToString() + ":" + rate.ToString());
				curRoll = Math.Round(curRoll + (rate / 10), 2);

				vessel.AutoPilot.TargetRoll = (float)curRoll;
				vessel.AutoPilot.TargetPitch = setRotP;
				vessel.AutoPilot.TargetHeading= setRotY;

				if (Math.Round(curRoll) == dstRoll)
				{
					curRoll = Math.Round(curRoll);
					vessel.AutoPilot.TargetPitch = (float)curRoll;
					done = true;
				}

				sleepTime = (int)(100 - (double)loopTime.ElapsedMilliseconds);
				if (sleepTime < 5) { sleepTime = 5; }
				Thread.Sleep(sleepTime);
			}

			if (done) { controlMode = 1; }
		}

		private void pitchProgram()
		{
			//Console.WriteLine("PITCH PROGRAM");
			double curPitch = Math.Round(flight.Pitch);
			double dstPitch = setRotP;
			double rate;
			bool done = false;

			if (dstPitch > curPitch)
			{
				rate = pitchRate;
			}
			else
			{
				rate = pitchRate * -1;
			}

			vessel.AutoPilot.Engage();

			Stopwatch loopTime = new Stopwatch();
			int sleepTime;

			while (curPitch != dstPitch && controlMode == 4)
			{
				loopTime.Restart();

				//Console.WriteLine("ROLLING " + dstPitch.ToString() + ":" + curPitch.ToString() + ":" + rate.ToString());
				curPitch = Math.Round(curPitch + (rate / 10), 2);

				vessel.AutoPilot.TargetPitch = (float)curPitch;
				vessel.AutoPilot.TargetRoll = setRotR;
				vessel.AutoPilot.TargetHeading = setRotY;

				if(Math.Round(curPitch) == dstPitch)
				{
					curPitch = Math.Round(curPitch);
					vessel.AutoPilot.TargetPitch = (float)curPitch;
					done = true;
				}

				loopTime.Stop();

				sleepTime = (int)(100 - (double)loopTime.ElapsedMilliseconds);
				if(sleepTime < 5) { sleepTime = 5; }
				Thread.Sleep(sleepTime);
			}

			if (done) { controlMode = 1; }
		}
	}
}
