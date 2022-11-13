namespace HomeCook.Services.Interfaces
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> Get();
        public bool IsDbContext();
    }
}
