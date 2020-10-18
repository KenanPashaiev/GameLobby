using System;
using System.Collections.Generic;
using System.Linq;
using GameLobby.Core;

namespace GameLobby.BL
{
    public class LobbyService
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly Random Random = new Random();

        public Lobby CreateLobby(IEnumerable<string> lobbyCodes, LobbyPlayer creator)
        {
            var code = string.Empty;
            var enumerable = lobbyCodes.ToList();
            if(!enumerable.Any())
            {
                code = new string(Enumerable.Repeat(Chars, 5)
                    .Select(s => s[Random.Next(s.Length)]).ToArray());
            }

            while (code == string.Empty)
            {
                code = new string(Enumerable.Repeat(Chars, 5)
                    .Select(s => s[Random.Next(s.Length)]).ToArray());
                if (!enumerable.Contains(code))
                {
                    break;
                }
                code = string.Empty;
            }

            return new Lobby { Code = code, Players = new HashSet<LobbyPlayer>{ creator } };
        }
    }
}
