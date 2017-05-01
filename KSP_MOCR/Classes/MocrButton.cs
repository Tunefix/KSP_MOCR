using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KSP_MOCR
{
	public class MocrButton : Control
	{
		public enum style {LIGHT, PUSH}
		public style buttonStyle { get; set; }

		private bool pressed = false;
		public bool lit { get; private set; }

		readonly private Brush SpacecraftButtonBrush = new SolidBrush(Color.FromArgb(255, 34, 34, 34));
		readonly private Pen SpacecraftButtonBorderPenDark = new Pen(Color.FromArgb(255, 26, 26, 26), 1f);
		readonly private Pen SpacecraftButtonBorderPenLight = new Pen(Color.FromArgb(255, 42, 42, 42), 1f);

		readonly Pen ControlOuterBorderPen = new Pen(Color.FromArgb(255, 128, 128, 128), 4f);
		readonly Pen ControlInnerBorderPen = new Pen(Color.FromArgb(255, 160, 160, 160), 2f);
		readonly Pen ControlBorderShadowPen = new Pen(Color.FromArgb(55, 0, 0, 0), 1f);

		readonly Brush textBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255));

		readonly Brush backgroundColorBlankDim = new SolidBrush(Color.FromArgb(255, 200, 200, 200));
		readonly Brush backgroundColorBlankLit = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
		readonly Brush backgroundColorRedDim = new SolidBrush(Color.FromArgb(255, 100, 0, 0));
		readonly Brush backgroundColorRedLit = new SolidBrush(Color.FromArgb(255, 255, 0, 0));
		readonly Brush backgroundColorAmberDim = new SolidBrush(Color.FromArgb(255, 100, 100, 0));
		readonly Brush backgroundColorAmberLit = new SolidBrush(Color.FromArgb(255, 255, 255, 0));
		readonly Brush backgroundColorGreenDim = new SolidBrush(Color.FromArgb(255, 0, 100, 0));
		readonly Brush backgroundColorGreenLit = new SolidBrush(Color.FromArgb(255, 0, 255, 0));

		Brush litBrush;
		Brush dimBrush;

		public enum color { BLANK, RED, AMBER, GREEN }
		private color buttonColor;
		
		public MocrButton()
		{
			this.DoubleBuffered = true;
			this.buttonStyle = style.PUSH;
			this.buttonColor = color.BLANK;
			this.lit = false;
			this.dimBrush = backgroundColorBlankDim;
			this.litBrush = backgroundColorBlankLit;
		}

		public void setLightColor(color c)
		{
			this.buttonColor = c;
			switch (c)
			{
				case color.BLANK:
					dimBrush = backgroundColorBlankDim;
					litBrush = backgroundColorBlankLit;
					break;
				case color.AMBER:
					dimBrush = backgroundColorAmberDim;
					litBrush = backgroundColorAmberLit;
					break;
				case color.RED:
					dimBrush = backgroundColorRedDim;
					litBrush = backgroundColorRedLit;
					break;
				case color.GREEN:
					dimBrush = backgroundColorGreenDim;
					litBrush = backgroundColorGreenLit;
					break;
			}
		}
		
		public void setPressedState(bool state)
		{
			this.pressed = state;
			this.Invalidate();
		}
		
		public void setLitState(bool state)
		{
			this.lit = state;
			this.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			if (buttonStyle == style.LIGHT)
			{
				DrawControlButton(g);
			}
			else
			{
				DrawSpacecraftButton(g);
			}
		}

		private void DrawControlButton(Graphics g)
		{
			// Setup brushes and pens
			Brush brush1;
			if (this.lit)
			{
				brush1 = litBrush;
			}
			else
			{
				brush1 = dimBrush;
			}

			float innerX = 7;
			float innerY = 7;
			if (this.pressed)
			{
				innerX = 8;
				innerY = 8;
			}

			// Background color
			g.FillRectangle(backgroundColorBlankDim, 3, 3, this.Width - 6, this.Height - 6);
			
			// Button color
			g.FillRectangle(brush1, 8, 8, this.Width - 16, this.Height - 16);
			
			// Draw Text
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			g.DrawString(this.Text, this.Font, textBrush, new RectangleF(innerX, innerY, this.Width - 14, this.Height - 14), format);
			
			
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
			g.DrawRectangle(ControlOuterBorderPen, 3, 3, this.Width - 6, this.Height - 6);
			
			// Border Shadow
			g.DrawRectangle(ControlBorderShadowPen, 5, 5, this.Width - 10, this.Height - 10);
			
			// Inner Border
			g.DrawRectangle(ControlInnerBorderPen, innerX, innerY, this.Width - 14, this.Height - 14);
			
			
			
		}

		private void DrawSpacecraftButton(Graphics g)
		{
			Brush brush1;
			Pen pen1;
			Pen pen2;

			if (this.pressed)
			{
				pen1 = SpacecraftButtonBorderPenDark;
				pen2 = SpacecraftButtonBorderPenLight;
				brush1 = SpacecraftButtonBrush;
			}
			else
			{
				pen1 = SpacecraftButtonBorderPenLight;
				pen2 = SpacecraftButtonBorderPenDark;
				brush1 = SpacecraftButtonBrush;
			}
			
			// Draw rounded corners
			float cornerRadius = 3f;
			g.FillEllipse(brush1, new RectangleF(1, 1, cornerRadius, cornerRadius));
			g.FillEllipse(brush1, new RectangleF(this.Width - 2 - cornerRadius, 1, cornerRadius, cornerRadius));
			g.FillEllipse(brush1, new RectangleF(this.Width - 2- cornerRadius, this.Height - 2- cornerRadius, cornerRadius, cornerRadius));
			g.FillEllipse(brush1, new RectangleF(1, this.Height - 2- cornerRadius, cornerRadius, cornerRadius));

			// Draw Cross between corners
			g.FillRectangle(brush1, new RectangleF(1 + (cornerRadius / 2), 1, this.Width - 2 - cornerRadius, this.Height - 2));
			g.FillRectangle(brush1, new RectangleF(1, 1 + (cornerRadius / 2), this.Width - 2 , this.Height - 2 - cornerRadius));

			// Draw corner Borders
			g.DrawArc(pen1, 1, 1, cornerRadius, cornerRadius, 180, 90);
			g.DrawArc(pen1, this.Width - 2 - cornerRadius, 1, cornerRadius, cornerRadius, 270, 45);
			g.DrawArc(pen2, this.Width - 2 - cornerRadius, 1, cornerRadius, cornerRadius, 315, 45);
			g.DrawArc(pen2, this.Width - 2 - cornerRadius, this.Height - 2 - cornerRadius, cornerRadius, cornerRadius, 0, 90);
			g.DrawArc(pen2, 1, this.Height - 2 - cornerRadius, cornerRadius, cornerRadius, 90, 45);
			g.DrawArc(pen1, 1, this.Height - 2 - cornerRadius, cornerRadius, cornerRadius, 135, 45);

			// Draw edge borders
			g.DrawLine(pen1, 1 + (cornerRadius / 2), 1, this.Width - 1 - cornerRadius, 1);
			g.DrawLine(pen2, this.Width - 1, 1 + (cornerRadius / 2), this.Width - 1, this.Height - 1 - cornerRadius);
			g.DrawLine(pen2, 1 + (cornerRadius / 2), this.Height - 1, this.Width - 1 - (cornerRadius / 2), this.Height - 1);
			g.DrawLine(pen1, 1, this.Height - 1 - (cornerRadius / 2), 1, 1 + (cornerRadius / 2));

			// Draw Text
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			float x = 2;
			float y = 2;
			if (this.pressed) { x = 3; y = 3; }
			g.DrawString(this.Text, this.Font, textBrush, new RectangleF(x, y, this.Width - 4, this.Height - 4), format);
		}
		
		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.pressed = true;
			this.Invalidate();
			base.OnMouseDown(e);
		}
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			this.pressed = false;
			this.Invalidate();
			base.OnMouseUp(e);
		}

		public void PerformClick()
		{
			this.OnClick(new EventArgs());
		}
	}
}
