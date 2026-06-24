using System;
using Content.Shared.Damage;
using Content.Shared.Flash.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Flash;

public sealed class DamagedByFlashingSystem : EntitySystem
{
	[Dependency]
	private DamageableSystem _damageable;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DamagedByFlashingComponent, FlashAttemptEvent>((EntityEventRefHandler<DamagedByFlashingComponent, FlashAttemptEvent>)OnFlashAttempt, (Type[])null, (Type[])null);
	}

	private void OnFlashAttempt(Entity<DamagedByFlashingComponent> ent, ref FlashAttemptEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_damageable.TryChangeDamage(Entity<DamagedByFlashingComponent>.op_Implicit(ent), ent.Comp.FlashDamage);
	}
}
