using babyNI_LoaderAPI.Middlewares;
using Serilog;

namespace babyNI_LoaderAPI.Services
{
    public class LoaderService
    {
        private readonly string parsedFolder = Environment.GetEnvironmentVariable("parserFolder");
        public async Task<string> UploadFile(string fileUploadPath)
        {
            if (fileUploadPath == null || fileUploadPath.Length == 0 || !fileUploadPath.Contains(".csv"))
            {
                return "Invalid file";
            }

            //var filePath = Path.Combine(parsedFolder, file.FileName);
            //if(!File.Exists(filePath)) { 
            //try
            //{
            //    using (var stream = new FileStream(filePath, FileMode.Create))
            //    {
            //        await file.CopyToAsync(stream);
            //    }
            //}
            //catch (Exception ex)
            //{

            //    Log.Information("File copy using stream was not successful: " + ex.Message);
            //}

            //}

            try
            {
                LoadData loadData = new LoadData();
                loadData.ExecuteLoader(fileUploadPath);

            }
            catch (Exception ex)
            {

                Log.Information("Could not load the file into the database using the api: ", ex.Message);
                return "Could not load the file into the database .";
            }
            return "File uploaded and loaded to the database.";

        }
    }
}
