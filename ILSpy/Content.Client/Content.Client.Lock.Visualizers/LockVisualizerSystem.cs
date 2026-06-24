using System;
using Content.Shared.Lock;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Lock.Visualizers;

public sealed class LockVisualizerSystem : VisualizerSystem<LockVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, LockVisualsComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		if (args.Sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)LockVisuals.Locked, ref flag, args.Component))
		{
			bool flag2 = default(bool);
			if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)LockVisuals.Locked, ref flag2, args.Component))
			{
				flag2 = true;
			}
			RSI baseRSI = args.Sprite.BaseRSI;
			State val = default(State);
			bool? flag3 = ((baseRSI != null) ? new bool?(baseRSI.TryGetState(StateId.op_Implicit(comp.StateUnlocked), ref val)) : ((bool?)null));
			bool flag4 = default(bool);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)StorageVisuals.Open, ref flag4, args.Component))
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)LockVisualLayers.Lock, !flag4);
			}
			else if (!flag3.Value)
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)LockVisualLayers.Lock, flag2);
			}
			if (!flag4 && flag3.Value)
			{
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)LockVisualLayers.Lock, StateId.op_Implicit(flag2 ? comp.StateLocked : comp.StateUnlocked));
			}
		}
	}
}
