using System;
using KRPC.Client.Services.SpaceCenter;
using System.Collections.Generic;

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
			try
			{
				return stream.Get();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.GetType().ToString() + ": " + e.Message + "\n" + e.StackTrace);
				return 0;
			}
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
			try
			{
				return stream.Get();
			}
			catch(Exception e)
			{
				Console.WriteLine(e.GetType().ToString() + ":" + e.Message + "\n" + e.StackTrace);
				return 0f;
			}
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
			try
			{
				return stream.Get();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.GetType().ToString() + ":" + e.Message + "\n" + e.StackTrace);
				return 0;
			}
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
			try
			{
				return stream.Get();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.GetType().ToString() + ":" + e.Message + "\n" + e.StackTrace);
				return false;
			}
		}

		public override void Remove() { stream.Remove(); }
	}
	
	class stringStream : Kstream
	{
		KRPC.Client.Stream<string> stream { get; set; }

		public stringStream(KRPC.Client.Stream<string> s)
		{
			stream = s;
		}

		public override dynamic Get()
		{
			try
			{
				return stream.Get();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.GetType().ToString() + ":" + e.Message + "\n" + e.StackTrace);
				return "";
			}
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
			try
			{
				return stream.Get();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.GetType().ToString() + ":" + e.Message + "\n" + e.StackTrace);
				return new Tuple<double, double, double>(1,1,1);
			}
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
			try
			{
				return stream.Get();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.GetType().ToString() + ":" + e.Message + "\n" + e.StackTrace);
				return new Tuple<double, double, double, double>(1,1,1,1);
			}
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
			try
			{
				return stream.Get();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.GetType().ToString() + ":" + e.Message + "\n" + e.StackTrace);
				return SASMode.StabilityAssist;
			}
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
			try
			{
				return stream.Get();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.GetType().ToString() + ":" + e.Message + "\n" + e.StackTrace);
				return null;
			}
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
			try
			{
				return stream.Get();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.GetType().ToString() + ":" + e.Message + "\n" + e.StackTrace);
				return VesselType.Ship;
			}
		}

		public override void Remove() { stream.Remove(); }
	}

	class vesselPartsStream : Kstream
	{
		KRPC.Client.Stream<Parts> stream { get; set; }

		public vesselPartsStream(KRPC.Client.Stream<Parts> s)
		{
			stream = s;
		}

		public override dynamic Get()
		{
			try
			{
				return stream.Get();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.GetType().ToString() + ":" + e.Message + "\n" + e.StackTrace);
				return null;
			}
		}

		public override void Remove() { stream.Remove(); }
	}

	class referenceFrameStream : Kstream
	{
		KRPC.Client.Stream<ReferenceFrame> stream { get; set; }

		public referenceFrameStream(KRPC.Client.Stream<ReferenceFrame> s)
		{
			stream = s;
		}

		public override dynamic Get()
		{
			try
			{
				return stream.Get();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.GetType().ToString() + ":" + e.Message + "\n" + e.StackTrace);
				return null;
			}
		}

		public override void Remove() { stream.Remove(); }
	}

	class IListNodeStream : Kstream
	{
		KRPC.Client.Stream<IList<KRPC.Client.Services.SpaceCenter.Node>> stream { get; set; }

		public IListNodeStream(KRPC.Client.Stream<IList<KRPC.Client.Services.SpaceCenter.Node>> s)
		{
			stream = s;
		}

		public override dynamic Get()
		{
			try
			{
				return stream.Get();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.GetType().ToString() + ":" + e.Message + "\n" + e.StackTrace);
				return null;
			}
		}

		public override void Remove() { stream.Remove(); }
	}
	
}
