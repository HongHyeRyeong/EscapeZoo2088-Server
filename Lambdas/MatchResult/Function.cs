using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using CommonProtocol;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using GameDB;
using System;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace MatchResult
{
    public class Function
    {
        public async Task<ResMatchResult> FunctionHandler(ReqMatchResult req, ILambdaContext context)
        {
            DBEnv.SetUp();
            var res = new ResMatchResult
            {
                ResponseType = ResponseType.Success
            };

            using (var db = new DBConnector())
            {
                var query = new StringBuilder();
                query.Append("SELECT * FROM users where userid = '")
                    .Append(req.userId).Append("';");

                int win = 0;
                int loss = 0;
                int score = 0;

                using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                {
                    if (cursor.Read())
                    {
                        win =(int) cursor["win"];
                        loss = (int)cursor["loss"];
                        score = (int)cursor["score"];
                    }
                }

                if (req.isWinner)
                {
                    win += 1;
                    score += 10;
                }
                else
                {
                    loss -= 1;
                    score -= 10;
                }

                res.score = score;
                query.Clear();
                query.Append("update users set win = '")
                .Append(win).Append("',loss ='").Append(loss).Append("',score ='").Append(score)
                .Append("' where userid = '").Append(req.userId).Append("';");
                await db.ExecuteNonQueryAsync(query.ToString());
            }
            return res;
            //��� ������Ʈ �¸��� +10 �й�� -10

        }
    }
}
