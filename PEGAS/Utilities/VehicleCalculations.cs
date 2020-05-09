using System;
using System.Runtime.CompilerServices;
using PEGAS.Maths;

namespace PEGAS.Utilities
{
	public class VehicleCalculations
	{
		public static double ConvertISPtoVE(double isp)
		{
			return isp * Constants.G0;
		}

		public static double CalculateDeltaV(double isp, double initialMass, double finalMass)
		{
			return ConvertISPtoVE(isp) * Math.Log(initialMass / finalMass);
		}
	}
}
