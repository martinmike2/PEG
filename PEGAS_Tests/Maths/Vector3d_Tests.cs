using NUnit.Framework;
using PEGAS.Maths;

namespace PEGAS_Tests.Maths
{
	[TestFixture]
	public class Vector3d_Tests
	{
		[Test]
		public void TestIdentityConstructor()
		{
			Vector3d _vector3d = new Vector3d();

			Assert.AreEqual(0, _vector3d.X);
			Assert.AreEqual(0, _vector3d.Y);
			Assert.AreEqual(0, _vector3d.Z);
		}

		[Test]
		public void TestExplicitConstrucotr()
		{
			Vector3d _vector3d = new Vector3d(1d, 2d, 3d);
			Assert.AreEqual(1, _vector3d.X);
			Assert.AreEqual(2, _vector3d.Y);
			Assert.AreEqual(3, _vector3d.Z);
		}

		[Test]
		public void DotProduct()
		{
			Vector3d v1 = new Vector3d(1, 2, 3);
			Vector3d v2 = new Vector3d(1, 2, 3);
			double dot = Vector3d.Dot(v1, v2);

			Assert.AreEqual(14, dot);
		}

		[Test]
		public void CrossProduct()
		{
			Vector3d v1 = new Vector3d(1, 1, 1);
			Vector3d v2 = new Vector3d(1, 1, 1);
			Vector3d v3 = Vector3d.Cross(v1, v2);

			Assert.AreEqual(0, v3.X);
			Assert.AreEqual(0, v3.Y);
			Assert.AreEqual(0, v3.Z);

			v1 = new Vector3d(2, 4, 6);
			v2 = new Vector3d(8, 10, 12);
			v3 = Vector3d.Cross(v1, v2);

			Assert.AreEqual(-12, v3.X);
			Assert.AreEqual(24, v3.Y);
			Assert.AreEqual(-12, v3.Z);
		}

		[Test]
		public void DivideVectorByScalar()
		{
			Vector3d v1 = new Vector3d(2, 2, 2);
			Vector3d v2 = Vector3d.Divide(v1, 2d);

			Assert.AreEqual(1, v2.X);
			Assert.AreEqual(1, v2.Y);
			Assert.AreEqual(1, v2.Z);

			v1 = new Vector3d(4, 6, 8);
			v2 = Vector3d.Divide(v1, 2);

			Assert.AreEqual(2, v2.X);
			Assert.AreEqual(3, v2.Y);
			Assert.AreEqual(4, v2.Z);
		}

		[Test]
		public void Magnitude()
		{
			Vector3d v1 = new Vector3d(1, 2, 3);
			double mag = v1.Magnitude();

			Assert.AreEqual(3.7416573867739413, mag);
		}
	}
}
