using KRPC.Client;
using KRPC.Client.Services.KRPC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class ConnectionScreen : MocrScreen
	{

		public ConnectionScreen(Screen form)
		{
			this.form = form;
			this.screenStreams = form.streamCollection;

			this.width = 60;
			this.height = 30;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			

				/*screenLabels[0].Text = "CONNECTED\n\n"
					+ "  VERSION: " + status.Version + "\n"
					+ "  ADAPTIVE RATE CONTROL: " + status.AdaptiveRateControl + "\n"
					+ "  BLOCKING RECEIVE: " + status.BlockingRecv + "\n"
					+ "  BYTES READ: " + status.BytesRead + "\n"
					+ "  BYTES READ RATE: " + status.BytesReadRate + "\n"
					+ "  BYTES WRITTEN: " + status.BytesWritten + "\n"
					+ "  BYTES WRITTEN RATE: " + status.BytesWrittenRate + "\n"
					+ "  TIME PER RPC UPDATE: " + status.ExecTimePerRpcUpdate + "\n"
					+ "  MAX TIME PER UPDATE: " + status.MaxTimePerUpdate + "\n"
					+ "  ONE RPC PER UPDATE: " + status.OneRpcPerUpdate + "\n"
					+ "  POLL TIME PER RPC UPDATE: " + status.PollTimePerRpcUpdate + "\n"
					+ "  RECEIVE TIMEOUT: " + status.RecvTimeout + "\n"
					+ "  RPC RATE: " + status.RpcRate + "\n"
					+ "  RPCS EXECUTED: " + status.RpcsExecuted + "\n"
					+ "  STREAM RPC RATE: " + status.StreamRpcRate + "\n"
					+ "  STREAM RPCS: " + status.StreamRpcs + "\n"
					+ "  STREAM RPCS EXECUTED: " + status.StreamRpcsExecuted + "\n"
					+ "  TIME PER RPC UPDATE: " + status.TimePerRpcUpdate + "\n"
					+ "  TIME PER STREAM UPDATE: " + status.TimePerStreamUpdate + "\n";
					*/
			
		}


		public override void makeElements()
		{
			for (int i = 0; i < 8; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 2; i++) screenButtons.Add(null); // Initialize Buttons

			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix
			
			screenLabels[4] = Helper.CreateLabel(0, 0, 60, 30); // Help text
			screenLabels[4].Text = "╥─── HELP ───\n"
									+ "║\n"
									+ "║  To switch between the different screens,\n"
									+ "║  hold down [CTRL] and type inn the number,\n"
									+ "║  then release [CTRL].\n"
									+ "║\n"
									+ "║     0 - HELP\n"
									+ "║     1 - ASCENT\n"
									+ "║     2 - BOOSTER\n"
									+ "║     3 - RESOURCES\n"
									+ "║     4 - ORBIT VIEW\n"
									+ "║     5 - ORBIT DATA\n"
									+ "║     6 - ELECTRICAL SYSTEMS\n"
									+ "║     8 - MAP VIEW\n"
									+ "║     9 - FONT/ALIGNMENT TEST\n"
									+ "║    12 - DATA STORAGE (PySSSMQ)\n"
									+ "║    50 - TERRAIN GRAPH\n"
									+ "║    51 - ALTITUDE / INERTIAL VELOCITY - GRAPH\n"
									+ "║    52 - ATTITUDE - GRAPHS (R,P,Y)\n"
									+ "║    53 - TIME TO APOPASIS / INERTIAL VELOCITY - GRAPH\n"
									+ "║    54 - ALTITUDE / RANGE - GRAPH\n"
									+ "║    55 - ALTITUDE / VERTICAL SPEED (H / HDOT) - GRAPH\n"
									+ "║    56 - FLIGHT PATH ANGLE / INERTIAL VELOCITY - GRAPH\n"
									+ "║   100 - PILOT\n"
									+ "║\n"
									+ "║   201-215 - STATUS REPORT (Requires PySSSMQ connection)\n"
									+ "║   220 - STATUS PANEL (Requires PySSSMQ connection)\n"
									+ "║\n"
									+ "║\n";

			screenLabels[5] = Helper.CreateLabel(0, 0, 60, 2, "──────────────────── CONNECTION DETAILS ────────────────────"); // Connection Header}

			screenLabels[7].Text = "";
		}

		public override void resize()
		{

		}
	}
}
