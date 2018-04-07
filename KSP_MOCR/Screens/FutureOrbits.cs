using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class FutureOrbits : MocrScreen
	{
		double MET = 0;
		double UT = 0;
		Orbit currentOrbit;
		IList<Node> nodes;
		TreeNode<Orbit> futOrb;

		public FutureOrbits(Screen form)
		{
			this.form = form;
			this.screenStreams = form.streamCollection;

			this.charSize = false;
			this.width = 656;
			this.height = 494;
			this.updateRate = 1000;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 250; i++) screenLabels.Add(null); // Initialize Labels

			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateCRTLabel(0, 0, 6, 1, "SCR 13");
			screenLabels[1] = Helper.CreateCRTLabel(26, 0, 30, 1, "FUTURE ORBITS", 4);
			screenLabels[2] = Helper.CreateCRTLabel(0, 1.5, 12, 1, "LT: XX:XX:XX");
			screenLabels[3] = Helper.CreateCRTLabel(29, 1.5, 14, 1, "MET: XXX:XX:XX");

			// THE FOUR ORBIT COLS
			for(int c = 0; c < 4; c++)
			{
				int b = (c * 20) + 10;
				int r = 3;
				if (c > 1) r = 19;

				int l = 36 * c;
				if (c > 1) l = 36 * (c - 2);

				screenLabels[b + 0] = Helper.CreateCRTLabel(l, r + 0, 35, 1, "───────────────────────────────────");
				screenLabels[b + 1] = Helper.CreateCRTLabel(l, r + 1, 35, 1, "");
				screenLabels[b + 2] = Helper.CreateCRTLabel(l, r + 2, 35, 1, "");
				screenLabels[b + 3] = Helper.CreateCRTLabel(l, r + 3, 35, 1, "");
				screenLabels[b + 4] = Helper.CreateCRTLabel(l, r + 4, 35, 1, "");
				screenLabels[b + 5] = Helper.CreateCRTLabel(l, r + 5, 35, 1, "");
				screenLabels[b + 6] = Helper.CreateCRTLabel(l, r + 6, 35, 1, "");
				screenLabels[b + 7] = Helper.CreateCRTLabel(l, r + 7, 35, 1, "");
				screenLabels[b + 8] = Helper.CreateCRTLabel(l, r + 8, 35, 1, "");
				screenLabels[b + 9] = Helper.CreateCRTLabel(l, r + 9, 35, 1, "");
				screenLabels[b + 10] = Helper.CreateCRTLabel(l, r + 10, 35, 1, "");
				screenLabels[b + 11] = Helper.CreateCRTLabel(l, r + 11, 35, 1, "");
				screenLabels[b + 12] = Helper.CreateCRTLabel(l, r + 12, 35, 1, "");
				screenLabels[b + 13] = Helper.CreateCRTLabel(l, r + 13, 35, 1, "");
				screenLabels[b + 14] = Helper.CreateCRTLabel(l, r + 14, 35, 1, "");
				screenLabels[b + 15] = Helper.CreateCRTLabel(l, r + 15, 35, 1, "");
			}

			// VERTICAL LINES
			for(int c = 1; c < 2; c++)
			{
				int a = (c * 30) + 157;
				int b = (c * 30) + 97;

				screenLabels[a + 3] = Helper.CreateCRTLabel((36 * c) - 1, 3, 17, 1, "┬");
				screenLabels[b + 3] = Helper.CreateCRTLabel((36 * c) - 1, 19, 17, 1, "┼");
				for (int i = 4; i < 25; i++)
				{
					screenLabels[a + i] = Helper.CreateCRTLabel((36 * c) - 1, i + 16, 17, 1, "│");
					screenLabels[b + i] = Helper.CreateCRTLabel((36 * c) - 1, i, 17, 1, "│");
				}
			}
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			screenLabels[2].Text = "LT: " + Helper.timeString(DateTime.Now.TimeOfDay.TotalSeconds);

			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				MET = screenStreams.GetData(DataType.vessel_MET);
				UT = screenStreams.GetData(DataType.spacecenter_universial_time);
				currentOrbit = screenStreams.GetData(DataType.vessel_orbit);
				nodes = screenStreams.GetData(DataType.control_nodes);

				screenLabels[3].Text = "MET: " + Helper.timeString(MET, 3);

				updateOrbit(0, currentOrbit, 0, "────────── CURRENT ORBIT ──────────");

				// FUTURE ORBITS
				futOrb = new TreeNode<Orbit>(currentOrbit); // ROOT NODE (ORBIT)

				// Nodes
				if (nodes != null)
				{
					foreach (Node node in nodes)
					{
						TreeNode<Orbit> child = futOrb.AddChild(node.Orbit, node.UT, "NODE");
						addAllOrbits(node.Orbit, child);
					}
				}

				// SOI CHANGES
				if(nodes == null && currentOrbit != null && currentOrbit.NextOrbit != null)
				{
					addAllOrbits(currentOrbit, futOrb);
				}

				Console.WriteLine("==== TREE LOG ====");
				logTree(futOrb);

				// PRINT FIRST THREE NEXT ORBITS
				if (currentOrbit != null)
				{
					getNextOrbit(futOrb, currentOrbit.Body.Name, 1, 0, 0);
				}
			}
		}

		private void getNextOrbit(TreeNode<Orbit> node, string body, int i, int n, int e)
		{
			
			if (node != null && node.Children.Count > 0 && (e < node.Children.Count || n < nodes.Count))
			{
				TreeNode<Orbit> child = node.Children.ElementAt(e);

				if (child.Data.Body.Name != body) // NEW SOI
				{
					updateOrbit(i, child.Data, node.Data.TimeToSOIChange + MET, "─────────── SOI CHANGE ────────────");
					body = child.Data.Body.Name;
				}
				else if(n < nodes.Count)
				{
					updateOrbit(i, child.Data, nodes[n].UT - UT + MET, "────────── MANEOUVER NODE ─────────");
					n++;
				}
				else
				{
					updateOrbit(i, null, 0);
				}

				if (child.Children.Count > 0)
				{
					node = child;
					e = 0;
				}
				else if (node.Children.Count == n && node.Parent != null)
				{
					node = node.Parent;
					e++;
				}
			}
			else
			{
				updateOrbit(i, null, 0);
			}

			if(i < 3)
			{
				i++;
				getNextOrbit(node, body, i, n, e);
			}
		}

		private void logTree(TreeNode<Orbit> root) { logTree(root, 0); }
		private void logTree(TreeNode<Orbit> root, int indent)
		{
			if (root.Data != null)
			{
				string ind = "";
				for (int i = 0; i < indent; i++) ind += " ";

				Console.WriteLine(ind + root.Type + " - " + root.Data.Body.Name + ": " + root.Ut.ToString());

				foreach (TreeNode<Orbit> child in root.Children)
				{
					logTree(child, indent + 4);
				}
			}
		}

		private void addAllOrbits(Orbit orbit, TreeNode<Orbit> parent)
		{
			if(orbit.NextOrbit != null)
			{
				TreeNode<Orbit> child = parent.AddChild(orbit.NextOrbit, orbit.TimeToSOIChange, "SOIC");
				addAllOrbits(orbit.NextOrbit, child);
			}
		}

		public override void resize()
		{
		}

		private void updateOrbit(int c, Orbit orbit, double met) { updateOrbit(c, orbit, met, ""); }
		private void updateOrbit(int c, Orbit orbit, double met, string title)
		{
			int b = (c * 20) + 10;

			if(title != "")
			{
				screenLabels[b + 0].Text = title;
			}
			else
			{
				screenLabels[b + 0].Text = "───────────────────────────────────";
			}

			if(met != 0)
			{
				screenLabels[b + 2].Text = "               MET: " + Helper.prtlen(Helper.timeString(met), 12, Helper.Align.RIGHT);
			}
			else
			{
				screenLabels[b + 2].Text = "";
			}

			if (orbit != null)
			{


				screenLabels[b + 4].Text = "              BODY: " + Helper.prtlen(orbit.Body.Name.ToUpper(), 12, Helper.Align.RIGHT);

				screenLabels[b + 6].Text = "          APOAPSIS: " + Helper.prtlen(Math.Round(orbit.ApoapsisAltitude).ToString(), 12, Helper.Align.RIGHT);
				screenLabels[b + 7].Text = "         PERIAPSIS: " + Helper.prtlen(Math.Round(orbit.PeriapsisAltitude).ToString(), 12, Helper.Align.RIGHT);

				if (double.IsInfinity(orbit.Period))
				{
					screenLabels[b + 9].Text = "            PERIOD:       ESCAPE";
				}
				else
				{
					screenLabels[b + 9].Text = "            PERIOD: " + Helper.prtlen(Helper.timeString(orbit.Period, 2), 12, Helper.Align.RIGHT);
				}

				screenLabels[b + 11].Text = "      ECCENTRICITY: " + Helper.prtlen(Helper.toFixed(orbit.Eccentricity, 5), 12, Helper.Align.RIGHT);
				screenLabels[b + 12].Text = "       INCLINATION: " + Helper.prtlen(Helper.toFixed(Helper.rad2deg(orbit.Inclination), 3), 12, Helper.Align.RIGHT);
				screenLabels[b + 13].Text = "  LONG OF ASC NODE: " + Helper.prtlen(Helper.toFixed(Helper.rad2deg(orbit.LongitudeOfAscendingNode), 3), 12, Helper.Align.RIGHT);
				screenLabels[b + 14].Text = "  ARG OF PERIAPSIS: " + Helper.prtlen(Helper.toFixed(Helper.rad2deg(orbit.ArgumentOfPeriapsis), 2), 12, Helper.Align.RIGHT);
			}
			else
			{
				for(int i = 4; i < 15; i++)
				{
					screenLabels[b + i].Text = "";
				}
			}
		}
	}

	class TreeNode<Orbit>
	{
		public Orbit Data { get; set; }
		public Double Ut { get; set; }
		public string Type { get; set; }
		public TreeNode<Orbit> Parent { get; set; }
		public ICollection<TreeNode<Orbit>> Children { get; set; }

		public TreeNode(Orbit data)
		{
			this.Data = data;
			this.Children = new LinkedList<TreeNode<Orbit>>();
		}

		public TreeNode<Orbit> AddChild(Orbit child, double UT, string type)
		{
			TreeNode<Orbit> childNode = new TreeNode<Orbit>(child) { Parent = this, Ut = UT, Type = type };
			this.Children.Add(childNode);
			return childNode;
		}



	}
}
