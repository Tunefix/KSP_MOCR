using System;
using System.Collections.Generic;

namespace KSP_MOCR
{
	public class DataStorage
	{
		private Dictionary<String, String> storage = new Dictionary<string, string>();
		private PySSSMQ_client client;
		
		public DataStorage(PySSSMQ_client client)
		{
			this.client = client;
		}

		// Used when you wish to update data in the dataStorage, and transmit to PySSSMQ-clients
		public void setData(String key, String value)
		{
			storeData(key, value);
			client.Send(key, value);
		}
		
		// Used when PySSSMQ-client receives data
		public void storeData(String key, String value)
		{
			Console.WriteLine("Storing Data: " + key + ", " + value);
			if (storage.ContainsKey(key))
			{
				storage[key] = value;
			}
			else
			{
				storage.Add(key, value);
			}
		}

		public String getData(String key)
		{
			Console.WriteLine("Getting Data: " + key);
			if (storage.ContainsKey(key))
			{
				Console.WriteLine("Returning Data: " + storage[key]);
				return storage[key];
			}
			else
			{
				return "";
			}
		}
	}
}
