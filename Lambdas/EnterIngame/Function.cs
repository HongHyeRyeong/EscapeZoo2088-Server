using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using GameDB;
using GameLiftWrapper;
using GameDB;
using System.Text;
using System.Threading;
using System.Linq;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace EnterIngame
{
    public class Function
    {
        public Function()
        {
            DBEnv.SetUp();
        }
        private readonly int ROUND_MATCHMAKING_END = -1;

        public async Task<ResEnterIngame> FunctionHandler(ReqEnterIngame req, ILambdaContext context)
        {
            var res = new ResEnterIngame
            {
                ResponseType = ResponseType.Fail
            };

            DateTime currentDateTime = DateTime.UtcNow;
            int totalUserCount = req.teamUserCount*2;

            long checkUserCount = 0;
            using (var db = new DBConnector())
            {
                var query = new StringBuilder();
                query.Append("select count(*) as result from gameInfo where gameSessionId = '")
                .Append(req.gameSessionId).Append("' and roundNum = -1;");

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


                    using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                    {
                        if (cursor.Read())
                        {
                            checkUserCount = (long)cursor["result"];
                        }
                    }
                    Thread.Sleep(5000);
                }

                List<PlayerInfos> playerList = new List<PlayerInfos>();

                query.Clear();
                query.Append("SELECT mbti,animal,userid FROM gameInfo where gameSessionId = '")
                    .Append(req.gameSessionId).Append("' and teamName = '").Append(req.teamName).Append("';");

                using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                {
                    for(int i=0; i<req.teamUserCount; i++)
                    {
                        if (cursor.Read())
                        {
                            string mbti = cursor["mbti"].ToString();
                            int animal = (int)cursor["animal"];
                            string userId = cursor["userid"].ToString();
                            PlayerInfos playerInfo = new PlayerInfos(userId, mbti, animal);
                            playerList.Add(playerInfo);
                        }
                    }
                }

                string strRound = "";
                query.Clear();
                query.Append("SELECT * FROM gameInfo WHERE gameSessionId = '")
                    .Append(req.gameSessionId).Append("' AND teamName = '")
                    .Append(req.teamName).Append("' AND userid = '")
                    .Append(req.userId).Append("';");
                using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                {
                    if (cursor.Read())
                    {
                        strRound = cursor["roundList"].ToString();
                    }
                }
                await db.ExecuteNonQueryAsync(query.ToString());
                string[] roundArray = strRound.Split("|");
                foreach (string num in roundArray)
                {
                    res.roundList.Add(int.Parse(num));
                }

                Console.WriteLine("Success");
                Console.WriteLine("ResponseType:" + ResponseType.Success);
                Console.WriteLine("gameSessionId:" + req.gameSessionId);
                Console.WriteLine("teamName:" + req.teamName);

                res.ResponseType = ResponseType.Success;
                res.gameSessionId = req.gameSessionId;
                res.teamName = req.teamName;
                res.playerInfos = playerList;
            }

            return res;
        }
    }
}
