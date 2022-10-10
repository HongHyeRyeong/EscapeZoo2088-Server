using System.Collections.Generic;

namespace CommonProtocol
{
    public class ResRestartGame : CBaseProtocol
    {
        public ResponseType ResponseType;
        public string gameSessionId;
        public string teamName;
        public string playerSessionId;
        public string userId;
        public int currentRoundNum;
        public int endRoundNum;
        public List<int> roundList;
        public int enemyRoundNum;
        public long sunriseTime;
    }
}
