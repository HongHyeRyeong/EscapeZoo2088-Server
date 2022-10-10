namespace CommonProtocol
{
    public class ReqRestartGame : CBaseProtocol
    {
        public string gameSessionId;
        public string teamName;
        public string playerSessionId;
        public string userId;
        public string mbti;
        public int animal;
        public int currentRoundNum;
        public int endRoundNum;
        public int teamUserCount;
    }
}
