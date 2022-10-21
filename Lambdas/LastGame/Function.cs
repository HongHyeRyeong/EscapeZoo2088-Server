using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using System;
using GameLiftWrapper;
using System.Collections.Generic;
using GameDB;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace LastGame
{
    public class Function
    {
        public Function()
        {
            DBEnv.SetUp();
        }

        public async Task<ResLastGame> FunctionHandler(ReqLastGame req, ILambdaContext context)
        {
            var res = new ResLastGame
            {
                ResponseType = ResponseType.Fail
            };

            //GameInfo gameInfo = GameInfo.Instance();
            int clearRoundNum = req.currentRoundNum + 1;
            //gameInfo.addUser(req.gameSessionId, req.teamName, req.userId, req.mbti, req.animal, clearRoundNum);
            bool isSuccessRoundNum = false;
            var db = new DBConnector();
            //using (var db = new DBConnector())
            {
                var query = new StringBuilder();
                query.Append("UPDATE gameInfo SET roundNum = ")
                    .Append(clearRoundNum)
                    .Append(" WHERE userid = '")
                    .Append(req.userId).Append("';");
                await db.ExecuteNonQueryAsync(query.ToString());
                isSuccessRoundNum = true;
                if (!isSuccessRoundNum)
                {
                    Console.WriteLine("Fail update RoundNum");

                    db.Dispose();
                    return res;
                }

                DateTime currentDateTime = DateTime.UtcNow;
                int totalUserCount = req.teamUserCount;
                int checkUserCount = 0;
                while (checkUserCount < totalUserCount)
                {
                    DateTime checkDateTime = DateTime.UtcNow;
                    TimeSpan dateDiff = checkDateTime - currentDateTime;
                    int diffSecond = dateDiff.Milliseconds;
                    if (diffSecond < 100)
                    {
                        continue;
                    }
                    currentDateTime = checkDateTime;
                    query.Clear();
                    query.Append("SELECT * FROM gameInfo where gameSessionId = '")
                       .Append(req.gameSessionId).Append("' AND teamName = '")
                       .Append(req.teamName).Append("' AND roundNum = ")
                       .Append(clearRoundNum).Append(";");
                    using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                    {
                        for (int i = 0; i < req.teamUserCount; i++)
                        {
                            if (cursor.Read())
                            {
                                checkUserCount++;
                            }
                        }
                    }

                    //checkUserCount = gameInfo.getUserCountInRound(req.gameSessionId, req.teamName, clearRoundNum);
                    query.Clear();
                    query.Append("SELECT * FROM gameInfo where gameSessionId = '")
                       .Append(req.gameSessionId).Append("' AND teamName = '")
                       .Append(req.teamName).Append("' AND roundNum = ")
                       .Append(clearRoundNum).Append(";");
                    using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                    {
                        for (int i = 0; i < req.teamUserCount; i++)
                        {
                            if (cursor.Read())
                            {
                                checkUserCount++;
                            }
                        }
                    }
                    if (checkUserCount == 0)
                    {
                        break;
                    }
                }
                if (checkUserCount == 0)
                {
                    Console.WriteLine("checkUserCount is Zero");

                    db.Dispose();
                    return res;
                }

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
                    for (int i = 0; i < req.teamUserCount; i++)
                    {
                        if (cursor.Read())
                        {
                            int result = int.Parse(cursor["roundNum"].ToString());
                            if (enemyRoundMax < result)
                            {
                                enemyRoundMax = result;
                            }
                        }
                    }
                }

                bool isWinner = true;
                query.Clear();
                query.Append("SELECT * FROM gameInfo where gameSessionId = '")
                    .Append(req.gameSessionId).Append("' AND teamName != '")
                    .Append(req.teamName).Append("' AND isWinner = ")
                    .Append(1).Append(";");
                using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                {
                    for (int i = 0; i < req.teamUserCount; i++)
                    {
                        if (cursor.Read())
                        {
                            isWinner = false;
                        }
                    }
                }
                if (isWinner)
                {
                    query.Clear();
                    query.Append("UPDATE gameInfo SET isWinner = ").Append(1)
                        .Append(" WHERE userid = '")
                        .Append(req.userId).Append("';");
                    await db.ExecuteNonQueryAsync(query.ToString());
                }

                db.Dispose();

                res.ResponseType = ResponseType.Success;
                res.gameSessionId = req.gameSessionId;
                res.teamName = req.teamName;
                res.userId = req.userId;
                res.enemyRoundNum = enemyRoundMax;
                res.isWinner = isWinner;

                Console.WriteLine("Success");
                Console.WriteLine("ResponseType:" + ResponseType.Success);
                Console.WriteLine("gameSessionId:" + req.gameSessionId);
                Console.WriteLine("teamName:" + req.teamName);
                Console.WriteLine("userId:" + req.userId);
                Console.WriteLine("enemyRoundNum:" + enemyRoundMax);
                Console.WriteLine("isWinner:" + isWinner);

                return res;
            }
        }
    }
}
