using babyNi_GetDataAPI.Database;
using babyNi_GetDataAPI.Models;

namespace babyNi_GetDataAPI.Services
{
    public class GetDataService
    {

        public List<AggDataModel> GetData(GetDataModel getDataModel)
        {
            string query;

            if(getDataModel.time_stampFrom == null || getDataModel.time_stampTo == null)
            {
                query = $@"SELECT * 
                        FROM {getDataModel.aggTime} 
                        WHERE {getDataModel.NeRequested} !='-'";
            }else
            {
                query = $@"SELECT * 
                        FROM {getDataModel.aggTime} 
                        WHERE {getDataModel.NeRequested} !='-'  AND 
                        time_stamp BETWEEN '{getDataModel.time_stampFrom}' AND '{getDataModel.time_stampTo}'
                        ";
            }
            var dbConnection = new DBConnection();
            var queryResult = dbConnection.ConnectAndExecuteReader(query);
            return queryResult;
            
        }
    }
}
