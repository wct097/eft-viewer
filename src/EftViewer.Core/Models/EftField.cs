using System;
using System.Collections.Generic;
using System.Text;

namespace EftViewer.Core.Models
{
    /// <summary>
    /// Represents a single field in an EFT record.
    /// Fields are identified by Type.FieldNumber (e.g., "1.002" for version).
    /// </summary>
    public class EftField
    {
        /// <summary>
        /// Record type number (e.g., 1, 2, 14).
        /// </summary>
        public int RecordType { get; }

        /// <summary>
        /// Field number within the record type.
        /// </summary>
        public int FieldNumber { get; }

        /// <summary>
        /// Full field identifier (e.g., "1.002").
        /// </summary>
        public string FieldId => $"{RecordType}.{FieldNumber:D3}";

        /// <summary>
        /// Raw bytes of the field value.
        /// </summary>
        public byte[] RawValue { get; }

        /// <summary>
        /// Subfields if the field contains RS-separated subfields.
        /// </summary>
        public IReadOnlyList<EftSubfield> Subfields { get; }

        /// <summary>
        /// Whether this field contains binary data (vs text).
        /// </summary>
        public bool IsBinary { get; }

        /// <summary>
        /// Text value if not binary. Returns null for binary fields.
        /// </summary>
        public string TextValue => IsBinary ? null : Encoding.ASCII.GetString(RawValue);

        public EftField(int recordType, int fieldNumber, byte[] rawValue, bool isBinary = false, List<EftSubfield> subfields = null)
        {
            RecordType = recordType;
            FieldNumber = fieldNumber;
            RawValue = rawValue ?? throw new ArgumentNullException(nameof(rawValue));
            IsBinary = isBinary;
            Subfields = subfields?.AsReadOnly() ?? (IReadOnlyList<EftSubfield>)Array.Empty<EftSubfield>();
        }

        public override string ToString()
        {
            if (IsBinary)
                return $"{FieldId}: <binary, {RawValue.Length} bytes>";
            return $"{FieldId}: {TextValue}";
        }
    }

    /// <summary>
    /// Represents a subfield within a field (RS-separated).
    /// </summary>
    public class EftSubfield
    {
        /// <summary>
        /// Subfield index (0-based).
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Information items within this subfield (US-separated).
        /// </summary>
        public IReadOnlyList<string> Items { get; }

        /// <summary>
        /// Raw value of the entire subfield.
        /// </summary>
        public string Value { get; }

        public EftSubfield(int index, string value, List<string> items = null)
        {
            Index = index;
            Value = value;
            Items = items?.AsReadOnly() ?? (IReadOnlyList<string>)Array.Empty<string>();
        }
    }
}
