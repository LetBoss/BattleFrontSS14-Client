// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Log.LogMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.Log;

public static class LogMessage
{
  public const string LogNameVerbose = "VERB";
  public const string LogNameDebug = "DEBG";
  public const string LogNameInfo = "INFO";
  public const string LogNameWarning = "WARN";
  public const string LogNameError = "ERRO";
  public const string LogNameFatal = "FATL";
  public const string LogNameUnknown = "UNKO";

  public static string LogLevelToName(LogLevel level)
  {
    string name;
    switch (level)
    {
      case LogLevel.Verbose:
        name = "VERB";
        break;
      case LogLevel.Debug:
        name = "DEBG";
        break;
      case LogLevel.Info:
        name = "INFO";
        break;
      case LogLevel.Warning:
        name = "WARN";
        break;
      case LogLevel.Error:
        name = "ERRO";
        break;
      case LogLevel.Fatal:
        name = "FATL";
        break;
      default:
        name = "UNKO";
        break;
    }
    return name;
  }
}
