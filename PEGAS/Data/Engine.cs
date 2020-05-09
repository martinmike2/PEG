namespace PEGAS.Data
{
	public class Engine
	{
		private double isp;
		private double flow;

		public double Isp
		{
			get => isp;
			set => isp = value;
		}

		public double Flow
		{
			get => flow;
			set => flow = value;
		}
	}
}
