using System;
using Content.Shared.Smoking;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Chemistry.Visualizers;

public sealed class SmokeVisualizerSystem : VisualizerSystem<SmokeVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, SmokeVisualsComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Color val = default(Color);
		if (args.Sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<Color>(uid, (Enum)SmokeVisuals.Color, ref val, (AppearanceComponent)null))
		{
			base.SpriteSystem.SetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), val);
		}
	}
}
