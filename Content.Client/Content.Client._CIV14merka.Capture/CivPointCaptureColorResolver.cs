using System;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.Capture;

public static class CivPointCaptureColorResolver
{
	public static readonly Color FriendlyColor = Color.FromHex((ReadOnlySpan<char>)"#59d46a", (Color?)null);

	public static readonly Color EnemyColor = Color.FromHex((ReadOnlySpan<char>)"#d85d5d", (Color?)null);

	public static readonly Color NeutralColor = Color.FromHex((ReadOnlySpan<char>)"#8b96a3", (Color?)null);

	public static Color GetRelationColor(int viewerTeamId, int teamId)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (teamId <= 0)
		{
			return NeutralColor;
		}
		if (viewerTeamId == 0 || viewerTeamId != teamId)
		{
			return EnemyColor;
		}
		return FriendlyColor;
	}

	public static Color GetCapturePulseColor(int viewerTeamId, int ownerTeamId, int capturingTeamId, float timeSeconds)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (capturingTeamId <= 0)
		{
			return GetRelationColor(viewerTeamId, ownerTeamId);
		}
		Color relationColor = GetRelationColor(viewerTeamId, capturingTeamId);
		float capturePulseAmount = GetCapturePulseAmount(timeSeconds);
		float num = ((ownerTeamId != 0 && ownerTeamId != capturingTeamId) ? 0.18f : 0.3f);
		return Blend(relationColor, Color.White, num + capturePulseAmount * (1f - num));
	}

	public static float GetCapturePulseAmount(float timeSeconds)
	{
		return MathF.Pow(0.5f + 0.5f * MathF.Sin(timeSeconds * 6f), 0.85f);
	}

	private static Color Blend(Color from, Color to, float amount)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		amount = Math.Clamp(amount, 0f, 1f);
		return new Color(from.R + (to.R - from.R) * amount, from.G + (to.G - from.G) * amount, from.B + (to.B - from.B) * amount, from.A + (to.A - from.A) * amount);
	}
}
