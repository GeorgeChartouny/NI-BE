using NI_BE.Aggregation;

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

        public void ExecuteLoader(string FileLoc)
        {

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

                            File.Move(FileLoc, moveLocation);
                            Console.WriteLine("File loaded successfully and moved to loaded folder.");

                            // Aggregate data into hourly table
                            var aggregateHourly = new HourlyAggregation();
                           bool hourlySuccess = aggregateHourly.CreateAndInsertHourlyTable();

                            if (hourlySuccess)
                            {
                                //Aggregate data into daily table
                                var aggregateDaily = new DailyAggregation();
                                aggregateDaily.CreateAndInsertDailyTable();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("File not loaded and not moved to loaded folder");
                    }
                }
                catch (Exception e)
                {

                    Console.WriteLine("Error in Loading process: " + e.Message);
                }


            }
        }


    }
}
