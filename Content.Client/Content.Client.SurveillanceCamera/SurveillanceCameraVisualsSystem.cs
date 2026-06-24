using System;
using Content.Shared.SurveillanceCamera;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.SurveillanceCamera;

public sealed class SurveillanceCameraVisualsSystem : EntitySystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SurveillanceCameraVisualsComponent, AppearanceChangeEvent>((ComponentEventRefHandler<SurveillanceCameraVisualsComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnAppearanceChange(EntityUid uid, SurveillanceCameraVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (args.AppearanceData.TryGetValue(SurveillanceCameraVisualsKey.Key, out var value) && value is SurveillanceCameraVisuals key && args.Sprite != null && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)SurveillanceCameraVisualsKey.Layer, ref num, false) && component.CameraSprites.TryGetValue(key, out string value2))
		{
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit(value2));
		}
	}
}
