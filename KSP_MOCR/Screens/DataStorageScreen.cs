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

			
			List<string> output = new List<string>();
			int c = 0;
			string tmpOut = "";
			foreach(KeyValuePair<string, string> item in data)
			{
				tmpOut += Helper.prtlen(item.Key, 10, Helper.Align.LEFT) + " :: ]" + item.Value + "[\n";
				c++;

				if(c == 28)
				{
					c = 0;
					output.Add(tmpOut);
					tmpOut = "";
				}
			}
			output.Add(tmpOut);

			for (int i = 0; i < 6; i++)
			{
				if(output.Count > i)
				{
					screenLabels[i].Text = output[i];
				}
				else
				{
					screenLabels[i].Text = "";
				}
			}
		}

		public override void makeElements()
		{
			for (int i = 0; i < 6; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs


			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateLabel(0, 1, 20, 28);
			screenLabels[0].Text = "DATA STORAGE\nNO DATA";

			screenLabels[1] = Helper.CreateLabel(20, 1, 20, 28);
			screenLabels[2] = Helper.CreateLabel(40, 1, 20, 28);
			screenLabels[3] = Helper.CreateLabel(60, 1, 20, 28);
			screenLabels[4] = Helper.CreateLabel(80, 1, 20, 28);
			screenLabels[5] = Helper.CreateLabel(100, 1, 20, 28);
		}

		public override void resize() { }
	}
}
