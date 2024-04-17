using Eshop.ApiGateway.Controllers;
using EShop.Infrastructure.Command.Product;
using EShop.Infrastructure.Event.Product;
using EShop.Infrastructure.Query.Product;
using MassTransit;
using MassTransit.Clients;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Runtime.CompilerServices;

// Temporary assembly generated when testing
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace EShop.ApiGateway.Test
{

    public class DerivedClass : Response<ProductCreated>
    {
        public ProductCreated Message => new ProductCreated() { ProductId = "101", ProductName = "Test Product", CreatedAt = DateTime.UtcNow };

        public Guid? MessageId => throw new NotImplementedException();

        public Guid? RequestId => throw new NotImplementedException();

        public Guid? CorrelationId => throw new NotImplementedException();

        public Guid? ConversationId => throw new NotImplementedException();

        public Guid? InitiatorId => throw new NotImplementedException();

        public DateTime? ExpirationTime => throw new NotImplementedException();

        public Uri? SourceAddress => throw new NotImplementedException();

        public Uri? DestinationAddress => throw new NotImplementedException();

        public Uri? ResponseAddress => throw new NotImplementedException();

        public Uri? FaultAddress => throw new NotImplementedException();

        public DateTime? SentTime => throw new NotImplementedException();

        public Headers Headers => throw new NotImplementedException();

        public HostInfo Host => throw new NotImplementedException();

        object Response.Message => throw new NotImplementedException();
    }

    [TestFixture]
    public class ProductControllerTest
    {
        ProductController controller = null;
        // Ctor injected dependencies need to be mocked. 
        // Setup runs before each test
        [SetUp]
        public void Setup()
        {
            // Mock Endpoint
            var sendEndpoint = new Mock<ISendEndpoint>();
            // Mock Buscontrol
            var busControl = new Mock<IBusControl>();
            // Get send endpoint needs a mocked return
            // When GetSendEndpoint is called, it returns (async as getsendendpoint() is async) sendEndpoint with type Uri
            busControl.Setup(x => x.GetSendEndpoint(It.IsAny<Uri>()))
                    .ReturnsAsync(sendEndpoint.Object);
            // Mock request client
            var deriveResponse = new Mock<DerivedClass>();
            var requestClient = new Mock<IRequestClient<GetProductById>>();
            requestClient.Setup(x => x.GetResponse<ProductCreated>(It.IsAny<GetProductById>(),
                                                   It.IsAny<CancellationToken>(),
                                                   It.IsAny<RequestTimeout>()))
                .ReturnsAsync(deriveResponse.Object);
            // Init controller
            controller = new(busControl.Object, requestClient.Object);

        }

        [Test]
        public async Task GetProductReturnsProductWithAcceptedResult()
        {
            // Arrange
            var expectedProduct = new ProductCreated()
            {
                ProductId = "101",
                ProductName = "Test Product",
                CreatedAt = DateTime.UtcNow
            };
            
            // Act
            var result = await controller.Get("111");

            // Assert
            Assert.IsTrue(((AcceptedResult)result).StatusCode == (int?)HttpStatusCode.Accepted);

            var recievedProduct = ((result as AcceptedResult).Value as Response<ProductCreated>).Message;
            Assert.IsTrue(recievedProduct.ProductId == expectedProduct.ProductId);;
        }

        [Test]
        public async Task AddProductReturnsAcceptedResult()
        {
            // Arrange 
            var createProduct = new Mock<CreateProductDto>();
            
            // Act
            var result = await controller.Add(createProduct.Object);

            // Assert
            // IsTrue fails the test on false 
            Assert.IsTrue(((AcceptedResult)result).StatusCode == (int?)HttpStatusCode.Accepted);
        }
    }
}