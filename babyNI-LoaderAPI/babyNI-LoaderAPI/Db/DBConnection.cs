using System.Data;
using Serilog;
using Vertica.Data.VerticaClient;

namespace babyNI_LoaderAPI.Db
{
    public class DBConnection
    {
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
                Log.Information("Connection Established.");
            }
            catch (Exception ex)
            {
                Log.Information("Cannot connect: " + ex.Message);
            }
        }

        public void CloseConnection(VerticaConnection conn)
        {
            try
            {
                conn.Close();
            }
            catch (Exception ex)
            {

                Log.Information("Cannot close the connection: " + ex.Message);
            }

        }

        public void AddQuery(VerticaCommand command, string query)
        {
            command.CommandText += query;
        }

        public bool ConnectAndExecuteQuery(string query)
        {
            VerticaConnectionStringBuilder builder = new VerticaConnectionStringBuilder();
            SetConnectionConfiguration(builder);
            VerticaConnection _conn = new VerticaConnection(builder.ToString());
            try
            {
                OpenConnection(_conn);

                try
                {
                    using (_conn)
                    {
                        VerticaCommand command = _conn.CreateCommand();
                        AddQuery(command, query);
                        command.Connection = _conn;

                        VerticaDataAdapter adapter = new VerticaDataAdapter();
                        adapter.SelectCommand = command;

                        DataTable table = new DataTable();
                        adapter.Fill(table);


                    }
                    Log.Information("Query Executed Successfully.");
                    CloseConnection(_conn);
                    return true;

                }
                catch (Exception ex)
                {
                    Log.Information("Failed to execute query: " + ex.Message);
                    return false;

                }
            }
            catch (Exception e)
            {

                Log.Information("Failed to connect to the database: " + e.Message);
                return false;

            }
        }
    }
}
