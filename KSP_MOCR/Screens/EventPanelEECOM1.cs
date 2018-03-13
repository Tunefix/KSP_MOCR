using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace KSP_MOCR
{ 
	class EventPanelEECOM1 : MocrScreen
	{
		public EventPanelEECOM1(Screen form)
		{
			this.form = form;
			this.form.BackColor = Color.FromArgb(255, 62, 64, 68);
			this.charSize = false;

			Image myimage = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "Resources\\darknoise.png");
			this.form.BackgroundImage = myimage;
			this.form.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Tile;

			this.width = 532;
			this.height = 196;
		}

		public override void makeElements()
		{
			for (int i = 0; i < 4; i++) screenScrews.Add(null); // Initialize Screws
			for (int i = 0; i < 72; i++) screenEventIndicators.Add(null); // Initialize EventIndicators


			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenScrews[0] = Helper.CreateScrew(4, 42, true);
			screenScrews[1] = Helper.CreateScrew(500, 42, true);
			screenScrews[2] = Helper.CreateScrew(4, 140, true);
			screenScrews[3] = Helper.CreateScrew(500, 140, true);

			/// ROW ONE
			screenEventIndicators[0] = Helper.createEventIndicator(54, 10, true, true);
			screenEventIndicators[0].upperText = "EECOM\nALL LIM";
			screenEventIndicators[0].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[1] = Helper.createEventIndicator(101, 10, true, true);
			screenEventIndicators[1].upperText = "CSM\nLIVE";
			screenEventIndicators[1].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[2] = Helper.createEventIndicator(148, 10, true, true);
			screenEventIndicators[2].upperText = "CSM FM TM\n1.1024MHZ";
			screenEventIndicators[2].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[3] = Helper.createEventIndicator(195, 10, true, true);
			screenEventIndicators[3].upperText = "DSE RUN\nCT0012X";
			screenEventIndicators[3].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[4] = Helper.createEventIndicator(242, 10, true, true);
			screenEventIndicators[4].upperText = "LOG BUS A\nCD0200V";
			screenEventIndicators[4].upperColor = EventIndicator.color.OFF; // AMBER

			screenEventIndicators[5] = Helper.createEventIndicator(289, 10, true, true);
			screenEventIndicators[5].upperText = "SCS BUS A\nCD0170X";
			screenEventIndicators[5].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[6] = Helper.createEventIndicator(336, 10, true, true);
			screenEventIndicators[6].upperText = "APEX A\nCD0230X";
			screenEventIndicators[6].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[7] = Helper.createEventIndicator(383, 10, true, true);
			screenEventIndicators[7].upperText = "ABORT REQ\nBS0080X";
			screenEventIndicators[7].upperColor = EventIndicator.color.OFF; // GREEN

			screenEventIndicators[8] = Helper.createEventIndicator(430, 10, true, true);
			screenEventIndicators[8].upperText = "EDS VOT 1\nCDO132X R";
			screenEventIndicators[8].upperColor = EventIndicator.color.OFF; // GREEN

			/// ROW TWO
			screenEventIndicators[9] = Helper.createEventIndicator(54, 32, true, true);
			screenEventIndicators[9].upperText = "LAUNCH\nLIMITS";
			screenEventIndicators[9].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[10] = Helper.createEventIndicator(101, 32, true, true);
			screenEventIndicators[10].upperText = "CSM\nSTATIC";
			screenEventIndicators[10].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[11] = Helper.createEventIndicator(148, 32, true, true);
			screenEventIndicators[11].upperText = "CSM PM\nDN LOCK";
			screenEventIndicators[11].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[12] = Helper.createEventIndicator(195, 32, true, true);
			screenEventIndicators[12].upperText = "SCE +20\nCT0015V";
			screenEventIndicators[12].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[13] = Helper.createEventIndicator(242, 32, true, true);
			screenEventIndicators[13].upperText = "LOG BUS B\nCD0201V";
			screenEventIndicators[13].upperColor = EventIndicator.color.OFF; // AMBER

			screenEventIndicators[14] = Helper.createEventIndicator(289, 32, true, true);
			screenEventIndicators[14].upperText = "SCS RCS B\nCD0171X";
			screenEventIndicators[14].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[15] = Helper.createEventIndicator(336, 32, true, true);
			screenEventIndicators[15].upperText = "APEX B\nCD0231X";
			screenEventIndicators[15].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[16] = Helper.createEventIndicator(383, 32, true, true);
			screenEventIndicators[16].upperText = "ABORT REQ\nBS0081X";
			screenEventIndicators[16].upperColor = EventIndicator.color.OFF; // GREEN

			screenEventIndicators[17] = Helper.createEventIndicator(430, 32, true, true);
			screenEventIndicators[17].upperText = "EDS VOT 2\nCD0133X T";
			screenEventIndicators[17].upperColor = EventIndicator.color.OFF; // GREEN

			/// ROW THREE
			screenEventIndicators[18] = Helper.createEventIndicator(54, 54, true, true);
			screenEventIndicators[18].upperText = "ENTRY\nLIMITS";
			screenEventIndicators[18].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[19] = Helper.createEventIndicator(101, 54, true, true);
			screenEventIndicators[19].upperText = "CSM\nLFI";
			screenEventIndicators[19].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[20] = Helper.createEventIndicator(148, 54, true, true);
			screenEventIndicators[20].upperText = "CSM PM TM\n1.024MHZ";
			screenEventIndicators[20].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[21] = Helper.createEventIndicator(195, 54, true, true);
			screenEventIndicators[21].upperText = "SCE -20\nCT0016V";
			screenEventIndicators[21].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[22] = Helper.createEventIndicator(242, 54, true, true);
			screenEventIndicators[22].upperText = "PYR BUS A\nCD0005V";
			screenEventIndicators[22].upperColor = EventIndicator.color.OFF; // AMBER

			screenEventIndicators[23] = Helper.createEventIndicator(289, 54, true, true);
			screenEventIndicators[23].upperText = "CM RCS A\nCD0173X";
			screenEventIndicators[23].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[24] = Helper.createEventIndicator(336, 54, true, true);
			screenEventIndicators[24].upperText = "DRO DEP A\nCE0001X";
			screenEventIndicators[24].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[25] = Helper.createEventIndicator(383, 54, true, true);
			screenEventIndicators[25].upperText = "CREW ABRT\nCD0130X";
			screenEventIndicators[25].upperColor = EventIndicator.color.OFF; // GREEN

			screenEventIndicators[26] = Helper.createEventIndicator(430, 54, true, true);
			screenEventIndicators[26].upperText = "EDS VOT 3\nCD0135X R";
			screenEventIndicators[26].upperColor = EventIndicator.color.OFF; // GREEN

			/// ROW FOUR
			screenEventIndicators[27] = Helper.createEventIndicator(54, 76, true, true);
			screenEventIndicators[27].upperText = "";
			screenEventIndicators[27].upperColor = EventIndicator.color.OFF;

			screenEventIndicators[28] = Helper.createEventIndicator(101, 76, true, true);
			screenEventIndicators[28].upperText = "CSM EVENTS\nPLAYBACK";
			screenEventIndicators[28].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[29] = Helper.createEventIndicator(148, 76, true, true);
			screenEventIndicators[29].upperText = "CSM UDL\n70KHZ";
			screenEventIndicators[29].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[30] = Helper.createEventIndicator(195, 76, true, true);
			screenEventIndicators[30].upperText = "SCE +10\nCT0018V";
			screenEventIndicators[30].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[31] = Helper.createEventIndicator(242, 76, true, true);
			screenEventIndicators[31].upperText = "PYR BUS B\nCD0006V";
			screenEventIndicators[31].upperColor = EventIndicator.color.OFF; // AMBER

			screenEventIndicators[32] = Helper.createEventIndicator(289, 76, true, true);
			screenEventIndicators[32].upperText = "CM RCS B\nCD0174X";
			screenEventIndicators[32].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[33] = Helper.createEventIndicator(336, 76, true, true);
			screenEventIndicators[33].upperText = "DRO DEP B\nCE0002X";
			screenEventIndicators[33].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[34] = Helper.createEventIndicator(383, 76, true, true);
			screenEventIndicators[34].upperText = "CREW ABRT\nCD0131X";
			screenEventIndicators[34].upperColor = EventIndicator.color.OFF; // GREEN

			screenEventIndicators[35] = Helper.createEventIndicator(430, 76, true, true);
			screenEventIndicators[35].upperText = "EDS ABT A\nCD0135X";
			screenEventIndicators[35].upperColor = EventIndicator.color.OFF; // GREEN

			/// ROW FIVE
			screenEventIndicators[36] = Helper.createEventIndicator(54, 98, true, true);
			screenEventIndicators[36].upperText = "FC 1\nLIMITS";
			screenEventIndicators[36].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[37] = Helper.createEventIndicator(101, 98, true, true);
			screenEventIndicators[37].upperText = "CO2 CAL\n14.7-30";
			screenEventIndicators[37].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[38] = Helper.createEventIndicator(148, 98, true, true);
			screenEventIndicators[38].upperText = "CSM G/A\n30KHZ";
			screenEventIndicators[38].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[39] = Helper.createEventIndicator(195, 98, true, true);
			screenEventIndicators[39].upperText = "SCE +5\nCT0017V";
			screenEventIndicators[39].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[40] = Helper.createEventIndicator(242, 98, true, true);
			screenEventIndicators[40].upperText = "";
			screenEventIndicators[40].upperColor = EventIndicator.color.OFF;

			screenEventIndicators[41] = Helper.createEventIndicator(289, 98, true, true);
			screenEventIndicators[41].upperText = "CSM SEP A\nCD0023X";
			screenEventIndicators[41].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[42] = Helper.createEventIndicator(336, 98, true, true);
			screenEventIndicators[42].upperText = "MN DEP A\nCE0003X";
			screenEventIndicators[42].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[43] = Helper.createEventIndicator(383, 98, true, true);
			screenEventIndicators[43].upperText = "";
			screenEventIndicators[43].upperColor = EventIndicator.color.OFF;

			screenEventIndicators[44] = Helper.createEventIndicator(430, 98, true, true);
			screenEventIndicators[44].upperText = "EDS ABT B\nCD0136X";
			screenEventIndicators[44].upperColor = EventIndicator.color.OFF; // GREEN

			/// ROW SIX
			screenEventIndicators[45] = Helper.createEventIndicator(54, 120, true, true);
			screenEventIndicators[45].upperText = "FC 2\nLIMITS";
			screenEventIndicators[45].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[46] = Helper.createEventIndicator(101, 120, true, true);
			screenEventIndicators[46].upperText = "CO2 CAL\n3.7-10";
			screenEventIndicators[46].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[47] = Helper.createEventIndicator(148, 120, true, true);
			screenEventIndicators[47].upperText = "CSM A/G\n1.25MHZ";
			screenEventIndicators[47].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[48] = Helper.createEventIndicator(195, 120, true, true);
			screenEventIndicators[48].upperText = "PCM INT\nCT0340X R";
			screenEventIndicators[48].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[49] = Helper.createEventIndicator(242, 120, true, true);
			screenEventIndicators[49].upperText = "";
			screenEventIndicators[49].upperColor = EventIndicator.color.OFF;

			screenEventIndicators[50] = Helper.createEventIndicator(289, 120, true, true);
			screenEventIndicators[50].upperText = "CSM SEP B\nCD0024X";
			screenEventIndicators[50].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[51] = Helper.createEventIndicator(336, 120, true, true);
			screenEventIndicators[51].upperText = "MN DEP B\nCE0004X";
			screenEventIndicators[51].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[52] = Helper.createEventIndicator(383, 120, true, true);
			screenEventIndicators[52].upperText = "";
			screenEventIndicators[52].upperColor = EventIndicator.color.OFF;

			screenEventIndicators[53] = Helper.createEventIndicator(430, 120, true, true);
			screenEventIndicators[53].upperText = "";
			screenEventIndicators[53].upperColor = EventIndicator.color.OFF;

			/// ROW SEVEN
			screenEventIndicators[54] = Helper.createEventIndicator(54, 142, true, true);
			screenEventIndicators[54].upperText = "FC 3\nLIMITS";
			screenEventIndicators[54].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[55] = Helper.createEventIndicator(101, 142, true, true);
			screenEventIndicators[55].upperText = "FLO CAL\n60F-10";
			screenEventIndicators[55].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[56] = Helper.createEventIndicator(148, 142, true, true);
			screenEventIndicators[56].upperText = "CSM PRN\nUPLINK";
			screenEventIndicators[56].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[57] = Helper.createEventIndicator(195, 142, true, true);
			screenEventIndicators[57].upperText = "";
			screenEventIndicators[57].upperColor = EventIndicator.color.OFF;

			screenEventIndicators[58] = Helper.createEventIndicator(242, 142, true, true);
			screenEventIndicators[58].upperText = "RNG SEP A\nCD1154X";
			screenEventIndicators[58].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[59] = Helper.createEventIndicator(289, 142, true, true);
			screenEventIndicators[59].upperText = "SLA SEP A\nCD0123X";
			screenEventIndicators[59].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[60] = Helper.createEventIndicator(336, 142, true, true);
			screenEventIndicators[60].upperText = "MN DIS A\nCE0321X";
			screenEventIndicators[60].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[61] = Helper.createEventIndicator(383, 142, true, true);
			screenEventIndicators[61].upperText = "";
			screenEventIndicators[61].upperColor = EventIndicator.color.OFF;

			screenEventIndicators[62] = Helper.createEventIndicator(430, 142, true, true);
			screenEventIndicators[62].upperText = "MC&W LTCH\nCS0150X R";
			screenEventIndicators[62].upperColor = EventIndicator.color.OFF; // GREEN

			/// ROW EIGHT
			screenEventIndicators[63] = Helper.createEventIndicator(54, 164, true, true);
			screenEventIndicators[63].upperText = "";
			screenEventIndicators[63].upperColor = EventIndicator.color.OFF;

			screenEventIndicators[64] = Helper.createEventIndicator(101, 164, true, true);
			screenEventIndicators[64].upperText = "FLO CAL\n120F-30";
			screenEventIndicators[64].upperColor = EventIndicator.color.OFF; // BLUE

			screenEventIndicators[65] = Helper.createEventIndicator(148, 164, true, true);
			screenEventIndicators[65].upperText = "";
			screenEventIndicators[65].upperColor = EventIndicator.color.OFF;

			screenEventIndicators[66] = Helper.createEventIndicator(195, 164, true, true);
			screenEventIndicators[66].upperText = "";
			screenEventIndicators[66].upperColor = EventIndicator.color.OFF;

			screenEventIndicators[67] = Helper.createEventIndicator(242, 164, true, true);
			screenEventIndicators[67].upperText = "RNG SEP B\nCD1155X";
			screenEventIndicators[67].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[68] = Helper.createEventIndicator(289, 164, true, true);
			screenEventIndicators[68].upperText = "SLA SEP B\nCD0124X";
			screenEventIndicators[68].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[69] = Helper.createEventIndicator(336, 164, true, true);
			screenEventIndicators[69].upperText = "MN DIS B\nCE0322X";
			screenEventIndicators[69].upperColor = EventIndicator.color.OFF; // WHITE

			screenEventIndicators[70] = Helper.createEventIndicator(383, 164, true, true);
			screenEventIndicators[70].upperText = "PROBE T\nCS0220T";
			screenEventIndicators[70].upperColor = EventIndicator.color.OFF; // AMBER

			screenEventIndicators[71] = Helper.createEventIndicator(430, 164, true, true);
			screenEventIndicators[71].upperText = "MC&W\nCS0150X R";
			screenEventIndicators[71].upperColor = EventIndicator.color.OFF; // GREEN
		}

		public override void resize()
		{
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
		}
	}
}
