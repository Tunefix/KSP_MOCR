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
	partial class Pilot1
	{
		private void updateDSKY()
		{
			// VERB DISPLAY
			String verbStr = "";
			if (verb != null)
			{
				verbStr = verb;
				if (verbStr.Length == 1) { verbStr = verbStr + " "; }
			}
			else if(activeVerb != -1)
			{
				verbStr = activeVerb.ToString();
				if (verbStr.Length == 1) { verbStr = verbStr + " "; }
			}			
			screenSegDisps[3].setValue(verbStr);
			
			// NOUN DISPLAY
			String nounStr = "";
			if (noun != null)
			{
				nounStr = noun;
				if (nounStr.Length == 1) { nounStr = nounStr + " "; }
			}
			else if (activeNoun != -1)
			{
				nounStr = activeNoun.ToString();
				if (nounStr.Length == 1) { nounStr = nounStr + " "; }
			}			
			screenSegDisps[4].setValue(nounStr);
			
			// PROG DISPLAY
			String progStr = "";
			if (activeProg != -1)
			{
				progStr = activeProg.ToString();
				if (progStr.Length == 1) { progStr = "0" + progStr; }
			}			
			screenSegDisps[5].setValue(progStr);

			if (activeNoun != -1 && activeVerb != -1 && activeVerb != 88)
			{
				runVerb(activeVerb, activeNoun);
			}

			if (activeProg != -1)
			{
				runProgram(activeProg);
			}

			/*
			 * DEBUG
			 */
			/*
			Console.WriteLine("NOUN: " + noun);
			Console.WriteLine("VERB: " + verb);
			Console.WriteLine("A-NOUN: " + activeNoun.ToString());
			Console.WriteLine("A-VERB: " + activeVerb.ToString());
			Console.WriteLine("A-PROG: " + activeProg.ToString());
			Console.WriteLine("NOUN-S: " + nounStr);
			Console.WriteLine("VERB-S: " + verbStr);
			Console.WriteLine("PROG-S: " + progStr);
			/**/
		}

		private void runProgram(int prog)
		{
			switch (prog)
			{
				case 0:
					// Idle
					break;
				case 1: // Initialisation program
					activeVerb = 16;
					activeNoun = 20;
					runProg1();
					break;
				case 2: // Ready to launch
					runProg2();
					break;
				case 3: // Ready to launch
					runProg3();
					break;
				case 11: // Launch program
					activeVerb = 16;
					activeNoun = 73;
					runProg11();
					break;
			}
		}

		private void runProg1()
		{
			/*
			 * COARSE ALIGN OF FDAI
			 */
			int FR = (int)Math.Round(FDAIRoll * 100);
			int FP = (int)Math.Round(FDAIPitch * 100);
			int FY = (int)Math.Round(FDAIYaw * 100);
			int TR = (int)Math.Round(roll * 100);
			int TP = (int)Math.Round(pitch * 100);
			int TY = (int)Math.Round(yaw * 100);

			int DR = FR - TR;
			int DP = FP - TP;
			int DY = FY - TY;
			
			if ((FR == TR || Math.Abs(DR) < 100)
				&& (FP == TP || Math.Abs(DP) < 100)
				&& (FY == TY || Math.Abs(DY) < 100)
			)
			{
				activeProg = 2;
			}
			else
			{
				if (FR != TR)
				{
					if (DR > 0)
					{
						FDAIRoll -= 1f;
					}
					else
					{
						FDAIRoll += 1f;
					}
				}
				
				if (FP != TP)
				{
					if (DP > 0)
					{
						FDAIPitch -= 1f;
					}
					else
					{
						FDAIPitch += 1f;
					}
				}
				
				if (FY != TY)
				{
					if (DY > 0)
					{
						FDAIYaw -= 1f;
					}
					else
					{
						FDAIYaw += 1f;
					}
				}
				
			}
		}
		
		private void runProg2()
		{
			/*
			 * FINE ALIGN OF FDAI
			 */
			int FR = (int)Math.Round(FDAIRoll * 100);
			int FP = (int)Math.Round(FDAIPitch * 100);
			int FY = (int)Math.Round(FDAIYaw * 100);
			int TR = (int)Math.Round(roll * 100);
			int TP = (int)Math.Round(pitch * 100);
			int TY = (int)Math.Round(yaw * 100);
			if (FR == TR && FP == TP && FY == TY)
			{
				activeProg = 3;
			}
			else
			{
				if (FR != TR)
				{
					int diff = FR - TR;
					if (diff > 0)
					{
						FDAIRoll -= 0.01f;
					}
					else
					{
						FDAIRoll += 0.01f;
					}
				}
				
				if (FP != TP)
				{
					int diff = FP - TP;
					if (diff > 0)
					{
						FDAIPitch -= 0.01f;
					}
					else
					{
						FDAIPitch += 0.01f;
					}
				}
				
				if (FY != TY)
				{
					int diff = FY - TY;
					if (diff > 0)
					{
						FDAIYaw -= 0.01f;
					}
					else
					{
						FDAIYaw += 0.01f;
					}
				}
				
			}
		}
		
		private void runProg3()
		{
			if (MET > 0)
			{
				activeProg = 11;
			}
		}
		
		private void runProg11()
		{
		}

		private void runVerb(int verb, int noun)
		{
			switch (verb)
			{
				case 16: // Monitor Data
					verb16(noun);
					break;
				case 35: // Lamp Test
					verb35();
					break;
				case 37: // Run Program
					verb37(noun);
					break;
				case 69: // Reset Display
					verb69();
					break;
			}
		}

		private void verb16(int noun)
		{
			// Registers (aka output)
			String r1 = "";
			String r2 = "";
			String r3 = "";
			// Precision
			int p1 = 0;
			int p2 = 0;
			int p3 = 0;
			// Noun data
			int?[] nounData = getNounData(noun);

			if (nounData[0] != null)
			{
				screenSegDisps[0].setValue(nounData[0].ToString(), (int)nounData[3]);
				screenSegDisps[1].setValue(nounData[1].ToString(), (int)nounData[4]);
				screenSegDisps[2].setValue(nounData[2].ToString(), (int)nounData[5]);
			}
			else
			{
				oprError = true;
			}
		}

		private void verb35()
		{
			storeProg = activeProg;
			screenSegDisps[0].setValue("88888");
			screenSegDisps[1].setValue("88888");
			screenSegDisps[2].setValue("88888");
			verb = null;
			noun = null;
			activeVerb = 88;
			activeNoun = 88;
			activeProg = 88;
		}

		private void verb37(int noun)
		{
			activeProg = noun;
			activeVerb = -1;
			activeNoun = -1;
		}

		private void verb69()
		{
			// Registers (aka output)
			String r1 = "";
			String r2 = "";
			String r3 = "";
			// Precision
			int p1 = 0;
			int p2 = 0;
			int p3 = 0;

			verb = null;
			noun = null;
			activeVerb = -1;
			activeNoun = -1;
			if (storeProg != -1)
			{
				activeProg = storeProg;
				storeProg = -1;
			}
			
			screenSegDisps[0].setValue(r1,p1);
			screenSegDisps[1].setValue(r2,p2);
			screenSegDisps[2].setValue(r3,p3);
		}

		private int?[] getNounData(int noun)
		{
			/*
			 * values structure:
			 * values[0]: R1
			 * values[1]: R2
			 * values[2]: R3
			 * values[3]: P1
			 * values[4]: P2
			 * values[5]: P3
			 *
			 * If noun not found all values are null
			 */
			int?[] values = new int?[6];
			switch (noun)
			{
				case 17: // Attitude
					values[0] = (int)Math.Round(roll * 100);
					values[1] = (int)Math.Round(pitch * 100);
					values[2] = (int)Math.Round(yaw * 100);
					values[3] = values[4] = values[5] = 2;
					break;
					
				case 20: // FDAI angles
					values[0] = (int)Math.Round(FDAIRoll * 100);
					values[1] = (int)Math.Round(FDAIPitch* 100);
					values[2] = (int)Math.Round(FDAIYaw * 100);
					values[3] = values[4] = values[5] = 2;
					break;
					
				case 36: // MET
					double secs = MET;
					int mins = (int)Math.Floor(secs / 60f);
					secs = secs - (mins * 60);
					int hrs = (int)Math.Floor(mins / 60f);
					mins = mins - (hrs * 60);
					
					values[0] = hrs;
					values[1] = mins;
					values[2] = (int)Math.Round(secs * 100);

					values[3] = 0;
					values[4] = 0;
					values[5] = 2;
					break;
				case 73: // Flight Data
					values[0] = (int)Math.Round(meanAltitude / 10);
					values[1] = (int)Math.Round(orbitSpeed * 10);
					values[2] = (int)Math.Round(pitch * 100);

					// Peg altitude
					if (meanAltitude > 1000000)
					{
						values[0] = 99999;
					}

					values[3] = 2;
					values[4] = 1;
					values[5] = 2;
					break;
			}
			return values;
		}
	}
}
