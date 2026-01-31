using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EftViewer.Core.Constants;
using EftViewer.Core.Exceptions;
using EftViewer.Core.Models;

namespace EftViewer.Core.Parsing
{
    /// <summary>
    /// Parser for ANSI/NIST-ITL EFT files.
    /// Handles both ASCII (Type-1, 2, 9+) and binary (Type-3-8) record formats.
    /// </summary>
    public class EftParser
    {
        private readonly List<string> _warnings = new List<string>();

        public EftFile Parse(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            byte[] data = File.ReadAllBytes(filePath);
            return Parse(data, filePath);
        }

        public EftFile Parse(Stream stream, string filePath = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return Parse(ms.ToArray(), filePath);
            }
        }

        public EftFile Parse(byte[] data, string filePath = null)
        {
            if (data == null || data.Length == 0)
                throw new EftParseException("File is empty or null");

            _warnings.Clear();
            var records = new List<EftRecord>();
            int position = 0;
            int recordIndex = 1;

            // First, parse Type-1 record to get the content list
            var type1Result = ParseType1Record(data, ref position, recordIndex);
            if (type1Result.Record != null)
            {
                records.Add(type1Result.Record);
                recordIndex++;
            }
            else
            {
                _warnings.Add("Failed to parse Type-1 transaction record");
                return new EftFile(filePath, records, new List<string>(_warnings));
            }

            // Get expected record list from Type-1 CNT field
            var expectedRecords = ParseContentField(type1Result.Record);

            // Parse remaining records based on their types
            foreach (var (recordType, idc) in expectedRecords)
            {
                if (position >= data.Length)
                {
                    _warnings.Add($"Unexpected end of file, expected record type {recordType}");
                    break;
                }

                try
                {
                    EftRecord record;
                    if (IsBinaryRecordType(recordType))
                    {
                        record = ParseBinaryRecord(data, ref position, recordIndex, recordType);
                    }
                    else
                    {
                        record = ParseAsciiRecord(data, ref position, recordIndex);
                    }

                    if (record != null)
                    {
                        records.Add(record);
                        recordIndex++;
                    }
                }
                catch (Exception ex)
                {
                    _warnings.Add($"Error parsing record {recordIndex} (type {recordType}): {ex.Message}");
                    // Try to skip to next record using length if we can determine it
                    break;
                }
            }

            return new EftFile(filePath, records, new List<string>(_warnings));
        }

        /// <summary>
        /// Binary record types (Type 3-8) use fixed-length headers instead of tagged fields.
        /// </summary>
        private bool IsBinaryRecordType(int recordType)
        {
            return recordType >= 3 && recordType <= 8;
        }

        private (TransactionRecord Record, int EndPosition) ParseType1Record(byte[] data, ref int position, int recordIndex)
        {
            // Type-1 is always ASCII format with tagged fields, terminated by FS
            int recordEnd = FindRecordEnd(data, position);
            if (recordEnd < 0)
            {
                recordEnd = data.Length;
                _warnings.Add("Type-1 record not terminated with FS separator");
            }

            int recordLength = recordEnd - position;
            var recordParser = new RecordParser();
            var parseResult = recordParser.ParseFields(data, position, recordLength);

            position = recordEnd + 1; // Move past FS

            if (parseResult.Fields.Count == 0 || parseResult.Fields[0].RecordType != 1)
            {
                return (null, position);
            }

            var record = new TransactionRecord(recordIndex, parseResult.Fields);
            return (record, position);
        }

        /// <summary>
        /// Parse CNT field (1.003) to get list of expected records.
        /// Format: count US type US IDC [RS type US IDC]...
        /// </summary>
        private List<(int RecordType, int Idc)> ParseContentField(TransactionRecord type1)
        {
            var result = new List<(int, int)>();
            var cntField = type1.GetField(3); // 1.003 CNT

            if (cntField == null || cntField.Subfields.Count == 0)
            {
                _warnings.Add("Missing or invalid CNT field (1.003)");
                return result;
            }

            // Skip first subfield (it's the count of Type-1 records, always 1)
            // Remaining subfields are: type US idc
            for (int i = 1; i < cntField.Subfields.Count; i++)
            {
                var subfield = cntField.Subfields[i];
                if (subfield.Items.Count >= 2)
                {
                    if (int.TryParse(subfield.Items[0], out int recordType) &&
                        int.TryParse(subfield.Items[1], out int idc))
                    {
                        result.Add((recordType, idc));
                    }
                }
                else if (!string.IsNullOrEmpty(subfield.Value))
                {
                    // Try parsing as "type US idc" from raw value
                    var parts = subfield.Value.Split((char)Separators.US);
                    if (parts.Length >= 2 &&
                        int.TryParse(parts[0], out int recordType) &&
                        int.TryParse(parts[1], out int idc))
                    {
                        result.Add((recordType, idc));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Parse a binary record (Type 3-8).
        /// These have a 4-byte length header followed by fixed fields and image data.
        /// </summary>
        private EftRecord ParseBinaryRecord(byte[] data, ref int position, int recordIndex, int expectedType)
        {
            if (position + 4 > data.Length)
            {
                throw new EftParseException("Insufficient data for binary record header", position);
            }

            // Read 4-byte record length (big-endian or could be ASCII digits)
            int recordLength = ReadBinaryRecordLength(data, position);

            if (recordLength <= 0 || position + recordLength > data.Length)
            {
                throw new EftParseException($"Invalid binary record length: {recordLength}", position);
            }

            // Extract record data
            byte[] recordData = new byte[recordLength];
            Array.Copy(data, position, recordData, 0, recordLength);

            // Parse based on record type
            var fields = new List<EftField>();
            int recordType = expectedType;

            if (expectedType == 4)
            {
                // Type-4: Fixed binary format
                // Bytes 0-3: LEN (already read)
                // Byte 4: IDC
                // Byte 5: IMP (impression type)
                // Bytes 6-11: FGP (finger positions, 6 bytes)
                // Byte 12: ISR (image scanning resolution)
                // Bytes 13-14: HLL (horizontal line length)
                // Bytes 15-16: VLL (vertical line length)
                // Byte 17: GCA (compression algorithm)
                // Bytes 18+: Image data

                fields.Add(new EftField(4, 1, Encoding.ASCII.GetBytes(recordLength.ToString()))); // LEN

                if (recordData.Length > 4)
                    fields.Add(new EftField(4, 2, new[] { recordData[4] })); // IDC
                if (recordData.Length > 5)
                    fields.Add(new EftField(4, 3, new[] { recordData[5] })); // IMP
                if (recordData.Length > 11)
                    fields.Add(new EftField(4, 4, SubArray(recordData, 6, 6))); // FGP
                if (recordData.Length > 12)
                    fields.Add(new EftField(4, 5, new[] { recordData[12] })); // ISR
                if (recordData.Length > 14)
                {
                    int hll = (recordData[13] << 8) | recordData[14];
                    fields.Add(new EftField(4, 6, Encoding.ASCII.GetBytes(hll.ToString()))); // HLL
                }
                if (recordData.Length > 16)
                {
                    int vll = (recordData[15] << 8) | recordData[16];
                    fields.Add(new EftField(4, 7, Encoding.ASCII.GetBytes(vll.ToString()))); // VLL
                }
                if (recordData.Length > 17)
                    fields.Add(new EftField(4, 8, new[] { recordData[17] })); // GCA

                // Image data (field 9)
                if (recordData.Length > 18)
                {
                    byte[] imageData = SubArray(recordData, 18, recordData.Length - 18);
                    fields.Add(new EftField(4, 9, imageData, isBinary: true));
                }
            }
            else
            {
                // Generic binary record - store raw data
                fields.Add(new EftField(recordType, 1, Encoding.ASCII.GetBytes(recordLength.ToString())));
                fields.Add(new EftField(recordType, 999, recordData, isBinary: true));
            }

            position += recordLength;

            // Skip FS separator if present
            if (position < data.Length && data[position] == Separators.FS)
            {
                position++;
            }

            return new FingerprintRecord(recordType, recordIndex, fields);
        }

        private int ReadBinaryRecordLength(byte[] data, int position)
        {
            // Check if it's ASCII digits (some implementations) or binary
            // ASCII: "0001234" (length as string)
            // Binary: 4 bytes big-endian

            // First check if first 4 bytes are ASCII digits
            bool isAscii = true;
            for (int i = 0; i < 4 && position + i < data.Length; i++)
            {
                if (data[position + i] < '0' || data[position + i] > '9')
                {
                    isAscii = false;
                    break;
                }
            }

            if (isAscii)
            {
                // Find the end of the ASCII length field (terminated by GS or colon)
                int lengthEnd = position;
                while (lengthEnd < data.Length &&
                       data[lengthEnd] >= '0' && data[lengthEnd] <= '9')
                {
                    lengthEnd++;
                }
                string lengthStr = Encoding.ASCII.GetString(data, position, lengthEnd - position);
                if (int.TryParse(lengthStr, out int len))
                {
                    return len;
                }
            }

            // Binary: big-endian 4-byte integer
            return (data[position] << 24) | (data[position + 1] << 16) |
                   (data[position + 2] << 8) | data[position + 3];
        }

        /// <summary>
        /// Parse an ASCII record (Type 1, 2, 9+).
        /// </summary>
        private EftRecord ParseAsciiRecord(byte[] data, ref int position, int recordIndex)
        {
            int recordEnd = FindRecordEnd(data, position);
            if (recordEnd < 0)
            {
                recordEnd = data.Length;
                _warnings.Add($"Record {recordIndex} not terminated with FS separator");
            }

            int recordLength = recordEnd - position;
            if (recordLength == 0)
            {
                position = recordEnd + 1;
                return null;
            }

            var recordParser = new RecordParser();
            var parseResult = recordParser.ParseFields(data, position, recordLength);

            int recordType = parseResult.Fields.Count > 0 ? parseResult.Fields[0].RecordType : 0;

            EftRecord record = CreateRecord(recordType, recordIndex, parseResult.Fields);

            foreach (var warning in parseResult.Warnings)
            {
                _warnings.Add($"Record {recordIndex}: {warning}");
            }

            position = recordEnd + 1;

            return record;
        }

        private EftRecord CreateRecord(int recordType, int recordIndex, List<EftField> fields)
        {
            switch (recordType)
            {
                case 1:
                    return new TransactionRecord(recordIndex, fields);
                case 2:
                    return new DescriptiveRecord(recordIndex, fields);
                case 4:
                case 14:
                    return new FingerprintRecord(recordType, recordIndex, fields);
                default:
                    var record = new UnsupportedRecord(recordType, recordIndex, fields);
                    _warnings.Add($"Unsupported record type {recordType}");
                    return record;
            }
        }

        private int FindRecordEnd(byte[] data, int startPosition)
        {
            for (int i = startPosition; i < data.Length; i++)
            {
                if (data[i] == Separators.FS)
                    return i;
            }
            return -1;
        }

        private static byte[] SubArray(byte[] data, int start, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, start, result, 0, Math.Min(length, data.Length - start));
            return result;
        }
    }
}
