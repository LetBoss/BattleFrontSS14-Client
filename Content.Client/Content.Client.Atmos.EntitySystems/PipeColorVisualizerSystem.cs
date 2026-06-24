using System;
using Content.Client.Atmos.Components;
using Content.Shared.Atmos.Piping;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.EntitySystems;

public sealed class PipeColorVisualizerSystem : VisualizerSystem<PipeColorVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, PipeColorVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		Color val2 = default(Color);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val) && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<Color>(uid, (Enum)PipeColorVisuals.Color, ref val2, args.Component))
		{
			ISpriteLayer val3 = val[(object)PipeVisualLayers.Pipe];
			val3.Color = ((Color)(ref val2)).WithAlpha(val3.Color.A);
		}
	}
}
