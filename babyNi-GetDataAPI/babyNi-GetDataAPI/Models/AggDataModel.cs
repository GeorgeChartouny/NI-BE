namespace babyNi_GetDataAPI.Models
{
    public class AggDataModel
    {
        public DateTime DATETIME_KEY { get; set; }
        public DateTime TIME_Stamp { get; set; }
        public string NE_TYPE { get; set; }
        public string NE_ALIAS { get; set; }
        public float RSL_INPUT_POWER { get; set; }
        public float MAX_RX_LEVEL { get; set; }
        public float RSL_DEVIATION { get; set; }
    }
}
