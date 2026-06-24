using System;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Rotate;

public sealed class XenoRotateSystem : EntitySystem
{
	[Dependency]
	private RotateToFaceSystem _rotateTo;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	public void RotateXeno(EntityUid uid, Direction direction, TimeSpan? delay = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		XenoRotateComponent rotationComp = ((EntitySystem)this).EnsureComp<XenoRotateComponent>(uid);
		rotationComp.TargetDirection = direction;
		rotationComp.Delay = delay ?? rotationComp.Delay;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)rotationComp, (MetaDataComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<XenoRotateComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoRotateComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		XenoRotateComponent rotate = default(XenoRotateComponent);
		TransformComponent xform = default(TransformComponent);
		while (query.MoveNext(ref uid, ref rotate, ref xform))
		{
			if (!(rotate.NextRotation > _timing.CurTime))
			{
				if (rotate.FirstRotation)
				{
					XenoRotateComponent xenoRotateComponent = rotate;
					Angle worldRotation = _transform.GetWorldRotation(xform);
					xenoRotateComponent.OriginalDirection = ((Angle)(ref worldRotation)).GetDir();
					rotate.NextRotation = _timing.CurTime + rotate.Delay;
					rotate.FirstRotation = false;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)rotate, (MetaDataComponent)null);
					_rotateTo.TryFaceAngle(uid, DirectionExtensions.ToAngle(rotate.TargetDirection), xform);
				}
				else if (rotate.OriginalDirection.HasValue)
				{
					_rotateTo.TryFaceAngle(uid, DirectionExtensions.ToAngle(rotate.OriginalDirection.Value), xform);
					((EntitySystem)this).RemCompDeferred<XenoRotateComponent>(uid);
				}
			}
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<XenoRotateComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoRotateComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		XenoRotateComponent rotate = default(XenoRotateComponent);
		TransformComponent xform = default(TransformComponent);
		while (query.MoveNext(ref uid, ref rotate, ref xform))
		{
			_rotateTo.TryFaceAngle(uid, DirectionExtensions.ToAngle(rotate.TargetDirection), xform);
		}
	}
}
