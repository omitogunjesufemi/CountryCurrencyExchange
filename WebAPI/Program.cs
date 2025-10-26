
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPI.Repositories;
using WebAPI.Services;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<CountryDBContext>(
                options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


            builder.Services.AddScoped<HttpClientService>();

            builder.Services.AddHttpClient("CountryDetails", cd =>
            {
                cd.BaseAddress = new Uri("https://restcountries.com/v2/all?fields=name,capital,region,population,flag,currencies");
            });

            builder.Services.AddHttpClient("ExchangeRates", er =>
            {
                er.BaseAddress = new Uri("https://open.er-api.com/v6/latest/USD");
            });

            builder.Services.AddScoped<ICountryRepository, CountryRepository>();
            builder.Services.AddScoped<CountryService>();


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
