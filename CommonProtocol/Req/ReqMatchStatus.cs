using System.Collections.Generic;


namespace CommonProtocol
{
    public class ReqMatchStatus : CBaseProtocol
    {
        public string userId;
        public int gameMap;
        public int character;
        public string mbti;
        public List<string> ticketIds = new List<string>();
    }
}
