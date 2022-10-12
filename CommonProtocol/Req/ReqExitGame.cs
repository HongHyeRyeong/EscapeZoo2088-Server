namespace CommonProtocol
{
    public class ReqExitGame : CBaseProtocol
    {
        public string gameSessionId;
        public string teamName;
        public string playerSessionId;
        public string userId;
        public int currentRoundNum;
        public int teamUserCount;
    }
}