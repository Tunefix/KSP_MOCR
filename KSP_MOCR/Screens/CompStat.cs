using System;
namespace KSP_MOCR
{
	class CompStat : MocrScreen
	{
		public CompStat(Screen form)
		{
			this.form = form;
			this.chartData = form.form.chartData;

			this.width = 120;
			this.height = 30;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 100; i++) screenLabels.Add(null); // Initialize Labels

			screenLabels[10] = Helper.CreateLabel(1, 4, 30, 1, "N20");
		}
		
		public override void updateLocalElements(object sender, EventArgs e)
		{
			screenLabels[10].Text = "N20";
		}
	}
}
