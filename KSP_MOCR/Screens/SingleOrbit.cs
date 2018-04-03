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

		// STATE VECTORS
		Tuple<double, double, double> positionVector;
		Tuple<double, double, double> velocityVector;

		// DIRECTION VECTORS
		Tuple<double, double, double> inerPrograde;
		Tuple<double, double, double> inerRetrograde;
		Tuple<double, double, double> inerNormal;
		Tuple<double, double, double> inerAntiNormal;
		Tuple<double, double, double> inerRadial;
		Tuple<double, double, double> inerAntiRadial;
		Tuple<double, double, double> surfPrograde;
		Tuple<double, double, double> surfRetrograde;
		Tuple<double, double, double> surfNormal;
		Tuple<double, double, double> surfAntiNormal;
		Tuple<double, double, double> surfRadial;
		Tuple<double, double, double> surfAntiRadial;

		// ROTATION
		double rR, rP, rY;

		double iR, iP, iY;
		double bR, bP, bY;
		double ifR, ifP, ifY;

		double sR, sP, sY;
		double sfR, sfP, sfY;

		// ORBITAL ELEMENTS
		double eccentricity;
		double semiMajorAxis;
		double Inclination;
		double LongitudeOfNode;
		double ArgumentOfPeriapsis;
		double TrueAnomaly;
		double OrbitPeriod;
		double SOIChange;
		double OrbitalSpeed;

		// FLIGHT ELEMENTS
		double altitude;
		double apoapsis;
		double periapsis;
		double timeToPeriapsis;
		double timeToApoapsis;


		public SingleOrbit(Screen form)
		{
			this.form = form;
			this.chartData = form.form.chartData;

			screenStreams = form.streamCollection;
			dataStorage = form.dataStorage;

			this.charSize = false;
			this.width = 656;
			this.height = 494;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 120; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 12; i++) screenIndicators.Add(null); // Initialize Indicators
			for (int i = 0; i < 2; i++) screenInputs.Add(null); // Initialize Inputs

			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateCRTLabel(0, 0, 5, 1, "SCR 5");
			screenLabels[1] = Helper.CreateCRTLabel(27, 0, 42, 1, "ORBIT DATA", 4); // Screen Title
			screenLabels[2] = Helper.CreateCRTLabel(0, 1.5, 12, 1, "LT: XX:XX:XX");
			screenLabels[3] = Helper.CreateCRTLabel(29, 1.5, 14, 1, "MET: XXX:XX:XX");

			screenLabels[4] = Helper.CreateCRTLabel(0, 3, 120, 1, "───────── ORBITAL ELEMENTS ──────────┬────── STATE VECTORS (INER) ──────");
			screenLabels[100] = Helper.CreateCRTLabel(37, 4, 1, 1, "│");
			screenLabels[101] = Helper.CreateCRTLabel(37, 5, 1, 1, "│");
			screenLabels[102] = Helper.CreateCRTLabel(37, 6, 1, 1, "│");
			screenLabels[103] = Helper.CreateCRTLabel(37, 7, 1, 1, "│");
			screenLabels[104] = Helper.CreateCRTLabel(37, 8, 1, 1, "│");

			// ORBITAL ELEMENTS
			screenLabels[10] = Helper.CreateCRTLabel(1, 5, 36, 1, "     ECCENTRICITY: ");
			screenLabels[11] = Helper.CreateCRTLabel(1, 6, 36, 1, "   SEMIMAJOR AXIS: ");
			screenLabels[12] = Helper.CreateCRTLabel(1, 7, 36, 1, "      INCLINATION: ");
			screenLabels[13] = Helper.CreateCRTLabel(1, 8, 36, 1, "LONGITUDE OF NODE: ");
			screenLabels[14] = Helper.CreateCRTLabel(1, 9, 36, 1, "ARG. OF PERIAPSIS: ");
			screenLabels[15] = Helper.CreateCRTLabel(1, 10, 36, 1, "     TRUE ANOMALY: ");
			screenLabels[16] = Helper.CreateCRTLabel(1, 12, 36, 1, "       ORB PERIOD: HH:MM:SS.sss");
			screenLabels[17] = Helper.CreateCRTLabel(1, 13, 36, 1, "       SOI CHANGE: HH:MM:SS.sss");
			screenLabels[18] = Helper.CreateCRTLabel(1, 14, 36, 1, "        ORB SPEED:    XXXXX.XX");


			// FLIGHT ELEMENTS
			screenLabels[20] = Helper.CreateCRTLabel(0, 16, 37, 1, "────────── FLIGHT ELEMENTS ──────────");
			screenLabels[21] = Helper.CreateCRTLabel(1, 18, 36, 1, "         ALTITUDE: ");
			screenLabels[22] = Helper.CreateCRTLabel(1, 19, 36, 1, "         APOAPSIS: ");
			screenLabels[23] = Helper.CreateCRTLabel(1, 20, 36, 1, "        PERIAPSIS: ");
			screenLabels[24] = Helper.CreateCRTLabel(1, 21, 36, 1, " TIME TO APOAPSIS: ");
			screenLabels[25] = Helper.CreateCRTLabel(1, 22, 36, 1, "TIME TO PERIAPSIS: ");

			screenLabels[29] = Helper.CreateCRTLabel(0, 24, 72, 1, "──── DIRECTION VECTORS ─────────────────────────────────────────────────");

			// STATE VECTORS
			screenLabels[32] = Helper.CreateCRTLabel(40, 5, 14, 1, "Rx: +XXXX.XXX");
			screenLabels[33] = Helper.CreateCRTLabel(40, 6, 14, 1, "Ry: +XXXX.XXX");
			screenLabels[34] = Helper.CreateCRTLabel(40, 7, 14, 1, "Rz: +XXXX.XXX");

			// SPEED VECTORS
			screenLabels[41] = Helper.CreateCRTLabel(55, 5, 14, 1, "Vx: +XXXX.XXX");
			screenLabels[42] = Helper.CreateCRTLabel(55, 6, 14, 1, "Vy: +XXXX.XXX");
			screenLabels[43] = Helper.CreateCRTLabel(55, 7, 14, 1, "Vz: -XXXX.XXX");

			// ROTATION
			screenLabels[49] = Helper.CreateCRTLabel(37, 10, 37, 1, "│                                 ");
			screenLabels[50] = Helper.CreateCRTLabel(37, 9, 37, 1, "├──────────── ROTATION ────────────");
			screenLabels[51] = Helper.CreateCRTLabel(37, 11, 37, 1, "│            ROLL   PITCH    YAW  ");
			screenLabels[52] = Helper.CreateCRTLabel(37, 12, 37, 1, "│ INER ATT: XXX.XX  XXX.XX  XXX.XX");
			screenLabels[53] = Helper.CreateCRTLabel(37, 13, 37, 1, "│    BURN : XXX.XX  XXX.XX  XXX.XX");
			screenLabels[54] = Helper.CreateCRTLabel(37, 14, 37, 1, "│    REF  : XXX.XX  XXX.XX  XXX.XX");
			screenLabels[55] = Helper.CreateCRTLabel(37, 15, 37, 1, "│    FDAI : XXX.XX  XXX.XX  XXX.XX");
			screenLabels[56] = Helper.CreateCRTLabel(37, 16, 37, 1, "┤          ───────────────────────");
			screenLabels[57] = Helper.CreateCRTLabel(37, 17, 37, 1, "│ SURF ATT: XXX.XX  XXX.XX  XXX.XX");
			screenLabels[58] = Helper.CreateCRTLabel(37, 18, 37, 1, "│    REF  : XXX.XX  XXX.XX  XXX.XX");
			screenLabels[59] = Helper.CreateCRTLabel(37, 19, 37, 1, "│    FDAI : XXX.XX  XXX.XX  XXX.XX");
			screenLabels[60] = Helper.CreateCRTLabel(37, 20, 37, 1, "│                                 ");
			screenLabels[61] = Helper.CreateCRTLabel(37, 21, 37, 1, "├──────────────────────────────────");

			// DIRECTION VECTORS
			screenLabels[70] = Helper.CreateCRTLabel(1, 25, 71, 1, "           ┌─────────── INER ───────────┐┌─────────── SURF ───────────┐");
			screenLabels[71] = Helper.CreateCRTLabel(1, 26, 71, 1, "               ROLL     PITCH     YAW        ROLL     PITCH     YAW");
			screenLabels[72] = Helper.CreateCRTLabel(1, 27, 11, 1, "  PROGRADE:");
			screenLabels[73] = Helper.CreateCRTLabel(1, 28, 11, 1, "RETROGRADE:");
			screenLabels[74] = Helper.CreateCRTLabel(1, 29, 11, 1, "    NORMAL:");
			screenLabels[75] = Helper.CreateCRTLabel(1, 30, 11, 1, "ANTINORMAL:");
			screenLabels[76] = Helper.CreateCRTLabel(1, 31, 11, 1, " RADIAL IN:");
			screenLabels[77] = Helper.CreateCRTLabel(1, 32, 11, 1, "RADIAL OUT:");
			//screenLabels[78] = Helper.CreateCRTLabel(1, 33, 11, 1, "    TARGET:");

			screenLabels[82] = Helper.CreateCRTLabel(13, 27, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[83] = Helper.CreateCRTLabel(13, 28, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[84] = Helper.CreateCRTLabel(13, 29, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[85] = Helper.CreateCRTLabel(13, 30, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[86] = Helper.CreateCRTLabel(13, 31, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[87] = Helper.CreateCRTLabel(13, 32, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			//screenLabels[88] = Helper.CreateCRTLabel(13, 33, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");

			screenLabels[92] = Helper.CreateCRTLabel(43, 27, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[93] = Helper.CreateCRTLabel(43, 28, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[94] = Helper.CreateCRTLabel(43, 29, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[95] = Helper.CreateCRTLabel(43, 30, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[96] = Helper.CreateCRTLabel(43, 31, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			screenLabels[97] = Helper.CreateCRTLabel(43, 32, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");
			//screenLabels[98] = Helper.CreateCRTLabel(43, 33, 28, 1, "XXXX.XXX  XXXX.XXX  XXXX.XXX");

		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			screenLabels[2].Text = "LT: " + Helper.timeString(DateTime.Now.TimeOfDay.TotalSeconds, 2);

			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{

				// GET DATA

				screenLabels[3].Text = "MET: " + Helper.timeString(screenStreams.GetData(DataType.vessel_MET), 3);
				

				//surfaceRefsmmat = form.connection.SpaceCenter().ActiveVessel.SurfaceReferenceFrame;
				//inertialRefsmmat = form.connection.SpaceCenter().ActiveVessel.Orbit.Body.NonRotatingReferenceFrame;

				positionVector = screenStreams.GetData(DataType.vessel_position);
				velocityVector = screenStreams.GetData(DataType.vessel_velocity);
				vesselSurfDirection = screenStreams.GetData(DataType.flight_direction);

				vesselInerDirection = screenStreams.GetData(DataType.flight_inertial_direction);
				vesselInerRotation = screenStreams.GetData(DataType.flight_inertial_rotation);

				// ROTATION
				rR = 0;
				rP = 0;
				rY = 0;

				iR = screenStreams.GetData(DataType.flight_inertial_roll);
				iP = screenStreams.GetData(DataType.flight_inertial_pitch);
				iY = screenStreams.GetData(DataType.flight_inertial_yaw);
				bR = 0;
				bP = 0;
				bY = 0;

				sR = screenStreams.GetData(DataType.flight_roll);
				sP = screenStreams.GetData(DataType.flight_pitch);
				sY = screenStreams.GetData(DataType.flight_heading);

				double.TryParse(dataStorage.getData("N20R1"), out rR);
				double.TryParse(dataStorage.getData("N20R2"), out rP);
				double.TryParse(dataStorage.getData("N20R3"), out rY);

				rR = rR / 100f;
				rP = rP / 100f;
				rY = rY / 100f;

				// DIRECTION VECTORS
				inerPrograde = screenStreams.GetData(DataType.flight_inertial_prograde);
				inerRetrograde = screenStreams.GetData(DataType.flight_inertial_retrograde);
				inerNormal = screenStreams.GetData(DataType.flight_inertial_normal);
				inerAntiNormal = screenStreams.GetData(DataType.flight_inertial_antiNormal);
				inerRadial = screenStreams.GetData(DataType.flight_inertial_radial);
				inerAntiRadial = screenStreams.GetData(DataType.flight_inertial_antiRadial);

				surfPrograde = screenStreams.GetData(DataType.flight_prograde);
				surfRetrograde = screenStreams.GetData(DataType.flight_retrograde);
				surfNormal = screenStreams.GetData(DataType.flight_normal);
				surfAntiNormal = screenStreams.GetData(DataType.flight_antiNormal);
				surfRadial = screenStreams.GetData(DataType.flight_radial);
				surfAntiRadial = screenStreams.GetData(DataType.flight_antiRadial);



				// ORBIT ELEMENTS
				eccentricity = screenStreams.GetData(DataType.orbit_eccentricity);
				semiMajorAxis = screenStreams.GetData(DataType.orbit_semiMajorAxis);
				Inclination = screenStreams.GetData(DataType.orbit_inclination);
				LongitudeOfNode = screenStreams.GetData(DataType.orbit_longitudeOfAscendingNode);
				ArgumentOfPeriapsis = screenStreams.GetData(DataType.orbit_argumentOfPeriapsis);
				TrueAnomaly = screenStreams.GetData(DataType.orbit_trueAnomaly);
				OrbitPeriod = screenStreams.GetData(DataType.orbit_period);
				SOIChange = screenStreams.GetData(DataType.orbit_timeToSOIChange);
				OrbitalSpeed = screenStreams.GetData(DataType.orbit_speed);

				// FLIGHT ELEMENTS
				altitude = screenStreams.GetData(DataType.flight_meanAltitude);
				apoapsis = screenStreams.GetData(DataType.orbit_apoapsisAltitude);
				periapsis = screenStreams.GetData(DataType.orbit_periapsisAltitude);
				timeToPeriapsis = screenStreams.GetData(DataType.orbit_timeToPeriapsis);
				timeToApoapsis = screenStreams.GetData(DataType.orbit_timeToApoapsis);

				// STATE VECTORS
				screenLabels[32].Text = "Rx: " + Helper.prtlen(Helper.toFixed(positionVector.Item1 / 1000d, 3, true), 9, Helper.Align.RIGHT);
				screenLabels[33].Text = "Ry: " + Helper.prtlen(Helper.toFixed(positionVector.Item2 / 1000d, 3, true), 9, Helper.Align.RIGHT);
				screenLabels[34].Text = "Rz: " + Helper.prtlen(Helper.toFixed(positionVector.Item3 / 1000d, 3, true), 9, Helper.Align.RIGHT);

				// SPEED VECTORS
				screenLabels[41].Text = "Vx: " + Helper.prtlen(Helper.toFixed(velocityVector.Item1, 3, true), 9, Helper.Align.RIGHT);
				screenLabels[42].Text = "Vy: " + Helper.prtlen(Helper.toFixed(velocityVector.Item2, 3, true), 9, Helper.Align.RIGHT);
				screenLabels[43].Text = "Vz: " + Helper.prtlen(Helper.toFixed(velocityVector.Item3, 3, true), 9, Helper.Align.RIGHT);


				// ROTATION

				string iRs = Helper.prtlen(Helper.toFixed(iR, 2), 7, Helper.Align.RIGHT);
				string iPs = Helper.prtlen(Helper.toFixed(iP, 2), 7, Helper.Align.RIGHT);
				string iYs = Helper.prtlen(Helper.toFixed(iY, 2), 7, Helper.Align.RIGHT);
				string bRs = Helper.prtlen(Helper.toFixed(bR, 2), 7, Helper.Align.RIGHT);
				string bPs = Helper.prtlen(Helper.toFixed(bP, 2), 7, Helper.Align.RIGHT);
				string bYs = Helper.prtlen(Helper.toFixed(bY, 2), 7, Helper.Align.RIGHT);
				string sRs = Helper.prtlen(Helper.toFixed(sR, 2), 7, Helper.Align.RIGHT);
				string sPs = Helper.prtlen(Helper.toFixed(sP, 2), 7, Helper.Align.RIGHT);
				string sYs = Helper.prtlen(Helper.toFixed(sY, 2), 7, Helper.Align.RIGHT);
				string rRs = Helper.prtlen(Helper.toFixed(rR, 2), 7, Helper.Align.RIGHT);
				string rPs = Helper.prtlen(Helper.toFixed(rP, 2), 7, Helper.Align.RIGHT);
				string rYs = Helper.prtlen(Helper.toFixed(rY, 2), 7, Helper.Align.RIGHT);

				ifR = normalize(iR + rR, -180, 180);
				ifP = normalize(iP + rP, -90, 90);
				ifY = normalize(iY + rY, 0, 360);

				sfR = normalize(sR + rR, -180, 180);
				sfP = normalize(sP + rP, -90, 90);
				sfY = normalize(sY + rY, 0, 360);

				string ifRs = Helper.prtlen(Helper.toFixed(ifR, 2), 7, Helper.Align.RIGHT);
				string ifPs = Helper.prtlen(Helper.toFixed(ifP, 2), 7, Helper.Align.RIGHT);
				string ifYs = Helper.prtlen(Helper.toFixed(ifY, 2), 7, Helper.Align.RIGHT);

				string sfRs = Helper.prtlen(Helper.toFixed(sfR, 2), 7, Helper.Align.RIGHT);
				string sfPs = Helper.prtlen(Helper.toFixed(sfP, 2), 7, Helper.Align.RIGHT);
				string sfYs = Helper.prtlen(Helper.toFixed(sfY, 2), 7, Helper.Align.RIGHT);

				screenLabels[52].Text = "│ INER ATT:" + iRs + " " + iPs + " " + iYs + "  │";
				screenLabels[53].Text = "│    BURN :" + bRs + " " + bPs + " " + bYs + "  │";
				screenLabels[54].Text = "│    REF  :" + rRs + " " + rPs + " " + rYs + "  │";
				screenLabels[55].Text = "│    FDAI :" + ifRs + " " + ifPs + " " + ifYs + "  │";
				screenLabels[57].Text = "│ SURF ATT:" + sRs + " " + sPs + " " + sYs + "  │";
				screenLabels[58].Text = "│    REF  :" + rRs + " " + rPs + " " + rYs + "  │";
				screenLabels[59].Text = "│    FDAI :" + sfRs + " " + sfPs + " " + sfYs + "  │";


				// ORBITAL ELEMENTS
				screenLabels[10].Text = "     ECCENTRICITY: " + Helper.prtlen(Helper.toFixed(eccentricity, 5), 11, Helper.Align.RIGHT);
				screenLabels[11].Text = "   SEMIMAJOR AXIS: " + Helper.prtlen(Helper.toFixed(semiMajorAxis, 3), 11, Helper.Align.RIGHT);
				screenLabels[12].Text = "      INCLINATION: " + Helper.prtlen(Helper.toFixed(Helper.rad2deg(Inclination), 3), 11, Helper.Align.RIGHT);
				screenLabels[13].Text = "LONGITUDE OF NODE: " + Helper.prtlen(Helper.toFixed(Helper.rad2deg(LongitudeOfNode), 3), 11, Helper.Align.RIGHT);
				screenLabels[14].Text = "ARG. OF PERIAPSIS: " + Helper.prtlen(Helper.toFixed(Helper.rad2deg(ArgumentOfPeriapsis), 3), 11, Helper.Align.RIGHT);
				screenLabels[15].Text = "     TRUE ANOMALY: " + Helper.prtlen(Helper.toFixed(TrueAnomaly, 3), 11, Helper.Align.RIGHT);
				screenLabels[16].Text = "       ORB PERIOD: " + Helper.prtlen(Helper.timeString(OrbitPeriod, true, 2), 11, Helper.Align.RIGHT);
				screenLabels[17].Text = "       SOI CHANGE: " + Helper.prtlen(Helper.timeString(SOIChange, true, 2), 11, Helper.Align.RIGHT);
				screenLabels[18].Text = "        ORB SPEED: " + Helper.prtlen(Helper.toFixed(OrbitalSpeed, 3), 11, Helper.Align.RIGHT);

				// FLIGHT ELEMENTS
				screenLabels[21].Text = "         ALTITUDE: " + Helper.prtlen(Helper.toFixed(altitude, 3), 11, Helper.Align.RIGHT);
				screenLabels[22].Text = "         APOAPSIS: " + Helper.prtlen(Helper.toFixed(apoapsis, 3), 11, Helper.Align.RIGHT);
				screenLabels[23].Text = "        PERIAPSIS: " + Helper.prtlen(Helper.toFixed(periapsis, 3), 11, Helper.Align.RIGHT);
				screenLabels[24].Text = " TIME TO APOAPSIS: " + Helper.prtlen(Helper.timeString(timeToApoapsis, true, 2), 11, Helper.Align.RIGHT);
				screenLabels[25].Text = "TIME TO PERIAPSIS: " + Helper.prtlen(Helper.timeString(timeToPeriapsis, true, 2), 11, Helper.Align.RIGHT);


				// DIRECTION VECTORS
				Tuple<double, double, double> inerProgradeRPY = inerVectorToRPY(inerPrograde);
				Tuple<double, double, double> inerRetrogradeRPY = inerVectorToRPY(inerRetrograde);
				Tuple<double, double, double> inerNormalRPY = inerVectorToRPY(inerNormal);
				Tuple<double, double, double> inerAntiNormalRPY = inerVectorToRPY(inerAntiNormal);
				Tuple<double, double, double> inerRadialRPY = inerVectorToRPY(inerRadial);
				Tuple<double, double, double> inerAntiRadialRPY = inerVectorToRPY(inerAntiRadial);

				Tuple<double, double, double> surfProgradeRPY = surfVectorToRPY(surfPrograde);
				Tuple<double, double, double> surfRetrogradeRPY = surfVectorToRPY(surfRetrograde);
				Tuple<double, double, double> surfNormalRPY = surfVectorToRPY(surfNormal);
				Tuple<double, double, double> surfAntiNormalRPY = surfVectorToRPY(surfAntiNormal);
				Tuple<double, double, double> surfRadialRPY = surfVectorToRPY(surfRadial);
				Tuple<double, double, double> surfAntiRadialRPY = surfVectorToRPY(surfAntiRadial);

				// INERTIAL REFERENCE DIRECTIONS
				string prR = Helper.prtlen(Helper.toFixed(inerProgradeRPY.Item1, 2), 8, Helper.Align.RIGHT);
				string prP = Helper.prtlen(Helper.toFixed(inerProgradeRPY.Item2, 2), 10, Helper.Align.RIGHT);
				string prY = Helper.prtlen(Helper.toFixed(inerProgradeRPY.Item3, 2), 10, Helper.Align.RIGHT);
				screenLabels[82].Text = prR + prP + prY;

				prR = Helper.prtlen(Helper.toFixed(inerRetrogradeRPY.Item1, 2), 8, Helper.Align.RIGHT);
				prP = Helper.prtlen(Helper.toFixed(inerRetrogradeRPY.Item2, 2), 10, Helper.Align.RIGHT);
				prY = Helper.prtlen(Helper.toFixed(inerRetrogradeRPY.Item3, 2), 10, Helper.Align.RIGHT);
				screenLabels[83].Text = prR + prP + prY;

				prR = Helper.prtlen(Helper.toFixed(inerNormalRPY.Item1, 2), 8, Helper.Align.RIGHT);
				prP = Helper.prtlen(Helper.toFixed(inerNormalRPY.Item2, 2), 10, Helper.Align.RIGHT);
				prY = Helper.prtlen(Helper.toFixed(inerNormalRPY.Item3, 2), 10, Helper.Align.RIGHT);
				screenLabels[84].Text = prR + prP + prY;

				prR = Helper.prtlen(Helper.toFixed(inerAntiNormalRPY.Item1, 2), 8, Helper.Align.RIGHT);
				prP = Helper.prtlen(Helper.toFixed(inerAntiNormalRPY.Item2, 2), 10, Helper.Align.RIGHT);
				prY = Helper.prtlen(Helper.toFixed(inerAntiNormalRPY.Item3, 2), 10, Helper.Align.RIGHT);
				screenLabels[85].Text = prR + prP + prY;

				prR = Helper.prtlen(Helper.toFixed(inerRadialRPY.Item1, 2), 8, Helper.Align.RIGHT);
				prP = Helper.prtlen(Helper.toFixed(inerRadialRPY.Item2, 2), 10, Helper.Align.RIGHT);
				prY = Helper.prtlen(Helper.toFixed(inerRadialRPY.Item3, 2), 10, Helper.Align.RIGHT);
				screenLabels[86].Text = prR + prP + prY;

				prR = Helper.prtlen(Helper.toFixed(inerAntiRadialRPY.Item1, 2), 8, Helper.Align.RIGHT);
				prP = Helper.prtlen(Helper.toFixed(inerAntiRadialRPY.Item2, 2), 10, Helper.Align.RIGHT);
				prY = Helper.prtlen(Helper.toFixed(inerAntiRadialRPY.Item3, 2), 10, Helper.Align.RIGHT);
				screenLabels[87].Text = prR + prP + prY;


				// SURFACE REFERENCE DIRECTIONS
				prR = Helper.prtlen(Helper.toFixed(surfProgradeRPY.Item1, 2), 8, Helper.Align.RIGHT);
				prP = Helper.prtlen(Helper.toFixed(surfProgradeRPY.Item2, 2), 10, Helper.Align.RIGHT);
				prY = Helper.prtlen(Helper.toFixed(surfProgradeRPY.Item3, 2), 10, Helper.Align.RIGHT);
				screenLabels[92].Text = prR + prP + prY;

				prR = Helper.prtlen(Helper.toFixed(surfRetrogradeRPY.Item1, 2), 8, Helper.Align.RIGHT);
				prP = Helper.prtlen(Helper.toFixed(surfRetrogradeRPY.Item2, 2), 10, Helper.Align.RIGHT);
				prY = Helper.prtlen(Helper.toFixed(surfRetrogradeRPY.Item3, 2), 10, Helper.Align.RIGHT);
				screenLabels[93].Text = prR + prP + prY;

				prR = Helper.prtlen(Helper.toFixed(surfNormalRPY.Item1, 2), 8, Helper.Align.RIGHT);
				prP = Helper.prtlen(Helper.toFixed(surfNormalRPY.Item2, 2), 10, Helper.Align.RIGHT);
				prY = Helper.prtlen(Helper.toFixed(surfNormalRPY.Item3, 2), 10, Helper.Align.RIGHT);
				screenLabels[94].Text = prR + prP + prY;

				prR = Helper.prtlen(Helper.toFixed(surfAntiNormalRPY.Item1, 2), 8, Helper.Align.RIGHT);
				prP = Helper.prtlen(Helper.toFixed(surfAntiNormalRPY.Item2, 2), 10, Helper.Align.RIGHT);
				prY = Helper.prtlen(Helper.toFixed(surfAntiNormalRPY.Item3, 2), 10, Helper.Align.RIGHT);
				screenLabels[95].Text = prR + prP + prY;

				prR = Helper.prtlen(Helper.toFixed(surfRadialRPY.Item1, 2), 8, Helper.Align.RIGHT);
				prP = Helper.prtlen(Helper.toFixed(surfRadialRPY.Item2, 2), 10, Helper.Align.RIGHT);
				prY = Helper.prtlen(Helper.toFixed(surfRadialRPY.Item3, 2), 10, Helper.Align.RIGHT);
				screenLabels[96].Text = prR + prP + prY;

				prR = Helper.prtlen(Helper.toFixed(surfAntiRadialRPY.Item1, 2), 8, Helper.Align.RIGHT);
				prP = Helper.prtlen(Helper.toFixed(surfAntiRadialRPY.Item2, 2), 10, Helper.Align.RIGHT);
				prY = Helper.prtlen(Helper.toFixed(surfAntiRadialRPY.Item3, 2), 10, Helper.Align.RIGHT);
				screenLabels[97].Text = prR + prP + prY;
			}
		}

		public override void resize() { }

		private double normalize(double value, double low_limit, double high_limit)
		{
			double val = value;
			double spread = high_limit - low_limit;
			if (value > high_limit) val = value - spread;
			if (value < low_limit) val = value + spread;
			return val;
		}

		private Tuple<double, double, double> surfVectorToRPY(Tuple<double, double, double> unitVector)
		{
			double roll = 0;
			double pitch = 0;
			double yaw = 0;
			double angle = 0;

			// VECTOR INFO: X = Up, Y = North, Z = East

			// YAW
			yaw = Helper.rad2deg(Math.Atan2(unitVector.Item2, unitVector.Item3) * -1) + 90;
			if (yaw < 0) yaw += 360; // Limit values


			// PITCH
			double yawLength = Math.Sqrt(Math.Pow(unitVector.Item2, 2) + Math.Pow(unitVector.Item3, 2));
			pitch = Helper.rad2deg(Math.Atan2(unitVector.Item1, yawLength));


			return new Tuple<double, double, double>(roll, pitch, yaw);
		}

		private Tuple<double, double, double> inerVectorToRPY(Tuple<double, double, double> unitVector)
		{
			double roll = 0;
			double pitch = 0;
			double yaw = 0;

			// VECTOR INFO: X = Arbitrary out of Equator, Y = Through North Pole of body, Z = Arbitrary out of Equator (90° between X and Z)

			// YAW
			yaw = Helper.rad2deg(Math.Atan2(unitVector.Item1, unitVector.Item3) * -1) + 90;
			if (yaw < 0) yaw += 360; // Limit values


			// PITCH
			double yawLength = Math.Sqrt(Math.Pow(unitVector.Item1, 2) + Math.Pow(unitVector.Item3, 2));
			pitch = Helper.rad2deg(Math.Atan2(unitVector.Item2, yawLength));


			return new Tuple<double, double, double>(roll, pitch, yaw);
		}
	}
}
