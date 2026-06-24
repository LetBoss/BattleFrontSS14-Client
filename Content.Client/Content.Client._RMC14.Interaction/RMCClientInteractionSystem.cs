using System;
using System.Numerics;
using Content.Shared._RMC14.Interaction;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Interaction;

public sealed class RMCClientInteractionSystem : EntitySystem
{
	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private TransformSystem _transform;

	public bool IsInteractionTransparency(EntityUid target, EntityUid? localEntity, IEye? eye)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			if (eye != null && ((EntitySystem)this).HasComp<InteractionTransparencyComponent>(target))
			{
				TransformComponent val = default(TransformComponent);
				SpriteComponent item = default(SpriteComponent);
				TransformComponent val2 = default(TransformComponent);
				if (!((EntitySystem)this).TryComp(target, ref val) || !((EntitySystem)this).TryComp<SpriteComponent>(target, ref item) || !((EntitySystem)this).TryComp(valueOrDefault, ref val2))
				{
					return false;
				}
				ValueTuple<Vector2, Angle> worldPositionRotation = ((SharedTransformSystem)_transform).GetWorldPositionRotation(val);
				Vector2 item2 = worldPositionRotation.Item1;
				Angle item3 = worldPositionRotation.Item2;
				Box2Rotated val3 = _sprite.CalculateBounds(Entity<SpriteComponent>.op_Implicit((target, item)), item2, item3, eye.Rotation);
				Vector2 position = ((SharedTransformSystem)_transform).GetMapCoordinates(val2).Position;
				return ((Box2Rotated)(ref val3)).Contains(position);
			}
		}
		return false;
	}
}
