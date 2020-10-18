using System;

namespace GameLobby.Core
{
    public class PlayerMessage
    {
        public LobbyPlayer Player { get; set; }

        public string Text { get; set; }

        public DateTime TimeSent { get; set; }
    }
}
