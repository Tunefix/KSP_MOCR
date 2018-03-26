using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class Noun
	{
		public string R1 { get; set; }
		public string R2 { get; set; }
		public string R3 { get; set; }

		public string R1P { get; set; }
		public string R2P { get; set; }
		public string R3P { get; set; }

		public string R1S { get; set; }
		public string R2S { get; set; }
		public string R3S { get; set; }

		public Noun(string r1, string r1p, string r1s, string r2, string r2p, string r2s, string r3, string r3p, string r3s)
		{
			R1 = r1;
			R2 = r2;
			R3 = r3;

			R1P = r1p;
			R2P = r2p;
			R3P = r3p;

			R1S = r1s;
			R2S = r2s;
			R3S = r3s;
		}

		public Noun(string r1, string r2, string r3)
		{
			R1 = r1;
			R2 = r2;
			R3 = r3;

			R1P = "0";
			R2P = "0";
			R3P = "0";

			R1S = "";
			R2S = "";
			R3S = "";
		}

		public Noun(string r1, string r1p, string r2, string r2p, string r3, string r3p)
		{
			R1 = r1;
			R2 = r2;
			R3 = r3;

			R1P = r1p;
			R2P = r2p;
			R3P = r3p;

			R1S = "";
			R2S = "";
			R3S = "";
		}
	}
}
