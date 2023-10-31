namespace NI_BE.DataDb
{
    public class LoadData
    {
        public string _fileLocation { get; set; }
        public string _fileName { get; set; }
        public LoadData()
        {
            
        }
        public LoadData(string FileLocation,string FileName)
        {
            _fileLocation = FileLocation;   
            _fileName = FileName;
        }

        public void ExecuteLoader(string FileLoc,string fileName)
        {
            DBConnection dBConnection = new DBConnection();
            if(FileLoc.Contains(".csv"))
            {
                try
                {
                    string CopyCommand = $@"COPY TRANS_MW_ERC_PM_TN_{fileName}
                FROM {FileLoc}
                DELIMITER ',' SKIP 1
                REJECTED DATA 'C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\UnloadedData\rejected_{fileName}.csv'
                EXCEPTIONS ''C:\Users\User\Desktop\G\Baby NI Project\Code\NI-BE\NI-BE\NI-BE\Data\UnloadedData\exceptions_{fileName}.csv'
                ";
                    dBConnection.ConnectAndExecuteQuery(CopyCommand);
                }
                catch (Exception e)
                {

                    Console.WriteLine("Failed to load data to the database: " + e.Message );
                }


            }
        }


    }
}
