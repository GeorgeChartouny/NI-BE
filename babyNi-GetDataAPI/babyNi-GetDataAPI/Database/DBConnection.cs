using babyNi_GetDataAPI.Models;
using Vertica.Data.VerticaClient;

namespace babyNi_GetDataAPI.Database
{
    public class DBConnection
    {
        public DBConnection()
        {
            
        }
        public List<AggDataModel> ConnectAndExecuteReader(string query)
        {

        VerticaConnectionStringBuilder builder = new VerticaConnectionStringBuilder();
        SetConnectionConfiguration(builder);

            VerticaConnection _conn = new VerticaConnection(builder.ToString());
            var result = new List<AggDataModel>();

            try
            {
                OpenConnection(_conn);


                try
                {
                    using(_conn)
                    {

                        VerticaCommand command =_conn.CreateCommand();
                        command.CommandText = query;

                        VerticaDataReader dataReader = command.ExecuteReader();
                        int rows = 0;

                        while (dataReader.Read())
                        {
                            var rowResult = new AggDataModel
                            {
                                DATETIME_KEY = dataReader.GetDateTime(dataReader.GetOrdinal("DATETIME_KEY")),
                                TIME_Stamp = dataReader.GetDateTime(dataReader.GetOrdinal("TIME_Stamp")),
                                NE_TYPE = dataReader.IsDBNull(dataReader.GetOrdinal("NE_TYPE")) ? null : dataReader.GetString(dataReader.GetOrdinal("NE_TYPE")),
                                NE_ALIAS = dataReader.IsDBNull(dataReader.GetOrdinal("NE_ALIAS")) ? null : dataReader.GetString(dataReader.GetOrdinal("NE_ALIAS")),
                                RSL_INPUT_POWER = dataReader.GetFloat(dataReader.GetOrdinal("RSL_INPUT_POWER")),
                                MAX_RX_LEVEL = dataReader.GetFloat(dataReader.GetOrdinal("MAX_RX_LEVEL")),
                                RSL_DEVIATION = dataReader.GetFloat(dataReader.GetOrdinal("RSL_DEVIATION"))
                            };
                            result.Add(rowResult);
                        }

                        dataReader.Close();
                        CloseConnection(_conn);


                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to read from the DB: " + ex.Message );

                }
            }catch (Exception ex)
            {

                Console.WriteLine("Failed to connect to the VerticaDB: " + ex.Message);
            }
            return result;
        }

        public void SetConnectionConfiguration(VerticaConnectionStringBuilder builder)
        {

            builder.Host = Environment.GetEnvironmentVariable("HOST");
            builder.Port = 5433;
            builder.Database = Environment.GetEnvironmentVariable("DATABASE");
            builder.User = Environment.GetEnvironmentVariable("USERNAME");
            builder.Password = Environment.GetEnvironmentVariable("PASSWORD");
        }

        public void OpenConnection(VerticaConnection conn)
        {
            try
            {

                conn.Open();
                Console.WriteLine("Connection Established.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot connect: " + ex.Message);
            }
        }

        public void CloseConnection(VerticaConnection conn)
        {
            try
            {
                conn.Close();
                Console.WriteLine("Connection Closed.");
            }
            catch (Exception ex)
            {

                Console.WriteLine("Cannot close the connection: " + ex.Message);
            }

        }
    }

   

    
}
