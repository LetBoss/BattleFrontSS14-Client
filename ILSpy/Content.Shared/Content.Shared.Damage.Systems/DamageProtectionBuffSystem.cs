using System;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.GameObjects;

namespace Content.Shared.Damage.Systems;

public sealed class DamageProtectionBuffSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DamageProtectionBuffComponent, DamageModifyEvent>((ComponentEventHandler<DamageProtectionBuffComponent, DamageModifyEvent>)OnDamageModify, (Type[])null, (Type[])null);
	}

	private void OnDamageModify(EntityUid uid, DamageProtectionBuffComponent component, DamageModifyEvent args)
	{
		foreach (DamageModifierSetPrototype modifier in component.Modifiers.Values)
		{
			args.Damage = DamageSpecifier.ApplyModifierSet(args.Damage, modifier);
		}
	}
}
