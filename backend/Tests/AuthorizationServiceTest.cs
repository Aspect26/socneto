using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Socneto.Domain.Models.Storage.Response;
using Socneto.Domain.Services;

namespace Tests
{
    [TestClass]
    public class AuthorizationServiceTest
    {
        [TestMethod]
        public async Task IsAuthorizedTest()
        {
            const string mockUserName = "Aragorn";
            var mockGuid = Guid.NewGuid();
            var mockUsersJob = new Job {JobId = mockGuid, Username = mockUserName};
            
            var mockStorageService = new Mock<IStorageService>();
            mockStorageService.Setup(o => o.GetJob(Guid.Empty)).Returns(Task.FromResult(mockUsersJob));
            
            var authorizationService = new AuthorizationService(mockStorageService.Object);

            var result = await authorizationService.IsUserAuthorizedToSeeJob(mockUserName, Guid.Empty);
            
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task IsNotAuthorizedTest()
        {
            const string mockUserName = "Aragorn";
            const string otherMockUserName = "Legolas";
            var mockGuid = Guid.NewGuid();
            var mockUsersJob = new Job {JobId = mockGuid, Username = mockUserName};
            
            var mockStorageService = new Mock<IStorageService>();
            mockStorageService.Setup(o => o.GetJob(Guid.Empty)).Returns(Task.FromResult(mockUsersJob));
            
            var authorizationService = new AuthorizationService(mockStorageService.Object);

            var result = await authorizationService.IsUserAuthorizedToSeeJob(otherMockUserName, Guid.Empty);
            
            Assert.AreEqual(false, result);
        }
    }
}