public class WeatherData
{
	public MainData Main { get; set; }
	public WeatherDescription[] Weather { get; set; }
}

public class MainData
{
	public double Temp { get; set; }
}

public class WeatherDescription
{
	public string Description { get; set; }
}
