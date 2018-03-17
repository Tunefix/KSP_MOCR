using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class CrtTest : MocrScreen
	{
		private int fontSize;

		public CrtTest(Screen form, int size)
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
			for (int i = 0; i < 80; i++) screenLabels.Add(null); // Initialize Labels

			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix


			int lines = 10;
			int chars = 10;

			switch(fontSize)
			{
				case 1:
					lines = 54;
					chars = 129;
					break;
				case 2:
					lines = 44;
					chars = 92;
					break;
				case 3:
					lines = 34;
					chars = 72;
					break;
				case 4:
					lines = 30;
					chars = 64;
					break;
				case 5:
					lines = 24;
					chars = 46;
					break;
			}

			string str = "";
			for (int i = 0; i < chars; i++)
			{
				if ((i+1) % 10 == 0)
				{
					str += Math.Floor((i+1) / 10d).ToString().Substring(0, 1);
				}
				else
				{
					str += "X";
				}
			}
			for (int i = 0; i < lines; i++)
			{
				screenLabels[i] = Helper.CreateCRTLabel(0, i, chars, 1, str, fontSize);
			}
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
		}

		public override void resize()
		{
		}
	}
}
