using System.Collections.Generic;

namespace GameLobby.Core
{
    public class Lobby
    {
        public string Code { get; set; }

        public HashSet<LobbyPlayer> Players { get; set; } = new HashSet<LobbyPlayer>();

        public HashSet<PlayerMessage> Messages { get; set; } = new HashSet<PlayerMessage>();
    }
}
