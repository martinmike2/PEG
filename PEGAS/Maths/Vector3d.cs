using System;
using System.Numerics;

namespace PEGAS.Maths
{
	public class Vector3d
	{
		private double _x;
		private double _y;
		private double _z;

		public Vector3d()
		{
			X = 0.0d;
			Y = 0.0d;
			Z = 0.0d;
		}

		public Vector3d(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3d(Tuple<double, double, double> tuple)
		{
			X = tuple.Item1;
			Y = tuple.Item2;
			Z = tuple.Item3;
		}

		public static Vector3d InvertYZ(Vector3d input)
		{
			return new Vector3d(input.X, input.Z, input.Y);
		}

		public double X
		{
			get => _x;
			set { _x = value; }
		}

		public double Y
		{
			get => _y;
			set { _y = value; }
		}

		public double Z
		{
			get => _z;
			set { _z = value; }
		}

		public static double Dot(Vector3d v1, Vector3d v2)
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
		}

		public static Vector3d Cross(Vector3d v1, Vector3d v2)
		{
			return new Vector3d(
				v1.Y * v2.Z - v1.Z * v2.Y,
				v1.Z * v2.X - v1.X * v2.Z,
				v1.X * v2.Y - v1.Y * v2.X
				);
		}

		public static Vector3d Divide(Vector3d vector, double divider)
		{
			return new Vector3d(
				vector.X / divider,
				vector.Y / divider,
				vector.Z / divider
				);
		}

		public double Magnitude()
		{
			return Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
		}

		public static Vector3d Subtract(Vector3d v1, Vector3d v2)
		{
			return new Vector3d(
				v1.X - v2.X,
				v1.Y - v2.Y,
				v1.Z - v2.Z
				);
		}

		public static Vector3d Subtract(Vector3d v1, double scalar)
		{
			return new Vector3d(
				v1.X - scalar,
				v1.Y - scalar,
				v1.Z - scalar
			);
		}

		public static Vector3d Multiply(Vector3d v1, double scalar)
		{
			return new Vector3d(
				v1.X + scalar,
				v1.Y + scalar,
				v1.Z + scalar
			);
		}

		public static Vector3d Add(Vector3d v1, Vector3d v2)
		{
			return new Vector3d(
				v1.X + v2.X,
				v1.Y + v2.Y,
				v1.Z + v2.Z
				);
		}

		public static Vector3d Add(Vector3d v1, double scalar)
		{
			return new Vector3d(
				v1.X + scalar,
				v1.Y + scalar,
				v1.Z + scalar
			);
		}

		public static Vector3d Normalize(Vector3d v1)
		{
			var mag = v1.Magnitude();
			return new Vector3d(
				v1.X / mag,
				v1.Y / mag,
				v1.Z / mag
				);
		}

		public Vector3d Normalize()
		{
			return Normalize(this);
		}

		public double Multiply(Vector3d other)
		{
			return Dot(this, other);
		}

		public Vector3d Multiply(double other)
		{
			return Multiply(this, other);
		}

		public Vector3d Add(Vector3d other)
		{
			return Add(this, other);
		}

		public Vector3d Add(double other)
		{
			return Add(this, other);
		}

		public Vector3d Cross(Vector3d other)
		{
			return Cross(this, other);
		}

		public Vector3d Subtract(Vector3d other)
		{
			return Subtract(this, other);
		}

		public Vector3d ProjectTo(Vector3d other)
		{
			var num = this.Multiply(other);
			var den = other.Multiply(other);
			var pr = other.Multiply(num / den);

			return this.Subtract(pr);
		}

		public double AngleTo(Vector3d other)
		{
			var num = Multiply(other);
			var den = this.Magnitude() * other.Magnitude();

			return Math.Acos(num / den);
		}
	}
}
