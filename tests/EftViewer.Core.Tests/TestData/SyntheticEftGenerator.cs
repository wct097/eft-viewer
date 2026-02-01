using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EftViewer.Core.Tests.TestData
{
    /// <summary>
    /// Generates synthetic EFT files for testing purposes.
    /// </summary>
    public static class SyntheticEftGenerator
    {
        private const byte FS = 0x1C; // File Separator (end of record)
        private const byte GS = 0x1D; // Group Separator (between fields)
        private const byte RS = 0x1E; // Record Separator (between subfields)
        private const byte US = 0x1F; // Unit Separator (between items)

        /// <summary>
        /// Generate a minimal valid EFT file with Type-1 and Type-2 records.
        /// </summary>
        public static byte[] GenerateMinimalFile()
        {
            using (var ms = new MemoryStream())
            {
                // Type-1 Transaction Record
                WriteField(ms, "1.001", "148");  // Record length
                WriteField(ms, "1.002", "0501"); // Version (ANSI/NIST-ITL 1-2011)
                WriteField(ms, "1.003", "1" + (char)US + "1" + (char)RS + "2" + (char)US + "00"); // CNT: 1 Type-1, 1 Type-2
                WriteField(ms, "1.004", "TEST"); // Transaction type
                WriteField(ms, "1.005", "20260131"); // Date
                WriteField(ms, "1.007", "DESTAG"); // Destination agency
                WriteField(ms, "1.008", "ORIGAG"); // Originating agency
                WriteField(ms, "1.009", "TCN123456", isLast: true); // Transaction control number
                ms.WriteByte(FS);

                // Type-2 Descriptive Record
                WriteField(ms, "2.001", "50"); // Record length
                WriteField(ms, "2.002", "00"); // IDC
                WriteField(ms, "2.901", "Test Subject Name", isLast: true); // User-defined field
                ms.WriteByte(FS);

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Generate an EFT file with fingerprint records (placeholder binary data).
        /// </summary>
        public static byte[] GenerateWithFingerprint()
        {
            using (var ms = new MemoryStream())
            {
                // Type-1 Transaction Record
                WriteField(ms, "1.001", "160");
                WriteField(ms, "1.002", "0501");
                WriteField(ms, "1.003", "1" + (char)US + "2" + (char)RS + "2" + (char)US + "00" + (char)RS + "14" + (char)US + "01");
                WriteField(ms, "1.004", "CRM");
                WriteField(ms, "1.005", "20260131");
                WriteField(ms, "1.007", "FBIATF00");
                WriteField(ms, "1.008", "ATF12345");
                WriteField(ms, "1.009", "TCN20260131001", isLast: true);
                ms.WriteByte(FS);

                // Type-2 Descriptive Record
                WriteField(ms, "2.001", "45");
                WriteField(ms, "2.002", "00");
                WriteField(ms, "2.020", "John Doe", isLast: true);
                ms.WriteByte(FS);

                // Type-14 Fingerprint Record
                WriteField(ms, "14.001", "200");
                WriteField(ms, "14.002", "01"); // IDC
                WriteField(ms, "14.003", "1");  // Right thumb
                WriteField(ms, "14.004", "0");  // Live scan plain
                WriteField(ms, "14.005", "ATF12345");
                WriteField(ms, "14.006", "512"); // Width
                WriteField(ms, "14.007", "512"); // Height
                WriteField(ms, "14.008", "1");   // Scale units (inches)
                WriteField(ms, "14.009", "500"); // Horizontal DPI
                WriteField(ms, "14.010", "500"); // Vertical DPI
                WriteField(ms, "14.011", "WSQ20"); // Compression
                WriteField(ms, "14.012", "8");    // Bits per pixel
                // Write placeholder binary image data for field 14.999
                WriteBinaryField(ms, "14.999", GeneratePlaceholderImageData(), isLast: true);
                ms.WriteByte(FS);

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Generate a malformed file for error handling tests.
        /// </summary>
        public static byte[] GenerateMalformed()
        {
            using (var ms = new MemoryStream())
            {
                // Type-1 with incomplete field
                WriteField(ms, "1.001", "50");
                WriteField(ms, "1.002", "0501");
                // Missing required fields, truncated
                ms.WriteByte(FS);

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Generate a file with unknown record type.
        /// </summary>
        public static byte[] GenerateWithUnknownType()
        {
            using (var ms = new MemoryStream())
            {
                // Type-1 Transaction Record
                WriteField(ms, "1.001", "100");
                WriteField(ms, "1.002", "0501");
                WriteField(ms, "1.003", "1" + (char)US + "1" + (char)RS + "99" + (char)US + "01"); // Unknown type 99
                WriteField(ms, "1.004", "TEST");
                WriteField(ms, "1.005", "20260131");
                WriteField(ms, "1.007", "DESTAG");
                WriteField(ms, "1.008", "ORIGAG");
                WriteField(ms, "1.009", "TCN999", isLast: true);
                ms.WriteByte(FS);

                // Type-99 Unknown Record
                WriteField(ms, "99.001", "30");
                WriteField(ms, "99.002", "01");
                WriteField(ms, "99.999", "Unknown data", isLast: true);
                ms.WriteByte(FS);

                return ms.ToArray();
            }
        }

        private static void WriteField(MemoryStream ms, string tag, string value, bool isLast = false)
        {
            var fieldBytes = Encoding.ASCII.GetBytes($"{tag}:{value}");
            ms.Write(fieldBytes, 0, fieldBytes.Length);
            if (!isLast)
            {
                ms.WriteByte(GS);
            }
        }

        private static void WriteBinaryField(MemoryStream ms, string tag, byte[] value, bool isLast = false)
        {
            var tagBytes = Encoding.ASCII.GetBytes($"{tag}:");
            ms.Write(tagBytes, 0, tagBytes.Length);
            ms.Write(value, 0, value.Length);
            if (!isLast)
            {
                ms.WriteByte(GS);
            }
        }

        private static byte[] GeneratePlaceholderImageData()
        {
            // Generate placeholder bytes that look like WSQ header
            // Real WSQ starts with 0xFF 0xA0 but we'll just use placeholder
            // IMPORTANT: Avoid control characters (0x1C-0x1F) that would be
            // interpreted as ANSI/NIST-ITL separators
            var data = new byte[100];
            data[0] = 0xFF;
            data[1] = 0xA0;
            // Fill rest with safe bytes (avoiding 0x1C-0x1F separators)
            for (int i = 2; i < data.Length; i++)
            {
                byte b = (byte)((i + 0x20) % 256);
                // Skip separator range
                if (b >= 0x1C && b <= 0x1F)
                    b = 0x20;
                data[i] = b;
            }
            return data;
        }
    }
}
