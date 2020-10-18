using System;

namespace GameLobby.Core
{
    public class LobbyPlayer
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public bool IsUser { get; set; }

        public bool IsLeader { get; set; }

        public bool IsConnected { get; set; }

        public int Level { get; set; }
    }
}
