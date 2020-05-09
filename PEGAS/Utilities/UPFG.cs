using System;
using System.Collections.Generic;
using PEGAS.Data;
using PEGAS.Maths;

namespace PEGAS.Utilities
{
	public class UPFG
	{
		public static bool UpfgConverged { get; set; }
		public static int UpfgStage { get; set; }
		public static double UPFGConvergenceDelay = 5;
		public static double UPFGFinalizationTime = 5;
		public static double UPFGConvergenceCriterion = 0.1;
		public static double UPFGGoodSolutionCriterion = 15;

		public static List<object> Iterate(Vehicle vehicle, Target target, VesselState state, VesselState previous)
		{
			var gamma = target.Angle;
			var iy = target.Normal;
			var rdval = target.Radius;
			var vdval = target.Velocity;

			var t = state.Time;
			var m = state.Mass.Value;
			var r = state.Radius;
			var v = state.Velocity;

			var cser = previous.Cse;
			var rbias = previous.Rbias;
			var rd = previous.Rd;
			var rgrav = previous.Rgrav;
			var tp = previous.Time;
			var vprev = previous.Velocity;
			var vgo = previous.Vgo;

			var n = vehicle.Stages.Count;
			var SM = new List<double>();
			var aL = new List<double>();
			var md = new List<double>();
			var ve = new List<double>();
			var fT = new List<double>();
			var aT = new List<double>();
			var tu = new List<double>();
			var tb = new List<double>();

			for (int i = 0; i < n; i++)
			{
				Stage stg = vehicle.Stages[i];
				var pack = stg.GetThrust();
				SM.Add(stg.Mode);
				aL.Add(stg.GLim * Constants.G0);
				fT.Add(pack[0]);
				md.Add(pack[1]);
				ve.Add(pack[2] * Constants.G0);
				aT.Add(fT[i] / stg.MassTotal);
				tu.Add(ve[i] / aT[i]);
				tb.Add(stg.MaxT);
			}


			var dt = t - tp;
			var dvsensed = Vector3d.Subtract(v, vprev);
			vgo = Vector3d.Subtract(vgo, dvsensed);
			tb[0] = tb[0] - previous.Tb;

			if (SM[0] == 1)
			{
				aT[0] = fT[0] / m;
			}
			else if (SM[0] == 2)
			{
				aT[0] = aL[0];
			}

			tu[0] = ve[0] / aT[0];
			var L = 0d;
			var Li = new List<double>();

			for (int i = 0; i < n - 1; i++)
			{
				if (SM[i] == 1)
				{
					Li.Add(ve[i] * Math.Log(tu[i] / (tu[i] - tb[i])));
				}
				else if (SM[i] == 2)
				{
					Li.Add(aL[i] * tb[i]);
				}
				else
				{
					Li.Add(0);
				}

				L = Li[i];

				if (L > vgo.Magnitude())
				{
					var veh = vehicle;
					veh.Stages.RemoveAt(veh.Stages.Count - 1);

					return UPFG.Iterate(veh, target, state, previous);
				}
			}

			Li.Add(vgo.Magnitude() - L);

			var tgoi = new List<double>();

			for (int i = 0; i < n; i++)
			{
				if (SM[i] == 1)
				{
					tb[i] = tu[i] * (1 - Math.Pow(Math.E, (-Li[i] / ve[i])));
				}
				else if (SM[i] == 2)
				{
					tb[i] = Li[i] / aL[i];
				}

				if (i == 0)
				{
					tgoi.Add(tb[i]);
				}
				else
				{
					tgoi.Add(tgoi[i - 1] + tb[i]);
				}
			}

			var L1 = Li[0];
			var tgo = tgoi[n - 1];

			L = 0d;
			var J = 0d;
			var S = 0d;
			var Q = 0d;
			var H = 0d;
			var P = 0d;

			var Ji = new List<double>();
			var Si = new List<double>();
			var Qi = new List<double>();
			var Pi = new List<double>();
			var tgoi1 = 0d;

			for (int i = 0; i < n; i++)
			{
				if (i > 0)
				{
					tgoi1 = tgoi[i - 1];
				}

				if (SM[i] == 1)
				{
					Ji.Add(tu[i] * Li[i] - ve[i] * tb[i]);
					Si.Add(-Ji[i] + tb[i] * Li[i]);
					Qi.Add(Si[i] * (tu[i] + tgoi1) - 0.5 * ve[i] * Math.Pow(tb[i], 2));
					Pi.Add(Qi[i] * (tu[i] + tgoi1) - 0.5 * ve[i] * Math.Pow(tb[i], 2) * (tb[i] / 3 + tgoi1));
				}
				else if (SM[i] == 2)
				{
					Ji.Add(0.5 * Li[i] * tb[i]);
					Si.Add(Ji[i]);
					Qi.Add(Si[i] * (tb[i] / 3 + tgoi1));
					Pi.Add((1 / 6) * Si[i] * (Math.Pow(tgoi[i], 2) + 2 * tgoi[i] * tgoi1 + 3 * Math.Pow(tgoi1, 2)));
				}

				Ji[i] = Ji[i] + Li[i] * tgoi1;
				Si[i] = Si[i] + L * tb[i];
				Qi[i] = Qi[i] + J * tb[i];
				Pi[i] = Pi[i] + H * tb[i];

				L += Li[i];
				J += Ji[i];
				S += Si[i];
				Q += Qi[i];
				P += Pi[i];
				H = J * tgoi[i] - Q;
			}

			var lambda = Vector3d.Normalize(vgo);

			if (previous.Tgo > 0)
			{
				rgrav = Vector3d.Multiply(rgrav, Math.Pow((tgo / previous.Tgo), 2));
			}

			var rgo = Vector3d.Subtract(
				rd,
				Vector3d.Add(
					Vector3d.Multiply(
						Vector3d.Add(r, v),
						tgo
					),
					rgrav
				)
			);


			var iz = Vector3d.Normalize(Vector3d.Cross(rd, iy));
			var rgoxy = Vector3d.Dot(Vector3d.Subtract(rgo, Vector3d.Dot(iz, rgo)), iz);
			var rgoz = Vector3d.Divide(Vector3d.Subtract(Vector3d.Multiply(lambda, rgoxy), S),
				Vector3d.Dot(lambda, iz));
			rgo = Vector3d.Add(Vector3d.Add(rgoz, rgoxy), Vector3d.Add(iz, rbias));

			var lambdade = Q - S * J / L;
			var lambdadot = Vector3d.Divide(Vector3d.Subtract(rgo, Vector3d.Multiply(lambda, S)), lambdade);
			var iF_ = Vector3d.Subtract(lambda, Vector3d.Multiply(lambdadot, J / L));
			iF_ = Vector3d.Normalize(iF_);
			var phi = Math.Acos(Vector3d.Dot(iF_, lambda) / (iF_.Magnitude() * lambda.Magnitude()));
			var phidot = -phi * L / J;
			var vthrust = Vector3d.Multiply(lambda,
				L - 0.5 * L * Math.Pow(phi, 2) - J * phi * phidot - 0.5 * Math.Pow(phidot, 2));
			var rthrst = S - 0.5 * S * Math.Pow(phi, 2) - Q * phi * phidot - 0.5 * P * Math.Pow(phidot, 2);

			var rthrust = Vector3d.Subtract(
				Vector3d.Multiply(lambda, rthrst),
				Vector3d.Multiply(
					Vector3d.Normalize(lambda),
					(S * phi + Q * phidot)
				)
			);

			var vbias = Vector3d.Subtract(vgo, vthrust);
			var rbs = Vector3d.Subtract(rgo, rthrst);

			var _up = Vector3d.Normalize(r);
			var _east = Vector3d.Normalize(Vector3d.Cross(
				new Vector3d(0, 0, 1),
				_up
			));
			var pitch = Math.Acos(Vector3d.Dot(iF_, _up) / (iF_.Magnitude() * _up.Magnitude()));
			var inplane = Vector3d.Subtract(
				iF_,
				Vector3d.Multiply(_up,
					Vector3d.Dot(iF_, _up) / Vector3d.Dot(_up, _up)
				));

			var yaw = Math.Acos(Vector3d.Dot(inplane, _east) / (inplane.Magnitude() * _east.Magnitude()));
			var tangent = Vector3d.Cross(_up, _east);

			if (Vector3d.Dot(inplane, tangent) < 0)
			{
				yaw = -yaw;
			}

			var rc1 = Vector3d.Subtract(Vector3d.Subtract(
					r,
					Vector3d.Multiply(rthrust, 0.1)
				),
				Vector3d.Multiply(
					vthrust,
					tgo / 30
				));

			var vc1 = Vector3d.Subtract(Vector3d.Add(
					Vector3d.Divide(Vector3d.Multiply(rthrust, 1.2), tgo),
					v
				),
				Vector3d.Multiply(vthrust, 0.1));

			var pck = CSER.CSE(rc1, vc1, tgo);
			cser = (Dictionary<string, object>) pck[2];
			var rgrv = Vector3d.Subtract(
				(Vector3d) pck[0],
				Vector3d.Subtract(
					rc1,
					Vector3d.Multiply(vc1, tgo)
				)
			);

			var vgrv = Vector3d.Subtract(
				(Vector3d) pck[1],
				vc1
			);

			var rp = Vector3d.Add(
				r,
				Vector3d.Add(
					Vector3d.Multiply(v, tgo),
					Vector3d.Add(rgrv, rthrust)
				)
			);

			rp = Vector3d.Subtract(
				rp,
				Vector3d.Multiply(
					iy,
					Vector3d.Dot(rp, iy)
				)
			);

			rd = Vector3d.Multiply(
				Vector3d.Normalize(rp),
				rdval
			);

			var ix = Vector3d.Normalize(rd);
			iz = Vector3d.Cross(ix, iy);

			var vv1 = new Vector3d(ix.X, iy.X, iz.Z);
			var vv2 = new Vector3d(ix.Y, iy.Y, iz.Y);
			var vv3 = new Vector3d(ix.Z, iy.Z, iz.Z);
			var vop = new Vector3d(Math.Sin(gamma), 0, Math.Cos(gamma));
			var vd = new Vector3d(
				Vector3d.Dot(vv1, vop),
				Vector3d.Dot(vv2, vop),
				Vector3d.Dot(vv3, vop)
			);
			vgo = Vector3d.Add(Vector3d.Subtract(
					vd,
					Vector3d.Subtract(
						vgrv,
						v
					)
				),
				vbias
			);

			// var current = new VesselState(cser);
			var current = new VesselState(t, null, null, v, cser, rbias, rd, rgrv, vgo, previous.Tb + dt, tgo);
			var guidance = new Guidance(iF_, pitch, yaw, 0, 0, tgo);

			return new List<object>
			{
				current,
				guidance,
				dt
			};
		}
	}
}
