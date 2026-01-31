using System.Linq;
using System.Text;
using EftViewer.Core.Metadata;
using EftViewer.Core.Models;

namespace EftViewer.Desktop.ViewModels
{
    /// <summary>
    /// Tree node representing an EFT field.
    /// </summary>
    public class FieldNodeViewModel : TreeNodeViewModel
    {
        private readonly EftField _field;

        public FieldNodeViewModel(EftField field)
        {
            _field = field;
            DisplayName = FieldDictionary.GetDisplayText(field.FieldId);
        }

        public EftField Field => _field;

        public override string DetailText
        {
            get
            {
                var sb = new StringBuilder();
                var fieldName = FieldDictionary.GetFieldName(_field.FieldId);

                sb.AppendLine($"Field ID: {_field.FieldId}");
                if (fieldName != null)
                {
                    sb.AppendLine($"Name: {fieldName}");
                }
                sb.AppendLine($"Record Type: {_field.RecordType}");
                sb.AppendLine($"Field Number: {_field.FieldNumber}");
                sb.AppendLine();

                if (_field.IsBinary)
                {
                    sb.AppendLine($"Value: <binary data>");
                    sb.AppendLine($"Size: {_field.RawValue.Length} bytes");

                    // Show hex preview of first 64 bytes
                    var preview = _field.RawValue.Take(64).ToArray();
                    sb.AppendLine();
                    sb.AppendLine("Hex Preview:");
                    sb.AppendLine(FormatHexDump(preview));
                }
                else
                {
                    sb.AppendLine($"Value: {_field.TextValue}");
                    sb.AppendLine($"Length: {_field.RawValue.Length} bytes");

                    // Show subfields if present
                    if (_field.Subfields.Count > 1)
                    {
                        sb.AppendLine();
                        sb.AppendLine("Subfields:");
                        foreach (var subfield in _field.Subfields)
                        {
                            sb.AppendLine($"  [{subfield.Index}]: {subfield.Value}");

                            // Show items if present
                            if (subfield.Items.Count > 1)
                            {
                                foreach (var item in subfield.Items)
                                {
                                    sb.AppendLine($"       - {item}");
                                }
                            }
                        }
                    }
                }

                return sb.ToString();
            }
        }

        private static string FormatHexDump(byte[] data)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < data.Length; i += 16)
            {
                sb.Append($"{i:X4}: ");

                // Hex bytes
                for (int j = 0; j < 16 && i + j < data.Length; j++)
                {
                    sb.Append($"{data[i + j]:X2} ");
                }

                // Pad if needed
                for (int j = data.Length - i; j < 16; j++)
                {
                    sb.Append("   ");
                }

                sb.Append(" ");

                // ASCII
                for (int j = 0; j < 16 && i + j < data.Length; j++)
                {
                    var b = data[i + j];
                    sb.Append(b >= 32 && b < 127 ? (char)b : '.');
                }

                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
