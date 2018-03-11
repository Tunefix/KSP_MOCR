using System;
using System.Drawing;
using System.Threading;

namespace KSP_MOCR
{
	public class StatusReport : MocrScreen
	{
		private String positionCode;
		
		public StatusReport(Screen form, String positionCode)
		{
			this.positionCode = positionCode;
			
			this.form = form;
			this.form.BackColor = Color.FromArgb(255, 62, 64, 68);

			this.width = 34;
			this.height = 9;
		}
		
		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 3; i++) screenButtons.Add(null); // Initialize Buttons
			for (int i = 0; i < 4; i++) screenScrews.Add(null); // Initialize Screws

			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateLabel(3.5, 0.25, 27, 1.5, positionCode + " STATUS REPORT");
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

			screenScrews[0] = Helper.CreateScrew(0, 0);
			screenScrews[1] = Helper.CreateScrew(31, 0);
			screenScrews[2] = Helper.CreateScrew(0, 7.5);
			screenScrews[3] = Helper.CreateScrew(31, 7.5);

			form.dataStorage.Pull();
			Thread.Sleep(1000);
			setButtonColor(positionCode + "S");
		}
		
		public override void updateLocalElements(object sender, EventArgs e)
		{
			setButtonColor(positionCode + "S");
		}

		public override void resize() { }

		private void setButtonColor(String key)
		{
			String colorData = form.dataStorage.getData(key);
			switch (colorData)
			{
				case "RED":
					screenButtons[0].setLitState(true);
					screenButtons[1].setLitState(false);
					screenButtons[2].setLitState(false);
					break;
					
				case "AMBER":
					screenButtons[0].setLitState(false);
					screenButtons[1].setLitState(true);
					screenButtons[2].setLitState(false);
					break;
					
				case "GREEN":
					screenButtons[0].setLitState(false);
					screenButtons[1].setLitState(false);
					screenButtons[2].setLitState(true);
					break;

				case "BLANK":
				default:
					screenButtons[0].setLitState(false);
					screenButtons[1].setLitState(false);
					screenButtons[2].setLitState(false);
					break;
			}
		}

		private void clickButton(object sender, EventArgs e, MocrButton button)
		{
			bool lit = button.lit;

			// Reset all
			screenButtons[0].setLitState(false);
			screenButtons[1].setLitState(false);
			screenButtons[2].setLitState(false);

			if (!lit)
			{
				button.setLitState(true);
				form.dataStorage.setData(positionCode + "S", button.buttonColor.ToString());
			}
			else
			{
				form.dataStorage.setData(positionCode + "S", MocrButton.color.BLANK.ToString());
			}
		}
	}
}
