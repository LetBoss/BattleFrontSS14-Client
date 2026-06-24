using System;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Shared.MouseRotator;

public abstract class SharedMouseRotatorSystem : EntitySystem
{
	[Dependency]
	private RotateToFaceSystem _rotate;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeAllEvent<RequestMouseRotatorRotationEvent>((EntitySessionEventHandler<RequestMouseRotatorRotationEvent>)OnRequestRotation, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<MouseRotatorComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<MouseRotatorComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		MouseRotatorComponent rotator = default(MouseRotatorComponent);
		TransformComponent xform = default(TransformComponent);
		while (query.MoveNext(ref uid, ref rotator, ref xform))
		{
			if (rotator.GoalRotation.HasValue && _rotate.TryRotateTo(uid, rotator.GoalRotation.Value, frameTime, rotator.AngleTolerance, MathHelper.DegreesToRadians(rotator.RotationSpeed), xform))
			{
				rotator.GoalRotation = null;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)rotator, (MetaDataComponent)null);
			}
		}
	}

	private void OnRequestRotation(RequestMouseRotatorRotationEvent msg, EntitySessionEventArgs args)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid ent = attachedEntity.GetValueOrDefault();
			MouseRotatorComponent rotator = default(MouseRotatorComponent);
			if (((EntitySystem)this).TryComp<MouseRotatorComponent>(ent, ref rotator))
			{
				rotator.GoalRotation = msg.Rotation;
				((EntitySystem)this).Dirty(ent, (IComponent)(object)rotator, (MetaDataComponent)null);
				return;
			}
		}
		((EntitySystem)this).Log.Error($"User {((EntitySessionEventArgs)(ref args)).SenderSession.Name} ({((EntitySessionEventArgs)(ref args)).SenderSession.UserId}) tried setting local rotation directly without a valid mouse rotator component attached!");
	}
}
