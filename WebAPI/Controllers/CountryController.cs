using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;
using System.Linq.Dynamic.Core;

namespace WebAPI.Controllers
{
    [Route("countries")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly CountryService _countryService;

        public CountryController(CountryService countryService)
        {
            _countryService = countryService;
        }

        [Route("/refresh")]
        [HttpPost]
        // POST /countries/refresh
        public async Task<IActionResult> AddCountryCurrencyExchangeToDB()
        {
            await _countryService.AddCountryRecordToDB();
            return Ok();
        }

        [Route("")]
        [HttpGet]
        public IActionResult GetAllCountries([FromQuery] string? region, [FromQuery] string? currency, [FromQuery] string? sort)
        {
            var countries = _countryService.GetAllCountriesRecord();
            var filteredCountries = countries.AsQueryable();

            if (!string.IsNullOrEmpty(region))
            {
                filteredCountries = filteredCountries.Where(c => c.Region == region);
            }

            if (!string.IsNullOrEmpty(currency))
            {
                filteredCountries = filteredCountries.Where(c => c.CurrencyCode == currency);
            }

            if (string.IsNullOrEmpty(sort))
            {
                string[] sortString = sort.Split('_');
                if (sortString.Length == 2)
                    filteredCountries = filteredCountries.OrderBy($"{sortString[0]} {sortString[1]}");
            }

            var allFilteredCountries = filteredCountries.Select(c => new
            {
                id = c.Id,
                name = c.Name,
                capital = c.Capital,
                region = c.Region,
                population = c.Population,
                currency_code = c.CurrencyCode,
                exchange_rate = c.ExchangeRate,
                estimated_gdp = c.EstimatedGDP,
                flag_url = c.FlagURL,
                last_refreshed_at = c.UpdatedAt.ToString("o")
            });

            return Ok(allFilteredCountries.ToList());
        }

        [Route("{countryName}")]
        [HttpGet]
        public IActionResult GetCountry([FromRoute]string countryName)
        {
            var country = _countryService.GetCountryRecord(countryName);
            if (country == null)
            {
                return NotFound();
            }

            var result = new
            {
                id = country.Id,
                name = country.Name,
                capital = country.Capital,
                region = country.Region,
                population = country.Population,
                currency_code = country.CurrencyCode,
                exchange_rate = country.ExchangeRate,
                estimated_gdp = country.EstimatedGDP,
                flag_url = country.FlagURL,
                last_refreshed_at = country.UpdatedAt.ToString("o")
            };

            return Ok(result);
        }
    }
}
