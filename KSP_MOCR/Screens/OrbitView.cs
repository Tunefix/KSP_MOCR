using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KSP_MOCR
{
	class OrbitView : MocrScreen
	{
		CelestialBody body;
		float bodyRadius;
		String bodyName;

		// TELEMETRY FIELDS
		double apopapsis;
		double periapsis;
		double eccentricity;
		double inclination;
		double sMa;
		double sma;
		double argOP;
		double lOAN;
		double radius;
		double trueAnomaly;
		double timeToPe;
		double timeToAp;
		double period;
		Orbit orbit;
		double UT;

		IList<Node> nodes;
		ReferenceFrame inerFrame;

		bool leftMouseButtonPressed = false;
		double mouseStartX = 0;
		double mouseStartY = 0;
		double mouseDX = 0;
		double mouseDY = 0;

		public OrbitView(Screen form)
		{
			this.form = form;
			this.screenStreams = form.streamCollection;

			this.charSize = false;
			this.width = 674;
			this.height = 508;
			this.updateRate = 500;

			
		}

		public override void makeElements()
		{
			for (int i = 0; i < 1; i++) screenInputs.Add(null); // Initialize Inputs
			screenInputs[0] = Helper.CreateInput(-2, -2, 1, 2); // Every page must have an input to capture keypresses on Unix

			screenOrbit = Helper.CreateOrbit(0, 0, 674, 508, true);
			screenOrbit.MouseDown += orbitMouseDown;
			screenOrbit.MouseMove += orbitMouseMove;
			screenOrbit.MouseUp += orbitMouseUp;
			screenOrbit.MouseWheel += orbitMouseWheel;
		}

		public override void updateLocalElements(object sender, EventArgs e)
		{
			if (form.form.connected && form.form.krpc.CurrentGameScene == GameScene.Flight)
			{
				orbit = screenStreams.GetData(DataType.vessel_orbit);
				nodes = screenStreams.GetData(DataType.control_nodes);
				body = screenStreams.GetData(DataType.orbit_celestialBody);
				UT = screenStreams.GetData(DataType.spacecenter_universial_time);

				if (body != null)
				{
					bodyRadius = body.EquatorialRadius;
					bodyName = body.Name;

					IList<CelestialBody> bodySatellites = body.Satellites;
					screenOrbit.setBody(body);
					screenOrbit.setOrbit(orbit);

					if (nodes != null && nodes.Count > 0)
					{
						screenOrbit.setBurnNodes(nodes);
						screenOrbit.setUT(UT);
					}
					else
					{
						screenOrbit.setBurnNodes(null);
					}
				}
				else
				{
					screenOrbit.setBody(null);
				}
				screenOrbit.Invalidate();
			}
		}

		public override void resize()
		{
			if (screenOrbit != null)
			{
				screenOrbit.Size = form.ClientSize;
			}
		}

		private void orbitMouseDown(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				leftMouseButtonPressed = true;
				mouseStartX = e.X;
				mouseStartY = e.Y;
			}
		}

		private void orbitMouseMove(object sender, MouseEventArgs e)
		{
			if(leftMouseButtonPressed)
			{
				mouseDX = e.X - mouseStartX;
				mouseDY = e.Y - mouseStartY;
				screenOrbit.setSlideTmpXY(mouseDX, mouseDY);
				screenOrbit.Invalidate();
			}
		}

		private void orbitMouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				leftMouseButtonPressed = false;
				screenOrbit.addSlideXY(mouseDX, mouseDY);
				screenOrbit.setSlideTmpXY(0, 0);
				screenOrbit.Invalidate();
			}
		}

		private void orbitMouseWheel(object sender, MouseEventArgs e)
		{
			screenOrbit.addZoom(e.Delta / 120);
			screenOrbit.Invalidate();
		}
	}
}
