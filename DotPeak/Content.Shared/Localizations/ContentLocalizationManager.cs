// Decompiled with JetBrains decompiler
// Type: Content.Shared.Localizations.ContentLocalizationManager
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Localizations;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

#nullable enable
namespace Content.Shared.Localizations;

public sealed class ContentLocalizationManager
{
  [Dependency]
  private ILocalizationManager _loc;
  [Dependency]
  private IConfigurationManager _cfg;
  public const string CultureRu = "ru-RU";
  public const string CultureEn = "en-US";
  public static readonly string[] TimeSpanMinutesFormats = new string[4]
  {
    "m\\:ss",
    "mm\\:ss",
    "%m",
    "mm"
  };
  private bool _ruLocaleLoaded;
  private static readonly Regex PluralEsRule = new Regex("^.*(s|sh|ch|x|z)$");

  public void Initialize()
  {
    CultureInfo cultureRu = new CultureInfo("ru-RU");
    CultureInfo cultureEn = new CultureInfo("en-US");
    this._loc.LoadCulture(cultureEn);
    try
    {
      this._loc.LoadCulture(cultureRu);
      this._ruLocaleLoaded = true;
    }
    catch (Exception ex)
    {
      IoCManager.Resolve<ILogManager>().GetSawmill("loc").Error("Failed to load ru-RU locale, falling back to en-US: " + ex.Message);
      this._ruLocaleLoaded = false;
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this._loc.AddFunction(cultureEn, "PRESSURE", ContentLocalizationManager.\u003C\u003EO.\u003C0\u003E__FormatPressure ?? (ContentLocalizationManager.\u003C\u003EO.\u003C0\u003E__FormatPressure = new LocFunction(ContentLocalizationManager.FormatPressure)));
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this._loc.AddFunction(cultureEn, "POWERWATTS", ContentLocalizationManager.\u003C\u003EO.\u003C1\u003E__FormatPowerWatts ?? (ContentLocalizationManager.\u003C\u003EO.\u003C1\u003E__FormatPowerWatts = new LocFunction(ContentLocalizationManager.FormatPowerWatts)));
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this._loc.AddFunction(cultureEn, "POWERJOULES", ContentLocalizationManager.\u003C\u003EO.\u003C2\u003E__FormatPowerJoules ?? (ContentLocalizationManager.\u003C\u003EO.\u003C2\u003E__FormatPowerJoules = new LocFunction(ContentLocalizationManager.FormatPowerJoules)));
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this._loc.AddFunction(cultureEn, "ENERGYWATTHOURS", ContentLocalizationManager.\u003C\u003EO.\u003C3\u003E__FormatEnergyWattHours ?? (ContentLocalizationManager.\u003C\u003EO.\u003C3\u003E__FormatEnergyWattHours = new LocFunction(ContentLocalizationManager.FormatEnergyWattHours)));
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this._loc.AddFunction(cultureEn, "UNITS", ContentLocalizationManager.\u003C\u003EO.\u003C4\u003E__FormatUnits ?? (ContentLocalizationManager.\u003C\u003EO.\u003C4\u003E__FormatUnits = new LocFunction(ContentLocalizationManager.FormatUnits)));
    this._loc.AddFunction(cultureEn, "TOSTRING", (LocFunction) (args => ContentLocalizationManager.FormatToString(cultureEn, args)));
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this._loc.AddFunction(cultureEn, "LOC", ContentLocalizationManager.\u003C\u003EO.\u003C5\u003E__FormatLoc ?? (ContentLocalizationManager.\u003C\u003EO.\u003C5\u003E__FormatLoc = new LocFunction(ContentLocalizationManager.FormatLoc)));
    this._loc.AddFunction(cultureEn, "NATURALFIXED", new LocFunction(this.FormatNaturalFixed));
    this._loc.AddFunction(cultureEn, "NATURALPERCENT", new LocFunction(this.FormatNaturalPercent));
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this._loc.AddFunction(cultureEn, "PLAYTIME", ContentLocalizationManager.\u003C\u003EO.\u003C6\u003E__FormatPlaytime ?? (ContentLocalizationManager.\u003C\u003EO.\u003C6\u003E__FormatPlaytime = new LocFunction(ContentLocalizationManager.FormatPlaytime)));
    this._loc.AddFunction(cultureEn, "MAKEPLURAL", new LocFunction(this.FormatMakePlural));
    this._loc.AddFunction(cultureEn, "MANY", new LocFunction(this.FormatMany));
    IoCManager.Resolve<RMCLocalizationManager>().Initialize(cultureEn);
    if (this._ruLocaleLoaded)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this._loc.AddFunction(cultureRu, "PRESSURE", ContentLocalizationManager.\u003C\u003EO.\u003C0\u003E__FormatPressure ?? (ContentLocalizationManager.\u003C\u003EO.\u003C0\u003E__FormatPressure = new LocFunction(ContentLocalizationManager.FormatPressure)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this._loc.AddFunction(cultureRu, "POWERWATTS", ContentLocalizationManager.\u003C\u003EO.\u003C1\u003E__FormatPowerWatts ?? (ContentLocalizationManager.\u003C\u003EO.\u003C1\u003E__FormatPowerWatts = new LocFunction(ContentLocalizationManager.FormatPowerWatts)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this._loc.AddFunction(cultureRu, "POWERJOULES", ContentLocalizationManager.\u003C\u003EO.\u003C2\u003E__FormatPowerJoules ?? (ContentLocalizationManager.\u003C\u003EO.\u003C2\u003E__FormatPowerJoules = new LocFunction(ContentLocalizationManager.FormatPowerJoules)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this._loc.AddFunction(cultureRu, "ENERGYWATTHOURS", ContentLocalizationManager.\u003C\u003EO.\u003C3\u003E__FormatEnergyWattHours ?? (ContentLocalizationManager.\u003C\u003EO.\u003C3\u003E__FormatEnergyWattHours = new LocFunction(ContentLocalizationManager.FormatEnergyWattHours)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this._loc.AddFunction(cultureRu, "UNITS", ContentLocalizationManager.\u003C\u003EO.\u003C4\u003E__FormatUnits ?? (ContentLocalizationManager.\u003C\u003EO.\u003C4\u003E__FormatUnits = new LocFunction(ContentLocalizationManager.FormatUnits)));
      this._loc.AddFunction(cultureRu, "TOSTRING", (LocFunction) (args => ContentLocalizationManager.FormatToString(cultureRu, args)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this._loc.AddFunction(cultureRu, "LOC", ContentLocalizationManager.\u003C\u003EO.\u003C5\u003E__FormatLoc ?? (ContentLocalizationManager.\u003C\u003EO.\u003C5\u003E__FormatLoc = new LocFunction(ContentLocalizationManager.FormatLoc)));
      this._loc.AddFunction(cultureRu, "NATURALFIXED", new LocFunction(this.FormatNaturalFixed));
      this._loc.AddFunction(cultureRu, "NATURALPERCENT", new LocFunction(this.FormatNaturalPercent));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this._loc.AddFunction(cultureRu, "PLAYTIME", ContentLocalizationManager.\u003C\u003EO.\u003C6\u003E__FormatPlaytime ?? (ContentLocalizationManager.\u003C\u003EO.\u003C6\u003E__FormatPlaytime = new LocFunction(ContentLocalizationManager.FormatPlaytime)));
      IoCManager.Resolve<RMCLocalizationManager>().Initialize(cultureRu);
    }
    if (this._cfg.IsCVarRegistered(CCVars.Language.Name))
    {
      this.ApplyLanguage(this._cfg.GetCVar<string>(CCVars.Language));
      this._cfg.OnValueChanged<string>(CCVars.Language, new Action<string>(this.OnLanguageChanged));
    }
    else
      this._loc.DefaultCulture = cultureRu;
  }

  private void OnLanguageChanged(string newLanguage) => this.ApplyLanguage(newLanguage);

  private void ApplyLanguage(string language)
  {
    if (language == "auto")
      language = "ru-RU";
    if (language == "ru-RU" && !this._ruLocaleLoaded)
      language = "en-US";
    this._loc.DefaultCulture = new CultureInfo(language);
  }

  private ILocValue FormatMany(LocArgs args)
  {
    return Math.Abs(((LocValue<double>) args.Args[1]).Value - 1.0) < 9.9999997473787516E-05 ? args.Args[0] : this.FormatMakePlural(args);
  }

  private ILocValue FormatNaturalPercent(LocArgs args)
  {
    double num1 = ((LocValue<double>) args.Args[0]).Value * 100.0;
    int num2 = (int) Math.Floor(((LocValue<double>) args.Args[1]).Value);
    NumberFormatInfo provider = (NumberFormatInfo) NumberFormatInfo.GetInstance((IFormatProvider) (this._loc.DefaultCulture ?? CultureInfo.GetCultureInfo("ru-RU"))).Clone();
    provider.NumberDecimalDigits = num2;
    return (ILocValue) new LocValueString(string.Format((IFormatProvider) provider, "{0:N}", (object) num1).TrimEnd('0').TrimEnd(char.Parse(provider.NumberDecimalSeparator)) + "%");
  }

  private ILocValue FormatNaturalFixed(LocArgs args)
  {
    double num1 = ((LocValue<double>) args.Args[0]).Value;
    int num2 = (int) Math.Floor(((LocValue<double>) args.Args[1]).Value);
    NumberFormatInfo provider = (NumberFormatInfo) NumberFormatInfo.GetInstance((IFormatProvider) (this._loc.DefaultCulture ?? CultureInfo.GetCultureInfo("ru-RU"))).Clone();
    provider.NumberDecimalDigits = num2;
    return (ILocValue) new LocValueString(string.Format((IFormatProvider) provider, "{0:N}", (object) num1).TrimEnd('0').TrimEnd(char.Parse(provider.NumberDecimalSeparator)));
  }

  private ILocValue FormatMakePlural(LocArgs args)
  {
    string[] strArray = ((LocValue<string>) args.Args[0]).Value.Split(" ", 1);
    string input = strArray[0];
    return ContentLocalizationManager.PluralEsRule.IsMatch(input) ? (strArray.Length == 1 ? (ILocValue) new LocValueString(input + "es") : (ILocValue) new LocValueString($"{input}es {strArray[1]}")) : (strArray.Length == 1 ? (ILocValue) new LocValueString(input + "s") : (ILocValue) new LocValueString($"{input}s {strArray[1]}"));
  }

  public static string FormatList(List<string> list)
  {
    int count = list.Count;
    string str1;
    if (count > 0)
    {
      switch (count)
      {
        case 1:
          str1 = list[0];
          break;
        case 2:
          str1 = $"{list[0]} and {list[1]}";
          break;
        default:
          string str2 = string.Join(", ", (IEnumerable<string>) list.GetRange(0, list.Count - 1));
          List<string> stringList = list;
          string str3 = stringList[stringList.Count - 1];
          str1 = $"{str2}, and {str3}";
          break;
      }
    }
    else
      str1 = string.Empty;
    return str1;
  }

  public static string FormatListToOr(List<string> list)
  {
    int count = list.Count;
    string or;
    if (count > 0)
    {
      switch (count)
      {
        case 1:
          or = list[0];
          break;
        case 2:
          or = $"{list[0]} or {list[1]}";
          break;
        default:
          or = string.Join(" or ", (IEnumerable<string>) list) ?? "";
          break;
      }
    }
    else
      or = string.Empty;
    return or;
  }

  public static string FormatDirection(Direction dir)
  {
    return Loc.GetString("zzzz-fmt-direction-" + dir.ToString());
  }

  public static string FormatPlaytime(TimeSpan time)
  {
    time = TimeSpan.FromMinutes(Math.Ceiling(time.TotalMinutes));
    return Loc.GetString("zzzz-fmt-playtime", ("hours", (object) (int) time.TotalHours), ("minutes", (object) time.Minutes));
  }

  private static ILocValue FormatLoc(LocArgs args)
  {
    return (ILocValue) new LocValueString(Loc.GetString(((LocValue<string>) args.Args[0]).Value, args.Options.Select<KeyValuePair<string, ILocValue>, (string, object)>((Func<KeyValuePair<string, ILocValue>, (string, object)>) (x => (x.Key, x.Value.Value))).ToArray<(string, object)>()));
  }

  private static ILocValue FormatToString(CultureInfo culture, LocArgs args)
  {
    ILocValue locValue = args.Args[0];
    string format = ((LocValue<string>) args.Args[1]).Value;
    object obj = locValue.Value;
    return obj is IFormattable formattable ? (ILocValue) new LocValueString(formattable.ToString(format, (IFormatProvider) culture)) : (ILocValue) new LocValueString(obj?.ToString() ?? "");
  }

  private static ILocValue FormatUnitsGeneric(
    LocArgs args,
    string mode,
    Func<double, double>? transformValue = null)
  {
    double num1 = ((LocValue<double>) args.Args[0]).Value;
    if (transformValue != null)
      num1 = transformValue(num1);
    int num2;
    for (num2 = 0; num1 > 1000.0 && num2 < 5; ++num2)
      num1 /= 1000.0;
    return (ILocValue) new LocValueString(Loc.GetString(mode, ("divided", (object) num1), ("places", (object) num2)));
  }

  private static ILocValue FormatPressure(LocArgs args)
  {
    return ContentLocalizationManager.FormatUnitsGeneric(args, "zzzz-fmt-pressure");
  }

  private static ILocValue FormatPowerWatts(LocArgs args)
  {
    return ContentLocalizationManager.FormatUnitsGeneric(args, "zzzz-fmt-power-watts");
  }

  private static ILocValue FormatPowerJoules(LocArgs args)
  {
    return ContentLocalizationManager.FormatUnitsGeneric(args, "zzzz-fmt-power-joules");
  }

  private static ILocValue FormatEnergyWattHours(LocArgs args)
  {
    return ContentLocalizationManager.FormatUnitsGeneric(args, "zzzz-fmt-energy-watt-hours", (Func<double, double>) (joules => joules * 0.00027777777777777778));
  }

  private static ILocValue FormatUnits(LocArgs args)
  {
    Units.TypeTable typeTable;
    if (!Units.Types.TryGetValue(((LocValue<string>) args.Args[0]).Value, out typeTable))
      throw new ArgumentException("Unknown unit type " + ((LocValue<string>) args.Args[0]).Value);
    string str = ((LocValue<string>) args.Args[1]).Value;
    double val = double.NegativeInfinity;
    double[] numArray = new double[args.Args.Count - 1];
    for (int index = 2; index < args.Args.Count; ++index)
    {
      double num = ((LocValue<double>) args.Args[index]).Value;
      if (num > val)
        val = num;
      numArray[index - 2] = num;
    }
    Units.TypeTable.Entry winner;
    if (!typeTable.TryGetUnit(val, out winner))
      throw new ArgumentException("Unit out of range for type");
    object[] objArray1 = new object[numArray.Length];
    for (int index = 0; index < numArray.Length; ++index)
      objArray1[index] = (object) (numArray[index] * winner.Factor);
    object[] objArray2 = objArray1;
    objArray2[objArray2.Length - 1] = (object) Loc.GetString("units-" + winner.Unit.ToLower());
    return (ILocValue) new LocValueString(string.Format(str.Replace("{UNIT", "{" + $"{objArray1.Length - 1}"), objArray1));
  }

  private static ILocValue FormatPlaytime(LocArgs args)
  {
    zero = TimeSpan.Zero;
    IReadOnlyList<ILocValue> args1 = args.Args;
    if (args1 == null || args1.Count <= 0 || !(args.Args[0].Value is TimeSpan zero))
      ;
    return (ILocValue) new LocValueString(ContentLocalizationManager.FormatPlaytime(zero));
  }
}
