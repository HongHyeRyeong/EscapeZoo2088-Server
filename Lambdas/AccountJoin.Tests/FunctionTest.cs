using System;
using Xunit;
using CommonProtocol;
using System.Net;
using Newtonsoft.Json;

namespace AccountJoin.Tests
{

    public class FunctionTest
    {
        [Fact]
        public void TestToAccountJoin()
        {
            var webClient = new WebClient();

            var req = new ReqAccountJoin
            {
                userId = "ksh",
                password = "chqhaks123",
                mbti = "inft"
            };
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            var responseBytes
                = webClient.UploadString(new Uri("https://opupgoihqd.execute-api.ap-northeast-2.amazonaws.com/test/") + "AccountJoin", "POST"
                , JsonConvert.SerializeObject(req));

            var res = JsonConvert.DeserializeObject<ResAccountJoin>(responseBytes);
            Assert.True(res.ResponseType == ResponseType.Success || res.ResponseType == ResponseType.DuplicateName);
        }
    }
}
