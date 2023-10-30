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
            //builder.Host = "10.10.4.231";
            builder.Host = Environment.GetEnvironmentVariable("HOST");
            builder.Port = 5433;
            builder.Database = Environment.GetEnvironmentVariable("DATABASE");
            builder.User = Environment.GetEnvironmentVariable("USERNAME");
            builder.Password = Environment.GetEnvironmentVariable("PASSWORD");

            VerticaConnection _conn = new VerticaConnection(builder.ToString());
            try
            {
                _conn.Open();
                Console.WriteLine("Connection Established.");

                try
                {
                    using (_conn)
                    {


                        //Create command
                        VerticaCommand command = _conn.CreateCommand();
                        command.CommandText = "select * from fact_TransactionsWeekly;";
                        command.CommandText += "create table if not exists TESTDOTNET(id INT PRIMARY KEY,Gender VARCHAR(255))SEGMENTED BY HASH(ID) ALL NODES";
                       // Command for tables to create if they do not exist in the database
                        command.CommandText += "CREATE TABLE IF NOT EXISTS TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER (" +
                            "NETWORK_SID INT NOT NULL," +
                            "DATETIME_KEY TIMESTAMPTZ NOT NULL," +
                            "NEID FLOAT NOT NULL," +
                            "OBJECT TEXT NOT NULL," +
                            "TIME TIMESTAMPTZ NOT NULL," +
                            "INTERVAL INT NOT NULL," +
                            "DIRECTION TEXT NOT NULL," +
                            "NEALIAS TEXT NOT NULL," +
                            "NETYPE TEXT NOT NULL," +
                            "RXLEVELBELOWTS1 FLOAT NOT NULL," +
                            "RXLEVELBELOWTS2 FLOAT NOT NULL," +
                            "MINRXLEVEL FLOAT NOT NULL," +
                            "MAXRXLEVEL FLOAT NOT NULL," +
                            "TXLEVELABOVETS1 FLOAT NOT NULL," +
                            "MINTXLEVEL FLOAT NOT NULL," +
                            "MAXTXLEVEL FLOAT NOT NULL," +
                            "FAILUREDESCRIPTION TEXT NOT NULL," +
                            "LINK TEXT NOT NULL," +
                            "TID TEXT NOT NULL," +
                            "FARENDTID TEXT NOT NULL," +
                            "SLOT TEXT NOT NULL," +
                            "PORT INT NOT NULL," +
                            ")SEGMENTED BY HASH(NETWORK_SID,DATETIME_KEY) ALL NODES;";

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
            catch (Exception ex)
            {

                Console.WriteLine("Failed to connect to the database: " + ex.Message);
            }

        }

    }
}
