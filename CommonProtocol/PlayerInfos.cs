namespace CommonProtocol
{
    public class PlayerInfos
    {
        public string userId;
        public string mbti;
        public int animal;

        public PlayerInfos(string userId, string mbti, int animal)
        {
            this.userId = userId;
            this.mbti = mbti;
            this.animal = animal;
        }
    }
}
