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
        public void TestToMatchStatus()
        {

            var webClient = new WebClient();

            var req = new ReqMatchStatus
            {
                ticketIds = { "b5c97566-5117-46ec-a706-8e09ed9a1f2d" }
            };
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            var responseBytes
                = webClient.UploadString(new Uri("https://opupgoihqd.execute-api.ap-northeast-2.amazonaws.com/test/") + "MatchStatus", "POST"
                , JsonConvert.SerializeObject(req));

            var res = JsonConvert.DeserializeObject<ResMatchStatus>(responseBytes);
            Assert.True(res.ResponseType == ResponseType.Success || res.ResponseType == ResponseType.DuplicateName);
        }
    }
}
