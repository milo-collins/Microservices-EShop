using EShop.Infrastructure.Authentication;
using EShop.Infrastructure.Command.User;
using EShop.Infrastructure.Event.User;
using EShop.Infrastructure.Security;
using EShop.User.DataProvider;
using EShop.User.Query.Api.Handlers;
using MassTransit.Testing;
using Moq;

namespace EShop.User.Query.Api.Test
{
    public class LoginUserHandlerTest
    {
        [Test(TestOf = typeof(LoginUserHandler))]
        public async Task LoginUserReturnsToken()
        {
            // Arrange 
            LoginUser loginUserMock = new LoginUser()
            {
                Username = "John_Doe",
                Password = "password1234"
            };
            
            UserCreated userCreatedMock = new UserCreated()
            {
                Username = "John_Doe",
                Password = "password1234",
                ContactNo = "123",
                UserId = "id101",
                EmailId = "john.doe@mock.com"
            };

            JwtAuthToken token = new JwtAuthToken
            {
                Token = "21dsa8wjawk+2@adw12vnb=",
                Expires = default(long)
            };

            // InMemoryTestHarness is an in memory message broker for testing MassTransit
            var harness = new InMemoryTestHarness();

            try
            {
                var service = new Mock<IUserService>();
                service.Setup(x => x.GetUserByUsername(It.IsAny<string>()))
                    .ReturnsAsync(userCreatedMock);

                var authHandler = new Mock<IAuthenticationHandler>();
                authHandler.Setup(x => x.Create(It.IsAny<string>()))
                    .Returns(token);

                var encrypter = new Mock<IEncrypter>();
                encrypter.Setup(e => e.GetHash(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(userCreatedMock.Password);

                var consumerHandler = harness.Consumer(() => new LoginUserHandler(service.Object, encrypter.Object, authHandler.Object));

                // Act
                await harness.Start();

                var requestClient = harness.CreateRequestClient<LoginUser>();
                var user = await requestClient.GetResponse<JwtAuthToken>(loginUserMock);

                // Assert
                Assert.That(user.Message.Token == token.Token);
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}