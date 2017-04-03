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
		List<Color?> statusFColor = new List<Color?>();
		List<Color?> statusBColor = new List<Color?>();

		public void setStatusColors(int id, Color f, Color b)
		{
			if(statusFColor.Count <= id)
			{
				while(statusFColor.Count <= id)
				{
					statusFColor.Add(null);
					statusBColor.Add(null);
				}
			}
			statusFColor[id] = f;
			statusBColor[id] = b;
		}

		public void setStatus(int id)
		{
			if(statusFColor.Count > id)
			{
				this.ForeColor = (Color)statusFColor[id];
				this.BackColor = (Color)statusBColor[id];
			}
		}
	}
}
