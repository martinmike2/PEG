using System.Collections.Generic;
using PEGAS.Maths;

namespace PEGAS.Data
{
	public class VesselState
	{
		private double time;
		private double? mass;
		private Vector3d radius;
		private Vector3d velocity;
		private Dictionary<string, object> cse;
		private double rbias;
		private Vector3d rd;
		private Vector3d rgrav;
		private Vector3d vgo;
		private double tb;
		private double tgo;


		public VesselState(double time, double? mass, Vector3d radius, Vector3d velocity, Dictionary<string, object> cse, double rbias, Vector3d rd, Vector3d rgrav, Vector3d vgo, double tb, double tgo)
		{
			this.time = time;
			this.mass = mass;
			this.radius = radius;
			this.velocity = velocity;
			this.cse = cse;
			this.rbias = rbias;
			this.rd = rd;
			this.rgrav = rgrav;
			this.vgo = vgo;
			this.tb = tb;
			this.tgo = tgo;
		}

		public double Tgo
		{
			get => tgo;
			set => tgo = value;
		}

		public double Tb
		{
			get => tb;
			set => tb = value;
		}

		public double Time
		{
			get => time;
			set => time = value;
		}

		public double? Mass
		{
			get => mass;
			set => mass = value.Value;
		}

		public Vector3d Radius
		{
			get => radius;
			set => radius = value;
		}

		public Vector3d Velocity
		{
			get => velocity;
			set => velocity = value;
		}

		public Dictionary<string, object> Cse
		{
			get => cse;
			set => cse = value;
		}

		public double Rbias
		{
			get => rbias;
			set => rbias = value;
		}

		public Vector3d Rd
		{
			get => rd;
			set => rd = value;
		}

		public Vector3d Rgrav
		{
			get => rgrav;
			set => rgrav = value;
		}

		public Vector3d Vgo
		{
			get => vgo;
			set => vgo = value;
		}
	}
}
