using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class NodesCRT : MocrScreen
	{
		double MET = 0;
		double UT = 0;
		IList<Node> nodes;
		ReferenceFrame frame;

		public NodesCRT(Screen form)
		{
			this.form = form;
			screenStreams = form.streamCollection;
			dataStorage = form.dataStorage;

			this.charSize = false;
			this.width = 674;
			this.height = 508;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 250; i++) screenLabels.Add(null); // Initialize Labels

			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateCRTLabel(0, 0, 7, 1, "SCR " + form.screenType.ToString());
			screenLabels[1] = Helper.CreateCRTLabel(30, 0, 30, 1, "NODES", 4);
			screenLabels[2] = Helper.CreateCRTLabel(0, 1.5, 12, 1, "LT: XX:XX:XX");
			screenLabels[3] = Helper.CreateCRTLabel(29, 1.5, 14, 1, "MET: XXX:XX:XX");

			for(int i = 1; i <= 2; i++)
			{
				for(int n = 0; n <= 19; n++)
				{
					screenLabels[(i * 20) + n] = Helper.CreateCRTLabel((i - 1) * 36, n + 4, 35, 1, "");
				}
			}

			// SEP LINE
			screenLabels[100] = Helper.CreateCRTLabel(35, 4, 1, 1, "┬");
			for (int i = 0; i < 26; i++)
			{
				screenLabels[101 + i] = Helper.CreateCRTLabel(35, 5 + i, 1, 1, "│");
			}
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
				nodes = screenStreams.GetData(DataType.control_nodes);
				frame = screenStreams.GetData(DataType.body_nonRotatingReferenceFrame);

				screenLabels[3].Text = "MET: " + Helper.timeString(MET, 3);

				for (int i = 1; i <= 2; i++)
				{
					if (nodes != null && nodes.Count >= i)
					{
						string metS = Helper.timeString(nodes[i - 1].UT - UT + MET, true, 3);
						string remS = Helper.timeString(nodes[i - 1].TimeTo, true, 3);

						screenLabels[(i * 20) + 0].Text = "────────────── NODE " + (i - 1).ToString() + " ─────────────";
						screenLabels[(i * 20) + 1].Text = "            MET:  " + metS;
						screenLabels[(i * 20) + 2].Text = "        TIME TO:  " + remS;
						screenLabels[(i * 20) + 4].Text = "       PROGRADE: " + Helper.prtlen(Helper.toFixed(nodes[i - 1].Prograde, 2), 10);
						screenLabels[(i * 20) + 5].Text = "         NORMAL: " + Helper.prtlen(Helper.toFixed(nodes[i - 1].Normal, 2), 10);
						screenLabels[(i * 20) + 6].Text = "         RADIAL: " + Helper.prtlen(Helper.toFixed(nodes[i - 1].Radial, 2), 10);

						screenLabels[(i * 20) + 8].Text = "       TOTAL ΔV: " + Helper.prtlen(Helper.toFixed(nodes[i - 1].DeltaV, 2), 10);
						screenLabels[(i * 20) + 9].Text = "        REM. ΔV: " + Helper.prtlen(Helper.toFixed(nodes[i - 1].RemainingDeltaV, 2), 10);


						Tuple<double, double, double> rot = Helper.RPYfromVector(nodes[i - 1].Direction(frame));
						screenLabels[(i * 20) + 11].Text = "           ┌──────── INER ────────┐";
						screenLabels[(i * 20) + 12].Text =  "             ROLL    PITCH    YAW";
						screenLabels[(i * 20) + 13].Text = "      ROT: " + Helper.prtlen(Helper.toFixed(Helper.rad2deg(rot.Item1), 2), 7)
							+ " " + Helper.prtlen(Helper.toFixed(Helper.rad2deg(rot.Item2), 2), 7)
							+ " " + Helper.prtlen(Helper.toFixed(Helper.rad2deg(rot.Item3), 2), 7);


						Tuple<double, double, double> dir = nodes[i - 1].BurnVector(frame);
						Tuple<double, double, double> rem = nodes[i - 1].RemainingBurnVector(frame);
						screenLabels[(i * 20) + 15].Text = "           ┌──────── INER ────────┐";
						screenLabels[(i * 20) + 16].Text = "              Vx      Vy      Vz";
						screenLabels[(i * 20) + 17].Text = "   VECTOR: " + Helper.prtlen(Helper.toFixed(dir.Item1, 2), 7) + " " + Helper.prtlen(Helper.toFixed(dir.Item3, 2), 7) + " " + Helper.prtlen(Helper.toFixed(dir.Item2, 2), 7);
						screenLabels[(i * 20) + 18].Text = " REM VECT: " + Helper.prtlen(Helper.toFixed(rem.Item1, 2), 7) + " " + Helper.prtlen(Helper.toFixed(rem.Item3, 2), 7) + " " + Helper.prtlen(Helper.toFixed(rem.Item2, 2), 7);
					}
					else
					{
						screenLabels[(i * 20) + 0].Text = "───────────────────────────────────";
						for (int n = 1; n <= 9; n++)
						{
							screenLabels[(i * 20) + n].Text = "";
						}
					}
				}
			}
		}
	}
}
