using NI_BE.DataDb;
using NI_BE.DataDb.Models;

namespace NI_BE.Services
{
    public class GetDataService
    {

        public List<AggDataModel> GetData(GetDataModel getDataModel)
        {
            string query;

            if (getDataModel.time_stamp == null) {
                query = $@"SELECT * 
                        FROM {getDataModel.aggTime} 
                        WHERE {getDataModel.NeRequested} !='-'";
            
            
            }else
            {

             query = $@"SELECT * 
                        FROM {getDataModel.aggTime} 
                        WHERE {getDataModel.NeRequested} !='-'  AND 
time_stamp = '{getDataModel.time_stamp}'

";
            }

            var dbConnection = new DBConnection();
           var queryResult =  dbConnection.ConnectAndExecuteReader(query);

            return queryResult;
        }
       
    }
}
