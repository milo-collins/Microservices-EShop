using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Infrastructure.Mongo
{
    //Implements IDatabaseInitializer
    public class MongoInitializer : IDatabaseInitializer
    {
        private bool _initialized;
        private IMongoDatabase _database;

        public MongoInitializer(IMongoDatabase database) 
        {
            _database = database;
        }

        public async Task InitializeAsync()
        {
            if (_initialized)
                return;
            //Conventions for Mongo DB to include
            IConventionPack conventionPack = new ConventionPack
            {
                //Ignore properties mongoDB doc does not have
                new IgnoreExtraElementsConvention(true),
                //Save data with camelCasing
                new CamelCaseElementNameConvention(),
                //Enums are saved by name and not by value
                new EnumRepresentationConvention(BsonType.String)
            };

            //(Name of convention rule, Convention Pack, condition filter)
            ConventionRegistry.Register("EShop", conventionPack, c => true);
            _initialized = true;

            await Task.CompletedTask;
        }
    }
}
