using KRPC.Client;
using KRPC.Client.Services;
using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class StreamsScreen : MocrScreen
	{
		string streams;
		string[] stream;
		string[] output;

		public StreamsScreen(Screen form)
		{
			this.screenStreams = form.streamCollection;
			this.form = form;

			this.width = 120;
			this.height = 30;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			streams = screenStreams.getStatus();
			stream = streams.Split('\n');
			output = new string[6];

			int c = 0;
			int o = 0;
			foreach(string s in stream)
			{
				output[o] += Helper.prtlen(s, 19, Helper.Align.LEFT) + "\n";
				if(c > 24) { c = 0; o++; }
				c++;
			}

			screenLabels[1].Text = output[0];
			screenLabels[2].Text = output[1];
			screenLabels[3].Text = output[2];
			screenLabels[4].Text = output[3];
			screenLabels[5].Text = output[4];
			screenLabels[6].Text = output[5];
		}

		public override void makeElements()
		{
			for (int i = 0; i < 7; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs


			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateLabel(0, 0, 118, 1); // Connection Status
			screenLabels[0].Text = "STREAMS:";

			screenLabels[1] = Helper.CreateLabel(0, 1, 20, 28, "NO DATA");
			screenLabels[2] = Helper.CreateLabel(20, 1, 20, 28);
			screenLabels[3] = Helper.CreateLabel(40, 1, 20, 28);
			screenLabels[4] = Helper.CreateLabel(60, 1, 20, 28);
			screenLabels[5] = Helper.CreateLabel(80, 1, 20, 28);
			screenLabels[6] = Helper.CreateLabel(100, 1, 20, 28);
		}

		public override void resize() { }
	}
}