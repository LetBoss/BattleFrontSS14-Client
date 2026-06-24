using System;
using Content.Shared.Throwing;
using Content.Shared.Timing;
using Content.Shared.Weapons.Melee.Components;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Weapons.Melee;

public sealed class UseDelayOnMeleeHitSystem : EntitySystem
{
	[Dependency]
	private UseDelaySystem _delay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<UseDelayOnMeleeHitComponent, MeleeHitEvent>((EntityEventRefHandler<UseDelayOnMeleeHitComponent, MeleeHitEvent>)OnMeleeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UseDelayOnMeleeHitComponent, ThrowDoHitEvent>((EntityEventRefHandler<UseDelayOnMeleeHitComponent, ThrowDoHitEvent>)OnThrowHitEvent, (Type[])null, (Type[])null);
	}

	private void OnThrowHitEvent(Entity<UseDelayOnMeleeHitComponent> ent, ref ThrowDoHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TryResetDelay(ent);
	}

	private void OnMeleeHit(Entity<UseDelayOnMeleeHitComponent> ent, ref MeleeHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TryResetDelay(ent);
	}

	private void TryResetDelay(Entity<UseDelayOnMeleeHitComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = ent.Owner;
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(uid, ref useDelay))
		{
			_delay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((uid, useDelay)), checkDelayed: true);
		}
	}
}
