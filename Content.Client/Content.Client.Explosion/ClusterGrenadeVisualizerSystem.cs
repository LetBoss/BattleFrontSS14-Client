using System;
using Content.Shared.Explosion.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Explosion;

public sealed class ClusterGrenadeVisualizerSystem : VisualizerSystem<ClusterGrenadeVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, ClusterGrenadeVisualsComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		int value = default(int);
		if (args.Sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)ClusterGrenadeVisuals.GrenadesCounter, ref value, args.Component))
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, StateId.op_Implicit($"{comp.State}-{value}"));
		}
	}
}
