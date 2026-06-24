using System;
using System.Numerics;
using Robust.Shared.Maths;

namespace Content.Shared.Humanoid;

public static class SkinColor
{
	public const float MaxTintedHuesSaturation = 0.1f;

	public const float MinTintedHuesLightness = 0.85f;

	public const float MinHuesLightness = 0.175f;

	public const float MinFeathersHue = 0.08055556f;

	public const float MaxFeathersHue = 29f / 60f;

	public const float MinFeathersSaturation = 0.2f;

	public const float MaxFeathersSaturation = 0.88f;

	public const float MinFeathersValue = 0.36f;

	public const float MaxFeathersValue = 0.55f;

	public static Color ValidHumanSkinTone => Color.FromHsv(new Vector4(0.07f, 0.2f, 1f, 1f));

	public static Color ValidTintedHuesSkinTone(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return TintedHues(color);
	}

	public static Color HumanSkinTone(int tone)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		tone = Math.Clamp(tone, 0, 100);
		int rangeOffset = tone - 20;
		float hue = 25f;
		float sat = 20f;
		float val = 100f;
		if (rangeOffset <= 0)
		{
			hue += (float)Math.Abs(rangeOffset);
		}
		else
		{
			sat += (float)rangeOffset;
			val -= (float)rangeOffset;
		}
		return Color.FromHsv(new Vector4(hue / 360f, sat / 100f, val / 100f, 1f));
	}

	public static float HumanSkinToneFromColor(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Vector4 hsv = Color.ToHsv(color);
		if (Math.Clamp(hsv.X, 5f / 72f, 1f) > 5f / 72f && (double)hsv.Z == 1.0)
		{
			return Math.Abs(45f - hsv.X * 360f);
		}
		return hsv.Y * 100f;
	}

	public static bool VerifyHumanSkinTone(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = Color.ToHsv(color);
		double hue = Math.Round(vector.X * 360f);
		double sat = Math.Round(vector.Y * 100f);
		double val = Math.Round(vector.Z * 100f);
		if (hue < 25.0 || hue > 45.0)
		{
			return false;
		}
		if (sat < 20.0 || val < 20.0)
		{
			return false;
		}
		return true;
	}

	public static Color TintedHues(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Vector4 newColor = Color.ToHsl(color);
		newColor.Y *= 0.1f;
		newColor.Z = MathHelper.Lerp(0.85f, 1f, newColor.Z);
		return Color.FromHsv(newColor);
	}

	public static bool VerifyTintedHues(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (Color.ToHsl(color).Y <= 0.1f)
		{
			return Color.ToHsl(color).Z >= 0.85f;
		}
		return false;
	}

	public static Color ProportionalVoxColor(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		Vector4 newColor = Color.ToHsv(color);
		newColor.X = newColor.X * 0.40277776f + 0.08055556f;
		newColor.Y = newColor.Y * 0.68f + 0.2f;
		newColor.Z = newColor.Z * 0.19f + 0.36f;
		return Color.FromHsv(newColor);
	}

	public static Color ClosestVoxColor(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Vector4 hsv = Color.ToHsv(color);
		hsv.X = Math.Clamp(hsv.X, 0.08055556f, 29f / 60f);
		hsv.Y = Math.Clamp(hsv.Y, 0.2f, 0.88f);
		hsv.Z = Math.Clamp(hsv.Z, 0.36f, 0.55f);
		return Color.FromHsv(hsv);
	}

	public static bool VerifyVoxFeathers(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Vector4 colorHsv = Color.ToHsv(color);
		if (colorHsv.X < 0.08055556f || colorHsv.X > 29f / 60f)
		{
			return false;
		}
		if (colorHsv.Y < 0.2f || colorHsv.Y > 0.88f)
		{
			return false;
		}
		if (colorHsv.Z < 0.36f || colorHsv.Z > 0.55f)
		{
			return false;
		}
		return true;
	}

	public static Color MakeHueValid(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Vector4 manipulatedColor = Color.ToHsv(color);
		manipulatedColor.Z = Math.Max(manipulatedColor.Z, 0.175f);
		return Color.FromHsv(manipulatedColor);
	}

	public static bool VerifyHues(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Color.ToHsv(color).Z >= 0.175f;
	}

	public static bool VerifySkinColor(HumanoidSkinColor type, Color color)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return type switch
		{
			HumanoidSkinColor.HumanToned => VerifyHumanSkinTone(color), 
			HumanoidSkinColor.TintedHues => VerifyTintedHues(color), 
			HumanoidSkinColor.Hues => VerifyHues(color), 
			HumanoidSkinColor.VoxFeathers => VerifyVoxFeathers(color), 
			_ => false, 
		};
	}

	public static Color ValidSkinTone(HumanoidSkinColor type, Color color)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(type switch
		{
			HumanoidSkinColor.HumanToned => ValidHumanSkinTone, 
			HumanoidSkinColor.TintedHues => ValidTintedHuesSkinTone(color), 
			HumanoidSkinColor.Hues => MakeHueValid(color), 
			HumanoidSkinColor.VoxFeathers => ClosestVoxColor(color), 
			_ => color, 
		});
	}
}
