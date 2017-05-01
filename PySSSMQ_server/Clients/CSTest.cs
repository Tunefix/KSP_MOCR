/**
 *
 * This is a test application to test the PySSSMQ_client
 * Lars Andre Landås <landas@gmail.com>
 *
 */

using System;

public class CSTest {

	static void Main() {

		CSTestClient mainP = new CSTestClient();
		mainP.Start();

	}

}

public class CSTestClient {

	public CSTestClient() {

	}

	public void Start() {

		string key = "test_key";
		string data = "testdata for key";

		string key2 = "test_key2";
		string data2 = "testdata for key2";

		PySSSMQ_client client = new PySSSMQ_client();
		if(!client.Connect("127.0.0.1")) {
			Console.WriteLine("Could not connect to server");
			return;
		}

		Console.WriteLine("Connected to server");

		client.AttachReceiveEvent(this.ReceiveData);
		Console.WriteLine("this.Recieved is attached to PySSSMQ_client");

		Console.WriteLine("Pull " + key);
		client.Pull(key);
		System.Threading.Thread.Sleep(1000);
	
		Console.WriteLine("Seding to server: key = " + key + ", data = " + data);
		client.Send(key, data);

		System.Threading.Thread.Sleep(1000);
		Console.WriteLine("Seding to server: key = " + key2 + ", data = " + data2);
		client.Send(key2, data2);

		System.Threading.Thread.Sleep(1000);
		Console.WriteLine("Pull " + key);
		client.Pull(key);

		System.Threading.Thread.Sleep(1000);
	}

	public void ReceiveData(string key, string val) {
		Console.WriteLine("ReceivedData: key = " + key + ", data = " + val);
	}


}
