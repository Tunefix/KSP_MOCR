using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	partial class AGC
	{
		private void runKeyInterrupt(string key)
		{
			switch(key)
			{
				case "ENTR":
					processEnterKey();
					break;
				case "VERB":
					processVerbKey();
					break;
				case "NOUN":
					processNounKey();
					break;
				case "KEYREL":
					processKeyRel();
					break;
				case "0":
				case "1":
				case "2":
				case "3":
				case "4":
				case "5":
				case "6":
				case "7":
				case "8":
				case "9":
					processNumKey(key);
					break;
				case "RSET":
					processRset();
					break;
				case "MINUS":
					processMinus();
					break;
				default:
					//ignore
					break;
			}

			// STORE KEYINPUT FOR MONITORING
			dataStorage.setData("AGC_SKEY", key);

			// Clear KEYRUPT
			dataStorage.setData("AGC_KEY", "");
		}

		private void processEnterKey()
		{
			bool processed = false;

			if (dataLoading)
			{
				switch(agcInputState)
				{
					case inputState.LOADR1:
						agcInputState = inputState.R2D1;
						break;
					case inputState.LOADR2:
						agcInputState = inputState.R3D1;
						break;
					case inputState.LOADR3:
						agcInputState = inputState.LOADED;
						break;
				}
			}
			else
			{
				if (pendingVerb != "" && agcInputState == inputState.ENTR)
				{
					// Set pending verb active and blank pendingVerb
					verb = pendingVerb;
					pendingVerb = "";
					processed = true;

					resetKeyRel();
				}

				if (pendingNoun != "" && agcInputState == inputState.ENTR)
				{
					// Set pending noun active and blank pendingNoun
					noun = pendingNoun;
					pendingNoun = "";
					processed = true;

					resetKeyRel();
				}
			}

			if (processed) agcInputState = inputState.NOT_RECEIVING;
		}

		private void resetKeyRel()
		{
			// Revert all store, flash and clear KEY REL light
			verbStore = "";
			nounStore = "";

			dataStorage.setData("AGC_VFLSH", "");
			dataStorage.setData("AGC_NFLSH", "");
			dataStorage.setData("AGC_PFLSH", "");
			dataStorage.setData("AGC_KEYREL", "");
		}

		private void processVerbKey()
		{
			// Blank the verb display and pending verb
			setVD1(" ");
			setVD2(" ");
			pendingVerb = "";

			// Get ready to accept verb digit 1
			agcInputState = inputState.VD1;
		}

		private void processNounKey()
		{
			// Blank the noun display and pending noun
			setND1(" ");
			setND2(" ");
			pendingNoun = "";

			// Get ready to accept noun digit 1
			agcInputState = inputState.ND1;
		}

		private void processKeyRel()
		{
			// Revert all pending, release state, and clear KEY REL light
			pendingNoun = "";
			pendingVerb = "";
			pendingProg = "";

			if(verbStore != "")
			{
				verb = verbStore;
				verbStore = "";
			}

			if (nounStore != "")
			{
				noun = nounStore;
				nounStore = "";
			}

			setVD(verb);
			setND(noun);
			setMD(prog);

			dataStorage.setData("AGC_VFLSH", "");
			dataStorage.setData("AGC_NFLSH", "");
			dataStorage.setData("AGC_PFLSH", "");


			agcInputState = inputState.NOT_RECEIVING;
			dataStorage.setData("AGC_KEYREL", "");
		}

		private void processRset()
		{
			// CLEAR ALL ALARMS
			
			dataStorage.setData("AGC_NOATT", "");
			dataStorage.setData("AGC_STBY", "");
			dataStorage.setData("AGC_KEYREL", "");
			dataStorage.setData("AGC_OPRERR", "");

			dataStorage.setData("AGC_TEMP", "");
			dataStorage.setData("AGC_GIMB", "");
			dataStorage.setData("AGC_PROGAL", "");
			dataStorage.setData("AGC_RESTRT", "");
			dataStorage.setData("AGC_TRACKR", "");
			dataStorage.setData("AGC_ALTAL", "");
			dataStorage.setData("AGC_VELAL", "");
		}

		private void processMinus()
		{
			switch (agcInputState)
			{
				case inputState.R1D1:
					pR1neg = "NEG";
					dataStorage.setData("AGC_R1S", "NEG");
					break;
				case inputState.R2D1:
					pR2neg = "NEG";
					dataStorage.setData("AGC_R2S", "NEG");
					break;
				case inputState.R3D1:
					pR3neg = "NEG";
					dataStorage.setData("AGC_R3S", "NEG");
					break;
			}
		}

		private void processNumKey(string key)
		{
			switch(agcInputState)
			{
				case inputState.VD1:
					pendingVerb += key;
					setVD1(key);
					agcInputState = inputState.VD2;
					break;
				case inputState.VD2:
					pendingVerb += key;
					setVD2(key);
					agcInputState = inputState.ENTR;
					break;
				case inputState.ND1:
					pendingNoun += key;
					setND1(key);
					agcInputState = inputState.ND2;
					break;
				case inputState.ND2:
					pendingNoun += key;
					setND2(key);
					agcInputState = inputState.ENTR;
					break;
				case inputState.R1D1:
					pendingR1 += key;
					setRD(key, 1, 1);
					agcInputState = inputState.R1D2;
					break;
				case inputState.R1D2:
					pendingR1 += key;
					setRD(key, 1, 2);
					agcInputState = inputState.R1D3;
					break;
				case inputState.R1D3:
					pendingR1 += key;
					setRD(key, 1, 3);
					agcInputState = inputState.R1D4;
					break;
				case inputState.R1D4:
					pendingR1 += key;
					setRD(key, 1, 4);
					agcInputState = inputState.R1D5;
					break;
				case inputState.R1D5:
					pendingR1 += key;
					setRD(key, 1, 5);
					agcInputState = inputState.LOADR1;
					break;
				case inputState.R2D1:
					pendingR2 += key;
					setRD(key, 2, 1);
					agcInputState = inputState.R2D2;
					break;
				case inputState.R2D2:
					pendingR2 += key;
					setRD(key, 2, 2);
					agcInputState = inputState.R2D3;
					break;
				case inputState.R2D3:
					pendingR2 += key;
					setRD(key, 2, 3);
					agcInputState = inputState.R2D4;
					break;
				case inputState.R2D4:
					pendingR2 += key;
					setRD(key, 2, 4);
					agcInputState = inputState.R2D5;
					break;
				case inputState.R2D5:
					pendingR2 += key;
					setRD(key, 2, 5);
					agcInputState = inputState.LOADR2;
					break;
				case inputState.R3D1:
					pendingR3 += key;
					setRD(key, 3, 1);
					agcInputState = inputState.R3D2;
					break;
				case inputState.R3D2:
					pendingR3 += key;
					setRD(key, 3, 2);
					agcInputState = inputState.R3D3;
					break;
				case inputState.R3D3:
					pendingR3 += key;
					setRD(key, 3, 3);
					agcInputState = inputState.R3D4;
					break;
				case inputState.R3D4:
					pendingR3 += key;
					setRD(key, 3, 4);
					agcInputState = inputState.R3D5;
					break;
				case inputState.R3D5:
					pendingR3 += key;
					setRD(key, 3, 5);
					agcInputState = inputState.LOADR3;
					break;
			}
		}
	}
}
