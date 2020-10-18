using System.Linq;
using AutoMapper;
using GameLobby.BL.DataTransfer;
using GameLobby.Core;

namespace GameLobby.API.MapperProfile
{
    public class GameLobbyProfile : Profile
    {
        public GameLobbyProfile()
        {
            CreateMap<PlayerMessage, PlayerMessageDto>()
                .ForMember(dest => dest.Id, 
                    opt => 
                        opt.MapFrom(src => src.Player.Id))
                .ForMember(dest => dest.Username,
                    opt =>
                        opt.MapFrom(src => src.Player.Username));
            CreateMap<Lobby, LobbyDto>();

            CreateMap<User, LobbyPlayer>().BeforeMap((src, dest) => dest.IsUser = true);
            CreateMap<Guest, LobbyPlayer>();
        }
    }
}
