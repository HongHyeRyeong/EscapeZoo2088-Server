namespace CommonProtocol
{
    public class ResLastGame : CBaseProtocol
    {
        public ResponseType ResponseType;
        public string gameSessionId;
        public string teamName;
        public string userId;
        public int enemyRoundNum;
        public bool isWinner;
    }
}
