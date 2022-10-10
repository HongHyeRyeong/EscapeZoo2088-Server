using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using System;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace LastRound
{
    public class Function
    {
        public async Task<ResLastRound> FunctionHandler(ReqLastRound req, ILambdaContext context)
        {
            var res = new ResLastRound
            {
                ResponseType = ResponseType.Success
            };



            res.GameSessionArn = req.GameSessionArn;
            res.isWinner = req.TeamName;
            res.userId = req.userId;

            return res;

        }
    }
}
