using System;
using Robust.Shared.Maths;

namespace Content.Shared._PUBG.Armor;

public static class PubgArmorHelpers
{
	public static float GetDurabilityRatio(PubgArmorComponent component)
	{
		if (component.MaxDurability <= 0f)
		{
			return 0f;
		}
		return Math.Clamp(component.Durability / component.MaxDurability, 0f, 1f);
	}

	public static Color GetDurabilityColor(float ratio)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Color val = new Color(1f, 0f, 0f, 1f);
		Color green = default(Color);
		((Color)(ref green))._002Ector(0f, 1f, 0f, 1f);
		return Color.InterpolateBetween(val, green, ratio);
	}
}
