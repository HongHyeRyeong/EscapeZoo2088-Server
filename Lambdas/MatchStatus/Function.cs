using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using GameLiftWrapper;
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
            var clientInfo = GameInfo.Instance();



            var match_response = await client.DescribeMatchmakingAsync(new DescribeMatchmakingRequest
            {
                TicketIds = req.ticketIds
            });

            var ticketInfo = match_response.TicketList[0];

            if (ticketInfo.Status == "COMPLETED")
            {
                string ipaddr = ticketInfo.GameSessionConnectionInfo.IpAddress;
                int Port = ticketInfo.GameSessionConnectionInfo.Port;
                string TeamName = ticketInfo.Players[0].Team;
                string Gamesessionid = ticketInfo.GameSessionConnectionInfo.GameSessionArn;
                

                foreach (MatchedPlayerSession psess in ticketInfo.GameSessionConnectionInfo.MatchedPlayerSessions)
                {
                    res.IpAddress = ipaddr;
                    res.PlayerSessionId = psess.PlayerSessionId;
                    if (TeamName == "blue")
                        res.Port = Port;
                    else
                        res.Port = Port + 1;
                    res.TeamName = TeamName;
                    res.Gamesessionid = Gamesessionid;

                    //gameInfo¿¡ °ª ³Ö¾îÁÜ
                    clientInfo.addUser(Gamesessionid, TeamName, req.userId,req.mbti,req.character, -1);
                    res.roundList = clientInfo.getRoundList(Gamesessionid, TeamName);
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
