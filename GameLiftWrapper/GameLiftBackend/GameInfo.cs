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
                Boolean isAnotherTeam = false;
                MatchInfo anotherTeam = null;
                foreach (KeyValuePair<MatchInfo, GameData> keyValuePair in dict)
                {
                    if (keyValuePair.Key.gameSessionId.Equals(gameSessionId) && !keyValuePair.Key.teamName.Equals(teamName))
                    {
                        isAnotherTeam = true;
                        anotherTeam = new MatchInfo(keyValuePair.Key.gameSessionId, keyValuePair.Key.teamName);
                        break;
                    }
                }

                if (isAnotherTeam)
                {
                    GameData value = dict[anotherTeam];
                    value.userInfo.Clear();
                    value.userInfo[userId] = roundNum;
                    dict.Add(key, value);
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
                    //List<int> shuffleList = roundList.OrderBy(a => Guid.NewGuid()).ToList();
                    int random1, temp;
                    for (int i = 0; i < roundList.Count - 1; ++i)
                    {
                        random1 = randomObj.Next(i+1, roundList.Count-1);

                        temp = roundList[i];
                        roundList[i] = roundList[random1];
                        roundList[random1] = temp;
                    }

                    long sunriseTime = randomObj.Next(10, 25);

                    GameData value = new GameData(userInfo, roundList, sunriseTime);
                    dict.Add(key, value);
                }
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

        public int getUserCountInGame(string gameSessionId)
        {
            if (gameSessionId == null)
            {
                return 0;
            }
            if (gameSessionId.Equals(""))
            {
                return 0;
            }

            int userCount = 0;
            foreach (KeyValuePair<MatchInfo, GameData> keyValuePair in dict)
            {
                if (keyValuePair.Key.gameSessionId.Equals(gameSessionId))
                {
                    GameData value = dict[keyValuePair.Key];
                    userCount += value.userInfo.Count;
                }
            }
            return userCount;
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

        public List<string> getUserListInTeam(string gameSessionId, string teamName, int roundNum)
        {
            if (gameSessionId == null || teamName == null)
            {
                return null;
            }
            if (gameSessionId.Equals("") || teamName.Equals(""))
            {
                return null;
            }

            MatchInfo key = new MatchInfo(gameSessionId, teamName);
            if (dict.ContainsKey(key))
            {
                GameData value = dict[key];
                if (value.userInfo.Count == 0)
                {
                    return null;
                }

                List<string> userList = new List<string>();
                foreach (KeyValuePair<string, int> keyValuePair in value.userInfo)
                {
                    if (keyValuePair.Value == roundNum)
                    {
                        userList.Add(keyValuePair.Key);
                    }
                }
                return userList;
            }
            return null;
        }

        public Dictionary<string, int> getEnemyRound(string gameSessionId, string teamName, int roundNum)
        {
            if (gameSessionId == null || teamName == null)
            {
                return null;
            }
            if (gameSessionId.Equals("") || teamName.Equals(""))
            {
                return null;
            }

            Dictionary<string, int> enemyRoundInfo = new Dictionary<string, int>();
            foreach (KeyValuePair<MatchInfo, GameData> keyValuePair in dict)
            {
                string keyGameSessionId = keyValuePair.Key.gameSessionId;
                string keyTeamName = keyValuePair.Key.teamName;
                if (keyGameSessionId.Equals(gameSessionId) && !keyTeamName.Equals(teamName))
                {
                    GameData value = dict[keyValuePair.Key];
                    int maxRound = 0;
                    foreach (KeyValuePair<string, int> keyValue in value.userInfo)
                    {
                        int userRound = keyValue.Value;
                        if (maxRound < userRound)
                        {
                            maxRound = userRound;
                        }
                    }
                    enemyRoundInfo[keyTeamName] = maxRound;
                }
            }
            return enemyRoundInfo;
        }

        public Boolean setSunriseTime(string gameSessionId, string teamName, long newSunriseTime)
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
                GameData value = dict[key];
                value.sunriseTime = newSunriseTime;
                dict[key] = value;
            }
            return true;
        }

        public long getSunriseTime(string gameSessionId, string teamName)
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
                return value.sunriseTime;
            }
            return 0;
        }

        public List<int> getRoundList(string gameSessionId, string teamName)
        {
            if (gameSessionId == null || teamName == null)
            {
                return null;
            }
            if (gameSessionId.Equals("") || teamName.Equals(""))
            {
                return null;
            }

            MatchInfo key = new MatchInfo(gameSessionId, teamName);
            if (dict.ContainsKey(key))
            {
                GameData value = dict[key];
                List<int> resultRoundList = new List<int>();
                foreach (int roundNum in value.roundList) {
                    resultRoundList.Add(roundNum);
                }
                return resultRoundList;
            }
            return null;
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
