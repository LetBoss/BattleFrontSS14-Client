using System;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Destructible;
using Content.Shared.Rejuvenate;
using Content.Shared.Slippery;
using Content.Shared.StatusEffectNew;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Damage.Systems;

public abstract class SharedGodmodeSystem : EntitySystem
{
	[Dependency]
	private DamageableSystem _damageable;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GodmodeComponent, BeforeDamageChangedEvent>((ComponentEventRefHandler<GodmodeComponent, BeforeDamageChangedEvent>)OnBeforeDamageChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GodmodeComponent, BeforeStatusEffectAddedEvent>((ComponentEventRefHandler<GodmodeComponent, BeforeStatusEffectAddedEvent>)OnBeforeStatusEffect, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GodmodeComponent, BeforeStaminaDamageEvent>((ComponentEventRefHandler<GodmodeComponent, BeforeStaminaDamageEvent>)OnBeforeStaminaDamage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GodmodeComponent, SlipAttemptEvent>((ComponentEventHandler<GodmodeComponent, SlipAttemptEvent>)OnSlipAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GodmodeComponent, DestructionAttemptEvent>((EntityEventRefHandler<GodmodeComponent, DestructionAttemptEvent>)OnDestruction, (Type[])null, (Type[])null);
	}

	private void OnSlipAttempt(EntityUid uid, GodmodeComponent component, SlipAttemptEvent args)
	{
		args.NoSlip = true;
	}

	private void OnBeforeDamageChanged(EntityUid uid, GodmodeComponent component, ref BeforeDamageChangedEvent args)
	{
		args.Cancelled = true;
	}

	private void OnBeforeStatusEffect(EntityUid uid, GodmodeComponent component, ref BeforeStatusEffectAddedEvent args)
	{
		args.Cancelled = true;
	}

	private void OnBeforeStaminaDamage(EntityUid uid, GodmodeComponent component, ref BeforeStaminaDamageEvent args)
	{
		args.Cancelled = true;
	}

	private void OnDestruction(Entity<GodmodeComponent> ent, ref DestructionAttemptEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	public virtual void EnableGodmode(EntityUid uid, GodmodeComponent? godmode = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (godmode == null)
		{
			godmode = ((EntitySystem)this).EnsureComp<GodmodeComponent>(uid);
		}
		DamageableComponent damageable = default(DamageableComponent);
		if (((EntitySystem)this).TryComp<DamageableComponent>(uid, ref damageable))
		{
			godmode.OldDamage = new DamageSpecifier(damageable.Damage);
		}
		((EntitySystem)this).RaiseLocalEvent<RejuvenateEvent>(uid, new RejuvenateEvent(), false);
	}

	public virtual void DisableGodmode(EntityUid uid, GodmodeComponent? godmode = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<GodmodeComponent>(uid, ref godmode, false))
		{
			DamageableComponent damageable = default(DamageableComponent);
			if (((EntitySystem)this).TryComp<DamageableComponent>(uid, ref damageable) && godmode.OldDamage != null)
			{
				_damageable.SetDamage(uid, damageable, godmode.OldDamage);
			}
			((EntitySystem)this).RemComp<GodmodeComponent>(uid);
		}
	}

	public bool ToggleGodmode(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		GodmodeComponent godmode = default(GodmodeComponent);
		if (((EntitySystem)this).TryComp<GodmodeComponent>(uid, ref godmode))
		{
			DisableGodmode(uid, godmode);
			return false;
		}
		EnableGodmode(uid, godmode);
		return true;
	}
}
