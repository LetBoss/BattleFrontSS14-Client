using System;
using Content.Shared.Actions.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Xenonids.Actions;

public sealed class XenoActionsSystem : EntitySystem
{
	[Dependency]
	private XenoSystem _xeno;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoOffensiveActionComponent, ActionValidateEvent>((EntityEventRefHandler<XenoOffensiveActionComponent, ActionValidateEvent>)OnValidateActionEntityTarget, (Type[])null, (Type[])null);
	}

	private void OnValidateActionEntityTarget(Entity<XenoOffensiveActionComponent> ent, ref ActionValidateEvent args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (args.Invalid)
		{
			return;
		}
		EntityUid? entity = ((EntitySystem)this).GetEntity(args.Input.EntityTarget);
		if (entity.HasValue)
		{
			EntityUid target = entity.GetValueOrDefault();
			if (!_xeno.CanAbilityAttackTarget(args.User, target, canAttackBarricades: false, canAttackWindows: true))
			{
				args.Invalid = true;
			}
		}
	}
}
