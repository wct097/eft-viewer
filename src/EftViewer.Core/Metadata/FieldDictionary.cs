using System.Collections.Generic;

namespace EftViewer.Core.Metadata
{
    /// <summary>
    /// Maps field identifiers to human-readable names based on ANSI/NIST-ITL standard.
    /// </summary>
    public static class FieldDictionary
    {
        private static readonly Dictionary<string, string> FieldNames = new Dictionary<string, string>
        {
            // Type-1 Transaction Information Record
            ["1.001"] = "Record Length",
            ["1.002"] = "Version Number",
            ["1.003"] = "File Content",
            ["1.004"] = "Type of Transaction",
            ["1.005"] = "Date",
            ["1.006"] = "Priority",
            ["1.007"] = "Destination Agency",
            ["1.008"] = "Originating Agency",
            ["1.009"] = "Transaction Control Number",
            ["1.010"] = "Transaction Control Reference",
            ["1.011"] = "Native Scanning Resolution",
            ["1.012"] = "Nominal Transmitting Resolution",
            ["1.013"] = "Domain Name",
            ["1.014"] = "Greenwich Mean Time",
            ["1.015"] = "Directory of Character Sets",

            // Type-2 User-Defined Descriptive Text Record
            ["2.001"] = "Record Length",
            ["2.002"] = "Information Designation Character",
            // Type-2 fields are largely user-defined, so we include common ones

            // Type-4 Grayscale Fingerprint Image (legacy)
            ["4.001"] = "Record Length",
            ["4.002"] = "Image Designation Character",
            ["4.003"] = "Impression Type",
            ["4.004"] = "Finger Position",
            ["4.005"] = "Image Scanning Resolution",
            ["4.006"] = "Horizontal Line Length",
            ["4.007"] = "Vertical Line Length",
            ["4.008"] = "Compression Algorithm",
            ["4.009"] = "Image Data",

            // Type-14 Variable-Resolution Fingerprint Image
            ["14.001"] = "Record Length",
            ["14.002"] = "Image Designation Character",
            ["14.003"] = "Finger Position",
            ["14.004"] = "Impression Type",
            ["14.005"] = "Source Agency",
            ["14.006"] = "Horizontal Line Length",
            ["14.007"] = "Vertical Line Length",
            ["14.008"] = "Scale Units",
            ["14.009"] = "Horizontal Pixel Scale",
            ["14.010"] = "Vertical Pixel Scale",
            ["14.011"] = "Compression Algorithm",
            ["14.012"] = "Bits Per Pixel",
            ["14.013"] = "Finger/Palm Position",
            ["14.014"] = "Print Position Descriptors",
            ["14.015"] = "Print Position Coordinates",
            ["14.016"] = "Scanned Horizontal Pixel Scale",
            ["14.017"] = "Scanned Vertical Pixel Scale",
            ["14.020"] = "Comment",
            ["14.024"] = "Quality Metric",
            ["14.030"] = "Device Monitoring Mode",
            ["14.999"] = "Image Data",

            // Type-10 Face/SMT Image (for future support)
            ["10.001"] = "Record Length",
            ["10.002"] = "Image Designation Character",
            ["10.003"] = "Image Type",
            ["10.999"] = "Image Data",
        };

        /// <summary>
        /// Get the human-readable name for a field, or null if not found.
        /// </summary>
        public static string GetFieldName(string fieldId)
        {
            return FieldNames.TryGetValue(fieldId, out var name) ? name : null;
        }

        /// <summary>
        /// Get the human-readable name for a field, or a default if not found.
        /// </summary>
        public static string GetFieldName(int recordType, int fieldNumber)
        {
            var fieldId = $"{recordType}.{fieldNumber:D3}";
            return GetFieldName(fieldId) ?? $"Field {fieldNumber}";
        }

        /// <summary>
        /// Get display text for a field (name + ID).
        /// </summary>
        public static string GetDisplayText(string fieldId)
        {
            var name = GetFieldName(fieldId);
            return name != null ? $"{name} ({fieldId})" : fieldId;
        }

        /// <summary>
        /// Get display text for a field (name + ID).
        /// </summary>
        public static string GetDisplayText(int recordType, int fieldNumber)
        {
            var fieldId = $"{recordType}.{fieldNumber:D3}";
            return GetDisplayText(fieldId);
        }
    }
}
