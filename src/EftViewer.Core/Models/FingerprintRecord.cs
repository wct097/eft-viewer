using System.Collections.Generic;

namespace EftViewer.Core.Models
{
    /// <summary>
    /// Fingerprint image record (Type-4 or Type-14).
    /// Type-4: Legacy grayscale fingerprint image (fixed 500 DPI).
    /// Type-14: Variable-resolution fingerprint image (current standard).
    /// </summary>
    public class FingerprintRecord : EftRecord
    {
        public FingerprintRecord(int recordType, int recordIndex, List<EftField> fields)
            : base(recordType, recordIndex, fields)
        {
            if (recordType != 4 && recordType != 14)
            {
                AddWarning($"FingerprintRecord created with unexpected type {recordType}");
            }
        }

        public override string DisplayName => RecordType == 4
            ? "Fingerprint (Legacy)"
            : "Fingerprint";

        /// <summary>
        /// Whether this is a Type-14 (current standard) vs Type-4 (legacy).
        /// </summary>
        public bool IsType14 => RecordType == 14;

        /// <summary>
        /// Image designation character (X.003 FGP for Type-4, 14.003 FGP for Type-14).
        /// Indicates which finger(s) are represented.
        /// </summary>
        public string FingerPosition => GetFieldValue(3);

        /// <summary>
        /// Impression type (X.004 IMP).
        /// </summary>
        public string ImpressionType => GetFieldValue(4);

        /// <summary>
        /// Horizontal line length / image width (X.006 HLL).
        /// </summary>
        public int? ImageWidth
        {
            get
            {
                var value = GetFieldValue(6);
                return int.TryParse(value, out var width) ? width : (int?)null;
            }
        }

        /// <summary>
        /// Vertical line length / image height (X.007 VLL).
        /// </summary>
        public int? ImageHeight
        {
            get
            {
                var value = GetFieldValue(7);
                return int.TryParse(value, out var height) ? height : (int?)null;
            }
        }

        /// <summary>
        /// Scale units (X.008 SLC). 1=inches, 2=centimeters.
        /// </summary>
        public string ScaleUnits => GetFieldValue(8);

        /// <summary>
        /// Horizontal pixel scale / DPI (X.009 HPS).
        /// </summary>
        public int? HorizontalResolution
        {
            get
            {
                var value = GetFieldValue(9);
                return int.TryParse(value, out var res) ? res : (int?)null;
            }
        }

        /// <summary>
        /// Vertical pixel scale / DPI (X.010 VPS).
        /// </summary>
        public int? VerticalResolution
        {
            get
            {
                var value = GetFieldValue(10);
                return int.TryParse(value, out var res) ? res : (int?)null;
            }
        }

        /// <summary>
        /// Compression algorithm (X.011 CGA).
        /// Common values: WSQ20 (WSQ), JPEGB (JPEG baseline), etc.
        /// </summary>
        public string CompressionAlgorithm => GetFieldValue(11);

        /// <summary>
        /// Bits per pixel (X.012 BPX).
        /// </summary>
        public int? BitsPerPixel
        {
            get
            {
                var value = GetFieldValue(12);
                return int.TryParse(value, out var bpp) ? bpp : (int?)null;
            }
        }

        /// <summary>
        /// Image data field number varies by type.
        /// Type-4: field 9, Type-14: field 999.
        /// </summary>
        public int ImageDataFieldNumber => RecordType == 4 ? 9 : 999;

        /// <summary>
        /// Raw image data (compressed, typically WSQ).
        /// </summary>
        public byte[] ImageData => GetField(ImageDataFieldNumber)?.RawValue;

        /// <summary>
        /// Whether image data is present.
        /// </summary>
        public bool HasImageData => ImageData != null && ImageData.Length > 0;

        /// <summary>
        /// Whether the compression algorithm indicates WSQ.
        /// </summary>
        public bool IsWsqCompressed
        {
            get
            {
                var algo = CompressionAlgorithm?.ToUpperInvariant();
                return algo != null && (algo.Contains("WSQ") || algo == "0");
            }
        }
    }
}
