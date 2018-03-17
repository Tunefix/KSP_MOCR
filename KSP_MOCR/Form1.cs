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
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Diagnostics;
using Microsoft.Win32;

namespace KSP_MOCR
{
	
	public partial class Form1 : Form
	{
		public Connection connection;
		public String connectionName = "";
		public String connectionIP = "";

		public PySSSMQ_client pySSSMQ = new PySSSMQ_client();
		public PySSSMQ_Handler pySSSMQ_handler;
		public ScriptSource pySSSMQ_server;

		public DataStorage dataStorage;

		public KRPC.Client.Services.KRPC.Service krpc;
		public KRPC.Client.Services.SpaceCenter.Service spaceCenter;

		public bool connected = false;

		public enum OS { UNIX, WINDOWS, OTHER }
		public OS system;

		private string screenCallup = "";
		public bool ctrlDown = false;

		static private List<PrivateFontCollection> _fontCollections;
		public Font font;
		public Font buttonFont;
		public Font smallFont;
		public Font smallFontB;
		public Font tinyFont;

		public Font CRTfont1;
		public Font CRTfont2;
		public Font CRTfont3;
		public Font CRTfont4;
		public Font CRTfont5;

		public double pxPrChar;
		public double pxPrLine;
		public double charOffset;
		public double lineOffset;

		public int padding_top = 4;
		public int padding_right = 4;
		public int padding_bottom = 4;
		public int padding_left = 4;

		//public Color foreColor = Color.FromArgb(255, 230, 240, 250);
		public Color foreColor = Color.FromArgb(255, 226, 241, 254); // MORE BLUE COLOR
		public Color chartAxisColor = Color.FromArgb(255, 119, 102, 51);

		

		public List<Color> chartLineColors = new List<Color>();

		public Bitmap indicatorImage;
		public Bitmap engineOff;
		public Bitmap engineOn;
		public Bitmap logo;

		enum Align { LEFT, RIGHT, CENTER };
		enum Resource { ElectricCharge, MonoPropellant, LiquidFuel, Oxidizer, Ore };

		public DateTime updateStart;
		public DateTime updateEnd;

		public List<Screen> screens = new List<Screen>();

		public System.Timers.Timer screenTimer;
		public System.Timers.Timer graphTimer;
		public System.Timers.Timer statusCheck;

		public StreamCollection streamCollection = StreamCollection.Instance;

		TextBox ipAddr;
		TextBox name;
		CustomLabel pySSSMQStatus;
		CustomLabel pySSSMQStatus2;
		CustomLabel kRPCStatus;
		CustomLabel gameScene;

		FileStream ostrm;
		StreamWriter writer;
		Logger logger;

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
			logger = new Logger();
			Console.SetOut(logger);
			setLogOut(true); // SET TRUE TO WRITE CONSOLE TO log.txt

			Console.WriteLine("FORM LOADING");

			// Set up fonts and graphics
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				system = OS.UNIX;
				Console.WriteLine("SYSTEM SET TO UNIX");

				font = new Font("Ubuntu Mono", 12, FontStyle.Regular);
				buttonFont = new Font("Ubuntu Mono", 10, FontStyle.Regular);
				smallFont = new Font("Ubuntu Mono", 8, FontStyle.Regular);
				smallFontB = new Font("Ubuntu Mono", 8, FontStyle.Bold);
				tinyFont = new Font("Ubuntu Mono", 7, FontStyle.Bold);

				CRTfont1 = new Font("Ubuntu Mono", 7, FontStyle.Regular);
				CRTfont2 = new Font("Ubuntu Mono", 9, FontStyle.Regular);
				CRTfont3 = new Font("Ubuntu Mono", 12, FontStyle.Regular);
				CRTfont4 = new Font("Ubuntu Mono", 14, FontStyle.Regular);
				CRTfont5 = new Font("Ubuntu Mono", 18, FontStyle.Regular);

				pxPrChar = 8;
				pxPrLine = 16;
				charOffset = -1;
				lineOffset = -1;

				// Load Images
				indicatorImage = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/Indicator.png");
				logo = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources/logo.png");
			}
			else
			{
				system = OS.WINDOWS;
				Console.WriteLine("SYSTEM SET TO WINDOWS");

				font = GetCustomFont(GetBytesFromFile(AppDomain.CurrentDomain.BaseDirectory + "Resources\\consola.ttf"), 12, FontStyle.Regular);
				buttonFont = GetCustomFont(GetBytesFromFile(AppDomain.CurrentDomain.BaseDirectory + "Resources\\consola.ttf"), 10, FontStyle.Regular);
				smallFont = GetCustomFont(GetBytesFromFile(AppDomain.CurrentDomain.BaseDirectory + "Resources\\consola.ttf"), 8, FontStyle.Regular);
				smallFontB = GetCustomFont(GetBytesFromFile(AppDomain.CurrentDomain.BaseDirectory + "Resources\\consola.ttf"), 8, FontStyle.Bold);
				tinyFont = GetCustomFont(GetBytesFromFile(AppDomain.CurrentDomain.BaseDirectory + "Resources\\consola.ttf"), 7, FontStyle.Bold);

				CRTfont1 = GetCustomFont(GetBytesFromFile(AppDomain.CurrentDomain.BaseDirectory + "Resources\\consola.ttf"), 7, FontStyle.Regular);
				CRTfont2 = GetCustomFont(GetBytesFromFile(AppDomain.CurrentDomain.BaseDirectory + "Resources\\consola.ttf"), 9, FontStyle.Regular);
				CRTfont3 = GetCustomFont(GetBytesFromFile(AppDomain.CurrentDomain.BaseDirectory + "Resources\\consola.ttf"), 12, FontStyle.Regular);
				CRTfont4 = GetCustomFont(GetBytesFromFile(AppDomain.CurrentDomain.BaseDirectory + "Resources\\consola.ttf"), 14, FontStyle.Regular);
				CRTfont5 = GetCustomFont(GetBytesFromFile(AppDomain.CurrentDomain.BaseDirectory + "Resources\\consola.ttf"), 18, FontStyle.Regular);

				pxPrChar = 9;
				pxPrLine = 19;
				charOffset = -2.5;
				lineOffset = 0;

				try
				{
					this.Icon = new Icon(AppDomain.CurrentDomain.BaseDirectory + "Resources\\MOCR.ico");
				}
				catch (Exception ex) { Console.WriteLine(ex.GetType().ToString() + ":" + ex.Message + "\n" + ex.StackTrace); }

				// Load Images
				//Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Indicator.png");
				indicatorImage = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Indicator.png");
				logo = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\logo.png");
			}

			// Setup DataStorage
			dataStorage = new DataStorage(pySSSMQ);

			// Initiate SteamCollection
			streamCollection = StreamCollection.Instance;

			// Setup PySSSMQ Handler
			pySSSMQ_handler = new PySSSMQ_Handler();
			pySSSMQ_handler.storage = dataStorage;

			// Enable key preview
			this.KeyPreview = true;

			// Setup form style
			this.BackColor = Color.FromArgb(255, 24, 24, 24);
			this.ForeColor = foreColor;
			this.ClientSize = new Size((int)(pxPrChar * 44) + padding_left + padding_right, (int)(pxPrLine * 20) + padding_top + padding_bottom);

			// Initiate Graph Timer
			graphTimer = new System.Timers.Timer();
			graphTimer.SynchronizingObject = this;
			graphTimer.AutoReset = false;
			graphTimer.Interval = 1000;
			graphTimer.Elapsed += graphTick;
			graphTimer.Start();

			// Initiate Status Check
			statusCheck = new System.Timers.Timer();
			statusCheck.SynchronizingObject = this;
			statusCheck.AutoReset = true;
			statusCheck.Interval = 1000;
			statusCheck.Elapsed += statusCheckTick;
			statusCheck.Start();


			// Load the connection screen
			//SetScreen(0);
			//makeScreen(0);
			//activeScreen = screens[0].activeScreen;

			// Fill form with connection-stuff
			fillForm();
		}

		private void fillForm()
		{
			// LOGO
			PictureBox emblem = new PictureBox();
			emblem.Image = logo;
			emblem.Location = new Point(padding_left, padding_top);
			emblem.Size = getSize(21, 9);
			emblem.SizeMode = PictureBoxSizeMode.CenterImage;
			this.Controls.Add(emblem);
			
			// ADDRESS AND NAME FIELDS
			CustomLabel ipAddrLbl = new CustomLabel();
			ipAddrLbl.setCharWidth(pxPrChar);
			ipAddrLbl.setlineHeight(pxPrLine);
			ipAddrLbl.setcharOffset(charOffset);
			ipAddrLbl.setlineOffset(lineOffset);
			ipAddrLbl.Font = font;
			ipAddrLbl.AutoSize = false;
			ipAddrLbl.Location = getPoint(22, 1);
			ipAddrLbl.Size = getSize(20, 1);
			ipAddrLbl.Text = "Server IP:";
			ipAddrLbl.Padding = new Padding(0);
			ipAddrLbl.Margin = new Padding(0);
			ipAddrLbl.ForeColor = foreColor;
			this.Controls.Add(ipAddrLbl);
			
			CustomLabel nameLbl = new CustomLabel();
			nameLbl.setCharWidth(pxPrChar);
			nameLbl.setlineHeight(pxPrLine);
			nameLbl.setcharOffset(charOffset);
			nameLbl.setlineOffset(lineOffset);
			nameLbl.Font = font;
			nameLbl.AutoSize = false;
			nameLbl.Location = getPoint(22, 4);
			nameLbl.Size = getSize(20, 1);
			nameLbl.Text = "Your ID:";
			nameLbl.Padding = new Padding(0);
			nameLbl.Margin = new Padding(0);
			nameLbl.ForeColor = foreColor;
			this.Controls.Add(nameLbl);
			
			ipAddr = new TextBox();
			ipAddr.Location = getPoint(22, 2);
			ipAddr.Size = getSize(20, 1);
			ipAddr.Font = font;
			ipAddr.ForeColor = foreColor;
			ipAddr.BorderStyle = BorderStyle.None;
			ipAddr.BackColor = Color.FromArgb(255, 32, 32, 32);
			ipAddr.AutoSize = false;
			ipAddr.Text = "127.0.0.1";
			ipAddr.KeyDown += checkForEnter;
			this.Controls.Add(ipAddr);
			
			name = new TextBox();
			name.Location = getPoint(22, 5);
			name.Size = getSize(20, 1);
			name.Font = font;
			name.ForeColor = foreColor;
			name.BorderStyle = BorderStyle.None;
			name.BackColor = Color.FromArgb(255, 32, 32, 32);
			name.AutoSize = false;
			name.Text = "Flight Director";
			name.KeyDown += checkForEnter;
			this.Controls.Add(name);
			
			// CONNECT BUTTON
			MocrButton button = new MocrButton();
			button.Location = getPoint(22, 7);
			button.Size = getSize(20, 1);
			button.Cursor = Cursors.Hand;
			button.Font = font;
			button.Text = "Connect";
			button.Padding = new Padding(0);
			button.Click += ConnectToServer;
			this.Controls.Add(button);
			
			// CONNECTION STATUS
			kRPCStatus = new CustomLabel();
			kRPCStatus.setCharWidth(pxPrChar);
			kRPCStatus.setlineHeight(pxPrLine);
			kRPCStatus.setcharOffset(charOffset);
			kRPCStatus.setlineOffset(lineOffset);
			kRPCStatus.Font = font;
			kRPCStatus.AutoSize = false;
			kRPCStatus.Location = getPoint(1, 9);
			kRPCStatus.Size = getSize(42, 1);
			kRPCStatus.Text = "   kRPC: NOT CONNECTED";
			kRPCStatus.Padding = new Padding(0);
			kRPCStatus.Margin = new Padding(0);
			kRPCStatus.ForeColor = foreColor;
			this.Controls.Add(kRPCStatus);
			
			pySSSMQStatus = new CustomLabel();
			pySSSMQStatus.setCharWidth(pxPrChar);
			pySSSMQStatus.setlineHeight(pxPrLine);
			pySSSMQStatus.setcharOffset(charOffset);
			pySSSMQStatus.setlineOffset(lineOffset);
			pySSSMQStatus.Font = font;
			pySSSMQStatus.AutoSize = false;
			pySSSMQStatus.Location = getPoint(1, 10);
			pySSSMQStatus.Size = getSize(42, 1);
			pySSSMQStatus.Text = "PySSSMQ: NOT CONNECTED";
			pySSSMQStatus.Padding = new Padding(0);
			pySSSMQStatus.Margin = new Padding(0);
			pySSSMQStatus.ForeColor = foreColor;
			this.Controls.Add(pySSSMQStatus);

			// GAME SCENE
			gameScene = new CustomLabel();
			gameScene.setCharWidth(pxPrChar);
			gameScene.setlineHeight(pxPrLine);
			gameScene.setcharOffset(charOffset);
			gameScene.setlineOffset(lineOffset);
			gameScene.Font = font;
			gameScene.AutoSize = false;
			gameScene.Location = getPoint(1, 12);
			gameScene.Size = getSize(42, 1);
			gameScene.Text = "GAME SCENE: ";
			gameScene.Padding = new Padding(0);
			gameScene.Margin = new Padding(0);
			gameScene.ForeColor = foreColor;
			this.Controls.Add(gameScene);

			// PySSSMQ BUTTON
			MocrButton pybutton = new MocrButton();
			pybutton.Location = getPoint(1, 17);
			pybutton.Size = getSize(24, 1);
			pybutton.Cursor = Cursors.Hand;
			pybutton.Font = font;
			pybutton.Text = "Start PySSSMQ-Server";
			pybutton.Padding = new Padding(0);
			pybutton.Click += StartPyServer;
			this.Controls.Add(pybutton);

			MocrButton pybuttonS = new MocrButton();
			pybuttonS.Location = getPoint(1, 18);
			pybuttonS.Size = getSize(24, 1);
			pybuttonS.Cursor = Cursors.Hand;
			pybuttonS.Font = font;
			pybuttonS.Text = "Stop PySSSMQ-Server";
			pybuttonS.Padding = new Padding(0);
			pybuttonS.Click += StopPyServer;
			this.Controls.Add(pybuttonS);

			// PySSSMQ-server status
			pySSSMQStatus2 = new CustomLabel();
			pySSSMQStatus2.setCharWidth(pxPrChar);
			pySSSMQStatus2.setlineHeight(pxPrLine);
			pySSSMQStatus2.setcharOffset(charOffset);
			pySSSMQStatus2.setlineOffset(lineOffset);
			pySSSMQStatus2.Font = font;
			pySSSMQStatus2.AutoSize = false;
			pySSSMQStatus2.Location = getPoint(1, 16);
			pySSSMQStatus2.Size = getSize(42, 5);
			pySSSMQStatus2.Text = "PySSSMQ-Server: NOT RUNNING";
			pySSSMQStatus2.Padding = new Padding(0);
			pySSSMQStatus2.Margin = new Padding(0);
			pySSSMQStatus2.ForeColor = foreColor;
			this.Controls.Add(pySSSMQStatus2);

		}
		
		private void checkForEnter(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter || e.KeyData == Keys.Return)
			{
				ConnectToServer(sender, e);
			}
		}

		private Point getPoint(int x, int y)
		{
			int top  = (int)(Math.Ceiling(y * pxPrLine) + padding_top);
			int left = (int)(Math.Ceiling(x * pxPrChar) + padding_left);

			return new Point(left, top);
		}

		private Size getSize(int x, int y)
		{
			int width  = (int)Math.Ceiling(x * pxPrChar);
			int height = (int)Math.Ceiling(y * pxPrLine);

			return new Size(width, height);
		}

		public void statusCheckTick(object sender, EventArgs e)
		{
			//Process[] pname = Process.GetProcesses();

			Process[] pname = Process.GetProcessesByName("pythonw");
			if (pname.Length > 0)
			{
				pySSSMQStatus2.Text = "PySSSMQ-Server: RUNNING";
			}
			else
			{
				pySSSMQStatus2.Text = "PySSSMQ-Server: NOT RUNNING";
			}

			if (krpc != null)
			{
				GameScene Scn = krpc.CurrentGameScene;
				string scene = Scn.ToString();
				gameScene.Text = "GAME SCENE: " + scene;

				if(streamCollection != null)
				{
					streamCollection.setGameScene(Scn);
				}
			}
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

		public void ConnectToServer(object sender, EventArgs e)
		{
			if (!connected)
			{
				Console.WriteLine("You pressed the 'Connect'-button, you clever you... :-)");
				pySSSMQStatus.Text = "PySSSMQ: CONNECTING";
				kRPCStatus.Text = "   kRPC: CONNECTING";

				try
				{
					IPAddress[] connectionAdrs = Dns.GetHostAddresses(ipAddr.Text);
					System.Net.IPAddress IP = connectionAdrs[0]; // IPv4
					
					// Store connection IP
					this.connectionIP = IP.ToString();

					connectionName = name.Text;

					connection = new Connection(name: connectionName, address: IP);

					krpc = connection.KRPC();
					spaceCenter = connection.SpaceCenter();

					streamCollection.setConnection(connection);

					// Setup graphable data
					setupChartData(streamCollection);

					kRPCStatus.Text = "   kRPC: CONNECTED";
					connected = true;
				}
				catch (System.Net.Sockets.SocketException)
				{
					MessageBox.Show("KRPC SERVER NOT RESPONDING");
					kRPCStatus.Text = "   kRPC: NOT CONNECTED";
				}
				catch (System.FormatException)
				{
					MessageBox.Show("NOT A VALID IP-ADDRESS");
					kRPCStatus.Text = "   kRPC: NOT CONNECTED";
				}
				catch (System.IO.IOException)
				{
					MessageBox.Show("IO ERROR");
					kRPCStatus.Text = "   kRPC: NOT CONNECTED";
				}

				// Connect to pySSMQ
				try
				{
					pySSSMQ.Connect(connectionIP);
					pySSSMQ.AttachReceiveEvent(pySSSMQ_handler.receive);
					if (pySSSMQ.IsConnected())
					{
						pySSSMQStatus.Text = "PySSSMQ: CONNECTED";
					}
					else
					{
						pySSSMQStatus.Text = "PySSSMQ: NOT CONNECTED";
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.GetType() + ":" + ex.Message);
					pySSSMQStatus.Text = "PySSSMQ: NOT CONNECTED";
				}
			}
			else
			{
				MessageBox.Show("Already Connected", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		public void StartPyServer(object sender, EventArgs e)
		{
			Process p = new Process();
			p.StartInfo.WorkingDirectory = System.Environment.CurrentDirectory;
			p.StartInfo.FileName = "python\\pythonw.exe";
			p.StartInfo.Arguments = " PySSSMQ_server.py";
			p.StartInfo.UseShellExecute = true;
			p.Start();
		}

		public void StopPyServer(object sender, EventArgs e)
		{
			Process p = new Process();
			p.StartInfo.WorkingDirectory = System.Environment.CurrentDirectory;
			p.StartInfo.FileName = "python\\pythonw.exe";
			p.StartInfo.Arguments = " PySSSMQ_server.py stop";
			p.StartInfo.UseShellExecute = true;
			p.Start();
		}

		public void DisconnectFromServer(object sender, EventArgs e)
		{
			connection.Dispose();
			connected = false;
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			// If PySSSMQ is running, end it.
			Process[] pname = Process.GetProcessesByName("pythonw");
			if (pname.Length > 0)
			{
				StopPyServer(null, null);
			}

			flushLog();
			Application.Exit();
		}

		private void makeScreen(int id)
		{
			screens.Add(new Screen(this, screens.Count, connection, streamCollection, dataStorage));
			screens[screens.Count - 1].Show();
			screens[screens.Count - 1].SetScreen(id);
		}

		public void closeScreen(int id)
		{
			screens[id].Close();
			screens.TrimExcess();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			//Console.WriteLine("CK: " + keyData.ToString());
			

			// Call the base class
			return base.ProcessCmdKey(ref msg, keyData);
		}


		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Close screens
			foreach (Screen scr in screens)
			{
				scr.Close();
			}
			
			streamCollection = null;
			if (graphTimer != null && graphTimer.Enabled) { graphTimer.Stop(); }
			if (screenTimer != null && screenTimer.Enabled) { screenTimer.Stop(); }
			if (connection != null) { connection.Dispose(); }
		}

		public void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			//Console.WriteLine("KD: " + e.KeyCode.ToString());

			int x = e.KeyValue & 0x000000ff;

			if (e.KeyCode == Keys.ControlKey && !ctrlDown)
			{
				ctrlDown = true;
				screenCallup = "";
			}

			if(ctrlDown && (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9))
			{
				screenCallup += (x - 48).ToString();
			}
			else if (ctrlDown && (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
			{
				screenCallup += (x - 96).ToString();
			}
		}

		public void Form1_KeyUp(object sender, KeyEventArgs e)
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
					//SetScreen(screenID);
					makeScreen(screenID);
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.GetType().ToString() + ":" + ex.Message + "\n" + ex.StackTrace);
				}
				screenCallup = "";
			}
		}

		private void setLogOut(bool on)
		{
			if (on)
			{
				try
				{
					ostrm = new FileStream("./log.txt", FileMode.Create, FileAccess.Write);
					writer = new StreamWriter(ostrm);

					writer.WriteLine(" === Started at " + DateTime.Now + " ===");
					writer.WriteLine(logger.output);

					writer.Close();
					ostrm.Close();
				}
				catch (Exception e)
				{
					Console.WriteLine("Cannot open log.txt for writing");
					Console.WriteLine(e.Message);
					return;
				}
			}
		}

		private void flushLog()
		{
			Console.WriteLine("=== Program ended normally " + DateTime.Now + " === ");
		}
	}
}