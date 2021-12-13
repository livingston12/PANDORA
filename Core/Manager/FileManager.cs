using ExcelDataReader;
using Pandora.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Pandora.Managers
{
    public class FileManager : IFileManager
    {
        private IEnumerable<string> supportFileFormats { get; } = new List<string>(){
            ".xlsx", ".xls", ".csv"
        };

        public DataSet ReadExcelFile(string fileName, Stream stream)
        {
            CheckFileExtension(fileName);
            using (stream)
            {
                var dataSetConfiguration = new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                };

                DataSet dataSet = Path.GetExtension(fileName)
                        .Equals(".csv", StringComparison.InvariantCultureIgnoreCase)
                    ? ReadFromCvsToDataSet(stream, dataSetConfiguration)
                    : ReadFromXlsToDataSet(stream, dataSetConfiguration);
                return dataSet;
            }
        }

        private void CheckFileExtension(string fileName)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            if (!IsSupportExcelExtension(fileName))
            {
                var execptionMessage = "Not support file formats, only support " + string.Join(", ", supportFileFormats) + ".";
                throw new InvalidDataException(execptionMessage);
            }
        }

        public bool IsSupportExcelExtension(string path)
        {
            var extension = Path.GetExtension(path);
            return supportFileFormats.Contains(extension);
        }

        private static DataSet ReadFromXlsToDataSet(Stream stream, ExcelDataSetConfiguration dataSetConfiguration)
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                return reader.AsDataSet(dataSetConfiguration);
            }
        }

        private static DataSet ReadFromCvsToDataSet(Stream stream, ExcelDataSetConfiguration dataSetConfiguration)
        {
            using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
            {
                return reader.AsDataSet(dataSetConfiguration);
            }
        }
    }
}