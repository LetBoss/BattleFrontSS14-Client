using System;
using Content.Shared.Shuttles.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Shuttles;

public sealed class ThrusterSystem : VisualizerSystem<ThrusterComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, ThrusterComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		if (args.Sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)ThrusterVisualState.State, ref flag, args.Component))
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ThrusterVisualLayers.ThrustOn, flag);
			bool flag2 = default(bool);
			SetThrusting(uid, flag && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)ThrusterVisualState.Thrusting, ref flag2, args.Component) && flag2, args.Sprite);
		}
	}

	private void SetThrusting(EntityUid uid, bool value, SpriteComponent sprite)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)ThrusterVisualLayers.Thrusting, ref num, false))
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, value);
		}
		int num2 = default(int);
		if (base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)ThrusterVisualLayers.ThrustingUnshaded, ref num2, false))
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, value);
		}
	}
}
