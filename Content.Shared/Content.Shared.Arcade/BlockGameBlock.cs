using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Arcade;

[Serializable]
[NetSerializable]
public struct BlockGameBlock(Vector2i position, BlockGameBlock.BlockGameBlockColor gameBlockColor)
{
	[Serializable]
	[NetSerializable]
	public enum BlockGameBlockColor
	{
		Red,
		Orange,
		Yellow,
		Green,
		Blue,
		LightBlue,
		Purple,
		GhostRed,
		GhostOrange,
		GhostYellow,
		GhostGreen,
		GhostBlue,
		GhostLightBlue,
		GhostPurple
	}

	public Vector2i Position = position;

	public readonly BlockGameBlockColor GameBlockColor = gameBlockColor;

	public static BlockGameBlockColor ToGhostBlockColor(BlockGameBlockColor inColor)
	{
		return inColor switch
		{
			BlockGameBlockColor.Red => BlockGameBlockColor.GhostRed, 
			BlockGameBlockColor.Orange => BlockGameBlockColor.GhostOrange, 
			BlockGameBlockColor.Yellow => BlockGameBlockColor.GhostYellow, 
			BlockGameBlockColor.Green => BlockGameBlockColor.GhostGreen, 
			BlockGameBlockColor.Blue => BlockGameBlockColor.GhostBlue, 
			BlockGameBlockColor.LightBlue => BlockGameBlockColor.GhostLightBlue, 
			BlockGameBlockColor.Purple => BlockGameBlockColor.GhostPurple, 
			_ => inColor, 
		};
	}

	public static Color ToColor(BlockGameBlockColor inColor)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		Color val;
		switch (inColor)
		{
		case BlockGameBlockColor.Red:
			return Color.Red;
		case BlockGameBlockColor.Orange:
			return Color.Orange;
		case BlockGameBlockColor.Yellow:
			return Color.Yellow;
		case BlockGameBlockColor.Green:
			return Color.Lime;
		case BlockGameBlockColor.Blue:
			return Color.Blue;
		case BlockGameBlockColor.Purple:
			return Color.DarkOrchid;
		case BlockGameBlockColor.LightBlue:
			return Color.Cyan;
		case BlockGameBlockColor.GhostRed:
			val = Color.Red;
			return ((Color)(ref val)).WithAlpha(0.33f);
		case BlockGameBlockColor.GhostOrange:
			val = Color.Orange;
			return ((Color)(ref val)).WithAlpha(0.33f);
		case BlockGameBlockColor.GhostYellow:
			val = Color.Yellow;
			return ((Color)(ref val)).WithAlpha(0.33f);
		case BlockGameBlockColor.GhostGreen:
			val = Color.Lime;
			return ((Color)(ref val)).WithAlpha(0.33f);
		case BlockGameBlockColor.GhostBlue:
			val = Color.Blue;
			return ((Color)(ref val)).WithAlpha(0.33f);
		case BlockGameBlockColor.GhostPurple:
			val = Color.DarkOrchid;
			return ((Color)(ref val)).WithAlpha(0.33f);
		case BlockGameBlockColor.GhostLightBlue:
			val = Color.Cyan;
			return ((Color)(ref val)).WithAlpha(0.33f);
		default:
			return Color.Olive;
		}
	}
}
