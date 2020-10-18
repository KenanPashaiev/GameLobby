using System;

namespace GameLobby.BL.DataTransfer
{
    public class PlayerMessageDto
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Text { get; set; }

        public DateTime TimeSent { get; set; }
    }
}
