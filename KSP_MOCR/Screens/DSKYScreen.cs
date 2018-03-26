using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KSP_MOCR
{
	class DSKYScreen : MocrScreen
	{
		string MD1, MD2; // PROGRAM
		string VD1, VD2; // VERB
		string ND1, ND2; // NOUN
		string R1S, R1D1, R1D2, R1D3, R1D4, R1D5; // REGISTER 1
		string R2S, R2D1, R2D2, R2D3, R2D4, R2D5; // REGISTER 2
		string R3S, R3D1, R3D2, R3D3, R3D4, R3D5; // REGISTER 3

		bool VFlash = false;
		bool NFlash = false;
		int VFC = 0; // Verb flash counter
		int NFC = 0; // Noun flash counter

		bool oprErr = false;

		enum nextInput { NONE, V1, V2, N1, N2}
		nextInput inputState = nextInput.NONE;

		public DSKYScreen(Screen form)
		{
			this.form = form;
			this.chartData = form.form.chartData;
			form.BackColor = Color.FromArgb(255, 16, 16, 16);

			screenStreams = form.streamCollection;
			dataStorage = form.dataStorage;

			this.charSize = false;
			this.width = 370;
			this.height = 436;
			this.updateRate = 500;
			this.timeWarning = 400;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 100; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 70; i++) screenIndicators.Add(null); // Initialize Indicators
			for (int i = 0; i < 80; i++) screenButtons.Add(null); // Initialize Buttons
			for (int i = 0; i < 6; i++) screenSegDisps.Add(null); // Initialize 7-Segment Displays

			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix


			// BUTTONS
			screenButtons[0] = Helper.CreateButton(12, 300, 46, 46, "VERB", true);
			screenButtons[0].MouseDown += verbPress;
			screenButtons[1] = Helper.CreateButton(12, 350, 46, 46, "NOUN", true);
			screenButtons[1].MouseDown += nounPress;
			screenButtons[2] = Helper.CreateButton(62, 275, 46, 46, "+", true);
			screenButtons[2].MouseDown += (sender, e) => keyPress(sender, e, "PLUS");
			screenButtons[3] = Helper.CreateButton(62, 325, 46, 46, "-", true);
			screenButtons[3].MouseDown += (sender, e) => keyPress(sender, e, "MINUS");
			screenButtons[4] = Helper.CreateButton(62, 375, 46, 46, "0", true);
			screenButtons[4].MouseDown += (sender, e) => numPress(sender, e, 0);
			screenButtons[5] = Helper.CreateButton(112, 275, 46, 46, "7", true);
			screenButtons[5].MouseDown += (sender, e) => numPress(sender, e, 7);
			screenButtons[6] = Helper.CreateButton(112, 325, 46, 46, "4", true);
			screenButtons[6].MouseDown += (sender, e) => numPress(sender, e, 4);
			screenButtons[7] = Helper.CreateButton(112, 375, 46, 46, "1", true);
			screenButtons[7].MouseDown += (sender, e) => numPress(sender, e, 1);
			screenButtons[8] = Helper.CreateButton(162, 275, 46, 46, "8", true);
			screenButtons[8].MouseDown += (sender, e) => numPress(sender, e, 8);
			screenButtons[9] = Helper.CreateButton(162, 325, 46, 46, "5", true);
			screenButtons[9].MouseDown += (sender, e) => numPress(sender, e, 5);
			screenButtons[10] = Helper.CreateButton(162, 375, 46, 46, "2", true);
			screenButtons[10].MouseDown += (sender, e) => numPress(sender, e, 2);
			screenButtons[11] = Helper.CreateButton(212, 275, 46, 46, "9", true);
			screenButtons[11].MouseDown += (sender, e) => numPress(sender, e, 9);
			screenButtons[12] = Helper.CreateButton(212, 325, 46, 46, "6", true);
			screenButtons[12].MouseDown += (sender, e) => numPress(sender, e, 6);
			screenButtons[13] = Helper.CreateButton(212, 375, 46, 46, "3", true);
			screenButtons[13].MouseDown += (sender, e) => numPress(sender, e, 3);
			screenButtons[14] = Helper.CreateButton(262, 275, 46, 46, "CLR", true);
			screenButtons[14].MouseDown += (sender, e) => keyPress(sender, e, "CLR");
			screenButtons[15] = Helper.CreateButton(262, 325, 46, 46, "PRO", true);
			screenButtons[15].MouseDown += (sender, e) => keyPress(sender, e, "PRO");
			screenButtons[16] = Helper.CreateButton(262, 375, 46, 46, "KEY\nREL", true);
			screenButtons[16].MouseDown += (sender, e) => keyPress(sender, e, "KEYREL");
			screenButtons[17] = Helper.CreateButton(312, 300, 46, 46, "ENTR", true);
			screenButtons[17].MouseDown += entrPress;
			screenButtons[18] = Helper.CreateButton(312, 350, 46, 46, "RSET", true);
			screenButtons[18].MouseDown += (sender, e) => keyPress(sender, e, "RSET");

			// 7-SEG DISPLAYS
			screenSegDisps[0] = Helper.CreateSegDisp(202, 132, 148, 36, 5, true, "R1", true);
			screenSegDisps[0].style = SegDisp.SegDispStyle.NO_BORDER;
			screenSegDisps[1] = Helper.CreateSegDisp(202, 174, 148, 36, 5, true, "R2", true);
			screenSegDisps[1].style = SegDisp.SegDispStyle.NO_BORDER;
			screenSegDisps[2] = Helper.CreateSegDisp(202, 216, 148, 36, 5, true, "R3", true);
			screenSegDisps[2].style = SegDisp.SegDispStyle.NO_BORDER;

			screenSegDisps[3] = Helper.CreateSegDisp(214, 84, 49, 36, 2, false, "V", true);
			screenSegDisps[3].style = SegDisp.SegDispStyle.NO_BORDER;
			screenSegDisps[4] = Helper.CreateSegDisp(300, 84, 49, 36, 2, false, "N", true);
			screenSegDisps[4].style = SegDisp.SegDispStyle.NO_BORDER;

			screenSegDisps[5] = Helper.CreateSegDisp(300, 32, 49, 36, 2, false, "P", true);
			screenSegDisps[5].style = SegDisp.SegDispStyle.NO_BORDER;

			// 7-SEG LABELS
			screenLabels[1] = Helper.CreateLabel(300, 18, 49, 13, "PROG", false, true);
			screenLabels[1].Font = form.smallFontB;
			screenLabels[1].align = CustomLabel.Alignment.CENTER;
			screenLabels[1].BackColor = Color.FromArgb(255, 0, 200, 0);
			screenLabels[1].ForeColor = Color.FromArgb(255, 0, 0, 0);
			screenLabels[1].setlineOffset(3);
			screenLabels[2] = Helper.CreateLabel(214, 70, 49, 13, "VERB", false, true);
			screenLabels[2].Font = form.smallFontB;
			screenLabels[2].align = CustomLabel.Alignment.CENTER;
			screenLabels[2].BackColor = Color.FromArgb(255, 0, 200, 0);
			screenLabels[2].ForeColor = Color.FromArgb(255, 0, 0, 0);
			screenLabels[2].setlineOffset(3);
			screenLabels[3] = Helper.CreateLabel(300, 70, 49, 13, "NOUN", false, true);
			screenLabels[3].Font = form.smallFontB;
			screenLabels[3].align = CustomLabel.Alignment.CENTER;
			screenLabels[3].BackColor = Color.FromArgb(255, 0, 200, 0);
			screenLabels[3].ForeColor = Color.FromArgb(255, 0, 0, 0);
			screenLabels[3].setlineOffset(3);

			// DSKY INDICATORS
			screenIndicators[50] = Helper.CreateIndicator(25, 18, 70, 30, "UPLINK\nACTY", true);
			screenIndicators[50].Font = form.smallFont;
			screenIndicators[51] = Helper.CreateIndicator(25, 52, 70, 30, "NO ATT", true);
			screenIndicators[51].Font = form.buttonFont;
			screenIndicators[52] = Helper.CreateIndicator(25, 86, 70, 30, "STBY", true);
			screenIndicators[52].Font = form.buttonFont;
			screenIndicators[53] = Helper.CreateIndicator(25, 120, 70, 30, "KEY REL", true);
			screenIndicators[53].Font = form.buttonFont;
			screenIndicators[54] = Helper.CreateIndicator(25, 154, 70, 30, "OPR ERR", true);
			screenIndicators[54].Font = form.buttonFont;
			screenIndicators[55] = Helper.CreateIndicator(25, 188, 70, 30, "", true);
			screenIndicators[55].Font = form.buttonFont;
			screenIndicators[56] = Helper.CreateIndicator(25, 222, 70, 30, "", true);
			screenIndicators[56].Font = form.buttonFont;

			screenIndicators[57] = Helper.CreateIndicator(101, 18, 70, 30, "TEMP", true);
			screenIndicators[57].Font = form.smallFont;
			screenIndicators[58] = Helper.CreateIndicator(101, 52, 70, 30, "GIMBAL\nLOCK", true);
			screenIndicators[58].Font = form.buttonFont;
			screenIndicators[59] = Helper.CreateIndicator(101, 86, 70, 30, "PROG", true);
			screenIndicators[59].Font = form.buttonFont;
			screenIndicators[60] = Helper.CreateIndicator(101, 120, 70, 30, "RESTART", true);
			screenIndicators[60].Font = form.buttonFont;
			screenIndicators[61] = Helper.CreateIndicator(101, 154, 70, 30, "TRACKER", true);
			screenIndicators[61].Font = form.buttonFont;
			screenIndicators[62] = Helper.CreateIndicator(101, 188, 70, 30, "ALT", true);
			screenIndicators[62].Font = form.buttonFont;
			screenIndicators[63] = Helper.CreateIndicator(101, 222, 70, 30, "VEL", true);
			screenIndicators[63].Font = form.buttonFont;

			screenIndicators[64] = Helper.CreateIndicator(214, 18, 49, 49, "COMP\nACTY", true);
			screenIndicators[64].Font = form.buttonFont;

			// Backplate (Custom control-class that can go into labels)
			screenLabels[0] = new DSKYBackplate();
			form.Controls.Add(screenLabels[0]);
			screenLabels[0].Location = new System.Drawing.Point(0, 0);
			screenLabels[0].Size = new System.Drawing.Size(width, height);
		}

		

		public override void updateLocalElements(object sender, EventArgs e)
		{

			// SET FLASH
			VFlash = dataStorage.getData("AGC_VFLSH") == "SET" ? true : false;
			NFlash = dataStorage.getData("AGC_NFLSH") == "SET" ? true : false;

			// PROGRAM
			MD1 = dataStorage.getData("AGC_MD1");
			MD2 = dataStorage.getData("AGC_MD2");
			screenSegDisps[5].setValue(MD1.ToString() + MD2.ToString());


			// VERB
			VD1 = dataStorage.getData("AGC_VD1");
			VD2 = dataStorage.getData("AGC_VD2");
			

			if(VFlash)
			{
				if(VFC < 1)
				{
					screenSegDisps[3].setValue(VD1.ToString() + VD2.ToString());
					VFC++;
				}
				else if(VFC < 2)
				{
					screenSegDisps[3].setValue("");
					VFC = 0;
				}
			}
			else
			{
				screenSegDisps[3].setValue(VD1.ToString() + VD2.ToString());
			}


			// NOUN
			ND1 = dataStorage.getData("AGC_ND1");
			ND2 = dataStorage.getData("AGC_ND2");

			if (NFlash)
			{
				if (NFC < 1)
				{
					screenSegDisps[4].setValue(ND1.ToString() + ND2.ToString());
					NFC++;
				}
				else if (NFC < 2)
				{
					screenSegDisps[4].setValue("");
					NFC = 0;
				}
			}
			else
			{
				screenSegDisps[4].setValue(ND1.ToString() + ND2.ToString());
			}


			// R1
			R1D1 = dataStorage.getData("AGC_R1D1");
			R1D2 = dataStorage.getData("AGC_R1D2");
			R1D3 = dataStorage.getData("AGC_R1D3");
			R1D4 = dataStorage.getData("AGC_R1D4");
			R1D5 = dataStorage.getData("AGC_R1D5");
			string sign = dataStorage.getData("AGC_R1S");
			SegDisp.SignState signState = SegDisp.SignState.AUTO;
			if(sign == "NEG") signState = SegDisp.SignState.MINUS;
			int.TryParse(dataStorage.getData("AGC_R1P"), out int p);
			screenSegDisps[0].setValue(R1D1.ToString() + R1D2.ToString() + R1D3.ToString() + R1D4.ToString() + R1D5.ToString(), p, signState);


			// R2
			R2D1 = dataStorage.getData("AGC_R2D1");
			R2D2 = dataStorage.getData("AGC_R2D2");
			R2D3 = dataStorage.getData("AGC_R2D3");
			R2D4 = dataStorage.getData("AGC_R2D4");
			R2D5 = dataStorage.getData("AGC_R2D5");
			sign = dataStorage.getData("AGC_R2S");
			signState = SegDisp.SignState.AUTO;
			if (sign == "NEG") signState = SegDisp.SignState.MINUS;
			int.TryParse(dataStorage.getData("AGC_R2P"), out p);
			screenSegDisps[1].setValue(R2D1.ToString() + R2D2.ToString() + R2D3.ToString() + R2D4.ToString() + R2D5.ToString(), p, signState);


			// R3
			R3D1 = dataStorage.getData("AGC_R3D1");
			R3D2 = dataStorage.getData("AGC_R3D2");
			R3D3 = dataStorage.getData("AGC_R3D3");
			R3D4 = dataStorage.getData("AGC_R3D4");
			R3D5 = dataStorage.getData("AGC_R3D5");
			sign = dataStorage.getData("AGC_R3S");
			signState = SegDisp.SignState.AUTO;
			if (sign == "NEG") signState = SegDisp.SignState.MINUS;
			int.TryParse(dataStorage.getData("AGC_R3P"), out p);
			screenSegDisps[2].setValue(R3D1.ToString() + R3D2.ToString() + R3D3.ToString() + R3D4.ToString() + R3D5.ToString(), p, signState);

			// INDICATORS
			screenIndicators[53].setStatus(dataStorage.getData("AGC_KEYREL") == "SET" ? Indicator.status.WHITE : Indicator.status.OFF);
			screenIndicators[54].setStatus(dataStorage.getData("AGC_OPRERR") == "SET" ? Indicator.status.WHITE : Indicator.status.OFF);


		}

		public override void resize()
		{
		}

		private void keyPress(object sender, EventArgs e, string key)
		{
			//Console.WriteLine("KEY" + key);
			dataStorage.setData("AGC_KEY", key);
		}

		private void verbPress(object sender, EventArgs e)
		{
			dataStorage.setData("AGC_KEY", "VERB");
		}

		private void nounPress(object sender, EventArgs e)
		{
			dataStorage.setData("AGC_KEY", "NOUN");
		}

		private void entrPress(object sender, EventArgs e)
		{
			dataStorage.setData("AGC_KEY", "ENTR");
		}

		private void numPress(object sender, EventArgs e, int num)
		{
			dataStorage.setData("AGC_KEY", num.ToString());
			//Console.WriteLine("KEY" + num.ToString());
		}

		public override bool keyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.V:
					screenButtons[0].setPressedState(true);
					verbPress(null, null);
					Thread.Sleep(100); // Duration of pressed state
					screenButtons[0].setPressedState(false);
					return true;
				case Keys.N:
					screenButtons[1].setPressedState(true);
					nounPress(null, null);
					Thread.Sleep(100);
					screenButtons[1].setPressedState(false);
					return true;
				case Keys.D0:
				case Keys.NumPad0:
					screenButtons[4].setPressedState(true);
					numPress(null, null, 0);
					Thread.Sleep(100);
					screenButtons[4].setPressedState(false);
					return true;
				case Keys.D1:
				case Keys.NumPad1:
					screenButtons[7].setPressedState(true);
					numPress(null, null, 1);
					Thread.Sleep(100);
					screenButtons[7].setPressedState(false);
					return true;
				case Keys.D2:
				case Keys.NumPad2:
					screenButtons[10].setPressedState(true);
					numPress(null, null, 2);
					Thread.Sleep(100);
					screenButtons[10].setPressedState(false);
					return true;
				case Keys.D3:
				case Keys.NumPad3:
					screenButtons[13].setPressedState(true);
					numPress(null, null, 3);
					Thread.Sleep(100);
					screenButtons[13].setPressedState(false);
					return true;
				case Keys.D4:
				case Keys.NumPad4:
					screenButtons[6].setPressedState(true);
					numPress(null, null, 4);
					Thread.Sleep(100);
					screenButtons[6].setPressedState(false);
					return true;
				case Keys.D5:
				case Keys.NumPad5:
					screenButtons[9].setPressedState(true);
					numPress(null, null, 5);
					Thread.Sleep(100);
					screenButtons[9].setPressedState(false);
					return true;
				case Keys.D6:
				case Keys.NumPad6:
					screenButtons[12].setPressedState(true);
					numPress(null, null, 6);
					Thread.Sleep(100);
					screenButtons[12].setPressedState(false);
					return true;
				case Keys.D7:
				case Keys.NumPad7:
					screenButtons[5].setPressedState(true);
					numPress(null, null, 7);
					Thread.Sleep(100);
					screenButtons[5].setPressedState(false);
					return true;
				case Keys.D8:
				case Keys.NumPad8:
					screenButtons[8].setPressedState(true);
					numPress(null, null, 8);
					Thread.Sleep(100);
					screenButtons[8].setPressedState(false);
					return true;
				case Keys.D9:
				case Keys.NumPad9:
					screenButtons[11].setPressedState(true);
					numPress(null, null, 9);
					Thread.Sleep(100);
					screenButtons[11].setPressedState(false);
					return true;
				case Keys.Enter:
					screenButtons[17].setPressedState(true);
					keyPress(null, null, "ENTR");
					Thread.Sleep(100);
					screenButtons[17].setPressedState(false);
					return true;
				case Keys.Add:
				case Keys.Oemplus:
					screenButtons[2].setPressedState(true);
					keyPress(null, null, "PLUS");
					Thread.Sleep(100);
					screenButtons[2].setPressedState(false);
					return true;
				case Keys.Subtract:
				case Keys.OemMinus:
					screenButtons[3].setPressedState(true);
					keyPress(null, null, "MINUS");
					Thread.Sleep(100);
					screenButtons[3].setPressedState(false);
					return true;
				case Keys.C:
					screenButtons[14].setPressedState(true);
					keyPress(null, null, "CLR");
					Thread.Sleep(100);
					screenButtons[14].setPressedState(false);
					return true;
				case Keys.P:
					screenButtons[15].setPressedState(true);
					keyPress(null, null, "PRO");
					Thread.Sleep(100);
					screenButtons[15].setPressedState(false);
					return true;
				case Keys.K:
					screenButtons[16].setPressedState(true);
					keyPress(null, null, "KEYREL");
					Thread.Sleep(100);
					screenButtons[16].setPressedState(false);
					return true;
				case Keys.R:
					screenButtons[18].setPressedState(true);
					keyPress(null, null, "RSET");
					Thread.Sleep(100);
					screenButtons[18].setPressedState(false);
					return true;
				default:
					return false;
			}
		}
	}
}
