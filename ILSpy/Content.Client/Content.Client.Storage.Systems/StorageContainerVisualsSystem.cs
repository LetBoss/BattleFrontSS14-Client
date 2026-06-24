using System;
using Content.Client.Storage.Components;
using Content.Shared.Rounding;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Storage.Systems;

public sealed class StorageContainerVisualsSystem : VisualizerSystem<StorageContainerVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, StorageContainerVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		int num2 = default(int);
		if (args.Sprite == null || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)StorageVisuals.StorageUsed, ref num, args.Component) || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)StorageVisuals.Capacity, ref num2, args.Component))
		{
			return;
		}
		float num3 = (float)num / (float)num2;
		int num4 = default(int);
		if (!base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)component.FillLayer, ref num4, false))
		{
			return;
		}
		int num5 = Math.Min(ContentHelpers.RoundToNearestLevels(num3, 1.0, component.MaxFillLevels + 1), component.MaxFillLevels);
		if (num5 > 0)
		{
			if (component.FillBaseName != null)
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num4, true);
				string text = component.FillBaseName + num5;
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num4, StateId.op_Implicit(text));
			}
		}
		else
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num4, false);
		}
	}
}
