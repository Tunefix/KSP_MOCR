using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace KSP_MOCR
{
	public class FDAILine
	{
		public Tuple<double, double, double>[] points { get; set;}
		public Pen pen { get; set; }
		
		public FDAILine(int numberOfPoints)
		{
			points = new Tuple<double, double, double>[numberOfPoints];
		}

		public FDAILine(FDAILine original)
		{
			points = original.points;
			pen = original.pen;
		}
	}
	
	public class FDAIPolygon
	{
		public Tuple<double, double, double>[] points { get; set;}
		public Brush brush { get; set; }
		
		public FDAIPolygon(int numberOfPoints)
		{
			points = new Tuple<double, double, double>[numberOfPoints];
		}
	}
}
