using System;
using System.Drawing;

namespace KSP_MOCR
{
	public class StatusPanel : MocrScreen
	{
		
		public StatusPanel(Screen form)
		{
			this.form = form;
			this.form.BackColor = Color.FromArgb(255, 62, 64, 68);

			Image myimage = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\darknoise.png");
			this.form.BackgroundImage = myimage;
			this.form.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Tile;

			this.width = 68;
			this.height = 11;
			this.updateRate = 200;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 20; i++) screenIndicators.Add(null); // Initialize Buttons
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			for (int i = 0; i < 4; i++) screenScrews.Add(null); // Initialize Screws

			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateLabel(4, 0.25, 60, 1.5, "CONTROLLER STATUS");
			screenLabels[0].type = CustomLabel.LabelType.ENGRAVED;

			screenIndicators[0] = Helper.CreateIndicator(4, 2, 10, 3, "BOOSTER");
			screenIndicators[0].indStyle = Indicator.style.BORDER;

			screenIndicators[1] = Helper.CreateIndicator(14, 2, 10, 3, "RETRO");
			screenIndicators[1].indStyle = Indicator.style.BORDER;

			screenIndicators[2] = Helper.CreateIndicator(24, 2, 10, 3, "FIDO");
			screenIndicators[2].indStyle = Indicator.style.BORDER;

			screenIndicators[3] = Helper.CreateIndicator(34, 2, 10, 3, "GUIDO");
			screenIndicators[3].indStyle = Indicator.style.BORDER;

			screenIndicators[4] = Helper.CreateIndicator(44, 2, 10, 3, "");
			screenIndicators[4].indStyle = Indicator.style.BORDER;

			screenIndicators[5] = Helper.CreateIndicator(54, 2, 10, 3, "");
			screenIndicators[5].indStyle = Indicator.style.BORDER;

			screenIndicators[6] = Helper.CreateIndicator(4, 5, 10, 3, "SURGEON");
			screenIndicators[6].indStyle = Indicator.style.BORDER;

			screenIndicators[7] = Helper.CreateIndicator(14, 5, 10, 3, "CAPCOM");
			screenIndicators[7].indStyle = Indicator.style.BORDER;

			screenIndicators[8] = Helper.CreateIndicator(24, 5, 10, 3, "EECOM");
			screenIndicators[8].indStyle = Indicator.style.BORDER;

			screenIndicators[9] = Helper.CreateIndicator(34, 5, 10, 3, "GNC");
			screenIndicators[9].indStyle = Indicator.style.BORDER;

			screenIndicators[10] = Helper.CreateIndicator(44, 5, 10, 3, "TELMU");
			screenIndicators[10].indStyle = Indicator.style.BORDER;
			
			screenIndicators[11] = Helper.CreateIndicator(54, 5, 10, 3, "CONTROL");
			screenIndicators[11].indStyle = Indicator.style.BORDER;
			
			screenIndicators[12] = Helper.CreateIndicator(4, 8, 10, 3, "INCO");
			screenIndicators[12].indStyle = Indicator.style.BORDER;
			
			screenIndicators[13] = Helper.CreateIndicator(14, 8, 10, 3, "O&P");
			screenIndicators[13].indStyle = Indicator.style.BORDER;
			
			screenIndicators[14] = Helper.CreateIndicator(24, 8, 10, 3, "AFLIGHT");
			screenIndicators[14].indStyle = Indicator.style.BORDER;
			
			screenIndicators[15] = Helper.CreateIndicator(34, 8, 10, 3, "FAO");
			screenIndicators[15].indStyle = Indicator.style.BORDER;
			
			screenIndicators[16] = Helper.CreateIndicator(44, 8, 10, 3, "NETWORK");
			screenIndicators[16].indStyle = Indicator.style.BORDER;

			screenIndicators[17] = Helper.CreateIndicator(54, 8, 10, 3, "");
			screenIndicators[17].indStyle = Indicator.style.BORDER;

			screenScrews[0] = Helper.CreateScrew(0.5, 2);
			screenScrews[1] = Helper.CreateScrew(64.5, 2);
			screenScrews[2] = Helper.CreateScrew(0.5, 8);
			screenScrews[3] = Helper.CreateScrew(64.5, 8);

			form.dataStorage.Pull();
		}
		
		public override void updateLocalElements(object sender, EventArgs e)
		{
			setIndicatorColor(screenIndicators[0], "BOOSTERS");
			setIndicatorColor(screenIndicators[1], "RETROS");
			setIndicatorColor(screenIndicators[2], "FIDOS");
			setIndicatorColor(screenIndicators[3], "GUIDOS");
			setIndicatorColor(screenIndicators[6], "SURGEONS");
			setIndicatorColor(screenIndicators[7], "CAPCOMS");
			setIndicatorColor(screenIndicators[8], "EECOMS");
			setIndicatorColor(screenIndicators[9], "GNCS");
			setIndicatorColor(screenIndicators[10], "TELMUS");
			setIndicatorColor(screenIndicators[11], "CONTROLS");
			setIndicatorColor(screenIndicators[12], "INCOS");
			setIndicatorColor(screenIndicators[13], "O&PS");
			setIndicatorColor(screenIndicators[14], "AFLIGHTS");
			setIndicatorColor(screenIndicators[15], "FAOS");
			setIndicatorColor(screenIndicators[16], "NETWORKS");

			this.form.Invalidate();
		}

		public override void resize() { }

		private void setIndicatorColor(Indicator ind, String key)
		{
			String colorData = form.dataStorage.getData(key);
			switch (colorData)
			{
				case "RED":
					ind.setStatus(Indicator.status.RED, true);
					break;
					
				case "AMBER":
					ind.setStatus(Indicator.status.AMBER, true);
					break;
					
				case "GREEN":
					ind.setStatus(Indicator.status.GREEN, true);
					break;
					
				case "BLANK":
					ind.setStatus(Indicator.status.OFF, false);
					break;
			}
		}
	}
}
