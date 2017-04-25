using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;
using KRPC.Client.Services.Drawing;

namespace KSP_MOCR
{
	class FDO :MocrScreen
	{
		CelestialBody body;
		float bodyRadius;
		String bodyName;
		
		// TELEMETRY FIELDS
		double MET = 0;
		double UT = 0;
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
		
		NumberFormatInfo format = new NumberFormatInfo();
			


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
			
			format.NumberGroupSeparator = "";
			format.NumberDecimalDigits = 10;
			format.NumberDecimalSeparator = ".";
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			// Always update Local Time
			screenLabels[1].Text = " LT: " + Helper.timeString(DateTime.Now.TimeOfDay.TotalSeconds);
			
			if (form.connected && form.krpc.CurrentGameScene == GameScene.Flight)
			{
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
				timeToPe = screenStreams.GetData(DataType.orbit_timeToPeriapsis);
				timeToAp = screenStreams.GetData(DataType.orbit_timeToApoapsis);
				
				MET = screenStreams.GetData(DataType.vessel_MET);
				UT = screenStreams.GetData(DataType.spacecenter_universial_time);
				
				screenLabels[2].Text = "MET: " + Helper.timeString(MET, 3);
				screenLabels[3].Text = "UT: " + Helper.timeString(UT, 5);

				// ORBIT DATA DATA AND LABELS
				double nextAp = MET + timeToAp;
				double nextPe = MET + timeToPe;

				screenLabels[30].Text = "NEXT PER: " + Helper.timeString(nextPe, 4);
				screenLabels[31].Text = "NEXT APO: " + Helper.timeString(nextAp, 4);

				// BURN-RESULTANT ORBIT
				
				double TIG = -1;
				try
				{
					if (screenInputs[0].Text != "") { TIG += int.Parse(screenInputs[0].Text) * 3600; }
					if (screenInputs[1].Text != "") { TIG += int.Parse(screenInputs[1].Text) * 60; }
					if (screenInputs[2].Text != "") { TIG += double.Parse(screenInputs[2].Text, format); }
					TIG += (UT - MET);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				screenLabels[20].Text = "BUT: " + Helper.timeString(TIG, 5);
				double TAAUT = form.connection.SpaceCenter().ActiveVessel.Orbit.TrueAnomalyAtUT(TIG);
				float my = form.connection.SpaceCenter().ActiveVessel.Orbit.Body.GravitationalParameter;

				// Make orbital plane vectors
				Tuple<double, double, double> velocityVectorInPlane = getVelocityVector(sMa, TAAUT, eccentricity, my);
				Tuple<double, double, double> positionVectorInPlane = getPositionVector(sMa, TAAUT, eccentricity);
				Tuple<Tuple<double, double, double>, Tuple<double, double, double>, Tuple<double, double, double>> referencePlane = getGeocentricReferenceFrame(lOAN, inclination, argOP);

				double alpha = Math.Acos(velocityVectorInPlane.Item1 / Math.Sqrt(Math.Pow(velocityVectorInPlane.Item1, 2) + Math.Pow(velocityVectorInPlane.Item2, 2)));
				if (velocityVectorInPlane.Item1 > 0)
				{
					if (velocityVectorInPlane.Item2 > 0)
					{
						// Leave alpha as is
					}
					else
					{
						alpha = -alpha;
					}
				}
				else
				{
					if (velocityVectorInPlane.Item2 > 0)
					{
						alpha = Math.PI - alpha;
					}
					else
					{
						alpha = -(Math.PI - alpha);
					}
				}
				
				String totalV = "N.A.";

				Tuple<double, double, double> burnVector = new Tuple<double, double, double>(0,0,0);
				
				try
				{
					double bX = 0;
					double bY = 0;
					double bZ = 0;
					if (screenInputs[3].Text != "") { bX = double.Parse(screenInputs[3].Text, format); } // Pro/retro
					if (screenInputs[4].Text != "") { bY = double.Parse(screenInputs[4].Text, format); } // Radial in/out
					if (screenInputs[5].Text != "") { bZ = double.Parse(screenInputs[5].Text, format); } // Normal / AntiNormal
					
					totalV = Helper.toFixed(Math.Sqrt((bX * bX) + (bY * bY) + (bZ * bZ)),3);
					burnVector = new Tuple<double, double, double>(bX,bY,bZ);

					velocityVectorInPlane = vectorChangeMagnitude(velocityVectorInPlane, bX);
					velocityVectorInPlane = vectorAddLeftMagnitude(velocityVectorInPlane, bY);
					velocityVectorInPlane = vectorAddUpMagnitude(velocityVectorInPlane, bZ);

					
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

				
				
				// Tranform vectors
				Tuple<double, double, double> burnVectorInPlane = rotateVectorAroundZ(burnVector, alpha);
				Tuple<double, double, double> velocityVector = transform(velocityVectorInPlane, referencePlane);
				Tuple<double, double, double> positionVector = transform(positionVectorInPlane, referencePlane);
				Tuple<double, double, double> inertialBurnVector = transform(burnVectorInPlane, referencePlane);
				
				screenLabels[20].Text = "Total ΔV: " + totalV;

				// BURN ANGLES
				Console.WriteLine(inertialBurnVector);

				double yaw = Helper.rad2deg(Math.Atan(inertialBurnVector.Item2 / inertialBurnVector.Item1));

				if (inertialBurnVector.Item1 > 0)
				{
					if (inertialBurnVector.Item2 > 0)
					{
						// Kepp yaw as is
					}
					else
					{
						yaw = 360 + yaw;
					}
				}
				else
				{
					yaw = 180 + yaw;
				}

				double pitch = Helper.rad2deg(Math.Asin(inertialBurnVector.Item3 / vectorMagnitude(inertialBurnVector)));
				
				screenLabels[21].Text = "BURN ANGLES: " + Helper.prtlen(Helper.toFixed(0, 2), 6) + "  " + Helper.prtlen(Helper.toFixed(pitch, 2), 6) + "  " + Helper.prtlen(Helper.toFixed(yaw, 2), 6);
				double yawOffset = 0;
				if (yaw >= 180)
				{
					yawOffset = 360 - yaw;
				}
				else
				{
					yawOffset = -yaw;
				}
				screenLabels[22].Text = "FDAI OFFSET: " + Helper.prtlen(Helper.toFixed(0, 2), 6) + "  " + Helper.prtlen(Helper.toFixed(-pitch, 2), 6) + "  " + Helper.prtlen(Helper.toFixed(yawOffset, 2), 6);

				screenOrbit.setBurnData(TAAUT, velocityVector,positionVector,my);

				screenOrbit.setOrbit(apopapsis, periapsis, sMa, sma, argOP, lOAN, radius, trueAnomaly);
				screenOrbit.Invalidate();
				
				// ZOOM
				screenLabels[23].Text = "ZOOM: " + Helper.toFixed(screenOrbit.getZoom(), 1);


				// ITERATE THROUGH ORBITS AND PATCHES
				String orbitsData = "";
				Orbit orbit = form.connection.SpaceCenter().ActiveVessel.Orbit;
				do
				{
					orbitsData += getOrbitsData(orbit);
				}
				while (orbit.NextOrbit != null);
				
				screenLabels[32].Text = orbitsData;
			}	
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenCharts.Add(null); // Initialize Charts
			for (int i = 0; i < 40; i++) screenLabels.Add(null); // Initialize Labels
			for (int i = 0; i < 10; i++) screenInputs.Add(null); // Initialize Inputs
			for (int i = 0; i < 10; i++) screenButtons.Add(null); // Initialize Buttons

			//screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenLabels[0] = Helper.CreateLabel(39, 0, 42, 1, "============ FLIGHT DYNAMICS =============");
			
			screenLabels[1] = Helper.CreateLabel(16, 1, 13); // Local Time
			screenLabels[2] = Helper.CreateLabel(0, 1, 14); // MET Time
			screenLabels[3] = Helper.CreateLabel(32, 1, 15); // UT Time
			
			// BURN DATA LABLES
			screenLabels[4] = Helper.CreateLabel(0, 2, 120, 1, "── CURRENT ORBITS DATA ──┬─────────── BURN DATA ───────────┬────────────────────");
			screenLabels[5] = Helper.CreateLabel(25, 3, 32, 1, "│                HRS   MIN   SEC");
			screenLabels[6] = Helper.CreateLabel(25, 4, 15, 1, "│          TIG:");
			screenLabels[7] = Helper.CreateLabel(25, 6, 32, 1, "│    [+]          [-]     ΔV M/S");
			screenLabels[8] = Helper.CreateLabel(25, 7, 25, 1, "│  PROGRADE / RETROGRADE:");
			screenLabels[9] = Helper.CreateLabel(25, 8, 25, 1, "│ RADIAL IN / RADIAL OUT:");
			screenLabels[10] = Helper.CreateLabel(25, 9, 25, 1, "│    NORMAL / ANTINORMAL:");
			
			screenLabels[11] = Helper.CreateLabel(25, 5, 1, 1, "│");
			screenLabels[12] = Helper.CreateLabel(59, 3, 1, 1, "│");
			screenLabels[13] = Helper.CreateLabel(59, 4, 1, 1, "│");
			screenLabels[14] = Helper.CreateLabel(59, 5, 1, 1, "│");
			screenLabels[15] = Helper.CreateLabel(59, 6, 1, 1, "│");
			screenLabels[16] = Helper.CreateLabel(59, 7, 1, 1, "│");
			screenLabels[17] = Helper.CreateLabel(59, 8, 1, 1, "│");
			screenLabels[18] = Helper.CreateLabel(59, 9, 1, 1, "│");
			screenLabels[19] = Helper.CreateLabel(25, 10, 35, 1, "├─────────────────────────────────┘");
			
			// BURN DATA INPUTS
			screenInputs[0] = Helper.CreateInput(41, 4, 5, 1); // HRS
			screenInputs[0].TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			screenInputs[0].Text = "000";
			screenInputs[1] = Helper.CreateInput(47, 4, 5, 1); // MIN
			screenInputs[1].TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			screenInputs[1].Text = "00";
			screenInputs[2] = Helper.CreateInput(53, 4, 5, 1); // SEC
			screenInputs[2].TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			screenInputs[2].Text = "00.0";
			
			screenInputs[3] = Helper.CreateInput(51, 7, 7, 1); // Z
			screenInputs[3].TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			screenInputs[3].Text = "0.0";
			screenInputs[4] = Helper.CreateInput(51, 8, 7, 1); // X
			screenInputs[4].TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			screenInputs[4].Text = "0.0";
			screenInputs[5] = Helper.CreateInput(51, 9, 7, 1); // Y
			screenInputs[5].TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			screenInputs[5].Text = "0.0";
			
			// DEBUG BURN VALUES
			screenLabels[20] = Helper.CreateLabel(26, 11, 35, 1, "");
			screenLabels[21] = Helper.CreateLabel(26, 12, 35, 1, "");
			screenLabels[22] = Helper.CreateLabel(26, 13, 35, 1, "");
			
			// ZOOM LABEL
			screenLabels[23] = Helper.CreateLabel(62, 1, 12, 1, "ZOOM: ");

			// ZOOM BUTTONS
			screenButtons[0] = Helper.CreateButton(74, 1, 5, 1, "-5");
			screenButtons[0].Font = form.buttonFont;
			screenButtons[0].Click += (sender, e) => changeZoom(sender, e, -5f);
			screenButtons[1] = Helper.CreateButton(80, 1, 5, 1, "-1");
			screenButtons[1].Font = form.buttonFont;
			screenButtons[1].Click += (sender, e) => changeZoom(sender, e, -1f);
			screenButtons[2] = Helper.CreateButton(86, 1, 5, 1, "-.1");
			screenButtons[2].Font = form.buttonFont;
			screenButtons[2].Click += (sender, e) => changeZoom(sender, e, -0.1f);
			screenButtons[3] = Helper.CreateButton(92, 1, 5, 1, "+.1");
			screenButtons[3].Font = form.buttonFont;
			screenButtons[3].Click += (sender, e) => changeZoom(sender, e, 0.1f);
			screenButtons[4] = Helper.CreateButton(98, 1, 5, 1, "+1");
			screenButtons[4].Font = form.buttonFont;
			screenButtons[4].Click += (sender, e) => changeZoom(sender, e, 1f);
			screenButtons[5] = Helper.CreateButton(104, 1, 5, 1, "+5");
			screenButtons[5].Font = form.buttonFont;
			screenButtons[5].Click += (sender, e) => changeZoom(sender, e, 5f);
			
			
			// ORBIT(S) INFO
			screenLabels[30] = Helper.CreateLabel(1, 4, 24, 1, "NEXT PERIAPSE: ");
			screenLabels[31] = Helper.CreateLabel(1, 5, 24, 1, "NEXT APOAPSE: ");
			
			screenLabels[32] = Helper.CreateLabel(1, 7, 24, 23, "");

			// OrbitGraph
			screenOrbit = Helper.CreateOrbit(62, 3, 58, 27);
			IList<CelestialBody> bodySatellites = body.Satellites;
			screenOrbit.setBody(body, bodyRadius, bodyName, bodySatellites);
		}

		private String getOrbitsData(Orbit orbit)
		{
			String output = "\n=== ORBIT ===";
			output += "\nBody: " + orbit.Body.Name;
			output += "\n Apo: " + Math.Round(orbit.ApoapsisAltitude).ToString();
			output += "\n Per: " + Math.Round(orbit.PeriapsisAltitude).ToString();
			output += "\n Inc: " + Helper.toFixed(orbit.Inclination,3);
			output += "\n\nTTSC: " + Helper.timeString(orbit.TimeToSOIChange,3);

			return output;
		}

		private void changeZoom(Object sender, EventArgs e, float change)
		{
			screenOrbit.setZoom(screenOrbit.getZoom() + change);
		}

		private Tuple<double, double, double> rotateVectorAroundZ(Tuple<double, double, double> v, double alpha)
		{
			double x = (v.Item1 * Math.Cos(alpha)) - (v.Item2 * Math.Sin(alpha));
			double y = (v.Item1 * Math.Sin(alpha)) + (v.Item2 * Math.Cos(alpha));
			double z = v.Item3;

			return new Tuple<double, double, double>(x, y, z);
		}

		private Tuple<Tuple<double, double, double>, Tuple<double, double, double>, Tuple<double, double, double>> getRotationMatrix(double alpha)
		{
			double a = Math.Cos(alpha);
			double b = -Math.Sin(alpha);
			double c = 0;

			double p = Math.Sin(alpha);
			double q = Math.Cos(alpha);
			double r = 0;

			double u = 0;
			double v = 0;
			double w = 1;

			Tuple<double, double, double> T1 = new Tuple<double, double, double>(a, b, c);
			Tuple<double, double, double> T2 = new Tuple<double, double, double>(p, q, r);
			Tuple<double, double, double> T3 = new Tuple<double, double, double>(u, v, w);
			
			return new Tuple<Tuple<double, double, double>, Tuple<double, double, double>, Tuple<double, double, double>>(T1, T2, T3);
		}
		
		private double angleBetweenVecotrs(Tuple<double, double, double> a, Tuple<double, double, double> b)
		{
			return Math.Acos(dotProduct(a,b)/(vectorMagnitude(a) * vectorMagnitude(b)));
		}

		private Tuple<double, double, double> getPositionVector(double semiMajorAxis, double trueAnomaly, double eccentricity)
		{
			double p = semiMajorAxis;
			double nu = trueAnomaly;
			double e = eccentricity;
			
			double X = (p * Math.Cos(nu)) / (1 + (e * Math.Cos(nu)));
			double Y = (p * Math.Sin(nu)) / (1 + (e * Math.Cos(nu)));
			double Z = 0;
			
			return new Tuple<double, double, double>(X, Y, Z);
		}
		
		private Tuple<double, double, double> getVelocityVector(double semiMajorAxis, double trueAnomaly, double eccentricity, double gravityMu)
		{
			double p = semiMajorAxis;
			double nu = trueAnomaly;
			double e = eccentricity;
			double mu = gravityMu;

			double X = -Math.Sqrt(mu/p) * Math.Sin(nu);
			double Y = Math.Sqrt(mu/p) * (e + Math.Cos(nu));
			double Z = 0;
			
			return new Tuple<double, double, double>(X, Y, Z);
		}

		private Tuple<Tuple<double, double, double>, Tuple<double, double, double>, Tuple<double, double, double>> getGeocentricReferenceFrame(double longitudeOfAscendingNode, double inclination, double argumentOfPeriapsis)
		{
			double Omega = longitudeOfAscendingNode;
			double omega = argumentOfPeriapsis;
			double i = inclination;

			double a = (Math.Cos(Omega) * Math.Cos(omega) - Math.Sin(Omega) * Math.Sin(omega) * Math.Cos(i));
			double b = (-Math.Cos(Omega) * Math.Sin(omega) - Math.Sin(Omega) * Math.Cos(omega) * Math.Cos(i));
			double c = (Math.Sin(Omega)*Math.Sin(i));
			
			double p = (Math.Sin(Omega) * Math.Cos(omega) + Math.Cos(Omega) * Math.Sin(omega) * Math.Cos(i));
			double q = (-Math.Sin(Omega) * Math.Sin(omega) + Math.Cos(Omega) * Math.Cos(omega) * Math.Cos(i));
			double r = (-Math.Cos(Omega)*Math.Sin(i));
			
			double u = (Math.Sin(omega) * Math.Sin(i));
			double v = (Math.Cos(omega) * Math.Sin(i));
			double w = (Math.Cos(i));

			Tuple<double, double, double> T1 = new Tuple<double, double, double>(a, b, c);
			Tuple<double, double, double> T2 = new Tuple<double, double, double>(p, q, r);
			Tuple<double, double, double> T3 = new Tuple<double, double, double>(u, v, w);

			return new Tuple<Tuple<double, double, double>, Tuple<double, double, double>, Tuple<double, double, double>>(T1, T2, T3);
		}

		private Tuple<double, double, double> crossProduct(Tuple<double, double, double> a, Tuple<double, double, double> b)
		{
			double s1 = (a.Item2 * b.Item3) - (a.Item3 * b.Item2);
			double s2 = (a.Item3 * b.Item1) - (a.Item1 * b.Item3);
			double s3 = (a.Item1 * b.Item2) - (a.Item2 * b.Item1);
			return new Tuple<double, double, double>(s1, s2, s3);
		}
		
		private double dotProduct(Tuple<double, double, double> a, Tuple<double, double, double> b)
		{
			return (a.Item1 * b.Item1) + (a.Item2 * b.Item2) + (a.Item3 * b.Item3);
		}
		
		private double vectorMagnitude(Tuple<double, double, double> v)
		{
			return Math.Sqrt(Math.Pow(v.Item1, 2) + Math.Pow(v.Item2, 2) + Math.Pow(v.Item3, 2));
		}

		private Tuple<double, double, double> vectorChangeMagnitude(Tuple<double, double, double> v, double change)
		{
			double oldMagnitude = vectorMagnitude(v);
			double scaler = (oldMagnitude + change) / oldMagnitude;

			return new Tuple<double, double, double>
			(
				v.Item1 * scaler,
				v.Item2 * scaler,
				v.Item3 * scaler
			);
		}

		private Tuple<double, double, double> vectorAddLeftMagnitude(Tuple<double, double, double> v, double change)
		{
			double alpha = Math.Acos(v.Item1 / vectorMagnitude(v));
			double beta = 90 - alpha;
			double deltaX = change * Math.Cos(beta);
			double deltaY = change * Math.Sin(beta);


			double scalerX = (v.Item1 + deltaX) / v.Item1;
			double scalerY = (v.Item2 + deltaY) / v.Item2;

			return new Tuple<double, double, double>
			(
				v.Item1 * scalerX,
				v.Item2 * scalerY,
				v.Item3 * 1
			);
		}
		
		private Tuple<double, double, double> vectorAddUpMagnitude(Tuple<double, double, double> v, double change)
		{
			return new Tuple<double, double, double>
			(
				v.Item1,
				v.Item2,
				v.Item3 + change
			);
		}
		
		private Tuple<double, double, double> transform (Tuple<double, double, double> v, Tuple<Tuple<double, double, double>, Tuple<double, double, double>, Tuple<double, double, double>> m)
		{
			double Item1 = (m.Item1.Item1 * v.Item1) + (m.Item1.Item2 * v.Item2) + (m.Item1.Item3 * v.Item3);
			double Item2 = (m.Item2.Item1 * v.Item1) + (m.Item2.Item2 * v.Item2) + (m.Item2.Item3 * v.Item3);
			double Item3 = (m.Item3.Item1 * v.Item1) + (m.Item3.Item2 * v.Item2) + (m.Item3.Item3 * v.Item3);

			return new Tuple<double, double, double>(Item1, Item2, Item3);
		}
	}
}
