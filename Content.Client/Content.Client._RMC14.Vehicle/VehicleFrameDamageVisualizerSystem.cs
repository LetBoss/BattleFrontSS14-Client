using System;
using Content.Shared._RMC14.Vehicle;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Vehicle;

public sealed class VehicleFrameDamageVisualizerSystem : VisualizerSystem<HardpointIntegrityComponent>
{
	private const float ShowThreshold = 0.9f;

	private const float MinAlpha = 0.1f;

	protected override void OnAppearanceChange(EntityUid uid, HardpointIntegrityComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		int num = default(int);
		if (sprite != null && sprite.LayerMapTryGet((object)"damaged_frame", ref num, false))
		{
			float num2 = default(float);
			if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<float>(uid, (Enum)VehicleFrameDamageVisuals.IntegrityFraction, ref num2, (AppearanceComponent)null))
			{
				float num3 = ((component.MaxIntegrity > 0f) ? component.MaxIntegrity : 1f);
				num2 = Math.Clamp(component.Integrity / num3, 0f, 1f);
			}
			if (num2 >= 0.9f)
			{
				sprite.LayerSetVisible(num, false);
				return;
			}
			float num4 = num2 / 0.9f;
			float num5 = 0.1f + 0.9f * (1f - num4);
			sprite.LayerSetVisible(num, true);
			int num6 = num;
			Color color = sprite.Color;
			sprite.LayerSetColor(num6, ((Color)(ref color)).WithAlpha(num5));
		}
	}
}
