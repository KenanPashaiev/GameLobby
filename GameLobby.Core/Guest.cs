using System;

namespace GameLobby.Core
{
    public class Guest
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public bool IsUser { get; set; }

        public bool IsLeader { get; set; }
    }
}
