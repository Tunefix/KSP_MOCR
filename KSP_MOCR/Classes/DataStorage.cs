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
		
		/// <summary>
		/// Used when you wish to update data in the dataStorage, and transmit to PySSSMQ-clients
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void setData(String key, String value)
		{
			storeData(key, value);
			if (client.IsConnected())
			{
				client.Send(key, value);
			}
		}
		
		/// <summary>
		/// Store data in local storage. This is called when PySSSMQ-client receives data.
		/// If in doubt, use setData();
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void storeData(String key, String value)
		{
			//Console.WriteLine("Storing Data: " + key + ", " + value);
			if (storage.ContainsKey(key))
			{
				storage[key] = value;
			}
			else
			{
				storage.Add(key, value);
			}
		}

		/// <summary>
		/// Get data from local storage
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public String getData(String key)
		{
			//Console.WriteLine("Getting Data: " + key);
			if (storage.ContainsKey(key))
			{
				//Console.WriteLine("Returning Data: " + storage[key]);
				return storage[key];
			}
			else
			{
				return "";
			}
		}

		/// <summary>
		/// Get a copy of the entire local storage
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, string> GetDictionary()
		{
			return new Dictionary<string, string>(storage);
		}
		
		/// <summary>
		/// Get all data from server and store it locally.
		/// Most usefull to sync up at start.
		/// </summary>
		public void Pull()
		{
			client.Pull("&");
		}
	}
}
