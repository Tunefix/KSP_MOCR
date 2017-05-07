using System;
using KRPC.Client.Services.SpaceCenter;

namespace KSP_MOCR
{
	abstract class Kstream
	{
		abstract public dynamic Get();
		abstract public void Remove();
	}

	class doubleStream : Kstream
	{
		KRPC.Client.Stream<double> stream { get; set; }

		public doubleStream(KRPC.Client.Stream<double> s)
		{
			stream = s;
		}

		public override dynamic Get()
		{
			return stream.Get();
		}

		public override void Remove() { stream.Remove(); }
	}

	class floatStream : Kstream
	{
		KRPC.Client.Stream<float> stream { get; set; }

		public floatStream(KRPC.Client.Stream<float> s)
		{
			stream = s;
		}

		public override dynamic Get()
		{
			return stream.Get();
		}

		public override void Remove() { stream.Remove(); }
	}

	class intStream : Kstream
	{
		KRPC.Client.Stream<int> stream { get; set; }

		public intStream(KRPC.Client.Stream<int> s)
		{
			stream = s;
		}

		public override dynamic Get()
		{
			return stream.Get();
		}

		public override void Remove() { stream.Remove(); }
	}

	class boolStream : Kstream
	{
		KRPC.Client.Stream<bool> stream { get; set; }

		public boolStream(KRPC.Client.Stream<bool> s)
		{
			stream = s;
		}

		public override dynamic Get()
		{
			return stream.Get();
		}

		public override void Remove() { stream.Remove(); }
	}

	class tuple3Stream : Kstream
	{
		KRPC.Client.Stream<Tuple<double, double, double>> stream { get; set; }

		public tuple3Stream(KRPC.Client.Stream<Tuple<double, double, double>> s)
		{
			stream = s;
		}

		public override dynamic Get()
		{
			return stream.Get();
		}

		public override void Remove() { stream.Remove(); }
	}

	class tuple4Stream : Kstream
	{
		KRPC.Client.Stream<Tuple<double, double, double, double>> stream { get; set; }

		public tuple4Stream(KRPC.Client.Stream<Tuple<double, double, double, double>> s)
		{
			stream = s;
		}

		public override dynamic Get()
		{
			return stream.Get();
		}

		public override void Remove() { stream.Remove(); }
	}

	class sasModeStream : Kstream
	{
		KRPC.Client.Stream<SASMode> stream { get; set; }

		public sasModeStream(KRPC.Client.Stream<SASMode> s)
		{
			stream = s;
		}

		public override dynamic Get()
		{
			return stream.Get();
		}

		public override void Remove() { stream.Remove(); }
	}

	class celestialBodyStream : Kstream
	{
		KRPC.Client.Stream<CelestialBody> stream { get; set; }

		public celestialBodyStream(KRPC.Client.Stream<CelestialBody> s)
		{
			stream = s;
		}

		public override dynamic Get()
		{
			return stream.Get();
		}

		public override void Remove() { stream.Remove(); }
	}
	
	class vesselTypeStream : Kstream
	{
		KRPC.Client.Stream<VesselType> stream { get; set; }

		public vesselTypeStream(KRPC.Client.Stream<VesselType> s)
		{
			stream = s;
		}

		public override dynamic Get()
		{
			return stream.Get();
		}

		public override void Remove() { stream.Remove(); }
	}
}
