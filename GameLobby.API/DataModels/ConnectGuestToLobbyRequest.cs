namespace GameLobby.API.DataModels
{
    public class ConnectGuestToLobbyRequest
    {
        public string Username { get; set; }

        public string LobbyCode { get; set; }
    }
}
