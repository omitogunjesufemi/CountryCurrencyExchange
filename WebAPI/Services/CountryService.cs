using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using WebAPI.Models;
using WebAPI.Repositories;
using WebAPI.Utils;

namespace WebAPI.Services
{
    public class CountryService
    {
        
        private readonly ICountryRepository repository;
        public CountryService(ICountryRepository countryRepository)
        {
            repository = countryRepository;
        }

        public async Task AddCountryRecordToDB(ICollection<CountryInfoDto> countryDetails, ExchangeDto exchangeRateDetails)
        {

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
                    if (exchangeRateDetails.Rates != null && exchangeRateDetails.Rates.ContainsKey(currencyCode))
                    {
                        exchangeRate = Math.Round(exchangeRateDetails.Rates[currencyCode], 2);
                        Random random = new Random();
                        double randNum = Math.Round((double) random.Next(1000, 2001), 2);
                        estimatedGDP = Math.Round(population * (randNum / (double)exchangeRate), 1);
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

                var (totalCount, top5Data, lastUpdated) = repository.GetSummaryData();
                await SummaryGenerator.GenerateAndSaveImageAsync(totalCount, top5Data, lastUpdated);
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

        //public Country? DBRecordSummary()
        //{
        //    try
        //    {
        //        var countries = repository.GetAllCountriesRecord();
        //        int totalCountries = countries.Count;
        //        if (totalCountries > 0)
        //        {
        //            countries = countries.OrderBy(c => c.EstimatedGDP).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

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
