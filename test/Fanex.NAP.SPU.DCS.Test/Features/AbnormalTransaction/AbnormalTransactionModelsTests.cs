namespace Fanex.NAP.SPU.DCS.Tests.Features.AbnormalTransaction
{
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Entities;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Model;
    using System;
    using Xunit;

    public class AbnormalTransactionModelsTests
    {
        private readonly IFixture fixture = new Fixture();

        [Fact]
        public void AbnormalTransactionEntity_PropertiesSetCorrectly()
        {
            // Arrange
            var id = fixture.Create<ulong>();
            var loginName = fixture.Create<string>();
            var transTime = fixture.Create<DateTime>();
            var userAgent = fixture.Create<string>();
            var ip = fixture.Create<string>();
            var os = fixture.Create<string>();
            var browser = fixture.Create<string>();
            var deviceTypeName = fixture.Create<string>();

            // Act
            var entity = new AbnormalTransactionEntity
            {
                ID = id,
                LoginName = loginName,
                TransTime = transTime,
                UserAgent = userAgent,
                IP = ip,
                OS = os,
                Browser = browser,
                DeviceTypeName = deviceTypeName
            };

            // Assert
            Assert.Equal(id, entity.ID);
            Assert.Equal(loginName, entity.LoginName);
            Assert.Equal(transTime, entity.TransTime);
            Assert.Equal(userAgent, entity.UserAgent);
            Assert.Equal(ip, entity.IP);
            Assert.Equal(os, entity.OS);
            Assert.Equal(browser, entity.Browser);
            Assert.Equal(deviceTypeName, entity.DeviceTypeName);
        }

        [Fact]
        public void AbnormalTransactionEntity_DefaultValues()
        {
            // Act
            var entity = new AbnormalTransactionEntity();

            // Assert
            Assert.Equal(0UL, entity.ID);
            Assert.Null(entity.LoginName);
            Assert.Equal(default(DateTime), entity.TransTime);
            Assert.Null(entity.UserAgent);
            Assert.Null(entity.IP);
            Assert.Null(entity.OS);
            Assert.Null(entity.Browser);
            Assert.Null(entity.DeviceTypeName);
        }

        [Fact]
        public void AbnormalTransactionModel_PropertiesSetCorrectly()
        {
            // Arrange
            var id = fixture.Create<ulong>();
            var loginName = fixture.Create<string>();
            var transTime = fixture.Create<DateTime>();
            var userAgent = fixture.Create<string>();
            var ip = fixture.Create<string>();
            var os = fixture.Create<string>();
            var browser = fixture.Create<string>();
            var device = fixture.Create<string>();

            // Act
            var model = new AbnormalTransactionModel
            {
                ID = id,
                LoginName = loginName,
                TransTime = transTime,
                UserAgent = userAgent,
                IP = ip,
                OS = os,
                Browser = browser,
                Device = device
            };

            // Assert
            Assert.Equal(id, model.ID);
            Assert.Equal(loginName, model.LoginName);
            Assert.Equal(transTime, model.TransTime);
            Assert.Equal(userAgent, model.UserAgent);
            Assert.Equal(ip, model.IP);
            Assert.Equal(os, model.OS);
            Assert.Equal(browser, model.Browser);
            Assert.Equal(device, model.Device);
        }

        [Fact]
        public void AbnormalTransactionModel_DefaultValues()
        {
            // Act
            var model = new AbnormalTransactionModel();

            // Assert
            Assert.Equal(0UL, model.ID);
            Assert.Null(model.LoginName);
            Assert.Equal(default(DateTime), model.TransTime);
            Assert.Null(model.UserAgent);
            Assert.Null(model.IP);
            Assert.Null(model.OS);
            Assert.Null(model.Browser);
            Assert.Null(model.Device);
        }

        [Fact]
        public void AbnormalTransactionUpdateModel_PropertiesSetCorrectly()
        {
            // Arrange
            var id = fixture.Create<ulong>();
            var status = fixture.Create<int>();

            // Act
            var updateModel = new AbnormalTransactionUpdateModel
            {
                ID = id,
                Status = status
            };

            // Assert
            Assert.Equal(id, updateModel.ID);
            Assert.Equal(status, updateModel.Status);
        }

        [Fact]
        public void AbnormalTransactionUpdateModel_DefaultValues()
        {
            // Act
            var updateModel = new AbnormalTransactionUpdateModel();

            // Assert
            Assert.Equal(0UL, updateModel.ID);
            Assert.Equal(0, updateModel.Status);
        }

        [Fact]
        public void RangeIDModel_PropertiesSetCorrectly()
        {
            // Arrange
            var minId = fixture.Create<ulong>();
            var maxId = fixture.Create<ulong>();

            // Act
            var rangeModel = new RangeIDModel
            {
                MinID = minId,
                MaxID = maxId
            };

            // Assert
            Assert.Equal(minId, rangeModel.MinID);
            Assert.Equal(maxId, rangeModel.MaxID);
        }

        [Fact]
        public void RangeIDModel_DefaultValues()
        {
            // Act
            var rangeModel = new RangeIDModel();

            // Assert
            Assert.Equal(0UL, rangeModel.MinID);
            Assert.Equal(0UL, rangeModel.MaxID);
        }

        [Fact]
        public void AbnormalTransactionEntity_WithNullValues_HandlesCorrectly()
        {
            // Act
            var entity = new AbnormalTransactionEntity
            {
                ID = 1,
                LoginName = null,
                UserAgent = null,
                IP = null,
                OS = null,
                Browser = null,
                DeviceTypeName = null
            };

            // Assert
            Assert.Equal(1UL, entity.ID);
            Assert.Null(entity.LoginName);
            Assert.Null(entity.UserAgent);
            Assert.Null(entity.IP);
            Assert.Null(entity.OS);
            Assert.Null(entity.Browser);
            Assert.Null(entity.DeviceTypeName);
        }

        [Fact]
        public void AbnormalTransactionModel_WithNullValues_HandlesCorrectly()
        {
            // Act
            var model = new AbnormalTransactionModel
            {
                ID = 1,
                LoginName = null,
                UserAgent = null,
                IP = null,
                OS = null,
                Browser = null,
                Device = null
            };

            // Assert
            Assert.Equal(1UL, model.ID);
            Assert.Null(model.LoginName);
            Assert.Null(model.UserAgent);
            Assert.Null(model.IP);
            Assert.Null(model.OS);
            Assert.Null(model.Browser);
            Assert.Null(model.Device);
        }

        [Fact]
        public void AbnormalTransactionEntity_WithEmptyStrings_HandlesCorrectly()
        {
            // Act
            var entity = new AbnormalTransactionEntity
            {
                ID = 1,
                LoginName = string.Empty,
                UserAgent = string.Empty,
                IP = string.Empty,
                OS = string.Empty,
                Browser = string.Empty,
                DeviceTypeName = string.Empty
            };

            // Assert
            Assert.Equal(1UL, entity.ID);
            Assert.Equal(string.Empty, entity.LoginName);
            Assert.Equal(string.Empty, entity.UserAgent);
            Assert.Equal(string.Empty, entity.IP);
            Assert.Equal(string.Empty, entity.OS);
            Assert.Equal(string.Empty, entity.Browser);
            Assert.Equal(string.Empty, entity.DeviceTypeName);
        }

        [Fact]
        public void AbnormalTransactionModel_WithEmptyStrings_HandlesCorrectly()
        {
            // Act
            var model = new AbnormalTransactionModel
            {
                ID = 1,
                LoginName = string.Empty,
                UserAgent = string.Empty,
                IP = string.Empty,
                OS = string.Empty,
                Browser = string.Empty,
                Device = string.Empty
            };

            // Assert
            Assert.Equal(1UL, model.ID);
            Assert.Equal(string.Empty, model.LoginName);
            Assert.Equal(string.Empty, model.UserAgent);
            Assert.Equal(string.Empty, model.IP);
            Assert.Equal(string.Empty, model.OS);
            Assert.Equal(string.Empty, model.Browser);
            Assert.Equal(string.Empty, model.Device);
        }

        [Fact]
        public void RangeIDModel_WithBoundaryValues_HandlesCorrectly()
        {
            // Act
            var rangeModel = new RangeIDModel
            {
                MinID = ulong.MinValue,
                MaxID = ulong.MaxValue
            };

            // Assert
            Assert.Equal(ulong.MinValue, rangeModel.MinID);
            Assert.Equal(ulong.MaxValue, rangeModel.MaxID);
        }

        [Fact]
        public void AbnormalTransactionUpdateModel_WithBoundaryValues_HandlesCorrectly()
        {
            // Act
            var updateModel = new AbnormalTransactionUpdateModel
            {
                ID = ulong.MaxValue,
                Status = int.MaxValue
            };

            // Assert
            Assert.Equal(ulong.MaxValue, updateModel.ID);
            Assert.Equal(int.MaxValue, updateModel.Status);
        }

        [Fact]
        public void AbnormalTransactionEntity_WithSpecialCharacters_HandlesCorrectly()
        {
            // Act
            var entity = new AbnormalTransactionEntity
            {
                ID = 1,
                LoginName = "test@domain.com",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                IP = "192.168.1.1",
                OS = "Windows 10",
                Browser = "Chrome 95.0",
                DeviceTypeName = "Desktop/Mobile"
            };

            // Assert
            Assert.Equal("test@domain.com", entity.LoginName);
            Assert.Equal("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36", entity.UserAgent);
            Assert.Equal("192.168.1.1", entity.IP);
            Assert.Equal("Windows 10", entity.OS);
            Assert.Equal("Chrome 95.0", entity.Browser);
            Assert.Equal("Desktop/Mobile", entity.DeviceTypeName);
        }

        [Fact]
        public void AbnormalTransactionModel_WithSpecialCharacters_HandlesCorrectly()
        {
            // Act
            var model = new AbnormalTransactionModel
            {
                ID = 1,
                LoginName = "test@domain.com",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                IP = "192.168.1.1",
                OS = "Windows 10",
                Browser = "Chrome 95.0",
                Device = "Desktop/Mobile"
            };

            // Assert
            Assert.Equal("test@domain.com", model.LoginName);
            Assert.Equal("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36", model.UserAgent);
            Assert.Equal("192.168.1.1", model.IP);
            Assert.Equal("Windows 10", model.OS);
            Assert.Equal("Chrome 95.0", model.Browser);
            Assert.Equal("Desktop/Mobile", model.Device);
        }
    }
}