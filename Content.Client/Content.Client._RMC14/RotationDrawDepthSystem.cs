using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client._RMC14;

public sealed class RotationDrawDepthSystem : EntitySystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void FrameUpdate(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<RotationDrawDepthComponent, SpriteComponent, TransformComponent> val = ((EntitySystem)this).EntityQueryEnumerator<RotationDrawDepthComponent, SpriteComponent, TransformComponent>();
		EntityUid item = default(EntityUid);
		RotationDrawDepthComponent rotationDrawDepthComponent = default(RotationDrawDepthComponent);
		SpriteComponent item2 = default(SpriteComponent);
		TransformComponent val2 = default(TransformComponent);
		while (val.MoveNext(ref item, ref rotationDrawDepthComponent, ref item2, ref val2))
		{
			Angle localRotation = val2.LocalRotation;
			if ((int)((Angle)(ref localRotation)).GetCardinalDir() == 0)
			{
				_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((item, item2)), rotationDrawDepthComponent.SouthDrawDepth);
			}
			else
			{
				_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((item, item2)), rotationDrawDepthComponent.DefaultDrawDepth);
			}
		}
	}
}
