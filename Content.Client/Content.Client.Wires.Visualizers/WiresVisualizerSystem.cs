using System;
using Content.Shared.Wires;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Wires.Visualizers;

public sealed class WiresVisualizerSystem : VisualizerSystem<WiresVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, WiresVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			int num = base.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)WiresVisualLayers.MaintenancePanel);
			if (args.AppearanceData.TryGetValue(WiresVisuals.MaintenancePanelState, out var value) && value is bool flag)
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, flag);
			}
			else
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, false);
			}
		}
	}
}
