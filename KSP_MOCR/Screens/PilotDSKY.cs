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

			// REG DISPLAYS
			int[] inputVerbs = { 21, 26 };
			if (activeVerb == -1 || inputVerbs.Contains(activeVerb))
			{
				if (r1 != "") { regValueShow(0, r1, r1sign, r1precision); }
				if (r2 != "") { regValueShow(1, r2, r2sign, r2precision); }
				if (r3 != "") { regValueShow(2, r3, r3sign, r3precision); }
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

		private void regValueShow(int dispID, String value, SegDisp.SignState sign, int precision)
		{
			screenSegDisps[dispID].setValue(value, precision, sign, false, SegDisp.Align.LEFT);
		}

		private void runProgram(int prog)
		{
			switch (prog)
			{
				case 0:
					// Idle
					runProg0();
					break;
				case 1: // Initialisation program
					activeVerb = 16;
					activeNoun = 19;
					runProg1();
					break;
				case 2: // Ready to launch, holding FDAI angles
					runProg2();
					break;
				case 11: // Launch program
					activeVerb = 16;
					activeNoun = 73;
					runProg11();
					break;
				case 30: // Load Burn Data
					keyRelCheck();
					runProg30();
					break;
			}
		}

		private void keyRelCheck()
		{
			if (activeVerb != -1)
			{
				keyRelPress = false;
				keyRel = true;
			}
		}

		private void runProg0()
		{
			/**
			 * IDLE PROGRAM
			 * Clear DKSY and idle
			 */
			if (runOnce)
			{
				r1 = "";
				r2 = "";
				r3 = "";
				screenSegDisps[0].setValue("", 0);
				screenSegDisps[1].setValue("", 0);
				screenSegDisps[2].setValue("", 0);
				runOnce = false;
			}
		}

		private void runProg1()
		{
			/*
			 * COARSE ALIGN OF FDAI TO LAUNCH ANGLES R: 90 + azimuth, P: 90; Y: 0
			 */
			int FR = (int)Math.Round((inerRoll + screenFDAI.offsetR) * 100);
			int FP = (int)Math.Round((inerPitch + screenFDAI.offsetP) * 100);
			int FY = (int)Math.Round((inerYaw + screenFDAI.offsetY) * 100);
			int TR = (int)Math.Round((90 + launchAzimuth) * 100);
			int TP = (int)Math.Round(90f * 100);
			int TY = (int)Math.Round(0f * 100);

			int DR = FR - TR;
			int DP = FP - TP;
			int DY = FY - TY;
			
			if ((FR == TR || Math.Abs(DR) < 100)
				&& (FP == TP || Math.Abs(DP) < 100)
				&& (FY == TY || Math.Abs(DY) < 100)
			)
			{
				// SET LAUNCH ANGLES INTO LockROT, set mode to lock
				// and load roll/pitch program values into setRot.
				lockRotR = 180;
				lockRotP = 87;
				lockRotY = (int)launchAzimuth;
				controlMode = 2;
				setRotR = (int)(90 + launchAzimuth);
				setRotP = 80;
				setRotY = (int)launchAzimuth;
				activeProg = 2;
			}
			else
			{
				if (FR != TR)
				{
					if (DR > 0)
					{
						screenFDAI.offsetR -= 1f;
					}
					else
					{
						screenFDAI.offsetR += 1f;
					}
				}
				
				if (FP != TP)
				{
					if (DP > 0)
					{
						screenFDAI.offsetP -= 1f;
					}
					else
					{
						screenFDAI.offsetP += 1f;
					}
				}
				
				if (FY != TY)
				{
					if (DY > 0)
					{
						screenFDAI.offsetY -= 1f;
					}
					else
					{
						screenFDAI.offsetY += 1f;
					}
				}
			}
		}
		
		private void runProg2()
		{
			/*
			 * FINE ALIGN OF LAUNCH REFSMMAT
			 *  (This continues to run, and hold the FDAI at 180 + azimuth,90,0 until launch)
			 *  It also updates setRotR with new angle should launch azimuth change
			 */
			int FR = (int)Math.Round((inerRoll + screenFDAI.offsetR) * 100);
			int FP = (int)Math.Round((inerPitch + screenFDAI.offsetP) * 100);
			int FY = (int)Math.Round((inerYaw + screenFDAI.offsetY) * 100);
			int TR = (int)Math.Round((90 + launchAzimuth) * 100);
			int TP = (int)Math.Round(90f * 100);
			int TY = (int)Math.Round(0f * 100);
			
			setRotR = (int)(90 + launchAzimuth);
		
			if (FR != TR)
			{
				int diff = FR - TR;
				if (diff > 0)
				{
					if (diff > 1)
					{
						screenFDAI.offsetR -= 1f;
					}
					else
					{
						screenFDAI.offsetR -= 0.01f;
					}
				}
				else
				{
					if (diff > 1)
					{
						screenFDAI.offsetR += 1f;
					}
					else
					{
						screenFDAI.offsetR += 0.01f;
					}
				}
			}
			
			if (FP != TP)
			{
				int diff = FP - TP;
				if (diff > 0)
				{
					screenFDAI.offsetP -= 0.01f;
				}
				else
				{
					screenFDAI.offsetP += 0.01f;
				}
			}
			
			if (FY != TY)
			{
				int diff = FY - TY;
				if (diff > 0)
				{
					screenFDAI.offsetY -= 0.01f;
				}
				else
				{
					screenFDAI.offsetY += 0.01f;
				}
			}
			
			if (MET > 0)
			{
				activeProg = 11;
			}
		}
		
		private void runProg11()
		{
		}
		
		private void runProg30()
		{
			switch (progStep)
			{
				case 0:
					activeVerb = -1;
					activeNoun = 20;
					r1 = " "; // Set to 1 space to show the decimal point
					r2 = " "; // Set to 1 space to show the decimal point
					r3 = " "; // Set to 1 space to show the decimal point
					r1precision = 2;
					r2precision = 2;
					r3precision = 2;
					r1sign = SegDisp.SignState.NONE;
					r2sign = SegDisp.SignState.NONE;
					r3sign = SegDisp.SignState.NONE;
					flashing = true;
					entrPress = false;
					progStep++;
					enterR1 = true;
					break;
				case 1:
					if (!keyRel) { activeNoun = 20; }
					if (entrPress && r1.Length == 5)
					{
						if (r1sign == SegDisp.SignState.MINUS)
						{
							dataStorage.Add(int.Parse(r1) * -1);
						}
						else
						{
							dataStorage.Add(int.Parse(r1));
						}
						entrPress = false;
						progStep++;
						enterR2 = true;
					}
					else if (entrPress && activeVerb == -1)
					{
						oprError = true;
						entrPress = false;
					}
					else if (entrPress)
					{
						entrPress = false;
					}
					break;
				case 2:
					if (!keyRel) { activeNoun = 20; }
					if (entrPress && r2.Length == 5)
					{
						if (r2sign == SegDisp.SignState.MINUS)
						{
							dataStorage.Add(int.Parse(r2) * -1);
						}
						else
						{
							dataStorage.Add(int.Parse(r2));
						}
						entrPress = false;
						progStep++;
						enterR3 = true;
					}
					else if (entrPress && activeVerb == -1)
					{
						oprError = true;
						entrPress = false;
					}
					else if (entrPress)
					{
						entrPress = false;
					}
					break;
				case 3:
					if (!keyRel) { activeNoun = 20; }
					if (entrPress && r3.Length == 5)
					{
						if (r3sign == SegDisp.SignState.MINUS)
						{
							dataStorage.Add(int.Parse(r3) * -1);
						}
						else
						{
							dataStorage.Add(int.Parse(r3));
						}
						entrPress = false;
						progStep++;
						activeNoun = 34;
						enterR1 = true;
					}
					else if (entrPress && activeVerb == -1)
					{
						oprError = true;
						entrPress = false;
					}
					else if (entrPress)
					{
						entrPress = false;
					}
					break;
				case 4: // BLANK REGISTERS FOR FETCH OF N34 DATA
					r1 = " "; // Set to 1 space to show the decimal point
					r2 = " "; // Set to 1 space to show the decimal point
					r3 = " "; // Set to 1 space to show the decimal point
					r1precision = 0;
					r2precision = 0;
					r3precision = 2;
					r1sign = SegDisp.SignState.NONE;
					r2sign = SegDisp.SignState.NONE;
					r3sign = SegDisp.SignState.NONE;
					progStep++;
					break;
				case 5:
					if (!keyRel) { activeNoun = 34; }
					if (entrPress && r1.Length == 5)
					{
						if (r1sign == SegDisp.SignState.MINUS)
						{
							dataStorage.Add(int.Parse(r1) * -1);
						}
						else
						{
							dataStorage.Add(int.Parse(r1));
						}
						entrPress = false;
						progStep++;
						enterR2 = true;
					}
					else if (entrPress && activeVerb == -1)
					{
						oprError = true;
						entrPress = false;
					}
					else if (entrPress)
					{
						entrPress = false;
					}
					break;
				case 6:
					if (!keyRel) { activeNoun = 34; }
					if (entrPress && r2.Length == 5)
					{
						if (r2sign == SegDisp.SignState.MINUS)
						{
							dataStorage.Add(int.Parse(r2) * -1);
						}
						else
						{
							dataStorage.Add(int.Parse(r2));
						}
						entrPress = false;
						progStep++;
						enterR3 = true;
					}
					else if (entrPress && activeVerb == -1)
					{
						oprError = true;
						entrPress = false;
					}
					else if (entrPress)
					{
						entrPress = false;
					}
					break;
				case 7:
					if (!keyRel) { activeNoun = 34; }
					if (entrPress && r3.Length == 5)
					{
						if (r3sign == SegDisp.SignState.MINUS)
						{
							dataStorage.Add(int.Parse(r3) * -1);
						}
						else
						{
							dataStorage.Add(int.Parse(r3));
						}
						entrPress = false;
						progStep++;
						activeNoun = 80;
						enterR1 = true;
					}
					else if (entrPress && activeVerb == -1)
					{
						oprError = true;
						entrPress = false;
					}
					else if (entrPress)
					{
						entrPress = false;
					}
					break;
				case 8: // BLANK REGISTERS FOR FETCH OF N80 DATA
					r1 = " "; // Set to 1 space to show the decimal point
					r2 = " "; // Set to 1 space to show the decimal point
					r3 = " "; // Set to 1 space to show the decimal point
					r1precision = 1;
					r2precision = 0;
					r3precision = 2;
					r1sign = SegDisp.SignState.NONE;
					r2sign = SegDisp.SignState.NONE;
					r3sign = SegDisp.SignState.NONE;
					progStep++;
					break;
				case 9:
					if (!keyRel) { activeNoun = 80; }
					if (entrPress && r1.Length == 5)
					{
						if (r1sign == SegDisp.SignState.MINUS)
						{
							dataStorage.Add(int.Parse(r1) * -1);
						}
						else
						{
							dataStorage.Add(int.Parse(r1));
						}
						entrPress = false;
						progStep++;
						enterR2 = true;
					}
					else if (entrPress && activeVerb == -1)
					{
						oprError = true;
						entrPress = false;
					}
					else if (entrPress)
					{
						entrPress = false;
					}
					break;
				case 10:
					if (!keyRel) { activeNoun = 80; }
					if (entrPress && r2.Length == 5)
					{
						if (r2sign == SegDisp.SignState.MINUS)
						{
							dataStorage.Add(int.Parse(r2) * -1);
						}
						else
						{
							dataStorage.Add(int.Parse(r2));
						}
						entrPress = false;
						progStep++;
						enterR3 = true;
					}
					else if (entrPress && activeVerb == -1)
					{
						oprError = true;
						entrPress = false;
					}
					else if (entrPress)
					{
						entrPress = false;
					}
					break;
				case 11:
					if (!keyRel) { activeNoun = 80; }
					if (entrPress && r3.Length == 5)
					{
						if (r3sign == SegDisp.SignState.MINUS)
						{
							dataStorage.Add(int.Parse(r3) * -1);
						}
						else
						{
							dataStorage.Add(int.Parse(r3));
						}

						// Store all the data
						screenFDAI.offsetR = dataStorage[0] / 100f;
						screenFDAI.offsetP = dataStorage[1] / 100f;
						screenFDAI.offsetY = dataStorage[2] / 100f;

						TIG = (dataStorage[3] * 3600) + (dataStorage[4] * 60) + (dataStorage[5] / 100f);

						deltaV = dataStorage[6] * 10f;

						burnTime = (dataStorage[7] * 60) + (dataStorage[8] / 100f);

						// Clear registers
						r1 = "";
						r2 = "";
						r3 = "";
						
						entrPress = false;
						progStep++;
						activeVerb = 16;
						activeNoun = 34;
						flashing = false;
						flashOn = false;
						flashTime.Stop();
					}
					else if (entrPress && activeVerb == -1)
					{
						oprError = true;
						entrPress = false;
					}
					else if (entrPress)
					{
						entrPress = false;
					}
					break;
				case 12: // SHOWING TTI (TIME TO IGNITION)
					if (!keyRel) { activeVerb = 16; activeNoun = 34; }
					if (TIG - MET <= 0)
					{
						activeVerb = 16;
						activeNoun = 80;
						progStep++;
					}
					break;
				case 13:
					activeProg = -1;
					progStep = 0;
					break;
			}
		}

		private void runVerb(int verb, int noun)
		{
			switch (verb)
			{
				case 16: // Monitor Data
					verb16(noun);
					break;					
				case 21: // Load R1
					verb21(noun);
					break;
				case 26: // Load R1,R2,R3
					verb26(noun);
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
				default:
					oprError = true;
					this.verb = verb.ToString().Length < 2 ? "0" + verb.ToString(): verb.ToString();
					this.noun = noun.ToString().Length < 2 ? "0" + noun.ToString(): noun.ToString();
					activeVerb = -1;
					activeNoun = -1;
					break;
			}
		}

		

		private void verb16(int noun)
		{
			// Noun data
			int?[] nounData = getNounData(noun);

			if (nounData[0] != null)
			{
				screenSegDisps[0].setValue(Math.Abs((int)nounData[0]).ToString(), (int)nounData[3], nounData[0] < 0 ? SegDisp.SignState.MINUS: SegDisp.SignState.PLUS);

				if (nounData[1] != null)
				{
					screenSegDisps[1].setValue(Math.Abs((int)nounData[1]).ToString(), (int)nounData[4], nounData[1] < 0 ? SegDisp.SignState.MINUS : SegDisp.SignState.PLUS);
				}

				if (nounData[2] != null)
				{
					screenSegDisps[2].setValue(Math.Abs((int)nounData[2]).ToString(), (int)nounData[5], nounData[2] < 0 ? SegDisp.SignState.MINUS : SegDisp.SignState.PLUS);
				}
			}
			else
			{
				oprError = true;
			}
		}

		// Load decimal 1 in R1
		private void verb21(int noun)
		{
			switch (progStep)
			{
				case 0:
					dataStorage.Clear();
					r1 = " ";
					r2 = " ";
					r3 = " ";
					r1precision = 0;
					r2precision = 0;
					r3precision = 0;
					r1sign = SegDisp.SignState.NONE;
					r2sign = SegDisp.SignState.NONE;
					r3sign = SegDisp.SignState.NONE;
					flashing = true;
					entrPress = false;
					progStep++;
					enterR1 = true;
					break;
				case 1:
					if (entrPress && r1.Length == 5)
					{
						if (r1sign == SegDisp.SignState.MINUS)
						{
							dataStorage.Add(int.Parse(r1) * -1);
						}
						else
						{
							dataStorage.Add(int.Parse(r1));
						}
						entrPress = false;
						progStep++;
					}
					else if (entrPress)
					{
						entrPress = false;
					}
					break;
				case 2:
					// STORE DATA IN NOUN AND CLEAR INPUTS
					setNounData(noun, dataStorage);
					r1 = "";
					r2 = "";
					r3 = "";
					blankRegisters();
					activeVerb = -1;
					activeNoun = -1;
					progStep = 0;
					break;
			}
		}

		// Load Decimal 1,2,3 in R1,R2,R3
		private void verb26(int noun)
		{
			switch (progStep)
			{
				case 0:
					dataStorage.Clear();
					r1 = " ";
					r2 = " ";
					r3 = " ";
					r1precision = 0;
					r2precision = 0;
					r3precision = 0;
					r1sign = SegDisp.SignState.NONE;
					r2sign = SegDisp.SignState.NONE;
					r3sign = SegDisp.SignState.NONE;
					flashing = true;
					entrPress = false;
					progStep++;
					enterR1 = true;
					break;
				case 1:
					if (entrPress && r1.Length == 5)
					{
						if (r1sign == SegDisp.SignState.MINUS)
						{
							dataStorage.Add(int.Parse(r1) * -1);
						}
						else
						{
							dataStorage.Add(int.Parse(r1));
						}
						entrPress = false;
						progStep++;
						enterR2 = true;
					}
					else if (entrPress)
					{
						entrPress = false;
					}
					break;
				case 2:
					if (entrPress && r2.Length == 5)
					{
						if (r2sign == SegDisp.SignState.MINUS)
						{
							dataStorage.Add(int.Parse(r2) * -1);
						}
						else
						{
							dataStorage.Add(int.Parse(r2));
						}
						entrPress = false;
						progStep++;
						enterR3 = true;
					}
					else if (entrPress)
					{
						entrPress = false;
					}
					break;
				case 3:
					if (entrPress && r3.Length == 5)
					{
						if (r3sign == SegDisp.SignState.MINUS)
						{
							dataStorage.Add(int.Parse(r3) * -1);
						}
						else
						{
							dataStorage.Add(int.Parse(r3));
						}
						entrPress = false;
						progStep++;
					}
					else if (entrPress)
					{
						entrPress = false;
					}
					break;
				case 4:
					// STORE DATA IN NOUN AND CLEAR INPUTS
					setNounData(noun, dataStorage);
					r1 = "";
					r2 = "";
					r3 = "";
					blankRegisters();
					activeVerb = -1;
					activeNoun = -1;
					progStep = 0;
					break;
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
			progStep = 0;
			dataStorage.Clear();
			dataStorage.TrimExcess();
			activeVerb = -1;
			activeNoun = -1;
		}

		private void verb69()
		{
			// Clear any register data
			this.r1 = "";
			this.r2 = "";
			this.r3 = "";
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

		private void blankRegisters()
		{
			screenSegDisps[0].setValue("", 0, SegDisp.SignState.NONE);
			screenSegDisps[1].setValue("", 0, SegDisp.SignState.NONE);
			screenSegDisps[2].setValue("", 0, SegDisp.SignState.NONE);
		}

		private void setNounData(int noun, List<int> data)
		{
			switch (noun)
			{
				case 20: // FDAI Offset angles
					screenFDAI.offsetR = data[0] / 100f;
					screenFDAI.offsetP = data[1] / 100f;
					screenFDAI.offsetY = data[2] / 100f;
					break;
					
				case 29: // Launch Azimuth
					launchAzimuth = data[0] / 100f;
					break;
			}
			
		}

		private int?[] getNounData(int noun)
		{
			// Shared Properties
			double secs;
			int mins;
			int hrs;
			
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
				case 17: // Surface Reference Attitude
					values[0] = (int)Math.Round(roll * 100);
					values[1] = (int)Math.Round(pitch * 100);
					values[2] = (int)Math.Round(yaw * 100);
					values[3] = values[4] = values[5] = 2;
					break;
					
				case 18: // Inertial Reference Attitude
					values[0] = (int)Math.Round(inerRoll * 100);
					values[1] = (int)Math.Round(inerPitch * 100);
					values[2] = (int)Math.Round(inerYaw * 100);
					values[3] = values[4] = values[5] = 2;
					break;
					
				case 19: // FDAI VIEW ANGLES
					values[0] = (int)Math.Round((screenFDAI.offsetR + screenFDAI.roll) * 100);
					values[1] = (int)Math.Round((screenFDAI.offsetP + screenFDAI.pitch) * 100);
					values[2] = (int)Math.Round((screenFDAI.offsetY + screenFDAI.yaw) * 100);
					values[3] = values[4] = values[5] = 2;
					break;
					
				case 20: // FDAI angles
					values[0] = (int)Math.Round(screenFDAI.offsetR * 100);
					values[1] = (int)Math.Round(screenFDAI.offsetP * 100);
					values[2] = (int)Math.Round(screenFDAI.offsetY * 100);
					values[3] = values[4] = values[5] = 2;
					break;
					
					
				case 21: // Autopilot Direction vector
					values[0] = (int)Math.Round(vector1 * 100);
					values[1] = (int)Math.Round(vector2 * 100);
					values[2] = (int)Math.Round(vector3 * 100);
					values[3] = values[4] = values[5] = 2;
					break;
					
					
				case 22: // Vessel SURF-mode Direction vector
					values[0] = (int)Math.Round(vesselSurfDirection.Item1 * 100);
					values[1] = (int)Math.Round(vesselSurfDirection.Item2 * 100);
					values[2] = (int)Math.Round(vesselSurfDirection.Item3 * 100);
					values[3] = values[4] = values[5] = 2;
					break;
					
					
				case 23: // Autopilot target RPY
					values[0] = (int)Math.Round(tRoll * 100);
					values[1] = (int)Math.Round(tPitch * 100);
					values[2] = (int)Math.Round(tYaw * 100);
					values[3] = values[4] = values[5] = 2;
					break;
					
				case 29: // Launch Azimuth
					values[0] = (int)Math.Round(launchAzimuth * 100);
					values[1] = null;
					values[2] = null;
					values[3] = 2;
					values[4] = values[5] = 0;
					break;
					
				case 34: // TIG (Time of ignition of event)
					secs = TIG;
					mins = (int)Math.Floor(secs / 60f);
					secs = secs - (mins * 60);
					hrs = (int)Math.Floor(mins / 60f);
					mins = mins - (hrs * 60);
					
					values[0] = hrs;
					values[1] = mins;
					values[2] = (int)Math.Round(secs * 100);

					values[3] = 0;
					values[4] = 0;
					values[5] = 2;
					break;
					
				case 35: // TTI (Time to ignition of event)
					secs = TIG - MET;
					mins = (int)Math.Floor(secs / 60f);
					secs = secs - (mins * 60);
					hrs = (int)Math.Floor(mins / 60f);
					mins = mins - (hrs * 60);
					
					values[0] = hrs;
					values[1] = mins;
					values[2] = (int)Math.Round(secs * 100);

					values[3] = 0;
					values[4] = 0;
					values[5] = 2;
					break;
					
				case 36: // MET
					secs = MET;
					mins = (int)Math.Floor(secs / 60f);
					secs = secs - (mins * 60);
					hrs = (int)Math.Floor(mins / 60f);
					mins = mins - (hrs * 60);
					
					values[0] = hrs;
					values[1] = mins;
					values[2] = (int)Math.Round(secs * 100);

					values[3] = 0;
					values[4] = 0;
					values[5] = 2;
					break;
					
				case 44: // Apoapsis and Periapsis
					values[0] = (int)Math.Round(apoapsis / 10);
					values[1] = (int)Math.Round(periapsis / 10);
					values[2] = (int)Math.Round(meanAltitude / 10);;
					values[3] = values[4] = values[5] = 2;
					break;
				case 73: // Flight Data
					values[0] = (int)Math.Round(meanAltitude / 10);
					values[1] = (int)Math.Round(orbitSpeed * 10);
					values[2] = (int)Math.Round(pitch * 100);

					values[3] = 2;
					values[4] = 1;
					values[5] = 2;
					break;
					
				case 80: // Burn data
					secs = burnTime;
					mins = (int)Math.Floor(secs / 60f);
					secs = secs - (mins * 60);
					values[0] = (int)Math.Round(deltaV * 10);
					values[1] = mins;
					values[2] = (int)Math.Round(secs * 100);

					values[3] = 1;
					values[4] = 0;
					values[5] = 2;
					break;
				default:
					oprError = true;
					this.verb = activeVerb.ToString().Length < 2 ? "0" + activeVerb.ToString(): activeVerb.ToString();
					this.noun = activeNoun.ToString().Length < 2 ? "0" + activeNoun.ToString(): activeNoun.ToString();
					activeVerb = -1;
					activeNoun = -1;
					break;
			}
			return values;
		}
	}
}
