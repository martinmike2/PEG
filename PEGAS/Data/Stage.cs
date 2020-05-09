using System.Collections.Generic;
using PEGAS.Maths;

namespace PEGAS.Data
{
	public class Stage
	{
		public double Mode { get; set; }

		public double GLim { get; set; }

		public List<Engine> Engines { get; set; }

		public double MassTotal { get; set; }

		public double MaxT { get; set; }

		public double MassFuel { get; set; }
		public double MinThrottle { get; set; }

		public List<double> GetThrust()
		{
			var F = 0d;
			var dm = 0d;
			var isp = 0d;

			for (int i = 0; i < Engines.Count; i++)
			{
				isp = Engines[i].Isp;
				var dm_ = Engines[i].Flow;
				dm += dm_;
				F += isp * dm_ * Constants.G0;
			}

			isp = F / (dm * Constants.G0);

			return new List<double>
			{
				F, dm, isp
			};
		}
	}
}
