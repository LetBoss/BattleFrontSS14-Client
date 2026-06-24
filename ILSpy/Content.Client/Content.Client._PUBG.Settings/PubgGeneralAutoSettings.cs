using System;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.Settings;

public static class PubgGeneralAutoSettings
{
	public const int Version = 3;

	private static readonly Vector2i ReferenceResolution = new Vector2i(1920, 1080);

	private static readonly Vector2i FullHdResolution = new Vector2i(1920, 1080);

	private static readonly Vector2i QhdResolution = new Vector2i(2560, 1440);

	private const int ReferenceMinimapWidgetSize = 200;

	private const int ReferenceMinimapOffsetX = 0;

	private const int ReferenceMinimapOffsetY = 0;

	private const int ReferenceMinimapMarkerScale = 100;

	public static void Apply(IConfigurationManager cfg, Vector2i screenSize, int appliedVersion)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (appliedVersion < 1)
		{
			ApplyMinimapSettings(cfg, screenSize);
		}
		if (appliedVersion < 2)
		{
			ApplyHealthHudSettings(cfg, screenSize);
		}
		if (appliedVersion < 3)
		{
			ApplyBattlefrontLayout(cfg);
		}
	}

	private static void ApplyBattlefrontLayout(IConfigurationManager cfg)
	{
		cfg.SetCVar<string>(CCVars.UILayout, "Battlefront", false);
	}

	private static void ApplyMinimapSettings(IConfigurationManager cfg, Vector2i screenSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)screenSize.X / (float)ReferenceResolution.X;
		float num2 = (float)screenSize.Y / (float)ReferenceResolution.Y;
		float num3 = MathF.Min(num, num2);
		cfg.SetCVar<int>(CCVars.PubgMinimapWidgetSize, Math.Clamp((int)MathF.Round(200f * num3), 120, 500), false);
		cfg.SetCVar<int>(CCVars.PubgMinimapOffsetX, Math.Clamp((int)MathF.Round(0f * num), -600, 600), false);
		cfg.SetCVar<int>(CCVars.PubgMinimapOffsetY, Math.Clamp((int)MathF.Round(0f * num2), -600, 600), false);
		cfg.SetCVar<int>(CCVars.PubgMinimapMarkerScale, Math.Clamp((int)MathF.Round(100f * num3), 50, 250), false);
	}

	private static void ApplyHealthHudSettings(IConfigurationManager cfg, Vector2i screenSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (screenSize.X >= QhdResolution.X && screenSize.Y >= QhdResolution.Y)
		{
			cfg.SetCVar<int>(CCVars.PubgHealthHudOffsetY, 112, false);
			cfg.SetCVar<int>(CCVars.PubgHealthHudOffsetX, -1000, false);
		}
		else if (screenSize.X >= FullHdResolution.X && screenSize.Y >= FullHdResolution.Y)
		{
			cfg.SetCVar<int>(CCVars.PubgHealthHudOffsetY, -14, false);
			cfg.SetCVar<int>(CCVars.PubgHealthHudOffsetX, -492, false);
		}
	}
}
