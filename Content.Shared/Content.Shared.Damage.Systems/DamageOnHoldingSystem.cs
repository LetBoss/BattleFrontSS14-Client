using System;
using Content.Shared.Damage.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Damage.Systems;

public sealed class DamageOnHoldingSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private DamageableSystem _damageableSystem;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DamageOnHoldingComponent, MapInitEvent>((ComponentEventHandler<DamageOnHoldingComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
	}

	public void SetEnabled(EntityUid uid, bool enabled, DamageOnHoldingComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<DamageOnHoldingComponent>(uid, ref component, true))
		{
			component.Enabled = enabled;
			component.NextDamage = _timing.CurTime;
		}
	}

	private void OnMapInit(EntityUid uid, DamageOnHoldingComponent component, MapInitEvent args)
	{
		component.NextDamage = _timing.CurTime;
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<DamageOnHoldingComponent> query = ((EntitySystem)this).EntityQueryEnumerator<DamageOnHoldingComponent>();
		EntityUid uid = default(EntityUid);
		DamageOnHoldingComponent component = default(DamageOnHoldingComponent);
		BaseContainer container = default(BaseContainer);
		while (query.MoveNext(ref uid, ref component))
		{
			if (component.Enabled && !(component.NextDamage > _timing.CurTime))
			{
				if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(uid, null, null)), ref container))
				{
					_damageableSystem.TryChangeDamage(container.Owner, component.Damage, ignoreResistances: false, interruptsDoAfters: true, null, uid);
				}
				component.NextDamage = _timing.CurTime + TimeSpan.FromSeconds(component.Interval);
			}
		}
	}
}
