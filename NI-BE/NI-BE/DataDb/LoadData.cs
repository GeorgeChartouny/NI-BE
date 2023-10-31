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
                    table = "TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER";
                }
                else if (fileName.Contains("TN_RFInputPower"))
                {
                    table = "TRANS_MW_ERC_PM_WAN_RFINPUTPOWER";
                }

                try
                {

                    string CopyCommand = $@"COPY {table}
                FROM LOCAL '{FileLoc}'
                DELIMITER ',' SKIP 1
                REJECTED DATA 'C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\UnloadedData\rejected_{fileName}'
                EXCEPTIONS 'C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\UnloadedData\exceptions_{fileName}' ;";
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
