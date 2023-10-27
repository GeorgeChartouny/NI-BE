namespace NI_BE.DataDb.Models
{
    public class SOEM1_TN
    {

        public required int NETWORK_SID { get; set; }
        public required DateTime DATETIME_KEY { get; set; }

        //FIELD FOR INPUT_POWER ONLY
        public string? NODENAME { get; set; }

        public required float NEID { get; set; }
        public required string OBJECT { get; set; }
        public required DateTime TIME { get; set; }
        public required int INTERVAL { get; set; }
        public required string  DIRECTION { get; set; }
        public required string NEALIAS { get; set; }
        public required string NETYPE { get; set; }


        //FIELD FOR RADIO_LINK_POWER ONLY
        public float RXLEVELBELOWTS1 { get; set; }
        public float RXLEVELBELOWTS2 { get; set; }
        public float MINRXLEVEL { get; set; }
        public float MAXRXLEVEL { get; set; }
        public float TXLEVELABOVETS1 { get; set; }
        public float MINTXLEVEL { get; set; }
        public float MAXTXLEVEL { get; set; }


        //FIELD FOR INPUT_POWER ONLY
        public float RFINPUTPOWER { get; set; }


        // FIELD FOR RADIO_LINK_POWER ONLY
        public string? FAILUREDESCRIPTION { get; set; }
        public string? LINK { get; set; }

        public required string TID { get; set; }
        public required string FARENDTID { get; set; }
        public required string SLOT { get; set; }
        public required int PORT { get; set; }


    }
}
