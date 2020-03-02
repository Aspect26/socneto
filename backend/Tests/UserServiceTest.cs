using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Socneto.Domain.Models.Storage.Response;
using Socneto.Domain.Services;

namespace Tests
{
    [TestClass]
    public class UserServiceTest
    {
        [TestMethod]
        public async Task IsAuthenticatedTest()
        {
            const string mockUserName = "Gandalf";
            const string mockPassword = "FlameOfAnor";
            var mockUser = new User {Username = mockUserName, Password = mockPassword};

            var mockStorageService = new Mock<IStorageService>();
            mockStorageService.Setup(o => o.GetUser(mockUserName)).Returns(Task.FromResult(mockUser));

            var userService = new UserService(mockStorageService.Object);

            var result = await userService.Authenticate(mockUserName, mockPassword);

            Assert.AreEqual(mockUser, result);
        }
        
        [TestMethod]
        public async Task DoesNotExistTest()
        {
            const string mockUserName = "Gandalf";
            const string mockOtherUserName = "Saruman";

            var mockStorageService = new Mock<IStorageService>();
            mockStorageService.Setup(o => o.GetUser(mockUserName)).Returns(Task.FromResult<User>(null));

            var userService = new UserService(mockStorageService.Object);

            var result = await userService.Authenticate(mockOtherUserName, "");

            Assert.AreEqual(null, result);
        }
        
        [TestMethod]
        public async Task WrongPasswordTest()
        {
            const string mockUserName = "Gandalf";
            const string mockPassword = "FlameOfAnor";
            const string mockOtherPassword = "FlameOfUdun";
            var mockUser = new User {Username = mockUserName, Password = mockPassword};

            var mockStorageService = new Mock<IStorageService>();
            mockStorageService.Setup(o => o.GetUser(mockUserName)).Returns(Task.FromResult(mockUser));

            var userService = new UserService(mockStorageService.Object);

            var result = await userService.Authenticate(mockUserName, mockOtherPassword);

            Assert.AreEqual(null, result);
        }
    }
}
