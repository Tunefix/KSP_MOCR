using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class CrtTest2 : MocrScreen
	{
		private int fontSize;

		public CrtTest2(Screen form, int size)
		{
			this.form = form;
			this.screenStreams = form.streamCollection;

			this.charSize = false;
			this.width = 656;
			this.height = 494;
			this.updateRate = 1000;

			fontSize = size;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 10; i++) screenLabels.Add(null); // Initialize Labels

			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			//string str = "ABCDE\nFGHIL\nJKLMN\nOPQRS\nTUVWX\nYZ   \n01234\n56789\n.-*  \n+()";
			//string str = "ABCDEFG\nHILJKLM\nNOPQRST\nUVWXYZ \n 01234 \n 56789 \n .-*+()";

			string str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ\n0123456789 .-*+()";


			screenLabels[0] = Helper.CreateCRTLabel(0, 0, 26, 2, str, 1);
			screenLabels[1] = Helper.CreateCRTLabel(0, 2, 26, 2, str, 2);
			screenLabels[2] = Helper.CreateCRTLabel(0, 4, 26, 2, str, 3);
			screenLabels[3] = Helper.CreateCRTLabel(0, 6, 26, 2, str, 4);
			screenLabels[4] = Helper.CreateCRTLabel(0, 8, 26, 2, str, 5);

			screenLabels[5] = Helper.CreateCRTLabel(26, 0, 26, 2, "ZZ", 1);
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
		}

		public override void resize()
		{
		}
	}
}
