using System;
using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Xenonids.Fruit;

public sealed class XenoFruitPlanterVisualsSystem : VisualizerSystem<XenoFruitPlanterVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, XenoFruitPlanterVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		int num = default(int);
		if (sprite == null || !base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)XenoFruitVisualLayers.Base, ref num, false))
		{
			return;
		}
		Color val = default(Color);
		if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<Color>(uid, (Enum)XenoFruitPlanterVisuals.Color, ref val, (AppearanceComponent)null))
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, false);
			return;
		}
		base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, true);
		base.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, val);
		bool flag = default(bool);
		bool flag2 = default(bool);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)XenoFruitPlanterVisuals.Downed, ref flag, (AppearanceComponent)null) && flag)
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, StateId.op_Implicit(component.Prefix + "_downed"));
		}
		else if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)XenoFruitPlanterVisuals.Resting, ref flag2, (AppearanceComponent)null) && flag2)
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, StateId.op_Implicit(component.Prefix + "_rest"));
		}
		else
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, StateId.op_Implicit(component.Prefix + "_walk"));
		}
	}
}
