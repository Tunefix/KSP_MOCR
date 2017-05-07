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
	public abstract class MocrScreen
	{
		public List<Color> chartLineColors = new List<Color>();
		public List<Label> screenLabels = new List<Label>();
		public List<TextBox> screenInputs = new List<TextBox>();
		public List<MocrButton> screenButtons = new List<MocrButton>();
		public List<Indicator> screenIndicators = new List<Indicator>();
		public List<EngineIndicator> screenEngines = new List<EngineIndicator>();
		public List<VerticalMeter> screenVMeters = new List<VerticalMeter>();
		public List<SegDisp> screenSegDisps = new List<SegDisp>();
		public List<MocrDropdown> screenDropdowns = new List<MocrDropdown>();
		public List<Plot> screenCharts = new List<Plot>();
		public List<Map> screenMaps = new List<Map>();
		public FDAI screenFDAI;
		public OrbitGraph screenOrbit;

		protected StreamCollection screenStreams;

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
				case 3:
					return new ResourcesScreen(form);
				case 4:
					return new FDO(form);
				case 8:
					return new MapScreen(form);
				case 9:
					Console.WriteLine("MAKING TEST");
					return new TestScreen(form);
				case 50:
					return new Terrain(form);
				case 51:
					return new AltVel(form);
				case 52:
					return new Attitude(form);
				case 100:
					return new Pilot1(form);
				case 201:
					return new StatusReport(form, "BOOSTER");
				case 202:
					return new StatusReport(form, "RETRO");
				case 203:
					return new StatusReport(form, "FIDO");
				case 204:
					return new StatusReport(form, "GUIDO");
				case 205:
					return new StatusReport(form, "SURGEON");
				case 206:
					return new StatusReport(form, "CAPCOM");
				case 207:
					return new StatusReport(form, "EECOM");
				case 208:
					return new StatusReport(form, "GNC");
				case 209:
					return new StatusReport(form, "TELMU");
				case 210:
					return new StatusReport(form, "CONTROL");
				case 211:
					return new StatusReport(form, "INCO");
				case 212:
					return new StatusReport(form, "O&P");
				case 213:
					return new StatusReport(form, "AFLIGHT");
				case 214:
					return new StatusReport(form, "FAO");
				case 215:
					return new StatusReport(form, "NETWORK");
				case 220:
					return new StatusPanel(form);
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

		public virtual void keyDown(object sender, KeyEventArgs e) { }
		public virtual void keyUp(object sender, KeyEventArgs e) { }
		//abstract public void destroyStreams();

		public void destroy()
		{
			// Clear old lables
			foreach (Label label in screenLabels) { if (label != null) { label.Dispose(); } }
			screenLabels.Clear();
			screenLabels.TrimExcess();

			// Clear old Inputs
			foreach (TextBox box in screenInputs) { if (box != null) { box.Dispose(); } }
			screenInputs.Clear();
			screenInputs.TrimExcess();

			// Clear old Buttons
			foreach (MocrButton button in screenButtons) { if (button != null) { button.Dispose(); } }
			screenButtons.Clear();
			screenButtons.TrimExcess();

			// Clear old charts
			foreach (Plot chart in screenCharts) { if (chart != null) { chart.Dispose(); } }
			screenCharts.Clear();
			screenCharts.TrimExcess();

			// Clear old indicators
			foreach (Indicator indicator in screenIndicators) { if (indicator != null) { indicator.Dispose(); } }
			screenIndicators.Clear();
			screenIndicators.TrimExcess();

			// Clear old EngineIndicators
			foreach (EngineIndicator indicator in screenEngines) { if (indicator != null) { indicator.Dispose(); } }
			screenEngines.Clear();
			screenEngines.TrimExcess();
			
			// Clear old VerticalMeters
			foreach (VerticalMeter meter in screenVMeters) { if (meter != null) { meter.Dispose(); } }
			screenVMeters.Clear();
			screenVMeters.TrimExcess();
			
			// Clear old 7-segment displays
			foreach (SegDisp disp in screenSegDisps) { if (disp != null) { disp.Dispose(); } }
			screenSegDisps.Clear();
			screenSegDisps.TrimExcess();
			
			// Clear Drop Downs
			foreach (MocrDropdown drop in screenDropdowns) { if (drop != null) { drop.Dispose(); } }
			screenDropdowns.Clear();
			screenDropdowns.TrimExcess();
			
			// Clear Maps
			foreach (Map map in screenMaps) { if (map != null) { map.Dispose(); } }
			screenMaps.Clear();
			screenMaps.TrimExcess();

			// Clear old FDAI
			if (screenFDAI != null)
			{
				screenFDAI.Dispose();
				screenFDAI = null;
			}

			// Clear old Orbit
			if (screenOrbit != null)
			{
				screenOrbit.Dispose();
				screenOrbit = null;
			}

			// Clear all streams
			if (screenStreams != null)
			{
				screenStreams.CloseStreams();
				screenStreams = null;
			}
		}
	}
}
