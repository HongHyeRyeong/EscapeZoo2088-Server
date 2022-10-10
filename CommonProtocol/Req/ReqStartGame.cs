namespace CommonProtocol
{
    public class ReqStartGame : CBaseProtocol
    {
        public string gameSessionId;
        public string teamName;
        public string playerSessionId;
        public string userId;
        public string mbti;
        public int animal;
        public int preRoundNum;
        public int endRoundNum;
        public int teamUserCount;
    }
}
