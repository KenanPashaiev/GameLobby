using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using GameLobby.BL.DataTransfer;
using GameLobby.Core;
using GameLobby.DAL;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace GameLobby.BL
{
    public class PlayerService
    {
        public const string ProtectPurpose = "LobbyCodeEncryption";

        private readonly IMapper _mapper;
        private readonly UserRepository _userRepository;
        private readonly PasswordHasher<object> _passwordHasher;
        private readonly JwtService _jwtService;
        public PlayerService(IMapper mapper,
            PasswordHasher<object> passwordHasher, 
            JwtService jwtService, 
            UserRepository userRepository, 
            IDataProtectionProvider dataProtectionProvider)
        {
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _userRepository = userRepository;
        }

        public async Task<UserDto> GetUser(Guid userId)
        {
            User user = await _userRepository.GetUserById(userId);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserLoginDTO> Register(UserRegisterDto userRegisterDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = userRegisterDto.Username,
                PasswordHash = _passwordHasher.HashPassword(null, userRegisterDto.Password)
            };

            await _userRepository.InsertUser(user);

            return new UserLoginDTO
            {
                Username = userRegisterDto.Username,
                Password = userRegisterDto.Password
            };
        }

        public async Task<string> Login(UserLoginDTO userLoginDto)
        {
            var user = await _userRepository.GetUserByUsername(userLoginDto.Username);
            if (user == null)
            {
                return null;
            }

            var passwordVerificationResult =
                _passwordHasher.VerifyHashedPassword(null, user.PasswordHash, userLoginDto.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new Claim(JwtService.PlayerIdClaim, user.Id.ToString())
            };
            var token = _jwtService.GenerateToken(claims);
            
            return token;
        }

        public string Enter(Guid playerId, string lobbyCode)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtService.PlayerIdClaim, playerId.ToString()),
                new Claim(JwtService.LobbyCodeClaim, lobbyCode)
            };
            var token = _jwtService.GenerateToken(claims);

            return token;
        }
    }
}
