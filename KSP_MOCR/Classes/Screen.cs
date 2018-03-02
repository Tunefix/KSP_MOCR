using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KRPC.Client;

namespace KSP_MOCR
{
	public class Screen : Form
	{
		public Connection connection;
		public StreamCollection streamCollection;
		public DataStorage dataStorage;
		public int ID;

		public Form1 form;
		
		public double pxPrChar;
		public double pxPrLine;
		public int padding_top;
		public int padding_left;
		public int padding_bottom;
		public int padding_right;
		public Font font;
		public Font buttonFont;
		public Font smallFont;
		public Color foreColor;
		public Color chartAxisColor;
		public double charOffset;
		public double lineOffset;

		public Dictionary<String, List<KeyValuePair<double, double?>>> chartData;
		
		
		public System.Timers.Timer screenTimer;
		public DateTime updateStart;
		public DateTime updateEnd;
		public MocrScreen activeScreen;

		public int screenType;
		
		public Screen(Form1 form, int id, Connection connection, StreamCollection streamCollection, DataStorage dataStorage)
		{
			this.form = form;
			this.ID = id;
			this.connection = connection;
			this.streamCollection = streamCollection;
			this.dataStorage = dataStorage;
			this.pxPrLine = form.pxPrLine;
			this.pxPrChar = form.pxPrChar;
			this.padding_top = form.padding_top;
			this.padding_left = form.padding_left;
			this.padding_right = form.padding_right;
			this.padding_bottom = form.padding_bottom;
			this.font = form.font;
			this.buttonFont = form.buttonFont;
			this.smallFont = form.smallFont;
			this.foreColor = form.foreColor;
			this.chartAxisColor = form.chartAxisColor;
			this.charOffset = form.charOffset;
			this.lineOffset = form.lineOffset;
			this.chartData = form.chartData;

			this.BackColor = form.BackColor;

			this.FormClosing += ScreenClosing;

			this.KeyPreview = true;

			this.KeyDown += localKeyDown;
			this.KeyUp += localKeyUp;

			
			
			// Initiate Screen Timer
			screenTimer = new System.Timers.Timer();
			screenTimer.SynchronizingObject = this;
			screenTimer.Stop();
			screenTimer.AutoReset = false;
			screenTimer.Interval = 1000;
			screenTimer.Elapsed += screenTick;
		}

		public void screenTick(object sender, EventArgs e)
		{
			//Console.WriteLine("Starting ScreenTick");
			updateStart = DateTime.Now;

			if (activeScreen != null)
			{
				activeScreen.updateElements(sender, e);
				this.Invalidate();
			}

			updateEnd = DateTime.Now;

			TimeSpan updateDuration = updateEnd - updateStart;
			int remainTime = activeScreen.updateRate - (int)updateDuration.TotalMilliseconds;

			if (remainTime < 50) {
				remainTime = 50;
				Console.WriteLine("LOW REMAIN TIME: "
					+ remainTime.ToString() + ", Time Spent: "
					+ ((int)updateDuration.TotalMilliseconds).ToString()
					+ ", ScrTp: " + this.screenType
					+ ", ScrID: " + this.ID);
			}

			screenTimer.Interval = remainTime;
			screenTimer.Start();
		}
		
		public void SetScreen(int id)
		{
			// Stop screen update
			screenTimer.Stop();

			// Dispose of old elements
			if (activeScreen != null) { activeScreen.destroy();}

			// Destroy old screen
			activeScreen = null;

			// Get Screen
			Helper.setForm(this);
			activeScreen = MocrScreen.Create(id, this);

			// If Screen exists: Make Elementes and resize
			if (activeScreen != null)
			{
				screenType = id;
				activeScreen.resizeForm();
				activeScreen.makeElements();

				// Set focus to input 0
				activeScreen.screenInputs[0].Focus();

				// Start the update process
				screenTimer.Start();

				this.Resize += activeScreen.resize;
			}
			else
			{
				MessageBox.Show("Screen " + id.ToString() + " not found", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Information);
				this.Close();
			}
		}

		private void ScreenClosing(object sender, FormClosingEventArgs e)
		{
			screenTimer.Stop();
			screenTimer.Dispose();
			if (streamCollection != null)
			{
				streamCollection.CloseStreams();
			}
			this.Dispose();
		}

		private void localKeyDown(object sender, KeyEventArgs e)
		{
			if(activeScreen != null && !form.ctrlDown)
			{
				if(!activeScreen.keyDown(sender, e))
				{
					this.form.Form1_KeyDown(sender, e);
				}
			}
			else
			{
				this.form.Form1_KeyDown(sender, e);
			}
		}

		private void localKeyUp(object sender, KeyEventArgs e)
		{
			if (activeScreen != null && !form.ctrlDown)
			{
				if(!activeScreen.keyUp(sender, e))
				{
					this.form.Form1_KeyUp(sender, e);
				}
			}
			else
			{
				this.form.Form1_KeyUp(sender, e);
			}
		}
	}
}
