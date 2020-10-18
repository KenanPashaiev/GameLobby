using System.Collections.Generic;
using GameLobby.Core;

namespace GameLobby.BL.DataTransfer
{
    public class LobbyDto
    {
        public string Code { get; set; }

        public HashSet<LobbyPlayer> Players { get; set; }

        public HashSet<PlayerMessageDto> Messages { get; set; }
    }
}
