using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KSP_MOCR
{ 
	/// <summary>
	/// This is a custom, minimal iplementation of an agc computer.
	/// Run it only on one instance of KSP_MOCR, as it will be shared
	/// between all running KSP_MOCR programs via PySSSMQ.
	/// 
	/// This also means that any (or all) DKSYs will interact against
	/// PySSSMQ, not the AGC directly.
	/// </summary>
	partial class AGC
	{
		Thread thread;
		bool running = false;
		Thread AGCThread;
		DateTime updateStart;
		DateTime updateEnd;

		DataStorage dataStorage;
		StreamCollection streams;
		Form1 form;

		// MAIN LOOP VARIABLES
		string[] nounlessVerbs = { "36", "37" };
		bool dataLoading = false;

		// PINBALL VARIABLES
		enum inputState { NOT_RECEIVING = 0, ENTR,
			LOADR1, LOADR2, LOADR3, LOADED,
			VD1, VD2, ND1, ND2, MD1, MD2,
			R1D1, R1D2, R1D3, R1D4, R1D5,
			R2D1, R2D2, R2D3, R2D4, R2D5,
			R3D1, R3D2, R3D3, R3D4, R3D5 }
		inputState agcInputState = inputState.NOT_RECEIVING;
		string pendingVerb = "";
		string pendingNoun = "";
		string pendingProg = "";
		string pendingR1 = "";
		string pendingR2 = "";
		string pendingR3 = "";
		string pR1neg = ""; // Pending R1 is negative (Minus-key pressed before first digit)
		string pR2neg = "";
		string pR3neg = "";

		string prog = "  ";
		string verb = "  ";
		string noun = "  ";

		string verbStore = "";
		string nounStore = "";

		string keyInterrupt = "";

		public AGC(DataStorage ds, StreamCollection streamCollection, Form1 f)
		{
			dataStorage = ds;
			streams = streamCollection;
			form = f;
		}

		public bool isRunning()
		{
			return running;
		}

		public void Start()
		{
			running = true;
			AGCThread = new Thread(runAGC);
			AGCThread.Start();
		}

		public void Stop()
		{
			running = false;
			AGCThread.Abort();
		}

		private void runAGC()
		{
			try
			{
				while (true)
				{
					updateStart = DateTime.Now;

					// Check for key interrupt
					keyInterrupt = dataStorage.getData("AGC_KEY");
					if(keyInterrupt != "")
					{
						//Console.WriteLine("KRUPT");
						runKeyInterrupt(keyInterrupt);
					}

					if((verb != "" && noun != "") || nounlessVerbs.Contains(verb))
					{
						runVerb();
					}

					runProgram();

					// UPDATE PySSSMQ WITH CURRENT VNP
					dataStorage.setData("AGC_PROG", prog);
					dataStorage.setData("AGC_VERB", verb);
					dataStorage.setData("AGC_NOUN", noun);

					dataStorage.setData("AGC_STATE", agcInputState.ToString());

					dataStorage.setData("AGC_PROGp", pendingProg);
					dataStorage.setData("AGC_VERBp", pendingVerb);
					dataStorage.setData("AGC_NOUNp", pendingNoun);

					dataStorage.setData("AGC_R1p", pendingR1);
					dataStorage.setData("AGC_R2p", pendingR2);
					dataStorage.setData("AGC_R3p", pendingR3);


					updateEnd = DateTime.Now;

					TimeSpan updateDuration = updateEnd - updateStart;
					
					int remainTime = 10 - (int)Math.Ceiling(updateDuration.TotalMilliseconds);
					if (remainTime < 0)
					{
						form.writeToLog("AGC OUT OF TIME: " + remainTime.ToString());
						form.writeToLog("verb: " + verb.ToString());
						form.writeToLog("noun: " + noun.ToString());
						remainTime = 0;
						
					}
					Thread.Sleep(remainTime);
				}
				
			}
			catch(ThreadAbortException)
			{
				form.writeToLog("AGC ABORTED");
			}
			catch(Exception ex)
			{
				form.writeToLog("AGC ECEPTION:");
				form.writeToLog(ex.GetType().ToString() + ":" + ex.Message + "\n" + ex.StackTrace);
			}
		}

		/*
		 * The following are general helper functions.
		 * All other functions of the AGC is in separate files
		 */

		private void clearDSKY()
		{
			setMD1(" ");
			setMD2(" ");
			setVD1(" ");
			setVD2(" ");
			setND1(" ");
			setND2(" ");

			setR(1, "", "0", "");
			setR(2, "", "0", "");
			setR(3, "", "0", "");
		}

		private void setMD(string md) { setMD1MD2(md.Substring(0, 1), md.Substring(1, 1)); }
		private void setMD1(string md1) { dataStorage.setData("AGC_MD1", md1); }
		private void setMD2(string md2) { dataStorage.setData("AGC_MD2", md2); }
		private void setMD1MD2(string md1, string md2)
		{
			dataStorage.setData("AGC_MD1", md1);
			dataStorage.setData("AGC_MD2", md2);
		}

		private void setVD(string vd) { setVD1VD2(vd.Substring(0, 1), vd.Substring(1, 1)); }
		private void setVD1(string vd1) { dataStorage.setData("AGC_VD1", vd1); }
		private void setVD2(string vd2) { dataStorage.setData("AGC_VD2", vd2); }
		private void setVD1VD2(string vd1, string vd2)
		{
			dataStorage.setData("AGC_VD1", vd1);
			dataStorage.setData("AGC_VD2", vd2);
		}

		private void setND(string nd) { setND1ND2(nd.Substring(0, 1), nd.Substring(1, 1)); }
		private void setND1(string nd1) { dataStorage.setData("AGC_ND1", nd1); }
		private void setND2(string nd2) { dataStorage.setData("AGC_ND2", nd2); }
		private void setND1ND2(string nd1, string nd2)
		{
			dataStorage.setData("AGC_ND1", nd1);
			dataStorage.setData("AGC_ND2", nd2);
		}

		private void setRD(string s, int r, int d)
		{
			dataStorage.setData("AGC_R" + r.ToString() + "D" + d.ToString(), s);
		}

		private void setR1R2R3(string s1, string s2, string s3)
		{
			setR(1, s1);
			setR(2, s2);
			setR(3, s3);
		}

		private void setR1R2R3(Noun n)
		{
			setR(1, n.R1, n.R1P, n.R1S);
			setR(2, n.R2, n.R2P, n.R2S);
			setR(3, n.R3, n.R3P, n.R3S);
		}

		private void setR(int r, string value) { setR(r, value, "0", "");  }
		private void setR(int r, string value, string p, string s)
		{
			int max = value.Length;
			if (max > 5) max = 5;
			int skip = 0;
			if (max < 5) skip = 5 - max;

			for(int i = 1; i <= 5; i++)
			{
				if (skip >= i)
				{
					dataStorage.setData("AGC_R" + r.ToString() + "D" + i.ToString(), " ");
				}
				else
				{
					dataStorage.setData("AGC_R" + r.ToString() + "D" + i.ToString(), value.Substring(i - 1 - skip, 1));
				}
			}

			// SET PRECISION AND SIGN
			dataStorage.setData("AGC_R" + r.ToString() + "P", p);
			dataStorage.setData("AGC_R" + r.ToString() + "S", s);
		}

		private void setRneg(int R, bool neg)
		{
			if(neg)
			{
				dataStorage.setData("AGC_R" + R.ToString() + "S", "NEG");
			}
			else
			{
				dataStorage.setData("AGC_R" + R.ToString() + "S", "POS");
			}
		}

	}
}
