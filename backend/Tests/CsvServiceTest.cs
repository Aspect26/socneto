using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Socneto.Domain.Services;

namespace Tests
{
    internal class MockModel
    {
        public const string IdentifierHeader = "identifier";
        public const string CountHeader = "count";
        
        [JsonProperty(IdentifierHeader)]
        public string Identifier { get; set; }
        
        [JsonProperty(CountHeader)]
        public int Count { get; set; }
    }
    
    [TestClass]
    public class CsvServiceTest
    {
        [TestMethod]
        public void TestGetEmptyWithoutHeaders()
        {
            var csvService = new CsvService();
            
            var resultCsv = csvService.GetCsv(new List<MockModel>(), false);
            var expectedCsv = string.Empty;
            
            Assert.AreEqual(expectedCsv, resultCsv, "CSV for empty list without headers should be empty string");
        }
        
        [TestMethod]
        public void TestGetEmptyWithHeaders()
        {
            var csvService = new CsvService();
            
            var resultCsv = csvService.GetCsv(new List<MockModel>(), true);
            var expectedCsv = $"{MockModel.IdentifierHeader},{MockModel.CountHeader}\n";
            
            Assert.AreEqual(expectedCsv, resultCsv, "CSV for empty list with headers should contain the headers");
        }
        
        [TestMethod]
        public void TestNotEmptyWithoutHeaders()
        {
            var csvService = new CsvService();
            
            var data = new List<MockModel>()
            {
                new MockModel{ Identifier = "Wrathchild", Count = 25 },
                new MockModel{ Identifier = "The Number of the Beast", Count = 666 },
                new MockModel{ Identifier = "The Prisoner", Count = 6 },
            };
            
            var resultCsv = csvService.GetCsv(data, false);
            var expectedCsv = $"{data[0].Identifier},{data[0].Count}\n" +
                              $"{data[1].Identifier},{data[1].Count}\n" +
                              $"{data[2].Identifier},{data[2].Count}\n";
            
            Assert.AreEqual(expectedCsv, resultCsv, "Incorrectly encoded data objects to CSV");
        }
        
        [TestMethod]
        public void TestNotEmptyWithHeaders()
        {
            var csvService = new CsvService();
            
            var data = new List<MockModel>()
            {
                new MockModel{ Identifier = "Wrathchild", Count = 25 },
                new MockModel{ Identifier = "The Number of the Beast", Count = 666 },
                new MockModel{ Identifier = "The Prisoner", Count = 6 },
            };
            
            var resultCsv = csvService.GetCsv(data, true);
            var expectedCsv = $"{MockModel.IdentifierHeader},{MockModel.CountHeader}\n" +
                              $"{data[0].Identifier},{data[0].Count}\n" +
                              $"{data[1].Identifier},{data[1].Count}\n" +
                              $"{data[2].Identifier},{data[2].Count}\n";
            
            Assert.AreEqual(expectedCsv, resultCsv, "Incorrectly encoded data objects to CSV");
        }
    }
}