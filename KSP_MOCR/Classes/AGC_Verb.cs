using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	partial class AGC
	{
		private void runVerb()
		{
			switch (verb)
			{
				case "06":
					verb06();
					break;
				case "16":
					verb16();
					break;
				case "26":
					verb26();
					break;
				case "36":
					verb36();
					break;
				case "37":
					verb37();
					break;
			}
		}



		private void verb06()
		{
			// Display Decimal in R1 or in R1,R2 or in R1,R2,R3
			setR1R2R3(getNoun(noun));

			// Since we are not updating this value, clear verb, but
			// do not clear verb display. Also set state to expect new ENTR.
			// Store the verb in pendingVerb, awaiting ENTR.
			pendingVerb = verb;
			verb = "";
			agcInputState = inputState.ENTR;
		}

		private void verb16()
		{
			// Monitor Decimal in R1 or in R1,R2 or in R1,R2,R3
			if (agcInputState == inputState.NOT_RECEIVING)
			{
				setR1R2R3(getNoun(noun));
			}
			else
			{
				dataStorage.setData("AGC_KEYREL", "SET");

				// STORE THE VERB AND NOUN FOR RESUME
				verbStore = verb;
				nounStore = noun;
			}
		}

		private void verb26()
		{
			// LOAD DECIMAL 1,2,3 INTO R1,R2,R3

			// Check that noun is set
			if (noun != "")
			{
				switch (agcInputState)
				{
					case inputState.NOT_RECEIVING: // First stage
						dataStorage.setData("AGC_VFLSH", "SET");
						dataStorage.setData("AGC_NFLSH", "SET");
						dataLoading = true;
						setR1R2R3("", "", "");
						agcInputState = inputState.R1D1;
						break;
					case inputState.LOADED:
						storeNoun();
						dataLoading = false;
						dataStorage.setData("AGC_VFLSH", "");
						dataStorage.setData("AGC_NFLSH", "");
						noun = "";
						verb = "";
						pendingR1 = "";
						pendingR2 = "";
						pendingR3 = "";
						pendingVerb = "";
						pendingNoun = "";
						setVD("  ");
						setND("  ");
						setR(1, "", "0", "");
						setR(2, "", "0", "");
						setR(3, "", "0", "");
						agcInputState = inputState.NOT_RECEIVING;
						break;
				}
			}
		}

		private void verb36()
		{
			// CLEAR THE DSKY
			// This will clear current verb and noun

			verb = "";
			noun = "";
			pendingVerb = "";
			pendingNoun = "";
			setVD("  ");
			setND("  ");
			setR1R2R3("", "", "");

			// Clear any KEY-REL, alarm
			dataStorage.setData("AGC_KEYREL", "");

			// STOP ANY VERB/NOUN/PROG FLASH
			dataStorage.setData("ATC_VFLSH", "");
			dataStorage.setData("ATC_NFLSH", "");
			dataStorage.setData("ATC_PFLSH", "");
		}

		private void verb37()
		{
			// REQUEST PROGRAM CHANGE
		}
	}
}
