using System;
using System.Collections.Generic;

namespace GameLobby.Core
{
    public class User
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public int Level { get; set; }

        public IEnumerable<Guid> Games { get; set; }

        public IEnumerable<Guid> Friends { get; set; }
    }
}
