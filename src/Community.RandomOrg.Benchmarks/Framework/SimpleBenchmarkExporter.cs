using System;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace Community.RandomOrg.Benchmarks.Framework
{
    /// <summary>Simple plain-text benchmark summary exporter.</summary>
    internal sealed class SimpleBenchmarkExporter : ExporterBase
    {
        /// <summary>Exports benchmark summary.</summary>
        /// <param name="summary">The benchmark summary.</param>
        /// <param name="logger">The logger for exporting to.</param>
        public override void ExportToLog(Summary summary, ILogger logger)
        {
            if (summary.Table.FullContent.Length == 0)
            {
                return;
            }

            summary.Table.PrintLine(summary.Table.FullHeader, logger, string.Empty, " |");

            var separator = new string[summary.Table.FullHeader.Length];

            for (var i = 0; i < summary.Table.FullHeader.Length; i++)
            {
                separator[i] = new string('-', summary.Table.Columns[i].Width);
            }

            summary.Table.PrintLine(separator, logger, string.Empty, "-|");

            for (var i = 0; i < summary.Table.FullContent.Length; i++)
            {
                summary.Table.PrintLine(summary.Table.FullContent[i], logger, string.Empty, " |");
            }
        }

        /// <summary>Gets the file name suffix.</summary>
        protected override string FileNameSuffix
        {
            get => "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}