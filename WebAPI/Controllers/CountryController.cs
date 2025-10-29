using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("countries")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly CountryService _countryService;
        private readonly HttpClientService _httpClientService;

        public CountryController(CountryService countryService, HttpClientService httpClientService)
        {
            _countryService = countryService;
            _httpClientService = httpClientService;
        }

        [Route("refresh")]
        [HttpPost]
        // POST /countries/refresh
        public async Task<IActionResult> AddCountryCurrencyExchangeToDB()
        {
            ICollection<CountryInfoDto> countryDetails;
            ExchangeDto exchangeRateDetails;

            try
            {
                var countryDetailsResponse = await _httpClientService.GetCountryAsync();
                countryDetails = JsonConvert.DeserializeObject<ICollection<CountryInfoDto>>(countryDetailsResponse);
            }
            catch
            {
                var errorMsg = new
                {
                    error = "External data source unavailable",
                    details = "Could not fetch data from Countries API"
                };
                return StatusCode(StatusCodes.Status503ServiceUnavailable, errorMsg);
            }

            try
            {
                var exchangeRateResponse = await _httpClientService.GetExchangeRatesAsync();
                exchangeRateDetails = JsonConvert.DeserializeObject<ExchangeDto>(exchangeRateResponse);
            }
            catch
            {
                var errorMsg = new 
                {
                    error = "External data source unavailable",
                    details = "Could not fetch data from Exchange rates API"
                };
                //return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Internal server error" });
                return StatusCode(StatusCodes.Status503ServiceUnavailable, errorMsg);
            }

            await _countryService.AddCountryRecordToDB(countryDetails, exchangeRateDetails);
            return Created("/countries/image", null);
        }

        [Route("image")]
        [HttpGet]
        public IActionResult GetSummaryImage()
        {
            const string imagePath = "cache/summary.png";

            if (!System.IO.File.Exists(imagePath))
            {
                return StatusCode(StatusCodes.Status404NotFound, new { error = "Summary image not found" });
            }

            try
            {
                var imageStream = System.IO.File.OpenRead(imagePath);
                return File(imageStream, "image/png");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Could not serve summary image due to I/O error" });
            }
        }

        [Route("")]
        [HttpGet]
        public IActionResult GetAllCountries([FromQuery] string? region, [FromQuery] string? currency, [FromQuery] string? sort)
        {
            var countries = _countryService.GetAllCountriesRecord();
            var filteredCountries = countries.AsQueryable();

            if (!string.IsNullOrEmpty(region))
            {
                filteredCountries = filteredCountries.Where(c => c.Region.ToLower().Trim() == region.ToLower().Trim());
            }

            if (!string.IsNullOrEmpty(currency))
            {
                filteredCountries = filteredCountries.Where(c => c.CurrencyCode.ToLower().Trim() == currency.ToLower().Trim());
            }

            if (!string.IsNullOrEmpty(sort))
            {
                string[] sortString = sort.Split('_');
                if (sortString.Length == 2)
                {
                    if (sortString[0] == "gdp")
                        sortString[0] = "EstimatedGDP";

                    filteredCountries = filteredCountries.OrderBy($"{sortString[0]} {sortString[1]}");
                }
                    
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

        [Route("{name}")]
        [HttpGet]
        public IActionResult GetCountry([FromRoute]string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return StatusCode(StatusCodes.Status400BadRequest, new { error = "Validation failed" });

                var country = _countryService.GetCountryRecord(name);
                if (country == null)
                {
                    var errorMessage = new { error = "Country not found" };
                    return StatusCode(StatusCodes.Status404NotFound, errorMessage);
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
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Internal server error" });
            }
        }

        [Route("{name}")]
        [HttpDelete]
        public IActionResult DeleteCountryRecord([FromRoute]string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return StatusCode(StatusCodes.Status400BadRequest, new { error = "Validation failed" });

                var country = _countryService.GetCountryRecord(name);
                if (country == null)
                {
                    var errorMessage = new { error = "Country not found" };
                    return StatusCode(StatusCodes.Status404NotFound, errorMessage);
                }

                _countryService.DeleteCountryRecord(country);
                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Internal server error" });
            }
        }
    }
}
