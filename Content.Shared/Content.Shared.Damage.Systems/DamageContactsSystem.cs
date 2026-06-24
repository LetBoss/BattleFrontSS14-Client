using System;
using Content.Shared.Damage.Components;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Shared.Damage.Systems;

public sealed class DamageContactsSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DamageContactsComponent, StartCollideEvent>((ComponentEventRefHandler<DamageContactsComponent, StartCollideEvent>)OnEntityEnter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DamageContactsComponent, EndCollideEvent>((ComponentEventRefHandler<DamageContactsComponent, EndCollideEvent>)OnEntityExit, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<DamagedByContactComponent> query = ((EntitySystem)this).EntityQueryEnumerator<DamagedByContactComponent>();
		EntityUid ent = default(EntityUid);
		DamagedByContactComponent damaged = default(DamagedByContactComponent);
		while (query.MoveNext(ref ent, ref damaged))
		{
			if (!(_timing.CurTime < damaged.NextSecond))
			{
				damaged.NextSecond = _timing.CurTime + TimeSpan.FromSeconds(1L);
				if (damaged.Damage != null)
				{
					_damageable.TryChangeDamage(ent, damaged.Damage, ignoreResistances: false, interruptsDoAfters: false);
				}
			}
		}
	}

	private void OnEntityExit(EntityUid uid, DamageContactsComponent component, ref EndCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid otherUid = args.OtherEntity;
		PhysicsComponent body = default(PhysicsComponent);
		if (!((EntitySystem)this).TryComp<PhysicsComponent>(otherUid, ref body))
		{
			return;
		}
		EntityQuery<DamageContactsComponent> damageQuery = ((EntitySystem)this).GetEntityQuery<DamageContactsComponent>();
		foreach (EntityUid ent in _physics.GetContactingEntities(otherUid, body, false))
		{
			if (!(ent == uid) && damageQuery.HasComponent(ent))
			{
				return;
			}
		}
		((EntitySystem)this).RemComp<DamagedByContactComponent>(otherUid);
	}

	private void OnEntityEnter(EntityUid uid, DamageContactsComponent component, ref StartCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		EntityUid otherUid = args.OtherEntity;
		if (!((EntitySystem)this).HasComp<DamagedByContactComponent>(otherUid) && !_whitelistSystem.IsWhitelistPass(component.IgnoreWhitelist, otherUid))
		{
			((EntitySystem)this).EnsureComp<DamagedByContactComponent>(otherUid).Damage = component.Damage;
		}
	}
}
