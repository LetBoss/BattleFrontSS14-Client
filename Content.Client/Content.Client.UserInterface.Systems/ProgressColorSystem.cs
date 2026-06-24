using System;
using System.Numerics;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Systems;

public sealed class ProgressColorSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _configuration;

	private bool _colorBlindFriendly;

	private static readonly Color[] Plasma = (Color[])(object)new Color[5]
	{
		new Color((byte)240, (byte)249, (byte)33, byte.MaxValue),
		new Color((byte)248, (byte)149, (byte)64, byte.MaxValue),
		new Color((byte)204, (byte)71, (byte)120, byte.MaxValue),
		new Color((byte)126, (byte)3, (byte)168, byte.MaxValue),
		new Color((byte)13, (byte)8, (byte)135, byte.MaxValue)
	};

	public override void Initialize()
	{
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _configuration, CCVars.AccessibilityColorblindFriendly, (CVarChanged<bool>)OnColorBlindFriendlyChanged, true);
	}

	private void OnColorBlindFriendlyChanged(bool value, in CVarChangeInfo info)
	{
		_colorBlindFriendly = value;
	}

	public Color GetProgressColor(float progress)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!_colorBlindFriendly)
		{
			if (progress >= 1f)
			{
				return new Color(0f, 1f, 0f, 1f);
			}
			return Color.FromHsv(new Vector4(5f / 18f * progress, 1f, 0.75f, 1f));
		}
		return InterpolateColorGaussian(Plasma, progress);
	}

	public static Color InterpolateColorGaussian(Color[] colors, double x)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 0.0;
		float num4 = 0f;
		double num5 = 1.0 / (double)(colors.Length - 1);
		double num6 = 0.0;
		foreach (Color val in colors)
		{
			double num7 = Math.Exp((0.0 - (x - num6)) * (x - num6) / 0.07) / Math.Sqrt(0.21991148575128555);
			num4 += (float)num7;
			num6 += num5;
			num += (double)val.R * num7;
			num2 += (double)val.G * num7;
			num3 += (double)val.B * num7;
		}
		return new Color((float)num / num4, (float)num2 / num4, (float)num3 / num4, 1f);
	}
}
