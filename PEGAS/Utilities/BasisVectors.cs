using PEGAS.Maths;

namespace PEGAS.Utilities
{
	public class BasisVectors
	{
		private Vector3d _rHat;
		private Vector3d _hVec;
		private Vector3d _hHat;
		private Vector3d _theta;
		private Vector3d _fHat;
		private double _angVel;
		private Vector3d _thetaHat;

		public Vector3d ThetaHat
		{
			get => _thetaHat;
			set => _thetaHat = value;
		}

		public Vector3d RHat
		{
			get => _rHat;
			set => _rHat = value;
		}

		public Vector3d HVec
		{
			get => _hVec;
			set => _hVec = value;
		}

		public Vector3d HHat
		{
			get => _hHat;
			set => _hHat = value;
		}

		public Vector3d Theta
		{
			get => _theta;
			set => _theta = value;
		}

		public Vector3d FHat
		{
			get => _fHat;
			set => _fHat = value;
		}

		public double AngVel
		{
			get => _angVel;
			set => _angVel = value;
		}

		public void ComputeRHat(Vector3d rVec, double radius)
		{
			RHat = Vector3d.Divide(rVec, radius);
		}

		public void ComputeHVec(Vector3d rVec, Vector3d vVec)
		{
			HVec = Vector3d.Cross(rVec, vVec);
		}

		public void ComputeHHat()
		{
			HHat = Vector3d.Divide(HVec, HVec.Magnitude());
		}

		public void ComputeTheta(Vector3d rVec)
		{
			Theta = Vector3d.Cross(HVec, rVec);
		}

		public void ComputeThetaHat(Vector3d rVec, Vector3d vVec)
		{
			ThetaHat = Vector3d.Divide(Theta, Theta.Magnitude());
		}

		public void ComputeAngularVelocity(Vector3d vVec, double radius)
		{

			AngVel = Vector3d.Dot(vVec, ThetaHat) / radius;
		}

		public void ComputeAll(Vector3d rVec, Vector3d vVec, double radius)
		{
			ComputeRHat(rVec, radius);
			ComputeHVec(rVec, vVec);
			ComputeHHat();
			ComputeTheta(rVec);
			ComputeAngularVelocity(vVec, radius);
		}
	}
}
