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
                string moveLocation = @"C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\LoadedParsedData";
                if (fileName.Contains("RADIO_LINK_POWER"))
                {
                    table = "TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER";
                }
                else if (fileName.Contains("TN_RFInputPower"))
                {
                    table = "TRANS_MW_ERC_PM_WAN_RFINPUTPOWER";
                }

                try
                {
                    // sql copy command based on the file passed in the parameter
                    string CopyCommand = $@"COPY {table}
                FROM LOCAL '{FileLoc}'
                DELIMITER ',' SKIP 1
                REJECTED DATA 'C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\UnloadedData\rejected_{fileName}'
                EXCEPTIONS 'C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\UnloadedData\exceptions_{fileName}' ;";
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
                            var aggregateHourly = new HourlyAggregation();
                           bool hourlySuccess = aggregateHourly.CreateAndInsertHourlyTable();

                            if (hourlySuccess)
                            {
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
