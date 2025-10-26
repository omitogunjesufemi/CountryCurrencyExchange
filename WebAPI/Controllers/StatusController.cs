using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("status")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly CountryService _countryService;
        public StatusController(CountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        public IActionResult GetStatus()
        {
            var statusReport = _countryService.GetCRStatus();
            var result = new
            {
                total_countries = statusReport?.TotalCountries ?? 0,
                last_refreshed_at = statusReport?.LastUpdated.ToString("o") ?? "N/A"
            };
            return Ok(result);
        }
    }
}
