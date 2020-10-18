using System;
using System.Security.Claims;
using System.Threading.Tasks;
using GameLobby.BL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GameLobby.API.Hubs
{
    public class LobbyHub : Hub
    {
        private readonly LobbyHandler _lobbyHandler;

        public LobbyHub(LobbyHandler lobbyHandler)
        {
            _lobbyHandler = lobbyHandler;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task SendLobbyMessageGuest(string message, string lobbyCode)
        {
            if (!(Context.User.Identity is ClaimsIdentity identity))
            {
                return;
            }

            var playerId = identity.FindFirst(JwtService.PlayerIdClaim).Value;
            if (playerId == null || !Guid.TryParse(playerId, out var playerGuid))
            {
                return;
            }

            var player = _lobbyHandler.GetPlayerDto(playerGuid);
            if (player == null)
            {
                return;
            }

            var messageDto = _lobbyHandler.SendMessage(player.Id, message, lobbyCode);
            await Clients.Groups(lobbyCode).SendAsync("Chat", messageDto);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task JoinLobbyNotify()
        {
            if (!(Context.User.Identity is ClaimsIdentity identity))
            {
                return;
            }

            var playerId = identity.FindFirst(JwtService.PlayerIdClaim).Value;
            var lobbyCode = identity.FindFirst(JwtService.LobbyCodeClaim).Value;
            if (string.IsNullOrWhiteSpace(playerId) || !Guid.TryParse(playerId, out var playerGuid) ||
                string.IsNullOrWhiteSpace(lobbyCode))
            {
                return;
            }

            var player = _lobbyHandler.GetPlayerDto(playerGuid, lobbyCode);
            if (player == null)
            {
                return;
            }

            await Clients.Groups(lobbyCode).SendAsync("JoinLobbyNotify", player.Username);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task LeaveLobby(string reason)
        {
            if (!(Context.User.Identity is ClaimsIdentity identity))
            {
                return;
            }

            var playerId = identity.FindFirst(JwtService.PlayerIdClaim).Value;
            var lobbyCode = identity.FindFirst(JwtService.LobbyCodeClaim).Value;
            if (string.IsNullOrWhiteSpace(playerId) || !Guid.TryParse(playerId, out var playerGuid) ||
                string.IsNullOrWhiteSpace(lobbyCode))
            {
                return;
            }

            var player = _lobbyHandler.DisconnectFromLobby(playerGuid, lobbyCode);
            await Clients.Groups(lobbyCode).SendAsync("LeaveLobbyNotify", player.Username, reason);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task OnConnectedAsync()
        {
            if (!(Context.User.Identity is ClaimsIdentity identity))
            {
                return;
            }

            var playerId = identity.FindFirst(JwtService.PlayerIdClaim).Value;
            var lobbyCode = identity.FindFirst(JwtService.LobbyCodeClaim).Value;
            if (string.IsNullOrWhiteSpace(playerId) || !Guid.TryParse(playerId, out var playerGuid) ||
                string.IsNullOrWhiteSpace(lobbyCode))
            {
                return;
            }

            var player = _lobbyHandler.GetPlayerDto(playerGuid, lobbyCode);
            if (player == null)
            {
                return;
            }

            _lobbyHandler.SetPlayerIsConnected(playerGuid, true, lobbyCode);
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyCode);
            await Clients.Groups(lobbyCode).SendAsync("Connect", player.Username);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (!(Context.User.Identity is ClaimsIdentity identity))
            {
                return;
            }

            var playerId = identity.FindFirst(JwtService.PlayerIdClaim).Value;
            var lobbyCode = identity.FindFirst(JwtService.LobbyCodeClaim).Value;
            if (string.IsNullOrWhiteSpace(playerId) || !Guid.TryParse(playerId, out var playerGuid) ||
                string.IsNullOrWhiteSpace(lobbyCode))
            {
                return;
            }

            var player = _lobbyHandler.GetPlayerDto(playerGuid, lobbyCode);
            if (player == null)
            {
                return;
            }

            _lobbyHandler.SetPlayerIsConnected(playerGuid, false, lobbyCode);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyCode);
            await Clients.Groups(lobbyCode).SendAsync("Disconnect", player.Username);
        }
    }
}
