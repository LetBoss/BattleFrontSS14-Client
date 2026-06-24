using System;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Weapons.Ranged.HoloTargeting;

public sealed class RMCHoloTargetedSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HoloTargetedComponent, DamageModifyEvent>((ComponentEventRefHandler<HoloTargetedComponent, DamageModifyEvent>)OnDamageModify, (Type[])null, (Type[])null);
	}

	public void ApplyHoloStacks(EntityUid uid, float decay, float stacks, float maxStacks)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		HoloTargetedComponent holoTargeted = ((EntitySystem)this).EnsureComp<HoloTargetedComponent>(uid);
		holoTargeted.Decay = decay;
		float newStacks = holoTargeted.Stacks + stacks;
		holoTargeted.Stacks = Math.Clamp(newStacks, 0f, maxStacks);
		holoTargeted.DecayDelay = 0f;
		holoTargeted.DecayTimer = 0f;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)holoTargeted, (MetaDataComponent)null);
	}

	private void OnDamageModify(EntityUid uid, HoloTargetedComponent component, ref DamageModifyEvent args)
	{
		float damageMultiplier = 1f + component.Stacks / 1000f;
		args.Damage *= damageMultiplier;
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<HoloTargetedComponent> query = ((EntitySystem)this).EntityQueryEnumerator<HoloTargetedComponent>();
		EntityUid uid = default(EntityUid);
		HoloTargetedComponent component = default(HoloTargetedComponent);
		while (query.MoveNext(ref uid, ref component))
		{
			component.DecayDelay += frameTime;
			if (!(component.DecayDelay >= 5f))
			{
				continue;
			}
			component.DecayTimer += frameTime;
			if (component.DecayTimer >= 1f)
			{
				component.DecayTimer = 0f;
				component.Stacks -= component.Decay;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
				if (component.Stacks <= 0f)
				{
					((EntitySystem)this).RemCompDeferred<HoloTargetedComponent>(uid);
				}
			}
		}
	}
}
