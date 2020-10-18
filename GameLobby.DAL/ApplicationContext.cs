using GameLobby.Core;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameLobby.DAL
{
    public class ApplicationContext
    {
        private const string UsersCollectionName = "users";
        private readonly IMongoDatabase _database;

        public ApplicationContext(IOptions<Settings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>(UsersCollectionName);
    }
}
