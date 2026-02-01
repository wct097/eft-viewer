using System.Collections.Generic;

namespace EftViewer.Core.Models
{
    /// <summary>
    /// Type-2 User-Defined Descriptive Text Record.
    /// Contains demographic and case-related information.
    /// </summary>
    public class DescriptiveRecord : EftRecord
    {
        public DescriptiveRecord(int recordIndex, List<EftField> fields)
            : base(2, recordIndex, fields)
        {
        }

        public override string DisplayName => "Descriptive Text";
    }
}
