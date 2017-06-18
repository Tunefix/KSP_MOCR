<<<<<<< Updated upstream
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class PySSSMQ_client {

	//TODO: Auto-reconnect if connection is lost (if this is not handled by System.Net.Sockets)

	private bool is_connected;
	private Socket socket;
	private Action<string,string>[] receive_event = new Action<string,string>[0];
	private Thread receive_thread;
	private string last_connect_error = "";

	public PySSSMQ_client() {
		this.socket = null;
		this.is_connected = false;
	}

	~PySSSMQ_client() {
		
		if(this.socket != null && this.socket.Connected) {
			this.socket.Close();
		}

		try {
			this.receive_thread.Abort();
		} catch {
			
		}

	}

	public bool Connect(string addr, int port = 8778) {
		
		if( this.is_connected ) {
			throw new Exception("Connection allready established");
		}

		this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		IAsyncResult result = this.socket.BeginConnect(addr, port, null, null);

		bool success = result.AsyncWaitHandle.WaitOne(2000, true);

		if( !success ||  !socket.Connected  ) {
			this.socket.Close();
			return false;
		}

		this.socket.EndConnect(result);

		this.receive_thread = new Thread( new ThreadStart(this.ReceiveData) );
		this.receive_thread.IsBackground = true;
		this.receive_thread.Start();

		this.is_connected = true;

		return this.is_connected;
	}

	public bool IsConnected() {
		if(socket != null)
		{
			return this.socket.Connected;
		}
		else
		{
			return false;
		}
	}

	public string LastConnectionError() {
		return this.last_connect_error;
	}

	public void AttachReceiveEvent(Action<string,string> method) {
		int length = this.receive_event.Length;
		Action<string,string>[] tmp = new Action<string,string>[length + 1];
		
		for(int x=0; x<length; ++x) {
			tmp[x] = this.receive_event[x];
		}

		tmp[length] = method;
		this.receive_event = tmp;
	}

	public void Send(string key, string val) {

		if(key.Length > 10) {
			throw new Exception("Key cannont be more than 10 bytes");
		}

		if(val.Length > 256) {
			throw new Exception("Value cannot be more than 256 bytes");
		}

		string send_data = "|" + key.PadRight(10, ' ') + val.Length.ToString().PadLeft(4,'0') + val;
		this.socket.Send(Encoding.ASCII.GetBytes(send_data));
	}

	public void Pull(string key) {
		if(key.Length > 10) {
			throw new Exception("Key cannont be more than 10 bytes");
		}

		string send_data = "&" + key.PadRight(10, ' ') + "".Length.ToString().PadLeft(4,'0');
		this.socket.Send(Encoding.ASCII.GetBytes(send_data));
	}

	public void ReceiveData() {

		byte[] bytes = new byte[256];
		string data;
		string key;
		string val;
		int length;

		try {
			while(true) {
				bytes = new byte[256];
				this.socket.Receive(bytes);
				data = Encoding.ASCII.GetString(bytes);

				if(data.Substring(0,1) != "|") {

				} else {
					int offset = 0;

					while(true) {
						length = int.Parse(data.Substring(offset+11, 4));
						key = data.Substring(offset+1,10).Trim();
						val = data.Substring(offset+15, length);
						offset += length + 15;
						this.SendEvent(key, val);

						if((data.Length > offset && data[offset] == '|')) {

						} else {
							break;
						}
					}
				}
			}
		} catch {
		}
	}

	private void SendEvent(string key, string val) {

		int num = this.receive_event.Length;
		for(int x=0; x<num; ++x) {
			receive_event[x](key, val);
		}

	}
}
=======
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class PySSSMQ_client {

	//TODO: Auto-reconnect if connection is lost (if this is not handled by System.Net.Sockets)

	private bool is_connected;
	private Socket socket;
	private Action<string,string>[] receive_event = new Action<string,string>[0];
	private Thread receive_thread;
	private string last_connect_error = "";

	public PySSSMQ_client() {
		this.socket = null;
		this.is_connected = false;
	}

	~PySSSMQ_client() {
		
		if(this.socket != null && this.socket.Connected) {
			this.socket.Close();
		}

		try {
			this.receive_thread.Abort();
		} catch {
			
		}

	}

	public bool Connect(string addr, int port = 8778) {
		
		if( this.is_connected ) {
			throw new Exception("Connection allready established");
		}

		this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		IAsyncResult result = this.socket.BeginConnect(addr, port, null, null);

		bool success = result.AsyncWaitHandle.WaitOne(2000, true);

		if( !success ||  !socket.Connected  ) {
			this.socket.Close();
			return false;
		}

		this.socket.EndConnect(result);

		this.receive_thread = new Thread( new ThreadStart(this.ReceiveData) );
		this.receive_thread.IsBackground = true;
		this.receive_thread.Start();

		this.is_connected = true;

		return this.is_connected;
	}

	public bool IsConnected() {

		return this.socket.Connected;
	}

	public string LastConnectionError() {
		return this.last_connect_error;
	}

	public void AttachReceiveEvent(Action<string,string> method) {
		int length = this.receive_event.Length;
		Action<string,string>[] tmp = new Action<string,string>[length + 1];
		
		for(int x=0; x<length; ++x) {
			tmp[x] = this.receive_event[x];
		}

		tmp[length] = method;
		this.receive_event = tmp;
	}

	public void Send(string key, string val) {

		if(key.Length > 10) {
			throw new Exception("Key cannont be more than 10 bytes");
		}

		if(val.Length > 256) {
			throw new Exception("Value cannot be more than 256 bytes");
		}

		string send_data = "|" + key.PadRight(10, ' ') + val.Length.ToString().PadLeft(4,'0') + val;
		this.socket.Send(Encoding.ASCII.GetBytes(send_data));
	}

	public void Pull(string key) {
		if(key.Length > 10) {
			throw new Exception("Key cannont be more than 10 bytes");
		}

		string send_data = "&" + key.PadRight(10, ' ') + "".Length.ToString().PadLeft(4,'0');
		this.socket.Send(Encoding.ASCII.GetBytes(send_data));
	}

	public void ReceiveData() {

		byte[] bytes = new byte[256];
		string data;
		string key;
		string val;
		int length;

		try {
			while(true) {
				bytes = new byte[256];
				this.socket.Receive(bytes);
				data = Encoding.ASCII.GetString(bytes);

				if(data.Substring(0,1) != "|") {

				} else {
					int offset = 0;

					while(true) {
						length = int.Parse(data.Substring(offset+11, 4));
						key = data.Substring(offset+1,10).Trim();
						val = data.Substring(offset+15, length);
						offset += length + 15;
						this.SendEvent(key, val);

						if((data.Length > offset && data[offset] == '|')) {

						} else {
							Console.WriteLine("Break");
							break;
						}
					}
				}
			}
		} catch {
		}
	}

	private void SendEvent(string key, string val) {

		int num = this.receive_event.Length;
		for(int x=0; x<num; ++x) {
			receive_event[x](key, val);
		}

	}
}
>>>>>>> Stashed changes
