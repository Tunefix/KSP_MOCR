using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace KSP_MOCR
{
	static class Helper
	{
		static Screen form;

		public enum Align { LEFT, RIGHT, CENTER };

		static Random gen = new Random(DateTime.Now.Millisecond);

		static public void setForm(Screen form)
		{
			Helper.form = form;
		}

		static public TextBox CreateInput(int x, int y) { return CreateInput(x, y, 8, 1, HorizontalAlignment.Left); }
		static public TextBox CreateInput(int x, int y, int w) { return CreateInput(x, y, w, 1, HorizontalAlignment.Left); }
		static public TextBox CreateInput(int x, int y, int w, int h) { return CreateInput(x, y, w, h, HorizontalAlignment.Left); }
		static public TextBox CreateInput(int x, int y, int w, int h, HorizontalAlignment align)
		{
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine) - 1;
			int top = (int)(Math.Ceiling(y * form.pxPrLine) + form.padding_top);
			int left = (int)(Math.Ceiling(x * form.pxPrChar) + form.padding_left);

			TextBox input = new TextBox();
			input.Location = new Point(left, top);
			input.Size = new Size(width, height);
			input.Font = form.font;
			input.ForeColor = form.foreColor;
			input.BorderStyle = BorderStyle.None;
			input.BackColor = Color.FromArgb(255, 32, 32, 32);
			input.AutoSize = false;
			input.TextAlign = align;
			form.Controls.Add(input);
			return input;
		}


		// <param name="x">int, Position from left in characters</param>
		// <param name="y">int, Position from top in lines</param>
		// <param name="w">int, Width of label in characters</param>
		// <param name="h">int, Height of label in lines</param>
		// <param name="t">String, Text of label</param>
		static public CustomLabel CreateLabel(double x, double y) { return CreateLabel(x, y, 8, 1, "", true); }
		static public CustomLabel CreateLabel(double x, double y, double w) { return CreateLabel(x, y, w, 1, "", true); }
		static public CustomLabel CreateLabel(double x, double y, double w, double h) { return CreateLabel(x, y, w, h, "", true); }
		static public CustomLabel CreateLabel(double x, double y, double w, double h, String t) { return CreateLabel(x, y, w, h, t, true); }
		static public CustomLabel CreateLabel(double x, double y, double w, double h, String t, bool bigText)
		{
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(Math.Ceiling(y * form.pxPrLine) + form.padding_top);
			int left = (int)(Math.Ceiling(x * form.pxPrChar) + form.padding_left);


			CustomLabel label = new CustomLabel();
			label.setCharWidth(form.pxPrChar);
			label.setlineHeight(form.pxPrLine);
			label.setcharOffset(form.charOffset);
			label.setlineOffset(form.lineOffset);
			label.Font = form.font;
			label.AutoSize = false;
			label.TextAlign = ContentAlignment.TopCenter;
			label.Location = new Point(left, top);
			label.Size = new Size(width, height);
			label.Text = t;
			label.Padding = new Padding(0);
			label.Margin = new Padding(0);
			label.FlatStyle = FlatStyle.Flat;
			label.BorderStyle = BorderStyle.None;
			label.ForeColor = form.foreColor;
			label.bigText = bigText;
			form.Controls.Add(label);
			return label;
		}


		static public MocrButton CreateButton(double x, double y) { return CreateButton(x, y, 8, 1, ""); }
		static public MocrButton CreateButton(double x, double y, double w) { return CreateButton(x, y, w, 1, ""); }
		static public MocrButton CreateButton(double x, double y, double w, double h, String t)
		{
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);

			MocrButton button = new MocrButton();
			button.Location = new Point(left, top);
			button.Size = new Size(width, height);
			//button.BackColor = Color.FromArgb(255, 32, 32, 32);
			button.Cursor = Cursors.Hand;
			//button.FlatAppearance.BorderColor = Color.FromArgb(255, 96, 96, 96);
			//button.FlatAppearance.BorderSize = 1;
			//button.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 64, 64, 64);
			//button.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 16, 16, 16);
			button.Font = form.font;
			button.Text = t;
			button.Padding = new Padding(0);
			form.Controls.Add(button);
			return button;
		}

		static public Screw CreateScrew(double x, double y)
		{
			int top = (int)(y * form.pxPrLine) + form.padding_top + 4;
			int left = (int)((x * form.pxPrChar) + form.padding_left) - 1;

			Screw screw = new Screw();
			screw.Location = new Point(left, top);
			screw.Size = new Size(29, 29);
			screw.angle = Helper.random() * 180;

			form.Controls.Add(screw);
			return screw;
		}


		static public Plot CreatePlot(int x, int y, int w, int h) { return CreatePlot(x, y, w, h, 0, -1, 0, -1); }
		static public Plot CreatePlot(int x, int y, int w, int h, int minX) { return CreatePlot(x, y, w, h, minX, -1, 0, -1); }
		static public Plot CreatePlot(int x, int y, int w, int h, int minX, int maxX) { return CreatePlot(x, y, w, h, minX, maxX, 0, -1); }
		static public Plot CreatePlot(int x, int y, int w, int h, int minX, int maxX, int minY) { return CreatePlot(x, y, w, h, minX, maxX, minY, -1); }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x">X-position in chars</param>
		/// <param name="y">Y-position in rows</param>
		/// <param name="w">Width in chars</param>
		/// <param name="h">Height in rows</param>
		/// <param name="minX">Minimum x-value (-1 for auto)</param>
		/// <param name="maxX">Maximum x-value (-1 for auto)</param>
		/// <param name="minY">Minimum y-value (-1 for auto)</param>
		/// <param name="maxY">Maximum y-value (-1 for auto)</param>
		/// <returns></returns>
		static public Plot CreatePlot(int x, int y, int w, int h, int minX, int maxX, int minY, int maxY)
		{
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);

			Plot plot = new Plot();
			plot.Size = new Size(width, height);
			plot.Location = new Point(left, top);
			plot.BackColor = form.BackColor;
			plot.Padding = new Padding(0);
			plot.Margin = new Padding(12);
			plot.Font = form.buttonFont;
			plot.gridColor = form.chartAxisColor;
			plot.labelColor = form.foreColor;
			plot.minX = minX;
			plot.maxX = maxX;
			plot.minY = minY;
			plot.maxY = maxY;

			plot.Invalidate();

			form.Controls.Add(plot);

			return plot;
		}


		static public Chart CreateChart(int x, int y, int w, int h) { return CreateChart(x, y, w, h, 0, -1, 0, -1); }
		static public Chart CreateChart(int x, int y, int w, int h, int minX) { return CreateChart(x, y, w, h, minX, -1, 0, -1); }
		static public Chart CreateChart(int x, int y, int w, int h, int minX, int maxX) { return CreateChart(x, y, w, h, minX, maxX, 0, -1); }
		static public Chart CreateChart(int x, int y, int w, int h, int minX, int maxX, int minY) { return CreateChart(x, y, w, h, minX, maxX, minY, -1); }
		static public Chart CreateChart(int x, int y, int w, int h, int minX, int maxX, int minY, int maxY)
		{
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);

			Chart chart = new Chart();
			//chart.Legends.Clear();
			//chart.Series.Clear();
			chart.Size = new Size(width, height);
			chart.Location = new Point(left, top);
			chart.BackColor = form.BackColor;
			chart.Padding = new Padding(0);

			var chartArea = new ChartArea();
			chartArea.AxisX = new Axis();
			chartArea.AxisY = new Axis();
			chartArea.AxisX.MajorGrid = new Grid();
			chartArea.AxisY.MajorGrid = new Grid();
			chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
			chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
			chartArea.BackColor = form.BackColor;
			chartArea.Position = new ElementPosition(0, 0, 100, 100);

			chartArea.AxisX.LabelStyle = new LabelStyle();
			chartArea.AxisX.LabelStyle.Font = form.buttonFont;
			chartArea.AxisX.LineColor = form.chartAxisColor;
			chartArea.AxisX.InterlacedColor = form.chartAxisColor;
			chartArea.AxisX.LabelStyle.ForeColor = form.foreColor;
			chartArea.AxisX.MajorGrid.LineColor = form.chartAxisColor;

			chartArea.AxisX.MinorGrid = new Grid();
			chartArea.AxisX.MinorGrid.LineColor = form.chartAxisColor;
			if (maxX != -1) chartArea.AxisX.Maximum = maxX;
			chartArea.AxisX.Minimum = minX;

			chartArea.AxisY.LabelStyle = new LabelStyle();
			chartArea.AxisY.LabelStyle.Font = form.buttonFont;
			chartArea.AxisY.LineColor = form.chartAxisColor;
			chartArea.AxisY.InterlacedColor = form.chartAxisColor;
			chartArea.AxisY.LabelStyle.ForeColor = form.foreColor;
			chartArea.AxisY.MajorGrid.LineColor = form.chartAxisColor;

			chartArea.AxisY.MinorGrid = new Grid();
			chartArea.AxisY.MinorGrid.LineColor = form.chartAxisColor;
			if (maxY != -1) chartArea.AxisY.Maximum = maxY;
			chartArea.AxisY.Minimum = minY;

			chart.ChartAreas.Add(chartArea);

			chart.Invalidate();

			form.Controls.Add(chart);
			return chart;
		}


		static public Indicator CreateIndicator(double x, double y, int w, int h, String t)
		{
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);

			Color fColor = form.foreColor;

			Indicator label = new Indicator();
			label.Font = form.font;
			label.Location = new Point(left, top);
			label.Size = new Size(width, height);
			label.Text = t;

			// Set the status to OFF
			label.setStatus(Indicator.status.OFF);

			form.Controls.Add(label);

			return label;
		}

		static public MocrDropdown CreateDropdown(double x, double y, int w, int h)
		{
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);

			MocrDropdown dropdown = new MocrDropdown();
			dropdown.Font = form.font;
			dropdown.Location = new Point(left, top);
			dropdown.Size = new Size(width, height);
			dropdown.FlatStyle = FlatStyle.Flat;
			dropdown.Margin = new Padding(0);

			form.Controls.Add(dropdown);

			return dropdown;
		}

		static public EngineIndicator CreateEngine(int x, int y, int w, int h, String t)
		{
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);

			EngineIndicator label = new EngineIndicator();
			label.Font = form.font;
			label.Location = new Point(left, top);
			label.Size = new Size(width, height);
			label.Text = t;

			form.Controls.Add(label);

			return label;
		}

		static public VerticalMeter CreateVMeter(int x, int y){return CreateVMeter(x, y, false, 0);}
		static public VerticalMeter CreateVMeter(int x, int y, bool doubleMeter){return CreateVMeter(x, y, doubleMeter, 0);}
		static public VerticalMeter CreateVMeter(int x, int y, bool doubleMeter, int scaleType)
		{
			int w = 0;
			int h = 9;
			if (doubleMeter) { w = 10; } else { w = 6; }
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);

			Color fColor = form.foreColor;
			VerticalMeter label = new VerticalMeter(form.smallFont,scaleType);
			label.Font = form.font;
			label.doubleMeter = doubleMeter;
			label.Location = new Point(left, top);
			label.Size = new Size(width, height);
			label.Padding = new Padding(0);
			label.Margin = new Padding(0);
			label.ForeColor = Color.FromArgb(128, fColor.R, fColor.G, fColor.B);

			form.Controls.Add(label);

			return label;
		}
		
		static public Map CreateMap(double x, double y, int w, int h)
		{
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);

			Map label = new Map();
			label.Location = new Point(left, top);
			label.Size = new Size(width, height);

			form.Controls.Add(label);

			return label;
		}
		
		static public SegDisp CreateSegDisp(double x, double y, int length, bool signed, String name)
		{
			int w;
			if (signed)
			{
				 w = (length + 1) * 3;
			}
			else
			{
				w = length * 3;
			}
			int h = 2;
			
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);

			Color fColor = form.foreColor;
			SegDisp label = new SegDisp(length, signed, name);
			label.Font = form.font;
			label.Location = new Point(left, top);
			label.Size = new Size(width, height);
			label.Padding = new Padding(0);
			label.Margin = new Padding(0);
			label.ForeColor = Color.FromArgb(128, fColor.R, fColor.G, fColor.B);
			label.pxPrDigitW = (float)(form.pxPrChar * 3);
			label.pxPrDigitH = (float)(form.pxPrLine * 2);

			form.Controls.Add(label);

			return label;
		}

		static public OrbitGraph CreateOrbit(int x, int y, int w, int h)
		{
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);
			
			OrbitGraph orbit = new OrbitGraph(form.buttonFont);
			orbit.Location = new Point(left, top);
			orbit.Size = new Size(width, height);

			form.Controls.Add(orbit);

			return orbit;
		}


		static public String prtlen(String s, int l) { return prtlen(s, l, Align.RIGHT); }
		static public String prtlen(String s, int l, Align a)
		{
			if (s.Length < l)
			{
				while (s.Length < l)
				{
					switch (a)
					{
						case Align.RIGHT:
							s = " " + s;
							break;
						case Align.CENTER:
							s = " " + s + " ";
							if (s.Length > l)
							{
								s.Substring(0, s.Length - 1);
							}
							break;
						case Align.LEFT:
						default:
							s += " ";
							break;
					}
				}
			}
			else
			{
				s = s.Substring(0, l);
			}
			return s;
		}


		static public String timeString(double t) { return timeString(t, true, 2); }
		static public String timeString(double t, int h_len) { return timeString(t, true, h_len); }
		static public String timeString(double t, bool show_hrs) { return timeString(t, show_hrs, 2); }
		static public String timeString(double t, bool show_hrs, int h_len)
		{
			String output = "";
			double hrs;
			double min;
			double sec;
			double ts = t; // tmp sec
			String hrs_s;
			String min_s;
			String sec_s;

			hrs = Math.Floor(ts / (60 * 60));
			ts = ts - (hrs * (60 * 60));
			hrs_s = hrs.ToString();
			while (hrs_s.Length < h_len) { hrs_s = "0" + hrs_s; }

			min = Math.Floor(ts / 60);
			ts = ts - (min * 60);
			min_s = min.ToString();
			while (min_s.Length < 2) { min_s = "0" + min_s; }

			sec = Math.Floor(ts);
			sec_s = sec.ToString();
			while (sec_s.Length < 2) { sec_s = "0" + sec_s; }

			if (show_hrs)
			{
				output = hrs_s + ":" + min_s + ":" + sec_s;
			}
			else
			{
				min = min + (hrs * 60);
				min_s = min.ToString();
				while (min_s.Length < 2) { min_s = "0" + min_s; }

				ts = ts - sec;
				ts = Math.Round(ts * 100f);
				sec_s = sec_s + "." + ts.ToString();
				
				output = min_s + ":" + sec_s;
			}

			return output;
		}


		static public String toFixed(double? d, int p)
		{
			NumberFormatInfo format = new NumberFormatInfo();
			format.NumberGroupSeparator = "";
			format.NumberDecimalDigits = p;
			format.NumberDecimalSeparator = ".";
			
			String r;
			if (d == null)
			{
				r = "";
			}
			else
			{
				double d2  = d.Value;
				String b = Math.Round(d2).ToString(format);
				r = Math.Round(d2, p).ToString(format);

				// Check that d isn't whole number, if so; add '.'
				int index = r.IndexOf(".");
				if (index == -1)
				{
					r += ".";
				}

				int extraSigns = 1; // The decimal sign
				//if (d2 < 0) { extraSigns++; } // The minus sign

				while (r.Length < b.Length + extraSigns + p)
				{
					r += "0";
				}
			}
			return r;
		}

		static public double rad2deg(double rad)
		{
			return rad * (180 / Math.PI);
		}

		static public double deg2rad(double deg)
		{
			return deg * (Math.PI / 180);
		}

		static public List<KeyValuePair<double, double?>> limit(List<KeyValuePair<double, double?>> data, int count)
		{
			List<KeyValuePair<double, double?>> output = new List<KeyValuePair<double, double?>>();
			if (data[count + 1].Value != null)
			{
				// Find last value key
				int index = 599;
				while (index > count)
				{
					if (data[index].Value != null)
					{
						break;
					}
					index--;
				}

				for (int i = (index - count); i < index; i++)
				{
					if (data[i].Value != null)
					{
						output.Add(new KeyValuePair<double, double?>(i, data[i].Value));
					}
					else
					{
						output.Add(new KeyValuePair<double, double?>(i, null));
					}
				}
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					if (data[i].Value != null)
					{
						output.Add(new KeyValuePair<double, double?>(i, data[i].Value));
					}
					else
					{
						output.Add(new KeyValuePair<double, double?>(i, null));
					}
				}
			}

			return output;
		}

		static public string int2str(int i, int length, String pad)
		{
			String str = i.ToString();
			String pre = "";
			
			if (i < 0)
			{
				str = str.Substring(1);
				pre = "-";
			}

			while (str.Length < length)
			{
				str = pad + str;
			}

			str = pre + str;

			return str;
		}

		/// <summary>
		/// Returns a pseudorandom value between 0 and 1
		/// </summary>
		/// <returns></returns>
		public static float random()
		{
			return gen.Next(0, 10000) / 10000f;
		}
	}
}
