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
	partial class Pilot1 : MocrScreen
	{
		private int setRotR = 0;
		private int setRotP = 0;
		private int setRotY = 0;
		private int rotStep = 5;

		private int lockRotR;
		private int lockRotP;
		private int lockRotY;
		
		private double rollRate = 1;
		private double pitchRate = 1;

		private int controlMode = 0; // 0: Free, 1: Autopilot, 2: Lock, 3: Roll program, 4: pitch program

		// DKSY PROPERITES
		bool runOnceProg = false;
		bool runOnceVerb = false;
		int progStep = 0;
		bool proPress = false;
		bool entrPress = false;
		bool keyRelPress = false;
		bool enterVerb = false;
		bool enterNoun = false;
		bool enterR1 = false;
		bool enterR2 = false;
		bool enterR3 = false;
		String noun = null;
		String verb = null;
		String r1 = "";
		String r2 = "";
		String r3 = "";
		int r1precision = 0;
		int r2precision = 0;
		int r3precision = 0;
		SegDisp.SignState r1sign = SegDisp.SignState.NONE;
		SegDisp.SignState r2sign = SegDisp.SignState.NONE;
		SegDisp.SignState r3sign = SegDisp.SignState.NONE;
		int activeVerb = -1;
		int activeNoun = -1;
		int activeProg = 0;
		int storeProg = -1;
		bool flashVerb = false;
		bool flashNoun = false;
		bool flashProg = false;
		bool flashR1 = false;
		bool flashR2 = false;
		bool flashR3 = false;
		bool flashOn = false;
		long flashStart = 0;
		DateTime flashNow;
		long flashDur = 500;
		bool oprError = false;
		bool keyRel = false;
		float TIG = 0;
		float deltaV = 0;
		float burnTime = 0;
		List<int> dataStorage = new List<int>();
		double launchAzimuth = 90;

		// FDAI PROPERTIES
		enum FDAIMode { SURF, INER }
		FDAIMode FDAImode;
		

		// DATA PROPERTIES
		int oldStage = -1;
		int currentStage = -1;
		double meanAltitude = 0;
		double MET = 0;
		float throttle = 0;
		float pitch;
		float roll;
		float yaw;
		float inerRoll = 0;
		float inerPitch = 0;
		float inerYaw = 0;
		bool SAS = false;
		bool RCS = false;
		bool gear = false;
		bool brakes = false;
		bool lights = false;
		bool abort = false;
		float gForce = 0;
		float maxElectric = 0;
		float curElectric = 0;
		float maxMonopropellant = 0;
		float curMonopropellant = 0;
		float maxStageFuel = 0;
		float curStageFuel = 0;
		float maxStageOx = 0;
		float curStageOx = 0;
		float maxTotFuel = 0;
		float curTotFuel = 0;
		float maxTotOx = 0;
		float curTotOx = 0;
		SASMode sasMode = 0;
		bool actionGroup0 = false;
		bool actionGroup1 = false;
		bool actionGroup2 = false;
		bool actionGroup3 = false;
		bool actionGroup4 = false;
		bool actionGroup5 = false;
		bool actionGroup6 = false;
		bool actionGroup7 = false;
		bool actionGroup8 = false;
		bool actionGroup9 = false;
		double orbitSpeed = 0;
		double apoapsis = 0;
		double periapsis = 0;
		Tuple<double, double, double> vesselSurfDirection;
		Tuple<double, double, double> vesselInerDirection;
		Tuple<double, double, double, double> vesselInerRotation;

		ReferenceFrame surfaceRefsmmat;
		ReferenceFrame inertialRefsmmat;

		// VECTOR DATA (FOR DIRECTION)
		double vector1 = 0;
		double vector2 = 0;
		double vector3 = 0;
		
		double tRoll = 0;
		double tPitch = 0;
		double tYaw = 0;

		DateTime start;
		DateTime end;
		TimeSpan dur;
		int block = 1;

		public Pilot1(Screen form)
		{
			this.form = form;
			this.chartData = form.form.chartData;
			screenStreams = new StreamCollection(form.connection);

			this.updateRate = 500;

			this.width = 120;
			this.height = 30;
			
			oldStage = currentStage = screenStreams.GetData(DataType.control_currentStage);
			
			Thread thread = new Thread(updateFDAIDKSY);
			thread.IsBackground = true;
			thread.Start();
		}

		private void updateFDAIDKSY()
		{
			DateTime loopTime = new DateTime();
			long start;
			long duration;
			
			int sleepTime;

			while (this != null)
			{
				start = loopTime.Ticks;

				// Refresh FDAI values and display
				if (screenFDAI != null)
				{
					if (FDAImode == FDAIMode.SURF)
					{
						screenFDAI.setRotation(screenStreams.GetData(DataType.flight_rotation));
					}
					else
					{
						screenFDAI.setRotation(screenStreams.GetData(DataType.flight_inertial_rotation));
					}

					screenFDAI.Invalidate();
				}

				// Rotate flashing status
				if (flashR1 || flashR2 || flashR3 || flashVerb || flashNoun || flashProg)
				{
					if (flashOn)
					{
						flashNow = DateTime.Now;
						if (flashNow.Ticks > (flashStart + (flashDur * 10000)))
						{
							flashOn = false;
							flashStart = flashNow.Ticks;
						}
					}
					else
					{
						flashNow = DateTime.Now;
						if (flashNow.Ticks > (flashStart + (flashDur * 10000)))
						{
							flashOn = true;
							flashStart = flashNow.Ticks;
						}
					}
				}

				// Run the computer
				// Refresh DSKY displays
				if (screenSegDisps.Count > 5 && screenSegDisps[5] != null)
				{
					updateDSKY();
					if (screenSegDisps[0] != null) { update7Seg(screenSegDisps[0], flashR1); }
					if (screenSegDisps[1] != null) { update7Seg(screenSegDisps[1], flashR2); }
					if (screenSegDisps[2] != null) { update7Seg(screenSegDisps[2], flashR3); }
					if (screenSegDisps[3] != null) { update7Seg(screenSegDisps[3], flashVerb); }
					if (screenSegDisps[4] != null) { update7Seg(screenSegDisps[4], flashNoun); }
					if (screenSegDisps[5] != null) { update7Seg(screenSegDisps[5], flashProg); }
				}


				duration = loopTime.Ticks - start;
				sleepTime = (int)(40 - (double)(duration / 10000));
				if (sleepTime < 20)
				{
					sleepTime = 20;
					if (screenIndicators.Count > 50 && screenIndicators[50] != null)
					{
						screenIndicators[50].setStatus(Indicator.status.GREEN);
					}
				}
				else
				{
					if (screenIndicators.Count > 50 && screenIndicators[50] != null)
					{
						screenIndicators[50].setStatus(Indicator.status.OFF);
					}
				}
				Thread.Sleep(sleepTime);
			}
		}

		private void update7Seg(SegDisp disp, bool flash)
		{
			if (flash)
			{
				if (flashOn)
				{
					disp.off = true;
				}
				else
				{
					disp.off = false;
				}
			}
			else
			{
				disp.off = false;
			}
			disp.Invalidate();
		}

		public override void makeElements()
		{
			for (int i = 0; i < 100; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 70; i++) screenIndicators.Add(null); // Initialize Indicators
			for (int i = 0; i < 80; i++) screenButtons.Add(null); // Initialize Buttons
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			for (int i = 0; i < 5; i++) screenVMeters.Add(null); // Initialize VMeters
			for (int i = 0; i < 6; i++) screenSegDisps.Add(null); // Initialize 7-Segment Displays

			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			// LABELS
			screenLabels[0] = Helper.CreateLabel(16, 1, 13); // Local Time
			screenLabels[1] = Helper.CreateLabel(0, 1, 14); // MET Time
			screenLabels[2] = Helper.CreateLabel(39, 0, 42, 1, "=============== PILOT MODULE ============="); // Screen Title
			screenLabels[3] = Helper.CreateLabel(84, 0, 39, 1, "├───────────── STATUS ─────────────┤"); // Status Headline
			screenLabels[4] = Helper.CreateLabel(84, 1, 1, 1, "│");
			screenLabels[5] = Helper.CreateLabel(0, 2, 85, 1, "────────────────────────────────────────────────────────────────────────────────────┤"); // Obrit/Position headline
			screenLabels[6] = Helper.CreateLabel(0, 3, 38, 1, "┌───── TRANS ─────┐┌────── ROT ──────┐");
			screenLabels[7] = Helper.CreateLabel(0, 10, 19, 1, "┌──── THROTTLE ───┐");
			screenLabels[8] = Helper.CreateLabel(0, 14, 38, 1, "┌───────── READOUT ─────────┐┌ STAGE ┐");
			screenLabels[9] = Helper.CreateLabel(0, 15, 25, 1, "      SET     CUR    LOCK");
			screenLabels[10] = Helper.CreateLabel(0, 20, 80, 1, "───────── PROGRAMS ─────┬─── ACTION GROUPS ───┬─────── SAS SETTING ───────┐");
			screenLabels[11] = Helper.CreateLabel(0, 21, 24, 1, "┌── ROLL ──┐┌── PITCH ─┐");
			screenLabels[12] = Helper.CreateLabel(0, 29, 75, 1, "────────────────────────┴────────────┴────────────────────────────────────┤");

			// THROTTLE CONTROLS
			screenButtons[0] = Helper.CreateButton(1, 12, 5, 1, "-10");
			screenButtons[0].Font = form.buttonFont;
			screenButtons[0].Click += (sender, e) => this.changeThrottle(sender, e, -0.1);
			screenButtons[1] = Helper.CreateButton(1, 11, 5, 1, "-1");
			screenButtons[1].Font = form.buttonFont;
			screenButtons[1].Click += (sender, e) => this.changeThrottle(sender, e, -0.01);
			screenButtons[2] = Helper.CreateButton(7, 11, 5, 1, "+1");
			screenButtons[2].Font = form.buttonFont;
			screenButtons[2].Click += (sender, e) => this.changeThrottle(sender, e, 0.01);
			screenButtons[3] = Helper.CreateButton(7, 12, 5, 1, "+10");
			screenButtons[3].Font = form.buttonFont;
			screenButtons[3].Click += (sender, e) => this.changeThrottle(sender, e, 0.1);
			screenButtons[16] = Helper.CreateButton(1, 13, 5, 1, "-100");
			screenButtons[16].Font = form.buttonFont;
			screenButtons[16].Click += (sender, e) => this.changeThrottle(sender, e, -1);
			screenButtons[17] = Helper.CreateButton(7, 13, 5, 1, "+100");
			screenButtons[17].Font = form.buttonFont;
			screenButtons[17].Click += (sender, e) => this.changeThrottle(sender, e, 1);

			// THROTTLE LABLES
			screenLabels[15] = Helper.CreateLabel(13, 11, 4, 1, "CUR:");
			screenLabels[16] = Helper.CreateLabel(13, 12, 4, 1, "100%");

			// ROTATIONAL LABELS
			screenLabels[17] = Helper.CreateLabel(33, 4, 4, 1, "RATE");

			// ROTATIONAL INDICATORS
			screenIndicators[22] = Helper.CreateIndicator(33, 5, 2, 2, "");
			screenIndicators[23] = Helper.CreateIndicator(33, 7, 2, 2, "");

			// ROTATIONAL CONTROLS
			screenButtons[4] = Helper.CreateButton(25, 4, 3, 2, "⭡");
			screenButtons[4].Click += this.pitchUp;
			screenButtons[5] = Helper.CreateButton(25, 7, 3, 2, "⭣");
			screenButtons[5].Click += this.pitchDown;
			screenButtons[6] = Helper.CreateButton(21, 7, 3, 2, "⭠");
			screenButtons[6].Click += this.yawLeft;
			screenButtons[7] = Helper.CreateButton(29, 7, 3, 2, "⭢");
			screenButtons[7].Click += this.yawRight;
			screenButtons[8] = Helper.CreateButton(21, 4, 3, 2, "⟲");
			screenButtons[8].Click += this.rollLeft;
			screenButtons[9] = Helper.CreateButton(29, 4, 3, 2, "⟳");
			screenButtons[9].Click += this.rollRight;

			screenButtons[10] = Helper.CreateButton(31, 10, 6, 1, "AUTO");
			screenButtons[10].Font = form.buttonFont;
			screenButtons[10].Click += (sender, e) => this.setMode(sender, e, 1);
			screenButtons[11] = Helper.CreateButton(31, 11, 6, 1, "FREE");
			screenButtons[11].Font = form.buttonFont;
			screenButtons[11].Click += (sender, e) => this.setMode(sender, e, 0);
			screenButtons[12] = Helper.CreateButton(31, 12, 6, 1, "LOCK");
			screenButtons[12].Font = form.buttonFont;
			screenButtons[12].Click += (sender, e) => this.setMode(sender, e, 2);

			screenButtons[13] = Helper.CreateButton(35, 5, 2, 2, "1");
			screenButtons[13].Font = form.buttonFont;
			screenButtons[13].Click += (sender, e) => this.setRotRate(sender, e, 1);

			screenButtons[14] = Helper.CreateButton(35, 7, 2, 2, "5");
			screenButtons[14].Font = form.buttonFont;
			screenButtons[14].Click += (sender, e) => this.setRotRate(sender, e, 5);

			// READOUT LABELS
			screenLabels[20] = Helper.CreateLabel(1, 16, 25, 1, "R: ±XXX.X  ±XXX.X  ±XXX.X");
			screenLabels[21] = Helper.CreateLabel(1, 17, 25, 1, "P: ±XXX.X  ±XXX.X  ±XXX.X");
			screenLabels[22] = Helper.CreateLabel(1, 18, 25, 1, "Y: ±XXX.X  ±XXX.X  ±XXX.X");

			// STAGE LABEL
			screenLabels[25] = Helper.CreateLabel(30, 15, 7, 1, "CUR: XX");

			// STAGE BUTTON
			screenButtons[15] = Helper.CreateButton(30, 17, 7, 2, "STAGE");
			screenButtons[15].Font = form.buttonFont;
			screenButtons[15].Click += this.stage;

			// PROGRAM LABEL
			screenLabels[30] = Helper.CreateLabel(1, 22, 23, 1, "RAT: X.X/s   RAT X.X/s");

			// PROGRAM BUTTONS
			screenButtons[20] = Helper.CreateButton(2, 23, 3, 1, "-");
			screenButtons[20].Font = form.buttonFont;
			screenButtons[20].Click += this.rollRateMinus;
			screenButtons[21] = Helper.CreateButton(7, 23, 3, 1, "+");
			screenButtons[21].Font = form.buttonFont;
			screenButtons[21].Click += this.rollRatePlus;
			screenButtons[22] = Helper.CreateButton(14, 23, 3, 1, "-");
			screenButtons[22].Font = form.buttonFont;
			screenButtons[22].Click += this.pitchRateMinus;
			screenButtons[23] = Helper.CreateButton(19, 23, 3, 1, "+");
			screenButtons[23].Font = form.buttonFont;
			screenButtons[23].Click += this.pitchRatePlus;

			screenButtons[24] = Helper.CreateButton(1, 27, 10, 1, "RUN");
			screenButtons[24].Font = form.buttonFont;
			screenButtons[24].Click += this.rollProgramRun;

			screenButtons[25] = Helper.CreateButton(13, 27, 10, 1, "RUN");
			screenButtons[25].Font = form.buttonFont;
			screenButtons[25].Click += this.pitchProgramRun;

			// PROGRAM INDICATORS
			screenIndicators[20] = Helper.CreateIndicator(1, 25, 10, 1, "ENGAGED");
			screenIndicators[21] = Helper.CreateIndicator(13, 25, 10, 1, "ENGAGED");

			// Status
			screenIndicators[0] = Helper.CreateIndicator(86, 1, 10, 1, "SAS");
			screenIndicators[1] = Helper.CreateIndicator(97, 1, 10, 1, "RCS");
			screenIndicators[2] = Helper.CreateIndicator(108, 1, 10, 1, "GEAR");
			screenIndicators[3] = Helper.CreateIndicator(86, 2, 10, 1, "BRAKES");
			screenIndicators[4] = Helper.CreateIndicator(97, 2, 10, 1, "LIGHTS");
			screenIndicators[5] = Helper.CreateIndicator(108, 2, 10, 1, "ABORT");

			screenIndicators[25] = Helper.CreateIndicator(20, 10, 10, 1, "AUTO");
			screenIndicators[26] = Helper.CreateIndicator(20, 11, 10, 1, "FREE/SAS");
			screenIndicators[27] = Helper.CreateIndicator(20, 12, 10, 1, "LOCK");
			screenIndicators[28] = Helper.CreateIndicator(20, 13, 10, 1, "PROGRAM");


			// SAS BUTTONS
			screenButtons[30] = Helper.CreateButton(48, 22, 7, 1, "PRO");
			screenButtons[30].Font = form.buttonFont;
			screenButtons[30].Click += (sender, e) => this.setSAS(sender, e, 1);

			screenButtons[31] = Helper.CreateButton(57, 22, 7, 1, "RETRO");
			screenButtons[31].Font = form.buttonFont;
			screenButtons[31].Click += (sender, e) => this.setSAS(sender, e, 2);

			screenButtons[32] = Helper.CreateButton(66, 22, 7, 1, "STAB");
			screenButtons[32].Font = form.buttonFont;
			screenButtons[32].Click += (sender, e) => this.setSAS(sender, e, 0);

			screenButtons[33] = Helper.CreateButton(48, 25, 7, 1, "NORM");
			screenButtons[33].Font = form.buttonFont;
			screenButtons[33].Click += (sender, e) => this.setSAS(sender, e, 3);

			screenButtons[34] = Helper.CreateButton(57, 25, 7, 1, "A NOR");
			screenButtons[34].Font = form.buttonFont;
			screenButtons[34].Click += (sender, e) => this.setSAS(sender, e, 4);

			screenButtons[35] = Helper.CreateButton(66, 25, 7, 1, "SAS");
			screenButtons[35].Font = form.buttonFont;
			screenButtons[35].Click += (sender, e) => this.setSAS(sender, e, 11);

			screenButtons[36] = Helper.CreateButton(48, 28, 7, 1, "R IN");
			screenButtons[36].Font = form.buttonFont;
			screenButtons[36].Click += (sender, e) => this.setSAS(sender, e, 5);

			screenButtons[37] = Helper.CreateButton(57, 28, 7, 1, "R OUT");
			screenButtons[37].Font = form.buttonFont;
			screenButtons[37].Click += (sender, e) => this.setSAS(sender, e, 6);

			screenButtons[38] = Helper.CreateButton(66, 28, 7, 1, "RCS");
			screenButtons[38].Font = form.buttonFont;
			screenButtons[38].Click += (sender, e) => this.setSAS(sender, e, 10);

			screenButtons[39] = Helper.CreateButton(39, 28, 7, 1, "NODE");
			screenButtons[39].Font = form.buttonFont;
			screenButtons[39].Click += (sender, e) => this.setSAS(sender, e, 7);


			// SAS INDICATORS
			screenIndicators[30] = Helper.CreateIndicator(48, 21, 7, 1, "PRO");
			screenIndicators[31] = Helper.CreateIndicator(57, 21, 7, 1, "RETRO");
			screenIndicators[32] = Helper.CreateIndicator(66, 21, 7, 1, "STAB");
			screenIndicators[33] = Helper.CreateIndicator(48, 24, 7, 1, "NORM");
			screenIndicators[34] = Helper.CreateIndicator(57, 24, 7, 1, "A NOR");
			screenIndicators[35] = Helper.CreateIndicator(66, 24, 7, 1, "SAS");
			screenIndicators[36] = Helper.CreateIndicator(48, 27, 7, 1, "R IN");
			screenIndicators[37] = Helper.CreateIndicator(57, 27, 7, 1, "R OUT");
			screenIndicators[38] = Helper.CreateIndicator(66, 27, 7, 1, "RCS");
			screenIndicators[39] = Helper.CreateIndicator(39, 27, 7, 1, "NODE");

			// ACTION GROUP LABELS RIGHT
			screenLabels[40] = Helper.CreateLabel(46, 21, 1, 1, "│");
			screenLabels[41] = Helper.CreateLabel(46, 22, 1, 1, "│");
			screenLabels[42] = Helper.CreateLabel(46, 23, 1, 1, "│");
			screenLabels[43] = Helper.CreateLabel(46, 24, 1, 1, "│");
			screenLabels[44] = Helper.CreateLabel(46, 25, 1, 1, "│");
			screenLabels[45] = Helper.CreateLabel(37, 26, 10, 1, "┌────────┘");
			screenLabels[46] = Helper.CreateLabel(37, 27, 1, 1, "│");
			screenLabels[47] = Helper.CreateLabel(37, 28, 1, 1, "│");

			// ActionGroup INDICATORS
			screenIndicators[40] = Helper.CreateIndicator(26, 21, 4, 1, "");
			screenIndicators[41] = Helper.CreateIndicator(31, 21, 4, 1, "");
			screenIndicators[42] = Helper.CreateIndicator(36, 21, 4, 1, "");
			screenIndicators[43] = Helper.CreateIndicator(41, 21, 4, 1, "");
			screenIndicators[44] = Helper.CreateIndicator(26, 24, 4, 1, "");
			screenIndicators[45] = Helper.CreateIndicator(31, 24, 4, 1, "");
			screenIndicators[46] = Helper.CreateIndicator(36, 24, 4, 1, "");
			screenIndicators[47] = Helper.CreateIndicator(41, 24, 4, 1, "");
			screenIndicators[48] = Helper.CreateIndicator(26, 27, 4, 1, "");
			screenIndicators[49] = Helper.CreateIndicator(31, 27, 4, 1, "");


			// ACTION GROUP BUTTONS
			screenButtons[40] = Helper.CreateButton(26, 22, 4, 1, "1");
			screenButtons[40].Font = form.buttonFont;
			screenButtons[40].Click += (sender, e) => toggleActionGroup(sender, e, 1);
			screenButtons[41] = Helper.CreateButton(31, 22, 4, 1, "2");
			screenButtons[41].Font = form.buttonFont;
			screenButtons[41].Click += (sender, e) => toggleActionGroup(sender, e, 2);
			screenButtons[42] = Helper.CreateButton(36, 22, 4, 1, "3");
			screenButtons[42].Font = form.buttonFont;
			screenButtons[42].Click += (sender, e) => toggleActionGroup(sender, e, 3);
			screenButtons[43] = Helper.CreateButton(41, 22, 4, 1, "4");
			screenButtons[43].Font = form.buttonFont;
			screenButtons[43].Click += (sender, e) => toggleActionGroup(sender, e, 4);
			screenButtons[44] = Helper.CreateButton(26, 25, 4, 1, "5");
			screenButtons[44].Font = form.buttonFont;
			screenButtons[44].Click += (sender, e) => toggleActionGroup(sender, e, 5);
			screenButtons[45] = Helper.CreateButton(31, 25, 4, 1, "6");
			screenButtons[45].Font = form.buttonFont;
			screenButtons[45].Click += (sender, e) => toggleActionGroup(sender, e, 6);
			screenButtons[46] = Helper.CreateButton(36, 25, 4, 1, "7");
			screenButtons[46].Font = form.buttonFont;
			screenButtons[46].Click += (sender, e) => toggleActionGroup(sender, e, 7);
			screenButtons[47] = Helper.CreateButton(41, 25, 4, 1, "8");
			screenButtons[47].Font = form.buttonFont;
			screenButtons[47].Click += (sender, e) => toggleActionGroup(sender, e, 8);
			screenButtons[48] = Helper.CreateButton(26, 28, 4, 1, "9");
			screenButtons[48].Font = form.buttonFont;
			screenButtons[48].Click += (sender, e) => toggleActionGroup(sender, e, 9);
			screenButtons[49] = Helper.CreateButton(31, 28, 4, 1, "0");
			screenButtons[49].Font = form.buttonFont;
			screenButtons[49].Click += (sender, e) => toggleActionGroup(sender, e, 0);

			// ACTION GROUP LABELS LEFT
			screenLabels[51] = Helper.CreateLabel(24, 21, 1, 1, "│");
			screenLabels[52] = Helper.CreateLabel(24, 22, 1, 1, "│");
			screenLabels[53] = Helper.CreateLabel(24, 23, 1, 1, "│");
			screenLabels[54] = Helper.CreateLabel(24, 24, 1, 1, "│");
			screenLabels[55] = Helper.CreateLabel(24, 25, 1, 1, "│");
			screenLabels[56] = Helper.CreateLabel(24, 26, 1, 1, "│");
			screenLabels[57] = Helper.CreateLabel(24, 27, 1, 1, "│");
			screenLabels[58] = Helper.CreateLabel(24, 28, 1, 1, "│");
			
			// SAS GROUP LABELS RIGHT
			screenLabels[60] = Helper.CreateLabel(74, 21, 1, 1, "│");
			screenLabels[61] = Helper.CreateLabel(74, 22, 1, 1, "│");
			screenLabels[62] = Helper.CreateLabel(74, 23, 1, 1, "│");
			screenLabels[63] = Helper.CreateLabel(74, 24, 1, 1, "│");
			screenLabels[64] = Helper.CreateLabel(74, 25, 1, 1, "│");
			screenLabels[65] = Helper.CreateLabel(74, 26, 1, 1, "│");
			screenLabels[66] = Helper.CreateLabel(74, 27, 1, 1, "│");
			screenLabels[67] = Helper.CreateLabel(74, 28, 1, 1, "│");

			// Vertical Meters
			screenVMeters[0] = Helper.CreateVMeter(83, 6, false,1);
			screenVMeters[0].subdivisions = 4;
			screenVMeters[0].setScale(0, 10);
			screenVMeters[1] = Helper.CreateVMeter(89, 6, true);
			screenVMeters[1].subdivisions = 2;
			screenVMeters[1].setScale(0, 100);
			screenVMeters[2] = Helper.CreateVMeter(99, 6, true);
			screenVMeters[2].subdivisions = 2;
			screenVMeters[2].setScale(0, 100);
			screenVMeters[3] = Helper.CreateVMeter(109, 6, true);
			screenVMeters[3].subdivisions = 2;
			screenVMeters[3].setScale(0, 100);
			
			// Vertical Meter Labels
			screenLabels[70] = Helper.CreateLabel(82, 3, 38, 1, "┌───────────── RESOURCES ────────────┐");
			screenLabels[71] = Helper.CreateLabel(82.5, 4, 38, 1, "         STAGE     TOTAL     TOTAL");
			screenLabels[72] = Helper.CreateLabel(82, 5, 38, 1, "  ACCEL  LF  OX    LF  OX    MP  EL");

			// DSKY
			screenSegDisps[0] = Helper.CreateSegDisp(83, 15.75, 5, true, "R1");
			screenSegDisps[1] = Helper.CreateSegDisp(83, 18.25, 5, true, "R2");
			screenSegDisps[2] = Helper.CreateSegDisp(83, 20.75, 5, true, "R3");
			
			screenSegDisps[3] = Helper.CreateSegDisp(104, 20.75, 2, false, "V");
			screenSegDisps[4] = Helper.CreateSegDisp(112, 20.75, 2, false, "N");
			
			screenSegDisps[5] = Helper.CreateSegDisp(112, 17, 2, false, "P");
			
			// DSKY INDICATORS
			screenIndicators[50] = Helper.CreateIndicator(103, 15.75, 7, 1, "COMP ACT");
			screenIndicators[50].Font = form.smallFont;
			screenIndicators[51] = Helper.CreateIndicator(103, 16.75, 7, 1, "PROG");
			screenIndicators[51].Font = form.buttonFont;
			screenIndicators[52] = Helper.CreateIndicator(103, 17.75, 7, 1, "OPR ERR");
			screenIndicators[52].Font = form.buttonFont;
			screenIndicators[53] = Helper.CreateIndicator(103, 18.75, 7, 1, "KEY REL");
			screenIndicators[53].Font = form.buttonFont;
			
			// DSKY LABELS
			screenLabels[75] = Helper.CreateLabel(111, 16, 8, 1, "─ PROG ─");
			screenLabels[76] = Helper.CreateLabel(111, 19.75, 8, 1, "─ NOUN ─");
			screenLabels[77] = Helper.CreateLabel(103, 19.75, 8, 1, "─ VERB ─");
			screenLabels[78] = Helper.CreateLabel(82, 15, 38, 1, "├────────────────────────────────────┤");

			// DSKY BUTTONS
			screenButtons[50] = Helper.CreateButton(78, 24.3, 5, 2, "VERB");
			screenButtons[50].Font = form.buttonFont;
			screenButtons[50].Click += verbClick;
			screenButtons[51] = Helper.CreateButton(78, 26.5, 5, 2, "NOUN");
			screenButtons[51].Font = form.buttonFont;
			screenButtons[51].Click += nounClick;
			
			screenButtons[52] = Helper.CreateButton(84, 23.2, 5, 2, "+");
			screenButtons[52].Font = form.buttonFont;
			screenButtons[52].Click += plusClick;
			screenButtons[53] = Helper.CreateButton(84, 25.4, 5, 2, "-");
			screenButtons[53].Font = form.buttonFont;
			screenButtons[53].Click += minusClick;
			screenButtons[54] = Helper.CreateButton(84, 27.6, 5, 2, "0");
			screenButtons[54].Font = form.buttonFont;
			screenButtons[54].Click += (sender, e) => dskyNumber(sender, e, 0);
			
			screenButtons[55] = Helper.CreateButton(90, 23.2, 5, 2, "7");
			screenButtons[55].Font = form.buttonFont;
			screenButtons[55].Click += (sender, e) => dskyNumber(sender, e, 7);
			screenButtons[56] = Helper.CreateButton(90, 25.4, 5, 2, "4");
			screenButtons[56].Font = form.buttonFont;
			screenButtons[56].Click += (sender, e) => dskyNumber(sender, e, 4);
			screenButtons[57] = Helper.CreateButton(90, 27.6, 5, 2, "1");
			screenButtons[57].Font = form.buttonFont;
			screenButtons[57].Click += (sender, e) => dskyNumber(sender, e, 1);
			
			screenButtons[58] = Helper.CreateButton(96, 23.2, 5, 2, "8");
			screenButtons[58].Font = form.buttonFont;
			screenButtons[58].Click += (sender, e) => dskyNumber(sender, e, 8);
			screenButtons[59] = Helper.CreateButton(96, 25.4, 5, 2, "5");
			screenButtons[59].Font = form.buttonFont;
			screenButtons[59].Click += (sender, e) => dskyNumber(sender, e, 5);
			screenButtons[60] = Helper.CreateButton(96, 27.6, 5, 2, "2");
			screenButtons[60].Font = form.buttonFont;
			screenButtons[60].Click += (sender, e) => dskyNumber(sender, e, 2);
			
			screenButtons[61] = Helper.CreateButton(102, 23.2, 5, 2, "9");
			screenButtons[61].Font = form.buttonFont;
			screenButtons[61].Click += (sender, e) => dskyNumber(sender, e, 9);
			screenButtons[62] = Helper.CreateButton(102, 25.4, 5, 2, "6");
			screenButtons[62].Font = form.buttonFont;
			screenButtons[62].Click += (sender, e) => dskyNumber(sender, e, 6);
			screenButtons[63] = Helper.CreateButton(102, 27.6, 5, 2, "3");
			screenButtons[63].Font = form.buttonFont;
			screenButtons[63].Click += (sender, e) => dskyNumber(sender, e, 3);
			
			screenButtons[64] = Helper.CreateButton(108, 23.2, 5, 2, "CLR");
			screenButtons[64].Font = form.buttonFont;
			screenButtons[64].Click += clrClick;
			screenButtons[65] = Helper.CreateButton(108, 25.4, 5, 2, "PRO");
			screenButtons[65].Font = form.buttonFont;
			screenButtons[65].Click += proClick;
			screenButtons[66] = Helper.CreateButton(108, 27.6, 5, 2, "KEY\nREL");
			screenButtons[66].Font = form.buttonFont;
			screenButtons[66].Click += keyRelClick;
			
			screenButtons[67] = Helper.CreateButton(114, 24.3, 5, 2, "ENTR");
			screenButtons[67].Font = form.buttonFont;
			screenButtons[67].Click += entrClick;
			screenButtons[68] = Helper.CreateButton(114, 26.5, 5, 2, "RSET");
			screenButtons[68].Font = form.buttonFont;
			screenButtons[68].Click += rsetClick;
			
			
			// FDAI LABELS
			screenLabels[80] = Helper.CreateLabel(38, 3, 8, 1, "┌ FDAI ─");
			screenLabels[81] = Helper.CreateLabel(38, 4, 1, 1, "│");
			screenLabels[82] = Helper.CreateLabel(38, 5, 1, 1, "│");
			screenLabels[83] = Helper.CreateLabel(38, 6, 1, 1, "│");
			screenLabels[84] = Helper.CreateLabel(38, 7, 1, 1, "│");
			screenLabels[85] = Helper.CreateLabel(38, 8, 1, 1, "│");
			screenLabels[86] = Helper.CreateLabel(38, 9, 1, 1, "│");
			screenLabels[87] = Helper.CreateLabel(38, 10, 1, 1, "│");
			screenLabels[88] = Helper.CreateLabel(38, 11, 1, 1, "│");
			screenLabels[89] = Helper.CreateLabel(38, 12, 1, 1, "│");
			screenLabels[90] = Helper.CreateLabel(38, 13, 1, 1, "│");
			screenLabels[91] = Helper.CreateLabel(38, 14, 1, 1, "│");
			screenLabels[92] = Helper.CreateLabel(38, 15, 1, 1, "│");
			screenLabels[93] = Helper.CreateLabel(38, 16, 1, 1, "│");
			screenLabels[94] = Helper.CreateLabel(38, 17, 1, 1, "│");
			screenLabels[95] = Helper.CreateLabel(38, 18, 1, 1, "│");
			screenLabels[96] = Helper.CreateLabel(38, 19, 8, 1, "└───────");
			
			// FDAI INDICATORS
			screenIndicators[60] = Helper.CreateIndicator(39, 4, 6, 1, "");
			screenIndicators[61] = Helper.CreateIndicator(39, 7, 6, 1, "");
			
			// FDAI BUTTONS
			screenButtons[70] = Helper.CreateButton(39, 5, 6, 1, "SURF");
			screenButtons[70].Font = form.buttonFont;
			screenButtons[70].Click += (sender, e) => setFDAIMode(sender, e, FDAIMode.SURF);
			
			screenButtons[71] = Helper.CreateButton(39, 8, 6, 1, "INER");
			screenButtons[71].Font = form.buttonFont;
			screenButtons[71].Click += (sender, e) => setFDAIMode(sender, e, FDAIMode.INER);
			
			
			// FDAI
			screenFDAI = new FDAI();
			screenFDAI.Font = form.buttonFont;
			screenFDAI.Location = new Point((int)(46 * form.pxPrChar)+4, (int)(3 * form.pxPrLine) + 4);
			screenFDAI.Size = new Size((int)(36 * form.pxPrChar), (int)(17 * form.pxPrLine));
			form.Controls.Add(screenFDAI);

			//for (int i = 0; i < 1; i++) form.screenCharts.Add(null); // Initialize Charts

			// Gee-Force vs. Time Graph
			//form.screenCharts[0] = Helper.CreateChart(60, 15, 60, 15, 0, 600);

			// Load PySSSMQ-DATA
			loadPySSSMQData();
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			// Re-usable data variable for graph data
			//List<Dictionary<int, Nullable<double>>> data = new List<Dictionary<int, Nullable<double>>>();

			// Always update Local Time
			screenLabels[0].Text = " LT: " + Helper.timeString(DateTime.Now.TimeOfDay.TotalSeconds);

			// ROTATIONAL INDICATORS Are local, so always update these also
			if (rotStep == 1)
			{
				screenIndicators[22].setStatus(Indicator.status.AMBER);
				screenIndicators[23].setStatus(Indicator.status.OFF);
			}
			else if (rotStep == 5)
			{
				screenIndicators[22].setStatus(Indicator.status.OFF);
				screenIndicators[23].setStatus(Indicator.status.AMBER);
			}

			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{

				// GET DATA
				
				start = DateTime.Now;
				currentStage = screenStreams.GetData(DataType.control_currentStage);
				
				screenStreams.setStage(currentStage);
				
				meanAltitude = screenStreams.GetData(DataType.flight_meanAltitude);
				MET = screenStreams.GetData(DataType.vessel_MET);
				throttle = screenStreams.GetData(DataType.control_throttle);
				pitch = screenStreams.GetData(DataType.flight_pitch);
				roll = screenStreams.GetData(DataType.flight_roll);
				yaw = screenStreams.GetData(DataType.flight_heading);
				SAS = screenStreams.GetData(DataType.control_SAS);
				RCS = screenStreams.GetData(DataType.control_RCS);
				gear = screenStreams.GetData(DataType.control_gear);
				brakes = screenStreams.GetData(DataType.control_brakes);
				lights = screenStreams.GetData(DataType.control_lights);
				abort = screenStreams.GetData(DataType.control_abort);
				gForce = screenStreams.GetData(DataType.flight_gForce);
				maxElectric = screenStreams.GetData(DataType.resource_total_max_electricCharge);
				curElectric = screenStreams.GetData(DataType.resource_total_amount_electricCharge);
				maxMonopropellant = screenStreams.GetData(DataType.resource_total_max_monoPropellant);
				curMonopropellant = screenStreams.GetData(DataType.resource_total_amount_monoPropellant);
				orbitSpeed = screenStreams.GetData(DataType.orbit_speed);
				apoapsis = screenStreams.GetData(DataType.orbit_apoapsisAltitude);
				periapsis = screenStreams.GetData(DataType.orbit_periapsisAltitude);

				if (currentStage != oldStage) // WE HAVE STAGED, GET NEW STAGE_RESOURCE DATA
				{
					maxStageFuel = screenStreams.GetData(DataType.resource_stage_max_liquidFuel, true);
					curStageFuel = screenStreams.GetData(DataType.resource_stage_amount_liquidFuel, true);
					maxStageOx = screenStreams.GetData(DataType.resource_stage_max_oxidizer, true);
					curStageOx = screenStreams.GetData(DataType.resource_stage_amount_oxidizer, true);
				}
				else // ON SAME STAGE AS BEFORE, USE CURRENT STAGE_RESOURCE DATA (if it exists)
				{
					maxStageFuel = screenStreams.GetData(DataType.resource_stage_max_liquidFuel);
					curStageFuel = screenStreams.GetData(DataType.resource_stage_amount_liquidFuel);
					maxStageOx = screenStreams.GetData(DataType.resource_stage_max_oxidizer);
					curStageOx = screenStreams.GetData(DataType.resource_stage_amount_oxidizer);
				}
				maxTotFuel = screenStreams.GetData(DataType.resource_total_max_liquidFuel);
				curTotFuel = screenStreams.GetData(DataType.resource_total_amount_liquidFuel);
				maxTotOx = screenStreams.GetData(DataType.resource_total_max_oxidizer);
				curTotOx = screenStreams.GetData(DataType.resource_total_amount_oxidizer);
				
				sasMode = screenStreams.GetData(DataType.control_SASmode);
				actionGroup0 = screenStreams.GetData(DataType.control_actionGroup0);
				actionGroup1 = screenStreams.GetData(DataType.control_actionGroup1);
				actionGroup2 = screenStreams.GetData(DataType.control_actionGroup2);
				actionGroup3 = screenStreams.GetData(DataType.control_actionGroup3);
				actionGroup4 = screenStreams.GetData(DataType.control_actionGroup4);
				actionGroup5 = screenStreams.GetData(DataType.control_actionGroup5);
				actionGroup6 = screenStreams.GetData(DataType.control_actionGroup6);
				actionGroup7 = screenStreams.GetData(DataType.control_actionGroup7);
				actionGroup8 = screenStreams.GetData(DataType.control_actionGroup8);
				actionGroup9 = screenStreams.GetData(DataType.control_actionGroup9);

				surfaceRefsmmat = form.connection.SpaceCenter().ActiveVessel.SurfaceReferenceFrame;
				inertialRefsmmat = form.connection.SpaceCenter().ActiveVessel.Orbit.Body.NonRotatingReferenceFrame;
				vesselInerDirection = screenStreams.GetData(DataType.flight_inertial_direction);
				vesselInerRotation = screenStreams.GetData(DataType.flight_inertial_rotation);
				inerRoll = screenStreams.GetData(DataType.flight_inertial_roll);
				
				vesselSurfDirection = screenStreams.GetData(DataType.flight_direction);
				//vesselSurfRotation = screenStreams.GetData(DataType.flight_rotation);

				setInerRotFromQuaternion(vesselInerRotation);
				
				screenLabels[1].Text = "MET: " + Helper.timeString(MET, 3);

				// THROTTLE
				screenLabels[16].Text = Helper.prtlen(Math.Ceiling(throttle * 100).ToString() + "%", 4, Helper.Align.RIGHT);


				// ROTATION READOUT
				double cR = roll;
				double cP = pitch;
				double cY = yaw;

				double sR = this.setRotR;
				double sP = this.setRotP;
				double sY = this.setRotY;

				double? nR;
				double? nP;
				double? nY;


				// IF IN LOCK MODE SHOW LOCK ANGLES
				if (controlMode == 2)
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
				screenLabels[20].Text = "R: " + Helper.prtlen(Helper.toFixed(sR, 1), 6) + "  " + Helper.prtlen(Helper.toFixed(cR, 1), 6) + "  " + Helper.prtlen(Helper.toFixed(nR, 1), 6);
				screenLabels[21].Text = "P: " + Helper.prtlen(Helper.toFixed(sP, 1), 6) + "  " + Helper.prtlen(Helper.toFixed(cP, 1), 6) + "  " + Helper.prtlen(Helper.toFixed(nP, 1), 6);
				screenLabels[22].Text = "Y: " + Helper.prtlen(Helper.toFixed(sY, 1), 6) + "  " + Helper.prtlen(Helper.toFixed(cY, 1), 6) + "  " + Helper.prtlen(Helper.toFixed(nY, 1), 6);


				// STAGE LABEL
				String stageTxt = currentStage.ToString();

				screenLabels[25].Text = "CUR: " + Helper.prtlen(stageTxt, 2);


				// Status
				if (SAS) { screenIndicators[0].setStatus(Indicator.status.GREEN); } else { screenIndicators[0].setStatus(Indicator.status.OFF); } // SAS
				if (RCS) { screenIndicators[1].setStatus(Indicator.status.GREEN); } else { screenIndicators[1].setStatus(Indicator.status.OFF); } // RCS
				if (gear) { screenIndicators[2].setStatus(Indicator.status.GREEN); } else { screenIndicators[2].setStatus(Indicator.status.OFF); } // GEAR
				if (brakes) { screenIndicators[3].setStatus(Indicator.status.RED); } else { screenIndicators[3].setStatus(Indicator.status.OFF); } // Break
				if (lights) { screenIndicators[4].setStatus(Indicator.status.AMBER); } else { screenIndicators[4].setStatus(Indicator.status.OFF); } // Lights
				if (abort) { screenIndicators[5].setStatus(Indicator.status.RED); } else { screenIndicators[5].setStatus(Indicator.status.OFF); } // Abort


				// Check for autopilot
				switch (controlMode)
				{
					case 0:
						screenIndicators[25].setStatus(Indicator.status.OFF);
						screenIndicators[26].setStatus(Indicator.status.AMBER);
						screenIndicators[27].setStatus(Indicator.status.OFF);
						screenIndicators[28].setStatus(Indicator.status.OFF);
						screenIndicators[20].setStatus(Indicator.status.OFF);
						screenIndicators[21].setStatus(Indicator.status.OFF);
						break;
					case 1:
						screenIndicators[25].setStatus(Indicator.status.GREEN);
						screenIndicators[26].setStatus(Indicator.status.OFF);
						screenIndicators[27].setStatus(Indicator.status.OFF);
						screenIndicators[28].setStatus(Indicator.status.OFF);
						screenIndicators[20].setStatus(Indicator.status.OFF);
						screenIndicators[21].setStatus(Indicator.status.OFF);
						break;
					case 2:
						screenIndicators[25].setStatus(Indicator.status.OFF);
						screenIndicators[26].setStatus(Indicator.status.OFF);
						screenIndicators[27].setStatus(Indicator.status.RED);
						screenIndicators[28].setStatus(Indicator.status.OFF);
						screenIndicators[20].setStatus(Indicator.status.OFF);
						screenIndicators[21].setStatus(Indicator.status.OFF);
						break;
					case 3:
						screenIndicators[25].setStatus(Indicator.status.OFF);
						screenIndicators[26].setStatus(Indicator.status.OFF);
						screenIndicators[27].setStatus(Indicator.status.OFF);
						screenIndicators[28].setStatus(Indicator.status.AMBER);
						screenIndicators[20].setStatus(Indicator.status.GREEN);
						screenIndicators[21].setStatus(Indicator.status.OFF);
						break;
					case 4:
						screenIndicators[25].setStatus(Indicator.status.OFF);
						screenIndicators[26].setStatus(Indicator.status.OFF);
						screenIndicators[27].setStatus(Indicator.status.OFF);
						screenIndicators[28].setStatus(Indicator.status.AMBER);
						screenIndicators[20].setStatus(Indicator.status.OFF);
						screenIndicators[21].setStatus(Indicator.status.GREEN);
						break;
				}
				// PROGRAM LABEL
				screenLabels[30].Text = "RAT: " + Helper.toFixed(rollRate, 1) + "/s  RAT: " + Helper.toFixed(pitchRate, 1) + "/s";

				// SAS INDICATORS
				for(int i = 30; i < 40; i++)
				{
					screenIndicators[i].setStatus(0);
				}

				if (SAS)
				{
					switch (sasMode)
					{
						case SASMode.Prograde:
							screenIndicators[30].setStatus(Indicator.status.AMBER);
							break;
						case SASMode.Retrograde:
							screenIndicators[31].setStatus(Indicator.status.AMBER);
							break;
						case SASMode.StabilityAssist:
							screenIndicators[32].setStatus(Indicator.status.AMBER);
							break;
						case SASMode.Normal:
							screenIndicators[33].setStatus(Indicator.status.AMBER);
							break;
						case SASMode.AntiNormal:
							screenIndicators[34].setStatus(Indicator.status.AMBER);
							break;
						case SASMode.Radial:
							screenIndicators[36].setStatus(Indicator.status.AMBER);
							break;
						case SASMode.AntiRadial:
							screenIndicators[37].setStatus(Indicator.status.AMBER);
							break;
						case SASMode.Maneuver:
							screenIndicators[39].setStatus(Indicator.status.AMBER);
							break;
					}
				}

				if (SAS) { screenIndicators[35].setStatus(Indicator.status.AMBER); } else { screenIndicators[35].setStatus(Indicator.status.OFF); }
				if (RCS) { screenIndicators[38].setStatus(Indicator.status.AMBER); } else { screenIndicators[38].setStatus(Indicator.status.OFF); }


				// Action Group Indicators
				if (actionGroup1) { screenIndicators[40].setStatus(Indicator.status.AMBER); } else { screenIndicators[40].setStatus(Indicator.status.OFF); }
				if (actionGroup2) { screenIndicators[41].setStatus(Indicator.status.AMBER); } else { screenIndicators[41].setStatus(Indicator.status.OFF); }
				if (actionGroup3) { screenIndicators[42].setStatus(Indicator.status.AMBER); } else { screenIndicators[42].setStatus(Indicator.status.OFF); }
				if (actionGroup4) { screenIndicators[43].setStatus(Indicator.status.AMBER); } else { screenIndicators[43].setStatus(Indicator.status.OFF); }
				if (actionGroup5) { screenIndicators[44].setStatus(Indicator.status.AMBER); } else { screenIndicators[44].setStatus(Indicator.status.OFF); }
				if (actionGroup6) { screenIndicators[45].setStatus(Indicator.status.AMBER); } else { screenIndicators[45].setStatus(Indicator.status.OFF); }
				if (actionGroup7) { screenIndicators[46].setStatus(Indicator.status.AMBER); } else { screenIndicators[46].setStatus(Indicator.status.OFF); }
				if (actionGroup8) { screenIndicators[47].setStatus(Indicator.status.AMBER); } else { screenIndicators[47].setStatus(Indicator.status.OFF); }
				if (actionGroup9) { screenIndicators[48].setStatus(Indicator.status.AMBER); } else { screenIndicators[48].setStatus(Indicator.status.OFF); }
				if (actionGroup0) { screenIndicators[49].setStatus(Indicator.status.AMBER); } else { screenIndicators[49].setStatus(Indicator.status.OFF); }

				// Vertical Meters
				screenVMeters[0].setValue1(gForce);
				screenVMeters[1].setValue1((curStageFuel / maxStageFuel) * 100);
				screenVMeters[1].setValue2((curStageOx / maxStageOx) * 100);
				screenVMeters[2].setValue1((curTotFuel / maxTotFuel) * 100);
				screenVMeters[2].setValue2((curTotOx / maxTotOx) * 100);
				screenVMeters[3].setValue1((curMonopropellant / maxMonopropellant) * 100);
				screenVMeters[3].setValue2((curElectric / maxElectric) * 100);


				// FDAI
				if (FDAImode == FDAIMode.SURF)
				{
					screenIndicators[60].setStatus(Indicator.status.AMBER);
					screenIndicators[61].setStatus(Indicator.status.OFF);
				}
				else
				{
					screenIndicators[60].setStatus(Indicator.status.OFF);
					screenIndicators[61].setStatus(Indicator.status.AMBER);
				}
				
				screenFDAI.Invalidate();


				// Graphs
				//data = new List<Dictionary<int, Nullable<double>>>();
				//data.Add(chartData["geeTime"]);
				//form.showData(0, data, false);


				// SET CONTROL ANGLES FOR AUTOPILOTS
				if (controlMode == 1)
				{
					tRoll = this.setRotR - screenFDAI.offsetR;
					tPitch = this.setRotP - screenFDAI.offsetP;
					tYaw = this.setRotY - screenFDAI.offsetY;
				}
				else if (controlMode == 2)
				{
					tRoll = this.lockRotR - screenFDAI.offsetR;
					tPitch = this.lockRotP - screenFDAI.offsetP;
					tYaw = this.lockRotY - screenFDAI.offsetY;
				}

				// SET AUTOPILOT REFERENCE FRAME AND TARGET UNIT VECTOR
				double X = 0;
				double Y = 0;
				double Z = 0;

				X = Math.Cos(Helper.deg2rad(tYaw)) * Math.Cos(Helper.deg2rad(tPitch));
				Y = Math.Sin(Helper.deg2rad(tYaw)) * Math.Cos(Helper.deg2rad(tPitch));
				Z = Math.Sin(Helper.deg2rad(tPitch));
				
				Tuple<double, double, double> targetVector = new Tuple<double, double, double>(1,0,0);
				if (FDAImode == FDAIMode.INER)
				{
					form.form.spaceCenter.ActiveVessel.AutoPilot.ReferenceFrame = inertialRefsmmat;
					targetVector = new Tuple<double, double, double>(Z, X, Y);
					vector1 = Z;
					vector2 = X;
					vector3 = Y;
				}
				else
				{
					form.form.spaceCenter.ActiveVessel.AutoPilot.ReferenceFrame = surfaceRefsmmat;
					targetVector = new Tuple<double, double, double>(Z, X, Y);
					vector1 = Z;
					vector2 = X;
					vector3 = Y;
				}

				// SET TARGET FOR AUTOPILOT IF MODE IS AUTO
				if (controlMode == 1 || controlMode == 2)
				{
					form.form.spaceCenter.ActiveVessel.AutoPilot.TargetDirection = targetVector;

					if (tPitch == 90)
					{
						form.form.spaceCenter.ActiveVessel.AutoPilot.TargetRoll = 0f;
					}
					else if (tPitch > 90 || tPitch < -90)
					{
						form.form.spaceCenter.ActiveVessel.AutoPilot.TargetRoll = (float)tRoll + 180;
					}
					else
					{
						form.form.spaceCenter.ActiveVessel.AutoPilot.TargetRoll = (float)tRoll;
					}
					//form.spaceCenter.ActiveVessel.AutoPilot.TargetRoll = (float)tPitch;
					//form.spaceCenter.ActiveVessel.AutoPilot.TargetRoll = (float)tYaw;
				}

				if (oprError)
				{
					screenIndicators[52].setStatus(Indicator.status.WHITE);
				}
				else
				{
					screenIndicators[52].setStatus(Indicator.status.OFF);
				}

				if (keyRel)
				{
					screenIndicators[53].setStatus(Indicator.status.WHITE);
				}
				else
				{
					screenIndicators[53].setStatus(Indicator.status.OFF);
				}

				oldStage = currentStage;
				/**/
			}
		}

		private void loadPySSSMQData()
		{
			if (form.form.pySSSMQ.IsConnected())
			{
				// Pull data
				form.form.pySSSMQ.Pull("N20R1");

				// Wait for data to arrive
				screenIndicators[50].setStatus(Indicator.status.GREEN);
				int timeout = 0;
				while(form.dataStorage.getData("N20R1") == "")
				{
					if (timeout > 20) // 10 seconds
					{
						break;
					}
					Thread.Sleep(500);
					timeout++;
				}
				screenIndicators[50].setStatus(Indicator.status.OFF);

				// Load data
				screenFDAI.offsetR = int.Parse(form.dataStorage.getData("N20R1")) / 100f;
				screenFDAI.offsetP = int.Parse(form.dataStorage.getData("N20R2")) / 100f;
				screenFDAI.offsetY = int.Parse(form.dataStorage.getData("N20R3")) / 100f;
			}
		}

		private void pitchUp(object sender, EventArgs e)
		{
			this.setRotP += this.rotStep; if (setRotP > 360) setRotP = (setRotP - 360);
			form.dataStorage.setData("setRotP", this.setRotP.ToString());
		}
		
		private void pitchDown(object sender, EventArgs e)
		{
			this.setRotP -= this.rotStep; if (setRotP < -360) setRotP = (setRotP + 360);
			form.dataStorage.setData("setRotP", this.setRotP.ToString());
		}
		
		private void yawLeft(object sender, EventArgs e)
		{
			this.setRotY -= this.rotStep; if (setRotY < -180) setRotY += 360;
			form.dataStorage.setData("setRotY", this.setRotY.ToString());
		}
		
		private void yawRight(object sender, EventArgs e)
		{
			this.setRotY += this.rotStep; if (setRotY >= 180) setRotY -= 360;
			form.dataStorage.setData("setRotY", this.setRotY.ToString());
		}
		
		private void rollLeft(object sender, EventArgs e)
		{
			this.setRotR -= this.rotStep; if (setRotR < -180) setRotR += 360;
			form.dataStorage.setData("setRotR", this.setRotR.ToString());
		}
		
		private void rollRight(object sender, EventArgs e)
		{
			this.setRotR += this.rotStep; if (setRotR > 180) setRotR -= 360;
			form.dataStorage.setData("setRotR", this.setRotR.ToString());
		}

		private void setMode(object sender, EventArgs e, int mode)
		{
			switch (mode)
			{
				case 0: // FREE
					form.form.spaceCenter.ActiveVessel.AutoPilot.Disengage();
					this.controlMode = 0;
					break;
				case 1: // AUTOPILOT
					form.form.spaceCenter.ActiveVessel.AutoPilot.Engage();
					this.controlMode = 1;
					break;
				case 2: // LOCK
					lockRotR = setRotR;
					lockRotP = setRotP;
					lockRotY = setRotY;
					
					form.dataStorage.setData("lockRotR", this.lockRotR.ToString());
					form.dataStorage.setData("lockRotP", this.lockRotP.ToString());
					form.dataStorage.setData("lockRotY", this.lockRotY.ToString());

					form.form.spaceCenter.ActiveVessel.AutoPilot.Engage();
					this.controlMode = 2;
					break;
				case 3: // Roll Program
					form.form.spaceCenter.ActiveVessel.AutoPilot.Engage();
					this.controlMode = 3;
					break;
				case 4: // Pitch Program
					form.form.spaceCenter.ActiveVessel.AutoPilot.Engage();
					this.controlMode = 4;
					break;
			}
			form.dataStorage.setData("ctrlMode", this.controlMode.ToString());
		}
		
		private void setRotRate(object sender, EventArgs e, int rate)
		{
			rotStep = rate;
			form.dataStorage.setData("rotStep", this.rotStep.ToString());
		}

		private void stage(object sender, EventArgs e)
		{
			form.form.spaceCenter.ActiveVessel.Control.ActivateNextStage();
		}

		private void toggleActionGroup(object sender, EventArgs e, uint group)
		{
			// Get action group status
			bool active = form.form.spaceCenter.ActiveVessel.Control.GetActionGroup(group);

			if (active) { form.form.spaceCenter.ActiveVessel.Control.SetActionGroup(group, false); }
			else { form.form.spaceCenter.ActiveVessel.Control.SetActionGroup(group, true); }
		}

		private void rollRateMinus(object sender, EventArgs e)
		{
			rollRate -= 0.1;
			if(rollRate < 0) { rollRate = 0; }
			form.dataStorage.setData("rollRate", this.rollRate.ToString());
		}

		private void rollRatePlus(object sender, EventArgs e)
		{
			rollRate += 0.1;
			form.dataStorage.setData("rollRate", this.rollRate.ToString());
		}

		private void pitchRateMinus(object sender, EventArgs e)
		{
			pitchRate -= 0.1;
			if (pitchRate < 0) { pitchRate = 0; }
			form.dataStorage.setData("pitchRate", this.pitchRate.ToString());
		}

		private void pitchRatePlus(object sender, EventArgs e)
		{
			pitchRate += 0.1;
			form.dataStorage.setData("pitchRate", this.pitchRate.ToString());
		}

		private void rollProgramRun(object sender, EventArgs e)
		{
			controlMode = 3;
			Thread thread = new Thread(rollProgram);
			thread.IsBackground = true;
			thread.Start();
		}

		private void pitchProgramRun(object sender, EventArgs e)
		{
			controlMode = 4;
			Thread thread = new Thread(pitchProgram);
			thread.IsBackground = true;
			thread.Start();
		}

		private void changeThrottle(object sender, EventArgs e, double change)
		{
			double cThr = form.form.spaceCenter.ActiveVessel.Control.Throttle;
			double nThr = cThr + change;
			if(nThr > 100){ nThr = 100; }
			if (nThr < 0) { nThr = 0; }
			form.form.spaceCenter.ActiveVessel.Control.Throttle = (float)nThr;
		}

		private void setSAS(object sender, EventArgs e, int mode)
		{
			try
			{
				switch (mode)
				{
					case 0:
						form.form.spaceCenter.ActiveVessel.Control.SASMode = SASMode.StabilityAssist;
						break;
					case 1:
						form.form.spaceCenter.ActiveVessel.Control.SASMode = SASMode.Prograde;
						break;
					case 2:
						form.form.spaceCenter.ActiveVessel.Control.SASMode = SASMode.Retrograde;
						break;
					case 3:
						form.form.spaceCenter.ActiveVessel.Control.SASMode = SASMode.Normal;
						break;
					case 4:
						form.form.spaceCenter.ActiveVessel.Control.SASMode = SASMode.AntiNormal;
						break;
					case 5:
						form.form.spaceCenter.ActiveVessel.Control.SASMode = SASMode.Radial;
						break;
					case 6:
						form.form.spaceCenter.ActiveVessel.Control.SASMode = SASMode.AntiRadial;
						break;
					case 7:
						if (form.form.spaceCenter.ActiveVessel.Control.Nodes.Count > 0)
						{
							form.form.spaceCenter.ActiveVessel.Control.SASMode = SASMode.Maneuver;
						}
						break;
					case 10:
						if (form.form.spaceCenter.ActiveVessel.Control.RCS)
						{
							form.form.spaceCenter.ActiveVessel.Control.RCS = false;
						}
						else
						{
							form.form.spaceCenter.ActiveVessel.Control.RCS = true;
						}
						break;
					case 11:
						if (form.form.spaceCenter.ActiveVessel.Control.SAS)
						{
							form.form.spaceCenter.ActiveVessel.Control.SAS = false;
						}
						else
						{
							form.form.spaceCenter.ActiveVessel.Control.SAS = true;
							form.form.spaceCenter.ActiveVessel.AutoPilot.Disengage();
							controlMode = 0;
						}
						break;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.GetType() + ": " + ex.Message + "\n" + ex.StackTrace);
			}
		}

		private void setFDAIMode(object sender, EventArgs e, FDAIMode mode)
		{
			FDAImode = mode;
			form.dataStorage.setData("FDAImode", this.FDAImode.ToString());
		}

		private void rollProgram()
		{
			//Console.WriteLine("ROLL PROGRAM");
			double curRoll = Math.Round(roll) - screenFDAI.offsetR;
			double dstRoll = setRotR - screenFDAI.offsetR;
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

			form.form.spaceCenter.ActiveVessel.AutoPilot.Engage();
			Stopwatch loopTime = new Stopwatch();
			int sleepTime;

			while (curRoll != dstRoll && controlMode == 3)
			{
				//Console.WriteLine("ROLLING " + dstRoll.ToString() + ":" + curRoll.ToString() + ":" + rate.ToString());
				loopTime.Restart();
				curRoll = Math.Round(curRoll + (rate / 10), 2);

				form.form.spaceCenter.ActiveVessel.AutoPilot.TargetRoll = (float)curRoll;
				form.form.spaceCenter.ActiveVessel.AutoPilot.TargetPitch = (float)(setRotP - screenFDAI.offsetP);
				form.form.spaceCenter.ActiveVessel.AutoPilot.TargetHeading= (float)(setRotY - screenFDAI.offsetY);

				if (Math.Round(curRoll) == dstRoll)
				{
					curRoll = Math.Round(curRoll);
					form.form.spaceCenter.ActiveVessel.AutoPilot.TargetPitch = (float)curRoll;
					done = true;
				}

				loopTime.Stop();

				sleepTime = (int)(100 - (double)loopTime.ElapsedMilliseconds);
				if (sleepTime < 5) { sleepTime = 5; }
				Thread.Sleep(sleepTime);
			}

			if (done) { controlMode = 1; }
		}

		private void pitchProgram()
		{
			//Console.WriteLine("PITCH PROGRAM");
			double curPitch = Math.Round(pitch) - screenFDAI.offsetP;
			double dstPitch = setRotP - screenFDAI.offsetP;
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

			form.form.spaceCenter.ActiveVessel.AutoPilot.Engage();

			Stopwatch loopTime = new Stopwatch();
			int sleepTime;

			while (curPitch != dstPitch && controlMode == 4)
			{
				loopTime.Restart();

				//Console.WriteLine("ROLLING " + dstPitch.ToString() + ":" + curPitch.ToString() + ":" + rate.ToString());
				curPitch = Math.Round(curPitch + (rate / 10), 2);

				form.form.spaceCenter.ActiveVessel.AutoPilot.TargetPitch = (float)curPitch;
				form.form.spaceCenter.ActiveVessel.AutoPilot.TargetRoll = (float)(setRotR - screenFDAI.offsetR);
				form.form.spaceCenter.ActiveVessel.AutoPilot.TargetHeading = (float)(setRotY - screenFDAI.offsetY);

				if(Math.Round(curPitch) == dstPitch)
				{
					curPitch = Math.Round(curPitch);
					form.form.spaceCenter.ActiveVessel.AutoPilot.TargetPitch = (float)curPitch;
					done = true;
				}

				loopTime.Stop();

				sleepTime = (int)(100 - (double)loopTime.ElapsedMilliseconds);
				if(sleepTime < 5) { sleepTime = 5; }
				Thread.Sleep(sleepTime);
			}

			if (done) { controlMode = 1; }
		}

		private void logBlock() { logBlock(""); }
		private void logBlock(String name)
		{
			end = DateTime.Now;
			dur = end - start;
			String outName = "";
			if (name != "") outName = "(" + name + ")";
			Console.WriteLine("Block " + block++ + outName + ": " + (int)dur.TotalMilliseconds);
			start = DateTime.Now;
		}
		
		private void setInerRotFromQuaternion(Tuple<double, double, double, double> q)
		{
			// 1:X, 2:Y, 3:Z, 4:W
			double x = q.Item1;
			double y = q.Item2;
			double z = q.Item3;
			double w = q.Item4;

			double a = 0;
			double b = 0;
			double c = 0;
			double d = 0;
			double e = 0;

			String order = "XZY";//XYZ

			switch (order)
			{

				case "ZYX":
					a = 2 * (x * y + w * z);
					b = w * w + x * x - y * y - z * z;
					c = -2 * (x * z - w * y);
					d = 2 * (y * z + w * x);
					e = w * w - x * x - y * y + z * z;
					break;
				case "ZXY":
					a = -2 * (x * y - w * z);
					b = w * w - x * x + y * y - z * z;
					c = 2 * (y * z + w * x);
					d = -2 * (x * z - w * y);
					e = w * w - x * x - y * y + z * z;
					break;
				case "XYZ":
					a = -2 * (y * z - w * x);
					b = w * w - x * x - y * y + z * z;
					c = 2 * (x * z + w * y);
					d = -2 * (x * y - w * z);
					e = w * w + x * x - y * y - z * z;
					break;
				case "XZY":
					a = 2 * (y * z + w * x);
					b = w * w - x * x + y * y - z * z;
					c = -2 * (x * y - w * z);
					d = 2 * (x * z + w * y);
					e = w * w + x * x - y * y - z * z;
					break;
				case "YXZ":
					a = 2 * (x * z + w * y);
					b = w * w - x * x - y * y + z * z;
					c = -2 * (y * z - w * x);
					d = 2 * (x * y + w * z);
					e = w * w - x * x + y * y - z * z;
					break;
			}
			
			inerRoll = (float)Helper.rad2deg(Math.Atan2(d, e)); // Z
			inerPitch = (float)Helper.rad2deg(Math.Asin(c)); // Y
			inerYaw = (float)Helper.rad2deg(Math.Atan2(a, b)); // X

			/*
			yaw = Helper.rad2deg(Math.Atan2(d, e)); // X
			pitch = Helper.rad2deg(Math.Asin(c)); // Y
			roll = Helper.rad2deg(Math.Atan2(a, b)); // Z
			/*
			threeaxisrot( 2*(q.x*q.y + q.w*q.z),
                     q.w*q.w + q.x*q.x - q.y*q.y - q.z*q.z,
                    -2*(q.x*q.z - q.w*q.y),
                     2*(q.y*q.z + q.w*q.x),
                     q.w*q.w - q.x*q.x - q.y*q.y + q.z*q.z,
                     res);
			res[0] = atan2( r31, r32 );
  			res[1] = asin ( r21 );
  			res[2] = atan2( r11, r12 );
			*/
		}
		
	}
}