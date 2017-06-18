using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KSP_MOCR
{
	public class Indicator : Control
	{
		readonly Dictionary<status, Color> FColor = new Dictionary<status, Color>()
		{
			{status.OFF, Color.FromArgb(200, 0, 0, 0)},
			{status.GREEN, Color.FromArgb(200, 0, 0, 0)},
			{status.RED, Color.FromArgb(200, 0, 0, 0)},
			{status.BLUE, Color.FromArgb(200, 0, 0, 0)},
			{status.AMBER, Color.FromArgb(200, 0, 0, 0)},
			{status.WHITE, Color.FromArgb(200, 0, 0, 0)}
		};

		readonly Dictionary<status, Color> BColor = new Dictionary<status, Color>()
		{
			{status.OFF, Color.FromArgb(255, 56, 56, 56)},
			{status.GREEN, Color.FromArgb(255, 32, 128, 32)},
			{status.RED, Color.FromArgb(255, 128, 32, 32)},
			{status.BLUE, Color.FromArgb(255, 32, 32, 128)},
			{status.AMBER, Color.FromArgb(255, 128, 100, 32)},
			{status.WHITE, Color.FromArgb(255, 255, 255, 253)}
		};
		
		readonly Dictionary<status, Color> BColorLit = new Dictionary<status, Color>()
		{
			{status.OFF, Color.FromArgb(255, 255, 255, 255)},
			{status.GREEN, Color.FromArgb(255, 32, 255, 32)},
			{status.RED, Color.FromArgb(255, 255, 32, 32)},
			{status.BLUE, Color.FromArgb(255, 32, 32, 128)},
			{status.AMBER, Color.FromArgb(255, 255, 200, 32)},
			{status.WHITE, Color.FromArgb(255, 255, 255, 253)}
		};
		
		public enum status { OFF, GREEN, RED, BLUE, AMBER, WHITE }
		public enum style { SIMPLE, BORDER }
		public style indStyle { get; set; }
		//private bool lit = false;
		
		private Brush backBrush;
		private Brush foreBrush;
		readonly Pen borderPen = new Pen(Color.FromArgb(255, 0, 0, 0), 1f);
		readonly Brush backgroundColor = new SolidBrush(Color.FromArgb(255, 200, 200, 200));
		readonly Pen ControlOuterBorderPen = new Pen(Color.FromArgb(255, 128, 128, 128), 1f);
		readonly Pen ControlInnerBorderPen = new Pen(Color.FromArgb(255, 160, 160, 160), 2f);
		readonly Pen ControlBorderShadowPen = new Pen(Color.FromArgb(55, 0, 0, 0), 1f);

		public Indicator()
		{
			this.DoubleBuffered = true;
			this.indStyle = style.SIMPLE;
		}

		public void setStatus(status s){setStatus(s, false);}
		public void setStatus(status s, bool lit)
		{
			this.ForeColor = FColor[s];
			if (lit)
			{
				this.BackColor = BColorLit[s];
			}
			else
			{
				this.BackColor = BColor[s];
			}
			this.backBrush = new SolidBrush(BackColor);
			this.foreBrush = new SolidBrush(ForeColor);
			this.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			if (indStyle == style.BORDER)
			{
				drawBorder(g);
			}
			else
			{
				drawSimple(g, e);
			}
		}
		
		private void drawBorder(Graphics g)
		{
			// Background color
			g.FillRectangle(backgroundColor, 1, 1, this.Width - 3, this.Height - 3);
			
			// Background color
			g.FillRectangle(backBrush, 3, 3, this.Width - 7, this.Height - 7);
			
			// Draw Text
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			g.DrawString(this.Text, this.Font, foreBrush, new RectangleF(3, 3, this.Width - 7, this.Height - 7), format);
			
			// Shading
			// LIGHT SHADE
			RectangleF mask = new RectangleF(-(this.Width / 2f), -(this.Height / 2f), this.Width * 2, this.Height * 2);
			GraphicsPath ellipsePath = new GraphicsPath();
			ellipsePath.AddEllipse(mask);
			//ellipsePath.AddRectangle(mask);

			using (var maskBrush = new PathGradientBrush(ellipsePath))
			{
				maskBrush.CenterPoint = new PointF(this.Width / 2f, this.Height / 2f);
				maskBrush.CenterColor = Color.FromArgb(0, 0, 0, 0);
				maskBrush.SurroundColors = new[] { Color.FromArgb(200, 0, 0, 0) };
				maskBrush.FocusScales = new PointF(0.2f, 0.2f);
	
				g.FillRectangle(maskBrush, mask);
			}

			// Border
			g.DrawRectangle(ControlOuterBorderPen, 0, 0, this.Width - 1, this.Height - 1);
			
			// Border Shadow
			g.DrawRectangle(ControlBorderShadowPen, 1, 1, this.Width - 3, this.Height - 3);
			
			// Inner Border
			g.DrawRectangle(ControlInnerBorderPen, 3, 3, this.Width - 7, this.Height - 7);
		}

		private void drawSimple(Graphics g, PaintEventArgs e)
		{
			// Background color
			g.FillRectangle(backBrush, 0, 0, this.Width, this.Height);
			
			// Draw Text
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			g.DrawString(this.Text, this.Font, foreBrush, new RectangleF(1, 1, this.Width - 2, this.Height - 2), format);
			
			// Shading
			// LIGHT SHADE
			RectangleF mask = new RectangleF(-(this.Width / 2f), -(this.Height / 2f), this.Width * 2, this.Height * 2);
			GraphicsPath ellipsePath = new GraphicsPath();
			ellipsePath.AddEllipse(mask);
			//ellipsePath.AddRectangle(mask);

			using (var maskBrush = new PathGradientBrush(ellipsePath))
			{
				maskBrush.CenterPoint = new PointF(this.Width / 2f, this.Height / 2f);
				maskBrush.CenterColor = Color.FromArgb(0, 0, 0, 0);
				maskBrush.SurroundColors = new[] { Color.FromArgb(160, 0, 0, 0) };
				maskBrush.FocusScales = new PointF(0.2f, 0.2f);
	
				g.FillRectangle(maskBrush, mask);
			}

			// Border
			g.DrawRectangle(borderPen, new Rectangle(0, 0, this.Width, this.Height));
		}
	}
}
