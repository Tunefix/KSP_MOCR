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
		KRPC.Schema.KRPC.Status status;

		public ConnectionScreen(Form1 form)
		{
			this.form = form;

			this.width = 120;
			this.height = 30;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			if(form.connected && form.connection == null)
			{
				screenButtons[0].Text = "Disconnect";
				screenButtons[0].Click -= form.ConnectToServer;
				screenButtons[0].Click += form.DisconnectFromServer;
			}

			if (form.connected && form.connection != null)
			{
				status = form.krpc.GetStatus();

				screenLabels[0].Text = "CONNECTED TO kRPC";

				if (form.pySSSMQ.IsConnected())
				{
					screenLabels[1].Text = "CONNECTED TO PySSSMQ";
				}
				else
				{
					screenLabels[1].Text = "NOT CONNECTED TO PySSSMQ";
				}

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
		}


		public override void makeElements()
		{
			for (int i = 0; i < 8; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 2; i++) screenInputs.Add(null); // Initialize Inputs
			for (int i = 0; i < 2; i++) screenButtons.Add(null); // Initialize Buttons

			screenLabels[0] = Helper.CreateLabel(1, 6, 25, 1); // Connection Status
			screenLabels[0].Text = "NOT CONNECTED TO kRPC";
			screenLabels[0].TextAlign = ContentAlignment.TopLeft;
			
			screenLabels[1] = Helper.CreateLabel(1, 7, 25, 1); // Connection Status
			screenLabels[1].Text = "NOT CONNECTED TO PySSSMQ";
			screenLabels[1].TextAlign = ContentAlignment.TopLeft;
			
			screenLabels[7] = Helper.CreateLabel(1, 9, 58, 20); // Connection Status
			screenLabels[7].Text = "";
			screenLabels[7].TextAlign = ContentAlignment.TopLeft;

			screenLabels[2] = Helper.CreateLabel(1, 1, 20, 1, "Server IP:"); // Input Label
			screenInputs[0] = Helper.CreateInput(1, 2, 20, 1); // Server IP
			if (form.connectionIP != "")
			{
				screenInputs[0].Text = form.connectionIP;
			}
			else
			{
				screenInputs[0].Text = "127.0.0.1";
			}
			screenInputs[0].KeyDown += checkForEnter;

			screenLabels[3] = Helper.CreateLabel(23, 1, 20, 1, "Your ID:"); // Input Label
			screenInputs[1] = Helper.CreateInput(23, 2, 20, 1); // Name
			if (form.connectionName != "")
			{
				screenInputs[1].Text = form.connectionName;
			}
			else
			{
				screenInputs[1].Text = "Flight Director";
			}
			screenInputs[1].KeyDown += checkForEnter;

			screenButtons[0] = Helper.CreateButton(45, 2, 14); // Connect-button
			screenButtons[0].Text = "Connect";
			screenButtons[0].Click += form.ConnectToServer;

			screenLabels[4] = Helper.CreateLabel(60, 0, 60, 20); // Help text
			screenLabels[4].Text = "╥─── HELP ───\n"
									+ "║\n"
									+ "║  To switch between the different screens,\n"
									+ "║  hold down [CTRL] and type inn the number,\n"
									+ "║  then release [CTRL].\n"
									+ "║\n"
									+ "║     0 - CONNECTION [ESC]\n"
									+ "║     1 - ASCENT\n"
									+ "║     2 - BOOSTER\n"
									+ "║     3 - RESOURCES\n"
									+ "║     4 - FLIGHT DYNAMICS\n"
									+ "║     8 - MAP VIEW\n"
									+ "║     9 - FONT/ALIGNMENT TEST\n"
									+ "║    50 - TERRAIN GRAPH\n"
									+ "║    51 - ALT/VEL GRAPH\n"
									+ "║    52 - ATTITUDE GRAPHS (R,P,Y)\n"
									+ "║   100 - PILOT\n"
									+ "║\n"
									+ "║   201-215 - STATUS REPORT (Requires PySSSMQ connection)\n"
									+ "║   220 - STATUS PANEL (Requires PySSSMQ connection)\n"
									+ "║\n"
									+ "║\n";
			screenLabels[4].TextAlign = ContentAlignment.TopLeft;

			screenLabels[5] = Helper.CreateLabel(0, 0, 60, 2, "──────────────────── CONNECTION DETAILS ────────────────────"); // Connection Header}

			screenLabels[7].Text = "";
		}

		private void checkForEnter(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter || e.KeyData == Keys.Return)
			{
				form.ConnectToServer(sender, e);
			}
		}
	}
}
