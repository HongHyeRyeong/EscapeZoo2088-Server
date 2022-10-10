using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using RestartGame;
using System.Net;
using Newtonsoft.Json;
using CommonProtocol;

namespace RestartGame.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestToRestartGame()
        {
            var webClient = new WebClient();

            var req = new ReqRestartGame
            {
                gameSessionId = "gameSessionId_1",
                teamName = "teamName_red",
                playerSessionId = "playerSessionId_1",
                userId = "userId_1",
                mbti = "infj",
                animal = 1,
                currentRoundNum = 1,
                endRoundNum = 7,
                teamUserCount = 2
            };
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            var responseBytes
                = webClient.UploadString(new Uri("https://opupgoihqd.execute-api.ap-northeast-2.amazonaws.com/test/") + "RestartGame", "POST"
                , JsonConvert.SerializeObject(req));

            var res = JsonConvert.DeserializeObject<ResStartGame>(responseBytes);
            Assert.True(res.ResponseType == ResponseType.Success);
        }
    }
}
