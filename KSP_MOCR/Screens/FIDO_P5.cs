using System;
using System.Drawing;
using System.Threading;

namespace KSP_MOCR
{
	public class FIDO_P5 : MocrScreen
	{
		/**
		 * EVENT INDICATOR (18)
		 */

		public FIDO_P5(Screen form)
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
			screenEventIndicators[0].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[0].upperOnColor = EventIndicator.color.WHITE_LIT;
			screenEventIndicators[0].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[0].lowerOnColor = EventIndicator.color.WHITE_LIT;

			screenEventIndicators[1] = Helper.createEventIndicator(126, 14, false, true);
			screenEventIndicators[1].upperText = "COND\nLAUNCH";
			screenEventIndicators[1].lowerText = "COND SIM\nLNCH";
			screenEventIndicators[1].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[1].upperOnColor = EventIndicator.color.GREEN_LIT;
			screenEventIndicators[1].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[1].lowerOnColor = EventIndicator.color.GREEN_LIT;

			screenEventIndicators[2] = Helper.createEventIndicator(196, 14, false, true);
			screenEventIndicators[2].upperText = "HIGH SPD\nORBIT";
			screenEventIndicators[2].lowerText = "L.S ORBIT";
			screenEventIndicators[2].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[2].upperOnColor = EventIndicator.color.WHITE_LIT;
			screenEventIndicators[2].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[2].lowerOnColor = EventIndicator.color.GREEN_LIT;

			screenEventIndicators[3] = Helper.createEventIndicator(266, 14, false, true);
			screenEventIndicators[3].upperText = "LM\nDESCENT";
			screenEventIndicators[3].lowerText = "LM\nASCENT";
			screenEventIndicators[3].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[3].upperOnColor = EventIndicator.color.WHITE_LIT;
			screenEventIndicators[3].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[3].lowerOnColor = EventIndicator.color.GREEN_LIT;

			screenEventIndicators[4] = Helper.createEventIndicator(336, 14, false, true);
			screenEventIndicators[4].upperText = "LUNAR\nSTAY";
			screenEventIndicators[4].lowerText = "ENTRY";
			screenEventIndicators[4].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[4].upperOnColor = EventIndicator.color.RED_LIT;
			screenEventIndicators[4].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[4].lowerOnColor = EventIndicator.color.GREEN_LIT;

			screenEventIndicators[5] = Helper.createEventIndicator(406, 14, false, true);
			screenEventIndicators[5].upperText = "TRANS";
			screenEventIndicators[5].lowerText = "LUNAR\nORBIT";
			screenEventIndicators[5].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[5].upperOnColor = EventIndicator.color.GREEN_LIT;
			screenEventIndicators[5].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[5].lowerOnColor = EventIndicator.color.GREEN_LIT;

			/// ROW TWO
			screenEventIndicators[6] = Helper.createEventIndicator(56, 70, false, true);
			screenEventIndicators[6].upperText = "HI PRIOR\nLANDING";
			screenEventIndicators[6].lowerText = "LOW PRIOR\nRESPONSE";
			screenEventIndicators[6].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[6].upperOnColor = EventIndicator.color.GREEN_LIT;
			screenEventIndicators[6].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[6].lowerOnColor = EventIndicator.color.AMBER_LIT;

			screenEventIndicators[7] = Helper.createEventIndicator(126, 70, false, true);
			screenEventIndicators[7].upperText = "ENTRY TRJ\nUPDATE";
			screenEventIndicators[7].lowerText = "S/C SET\nENTRY UPD";
			screenEventIndicators[7].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[7].upperOnColor = EventIndicator.color.AMBER_LIT;
			screenEventIndicators[7].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[7].lowerOnColor = EventIndicator.color.AMBER_LIT;

			screenEventIndicators[8] = Helper.createEventIndicator(196, 70, false, true);
			screenEventIndicators[8].upperText = "CSM STA\nCONTACTS";
			screenEventIndicators[8].lowerText = "LM STA\nCONTACTS";
			screenEventIndicators[8].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[8].upperOnColor = EventIndicator.color.AMBER_LIT;
			screenEventIndicators[8].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[8].lowerOnColor = EventIndicator.color.AMBER_LIT;

			screenEventIndicators[9] = Helper.createEventIndicator(266, 70, false, true);
			screenEventIndicators[9].upperText = "CSM DC";
			screenEventIndicators[9].lowerText = "LM DC";
			screenEventIndicators[9].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[9].upperOnColor = EventIndicator.color.AMBER_LIT;
			screenEventIndicators[9].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[9].lowerOnColor = EventIndicator.color.WHITE_LIT;

			screenEventIndicators[10] = Helper.createEventIndicator(336, 70, false, true);
			screenEventIndicators[10].upperText = "CSM UPTD\nINCOM";
			screenEventIndicators[10].lowerText = "LM UPTD\nINCOM";
			screenEventIndicators[10].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[10].upperOnColor = EventIndicator.color.WHITE_LIT;
			screenEventIndicators[10].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[10].lowerOnColor = EventIndicator.color.WHITE_LIT;

			screenEventIndicators[11] = Helper.createEventIndicator(406, 70, false, true);
			screenEventIndicators[11].upperText = "CSM TRAJ\nUPDATE";
			screenEventIndicators[11].lowerText = "LM TRAJ\nUPDATE";
			screenEventIndicators[11].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[11].upperOnColor = EventIndicator.color.WHITE_LIT;
			screenEventIndicators[11].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[11].lowerOnColor = EventIndicator.color.WHITE_LIT;

			/// ROW THREE
			screenEventIndicators[12] = Helper.createEventIndicator(56, 126, false, true);
			screenEventIndicators[12].upperText = "RNDZ\nPLANNING";
			screenEventIndicators[12].lowerText = "TLI\nPLANNING";
			screenEventIndicators[12].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[12].upperOnColor = EventIndicator.color.WHITE_LIT;
			screenEventIndicators[12].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[12].lowerOnColor = EventIndicator.color.WHITE_LIT;

			screenEventIndicators[13] = Helper.createEventIndicator(126, 126, false, true);
			screenEventIndicators[13].upperText = "MIDCOURSE\nPLANNING";
			screenEventIndicators[13].lowerText = "LOI\nPLANNING";
			screenEventIndicators[13].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[13].upperOnColor = EventIndicator.color.WHITE_LIT;
			screenEventIndicators[13].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[13].lowerOnColor = EventIndicator.color.WHITE_LIT;

			screenEventIndicators[14] = Helper.createEventIndicator(196, 126, false, true);
			screenEventIndicators[14].upperText = "RTE\nPLANNING";
			screenEventIndicators[14].lowerText = "PRIME\nDEORBIT";
			screenEventIndicators[14].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[14].upperOnColor = EventIndicator.color.WHITE_LIT;
			screenEventIndicators[14].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[14].lowerOnColor = EventIndicator.color.WHITE_LIT;

			screenEventIndicators[15] = Helper.createEventIndicator(266, 126, false, true);
			screenEventIndicators[15].upperText = "CONT\nDEORBIT";
			screenEventIndicators[15].lowerText = "MANUAL\nDEORBIT";
			screenEventIndicators[15].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[15].upperOnColor = EventIndicator.color.WHITE_LIT;
			screenEventIndicators[15].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[15].lowerOnColor = EventIndicator.color.WHITE_LIT;

			screenEventIndicators[16] = Helper.createEventIndicator(336, 126, false, true);
			screenEventIndicators[16].upperText = "VECTOR\nCOMP PROC";
			screenEventIndicators[16].lowerText = "FDO SW'S\nLIVE";
			screenEventIndicators[16].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[16].upperOnColor = EventIndicator.color.WHITE_LIT;
			screenEventIndicators[16].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[16].lowerOnColor = EventIndicator.color.WHITE_LIT;

			screenEventIndicators[17] = Helper.createEventIndicator(406, 126, false, true);
			screenEventIndicators[17].upperText = "RESTART\nTAPE";
			screenEventIndicators[17].lowerText = "RESIDUAL\nCOMP";
			screenEventIndicators[17].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[17].upperOnColor = EventIndicator.color.WHITE_LIT;
			screenEventIndicators[17].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[17].lowerOnColor = EventIndicator.color.AMBER_LIT;

			screenScrews[0] = Helper.CreateScrew(4, 36, true);
			screenScrews[1] = Helper.CreateScrew(500, 36, true);
			screenScrews[2] = Helper.CreateScrew(4, 134, true);
			screenScrews[3] = Helper.CreateScrew(500, 134, true);
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			for(int i = 0; i < screenEventIndicators.Count; i++)
			{
				screenEventIndicators[i].turnOnUpper();
				screenEventIndicators[i].turnOnLower();
			}
		}

		public override void resize() { }

		private void clickButton(object sender, EventArgs e, MocrButton button)
		{
		}
	}
}
