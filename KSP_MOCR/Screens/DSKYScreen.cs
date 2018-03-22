using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class DSKYScreen : MocrScreen
	{
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
			screenButtons[1] = Helper.CreateButton(12, 350, 46, 46, "NOUN", true);
			screenButtons[2] = Helper.CreateButton(62, 275, 46, 46, "+", true);
			screenButtons[3] = Helper.CreateButton(62, 325, 46, 46, "-", true);
			screenButtons[4] = Helper.CreateButton(62, 375, 46, 46, "0", true);
			screenButtons[5] = Helper.CreateButton(112, 275, 46, 46, "7", true);
			screenButtons[6] = Helper.CreateButton(112, 325, 46, 46, "4", true);
			screenButtons[7] = Helper.CreateButton(112, 375, 46, 46, "1", true);
			screenButtons[8] = Helper.CreateButton(162, 275, 46, 46, "8", true);
			screenButtons[9] = Helper.CreateButton(162, 325, 46, 46, "5", true);
			screenButtons[10] = Helper.CreateButton(162, 375, 46, 46, "2", true);
			screenButtons[11] = Helper.CreateButton(212, 275, 46, 46, "9", true);
			screenButtons[12] = Helper.CreateButton(212, 325, 46, 46, "6", true);
			screenButtons[13] = Helper.CreateButton(212, 375, 46, 46, "3", true);
			screenButtons[14] = Helper.CreateButton(262, 275, 46, 46, "CLR", true);
			screenButtons[15] = Helper.CreateButton(262, 325, 46, 46, "PRO", true);
			screenButtons[16] = Helper.CreateButton(262, 375, 46, 46, "KEY\nREL", true);
			screenButtons[17] = Helper.CreateButton(312, 300, 46, 46, "ENTR", true);
			screenButtons[18] = Helper.CreateButton(312, 350, 46, 46, "RSET", true);

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
		}

		public override void resize()
		{
		}
	}
}
