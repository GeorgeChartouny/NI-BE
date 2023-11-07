namespace NI_BE.DataDb.Models
{
    public class AggDataModel
    {
        public DateTime DATETIME_KEY { get; set; }
        public DateTime TIME_Stamp { get; set; }
        public string NETYPE { get; set; }
        public string NEALIAS { get; set; }
        public float RSL_INPUT_POWER { get; set; }
        public float MAX_RX_LEVEL { get; set; }
        public float RSL_DEVIATION { get; set; }
    }
}
