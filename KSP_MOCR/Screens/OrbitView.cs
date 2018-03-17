using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class OrbitView : MocrScreen
	{
		CelestialBody body;
		float bodyRadius;
		String bodyName;

		// TELEMETRY FIELDS
		double apopapsis;
		double periapsis;
		double eccentricity;
		double inclination;
		double sMa;
		double sma;
		double argOP;
		double lOAN;
		double radius;
		double trueAnomaly;
		double timeToPe;
		double timeToAp;
		double period;

		public OrbitView(Screen form)
		{
			this.form = form;
			this.screenStreams = form.streamCollection;

			this.charSize = false;
			this.width = 674;
			this.height = 508;
			this.updateRate = 1000;

			body = form.connection.SpaceCenter().ActiveVessel.Orbit.Body;
			bodyRadius = body.EquatorialRadius;
			bodyName = body.Name;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenOrbit = Helper.CreateOrbit(0, 0, 674, 508, true);
			IList<CelestialBody> bodySatellites = body.Satellites;
			screenOrbit.setBody(body, bodyRadius, bodyName, bodySatellites);
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				period = screenStreams.GetData(DataType.orbit_period);
				apopapsis = screenStreams.GetData(DataType.orbit_apoapsis);
				periapsis = screenStreams.GetData(DataType.orbit_periapsis);
				sMa = screenStreams.GetData(DataType.orbit_semiMajorAxis);
				sma = screenStreams.GetData(DataType.orbit_semiMinorAxis);
				argOP = screenStreams.GetData(DataType.orbit_argumentOfPeriapsis);
				lOAN = screenStreams.GetData(DataType.orbit_longitudeOfAscendingNode);
				eccentricity = screenStreams.GetData(DataType.orbit_eccentricity);
				inclination = screenStreams.GetData(DataType.orbit_inclination);
				radius = screenStreams.GetData(DataType.orbit_radius);
				trueAnomaly = screenStreams.GetData(DataType.orbit_trueAnomaly);


				screenOrbit.setOrbit(apopapsis, periapsis, sMa, sma, argOP, lOAN, radius, trueAnomaly);

				screenOrbit.Invalidate();
			}
		}

		public override void resize()
		{
			//screenOrbit.Size = form.ClientSize;
		}

		
	}
}
