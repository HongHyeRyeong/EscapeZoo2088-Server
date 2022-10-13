using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using GameDB;
using GameLiftWrapper;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace ExitGame
{
    public class Function
    {
        public Function()
        {
            DBEnv.SetUp();
        }

        public async Task<ResExitGame> FunctionHandler(ReqExitGame req, ILambdaContext context)
        {
            var res = new ResExitGame
            {
                ResponseType = ResponseType.Fail
            };

            //GameInfo gameInfo = GameInfo.Instance();
            //int remainUserCount = gameInfo.removeUser(req.gameSessionId, req.teamName, req.userId);
            bool isExistUser = false;
            var db = new DBConnector();
            //using (var db = new DBConnector())
            {
                var query = new StringBuilder();
                query.Append("SELECT * gameInfo WHERE gameSessionId = '")
                    .Append(req.gameSessionId).Append("' AND teamName = '")
                    .Append(req.teamName).Append("' AND userid = '")
                    .Append(req.userId).Append("';");
                using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                {
                    if (cursor.Read())
                    {
                        isExistUser = true;
                    }
                }
                if (!isExistUser)
                {
                    Console.WriteLine("NOT EXIST User");

                    db.Dispose();
                    return res;
                }

                query.Clear();
                query.Append("UPDATE gameInfo SET gameSessionId = '")
                    .Append(req.userId).Append("', teamName = '")
                    .Append(req.userId).Append("', status = 'matching' ")
                    .Append("WHERE userid = '").Append(req.userId).Append("';");
                await db.ExecuteNonQueryAsync(query.ToString());
            }

            db.Dispose();

            Console.WriteLine("Success");

            res.ResponseType = ResponseType.Success;
            Console.WriteLine("ResponseType:" + ResponseType.Success);
            
            return res;
        }
    }
}
