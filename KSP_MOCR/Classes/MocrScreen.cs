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
		public List<CustomLabel> screenLabels = new List<CustomLabel>();
		public List<TextBox> screenInputs = new List<TextBox>();
		public List<MocrButton> screenButtons = new List<MocrButton>();
		public List<Indicator> screenIndicators = new List<Indicator>();
		public List<EngineIndicator> screenEngines = new List<EngineIndicator>();
		public List<VerticalMeter> screenVMeters = new List<VerticalMeter>();
		public List<SegDisp> screenSegDisps = new List<SegDisp>();
		public List<MocrDropdown> screenDropdowns = new List<MocrDropdown>();
		public List<Plot> screenCharts = new List<Plot>();
		public List<Map> screenMaps = new List<Map>();
		public List<Screw> screenScrews = new List<Screw>();
		public List<EventIndicator> screenEventIndicators = new List<EventIndicator>();
		public FDAI screenFDAI;
		public OrbitGraph screenOrbit;

		public StreamCollection screenStreams;
		public DataStorage dataStorage;

		public int width = 120; // in chars
		public int height = 30; // in rows
		public bool charSize = true; // Wheter size and positions are in chars/rows or pixels(without padding)

		private static int screenType;
		private static string idstr;
		private static string name;

		public int updateRate = 1000;
		public int timeWarning = 250;

		public Dictionary<String, List<KeyValuePair<double, double?>>> chartData;

		public Screen form;

		private Object updateLock = new System.Object();
		

		public static MocrScreen Create(int id, Screen form)
		{
			screenType = id;
			idstr = id.ToString();
			name = "";
			MocrScreen scr = null;
			switch (id)
			{
				case 0:
					scr = new ConnectionScreen(form);
					name = "HELP";
					break;
				case 1:
					scr = new AscentScreen(form);
					name = "OVERVIEW";
					break;
				case 2:
					scr = new BoosterScreen(form);
					name = "BOOSTER";
					break;
				case 3:
					scr = new ResourcesScreen(form);
					name = "CONSUMABLES";
					break;
				case 4:
					scr = new OrbitView(form);
					name = "SPACECRAFT ORBIT (INCLINATION NOT INCLUDED)";
					break;
				case 5:
					scr = new SingleOrbit(form);
					name = "ORBITAL DATA";
					break;
				case 6:
					scr = new Electrical(form);
					name = "ELECTRICAL SYSTEMS";
					break;
				case 7:
					scr = new Temperature(form);
					name = "TEMPERATURE / ABLATORS / RADIATORS";
					break;
				case 8:
					scr = new MapScreen(form);
					name = "MAP";
					break;
				case 9:
					Console.WriteLine("MAKING TEST");
					scr = new TestScreen(form);
					name = "TEST";
					break;
				case 11:
					scr = new StreamsScreen(form);
					name = "CURRENT STREAMS FROM KRPC";
					break;
				case 12:
					scr = new DataStorageScreen(form);
					name = "CURRENT DATA IN DATA-STORAGE (PySSSMQ)";
					break;
				case 40:
					scr = new FDO(form);
					name = "FIDO";
					break;
				case 50:
					scr = new Terrain(form);
					name = "";
					break;
				case 51:
					scr = new AltVel(form);
					name = "";
					break;
				case 52:
					scr = new Attitude(form);
					name = "";
					break;
				case 53:
					scr = new TApoVel(form);
					name = "";
					break;
				case 54:
					scr = new AltRange(form);
					name = "";
					break;
				case 55:
					scr = new HvsHdot(form);
					name = "";
					break;
				case 56:
					scr = new FpaVel(form);
					name = "";
					break;
				case 60:
					scr = new FIDO_P5(form);
					name = "FIDO - EVENT INDICATOR #1";
					break;
				case 61:
					scr = new FIDO_P3(form);
					name = "FIDO - PHASE CONTROL KEYBORAD";
					break;
				case 70:
					scr = new EventPanelEECOM1(form);
					name = "Event Indicator - EECOM #1";
					break;
				case 91:
					scr = new CrtTest(form, 1);
					name = "CRT TEST SCREEN - FONT SIZE 1";
					break;
				case 92:
					scr = new CrtTest(form, 2);
					name = "CRT TEST SCREEN - FONT SIZE 2";
					break;
				case 93:
					scr = new CrtTest(form, 3);
					name = "CRT TEST SCREEN - FONT SIZE 3";
					break;
				case 94:
					scr = new CrtTest(form, 4);
					name = "CRT TEST SCREEN - FONT SIZE 4";
					break;
				case 95:
					scr = new CrtTest(form, 5);
					name = "CRT TEST SCREEN - FONT SIZE 5";
					break;
				case 99:
					scr = new CrtTest2(form, 5);
					name = "CRT TEST SCREEN #2";
					break;
				/*case 100:
					scr = new Pilot1(form);
					name = "";
					break;*/
				case 101:
					scr = new FDAIScreen(form);
					name = "FDAI";
					break;
				case 102:
					scr = new DSKYScreen(form);
					name = "DSKY/AGC";
					break;
				case 201:
					scr = new StatusReport(form, "BOOSTER");
					name = "";
					break;
				case 202:
					scr = new StatusReport(form, "RETRO");
					name = "";
					break;
				case 203:
					scr = new StatusReport(form, "FIDO");
					name = "";
					break;
				case 204:
					scr = new StatusReport(form, "GUIDO");
					name = "";
					break;
				case 205:
					scr = new StatusReport(form, "SURGEON");
					name = "";
					break;
				case 206:
					scr = new StatusReport(form, "CAPCOM");
					name = "";
					break;
				case 207:
					scr = new StatusReport(form, "EECOM");
					name = "";
					break;
				case 208:
					scr = new StatusReport(form, "GNC");
					name = "";
					break;
				case 209:
					scr = new StatusReport(form, "TELMU");
					name = "";
					break;
				case 210:
					scr = new StatusReport(form, "CONTROL");
					name = "";
					break;
				case 211:
					scr = new StatusReport(form, "INCO");
					name = "";
					break;
				case 212:
					scr = new StatusReport(form, "O&P");
					name = "";
					break;
				case 213:
					scr = new StatusReport(form, "AFLIGHT");
					name = "";
					break;
				case 214:
					scr = new StatusReport(form, "FAO");
					name = "";
					break;
				case 215:
					scr = new StatusReport(form, "NETWORK");
					name = "";
					break;
				case 220:
					scr = new StatusPanel(form);
					name = "";
					break;
			}

			
			if(name != "")
			{
				form.Text = idstr + " - " + name;
			}
			else
			{
				form.Text = idstr;
			}

			if(scr != null)
			{
				return scr;
			}

			return null;
		}

		public void resizeForm()
		{
			int w, h;
			if (charSize)
			{
				w = (int)Math.Ceiling(this.width * form.pxPrChar) + form.padding_left + form.padding_right;
				h = (int)Math.Ceiling(this.height * form.pxPrLine) + form.padding_top + form.padding_bottom;
			}
			else
			{
				w = width;
				h = height;
			}
			this.form.ClientSize = new Size(w, h);
			resize();
		}

		public void updateElements(object sender, EventArgs e)
		{
			lock (updateLock)
			{
				updateLocalElements(sender, e);
			}
		}

		abstract public void updateLocalElements(object sender, EventArgs e);
		abstract public void makeElements();
		public void resize(object sender, EventArgs e)
		{
			resize();
		}
		abstract public void resize();

		public virtual bool keyDown(object sender, KeyEventArgs e) { return false; }
		public virtual bool keyUp(object sender, KeyEventArgs e) { return false; }
		//abstract public void destroyStreams();

		public void destroy()
		{
			lock (updateLock)
			{
				// Clear old lables
				foreach (Control label in screenLabels) { if (label != null) { label.Dispose(); } }
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

				// Clear Screws
				foreach (Screw screw in screenScrews) { if (screw != null) { screw.Dispose(); } }
				screenScrews.Clear();
				screenScrews.TrimExcess();

				// Clear EventIndicators
				foreach (EventIndicator indicator in screenEventIndicators) { if (indicator != null) { indicator.Dispose(); } }
				screenEventIndicators.Clear();
				screenEventIndicators.TrimExcess();

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
}
