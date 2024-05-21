namespace MCB.DataAccess
{
    public class CorruptionData
    {
        public string CountryName { get; set; }
        public string Iso3 { get; set; }
        public string Region { get; set; }
        public int Year { get; set; }
        public double CpiScore { get; set; }
        public int Rank { get; set; }
        public int Sources { get; set; }
        public double StandardError { get; set; }
    }
}
