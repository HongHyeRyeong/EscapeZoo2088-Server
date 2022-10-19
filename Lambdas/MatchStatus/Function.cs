using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using System.Collections.Generic;
using System;
using System.Linq;
using GameDB;
using System.Text;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace MatchStatus
{
    public class Function
    {
        public Function()
        {
            DBEnv.SetUp();
        }

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
            //using (var db = new DBConnector())
            {
                if (ticketInfo.Status == "COMPLETED")
                {
                    string ipaddr = ticketInfo.GameSessionConnectionInfo.IpAddress;
                    int Port = ticketInfo.GameSessionConnectionInfo.Port;
                    string TeamName = ticketInfo.Players[0].Team;
                    string Gamesessionid = ticketInfo.GameSessionConnectionInfo.GameSessionArn;

                    Random randomObj = new Random();
                    List<int> roundList = new List<int>() { 0, 1, 2, 3};
                    //for (int i = 0; i < 4; i++)
                    //{
                    //    roundList.Add(i);
                    //}

                    //int random1, temp;
                    //for (int i = 0; i < roundList.Count; ++i)
                    //{
                    //    random1 = randomObj.Next(0, roundList.Count - 1);

                    //    temp = roundList[i];
                    //    roundList[i] = roundList[random1];
                    //    roundList[random1] = temp;
                    //}

                    List<string> strRoundList = roundList.Select(i => i.ToString()).ToList();
                    string strRound = string.Join("|", strRoundList);

                    long sunriseTime = 3; // randomObj.Next(10, 25);
                    string strSunriseTime = sunriseTime.ToString();

                    foreach (MatchedPlayerSession psess in ticketInfo.GameSessionConnectionInfo.MatchedPlayerSessions)
                    {
                        res.IpAddress = ipaddr;
                        res.PlayerSessionId = psess.PlayerSessionId;
                        res.Port = Port;
                        res.TeamName = TeamName;
                        res.Gamesessionid = Gamesessionid;
                        res.roundList = roundList;
                        break;
                    }

                    var db = new DBConnector();
                    var query = new StringBuilder();
                    query.Append("update gameInfo set gameSessionId = '")
                    .Append(Gamesessionid).Append("',teamName ='").Append(TeamName).Append("',animal =").Append(req.animal)
                    .Append(",roundNum = -1").Append(",roundList ='").Append(strRound)
                    .Append("',sunriseTime =").Append(sunriseTime)
                    .Append(" where userid = '").Append(req.userId).Append("';");
                    await db.ExecuteNonQueryAsync(query.ToString());

                    db.Dispose();
                }
                else
                {
                    res.ResponseType = ResponseType.Proceeding;
                }
            }
            

            return res;

        }
    }
}
