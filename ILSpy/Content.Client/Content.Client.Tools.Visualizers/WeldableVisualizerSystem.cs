using System;
using Content.Shared.Tools.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Tools.Visualizers;

public sealed class WeldableVisualizerSystem : VisualizerSystem<WeldableComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, WeldableComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			bool flag = default(bool);
			((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)WeldableVisuals.IsWelded, ref flag, args.Component);
			int num = default(int);
			if (base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)WeldableLayers.BaseWelded, ref num, false))
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, flag);
			}
		}
	}
}
