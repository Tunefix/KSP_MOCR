using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KRPC.Client.Services.SpaceCenter;
using KRPC.Client.Services.KRPC;

namespace KSP_MOCR
{
	public class MapScreen : MocrScreen
	{
		CelestialBody body;

		int taillength = 3000;
		bool tail = false;
		bool fade = false;

		public MapScreen(Screen form)
		{
			this.form = form;
			this.chartData = form.form.chartData;
			this.screenStreams = form.streamCollection;

			this.form.FormBorderStyle = FormBorderStyle.Sizable;

			this.width = 120;
			this.height = 30;

			this.updateRate = 1000;

			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				this.body = form.connection.SpaceCenter().ActiveVessel.Orbit.Body;
			}
		}

		public override bool keyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F11)
			{
				if (this.form.WindowState == FormWindowState.Maximized)
				{
					this.form.TopMost = false;
					this.form.FormBorderStyle = FormBorderStyle.Sizable;
					this.form.WindowState = FormWindowState.Normal;
				}
				else
				{
					this.form.TopMost = true;
					this.form.FormBorderStyle = FormBorderStyle.None;
					this.form.WindowState = FormWindowState.Maximized;
				}
				return true;
			}
			return false;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenMaps.Add(null); // Initialize Map
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			for (int i = 0; i < 70; i++) screenIndicators.Add(null); // Initialize Indicators
			for (int i = 0; i < 80; i++) screenButtons.Add(null); // Initialize Buttons
			for (int i = 0; i < 1; i++) screenLabels.Add(null); // Initialize Labels

			screenInputs[0] = Helper.CreateInput(7, 29, 6, 1, HorizontalAlignment.Right); // Every page must have an input to capture keypresses on Unix
			screenInputs[0].Text = taillength.ToString();
			screenInputs[0].TextChanged += (sender, e) => updateTailLength(sender, e);

			screenLabels[0] = Helper.CreateLabel(7, 28, 6, 1, "LENGTH");

			screenMaps[0] = Helper.CreateMap(0, 0, 120, 28);
			screenMaps[0].bodyName = "Kerbin";

			screenButtons[0] = Helper.CreateButton(1, 29, 5, 1, "TAIL");
			screenButtons[0].Font = form.buttonFont;
			screenButtons[0].Click += (sender, e) => this.toggleTail(sender, e);

			screenButtons[1] = Helper.CreateButton(14, 29, 5, 1, "FADE");
			screenButtons[1].Font = form.buttonFont;
			screenButtons[1].Click += (sender, e) => this.toggleFade(sender, e);

			screenIndicators[0] = Helper.CreateIndicator(1, 28, 5, 1, "");
			screenIndicators[1] = Helper.CreateIndicator(14, 28, 5, 1, "");
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			int width;
			int height;
			int offsetX = 0;
			int offsetY = 0;
			int controls = (int)((form.pxPrLine * 2) + form.padding_bottom);

			if (form.ClientSize.Width > 2 * (form.ClientSize.Height - controls))
			{
				width = (form.ClientSize.Height - controls) * 2;
				height = form.ClientSize.Height - controls;
				offsetX = (int)Math.Round((form.ClientSize.Width - width) / 2f);
			}
			else
			{
				width = form.ClientSize.Width;
				height = (int)Math.Floor((form.ClientSize.Width / 2f) - controls);
				offsetY = (int)Math.Round((form.ClientSize.Height - height - controls) / 2f);
			}


			screenMaps[0].Width = width;
			screenMaps[0].Height = height;
			screenMaps[0].Location = new Point(offsetX, offsetY);
			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				screenMaps[0].body = screenStreams.GetData(DataType.orbit_celestialBody);
				screenMaps[0].vesselType = screenStreams.GetData(DataType.vessel_type);
				screenMaps[0].trackHistoryLat = form.form.chartData["lat"];
				screenMaps[0].trackHistoryLon = form.form.chartData["lon"];
				screenMaps[0].tail = tail;
				screenMaps[0].fade = fade;
				screenMaps[0].taillength = taillength;
				screenMaps[0].Invalidate();
			}

			screenInputs[0].Text = taillength.ToString();
		}

		public override void resize()
		{
			if (screenIndicators.Count > 0)
			{
				int w = form.ClientSize.Width;
				int h = form.ClientSize.Height;
				int pB = form.padding_bottom;

				screenIndicators[0].Location = new Point(screenIndicators[0].Location.X, (int)(h - (form.pxPrLine * 2) - pB));
				screenIndicators[1].Location = new Point(screenIndicators[1].Location.X, (int)(h - (form.pxPrLine * 2) - pB));

				screenButtons[0].Location = new Point(screenButtons[0].Location.X, (int)(h - (form.pxPrLine * 1) - pB));
				screenButtons[1].Location = new Point(screenButtons[1].Location.X, (int)(h - (form.pxPrLine * 1) - pB));

				screenLabels[0].Location = new Point(screenLabels[0].Location.X, (int)(h - (form.pxPrLine * 2) - pB));

				screenInputs[0].Location = new Point(screenInputs[0].Location.X, (int)(h - (form.pxPrLine * 1) - pB));
			}
		}

		public void toggleTail(object sender, EventArgs e)
		{
			if (screenIndicators[0].color == Indicator.status.AMBER)
			{
				screenIndicators[0].setStatus(Indicator.status.OFF);
				tail = false;
			}
			else
			{
				screenIndicators[0].setStatus(Indicator.status.AMBER);
				tail = true;
			}
		}

		public void toggleFade(object sender, EventArgs e)
		{
			if (screenIndicators[1].color == Indicator.status.AMBER)
			{
				screenIndicators[1].setStatus(Indicator.status.OFF);
				fade = false;
			}
			else
			{
				screenIndicators[1].setStatus(Indicator.status.AMBER);
				fade = true;
			}
		}

		public void updateTailLength(object sender, EventArgs e)
		{
			if (int.TryParse(screenInputs[0].Text, out int length))
			{
				if (length > 10000) length = 10000;
				taillength = length;
			}
		}

		public override bool keyDown(object sender, KeyEventArgs e)
		{
			Console.WriteLine(e.KeyCode.ToString());
			return false;
		}
	}
}
