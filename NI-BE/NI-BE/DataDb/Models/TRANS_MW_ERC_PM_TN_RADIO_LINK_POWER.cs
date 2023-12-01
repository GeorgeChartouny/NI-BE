namespace NI_BE.DataDb.Models
{
    public class TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER

    {

        public int NETWORK_SID { get; set; }
        public DateTime DATETIME_KEY { get; set; }
        public  float NEID { get; set; }
        public  string OBJECT { get; set; }
        public  DateTime TIME { get; set; }
        public  int INTERVAL { get; set; }
        public  string  DIRECTION { get; set; }
        public  string NEALIAS { get; set; }
        public  string NETYPE { get; set; }
        public float RXLEVELBELOWTS1 { get; set; }
        public float RXLEVELBELOWTS2 { get; set; }
        public float MINRXLEVEL { get; set; }
        public float MAXRXLEVEL { get; set; }
        public float TXLEVELABOVETS1 { get; set; }
        public float MINTXLEVEL { get; set; }
        public float MAXTXLEVEL { get; set; }
        public string FAILUREDESCRIPTION { get; set; }
        public string LINK { get; set; }
        public string TID { get; set; }
        public string FARENDTID { get; set; }
        public string SLOT { get; set; }
        public int PORT { get; set; }


    }
}
