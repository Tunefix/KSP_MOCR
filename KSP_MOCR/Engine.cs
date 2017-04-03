using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KSP_MOCR
{
	class EngineIndicator : Label
	{
		public Form1 form;

		public double offsetX;
		public double offsetY;

		public void setStatus(bool on)
		{
			if (on)
			{
				this.Image = form.engineOn;
				this.ForeColor = Color.FromArgb(255, 16, 16, 16);
			}
			else
			{
				this.Image = form.engineOff;
				this.ForeColor = form.foreColor;
			}
		}
	}
}
