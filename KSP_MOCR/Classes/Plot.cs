﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KSP_MOCR
{
	public class Plot : Control
	{
		public Color gridColor { get; set; }
		public Color labelColor { get; set; }
		public int minX { get; set; }
		public int maxX { get; set; }
		public int minY { get; set; }
		public int maxY { get; set; }
		private bool autoX = false;
		private bool autoY = false;

		private int plotTop;
		private int plotRight;
		private int plotBottom;
		private int plotLeft;
		private int plotWidth;
		private int plotHeight;

		private int XAxisHeight = 20;
		private int YAxisWidth = 40;

		private double xScaler;
		private double yScaler;

		private Dictionary<int, double?>[] series;
		private bool multiAxis = false;
		double[] axisData;
		Type[] seriesType;

		private Pen axisPen;
		private Pen gridPen;
		private Brush labelBrush;
		PointF[] line = new PointF[2];
		Pen linePen;

		public enum Type { LINE, CROSS }

		private StringFormat stringFormat;

		readonly List<Color> chartLineColors = new List<Color>();

		public Plot()
		{
			chartLineColors.Add(Color.FromArgb(255, 204, 51, 0));
			chartLineColors.Add(Color.FromArgb(255, 0, 51, 204));
			chartLineColors.Add(Color.FromArgb(255, 0, 169, 51));
			chartLineColors.Add(Color.FromArgb(100, 251, 251, 251));
			chartLineColors.Add(Color.FromArgb(100, 251, 251, 251));

			// Turn on double buffering
			this.DoubleBuffered = true;
		}

		public void setSeriesColor(int id, Color c)
		{
			while (chartLineColors.Count <= id)
			{
				chartLineColors.Add(Color.FromArgb(255, 251, 251, 252));
			}
			chartLineColors[id] = c;
		}

		public void setData(List<Dictionary<int, double?>> data, List<Type> types, bool multipleYaxis)
		{
			series = data.ToArray();
			seriesType = types.ToArray();
			multiAxis = multipleYaxis;
			this.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (series != null)
			{
				axisPen = new Pen(gridColor, 2.0f);
				gridPen = new Pen(gridColor, 1.0f);
				labelBrush = new SolidBrush(labelColor);

				Graphics g = e.Graphics;
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

				if (maxX == -1 || autoX) { maxX = findMaxX(); autoX = true; }
				if (minX == -1 || autoY) { minX = findMinX(); autoY = true; }

				plotTop = Margin.Top;
				plotRight = Size.Width - Margin.Right;
				plotBottom = Size.Height - Margin.Bottom - XAxisHeight;

				if (!multiAxis)
				{
					if (maxY == -1 || autoX) { maxY = findMaxY(); autoX = true; }
					if (minY == -1 || autoY) { minY = findMinY(); autoY = true; }
					plotLeft = YAxisWidth;
					plotWidth = plotRight - plotLeft;
					plotHeight = plotBottom - plotTop;

					YAxisWidth = maxY.ToString().Length * 8;

					// Draw YAxis With Labels
					// Determine best grid-size
					axisData = getAxisData("Y");
					yScaler = axisData[3];

					// Draw YAxis With Labels
					g.DrawLine(axisPen, plotLeft, plotBottom, plotLeft, plotTop);

					for (int i = 0; i <= axisData[2]; i++)
					{
						int y = plotBottom - (int)Math.Round((axisData[1] * i));
						g.DrawLine(gridPen, plotLeft, y, plotRight, y);

						stringFormat = new StringFormat();
						stringFormat.Alignment = StringAlignment.Far;
						stringFormat.LineAlignment = StringAlignment.Center;
						g.DrawString(((i * axisData[0]) + minY).ToString(), Font, labelBrush, plotLeft, y, stringFormat);
					}
				}
				else
				{
					plotLeft = YAxisWidth * series.Length;
					plotWidth = plotRight - plotLeft;
					plotHeight = plotBottom - plotTop;
				}



				// Determine best grid-size
				axisData = getAxisData("X");
				xScaler = axisData[3];


				// Draw XAxis With Labels
				g.DrawLine(axisPen, plotLeft, plotBottom, plotRight, plotBottom);

				for (int i = 0; i <= axisData[2]; i++)
				{
					int x = (int)Math.Round(plotLeft + (axisData[1] * i));
					g.DrawLine(gridPen, x, plotBottom, x, plotTop);

					stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Near;
					g.DrawString(((i * axisData[0]) + minX).ToString(), Font, labelBrush, x, plotBottom, stringFormat);
				}


				int n = 0;
				foreach (Dictionary<int, double?> serie in series)
				{
					if (multiAxis)
					{
						if (maxY == -1 || autoX) { maxY = findMaxY(serie); autoX = true; }
						if (minY == -1 || autoY) { minY = findMinY(serie); autoX = true; }

						// Draw THIS YAxis with Labels
						// Determine best grid-size
						axisData = getAxisData("Y");
						yScaler = axisData[3];


						// Draw YAxis With Labels
						g.DrawLine(axisPen, plotLeft - (n * YAxisWidth), plotBottom, plotLeft - (n * YAxisWidth), plotTop);

						for (int i = 0; i <= axisData[2]; i++)
						{
							int y = plotBottom - (int)Math.Round((axisData[1] * i));
							g.DrawLine(gridPen, plotLeft, y, plotRight, y);

							stringFormat = new StringFormat();
							stringFormat.Alignment = StringAlignment.Far;
							stringFormat.LineAlignment = StringAlignment.Center;
							g.DrawString(((i * axisData[0]) + minY).ToString(), Font, labelBrush, plotLeft, y, stringFormat);
						}
					}

					if (seriesType[n] == Type.CROSS)
					{
						drawCross(g, n, serie);
					}
					else
					{
						drawLine(g, n, serie);
					}
					n++;
				}
			}
		}

		private void drawCross(Graphics g, int n, Dictionary<int, double?> serie)
		{
			/**
			 * DRAW CROSSES
			 */
			linePen = new Pen(chartLineColors[n], 2.0f);
			double? value;
			for (int i = 0; i < serie.Count; i++)
			{
				if (serie.ContainsKey(i))
				{
					value = serie[i];
					if (value != null)
					{
						int x = (int)Math.Round((i / xScaler) + plotLeft + (minX / xScaler));
						float y = (float)(plotBottom - (value / yScaler) + (minY / yScaler));
						line[0] = new PointF(x - 4, y - 4);
						line[1] = new PointF(x + 4, y + 4);
						g.DrawLines(linePen, line);
						line[0] = new PointF(x - 4, y + 4);
						line[1] = new PointF(x + 4, y - 4);
						g.DrawLines(linePen, line);
					}
				}
			}
		}

		private void drawLine(Graphics g, int n, Dictionary<int, double?> serie)
		{
			/*
			 * DRAW THE LINE
			 */
			linePen = new Pen(chartLineColors[n], 2.0f);
			bool started = false;

			double? value;
			for (int i = 0; i < this.maxX; i++)
			{
				if (serie.ContainsKey(i))
				{
					value = serie[i];

					if (value != null && !started && yScaler != 0)
					{
						started = true;
						int x = (int)Math.Round((i / xScaler) + plotLeft + (minX / xScaler));
						float y = (float)(plotBottom - (value / yScaler) + (minY / yScaler));
						line[0] = new PointF(x, y);
					}
					else if (value != null && started && yScaler != 0)
					{
						int x = (int)Math.Round((i / xScaler) + plotLeft + (minX / xScaler));
						float y = (float)(plotBottom - (value / yScaler) + (minY / yScaler));
						line[1] = new PointF(x, y);
						g.DrawLines(linePen, line);
						line[0] = line[1];
					}
				}
			}
		}


		private double[] getAxisData(String a)
		{
			double[] ret = new double[4];
			if (a == "X")
			{
				int split = maxX - minX;
				double xPrPx = split / (double)plotWidth;
				int xLabelMaxWidth = maxX.ToString().Length * 12;
				int maxLabels = (int)Math.Floor((double)(plotWidth / xLabelMaxWidth));
				double minStep = (split / (double)maxLabels);
				int minStepInt = (int)minStep;
				double gridStep = Math.Ceiling(minStep / Math.Pow(10, minStepInt.ToString().Length - 1)) * Math.Pow(10, minStepInt.ToString().Length - 1);
				int gridLines = (int)Math.Floor(split / gridStep);
				double gridStepPx = gridStep / xPrPx;

				ret[0] = gridStep;
				ret[1] = gridStepPx;
				ret[2] = gridLines;
				ret[3] = xPrPx;
			}
			else
			{
				int split = maxY - minY;
				double xPrPx = split / (double)plotHeight;
				int xLabelMaxWidth = 16;
				int maxLabels = (int)Math.Floor((double)(plotHeight / xLabelMaxWidth));
				double minStep = (split / (double)maxLabels);
				int minStepInt = (int)minStep;
				double gridStep = Math.Ceiling(minStep / Math.Pow(10, minStepInt.ToString().Length - 1)) * Math.Pow(10, minStepInt.ToString().Length - 1);
				int gridLines = (int)Math.Floor(split / gridStep);
				double gridStepPx = gridStep / xPrPx;

				ret[0] = gridStep;
				ret[1] = gridStepPx;
				ret[2] = gridLines;
				ret[3] = xPrPx;
			}
			return ret;
		}

		private int findMaxX()
		{
			int? tMax = null;
			lock(series)
			{
				foreach (Dictionary<int, double?> serie in series)
				{
					lock(serie)
					{
						foreach (KeyValuePair<int, double?> point in serie)
						{
							if (tMax == null)
							{
								tMax = point.Key;
							}
							else if (point.Key > tMax)
							{
								tMax = point.Key;
							}
						}
					}
				}
			}
			return tMax == null ? 1: (int)tMax;
		}

		private int findMinX()
		{
			int? tMin = null;
			lock(series)
			{
				foreach (Dictionary<int, double?> serie in series)
				{
					lock(serie)
					{
						foreach (KeyValuePair<int, double?> point in serie)
						{
							if (tMin == null)
							{
								tMin = point.Key;
							}
							else if (point.Key < tMin)
							{
								tMin = point.Key;
							}
						}
					}
				}
			}
			return tMin == null ? 0: (int)tMin;
		}

		private int findMaxY()
		{
			double? tMax = null;
			lock(series)
			{
				foreach (Dictionary<int, double?> serie in series)
				{
					lock(serie)
					{
						foreach (KeyValuePair<int, double?> point in serie)
						{
							if (tMax == null)
							{
								tMax = point.Value;
							}
							else if (point.Value > tMax)
							{
								tMax = point.Value;
							}
						}
					}
				}
			}

			if (tMax == null)
			{
				return 0;
			}
			return (int)Math.Ceiling((double)tMax);
		}

		private int findMinY()
		{
			double? tMin = null;
			lock(series)
			{
				foreach (Dictionary<int, double?> serie in series)
				{
					lock(serie)
					{
						foreach (KeyValuePair<int, double?> point in serie)
						{
							if (tMin == null)
							{
								tMin = point.Value;
							}
							else if (point.Value < tMin)
							{
								tMin = point.Value;
							}
						}
					}
				}
			}
			return tMin == null ? 0 : (int)Math.Floor((double)tMin);
		}

		private int findMaxY(Dictionary<int, double?> serie)
		{
			double? tMax = null;
			lock(serie)
			{
				foreach (KeyValuePair<int, double?> point in serie)
				{
					if (tMax == null)
					{
						tMax = point.Value;
					}
					else if (point.Value > tMax)
					{
						tMax = point.Value;
					}
				}
			}
			if (tMax != null)
			{
				return (int)Math.Ceiling((double)tMax);
			}
			else
			{
				return 1;
			}
		}

		private int findMinY(Dictionary<int, double?> serie)
		{
			double? tMin = null;
			lock(serie)
			{
				foreach (KeyValuePair<int, double?> point in serie)
				{
					if (tMin == null)
					{
						tMin = point.Value;
					}
					else if (point.Value < tMin)
					{
						tMin = point.Value;
					}
				}
			}
			return tMin == null ? 0 : (int)Math.Floor((double)tMin);
		}
	}
}
