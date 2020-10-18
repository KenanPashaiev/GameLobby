using System;

namespace GameLobby.API.DataModels
{
    public class ConnectUserToLobbyRequest
    {
        public Guid Id { get; set; }
        public string LobbyCode { get; set; }
    }
}
