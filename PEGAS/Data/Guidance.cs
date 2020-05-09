using PEGAS.Maths;

namespace PEGAS.Data
{
	public class Guidance
	{
		private Vector3d vector;
		private double pitch;
		private double yaw;
		private double pitchdot;
		private double yawdot;
		private double tgo;


		public Guidance(Vector3d vector, double pitch, double yaw, double pitchdot, double yawdot, double tgo)
		{
			this.vector = vector;
			this.pitch = pitch;
			this.yaw = yaw;
			this.pitchdot = pitchdot;
			this.yawdot = yawdot;
			this.tgo = tgo;
		}

		public Vector3d Vector
		{
			get => vector;
			set => vector = value;
		}

		public double Pitch
		{
			get => pitch;
			set => pitch = value;
		}

		public double Yaw
		{
			get => yaw;
			set => yaw = value;
		}

		public double Pitchdot
		{
			get => pitchdot;
			set => pitchdot = value;
		}

		public double Yawdot
		{
			get => yawdot;
			set => yawdot = value;
		}

		public double Tgo
		{
			get => tgo;
			set => tgo = value;
		}
	}
}
