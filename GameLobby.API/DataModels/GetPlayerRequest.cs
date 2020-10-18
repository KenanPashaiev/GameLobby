using System;

namespace GameLobby.API.DataModels
{
    public class GetPlayerRequest
    {
        public Guid PlayerId { get; set; }

        public string LobbyCode { get; set; }
    }
}
