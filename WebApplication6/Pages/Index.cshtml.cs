using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IDistributedCache _cache;

    public string HyderabadTemperature { get; set; }
    public string HyderabadWeather { get; set; }
    public string LondonTemperature { get; set; }
    public string LondonWeather { get; set; }

    public IndexModel(IHttpClientFactory clientFactory, IDistributedCache cache)
    {
        _clientFactory = clientFactory;
        _cache = cache;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        
        var cachedHyderabadData = await _cache.GetStringAsync("Hyderabad");
        var cachedLondonData = await _cache.GetStringAsync("London");

        if (cachedHyderabadData != null && cachedLondonData != null)
        {
            
            var hyderabadData = JsonConvert.DeserializeObject<WeatherData>(cachedHyderabadData);
            var londonData = JsonConvert.DeserializeObject<WeatherData>(cachedLondonData);

            HyderabadTemperature = hyderabadData.Main.Temp.ToString("0.0") + " °C";
            HyderabadWeather = hyderabadData.Weather[0].Description;

            LondonTemperature = londonData.Main.Temp.ToString("0.0") + " °C";
            LondonWeather = londonData.Weather[0].Description;

            return Page();
        }
        else
        {
           
            var hyderabadResponse = await FetchWeatherForCity("Hyderabad");
            var londonResponse = await FetchWeatherForCity("London");

            HyderabadTemperature = hyderabadResponse.Main.Temp.ToString("0.0") + " °C";
            HyderabadWeather = hyderabadResponse.Weather[0].Description;

            LondonTemperature = londonResponse.Main.Temp.ToString("0.0") + " °C";
            LondonWeather = londonResponse.Weather[0].Description;

            
            var hyderabadCacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) 
            };
            await _cache.SetStringAsync("Hyderabad", JsonConvert.SerializeObject(hyderabadResponse), hyderabadCacheOptions);

            var londonCacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) 
            };
            await _cache.SetStringAsync("London", JsonConvert.SerializeObject(londonResponse), londonCacheOptions);

            return Page();
        }
    }

    private async Task<WeatherData> FetchWeatherForCity(string city)
    {
        using (var httpClient = _clientFactory.CreateClient())
        {
            var response = await httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={city}&appid=64cbbed2ffa169fc27667980f1679456&units=metric");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WeatherData>(json);
        }
    }
}
