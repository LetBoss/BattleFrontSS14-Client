using System;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Systems;

public sealed class DoAfterMobCollisionSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ActiveDoAfterComponent, AttemptMobCollideEvent>((ComponentEventRefHandler<ActiveDoAfterComponent, AttemptMobCollideEvent>)OnAttemptMobCollide, (Type[])null, (Type[])null);
	}

	private void OnAttemptMobCollide(EntityUid uid, ActiveDoAfterComponent component, ref AttemptMobCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DoAfterComponent doAfterComp = default(DoAfterComponent);
		if (!((EntitySystem)this).TryComp<DoAfterComponent>(uid, ref doAfterComp))
		{
			return;
		}
		foreach (Content.Shared.DoAfter.DoAfter doAfter in doAfterComp.DoAfters.Values)
		{
			if (!doAfter.Cancelled && !doAfter.Completed && doAfter.Args.RootEntity)
			{
				args.Cancelled = true;
				break;
			}
		}
	}
}
