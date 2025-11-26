using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace RailwayKiosk.Tests
{
    [TestClass]
    public class TrainServiceTests
    {
        [TestMethod]
        public void Filter_ByDestinationAndTypeAndTime_Works()
        {
            var now = DateTime.Now;
            var data = new List<Train>
            {
                new Train { TrainNumber = "101", Destination = "Lahore", TrainType = "Express", DepartureTime = now, Status = "on time" },
                new Train { TrainNumber = "102", Destination = "Karachi", TrainType = "Local", DepartureTime = now.AddHours(3), Status = "delayed" },
                new Train { TrainNumber = "103", Destination = "Lahore", TrainType = "Local", DepartureTime = now.AddMinutes(30), Status = "on time" }
            };
            var result = TrainService.Filter(data, null, "Lahore", "Local", now);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("103", result[0].TrainNumber);
        }

        [TestMethod]
        public void Filter_ByNumber_Substring_Matches()
        {
            var now = DateTime.Now;
            var data = new List<Train>
            {
                new Train { TrainNumber = "555A", Destination = "Rawalpindi", TrainType = "Express", DepartureTime = now },
                new Train { TrainNumber = "123", Destination = "Multan", TrainType = "Local", DepartureTime = now }
            };
            var result = TrainService.Filter(data, "55", "All", "All", now);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("555A", result[0].TrainNumber);
        }
    }
}
