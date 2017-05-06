using System;
using System.Collections.Generic;
using KRPC.Client.Services.KRPC;

namespace KSP_MOCR
{
	class ResourcesScreen : MocrScreen
	{
		public ResourcesScreen(Form1 form)
		{
			this.form = form;
			this.chartData = form.chartData;

			screenStreams = new StreamCollection(form.connection);

            this.updateRate = 1000;

			this.width = 120;
			this.height = 30;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{

			// Re-usable data variable for graph data
			//List<Dictionary<int, double?>> data = new List<Dictionary<int, double?>>();

			if (form.connected && form.krpc.CurrentGameScene == GameScene.Flight)
			{
				double MET = screenStreams.GetData(DataType.vessel_MET);
				float maxTotFuel = screenStreams.GetData(DataType.resource_total_max_liquidFuel);
				float curTotFuel = screenStreams.GetData(DataType.resource_total_amount_liquidFuel);
				float maxStgFuel = screenStreams.GetData(DataType.resource_stage_max_liquidFuel);
				float curStgFuel = screenStreams.GetData(DataType.resource_stage_amount_liquidFuel);
				
				float maxTotOx = screenStreams.GetData(DataType.resource_total_max_oxidizer);
				float curTotOx = screenStreams.GetData(DataType.resource_total_amount_oxidizer);
				float maxStgOx = screenStreams.GetData(DataType.resource_stage_max_oxidizer);
				float curStgOx = screenStreams.GetData(DataType.resource_stage_amount_oxidizer);

				float maxTotMono = screenStreams.GetData(DataType.resource_total_max_monoPropellant);
				float curTotMono = screenStreams.GetData(DataType.resource_total_amount_monoPropellant);
				float maxStgMono = screenStreams.GetData(DataType.resource_stage_max_monoPropellant);
				float curStgMono = screenStreams.GetData(DataType.resource_stage_amount_monoPropellant);

				float maxTotElec = screenStreams.GetData(DataType.resource_total_max_electricCharge);
				float curTotElec = screenStreams.GetData(DataType.resource_total_amount_electricCharge);
				float maxStgElec = screenStreams.GetData(DataType.resource_stage_max_electricCharge);
				float curStgElec = screenStreams.GetData(DataType.resource_stage_amount_electricCharge);

				screenLabels[1].Text = " LT: " + Helper.timeString(DateTime.Now.TimeOfDay.TotalSeconds);
				screenLabels[2].Text = "MET: " + Helper.timeString(MET, 3);

				String mTF = Helper.prtlen(Math.Round(maxTotFuel).ToString(), 7);
				String cTF = Helper.prtlen(Math.Round(curTotFuel).ToString(), 7);
				String pTF = Helper.prtlen(Math.Floor((curTotFuel / maxTotFuel) * 100).ToString(), 4) + "% ";
				String mSF = Helper.prtlen(Math.Round(maxStgFuel).ToString(), 7);
				String cSF = Helper.prtlen(Math.Round(curStgFuel).ToString(), 7);
				String pSF = Helper.prtlen(Math.Floor((curStgFuel / maxStgFuel) * 100).ToString(), 4) + "% ";
				String rF = "0";
				
				String mTO = Helper.prtlen(Math.Round(maxTotFuel).ToString(), 7);
				String cTO = Helper.prtlen(Math.Round(curTotFuel).ToString(), 7);
				String pTO = Helper.prtlen(Math.Floor((curTotFuel / maxTotFuel) * 100).ToString(), 4) + "% ";
				String mSO = Helper.prtlen(Math.Round(maxStgFuel).ToString(), 7);
				String cSO = Helper.prtlen(Math.Round(curStgFuel).ToString(), 7);
				String pSO = Helper.prtlen(Math.Floor((curStgFuel / maxStgFuel) * 100).ToString(), 4) + "% ";
				String rO = "0";

				String mTM = Helper.prtlen(Helper.toFixed(maxTotMono,2), 7);
				String cTM = Helper.prtlen(Helper.toFixed(curTotMono,2), 7);
				String pTM = Helper.prtlen(Math.Floor((curTotMono / maxTotMono) * 100).ToString(), 4) + "% ";
				String mSM = Helper.prtlen(Helper.toFixed(maxStgMono,2), 7);
				String cSM = Helper.prtlen(Helper.toFixed(curStgMono,2), 7);
				String pSM = Helper.prtlen(Math.Floor((curStgMono / maxStgMono) * 100).ToString(), 4) + "% ";
				String rM = "0";

				String mTE = Helper.prtlen(Helper.toFixed(maxTotElec, 2), 7);
				String cTE = Helper.prtlen(Helper.toFixed(curTotElec, 2), 7);
				String pTE = Helper.prtlen(Math.Floor((curTotElec / maxTotElec) * 100).ToString(), 4) + "% ";
				String mSE = Helper.prtlen(Helper.toFixed(maxStgElec, 2), 7);
				String cSE = Helper.prtlen(Helper.toFixed(curStgElec, 2), 7);
				String pSE = Helper.prtlen(Math.Floor((curStgElec / maxStgElec) * 100).ToString(), 4) + "% ";
				String rE = "0";



				screenLabels[6].Text = "       FUEL  ║" + mSF + "│" + cSF + "│" + pSF + "║" + mTF + "│" + cTF + "│" + pTF + "║" + rF;
				screenLabels[7].Text = "   OXIDIZER  ║" + mSO + "│" + cSO + "│" + pSO + "║" + mTO + "│" + cTO + "│" + pTO + "║" + rO;
				screenLabels[9].Text = "   MONOPROP  ║" + mSM + "│" + cSM + "│" + pSM + "║" + mTM + "│" + cTM + "│" + pTM + "║" + rM;
				screenLabels[11].Text = "ELECTRICITY  ║" + mSE + "│" + cSE + "│" + pSE + "║" + mTE + "│" + cTE + "│" + pTE + "║" + rE;

				/*
				data = new List<Dictionary<int, double?>>();
				data.Add(chartData["altitudeSpeed"]);
				screenCharts[0].setData(data, false);
				*/
			}
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenCharts.Add(null); // Initialize Charts
			for (int i = 0; i < 100; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs

			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix


			screenLabels[0] = Helper.CreateLabel(39, 0, 42, 1, "========== SPACECRAFT RESOURCES ==========");
			screenLabels[1] = Helper.CreateLabel(16, 1, 13); // Local Time
			screenLabels[2] = Helper.CreateLabel(0, 1, 14); // MET Time

			screenLabels[3] = Helper.CreateLabel(0, 3, 70, 1, "                       STAGE                  TOTAL         ");
			screenLabels[4] = Helper.CreateLabel(0, 4, 70, 1, "             ║  MAX     CUR     %   ║  MAX     CUR     %   ║  RATE");
			screenLabels[5] = Helper.CreateLabel(0, 5, 70, 1, "             ║       │       │      ║       │       │      ║ ");
			screenLabels[6] = Helper.CreateLabel(0, 6, 70, 1, "       FUEL  ║XXXXXXX│XXXXXXX│ 100% ║XXXXXXX│XXXXXXX│ 100% ║±XXX.XX");
			screenLabels[7] = Helper.CreateLabel(0, 7, 70, 1, "   OXIDIZER  ║XXXXXXX│XXXXXXX│ 100% ║XXXXXXX│XXXXXXX│ 100% ║±XXX.XX");
			screenLabels[8] = Helper.CreateLabel(0, 8, 70, 1, "             ║       │       │      ║       │       │      ║ ");
			screenLabels[9] = Helper.CreateLabel(0, 9, 70, 1, "   MONOPROP  ║XXXX.XX│XXXX.XX│ 100% ║XXXX.XX│XXXX.XX│ 100% ║±XXX.XX");
			screenLabels[10] = Helper.CreateLabel(0, 10, 70, 1, "             ║       │       │      ║       │       │      ║ ");
			screenLabels[11] = Helper.CreateLabel(0, 11, 70, 1, "ELECTRICITY  ║XXXX.XX│XXXX.XX│ 100% ║XXXX.XX│XXXX.XX│ 100% ║±XXX.XX");

			// Altitude vs. Orbital Speed
			//screenCharts[0] = Helper.CreatePlot(0, 1, 120, 30, 0, 3000, 0, -1);
		}
	}
}
