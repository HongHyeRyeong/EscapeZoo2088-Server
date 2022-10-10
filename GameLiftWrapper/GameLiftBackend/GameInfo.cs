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

        public Boolean addUser(string gameSessionId, string teamName, string userId, string mbti, int animal, int roundNum)
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
                UserData userData = new UserData(roundNum, mbti, animal);
                value.userInfo.Add(userId, userData);
                dict[key] = value;
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
                    GameData anotherTeamValue = dict[anotherTeam];
                    Dictionary<string, UserData> userInfo = new Dictionary<string, UserData>();
                    userInfo[userId] = new UserData(roundNum, mbti, animal);

                    List<int> roundList = new List<int>();
                    foreach (int num in anotherTeamValue.roundList)
                    {
                        roundList.Add(num);
                    }

                    long sunriseTime = anotherTeamValue.sunriseTime;

                    GameData value = new GameData(userInfo, roundList, sunriseTime, false);
                    dict.Add(key, value);
                }
                else
                {
                    Dictionary<string, UserData> userInfo = new Dictionary<string, UserData>();
                    userInfo[userId] = new UserData(roundNum, mbti, animal);

                    Random randomObj = new Random();
                    List<int> roundList = new List<int>();
                    for (int i = 0; i < 7; i++)
                    {
                        roundList.Add(i);
                    }
                    //List<int> shuffleList = roundList.OrderBy(a => Guid.NewGuid()).ToList();
                    int random1, temp;
                    for (int i = 0; i < roundList.Count; ++i)
                    {
                        random1 = randomObj.Next(0, roundList.Count-1);

                        temp = roundList[i];
                        roundList[i] = roundList[random1];
                        roundList[random1] = temp;
                    }

                    long sunriseTime = randomObj.Next(10, 25);

                    GameData value = new GameData(userInfo, roundList, sunriseTime, false);
                    dict.Add(key, value);
                }
            }
            return true;
        }

        public Boolean updateUser(string gameSessionId, string teamName, string userId, int roundNum)
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
            GameData value = dict[key];
            UserData userData = new UserData(roundNum, value.userInfo[userId].mbti, value.userInfo[userId].animal);
            value.userInfo.Add(userId, userData);
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
                return true;
            }
            else
            {
                dict.Add(key, value);
                return true;
            }
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
                foreach (KeyValuePair<string, UserData> keyValuePair in value.userInfo)
                {
                    if (keyValuePair.Value.roundNum == roundNum)
                    {
                        checkCount++;
                    }
                }
                return checkCount;
            }
            return 0;
        }

        public Dictionary<string, UserData> getUserListInTeam(string gameSessionId, string teamName)
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

                Dictionary<string, UserData> userList = new Dictionary<string, UserData>();
                foreach (KeyValuePair<string, UserData> keyValuePair in value.userInfo)
                {
                    UserData userData = new UserData(keyValuePair.Value.roundNum, keyValuePair.Value.mbti, keyValuePair.Value.animal);
                    userList.Add(keyValuePair.Key, userData);
                }
                return userList;
            }
            return null;
        }

        public Dictionary<string, int> getEnemyRound(string gameSessionId, string teamName)
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
                    foreach (KeyValuePair<string, UserData> keyValue in value.userInfo)
                    {
                        int userRound = keyValue.Value.roundNum;
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

        public bool isWinner(string gameSessionId, string teamName)
        {
            if (gameSessionId == null || teamName == null)
            {
                return false;
            }
            if (gameSessionId.Equals("") || teamName.Equals(""))
            {
                return false;
            }

            string winnerGameSessionId = "";
            string winnerTeamName = "";
            foreach (KeyValuePair<MatchInfo, GameData> keyValuePair in dict)
            {
                string keyGameSessionId = keyValuePair.Key.gameSessionId;
                string keyTeamName = keyValuePair.Key.teamName;
                GameData gameData = keyValuePair.Value;
                if (gameData.isWinner)
                {
                    winnerGameSessionId = keyGameSessionId;
                    winnerTeamName = keyTeamName;
                }
            }

            if (winnerTeamName.Equals(""))
            {
                MatchInfo key = new MatchInfo(gameSessionId, teamName);
                dict[key].isWinner = true;
                return true;
            }
            else
            {
                MatchInfo key = new MatchInfo(gameSessionId, teamName);
                dict[key].isWinner = false;
                return false;
            }
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
        public Dictionary<string, UserData> userInfo = new Dictionary<string, UserData>();
        public List<int> roundList = new List<int>();
        public long sunriseTime = 0;
        public bool isWinner = false;

        public GameData(Dictionary<string, UserData> userInfo, List<int> roundList, long sunriseTime, bool isWinner)
        {
            foreach (KeyValuePair<string, UserData> keyValuePair in userInfo)
            {
                UserData userData = new UserData(keyValuePair.Value.roundNum, keyValuePair.Value.mbti, keyValuePair.Value.animal);
                this.userInfo[keyValuePair.Key] = userData;
            }
            foreach (int roundNum in roundList)
            {
                this.roundList.Add(roundNum);
            }
            this.sunriseTime = sunriseTime;
            this.isWinner = isWinner;
        }
    }

    public class UserData
    {
        public int roundNum;
        public string mbti;
        public int animal;

        public UserData(int roundNum, string mbti, int animal)
        {
            this.roundNum = roundNum;
            this.mbti = mbti;
            this.animal = animal;
        }
    }
}
