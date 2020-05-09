using System.Collections.Generic;
using System.Net;
using KRPC.Client.Services.SpaceCenter;
using PEGAS.Data;
using PEGAS.Events;
using PEGAS.Maths;
using PEGAS.Utilities;

namespace PEGAS
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			var upfg = new UPFG();


			UPFG.UpfgStage = -1;
			UPFG.UpfgConverged = false;
			Globals.StageEventFlag = false;
			Globals.SystemEvents = new List<SystemEvent>();
			Globals.SystemEventPointer = -1;
			Globals.SystemEventFlag = false;
			Globals.UserEventPointer = -1;
			Globals.UserEventFlag = false;
			Globals.CommsEventFlag = false;
			Globals.SteeringRoll = 0;
			Globals.Connection = new KSPConnection(
				"PEGAS",
					IPAddress.Parse("172.0.0.1"),
				123,
				1234
				);
			Vehicle.ThrottleDisplay = 1;
			Vehicle.ThrottleSetting = 1;
			Vehicle.StagingInProgress = false;


			// load vehicle
			// load controls
			// load sequence
			// load mission

		}
	}

	
}
