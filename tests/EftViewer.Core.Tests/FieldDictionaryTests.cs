using EftViewer.Core.Metadata;
using Xunit;

namespace EftViewer.Core.Tests
{
    public class FieldDictionaryTests
    {
        [Theory]
        [InlineData("1.001", "Record Length")]
        [InlineData("1.002", "Version Number")]
        [InlineData("1.003", "File Content")]
        [InlineData("1.007", "Destination Agency")]
        [InlineData("1.008", "Originating Agency")]
        [InlineData("14.011", "Compression Algorithm")]
        [InlineData("14.999", "Image Data")]
        public void GetFieldName_KnownField_ReturnsName(string fieldId, string expectedName)
        {
            // Act
            var result = FieldDictionary.GetFieldName(fieldId);

            // Assert
            Assert.Equal(expectedName, result);
        }

        [Fact]
        public void GetFieldName_UnknownField_ReturnsNull()
        {
            // Act
            var result = FieldDictionary.GetFieldName("99.999");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetFieldName_WithTypeAndNumber_ReturnsName()
        {
            // Act
            var result = FieldDictionary.GetFieldName(1, 2);

            // Assert
            Assert.Equal("Version Number", result);
        }

        [Fact]
        public void GetFieldName_UnknownWithTypeAndNumber_ReturnsDefault()
        {
            // Act
            var result = FieldDictionary.GetFieldName(99, 999);

            // Assert
            Assert.Equal("Field 999", result);
        }

        [Fact]
        public void GetDisplayText_KnownField_ReturnsNameAndId()
        {
            // Act
            var result = FieldDictionary.GetDisplayText("1.002");

            // Assert
            Assert.Equal("Version Number (1.002)", result);
        }

        [Fact]
        public void GetDisplayText_UnknownField_ReturnsIdOnly()
        {
            // Act
            var result = FieldDictionary.GetDisplayText("99.999");

            // Assert
            Assert.Equal("99.999", result);
        }
    }
}
