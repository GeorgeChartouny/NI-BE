using NI_BE.DataDb;
using NI_BE.DataDb.Models;

namespace NI_BE.Services
{
    public class GetDataService
    {

        public List<string> GetData(string NeRequested, DateTime datetime_key, string aggTime)
        {


            string query = $@"SELECT * 
                        FROM {aggTime} 
                        WHERE {NeRequested} IS NOT NULL AND DATETIM_KEY = '{datetime_key}';
";

            var dbConnection = new DBConnection();
           var queryResult =  dbConnection.ConnectAndExecuteReader(query);

            return queryResult;
        }
       
    }
}
