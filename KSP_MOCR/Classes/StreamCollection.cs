using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using KRPC.Client;
using KRPC.Client.Services;
using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;

namespace KSP_MOCR
{
	public sealed class StreamCollection
	{
		private KRPC.Client.Connection connection;
		private static Dictionary<DataType, Kstream> streams = new Dictionary<DataType, Kstream>();

		private static readonly StreamCollection instance = new StreamCollection();

		private int stage;
		private bool hasConnection = false;

		public GameScene gameScene = GameScene.SpaceCenter;

		public System.Object streamlock = new System.Object();

		// Some much used variables
		KRPC.Client.Services.SpaceCenter.Service spaceCenter;
		Flight flight;
		Vessel vessel;
		Control control;
		Orbit orbit;
		Resources resources;
		Resources resources_stage;
		ReferenceFrame inertialRefFrame;
		ReferenceFrame surfaceRefFrame;
		ReferenceFrame mapRefFrame;
		Flight inertFlight;
		Flight mapFlight;

		static StreamCollection()
		{
		}

		private StreamCollection()
		{
		}

		public static StreamCollection Instance
		{
			get
			{
				return instance;
			}
		}

		public StreamCollection(Connection con)
		{
			connection = con;
			hasConnection = true;
		}

		public void setConnection(Connection con)
		{
			connection = con;
			hasConnection = true;
		}

		public void setGameScene(GameScene s)
		{
			gameScene = s;

			// CLEAR STREAMS IF SCENE IS NOT FLIGHT
			if(s != GameScene.Flight && streams.Count > 0)
			{
				CloseStreams();
			}
		}

		public dynamic GetData(DataType type){return GetData(type, false);}
		public dynamic GetData(DataType type, bool force_reStream)
		{
			if (hasConnection && gameScene == GameScene.Flight)
			{
				if (!streams.ContainsKey(type) || force_reStream)
				{
					// If forced, clear out old stream (is it exists)
					if (force_reStream && streams.ContainsKey(type))
					{
						streams[type].Remove(); streams.Remove(type);
						//Console.WriteLine(DateTime.Now.ToString() + "." + DateTime.Now.Millisecond.ToString() + " REMOVED STREAM: " + type.ToString());
						// GIVE THE SERVER SOME TIME TO REACT
						Thread.Sleep(1);
					}

					try
					{
						addStream(type);
						//Console.WriteLine(DateTime.Now.ToString() + "." + DateTime.Now.Millisecond.ToString() + " ADDED STREAM: " + type.ToString());
					}
					catch (Exception e)
					{
						Console.WriteLine(e.GetType().ToString() + ":" + e.Message + "\n" + e.StackTrace);
						return null;
					}
				}

				dynamic output;
				try
				{
					Kstream stream = streams[type];
					output = stream.Get();
				}
				catch (Exception e)
				{
					Console.WriteLine(e.GetType().ToString() + ":" + e.Message + "\n" + e.StackTrace);
					output = GetData(type, true);
				}

				return output;
			}
			return getNullResult(type);
		}

		private dynamic getNullResult(DataType type)
		{
			switch (type)
			{
				case DataType.vessel_type:
					return VesselType.Ship;
				case DataType.orbit_celestialBody:
				case DataType.vessel_parts:
				case DataType.vessel_referenceFrame:
					return null;
				case DataType.flight_inertial_direction:
				case DataType.flight_direction:
				case DataType.vessel_position:
				case DataType.vessel_velocity:
				case DataType.flight_inertial_prograde:
				case DataType.flight_inertial_retrograde:
				case DataType.flight_inertial_radial:
				case DataType.flight_inertial_antiRadial:
				case DataType.flight_inertial_normal:
				case DataType.flight_inertial_antiNormal:
				case DataType.flight_prograde:
				case DataType.flight_retrograde:
				case DataType.flight_radial:
				case DataType.flight_antiRadial:
				case DataType.flight_normal:
				case DataType.flight_antiNormal:
					return new Tuple<double, double, double>(1, 1, 1);
				case DataType.flight_inertial_rotation:
				case DataType.flight_rotation:
					return new Tuple<double, double, double, double>(1, 1, 1, 0);
				case DataType.body_name:
					return "";
				case DataType.control_SAS:
				case DataType.control_RCS:
				case DataType.control_abort:
				case DataType.control_actionGroup0:
				case DataType.control_actionGroup1:
				case DataType.control_actionGroup2:
				case DataType.control_actionGroup3:
				case DataType.control_actionGroup4:
				case DataType.control_actionGroup5:
				case DataType.control_actionGroup6:
				case DataType.control_actionGroup7:
				case DataType.control_actionGroup8:
				case DataType.control_actionGroup9:
				case DataType.control_antennas:
				case DataType.control_brakes:
				case DataType.control_cargoBays:
				case DataType.control_lights:
				case DataType.control_gear:
					return false;
				default:
					return 0;
			}
		}

		public void CloseStreams()
		{
			lock(streamlock)
			{
				foreach (KeyValuePair<DataType, Kstream> stream in streams)
				{
					stream.Value.Remove();
				}
				streams.Clear();
			}
		}

		public void setStage(int stage)
		{
			this.stage = stage;
		}

		private void addStream(DataType type)
		{
			// Some much used variables
			spaceCenter = connection.SpaceCenter();
			
			vessel = connection.SpaceCenter().ActiveVessel;
			control = connection.SpaceCenter().ActiveVessel.Control;
			orbit = connection.SpaceCenter().ActiveVessel.Orbit;
			resources = connection.SpaceCenter().ActiveVessel.Resources;
			resources_stage =  connection.SpaceCenter().ActiveVessel.ResourcesInDecoupleStage(stage, false);
			inertialRefFrame = orbit.Body.NonRotatingReferenceFrame;
			mapRefFrame = orbit.Body.ReferenceFrame;
			inertFlight = connection.SpaceCenter().ActiveVessel.Flight(inertialRefFrame);
			surfaceRefFrame = vessel.SurfaceReferenceFrame;
			flight = connection.SpaceCenter().ActiveVessel.Flight(surfaceRefFrame);
			mapFlight = connection.SpaceCenter().ActiveVessel.Flight(mapRefFrame);

			
			Kstream stream;

			switch (type)
			{
				///// BODY DATA /////
				case DataType.body_radius:
					stream = new floatStream(connection.AddStream(() => orbit.Body.EquatorialRadius));
					break;
					
				case DataType.body_gravityParameter:
					stream = new floatStream(connection.AddStream(() => orbit.Body.GravitationalParameter));
					break;
					
				case DataType.body_rotSpeed:
					stream = new floatStream(connection.AddStream(() => orbit.Body.RotationalSpeed));
					break;
					
				case DataType.body_name:
					stream = new stringStream(connection.AddStream(() => orbit.Body.Name));
					break;

				case DataType.body_mass:
					stream = new floatStream(connection.AddStream(() => orbit.Body.Mass));
					break;

				case DataType.body_rotPeriod:
					stream = new floatStream(connection.AddStream(() => orbit.Body.RotationalPeriod));
					break;

				///// CONTROL DATA /////

				case DataType.control_SAS:
					stream = new boolStream(connection.AddStream(() => control.SAS));
					break;

				case DataType.control_SASmode:
					stream = new sasModeStream(connection.AddStream(() => control.SASMode));
					break;

				case DataType.control_RCS:
					stream = new boolStream(connection.AddStream(() => control.RCS));
					break;

				case DataType.control_gear:
					stream = new boolStream(connection.AddStream(() => control.Gear));
					break;

				case DataType.control_brakes:
					stream = new boolStream(connection.AddStream(() => control.Brakes));
					break;

				case DataType.control_lights:
					stream = new boolStream(connection.AddStream(() => control.Lights));
					break;

				case DataType.control_abort:
					stream = new boolStream(connection.AddStream(() => control.Abort));
					break;

				case DataType.control_actionGroup0:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(0)));
					break;

				case DataType.control_actionGroup1:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(1)));
					break;

				case DataType.control_actionGroup2:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(2)));
					break;

				case DataType.control_actionGroup3:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(3)));
					break;

				case DataType.control_actionGroup4:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(4)));
					break;

				case DataType.control_actionGroup5:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(5)));
					break;

				case DataType.control_actionGroup6:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(6)));
					break;

				case DataType.control_actionGroup7:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(7)));
					break;

				case DataType.control_actionGroup8:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(8)));
					break;

				case DataType.control_actionGroup9:
					stream = new boolStream(connection.AddStream(() => control.GetActionGroup(9)));
					break;

				case DataType.control_throttle:
					stream = new floatStream(connection.AddStream(() => control.Throttle));
					break;

				case DataType.control_currentStage:
					stream = new intStream(connection.AddStream(() => control.CurrentStage));
					break;

				///// FLIGHT DATA /////

				case DataType.flight_gForce:
					stream = new floatStream(connection.AddStream(() => flight.GForce));
					break;

				case DataType.flight_angleOfAttack:
					stream = new floatStream(connection.AddStream(() => flight.AngleOfAttack));
					break;

				case DataType.flight_meanAltitude:
					stream = new doubleStream(connection.AddStream(() => flight.MeanAltitude));
					break;

				case DataType.flight_surfaceAltitude:
					stream = new doubleStream(connection.AddStream(() => flight.SurfaceAltitude));
					break;

				case DataType.flight_bedrockAltitude:
					stream = new doubleStream(connection.AddStream(() => flight.BedrockAltitude));
					break;

				case DataType.flight_elevation:
					stream = new doubleStream(connection.AddStream(() => flight.Elevation));
					break;

				case DataType.flight_latitude:
					stream = new doubleStream(connection.AddStream(() => flight.Latitude));
					break;

				case DataType.flight_longitude:
					stream = new doubleStream(connection.AddStream(() => flight.Longitude));
					break;

				case DataType.flight_map_latitude:
					stream = new doubleStream(connection.AddStream(() => mapFlight.Latitude));
					break;

				case DataType.flight_map_longitude:
					stream = new doubleStream(connection.AddStream(() => mapFlight.Longitude));
					break;

				case DataType.flight_velocity:
					stream = new tuple3Stream(connection.AddStream(() => flight.Velocity));
					break;

				case DataType.flight_speed:
					stream = new doubleStream(connection.AddStream(() => flight.Speed));
					break;

				case DataType.flight_horizontalSpeed:
					stream = new doubleStream(connection.AddStream(() => flight.HorizontalSpeed));
					break;

				case DataType.flight_verticalSpeed:
					stream = new doubleStream(connection.AddStream(() => flight.VerticalSpeed));
					break;

				case DataType.flight_centerOfMass:
					stream = new tuple3Stream(connection.AddStream(() => flight.CenterOfMass));
					break;

				case DataType.flight_rotation:
					stream = new tuple4Stream(connection.AddStream(() => flight.Rotation));
					break;

				case DataType.flight_direction:
					stream = new tuple3Stream(connection.AddStream(() => flight.Direction));
					break;

				case DataType.flight_pitch:
					stream = new floatStream(connection.AddStream(() => flight.Pitch));
					break;

				case DataType.flight_heading:
					stream = new floatStream(connection.AddStream(() => flight.Heading));
					break;

				case DataType.flight_roll:
					stream = new floatStream(connection.AddStream(() => flight.Roll));
					break;

				case DataType.flight_atmosphereDensity:
					stream = new floatStream(connection.AddStream(() => flight.AtmosphereDensity));
					break;

				case DataType.flight_dynamicPressure:
					stream = new floatStream(connection.AddStream(() => flight.DynamicPressure));
					break;

				case DataType.flight_staticPressure:
					stream = new floatStream(connection.AddStream(() => flight.StaticPressure));
					break;

				case DataType.flight_prograde:
					stream = new tuple3Stream(connection.AddStream(() => flight.Prograde));
					break;

				case DataType.flight_retrograde:
					stream = new tuple3Stream(connection.AddStream(() => flight.Retrograde));
					break;

				case DataType.flight_radial:
					stream = new tuple3Stream(connection.AddStream(() => flight.Radial));
					break;

				case DataType.flight_antiRadial:
					stream = new tuple3Stream(connection.AddStream(() => flight.AntiRadial));
					break;

				case DataType.flight_normal:
					stream = new tuple3Stream(connection.AddStream(() => flight.Normal));
					break;

				case DataType.flight_antiNormal:
					stream = new tuple3Stream(connection.AddStream(() => flight.AntiNormal));
					break;


				///// INERTIAL FLIGHT DATA /////

				case DataType.flight_inertial_roll:
					stream = new floatStream(connection.AddStream(() => inertFlight.Roll));
					break;
				case DataType.flight_inertial_pitch:
					stream = new floatStream(connection.AddStream(() => inertFlight.Pitch));
					break;
				case DataType.flight_inertial_yaw:
					stream = new floatStream(connection.AddStream(() => inertFlight.Heading));
					break;
				case DataType.flight_inertial_direction:
					stream = new tuple3Stream(connection.AddStream(() => inertFlight.Direction));
					break;
				case DataType.flight_inertial_rotation:
					stream = new tuple4Stream(connection.AddStream(() => inertFlight.Rotation));
					break;
				case DataType.flight_inertial_velocity:
					stream = new tuple3Stream(connection.AddStream(() => inertFlight.Velocity));
					break;

				case DataType.flight_inertial_prograde:
					stream = new tuple3Stream(connection.AddStream(() => inertFlight.Prograde));
					break;
				case DataType.flight_inertial_retrograde:
					stream = new tuple3Stream(connection.AddStream(() => inertFlight.Retrograde));
					break;
				case DataType.flight_inertial_radial:
					stream = new tuple3Stream(connection.AddStream(() => inertFlight.Radial));
					break;
				case DataType.flight_inertial_antiRadial:
					stream = new tuple3Stream(connection.AddStream(() => inertFlight.AntiRadial));
					break;
				case DataType.flight_inertial_normal:
					stream = new tuple3Stream(connection.AddStream(() => inertFlight.Normal));
					break;
				case DataType.flight_inertial_antiNormal:
					stream = new tuple3Stream(connection.AddStream(() => inertFlight.AntiNormal));
					break;


				///// ORBIT DATA /////

				case DataType.orbit_apoapsisAltitude:
					stream = new doubleStream(connection.AddStream(() => orbit.ApoapsisAltitude));
					break;

				case DataType.orbit_apoapsis:
					stream = new doubleStream(connection.AddStream(() => orbit.Apoapsis));
					break;

				case DataType.orbit_periapsisAltitude:
					stream = new doubleStream(connection.AddStream(() => orbit.PeriapsisAltitude));
					break;

				case DataType.orbit_periapsis:
					stream = new doubleStream(connection.AddStream(() => orbit.Periapsis));
					break;

				case DataType.orbit_radius:
					stream = new doubleStream(connection.AddStream(() => orbit.Radius));
					break;

				case DataType.orbit_speed:
					stream = new doubleStream(connection.AddStream(() => orbit.Speed));
					break;

				case DataType.orbit_celestialBody:
					stream = new celestialBodyStream(connection.AddStream(() => orbit.Body));
					break;

				case DataType.orbit_semiMajorAxis:
					stream = new doubleStream(connection.AddStream(() => orbit.SemiMajorAxis));
					break;

				case DataType.orbit_semiMinorAxis:
					stream = new doubleStream(connection.AddStream(() => orbit.SemiMinorAxis));
					break;

				case DataType.orbit_argumentOfPeriapsis:
					stream = new doubleStream(connection.AddStream(() => orbit.ArgumentOfPeriapsis));
					break;

				case DataType.orbit_longitudeOfAscendingNode:
					stream = new doubleStream(connection.AddStream(() => orbit.LongitudeOfAscendingNode));
					break;

				case DataType.orbit_eccentricity:
					stream = new doubleStream(connection.AddStream(() => orbit.Eccentricity));
					break;

				case DataType.orbit_inclination:
					stream = new doubleStream(connection.AddStream(() => orbit.Inclination));
					break;

				case DataType.orbit_trueAnomaly:
					stream = new doubleStream(connection.AddStream(() => orbit.TrueAnomaly));
					break;
					
				case DataType.orbit_timeToApoapsis:
					stream = new doubleStream(connection.AddStream(() => orbit.TimeToApoapsis));
					break;
					
				case DataType.orbit_timeToPeriapsis:
					stream = new doubleStream(connection.AddStream(() => orbit.TimeToPeriapsis));
					break;
					
				case DataType.orbit_period:
					stream = new doubleStream(connection.AddStream(() => orbit.Period));
					break;

				case DataType.orbit_timeToSOIChange:
					stream = new doubleStream(connection.AddStream(() => orbit.TimeToSOIChange));
					break;



				///// RESOURCE DATA /////

				case DataType.resource_total_max_electricCharge:
					stream = new floatStream(connection.AddStream(() => resources.Max("ElectricCharge")));
					break;

				case DataType.resource_total_amount_electricCharge:
					stream = new floatStream(connection.AddStream(() => resources.Amount("ElectricCharge")));
					break;

				case DataType.resource_stage_max_electricCharge:
					stream = new floatStream(connection.AddStream(() => resources_stage.Max("ElectricCharge")));
					break;

				case DataType.resource_stage_amount_electricCharge:
					stream = new floatStream(connection.AddStream(() => resources_stage.Amount("ElectricCharge")));
					break;


				case DataType.resource_total_max_monoPropellant:
					stream = new floatStream(connection.AddStream(() => resources.Max("MonoPropellant")));
					break;

				case DataType.resource_total_amount_monoPropellant:
					stream = new floatStream(connection.AddStream(() => resources.Amount("MonoPropellant")));
					break;

				case DataType.resource_stage_max_monoPropellant:
					stream = new floatStream(connection.AddStream(() => resources_stage.Max("MonoPropellant")));
					break;

				case DataType.resource_stage_amount_monoPropellant:
					stream = new floatStream(connection.AddStream(() => resources_stage.Amount("MonoPropellant")));
					break;


				case DataType.resource_total_max_liquidFuel:
					stream = new floatStream(connection.AddStream(() => resources.Max("LiquidFuel")));
					break;

				case DataType.resource_total_amount_liquidFuel:
					stream = new floatStream(connection.AddStream(() => resources.Amount("LiquidFuel")));
					break;

				case DataType.resource_stage_max_liquidFuel:
					stream = new floatStream(connection.AddStream(() => resources_stage.Max("LiquidFuel")));
					break;

				case DataType.resource_stage_amount_liquidFuel:
					stream = new floatStream(connection.AddStream(() => resources_stage.Amount("LiquidFuel")));
					break;


				case DataType.resource_stage_max_oxidizer:
					stream = new floatStream(connection.AddStream(() => resources_stage.Max("Oxidizer")));
					break;

				case DataType.resource_stage_amount_oxidizer:
					stream = new floatStream(connection.AddStream(() => resources_stage.Amount("Oxidizer")));
					break;
				case DataType.resource_total_max_oxidizer:
					stream = new floatStream(connection.AddStream(() => resources.Max("Oxidizer")));
					break;

				case DataType.resource_total_amount_oxidizer:
					stream = new floatStream(connection.AddStream(() => resources.Amount("Oxidizer")));
					break;
					
					
				///// SPACECENTER DATA /////
				case DataType.spacecenter_universial_time:
					stream = new doubleStream(connection.AddStream(() => spaceCenter.UT));
					break;


				///// VESSEL DATA /////
				case DataType.vessel_MET:
					stream = new doubleStream(connection.AddStream(() => vessel.MET));
					break;
				case DataType.vessel_type:
					stream = new vesselTypeStream(connection.AddStream(() => vessel.Type));
					break;
				case DataType.vessel_mass:
					stream = new floatStream(connection.AddStream(() => vessel.Mass));
					break;
				case DataType.vessel_dryMass:
					stream = new floatStream(connection.AddStream(() => vessel.DryMass));
					break;

				case DataType.vessel_position:
					stream = new tuple3Stream(connection.AddStream(() => vessel.Position(vessel.Orbit.Body.NonRotatingReferenceFrame)));
					break;
				case DataType.vessel_velocity:
					stream = new tuple3Stream(connection.AddStream(() => vessel.Velocity(vessel.Orbit.Body.NonRotatingReferenceFrame)));
					break;
				case DataType.vessel_parts:
					stream = new vesselPartsStream(connection.AddStream(() => vessel.Parts));
					break;

				case DataType.vessel_referenceFrame:
					stream = new referenceFrameStream(connection.AddStream(() => vessel.ReferenceFrame));
					break;


				default:
					throw (new Exception("DataType: " + type.ToString() + " not supported"));
			}

			// Safety check if type already exists in streams
			if (!streams.ContainsKey(type))
			{
				streams.Add(type, stream);
			}
		}

		private void getFlight()
		{
			flight = connection.SpaceCenter().ActiveVessel.Flight();
		}

		private void getVessel()
		{
			vessel = connection.SpaceCenter().ActiveVessel;
		}

		private void getControl()
		{
			control = connection.SpaceCenter().ActiveVessel.Control;
		}

		private void getOrbit()
		{
			orbit = connection.SpaceCenter().ActiveVessel.Orbit;
		}

		private void getResources()
		{
			resources = connection.SpaceCenter().ActiveVessel.Resources;
		}

		private void getResourcesStage(int stg)
		{
			resources_stage = connection.SpaceCenter().ActiveVessel.ResourcesInDecoupleStage(stg, false);
		}

		private void getInertialRefFrame()
		{
			inertialRefFrame = orbit.Body.NonRotatingReferenceFrame;
		}

		private void getInertFlight()
		{
			getInertialRefFrame();
			inertFlight = connection.SpaceCenter().ActiveVessel.Flight(inertialRefFrame);
		}

		public string getStatus()
		{
			string output = "STREAMS\n";

			foreach(KeyValuePair<DataType, Kstream> stream in streams)
			{
				output += stream.Key.ToString() + "\n";
			}

			return output;
		}
	}
}
