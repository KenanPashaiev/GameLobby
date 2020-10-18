using System.Threading.Tasks;
using GameLobby.BL;
using GameLobby.BL.DataTransfer;
using Microsoft.AspNetCore.Mvc;

namespace GameLobby.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly PlayerService _playerService;

        public PlayerController(PlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserRegisterDto userRegisterDto)
        {
            await _playerService.Register(userRegisterDto);

            return null;
        }
    }
}
