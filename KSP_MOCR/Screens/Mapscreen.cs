using System;
using System.Collections.Generic;
using System.Drawing;
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

			this.width = 120;
			this.height = 30;
			
			this.updateRate = 1000;

			this.body = form.connection.SpaceCenter().ActiveVessel.Orbit.Body;

			for (int i = 0; i < 1200; i++)
			{
				trackHistory.Add(i, null);
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
		
			screenMaps[0].Width = form.ClientSize.Width;
			screenMaps[0].Height = form.ClientSize.Height;
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
