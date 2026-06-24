// Decompiled with JetBrains decompiler
// Type: Content.Shared.Temperature.TemperatureHelpers
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

#nullable disable
namespace Content.Shared.Temperature;

public static class TemperatureHelpers
{
  public static float CelsiusToKelvin(float celsius) => celsius + 273.15f;

  public static float CelsiusToFahrenheit(float celsius)
  {
    return (float) ((double) celsius * 9.0 / 5.0 + 32.0);
  }

  public static float KelvinToCelsius(float kelvin) => kelvin - 273.15f;

  public static float KelvinToFahrenheit(float kelvin)
  {
    return TemperatureHelpers.CelsiusToFahrenheit(TemperatureHelpers.KelvinToCelsius(kelvin));
  }

  public static float FahrenheitToCelsius(float fahrenheit)
  {
    return (float) (((double) fahrenheit - 32.0) * 5.0 / 9.0);
  }

  public static float FahrenheitToKelvin(float fahrenheit)
  {
    return TemperatureHelpers.CelsiusToKelvin(TemperatureHelpers.FahrenheitToCelsius(fahrenheit));
  }
}
