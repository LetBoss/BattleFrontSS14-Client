using System;
using Content.Shared.Pinpointer;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Pinpointer;

public sealed class PinpointerSystem : SharedPinpointerSystem
{
	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<PinpointerComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<PinpointerComponent, SpriteComponent>();
		EntityUid item = default(EntityUid);
		PinpointerComponent pinpointerComponent = default(PinpointerComponent);
		SpriteComponent item2 = default(SpriteComponent);
		while (val.MoveNext(ref item, ref pinpointerComponent, ref item2))
		{
			if (pinpointerComponent.HasTarget)
			{
				IEye currentEye = _eyeManager.CurrentEye;
				Angle val2 = pinpointerComponent.ArrowAngle + currentEye.Rotation;
				Distance distanceToTarget = pinpointerComponent.DistanceToTarget;
				if (distanceToTarget - 2 <= Distance.Close)
				{
					_sprite.LayerSetRotation(Entity<SpriteComponent>.op_Implicit((item, item2)), (Enum)PinpointerLayers.Screen, val2);
				}
				else
				{
					_sprite.LayerSetRotation(Entity<SpriteComponent>.op_Implicit((item, item2)), (Enum)PinpointerLayers.Screen, Angle.Zero);
				}
			}
		}
	}
}
