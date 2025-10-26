using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Capital { get; set; }
        public string? Region { get; set; }
        public long Population { get; set; }
        public string? CurrencyCode { get; set; }
        public decimal? ExchangeRate { get; set; }
        public double? EstimatedGDP { get; set; }
        public string? FlagURL { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
