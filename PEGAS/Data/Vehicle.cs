using System;
using System.Collections.Generic;

namespace PEGAS.Data
{
	public struct Vehicle
	{
		public static bool StagingInProgress { get; set; }
		public static double ThrottleSetting { get; set; }
		public static double ThrottleDisplay { get; set; }

		public List<Stage> Stages { get; set; }
		public string Name { get; set; }
		public double MassTotal { get; set; }
		public double MassFuel { get; set; }
		public double MassDry { get; set; }
		public double GLim { get; set; }
		public double MinThrottle { get; set; }
		public double Throttle { get; set; }
		public bool ShutdownRequired { get; set; }
		public List<Engine> Engines { get; set; }
		public Dictionary<string, object> Staging { get; set; }

		public List<double> GetThrust()
		{
			var F = 0d;
			var dm = 0d;
			var isp = 0d;

			foreach (var stage in Stages)
			{
				var thrust = stage.GetThrust();
				F += thrust[0];
				dm += thrust[1];
				isp += thrust[2];
			}

			return new List<double>
			{
				F, dm, isp
			};
		}

		public double ConstAccBurnTime(Stage stage)
		{
			var engines = stage.GetThrust();
			var isp = engines[2];
			var baseFlow = engines[1];
			var mass = stage.MassTotal;
			var fuel = stage.MassFuel;
			var gLim = stage.GLim;
			var tMin = stage.MinThrottle;
			var maxBurnTime = isp / gLim * Math.Log(mass / (mass - fuel));

			if (tMin == 0)
			{
				return maxBurnTime;
			}

			var violationTime = -isp / gLim * Math.Log(tMin);
			var constThrustTime = 0d;

			if (violationTime < maxBurnTime)
			{
				var burnedFuel = mass * (1 - Math.Pow(Math.E, -gLim / isp * violationTime));
				constThrustTime = (fuel - burnedFuel) / (baseFlow * tMin);
			}

			return maxBurnTime + constThrustTime;
		}
	}
}
