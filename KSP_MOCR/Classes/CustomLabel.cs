using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
		public bool bigText = true; // Wheter to use text that fill the height. (More than default)
		public bool blur = false; // Slighty blurry text, looks a little like a crt monitor
		public bool glow = false; // A slight glow of the text, also a crt-effect.
		public enum LabelType { NORMAL, ENGRAVED, CRT}
		public LabelType type = LabelType.NORMAL;

		public CustomLabel()
		{
		}

		public void setCharWidth(double w) { this.charWidth = w; }
		public void setlineHeight(double h) { this.lineHeight = h; }
		public void setcharOffset(double o) { this.charOffset = o; }
		public void setlineOffset(double o) { this.lineOffset = o; }

		protected override void OnPaint(PaintEventArgs e)
		{
			// Simple draw-line
			//e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), -2.5f, 0);

			// Some quality enhancments
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

			switch (type)
			{
				case LabelType.NORMAL:
					DrawNormal(e);
					break;
				case LabelType.ENGRAVED:
					DrawEngraved(e);
					break;
				case LabelType.CRT:
					DrawCRT(e);
					break;
				default:
					DrawNormal(e);
					break;
			}
		}

		void DrawCRT(PaintEventArgs e)
		{
			if (blur)
			{
				e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			}
			else
			{
				e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			}

			// Draw each character on its own, to align everything
			float yPos = (float)lineOffset;
			double charsBack = 0;
			GraphicsState state = e.Graphics.Save();

			double scaledLineHeight = lineHeight;
			
			e.Graphics.ScaleTransform(1.0f, 1.20f);
			scaledLineHeight = lineHeight / 1.20f;
			yPos = (float)(lineOffset - (lineHeight * 0.20f));


			for (int i = 0; i < Text.Length; i++)
			{
				String letter = Text.Substring(i, 1);

				if (letter == "\n")
				{
					yPos += (float)scaledLineHeight;
					charsBack = (i + 1) * charWidth;
				}
				else
				{
					float xPos = (float)(charOffset + (charWidth * i) - charsBack);
					e.Graphics.DrawString(letter, Font, new SolidBrush(ForeColor), xPos, yPos);
				}
			}

			e.Graphics.Restore(state);
		}

		void DrawEngraved(PaintEventArgs e)
		{
			// BACKGROUND-COLOR
			Brush bColor = new SolidBrush(Color.FromArgb(200, 220, 220, 220));
			e.Graphics.FillRectangle(bColor, 2, 0, this.Width - 4, this.Height);

			// DARK TEXT
			ForeColor = Color.FromArgb(200, 0, 0, 0);

			// BORDERS
			Pen dBorder = new Pen(Color.FromArgb(255, 160, 160, 160), 3f);
			Pen ShadowPen = new Pen(Color.FromArgb(128, 0, 0, 0), 1f);

			e.Graphics.DrawLine(ShadowPen, 0f, 3.5f, Width, 3.5f); // Top Shadow
			e.Graphics.DrawLine(dBorder, 0f, 1.5f, Width, 1.5f); // Top

			e.Graphics.DrawLine(ShadowPen, 0f, Height - 3.5f, Width, Height - 3.5f); // Bottom Shadow
			e.Graphics.DrawLine(dBorder, 0f, Height - 2f, Width, Height - 2f); // Bottom

			e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			drawCenterText(e);
		}

		void DrawNormal(PaintEventArgs e)
		{
			if (blur)
			{
				e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			}
			else
			{
				e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			}

			drawText(e);
		}

		void drawText(PaintEventArgs e)
		{
			// Draw each character on its own, to align everything
			float yPos = (float)lineOffset;
			double charsBack = 0;
			GraphicsState state = e.Graphics.Save();

			double scaledLineHeight = lineHeight;

			if (bigText)
			{
				e.Graphics.ScaleTransform(1.0f, 1.20f);
				scaledLineHeight = lineHeight / 1.20f;
				yPos = (float)(lineOffset - (lineHeight * 0.20f));
			}

			for (int i = 0; i < Text.Length; i++)
			{
				String letter = Text.Substring(i, 1);

				if (letter == "\n")
				{
					yPos += (float)scaledLineHeight;
					charsBack = (i + 1) * charWidth;
				}
				else
				{
					float xPos = (float)(charOffset + (charWidth * i) - charsBack);
					e.Graphics.DrawString(letter, Font, new SolidBrush(ForeColor), xPos, yPos);
				}
			}

			if (bigText)
			{
				e.Graphics.Restore(state);
			}
		}

		void drawCenterText(PaintEventArgs e)
		{
			// Find x-offset
			double spaceReserve = Width - (Text.Length * charWidth);
			charOffset = spaceReserve / 2;

			// Find y-offset (Engraved are always 1 line)
			spaceReserve = Height - lineHeight;
			lineOffset = 1.5f;
			float yPos = (float)(lineOffset + (spaceReserve / 2));


			// Draw each character on its own, to align everything
			double charsBack = 0;
			GraphicsState state = e.Graphics.Save();

			double scaledLineHeight = lineHeight;

			if (bigText)
			{
				e.Graphics.ScaleTransform(1.0f, 1.20f);
				scaledLineHeight = lineHeight / 1.20f;
				yPos = (float)(yPos - (lineHeight * 0.20f));
			}

			for (int i = 0; i < Text.Length; i++)
			{
				String letter = Text.Substring(i, 1);

				if (letter == "\n")
				{
					yPos += (float)scaledLineHeight;
					charsBack = (i + 1) * charWidth;
				}
				else
				{
					Brush blur = new SolidBrush(Color.FromArgb(128, ForeColor.R, ForeColor.G, ForeColor.B));
					float xPos = (float)(charOffset + (charWidth * i) - charsBack);
					e.Graphics.DrawString(letter, Font, new SolidBrush(ForeColor), xPos, yPos);
					e.Graphics.DrawString(letter, Font, blur, xPos + 1f, yPos);
					e.Graphics.DrawString(letter, Font, blur, xPos - 1f, yPos);

				}
			}

			if (bigText)
			{
				e.Graphics.Restore(state);
			}
		}
	}
}
