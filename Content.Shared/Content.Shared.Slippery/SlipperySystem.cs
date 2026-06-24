using System;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Inventory;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.StatusEffect;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Slippery;

public sealed class SlipperySystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private StatusEffectsSystem _statusEffects;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SpeedModifierContactsSystem _speedModifier;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SlipperyComponent, StepTriggerAttemptEvent>((ComponentEventRefHandler<SlipperyComponent, StepTriggerAttemptEvent>)HandleAttemptCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlipperyComponent, StepTriggeredOffEvent>((ComponentEventRefHandler<SlipperyComponent, StepTriggeredOffEvent>)HandleStepTrigger, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NoSlipComponent, SlipAttemptEvent>((ComponentEventHandler<NoSlipComponent, SlipAttemptEvent>)OnNoSlipAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlowedOverSlipperyComponent, SlipAttemptEvent>((ComponentEventHandler<SlowedOverSlipperyComponent, SlipAttemptEvent>)OnSlowedOverSlipAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThrownItemComponent, SlipCausingAttemptEvent>((ComponentEventRefHandler<ThrownItemComponent, SlipCausingAttemptEvent>)OnThrownSlipAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NoSlipComponent, InventoryRelayedEvent<SlipAttemptEvent>>((ComponentEventHandler<NoSlipComponent, InventoryRelayedEvent<SlipAttemptEvent>>)delegate(EntityUid e, NoSlipComponent c, InventoryRelayedEvent<SlipAttemptEvent> ev)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			OnNoSlipAttempt(e, c, ev.Args);
		}, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlowedOverSlipperyComponent, InventoryRelayedEvent<SlipAttemptEvent>>((ComponentEventHandler<SlowedOverSlipperyComponent, InventoryRelayedEvent<SlipAttemptEvent>>)delegate(EntityUid e, SlowedOverSlipperyComponent c, InventoryRelayedEvent<SlipAttemptEvent> ev)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			OnSlowedOverSlipAttempt(e, c, ev.Args);
		}, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlowedOverSlipperyComponent, InventoryRelayedEvent<GetSlowedOverSlipperyModifierEvent>>((ComponentEventRefHandler<SlowedOverSlipperyComponent, InventoryRelayedEvent<GetSlowedOverSlipperyModifierEvent>>)OnGetSlowedOverSlipperyModifier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlipperyComponent, EndCollideEvent>((ComponentEventRefHandler<SlipperyComponent, EndCollideEvent>)OnEntityExit, (Type[])null, (Type[])null);
	}

	private void HandleStepTrigger(EntityUid uid, SlipperyComponent component, ref StepTriggeredOffEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		TrySlip(uid, component, args.Tripper);
	}

	private void HandleAttemptCollide(EntityUid uid, SlipperyComponent component, ref StepTriggerAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		args.Continue |= CanSlip(uid, args.Tripper);
	}

	private static void OnNoSlipAttempt(EntityUid uid, NoSlipComponent component, SlipAttemptEvent args)
	{
		args.NoSlip = true;
	}

	private void OnSlowedOverSlipAttempt(EntityUid uid, SlowedOverSlipperyComponent component, SlipAttemptEvent args)
	{
		args.SlowOverSlippery = true;
	}

	private void OnThrownSlipAttempt(EntityUid uid, ThrownItemComponent comp, ref SlipCausingAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnGetSlowedOverSlipperyModifier(EntityUid uid, SlowedOverSlipperyComponent comp, ref InventoryRelayedEvent<GetSlowedOverSlipperyModifierEvent> args)
	{
		args.Args.SlowdownModifier *= comp.SlowdownModifier;
	}

	private void OnEntityExit(EntityUid uid, SlipperyComponent component, ref EndCollideEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<SpeedModifiedByContactComponent>(args.OtherEntity))
		{
			_speedModifier.AddModifiedEntity(args.OtherEntity);
		}
	}

	private bool CanSlip(EntityUid uid, EntityUid toSlip)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!_container.IsEntityInContainer(uid, (MetaDataComponent)null))
		{
			return _statusEffects.CanApplyEffect(toSlip, "Stun");
		}
		return false;
	}

	public void TrySlip(EntityUid uid, SlipperyComponent component, EntityUid other, bool requiresContact = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<KnockedDownComponent>(other) && !component.SlipData.SuperSlippery)
		{
			return;
		}
		SlipAttemptEvent attemptEv = new SlipAttemptEvent(uid);
		((EntitySystem)this).RaiseLocalEvent<SlipAttemptEvent>(other, attemptEv, false);
		if (attemptEv.SlowOverSlippery)
		{
			_speedModifier.AddModifiedEntity(other);
		}
		if (attemptEv.NoSlip)
		{
			return;
		}
		SlipCausingAttemptEvent attemptCausingEv = default(SlipCausingAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<SlipCausingAttemptEvent>(uid, ref attemptCausingEv, false);
		if (attemptCausingEv.Cancelled)
		{
			return;
		}
		SlipEvent ev = new SlipEvent(other);
		((EntitySystem)this).RaiseLocalEvent<SlipEvent>(uid, ref ev, false);
		PhysicsComponent physics = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(other, ref physics) && !((EntitySystem)this).HasComp<SlidingComponent>(other))
		{
			_physics.SetLinearVelocity(other, physics.LinearVelocity * component.SlipData.LaunchForwardsMultiplier, true, true, (FixturesComponent)null, physics);
			if (component.SlipData.SuperSlippery && requiresContact)
			{
				((EntitySystem)this).EnsureComp<SlidingComponent>(other).CollidingEntities.Add(uid);
			}
		}
		bool num = !_statusEffects.HasStatusEffect(other, "KnockedDown");
		_stun.TryParalyze(other, component.SlipData.ParalyzeTime, refresh: true);
		if (num)
		{
			_audio.PlayPredicted(component.SlipSound, other, (EntityUid?)other, (AudioParams?)null);
		}
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(27, 2);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(other)), "mob", "ToPrettyString(other)");
		handler.AppendLiteral(" slipped on collision with ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "entity", "ToPrettyString(uid)");
		adminLogger.Add(LogType.Slip, LogImpact.Low, ref handler);
	}
}
