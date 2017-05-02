using System;
namespace KSP_MOCR
{
	public class StatusPanel : MocrScreen
	{
		
		public StatusPanel(Form1 form)
		{
			this.form = form;

			this.width = 60;
			this.height = 10;
			this.updateRate = 200;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 20; i++) screenIndicators.Add(null); // Initialize Buttons
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs

			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateLabel(0, 0, 60, 1, Helper.prtlen("CONTROLLER STATUS", 40, Helper.Align.CENTER));

			screenIndicators[0] = Helper.CreateIndicator(0, 1, 10, 3, "BOOSTER");
			screenIndicators[0].indStyle = Indicator.style.BORDER;

			screenIndicators[1] = Helper.CreateIndicator(10, 1, 10, 3, "RETRO");
			screenIndicators[1].indStyle = Indicator.style.BORDER;

			screenIndicators[2] = Helper.CreateIndicator(20, 1, 10, 3, "FIDO");
			screenIndicators[2].indStyle = Indicator.style.BORDER;

			screenIndicators[3] = Helper.CreateIndicator(30, 1, 10, 3, "GUIDO");
			screenIndicators[3].indStyle = Indicator.style.BORDER;

			screenIndicators[4] = Helper.CreateIndicator(0, 4, 10, 3, "SURGEON");
			screenIndicators[4].indStyle = Indicator.style.BORDER;

			screenIndicators[5] = Helper.CreateIndicator(10, 4, 10, 3, "CAPCOM");
			screenIndicators[5].indStyle = Indicator.style.BORDER;

			screenIndicators[6] = Helper.CreateIndicator(20, 4, 10, 3, "EECOM");
			screenIndicators[6].indStyle = Indicator.style.BORDER;

			screenIndicators[7] = Helper.CreateIndicator(30, 4, 10, 3, "GNC");
			screenIndicators[7].indStyle = Indicator.style.BORDER;

			screenIndicators[8] = Helper.CreateIndicator(40, 4, 10, 3, "TELMU");
			screenIndicators[8].indStyle = Indicator.style.BORDER;
			
			screenIndicators[9] = Helper.CreateIndicator(50, 4, 10, 3, "CONTROL");
			screenIndicators[9].indStyle = Indicator.style.BORDER;
			
			screenIndicators[10] = Helper.CreateIndicator(0, 7, 10, 3, "INCO");
			screenIndicators[10].indStyle = Indicator.style.BORDER;
			
			screenIndicators[11] = Helper.CreateIndicator(10, 7, 10, 3, "O&P");
			screenIndicators[11].indStyle = Indicator.style.BORDER;
			
			screenIndicators[12] = Helper.CreateIndicator(10, 7, 10, 3, "AFLIGHT");
			screenIndicators[12].indStyle = Indicator.style.BORDER;
			
			screenIndicators[13] = Helper.CreateIndicator(30, 7, 10, 3, "FAO");
			screenIndicators[13].indStyle = Indicator.style.BORDER;
			
			screenIndicators[14] = Helper.CreateIndicator(40, 7, 10, 3, "NETWORK");
			screenIndicators[14].indStyle = Indicator.style.BORDER;

			form.dataStorage.Pull();
		}
		
		public override void updateLocalElements(object sender, EventArgs e)
		{
			setIndicatorColor(screenIndicators[0], "BOOSTERS");
			setIndicatorColor(screenIndicators[1], "RETROS");
			setIndicatorColor(screenIndicators[2], "FIDOS");
			setIndicatorColor(screenIndicators[3], "GUIDOS");
			setIndicatorColor(screenIndicators[4], "SURGEONS");
			setIndicatorColor(screenIndicators[5], "CAPCOMS");
			setIndicatorColor(screenIndicators[6], "EECOMS");
			setIndicatorColor(screenIndicators[7], "GNCS");
			setIndicatorColor(screenIndicators[8], "TELMUS");
			setIndicatorColor(screenIndicators[9], "CONTROLS");
			setIndicatorColor(screenIndicators[10], "INCOS");
			setIndicatorColor(screenIndicators[11], "O&PS");
			setIndicatorColor(screenIndicators[12], "AFLIGHTS");
			setIndicatorColor(screenIndicators[13], "FAOS");
			setIndicatorColor(screenIndicators[14], "NETWORKS");
		}

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
