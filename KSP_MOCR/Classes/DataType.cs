using System;
namespace KSP_MOCR
{
	public enum DataType
	{
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

		flight_gForce,
		flight_meanAltitude,
		flight_surfaceAltitude,
		flight_bedrockAltitude,
		flight_elevation,
		flight_latitude,
		flight_longitude,
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
		flight_prograde,
		flight_retrograde,
		flight_normal,
		flight_antiNormal,
		flight_radial,
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

		orbit_celestialBody, // CelestialBody
		orbit_apoapsis, // double
		orbit_periapsis, // double
		orbit_apoapsisAltitude, // double
		orbit_periapsisAltitude, // double
		orbit_semiMajorAxis, // double
		orbit_semiMinorAxis, // double
		orbit_radius, // double
		orbit_speed, // double
		orbit_period, // double
		orbit_timeToApoapsis, // double
		orbit_timeToPeriapsis, // double
		orbit_eccentricity, // double
		orbit_inclination, // double
		orbit_longitudeOfAscendingNode, // double
		orbit_argumentOfPeriapsis, // double
		orbit_meanAnomalyAtEpoch, // double
		orbit_epoch, // double
		orbit_meanAnomaly, // double
		orbit_eccentricAnomaly, // double
		orbit_trueAnomaly, // double
		orbit_orbitalSpeed, // double

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

		vessel_name,
		vesel_type,
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
		vessel_position,
		vessel_boundingBox,
		vessel_velocity,
		vessel_rotation,
		vessel_direction,
		vessel_angularVelocity
	}
}
