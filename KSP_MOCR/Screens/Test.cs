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
		KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Flight> flight_stream;

		public TestScreen(Form1 form)
		{
			this.form = form;

			this.width = 120;
			this.height = 30;

			var vessel = this.form.spaceCenter.ActiveVessel;
			Console.WriteLine("Vessel: " + vessel.ToString());
			var refframe = vessel.Orbit.Body.ReferenceFrame;
			Console.WriteLine("RefFrame: " + refframe.ToString());

			Console.WriteLine("Adding stream");
			try
			{
				this.flight_stream = this.form.connection.AddStream(() => vessel.Flight(refframe));
			}
			catch (Exception ex)
			{
				Console.WriteLine("Ex: " + ex.ToString());
			}
			Console.WriteLine("Stream added");
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			if (form.connected && form.connection != null)
			{
			}

			if (form.connected && form.krpc.CurrentGameScene == GameScene.Flight) // krpc.CurrentGameScene is 1 RPC
			{
				// GET DATA
				Console.WriteLine("Getting data");
				flight = flight_stream.Get();
				Console.WriteLine("Got Data");

				screenLabels[1].Text = flight.MeanAltitude.ToString();


				//flight = GetData.getFlight(); // 7 RPC
				//screenLabels[1].Text = flight.MeanAltitude.ToString(); // 7 RPC

				//screenLabels[1].Text = connection.SpaceCenter().ActiveVessel.Flight().MeanAltitude.ToString(); // 3 RPC
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
