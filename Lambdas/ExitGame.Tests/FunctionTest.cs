using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using ExitGame;
using System.Net;
using Newtonsoft.Json;
using CommonProtocol;

namespace ExitGame.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestToExitGame()
        {
            var webClient = new WebClient();

            var req = new ReqExitGame
            {
                gameSessionId = "gameSessionId_1",
                teamName = "teamName_red",
                playerSessionId = "playerSessionId_1",
                userId = "userId_1",
                currentRoundNum = 2,
                teamUserCount = 2
            };
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            var responseBytes
                = webClient.UploadString(new Uri("https://opupgoihqd.execute-api.ap-northeast-2.amazonaws.com/test/") + "ExitGame", "POST"
                , JsonConvert.SerializeObject(req));

            var res = JsonConvert.DeserializeObject<ResExitGame>(responseBytes);
            Assert.True(res.ResponseType == ResponseType.Success);
        }
    }
}
