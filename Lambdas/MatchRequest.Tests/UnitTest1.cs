using System;
using System.Threading;

using Xunit;

using System.Net;
using CommonProtocol;
using Newtonsoft.Json;

namespace MatchRequest.Tests
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

            if(res.ResponseType == ResponseType.Success || res.ResponseType == ResponseType.DuplicateName)
            {
                Thread.Sleep(1000);
                var reqSt = new ReqMatchStatus
                {
                    ticketIds = { res.ticketId }
                };
                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                var responseBytesSt
                    = webClient.UploadString(new Uri("https://opupgoihqd.execute-api.ap-northeast-2.amazonaws.com/test/") + "MatchStatus", "POST"
                    , JsonConvert.SerializeObject(reqSt));

                var resSt = JsonConvert.DeserializeObject<ResMatchStatus>(responseBytesSt);
                Console.WriteLine(resSt);
                Assert.True(resSt.ResponseType == ResponseType.Proceeding || resSt.ResponseType == ResponseType.DuplicateName);
            }
        }
    }
}
