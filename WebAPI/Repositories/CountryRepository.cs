using WebAPI.Models;

namespace WebAPI.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly CountryDBContext _dbContext;

        public CountryRepository(CountryDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public bool AddCountryRecord(Country country)
        {
            try
            {
                _dbContext.Countries.Add(country);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateCountryRecord(Country country, Country updateCountry)
        {
            try
            {
                country.Name = updateCountry.Name;
                country.Capital = updateCountry.Capital;
                country.Region = updateCountry.Region;
                country.Population = updateCountry.Population;
                country.CurrencyCode = updateCountry.CurrencyCode;
                country.ExchangeRate = updateCountry.ExchangeRate;
                country.EstimatedGDP = updateCountry.EstimatedGDP;
                country.FlagURL = updateCountry.FlagURL;
                country.UpdatedAt = DateTime.Now;
                _dbContext.Countries.Update(country);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteCountryRecord(Country country)
        {
            try
            {
                _dbContext.Countries.Remove(country);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ICollection<Country> GetAllCountriesRecord()
        {
            var countriesLlist = _dbContext.Countries.ToList();
            return countriesLlist;
        }

        public Country? GetCountryRecord(string countryName)
        {
            Country? retrivedCountry = _dbContext.Countries.FirstOrDefault(c => c.Name.ToLower() == countryName.ToLower().Trim());
            return retrivedCountry;
        }
    }
}
