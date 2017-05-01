using System;
namespace KSP_MOCR
{
	static class OrbitFunctions
	{
		static Form1 form;

		static public void setForm(Form1 form)
		{
			OrbitFunctions.form = form;
		}
		
		static public double getInclination(Tuple<double, double, double> angularMomentum)
		{
			Tuple<double, double, double> kVector = new Tuple<double, double, double>(0, 0, 1);
			return angleBetweenVecotrs(angularMomentum, kVector);
		}

		static public Tuple<double, double, double> getNVector(Tuple<double, double, double> angularMomentum)
		{
			return new Tuple<double, double, double>(angularMomentum.Item2 * -1, angularMomentum.Item1, 0);
		}

		static public double angleBetweenVecotrs(Tuple<double, double, double> a, Tuple<double, double, double> b)
		{
			return Math.Acos(dotProduct(a,b)/(vectorMagnitude(a) * vectorMagnitude(b)));
		}

		static public double getSemiMinorAxis(double semiMajorAxis, double eccentricity)
		{
			return semiMajorAxis * Math.Sqrt(1 - (eccentricity * eccentricity));
		}

		static public double getPeriapsis(double eccentricity, double semiMajorAxis)
		{
			return (1 - eccentricity) * semiMajorAxis;
		}

		static public double getApoapsis(double eccentricity, double semiMajorAxis)
		{
			return (1 + eccentricity) * semiMajorAxis;
		}

		static public double getLongitudeOfAscendingNode(Tuple<double, double, double> nVector)
		{
			Tuple<double, double, double> xVector = new Tuple<double, double, double>(1, 0, 0);
			double absoluteN = vectorAbsolute(nVector);
			double loan;

			if (nVector.Item2 >= 0)
			{
				//loan = Math.Acos(nVector.Item1 / absoluteN);
				loan = angleBetweenVecotrs(nVector, xVector);
			}
			else
			{
				//loan = (2 * Math.PI) - Math.Acos(nVector.Item1 / absoluteN);
				loan = (2 * Math.PI) - angleBetweenVecotrs(nVector, xVector);
			}
			return loan;
		}

		static public double getSemiMajorAxis(double radius, double velocity, double gravParam)
		{
			return 1 / ((2 / radius) - ((velocity * velocity) / gravParam));
		}

		static public double getEccentricity(double semiMajorAxis, Tuple<double, double, double> angularMomentum, double gravParam)
		{
			return Math.Sqrt(1 - ((gravParam * semiMajorAxis) / (squareVector(angularMomentum))));
		}

		static public double getEccentricity(Tuple<double, double, double> eccentricityVector)
		{
			return vectorMagnitude(eccentricityVector);
		}

		static public double getTrueAnomaly(Tuple<double, double, double> eccentricityVector, Tuple<double, double, double> positionVector)
		{
			double absoluteEV = vectorAbsolute(eccentricityVector);
			double absolutePV = vectorAbsolute(positionVector);
			double step1 = (eccentricityVector.Item1 * positionVector.Item1)
			+ (eccentricityVector.Item2 * positionVector.Item2)
			+ (eccentricityVector.Item3 * positionVector.Item3);
			double step2 = step1 / (absoluteEV * absolutePV);
			return Math.Acos(step2);
		}

		static public double getArgumentOfPeriapsis(Tuple<double, double, double> eccentricityVector, Tuple<double, double, double> nVector)
		{
			double tmp = Math.Acos(dotProduct(eccentricityVector, nVector) / (vectorMagnitude(eccentricityVector)*vectorMagnitude(nVector)));
			
			if (eccentricityVector.Item3 <= 0)
			{
				return tmp;
			}
			return (2 * Math.PI) - tmp;
		}

		static public Tuple<double, double, double> getAngularMomentum(Tuple<double, double, double> positionVector, Tuple<double, double, double> velocityVector)
		{
			return crossProduct(positionVector, velocityVector);
		}

		static public Tuple<double, double, double> getEccentricityVector(
			Tuple<double, double, double> veloctiyVector,
			Tuple<double, double, double> angularMomentum,
			Tuple<double, double, double> positionVector,
			double gravParam
			)
		{
			Tuple<double, double, double> step1 = crossProduct(veloctiyVector, angularMomentum);
			Tuple<double, double, double> step2 = vectorDivision(step1, gravParam);
			Tuple<double, double, double> step3 = vectorDivision(positionVector, vectorMagnitude(positionVector));
			Tuple<double, double, double> step4 = vectorSubtrackt(step2, step3);
			return step4;
		}

		static public Tuple<double, double, double> getEccentricityVector(
			Tuple<double, double, double> veloctiyVector,
			Tuple<double, double, double> positionVector,
			double burn_velocityAtTIG,
			double gravParam,
			double burn_radiusAtTIG
			)
		{
			double step1 = Math.Pow(burn_velocityAtTIG,2);
			double step2 = step1 / gravParam;
			double step3 = 1 / burn_radiusAtTIG;
			double step4 = step2 - step3;

			Tuple<double, double, double> step5 = vectorMultiply(positionVector, step4);

			double step6 = dotProduct(veloctiyVector, positionVector);
			double step7 = step6 / gravParam;
			
			Tuple<double, double, double> step8 = vectorMultiply(veloctiyVector, step7);

			Tuple<double, double, double> step9 = vectorAdd(step5, step8);

			return step9;
		}
		
		static public Tuple<double, double, double> crossProduct(Tuple<double, double, double> a, Tuple<double, double, double> b)
		{
			double s1 = (a.Item2 * b.Item3) - (a.Item3 * b.Item2);
			double s2 = (a.Item3 * b.Item1) - (a.Item1 * b.Item3);
			double s3 = (a.Item1 * b.Item2) - (a.Item2 * b.Item1);
			return new Tuple<double, double, double>(s1, s2, s3);
		}

		static public double dotProduct(Tuple<double, double, double> a, Tuple<double, double, double> b)
		{
			return (a.Item1 * b.Item1) + (a.Item2 * b.Item2) + (a.Item3 * b.Item3);
		}
		
		static public Tuple<double, double, double> vectorMultiply(Tuple<double, double, double> v, double s)
		{
			return new Tuple<double, double, double>(v.Item1 * s, v.Item2 * s, v.Item3 * s);
		}

		static public double vectorAbsolute(Tuple<double, double, double> v)
		{
			return Math.Sqrt(Math.Pow(v.Item1, 2) + Math.Pow(v.Item2, 2) + Math.Pow(v.Item3, 2));
		}

		static public double vectorMagnitude(Tuple<double, double, double> v)
		{
			return Math.Sqrt(Math.Pow(v.Item1, 2) + Math.Pow(v.Item2, 2) + Math.Pow(v.Item3, 2));
		}

		static public Tuple<double, double, double> vectorDivision(Tuple<double, double, double> v, double d)
		{
			double s1 = v.Item1 / d;
			double s2 = v.Item2 / d;
			double s3 = v.Item3 / d;
			return new Tuple<double, double, double>(s1, s2, s3);
		}

		static public Tuple<double, double, double> vectorAdd(Tuple<double, double, double> a, Tuple<double, double, double> b)
		{
			double s1 = a.Item1 + b.Item1;
			double s2 = a.Item2 + b.Item2;
			double s3 = a.Item3 + b.Item3;
			return new Tuple<double, double, double>(s1, s2, s3);
		}

		static public Tuple<double, double, double> vectorSubtrackt(Tuple<double, double, double> a, Tuple<double, double, double> b)
		{
			double s1 = a.Item1 - b.Item1;
			double s2 = a.Item2 - b.Item2;
			double s3 = a.Item3 - b.Item3;
			return new Tuple<double, double, double>(s1, s2, s3);
		}

		static public double squareVector(Tuple<double, double, double> v)
		{
			return (v.Item1 * v.Item1) + (v.Item2 * v.Item2) + (v.Item3 * v.Item3);
		}
	}
}
