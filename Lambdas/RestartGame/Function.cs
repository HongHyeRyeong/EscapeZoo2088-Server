using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using GameDB;
using GameLiftWrapper;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace RestartGame
{
    public class Function
    {
        public Function()
        {
            DBEnv.SetUp();
        }

        public async Task<ResRestartGame> FunctionHandler(ReqRestartGame req, ILambdaContext context)
        {
            var res = new ResRestartGame
            {
                ResponseType = ResponseType.Fail
            };

            //GameInfo gameInfo = GameInfo.Instance();
            int currentRoundNum = req.currentRoundNum + 10;
            //gameInfo.addUser(req.gameSessionId, req.teamName, req.userId, req.mbti, req.animal, currentRoundNum);
            bool isSuccessRoundNum = false;
            using (var db = new DBConnector())
            {
                var query = new StringBuilder();
                query.Append("UPDATE gameInfo SET roundNum = ")
                    .Append(currentRoundNum)
                    .Append(" WHERE userid = '")
                    .Append(req.userId).Append("';");
                await db.ExecuteNonQueryAsync(query.ToString());
                isSuccessRoundNum = true;
                if (!isSuccessRoundNum)
                {
                    Console.WriteLine("Fail update RoundNum");
                    return res;
                }

                DateTime currentDateTime = DateTime.UtcNow;
                int totalUserCount = req.teamUserCount;

                int checkUserCount = 0;
                while (checkUserCount < totalUserCount)
                {
                    DateTime checkDateTime = DateTime.UtcNow;
                    TimeSpan dateDiff = checkDateTime - currentDateTime;
                    int diffSecond = dateDiff.Seconds;
                    if (diffSecond < 1)
                    {
                        continue;
                    }
                    currentDateTime = checkDateTime;

                    //checkUserCount = gameInfo.getUserCountInRound(req.gameSessionId, req.teamName, currentRoundNum);
                    query.Clear();
                    query.Append("SELECT * FROM gameInfo where gameSessionId = '")
                       .Append(req.gameSessionId).Append("' AND teamName = '")
                       .Append(req.teamName).Append("' AND roundNum = ")
                       .Append(currentRoundNum).Append(";");
                    using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                    {
                        if (cursor.Read())
                        {
                            checkUserCount++;
                        }
                    }
                    await db.ExecuteNonQueryAsync(query.ToString());
                    if (checkUserCount == 0)
                    {
                        break;
                    }
                }

                if (checkUserCount == 0)
                {
                    Console.WriteLine("checkUserCount is Zero");
                    return res;
                }
                Console.WriteLine("Success");

                res.ResponseType = ResponseType.Success;
                res.gameSessionId = req.gameSessionId;
                res.teamName = req.teamName;
                res.playerSessionId = req.playerSessionId;
                res.userId = req.userId;
                res.currentRoundNum = currentRoundNum;
                res.endRoundNum = req.endRoundNum;

                //List<int> roundList = gameInfo.getRoundList(req.gameSessionId, req.teamName);
                res.roundList = new List<int>();
                string roundListInfo = "";
                query.Clear();
                query.Append("SELECT * FROM gameInfo where gameSessionId = '")
                    .Append(req.gameSessionId).Append("' AND teamName = '")
                    .Append(req.teamName).Append("';");
                using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                {
                    if (cursor.Read())
                    {
                        roundListInfo = cursor["roundList"].ToString();
                        res.sunriseTime = Convert.ToInt64(cursor["sunriseTime"].ToString());
                    }
                }
                await db.ExecuteNonQueryAsync(query.ToString());
                string[] roundStr = roundListInfo.Split('|');
                foreach (var round in roundStr)
                {
                    res.roundList.Add(Convert.ToInt32(round));
                }
                //foreach (int num in roundList)
                //{
                //    res.roundList.Add(num);
                //    roundListInfo += num + " | ";
                //}

                int enemyRoundMax = 0;
                //Dictionary<string, int> enemyRoundDict = gameInfo.getEnemyRound(req.gameSessionId, req.teamName);
                //foreach (KeyValuePair<string, int> keyValuePair in enemyRoundDict)
                //{
                //    enemyRoundValue = keyValuePair.Value;
                //    break;
                //}
                query.Clear();
                query.Append("SELECT * FROM gameInfo where gameSessionId = '")
                    .Append(req.gameSessionId).Append("' AND teamName != '")
                    .Append(req.teamName).Append("';");
                using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                {
                    if (cursor.Read())
                    {
                        int result = Convert.ToInt32(cursor["roundNum"].ToString());
                        if (enemyRoundMax < result)
                        {
                            enemyRoundMax = result;
                        }
                    }
                }
                await db.ExecuteNonQueryAsync(query.ToString());
                res.enemyRoundNum = enemyRoundMax;

                //res.sunriseTime = gameInfo.getSunriseTime(req.gameSessionId, req.teamName);

                Console.WriteLine("Success");
                Console.WriteLine("ResponseType:" + ResponseType.Success);
                Console.WriteLine("gameSessionId:" + req.gameSessionId);
                Console.WriteLine("teamName:" + req.teamName);
                Console.WriteLine("playerSessionId:" + req.playerSessionId);
                Console.WriteLine("userId:" + req.userId);
                Console.WriteLine("currentRoundNum:" + currentRoundNum);
                Console.WriteLine("endRoundNum:" + req.endRoundNum);
                Console.WriteLine("roundList:" + roundListInfo);
                Console.WriteLine("enemyRoundNum:" + enemyRoundMax);
                Console.WriteLine("sunriseTime:" + res.sunriseTime);
                return res;
            }
        }
    }
}
