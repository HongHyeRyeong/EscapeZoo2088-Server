using System.Collections.Generic;

namespace CommonProtocol
{
    public class ResEnterIngame : CBaseProtocol
    {
        public ResponseType ResponseType;
        public string gameSessionId;
        public string teamName;
        public List<PlayerInfos> playerInfos;
    }
}
