using EShop.Infrastructure.Command.User;
using EShop.Infrastructure.Event.User;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Xml.Linq;

namespace EShop.User.DataProvider
{
    public class UserRepository : IUserRepository
    {
        // Inject Mongo DB
        private IMongoDatabase _database;
        private IMongoCollection<CreateUser> _collection => _database.GetCollection<CreateUser>("user");
        public UserRepository(IMongoDatabase database)
        {
            _database= database;
        }
        public async Task<UserCreated> AddUser(CreateUser user)
        {
            await _collection.InsertOneAsync(user);
            return new UserCreated() { 
                ContactNo= user.ContactNo,
                EmailId = user.EmailId,
                Password = user.Password,
                Username = user.Username 
            };
        }

        public async Task<UserCreated> GetUser(CreateUser user)
        {
            var userResult = await _collection.AsQueryable().FirstOrDefaultAsync(x => x.Username == user.Username);
            if (userResult == null)
            {
                return new UserCreated();
            }
            return new UserCreated()
            {
                Username = userResult.Username,
                EmailId = userResult.EmailId,
                Password = userResult.Password,
                ContactNo = userResult.ContactNo,
                UserId = userResult.UserId
            };
            throw new NotImplementedException();
        }

        public async Task<UserCreated> GetUserByUsername(string name)
        {
            var userResult = await _collection.AsQueryable().FirstOrDefaultAsync(x => x.Username == name);
            if (userResult == null)
            {
                return new UserCreated();
            }

            return new UserCreated()
            {
                Username = userResult.Username,
                EmailId = userResult.EmailId,
                Password = userResult.Password,
                ContactNo = userResult.ContactNo,
                UserId = userResult.UserId
            };
            throw new NotImplementedException();
        }
    }
}
