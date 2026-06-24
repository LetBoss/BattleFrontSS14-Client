using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Robust.Shared.ColorNaming;

public static class ColorNaming
{
	private static readonly (float Hue, string Loc)[] HueNames = new(float, string)[9]
	{
		(float.DegreesToRadians(0f), "color-pink"),
		(float.DegreesToRadians(15f), "color-red"),
		(float.DegreesToRadians(45f), "color-orange"),
		(float.DegreesToRadians(90f), "color-yellow"),
		(float.DegreesToRadians(135f), "color-green"),
		(float.DegreesToRadians(180f), "color-cyan"),
		(float.DegreesToRadians(240f), "color-blue"),
		(float.DegreesToRadians(285f), "color-purple"),
		(float.DegreesToRadians(330f), "color-pink")
	};

	private static readonly (float Hue, string Loc) HueFallback = (Hue: float.DegreesToRadians(361f), Loc: "color-pink");

	private const float BrownLightnessThreshold = 0.675f;

	private static readonly LocId OrangeString = "color-orange";

	private static readonly LocId BrownString = "color-brown";

	private const float VeryDarkLightnessThreshold = 0.25f;

	private const float DarkLightnessThreshold = 0.5f;

	private const float NeutralLightnessThreshold = 0.7f;

	private const float LightLightnessThreshold = 0.85f;

	private static readonly LocId VeryDarkString = "color-very-dark";

	private static readonly LocId DarkString = "color-dark";

	private static readonly LocId LightString = "color-light";

	private static readonly LocId VeryLightString = "color-very-light";

	private static readonly LocId MixedHueString = "color-mixed-hue";

	private static readonly LocId LightLowChromaString = "color-pale";

	private static readonly LocId DarkLowChromaString = "color-gray-adjective";

	private static readonly LocId HighChromaString = "color-strong";

	private static readonly LocId WhiteString = "color-white";

	private static readonly LocId GrayString = "color-gray";

	private static readonly LocId BlackString = "color-black";

	private const float LowChromaThreshold = 0.07f;

	private const float HighChromaThreshold = 0.16f;

	private const float LightLowChromaThreshold = 0.6f;

	private const float WhiteLightnessThreshold = 0.99f;

	private const float BlackLightnessThreshold = 0.01f;

	private const float GrayChromaThreshold = 0.01f;

	private static (string Loc, float AdjustedLightness) DescribeHue(Vector4 oklch, ILocalizationManager localization)
	{
		Unsafe.SkipInit(out float num);
		Unsafe.SkipInit(out float num2);
		Unsafe.SkipInit(out float num3);
		Unsafe.SkipInit(out float num4);
		VectorHelpers.Deconstruct(oklch, ref num, ref num2, ref num3, ref num4);
		float num5 = num;
		float num6 = num3;
		for (int i = 0; i < HueNames.Length; i++)
		{
			(float, string) tuple = HueNames[i];
			(float, string) tuple2 = ((i + 1 < HueNames.Length) ? HueNames[i + 1] : HueFallback);
			if (tuple.Item1 > num6 || num6 >= tuple2.Item1)
			{
				continue;
			}
			string text = tuple.Item2;
			float item = num5;
			if ((LocId)tuple.Item2 == OrangeString && num5 <= 0.675f)
			{
				text = BrownString;
			}
			else if ((LocId)tuple.Item2 == OrangeString)
			{
				item = num5 - 0.675f + 0.5f;
			}
			if (num6 >= (tuple.Item1 + tuple2.Item1) / 2f && tuple.Item2 != tuple2.Item2)
			{
				if (localization.TryGetString(text + "-" + tuple2.Item2, out string value))
				{
					return (Loc: value, AdjustedLightness: item);
				}
				return (Loc: localization.GetString(MixedHueString, ("a", localization.GetString(text)), ("b", localization.GetString(tuple2.Item2))), AdjustedLightness: item);
			}
			return (Loc: localization.GetString(text), AdjustedLightness: item);
		}
		return (Loc: localization.GetString("color-unknown"), AdjustedLightness: num5);
	}

	private static string? DescribeChroma(Vector4 oklch, ILocalizationManager localization)
	{
		Unsafe.SkipInit(out float num);
		Unsafe.SkipInit(out float num2);
		Unsafe.SkipInit(out float num3);
		Unsafe.SkipInit(out float num4);
		VectorHelpers.Deconstruct(oklch, ref num, ref num2, ref num3, ref num4);
		float num5 = num;
		float num6 = num2;
		if (num6 <= 0.07f)
		{
			if (num5 >= 0.6f)
			{
				return localization.GetString(LightLowChromaString);
			}
			return localization.GetString(DarkLowChromaString);
		}
		if (num6 >= 0.16f)
		{
			return localization.GetString(HighChromaString);
		}
		return null;
	}

	private static string? DescribeLightness(Vector4 oklch, ILocalizationManager localization)
	{
		float x = oklch.X;
		if (!(x < 0.25f))
		{
			if (!(x < 0.5f))
			{
				if (!(x < 0.7f))
				{
					if (x < 0.85f)
					{
						return localization.GetString(LightString);
					}
					return localization.GetString(VeryLightString);
				}
				return null;
			}
			return localization.GetString(DarkString);
		}
		return localization.GetString(VeryDarkString);
	}

	public static string Describe(Color srgb, ILocalizationManager localization)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Vector4 oklch = Color.ToLch(Color.ToLab(Color.FromSrgb(srgb)));
		if (oklch.X >= 0.99f)
		{
			return localization.GetString(WhiteString);
		}
		if (oklch.X <= 0.01f)
		{
			return localization.GetString(BlackString);
		}
		(string Loc, float AdjustedLightness) tuple = DescribeHue(oklch, localization);
		string text = tuple.Loc;
		float item = tuple.AdjustedLightness;
		oklch.X = item;
		string text2 = DescribeChroma(oklch, localization);
		string? text3 = DescribeLightness(oklch, localization);
		if (oklch.Y <= 0.01f)
		{
			text = localization.GetString(GrayString);
			text2 = null;
		}
		string text4 = text;
		string text5 = text2;
		string text6 = text3;
		if (text4 != null)
		{
			if (text5 != null)
			{
				if (text6 != null)
				{
					return localization.GetString("color-hue-chroma-lightness", ("hue", text4), ("chroma", text5), ("lightness", text6));
				}
				string item2 = text4;
				string item3 = text5;
				return localization.GetString("color-hue-chroma", ("hue", item2), ("chroma", item3));
			}
			if (text6 != null)
			{
				string item4 = text4;
				string item5 = text6;
				return localization.GetString("color-hue-lightness", ("hue", item4), ("lightness", item5));
			}
			return text4;
		}
		global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException((text4, text5, text6));
		Unsafe.SkipInit(out string result);
		return result;
	}
}
