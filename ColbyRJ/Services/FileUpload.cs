using Microsoft.AspNetCore.Components.Forms;

namespace ColbyRJ.Services
{
    public class FileUpload :IFileUpload
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public FileUpload(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        public bool DeleteFile(string fileName, string folder)
        {
            try
            {
                var path = $"{_webHostEnvironment.WebRootPath}\\{folder}\\{fileName}";
                if (File.Exists(path))
                {
                    File.Delete(path);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> UploadFile(IBrowserFile file, string folder)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(file.Name);
                var fileName = Guid.NewGuid().ToString() + fileInfo.Extension;
                var folderDirectory = $"{_webHostEnvironment.WebRootPath}\\{folder}";
                var path = Path.Combine(_webHostEnvironment.WebRootPath, folder, fileName);

                var memoryStream = new MemoryStream();
                // set to 10 mb - 
                await file.OpenReadStream(10485760).CopyToAsync(memoryStream);

                if (!Directory.Exists(folderDirectory))
                {
                    Directory.CreateDirectory(folderDirectory);
                }

                await using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    memoryStream.WriteTo(fs);
                }
                //var url = $"{_configuration.GetValue<string>("ServerUrl")}";
                //var fullPath = $"{url}{folder}/{fileName}";
                //var pathFileName = $"{folder}/{fileName}";
                return fileName;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string UploadFile2(IBrowserFile file, string folder)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(file.Name);
                var fileName = Guid.NewGuid().ToString() + fileInfo.Extension;
                var folderDirectory = $"{_webHostEnvironment.WebRootPath}\\{folder}";
                var path = Path.Combine(_webHostEnvironment.WebRootPath, folder, fileName);

                var memoryStream = new MemoryStream();
                file.OpenReadStream().CopyTo(memoryStream);

                if (!Directory.Exists(folderDirectory))
                {
                    Directory.CreateDirectory(folderDirectory);
                }

                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    memoryStream.WriteTo(fs);
                }
                return fileName;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
