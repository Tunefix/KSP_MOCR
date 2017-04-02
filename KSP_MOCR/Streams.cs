using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRPC.Client;
using KRPC.Client.Services;
using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;

namespace KSP_MOCR
{
	public class Streams
	{
		private List<IStream> streams = new List<IStream>();
		private KRPC.Client.Connection connection;

		public void setConnection(KRPC.Client.Connection con)
		{
			this.connection = con;
		}

		public IStream getStream(Type function)
		{
			// See if function already has stream
			foreach(IStream st in streams)
			{
				if(st.GetType().Equals(function))
				{
					return st;
				}
			}

			// Else create new stream
			/*IStream stream = new function.GetType()();
			streams.Add(stream);*/
			return null;
		}
	}

	public interface IStream { }

	public class vesselStream : IStream
	{
		public KRPC.Client.Stream<KRPC.Client.Services.SpaceCenter.Vessel> getVesselStream(Connection con, KRPC.Client.Services.SpaceCenter.Service sc)
		{
			return con.AddStream(() => sc.ActiveVessel);
		}
	}
}
