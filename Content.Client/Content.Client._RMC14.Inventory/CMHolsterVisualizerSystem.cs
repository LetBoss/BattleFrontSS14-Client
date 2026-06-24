using System;
using Content.Shared._RMC14.Inventory;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Inventory;

public sealed class CMHolsterVisualizerSystem : VisualizerSystem<CMHolsterComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, CMHolsterComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		int num = default(int);
		if (sprite != null && base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)CMHolsterLayers.Fill, ref num, false))
		{
			if (component.Contents.Count != 0)
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, true);
			}
			else
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, false);
			}
		}
	}
}
