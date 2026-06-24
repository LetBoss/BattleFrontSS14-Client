using System;
using Content.Shared.Damage.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Damage;

public sealed class PassiveDamageSystem : EntitySystem
{
	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PassiveDamageComponent, MapInitEvent>((ComponentEventHandler<PassiveDamageComponent, MapInitEvent>)OnPendingMapInit, (Type[])null, (Type[])null);
	}

	private void OnPendingMapInit(EntityUid uid, PassiveDamageComponent component, MapInitEvent args)
	{
		component.NextDamage = _timing.CurTime + TimeSpan.FromSeconds(1.0);
	}

	public override void Update(float frameTime)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		TimeSpan curTime = _timing.CurTime;
		EntityQueryEnumerator<PassiveDamageComponent, DamageableComponent, MobStateComponent> query = ((EntitySystem)this).EntityQueryEnumerator<PassiveDamageComponent, DamageableComponent, MobStateComponent>();
		EntityUid uid = default(EntityUid);
		PassiveDamageComponent comp = default(PassiveDamageComponent);
		DamageableComponent damage = default(DamageableComponent);
		MobStateComponent mobState = default(MobStateComponent);
		while (query.MoveNext(ref uid, ref comp, ref damage, ref mobState))
		{
			if (comp.NextDamage > curTime || (comp.DamageCap != 0 && damage.TotalDamage >= comp.DamageCap))
			{
				continue;
			}
			comp.NextDamage = curTime + TimeSpan.FromSeconds(1.0);
			foreach (MobState allowedState in comp.AllowedStates)
			{
				if (allowedState == mobState.CurrentState)
				{
					_damageable.TryChangeDamage(uid, comp.Damage, ignoreResistances: true, interruptsDoAfters: false, damage);
				}
			}
		}
	}
}
