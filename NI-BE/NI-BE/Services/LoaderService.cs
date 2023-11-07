using NI_BE.DataDb;

namespace NI_BE.Services
{
    public class LoaderService
    {
        private readonly string parsedFolder = Environment.GetEnvironmentVariable("parserFolder");


        public async Task<string> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0 || !file.FileName.Contains(".csv"))
            {
                return "Invalid file";
            }

            var filePath = Path.Combine(parsedFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                LoadData loadData = new LoadData();
                loadData.ExecuteLoader(file.FileName);

            }
            catch (Exception ex)
            {

                Console.WriteLine("Could not load the file into the database using the api: ",ex.Message);
            }

            return "File uploaded and loaded to the database.";
        }
    }


}
