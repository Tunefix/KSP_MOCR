using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KSP_MOCR
{
	class Indicator : Label
	{
		readonly Dictionary<status, Color> FColor = new Dictionary<status, Color>()
		{
			{status.OFF, Color.FromArgb(200, 0, 0, 0)},
			{status.GREEN, Color.FromArgb(200, 0, 0, 0)},
			{status.RED, Color.FromArgb(200, 0, 0, 0)},
			{status.BLUE, Color.FromArgb(200, 0, 0, 0)},
			{status.AMBER, Color.FromArgb(200, 0, 0, 0)},
			{status.WHITE, Color.FromArgb(200, 0, 0, 0)}
		};

		readonly Dictionary<status, Color> BColor = new Dictionary<status, Color>()
		{
			{status.OFF, Color.FromArgb(255, 32, 32, 32)},
			{status.GREEN, Color.FromArgb(255, 32, 128, 32)},
			{status.RED, Color.FromArgb(255, 128, 32, 32)},
			{status.BLUE, Color.FromArgb(255, 32, 32, 128)},
			{status.AMBER, Color.FromArgb(255, 128, 100, 32)},
			{status.WHITE, Color.FromArgb(200, 255, 255, 253)}
		};
		
		public enum status { OFF, GREEN, RED, BLUE, AMBER, WHITE }
		
		public void setStatus(status s)
		{
			this.ForeColor = FColor[s];
			this.BackColor = BColor[s];
			this.Invalidate();
		}
	}
}
