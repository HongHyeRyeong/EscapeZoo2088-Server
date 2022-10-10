using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using GameLiftWrapper;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace StartGame
{
    public class Function
    {
        public async Task<ResStartGame> FunctionHandler(ReqStartGame req, ILambdaContext context)
        {
            var res = new ResStartGame
            {
                ResponseType = ResponseType.Fail
            };

            GameInfo gameInfo = GameInfo.Instance();
            int currentRoundNum = req.preRoundNum + 1;
            gameInfo.addUser(req.gameSessionId, req.teamName, req.userId, req.mbti, req.animal, currentRoundNum);

            DateTime currentDateTime = DateTime.UtcNow;
            int totalUserCount = req.teamUserCount;
            if (req.preRoundNum == 0)
            {
                totalUserCount *= 2;
            }

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

                if (req.preRoundNum == 0)
                {
                    checkUserCount = gameInfo.getUserCountInGame(req.gameSessionId);
                }
                else
                {
                    checkUserCount = gameInfo.getUserCountInRound(req.gameSessionId, req.teamName, currentRoundNum);
                }

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
            
            List<int> roundList = gameInfo.getRoundList(req.gameSessionId, req.teamName);
            res.roundList = new List<int>();
            string roundListInfo = "";
            foreach (int num in roundList)
            {
                res.roundList.Add(num);
                roundListInfo += num + " | ";
            }

            int enemyRoundValue = 0;
            Dictionary<string, int> enemyRoundDict = gameInfo.getEnemyRound(req.gameSessionId, req.teamName);
            foreach (KeyValuePair<string, int> keyValuePair in enemyRoundDict)
            {
                enemyRoundValue = keyValuePair.Value;
                break;
            }
            res.enemyRoundNum = enemyRoundValue;

            res.sunriseTime = gameInfo.getSunriseTime(req.gameSessionId, req.teamName);

            Console.WriteLine("Success");
            Console.WriteLine("ResponseType:" + ResponseType.Success);
            Console.WriteLine("gameSessionId:" + req.gameSessionId);
            Console.WriteLine("teamName:" + req.teamName);
            Console.WriteLine("playerSessionId:" + req.playerSessionId);
            Console.WriteLine("userId:" + req.userId);
            Console.WriteLine("currentRoundNum:" + currentRoundNum);
            Console.WriteLine("endRoundNum:" + req.endRoundNum);
            Console.WriteLine("roundList:" + roundListInfo);
            Console.WriteLine("enemyRoundNum:" + enemyRoundValue);
            Console.WriteLine("sunriseTime:" + res.sunriseTime);
            return res;
        }
    }
}
