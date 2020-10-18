using System;
using System.Collections.Generic;
using System.Text;

namespace GameLobby.BL.DataTransfer
{
    public class PlayerDto
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public bool IsUser { get; set; }

        public bool IsLeader { get; set; }
    }
}
