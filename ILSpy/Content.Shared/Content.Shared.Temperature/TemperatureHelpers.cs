namespace Content.Shared.Temperature;

public static class TemperatureHelpers
{
	public static float CelsiusToKelvin(float celsius)
	{
		return celsius + 273.15f;
	}

	public static float CelsiusToFahrenheit(float celsius)
	{
		return celsius * 9f / 5f + 32f;
	}

	public static float KelvinToCelsius(float kelvin)
	{
		return kelvin - 273.15f;
	}

	public static float KelvinToFahrenheit(float kelvin)
	{
		return CelsiusToFahrenheit(KelvinToCelsius(kelvin));
	}

	public static float FahrenheitToCelsius(float fahrenheit)
	{
		return (fahrenheit - 32f) * 5f / 9f;
	}

	public static float FahrenheitToKelvin(float fahrenheit)
	{
		return CelsiusToKelvin(FahrenheitToCelsius(fahrenheit));
	}
}
