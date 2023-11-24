using NI_BE.Aggregation;
using NI_BE.DataDb;
using Serilog;

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

            var filePath = Path.Combine(loadedData, file.FileName+"_Aggregated.csv");

            if (!File.Exists(filePath))
            {


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

                Log.Information("Could not aggregate the data using the api: ", ex.Message);
            }
                Log.Information("Data aggregated and loaded to the database.");
            return "Data aggregated and loaded to the database.";
            }else
            {
                Log.Information("This file is already aggregated.");
                return "This file is already aggregated.";
            }
        }
    }
}

