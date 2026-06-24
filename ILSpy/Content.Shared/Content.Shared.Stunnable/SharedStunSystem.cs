using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Bed.Sleep;
using Content.Shared.Damage.Components;
using Content.Shared.Database;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Throwing;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Events;

namespace Content.Shared.Stunnable;

public abstract class SharedStunSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _blocker;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeedModifier;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private StandingStateSystem _standingState;

	[Dependency]
	private StatusEffectsSystem _statusEffect;

	public const float KnockDownModifier = 0.2f;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<KnockedDownComponent, ComponentInit>((ComponentEventHandler<KnockedDownComponent, ComponentInit>)OnKnockInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<KnockedDownComponent, ComponentShutdown>((ComponentEventHandler<KnockedDownComponent, ComponentShutdown>)OnKnockShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<KnockedDownComponent, StandAttemptEvent>((ComponentEventHandler<KnockedDownComponent, StandAttemptEvent>)OnStandAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlowedDownComponent, ComponentInit>((ComponentEventHandler<SlowedDownComponent, ComponentInit>)OnSlowInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlowedDownComponent, ComponentShutdown>((ComponentEventHandler<SlowedDownComponent, ComponentShutdown>)OnSlowRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunnedComponent, ComponentStartup>((ComponentEventHandler<StunnedComponent, ComponentStartup>)UpdateCanMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunnedComponent, ComponentShutdown>((ComponentEventHandler<StunnedComponent, ComponentShutdown>)UpdateCanMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunOnContactComponent, StartCollideEvent>((EntityEventRefHandler<StunOnContactComponent, StartCollideEvent>)OnStunOnContactCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<KnockedDownComponent, InteractHandEvent>((ComponentEventHandler<KnockedDownComponent, InteractHandEvent>)OnInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlowedDownComponent, RefreshMovementSpeedModifiersEvent>((ComponentEventHandler<SlowedDownComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshMovespeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<KnockedDownComponent, TileFrictionEvent>((ComponentEventRefHandler<KnockedDownComponent, TileFrictionEvent>)OnKnockedTileFriction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunnedComponent, ChangeDirectionAttemptEvent>((ComponentEventHandler<StunnedComponent, ChangeDirectionAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunnedComponent, UpdateCanMoveEvent>((ComponentEventHandler<StunnedComponent, UpdateCanMoveEvent>)OnMoveAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunnedComponent, InteractionAttemptEvent>((EntityEventRefHandler<StunnedComponent, InteractionAttemptEvent>)OnAttemptInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunnedComponent, UseAttemptEvent>((ComponentEventHandler<StunnedComponent, UseAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunnedComponent, ThrowAttemptEvent>((ComponentEventHandler<StunnedComponent, ThrowAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunnedComponent, DropAttemptEvent>((ComponentEventHandler<StunnedComponent, DropAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunnedComponent, AttackAttemptEvent>((ComponentEventHandler<StunnedComponent, AttackAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunnedComponent, PickupAttemptEvent>((ComponentEventHandler<StunnedComponent, PickupAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunnedComponent, IsEquippingAttemptEvent>((ComponentEventHandler<StunnedComponent, IsEquippingAttemptEvent>)OnEquipAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunnedComponent, IsUnequippingAttemptEvent>((ComponentEventHandler<StunnedComponent, IsUnequippingAttemptEvent>)OnUnequipAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, MobStateChangedEvent>((ComponentEventHandler<MobStateComponent, MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
	}

	private void OnAttemptInteract(Entity<StunnedComponent> ent, ref InteractionAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnMobStateChanged(EntityUid uid, MobStateComponent component, MobStateChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		StatusEffectsComponent status = default(StatusEffectsComponent);
		if (((EntitySystem)this).TryComp<StatusEffectsComponent>(uid, ref status))
		{
			switch (args.NewMobState)
			{
			case MobState.Critical:
				_statusEffect.TryRemoveStatusEffect(uid, "Stun");
				break;
			case MobState.Dead:
				_statusEffect.TryRemoveStatusEffect(uid, "Stun");
				break;
			case MobState.Invalid:
				break;
			case MobState.Alive:
				break;
			}
		}
	}

	private void UpdateCanMove(EntityUid uid, StunnedComponent component, EntityEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_blocker.UpdateCanMove(uid);
	}

	private void OnStunOnContactCollide(Entity<StunOnContactComponent> ent, ref StartCollideEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		StatusEffectsComponent status = default(StatusEffectsComponent);
		if (!(args.OurFixtureId != ent.Comp.FixtureId) && !_entityWhitelist.IsBlacklistPass(ent.Comp.Blacklist, args.OtherEntity) && ((EntitySystem)this).TryComp<StatusEffectsComponent>(args.OtherEntity, ref status))
		{
			TryStun(args.OtherEntity, ent.Comp.Duration, refresh: true, status);
			TryKnockdown(args.OtherEntity, ent.Comp.Duration, refresh: true, status);
		}
	}

	private void OnKnockInit(EntityUid uid, KnockedDownComponent component, ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_standingState.Down(uid);
	}

	private void OnKnockShutdown(EntityUid uid, KnockedDownComponent component, ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_standingState.Stand(uid);
	}

	private void OnStandAttempt(EntityUid uid, KnockedDownComponent component, StandAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)((Component)component).LifeStage <= 6)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnSlowInit(EntityUid uid, SlowedDownComponent component, ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeedModifier.RefreshMovementSpeedModifiers(uid);
	}

	private void OnSlowRemove(EntityUid uid, SlowedDownComponent component, ComponentShutdown args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		component.SprintSpeedModifier = 1f;
		component.WalkSpeedModifier = 1f;
		_movementSpeedModifier.RefreshMovementSpeedModifiers(uid);
	}

	private void OnRefreshMovespeed(EntityUid uid, SlowedDownComponent component, RefreshMovementSpeedModifiersEvent args)
	{
		args.ModifySpeed(component.WalkSpeedModifier, component.SprintSpeedModifier);
	}

	public bool TryStun(EntityUid uid, TimeSpan time, bool refresh, StatusEffectsComponent? status = null, bool force = false)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (time <= TimeSpan.Zero)
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		if (!_statusEffect.TryAddStatusEffect<StunnedComponent>(uid, "Stun", time, refresh, (StatusEffectsComponent?)null, force))
		{
			return false;
		}
		StunnedEvent ev = default(StunnedEvent);
		((EntitySystem)this).RaiseLocalEvent<StunnedEvent>(uid, ref ev, false);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(21, 2);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "user", "ToPrettyString(uid)");
		handler.AppendLiteral(" stunned for ");
		handler.AppendFormatted(time.Seconds, "time.Seconds");
		handler.AppendLiteral(" seconds");
		adminLogger.Add(LogType.Stamina, LogImpact.Medium, ref handler);
		return true;
	}

	public bool TryKnockdown(EntityUid uid, TimeSpan time, bool refresh, StatusEffectsComponent? status = null, bool force = false)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (time <= TimeSpan.Zero)
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		if (!_statusEffect.TryAddStatusEffect<KnockedDownComponent>(uid, "KnockedDown", time, refresh, (StatusEffectsComponent?)null, force))
		{
			return false;
		}
		KnockedDownEvent ev = default(KnockedDownEvent);
		((EntitySystem)this).RaiseLocalEvent<KnockedDownEvent>(uid, ref ev, false);
		return true;
	}

	public bool TryParalyze(EntityUid uid, TimeSpan time, bool refresh, StatusEffectsComponent? status = null, bool force = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		if (TryKnockdown(uid, time, refresh, status, force))
		{
			return TryStun(uid, time, refresh, status, force);
		}
		return false;
	}

	public bool TrySlowdown(EntityUid uid, TimeSpan time, bool refresh, float walkSpeedMultiplier = 1f, float runSpeedMultiplier = 1f, StatusEffectsComponent? status = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		if (time <= TimeSpan.Zero)
		{
			return false;
		}
		if (_statusEffect.TryAddStatusEffect<SlowedDownComponent>(uid, "SlowedDown", time, refresh, status, false))
		{
			SlowedDownComponent slowedDownComponent = ((EntitySystem)this).Comp<SlowedDownComponent>(uid);
			walkSpeedMultiplier = Math.Clamp(walkSpeedMultiplier, 0f, 1f);
			runSpeedMultiplier = Math.Clamp(runSpeedMultiplier, 0f, 1f);
			slowedDownComponent.WalkSpeedModifier *= walkSpeedMultiplier;
			slowedDownComponent.SprintSpeedModifier *= runSpeedMultiplier;
			_movementSpeedModifier.RefreshMovementSpeedModifiers(uid);
			return true;
		}
		return false;
	}

	public void UpdateStunModifiers(Entity<StaminaComponent?> ent, float walkSpeedModifier = 1f, float runSpeedModifier = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StaminaComponent>(Entity<StaminaComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			if ((MathHelper.CloseTo(walkSpeedModifier, 1f, 1E-07f) && MathHelper.CloseTo(runSpeedModifier, 1f, 1E-07f) && ent.Comp.StaminaDamage == 0f) || (walkSpeedModifier == 0f && runSpeedModifier == 0f))
			{
				((EntitySystem)this).RemComp<SlowedDownComponent>(Entity<StaminaComponent>.op_Implicit(ent));
				return;
			}
			SlowedDownComponent comp = default(SlowedDownComponent);
			((EntitySystem)this).EnsureComp<SlowedDownComponent>(Entity<StaminaComponent>.op_Implicit(ent), ref comp);
			comp.WalkSpeedModifier = walkSpeedModifier;
			comp.SprintSpeedModifier = runSpeedModifier;
			_movementSpeedModifier.RefreshMovementSpeedModifiers(Entity<StaminaComponent>.op_Implicit(ent));
			((EntitySystem)this).Dirty<StaminaComponent>(ent, (MetaDataComponent)null);
		}
	}

	public void UpdateStunModifiers(Entity<StaminaComponent?> ent, float speedModifier = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateStunModifiers(ent, speedModifier, speedModifier);
	}

	private void OnInteractHand(EntityUid uid, KnockedDownComponent knocked, InteractHandEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !(knocked.HelpTimer > 0f) && !((EntitySystem)this).HasComp<SleepingComponent>(uid))
		{
			knocked.HelpTimer = knocked.HelpInterval / 2f;
			_statusEffect.TryRemoveTime(uid, "KnockedDown", TimeSpan.FromSeconds(knocked.HelpInterval));
			_audio.PlayPredicted(knocked.StunAttemptSound, uid, (EntityUid?)args.User, (AudioParams?)null);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)knocked, (MetaDataComponent)null);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnKnockedTileFriction(EntityUid uid, KnockedDownComponent component, ref TileFrictionEvent args)
	{
	}

	private void OnMoveAttempt(EntityUid uid, StunnedComponent stunned, UpdateCanMoveEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)((Component)stunned).LifeStage <= 6)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnAttempt(EntityUid uid, StunnedComponent stunned, CancellableEntityEventArgs args)
	{
		args.Cancel();
	}

	private void OnEquipAttempt(EntityUid uid, StunnedComponent stunned, IsEquippingAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (args.Equipee == uid)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnUnequipAttempt(EntityUid uid, StunnedComponent stunned, IsUnequippingAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (args.Unequipee == uid)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
