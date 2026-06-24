using System;
using Content.Shared.Damage.Components;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Throwing;

public sealed class RMCThrowingSystem : EntitySystem
{
	[Dependency]
	private ThrownItemSystem _thrown;

	private EntityQuery<ThrownItemComponent> _thrownItemQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_thrownItemQuery = ((EntitySystem)this).GetEntityQuery<ThrownItemComponent>();
		((EntitySystem)this).SubscribeLocalEvent<DamageOtherOnHitComponent, ThrownEvent>((EntityEventRefHandler<DamageOtherOnHitComponent, ThrownEvent>)OnDamageOtherOnHitThrown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThrownLimitHitsComponent, ThrowDoHitEvent>((EntityEventRefHandler<ThrownLimitHitsComponent, ThrowDoHitEvent>)OnThrownLimitHitsDoHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThrownLimitHitsComponent, LandEvent>((EntityEventRefHandler<ThrownLimitHitsComponent, LandEvent>)OnThrownLimitHitsLand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThrownLimitHitsComponent, StopThrowEvent>((EntityEventRefHandler<ThrownLimitHitsComponent, StopThrowEvent>)OnThrownLimitHitsStopThrow, (Type[])null, (Type[])null);
	}

	private void OnDamageOtherOnHitThrown(Entity<DamageOtherOnHitComponent> ent, ref ThrownEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		ThrownLimitHitsComponent limit = ((EntitySystem)this).EnsureComp<ThrownLimitHitsComponent>(Entity<DamageOtherOnHitComponent>.op_Implicit(ent));
		limit.Hit = false;
		((EntitySystem)this).Dirty(Entity<DamageOtherOnHitComponent>.op_Implicit(ent), (IComponent)(object)limit, (MetaDataComponent)null);
	}

	private void OnThrownLimitHitsLand(Entity<ThrownLimitHitsComponent> ent, ref LandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Hit = false;
		((EntitySystem)this).Dirty<ThrownLimitHitsComponent>(ent, (MetaDataComponent)null);
	}

	private void OnThrownLimitHitsDoHit(Entity<ThrownLimitHitsComponent> ent, ref ThrowDoHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Hit = true;
		((EntitySystem)this).Dirty<ThrownLimitHitsComponent>(ent, (MetaDataComponent)null);
		ThrownItemComponent thrown = default(ThrownItemComponent);
		if (_thrownItemQuery.TryComp(Entity<ThrownLimitHitsComponent>.op_Implicit(ent), ref thrown))
		{
			_thrown.StopThrow(Entity<ThrownLimitHitsComponent>.op_Implicit(ent), thrown);
		}
	}

	private void OnThrownLimitHitsStopThrow(Entity<ThrownLimitHitsComponent> ent, ref StopThrowEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<ThrownLimitHitsComponent>(Entity<ThrownLimitHitsComponent>.op_Implicit(ent));
	}
}
