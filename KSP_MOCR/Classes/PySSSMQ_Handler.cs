using System;
namespace KSP_MOCR
{
	public class PySSSMQ_Handler
	{
		public DataStorage storage { set; get; }

		public PySSSMQ_Handler()
		{
		}

		public void receive(string key, string val)
		{
			storage.storeData(key, val);
		}
	}
}
