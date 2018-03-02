using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_MOCR
{
	class Logger : TextWriter
	{
		public string output = "";

		public override void Write(char value)
		{
			//Do something, like write to a file or something
		}

		public override void Write(string value)
		{
			//Do something, like write to a file or something
		}

		public override void WriteLine(string value)
		{
			var standardOutput = new StreamWriter(Console.OpenStandardOutput());
			standardOutput.AutoFlush = true;
			Console.SetOut(standardOutput);
			Console.WriteLine(value);
			Console.SetOut(this);
			//output += "\r\n" + value;

			try
			{
				FileStream ostrm = new FileStream("./log.txt", FileMode.Append, FileAccess.Write);
				StreamWriter writer = new StreamWriter(ostrm);
				
				writer.WriteLine(value);

				writer.Close();
				ostrm.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine("Cannot open log.txt for writing");
				Console.WriteLine(e.Message);
				return;
			}
		}

		public override Encoding Encoding
		{
			get
			{
				return Encoding.ASCII;
			}
		}
	}
}
