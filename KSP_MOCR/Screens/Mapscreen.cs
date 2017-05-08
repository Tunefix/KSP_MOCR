using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KRPC.Client.Services.SpaceCenter;

namespace KSP_MOCR
{
	public class MapScreen : MocrScreen
	{
		CelestialBody body;
		Dictionary<int, PointF?> trackHistory = new Dictionary<int, PointF?>();
		double MET;
		double longitude;
		double latitude;
		
		public MapScreen(Form1 form)
		{
			this.form = form;
			this.chartData = form.chartData;
			screenStreams = new StreamCollection(form.connection);

			this.form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

			this.width = 120;
			this.height = 30;
			
			this.updateRate = 1000;

			this.body = form.connection.SpaceCenter().ActiveVessel.Orbit.Body;

			for (int i = 0; i < 1200; i++)
			{
				trackHistory.Add(i, null);
			}
		}
		
		public override void keyUp(object sender, KeyEventArgs e)
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
			}
		}
		
		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenMaps.Add(null); // Initialize Map
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenMaps[0] = Helper.CreateMap(0,0,60,30);
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			MET = screenStreams.GetData(DataType.vessel_MET);
			longitude = screenStreams.GetData(DataType.flight_map_longitude);
			latitude = screenStreams.GetData(DataType.flight_map_latitude);

			if ((int)(Math.Round(MET)) % 20 == 0)
			{
				for (int i = 1; i < trackHistory.Count; i++)
				{
					trackHistory[i - 1] = trackHistory[i];
				}
				trackHistory[trackHistory.Count - 1] = new PointF((float)longitude, (float)latitude);
			}

			int width;
			int height;
			int offsetX = 0;
			int offsetY = 0;
			if (form.ClientSize.Width > 2 * form.ClientSize.Height)
			{
				width = form.ClientSize.Height * 2;
				height = form.ClientSize.Height;
				offsetX = (int)Math.Round((form.ClientSize.Width - width) / 2f);
			}
			else
			{
				width = form.ClientSize.Width;
				height = (int)Math.Floor(form.ClientSize.Width / 2f);
				offsetY = (int)Math.Round((form.ClientSize.Height - height) / 2f);
			}
			screenMaps[0].Width = width;
			screenMaps[0].Height = height;
			screenMaps[0].Location = new Point(offsetX, offsetY);
			screenMaps[0].body = screenStreams.GetData(DataType.orbit_celestialBody);
			screenMaps[0].vesselType = screenStreams.GetData(DataType.vessel_type);
			screenMaps[0].longitude = longitude;
			screenMaps[0].latitude = latitude;
			screenMaps[0].positionVector = screenStreams.GetData(DataType.vessel_position);
			screenMaps[0].velocityVector = screenStreams.GetData(DataType.vessel_velocity);
			screenMaps[0].bodyGravityParam = screenStreams.GetData(DataType.body_gravityParameter);
			screenMaps[0].bodyRadius = screenStreams.GetData(DataType.body_radius);
			screenMaps[0].bodyRotSpeed = screenStreams.GetData(DataType.body_rotSpeed);
			screenMaps[0].trackHistory = trackHistory;
		}
	}
}
