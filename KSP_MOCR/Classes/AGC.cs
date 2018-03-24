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
	class AGC
	{
		Thread thread;
		bool running = false;
		Thread AGCThread;
		DateTime updateStart;
		DateTime updateEnd;

		DataStorage dataStorage;
		StreamCollection streams;

		string prog = "00";
		string verb;
		string noun;

		string keyInterrupt = "";

		public AGC(DataStorage ds, StreamCollection streamCollection)
		{
			dataStorage = ds;
			streams = streamCollection;
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

					prog = dataStorage.getData("AGC_MD1") + dataStorage.getData("AGC_MD2");
					runProgram();

					if (keyInterrupt == "")
					{
						verb = dataStorage.getData("AGC_VD1") + dataStorage.getData("AGC_VD2");
						noun = dataStorage.getData("AGC_ND1") + dataStorage.getData("AGC_ND2");
						runVerb();
					}
					else
					{
						dataStorage.setData("AGC_KEYREL", "SET");
					}

					updateEnd = DateTime.Now;

					TimeSpan updateDuration = updateEnd - updateStart;
					int remainTime = 100 - (int)Math.Ceiling(updateDuration.TotalMilliseconds);

					Thread.Sleep(remainTime);
				}
				
			}
			catch(ThreadAbortException)
			{
				Console.WriteLine("AGC ABORTED");
			}
		}

		private void runProgram()
		{
			//Console.WriteLine("RUNNING PROGRAM: " + p.ToString());

			// Check for new program request
			string R00 = dataStorage.getData("AGC_R00");
			if (R00 != "")
			{
				// Set program and clear R00
				prog = R00;
				dataStorage.setData("AGC_R00", "");
				clearDSKY();
				dataStorage.setData("AGC_MD1", R00.Substring(0, 1));
				dataStorage.setData("AGC_MD2", R00.Substring(1, 1));
			}

			switch (prog)
			{
				case "00": // IDLE PROGRAM
					
					break;
			}
		}

		private void runVerb()
		{
			switch(verb)
			{
				case "16":
					verb16();
					break;
			}
		}

		private void verb16()
		{
			switch(noun)
			{
				case "36":
					// DISPLAY MET
					if (streams != null)
					{
						double met = streams.GetData(DataType.vessel_MET);
						double hrs = Math.Floor(met / 3600f);
						double min = Math.Floor((met - (hrs * 3600)) / 60);
						double sec = Math.Floor(met - (hrs * 3600) - (min * 60));
						setR1R2R3(hrs.ToString(), min.ToString(), sec.ToString());
					}
					break;
			}
		}

		private void clearDSKY()
		{
			dataStorage.setData("AGC_MD1", " ");
			dataStorage.setData("AGC_MD2", " ");
			dataStorage.setData("AGC_VD1", " ");
			dataStorage.setData("AGC_VD2", " ");
			dataStorage.setData("AGC_ND1", " ");
			dataStorage.setData("AGC_ND2", " ");

			dataStorage.setData("AGC_R1D1", " ");
			dataStorage.setData("AGC_R1D2", " ");
			dataStorage.setData("AGC_R1D3", " ");
			dataStorage.setData("AGC_R1D4", " ");
			dataStorage.setData("AGC_R1D5", " ");

			dataStorage.setData("AGC_R2D1", " ");
			dataStorage.setData("AGC_R2D2", " ");
			dataStorage.setData("AGC_R2D3", " ");
			dataStorage.setData("AGC_R2D4", " ");
			dataStorage.setData("AGC_R2D5", " ");

			dataStorage.setData("AGC_R3D1", " ");
			dataStorage.setData("AGC_R3D2", " ");
			dataStorage.setData("AGC_R3D3", " ");
			dataStorage.setData("AGC_R3D4", " ");
			dataStorage.setData("AGC_R3D5", " ");
		}

		private void setR1R2R3(string s1, string s2, string s3)
		{
			setR(s1, 1);
			setR(s2, 2);
			setR(s3, 3);
		}

		private void setR(string s, int r)
		{
			int max = s.Length;
			if (max > 5) max = 5;

			for(int i = max; i > 0; i--)
			{
				dataStorage.setData("AGC_R" + r.ToString() + "D" + i.ToString(), s.Substring(i - 1, 1));
			}
		}

	}
}
