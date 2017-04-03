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
	class TestScreen : MocrScreen
	{

		KRPC.Schema.KRPC.Status status;
		private KRPC.Client.Services.SpaceCenter.Flight flight;

		public TestScreen(Form1 form)
		{
			this.form = form;

			this.connection = form.connection;
			this.krpc = this.connection.KRPC();
			this.spaceCenter = this.connection.SpaceCenter();

			this.width = 120;
			this.height = 30;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			if (form.connected && connection != null)
			{
			}

			if (form.connected && krpc.CurrentGameScene == GameScene.Flight)
			{

				// GET DATA
				flight = GetData.getFlight();
				screenLabels[1].Text = flight.MeanAltitude.ToString();
			}
		}


		public override void makeElements()
		{
			for (int i = 0; i < 5; i++) screenLabels.Add(null); // Initialize Labels

			screenLabels[0] = Helper.CreateLabel(1, 4, 58, 26); // Connection Status
			screenLabels[0].Text = "NOT CONNECTED";
			screenLabels[0].TextAlign = ContentAlignment.TopLeft;

			screenLabels[1] = Helper.CreateLabel(61, 4, 58, 26); // TEST

			screenLabels[4] = Helper.CreateLabel(0, 0, 60, 2, "──────────────────── CONNECTION DETAILS ────────────────────"); // Connection Header}
		}

		public override void destroyStreams()
		{
			
		}
	}
}
