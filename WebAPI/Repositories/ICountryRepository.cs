using WebAPI.Models;

namespace WebAPI.Repositories
{
    public interface ICountryRepository
    {
        public bool AddCountryRecord(Country country);
        public bool UpdateCountryRecord(Country country, Country updateCountry);
        public ICollection<Country> GetAllCountriesRecord();
        public Country? GetCountryRecord(string countryName);
        public bool DeleteCountryRecord(Country country);
        (int TotalCount, List<(string Name, double EstimatedGdp)>, DateTime LastRefreshedAt) GetSummaryData();
    }
}
