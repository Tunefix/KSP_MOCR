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
	class DataStorageScreen : MocrScreen
	{
		public DataStorageScreen(Screen form)
		{
			this.screenStreams = form.streamCollection;
			this.form = form;

			this.width = 120;
			this.height = 30;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			Dictionary<string, string> data = form.dataStorage.GetDictionary();

			string output = "";
			foreach(KeyValuePair<string, string> item in data)
			{
				output += item.Key + " :: " + item.Value + "\n";
			}
			screenLabels[0].Text = "DATA STORAGE\n" + output;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 2; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs


			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateLabel(1, 1, 118, 28); // Connection Status
			screenLabels[0].Text = "DATA STORAGE\nNO DATA";
			screenLabels[0].TextAlign = ContentAlignment.TopLeft;
		}

		public override void resize() { }
	}
}
