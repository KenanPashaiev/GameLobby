using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GameLobby.BL;
using GameLobby.BL.DataTransfer;
using GameLobby.Core;

namespace GameLobby.API
{
    public class LobbyHandler
    {
        private readonly IMapper _mapper;
        private readonly LobbyService _lobbyService;
        private readonly PlayerService _playerService;

        public HashSet<Lobby> Lobbies { get; set; } = new HashSet<Lobby>();

        public LobbyHandler(IMapper mapper,
            LobbyService lobbyService, 
            PlayerService playerService)
        {
            _mapper = mapper;
            _lobbyService = lobbyService;
            _playerService = playerService;
        }

        public LobbyDto CreateLobby(LobbyPlayer creator)
        {
            var lobby = _lobbyService.CreateLobby(Lobbies.Select(l => l.Code), creator);
            Lobbies.Add(lobby);

            return _mapper.Map<LobbyDto>(lobby);
        }

        public LobbyDto GetLobbyDto(string lobbyCode)
        {
            var lobby = Lobbies.FirstOrDefault(l => l.Code == lobbyCode);
            return lobby == null ? null : _mapper.Map<LobbyDto>(lobby);
        }

        public LobbyPlayer GetPlayerDto(Guid playerId, string lobbyCode = null)
        {
            Lobby lobby;
            if (string.IsNullOrEmpty(lobbyCode))
            {
                lobby = Lobbies.FirstOrDefault(l => l.Players.
                    FirstOrDefault(p => p.Id == playerId) != null);
            }
            else
            {
                lobby = Lobbies.FirstOrDefault(l => l.Code == lobbyCode);
            }

            var lobbyPlayer = lobby?.Players.FirstOrDefault(p => p.Id == playerId);
            return lobbyPlayer == null ? null : _mapper.Map<LobbyPlayer>(lobbyPlayer);
        }

        public LobbyDto ConnectToLobby(LobbyPlayer player, string lobbyCode)
        {
            var lobby = Lobbies.SingleOrDefault(l => l.Code == lobbyCode);
            if (lobby == null)
            {
                return null;
            }

            lobby.Players.Add(player);
            return _mapper.Map<LobbyDto>(lobby);
        }

        public LobbyPlayer DisconnectFromLobby(Guid playerId, string lobbyCode)
        {
            var lobby = Lobbies.SingleOrDefault(l => l.Code == lobbyCode);
            var player = lobby?.Players.FirstOrDefault(p => p.Id == playerId);
            if (player == null)
            {
                return null;
            }

            var playerWasRemoved = lobby.Players.RemoveWhere(p => p.Id == player.Id) == 1;
            if(!lobby.Players.Any())
            {
                Lobbies.Remove(lobby);
                return player;
            }

            if (player.IsLeader)
            {
                lobby.Players.First().IsLeader = true;
            }

            return playerWasRemoved ? player : null;
        }

        public bool SetLobbyLeader(Guid playerId, string lobbyCode)
        {
            var lobby = Lobbies.SingleOrDefault(l => l.Code == lobbyCode);
            var player = lobby?.Players.FirstOrDefault(p => p.Id == playerId);
            if (player == null)
            {
                return false;
            }

            player.IsLeader = true;
            return true;
        }

        public bool KickPlayer(Guid playerId, string lobbyCode)
        {
            var lobby = Lobbies.SingleOrDefault(l => l.Code == lobbyCode);
            if (lobby == null)
            {
                return false;
            }

            return lobby.Players.RemoveWhere(player => player.Id == playerId) == 1;
        }

        public PlayerMessageDto SendMessage(Guid playerId, string message, string lobbyCode)
        {
            var lobby = Lobbies.SingleOrDefault(l => l.Code == lobbyCode);
            var player = lobby?.Players.FirstOrDefault(p => p.Id == playerId);
            if (player == null)
            {
                return null;
            }

            var messageToAdd = new PlayerMessage
            {
                Player = player,
                Text = message,
                TimeSent = DateTime.Now
            };

            var messageAdded = lobby.Messages.Add(messageToAdd);
            if (!messageAdded)
            {
                return null;
            }

            return _mapper.Map<PlayerMessageDto>(messageToAdd);
        }

        public bool SetPlayerIsConnected(Guid playerId, bool isConnected, string lobbyCode = null)
        {
            Lobby lobby;
            if (string.IsNullOrEmpty(lobbyCode))
            {
                lobby = Lobbies.FirstOrDefault(l => l.Players.
                    FirstOrDefault(p => p.Id == playerId) != null);
            }
            else
            {
                lobby = Lobbies.FirstOrDefault(l => l.Code == lobbyCode);
            }

            var lobbyPlayer = lobby?.Players.FirstOrDefault(p => p.Id == playerId);
            if (lobbyPlayer == null)
            {
                return false;
            }

            lobbyPlayer.IsConnected = isConnected;
            return true;
        }
    }
}
