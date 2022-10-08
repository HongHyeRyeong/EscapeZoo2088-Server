using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using System.Net;
using CommonProtocol;
using Newtonsoft.Json;

namespace myPage.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestToMatchRequest()
        {
            var webClient = new WebClient();

            var req = new ReqMatchRequest
            {
              userId = "value1",
              gameMap= 1,
              score=1000
            };
        webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            var responseBytes
                = webClient.UploadString(new Uri("https://opupgoihqd.execute-api.ap-northeast-2.amazonaws.com/test/") + "MatchRequest", "POST"
                , JsonConvert.SerializeObject(req));

            var res = JsonConvert.DeserializeObject<ResMatchRequest>(responseBytes);
            Assert.True(res.ResponseType == ResponseType.Success || res.ResponseType == ResponseType.DuplicateName);
        }
    }
}
