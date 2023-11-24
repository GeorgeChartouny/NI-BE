using Serilog;

namespace NI_BE.Middleware
{
    public class UploadFileToApi
    {


        public async Task<string> UploadFileToApiAsync(string apiEndPoint, string filePath)
        {

            if (string.IsNullOrEmpty(apiEndPoint) || string.IsNullOrEmpty(filePath))
            {
                Log.Information("API or file path is null or invalid.");
                throw new ArgumentException("API or file path is null or invalid.");
            }

            if (!File.Exists(filePath))
            {
                Log.Information("File not found at the specified path. " + filePath);
                throw new FileNotFoundException("File not found at the specified path. ", filePath);
            }
            using (HttpClient client = new HttpClient())

            //var content = new MultipartFormDataContent();
            using (var content = new MultipartFormDataContent())
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var fileContent = new StreamContent(fileStream);
                content.Add(fileContent, "file", Path.GetFileName(filePath));
                return await SendRequestAsync(client, apiEndPoint, content);
            }

            //    using (var fileStream = new FileStream(filePath, FileMode.Open))
            //    {
            //        var fileContent = new StreamContent(fileStream);

            //        content.Add(fileContent, "file", Path.GetFileName(filePath));

            //        HttpResponseMessage response = await client.PostAsync(apiEndpoint, content);


            //if (response.IsSuccessStatusCode)
            //{
            //    string apiResponse = await response.Content.ReadAsStringAsync();
            //    Log.Information(apiResponse);
            //}else
            //        {
            //            Log.Information("File upload failed with status: " + response.StatusCode);
            //        }
            //    }

        }

        private async Task<string> SendRequestAsync(HttpClient client, string apiEndpoint, HttpContent content)
        {
            HttpResponseMessage response = await client.PostAsync(apiEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                Log.Information(apiResponse);
                return apiResponse;

            }
            else
            {
                string errorMessage = "File upload failed with status: " + response.StatusCode;
                Log.Information(errorMessage);
                throw new HttpRequestException(errorMessage);

            }
        }

    }
}
