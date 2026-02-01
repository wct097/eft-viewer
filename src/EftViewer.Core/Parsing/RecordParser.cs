using System;
using System.Collections.Generic;
using System.Text;
using EftViewer.Core.Constants;
using EftViewer.Core.Models;

namespace EftViewer.Core.Parsing
{
    /// <summary>
    /// Parses fields within a single record.
    /// </summary>
    internal class RecordParser
    {
        /// <summary>
        /// Result of parsing a record's fields.
        /// </summary>
        public class ParseResult
        {
            public List<EftField> Fields { get; } = new List<EftField>();
            public List<string> Warnings { get; } = new List<string>();
        }

        /// <summary>
        /// Parse all fields from a record.
        /// </summary>
        public ParseResult ParseFields(byte[] data, int recordStart, int recordLength)
        {
            var result = new ParseResult();
            int position = recordStart;
            int recordEnd = recordStart + recordLength;

            while (position < recordEnd)
            {
                // Find the field tag (e.g., "1.002:")
                var tagResult = ParseFieldTag(data, position, recordEnd);
                if (tagResult == null)
                {
                    // No more valid tags found
                    break;
                }

                int recordType = tagResult.Value.RecordType;
                int fieldNumber = tagResult.Value.FieldNumber;
                int valueStart = tagResult.Value.ValueStart;

                // Determine if this is a binary field
                bool isBinary = IsBinaryField(recordType, fieldNumber);

                // Find end of this field
                int fieldEnd;
                if (isBinary)
                {
                    // Binary fields (like image data in field 999) extend to the end of the record
                    // because binary data can contain GS/FS bytes that would cause false termination
                    fieldEnd = recordEnd;
                }
                else
                {
                    // Non-binary fields end at GS separator or end of record
                    fieldEnd = FindFieldEnd(data, valueStart, recordEnd);
                }
                int valueLength = fieldEnd - valueStart;

                // Extract the field value
                byte[] rawValue = new byte[valueLength];
                Array.Copy(data, valueStart, rawValue, 0, valueLength);

                // Parse subfields for non-binary fields
                List<EftSubfield> subfields = null;
                if (!isBinary && valueLength > 0)
                {
                    subfields = ParseSubfields(rawValue);
                }

                var field = new EftField(recordType, fieldNumber, rawValue, isBinary, subfields);
                result.Fields.Add(field);

                // Move past the field and GS separator
                position = fieldEnd;
                if (position < recordEnd && data[position] == Separators.GS)
                {
                    position++;
                }
            }

            return result;
        }

        private (int RecordType, int FieldNumber, int ValueStart)? ParseFieldTag(byte[] data, int start, int end)
        {
            // Look for pattern: digits "." digits ":"
            int pos = start;

            // Skip any leading separators
            while (pos < end && (data[pos] == Separators.GS || data[pos] == Separators.RS || data[pos] == Separators.US))
            {
                pos++;
            }

            if (pos >= end)
                return null;

            // Parse record type number
            int recordTypeStart = pos;
            while (pos < end && data[pos] >= '0' && data[pos] <= '9')
            {
                pos++;
            }

            if (pos == recordTypeStart || pos >= end || data[pos] != Separators.Period)
                return null;

            int recordType = ParseInt(data, recordTypeStart, pos - recordTypeStart);
            pos++; // Skip period

            // Parse field number
            int fieldNumberStart = pos;
            while (pos < end && data[pos] >= '0' && data[pos] <= '9')
            {
                pos++;
            }

            if (pos == fieldNumberStart || pos >= end || data[pos] != Separators.Colon)
                return null;

            int fieldNumber = ParseInt(data, fieldNumberStart, pos - fieldNumberStart);
            pos++; // Skip colon

            return (recordType, fieldNumber, pos);
        }

        private int ParseInt(byte[] data, int start, int length)
        {
            int result = 0;
            for (int i = 0; i < length; i++)
            {
                result = result * 10 + (data[start + i] - '0');
            }
            return result;
        }

        private int FindFieldEnd(byte[] data, int start, int recordEnd)
        {
            for (int i = start; i < recordEnd; i++)
            {
                if (data[i] == Separators.GS || data[i] == Separators.FS)
                    return i;
            }
            return recordEnd;
        }

        private bool IsBinaryField(int recordType, int fieldNumber)
        {
            // Image data fields are binary
            switch (recordType)
            {
                case 4:
                    return fieldNumber == 9; // Type-4 image data
                case 10:
                case 14:
                case 15:
                    return fieldNumber == 999; // Type-10/14/15 image data
                default:
                    return false;
            }
        }

        private List<EftSubfield> ParseSubfields(byte[] data)
        {
            var subfields = new List<EftSubfield>();
            var currentSubfield = new List<byte>();
            int subfieldIndex = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == Separators.RS)
                {
                    // End of subfield
                    var subfieldValue = Encoding.ASCII.GetString(currentSubfield.ToArray());
                    var items = ParseItems(currentSubfield.ToArray());
                    subfields.Add(new EftSubfield(subfieldIndex, subfieldValue, items));
                    currentSubfield.Clear();
                    subfieldIndex++;
                }
                else
                {
                    currentSubfield.Add(data[i]);
                }
            }

            // Don't forget the last subfield
            if (currentSubfield.Count > 0 || subfields.Count > 0)
            {
                var subfieldValue = Encoding.ASCII.GetString(currentSubfield.ToArray());
                var items = ParseItems(currentSubfield.ToArray());
                subfields.Add(new EftSubfield(subfieldIndex, subfieldValue, items));
            }

            return subfields.Count > 0 ? subfields : null;
        }

        private List<string> ParseItems(byte[] data)
        {
            var items = new List<string>();
            var currentItem = new List<byte>();

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == Separators.US)
                {
                    items.Add(Encoding.ASCII.GetString(currentItem.ToArray()));
                    currentItem.Clear();
                }
                else
                {
                    currentItem.Add(data[i]);
                }
            }

            // Don't forget the last item
            items.Add(Encoding.ASCII.GetString(currentItem.ToArray()));

            return items.Count > 1 ? items : null;
        }
    }
}
