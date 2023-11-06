

using NI_BE.DataDb;

namespace NI_BE.Aggregation
{
    public class Hourly
    {
        public string _fileLocation { get; set; }
        public string _fileName { get; set; }
        public Hourly()
        {

        }


        public void CreateHourlyTable()
        {
            DBConnection dBConnection = new DBConnection();

            try
            {
                string command = @"CREATE TABLE IF NOT EXISTS TRANS_MW_AGG_SLOT_HOURLY(
                    DATETIME_KEY datetime,
                    TIME_Stamp datetime,
                    NE VARCHAR(20),
                    RSL_INPUT_POWER FLOAT,
                    MAX_RX_LEVEL FLOAT,
                    RSL_DEVIATION FLOAT
                    ) SEGMENTED BY HASH( TIME_Stamp ) ALL NODES;";

                bool success = dBConnection.ConnectAndExecuteQuery(command);
                if(success)
                {
                    Console.WriteLine("Hourly table created.");
                }
                else
                {
                    Console.WriteLine("Creating table Hourly was not succeeded.");
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error creating hourlyTable: " + ex.Message);
            }

        }

    }
}
