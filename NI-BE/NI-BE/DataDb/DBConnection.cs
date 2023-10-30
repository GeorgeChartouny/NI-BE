using System.Data;
using Vertica.Data.VerticaClient;

namespace NI_BE.DataDb
{
    public class DBConnection
    {
        public DBConnection()
        {

        }
        public void EstablishConnection()
        {
            VerticaConnectionStringBuilder builder = new VerticaConnectionStringBuilder();
            builder.Host = "10.10.4.231";
            builder.Port = 5433;
            builder.Database = "test";
            builder.User = "bootcamp5";
            builder.Password = "bootcamp52023";

            VerticaConnection _conn = new VerticaConnection(builder.ToString());
            try
            {
            _conn.Open();

            }
            catch (Exception ex)
            {

                Console.WriteLine("mafi connection: " + ex.Message);
            }

            try
            {
                using (_conn)
                {
                    //Create command
                    VerticaCommand command = _conn.CreateCommand();
                    command.CommandText = "select * from fact_TransactionsWeekly;";

                    //Associate the command with the connection
                    command.Connection = _conn;

                    //Create the DataAdapter
                    VerticaDataAdapter adapter = new VerticaDataAdapter();
                    adapter.SelectCommand = command;

                    //Fill the table
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    // Display each row and column value
                    int i = 1;
                    foreach (DataRow row in table.Rows)
                    {
                        foreach (DataColumn column in table.Columns)
                        {
                            Console.WriteLine(row[column] + "\t");
                        }
                        Console.WriteLine();
                        i++;
                    }
                    Console.WriteLine(i + "rows returned.");
                }
                _conn.Close();

            }
            catch (Exception e)
            {

                Console.WriteLine("Error esablishing connection: " + e.Message);
            }

        }

    }
}
