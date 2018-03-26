using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	partial class AGC
	{
		private Noun getNoun(string noun)
		{
			// DECLARE ALL VARIABLES
			string shrs, smin, ssec;
			string r1, r2, r3;
			string r1s, r2s, r3s;
			double met, hrs, min, sec;
			double R, P, Y, d1, d2, d3;
			DateTime now;

			switch (noun)
			{
				// ALL THE SIMPLE NOUNS
				case "20": // FDAI offset angles
				case "23": // FDAI target angles
				case "29": // Launch azimuth
					return getNounFromStorage(noun);
				case "17":
					// SURFACE REFERENCE ATTITUDE
					r1 = r2 = r3 = r1s = r2s = r3s = "";
					if (streams != null)
					{
						R = streams.GetData(DataType.flight_roll);
						P = streams.GetData(DataType.flight_pitch);
						Y = streams.GetData(DataType.flight_heading);

						if (R < 0) r1s = "NEG";
						if (P < 0) r2s = "NEG";
						if (Y < 0) r3s = "NEG";

						r1 = Math.Round(R * 100f).ToString();
						r2 = Math.Round(P * 100f).ToString();
						r3 = Math.Round(Y * 100f).ToString();
					}
					return new Noun(r1, "2", r1s, r2, "2", r2s, r3, "2", r3s);
				case "18":
					// INERTIAL REFERENCE ATTITUDE
					r1 = r2 = r3 = r1s = r2s = r3s = "";
					if (streams != null)
					{
						R = streams.GetData(DataType.flight_inertial_roll);
						P = streams.GetData(DataType.flight_inertial_pitch);
						Y = streams.GetData(DataType.flight_inertial_yaw);

						if (R < 0) r1s = "NEG";
						if (P < 0) r2s = "NEG";
						if (Y < 0) r3s = "NEG";

						r1 = Math.Round(R * 100f).ToString();
						r2 = Math.Round(P * 100f).ToString();
						r3 = Math.Round(Y * 100f).ToString();
					}
					return new Noun(r1, "2", r1s, r2, "2", r2s, r3, "2", r3s);
				case "19":
					// FDAI ATTITUDE (ATTITUDE + OFFSET)
					r1 = r2 = r3 = r1s = r2s = r3s = "";
					if (streams != null)
					{
						if (dataStorage.getData("FDAI_MODE") == "SURF")
						{
							R = streams.GetData(DataType.flight_roll);
							P = streams.GetData(DataType.flight_pitch);
							Y = streams.GetData(DataType.flight_heading);
						}
						else
						{
							R = streams.GetData(DataType.flight_inertial_roll);
							P = streams.GetData(DataType.flight_inertial_pitch);
							Y = streams.GetData(DataType.flight_inertial_yaw);
						}

						// Offset angles
						double.TryParse(dataStorage.getData("AGC_N20R1"), out d1);
						d1 = d1 / 100f;
						if (dataStorage.getData("AGC_N20R1S") == "NEG") d1 = d1 * -1;

						double.TryParse(dataStorage.getData("AGC_N20R2"), out d2);
						d2 = d2 / 100f;
						if (dataStorage.getData("AGC_N20R2S") == "NEG") d2 = d2 * -1;

						double.TryParse(dataStorage.getData("AGC_N20R3"), out d3);
						d3 = d3 / 100f;
						if (dataStorage.getData("AGC_N20R3S") == "NEG") d3 = d3 * -1;

						R = R + d1;
						P = P + d2;
						Y = Y + d3;

						if (R < 0) r1s = "NEG";
						if (P < 0) r2s = "NEG";
						if (Y < 0) r3s = "NEG";

						r1 = Math.Round(R * 100f).ToString();
						r2 = Math.Round(P * 100f).ToString();
						r3 = Math.Round(Y * 100f).ToString();
					}
					return new Noun(r1, "2", r1s, r2, "2", r2s, r3, "2", r3s);
				case "36":
					// DISPLAY MET
					shrs = "0";
					smin = "00";
					ssec = "00";
					if (streams != null)
					{
						met = streams.GetData(DataType.vessel_MET);
						hrs = Math.Floor(met / 3600f);
						min = Math.Floor((met - (hrs * 3600)) / 60);
						sec = Math.Floor(met - (hrs * 3600) - (min * 60));
						shrs = hrs.ToString();
						smin = min.ToString();
						ssec = sec.ToString();

						if (min < 10) smin = "0" + smin;
						if (sec < 10) ssec = "0" + ssec;
					}
					return new Noun(shrs, smin, ssec);

				case "37":
					// DISPLAY LT
					shrs = "0";
					smin = "00";
					ssec = "00";

					now = DateTime.Now;
					hrs = now.Hour;
					min = now.Minute;
					sec = now.Second;
					shrs = hrs.ToString();
					smin = min.ToString();
					ssec = sec.ToString();

					if (hrs < 10) shrs = "0" + shrs;
					if (min < 10) smin = "0" + smin;
					if (sec < 10) ssec = "0" + ssec;
					
					return new Noun(shrs, smin, ssec);

				default:
					// TURN ON OPR ERR LIGHT
					dataStorage.setData("AGC_OPRERR", "SET");
					return new Noun("", "", "");
			}
		}

		private Noun getNounFromStorage(string noun)
		{
			// GET ALL NOUN VALUES
			string R1 = dataStorage.getData("AGC_N" + noun + "R1");
			string R1P = dataStorage.getData("AGC_N" + noun + "R1P");
			string R1S = dataStorage.getData("AGC_N" + noun + "R1S");

			string R2 = dataStorage.getData("AGC_N" + noun + "R2");
			string R2P = dataStorage.getData("AGC_N" + noun + "R2P");
			string R2S = dataStorage.getData("AGC_N" + noun + "R2S");

			string R3 = dataStorage.getData("AGC_N" + noun + "R3");
			string R3P = dataStorage.getData("AGC_N" + noun + "R3P");
			string R3S = dataStorage.getData("AGC_N" + noun + "R3S");

			return new Noun(R1, R1P, R1S, R2, R2P, R2S, R3, R3P, R3S);
		}

		private void storeNoun()
		{
			string r1, r2, r3;

			switch(noun)
			{
				case "20": // FDAI OFFSET ANGLES
					storeNounSet("20", pendingR1, "2", pR1neg, pendingR2, "2", pR2neg, pendingR3, "2", pR3neg);
					break;

				case "23": // FDAI TARGET ANGLES
					storeNounSet("23", pendingR1, "2", pR1neg, pendingR2, "2", pR2neg, pendingR3, "2", pR3neg);
					break;
			}

			// CLEAR ALL PENDING
			pendingR1 = "";
			pendingR2 = "";
			pendingR3 = "";
			pR1neg = "";
			pR2neg = "";
			pR3neg = "";
		}

		private void storeNounSet(string nouns, string r1, string r1p, string r1s, string r2, string r2p, string r2s, string r3, string r3p, string r3s)
		{
			dataStorage.setData("AGC_N" + noun + "R1", r1);
			dataStorage.setData("AGC_N" + noun + "R2", r2);
			dataStorage.setData("AGC_N" + noun + "R3", r3);
			dataStorage.setData("AGC_N" + noun + "R1P", r1p);
			dataStorage.setData("AGC_N" + noun + "R2P", r2p);
			dataStorage.setData("AGC_N" + noun + "R3P", r3p);
			dataStorage.setData("AGC_N" + noun + "R1S", r1s);
			dataStorage.setData("AGC_N" + noun + "R2S", r2s);
			dataStorage.setData("AGC_N" + noun + "R3S", r3s);
		}
	}
}
