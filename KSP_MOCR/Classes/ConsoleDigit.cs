using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace KSP_MOCR
{
	public class ConsoleDigit : Control
	{
		string[] digits;
		int curDig = 0;

		readonly static Color borderColor = Color.FromArgb(255, 16, 16, 16);
		readonly static Pen borderPen = new Pen(new SolidBrush(borderColor), 5f);

		readonly static Color backColor = Color.FromArgb(255, 32, 32, 32);
		readonly static Brush backBrush = new SolidBrush(backColor);

		readonly static Color digColor = Color.FromArgb(255, 200, 204, 194);
		readonly static Brush digBrush = new SolidBrush(digColor);

		public ConsoleDigit(string[] digs)
		{
			this.DoubleBuffered = true;
			digits = digs;
		}

		public void setDigID(int dig)
		{
			if (dig < digits.Length && dig >= 0)
			{
				curDig = dig;
			}
			this.Invalidate();
		}

		public void digInc()
		{
			curDig++;

			if(curDig >= digits.Length)
			{
				curDig = 0;
			}
			this.Invalidate();
		}

		public void digDec()
		{
			curDig--;

			if(curDig < 0)
			{
				curDig = digits.Length - 1;
			}
			this.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.InterpolationMode = InterpolationMode.HighQualityBilinear;
			g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

			drawBackground(g);
			drawDigits(g);
			drawBorder(g);
			
		}

		private void drawBackground(Graphics g)
		{
			g.FillRectangle(backBrush, 0, 0, Width, Height);
		}

		private void drawBorder(Graphics g)
		{
			g.DrawRectangle(borderPen, 2f, 2f, Width - 4f, Height - 4f);
		}

		private void drawDigits(Graphics g)
		{
			// DRAW CURRENT DIGIT IN CENTER
			PointF point = new PointF((Width / 2f) - 6.5f, (Height / 2f) - 9.5f);
			g.DrawString(getDig(curDig), Font, digBrush, point);

			// DRAW NEXT DIGIT ABOVE
			point = new PointF((Width / 2f) - 6.5f, (Height / 2f) - 26f);
			g.DrawString(getDig(curDig + 1), Font, digBrush, point);

			// DRAW PREW DIGIT BELOW
			point = new PointF((Width / 2f) - 6.5f, (Height / 2f) + 7f);
			g.DrawString(getDig(curDig - 1), Font, digBrush, point);
		}

		private string getDig(int id)
		{
			if(id >= 0 && id < digits.Length)
			{
				return digits[id];
			}
			else if(id < 0)
			{
				while(id < 0)
				{
					id += digits.Length;
				}

				return digits[id];
			}
			else if(id >= digits.Length)
			{
				while(id >= digits.Length)
				{
					id -= digits.Length;
				}

				return digits[id];
			}
			return "X";
		}
	}
}
