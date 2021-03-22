using Fanex.NAP.SPU.DCS.Features.UserAgent;
using Fanex.NAP.SPU.DCS.Features.UserAgent.Entities;
using Fanex.NAP.SPU.DCS.Shares;
using Fanex.NAP.SPU.DCS.Shares.Models;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UAParser;
using Xunit;

namespace Fanex.NAP.SPU.DCS.Test.Features.UserAgent
{
    public class UserAgentTests
    {
        private readonly IUserAgentDataAccess userAgentDataAccess;
        private readonly IUserAgentServices userAgentService;

        public UserAgentTests()
        {
            userAgentDataAccess = Substitute.For<IUserAgentDataAccess>();

            userAgentService = new UserAgentServices(userAgentDataAccess);
        }

        [Fact]
        public void GetUserAgent_WhenCalled_ReturnUserAgentList()
        {
            var userAgentEntities = new List<UserAgentEntity> {
                new UserAgentEntity
                {
                    CreatedDate = new DateTime(2020, 01, 09, 00, 00, 00),
                    UserAgent = "00140dfba5212b0f74c2114ebe01d719",
                    UserAgentKey = "Mozilla/5.0 (Linux; Android 10; SM-N770F) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Mobile Safari/537.36"
                },
                new UserAgentEntity
                {
                    CreatedDate = new DateTime(2020, 02, 09, 00, 00, 00),
                    UserAgent = "002a30d8a4e32ff1a501ea52875bff6c",
                    UserAgentKey = "Mozilla/5.0 (Linux; Android 9; ASK-AL00x) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.101 Mobile Safari/537.36"
                }            
            };


            userAgentDataAccess.GetUserAgent(100).Returns(userAgentEntities);
            var result = userAgentService.GetUserAgent(100);

            result.Should().BeEquivalentTo(userAgentEntities);
        }

        [Theory]
        [InlineData("xyz")]
        public void UpdateUserAgent_WhenSuccess_ReturnDataResult(string userAgentJson)
        {
            var expectedResut = new DataResult
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty
            };

            userAgentDataAccess.UpdateUserAgent(userAgentJson).Returns(expectedResut);
            var result = userAgentService.UpdateUserAgent(userAgentJson);

            result.ErrorCode.Should().Be(0);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void UpdateUserAgent_WhenFail_ReturnDataResult(string userAgentJson)
        {
            var expectedResut = new DataResult
            {
                ErrorCode = -1,
                ErrorMessage = "Parameter is empty"
            };

            userAgentDataAccess.UpdateUserAgent(userAgentJson).Returns(expectedResut);
            var result = userAgentService.UpdateUserAgent(userAgentJson);

            result.ErrorCode.Should().Be(-1);
        }

        [Fact]
        public void TransformUserAgent_WhenSuccess_ReturnUserAgentModel()
        {
            var userAgentEntities = new List<UserAgentEntity> {
            new UserAgentEntity
            {
                CreatedDate = new DateTime(2020, 01, 09, 00, 00, 00),
                UserAgent = "Mozilla/5.0 (Linux; Android 10; SM-N770F) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Mobile Safari/537.36",
                UserAgentKey = "00140dfba5212b0f74c2114ebe01d719"
            },
            new UserAgentEntity
            {
                CreatedDate = new DateTime(2020, 02, 09, 00, 00, 00),
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 12_1_4 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) FxiOS/17.3b15323 Mobile/16D57 Safari/605.1.15",
                UserAgentKey = "0031fadec5836ee87da7c28f72a6bd1e"
            }
            };

            var expectedResult = new List<UserAgentModel>
            {
                new UserAgentModel
                {
                    BrowserName = "Chrome Mobile",
                    OSName = "Android",
                    CreatedDate = new DateTime(2020, 01, 09, 00, 00, 00),
                    UserAgentKey = "00140dfba5212b0f74c2114ebe01d719"
                },
                new UserAgentModel
                {
                    BrowserName = "Firefox iOS",
                    OSName = "iOS",
                    CreatedDate = new DateTime(2020, 02, 09, 00, 00, 00),
                    UserAgentKey = "0031fadec5836ee87da7c28f72a6bd1e"
                },
            };

            var result = userAgentService.TransformUserAgent(userAgentEntities);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void TransformUserAgent_WhenFailed_ReturnUserAgentModel()
        {
            var userAgentEntities = new List<UserAgentEntity>();

            var expectedResult = new List<UserAgentModel>();

            var result = userAgentService.TransformUserAgent(userAgentEntities);

            result.Should().BeEquivalentTo(expectedResult);
            result.Count.Should().Be(0);
        }

        [Fact]
        public void CovertUserAgent_WhenSuccess_ReturnClientInfo()
        {
            var userAgentString = "Mozilla/5.0 (Linux; Android 10; SM-N770F) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Mobile Safari/537.36";
            var browserName = "Chrome Mobile";
            var osName = "Android";

            var result = userAgentService.ConvertUserAgent(userAgentString);

            result.UA.Family.Should().Be(browserName);
            result.OS.Family.Should().Be(osName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CovertUserAgent_WhenFailed_ReturnNull(string userAgent)
        {

            var result = userAgentService.ConvertUserAgent(userAgent);

            result.Should().BeNull();
        }
    }
}
