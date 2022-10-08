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
        public async Task<ResMatchStatus> FunctionHandler(ReqMatchStatus req, ILambdaContext context)
        {
            var res = new ResMatchStatus
            {
                ResponseType = ResponseType.Success
            };

            var client = new AmazonGameLiftClient();

            var match_response = await client.DescribeMatchmakingAsync(new DescribeMatchmakingRequest
            {
                TicketIds = req.ticketIds
            });

            var ticketInfo = match_response.TicketList[0];
            string matchTicket = ticketInfo.TicketId;

            if(ticketInfo.Status == "COMPLETED")
            {
                string ipaddr = ticketInfo.GameSessionConnectionInfo.IpAddress;
                int Port = ticketInfo.GameSessionConnectionInfo.Port;
                foreach(MatchedPlayerSession psess in ticketInfo.GameSessionConnectionInfo.MatchedPlayerSessions)
                {
                    res.IpAddress = ipaddr;
                    res.PlayerSessionId = psess.PlayerSessionId;
                    res.Port = Port;
                    break;
                }
            }
            else
            {
                res.ResponseType = ResponseType.Fail;
            }

            return res;
        }
    }
}
