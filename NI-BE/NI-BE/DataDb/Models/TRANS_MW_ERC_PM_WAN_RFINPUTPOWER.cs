namespace NI_BE.DataDb.Models
{
    public class TRANS_MW_ERC_PM_WAN_RFINPUTPOWER
    {

        public  int NETWORK_SID { get; set; }
        public  DateTime DATETIME_KEY { get; set; }
        public string NODENAME { get; set; }
        public float NEID { get; set; }
        public string OBJECT { get; set; }
        public DateTime TIME { get; set; }
        public int INTERVAL { get; set; }
        public string DIRECTION { get; set; }
        public string NEALIAS { get; set; }
        public string NETYPE { get; set; }
        public float RFINPUTPOWER { get; set; }
        public  string TID { get; set; }
        public  string FARENDTID { get; set; }
        public  string SLOT { get; set; }
        public  int PORT { get; set; }

    }
}
