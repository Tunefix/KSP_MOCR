using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace KSP_MOCR
{
	/*
	 * I am using this class for labels to have full controll over where the text is
	 * rendered in the label-coordinate system. 
	 * */
	public class CustomLabel : Label
	{
		double charWidth = 9;
		double lineHeight = 19;
		double charOffset = -2.5;
		double lineOffset = 0;

		public CustomLabel()
		{
		}

		public void setCharWidth(double w){this.charWidth = w;}
		public void setlineHeight(double h) { this.lineHeight = h;}
		public void setcharOffset(double o) { this.charOffset = o; }
		public void setlineOffset(double o) { this.lineOffset = o; }

		protected override void OnPaint(PaintEventArgs e)
		{
			// Simple draw-line
			//e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), -2.5f, 0);

			// Draw each character on its own, to align everything
			float yPos = (float)lineOffset;
			double charsBack = 0;
			for (int i = 0; i < Text.Length; i++)
			{
				String letter = Text.Substring(i, 1);

				if (letter == "\n")
				{
					yPos += (float)lineHeight;
					charsBack = (i+1) * charWidth;
				}
				else
				{
					float xPos = (float)(charOffset + (charWidth * i) - charsBack);
					e.Graphics.DrawString(letter, Font, new SolidBrush(ForeColor), xPos, yPos);
				}
			}
		}
	}

	static class Helper
	{
		static Form1 form;

		public enum Align { LEFT, RIGHT, CENTER };

		static public void setForm(Form1 form)
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
		static public Label CreateLabel(double x, double y) { return CreateLabel(x, y, 8, 1, ""); }
		static public Label CreateLabel(double x, double y, double w) { return CreateLabel(x, y, w, 1, ""); }
		static public Label CreateLabel(double x, double y, double w, double h) { return CreateLabel(x, y, w, h, ""); }
		static public Label CreateLabel(double x, double y, double w, double h, String t)
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
			form.Controls.Add(label);
			return label;
		}


		static public Button CreateButton(double x, double y) { return CreateButton(x, y, 8, 1, ""); }
		static public Button CreateButton(double x, double y, double w) { return CreateButton(x, y, w, 1, ""); }
		static public Button CreateButton(double x, double y, double w, double h, String t)
		{
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);

			Button button = new Button();
			button.Location = new Point(left, top);
			button.Size = new Size(width, height);
			button.BackColor = Color.FromArgb(255, 32, 32, 32);
			button.FlatStyle = FlatStyle.Flat;
			button.Cursor = Cursors.Hand;
			button.FlatAppearance.BorderColor = Color.FromArgb(255, 96, 96, 96);
			button.FlatAppearance.BorderSize = 1;
			button.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 64, 64, 64);
			button.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 16, 16, 16);
			button.Font = form.font;
			button.Text = t;
			button.Padding = new Padding(0);
			button.UseCompatibleTextRendering = true;
			form.Controls.Add(button);
			return button;
		}


		static public Plot CreatePlot(int x, int y, int w, int h) { return CreatePlot(x, y, w, h, 0, -1, 0, -1); }
		static public Plot CreatePlot(int x, int y, int w, int h, int minX) { return CreatePlot(x, y, w, h, minX, -1, 0, -1); }
		static public Plot CreatePlot(int x, int y, int w, int h, int minX, int maxX) { return CreatePlot(x, y, w, h, minX, maxX, 0, -1); }
		static public Plot CreatePlot(int x, int y, int w, int h, int minX, int maxX, int minY) { return CreatePlot(x, y, w, h, minX, maxX, minY, -1); }
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


		static public Indicator CreateIndicator(int x, int y, int w, int h, String t)
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
			label.Padding = new Padding(0);
			label.Margin = new Padding(0);
			label.BorderStyle = BorderStyle.None;
			label.UseCompatibleTextRendering = true;
			label.TextAlign = ContentAlignment.MiddleCenter;
			label.ForeColor = Color.FromArgb(128, fColor.R, fColor.G, fColor.B);
			label.ImageAlign = ContentAlignment.MiddleCenter;
			label.BackColor = Color.FromArgb(255, 255, 0, 0);

			// Try to find appropriate background image
			if(form.indicatorImages[w][h] != null)
			{
				label.Image = form.indicatorImages[w][h];
			}
			else
			{
				label.Image = form.indicatorImage;
			}

			// Set the five deafult colors
			label.setStatusColors(0, Color.FromArgb(200, 0, 0, 0), Color.FromArgb(255, 32, 32, 32)); // Status 0 (Off/Gray)
			label.setStatusColors(1, Color.FromArgb(200, 0, 0, 0), Color.FromArgb(255, 32, 128, 32)); // Status 1 (ON/Green)
			label.setStatusColors(2, Color.FromArgb(200, 0, 0, 0), Color.FromArgb(255, 128, 32, 32)); // Status 2 (ON/Red)
			label.setStatusColors(3, Color.FromArgb(200, 0, 0, 0), Color.FromArgb(255, 32, 32, 128)); // Status 3 (ON/Blue)
			label.setStatusColors(4, Color.FromArgb(200, 0, 0, 0), Color.FromArgb(255, 128, 100, 32)); // Status 3 (ON/Amber)

			// Set the status to 0 (OFF)
			label.setStatus(0);

			form.Controls.Add(label);

			return label;
		}

		static public EngineIndicator CreateEngine(int x, int y, int w, int h, String t)
		{
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);

			Color fColor = form.foreColor;
			EngineIndicator label = new EngineIndicator();
			label.form = form;
			label.Font = form.font;
			label.Location = new Point(left, top);
			label.Size = new Size(width, height);
			label.Text = t;
			label.Padding = new Padding(0);
			label.Margin = new Padding(0);
			label.BorderStyle = BorderStyle.None;
			label.UseCompatibleTextRendering = true;
			label.TextAlign = ContentAlignment.MiddleCenter;
			label.ForeColor = Color.FromArgb(128, fColor.R, fColor.G, fColor.B);
			label.Image = form.engineOff;
			label.ImageAlign = ContentAlignment.MiddleCenter;

			form.Controls.Add(label);

			return label;
		}

		static public VerticalMeter CreateVMeter(int x, int y){return CreateVMeter(x, y, false);}
		static public VerticalMeter CreateVMeter(int x, int y, bool doubleMeter)
		{
			int w = 0;
			int h = 9;
			if (doubleMeter) { w = 10; } else { w = 6; }
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);

			Color fColor = form.foreColor;
			VerticalMeter label = new VerticalMeter(form.smallFont);
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
		
		
		static public SegDisp CreateSegDisp(double x, double y, int length, bool signed)
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
			SegDisp label = new SegDisp(length, signed);
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


		static public String timeString(double t) { return timeString(t, 2); }
		static public String timeString(double t, int h_len)
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

			sec = Math.Round(ts);
			sec_s = sec.ToString();
			while (sec_s.Length < 2) { sec_s = "0" + sec_s; }

			output = hrs_s + ":" + min_s + ":" + sec_s;

			return output;
		}


		static public String toFixed(double? d, int p)
		{
			NumberFormatInfo format = new NumberFormatInfo();
			format.NumberGroupSeparator = "";
			format.NumberDecimalDigits = 10;
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
					r += ",";
				}

				while (r.Length < b.Length + 1 + p) // The +1 is for the decimal sign
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
	}
}
