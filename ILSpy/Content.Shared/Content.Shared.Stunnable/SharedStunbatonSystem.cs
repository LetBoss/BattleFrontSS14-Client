using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Stunnable;

public abstract class SharedStunbatonSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StunbatonComponent, ItemToggleActivateAttemptEvent>((EntityEventRefHandler<StunbatonComponent, ItemToggleActivateAttemptEvent>)TryTurnOn, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunbatonComponent, ItemToggleDeactivateAttemptEvent>((EntityEventRefHandler<StunbatonComponent, ItemToggleDeactivateAttemptEvent>)TryTurnOff, (Type[])null, (Type[])null);
	}

	protected virtual void TryTurnOn(Entity<StunbatonComponent> entity, ref ItemToggleActivateAttemptEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (args.User.HasValue && !_actionBlocker.CanComplexInteract(args.User.Value))
		{
			args.Cancelled = true;
		}
	}

	protected virtual void TryTurnOff(Entity<StunbatonComponent> entity, ref ItemToggleDeactivateAttemptEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (args.User.HasValue && !_actionBlocker.CanComplexInteract(args.User.Value))
		{
			args.Cancelled = true;
		}
	}
}
