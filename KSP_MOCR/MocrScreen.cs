using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using KRPC.Client;

namespace KSP_MOCR
{
	abstract class MocrScreen
	{
		public List<Color> chartLineColors = new List<Color>();
		public List<Label> screenLabels = new List<Label>();
		public List<TextBox> screenInputs = new List<TextBox>();
		public List<Button> screenButtons = new List<Button>();
		public List<Indicator> screenIndicators = new List<Indicator>();
		public List<EngineIndicator> screenEngines = new List<EngineIndicator>();
		public FDAI screenFDAI;

		public int width = 120; // in chars
		public int height = 30; // in rows

		public int updateRate = 1000;

		public Dictionary<String, Dictionary<int, Nullable<double>>> chartData;

		public Form1 form;

		public static MocrScreen Create(int id, Form1 form)
		{
			switch (id)
			{
				case 0:
					return new ConnectionScreen(form);
				case 1:
					return new AscentScreen(form);
				case 2:
					return new BoosterScreen(form);
				case 9:
					return new TestScreen(form);
				case 50:
					return new Terrain(form);
				case 100:
					return new Pilot1(form);
			}
			return null;
		}

		public void resizeForm()
		{
			int w = (int)Math.Ceiling(this.width * form.pxPrChar) + form.padding_left + form.padding_right;
			int h = (int)Math.Ceiling(this.height * form.pxPrLine) + form.padding_top + form.padding_bottom;
			this.form.ClientSize = new Size(w, h);
		}

		public void updateElements(object sender, EventArgs e)
		{
			updateLocalElements(sender, e);
		}

		abstract public void updateLocalElements(object sender, EventArgs e);
		abstract public void makeElements();
		abstract public void destroyStreams();

		public void destroy()
		{
			// Clear old lables
			foreach (Label label in screenLabels) { if (label != null) { label.Dispose(); } }
			screenLabels.Clear();

			// Clear old Inputs
			foreach (TextBox box in screenInputs) { if (box != null) { box.Dispose(); } }
			screenInputs.Clear();

			// Clear old Buttons
			foreach (Button button in screenButtons) { if (button != null) { button.Dispose(); } }
			screenButtons.Clear();

			// Clear old charts
			foreach (Chart chart in form.screenCharts) { if (chart != null) { chart.Dispose(); } }
			form.screenCharts.Clear();

			// Clear old indicators
			foreach (Indicator indicator in screenIndicators) { if (indicator != null) { indicator.Dispose(); } }
			screenIndicators.Clear();

			// Clear old EngineIndicators
			foreach (EngineIndicator indicator in screenEngines) { if (indicator != null) { indicator.Dispose(); } }
			screenEngines.Clear();

			// Clear old FDAI
			if (screenFDAI != null)
			{
				screenFDAI.Dispose();
				screenFDAI = null;
			}

			// Clear all streams
			//destroyStreams();
		}
	}
}
