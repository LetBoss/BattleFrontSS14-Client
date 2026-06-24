using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Content.Shared._RMC14.Localizations;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;

namespace Content.Shared.Localizations;

public sealed class ContentLocalizationManager
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static LocFunction _003C0_003E__FormatPressure;

		public static LocFunction _003C1_003E__FormatPowerWatts;

		public static LocFunction _003C2_003E__FormatPowerJoules;

		public static LocFunction _003C3_003E__FormatEnergyWattHours;

		public static LocFunction _003C4_003E__FormatUnits;

		public static LocFunction _003C5_003E__FormatLoc;

		public static LocFunction _003C6_003E__FormatPlaytime;
	}

	[Dependency]
	private ILocalizationManager _loc;

	[Dependency]
	private IConfigurationManager _cfg;

	public const string CultureRu = "ru-RU";

	public const string CultureEn = "en-US";

	public static readonly string[] TimeSpanMinutesFormats = new string[4] { "m\\:ss", "mm\\:ss", "%m", "mm" };

	private bool _ruLocaleLoaded;

	private static readonly Regex PluralEsRule = new Regex("^.*(s|sh|ch|x|z)$");

	public void Initialize()
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected O, but got Unknown
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Expected O, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Expected O, but got Unknown
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Expected O, but got Unknown
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Expected O, but got Unknown
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Expected O, but got Unknown
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Expected O, but got Unknown
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Expected O, but got Unknown
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Expected O, but got Unknown
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Expected O, but got Unknown
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Expected O, but got Unknown
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Expected O, but got Unknown
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Expected O, but got Unknown
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Expected O, but got Unknown
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Expected O, but got Unknown
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Expected O, but got Unknown
		CultureInfo cultureRu = new CultureInfo("ru-RU");
		CultureInfo cultureEn = new CultureInfo("en-US");
		_loc.LoadCulture(cultureEn);
		try
		{
			_loc.LoadCulture(cultureRu);
			_ruLocaleLoaded = true;
		}
		catch (Exception ex)
		{
			IoCManager.Resolve<ILogManager>().GetSawmill("loc").Error("Failed to load ru-RU locale, falling back to en-US: " + ex.Message);
			_ruLocaleLoaded = false;
		}
		ILocalizationManager loc = _loc;
		CultureInfo cultureInfo = cultureEn;
		object obj = _003C_003EO._003C0_003E__FormatPressure;
		if (obj == null)
		{
			LocFunction val = FormatPressure;
			_003C_003EO._003C0_003E__FormatPressure = val;
			obj = (object)val;
		}
		loc.AddFunction(cultureInfo, "PRESSURE", (LocFunction)obj);
		ILocalizationManager loc2 = _loc;
		CultureInfo cultureInfo2 = cultureEn;
		object obj2 = _003C_003EO._003C1_003E__FormatPowerWatts;
		if (obj2 == null)
		{
			LocFunction val2 = FormatPowerWatts;
			_003C_003EO._003C1_003E__FormatPowerWatts = val2;
			obj2 = (object)val2;
		}
		loc2.AddFunction(cultureInfo2, "POWERWATTS", (LocFunction)obj2);
		ILocalizationManager loc3 = _loc;
		CultureInfo cultureInfo3 = cultureEn;
		object obj3 = _003C_003EO._003C2_003E__FormatPowerJoules;
		if (obj3 == null)
		{
			LocFunction val3 = FormatPowerJoules;
			_003C_003EO._003C2_003E__FormatPowerJoules = val3;
			obj3 = (object)val3;
		}
		loc3.AddFunction(cultureInfo3, "POWERJOULES", (LocFunction)obj3);
		ILocalizationManager loc4 = _loc;
		CultureInfo cultureInfo4 = cultureEn;
		object obj4 = _003C_003EO._003C3_003E__FormatEnergyWattHours;
		if (obj4 == null)
		{
			LocFunction val4 = FormatEnergyWattHours;
			_003C_003EO._003C3_003E__FormatEnergyWattHours = val4;
			obj4 = (object)val4;
		}
		loc4.AddFunction(cultureInfo4, "ENERGYWATTHOURS", (LocFunction)obj4);
		ILocalizationManager loc5 = _loc;
		CultureInfo cultureInfo5 = cultureEn;
		object obj5 = _003C_003EO._003C4_003E__FormatUnits;
		if (obj5 == null)
		{
			LocFunction val5 = FormatUnits;
			_003C_003EO._003C4_003E__FormatUnits = val5;
			obj5 = (object)val5;
		}
		loc5.AddFunction(cultureInfo5, "UNITS", (LocFunction)obj5);
		_loc.AddFunction(cultureEn, "TOSTRING", (LocFunction)((LocArgs args) => FormatToString(cultureEn, args)));
		ILocalizationManager loc6 = _loc;
		CultureInfo cultureInfo6 = cultureEn;
		object obj6 = _003C_003EO._003C5_003E__FormatLoc;
		if (obj6 == null)
		{
			LocFunction val6 = FormatLoc;
			_003C_003EO._003C5_003E__FormatLoc = val6;
			obj6 = (object)val6;
		}
		loc6.AddFunction(cultureInfo6, "LOC", (LocFunction)obj6);
		_loc.AddFunction(cultureEn, "NATURALFIXED", new LocFunction(FormatNaturalFixed));
		_loc.AddFunction(cultureEn, "NATURALPERCENT", new LocFunction(FormatNaturalPercent));
		ILocalizationManager loc7 = _loc;
		CultureInfo cultureInfo7 = cultureEn;
		object obj7 = _003C_003EO._003C6_003E__FormatPlaytime;
		if (obj7 == null)
		{
			LocFunction val7 = FormatPlaytime;
			_003C_003EO._003C6_003E__FormatPlaytime = val7;
			obj7 = (object)val7;
		}
		loc7.AddFunction(cultureInfo7, "PLAYTIME", (LocFunction)obj7);
		_loc.AddFunction(cultureEn, "MAKEPLURAL", new LocFunction(FormatMakePlural));
		_loc.AddFunction(cultureEn, "MANY", new LocFunction(FormatMany));
		IoCManager.Resolve<RMCLocalizationManager>().Initialize(cultureEn);
		if (_ruLocaleLoaded)
		{
			ILocalizationManager loc8 = _loc;
			CultureInfo cultureInfo8 = cultureRu;
			object obj8 = _003C_003EO._003C0_003E__FormatPressure;
			if (obj8 == null)
			{
				LocFunction val8 = FormatPressure;
				_003C_003EO._003C0_003E__FormatPressure = val8;
				obj8 = (object)val8;
			}
			loc8.AddFunction(cultureInfo8, "PRESSURE", (LocFunction)obj8);
			ILocalizationManager loc9 = _loc;
			CultureInfo cultureInfo9 = cultureRu;
			object obj9 = _003C_003EO._003C1_003E__FormatPowerWatts;
			if (obj9 == null)
			{
				LocFunction val9 = FormatPowerWatts;
				_003C_003EO._003C1_003E__FormatPowerWatts = val9;
				obj9 = (object)val9;
			}
			loc9.AddFunction(cultureInfo9, "POWERWATTS", (LocFunction)obj9);
			ILocalizationManager loc10 = _loc;
			CultureInfo cultureInfo10 = cultureRu;
			object obj10 = _003C_003EO._003C2_003E__FormatPowerJoules;
			if (obj10 == null)
			{
				LocFunction val10 = FormatPowerJoules;
				_003C_003EO._003C2_003E__FormatPowerJoules = val10;
				obj10 = (object)val10;
			}
			loc10.AddFunction(cultureInfo10, "POWERJOULES", (LocFunction)obj10);
			ILocalizationManager loc11 = _loc;
			CultureInfo cultureInfo11 = cultureRu;
			object obj11 = _003C_003EO._003C3_003E__FormatEnergyWattHours;
			if (obj11 == null)
			{
				LocFunction val11 = FormatEnergyWattHours;
				_003C_003EO._003C3_003E__FormatEnergyWattHours = val11;
				obj11 = (object)val11;
			}
			loc11.AddFunction(cultureInfo11, "ENERGYWATTHOURS", (LocFunction)obj11);
			ILocalizationManager loc12 = _loc;
			CultureInfo cultureInfo12 = cultureRu;
			object obj12 = _003C_003EO._003C4_003E__FormatUnits;
			if (obj12 == null)
			{
				LocFunction val12 = FormatUnits;
				_003C_003EO._003C4_003E__FormatUnits = val12;
				obj12 = (object)val12;
			}
			loc12.AddFunction(cultureInfo12, "UNITS", (LocFunction)obj12);
			_loc.AddFunction(cultureRu, "TOSTRING", (LocFunction)((LocArgs args) => FormatToString(cultureRu, args)));
			ILocalizationManager loc13 = _loc;
			CultureInfo cultureInfo13 = cultureRu;
			object obj13 = _003C_003EO._003C5_003E__FormatLoc;
			if (obj13 == null)
			{
				LocFunction val13 = FormatLoc;
				_003C_003EO._003C5_003E__FormatLoc = val13;
				obj13 = (object)val13;
			}
			loc13.AddFunction(cultureInfo13, "LOC", (LocFunction)obj13);
			_loc.AddFunction(cultureRu, "NATURALFIXED", new LocFunction(FormatNaturalFixed));
			_loc.AddFunction(cultureRu, "NATURALPERCENT", new LocFunction(FormatNaturalPercent));
			ILocalizationManager loc14 = _loc;
			CultureInfo cultureInfo14 = cultureRu;
			object obj14 = _003C_003EO._003C6_003E__FormatPlaytime;
			if (obj14 == null)
			{
				LocFunction val14 = FormatPlaytime;
				_003C_003EO._003C6_003E__FormatPlaytime = val14;
				obj14 = (object)val14;
			}
			loc14.AddFunction(cultureInfo14, "PLAYTIME", (LocFunction)obj14);
			IoCManager.Resolve<RMCLocalizationManager>().Initialize(cultureRu);
		}
		if (_cfg.IsCVarRegistered(((CVarDef)CCVars.Language).Name))
		{
			string selectedLanguage = _cfg.GetCVar<string>(CCVars.Language);
			ApplyLanguage(selectedLanguage);
			_cfg.OnValueChanged<string>(CCVars.Language, (Action<string>)OnLanguageChanged, false);
		}
		else
		{
			_loc.DefaultCulture = cultureRu;
		}
	}

	private void OnLanguageChanged(string newLanguage)
	{
		ApplyLanguage(newLanguage);
	}

	private void ApplyLanguage(string language)
	{
		if (language == "auto")
		{
			language = "ru-RU";
		}
		if (language == "ru-RU" && !_ruLocaleLoaded)
		{
			language = "en-US";
		}
		CultureInfo culture = new CultureInfo(language);
		_loc.DefaultCulture = culture;
	}

	private ILocValue FormatMany(LocArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		if (!(Math.Abs(((LocValue<double>)(LocValueNumber)((LocArgs)(ref args)).Args[1]).Value - 1.0) < 9.999999747378752E-05))
		{
			return (ILocValue)(LocValueString)FormatMakePlural(args);
		}
		return (ILocValue)(LocValueString)((LocArgs)(ref args)).Args[0];
	}

	private ILocValue FormatNaturalPercent(LocArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		double number = ((LocValue<double>)(LocValueNumber)((LocArgs)(ref args)).Args[0]).Value * 100.0;
		int maxDecimals = (int)Math.Floor(((LocValue<double>)(LocValueNumber)((LocArgs)(ref args)).Args[1]).Value);
		NumberFormatInfo formatter = (NumberFormatInfo)NumberFormatInfo.GetInstance(_loc.DefaultCulture ?? CultureInfo.GetCultureInfo("ru-RU")).Clone();
		formatter.NumberDecimalDigits = maxDecimals;
		return (ILocValue)new LocValueString(string.Format(formatter, "{0:N}", number).TrimEnd('0').TrimEnd(char.Parse(formatter.NumberDecimalSeparator)) + "%");
	}

	private ILocValue FormatNaturalFixed(LocArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		double number = ((LocValue<double>)(LocValueNumber)((LocArgs)(ref args)).Args[0]).Value;
		int maxDecimals = (int)Math.Floor(((LocValue<double>)(LocValueNumber)((LocArgs)(ref args)).Args[1]).Value);
		NumberFormatInfo formatter = (NumberFormatInfo)NumberFormatInfo.GetInstance(_loc.DefaultCulture ?? CultureInfo.GetCultureInfo("ru-RU")).Clone();
		formatter.NumberDecimalDigits = maxDecimals;
		return (ILocValue)new LocValueString(string.Format(formatter, "{0:N}", number).TrimEnd('0').TrimEnd(char.Parse(formatter.NumberDecimalSeparator)));
	}

	private ILocValue FormatMakePlural(LocArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		string[] split = ((LocValue<string>)(LocValueString)((LocArgs)(ref args)).Args[0]).Value.Split(" ", 1);
		string firstWord = split[0];
		if (PluralEsRule.IsMatch(firstWord))
		{
			if (split.Length != 1)
			{
				return (ILocValue)new LocValueString(firstWord + "es " + split[1]);
			}
			return (ILocValue)new LocValueString(firstWord + "es");
		}
		if (split.Length != 1)
		{
			return (ILocValue)new LocValueString(firstWord + "s " + split[1]);
		}
		return (ILocValue)new LocValueString(firstWord + "s");
	}

	public static string FormatList(List<string> list)
	{
		int count = list.Count;
		if (count > 0)
		{
			switch (count)
			{
			case 1:
				return list[0];
			case 2:
				return list[0] + " and " + list[1];
			default:
				return string.Join(", ", list.GetRange(0, list.Count - 1)) + ", and " + list[list.Count - 1];
			}
		}
		return string.Empty;
	}

	public static string FormatListToOr(List<string> list)
	{
		int count = list.Count;
		if (count > 0)
		{
			return count switch
			{
				1 => list[0], 
				2 => list[0] + " or " + list[1], 
				_ => string.Join(" or ", list) ?? "", 
			};
		}
		return string.Empty;
	}

	public unsafe static string FormatDirection(Direction dir)
	{
		return Loc.GetString("zzzz-fmt-direction-" + ((object)(*(Direction*)(&dir))/*cast due to constrained. prefix*/).ToString());
	}

	public static string FormatPlaytime(TimeSpan time)
	{
		time = TimeSpan.FromMinutes(Math.Ceiling(time.TotalMinutes));
		int hours = (int)time.TotalHours;
		int minutes = time.Minutes;
		return Loc.GetString("zzzz-fmt-playtime", new(string, object)[2]
		{
			("hours", hours),
			("minutes", minutes)
		});
	}

	private static ILocValue FormatLoc(LocArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		return (ILocValue)new LocValueString(Loc.GetString(((LocValue<string>)(LocValueString)((LocArgs)(ref args)).Args[0]).Value, ((LocArgs)(ref args)).Options.Select((KeyValuePair<string, ILocValue> x) => (Key: x.Key, x.Value.Value)).ToArray()));
	}

	private static ILocValue FormatToString(CultureInfo culture, LocArgs args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		ILocValue obj = ((LocArgs)(ref args)).Args[0];
		string fmt = ((LocValue<string>)(LocValueString)((LocArgs)(ref args)).Args[1]).Value;
		object obj2 = obj.Value;
		if (obj2 is IFormattable formattable)
		{
			return (ILocValue)new LocValueString(formattable.ToString(fmt, culture));
		}
		return (ILocValue)new LocValueString(obj2?.ToString() ?? "");
	}

	private static ILocValue FormatUnitsGeneric(LocArgs args, string mode, Func<double, double>? transformValue = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		double pressure = ((LocValue<double>)(LocValueNumber)((LocArgs)(ref args)).Args[0]).Value;
		if (transformValue != null)
		{
			pressure = transformValue(pressure);
		}
		int places = 0;
		while (pressure > 1000.0 && places < 5)
		{
			pressure /= 1000.0;
			places++;
		}
		return (ILocValue)new LocValueString(Loc.GetString(mode, new(string, object)[2]
		{
			("divided", pressure),
			("places", places)
		}));
	}

	private static ILocValue FormatPressure(LocArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return FormatUnitsGeneric(args, "zzzz-fmt-pressure");
	}

	private static ILocValue FormatPowerWatts(LocArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return FormatUnitsGeneric(args, "zzzz-fmt-power-watts");
	}

	private static ILocValue FormatPowerJoules(LocArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return FormatUnitsGeneric(args, "zzzz-fmt-power-joules");
	}

	private static ILocValue FormatEnergyWattHours(LocArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return FormatUnitsGeneric(args, "zzzz-fmt-energy-watt-hours", (double joules) => joules * 0.0002777777777777778);
	}

	private static ILocValue FormatUnits(LocArgs args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		if (!Units.Types.TryGetValue(((LocValue<string>)(LocValueString)((LocArgs)(ref args)).Args[0]).Value, out Units.TypeTable ut))
		{
			throw new ArgumentException("Unknown unit type " + ((LocValue<string>)(LocValueString)((LocArgs)(ref args)).Args[0]).Value);
		}
		string fmtstr = ((LocValue<string>)(LocValueString)((LocArgs)(ref args)).Args[1]).Value;
		double max = double.NegativeInfinity;
		double[] iargs = new double[((LocArgs)(ref args)).Args.Count - 1];
		for (int i = 2; i < ((LocArgs)(ref args)).Args.Count; i++)
		{
			double n = ((LocValue<double>)(LocValueNumber)((LocArgs)(ref args)).Args[i]).Value;
			if (n > max)
			{
				max = n;
			}
			iargs[i - 2] = n;
		}
		if (!ut.TryGetUnit(max, out Units.TypeTable.Entry mu))
		{
			throw new ArgumentException("Unit out of range for type");
		}
		object[] fargs = new object[iargs.Length];
		for (int j = 0; j < iargs.Length; j++)
		{
			fargs[j] = iargs[j] * mu.Factor;
		}
		fargs[^1] = Loc.GetString("units-" + mu.Unit.ToLower());
		return (ILocValue)new LocValueString(string.Format(fmtstr.Replace("{UNIT", "{" + $"{fargs.Length - 1}"), fargs));
	}

	private static ILocValue FormatPlaytime(LocArgs args)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		TimeSpan time = TimeSpan.Zero;
		IReadOnlyList<ILocValue> args2 = ((LocArgs)(ref args)).Args;
		if (args2 != null && args2.Count > 0 && ((LocArgs)(ref args)).Args[0].Value is TimeSpan timeArg)
		{
			time = timeArg;
		}
		return (ILocValue)new LocValueString(FormatPlaytime(time));
	}
}
