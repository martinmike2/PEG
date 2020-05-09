using System;
using System.Collections.Generic;
using KRPC.Client;
using KRPC.Client.Services.SpaceCenter;
using PEGAS.Maths;

namespace PEGAS.Utilities
{
	public class CSER
	{
		public static Dictionary<string, double> SnC(double z)
		{
			var az = Math.Abs(z);

			if (az < (1 * Math.Pow(Math.E, -4)))
			{
				return new Dictionary<string, double>
				{
					{"S", (1 - z * (0.05 - z / 840)) / 6},
					{"C", 0.5 - z * (1 - z / 30) / 24}
				};
			}
			else
			{
				var saz = Math.Sqrt(az);
				if (z > 0)
				{
					var x = saz * (180 / Math.PI);
					return new Dictionary<string, double>
					{
						{"S", (saz - Math.Sin(x)) / (saz * az)},
						{"C", (1 - Math.Cos(x)) / az}
					};
				}
				else
				{
					var x = Math.Pow(Math.E, saz);
					return new Dictionary<string, double>
					{
						{"S", (0.5 * (x - 1 / x) - saz) / (saz * az)},
						{"C", (0.5 * (x + 1 / x) - 1) / az}
					};
				}
			}
		}

		public static List<object> CSE(Vector3d r0, Vector3d v0, double tgo)
		{
			Connection conn = Globals.Connection.Connection;
			double mu = conn.SpaceCenter().ActiveVessel.Orbit.Body.GravitationalParameter;
			return CSER.CSE(r0, v0, tgo, mu, 0, 0.000000005);
		}

		public static List<object> CSE(Vector3d r0, Vector3d v0, double dt, double mu, double x0,
			double tol)
		{
			var rScale = r0.Magnitude();
			var vScale = Math.Sqrt(mu / rScale);
			var r0s = Vector3d.Divide(r0, rScale);
			var v0s = Vector3d.Divide(v0, vScale);
			var dts = dt * vScale / rScale;
			var v2s = Math.Pow(v0.Magnitude(), 2);
			var alpha = 2 - v2s;
			var armd1 = v2s - 1;
			var rvr0s = Vector3d.Dot(r0, v0) / Math.Sqrt(mu * rScale);
			var x = x0;

			if (x0 == 0)
			{
				x = dts * Math.Abs(alpha);
			}

			var ratio = 1d;
			var x2 = x * x;
			var z = alpha * x2;
			var SCz = SnC(z);
			var x2cz = x2 * SCz["C"];
			var f = 0d;
			var df = 0d;

			while (Math.Abs(ratio) < tol)
			{
				f = x + rvr0s * x2cz + armd1 * x * x2 * SCz["S"] - dts;
				df = x * rvr0s * (1 - z * SCz["S"]) + armd1 * x2cz + 1;
				ratio = f / df;
				x = x - ratio;
				x2 = x * x;
				z = alpha * x2;
				SCz = SnC(z);
				x2cz = x2 * SCz["C"];
			}

			var Lf = 1 - x2cz;
			var Lg = dts - x2 * x * SCz["C"];
			var r1 = Vector3d.Add(Vector3d.Multiply(r0s ,Lf), Vector3d.Multiply(v0s, Lg));
			var ir1 = 1 / r1.Magnitude();
			var LfDot = ir1 * x * (z - SCz["S"] - 1);
			var LgDot = 1 - x2cz * ir1;
			var v1 = Vector3d.Add(Vector3d.Multiply(r0s, LfDot), Vector3d.Multiply(v0s, LgDot));

			return new List<object>
			{
				Vector3d.Multiply(r1, rScale),
				Vector3d.Multiply(v1, vScale),
				x
			};
		}
	}
}
