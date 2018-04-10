using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class Nodes : MocrScreen
	{
		IList<KRPC.Client.Services.SpaceCenter.Node> nodes;
		int activeNode = -1; // -1 for no node

		public Nodes(Screen form)
		{

			this.form = form;
			this.form.BackColor = Color.FromArgb(255, 62, 64, 68);
			screenStreams = form.streamCollection;
			dataStorage = form.dataStorage;

			Image myimage = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\darknoise.png");
			this.form.BackgroundImage = myimage;
			this.form.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Tile;

			this.charSize = false;
			this.width = 532;
			this.height = 392;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 4; i++) screenScrews.Add(null); // Initialize Screws
			for (int i = 0; i < 20; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 60; i++) screenButtons.Add(null); // Initialize Buttons
			for (int i = 0; i < 40; i++) screenDigits.Add(null); // Initialize Digits

			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateLabel(39, 8, 454, 29, "MANEUVER NODE CONFIGURATION", true, true);
			screenLabels[0].type = CustomLabel.LabelType.ENGRAVED;

			string[] digits = new string[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " "};
			string[] signs = new string[] { " ", "+", "-"};


			// NODE SELECT
			screenLabels[1] = Helper.CreateLabel(64, 45, 50, 19, "┌NODE┐", true, true);

			screenDigits[0] = Helper.CreateConsoleDigit(80, 64, digits, true);
			screenDigits[0].setDigID(10);

			screenButtons[0] = Helper.CreateButton(78, 102, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[0].Click += (sender, e) => nextNode();
			screenButtons[1] = Helper.CreateButton(78, 129, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[1].Click += (sender, e) => prevNode();

			// MET
			screenLabels[2] = Helper.CreateLabel(233, 45, 235, 19, "┌───── MET OF NODE ──────┐", true, true);
			screenLabels[3] = Helper.CreateLabel(326, 80, 9, 19, ":", true, true);
			screenLabels[4] = Helper.CreateLabel(393, 80, 9, 19, ":", true, true);

			screenDigits[1] = Helper.CreateConsoleDigit(244, 64, digits, true);
			screenDigits[1].setDigID(10);
			screenDigits[2] = Helper.CreateConsoleDigit(271, 64, digits, true);
			screenDigits[2].setDigID(10);
			screenDigits[3] = Helper.CreateConsoleDigit(298, 64, digits, true);
			screenDigits[3].setDigID(10);

			screenDigits[4] = Helper.CreateConsoleDigit(338, 64, digits, true);
			screenDigits[4].setDigID(10);
			screenDigits[5] = Helper.CreateConsoleDigit(365, 64, digits, true);
			screenDigits[5].setDigID(10);

			screenDigits[6] = Helper.CreateConsoleDigit(405, 64, digits, true);
			screenDigits[6].setDigID(10);
			screenDigits[7] = Helper.CreateConsoleDigit(432, 64, digits, true);
			screenDigits[7].setDigID(10);

			screenButtons[2] = Helper.CreateButton(241, 102, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[2].MouseDown += (sender, e) => changeMET(360000);
			screenButtons[3] = Helper.CreateButton(241, 129, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[3].MouseDown += (sender, e) => changeMET(-360000);

			screenButtons[4] = Helper.CreateButton(268, 102, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[4].MouseDown += (sender, e) => changeMET(36000);
			screenButtons[5] = Helper.CreateButton(268, 129, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[5].MouseDown += (sender, e) => changeMET(-36000);

			screenButtons[6] = Helper.CreateButton(295, 102, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[6].MouseDown += (sender, e) => changeMET(3600);
			screenButtons[7] = Helper.CreateButton(295, 129, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[7].MouseDown += (sender, e) => changeMET(-3600);

			screenButtons[8] = Helper.CreateButton(335, 102, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[8].MouseDown += (sender, e) => changeMET(600);
			screenButtons[9] = Helper.CreateButton(335, 129, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[9].MouseDown += (sender, e) => changeMET(-600);

			screenButtons[10] = Helper.CreateButton(362, 102, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[10].MouseDown += (sender, e) => changeMET(60);
			screenButtons[11] = Helper.CreateButton(362, 129, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[11].MouseDown += (sender, e) => changeMET(-60);

			screenButtons[12] = Helper.CreateButton(402, 102, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[12].MouseDown += (sender, e) => changeMET(10);
			screenButtons[13] = Helper.CreateButton(402, 129, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[13].MouseDown += (sender, e) => changeMET(-10);

			screenButtons[14] = Helper.CreateButton(429, 102, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[14].MouseDown += (sender, e) => changeMET(1);
			screenButtons[15] = Helper.CreateButton(429, 129, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[15].MouseDown += (sender, e) => changeMET(-1);


			// PRO/RETRO
			screenLabels[5] = Helper.CreateLabel(64, 161, 200, 19, "┌──── PRO/RETRO ─────┐", true, true);
			screenLabels[6] = Helper.CreateLabel(214, 202, 9, 19, ".", true, true);

			screenDigits[8] = Helper.CreateConsoleDigit(78, 180, signs, true);
			screenDigits[8].setDigID(10);
			screenDigits[9] = Helper.CreateConsoleDigit(105, 180, digits, true);
			screenDigits[9].setDigID(10);
			screenDigits[10] = Helper.CreateConsoleDigit(132, 180, digits, true);
			screenDigits[10].setDigID(10);
			screenDigits[11] = Helper.CreateConsoleDigit(159, 180, digits, true);
			screenDigits[11].setDigID(10);
			screenDigits[12] = Helper.CreateConsoleDigit(186, 180, digits, true);
			screenDigits[12].setDigID(10);
			screenDigits[13] = Helper.CreateConsoleDigit(226, 180, digits, true);
			screenDigits[13].setDigID(10);

			screenButtons[16] = Helper.CreateButton(102, 218, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[16].MouseDown += (sender, e) => changeV(1000, "PRO");
			screenButtons[17] = Helper.CreateButton(102, 245, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[17].MouseDown += (sender, e) => changeV(-1000, "PRO");
			screenButtons[18] = Helper.CreateButton(129, 218, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[18].MouseDown += (sender, e) => changeV(100, "PRO");
			screenButtons[19] = Helper.CreateButton(129, 245, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[19].MouseDown += (sender, e) => changeV(-100, "PRO");
			screenButtons[20] = Helper.CreateButton(156, 218, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[20].MouseDown += (sender, e) => changeV(10, "PRO");
			screenButtons[21] = Helper.CreateButton(156, 245, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[21].MouseDown += (sender, e) => changeV(-10, "PRO");
			screenButtons[22] = Helper.CreateButton(183, 218, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[22].MouseDown += (sender, e) => changeV(1, "PRO");
			screenButtons[23] = Helper.CreateButton(183, 245, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[23].MouseDown += (sender, e) => changeV(-1, "PRO");
			screenButtons[24] = Helper.CreateButton(223, 218, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[24].MouseDown += (sender, e) => changeV(0.1, "PRO");
			screenButtons[25] = Helper.CreateButton(223, 245, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[25].MouseDown += (sender, e) => changeV(-0.1, "PRO");



			// RAD/ANTIRAD
			screenLabels[7] = Helper.CreateLabel(268, 161, 200, 19, "┌─── RAD/ANTIRAD ────┐", true, true);
			screenLabels[8] = Helper.CreateLabel(418, 202, 9, 19, ".", true, true);

			screenDigits[14] = Helper.CreateConsoleDigit(282, 180, signs, true);
			screenDigits[14].setDigID(10);
			screenDigits[15] = Helper.CreateConsoleDigit(309, 180, digits, true);
			screenDigits[15].setDigID(10);
			screenDigits[16] = Helper.CreateConsoleDigit(336, 180, digits, true);
			screenDigits[16].setDigID(10);
			screenDigits[17] = Helper.CreateConsoleDigit(363, 180, digits, true);
			screenDigits[17].setDigID(10);
			screenDigits[18] = Helper.CreateConsoleDigit(390, 180, digits, true);
			screenDigits[18].setDigID(10);
			screenDigits[19] = Helper.CreateConsoleDigit(430, 180, digits, true);
			screenDigits[19].setDigID(10);

			screenButtons[26] = Helper.CreateButton(306, 218, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[26].MouseDown += (sender, e) => changeV(1000, "RAD");
			screenButtons[27] = Helper.CreateButton(306, 245, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[27].MouseDown += (sender, e) => changeV(-1000, "RAD");
			screenButtons[28] = Helper.CreateButton(333, 218, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[28].MouseDown += (sender, e) => changeV(100, "RAD");
			screenButtons[29] = Helper.CreateButton(333, 245, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[29].MouseDown += (sender, e) => changeV(-100, "RAD");
			screenButtons[30] = Helper.CreateButton(360, 218, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[30].MouseDown += (sender, e) => changeV(10, "RAD");
			screenButtons[31] = Helper.CreateButton(360, 245, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[31].MouseDown += (sender, e) => changeV(-10, "RAD");
			screenButtons[32] = Helper.CreateButton(387, 218, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[32].MouseDown += (sender, e) => changeV(1, "RAD");
			screenButtons[33] = Helper.CreateButton(387, 245, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[33].MouseDown += (sender, e) => changeV(-1, "RAD");
			screenButtons[34] = Helper.CreateButton(427, 218, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[34].MouseDown += (sender, e) => changeV(0.1, "RAD");
			screenButtons[35] = Helper.CreateButton(427, 245, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[35].MouseDown += (sender, e) => changeV(-0.1, "RAD");


			// NORMAL/ANTINORM
			screenLabels[9] = Helper.CreateLabel(268, 276, 200, 19, "┌── NORN/ANTINORM ───┐", true, true);
			screenLabels[10] = Helper.CreateLabel(418, 317, 9, 19, ".", true, true);

			screenDigits[20] = Helper.CreateConsoleDigit(282, 295, signs, true);
			screenDigits[20].setDigID(10);
			screenDigits[21] = Helper.CreateConsoleDigit(309, 295, digits, true);
			screenDigits[21].setDigID(10);
			screenDigits[22] = Helper.CreateConsoleDigit(336, 295, digits, true);
			screenDigits[22].setDigID(10);
			screenDigits[23] = Helper.CreateConsoleDigit(363, 295, digits, true);
			screenDigits[23].setDigID(10);
			screenDigits[24] = Helper.CreateConsoleDigit(390, 295, digits, true);
			screenDigits[24].setDigID(10);
			screenDigits[25] = Helper.CreateConsoleDigit(430, 295, digits, true);
			screenDigits[25].setDigID(10);

			screenButtons[36] = Helper.CreateButton(306, 333, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[36].MouseDown += (sender, e) => changeV(1000, "NORM");
			screenButtons[37] = Helper.CreateButton(306, 360, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[37].MouseDown += (sender, e) => changeV(-1000, "NORM");
			screenButtons[38] = Helper.CreateButton(333, 333, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[38].MouseDown += (sender, e) => changeV(100, "NORM");
			screenButtons[39] = Helper.CreateButton(333, 360, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[39].MouseDown += (sender, e) => changeV(-100, "NORM");
			screenButtons[40] = Helper.CreateButton(360, 333, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[40].MouseDown += (sender, e) => changeV(10, "NORM");
			screenButtons[41] = Helper.CreateButton(360, 360, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[41].MouseDown += (sender, e) => changeV(-10, "NORM");
			screenButtons[42] = Helper.CreateButton(387, 333, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[42].MouseDown += (sender, e) => changeV(1, "NORM");
			screenButtons[43] = Helper.CreateButton(387, 360, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[43].MouseDown += (sender, e) => changeV(-1, "NORM");
			screenButtons[44] = Helper.CreateButton(427, 333, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[44].MouseDown += (sender, e) => changeV(0.1, "NORM");
			screenButtons[45] = Helper.CreateButton(427, 360, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[45].MouseDown += (sender, e) => changeV(-0.1, "NORM");


			// ADD/DELETE NODE
			screenButtons[50] = Helper.CreateButton(130, 65, 84, 38, "ADD NODE", true);
			screenButtons[50].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[50].Click += (sender, e) => addNode(600);
			screenButtons[51] = Helper.CreateButton(130, 105, 84, 38, "REM NODE", true);
			screenButtons[51].buttonStyle = MocrButton.style.LIGHT;
			screenButtons[51].Click += (sender, e) => remNode();


			// SELECT TARGET
			/*
			string[] targets = getTargetList(true);
			screenDigits[28] = Helper.CreateConsoleDigit(78, 333, targets, true);
			screenDigits[28].setDigID(0);
			screenButtons[50] = Helper.CreateButton(223, 333, 28, 28, "+", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[50].MouseDown += (sender, e) => changeTarget(1);
			screenButtons[51] = Helper.CreateButton(223, 360, 28, 28, "-", true, Helper.ButtonType.TINY_PUSH);
			screenButtons[51].MouseDown += (sender, e) => changeTarget(-1);
			/**/


			screenScrews[0] = Helper.CreateScrew(4, 71, true);
			screenScrews[1] = Helper.CreateScrew(500, 71, true);
			screenScrews[2] = Helper.CreateScrew(4, 295, true);
			screenScrews[3] = Helper.CreateScrew(500, 295, true);
		}

		public override void resize()
		{
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				nodes = screenStreams.GetData(DataType.control_nodes);

				if(nodes != null && nodes.Count > 0 && activeNode == -1)
				{
					activeNode = 0;
				}
				else if(nodes == null || nodes.Count == 0)
				{
					activeNode = -1;
				}
				else if(activeNode >= nodes.Count)
				{
					activeNode = nodes.Count - 1;
				}


				if(activeNode != -1)
				{
					KRPC.Client.Services.SpaceCenter.Node node = nodes[activeNode];

					double liftoff = screenStreams.GetData(DataType.spacecenter_universial_time) - screenStreams.GetData(DataType.vessel_MET);

					screenDigits[0].setDigID(activeNode);

					// SET MET TIME
					double MET = node.UT - liftoff;

					double hour100 = Math.Floor(MET / 360000);
					double hour10 = Math.Floor((MET - (hour100 * 360000)) / 36000);
					double hour1 = Math.Floor((MET - (hour100 * 360000) - (hour10 * 36000)) / 3600);
					double min10 = Math.Floor((MET - (hour100 * 360000) - (hour10 * 36000) - (hour1 * 3600)) / 600);
					double min1 = Math.Floor((MET - (hour100 * 360000) - (hour10 * 36000) - (hour1 * 3600) - (min10 * 600)) / 60);
					double sec10 = Math.Floor((MET - (hour100 * 360000) - (hour10 * 36000) - (hour1 * 3600) - (min10 * 600) - (min1 * 60)) / 10);
					double sec1 = Math.Floor((MET - (hour100 * 360000) - (hour10 * 36000) - (hour1 * 3600) - (min10 * 600) - (min1 * 60) - (sec10 * 10)));

					screenDigits[1].setDigID((int)hour100);
					screenDigits[2].setDigID((int)hour10);
					screenDigits[3].setDigID((int)hour1);
					screenDigits[4].setDigID((int)min10);
					screenDigits[5].setDigID((int)min1);
					screenDigits[6].setDigID((int)sec10);
					screenDigits[7].setDigID((int)sec1);

					// GET PROGRADE/RETROGRADE
					double deltaV = node.Prograde;

					// Set sign
					if(deltaV >= 0)
					{
						screenDigits[8].setDigID(1);
					}
					else
					{
						screenDigits[8].setDigID(2);
					}

					deltaV = Math.Abs(deltaV);

					double deltaV1000 = Math.Floor(deltaV / 1000);
					double deltaV100 = Math.Floor((deltaV - (deltaV1000 * 1000)) / 100);
					double deltaV10 = Math.Floor((deltaV - (deltaV1000 * 1000) - (deltaV100 * 100)) / 10);
					double deltaV1 = Math.Floor((deltaV - (deltaV1000 * 1000) - (deltaV100 * 100) - (deltaV10 * 10)));
					double deltaVD = Math.Floor((deltaV - (deltaV1000 * 1000) - (deltaV100 * 100) - (deltaV10 * 10) - deltaV1) * 10);

					screenDigits[9].setDigID((int)deltaV1000);
					screenDigits[10].setDigID((int)deltaV100);
					screenDigits[11].setDigID((int)deltaV10);
					screenDigits[12].setDigID((int)deltaV1);
					screenDigits[13].setDigID((int)deltaVD);


					// GET RADIAL/ANTIRADIAL
					deltaV = node.Radial;

					// Set sign
					if (deltaV >= 0)
					{
						screenDigits[14].setDigID(1);
					}
					else
					{
						screenDigits[14].setDigID(2);
					}

					deltaV = Math.Abs(deltaV);

					deltaV1000 = Math.Floor(deltaV / 1000);
					deltaV100 = Math.Floor((deltaV - (deltaV1000 * 1000)) / 100);
					deltaV10 = Math.Floor((deltaV - (deltaV1000 * 1000) - (deltaV100 * 100)) / 10);
					deltaV1 = Math.Floor((deltaV - (deltaV1000 * 1000) - (deltaV100 * 100) - (deltaV10 * 10)));
					deltaVD = Math.Floor((deltaV - (deltaV1000 * 1000) - (deltaV100 * 100) - (deltaV10 * 10) - deltaV1) * 10);

					screenDigits[15].setDigID((int)deltaV1000);
					screenDigits[16].setDigID((int)deltaV100);
					screenDigits[17].setDigID((int)deltaV10);
					screenDigits[18].setDigID((int)deltaV1);
					screenDigits[19].setDigID((int)deltaVD);



					// GET NORMAL/ANTINORMAL
					deltaV = node.Normal;

					// Set sign
					if (deltaV >= 0)
					{
						screenDigits[20].setDigID(1);
					}
					else
					{
						screenDigits[20].setDigID(2);
					}

					deltaV = Math.Abs(deltaV);

					deltaV1000 = Math.Floor(deltaV / 1000);
					deltaV100 = Math.Floor((deltaV - (deltaV1000 * 1000)) / 100);
					deltaV10 = Math.Floor((deltaV - (deltaV1000 * 1000) - (deltaV100 * 100)) / 10);
					deltaV1 = Math.Floor((deltaV - (deltaV1000 * 1000) - (deltaV100 * 100) - (deltaV10 * 10)));
					deltaVD = Math.Floor((deltaV - (deltaV1000 * 1000) - (deltaV100 * 100) - (deltaV10 * 10) - deltaV1) * 10);

					screenDigits[21].setDigID((int)deltaV1000);
					screenDigits[22].setDigID((int)deltaV100);
					screenDigits[23].setDigID((int)deltaV10);
					screenDigits[24].setDigID((int)deltaV1);
					screenDigits[25].setDigID((int)deltaVD);
				}
				else
				{
					// NULL ALL DIGITS
					for (int i = 0; i <= 25; i++)
					{
						if(i == 8 || i == 14 || i == 20)
						{
							screenDigits[i].setDigID(0); // THE PRO/NORM/RAD SIGNS
						}
						else
						{
							screenDigits[i].setDigID(10);
						}
					}
				}
			}
		}

		private void addNode(int futureSecs)
		{
			if (form.form.spaceCenter != null && form.form.spaceCenter.ActiveVessel != null)
			{
				double ut = form.form.spaceCenter.UT + futureSecs;
				form.form.spaceCenter.ActiveVessel.Control.AddNode(ut);
			}
		}

		private void remNode()
		{
			if (activeNode != -1)
			{
				form.form.spaceCenter.ActiveVessel.Control.Nodes[activeNode].Remove();
			}
		}

		private void nextNode()
		{
			if(activeNode + 1 < nodes.Count)
			{
				activeNode++;
			}
		}

		private void prevNode()
		{
			if (activeNode > 0)
			{
				activeNode--;
			}
		}

		private void changeMET(int secs)
		{
			if (activeNode != -1)
			{
				form.form.spaceCenter.ActiveVessel.Control.Nodes[activeNode].UT += secs;
				updateLocalElements(null, null);
			}
		}

		private void changeTarget(int dir)
		{
			if(dir == 1)
			{
				// UP
				screenDigits[28].digInc();
			}
			else if(dir == -1)
			{
				// DOWN
				screenDigits[28].digDec();
			}
			dataStorage.setData("target", screenDigits[28].getCurrentDigit());
		}


		private void changeV(double vs, string dir)
		{
			if (activeNode != -1)
			{
				switch (dir)
				{
					case "PRO":
						form.form.spaceCenter.ActiveVessel.Control.Nodes[activeNode].Prograde += vs;
						break;
					case "RAD":
						form.form.spaceCenter.ActiveVessel.Control.Nodes[activeNode].Radial += vs;
						break;
					case "NORM":
						form.form.spaceCenter.ActiveVessel.Control.Nodes[activeNode].Normal += vs;
						break;
				}
				updateLocalElements(null, null);
			}
		}

		private string[] getTargetList(bool withBlankAtIndex0)
		{
			// The possible targets are somewhat simplified to:
			//  * Satellites around the current body being orbited (Planets/Moons)
			//  * The parent of the body currently being orbited

			List<string> targets = new List<string>();

			CelestialBody body = screenStreams.GetData(DataType.orbit_celestialBody);

			// Blank index 0
			if (withBlankAtIndex0)
			{
				targets.Add(" ");
			}

			// Satellites
			foreach (CelestialBody sat in body.Satellites)
			{
				targets.Add(sat.Name);
			}

			// Parent
			if(body.Orbit.Body != null)
			{
				targets.Add(body.Orbit.Body.Name);
			}

			return targets.ToArray();
		}
	}
}
