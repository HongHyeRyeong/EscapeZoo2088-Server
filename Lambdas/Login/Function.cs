using Amazon.Lambda.Core;
using CommonProtocol;
using GameDB;
using System.Text;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(CustomSerializer.LambdaSerializer))]

namespace Login
{
    public class Function
    {
        public async Task<ResAccountJoin> FunctionHandler(ReqAccountJoin req, ILambdaContext context)
        {
            DBEnv.SetUp();
            var res = new ResAccountJoin
            {
                ResponseType = ResponseType.Success
            };

            using (var db = new DBConnector())
            {
                var query = new StringBuilder();
                query.Append("SELECT userid,score FROM users WHERE userid ='")
                    .Append(req.userId).Append("' and password = '").Append(req.password).Append("';");

                using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                {
                    if (cursor.Read())
                    {
                        res.userId = cursor["userId"].ToString();
                        res.score = (int) cursor["score"];
                        return res;
                    }

                }
            }
            res.ResponseType = ResponseType.Fail;
            return res;
        }
    }
}
