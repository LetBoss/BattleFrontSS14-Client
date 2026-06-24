using System.Collections.Generic;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Shared.Humanoid.Markings;

public static class MarkingColoring
{
	public static List<Color> GetMarkingLayerColors(MarkingPrototype prototype, Color? skinColor, Color? eyeColor, MarkingSet markingSet)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		List<Color> colors = new List<Color>();
		if (!prototype.RMCFollowSkinColor)
		{
			return colors;
		}
		Color defaultColor = prototype.Coloring.Default.GetColor(skinColor, eyeColor, markingSet);
		if (prototype.Coloring.Layers == null)
		{
			for (int i = 0; i < prototype.Sprites.Count; i++)
			{
				colors.Add(defaultColor);
			}
			return colors;
		}
		for (int j = 0; j < prototype.Sprites.Count; j++)
		{
			SpriteSpecifier val = prototype.Sprites[j];
			Rsi rsi = (Rsi)(object)((val is Rsi) ? val : null);
			string text;
			if (rsi == null)
			{
				Texture texture = (Texture)(object)((val is Texture) ? val : null);
				if (texture != null)
				{
					ResPath texturePath = texture.TexturePath;
					text = ((ResPath)(ref texturePath)).Filename;
				}
				else
				{
					text = null;
				}
			}
			else
			{
				text = rsi.RsiState;
			}
			string name = text;
			LayerColoringDefinition layerColoring;
			if (name == null)
			{
				colors.Add(defaultColor);
			}
			else if (prototype.Coloring.Layers.TryGetValue(name, out layerColoring))
			{
				Color marking_color = layerColoring.GetColor(skinColor, eyeColor, markingSet);
				colors.Add(marking_color);
			}
			else
			{
				colors.Add(defaultColor);
			}
		}
		return colors;
	}
}
