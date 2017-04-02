using KRPC.Client;
using KRPC.Client.Services.KRPC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class ConnectionScreen : MocrScreen
	{
		KRPC.Schema.KRPC.Status status;
		KRPC.Client.Stream<KRPC.Schema.KRPC.Status> status_stream;

		public ConnectionScreen(Form1 form)
		{
			this.form = form;
			this.help = new KSP_MOCR.helper(form);

			this.width = 120;
			this.height = 30;
		}

		public override void destroyStreams()
		{

		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			if(form.connected && this.connection == null)
			{
				this.connection = form.connection;
				this.krpc = form.krpc;

				screenButtons[0].Text = "Disconnect";
				screenButtons[0].Click -= form.ConnectToServer;
				screenButtons[0].Click += form.DisconnectFromServer;
			}

			if (form.connected && this.connection != null)
			{

				//if (status_stream == null) status_stream = connection.AddStream(() => krpc.GetStatus());

				//Console.WriteLine("Will now try to get status");
				status = krpc.GetStatus();   //status_stream.Get();

				//Console.WriteLine("Status Gotten");

				screenLabels[0].Text = "CONNECTED\n\n"
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
				//Console.WriteLine("Label updated");
			}
		}


		public override void makeElements()
		{
			for (int i = 0; i < 5; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 2; i++) screenInputs.Add(null); // Initialize Inputs
			for (int i = 0; i < 1; i++) screenButtons.Add(null); // Initialize Buttons

			screenLabels[0] = help.CreateLabel(1, 4, 58, 26); // Connection Status
			screenLabels[0].Text = "NOT CONNECTED";
			screenLabels[0].TextAlign = ContentAlignment.TopLeft;

			screenLabels[1] = help.CreateLabel(1, 1, 20, 1, "Server IP:"); // Input Label
			screenInputs[0] = help.CreateInput(1, 2, 20, 1); // Server IP
			screenInputs[0].Text = "127.0.0.1";

			screenLabels[2] = help.CreateLabel(23, 1, 20, 1, "Your ID:"); // Input Label
			screenInputs[1] = help.CreateInput(23, 2, 20, 1); // Name
			screenInputs[1].Text = "Flight Director";

			screenButtons[0] = help.CreateButton(45, 2, 14); // Connect-button
			screenButtons[0].Text = "Connect";
			screenButtons[0].Click += form.ConnectToServer;

			screenLabels[3] = help.CreateLabel(60, 0, 60, 20); // Help text
			screenLabels[3].Text = "╥─── HELP ───\n"
									+ "║\n"
									+ "║  To switch between the different screens,\n"
									+ "║  hold down [CTRL] and type inn the number,\n"
									+ "║  then release [CTRL].\n"
									+ "║\n"
									+ "║     0 - CONNECTION [ESC]\n"
									+ "║     1 - ASCENT\n"
									+ "║     2 - BOOSTER\n"
									+ "║    50 - TERRAIN GRAPH\n"
									+ "║   100 - PILOT\n"
									+ "║\n"
									+ "║\n"
									+ "║\n"
									+ "║\n"
									+ "║\n";
			screenLabels[3].TextAlign = ContentAlignment.TopLeft;

			screenLabels[4] = help.CreateLabel(0, 0, 60, 2, "──────────────────── CONNECTION DETAILS ────────────────────"); // Connection Header}
		}
	}
}
