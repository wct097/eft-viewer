using System;
using System.Collections.Generic;
using System.Linq;

namespace EftViewer.Core.Models
{
    /// <summary>
    /// Base class for all EFT record types.
    /// </summary>
    public class EftRecord
    {
        /// <summary>
        /// Record type number (1, 2, 4, 10, 14, etc.).
        /// </summary>
        public int RecordType { get; }

        /// <summary>
        /// Record index within the file (1-based).
        /// </summary>
        public int RecordIndex { get; }

        /// <summary>
        /// Fields contained in this record.
        /// </summary>
        public IReadOnlyList<EftField> Fields { get; }

        /// <summary>
        /// Whether this record type is fully supported by the parser.
        /// </summary>
        public bool IsSupported { get; protected set; } = true;

        /// <summary>
        /// Any warnings encountered while parsing this record.
        /// </summary>
        public IReadOnlyList<string> Warnings { get; }

        protected readonly List<string> _warnings = new List<string>();

        public EftRecord(int recordType, int recordIndex, List<EftField> fields)
        {
            RecordType = recordType;
            RecordIndex = recordIndex;
            Fields = fields?.AsReadOnly() ?? throw new ArgumentNullException(nameof(fields));
            Warnings = _warnings.AsReadOnly();
        }

        /// <summary>
        /// Get a field by its field number.
        /// </summary>
        public EftField GetField(int fieldNumber)
        {
            return Fields.FirstOrDefault(f => f.FieldNumber == fieldNumber);
        }

        /// <summary>
        /// Get a field's text value by field number.
        /// </summary>
        public string GetFieldValue(int fieldNumber)
        {
            return GetField(fieldNumber)?.TextValue;
        }

        /// <summary>
        /// Record length from field X.001.
        /// </summary>
        public int? Length => int.TryParse(GetFieldValue(1), out var len) ? len : (int?)null;

        /// <summary>
        /// Display name for this record type.
        /// </summary>
        public virtual string DisplayName => $"Type-{RecordType}";

        public override string ToString()
        {
            return $"{DisplayName} (Record {RecordIndex}, {Fields.Count} fields)";
        }

        protected void AddWarning(string warning)
        {
            _warnings.Add(warning);
        }
    }
}
