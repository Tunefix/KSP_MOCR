using System;
namespace KSP_MOCR
{
	public class StatusPanel : MocrScreen
	{
		
		public StatusPanel(Form1 form)
		{
			this.form = form;

			this.width = 40;
			this.height = 16;
		}
		
		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 20; i++) screenIndicators.Add(null); // Initialize Buttons

			screenLabels[0] = Helper.CreateLabel(0, 0, 40, 1, Helper.prtlen("CONTROLLER STATUS", 40, Helper.Align.CENTER));

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
		}
		
		public override void updateLocalElements(object sender, EventArgs e)
		{
			
		}
	}
}
