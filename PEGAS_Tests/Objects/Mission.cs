using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PEGAS_Tests.Objects
{
	[TestFixture]
	public class Mission
	{
		string Mission1 = "{\"Payload\": 100,\"Apoapsis\": 400,\"Periapsis\": 400,\"Altitude\": 400,\"Inclination\": 98.7,\"Lan\": 0,\"Direction\": \"nearest\" }";

		[Test]
		public void CreateMission()
		{
			var mission = JsonSerializer.Deserialize<PEGAS.Data.Mission>(Mission1);

			Assert.AreEqual(100, mission.Payload.Value);
			Assert.AreEqual(400, mission.Apoapsis);
			Assert.AreEqual(400, mission.Periapsis);
			Assert.AreEqual(400, mission.Altitude.Value);
			Assert.AreEqual(98.7, mission.Inclination.Value);
			Assert.AreEqual(0, mission.Lan.Value);
			Assert.AreEqual("nearest", mission.Direction);
		}
	}
}
