using System.IO;
using System.Linq;
using System.Text;
using EftViewer.Core.Models;

namespace EftViewer.Desktop.ViewModels
{
    /// <summary>
    /// Tree node representing the root EFT file.
    /// </summary>
    public class FileNodeViewModel : TreeNodeViewModel
    {
        private readonly EftFile _file;

        public FileNodeViewModel(EftFile file)
        {
            _file = file;
            DisplayName = Path.GetFileName(file.FilePath ?? "EFT File");

            // Add record children
            foreach (var record in file.Records)
            {
                Children.Add(new RecordNodeViewModel(record));
            }

            IsExpanded = true;
        }

        public EftFile File => _file;

        public override string DetailText
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine($"File: {_file.FilePath ?? "(no path)"}");
                sb.AppendLine($"Records: {_file.RecordCount}");
                sb.AppendLine($"Fingerprints: {_file.FingerprintCount}");

                if (_file.Transaction != null)
                {
                    sb.AppendLine();
                    sb.AppendLine("Transaction Info:");
                    sb.AppendLine($"  Version: {_file.Transaction.Version}");
                    sb.AppendLine($"  Type: {_file.Transaction.TransactionType}");
                    sb.AppendLine($"  Date: {_file.Transaction.Date}");
                    sb.AppendLine($"  Origin: {_file.Transaction.OriginatingAgency}");
                    sb.AppendLine($"  Destination: {_file.Transaction.DestinationAgency}");
                    sb.AppendLine($"  TCN: {_file.Transaction.TransactionControlNumber}");
                }

                if (_file.Warnings.Any())
                {
                    sb.AppendLine();
                    sb.AppendLine("Warnings:");
                    foreach (var warning in _file.Warnings)
                    {
                        sb.AppendLine($"  - {warning}");
                    }
                }

                return sb.ToString();
            }
        }
    }
}
