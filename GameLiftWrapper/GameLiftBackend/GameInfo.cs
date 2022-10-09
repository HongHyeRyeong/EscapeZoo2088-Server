using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLiftWrapper
{
    public class GameInfo
    {
        private static GameInfo staticGameInfo;

        public static GameInfo Instance()
        {
            if (staticGameInfo == null)
            {
                staticGameInfo = new GameInfo();
            }
            return staticGameInfo;
        }

        Dictionary<MatchInfo, GameData> dict = new Dictionary<MatchInfo, GameData>();

        public Boolean addUser(string gameSessionId, string teamName, string userId, int roundNum)
        {
            if (gameSessionId == null || teamName == null || userId == null)
            {
                return false;
            }
            if (gameSessionId.Equals("") || teamName.Equals("") || userId.Equals(""))
            {
                return false;
            }

            MatchInfo key = new MatchInfo(gameSessionId, teamName);
            if (dict.ContainsKey(key))
            {
                GameData value = dict[key];
                if (!value.userInfo.ContainsKey(userId))
                {
                    value.userInfo[userId] = roundNum;
                    dict[key] = value;
                }
            }
            else
            {
                Dictionary<string, int> userInfo = new Dictionary<string, int>();
                userInfo[userId] = roundNum;

                Random randomObj = new Random();
                List<int> roundList = new List<int>();
                for (int i = 0; i < 7; i++)
                {
                    roundList.Add(i);
                }
                List<int> shuffleList = roundList.OrderBy(a => Guid.NewGuid()).ToList();

                long randomTime = randomObj.Next(10, 15);
                var timeSpan = (DateTime.UtcNow.AddSeconds(randomTime) - new DateTime(1970, 1, 1, 0, 0, 0));
                long sunriseTime = (long)timeSpan.TotalSeconds;

                GameData value = new GameData(userInfo, shuffleList, sunriseTime);
                dict.Add(key, value);
            }
            return true;
        }

        public Boolean removeGame(string gameSessionId, string teamName)
        {
            if (gameSessionId == null || teamName == null)
            {
                return false;
            }
            if (gameSessionId.Equals("") || teamName.Equals(""))
            {
                return false;
            }

            MatchInfo key = new MatchInfo(gameSessionId, teamName);
            if (dict.ContainsKey(key))
            {
                dict.Remove(key);
                return true;
            }
            return false;
        }

        public Boolean isExistUser(string gameSessionId, string teamName, string userId)
        {
            if (gameSessionId == null || teamName == null || userId == null)
            {
                return false;
            }
            if (gameSessionId.Equals("") || teamName.Equals("") || userId.Equals(""))
            {
                return false;
            }

            MatchInfo key = new MatchInfo(gameSessionId, teamName);
            if (dict.ContainsKey(key))
            {
                GameData value = dict[key];
                if (value.userInfo.ContainsKey(userId))
                {
                    return true;
                }
            }
            return false;
        }

        public int removeUser(string gameSessionId, string teamName, string userId)
        {
            if (gameSessionId == null || teamName == null || userId == null)
            {
                return 0;
            }
            if (gameSessionId.Equals("") || teamName.Equals("") || userId.Equals(""))
            {
                return 0;
            }

            MatchInfo key = new MatchInfo(gameSessionId, teamName);
            if (dict.ContainsKey(key))
            {
                GameData value = dict[key];
                if (value.userInfo.ContainsKey(userId))
                {
                    value.userInfo.Remove(userId);
                }
                return value.userInfo.Count;
            }
            return 0;
        }

        public int getUserCount(string gameSessionId, string teamName)
        {
            if (gameSessionId == null || teamName == null)
            {
                return 0;
            }
            if (gameSessionId.Equals("") || teamName.Equals(""))
            {
                return 0;
            }

            MatchInfo key = new MatchInfo(gameSessionId, teamName);
            if (dict.ContainsKey(key))
            {
                GameData value = dict[key];
                return value.userInfo.Count;
            }
            return 0;
        }

        public int getUserCountInRound(string gameSessionId, string teamName, int roundNum)
        {
            if (gameSessionId == null || teamName == null)
            {
                return 0;
            }
            if (gameSessionId.Equals("") || teamName.Equals(""))
            {
                return 0;
            }

            MatchInfo key = new MatchInfo(gameSessionId, teamName);
            if (dict.ContainsKey(key))
            {
                GameData value = dict[key];
                int checkCount = 0;
                foreach (KeyValuePair<string, int> keyValuePair in value.userInfo)
                {
                    if (keyValuePair.Value == roundNum)
                    {
                        checkCount++;
                    }
                }
                return checkCount;
            }
            return 0;
        }
    }

    public class MatchInfo
    {
        public string gameSessionId;
        public string teamName;

        public MatchInfo(string gameSessionId, string teamName)
        {
            this.gameSessionId = gameSessionId;
            this.teamName = teamName;
        }
    }

    public class GameData
    {
        public Dictionary<string, int> userInfo = new Dictionary<string, int>();
        public List<int> roundList = new List<int>();
        public long sunriseTime = 0;

        public GameData(Dictionary<string, int> userInfo, List<int> roundList, long sunriseTime)
        {
            foreach (KeyValuePair<string, int> keyValuePair in userInfo)
            {
                this.userInfo[keyValuePair.Key] = keyValuePair.Value;
            }
            foreach (int roundNum in roundList)
            {
                this.roundList.Add(roundNum);
            }
            this.sunriseTime = sunriseTime;
        }
    }
}
