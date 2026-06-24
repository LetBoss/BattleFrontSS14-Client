using System;
using Content.Shared.Light.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Light.Visualizers;

public sealed class LightBulbSystem : VisualizerSystem<LightBulbComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, LightBulbComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite == null)
		{
			return;
		}
		LightBulbState lightBulbState = default(LightBulbState);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<LightBulbState>(uid, (Enum)LightBulbVisuals.State, ref lightBulbState, args.Component))
		{
			switch (lightBulbState)
			{
			case LightBulbState.Normal:
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)LightBulbVisualLayers.Base, StateId.op_Implicit(comp.NormalSpriteState));
				break;
			case LightBulbState.Broken:
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)LightBulbVisualLayers.Base, StateId.op_Implicit(comp.BrokenSpriteState));
				break;
			case LightBulbState.Burned:
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)LightBulbVisualLayers.Base, StateId.op_Implicit(comp.BurnedSpriteState));
				break;
			}
		}
		Color val = default(Color);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<Color>(uid, (Enum)LightBulbVisuals.Color, ref val, args.Component))
		{
			base.SpriteSystem.SetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), val);
		}
	}
}
