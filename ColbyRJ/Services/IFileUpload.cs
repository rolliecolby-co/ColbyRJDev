using Microsoft.AspNetCore.Components.Forms;

namespace ColbyRJ.Services
{
    public interface IFileUpload
    {
        Task<string> UploadFile(IBrowserFile file, string folder);
        string UploadFile2(IBrowserFile file, string folder);
        bool DeleteFile(string fileName, string folder);
    }
}
