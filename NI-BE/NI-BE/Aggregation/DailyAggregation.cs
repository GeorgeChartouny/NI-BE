using NI_BE.DataDb;
using Serilog;

namespace NI_BE.Aggregation
{
    public class DailyAggregation
    {
        public DailyAggregation()
        {

        }

        public bool CreateAndInsertDailyTable()
        {
            DBConnection dBConnection = new DBConnection();

            try
            {
                string createCommand = @"CREATE TABLE IF NOT EXISTS TRANS_MW_AGG_SLOT_DAILY(
                DATETIME_KEY datetime,
                TIME_Stamp datetime,
                NE_TYPE VARCHAR(50),
                NE_ALIAS VARCHAR(50),
                RSL_INPUT_POWER FLOAT,
                MAX_RX_LEVEL FLOAT,
                RSL_DEVIATION FLOAT
                ) SEGMENTED BY HASH( TIME_Stamp ) ALL NODES;";

                bool createSuccess = dBConnection.ConnectAndExecuteQuery(createCommand);

                if (createSuccess)
                {
                    string aggregateCommand = @"
                    INSERT INTO TRANS_MW_AGG_SLOT_DAILY(DATETIME_KEY, TIME_Stamp, NE_TYPE,NE_ALIAS, RSL_INPUT_POWER, MAX_RX_LEVEL, RSL_DEVIATION) 
                    SELECT DATETIME_KEY, DATE_TRUNC('DAY',TIME_STAMP), 
                     NE_TYPE,
                     '-',
                    MAX(RSL_INPUT_POWER)AS RSL_INPUT_POWER,
                    MAX(MAX_RX_LEVEL) AS MAX_RS_LEVEL,
                    (ABS(MAX(RSL_INPUT_POWER)))-(ABS(MAX(MAX_RX_LEVEL))) AS RSL_DEVIATION
                    FROM TRANS_MW_AGG_SLOT_HOURLY
                    WHERE NE_TYPE IS NOT NULL
                    GROUP BY 1,2,3;

                    INSERT INTO TRANS_MW_AGG_SLOT_DAILY(DATETIME_KEY, TIME_Stamp, NE_TYPE,NE_ALIAS, RSL_INPUT_POWER, MAX_RX_LEVEL, RSL_DEVIATION) 
                    SELECT DATETIME_KEY, DATE_TRUNC('DAY',TIME_STAMP), 
                     '-',
                     NE_ALIAS,
                    MAX(RSL_INPUT_POWER)AS RSL_INPUT_POWER,
                    MAX(MAX_RX_LEVEL) AS MAX_RS_LEVEL,
                    (ABS(MAX(RSL_INPUT_POWER)))-(ABS(MAX(MAX_RX_LEVEL))) AS RSL_DEVIATION
                    FROM TRANS_MW_AGG_SLOT_HOURLY
                    WHERE NE_ALIAS IS NOT NULL
                    GROUP BY 1,2,4;
                    ";

                    bool aggregateSuccess = dBConnection.ConnectAndExecuteQuery(aggregateCommand);
                    if (aggregateSuccess)
                    {
                        Log.Information("Data inserted into daily table successfully.");

                    }
                    else
                    {
                        Log.Information("Could not insert data into daily table. Check that all appropriate tables have data.");

                    }
                }
                else
                {
                    Log.Information("Creating table daily was not succeeded.");

                }

            }
            catch (Exception ex)
            {
                Log.Information("Could not create the daily table: " + ex.Message);
                return false;
            }
            return true;
        }
    }
}
