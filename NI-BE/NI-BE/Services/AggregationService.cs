using NI_BE.Aggregation;
using NI_BE.DataDb;

namespace NI_BE.Services
{
    public class AggregationService
    {
        private readonly string loadedData = Environment.GetEnvironmentVariable("loadedData");

        public async Task<string> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0 || !file.FileName.Contains(".csv"))
            {
                return "Invalid file";
            }

            var filePath = Path.Combine(loadedData, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                var aggregateHourly = new HourlyAggregation();
                bool hourlySuccess = aggregateHourly.CreateAndInsertHourlyTable();

                if (hourlySuccess)
                {
                    //Aggregate data into daily table
                    var aggregateDaily = new DailyAggregation();
                    aggregateDaily.CreateAndInsertDailyTable();
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine("Could not aggregate the data using the api: ", ex.Message);
            }

            return "Data aggregated and loaded to the database.";
        }
    }
}

