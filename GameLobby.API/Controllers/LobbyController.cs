using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using GameLobby.API.DataModels;
using GameLobby.BL;
using GameLobby.BL.DataTransfer;
using GameLobby.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace GameLobby.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LobbyController: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly LobbyHandler _lobbyHandler;
        private readonly PlayerService _playerService;

        private static string PlayerWithUsernamePresentInLobby(string username) =>
            $"Player with the username: '{username}' is already present in this lobby";

        private static string LobbyDoesNotExist(string lobbyCode) =>
            $"Lobby with code '{lobbyCode}' does not exist";

        public LobbyController(LobbyHandler lobbyHandler, 
            PlayerService playerService, 
            IMapper mapper)
        {
            _lobbyHandler = lobbyHandler;
            _playerService = playerService;
            _mapper = mapper;
        }

        #region Get

        [HttpGet("[action]")]
        [Authorize]
        public IActionResult GetCurrentLobby([FromQuery]GetLobbyRequest getLobbyRequest)
        {
            if (!(HttpContext.User.Identity is ClaimsIdentity identity))
            {
                return BadRequest();
            }

            var playerId = identity.FindFirst(JwtService.PlayerIdClaim).Value;
            if (playerId == null || !Guid.TryParse(playerId, out var playerGuid))
            {
                return BadRequest();
            }

            var lobbyDto = _lobbyHandler.GetLobbyDto(getLobbyRequest.LobbyCode);
            if (lobbyDto == null || lobbyDto.Players.FirstOrDefault(player => player.Id == playerGuid) == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                Lobby = lobbyDto
            });
        }

        #endregion

        #region Guest

        [HttpPost("[action]")]
        public IActionResult CreateLobbyGuest(CreateLobbyGuestRequest createLobbyGuestRequest)
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
            {
                return BadRequest();
            }

            var lobbyPlayer = new LobbyPlayer
            {
                Id = Guid.NewGuid(),
                Username = createLobbyGuestRequest.Username,
                IsLeader = true
            };
            var lobbyDto = _lobbyHandler.CreateLobby(lobbyPlayer);
            var jwtToken = _playerService.Enter(lobbyPlayer.Id, lobbyDto.Code);
            return Ok(new
            {
                Lobby = lobbyDto,
                AccessToken = jwtToken
            });
        }


        [HttpPost("[action]")]
        public IActionResult ConnectLobbyGuest(ConnectGuestToLobbyRequest connectGuestToLobbyRequest)
        {
            //TODO: Add fluent validation
            if (string.IsNullOrEmpty(connectGuestToLobbyRequest.Username) ||
                string.IsNullOrEmpty(connectGuestToLobbyRequest.LobbyCode))
            {
                return BadRequest();
            }

            var lobby = _lobbyHandler.GetLobbyDto(connectGuestToLobbyRequest.LobbyCode);
            if (lobby == null)
            {
                var errorDictionary = new Dictionary<string, string[]>
                {
                    { nameof(Lobby), new[] { LobbyDoesNotExist(connectGuestToLobbyRequest.Username) } }
                };
                return BadRequest(new ValidationProblemDetails(errorDictionary));
            }

            if (lobby.Players.FirstOrDefault(player => player.Username == connectGuestToLobbyRequest.Username) != null)
            {
                var errorDictionary = new Dictionary<string, string[]>
                {
                    { nameof(Lobby), new[] { PlayerWithUsernamePresentInLobby(connectGuestToLobbyRequest.Username) } }
                };
                return BadRequest(new ValidationProblemDetails(errorDictionary));
            }

            var lobbyPlayer = new LobbyPlayer()
            {
                Id = Guid.NewGuid(),
                Username = connectGuestToLobbyRequest.Username
            };

            var lobbyDto = _lobbyHandler.ConnectToLobby(lobbyPlayer, connectGuestToLobbyRequest.LobbyCode);
            var jwtToken = _playerService.Enter(lobbyPlayer.Id, lobbyDto.Code);

            return Ok(new
            {
                Lobby = lobbyDto,
                AccessToken = jwtToken
            });
        }

        #endregion

        #region User

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> CreateLobbyUser()
        {
            if (!(HttpContext.User.Identity is ClaimsIdentity identity))
            {
                return BadRequest();
            }

            var userId = identity.FindFirst(JwtService.PlayerIdClaim).Value;
            if (!Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest();
            }

            var userDto = await _playerService.GetUser(userGuid);
            var lobbyDto = _lobbyHandler.CreateLobby(_mapper.Map<LobbyPlayer>(userDto));
            return Ok(new
            {
                Lobby = lobbyDto
            });
        }

        #endregion
    }
}
