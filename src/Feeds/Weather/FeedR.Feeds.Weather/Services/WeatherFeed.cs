using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using FeedR.Feeds.Weather.Models;

namespace FeedR.Feeds.Weather.Services;

internal sealed class WeatherFeed : IWeatherFeed
{
    // read key, url and interval from an Weather API application settings
    private const string ApiKey = "34da06ff82664814a42133637230604";
    private const string ApiUrl = "http://api.weatherapi.com/v1/current.json";
    private const int IntervalSeconds = 5;
    
    private readonly HttpClient _client;
    private readonly ILogger<WeatherFeed> _logger;

    public WeatherFeed(HttpClient client, ILogger<WeatherFeed> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async IAsyncEnumerable<WeatherData> SubscribeAsync(
        string location,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var url = $"{ApiUrl}?key={ApiKey}&query={location}&aqi=no";

        while (!cancellationToken.IsCancellationRequested)
        {
            WeatherApiResponse? response = null;
            try // alternatively, extend the Polly policy to handle this error
            {
                response = await _client.GetFromJsonAsync<WeatherApiResponse>(url, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }

            if (response is null)
            {
                continue;
            }

            yield return new WeatherData(
                Location: $"{response.Location.Name}, {response.Location.Country}",
                Temperature: response.Current.TempC,
                Humidity: response.Current.Humidity,
                WindSpeed: response.Current.WindKph,
                Condition: response.Current.Condition.Text
            );

            await Task.Delay(TimeSpan.FromSeconds(IntervalSeconds), cancellationToken);
        }
    }

    private record WeatherApiResponse(Location Location, Weather Current);

    private record Location(string Name, string Country);

    private record Weather(
        [property: JsonPropertyName("temp_c")] double TempC,
        double Humidity,
        Condition Condition,
        [property: JsonPropertyName("wind_kph")] double WindKph);

    private record Condition(string Text);
}