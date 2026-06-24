using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public static class PubgPartyPingColorResolver
{
	private static readonly Color[] SourcePalette = (Color[])(object)new Color[6]
	{
		Color.FromHex((ReadOnlySpan<char>)"#4FC3F7", (Color?)null),
		Color.FromHex((ReadOnlySpan<char>)"#66BB6A", (Color?)null),
		Color.FromHex((ReadOnlySpan<char>)"#FFB74D", (Color?)null),
		Color.FromHex((ReadOnlySpan<char>)"#F06292", (Color?)null),
		Color.FromHex((ReadOnlySpan<char>)"#26A69A", (Color?)null),
		Color.FromHex((ReadOnlySpan<char>)"#9575CD", (Color?)null)
	};

	public static Color GetColor(NetEntity source)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		int num = source.Id;
		switch (num)
		{
		case 0:
			return SourcePalette[0];
		case int.MinValue:
			num = int.MaxValue;
			break;
		}
		int num2 = Math.Abs(num) % SourcePalette.Length;
		return SourcePalette[num2];
	}
}
