using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GameLobby.Core;
using MongoDB.Driver;

namespace GameLobby.DAL
{
    public class UserRepository
    {
        private readonly ApplicationContext _applicationContext;

        public UserRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<User> GetUserById(Guid userId)
        {
            var filter = Builders<User>.Filter.Where(user => user.Id == userId);
            return await _applicationContext.Users.Find(filter).Limit(1).SingleAsync();
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var filter = Builders<User>.Filter.Where(user => user.Username == username);
            return await _applicationContext.Users.Find(filter).Limit(1).SingleAsync();
        }

        public async Task InsertUser(User user)
        {
            await _applicationContext.Users.InsertOneAsync(user);
        }
    }
}
