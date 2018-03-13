using System;
using System.Drawing;
using System.Threading;

namespace KSP_MOCR
{
	public class EventPanelFido1 : MocrScreen
	{
		bool[] litUpper = new bool[18]; // WHETER UPPER EVENT INDICATOR IS LIT
		bool[] litLower = new bool[18]; // WHETER LOWER EVENT INDICATOR IS LIT

		public EventPanelFido1(Screen form)
		{
			this.form = form;
			this.form.BackColor = Color.FromArgb(255, 62, 64, 68);
			this.charSize = false;

			Image myimage = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\darknoise.png");
			this.form.BackgroundImage = myimage;
			this.form.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Tile;

			this.width = 532;
			this.height = 196;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 3; i++) screenButtons.Add(null); // Initialize Buttons
			for (int i = 0; i < 4; i++) screenScrews.Add(null); // Initialize Screws
			for (int i = 0; i < 18; i++) screenEventIndicators.Add(null); // Initialize EventIndicators
			

			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			/*
			screenLabels[0] = Helper.CreateLabel(3.5, 0.25, 27, 1.5, "EVENT");
			screenLabels[0].type = CustomLabel.LabelType.ENGRAVED;

			screenButtons[0] = Helper.CreateButton(3.5, 2, 9, 6, "");
			screenButtons[0].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[0].setLightColor(MocrButton.color.RED);
			screenButtons[0].Click += (sender, e) => clickButton(sender, e, screenButtons[0]);

			screenButtons[1] = Helper.CreateButton(12.5, 2, 9, 6, "");
			screenButtons[1].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[1].setLightColor(MocrButton.color.AMBER);
			screenButtons[1].Click += (sender, e) => clickButton(sender, e, screenButtons[1]);

			screenButtons[2] = Helper.CreateButton(21.5, 2, 9, 6, "");
			screenButtons[2].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[2].setLightColor(MocrButton.color.GREEN);
			screenButtons[2].Click += (sender, e) => clickButton(sender, e, screenButtons[2]);
			*/


			// ROW ONE
			screenEventIndicators[0] = Helper.createEventIndicator(56, 14, false, true);
			screenEventIndicators[0].upperText = "PRELAUNCH";
			screenEventIndicators[0].lowerText = "LAUNCH";
			screenEventIndicators[0].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[0].lowerColor = EventIndicator.color.OFF;

			screenEventIndicators[1] = Helper.createEventIndicator(126, 14, false, true);
			screenEventIndicators[1].upperText = "COND\nLAUNCH";
			screenEventIndicators[1].lowerText = "COND SIM\nLNCH";
			screenEventIndicators[1].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[1].lowerColor = EventIndicator.color.OFF;

			screenEventIndicators[2] = Helper.createEventIndicator(196, 14, false, true);
			screenEventIndicators[2].upperText = "HIGH SPD\nORBIT";
			screenEventIndicators[2].lowerText = "L.S ORBIT";
			screenEventIndicators[2].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[2].lowerColor = EventIndicator.color.OFF;

			screenEventIndicators[3] = Helper.createEventIndicator(266, 14, false, true);
			screenEventIndicators[3].upperText = "LM\nDESCENT";
			screenEventIndicators[3].lowerText = "LM\nASCENT";
			screenEventIndicators[3].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[3].lowerColor = EventIndicator.color.OFF;

			screenEventIndicators[4] = Helper.createEventIndicator(336, 14, false, true);
			screenEventIndicators[4].upperText = "LUNAR\nSTAY";
			screenEventIndicators[4].lowerText = "ENTRY";
			screenEventIndicators[4].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[4].lowerColor = EventIndicator.color.OFF;

			screenEventIndicators[5] = Helper.createEventIndicator(406, 14, false, true);
			screenEventIndicators[5].upperText = "TRANS";
			screenEventIndicators[5].lowerText = "LUNAR\nORBIT";
			screenEventIndicators[5].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[5].lowerColor = EventIndicator.color.OFF;

			/// ROW TWO
			screenEventIndicators[6] = Helper.createEventIndicator(56, 70, false, true);
			screenEventIndicators[6].upperText = "HI PRIOR\nLANDING";
			screenEventIndicators[6].lowerText = "LOW PRIOR\nRESPONSE";
			screenEventIndicators[6].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[6].lowerColor = EventIndicator.color.OFF;

			screenEventIndicators[7] = Helper.createEventIndicator(126, 70, false, true);
			screenEventIndicators[7].upperText = "ENTRY TRJ\nUPDATE";
			screenEventIndicators[7].lowerText = "S/C SET\nENTRY UPD";
			screenEventIndicators[7].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[7].lowerColor = EventIndicator.color.OFF;

			screenEventIndicators[8] = Helper.createEventIndicator(196, 70, false, true);
			screenEventIndicators[8].upperText = "CSM STA\nCONTACTS";
			screenEventIndicators[8].lowerText = "LM STA\nCONTACTS";
			screenEventIndicators[8].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[8].lowerColor = EventIndicator.color.OFF;

			screenEventIndicators[9] = Helper.createEventIndicator(266, 70, false, true);
			screenEventIndicators[9].upperText = "CSM DC";
			screenEventIndicators[9].lowerText = "LM DC";
			screenEventIndicators[9].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[9].lowerColor = EventIndicator.color.OFF;

			screenEventIndicators[10] = Helper.createEventIndicator(336, 70, false, true);
			screenEventIndicators[10].upperText = "CSM UPTD\nINCOM";
			screenEventIndicators[10].lowerText = "LM UPTD\nINCOM";
			screenEventIndicators[10].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[10].lowerColor = EventIndicator.color.OFF;

			screenEventIndicators[11] = Helper.createEventIndicator(406, 70, false, true);
			screenEventIndicators[11].upperText = "CSM TRAJ\nUPDATE";
			screenEventIndicators[11].lowerText = "LM TRAJ\nUPDATE";
			screenEventIndicators[11].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[11].lowerColor = EventIndicator.color.OFF;

			/// ROW THREE
			screenEventIndicators[12] = Helper.createEventIndicator(56, 126, false, true);
			screenEventIndicators[12].upperText = "RNDZ\nPLANNING";
			screenEventIndicators[12].lowerText = "TLI\nPLANNING";
			screenEventIndicators[12].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[12].lowerColor = EventIndicator.color.OFF;

			screenEventIndicators[13] = Helper.createEventIndicator(126, 126, false, true);
			screenEventIndicators[13].upperText = "MIDCOURSE\nPLANNING";
			screenEventIndicators[13].lowerText = "LOI\nPLANNING";
			screenEventIndicators[13].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[13].lowerColor = EventIndicator.color.OFF;

			screenEventIndicators[14] = Helper.createEventIndicator(196, 126, false, true);
			screenEventIndicators[14].upperText = "RTE\nPLANNING";
			screenEventIndicators[14].lowerText = "PRIME\nDEORBIT";
			screenEventIndicators[14].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[14].lowerColor = EventIndicator.color.OFF;

			screenEventIndicators[15] = Helper.createEventIndicator(266, 126, false, true);
			screenEventIndicators[15].upperText = "CONT\nDEORBIT";
			screenEventIndicators[15].lowerText = "MANUAL\nDEORBIT";
			screenEventIndicators[15].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[15].lowerColor = EventIndicator.color.OFF;

			screenEventIndicators[16] = Helper.createEventIndicator(336, 126, false, true);
			screenEventIndicators[16].upperText = "VECTOR\nCOMP PROC";
			screenEventIndicators[16].lowerText = "FDO SW'S\nLIVE";
			screenEventIndicators[16].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[16].lowerColor = EventIndicator.color.OFF;

			screenEventIndicators[17] = Helper.createEventIndicator(406, 126, false, true);
			screenEventIndicators[17].upperText = "RESTART\nTAPE";
			screenEventIndicators[17].lowerText = "RESIDUAL\nCOMP";
			screenEventIndicators[17].upperColor = EventIndicator.color.OFF;
			screenEventIndicators[17].lowerColor = EventIndicator.color.OFF;

			screenScrews[0] = Helper.CreateScrew(4, 42, true);
			screenScrews[1] = Helper.CreateScrew(500, 42, true);
			screenScrews[2] = Helper.CreateScrew(4, 140, true);
			screenScrews[3] = Helper.CreateScrew(500, 140, true);
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			// TO SET AN EVENT-INDICATOR LIGHT (OFF OR ON), SET THE CORRESPONDING litUpper/litLower false/true
			

			int r;
			r = litUpper[0] ? setButtonColor(0, false, EventIndicator.color.WHITE_LIT) : setButtonColor(0, false, EventIndicator.color.OFF);
			r = litLower[0] ? setButtonColor(0, true, EventIndicator.color.WHITE_LIT) : setButtonColor(0, true, EventIndicator.color.OFF);

			r = litUpper[1] ? setButtonColor(1, false, EventIndicator.color.GREEN_LIT) : setButtonColor(1, false, EventIndicator.color.OFF);
			r = litLower[1] ? setButtonColor(1, true, EventIndicator.color.GREEN_LIT) : setButtonColor(1, true, EventIndicator.color.OFF);

			r = litUpper[2] ? setButtonColor(2, false, EventIndicator.color.WHITE_LIT) : setButtonColor(2, false, EventIndicator.color.OFF);
			r = litLower[2] ? setButtonColor(2, true, EventIndicator.color.GREEN_LIT) : setButtonColor(2, true, EventIndicator.color.OFF);

			r = litUpper[3] ? setButtonColor(3, false, EventIndicator.color.WHITE_LIT) : setButtonColor(3, false, EventIndicator.color.OFF);
			r = litLower[3] ? setButtonColor(3, true, EventIndicator.color.GREEN_LIT) : setButtonColor(3, true, EventIndicator.color.OFF);

			r = litUpper[4] ? setButtonColor(4, false, EventIndicator.color.RED_LIT) : setButtonColor(4, false, EventIndicator.color.OFF);
			r = litLower[4] ? setButtonColor(4, true, EventIndicator.color.GREEN_LIT) : setButtonColor(4, true, EventIndicator.color.OFF);

			r = litUpper[5] ? setButtonColor(5, false, EventIndicator.color.GREEN_LIT) : setButtonColor(5, false, EventIndicator.color.OFF);
			r = litLower[5] ? setButtonColor(5, true, EventIndicator.color.GREEN_LIT) : setButtonColor(5, true, EventIndicator.color.OFF);

			r = litUpper[6] ? setButtonColor(6, false, EventIndicator.color.GREEN_LIT) : setButtonColor(6, false, EventIndicator.color.OFF);
			r = litLower[6] ? setButtonColor(6, true, EventIndicator.color.AMBER_LIT) : setButtonColor(6, true, EventIndicator.color.OFF);

			r = litUpper[7] ? setButtonColor(7, false, EventIndicator.color.AMBER_LIT) : setButtonColor(7, false, EventIndicator.color.OFF);
			r = litLower[7] ? setButtonColor(7, true, EventIndicator.color.AMBER_LIT) : setButtonColor(7, true, EventIndicator.color.OFF);

			r = litUpper[8] ? setButtonColor(8, false, EventIndicator.color.AMBER_LIT) : setButtonColor(8, false, EventIndicator.color.OFF);
			r = litLower[8] ? setButtonColor(8, true, EventIndicator.color.AMBER_LIT) : setButtonColor(8, true, EventIndicator.color.OFF);

			r = litUpper[9] ? setButtonColor(9, false, EventIndicator.color.AMBER_LIT) : setButtonColor(9, false, EventIndicator.color.OFF);
			r = litLower[9] ? setButtonColor(9, true, EventIndicator.color.WHITE_LIT) : setButtonColor(9, true, EventIndicator.color.OFF);

			r = litUpper[10] ? setButtonColor(10, false, EventIndicator.color.WHITE_LIT) : setButtonColor(10, false, EventIndicator.color.OFF);
			r = litLower[10] ? setButtonColor(10, true, EventIndicator.color.WHITE_LIT) : setButtonColor(10, true, EventIndicator.color.OFF);

			r = litUpper[11] ? setButtonColor(11, false, EventIndicator.color.WHITE_LIT) : setButtonColor(11, false, EventIndicator.color.OFF);
			r = litLower[11] ? setButtonColor(11, true, EventIndicator.color.WHITE_LIT) : setButtonColor(11, true, EventIndicator.color.OFF);

			r = litUpper[12] ? setButtonColor(12, false, EventIndicator.color.WHITE_LIT) : setButtonColor(12, false, EventIndicator.color.OFF);
			r = litLower[12] ? setButtonColor(12, true, EventIndicator.color.WHITE_LIT) : setButtonColor(12, true, EventIndicator.color.OFF);

			r = litUpper[13] ? setButtonColor(13, false, EventIndicator.color.WHITE_LIT) : setButtonColor(13, false, EventIndicator.color.OFF);
			r = litLower[13] ? setButtonColor(13, true, EventIndicator.color.WHITE_LIT) : setButtonColor(13, true, EventIndicator.color.OFF);

			r = litUpper[14] ? setButtonColor(14, false, EventIndicator.color.WHITE_LIT) : setButtonColor(14, false, EventIndicator.color.OFF);
			r = litLower[14] ? setButtonColor(14, true, EventIndicator.color.WHITE_LIT) : setButtonColor(14, true, EventIndicator.color.OFF);

			r = litUpper[15] ? setButtonColor(15, false, EventIndicator.color.WHITE_LIT) : setButtonColor(15, false, EventIndicator.color.OFF);
			r = litLower[15] ? setButtonColor(15, true, EventIndicator.color.WHITE_LIT) : setButtonColor(15, true, EventIndicator.color.OFF);

			r = litUpper[16] ? setButtonColor(16, false, EventIndicator.color.WHITE_LIT) : setButtonColor(16, false, EventIndicator.color.OFF);
			r = litLower[16] ? setButtonColor(16, true, EventIndicator.color.WHITE_LIT) : setButtonColor(16, true, EventIndicator.color.OFF);

			r = litUpper[17] ? setButtonColor(17, false, EventIndicator.color.WHITE_LIT) : setButtonColor(17, false, EventIndicator.color.OFF);
			r = litLower[17] ? setButtonColor(17, true, EventIndicator.color.AMBER_LIT) : setButtonColor(17, true, EventIndicator.color.OFF);

			form.Invalidate();
		}

		public override void resize() { }

		private int setButtonColor(int id, bool lower, EventIndicator.color c)
		{
			screenEventIndicators[id].updateState(lower, c);
			return 1;
		}

		private void clickButton(object sender, EventArgs e, MocrButton button)
		{
		}
	}
}
