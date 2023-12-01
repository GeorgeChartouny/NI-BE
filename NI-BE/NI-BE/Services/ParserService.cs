using NI_BE.DataDb;
using System.Text;
using NI_BE.Parser;
using System.Security.Cryptography;
using NI_BE.DataDb.Models;

namespace NI_BE.Services
{
    public class ParserService
    {
        private readonly string watcherFolder = Environment.GetEnvironmentVariable("watcherFolder");



        public async Task<string> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "Invalid file";
            }

            var filePath = Path.Combine(watcherFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return "File uploaded and parsed.";
        }

    }

}
