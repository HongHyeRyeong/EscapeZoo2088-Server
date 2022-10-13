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
        public Function()
        {
            DBEnv.SetUp();
        }


        public async Task<ResAccountJoin> FunctionHandler(ReqAccountJoin req, ILambdaContext context)
        {
            var res = new ResAccountJoin
            {
                ResponseType = ResponseType.Success
            };

            var db = new DBConnector();
            //using (var db = new DBConnector())
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

                        db.Dispose();
                        return res;
                    }

                }
            }
            db.Dispose();
            res.ResponseType = ResponseType.Fail;
            return res;
        }
    }
}
