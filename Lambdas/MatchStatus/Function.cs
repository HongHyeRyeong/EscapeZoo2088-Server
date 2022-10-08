using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using System;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace MatchStatus
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

            Console.WriteLine("ticketInfo = " + ticketInfo.TicketId);
            Console.WriteLine("ticketInfoStatus = " + ticketInfo.Status);
           // ticketInfo.Players[0].Team;
            if (ticketInfo.Status == "COMPLETED")
            {
                string ipaddr = ticketInfo.GameSessionConnectionInfo.IpAddress;
                int Port = ticketInfo.GameSessionConnectionInfo.Port;
                string TeamName = ticketInfo.Players[0].Team;
                string GameSessionArn = ticketInfo.GameSessionConnectionInfo.GameSessionArn;
                foreach (MatchedPlayerSession psess in ticketInfo.GameSessionConnectionInfo.MatchedPlayerSessions)
                {
                    res.IpAddress = ipaddr;
                    res.PlayerSessionId = psess.PlayerSessionId;
                    res.Port = Port;
                    res.TeamName = TeamName;
                    res.GameSessionArn = GameSessionArn;
                    break;
                }
            }
            else
            {
                res.ResponseType = ResponseType.Proceeding;
            }

            return res;

        }
    }
}
