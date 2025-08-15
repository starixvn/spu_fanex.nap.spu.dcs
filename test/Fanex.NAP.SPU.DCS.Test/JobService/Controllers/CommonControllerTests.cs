using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Fanex.NAP.SPU.DCS.JobService.Controllers;
using Fanex.NAP.SPU.DCS.JobService.Models;
using Fanex.NAP.SPU.DCS.JobService;
using System.Net;
using Xunit;
using System;

namespace Fanex.NAP.SPU.DCS.Test.JobService.Controllers
{
    public class CommonControllerTests
    {
        private readonly IAppSettings _mockAppSettings;
        private readonly CommonController _controller;
        private readonly HttpContext _httpContext;

        public CommonControllerTests()
        {
            _mockAppSettings = Substitute.For<IAppSettings>();
            _controller = new CommonController(_mockAppSettings);
            
            // Setup HttpContext
            _httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };
        }

        [Fact]
        public void Index_WithValidEnvironment_ReturnsWhoamiModel()
        {
            // Arrange
            var expectedEnvironment = "Development";
            var expectedServerName = Environment.MachineName;
            var expectedLocalIP = "127.0.0.1";
            
            _mockAppSettings.Environment.Returns(expectedEnvironment);
            
            // Setup HttpConnectionFeature
            var httpConnectionFeature = Substitute.For<IHttpConnectionFeature>();
            httpConnectionFeature.LocalIpAddress.Returns(IPAddress.Parse(expectedLocalIP));
            _httpContext.Features.Set(httpConnectionFeature);

            // Act
            var result = _controller.Index();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEnvironment, result.Environment);
            Assert.Equal(expectedServerName, result.ServerName);
            Assert.Equal(expectedLocalIP, result.LocalIP);
            Assert.Equal(DateTime.Now.Date, result.ServerTime.Date);
        }

        [Fact]
        public void Index_WithNullLocalIP_ReturnsWhoamiModelWithNullLocalIP()
        {
            // Arrange
            var expectedEnvironment = "Production";
            var expectedServerName = Environment.MachineName;
            
            _mockAppSettings.Environment.Returns(expectedEnvironment);
            
            // Setup HttpConnectionFeature with null LocalIP
            var httpConnectionFeature = Substitute.For<IHttpConnectionFeature>();
            httpConnectionFeature.LocalIpAddress.Returns((IPAddress)null);
            _httpContext.Features.Set(httpConnectionFeature);

            // Act
            var result = _controller.Index();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEnvironment, result.Environment);
            Assert.Equal(expectedServerName, result.ServerName);
            Assert.Null(result.LocalIP);
            Assert.Equal(DateTime.Now.Date, result.ServerTime.Date);
        }

        [Fact]
        public void Index_WithNullHttpConnectionFeature_ReturnsWhoamiModelWithNullLocalIP()
        {
            // Arrange
            var expectedEnvironment = "UAT";
            var expectedServerName = Environment.MachineName;
            
            _mockAppSettings.Environment.Returns(expectedEnvironment);
            
            // Don't set HttpConnectionFeature

            // Act
            var result = _controller.Index();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEnvironment, result.Environment);
            Assert.Equal(expectedServerName, result.ServerName);
            Assert.Null(result.LocalIP);
            Assert.Equal(DateTime.Now.Date, result.ServerTime.Date);
        }

        [Fact]
        public void Index_WithDifferentEnvironments_ReturnsCorrectEnvironment()
        {
            // Arrange
            var environments = new[] { "Development", "UAT", "Production", "AWS" };
            
            foreach (var environment in environments)
            {
                _mockAppSettings.Environment.Returns(environment);
                
                // Setup HttpConnectionFeature
                var httpConnectionFeature = Substitute.For<IHttpConnectionFeature>();
                httpConnectionFeature.LocalIpAddress.Returns(IPAddress.Parse("192.168.1.1"));
                _httpContext.Features.Set(httpConnectionFeature);

                // Act
                var result = _controller.Index();

                // Assert
                Assert.Equal(environment, result.Environment);
            }
        }
    }
}
