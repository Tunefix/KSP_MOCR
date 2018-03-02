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
	public class SingleOrbit : MocrScreen
	{
		ReferenceFrame surfaceRefsmmat;
		ReferenceFrame inertialRefsmmat;
		Tuple<double, double, double> vesselSurfDirection;
		Tuple<double, double, double> vesselInerDirection;
		Tuple<double, double, double, double> vesselInerRotation;
		
		float inerRoll = 0;
		float inerPitch = 0;
		float inerYaw = 0;
		float surfRoll = 0;
		float surfPitch = 0;
		float surfYaw = 0;


		public SingleOrbit(Screen form)
		{
			this.form = form;
			this.chartData = form.form.chartData;
			
			this.screenStreams = form.streamCollection;

			this.width = 120;
			this.height = 30;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 120; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 12; i++) screenIndicators.Add(null); // Initialize Indicators
			for (int i = 0; i < 2; i++) screenInputs.Add(null); // Initialize Inputs

			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateLabel(0, 1, 14, 1, "MET: ");
			screenLabels[1] = Helper.CreateLabel(16, 1, 14, 1, "LT: ");
			screenLabels[2] = Helper.CreateLabel(0, 2, 120, 1, "─────────── ORBITAL ELEMENTS ───────────────────────────────────────────────┬───────────────────────────────────");
			screenLabels[3] = Helper.CreateLabel(39, 0, 42, 1, "============== ORBIT MODULE =============="); // Screen Title
			
			// ORBITAL ELEMENTS
			screenLabels[10] = Helper.CreateLabel(1, 4, 39, 1, "         Eccentricity: ");
			screenLabels[11] = Helper.CreateLabel(1, 5, 39, 1, "       Semimajor Axis: ");
			screenLabels[12] = Helper.CreateLabel(1, 6, 39, 1, "          Inclination: ");
			screenLabels[13] = Helper.CreateLabel(1, 7, 39, 1, "    Longitude of Node: ");
			screenLabels[14] = Helper.CreateLabel(1, 8, 39, 1, "Argument of Periapsis: ");
			screenLabels[15] = Helper.CreateLabel(1, 9, 39, 1, "         True Anomaly: ");
			screenLabels[16] = Helper.CreateLabel(42, 4, 32, 1, "            Period: HH:MM:SS.sss");
			screenLabels[17] = Helper.CreateLabel(42, 5, 32, 1, "Time to SOI Change: HH:MM:SS.sss");
			screenLabels[18] = Helper.CreateLabel(42, 6, 32, 1, "     Orbital Speed:    XXXXX.XX");
			
			
			// FLIGHT ELEMENTS
			screenLabels[20] = Helper.CreateLabel(1, 11, 41, 1, "─────────── FLIGHT ELEMENTS ────────────┤");
			screenLabels[21] = Helper.CreateLabel(1, 13, 40, 1, "             Altitude: ");
			screenLabels[22] = Helper.CreateLabel(1, 14, 40, 1, "             Apoapsis: ");
			screenLabels[23] = Helper.CreateLabel(1, 15, 40, 1, "            Periapsis: ");
			screenLabels[24] = Helper.CreateLabel(1, 16, 40, 1, "     Time to Apoapsis: ");
			screenLabels[25] = Helper.CreateLabel(1, 17, 40, 1, "    Time to Periapsis: ");
			
			screenLabels[29] = Helper.CreateLabel(1, 19, 40, 1, "──── DIRECTION VECTORS ───────────────────────────────");
			
			// VECTORS
			screenLabels[30] = Helper.CreateLabel(78, 8, 40, 1, "──────── STATE VECTORS ────────");
			//screenLabels[31] = Helper.CreateLabel(78, 9, 1, 1, "│");
			screenLabels[32] = Helper.CreateLabel(79, 10, 14, 1, "Rx: +XXXX.XXX");
			screenLabels[33] = Helper.CreateLabel(79, 11, 14, 1, "Ry: +XXXX.XXX");
			screenLabels[34] = Helper.CreateLabel(79, 12, 14, 1, "Rz: +XXXX.XXX");
			//screenLabels[35] = Helper.CreateLabel(78, 13, 1, 1, "│");
			
			// SPEED
			screenLabels[41] = Helper.CreateLabel(94, 10, 14, 1, "Vx: +XXXX.XXX");
			screenLabels[42] = Helper.CreateLabel(94, 11, 14, 1, "Vy: +XXXX.XXX");
			screenLabels[43] = Helper.CreateLabel(94, 12, 14, 1, "Vz: -XXXX.XXX");
			
			// ROTATION
			screenLabels[49] = Helper.CreateLabel(41, 9, 37, 1, "│                                   │");
			screenLabels[50] = Helper.CreateLabel(41, 8, 37, 1, "┌──────────── ROTATION ────────────┴┬");
			screenLabels[51] = Helper.CreateLabel(41, 10, 37, 1, "│            ROLL   PITCH    YAW    │");
			screenLabels[52] = Helper.CreateLabel(41, 11, 37, 1, "│ INER ATT: XXX.XX  XXX.XX  XXX.XX  │");
			screenLabels[53] = Helper.CreateLabel(41, 12, 37, 1, "│    BURN : XXX.XX  XXX.XX  XXX.XX  │");
			screenLabels[54] = Helper.CreateLabel(41, 13, 37, 1, "│    REF  : XXX.XX  XXX.XX  XXX.XX  │");
			screenLabels[55] = Helper.CreateLabel(41, 14, 37, 1, "│    FDAI : XXX.XX  XXX.XX  XXX.XX  │");
			screenLabels[56] = Helper.CreateLabel(41, 15, 37, 1, "│          ──────────────────────── │");
			screenLabels[57] = Helper.CreateLabel(41, 16, 37, 1, "│ SURF ATT: XXX.XX  XXX.XX  XXX.XX  │");
			screenLabels[58] = Helper.CreateLabel(41, 17, 37, 1, "│    REF  : XXX.XX  XXX.XX  XXX.XX  │");
			screenLabels[59] = Helper.CreateLabel(41, 18, 37, 1, "│    FDAI : XXX.XX  XXX.XX  XXX.XX  │");
			screenLabels[60] = Helper.CreateLabel(41, 19, 37, 1, "┴───────────────────────────────────┤");

			// DIRECTION VECTORS
			screenLabels[70] = Helper.CreateLabel(1, 20, 80, 1, "            ┌─────────── INER ────────────┐┌─────────── SURF ────────────┐");
			screenLabels[72] = Helper.CreateLabel(1, 22, 11, 1, "  PROGRADE:");
			screenLabels[73] = Helper.CreateLabel(1, 23, 11, 1, "RETROGRADE:");
			screenLabels[74] = Helper.CreateLabel(1, 24, 11, 1, "    NORMAL:");
			screenLabels[75] = Helper.CreateLabel(1, 25, 11, 1, "ANTINORMAL:");
			screenLabels[76] = Helper.CreateLabel(1, 26, 11, 1, " RADIAL IN:");
			screenLabels[77] = Helper.CreateLabel(1, 27, 11, 1, "RADIAL OUT:");
			screenLabels[78] = Helper.CreateLabel(1, 28, 11, 1, "    TARGET:");
			
			screenLabels[82] = Helper.CreateLabel(14, 22, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[83] = Helper.CreateLabel(14, 23, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[84] = Helper.CreateLabel(14, 24, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[85] = Helper.CreateLabel(14, 25, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[86] = Helper.CreateLabel(14, 26, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[87] = Helper.CreateLabel(14, 27, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[88] = Helper.CreateLabel(14, 28, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			
			screenLabels[92] = Helper.CreateLabel(46, 22, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[93] = Helper.CreateLabel(46, 23, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[94] = Helper.CreateLabel(46, 24, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[95] = Helper.CreateLabel(46, 25, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[96] = Helper.CreateLabel(46, 26, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[97] = Helper.CreateLabel(46, 27, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[98] = Helper.CreateLabel(46, 28, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");

			// LINES
			screenLabels[100] = Helper.CreateLabel(76, 3, 1, 1, "│");
			screenLabels[101] = Helper.CreateLabel(76, 4, 1, 1, "│");
			screenLabels[102] = Helper.CreateLabel(76, 5, 1, 1, "│");
			screenLabels[103] = Helper.CreateLabel(76, 6, 1, 1, "│");
			screenLabels[104] = Helper.CreateLabel(76, 7, 1, 1, "│");

			screenLabels[105] = Helper.CreateLabel(40, 8, 1, 1, "└");
			screenLabels[106] = Helper.CreateLabel(40, 7, 1, 1, "╵");
			screenLabels[107] = Helper.CreateLabel(40, 6, 1, 1, "╵");
			screenLabels[108] = Helper.CreateLabel(40, 5, 1, 1, "╵");
			screenLabels[109] = Helper.CreateLabel(40, 4, 1, 1, "╵");
			screenLabels[110] = Helper.CreateLabel(40, 3, 1, 1, "╵");
		}
		
		public override void updateLocalElements(object sender, EventArgs e)
		{
			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{

				// GET DATA

				screenLabels[0].Text = "MET: " + Helper.timeString(screenStreams.GetData(DataType.vessel_MET), 3);
				screenLabels[1].Text = "LT: " + Helper.timeString(DateTime.Now.TimeOfDay.TotalSeconds, 2);

				surfaceRefsmmat = form.connection.SpaceCenter().ActiveVessel.SurfaceReferenceFrame;
				inertialRefsmmat = form.connection.SpaceCenter().ActiveVessel.Orbit.Body.NonRotatingReferenceFrame;
				vesselInerDirection = screenStreams.GetData(DataType.flight_inertial_direction);
				vesselInerRotation = screenStreams.GetData(DataType.flight_inertial_rotation);
				inerRoll = screenStreams.GetData(DataType.flight_inertial_roll);
				inerPitch = screenStreams.GetData(DataType.flight_inertial_pitch);
				inerYaw = screenStreams.GetData(DataType.flight_inertial_yaw);

				surfRoll = screenStreams.GetData(DataType.flight_roll);
				surfPitch = screenStreams.GetData(DataType.flight_pitch);
				surfYaw = screenStreams.GetData(DataType.flight_heading);

				vesselSurfDirection = screenStreams.GetData(DataType.flight_direction);
				

				// ROTATION

				string iR = Helper.prtlen(Helper.toFixed(inerRoll, 2), 7, Helper.Align.RIGHT);
				string iP = Helper.prtlen(Helper.toFixed(inerPitch, 2), 7, Helper.Align.RIGHT);
				string iY = Helper.prtlen(Helper.toFixed(inerYaw, 2), 7, Helper.Align.RIGHT);
				string sR = Helper.prtlen(Helper.toFixed(surfRoll, 2), 7, Helper.Align.RIGHT);
				string sP = Helper.prtlen(Helper.toFixed(surfPitch, 2), 7, Helper.Align.RIGHT);
				string sY = Helper.prtlen(Helper.toFixed(surfYaw, 2), 7, Helper.Align.RIGHT);

				screenLabels[52].Text = "│ INER ATT:" + iR + " " + iP + " " + iY + "  │";
				screenLabels[53].Text = "│    BURN : XXX.XX  XXX.XX  XXX.XX  │";
				screenLabels[54].Text = "│    REF  : XXX.XX  XXX.XX  XXX.XX  │";
				screenLabels[55].Text = "│    FDAI : XXX.XX  XXX.XX  XXX.XX  │";
				screenLabels[57].Text = "│ SURF ATT:" + sR + " " + sP + " " + sY + "  │";
				screenLabels[58].Text = "│    REF  : XXX.XX  XXX.XX  XXX.XX  │";
				screenLabels[59].Text = "│    FDAI : XXX.XX  XXX.XX  XXX.XX  │";
			}
		}

		public override void resize() { }
	}
}
