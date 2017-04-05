using System;
using System.Threading;
using KRPC.Client;
using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;


public class KrpcData
{

	private Thread fthread;
	private bool fetch_data;
	private bool fetch_data_lock;
	private uint error_count;


	private KRPC.Client.Services.KRPC.Service krpc;
	private KRPC.Client.Services.SpaceCenter.Service spaceCenter;
	private KRPC.Client.Services.SpaceCenter.Vessel vessel;
	private KRPC.Client.Services.SpaceCenter.Flight flight;
	private KRPC.Client.Services.SpaceCenter.Resources vessel_resources;
	private KRPC.Client.Services.SpaceCenter.Control vessel_control;
	private KRPC.Client.Services.SpaceCenter.Resources vessel_resources_stage;

	private KRPC.Client.Services.KRPC.GameScene _CurrentGameScene;
	private bool _Isset_CurrentGameScene = false;
	private double _ActiveVessel_MET = -1;
	private uint count = 0;

	public KrpcData(KRPC.Client.Services.KRPC.Service arg_krpc, KRPC.Client.Services.SpaceCenter.Service arg_spaceCenter)
	{
		this.krpc = arg_krpc;
		this.spaceCenter = arg_spaceCenter;
		this.fetch_data = true;
		this.fetch_data_lock = false;
		this.error_count = 0;
		this.vessel = spaceCenter.ActiveVessel;
		this.flight = spaceCenter.ActiveVessel.Flight(spaceCenter.ActiveVessel.Orbit.Body.ReferenceFrame);
		this.vessel_control = spaceCenter.ActiveVessel.Control;
		this.vessel_resources = spaceCenter.ActiveVessel.Resources;
		this.vessel_resources_stage = spaceCenter.ActiveVessel.ResourcesInDecoupleStage(spaceCenter.ActiveVessel.Control.CurrentStage);
	}

	public void Run()
	{
		this.fthread = new Thread(new ThreadStart(this.FetchData));
		this.fthread.IsBackground = true;
		this.fthread.Start();
	}

	public void FetchData()
	{
		while(this.fetch_data)
		{
			while(this.fetch_data_lock);
			this.fetch_data_lock = true;
			
			try 
			{
				this.StartThread(this.FetchData__CurrentGameScene);
				this.StartThread(this.FetchData__ActiveVessel_MET);
			}
			catch(Exception ex)
			{
				if(this.error_count++ > 100)
				{
					throw ex;
				}
			}

			this.fetch_data_lock = false;
			System.Threading.Thread.Sleep(1000);

		}
	}

	private void StartThread(Action func)
	{
		Thread xthread;
		xthread = new Thread(new ThreadStart(func));
		xthread.IsBackground = true;
		xthread.Start();
		++this.count;
	}

	/* CurrentGameScene */
	public void FetchData__CurrentGameScene()
	{
		this._CurrentGameScene = this.krpc.CurrentGameScene;
		this._Isset_CurrentGameScene = true;
	}

	public KRPC.Client.Services.KRPC.GameScene CurrentGameScene {
		get {
			uint x = 0;
			while(!this._Isset_CurrentGameScene && ++x < 10) {
				System.Threading.Thread.Sleep(100);
			}
			return this._CurrentGameScene;
		}
	}

	/* ActiveVessel_MET */
	public void FetchData__ActiveVessel_MET()
	{
		this._ActiveVessel_MET = this.vessel.MET;
	}

	public double ActiveVessel_MET {
		get {
			return this._ActiveVessel_MET;
		}
	}
}
