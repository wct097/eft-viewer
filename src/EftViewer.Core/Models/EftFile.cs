using System;
using System.Collections.Generic;
using System.Linq;

namespace EftViewer.Core.Models
{
    /// <summary>
    /// Root container representing a parsed EFT file.
    /// </summary>
    public class EftFile
    {
        /// <summary>
        /// Original file path, if loaded from disk.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// All records in the file, in order.
        /// </summary>
        public IReadOnlyList<EftRecord> Records { get; }

        /// <summary>
        /// Type-1 Transaction record (required, exactly one per file).
        /// </summary>
        public TransactionRecord Transaction { get; }

        /// <summary>
        /// Type-2 Descriptive records.
        /// </summary>
        public IReadOnlyList<DescriptiveRecord> DescriptiveRecords { get; }

        /// <summary>
        /// Type-4 and Type-14 Fingerprint records.
        /// </summary>
        public IReadOnlyList<FingerprintRecord> FingerprintRecords { get; }

        /// <summary>
        /// Records of unsupported or unknown types.
        /// </summary>
        public IReadOnlyList<EftRecord> UnsupportedRecords { get; }

        /// <summary>
        /// Warnings encountered during parsing.
        /// </summary>
        public IReadOnlyList<string> Warnings { get; }

        /// <summary>
        /// Whether the file was parsed successfully (may still have warnings).
        /// </summary>
        public bool IsValid => Transaction != null;

        /// <summary>
        /// Total number of records.
        /// </summary>
        public int RecordCount => Records.Count;

        /// <summary>
        /// Total number of fingerprint images.
        /// </summary>
        public int FingerprintCount => FingerprintRecords.Count;

        public EftFile(
            string filePath,
            List<EftRecord> records,
            List<string> warnings)
        {
            FilePath = filePath;
            Records = records?.AsReadOnly() ?? throw new ArgumentNullException(nameof(records));
            Warnings = warnings?.AsReadOnly() ?? (IReadOnlyList<string>)Array.Empty<string>();

            // Categorize records by type
            Transaction = records.OfType<TransactionRecord>().FirstOrDefault();
            DescriptiveRecords = records.OfType<DescriptiveRecord>().ToList().AsReadOnly();
            FingerprintRecords = records.OfType<FingerprintRecord>().ToList().AsReadOnly();
            UnsupportedRecords = records.Where(r => !r.IsSupported).ToList().AsReadOnly();
        }

        /// <summary>
        /// Get a summary string for display in status bar.
        /// </summary>
        public string GetSummary()
        {
            var parts = new List<string>
            {
                $"{RecordCount} record{(RecordCount != 1 ? "s" : "")}"
            };

            if (FingerprintCount > 0)
            {
                parts.Add($"{FingerprintCount} fingerprint{(FingerprintCount != 1 ? "s" : "")}");
            }

            if (Warnings.Count > 0)
            {
                parts.Add($"{Warnings.Count} warning{(Warnings.Count != 1 ? "s" : "")}");
            }

            return string.Join(" | ", parts);
        }

        public override string ToString()
        {
            return $"EftFile: {FilePath ?? "(no path)"} - {GetSummary()}";
        }
    }
}
