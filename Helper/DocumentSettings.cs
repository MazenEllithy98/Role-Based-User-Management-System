using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Demo.PL.Helper
{
    public static class DocumentSettings
    {
        public static async Task<string> UploadFileAsync(IFormFile File , string FolderName)
        {
            //Get Located folder path
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "\\wwwroot\\Files" , FolderName);
            //Get File Name and make it Unique
            string fileName = $"{Guid.NewGuid()}{File?.FileName}";
            //Get File Path
            string filePath = Path.Combine(folderPath, fileName);
            //Save file as Streams : [Data Per Time]
            using var fs = new FileStream(filePath , FileMode.Create);
            await File?.CopyToAsync(fs);
            return fileName;


        }

        public static void DeleteFile(string fileName , string FolderName)
        {
            if (fileName is not null && FolderName is not null)
            {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files" , FolderName , fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
            }

        }
    }
}
