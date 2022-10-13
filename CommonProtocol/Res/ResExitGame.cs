using System.Collections.Generic;

namespace CommonProtocol
{
    public class ResExitGame : CBaseProtocol
    {
        public ResponseType ResponseType;
        public string gameSessionId;
        public string teamName;
        public List<PlayerInfos> playerInfos;
    }
}