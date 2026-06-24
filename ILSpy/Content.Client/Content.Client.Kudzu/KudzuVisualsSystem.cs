using System;
using Content.Shared.Spreader;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Kudzu;

public sealed class KudzuVisualsSystem : VisualizerSystem<KudzuVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, KudzuVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		int value = default(int);
		int value2 = default(int);
		if (args.Sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)KudzuVisuals.Variant, ref value, args.Component) && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)KudzuVisuals.GrowthLevel, ref value2, args.Component))
		{
			int num = base.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), $"{component.Layer}");
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit($"kudzu_{value2}{value}"));
		}
	}
}
