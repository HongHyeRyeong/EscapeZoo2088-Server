using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using System.Collections.Generic;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace MatchRequest
{
    public class Function
    {
        public async Task<ResMatchRequest> FunctionHandler(ReqMatchRequest req, ILambdaContext context)
        {
            var res = new ResMatchRequest
            {
                ResponseType = ResponseType.Success
            };

            var client = new AmazonGameLiftClient();
            var scoreValue = new AttributeValue();
            scoreValue.N = req.score;
            var gameMapValue = new AttributeValue();
            gameMapValue.N = req.gameMap;

            var Dtemp = new Dictionary<string, AttributeValue>();
            Dtemp.Add("score", scoreValue);
            Dtemp.Add("gameMap", gameMapValue);


            var match_response = await client.StartMatchmakingAsync(new StartMatchmakingRequest
            {
                ConfigurationName = "escapeZooMatchConfig",
                Players = new List<Player>
                {
                    new Player
                    {
                        PlayerId = req.userId,
                        PlayerAttributes = Dtemp
                    }
                }
            });

            res.ticketId = match_response.MatchmakingTicket.TicketId;

            if(res.ticketId != null)
                return res;

            res.ResponseType = ResponseType.Fail;
            return res;
        }
    }
}
