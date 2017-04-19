using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using KRPC.Client;
using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;
using System.Drawing.Text;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Net;
using System.Threading;

namespace KSP_MOCR
{
	
	public partial class Form1 : Form
	{
		public Connection connection;

		public KRPC.Client.Services.KRPC.Service krpc;
		public KRPC.Client.Services.SpaceCenter.Service spaceCenter;

		public bool connected = false;

		MocrScreen activeScreen;
		public enum OS { UNIX, WINDOWS, OTHER }
		public OS system;

		private string screenCallup = "";
		public bool ctrlDown = false;

		static private List<PrivateFontCollection> _fontCollections;
		public Font font;
		public Font buttonFont;
		public Font smallFont;

		public double pxPrChar;
		public double pxPrLine;
		public double charOffset;
		public double lineOffset;

		public int padding_top = 4;
		public int padding_right = 4;
		public int padding_bottom = 4;
		public int padding_left = 4;

		public Color foreColor = Color.FromArgb(255, 239, 239, 239);
		public Color chartAxisColor = Color.FromArgb(255, 119, 102, 51);

		public List<Color> chartLineColors = new List<Color>();

		public List<List<Bitmap>> indicatorImages = new List<List<Bitmap>>();

		public Bitmap indicatorImage;
		public Bitmap engineOff;
		public Bitmap engineOn;

		enum Align { LEFT, RIGHT, CENTER };
		enum Resource { ElectricCharge, MonoPropellant, LiquidFuel, Oxidizer, Ore };

		public DateTime updateStart;
		public DateTime updateEnd;

		public System.Timers.Timer screenTimer;
		public System.Timers.Timer graphTimer;

		public Form1()
		{
			InitializeComponent();
		}

		static public Font GetCustomFont(byte[] fontData, float size, FontStyle style)
		{
			if (_fontCollections == null) _fontCollections = new List<PrivateFontCollection>();
			PrivateFontCollection fontCol = new PrivateFontCollection();
			IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);

			Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
			fontCol.AddMemoryFont(fontPtr, fontData.Length);
			Marshal.FreeCoTaskMem(fontPtr); //<-- It works!
			_fontCollections.Add(fontCol);
			return new Font(fontCol.Families[0], size, style);
		}

		public static byte[] GetBytesFromFile(string fullFilePath)
		{
			// this method is limited to 2^32 byte files (4.2 GB)

			FileStream fs = File.OpenRead(fullFilePath);
			try
			{
				byte[] bytes = new byte[fs.Length];
				fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
				fs.Close();
				return bytes;
			}
			finally
			{
				fs.Close();
			}

		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Console.WriteLine("FORM LOADING");

			// Init indicator images
			for (int i = 0; i < 11; i++)
			{
				indicatorImages.Add(new List<Bitmap>());
				for (int n = 0; n < 5; n++)
				{
					indicatorImages[i].Add(null);
				}
			}

			// Set up fonts and graphics
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				system = OS.UNIX;
				Console.WriteLine("SYSTEM SET TO UNIX");

				font = new Font("Ubuntu Mono", 12, FontStyle.Regular);
				buttonFont = new Font("Ubuntu Mono", 10, FontStyle.Regular);
				smallFont = new Font("Ubuntu Mono", 8, FontStyle.Regular);

				pxPrChar = 8;
				pxPrLine = 16;
				charOffset = -1;
				lineOffset = -1;

				// Load Images
				indicatorImage = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/Indicator.png");
				indicatorImages[9][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/Indicator9x1.png");
				indicatorImages[8][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/Indicator8x1.png");
				indicatorImages[7][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/Indicator7x1.png");
				indicatorImages[6][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/Indicator6x1.png");
				indicatorImages[5][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/Indicator5x1.png");
				indicatorImages[4][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/Indicator4x1.png");
				indicatorImages[3][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/Indicator3x1.png");
				indicatorImages[2][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/Indicator2x1.png");
				indicatorImages[1][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/Indicator1x1.png");
				indicatorImages[2][2] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/Indicator2x2.png");
				engineOn = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/EngineOn.png");
				engineOff = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/EngineOff.png");
			}
			else
			{
				system = OS.WINDOWS;
				Console.WriteLine("SYSTEM SET TO WINDOWS");

				font = GetCustomFont(GetBytesFromFile(AppDomain.CurrentDomain.BaseDirectory + "Resources\\consola.ttf"), 12, FontStyle.Regular);
				buttonFont = GetCustomFont(GetBytesFromFile(AppDomain.CurrentDomain.BaseDirectory + "Resources\\consola.ttf"), 10, FontStyle.Regular);
				smallFont = GetCustomFont(GetBytesFromFile(AppDomain.CurrentDomain.BaseDirectory + "Resources\\consola.ttf"), 8, FontStyle.Regular);

				pxPrChar = 9;
				pxPrLine = 19;
				charOffset = -2.5;
				lineOffset = 0;

				try
				{
					this.Icon = new Icon(AppDomain.CurrentDomain.BaseDirectory + "Resources\\MOCR.ico");
				}
				catch (Exception) { }

				// Load Images
				Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Indicator.png");
				indicatorImage = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Indicator.png");
				indicatorImages[9][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Indicator9x1.png");
				indicatorImages[8][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Indicator8x1.png");
				indicatorImages[7][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Indicator7x1.png");
				indicatorImages[6][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Indicator6x1.png");
				indicatorImages[5][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Indicator5x1.png");
				indicatorImages[4][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Indicator4x1.png");
				indicatorImages[3][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Indicator3x1.png");
				indicatorImages[2][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Indicator2x1.png");
				indicatorImages[1][1] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Indicator1x1.png");
				indicatorImages[2][2] = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Indicator2x2.png");
				engineOn = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\EngineOn.png");
				engineOff = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\EngineOff.png");
			}

			// Setup Helper
			Helper.setForm(this);

			// Enable key preview
			this.KeyPreview = true;

			// Setup form style
			this.BackColor = Color.FromArgb(255, 16, 16, 16);
			this.ForeColor = foreColor;
			this.ClientSize = new Size((int)(pxPrChar * 120) + padding_left + padding_right, (int)(pxPrLine * 30) + padding_top + padding_bottom);

			// Initiate Screen Timer
			screenTimer = new System.Timers.Timer();
			screenTimer.SynchronizingObject = this;
			screenTimer.Stop();
			screenTimer.AutoReset = false;
			screenTimer.Interval = 1000;
			screenTimer.Elapsed += screenTick;

			// Initiate Graph Timer
			graphTimer = new System.Timers.Timer();
			screenTimer.SynchronizingObject = this;
			graphTimer.AutoReset = false;
			graphTimer.Interval = 1000;
			graphTimer.Elapsed += graphTick;
			graphTimer.Start();


			// Load the connection screen
			SetScreen(0);
		}

		public void graphTick(object sender, EventArgs e)
		{
			DateTime start = DateTime.Now;

			updateChartData(sender, e);

			DateTime end = DateTime.Now;

			TimeSpan duration = end - start;
			int remainTime = 1000 - (int)duration.TotalMilliseconds;

			if (remainTime < 100) { remainTime = 100; }

			graphTimer.Interval = remainTime;
			graphTimer.Start();
		}

		public void screenTick(object sender, EventArgs e)
		{
			//Console.WriteLine("Starting ScreenTick");
			updateStart = DateTime.Now;

			if (activeScreen != null)
			{
				activeScreen.updateElements(sender, e);
			}

			updateEnd = DateTime.Now;

			TimeSpan updateDuration = updateEnd - updateStart;
			int remainTime = activeScreen.updateRate - (int)updateDuration.TotalMilliseconds;

			if (remainTime < 100) { remainTime = 100; }
			Console.WriteLine("Remain Time: " + remainTime.ToString() + ", Time Spent: " + ((int)updateDuration.TotalMilliseconds).ToString());

			screenTimer.Interval = remainTime;
			screenTimer.Start();
		}

		public void ConnectToServer(object sender, EventArgs e)
		{
			Console.WriteLine("You pressed the 'Connect'-button, you clever you... :-)");
			activeScreen.screenLabels[0].Text = "Connecting to " + activeScreen.screenInputs[0].Text;

			try
			{
				IPAddress[] adrs = Dns.GetHostAddresses(activeScreen.screenInputs[0].Text);
				System.Net.IPAddress IP = adrs[0]; // IPv4

				String name = activeScreen.screenInputs[0].Text;

				connection = new Connection(name: name, address: IP);

				krpc = connection.KRPC();
				spaceCenter = connection.SpaceCenter();

				// Setup graphable data
				setupChartData();

				activeScreen.screenLabels[0].Text = "Connected";
				connected = true;
			}
			catch(System.Net.Sockets.SocketException)
			{
				MessageBox.Show("KRPC SERVER NOT RESPONDING");
				activeScreen.screenLabels[0].Text = "NOT CONNECTED";
			}
			catch(System.FormatException)
			{
				MessageBox.Show("NOT A VALID IP-ADDRESS");
				activeScreen.screenLabels[0].Text = "NOT CONNECTED";
			}
			catch(System.IO.IOException)
			{
				MessageBox.Show("IO ERROR");
				activeScreen.screenLabels[0].Text = "NOT CONNECTED";
			}
		}

		public void DisconnectFromServer(object sender, EventArgs e)
		{
			connection.Dispose();
			connected = false;
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			Application.Exit();
		}

		private void SetScreen(int id)
		{
			// Stop screen update
			screenTimer.Stop();

			// Dispose of old elements
			if (activeScreen != null) { activeScreen.destroy();}

			// Destroy old screen
			activeScreen = null;

            // Get Screen
			activeScreen = MocrScreen.Create(id, this);

			// If Screen exists: Make Elementes and resize
			if (activeScreen != null)
			{
				activeScreen.resizeForm();
				activeScreen.makeElements();

				// Set focus to input 0
				activeScreen.screenInputs[0].Focus();

				// Start the update process
				screenTimer.Start();
			}
			else
			{
				MessageBox.Show("Screen " + id.ToString() + " not found","INFORMATION",MessageBoxButtons.OK,MessageBoxIcon.Information);
				SetScreen(0);
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			//Console.WriteLine("CK: " + keyData.ToString());
			switch (keyData)
			{
				case Keys.F1:
					SetScreen(1); // Ascent Screen
					return true;
				case Keys.F12:
					SetScreen(12); // Test Screen
					return true;
				case Keys.Escape:
					SetScreen(0); // Connection Screen
					return true;
			}

			// Call the base class
			return base.ProcessCmdKey(ref msg, keyData);
		}


		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (graphTimer != null && graphTimer.Enabled) { graphTimer.Stop(); }
			if (screenTimer != null && screenTimer.Enabled) { screenTimer.Stop(); }
			if (connection != null) { connection.Dispose(); }
		}

		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			//Console.WriteLine("KD: " + e.KeyCode.ToString());

			int x = e.KeyValue & 0x000000ff;

			if (e.KeyCode == Keys.ControlKey && !ctrlDown)
			{
				ctrlDown = true;
			}

			if(ctrlDown && (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9))
			{
				screenCallup += (x - 48).ToString();
			}
			else if (ctrlDown && (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
			{
				screenCallup += (x - 96).ToString();
			}
			if (activeScreen != null)
			{
				activeScreen.keyDown(sender, e);
			}
		}

		private void Form1_KeyUp(object sender, KeyEventArgs e)
		{
			//Console.WriteLine("U: " + e.KeyValue);
			if (e.KeyCode == Keys.ControlKey && ctrlDown)
			{
				ctrlDown = false;

				// Trim leading zeroes down to 1 zero (for callup of page 0)
				while(screenCallup.Length > 1 && screenCallup.Substring(0,1) == "0")
				{
					screenCallup = screenCallup.Substring(1);
				}

				try
				{
					Console.WriteLine("CALLING: " + screenCallup);
					int screenID = int.Parse(screenCallup);
					SetScreen(screenID);
				}
				catch(Exception ex)
				{
					if(ex is ArgumentNullException)
					{
						Console.WriteLine(ex.ToString());
					}

					if (ex is FormatException)
					{
						// Do nothing
					}

					if (ex is OverflowException)
					{
						Console.WriteLine(ex.ToString());
					}
				}
				screenCallup = "";
			}

			if (activeScreen != null)
			{
				activeScreen.keyUp(sender, e);
			}
		}
	}
}
