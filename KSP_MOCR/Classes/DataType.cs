using System;
namespace KSP_MOCR
{
	public enum DataType
	{
		/// <summary>
		/// string
		/// </summary>
		body_name,
		/// <summary>
		/// float
		/// </summary>
		body_radius,
		/// <summary>
		/// float
		/// </summary>
		body_gravityParameter,
		/// <summary>
		/// float
		/// </summary>
		body_rotSpeed,
		/// <summary>
		/// float
		/// </summary>
		body_rotPeriod,
		/// <summary>
		/// float
		/// </summary>
		body_mass,
		/// <summary>
		/// The reference frame that is fixed relative to this celestial body, and orientated in a fixed direction (it does not rotate with the body).
		/// </summary>
		body_nonRotatingReferenceFrame,

		control_SAS,
		control_SASmode,
		control_speedMode,
		control_RCS,
		control_reactionWheels,
		control_gear,
		control_legs,
		control_wheels,
		control_lights,
		control_brakes,
		control_antennas,
		control_cargoBays,
		control_intakes,
		control_parachutes,
		control_radiators,
		control_resourceHarversters,
		control_resourceHarverstersActive,
		control_solarPanels,
		control_abort,
		control_throttle,
		control_pitch,
		control_yaw,
		control_roll,
		control_forward,
		control_up,
		control_right,
		control_wheelThrottle,
		control_wheelSteering,
		control_currentStage,
		control_actionGroup0,
		control_actionGroup1,
		control_actionGroup2,
		control_actionGroup3,
		control_actionGroup4,
		control_actionGroup5,
		control_actionGroup6,
		control_actionGroup7,
		control_actionGroup8,
		control_actionGroup9,
		/// <summary>
		/// Returns a list of all existing maneuver nodes, ordered by time from first to last.
		/// </summary>
		control_nodes,

		flight_gForce,
		/// <summary>
		/// The altitude above sea level, in meters.
		/// </summary>
		flight_meanAltitude,
		/// <summary>
		/// The altitude above the surface of the body or sea level, whichever is closer, in meters.
		/// </summary>
		flight_surfaceAltitude,
		/// <summary>
		/// The altitude above the surface of the body, in meters. When over water, this is the altitude above the sea floor.
		/// </summary>
		flight_bedrockAltitude,
		/// <summary>
		/// The elevation of the terrain under the vessel, in meters. This is the height of the terrain above sea level, and is negative when the vessel is over the sea.
		/// </summary>
		flight_elevation,
		flight_latitude,
		flight_longitude,
		flight_map_latitude,
		flight_map_longitude,
		flight_velocity,
		flight_speed,
		flight_horizontalSpeed,
		flight_verticalSpeed,
		flight_centerOfMass,
		flight_rotation,
		flight_direction,
		flight_pitch,
		flight_heading,
		flight_roll,
		/// <summary>
		/// Tuple<double, double, double>
		/// </summary>
		flight_prograde,
		/// <summary>
		/// Tuple<double, double, double>
		/// </summary>
		flight_retrograde,
		/// <summary>
		/// Tuple<double, double, double>
		/// </summary>
		flight_normal,
		/// <summary>
		/// Tuple<double, double, double>
		/// </summary>
		flight_antiNormal,
		/// <summary>
		/// Tuple<double, double, double>
		/// </summary>
		flight_radial,
		/// <summary>
		/// Tuple<double, double, double>
		/// </summary>
		flight_antiRadial,
		flight_atmosphereDensity,
		flight_dynamicPressure,
		flight_staticPressure,
		flight_staticPressureAtMSL,
		flight_aerodynamicForce,
		flight_lift,
		flight_drag,
		flight_speedOfSound,
		flight_mach,
		flight_trueAirSpeed,
		flight_equivalentAirSpeed,
		flight_terminalVelocity,
		flight_angleOfAttack,
		flight_sideslipAngle,
		flight_totalAirTemperature,
		flight_staticAirTemperature,
		

		flight_inertial_roll,
		flight_inertial_pitch,
		flight_inertial_yaw,
		flight_inertial_velocity,
		flight_inertial_speed,
		flight_inertial_direction,
		flight_inertial_rotation,

		/// <summary>
		/// Tuple<double, double, double>
		/// </summary>
		flight_inertial_prograde,
		/// <summary>
		/// Tuple<double, double, double>
		/// </summary>
		flight_inertial_retrograde,
		/// <summary>
		/// Tuple<double, double, double>
		/// </summary>
		flight_inertial_normal,
		/// <summary>
		/// Tuple<double, double, double>
		/// </summary>
		flight_inertial_antiNormal,
		/// <summary>
		/// Tuple<double, double, double>
		/// </summary>
		flight_inertial_radial,
		/// <summary>
		/// Tuple<double, double, double>
		/// </summary>
		flight_inertial_antiRadial,

		/// <summary>
		/// CelestialBody
		/// </summary>
		orbit_celestialBody,
		/// <summary>
		/// double, Gets the apoapsis of the orbit, in meters, from the center of mass of the body being orbited.
		/// </summary>
		orbit_apoapsis,
		/// <summary>
		/// double, The periapsis of the orbit, in meters, from the center of mass of the body being orbited.
		/// </summary>
		orbit_periapsis,
		/// <summary>
		/// double, The apoapsis of the orbit, in meters, above the sea level of the body being orbited.
		/// </summary>
		orbit_apoapsisAltitude,
		/// <summary>
		/// double, The periapsis of the orbit, in meters, above the sea level of the body being orbited.
		/// </summary>
		orbit_periapsisAltitude,
		/// <summary>
		/// double
		/// </summary>
		orbit_semiMajorAxis,
		/// <summary>
		/// double
		/// </summary>
		orbit_semiMinorAxis,
		/// <summary>
		/// double
		/// </summary>
		orbit_radius,
		/// <summary>
		/// double
		/// </summary>
		orbit_speed,
		/// <summary>
		/// double
		/// </summary>
		orbit_period,
		/// <summary>
		/// double
		/// </summary>
		orbit_timeToApoapsis, // double
		/// <summary>
		/// double
		/// </summary>
		orbit_timeToPeriapsis, // double
		/// <summary>
		/// double
		/// </summary>
		orbit_eccentricity, // double
		/// <summary>
		/// double
		/// </summary>
		orbit_inclination, // double
		/// <summary>
		/// double
		/// </summary>
		orbit_longitudeOfAscendingNode, // double
		/// <summary>
		/// double
		/// </summary>
		orbit_argumentOfPeriapsis, // double
		/// <summary>
		/// double
		/// </summary>
		orbit_meanAnomalyAtEpoch, // double
		/// <summary>
		/// double
		/// </summary>
		orbit_epoch, // double
		/// <summary>
		/// double
		/// </summary>
		orbit_meanAnomaly, // double
		/// <summary>
		/// double
		/// </summary>
		orbit_eccentricAnomaly, // double
		/// <summary>
		/// double
		/// </summary>
		orbit_trueAnomaly, // double
		/// <summary>
		/// double
		/// </summary>
		orbit_orbitalSpeed, // double
		/// <summary>
		/// double
		/// </summary>
		orbit_timeToSOIChange,


		resource_total_max_electricCharge,
		resource_total_max_monoPropellant,
		resource_total_max_liquidFuel,
		resource_total_max_oxidizer,

		resource_total_amount_electricCharge,
		resource_total_amount_monoPropellant,
		resource_total_amount_liquidFuel,
		resource_total_amount_oxidizer,

		resource_stage_max_electricCharge,
		resource_stage_max_monoPropellant,
		resource_stage_max_liquidFuel,
		resource_stage_3_max_liquidFuel,
		resource_stage_max_oxidizer,

		resource_stage_amount_electricCharge,
		resource_stage_amount_monoPropellant,
		resource_stage_amount_liquidFuel,
		resource_stage_amount_oxidizer,
		
		spacecenter_universial_time,

		vessel_name,
		vessel_type,
		vessel_situation,
		vessel_recoverable,
		vessel_MET,
		vessel_biome,
		vessel_mass,
		vessel_dryMass,
		vessel_thrust,
		vessel_availableThrust,
		vessel_maxThrust,
		vessel_maxVacuumThrust,
		vessel_specificImpulse,
		vessel_vacuumSpecificImpulse,
		vessel_kerbinSeaLevelSpecificImpulse,
		vessel_momentOfInertia,
		vessel_inertiaTensor,
		vessel_availableTorque,
		vessel_availableReactionWhelTorque,
		vessel_availableEngineTorque,
		vessel_availableControlSurfaceTorque,
		vessel_availableOtherTorque,
		vessel_referenceFrame,
		vessel_orbitalReferenceFrame,
		vessel_surfaceReferenceFrame,
		vessel_surfaceVelocityReferenceFrame,
		/// <summary>
		/// Tuple<double, double, double>
		/// The position of the center of mass of the vessel, in the given reference frame.
		/// </summary>
		vessel_position,
		vessel_boundingBox,
		/// <summary>
		/// Tuple<double, double, double>
		/// The velocity of the center of mass of the vessel, in the given reference frame.
		/// </summary>
		vessel_velocity,
		vessel_rotation,
		vessel_direction,
		vessel_angularVelocity,
		/// <summary>
		/// Parts
		/// </summary>
		vessel_parts,
		/// <summary>
		/// Orbit, The current orbit of the vessel.
		/// </summary>
		vessel_orbit,
		vessel_autoPilot
	}
}
