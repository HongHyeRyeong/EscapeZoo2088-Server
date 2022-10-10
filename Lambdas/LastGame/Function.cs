using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using System;
using GameLiftWrapper;
using System.Collections.Generic;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace LastGame
{
    public class Function
    {
        public async Task<ResLastGame> FunctionHandler(ReqLastGame req, ILambdaContext context)
        {
            var res = new ResLastGame
            {
                ResponseType = ResponseType.Fail
            };

            GameInfo gameInfo = GameInfo.Instance();
            int clearRoundNum = req.currentRoundNum + 1;
            gameInfo.addUser(req.gameSessionId, req.teamName, req.userId, req.mbti, req.animal, clearRoundNum);

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

                checkUserCount = gameInfo.getUserCountInRound(req.gameSessionId, req.teamName, clearRoundNum);

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

            int enemyRoundValue = 0;
            Dictionary<string, int> enemyRoundDict = gameInfo.getEnemyRound(req.gameSessionId, req.teamName);
            foreach (KeyValuePair<string, int> keyValuePair in enemyRoundDict)
            {
                enemyRoundValue = keyValuePair.Value;
                break;
            }

            bool isWinner = gameInfo.isWinner(req.gameSessionId, req.teamName);

            res.ResponseType = ResponseType.Success;
            res.gameSessionId = req.gameSessionId;
            res.teamName = req.teamName;
            res.userId = req.userId;
            res.enemyRoundNum = enemyRoundValue;
            res.isWinner = isWinner;

            Console.WriteLine("Success");
            Console.WriteLine("ResponseType:" + ResponseType.Success);
            Console.WriteLine("gameSessionId:" + req.gameSessionId);
            Console.WriteLine("teamName:" + req.teamName);
            Console.WriteLine("userId:" + req.userId);
            Console.WriteLine("enemyRoundNum:" + enemyRoundValue);
            Console.WriteLine("isWinner:" + isWinner);

            return res;

        }
    }
}
