using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class Debug : MocrScreen
	{
		double MET = 0;
		double UT = 0;
		ReferenceFrame frame;
		AutoPilot autoPilot;
		Control control;
		bool autoPilotState = false;
		bool RCS = false;

		public Debug(Screen form)
		{
			this.form = form;
			this.chartData = form.form.chartData;

			screenStreams = form.streamCollection;
			dataStorage = form.dataStorage;

			this.charSize = false;
			this.width = 656;
			this.height = 494;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 100; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 5; i++) screenInputs.Add(null); // Initialize Inputs
			for (int i = 0; i < 5; i++) screenButtons.Add(null); // Initialize Buttons

			screenLabels[0] = Helper.CreateCRTLabel(0, 0, 7, 1, "SCR " + form.screenType.ToString());
			screenLabels[1] = Helper.CreateCRTLabel(30, 0, 30, 1, "DEBUG", 4);
			screenLabels[2] = Helper.CreateCRTLabel(0, 1.5, 12, 1, "LT: XX:XX:XX");
			screenLabels[3] = Helper.CreateCRTLabel(29, 1.5, 14, 1, "MET: XXX:XX:XX");

			// AUTOPILOT STATUS/TOGGLE BUTTONS
			screenButtons[0] = Helper.CreateButton(0, 3, 10, 2, "AUTOPILOT", false, MocrButton.style.THIN_BORDER_LIGHT);
			screenButtons[0].setLightColor(MocrButton.color.RED);
			screenButtons[0].MouseClick += toggleAutopilot;

			screenButtons[1] = Helper.CreateButton(11, 3, 10, 2, "SAS", false, MocrButton.style.THIN_BORDER_LIGHT);
			screenButtons[1].setLightColor(MocrButton.color.AMBER);
			screenButtons[1].MouseClick += toggleSAS;

			screenButtons[2] = Helper.CreateButton(22, 3, 10, 2, "RCS", false, MocrButton.style.THIN_BORDER_LIGHT);
			screenButtons[2].setLightColor(MocrButton.color.AMBER);
			screenButtons[2].MouseClick += toggleRCS;

			screenLabels[4] = Helper.CreateLabel(0, 6, 22, 1, "DIRECTION UNIT VECTOR");
			screenLabels[5] = Helper.CreateLabel(0, 7, 3, 1, "Dx:");
			screenLabels[6] = Helper.CreateLabel(0, 8, 3, 1, "Dy:");
			screenLabels[7] = Helper.CreateLabel(0, 9, 3, 1, "Dz:");
			screenLabels[8] = Helper.CreateLabel(0, 10, 3, 1, " R:");
			screenInputs[0] = Helper.CreateInput(3, 7, 8, 1);
			screenInputs[1] = Helper.CreateInput(3, 8, 8, 1);
			screenInputs[2] = Helper.CreateInput(3, 9, 8, 1);
			screenInputs[3] = Helper.CreateInput(3, 10, 8, 1);

			screenButtons[3] = Helper.CreateButton(0, 12, 15, 1, "SET INER DIR", false, MocrButton.style.PUSH);
			screenButtons[3].MouseClick += setINER;

			screenButtons[4] = Helper.CreateButton(16, 12, 15, 1, "SET SURF DIR", false, MocrButton.style.PUSH);
			screenButtons[4].MouseClick += setSURF;

			screenLabels[9] = Helper.CreateLabel(0, 14, 30, 5, "SURF:\nX: UP\nY: NORTH\nZ: EAST");

			screenLabels[10] = Helper.CreateLabel(30, 14, 50, 5, "");
		}

		public override void resize()
		{
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			screenLabels[2].Text = "LT: " + Helper.timeString(DateTime.Now.TimeOfDay.TotalSeconds);

			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				MET = screenStreams.GetData(DataType.vessel_MET);
				UT = screenStreams.GetData(DataType.spacecenter_universial_time);
				frame = screenStreams.GetData(DataType.body_nonRotatingReferenceFrame);
				RCS = screenStreams.GetData(DataType.control_RCS);
				autoPilot = screenStreams.GetData(DataType.vessel_autoPilot);
				screenLabels[3].Text = "MET: " + Helper.timeString(MET, 3);

				if(autoPilotState)
				{
					screenButtons[0].setLitState(true);
				}
				else
				{
					screenButtons[0].setLitState(false);
				}

				if (autoPilot.SAS)
				{
					screenButtons[1].setLitState(true);
				}
				else
				{
					screenButtons[1].setLitState(false);
				}

				if (RCS)
				{
					screenButtons[2].setLitState(true);
				}
				else
				{
					screenButtons[2].setLitState(false);
				}

				Tuple<double, double, double> dir = screenStreams.GetData(DataType.flight_direction);
				Tuple<double, double, double> idir = screenStreams.GetData(DataType.flight_inertial_direction);
				string dirs = "SURF: ";
				dirs += Helper.toFixed(dir.Item1, 3).ToString();
				dirs += " :: " + Helper.toFixed(dir.Item2, 3).ToString();
				dirs += " :: " + Helper.toFixed(dir.Item3, 3).ToString();
				dirs += "\nINER: " + Helper.toFixed(idir.Item1, 3).ToString();
				dirs += " :: " + Helper.toFixed(idir.Item2, 3).ToString();
				dirs += " :: " + Helper.toFixed(idir.Item3, 3).ToString();

				Tuple<double, double, double, double> rot = screenStreams.GetData(DataType.flight_rotation);
				Tuple<double, double, double, double> irot = screenStreams.GetData(DataType.flight_inertial_rotation);
				dirs += "\n\nSURF: ";
				dirs += Helper.toFixed(rot.Item1, 3).ToString();
				dirs += " :: " + Helper.toFixed(rot.Item2, 3).ToString();
				dirs += " :: " + Helper.toFixed(rot.Item3, 3).ToString();
				dirs += " :: " + Helper.toFixed(rot.Item4, 3).ToString();

				dirs += "\nINER: ";
				dirs += Helper.toFixed(irot.Item1, 3).ToString();
				dirs += " :: " + Helper.toFixed(irot.Item2, 3).ToString();
				dirs += " :: " + Helper.toFixed(irot.Item3, 3).ToString();
				dirs += " :: " + Helper.toFixed(irot.Item4, 3).ToString();




				screenLabels[10].Text = dirs;
			}
		}

		private void toggleAutopilot(object sender, EventArgs e)
		{
			if (autoPilot != null)
			{
				if (autoPilotState)
				{
					autoPilotState = false;
					autoPilot.Disengage();
				}
				else
				{
					autoPilotState = true;
					autoPilot.Engage();
				}
			}
		}

		private void toggleSAS(object sender, EventArgs e)
		{
			if (form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				if (autoPilot.SAS)
				{
					autoPilot.SAS = false;
				}
				else
				{
					autoPilot.SAS = true;
				}
			}
		}

		private void toggleRCS(object sender, EventArgs e)
		{
			if (form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				if (RCS)
				{
					form.form.spaceCenter.ActiveVessel.Control.RCS = false;
				}
				else
				{
					form.form.spaceCenter.ActiveVessel.Control.RCS = true;
				}
			}
		}

		private void setINER(object sender, EventArgs e)
		{
			if (form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				autoPilot.ReferenceFrame = screenStreams.GetData(DataType.body_nonRotatingReferenceFrame);
				autoPilot.TargetDirection = getTuple();

				double r = 0;
				double.TryParse(screenInputs[3].Text, out r);
				autoPilot.TargetRoll = (float)r;
			}
		}

		private void setSURF(object sender, EventArgs e)
		{
			if (form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				autoPilot.ReferenceFrame = screenStreams.GetData(DataType.vessel_surfaceReferenceFrame);
				autoPilot.TargetDirection = getTuple();

				double r = 0;
				double.TryParse(screenInputs[3].Text, out r);
				autoPilot.TargetRoll = (float)r;
			}
		}

		private Tuple<double, double, double> getTuple()
		{
			double x = 0;
			double y = 0;
			double z = 0;

			double.TryParse(screenInputs[0].Text, out x);
			double.TryParse(screenInputs[1].Text, out y);
			double.TryParse(screenInputs[2].Text, out z);

			return new Tuple<double, double, double>(x, y, z);
		}
	}
}
