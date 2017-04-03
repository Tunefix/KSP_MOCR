using System;
using KRPC;
using KRPC.Client;
using KRPC.Client.Services;
using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;

namespace KSP_MOCR
{
	public static class GetData
	{
		private static KRPC.Client.Services.SpaceCenter.Service spaceCenter;

		private static Form1 form;

		static public void setForm(Form1 f){GetData.form = f;}

		static public void setSpaceCenter(KRPC.Client.Services.SpaceCenter.Service sc) { GetData.spaceCenter = sc; }

		static public Flight getFlight()
		{
			KRPC.Client.Services.SpaceCenter.Flight flight = null;
		    KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Flight> flight_stream;

			spaceCenter = form.connection.SpaceCenter();
			ReferenceFrame flightRef = spaceCenter.ActiveVessel.Orbit.Body.ReferenceFrame;

			if (form.system == Form1.OS.UNIX)
			{
				flight = spaceCenter.ActiveVessel.Flight(flightRef);
				Console.WriteLine("Got Flight with RPC");
			}
			else
			{
				flight_stream = form.connection.AddStream(() => spaceCenter.ActiveVessel.Flight(flightRef));
				flight = flight_stream.Get();
				Console.WriteLine("Got Flight with Stream");
			}

			return flight;
		}

		static public Orbit getOrbit(Connection con)
		{
			KRPC.Client.Services.SpaceCenter.Orbit orbit = null;
			KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Orbit> orbit_stream;

			spaceCenter = con.SpaceCenter();

			try
			{
				orbit_stream = con.AddStream(() => spaceCenter.ActiveVessel.Orbit);
				orbit = orbit_stream.Get();
			}
			catch (Exception)
			{
				orbit = spaceCenter.ActiveVessel.Orbit;
			}

			return orbit;
		}

		static public Vessel getVessel(Connection con)
		{
			KRPC.Client.Services.SpaceCenter.Vessel vessel = null;
			KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Vessel> vessel_stream;

			spaceCenter = con.SpaceCenter();

			try
			{
				vessel_stream = con.AddStream(() => spaceCenter.ActiveVessel);
				vessel = vessel_stream.Get();
			}
			catch (Exception)
			{
				vessel = spaceCenter.ActiveVessel;
			}

			return vessel;
		}
	}
}
