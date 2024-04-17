using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Infrastructure.Mongo
{
    // Connects to MongoDb
    public static class Extension
    {
        // Adds the db to service collection
        public static void AddMongoDb( this IServiceCollection services, IConfiguration configuration)
        {
            var configSection = configuration.GetSection("mongo");

            var mongoConfig = new MongoConfig();

            configSection.Bind(mongoConfig);

            // Adds mongo client to service collection with a single instance
            services.AddSingleton<IMongoClient>( client =>
            {
                return new MongoClient(mongoConfig.ConnectionString);
            });
            // Fetches client from collection and uses it to get database
            services.AddSingleton<IMongoDatabase>( client =>
            {
                var mongoClient = client.GetService<IMongoClient>();
                if(mongoClient == null)
                {
                    throw new ArgumentNullException(nameof(mongoClient));
                }

                // Map database object to service using IMongoDatabase
                return mongoClient.GetDatabase(mongoConfig.Database);
            });

            //Injects mongo conventions into service
            services.AddSingleton<IDatabaseInitializer, MongoInitializer>();
        }
    }
}
