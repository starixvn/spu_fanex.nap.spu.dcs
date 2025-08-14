namespace Fanex.NAP.SPU.DCS.Tests.Features.AbnormalTransaction
{
    using AutoFixture;
    using Fanex.NAP.SPU.DCS.Features.AbnormalTransaction.Criteria;
    using Xunit;

    public class AbnormalTransactionCriteriaTests
    {
        private readonly IFixture fixture = new Fixture();

        [Fact]
        public void AbnormalTransactionGetCriteria_PropertiesSetCorrectly()
        {
            // Arrange
            var size = fixture.Create<int>();
            var minId = fixture.Create<ulong>();
            var maxId = fixture.Create<ulong>();

            // Act
            var criteria = new AbnormalTransactionGetCriteria
            {
                Size = size,
                MinID = minId,
                MaxID = maxId
            };

            // Assert
            Assert.Equal(size, criteria.Size);
            Assert.Equal(minId, criteria.MinID);
            Assert.Equal(maxId, criteria.MaxID);
        }

        [Fact]
        public void AbnormalTransactionGetCriteria_DefaultValues()
        {
            // Act
            var criteria = new AbnormalTransactionGetCriteria();

            // Assert
            Assert.Equal(0, criteria.Size);
            Assert.Equal(0UL, criteria.MinID);
            Assert.Equal(0UL, criteria.MaxID);
        }

        [Fact]
        public void AbnormalTransactionGetCriteria_GetSettingKey_ReturnsCorrectValue()
        {
            // Arrange
            var criteria = new AbnormalTransactionGetCriteria();

            // Act
            var settingKey = criteria.GetSettingKey();

            // Assert
            Assert.Equal("DCS_DC_AbnormalTransaction_Get", settingKey);
        }

        [Fact]
        public void AbnormalTransactionGetCriteria_IsValid_ReturnsTrue()
        {
            // Arrange
            var criteria = new AbnormalTransactionGetCriteria();

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void AbnormalTransactionGetCriteria_WithNegativeSize_IsStillValid()
        {
            // Arrange
            var criteria = new AbnormalTransactionGetCriteria
            {
                Size = -1
            };

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid);
            Assert.Equal(-1, criteria.Size);
        }

        [Fact]
        public void AbnormalTransactionGetCriteria_WithZeroSize_IsValid()
        {
            // Arrange
            var criteria = new AbnormalTransactionGetCriteria
            {
                Size = 0
            };

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid);
            Assert.Equal(0, criteria.Size);
        }

        [Fact]
        public void AbnormalTransactionGetCriteria_WithMaxSize_IsValid()
        {
            // Arrange
            var criteria = new AbnormalTransactionGetCriteria
            {
                Size = int.MaxValue
            };

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid);
            Assert.Equal(int.MaxValue, criteria.Size);
        }

        [Fact]
        public void AbnormalTransactionGetCriteria_WithBoundaryIdValues_HandlesCorrectly()
        {
            // Arrange & Act
            var criteria = new AbnormalTransactionGetCriteria
            {
                Size = 100,
                MinID = ulong.MinValue,
                MaxID = ulong.MaxValue
            };

            // Assert
            Assert.Equal(100, criteria.Size);
            Assert.Equal(ulong.MinValue, criteria.MinID);
            Assert.Equal(ulong.MaxValue, criteria.MaxID);
            Assert.True(criteria.IsValid());
        }

        [Fact]
        public void AbnormalTransactionUpdateCriteria_PropertiesSetCorrectly()
        {
            // Arrange
            var transJson = fixture.Create<string>();
            var minId = fixture.Create<ulong>();
            var maxId = fixture.Create<ulong>();

            // Act
            var criteria = new AbnormalTransactionUpdateCriteria
            {
                TransJson = transJson,
                MinID = minId,
                MaxID = maxId
            };

            // Assert
            Assert.Equal(transJson, criteria.TransJson);
            Assert.Equal(minId, criteria.MinID);
            Assert.Equal(maxId, criteria.MaxID);
        }

        [Fact]
        public void AbnormalTransactionUpdateCriteria_DefaultValues()
        {
            // Act
            var criteria = new AbnormalTransactionUpdateCriteria();

            // Assert
            Assert.Null(criteria.TransJson);
            Assert.Equal(0UL, criteria.MinID);
            Assert.Equal(0UL, criteria.MaxID);
        }

        [Fact]
        public void AbnormalTransactionUpdateCriteria_GetSettingKey_ReturnsCorrectValue()
        {
            // Arrange
            var criteria = new AbnormalTransactionUpdateCriteria();

            // Act
            var settingKey = criteria.GetSettingKey();

            // Assert
            Assert.Equal("DCS_DC_AbnormalTransaction_Update", settingKey);
        }

        [Fact]
        public void AbnormalTransactionUpdateCriteria_IsValid_ReturnsTrue()
        {
            // Arrange
            var criteria = new AbnormalTransactionUpdateCriteria();

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void AbnormalTransactionUpdateCriteria_WithNullTransJson_IsValid()
        {
            // Arrange
            var criteria = new AbnormalTransactionUpdateCriteria
            {
                TransJson = null,
                MinID = 1,
                MaxID = 2
            };

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid);
            Assert.Null(criteria.TransJson);
        }

        [Fact]
        public void AbnormalTransactionUpdateCriteria_WithEmptyTransJson_IsValid()
        {
            // Arrange
            var criteria = new AbnormalTransactionUpdateCriteria
            {
                TransJson = string.Empty,
                MinID = 1,
                MaxID = 2
            };

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid);
            Assert.Equal(string.Empty, criteria.TransJson);
        }

        [Fact]
        public void AbnormalTransactionUpdateCriteria_WithLargeJsonString_IsValid()
        {
            // Arrange
            var largeJson = new string('x', 10000);
            var criteria = new AbnormalTransactionUpdateCriteria
            {
                TransJson = largeJson,
                MinID = 1,
                MaxID = 2
            };

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid);
            Assert.Equal(largeJson, criteria.TransJson);
        }

        [Fact]
        public void AbnormalTransactionUpdateCriteria_WithSpecialCharactersInJson_IsValid()
        {
            // Arrange
            var specialJson = "{ \"test\": \"value with special chars: ραινσϊ & <xml> & \\\"quotes\\\"\" }";
            var criteria = new AbnormalTransactionUpdateCriteria
            {
                TransJson = specialJson,
                MinID = 1,
                MaxID = 2
            };

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid);
            Assert.Equal(specialJson, criteria.TransJson);
        }

        [Fact]
        public void AbnormalTransactionUpdateCriteria_WithBoundaryIdValues_IsValid()
        {
            // Arrange
            var criteria = new AbnormalTransactionUpdateCriteria
            {
                TransJson = "{}",
                MinID = ulong.MinValue,
                MaxID = ulong.MaxValue
            };

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid);
            Assert.Equal(ulong.MinValue, criteria.MinID);
            Assert.Equal(ulong.MaxValue, criteria.MaxID);
        }

        [Fact]
        public void AbnormalTransactionUpdateCriteria_WithZeroIdValues_IsValid()
        {
            // Arrange
            var criteria = new AbnormalTransactionUpdateCriteria
            {
                TransJson = "{}",
                MinID = 0,
                MaxID = 0
            };

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid);
            Assert.Equal(0UL, criteria.MinID);
            Assert.Equal(0UL, criteria.MaxID);
        }

        [Fact]
        public void AbnormalTransactionUpdateCriteria_WithMinIdGreaterThanMaxId_IsStillValid()
        {
            // Arrange
            var criteria = new AbnormalTransactionUpdateCriteria
            {
                TransJson = "{}",
                MinID = 100,
                MaxID = 50
            };

            // Act
            var isValid = criteria.IsValid();

            // Assert
            Assert.True(isValid); // Validation doesn't check logical consistency
            Assert.Equal(100UL, criteria.MinID);
            Assert.Equal(50UL, criteria.MaxID);
        }

        [Fact]
        public void BothCriteria_SettingKeysAreDifferent()
        {
            // Arrange
            var getCriteria = new AbnormalTransactionGetCriteria();
            var updateCriteria = new AbnormalTransactionUpdateCriteria();

            // Act
            var getKey = getCriteria.GetSettingKey();
            var updateKey = updateCriteria.GetSettingKey();

            // Assert
            Assert.NotEqual(getKey, updateKey);
            Assert.Equal("DCS_DC_AbnormalTransaction_Get", getKey);
            Assert.Equal("DCS_DC_AbnormalTransaction_Update", updateKey);
        }

        [Fact]
        public void BothCriteria_IsValidAlwaysReturnsTrue()
        {
            // Arrange
            var getCriteria = new AbnormalTransactionGetCriteria();
            var updateCriteria = new AbnormalTransactionUpdateCriteria();

            // Act & Assert
            Assert.True(getCriteria.IsValid());
            Assert.True(updateCriteria.IsValid());
        }
    }
}