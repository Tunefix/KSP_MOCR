using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using KRPC.Client;
using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;
using System.Threading;
using System.Diagnostics;

namespace KSP_MOCR
{
	partial class Pilot1
	{
	public override void keyDown(object sender, KeyEventArgs e)
		{
			Console.WriteLine("KD: " + e.KeyCode.ToString());
			switch (e.KeyCode)
			{
				case Keys.V:
					screenButtons[50].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[50].Invalidate();
					screenButtons[50].PerformClick();
					break;
				case Keys.N:
					screenButtons[51].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[51].Invalidate();
					screenButtons[51].PerformClick();
					break;
				case Keys.Enter:	
				case Keys.E:
					screenButtons[67].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[67].Invalidate();
					screenButtons[67].PerformClick();
					break;	
				case Keys.C:
					screenButtons[64].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[64].Invalidate();
					screenButtons[64].PerformClick();
					break;
				case Keys.P:
					screenButtons[65].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[65].Invalidate();
					screenButtons[65].PerformClick();
					break;
				case Keys.R:
					screenButtons[68].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[68].Invalidate();
					screenButtons[68].PerformClick();
					break;
					
				case Keys.D0:
				case Keys.NumPad0:
					screenButtons[54].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[54].Invalidate();
					screenButtons[54].PerformClick();
					break;
				case Keys.D1:
				case Keys.NumPad1:
					screenButtons[57].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[57].Invalidate();
					screenButtons[57].PerformClick();
					break;
				case Keys.D2:
				case Keys.NumPad2:
					screenButtons[60].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[60].Invalidate();
					screenButtons[60].PerformClick();
					break;
				case Keys.D3:
				case Keys.NumPad3:
					screenButtons[63].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[63].Invalidate();
					screenButtons[63].PerformClick();
					break;
				case Keys.D4:
				case Keys.NumPad4:
					screenButtons[56].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[56].Invalidate();
					screenButtons[56].PerformClick();
					break;
				case Keys.D5:
				case Keys.NumPad5:
					screenButtons[59].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[59].Invalidate();
					screenButtons[59].PerformClick();
					break;
				case Keys.D6:
				case Keys.NumPad6:
					screenButtons[62].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[62].Invalidate();
					screenButtons[62].PerformClick();
					break;
				case Keys.D7:
				case Keys.NumPad7:
					screenButtons[55].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[55].Invalidate();
					screenButtons[55].PerformClick();
					break;
				case Keys.D8:
				case Keys.NumPad8:
					screenButtons[58].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[58].Invalidate();
					screenButtons[58].PerformClick();
					break;
				case Keys.D9:
				case Keys.NumPad9:
					screenButtons[61].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[61].Invalidate();
					screenButtons[61].PerformClick();
					break;
			}
		}
		
		public override void keyUp(object sender, KeyEventArgs e)
		{
			Console.WriteLine("KU: " + e.KeyCode.ToString());
			switch (e.KeyCode)
			{
				case Keys.V:
					screenButtons[50].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
				case Keys.N:
					screenButtons[51].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
				case Keys.Enter:	
				case Keys.E:
					screenButtons[67].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
				case Keys.C:
					screenButtons[64].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
				case Keys.P:
					screenButtons[65].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
				case Keys.R:
					screenButtons[68].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
					
				case Keys.D0:
				case Keys.NumPad0:
					screenButtons[54].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
				case Keys.D1:
				case Keys.NumPad1:
					screenButtons[57].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
				case Keys.D2:
				case Keys.NumPad2:
					screenButtons[60].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
				case Keys.D3:
				case Keys.NumPad3:
					screenButtons[63].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
				case Keys.D4:
				case Keys.NumPad4:
					screenButtons[56].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
				case Keys.D5:
				case Keys.NumPad5:
					screenButtons[59].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
				case Keys.D6:
				case Keys.NumPad6:
					screenButtons[62].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
				case Keys.D7:
				case Keys.NumPad7:
					screenButtons[55].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
				case Keys.D8:
				case Keys.NumPad8:
					screenButtons[58].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
				case Keys.D9:
				case Keys.NumPad9:
					screenButtons[61].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
					
			}
		}

		private void verbClick(object sender, EventArgs e)
		{
			enterVerb = true;
			verb = null;
		}
		
		private void nounClick(object sender, EventArgs e)
		{
			enterNoun = true;
			noun = null;
		}
		
		private void entrClick(object sender, EventArgs e)
		{
			// TODO: MAGIC!
			if (verb != null && noun != null)
			{
				activeVerb = int.Parse(verb);
				verb = null;
				activeNoun = int.Parse(noun);
				noun = null;
			}

			if (activeVerb != -1 && noun != null)
			{
				activeNoun = int.Parse(noun);
				noun = null;
			}

			if (verb == "37") // Run Program
			{
				enterNoun = true;
				noun = null;
			}

			if (verb == "69"){runVerb(69, 0);}// Reset DSKY
			if (verb == "35"){runVerb(35, 0);}// Lamp Test
		}
		
		private void proClick(object sender, EventArgs e)
		{
		}
		
		private void rsetClick(object sender, EventArgs e)
		{
		}
		
		private void clrClick(object sender, EventArgs e)
		{
			if (enterVerb) { verb = null; }
			if (enterNoun) { noun = null; }
			if (enterR1) { r1 = null; }
			if (enterR2) { r2 = null; }
			if (enterR3) { r3 = null; }
		}

		private void dskyNumber(object sender, EventArgs e, int n)
		{
			if (enterVerb)
			{
				if (verb == null)
				{
					verb = n.ToString();
				}
				else
				{
					if (verb.ToString().Length < 2)
					{
						verb = verb + n.ToString();
					}
				}

				if (verb.ToString().Length == 2)
				{
					enterVerb = false;
				}
			}
			else if (enterNoun)
			{
				if (noun == null)
				{
					noun = n.ToString();
				}
				else
				{
					if (noun.ToString().Length < 2)
					{
						noun = noun + n.ToString();
					}
				}

				if (noun.ToString().Length == 2)
				{
					enterNoun = false;
				}
			}
		}
	}
}
