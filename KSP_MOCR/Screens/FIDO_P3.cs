using System;
using System.Drawing;
using System.Threading;

namespace KSP_MOCR
{
	public class FIDO_P3 : MocrScreen
	{
		/**
		 * PHASE CONTROL KEYBOARD
		 */

		public FIDO_P3 (Screen form)
		{
			this.form = form;
			this.form.BackColor = Color.FromArgb(255, 62, 64, 68);
			this.charSize = false;

			Image myimage = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\darknoise.png");
			this.form.BackgroundImage = myimage;
			this.form.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Tile;

			this.width = 532;
			this.height = 392;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 15; i++) screenButtons.Add(null); // Initialize Buttons
			for (int i = 0; i < 4; i++) screenScrews.Add(null); // Initialize Screws
			for (int i = 0; i < 3; i++) screenEventIndicators.Add(null); // Initialize EventIndicators


			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix


			screenLabels[0] = Helper.CreateLabel(162, 50, 208, 29, "COMPUTER PHASE", true, true);
			screenLabels[0].type = CustomLabel.LabelType.ENGRAVED;

			screenButtons[0] = Helper.CreateButton(56, 112, 70, 56, "NO EVENT", true);
			screenButtons[0].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[0].setLightColor(MocrButton.color.RED);
			screenButtons[0].Click += (sender, e) => clickButton(sender, e, screenButtons[0]);

			screenEventIndicators[0] = Helper.createEventIndicator(126, 112, false, true);
			screenEventIndicators[0].upperText = "RTCC ON";
			screenEventIndicators[0].lowerText = "LES ABORT";
			screenEventIndicators[0].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[0].upperOnColor = EventIndicator.color.WHITE_LIT;
			screenEventIndicators[0].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[0].lowerOnColor = EventIndicator.color.GREEN_LIT;

			screenButtons[1] = Helper.CreateButton(196, 112, 70, 56, "EVENT", true);
			screenButtons[1].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[1].setLightColor(MocrButton.color.AMBER);
			screenButtons[1].Click += (sender, e) => clickButton(sender, e, screenButtons[1]);

			screenButtons[2] = Helper.CreateButton(266, 112, 70, 56, "", true);
			screenButtons[2].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[2].setLightColor(MocrButton.color.BLANK);
			screenButtons[2].Click += (sender, e) => clickButton(sender, e, screenButtons[2]);

			screenEventIndicators[1] = Helper.createEventIndicator(336, 112, false, true);
			screenEventIndicators[1].upperText = "";
			screenEventIndicators[1].lowerText = "";
			screenEventIndicators[1].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[1].upperOnColor = EventIndicator.color.OFF;
			screenEventIndicators[1].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[1].lowerOnColor = EventIndicator.color.OFF;

			screenButtons[3] = Helper.CreateButton(406, 112, 70, 56, "", true);
			screenButtons[3].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[3].setLightColor(MocrButton.color.BLANK);
			screenButtons[3].Click += (sender, e) => clickButton(sender, e, screenButtons[2]);

			// ROW TWO
			screenButtons[4] = Helper.CreateButton(56, 168, 70, 56, "ABORT", true);
			screenButtons[4].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[4].setLightColor(MocrButton.color.BLANK);
			screenButtons[4].Click += (sender, e) => clickButton(sender, e, screenButtons[0]);

			screenButtons[5] = Helper.CreateButton(126, 168, 70, 56, "HOLD", true);
			screenButtons[5].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[5].setLightColor(MocrButton.color.BLANK);
			screenButtons[5].Click += (sender, e) => clickButton(sender, e, screenButtons[0]);

			screenButtons[6] = Helper.CreateButton(196, 168, 70, 56, "ORBIT", true);
			screenButtons[6].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[6].setLightColor(MocrButton.color.BLANK);
			screenButtons[6].Click += (sender, e) => clickButton(sender, e, screenButtons[0]);

			screenButtons[7] = Helper.CreateButton(266, 168, 70, 56, "ZERO LIFT", true);
			screenButtons[7].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[7].setLightColor(MocrButton.color.BLANK);
			screenButtons[7].Click += (sender, e) => clickButton(sender, e, screenButtons[0]);

			screenButtons[8] = Helper.CreateButton(336, 168, 70, 56, "HALF LIFT", true);
			screenButtons[8].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[8].setLightColor(MocrButton.color.BLANK);
			screenButtons[8].Click += (sender, e) => clickButton(sender, e, screenButtons[0]);

			screenButtons[9] = Helper.CreateButton(406, 168, 70, 56, "MAX LIFT", true);
			screenButtons[9].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[9].setLightColor(MocrButton.color.BLANK);
			screenButtons[9].Click += (sender, e) => clickButton(sender, e, screenButtons[0]);

			// ROW THREE
			screenButtons[10] = Helper.CreateButton(56, 224, 70, 56, "NO EVENT", true);
			screenButtons[10].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[10].setLightColor(MocrButton.color.RED);
			screenButtons[10].Click += (sender, e) => clickButton(sender, e, screenButtons[0]);

			screenEventIndicators[2] = Helper.createEventIndicator(126, 224, false, true);
			screenEventIndicators[2].upperText = "RTCC\nTHRUST ON";
			screenEventIndicators[2].lowerText = "SPS\nIGNITION";
			screenEventIndicators[2].upperOffColor = EventIndicator.color.OFF;
			screenEventIndicators[2].upperOnColor = EventIndicator.color.WHITE_LIT;
			screenEventIndicators[2].lowerOffColor = EventIndicator.color.OFF;
			screenEventIndicators[2].lowerOnColor = EventIndicator.color.GREEN_LIT;

			screenButtons[11] = Helper.CreateButton(196, 224, 70, 56, "EVENT", true);
			screenButtons[11].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[11].setLightColor(MocrButton.color.AMBER);
			screenButtons[11].Click += (sender, e) => clickButton(sender, e, screenButtons[0]);

			screenButtons[12] = Helper.CreateButton(266, 224, 70, 56, "S-IVB ENG\nIGN", true);
			screenButtons[12].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[12].setLightColor(MocrButton.color.GREEN);
			screenButtons[12].Click += (sender, e) => clickButton(sender, e, screenButtons[0]);

			screenButtons[13] = Helper.CreateButton(336, 224, 70, 56, "SPS\nIGNITION", true);
			screenButtons[13].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[13].setLightColor(MocrButton.color.GREEN);
			screenButtons[13].Click += (sender, e) => clickButton(sender, e, screenButtons[0]);

			screenButtons[14] = Helper.CreateButton(406, 224, 70, 56, "LM\nIGNITION", true);
			screenButtons[14].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[14].setLightColor(MocrButton.color.GREEN);
			screenButtons[14].Click += (sender, e) => clickButton(sender, e, screenButtons[0]);




			screenScrews[0] = Helper.CreateScrew(4, 71, true);
			screenScrews[1] = Helper.CreateScrew(500, 71, true);
			screenScrews[2] = Helper.CreateScrew(4, 295, true);
			screenScrews[3] = Helper.CreateScrew(500, 295, true);
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
		}

		public override void resize() { }

		private void clickButton(object sender, EventArgs e, MocrButton button)
		{
		}
	}
}
