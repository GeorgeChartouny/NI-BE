using NI_BE.Aggregation;
using Serilog;

namespace NI_BE.DataDb
{
    public class LoadData
    {
        public string _fileLocation { get; set; }
        public string _fileName { get; set; }
        public LoadData()
        {

        }
        public LoadData(string FileLocation, string FileName)
        {
            _fileLocation = FileLocation;
            _fileName = FileName;
        }

        public async void ExecuteLoader(string FileLoc)
        {
            string parsedFolder = Environment.GetEnvironmentVariable("parserFolder");

            DBConnection dBConnection = new DBConnection();
            if (FileLoc.Contains(".csv"))
            {
                string table = "";
                string fileName = FileLoc.Split("\\").Last();
                string moveLocation = Environment.GetEnvironmentVariable("loadedData");
                if (fileName.Contains("RADIO_LINK_POWER"))
                {
                    table = Environment.GetEnvironmentVariable("table_Radio_link_power");
                }
                else if (fileName.Contains("TN_RFInputPower"))
                {
                    table = Environment.GetEnvironmentVariable("table_RF_Input_power");
                }

                if (!File.Exists(parsedFolder + "\\" + fileName) && !FileLoc.Equals( parsedFolder+fileName))
                {
                    FileLoc = parsedFolder + "\\" + FileLoc;
                }
                try
                {
                    // sql copy command based on the file passed in the parameter
                    string CopyCommand = $@"COPY {table}
                FROM LOCAL '{FileLoc}'
                DELIMITER ',' SKIP 1
                REJECTED DATA '{Environment.GetEnvironmentVariable("rejectedPath")}{fileName}'
                EXCEPTIONS '{Environment.GetEnvironmentVariable("exceptionPath")}{fileName}' ;";
                    bool success = dBConnection.ConnectAndExecuteQuery(CopyCommand);
                    if (success)
                    {
                        // move the parsed file to LoadedData folder
                        string newFileName = fileName + "_Loaded.csv";
                        moveLocation = Path.Combine(moveLocation, newFileName);
                        if (!File.Exists(moveLocation))
                        {

                            try
                            {

                         
                            File.Copy(FileLoc, moveLocation);
                            Log.Information("File loaded successfully and moved to loaded folder.");

                            }
                            catch (Exception ex)
                            {

                                Log.Information("Unable to copy the file to the loaded folder: " + ex.Message);
                            }
                            // Aggregate data into hourly table
                            // var aggregateHourly = new HourlyAggregation();
                            //bool hourlySuccess = aggregateHourly.CreateAndInsertHourlyTable();
                        }
                            try
                            {
                                string apiEndpoint =$@"{Environment.GetEnvironmentVariable("appUrl")}"+ "/api/Aggregation/aggregate-file";
                                string filePath = Path.GetFullPath(moveLocation);

                                using(HttpClient client = new HttpClient())
                                {
                                    var content = new MultipartFormDataContent();

                                    using (var fileStream = new FileStream(filePath, FileMode.Open))
                                    {
                                        var fileContent = new StreamContent(fileStream);
                                        content.Add(fileContent, "file", Path.GetFileName(filePath));

                                        HttpResponseMessage hourlySuccess = await client.PostAsync(apiEndpoint, content);
                                    }
                                }
                            }catch (Exception ex)
                            {

                                Log.Information("Could not call the aggregated api : " + ex.Message);
                            }

                            //if (hourlySuccess)
                            //{
                            //    //Aggregate data into daily table
                            //    var aggregateDaily = new DailyAggregation();
                            //    aggregateDaily.CreateAndInsertDailyTable();
                            //}
                    }
                    else
                    {
                        Log.Information("File not loaded and not moved to loaded folder");
                    }
                }
                catch (Exception e)
                {

                    Log.Information("Error in Loading process: " + e.Message);
                }


            }
        }


    }
}
