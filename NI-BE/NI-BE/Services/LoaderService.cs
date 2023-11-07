namespace NI_BE.Services
{
    public class LoaderService
    {
        private readonly string watcherFolder = Environment.GetEnvironmentVariable("parsedFolder");


        public async Task<string> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0 || !file.FileName.Contains(".csv"))
            {
                return "Invalid file";
            }

            var filePath = Path.Combine(watcherFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return "File uploaded and loaded to the database.";
        }
    }


}
