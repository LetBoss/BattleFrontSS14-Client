using System;
using System.Numerics;
using Content.Client.Botany.Components;
using Content.Shared.Botany;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Botany;

public sealed class PotencyVisualsSystem : VisualizerSystem<PotencyVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, PotencyVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		float num = default(float);
		if (args.Sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<float>(uid, (Enum)ProduceVisuals.Potency, ref num, args.Component))
		{
			float num2 = MathHelper.Lerp(component.MinimumScale, component.MaximumScale, num / 100f);
			base.SpriteSystem.SetScale(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), new Vector2(num2, num2));
		}
	}
}
