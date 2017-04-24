using System;
using System.Collections.Generic;

namespace KSP_MOCR
{
	partial class FDAI
	{
		private Tuple<double,double,double>[] getDigitPoints(int digit, double xSize, double ySize, double zSize)
		{
			List<Tuple<double,double,double>> output = new List<Tuple<double,double,double>>();

			switch (digit)
			{
				case 0:
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.1f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.159f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.3f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.7f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.841f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.9f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.217f * xSize, 0.841f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.7f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.3f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.217f * xSize, 0.159f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.1f * ySize, zSize));
					break;
				case 1:
					output.Add(new Tuple<double,double,double>(0.25f * xSize, 0.4f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.75f * xSize, 0.1f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.75f * xSize, 0.9f * ySize, zSize));
					break;
					
				case 2:
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.3f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.217f * xSize, 0.159f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.1f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.159f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.3f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.9f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.9f * ySize, zSize));
					break;
					
				case 3:
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.3f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.217f * xSize, 0.159f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.1f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.159f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.3f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.441f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.5f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.559f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.7f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.841f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.9f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.217f * xSize, 0.841f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.7f * ySize, zSize));
					break;
					
				case 4:
					output.Add(new Tuple<double,double,double>(0.8f * xSize, 0.1f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.7f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.8f * xSize, 0.7f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.8f * xSize, 0.5f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.8f * xSize, 0.9f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.8f * xSize, 0.7f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.7f * ySize, zSize));
					break;
					
				case 5:
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.1f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.1f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.5f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.217f * xSize, 0.359f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.3f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.359f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.5f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.7f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.841f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.9f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.217f * xSize, 0.841f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.7f * ySize, zSize));
					break;
					
				case 6:
					output.Add(new Tuple<double,double,double>(0.8f * xSize, 0.1f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.7f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.217f * xSize, 0.841f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.9f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.841f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.7f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.787f * xSize, 0.559f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.5f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.217f * xSize, 0.559f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.7f * ySize, zSize));
					break;
					
				case 7:
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.3f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.1f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.1f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.3f * xSize, 0.9f * ySize, zSize));
					break;
				
				case 8:
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.1f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.159f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.3f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.441f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.5f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.217f * xSize, 0.559f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.7f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.217f * xSize, 0.841f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.9f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.841f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.7f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.559f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.5f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.217f * xSize, 0.441f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.3f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.217f * xSize, 0.159f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.1f * ySize, zSize));
					break;
				
				case 9:
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.3f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.441f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.5f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.217f * xSize, 0.441f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.1f * xSize, 0.3f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.217f * xSize, 0.159f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.5f * xSize, 0.1f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.783f * xSize, 0.159f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.9f * xSize, 0.3f * ySize, zSize));
					output.Add(new Tuple<double,double,double>(0.3f * xSize, 0.9f * ySize, zSize));
					break;
			}

			return output.ToArray();
		}
	}
}
