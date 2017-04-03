using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace KSP_MOCR
{
	class helper
	{
		Form1 form;

		public enum Align { LEFT, RIGHT, CENTER };

		public helper(Form1 form)
		{
			this.form = form;
		}

		public TextBox CreateInput(int x, int y) { return CreateInput(x, y, 8, 1, HorizontalAlignment.Left); }
		public TextBox CreateInput(int x, int y, int w) { return CreateInput(x, y, w, 1, HorizontalAlignment.Left); }
		public TextBox CreateInput(int x, int y, int w, int h) { return CreateInput(x, y, w, h, HorizontalAlignment.Left); }
		public TextBox CreateInput(int x, int y, int w, int h, HorizontalAlignment align)
		{
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine) - 1;
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);

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
		public Label CreateLabel(int x, int y) { return CreateLabel(x, y, 8, 1, ""); }
		public Label CreateLabel(int x, int y, int w) { return CreateLabel(x, y, w, 1, ""); }
		public Label CreateLabel(int x, int y, int w, int h) { return CreateLabel(x, y, w, h, ""); }
		public Label CreateLabel(int x, int y, int w, int h, String t)
		{
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);


			Label label = new Label();
			label.Font = form.font;
			label.Location = new Point(left, top);
			label.Size = new Size(width, height);
			label.Text = t;
			label.Padding = new Padding(0);
			label.Margin = new Padding(0);
			label.FlatStyle = FlatStyle.System;
			label.UseCompatibleTextRendering = true;
			label.ForeColor = form.foreColor;
			form.Controls.Add(label);
			return label;
		}


		public Button CreateButton(double x, double y) { return CreateButton(x, y, 8, 1, ""); }
		public Button CreateButton(double x, double y, double w) { return CreateButton(x, y, w, 1, ""); }
		public Button CreateButton(double x, double y, double w, double h, String t)
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


		public Chart CreateChart(int x, int y, int w, int h) { return CreateChart(x, y, w, h, 0, -1, 0, -1); }
		public Chart CreateChart(int x, int y, int w, int h, int minX) { return CreateChart(x, y, w, h, minX, -1, 0, -1); }
		public Chart CreateChart(int x, int y, int w, int h, int minX, int maxX) { return CreateChart(x, y, w, h, minX, maxX, 0, -1); }
		public Chart CreateChart(int x, int y, int w, int h, int minX, int maxX, int minY) { return CreateChart(x, y, w, h, minX, maxX, minY, -1); }
		public Chart CreateChart(int x, int y, int w, int h, int minX, int maxX, int minY, int maxY)
		{
			int width = (int)Math.Ceiling((w * form.pxPrChar));
			int height = (int)Math.Ceiling(h * form.pxPrLine);
			int top = (int)(y * form.pxPrLine) + form.padding_top;
			int left = (int)((x * form.pxPrChar) + form.padding_left);

			Chart chart = new Chart();
			chart.Legends.Clear();
			chart.Series.Clear();
			chart.Size = new Size(width, height);
			chart.Location = new Point(left, top);
			chart.BackColor = form.BackColor;
			chart.Padding = new Padding(0);

			var chartArea = new ChartArea();
			chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
			chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
			chartArea.BackColor = form.BackColor;
			chartArea.Position = new ElementPosition(0, 0, 100, 100);

			chartArea.AxisX.LabelStyle.Font = new Font("Consolas", 8);
			chartArea.AxisX.LineColor = form.chartAxisColor;
			chartArea.AxisX.InterlacedColor = form.chartAxisColor;
			chartArea.AxisX.LabelStyle.ForeColor = form.foreColor;
			chartArea.AxisX.MajorGrid.LineColor = form.chartAxisColor;
			chartArea.AxisX.MinorGrid.LineColor = form.chartAxisColor;
			if (maxX != -1) chartArea.AxisX.Maximum = maxX;
			chartArea.AxisX.Minimum = minX;

			chartArea.AxisY.LabelStyle.Font = new Font("Consolas", 8);
			chartArea.AxisY.LineColor = form.chartAxisColor;
			chartArea.AxisY.InterlacedColor = form.chartAxisColor;
			chartArea.AxisY.LabelStyle.ForeColor = form.foreColor;
			chartArea.AxisY.MajorGrid.LineColor = form.chartAxisColor;
			chartArea.AxisY.MinorGrid.LineColor = form.chartAxisColor;
			if (maxY != -1) chartArea.AxisY.Maximum = maxY;
			chartArea.AxisY.Minimum = minY;

			chart.ChartAreas.Add(chartArea);

			chart.Invalidate();

			form.Controls.Add(chart);
			return chart;
		}


		public Indicator CreateIndicator(int x, int y, int w, int h, String t)
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

		public EngineIndicator CreateEngine(int x, int y, int w, int h, String t)
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


		public String prtlen(String s, int l) { return prtlen(s, l, Align.RIGHT); }
		public String prtlen(String s, int l, Align a)
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


		public String timeString(double t) { return timeString(t, 2); }
		public String timeString(double t, int h_len)
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


		public String toFixed(double? d, int p)
		{
			String r;
			if (d == null)
			{
				r = "";
			}
			else
			{
				double d2  = d.Value;
				String b = Math.Round(d2).ToString();
				r = Math.Round(d2, p).ToString();

				// Check that d isn't whole number, if so; add ','
				int index = r.IndexOf(",");
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
	}
}
