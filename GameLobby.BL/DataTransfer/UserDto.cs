using System;
using System.Collections.Generic;
using GameLobby.Core;

namespace GameLobby.BL.DataTransfer
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public int Level { get; set; }

        public IEnumerable<Guid> Games { get; set; }

        public IEnumerable<Guid> Friends { get; set; }
    }
}
