namespace WebAPI.Models
{
    public class SummaryDto
    {
        public int TotalCountries { get; set; }
        public List<string> TopFiveCountriesByGDP { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
