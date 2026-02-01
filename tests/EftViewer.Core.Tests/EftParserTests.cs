using EftViewer.Core.Models;
using EftViewer.Core.Parsing;
using EftViewer.Core.Tests.TestData;
using Xunit;

namespace EftViewer.Core.Tests
{
    public class EftParserTests
    {
        private readonly EftParser _parser = new EftParser();

        [Fact]
        public void Parse_MinimalFile_ReturnsValidEftFile()
        {
            // Arrange
            var data = SyntheticEftGenerator.GenerateMinimalFile();

            // Act
            var result = _parser.Parse(data);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsValid);
            Assert.Equal(2, result.RecordCount);
        }

        [Fact]
        public void Parse_MinimalFile_HasTransactionRecord()
        {
            // Arrange
            var data = SyntheticEftGenerator.GenerateMinimalFile();

            // Act
            var result = _parser.Parse(data);

            // Assert
            Assert.NotNull(result.Transaction);
            Assert.Equal(1, result.Transaction.RecordType);
            Assert.Equal("0501", result.Transaction.Version);
            Assert.Equal("TEST", result.Transaction.TransactionType);
        }

        [Fact]
        public void Parse_MinimalFile_HasDescriptiveRecord()
        {
            // Arrange
            var data = SyntheticEftGenerator.GenerateMinimalFile();

            // Act
            var result = _parser.Parse(data);

            // Assert
            Assert.Single(result.DescriptiveRecords);
            Assert.Equal(2, result.DescriptiveRecords[0].RecordType);
        }

        [Fact]
        public void Parse_WithFingerprint_HasFingerprintRecord()
        {
            // Arrange
            var data = SyntheticEftGenerator.GenerateWithFingerprint();

            // Act
            var result = _parser.Parse(data);

            // Assert
            Assert.Single(result.FingerprintRecords);
            var fingerprint = result.FingerprintRecords[0];
            Assert.Equal(14, fingerprint.RecordType);
            Assert.True(fingerprint.IsType14);
            Assert.Equal("1", fingerprint.FingerPosition);
            Assert.Equal(512, fingerprint.ImageWidth);
            Assert.Equal(512, fingerprint.ImageHeight);
            Assert.Equal(500, fingerprint.HorizontalResolution);
            Assert.Equal("WSQ20", fingerprint.CompressionAlgorithm);
            Assert.True(fingerprint.IsWsqCompressed);
            Assert.True(fingerprint.HasImageData);
        }

        [Fact]
        public void Parse_WithFingerprint_ImageDataIsExtracted()
        {
            // Arrange
            var data = SyntheticEftGenerator.GenerateWithFingerprint();

            // Act
            var result = _parser.Parse(data);

            // Assert
            var fingerprint = result.FingerprintRecords[0];
            Assert.NotNull(fingerprint.ImageData);
            Assert.True(fingerprint.ImageData.Length > 0);
            // Verify WSQ header placeholder
            Assert.Equal(0xFF, fingerprint.ImageData[0]);
            Assert.Equal(0xA0, fingerprint.ImageData[1]);
        }

        [Fact]
        public void Parse_MalformedFile_ReturnsPartialResult()
        {
            // Arrange
            var data = SyntheticEftGenerator.GenerateMalformed();

            // Act
            var result = _parser.Parse(data);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.RecordCount >= 1);
            // Should have transaction record even if incomplete
            Assert.NotNull(result.Transaction);
        }

        [Fact]
        public void Parse_UnknownRecordType_CreatesGenericRecord()
        {
            // Arrange
            var data = SyntheticEftGenerator.GenerateWithUnknownType();

            // Act
            var result = _parser.Parse(data);

            // Assert
            Assert.Equal(2, result.RecordCount);
            Assert.Single(result.UnsupportedRecords);
            Assert.True(result.Warnings.Count > 0);
        }

        [Fact]
        public void Parse_EmptyData_ThrowsException()
        {
            // Arrange
            var data = new byte[0];

            // Act & Assert
            Assert.Throws<EftViewer.Core.Exceptions.EftParseException>(() => _parser.Parse(data));
        }

        [Fact]
        public void Parse_NullData_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<EftViewer.Core.Exceptions.EftParseException>(() => _parser.Parse((byte[])null));
        }

        [Fact]
        public void Parse_TransactionRecord_ExtractsAllFields()
        {
            // Arrange
            var data = SyntheticEftGenerator.GenerateMinimalFile();

            // Act
            var result = _parser.Parse(data);

            // Assert
            var txn = result.Transaction;
            Assert.Equal("20260131", txn.Date);
            Assert.Equal("DESTAG", txn.DestinationAgency);
            Assert.Equal("ORIGAG", txn.OriginatingAgency);
            Assert.Equal("TCN123456", txn.TransactionControlNumber);
        }

        [Fact]
        public void Parse_FieldWithSubfields_ParsesCorrectly()
        {
            // Arrange
            var data = SyntheticEftGenerator.GenerateMinimalFile();

            // Act
            var result = _parser.Parse(data);

            // Assert
            // Field 1.003 (CNT) has subfields
            var cntField = result.Transaction.GetField(3);
            Assert.NotNull(cntField);
            Assert.True(cntField.Subfields.Count >= 2);
        }

        [Fact]
        public void GetSummary_ReturnsFormattedString()
        {
            // Arrange
            var data = SyntheticEftGenerator.GenerateWithFingerprint();

            // Act
            var result = _parser.Parse(data);
            var summary = result.GetSummary();

            // Assert
            Assert.Contains("record", summary);
            Assert.Contains("fingerprint", summary);
        }
    }
}
