using Amazon.Lambda.Core;
using CommonProtocol;
using GameDB;
using System.Text;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace myPage
{
    public class Function
    {
        public Function()
        {
            DBEnv.SetUp();
        }

        public async Task<ResMyPage> FunctionHandler(ReqMyPage req, ILambdaContext context)
        {
            var res = new ResMyPage
            {
                ResponseType = ResponseType.Success
            };

            using (var db = new DBConnector())
            {
                var query = new StringBuilder();
                query.Append("SELECT * FROM users WHERE userid ='")
                    .Append(req.userId).Append("';");

                using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                {
                    if (cursor.Read())
                    {
                        res.userId = cursor["userId"].ToString();
                        res.mbti = cursor["mbti"].ToString();
                        res.win = (int)cursor["win"];
                        res.loss = (int)cursor["loss"];
                        res.score = (int)cursor["score"];
                        return res;
                    }

                }
            }
            res.ResponseType = ResponseType.Fail;
            return res;
        }
    }
}
