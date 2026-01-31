using System;

namespace EftViewer.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when parsing an EFT file fails.
    /// </summary>
    public class EftParseException : Exception
    {
        /// <summary>
        /// Position in the file where the error occurred.
        /// </summary>
        public long Position { get; }

        /// <summary>
        /// Record type being parsed when error occurred, if known.
        /// </summary>
        public int? RecordType { get; }

        /// <summary>
        /// Field identifier being parsed when error occurred, if known.
        /// </summary>
        public string FieldId { get; }

        public EftParseException(string message) : base(message)
        {
        }

        public EftParseException(string message, long position) : base(message)
        {
            Position = position;
        }

        public EftParseException(string message, long position, int recordType) : base(message)
        {
            Position = position;
            RecordType = recordType;
        }

        public EftParseException(string message, long position, int recordType, string fieldId) : base(message)
        {
            Position = position;
            RecordType = recordType;
            FieldId = fieldId;
        }

        public EftParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
