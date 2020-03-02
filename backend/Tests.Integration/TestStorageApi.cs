using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Integration
{
    [TestClass]
    public class TestStorageApi
    {
        [TestMethod]
        public void TestAuthorizationServiceWithStorage()
        {
            const string mockUserName = "Aragorn";
            var mockGuid = Guid.NewGuid();
            var mockUsersJob = new Job {JobId = mockGuid, Username = mockUserName};
            
            var storageService = new Mock<IStorageService>();
            mockStorageService.Setup(o => o.GetJob(Guid.Empty)).Returns(Task.FromResult(mockUsersJob));
            
            var authorizationService = new AuthorizationService(mockStorageService.Object);

            var result = await authorizationService.IsUserAuthorizedToSeeJob(mockUserName, Guid.Empty);
            
            Assert.AreEqual(true, result);
        }
    }
}