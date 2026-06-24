using System;
using System.Collections.Generic;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Timing;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class SharedFireGroupSystem : EntitySystem
{
	[Dependency]
	private UseDelaySystem _delay;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCFireGroupComponent, GunShotEvent>((EntityEventRefHandler<RMCFireGroupComponent, GunShotEvent>)OnGunShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFireGroupComponent, ShotAttemptedEvent>((EntityEventRefHandler<RMCFireGroupComponent, ShotAttemptedEvent>)OnShotAttempt, (Type[])null, (Type[])null);
	}

	private void OnGunShot(Entity<RMCFireGroupComponent> ent, ref GunShotEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		EntityUid weapon = ent.Owner;
		RMCFireGroupComponent comp = ent.Comp;
		EntityUid user = args.User;
		RMCFireGroupComponent fireGroup = default(RMCFireGroupComponent);
		UseDelayComponent useDelay = default(UseDelayComponent);
		foreach (EntityUid item in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(user)))
		{
			if (((EntitySystem)this).TryComp<RMCFireGroupComponent>(item, ref fireGroup) && !(item == weapon) && !(fireGroup.Group != comp.Group) && ((EntitySystem)this).TryComp<UseDelayComponent>(item, ref useDelay))
			{
				(EntityUid, UseDelayComponent) itemEnt = (item, useDelay);
				_delay.SetLength(Entity<UseDelayComponent>.op_Implicit(itemEnt), comp.Delay, comp.UseDelayID);
				_delay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit(itemEnt), checkDelayed: true, comp.UseDelayID);
			}
		}
		RMCUserFireGroupComponent userGroup = ((EntitySystem)this).EnsureComp<RMCUserFireGroupComponent>(args.User);
		userGroup.LastFired[ent.Comp.Group] = _timing.CurTime;
		userGroup.LastGun[ent.Comp.Group] = Entity<RMCFireGroupComponent>.op_Implicit(ent);
		((EntitySystem)this).Dirty(args.User, (IComponent)(object)userGroup, (MetaDataComponent)null);
	}

	private void OnShotAttempt(Entity<RMCFireGroupComponent> ent, ref ShotAttemptedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			UseDelayComponent useDelayComponent = default(UseDelayComponent);
			RMCUserFireGroupComponent fireGroup = default(RMCUserFireGroupComponent);
			TimeSpan last;
			if (((EntitySystem)this).TryComp<UseDelayComponent>(ent.Owner, ref useDelayComponent) && _delay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((ent.Owner, useDelayComponent)), ent.Comp.UseDelayID))
			{
				args.Cancel();
			}
			else if (((EntitySystem)this).TryComp<RMCUserFireGroupComponent>(args.User, ref fireGroup) && fireGroup.LastFired.TryGetValue(ent.Comp.Group, out last) && _timing.CurTime < last + ent.Comp.Delay && fireGroup.LastGun.GetValueOrDefault(ent.Comp.Group) != ent.Owner)
			{
				args.Cancel();
			}
		}
	}
}
