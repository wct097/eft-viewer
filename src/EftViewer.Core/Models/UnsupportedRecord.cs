using System.Collections.Generic;

namespace EftViewer.Core.Models
{
    /// <summary>
    /// Represents a record type that is not fully supported by the parser.
    /// The raw data is preserved but specialized interpretation is not available.
    /// </summary>
    public class UnsupportedRecord : EftRecord
    {
        public UnsupportedRecord(int recordType, int recordIndex, List<EftField> fields)
            : base(recordType, recordIndex, fields)
        {
            IsSupported = false;
        }

        public override string DisplayName => $"Type-{RecordType} (Unsupported)";
    }
}
