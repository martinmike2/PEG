using System;
using KRPC.Client;
using KRPC.Client.Services.SpaceCenter;
using PEGAS.Maths;

namespace PEGAS.Data
{
	public class Target
	{
		private double angle;
		private Vector3d normal;
		private double radius;
		private double velocity;

		public Target(double angle, Vector3d normal, double radius, double velocity)
		{
			this.angle = angle;
			this.normal = normal;
			this.radius = radius;
			this.velocity = velocity;
		}


		public double Angle
		{
			get => angle;
			set => angle = value;
		}

		public Vector3d Normal
		{
			get => normal;
			set => normal = value;
		}

		public double Radius
		{
			get => radius;
			set => radius = value;
		}

		public double Velocity
		{
			get => velocity;
			set => velocity = value;
		}

		public void FinishSetup(Connection connection, Mission mission)
		{
			var r = connection.SpaceCenter().ActiveVessel.Orbit.Body.EquatorialRadius;
			var mu = connection.SpaceCenter().ActiveVessel.Orbit.Body.GravitationalParameter;
			var pe = mission.Periapsis * 1000 + r;
			var ap = mission.Apoapsis * 1000 + r;
			Radius = mission.Altitude.Value * 1000 + r;
			var sma = (pe + ap) / 2;
			var vpe = Math.Sqrt(mu * (2 / pe - 1 / sma));
			var srm = pe * vpe;
			Velocity = Math.Sqrt(mu * (2 / Radius - 1 / sma));
			var flightPathAngle = Math.Acos(srm / (Velocity * Radius));
			Angle = flightPathAngle;
			Normal = new Vector3d();
		}
	}
}
