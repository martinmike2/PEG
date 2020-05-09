using System;
using KRPC.Client;
using KRPC.Client.Services.SpaceCenter;
using PEGAS.Maths;
using PEGAS.Utilities;

namespace PEGAS.Data
{
	public class Mission
	{
		public double? Payload { get; set; }

		public double Apoapsis { get; set; }

		public double Periapsis { get; set; }

		public double? Altitude { get; set; }

		public double? Inclination { get; set; }

		public double? Lan { get; set; }

		public string Direction { get; set; }

		public void FinishSetup(Connection connection, Controls controls)
		{
			if (Altitude.HasValue)
			{
				if (Altitude < Periapsis || Altitude > Apoapsis)
				{
					Altitude = Periapsis;
				}
			}
			else
			{
				Altitude = Periapsis;
			}

			if (Direction != null)
			{
				Direction = "nearest";
			}

			if (Inclination.HasValue)
			{
				while (Inclination.Value < -180)
				{
					Inclination = Inclination.Value + 360;
				}

				while (Inclination.Value > 180)
				{
					Inclination = Inclination.Value - 360;
				}
			}
			else
			{
				if (Direction == "nearest")
				{
					Direction = "north";
				}

				Vector3d currentNode = Globals.NodeVector(Inclination.Value, Direction);
				var spv = Globals.SolarPrimeVector(connection.SpaceCenter().ActiveVessel.Orbit.Body.ReferenceFrame);
				var currentLan = Math.Acos(currentNode.Multiply(spv) / (currentNode.Magnitude() * spv.Magnitude()));

				if (Vector3d.Dot(new Vector3d(0, 1, 0), currentNode.Cross(spv)) < 0)
				{
					currentLan = 360 - currentLan;
				}

				Lan = currentLan + (controls.LaunchTimeAdvance + 30) /
					connection.SpaceCenter().ActiveVessel.Orbit.Body.RotationalPeriod * 360;
			}
		}
	}
}
