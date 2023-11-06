

using NI_BE.DataDb;

namespace NI_BE.Aggregation
{
    public class Hourly
    {
        public string _fileLocation { get; set; }
        public string _fileName { get; set; }
        public Hourly()
        {

        }


        public bool CreateHourlyTable()
        {
            DBConnection dBConnection = new DBConnection();

            try
            {
                string createTableCommand = @"CREATE TABLE IF NOT EXISTS TRANS_MW_AGG_SLOT_HOURLY(
                    DATETIME_KEY datetime,
                    TIME_Stamp datetime,
                    NE VARCHAR(20),
                    RSL_INPUT_POWER FLOAT,
                    MAX_RX_LEVEL FLOAT,
                    RSL_DEVIATION FLOAT
                    ) SEGMENTED BY HASH( TIME_Stamp ) ALL NODES;";

                bool success = dBConnection.ConnectAndExecuteQuery(createTableCommand);
                if (success)
                {
                    Console.WriteLine("Hourly table created.");
                    string aggregateCommand = @"
                    INSERT INTO TRANS_MW_AGG_SLOT_HOURLY(DATETIME_KEY, TIME_Stamp, NE_TYPE,NE_ALIAS, RSL_INPUT_POWER, MAX_RX_LEVEL, RSL_DEVIATION) 
                    SELECT  RADIO.DATETIME_KEY,date_trunc('hour',RADIO.TIME), CONCAT('NE_TYPE ',RADIO.NETYPE) ,NULL,
                    MAX(RADIO.maxrxlevel)AS MAX_RX_LEVEL,
                    MAX(RFINPUT.RFInputPower)AS RSL_INPUT_POWER,
                    (ABS(MAX(RFINPUT.RFInputPower)))-(ABS(MAX(RADIO.maxrxlevel))) AS RSL_DEVIATION
                    FROM trans_mw_erc_pm_tn_radio_link_power AS RADIO
                    INNER JOIN TRANS_MW_ERC_PM_WAN_RFINPUTPOWER AS RFINPUT ON RFINPUT.NETYPE = RADIO.NETYPE
                    GROUP BY 1,2,3;

                    INSERT INTO TRANS_MW_AGG_SLOT_HOURLY(DATETIME_KEY, TIME_Stamp, NE_TYPE,NE_ALIAS, RSL_INPUT_POWER, MAX_RX_LEVEL, RSL_DEVIATION) 
                    SELECT  RADIO.DATETIME_KEY,date_trunc('hour',RADIO.TIME),NULL,CONCAT('NE_ALIAS ',RADIO.NEALIAS),
                    MAX(RADIO.maxrxlevel)AS MAX_RX_LEVEL,
                    MAX(RFINPUT.RFInputPower)AS RSL_INPUT_POWER,
                    (ABS(MAX(RFINPUT.RFInputPower)))-(ABS(MAX(RADIO.maxrxlevel))) AS RSL_DEVIATION
                    FROM trans_mw_erc_pm_tn_radio_link_power AS RADIO
                    INNER JOIN TRANS_MW_ERC_PM_WAN_RFINPUTPOWER AS RFINPUT ON RFINPUT.NETYPE = RADIO.NETYPE
                    GROUP BY 1,2,4;
                        ";
                    bool success2 = dBConnection.ConnectAndExecuteQuery(aggregateCommand);
                    if (success2)
                    {
                        Console.WriteLine("Data inserted into hourly table successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Could not insert data into hourly table.");
                    }

                }
                else
                {
                    Console.WriteLine("Creating table Hourly was not succeeded.");
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine("Error creating hourly Table: " + ex.Message);
                return false;
            }
            return true;

        }

    }
}
