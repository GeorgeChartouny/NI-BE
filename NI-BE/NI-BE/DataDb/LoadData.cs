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
        //csvFile	"C:\\Users\\User\\Desktop\\G\\Baby NI Project\\Code\\NI-BE\\NI-BE\\NI-BE\\Data\\ParsedData\\SOEM1_TN_RADIO_LINK_POWER_20200312_001500.csv"	string

        public void ExecuteLoader(string FileLoc)
        {

            DBConnection dBConnection = new DBConnection();
            if (FileLoc.Contains(".csv"))
            {
                string table = "";
                string fileName = FileLoc.Split("\\").Last();
                if (fileName.Contains("RADIO_LINK_POWER"))
                {
                    table = $"TRANS_MW_ERC_PM_TN_{fileName}";
                }
                else if (fileName.Contains("TN_RFInputPower"))
                {
                    table = $"TRANS_MW_ERC_PM_WAN_{fileName}";
                }

                try
                {

                    string CopyCommand = $@"COPY {table}
                FROM {FileLoc}
                DELIMITER ',' SKIP 1
                REJECTED DATA 'C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\UnloadedData\rejected_{fileName}.csv'
                EXCEPTIONS 'C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\UnloadedData\exceptions_{fileName}.csv' ;";
                    dBConnection.ConnectAndExecuteQuery(CopyCommand);
                    
                }
                catch (Exception e)
                {

                    Console.WriteLine("Failed to load data to the database: " + e.Message);
                }


            }
        }


    }
}
