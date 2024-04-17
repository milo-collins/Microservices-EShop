namespace Eshop.ApiGateway.DTO
{
    public class AsyncRoutesOption
    {
        public bool Authenticate { get; set; }
        public Dictionary<string, AsyncRouteOption> Routes { get; set; }
    }

    public class AsyncRouteOption
    {
        public string Queue { get; set; }
        public string CommandType { get; set; }
    }
}
