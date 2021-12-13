using System.Data;
using System.IO;

namespace Pandora.Core.Interfaces
{
    public interface IFileManager
    {
        DataSet ReadExcelFile(string fileName, Stream stream);
        bool IsSupportExcelExtension(string path);
    }
}