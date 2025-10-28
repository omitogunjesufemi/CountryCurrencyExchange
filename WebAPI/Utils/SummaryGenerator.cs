using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace WebAPI.Utils
{
    public class SummaryGenerator
    {
        private const string ImagePath = "cache/summary.png";

        private static readonly FontFamily? DefaultFontFamily;

        static SummaryGenerator()
        {
            try
            {
                FontCollection fonts = new();
                DefaultFontFamily = fonts.Add("Fonts/Arial.ttf");
            }
            catch (Exception)
            {

                DefaultFontFamily = SystemFonts.TryGet("Arial", out var fallbackFont) ? fallbackFont : null;
            }
        }

        public static async Task GenerateAndSaveImageAsync(int totalCountries, List<(string Name, double EstimatedGdp)> topGdpCountries, DateTime lastRefreshedAt)
        {
            if (DefaultFontFamily == null)
                return;

            int width = 800;
            int height = 500;
            float currentY = 50;
            float padding = 20;

            Font? titleFont = DefaultFontFamily?.CreateFont(24);
            Font? bodyFont = DefaultFontFamily?.CreateFont(18);

            using (Image<Rgba32> image = new Image<Rgba32>(width, height, Color.White))
            {
                image.Mutate(x =>
                {
                    x.BackgroundColor(Color.ParseHex("#F5F5F5"));

                    string title = "Country Data Summary";
                    x.DrawText(title, titleFont, Color.Black, new PointF(padding, currentY));
                    currentY += 60;


                    string totalText = $"Total Countries: {totalCountries}";
                    x.DrawText(totalText, bodyFont, Color.DarkBlue, new PointF(padding, currentY));
                    currentY += 50;

                    string gdpTitle = "Top 5 Countries by Estimated GDP (USD)";
                    x.DrawText(gdpTitle, bodyFont, Color.Black, new PointF(padding, currentY));
                    currentY += 30;

                    for (int i = 0; i < topGdpCountries.Count; i++)
                    {
                        var country = topGdpCountries[i];
                        string gdpValue = country.EstimatedGdp.ToString("N0", CultureInfo.InvariantCulture);
                        string countryLine = $"{i + 1}. {country.Name} - ${gdpValue}";
                        x.DrawText(countryLine, bodyFont, Color.Black, new PointF(padding + 20, currentY));
                        currentY += 30;
                    }
                    currentY = height - 50;

                    string timeText = $"Last Refreshed: {lastRefreshedAt:yyyy-MM-dd HH:mm:ss UTC}";
                    x.DrawText(timeText, bodyFont, Color.Gray, new PointF(padding, currentY));
                });

                string? directory = Path.GetDirectoryName(ImagePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                await image.SaveAsPngAsync(ImagePath);
            }
        }
    }
}
