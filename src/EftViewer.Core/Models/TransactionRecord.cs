using System.Collections.Generic;

namespace EftViewer.Core.Models
{
    /// <summary>
    /// Type-1 Transaction Information Record.
    /// Required in every EFT file, contains file-level metadata.
    /// </summary>
    public class TransactionRecord : EftRecord
    {
        public TransactionRecord(int recordIndex, List<EftField> fields)
            : base(1, recordIndex, fields)
        {
        }

        public override string DisplayName => "Transaction Info";

        /// <summary>
        /// Version number (1.002 VER).
        /// </summary>
        public string Version => GetFieldValue(2);

        /// <summary>
        /// File content descriptor (1.003 CNT).
        /// </summary>
        public string ContentDescriptor => GetFieldValue(3);

        /// <summary>
        /// Type of transaction (1.004 TOT).
        /// </summary>
        public string TransactionType => GetFieldValue(4);

        /// <summary>
        /// Date of transaction (1.005 DAT).
        /// </summary>
        public string Date => GetFieldValue(5);

        /// <summary>
        /// Priority (1.006 PRY).
        /// </summary>
        public string Priority => GetFieldValue(6);

        /// <summary>
        /// Destination agency identifier (1.007 DAI).
        /// </summary>
        public string DestinationAgency => GetFieldValue(7);

        /// <summary>
        /// Originating agency identifier (1.008 ORI).
        /// </summary>
        public string OriginatingAgency => GetFieldValue(8);

        /// <summary>
        /// Transaction control number (1.009 TCN).
        /// </summary>
        public string TransactionControlNumber => GetFieldValue(9);

        /// <summary>
        /// Transaction control reference (1.010 TCR).
        /// </summary>
        public string TransactionControlReference => GetFieldValue(10);

        /// <summary>
        /// Native scanning resolution (1.011 NSR).
        /// </summary>
        public string NativeScanningResolution => GetFieldValue(11);

        /// <summary>
        /// Nominal transmitting resolution (1.012 NTR).
        /// </summary>
        public string NominalTransmittingResolution => GetFieldValue(12);

        /// <summary>
        /// Domain name (1.013 DOM).
        /// </summary>
        public string DomainName => GetFieldValue(13);

        /// <summary>
        /// Greenwich mean time (1.014 GMT).
        /// </summary>
        public string GreenwichMeanTime => GetFieldValue(14);
    }
}
