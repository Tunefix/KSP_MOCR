using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class FDAIScreen : MocrScreen
	{
		enum FDAIMode { SURF, INER }
		FDAIMode FDAImode;

		Tuple<double, double, double, double> rot;
		Tuple<double, double, double> RPY;
		double[] lastRoll = { 0, 0, 0, 0, 0 };
		double[] lastPitch = { 0, 0, 0, 0, 0 };
		double[] lastYaw = { 0, 0, 0, 0, 0 };
		double Roll = 0;
		double Pitch = 0;
		double Yaw = 0;
		double errorScale = 15;
		double rateScale = 10;

		public FDAIScreen(Screen form)
		{
			this.form = form;
			this.chartData = form.form.chartData;

			screenStreams = form.streamCollection;
			dataStorage = form.dataStorage;

			this.charSize = false;
			this.width = 656;
			this.height = 494;
			this.updateRate = 50;
			this.timeWarning = 40;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			for (int i = 0; i < 2; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 5; i++) screenIndicators.Add(null); // Initialize Indicators
			for (int i = 0; i < 5; i++) screenButtons.Add(null); // Initialize Buttons

			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			// FDAI
			screenFDAI = new FDAI();
			screenFDAI.Font = form.buttonFont;
			screenFDAI.Location = new Point(100, 0);
			screenFDAI.Size = new Size(form.ClientSize.Width - 100, form.ClientSize.Height);
			form.Controls.Add(screenFDAI);

			// MODE BUTTONS/INDICATORS
			screenLabels[0] = Helper.CreateLabel(2.5, 0, 4, 1, "MODE");
			screenIndicators[0] = Helper.CreateIndicator(0, 1, 5, 1, "SURF");
			screenButtons[0] = Helper.CreateButton(5, 1, 4, 1, "");
			screenButtons[0].Click += (sender, e) => setMode(FDAIMode.SURF);
			screenIndicators[1] = Helper.CreateIndicator(0, 2, 5, 1, "INER");
			screenButtons[1] = Helper.CreateButton(5, 2, 4, 1, "");
			screenButtons[1].Click += (sender, e) => setMode(FDAIMode.INER);

			// RATE BUTTONS/INDICATORS
			screenLabels[1] = Helper.CreateLabel(0, 4, 9, 1, "E|R SCALE");
			screenIndicators[2] = Helper.CreateIndicator(0, 5, 5, 1, "5|1");
			screenButtons[2] = Helper.CreateButton(5, 5, 4, 1, "");
			screenButtons[2].Click += (sender, e) => setScale(5, 1);
			screenIndicators[3] = Helper.CreateIndicator(0, 6, 5, 1, "5|5");
			screenButtons[3] = Helper.CreateButton(5, 6, 4, 1, "");
			screenButtons[3].Click += (sender, e) => setScale(5, 5);
			screenIndicators[4] = Helper.CreateIndicator(0, 7, 5, 1, "15|10");
			screenButtons[4] = Helper.CreateButton(5, 7, 4, 1, "");
			screenButtons[4].Click += (sender, e) => setScale(15, 10);

			// Load PySSSMQ-DATA
			loadPySSSMQData();
		}

		public override void resize()
		{
			if (screenFDAI != null)
			{
				screenFDAI.Size = new Size(form.ClientSize.Width - 100, form.ClientSize.Height);
			}
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			// GET AND SET OFFSET ANGLES
			double.TryParse(dataStorage.getData("AGC_N20R1"), out double or);
			string ors = dataStorage.getData("AGC_N20R1S"); // NEG if negative
			if (ors == "NEG") or = or * -1;

			double.TryParse(dataStorage.getData("AGC_N20R2"), out double op);
			string ops = dataStorage.getData("AGC_N20R2S");
			if (ops == "NEG") op = op * -1;

			double.TryParse(dataStorage.getData("AGC_N20R3"), out double oy);
			string oys = dataStorage.getData("AGC_N20R3S");
			if (oys == "NEG") oy = oy * -1;

			screenFDAI.setOffset(or / 100.0, op / 100.0, oy / 100.0);


			// UPDATE ROTATION, ERRORS, AND RATES
			if (screenFDAI != null && form.form.connected)
			{
				if (FDAImode == FDAIMode.SURF)
				{
					rot = screenStreams.GetData(DataType.flight_rotation);
					Roll = screenStreams.GetData(DataType.flight_roll);
					Pitch = screenStreams.GetData(DataType.flight_pitch);
					Yaw = screenStreams.GetData(DataType.flight_heading);
				}
				else
				{
					rot = screenStreams.GetData(DataType.flight_inertial_rotation);
					Roll = screenStreams.GetData(DataType.flight_inertial_roll);
					Pitch = screenStreams.GetData(DataType.flight_inertial_pitch);
					Yaw = screenStreams.GetData(DataType.flight_inertial_yaw);
				}


				// GET TARGET ANGLES
				double.TryParse(dataStorage.getData("AGC_N23R1"), out double tr);
				string trs = dataStorage.getData("AGC_N23R1S");
				if (trs == "NEG") tr = tr * -1;

				double.TryParse(dataStorage.getData("AGC_N23R2"), out double tp);
				string tps = dataStorage.getData("AGC_N23R2S");
				if (tps == "NEG") tp = tp * -1;

				double.TryParse(dataStorage.getData("AGC_N23R3"), out double ty);
				string tys = dataStorage.getData("AGC_N23R3S");
				if (tys == "NEG") ty = ty * -1;


				RPY = Helper.RPYFromQuaternion(rot);

				// CALCULATE RATES
				double[] hRr = new double[5];
				double[] hPr = new double[5];
				double[] hYr = new double[5];
				for (int i = 0; i < 4; i++)
				{
					hRr[i] = (lastRoll[i + 1] - lastRoll[i]) / (this.updateRate / 1000f);
					hPr[i] = (lastPitch[i + 1] - lastPitch[i]) / (this.updateRate / 1000f);
					hYr[i] = (lastYaw[i + 1] - lastYaw[i]) / (this.updateRate / 1000f);
				}
				hRr[4] = (RPY.Item1 - lastRoll[4]) / (this.updateRate / 1000f);
				hPr[4] = (RPY.Item2 - lastPitch[4]) / (this.updateRate / 1000f);
				hYr[4] = (RPY.Item3 - lastYaw[4]) / (this.updateRate / 1000f);


				// SUM AND AVERAGE RATES
				double r = 0;
				double p = 0;
				double y = 0;
				for (int i = 0; i < 5; i++)
				{
					r += hRr[i];
					p += hPr[i];
					y += hYr[i];
				}

				r = r / 5.0;
				p = p / 5.0;
				y = y / 5.0;


				// Insert new value into rate arrays
				for(int i = 0; i < 4; i++)
				{
					lastRoll[i] = lastRoll[i + 1];
					lastPitch[i] = lastPitch[i + 1];
					lastYaw[i] = lastYaw[i + 1];
				}
				lastRoll[4] = RPY.Item1;
				lastPitch[4] = RPY.Item2;
				lastYaw[4] = RPY.Item3;

				// SCALE IS CHOSEN BETWEEN 1, 5, or 10 degrees pr second
				// CORRESPONDING ERROR NEEDLES SCALE IS 5, 5, and 15 degrees.

				screenFDAI.setRates(r, p, y, rateScale);

				

				double rollError = findError(tr / 100f, Roll, "ROLL");
				double pitchError = findError(tp / 100f, Pitch, "PITCH");
				double yawError = findError(ty / 100f, Yaw, "YAW");

				screenFDAI.setError(rollError, pitchError, yawError, errorScale);

				screenFDAI.setRotation(rot);
			}

			// UPDATE INDICATORS
			if (rateScale == 1) { screenIndicators[2].setStatus(Indicator.status.AMBER); } else { screenIndicators[2].setStatus(Indicator.status.OFF); }
			if (rateScale == 5) { screenIndicators[3].setStatus(Indicator.status.AMBER); } else { screenIndicators[3].setStatus(Indicator.status.OFF); }
			if (rateScale == 10) { screenIndicators[4].setStatus(Indicator.status.AMBER); } else { screenIndicators[4].setStatus(Indicator.status.OFF); }

			if (FDAImode == FDAIMode.SURF)
			{
				screenIndicators[0].setStatus(Indicator.status.AMBER);
				screenIndicators[1].setStatus(Indicator.status.OFF);
			}
			else
			{
				screenIndicators[0].setStatus(Indicator.status.OFF);
				screenIndicators[1].setStatus(Indicator.status.AMBER);
			}

			screenFDAI.Invalidate();
		}

		private void loadPySSSMQData()
		{
			// Load data
			int.TryParse(form.dataStorage.getData("N20R1"), out int oR);
			int.TryParse(form.dataStorage.getData("N20R2"), out int oP);
			int.TryParse(form.dataStorage.getData("N20R3"), out int oY);

			screenFDAI.offsetR = oR / 100f;
			screenFDAI.offsetP = oP / 100f;
			screenFDAI.offsetY = oY / 100f;

			string mode = dataStorage.getData("FDAI_MODE");
			switch(mode)
			{
				case "SURF":
					FDAImode = FDAIMode.SURF;
					break;
				case "INER":
					FDAImode = FDAIMode.INER;
					break;
				default:
					setMode(FDAIMode.SURF);
					break;
			}
		}

		private double findError(double target, double value, string axis)
		{
			double init = value - target;

			switch(axis)
			{
				case "ROLL":
					//if (init > 180) return -(init - 180);
					//if (init < -180) return -(init + 180);
					break;
				case "YAW":
					if (init > 180) return -(360 - init);
					if (init < -180) return -(360  + init);
					break;
			}
			return init;
		}

		private void setMode(FDAIMode mode)
		{
			FDAImode = mode;
			if(mode == FDAIMode.SURF)
			{
				dataStorage.setData("FDAI_MODE", "SURF");
			}
			else
			{
				dataStorage.setData("FDAI_MODE", "INER");
			}
		}

		private void setScale(double error, double rate)
		{
			errorScale = error;
			rateScale = rate;
		}
	}
}
