using System;
using Content.Client.Light.Components;
using Content.Shared.Light.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Light.EntitySystems;

public sealed class EmergencyLightSystem : VisualizerSystem<EmergencyLightComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, EmergencyLightComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			bool flag = default(bool);
			if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)EmergencyLightVisuals.On, ref flag, args.Component))
			{
				flag = false;
			}
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)EmergencyLightVisualLayers.LightOff, !flag);
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)EmergencyLightVisualLayers.LightOn, flag);
			Color val = default(Color);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<Color>(uid, (Enum)EmergencyLightVisuals.Color, ref val, args.Component))
			{
				base.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)EmergencyLightVisualLayers.LightOn, val);
				base.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)EmergencyLightVisualLayers.LightOff, val);
			}
		}
	}
}
