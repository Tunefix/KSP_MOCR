using System;
using System.Collections.Generic;
using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;

namespace KSP_MOCR
{
	class FDO :MocrScreen
	{
		CelestialBody body;
		float bodyRadius;
		String bodyName;


		public FDO(Form1 form)
		{
            this.form = form;
			this.chartData = form.chartData;
			screenStreams = new StreamCollection(form.connection);

			this.width = 120;
			this.height = 30;

			body = form.connection.SpaceCenter().ActiveVessel.Orbit.Body;
			bodyRadius = body.EquatorialRadius;
			bodyName = body.Name;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			if (form.connected && form.krpc.CurrentGameScene == GameScene.Flight)
			{
				double apopapsis = screenStreams.GetData(DataType.orbit_apoapsis);
				double periapsis = screenStreams.GetData(DataType.orbit_periapsis);
				double sMa = screenStreams.GetData(DataType.orbit_semiMajorAxis);
				double sma = screenStreams.GetData(DataType.orbit_semiMinorAxis);
				double argOP = screenStreams.GetData(DataType.orbit_argumentOfPeriapsis);
				double lOAN = screenStreams.GetData(DataType.orbit_longitudeOfAscendingNode);
				double radius = screenStreams.GetData(DataType.orbit_radius);
				double trueAnomaly = screenStreams.GetData(DataType.orbit_trueAnomaly);

				screenOrbit.setOrbit(apopapsis, periapsis, sMa, sma, argOP, lOAN, radius, trueAnomaly);
				screenOrbit.Invalidate();
			}	
		}


		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenCharts.Add(null); // Initialize Charts
			for (int i = 0; i < 1; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs

			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateLabel(39, 0, 42, 1, "============ FLIGHT DYNAMICS =============");

			// OrbitGraph
			screenOrbit = Helper.CreateOrbit(60, 1, 60, 30);
			screenOrbit.setBody(body, bodyRadius, bodyName);
		}
	}
}
