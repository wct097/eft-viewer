using System;
using System.Collections.Generic;
using System.IO;
using EftViewer.Core.Constants;
using EftViewer.Core.Exceptions;
using EftViewer.Core.Models;

namespace EftViewer.Core.Parsing
{
    /// <summary>
    /// Parser for ANSI/NIST-ITL EFT files.
    /// </summary>
    public class EftParser
    {
        private readonly List<string> _warnings = new List<string>();

        /// <summary>
        /// Parse an EFT file from a file path.
        /// </summary>
        public EftFile Parse(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            byte[] data = File.ReadAllBytes(filePath);
            return Parse(data, filePath);
        }

        /// <summary>
        /// Parse an EFT file from a stream.
        /// </summary>
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

        /// <summary>
        /// Parse an EFT file from raw bytes.
        /// </summary>
        public EftFile Parse(byte[] data, string filePath = null)
        {
            if (data == null || data.Length == 0)
                throw new EftParseException("File is empty or null");

            _warnings.Clear();
            var records = new List<EftRecord>();
            int position = 0;
            int recordIndex = 1;

            while (position < data.Length)
            {
                // Skip any leading whitespace or null bytes
                while (position < data.Length && (data[position] == 0 || data[position] == ' ' || data[position] == '\r' || data[position] == '\n'))
                {
                    position++;
                }

                if (position >= data.Length)
                    break;

                try
                {
                    var record = ParseRecord(data, ref position, recordIndex);
                    if (record != null)
                    {
                        records.Add(record);
                        recordIndex++;
                    }
                }
                catch (EftParseException ex)
                {
                    _warnings.Add($"Error parsing record {recordIndex} at position {ex.Position}: {ex.Message}");
                    // Try to recover by finding next FS separator
                    position = FindNextRecordStart(data, position);
                    if (position < 0)
                        break;
                }
            }

            if (records.Count == 0)
            {
                _warnings.Add("No records found in file");
            }

            return new EftFile(filePath, records, new List<string>(_warnings));
        }

        private EftRecord ParseRecord(byte[] data, ref int position, int recordIndex)
        {
            int startPosition = position;

            // Find the end of this record (FS separator)
            int recordEnd = FindRecordEnd(data, position);
            if (recordEnd < 0)
            {
                // No FS found, assume rest of file is this record
                recordEnd = data.Length;
                _warnings.Add($"Record {recordIndex} not terminated with FS separator");
            }

            // Extract record data
            int recordLength = recordEnd - position;
            if (recordLength == 0)
            {
                position = recordEnd + 1;
                return null;
            }

            // Parse fields from record data
            var recordParser = new RecordParser();
            var parseResult = recordParser.ParseFields(data, position, recordLength);

            // Determine record type from first field
            int recordType = DetermineRecordType(parseResult.Fields);

            // Create appropriate record type
            EftRecord record = CreateRecord(recordType, recordIndex, parseResult.Fields);

            // Add any field parsing warnings
            foreach (var warning in parseResult.Warnings)
            {
                _warnings.Add($"Record {recordIndex}: {warning}");
            }

            // Move position past the FS separator
            position = recordEnd + 1;

            return record;
        }

        private int DetermineRecordType(List<EftField> fields)
        {
            if (fields.Count == 0)
                return 0;

            // Record type is determined by the first field's type component
            return fields[0].RecordType;
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
                    // Unsupported record type - create generic record marked as unsupported
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

        private int FindNextRecordStart(byte[] data, int startPosition)
        {
            // Find the next FS and return position after it
            for (int i = startPosition; i < data.Length; i++)
            {
                if (data[i] == Separators.FS)
                    return i + 1;
            }
            return -1;
        }
    }
}
