using System;
using Content.Client.Power;
using Content.Shared.Atmos.Visuals;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Atmos.Visualizers;

public sealed class PortableScrubberSystem : VisualizerSystem<PortableScrubberVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, PortableScrubberVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			bool flag = default(bool);
			bool flag2 = default(bool);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)PortableScrubberVisuals.IsFull, ref flag, args.Component) && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)PortableScrubberVisuals.IsRunning, ref flag2, args.Component))
			{
				string text = (flag2 ? component.RunningState : component.IdleState);
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PortableScrubberVisualLayers.IsRunning, StateId.op_Implicit(text));
				string text2 = (flag ? component.FullState : component.ReadyState);
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PowerDeviceVisualLayers.Powered, StateId.op_Implicit(text2));
			}
			bool flag3 = default(bool);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)PortableScrubberVisuals.IsDraining, ref flag3, args.Component))
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PortableScrubberVisualLayers.IsDraining, flag3);
			}
		}
	}
}
