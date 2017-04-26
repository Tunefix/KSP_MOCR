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
			//Console.WriteLine("KD: " + e.KeyCode.ToString());
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
				case Keys.K:
					screenButtons[66].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[66].Invalidate();
					screenButtons[66].PerformClick();
					break;
					
				case Keys.Subtract:
				case Keys.OemMinus:
					screenButtons[53].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[53].Invalidate();
					screenButtons[53].PerformClick();
					break;
				case Keys.Add:
				case Keys.Oemplus:
					screenButtons[52].BackColor = Color.FromArgb(255, 16, 16, 16);
					screenButtons[52].Invalidate();
					screenButtons[52].PerformClick();
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
			//Console.WriteLine("KU: " + e.KeyCode.ToString());
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
				case Keys.K:
					screenButtons[66].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
					
				case Keys.Subtract:
				case Keys.OemMinus:
					screenButtons[53].BackColor = Color.FromArgb(255, 32, 32, 32);
					break;
				case Keys.Add:
				case Keys.Oemplus:
					screenButtons[52].BackColor = Color.FromArgb(255, 32, 32, 32);
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
			verb = "";
		}
		
		private void nounClick(object sender, EventArgs e)
		{
			enterNoun = true;
			noun = "";
		}
		
		private void keyRelClick(object sender, EventArgs e)
		{
			//Console.WriteLine("KeyRelease");
			keyRel = false;
			activeVerb = -1;
			activeNoun = -1;
			verb = null;
			noun = null;
		}
		
		private void entrClick(object sender, EventArgs e)
		{
			entrPress = true;

			//Console.WriteLine("B AV: " + activeVerb.ToString());
			//Console.WriteLine("B AN: " + activeNoun.ToString());
			//Console.WriteLine("B ve: " + verb);
			//Console.WriteLine("B no: " + noun);

			if (verb != null && noun != null)
			{
				activeVerb = int.Parse(verb);
				verb = null;
 				activeNoun = int.Parse(noun);
				noun = null;
				if (verb == "37")
				{
					runOnce = true;
				}
			}
			else if (activeVerb != -1 && noun != null)
			{
				activeNoun = int.Parse(noun);
				noun = null;
			}
			else if (verb == "37") // Run Program
			{
				enterNoun = true;
				noun = null;
			}
			else
			{
				if (verb != "" && verb != null)
				{
					if (activeNoun != -1)
					{
						runVerb(int.Parse(verb), activeNoun);
					}
					else
					{
						runVerb(int.Parse(verb), 0);
					}
				}
			}
			
			//Console.WriteLine("A AV: " + activeVerb.ToString());
			//Console.WriteLine("A AN: " + activeNoun.ToString());
			//Console.WriteLine("A ve: " + verb);
			//Console.WriteLine("A no: " + noun);
		}
		
		private void proClick(object sender, EventArgs e)
		{
		}
		
		private void rsetClick(object sender, EventArgs e)
		{
			oprError = false;
			screenIndicators[52].setStatus(Indicator.status.OFF);
		}
		
		private void plusClick(object sender, EventArgs e)
		{
			dskyNumber(sender, e, 10);
		}
		
		private void minusClick(object sender, EventArgs e)
		{
			dskyNumber(sender, e, 11);
		}
		
		private void clrClick(object sender, EventArgs e)
		{
			if (enterVerb) { verb = null; }
			if (enterNoun) { noun = null; }
			if (enterR1) { r1 = " "; }
			if (enterR2) { r2 = " "; }
			if (enterR3) { r3 = " "; }
		}

		private void dskyNumber(object sender, EventArgs e, int n)
		{
			if (enterVerb)
			{
				if (n >= 10) { oprError = true;}
				verb = enterData(n, verb, out enterVerb, 2);
			}
			else if (enterNoun)
			{
				if (n >= 10) { oprError = true;}
				noun = enterData(n, noun, out enterNoun, 2);
			}
			else if (enterR1)
			{
				if (n == 10) { r1sign = SegDisp.SignState.PLUS; }
				else if (n == 11) { r1sign = SegDisp.SignState.MINUS; }
				else { r1 = enterData(n, r1, out enterR1, 5); }
			}
			else if (enterR2)
			{
				if (n == 10) { r2sign = SegDisp.SignState.PLUS; }
				else if (n == 11) { r2sign = SegDisp.SignState.MINUS; }
				else { r2 = enterData(n, r2, out enterR2, 5); }
			}
			else if (enterR3)
			{
				if (n == 10) { r3sign = SegDisp.SignState.PLUS; }
				else if (n == 11) { r3sign = SegDisp.SignState.MINUS; }
				else { r3 = enterData(n, r3, out enterR3, 5); }
			}
		}

		private String enterData(int number, string field, out bool enterField, int size)
		{
			if (field == null || field == " ")
			{
				field = number.ToString();
			}
			else
			{
				field = field + number.ToString();
			}

			if (field.ToString().Length >= size)
			{
				enterField = false;
			}
			else
			{
				enterField = true;
			}

			return field;
		}
	}
}
