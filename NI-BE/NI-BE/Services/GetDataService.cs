using NI_BE.DataDb;
using NI_BE.DataDb.Models;

namespace NI_BE.Services
{
    public class GetDataService
    {

        public List<AggDataModel> GetData(GetDataModel getDataModel)
        {

            string query = $@"SELECT * 
                        FROM {getDataModel.aggTime} 
                        WHERE {getDataModel.NeRequested} !='-' AND DATETIME_KEY = '{getDataModel.datetime_key}';
";

            var dbConnection = new DBConnection();
           var queryResult =  dbConnection.ConnectAndExecuteReader(query);

            return queryResult;
        }
       
    }
}
