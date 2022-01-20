using System;
using System.Threading.Tasks;
using Pandora.Core.Interfaces;
using IO = System.IO;
using Pandora.Managers;
using Microsoft.AspNetCore.Hosting;
using static Pandora.Managers.FileUpload;
using Pandora.Core.ViewModels;
using System.Linq;

namespace Pandora.Services
{
    public class ImageService : IImageService
    {

        private readonly IWebHostEnvironment WebHostEnvironment;
        public readonly string UrlBase;
        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
            UrlBase = IO.Path.Combine(webHostEnvironment.ContentRootPath, "uploads");
        }

        public async Task<(byte[] file, string extention)> GetAsync(string directory, string name)
        {
            (byte[] file, string extention) result = await GetImages(directory, name);
            return result;
        }

        private async Task<(byte[] file, string extention)> GetImages(string directory, string name)
        {
            (byte[] file, string extention) result = (null, string.Empty);
            try
            {
                name = cleanValues(name);
                directory = cleanValues(directory);
                string directoryImage = IO.Path.Combine(UrlBase, directory);
                string urlImage = GetURLFile(directoryImage, name);

                if (!string.IsNullOrEmpty(urlImage))
                {
                    IO.File.SetAttributes(urlImage, IO.FileAttributes.Normal);
                }

                if (IO.File.Exists(urlImage))
                {
                    result.file = await IO.File.ReadAllBytesAsync(urlImage);
                    result.extention = IO.Path.GetExtension(urlImage);
                }
                else
                {
                    urlImage = GetURLFile(UrlBase, "notImage");
                    result.file = await IO.File.ReadAllBytesAsync(urlImage);
                    result.extention = IO.Path.GetExtension(urlImage);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("No tiene acceso para acceder a la ruta", ex);
            }

            return result;
        }

        private string GetURLFile(string directoryImage, string fileName)
        {
            string urlImage = string.Empty;
            string[] directoryFiles = IO.Directory.GetFiles(directoryImage)
                                          .Where(x => x.Contains(fileName))
                                          .ToList()
                                          .Select(x => x.Split("\\"))
                                          .Where(x => x.LastOrDefault().Split(".")[0] == fileName)
                                          .FirstOrDefault();
            if (directoryFiles != null)
            {
                urlImage = IO.Path.Combine(directoryFiles);
            }

            return urlImage;
        }

        public async Task<ImageViewModel> PostAsync(FileUpload fileUpload, string directory)
        {
            ImageViewModel result = new ImageViewModel();

            if (fileUpload.files.Length > 0)
            {
                string fileName = fileUpload.files.FileName;
                fileName = cleanValues(fileName);
                directory = cleanValues(directory);
                string url = IO.Path.Combine(UrlBase, directory);
                checkDirectoryExist(url);
                url = IO.Path.Combine(url, fileUpload.files.FileName);
                result = await saveFile(url, fileUpload);
            }
            else
            {
                result.uploaded = false;
                result.message = "El archivo no puede ser blanco";
                result.type = TypeImage.empty.ToString();
            }
            return result;
        }

        private static string cleanValues(string value)
        {
            return value.Trim().ToLower().Replace(" ", string.Empty);
        }

        private void checkDirectoryExist(string url)
        {
            if (!(IO.Directory.Exists(UrlBase)))
            {
                IO.Directory.CreateDirectory(UrlBase);
            }
            if (!(IO.Directory.Exists(url)))
            {
                IO.Directory.CreateDirectory(url);
            }
        }

        private static async Task<ImageViewModel> saveFile(string url, FileUpload fileUpload)
        {
            ImageViewModel result = new ImageViewModel();
            try
            {
                using (IO.FileStream fileStream = IO.File.Create(url))
                {
                    await fileUpload.files.CopyToAsync(fileStream);
                    fileStream.Flush();
                    result.uploaded = true;
                    result.message = "Insertado correctamente";
                    result.type = TypeImage.inserted.ToString();
                }
            }
            catch (System.Exception ex)
            {
                result.uploaded = false;
                result.message = ex.Message;
                result.type = TypeImage.error.ToString();
            }

            return result;
        }
    }

}
