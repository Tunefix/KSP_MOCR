using KRPC.Client;
using KRPC.Client.Services;
using KRPC.Client.Services.KRPC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class TestScreen : MocrScreen
	{

		//KRPC.Schema.KRPC.Status status;
		//KRPC.Client.Stream<KRPC.Schema.KRPC.Status> stream;

		public TestScreen(Form1 form)
		{
			this.form = form;
			this.help = new KSP_MOCR.helper(form);

			this.width = 120;
			this.height = 30;

			//stream = connection.AddStream(() => krpc.GetStatus());
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			if (form.connected && connection == null)
			{
				//this.connection = form.connection;
				//this.krpc = this.connection.KRPC();
			}
		}


		public override void makeElements()
		{
			for (int i = 0; i < 5; i++) screenLabels.Add(null); // Initialize Labels

			screenLabels[0] = help.CreateLabel(1, 4, 58, 26); // Connection Status
			screenLabels[0].Text = "NOT CONNECTED";
			screenLabels[0].TextAlign = ContentAlignment.TopLeft;

			screenLabels[1] = help.CreateLabel(61, 4, 58, 26); // TEST

			screenLabels[4] = help.CreateLabel(0, 0, 60, 2, "──────────────────── CONNECTION DETAILS ────────────────────"); // Connection Header}
		}

		public override void destroyStreams()
		{
			
		}
	}
}
