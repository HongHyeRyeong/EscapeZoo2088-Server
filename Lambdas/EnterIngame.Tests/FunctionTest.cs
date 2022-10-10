using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using CreateGame;
using System.Net;
using Newtonsoft.Json;
using CommonProtocol;

namespace CreateGame.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestToEnterIngame()
        {
            var webClient = new WebClient();

            var req = new ReqEnterIngame
            {
                gameSessionId = "gameSessionId_1",
                teamName = "teamName_red",
                teamUserCount = 2
            };
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            var responseBytes
                = webClient.UploadString(new Uri("https://opupgoihqd.execute-api.ap-northeast-2.amazonaws.com/test/") + "EnterIngame", "POST"
                , JsonConvert.SerializeObject(req));

            var res = JsonConvert.DeserializeObject<ResEnterIngame>(responseBytes);
            Assert.True(res.ResponseType == ResponseType.Success || res.ResponseType == ResponseType.DuplicateName);
        }
    }
}
