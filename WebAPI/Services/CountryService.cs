using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services
{
    public class CountryService
    {
        private readonly HttpClientService _httpClientService;
        private readonly ICountryRepository repository;
        public CountryService(HttpClientService httpClientService, ICountryRepository countryRepository)
        {
            _httpClientService = httpClientService;
            repository = countryRepository;
        }

        public async Task AddCountryRecordToDB()
        {
            var countryDetailsResponse = await _httpClientService.GetCountryAsync();
            var countryDetails = JsonConvert.DeserializeObject<ICollection<CountryInfoDto>>(countryDetailsResponse);


            foreach (var country in countryDetails)
            {                
                long population = country.Population;
                string? currencyCode = null;
                decimal? exchangeRate = null;
                double? estimatedGDP = 0;

                var currencyCodeValue = country.Currencies?.FirstOrDefault();
                if (currencyCodeValue != null)
                {
                    currencyCode = currencyCodeValue.Code;
                    var exchangeRateResponse = await _httpClientService.GetExchangeRatesAsync();
                    var exchangeRateDetails = JsonConvert.DeserializeObject<ExchangeDto>(exchangeRateResponse);

                    if (exchangeRateDetails.Rates != null && exchangeRateDetails.Rates.ContainsKey(currencyCode))
                    {
                        exchangeRate = exchangeRateDetails.Rates[currencyCode];
                        Random random = new Random();
                        estimatedGDP = (double)random.Next(1000, 2001) / (double)exchangeRate;
                    }
                    else
                    {
                        estimatedGDP = null;
                    }
                }

                var existingCountry = repository.GetCountryRecord(country.Name ?? string.Empty);

                var newCountryRecord = new Country
                {
                    Name = country.Name,
                    Capital = country.Capital,
                    Region = country.Region,
                    Population = population,
                    CurrencyCode = currencyCode,
                    ExchangeRate = exchangeRate,
                    EstimatedGDP = estimatedGDP,
                    FlagURL = country.Flag,
                    UpdatedAt = DateTime.Now
                };

                if (existingCountry == null)
                {
                    repository.AddCountryRecord(newCountryRecord);
                }
                else
                {
                    repository.UpdateCountryRecord(existingCountry, newCountryRecord);
                }                
            }
        }

        public ICollection<Country> GetAllCountriesRecord()
        {
            try
            {
                return repository.GetAllCountriesRecord();
            }
            catch (Exception ex)
            {
                return Array.Empty<Country>();

            }
        }

        public Country? GetCountryRecord(string countryName)
        {
            try
            {
                return repository.GetCountryRecord(countryName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool DeleteCountryRecord(Country country)
        {
            try
            {
                return repository.DeleteCountryRecord(country);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public CRStatus? GetCRStatus()
        {
            try
            {
                var countries = repository.GetAllCountriesRecord();
                var crStatus = new CRStatus();

                if (countries.Count > 0)
                {
                     crStatus.TotalCountries = countries.Count;
                     crStatus.LastUpdated = countries.Max(c => c.UpdatedAt);

                    return crStatus;
                }

                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
