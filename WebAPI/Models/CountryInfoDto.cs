namespace WebAPI.Models
{
    public class CountryInfoDto
    {
        public string? Name { get; set; }
        public string? Capital { get; set; }
        public string? Region { get; set; }
        public long Population { get; set; }
        public ICollection<CurrencyDto> Currencies { get; set; }
        public string? Flag { get; set; }
        public bool? Independent { get; set; }
    }
}
