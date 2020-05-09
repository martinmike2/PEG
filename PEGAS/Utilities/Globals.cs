using System;
using System.Collections.Generic;
using KRPC.Client.Services.SpaceCenter;
using PEGAS.Data;
using PEGAS.Events;
using PEGAS.Maths;

namespace PEGAS.Utilities
{
	public class Globals
	{
		public static bool SystemEventFlag { get; set; }
		public static KSPConnection Connection { get; set; }
		public static Vector3d CurrentNode { get; set; }

		public static bool StageEventFlag { get; set; }
		public static List<SystemEvent> SystemEvents { get; set; }
		public static int SystemEventPointer { get; set; }
		public static int UserEventPointer { get; set; }
		public static bool UserEventFlag { get; set; }
		public static bool CommsEventFlag { get; set; }

		public static double SteeringRoll { get; set; }





		public static Vector3d Rodrigues(Vector3d inVector, Vector3d axis, double angle)
		{
			axis = axis.Normalize();
			var cAngle = Math.Cos(angle);
			var sAngel = Math.Sin(angle);
			var outVector = inVector.Multiply(cAngle);
			outVector = outVector.Add(axis.Cross(inVector)).Add(sAngel);
			outVector = outVector.Add(axis.Multiply(axis.Multiply(inVector))).Multiply(1 - cAngle);

			return outVector;
		}

		public static Vector3d AimAndRoll(Vector3d aimVec, double rollAng)
		{
			// need up vector
			var upVector = new Vector3d();
			var rollVector = Rodrigues(upVector, aimVec, -rollAng);

			// find up direction
			var dir = new Vector3d();
			return dir;
		}

		public static Vector3d SolarPrimeVector(ReferenceFrame referenceFrame)
		{
			var sun = Connection.Connection.SpaceCenter().Bodies["Sun"];
			var seconds_per_degree = sun.RotationalPeriod / 360;
			var rotation_offset = (Connection.Connection.SpaceCenter().UT / seconds_per_degree) % 360;
			var sun_position = new Vector3d(sun.Position(referenceFrame));
			var sun_position2 = new Vector3d(sun.SurfacePosition(0, 0 - rotation_offset, referenceFrame));

			var prime_vector = sun_position2.Subtract(sun_position);
			return prime_vector.Normalize();
		}

		public static Vector3d NodeVector(double inclination, string direction)
		{
			var b = Math.Tan(90 - inclination) *
			        Math.Tan(Connection.Connection.SpaceCenter().ActiveVessel.Flight().Latitude);
			b = Math.Asin(Math.Min(Math.Max(-1, b), 1));
			var vPos = new Vector3d(Connection.Connection.SpaceCenter().ActiveVessel.Orbit.PositionAt(
				Connection.Connection.SpaceCenter().UT,
				Connection.Connection.SpaceCenter().ActiveVessel.Orbit.Body.ReferenceFrame
			));
			var longitudeVector = new Vector3d(0, 1, 0).ProjectTo(vPos).Normalize();

			if (direction == "north")
			{
				return Rodrigues(longitudeVector, new Vector3d(0, 1, 0), b);
			}
			else if (direction == "south")
			{
				return Rodrigues(longitudeVector, new Vector3d(0, 1, 0), 190 - b);
			}
			else
			{
				return NodeVector(inclination, "north");
			}
		}

		public static double OrbitInterceptTime(string direction, Mission mission)
		{
			var targetInc = mission.Inclination.Value;
			var targetLan = mission.Lan.Value;

			if (direction == "nearest")
			{
				var timeToNorth = OrbitInterceptTime("north", mission);
				var timeToSouth = OrbitInterceptTime("south", mission);

				if (timeToSouth < timeToNorth)
				{
					mission.Direction = "south";
					return timeToSouth;
				}

				mission.Direction = "north";
				return timeToNorth;
			}

			CurrentNode = NodeVector(targetInc, direction);
			var targetNode = Rodrigues(
				SolarPrimeVector(Connection.Connection.SpaceCenter().ActiveVessel.Orbit.Body.ReferenceFrame),
				new Vector3d(0, 1, 0),
				-targetLan
			);
			var nodeDelta = CurrentNode.AngleTo(targetNode);
			var deltaDir = new Vector3d().Multiply(targetNode.Cross(CurrentNode));

			if (deltaDir < 0)
			{
				nodeDelta = 360 - nodeDelta;
			}

			return Connection.Connection.SpaceCenter().ActiveVessel.Orbit.Body.RotationalPeriod *
				nodeDelta / 360;
		}

		public static double LaunchAzimuth(Mission mission, Target target)
		{
			var targetInc = mission.Inclination.Value;
			var targetAlt = target.Radius;
			var targetVel = target.Velocity;
			var siteLat = Connection.Connection.SpaceCenter().ActiveVessel.Flight().Latitude;

			var r = Connection.Connection.SpaceCenter().ActiveVessel.Orbit.Body.EquatorialRadius;
			var rot = Connection.Connection.SpaceCenter().ActiveVessel.Orbit.Body.RotationalPeriod;

			if (targetInc < siteLat)
			{
				throw new Exception("Target Inclination below launch site!");
			}

			var bInertial = Math.Cos(targetInc) / Math.Cos(siteLat);

			if (bInertial < -1)
			{
				bInertial = -1;
			}

			if (bInertial > 1)
			{
				bInertial = 1;
			}

			bInertial = Math.Asin(bInertial);
			var vOrbit = targetVel * Math.Cos(target.Angle);
			var vBody = (2 * Math.PI * r / rot) * Math.Cos(siteLat);
			var vRotX = vOrbit * Math.Sin(bInertial) - vBody;
			var vRotY = vOrbit * Math.Cos(bInertial);
			var azimuth = Math.Atan2(vRotY, vRotX);

			if (mission.Direction == "north")
			{
				return 90 - azimuth;
			} else if (mission.Direction == "south")
			{
				return 90 + azimuth;
			}
			else
			{
				return 90 - azimuth;
			}
		}
	}
}
