using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using GameLiftWrapper;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace EnterIngame
{
    public class Function
    {
        private readonly int ROUND_MATCHMAKING_END = -1;

        public async Task<ResEnterIngame> FunctionHandler(ReqEnterIngame req, ILambdaContext context)
        {
            var res = new ResEnterIngame
            {
                ResponseType = ResponseType.Fail
            };

            GameInfo gameInfo = GameInfo.Instance();

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

                checkUserCount = gameInfo.getUserCountInRound(req.gameSessionId, req.teamName, ROUND_MATCHMAKING_END);

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

            Dictionary<string, UserData> userListInTeam = gameInfo.getUserListInTeam(req.gameSessionId, req.teamName);
            List<PlayerInfos> playerList = new List<PlayerInfos>();
            foreach (KeyValuePair<string, UserData> keyValuePair in userListInTeam)
            {
                string key = keyValuePair.Key;
                UserData value = keyValuePair.Value;
                if (value.roundNum == ROUND_MATCHMAKING_END)
                {
                    string mbti = value.mbti;
                    int animal = value.animal;
                    PlayerInfos playerInfo = new PlayerInfos(key, mbti, animal);
                    playerList.Add(playerInfo);
                }
            }

            Console.WriteLine("Success");

            res.ResponseType = ResponseType.Success;
            res.gameSessionId = req.gameSessionId;
            res.teamName = req.teamName;

            string strPlayerInfo = "";
            foreach (PlayerInfos info in playerList)
            {
                res.playerInfos.Add(info);
                strPlayerInfo += "(" + info.userId + ", " + info.mbti + ", " + info.animal + ") ";
            }

            Console.WriteLine("ResponseType:" + ResponseType.Success);
            Console.WriteLine("gameSessionId:" + req.gameSessionId);
            Console.WriteLine("teamName:" + req.teamName);
            Console.WriteLine("playerInfos: " + strPlayerInfo);

            return res;
        }
    }
}
