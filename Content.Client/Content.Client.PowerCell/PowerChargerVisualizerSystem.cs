using System;
using Content.Shared.Power;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.PowerCell;

public sealed class PowerChargerVisualizerSystem : VisualizerSystem<PowerChargerVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, PowerChargerVisualsComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			bool flag = default(bool);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)CellVisual.Occupied, ref flag, args.Component) && flag)
			{
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PowerChargerVisualLayers.Base, StateId.op_Implicit(comp.OccupiedState));
			}
			else
			{
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PowerChargerVisualLayers.Base, StateId.op_Implicit(comp.EmptyState));
			}
			CellChargerStatus key = default(CellChargerStatus);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<CellChargerStatus>(uid, (Enum)CellVisual.Light, ref key, args.Component) && comp.LightStates.TryGetValue(key, out string value))
			{
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PowerChargerVisualLayers.Light, StateId.op_Implicit(value));
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PowerChargerVisualLayers.Light, true);
			}
			else
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PowerChargerVisualLayers.Light, false);
			}
		}
	}
}
