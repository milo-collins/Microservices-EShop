namespace EShop.Infrastructure.Mongo
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync();
    }    
}
