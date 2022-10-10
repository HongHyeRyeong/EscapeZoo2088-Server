using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using StartGame;
using System.Net;
using Newtonsoft.Json;
using CommonProtocol;

namespace StartGame.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestToStartGame()
        {
            var webClient = new WebClient();

            var req = new ReqStartGame
            {
                gameSessionId = "gameSessionId_1",
                teamName = "teamName_red",
                playerSessionId = "playerSessionId_1",
                userId = "userId_1",
                mbti = "infj",
                animal = 1,
                preRoundNum = 0,
                endRoundNum = 7,
                teamUserCount = 2
            };
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            var responseBytes
                = webClient.UploadString(new Uri("https://opupgoihqd.execute-api.ap-northeast-2.amazonaws.com/test/") + "StartGame", "POST"
                , JsonConvert.SerializeObject(req));

            var res = JsonConvert.DeserializeObject<ResStartGame>(responseBytes);
            Assert.True(res.ResponseType == ResponseType.Success);
        }
    }
}
