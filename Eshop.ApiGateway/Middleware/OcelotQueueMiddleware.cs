using Eshop.ApiGateway.DTO;
using EShop.Infrastructure.Command;
using EShop.Infrastructure.Command.Product;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Reflection;
using System.Threading.Tasks;

namespace Eshop.ApiGateway.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class OcelotQueueMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDictionary<string, AsyncRouteOption> _asyncRoutes;
        private static Assembly _commandAssembly = Assembly.GetAssembly(typeof(IEShopCommand));

        public OcelotQueueMiddleware(RequestDelegate next, IOptions<AsyncRoutesOption> asyncRoutesOptions)
        {
            _next = next;
            _asyncRoutes = asyncRoutesOptions.Value.Routes;
        }

        // PublisEndpoint published request to rabbitmq queue
        public async Task Invoke(HttpContext httpContext, IPublishEndpoint publishEndpoint)
        {
            try
            {
                // Sends payload to the appropriate microservice 
                if (httpContext.Request.Method == HttpMethod.Post.ToString())
                {
                    var payload = await PreparePayload(httpContext);

                    // Publishes to endpoint where it is consumed by the service
                    await publishEndpoint.Publish(payload);
                    httpContext.Response.StatusCode = 201;

                    await httpContext.Response.WriteAsync("Request Accepted");
                }
                else
                {
                    await _next(httpContext);
                }
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = 501;
                await httpContext.Response.WriteAsync($"Eror:{ex.Message}");
            }
        }

         async Task<object> PreparePayload(HttpContext httpContext)
        {
            // Fetch request body
            var reader = new StreamReader(httpContext.Request.Body);
            var content = await reader.ReadToEndAsync();

            string type = _asyncRoutes[httpContext.Request.Path].CommandType;

            var requiredType = _commandAssembly.ExportedTypes
                .Where(ty => ty.Name == type).FirstOrDefault();

            var payload = JsonConvert.DeserializeObject(content, requiredType);
            return payload;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class OcelotQueueMiddlewareExtensions
    {
        public static IApplicationBuilder UseOcelotQueueMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OcelotQueueMiddleware>();
        }
    }
}
