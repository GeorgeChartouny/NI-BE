using System.Data;
using System.Reflection.PortableExecutable;
using NI_BE.DataDb.Models;
using Serilog;
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

            SetConnectionConfiguration(builder);

            VerticaConnection _conn = new VerticaConnection(builder.ToString());
            try
            {
                OpenConnection(_conn);

                try
                {
                    using (_conn)
                    {


                        //Create command
                        VerticaCommand command = _conn.CreateCommand();

                        // Command for tables to create if they do not exist in the database
                        string CreateRadioTable = @"CREATE TABLE IF NOT EXISTS TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER (
                            NETWORK_SID INT NOT NULL,
                            DATETIME_KEY DATETIME NOT NULL,
                            NEID FLOAT NOT NULL,
                            OBJECT VARCHAR(255) NOT NULL,
                            TIME DATETIME NOT NULL,
                            ""INTERVAL"" INT NOT NULL,
                            DIRECTION VARCHAR(255) NOT NULL,
                            NEALIAS VARCHAR(255) NOT NULL,
                            NETYPE VARCHAR(255) NOT NULL,
                            RXLEVELBELOWTS1 FLOAT NOT NULL,
                            RXLEVELBELOWTS2 FLOAT NOT NULL,
                            MINRXLEVEL FLOAT NOT NULL,
                            MAXRXLEVEL FLOAT NOT NULL,
                            TXLEVELABOVETS1 FLOAT NOT NULL,
                            MINTXLEVEL FLOAT NOT NULL,
                            MAXTXLEVEL FLOAT NOT NULL,
                            FAILUREDESCRIPTION VARCHAR(255) NOT NULL,
                            LINK VARCHAR(255) NOT NULL,
                            TID VARCHAR(255) NOT NULL,
                            FARENDTID VARCHAR(255) NOT NULL,
                            SLOT VARCHAR(255) NOT NULL,
                            PORT INT NOT NULL
                            )SEGMENTED BY HASH(NETWORK_SID,DATETIME_KEY) ALL NODES;";

                        string CreateRFInputTable = @"CREATE TABLE IF NOT EXISTS TRANS_MW_ERC_PM_WAN_RFINPUTPOWER (
                            NETWORK_SID INT NOT NULL,
                            DATETIME_KEY DATETIME NOT NULL,
                            NODENAME VARCHAR(255) NOT NULL,
                            NEID FLOAT NOT NULL,
                            OBJECT VARCHAR(255) NOT NULL,
                            TIME DATETIME NOT NULL,
                            ""INTERVAL"" INT NOT NULL,
                            DIRECTION VARCHAR(255) NOT NULL,
                            NEALIAS VARCHAR(255) NOT NULL,
                            NETYPE VARCHAR(255) NOT NULL,
                            RFINPUTPOWER FLOAT NOT NULL,
                            TID VARCHAR(255) NOT NULL,
                            FARENDTID VARCHAR(255) NOT NULL,
                            SLOT VARCHAR(255) NOT NULL,
                            PORT INT NOT NULL
                            ) SEGMENTED BY HASH(NETWORK_SID, DATETIME_KEY) ALL NODES;";


                        AddQuery(command, CreateRadioTable);
                        AddQuery(command, CreateRFInputTable);

                        //Associate the command with the connection
                        command.Connection = _conn;

                        //Create the DataAdapter
                        VerticaDataAdapter adapter = new VerticaDataAdapter();
                        adapter.SelectCommand = command;

                        //Fill the table
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        // Display each row and column value
                        //int i = 1;
                        //foreach (DataRow row in table.Rows)
                        //{
                        //    foreach (DataColumn column in table.Columns)
                        //    {
                        //        Console.WriteLine(row[column] + "\t");
                        //    }
                        //    Console.WriteLine();
                        //    i++;
                        //}
                        //Console.WriteLine(i + "rows returned.");
                    }
                    CloseConnection(_conn);

                }
                catch (Exception e)
                {

                   // Console.WriteLine("Failed to execute query: " + e.Message);
                    Log.Information("Failed to execute query: " + e.Message);
                }

            }
            catch (Exception ex)
            {

               // Console.WriteLine("Failed to connect to the database: " + ex.Message);
                Log.Information("Failed to connect to the database: " + ex.Message);
            }

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
                //Console.WriteLine("Connection Established.");
                Log.Information("Connection Established.");
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Cannot connect: " + ex.Message);
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

                Console.WriteLine("Cannot close the connection: " + ex.Message);
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
                    Console.WriteLine("Query Executed Successfully.");
                    CloseConnection(_conn);
                    return true;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to execute query: " + ex.Message);
                    return false;

                }
            }
            catch (Exception e)
            {

                Console.WriteLine("Failed to connect to the database: " + e.Message);
                return false;

            }
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
                    using (_conn)
                    {
                        VerticaCommand command = _conn.CreateCommand();
                        command.CommandText = query;

                        VerticaDataReader dataReader = command.ExecuteReader();
                        int rows = 0;
                        while (dataReader.Read())
                        {
                            //result.Add(string.Join(",", dataReader[rows]));
                            //rows++;

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

                    Console.WriteLine("Failed to read from the database: " + ex.Message);

                }
            }
            catch (Exception e)
            {


                Console.WriteLine("Failed to connect to the database: " + e.Message);

            }
            return result;
        }
    }
}
