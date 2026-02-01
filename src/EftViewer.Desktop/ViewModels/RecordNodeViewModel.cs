using System.Text;
using EftViewer.Core.Models;

namespace EftViewer.Desktop.ViewModels
{
    /// <summary>
    /// Tree node representing an EFT record.
    /// </summary>
    public class RecordNodeViewModel : TreeNodeViewModel
    {
        private readonly EftRecord _record;

        public RecordNodeViewModel(EftRecord record)
        {
            _record = record;
            DisplayName = $"{record.DisplayName} (Record {record.RecordIndex})";

            // Add field children
            foreach (var field in record.Fields)
            {
                Children.Add(new FieldNodeViewModel(field));
            }
        }

        public EftRecord Record => _record;

        public override string DetailText
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Record Type: {_record.RecordType}");
                sb.AppendLine($"Display Name: {_record.DisplayName}");
                sb.AppendLine($"Record Index: {_record.RecordIndex}");
                sb.AppendLine($"Fields: {_record.Fields.Count}");

                if (_record.Length.HasValue)
                {
                    sb.AppendLine($"Length: {_record.Length} bytes");
                }

                // Add type-specific details
                if (_record is TransactionRecord txn)
                {
                    sb.AppendLine();
                    sb.AppendLine("Transaction Details:");
                    sb.AppendLine($"  Version: {txn.Version}");
                    sb.AppendLine($"  Type: {txn.TransactionType}");
                    sb.AppendLine($"  Date: {txn.Date}");
                }
                else if (_record is FingerprintRecord fp)
                {
                    sb.AppendLine();
                    sb.AppendLine("Fingerprint Details:");
                    sb.AppendLine($"  Position: {fp.FingerPosition}");
                    sb.AppendLine($"  Size: {fp.ImageWidth}x{fp.ImageHeight}");
                    sb.AppendLine($"  Resolution: {fp.HorizontalResolution} DPI");
                    sb.AppendLine($"  Compression: {fp.CompressionAlgorithm}");
                    sb.AppendLine($"  Has Image: {fp.HasImageData}");
                    if (fp.HasImageData)
                    {
                        sb.AppendLine($"  Image Data Size: {fp.ImageData?.Length} bytes");
                    }
                }

                if (_record.Warnings.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("Warnings:");
                    foreach (var warning in _record.Warnings)
                    {
                        sb.AppendLine($"  - {warning}");
                    }
                }

                return sb.ToString();
            }
        }

        public override bool HasImage => _record is FingerprintRecord fp && fp.HasImageData;

        public override byte[]? ImageData => (_record as FingerprintRecord)?.ImageData;
    }
}
