using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace KSP_MOCR
{
	public class SegDisp : Control
	{
		readonly int numbers = 1;
		readonly bool sign = true;
		String value = "";
		int precision = 0; // Number of decimals
		SignState signState = SignState.AUTO; // Force state of sign (+/-)
		bool pad = true;
		Align align = Align.RIGHT;

		public float pxPrDigitW = 0;
		public float pxPrDigitH = 0;

		PointF[] points;
		
		public enum SignState { NONE, PLUS, MINUS, AUTO }
		public enum Align{LEFT, RIGHT}

		readonly Brush digitBrush = new SolidBrush(Color.FromArgb(255, 0, 200, 0));
		readonly Brush digitDimBrush = new SolidBrush(Color.FromArgb(16, 240, 255, 230));
		readonly Pen borderPen = new Pen(Color.FromArgb(255, 32, 32, 32), 3f);

		readonly String name;
		
		public SegDisp(int numbers, bool signed, String name)
		{
			this.numbers = numbers;
			this.sign = signed;
			this.DoubleBuffered = true;
			this.name = name;
		}

		public void setValue(String value) { setValue(value, 0, SignState.AUTO, true, Align.RIGHT); }
		public void setValue(String value, int precision) { setValue(value, precision, SignState.AUTO, true, Align.RIGHT); }
		public void setValue(String value, int precision, SignState sign) { setValue(value, precision, sign, true, Align.RIGHT); }
		public void setValue(String value, int precision, SignState sign, bool pad, Align align)
		{
			this.value = value;
			this.precision = precision;
			this.signState = sign;
			this.pad = pad;
			this.align = align;
			//this.Invalidate(); // This gets done by a separate thread under pilot-screen
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			float charWidth;
			if (sign)
			{
				charWidth = (Width - 6) / (numbers + 1);
			}
			else
			{
				charWidth = (Width - 6) / (numbers);
			}
			float charHeight = (Height - 6);
			float marginTop = 3;
			float marginLeft = 3;
			
			RectangleF rect;
			
			// Black background
			Rectangle irect = new Rectangle(0, 0, Width, Height);
			g.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, 0, 0)), irect);

			if (value == "")
			{
				int start;
				int stop;

				if (sign)
				{
					// Blank the +/- Sign
					rect = new RectangleF(marginLeft, marginTop, charWidth, charHeight);
					drawSignBlank(g, rect);

					start = 1;
					stop = numbers + 1;
				}
				else
				{
					start = 0;
					stop = numbers;
				}
				
				for (int i = start; i < stop; i++)
				{
					rect = new RectangleF((i * charWidth) + marginLeft, marginTop, charWidth * 0.95f, charHeight);
					drawBlank(g, rect);
				}
			}
			else
			{
				int start;
				int stop;
				int indexOffset;

				if (sign)
				{
					// Print Sign
					rect = new RectangleF(marginLeft, marginTop, charWidth, charHeight);
					try
					{
						int number = int.Parse(value);
						if ((signState == SignState.AUTO && number < 0) || signState == SignState.MINUS)
						{
							drawMinus(g, rect);
						}
						else if ((signState == SignState.AUTO && number >= 0) || signState == SignState.PLUS)
						{
							drawPlus(g, rect);
						}
						else
						{
							drawSignBlank(g, rect);
						}
					}
					catch (Exception)
					{
						if (signState == SignState.MINUS)
						{
							drawMinus(g, rect);
						}
						else if (signState == SignState.PLUS)
						{
							drawPlus(g, rect);
						}
						else
						{
							drawSignBlank(g, rect);
						}
					}
					
					start = 1;
					stop = numbers + 1;
					indexOffset = -1;
				}
				else
				{
					start = 0;
					stop = numbers;
					indexOffset = 0;
				}


				// Pad/cut value to fit
				if (value.Length > numbers)
				{
					value = value.Substring(value.Length - numbers);
				}
				else
				{
					if (precision != 0 && pad) // Pad with 0's leftwards to decimal point (i.e. make [   . 0] to [  0.00]
					{
						while (value.Length < numbers)
						{
							if (value.Length <= precision)
							{
								value = "0" + value;
							}
							else
							{
								value = " " + value;
							}
						}
						
					}
					else
					{
						while (value.Length < numbers)
						{
							if (align == Align.LEFT)
							{
								value = value + " ";
							}
							else
							{
								value = " " + value;
							}
						}
					}
				}
				
				for (int i = start; i < stop; i++)
				{
					rect = new RectangleF((i * charWidth) + marginLeft, marginTop, charWidth * 0.95f, charHeight);
					
					int index = i + indexOffset;
					String digit = value.Substring(index, 1);
					switch (digit)
					{
						case "0":
							draw0(g, rect);
							break;
						case "1":
							draw1(g, rect);
							break;
						case "2":
							draw2(g, rect);
							break;
						case "3":
							draw3(g, rect);
							break;
						case "4":
							draw4(g, rect);
							break;
						case "5":
							draw5(g, rect);
							break;
						case "6":
							draw6(g, rect);
							break;
						case "7":
							draw7(g, rect);
							break;
						case "8":
							draw8(g, rect);
							break;
						case "9":
							draw9(g, rect);
							break;
						default:
							drawBlank(g, rect);
							break;
					}

					// Draw point at precision place
					if (index != numbers - 1) // No decimal sign at after last digit
					{
						if (numbers - 1 - index == precision)
						{
							drawPoint(g, rect, false);
						}
						else
						{
							drawPoint(g, rect, true);
						}
					}
				}
			}

			// Border Line
			g.DrawRectangle(borderPen, irect);
		}

		/**
		 * SEGMENT LAYOUT
		 *          0
		 *      --------
		 *   1 /     2 /
		 *    /  3    /
		 *    -------
		 * 4/     5 /
		 * /   6   /
		 * --------
		 *
		 */
		 
		private void drawBlank(Graphics g, RectangleF rect)
		{
			drawSeg0(g, rect, true);
			drawSeg1(g, rect, true);
			drawSeg2(g, rect, true);
			drawSeg3(g, rect, true);
			drawSeg4(g, rect, true);
			drawSeg5(g, rect, true);
			drawSeg6(g, rect, true);
		}
		
		private void draw0(Graphics g, RectangleF rect)
		{
			drawSeg0(g, rect, false);
			drawSeg1(g, rect, false);
			drawSeg2(g, rect, false);
			drawSeg3(g, rect, true);
			drawSeg4(g, rect, false);
			drawSeg5(g, rect, false);
			drawSeg6(g, rect, false);
		}
		
		private void draw1(Graphics g, RectangleF rect)
		{
			drawSeg0(g, rect, true);
			drawSeg1(g, rect, true);
			drawSeg2(g, rect, false);
			drawSeg3(g, rect, true);
			drawSeg4(g, rect, true);
			drawSeg5(g, rect, false);
			drawSeg6(g, rect, true);
		}
		
		private void draw2(Graphics g, RectangleF rect)
		{
			drawSeg0(g, rect, false);
			drawSeg1(g, rect, true);
			drawSeg2(g, rect, false);
			drawSeg3(g, rect, false);
			drawSeg4(g, rect, false);
			drawSeg5(g, rect, true);
			drawSeg6(g, rect, false);
		}
		
		private void draw3(Graphics g, RectangleF rect)
		{
			drawSeg0(g, rect, false);
			drawSeg1(g, rect, true);
			drawSeg2(g, rect, false);
			drawSeg3(g, rect, false);
			drawSeg4(g, rect, true);
			drawSeg5(g, rect, false);
			drawSeg6(g, rect, false);
		}
		
		private void draw4(Graphics g, RectangleF rect)
		{
			drawSeg0(g, rect, true);
			drawSeg1(g, rect, false);
			drawSeg2(g, rect, false);
			drawSeg3(g, rect, false);
			drawSeg4(g, rect, true);
			drawSeg5(g, rect, false);
			drawSeg6(g, rect, true);
		}
		
		private void draw5(Graphics g, RectangleF rect)
		{
			drawSeg0(g, rect, false);
			drawSeg1(g, rect, false);
			drawSeg2(g, rect, true);
			drawSeg3(g, rect, false);
			drawSeg4(g, rect, true);
			drawSeg5(g, rect, false);
			drawSeg6(g, rect, false);
		}
		
		private void draw6(Graphics g, RectangleF rect)
		{
			drawSeg0(g, rect, false);
			drawSeg1(g, rect, false);
			drawSeg2(g, rect, true);
			drawSeg3(g, rect, false);
			drawSeg4(g, rect, false);
			drawSeg5(g, rect, false);
			drawSeg6(g, rect, false);
		}
		
		private void draw7(Graphics g, RectangleF rect)
		{
			drawSeg0(g, rect, false);
			drawSeg1(g, rect, true);
			drawSeg2(g, rect, false);
			drawSeg3(g, rect, true);
			drawSeg4(g, rect, true);
			drawSeg5(g, rect, false);
			drawSeg6(g, rect, true);
		}

		private void draw8(Graphics g, RectangleF rect)
		{
			drawSeg0(g, rect, false);
			drawSeg1(g, rect, false);
			drawSeg2(g, rect, false);
			drawSeg3(g, rect, false);
			drawSeg4(g, rect, false);
			drawSeg5(g, rect, false);
			drawSeg6(g, rect, false);
		}
		
		private void draw9(Graphics g, RectangleF rect)
		{
			drawSeg0(g, rect, false);
			drawSeg1(g, rect, false);
			drawSeg2(g, rect, false);
			drawSeg3(g, rect, false);
			drawSeg4(g, rect, true);
			drawSeg5(g, rect, false);
			drawSeg6(g, rect, false);
		}

		private void drawSignBlank(Graphics g, RectangleF rect)
		{
			drawSignUpDown(g, rect, true);
			drawSignHorisontal(g, rect, true);
		}

		private void drawPlus(Graphics g, RectangleF rect)
		{
			drawSignUpDown(g, rect, false);
			drawSignHorisontal(g, rect, false);
		}
		
		private void drawMinus(Graphics g, RectangleF rect)
		{
			drawSignUpDown(g, rect, true);
			drawSignHorisontal(g, rect, false);
		}

		private void drawPoint(Graphics g, RectangleF rect, bool dim)
		{
			RectangleF prect = new RectangleF();
			prect.Location = new PointF(rect.Left + (rect.Width * 0.9f), rect.Top + (rect.Height * 0.9f));
			prect.Size = new SizeF(rect.Width * 0.1f, rect.Height * 0.1f);
			
			if (dim)
			{
				g.FillEllipse(digitDimBrush, prect);
			}
			else
			{
				g.FillEllipse(digitBrush, prect);
			}
		}

		private void drawSeg0(Graphics g, RectangleF rect, bool dim)
		{
			// Seg 0
			points = new PointF[6];
			points[0] = new PointF(rect.Left + (rect.Width * 0.375f), rect.Top + (0));
			points[1] = new PointF(rect.Left + (rect.Width * 0.875f), rect.Top + (0));
			points[2] = new PointF(rect.Left + (rect.Width * 0.9275f), rect.Top + (rect.Height * 0.04f));
			points[3] = new PointF(rect.Left + (rect.Width * 0.825f), rect.Top + (rect.Height * 0.1f));
			points[4] = new PointF(rect.Left + (rect.Width * 0.375f), rect.Top + (rect.Height * 0.1f));
			points[5] = new PointF(rect.Left + (rect.Width * 0.3f), rect.Top + (rect.Height * 0.04f));
			if (!dim) { g.FillPolygon(digitBrush, points); } else { g.FillPolygon(digitDimBrush, points); }
		}

		private void drawSeg1(Graphics g, RectangleF rect, bool dim)
		{
			// Seg 1
			points = new PointF[6];
			points[0] = new PointF(rect.Left + (rect.Width * 0.195f), rect.Top + (rect.Height * 0.11f));
			points[1] = new PointF(rect.Left + (rect.Width * 0.2725f), rect.Top + (rect.Height * 0.06f));
			points[2] = new PointF(rect.Left + (rect.Width * 0.335f), rect.Top + (rect.Height * 0.12f));
			points[3] = new PointF(rect.Left + (rect.Width * 0.25f), rect.Top + (rect.Height * 0.415f));
			points[4] = new PointF(rect.Left + (rect.Width * 0.145f), rect.Top + (rect.Height * 0.485f));
			points[5] = new PointF(rect.Left + (rect.Width * 0.105f), rect.Top + (rect.Height * 0.44f));

			if (!dim) { g.FillPolygon(digitBrush, points); } else { g.FillPolygon(digitDimBrush, points); }
		}

		private void drawSeg2(Graphics g, RectangleF rect, bool dim)
		{
			// Seg 2
			points = new PointF[6];
			points[0] = new PointF(rect.Left + (rect.Width * 0.96f), rect.Top + (rect.Height * 0.06f));
			points[1] = new PointF(rect.Left + (rect.Width * 1.0f), rect.Top + (rect.Height * 0.1f));
			points[2] = new PointF(rect.Left + (rect.Width * 0.91f), rect.Top + (rect.Height * 0.45f));
			points[3] = new PointF(rect.Left + (rect.Width * 0.845f), rect.Top + (rect.Height * 0.4875f));
			points[4] = new PointF(rect.Left + (rect.Width * 0.775f), rect.Top + (rect.Height * 0.425f));
			points[5] = new PointF(rect.Left + (rect.Width * 0.85f), rect.Top + (rect.Height * 0.125f));
			if (!dim) { g.FillPolygon(digitBrush, points); } else { g.FillPolygon(digitDimBrush, points); }
		}

		private void drawSeg3(Graphics g, RectangleF rect, bool dim)
		{
			// Seg 3
			points = new PointF[6];
			points[0] = new PointF(rect.Left + (rect.Width * 0.2575f), rect.Top + (rect.Height * 0.45f));
			points[1] = new PointF(rect.Left + (rect.Width * 0.75f), rect.Top + (rect.Height * 0.45f));
			points[2] = new PointF(rect.Left + (rect.Width * 0.8f), rect.Top + (rect.Height * 0.5f));
			points[3] = new PointF(rect.Left + (rect.Width * 0.725f), rect.Top + (rect.Height * 0.55f));
			points[4] = new PointF(rect.Left + (rect.Width * 0.25f), rect.Top + (rect.Height * 0.55f));
			points[5] = new PointF(rect.Left + (rect.Width * 0.1825f), rect.Top + (rect.Height * 0.5f));
			if (!dim) { g.FillPolygon(digitBrush, points); } else { g.FillPolygon(digitDimBrush, points); }
		}

		private void drawSeg4(Graphics g, RectangleF rect, bool dim)
		{
			// Seg 4
			points = new PointF[6];
			points[0] = new PointF(rect.Left + (rect.Width * 0.1425f), rect.Top + (rect.Height * 0.5125f));
			points[1] = new PointF(rect.Left + (rect.Width * 0.21f), rect.Top + (rect.Height * 0.575f));
			points[2] = new PointF(rect.Left + (rect.Width * 0.135f), rect.Top + (rect.Height * 0.885f));
			points[3] = new PointF(rect.Left + (rect.Width * 0.05f), rect.Top + (rect.Height * 0.94f));
			points[4] = new PointF(rect.Left + (rect.Width * 0.0f), rect.Top + (rect.Height * 0.9f));
			points[5] = new PointF(rect.Left + (rect.Width * 0.075f), rect.Top + (rect.Height * 0.56f));
			if (!dim) { g.FillPolygon(digitBrush, points); } else { g.FillPolygon(digitDimBrush, points); }
		}

		private void drawSeg5(Graphics g, RectangleF rect, bool dim)
		{
			// Seg 5
			points = new PointF[6];
			points[0] = new PointF(rect.Left + (rect.Width * 0.84f), rect.Top + (rect.Height * 0.5125f));
			points[1] = new PointF(rect.Left + (rect.Width * 0.89f), rect.Top + (rect.Height * 0.56f));
			points[2] = new PointF(rect.Left + (rect.Width * 0.8f), rect.Top + (rect.Height * 0.9f));
			points[3] = new PointF(rect.Left + (rect.Width * 0.76f), rect.Top + (rect.Height * 0.94f));
			points[4] = new PointF(rect.Left + (rect.Width * 0.675f), rect.Top + (rect.Height * 0.875f));
			points[5] = new PointF(rect.Left + (rect.Width * 0.75f), rect.Top + (rect.Height * 0.575f));
			if (!dim) { g.FillPolygon(digitBrush, points); } else { g.FillPolygon(digitDimBrush, points); }
		}

		private void drawSeg6(Graphics g, RectangleF rect, bool dim)
		{
			// Seg 6
			points = new PointF[6];
			points[0] = new PointF(rect.Left + (rect.Width * 0.175f), rect.Top + (rect.Height * 0.9f));
			points[1] = new PointF(rect.Left + (rect.Width * 0.65f), rect.Top + (rect.Height * 0.9f));
			points[2] = new PointF(rect.Left + (rect.Width * 0.725f), rect.Top + (rect.Height * 0.96f));
			points[3] = new PointF(rect.Left + (rect.Width * 0.675f), rect.Top + (rect.Height * 1.0f));
			points[4] = new PointF(rect.Left + (rect.Width * 0.125f), rect.Top + (rect.Height * 1.0f));
			points[5] = new PointF(rect.Left + (rect.Width * 0.075f), rect.Top + (rect.Height * 0.96f));
			if (!dim) { g.FillPolygon(digitBrush, points); } else { g.FillPolygon(digitDimBrush, points); }
		}
		
		private void drawSignUpDown(Graphics g, RectangleF rect, bool dim)
		{
			points = new PointF[4];
			points[0] = new PointF(rect.Left + (rect.Width * 0.45f), rect.Top + (rect.Height * 0.44f));
			points[1] = new PointF(rect.Left + (rect.Width * 0.45f), rect.Top + (rect.Height * 0.25f));
			points[2] = new PointF(rect.Left + (rect.Width * 0.55f), rect.Top + (rect.Height * 0.25f));
			points[3] = new PointF(rect.Left + (rect.Width * 0.55f), rect.Top + (rect.Height * 0.44f));
			if (!dim) { g.FillPolygon(digitBrush, points); } else { g.FillPolygon(digitDimBrush, points); }
			
			points = new PointF[4];
			points[0] = new PointF(rect.Left + (rect.Width * 0.45f), rect.Top + (rect.Height * 0.56f));
			points[1] = new PointF(rect.Left + (rect.Width * 0.55f), rect.Top + (rect.Height * 0.56f));
			points[2] = new PointF(rect.Left + (rect.Width * 0.55f), rect.Top + (rect.Height * 0.75f));
			points[3] = new PointF(rect.Left + (rect.Width * 0.45f), rect.Top + (rect.Height * 0.75f));
			if (!dim) { g.FillPolygon(digitBrush, points); } else { g.FillPolygon(digitDimBrush, points); }
		}
		
		private void drawSignHorisontal(Graphics g, RectangleF rect, bool dim)
		{
			points = new PointF[4];
			points[0] = new PointF(rect.Left + (rect.Width * 0.2f), rect.Top + (rect.Height * 0.45f));
			points[1] = new PointF(rect.Left + (rect.Width * 0.8f), rect.Top + (rect.Height * 0.45f));
			points[2] = new PointF(rect.Left + (rect.Width * 0.8f), rect.Top + (rect.Height * 0.55f));
			points[3] = new PointF(rect.Left + (rect.Width * 0.2f), rect.Top + (rect.Height * 0.55f));
			if (!dim) { g.FillPolygon(digitBrush, points); } else { g.FillPolygon(digitDimBrush, points); }
		}
	}
}
