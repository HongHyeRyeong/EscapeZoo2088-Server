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
        public void TestTomyPage()
        {

            var webClient = new WebClient();

            var req = new ReqMyPage
            {
                userId = "ksh"
            };
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            var responseBytes
                = webClient.UploadString(new Uri("https://opupgoihqd.execute-api.ap-northeast-2.amazonaws.com/test/") + "myPage", "POST"
                , JsonConvert.SerializeObject(req));

            var res = JsonConvert.DeserializeObject<ResMyPage>(responseBytes);
            Assert.True(res.ResponseType == ResponseType.Success || res.ResponseType == ResponseType.DuplicateName);
        }
    }
}
