namespace EftViewer.Core.Constants
{
    /// <summary>
    /// ANSI/NIST-ITL separator characters used in EFT file format.
    /// </summary>
    public static class Separators
    {
        /// <summary>File Separator - end of record (0x1C)</summary>
        public const byte FS = 0x1C;

        /// <summary>Group Separator - between fields (0x1D)</summary>
        public const byte GS = 0x1D;

        /// <summary>Record Separator - between subfields (0x1E)</summary>
        public const byte RS = 0x1E;

        /// <summary>Unit Separator - between information items (0x1F)</summary>
        public const byte US = 0x1F;

        /// <summary>Colon - separates tag from value in text fields</summary>
        public const byte Colon = 0x3A;

        /// <summary>Period - separates record type from field number in tags</summary>
        public const byte Period = 0x2E;
    }
}
