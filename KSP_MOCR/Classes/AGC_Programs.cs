using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	partial class AGC
	{
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
	}
}
