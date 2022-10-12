using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using GameLiftWrapper;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace ExitGame
{
    public class Function
    {
        public async Task<ResExitGame> FunctionHandler(ReqExitGame req, ILambdaContext context)
        {
            var res = new ResExitGame
            {
                ResponseType = ResponseType.Fail
            };

            GameInfo gameInfo = GameInfo.Instance();
            int remainUserCount = gameInfo.removeUser(req.gameSessionId, req.teamName, req.userId);
            
            Console.WriteLine("Success");

            res.ResponseType = ResponseType.Success;
            Console.WriteLine("ResponseType:" + ResponseType.Success);
            
            return res;
        }
    }
}
