using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Bed.Sleep;
using Content.Shared.Buckle.Components;
using Content.Shared.CombatMode.Pacification;
using Content.Shared.Damage;
using Content.Shared.Damage.ForceSay;
using Content.Shared.Emoting;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Pointing;
using Content.Shared.Pulling.Events;
using Content.Shared.Speech;
using Content.Shared.Standing;
using Content.Shared.Strip.Components;
using Content.Shared.Throwing;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Timing;

namespace Content.Shared.Mobs.Systems;

[Virtual]
public class MobStateSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _blocker;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private ILogManager _logManager;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private DamageableSystem _damageable;

	private ISawmill _sawmill;

	private EntityQuery<MobStateComponent> _mobStateQuery;

	public override void Initialize()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_sawmill = _logManager.GetSawmill("MobState");
		_mobStateQuery = ((EntitySystem)this).GetEntityQuery<MobStateComponent>();
		((EntitySystem)this).Initialize();
		SubscribeEvents();
	}

	public bool IsAlive(EntityUid target, MobStateComponent? component = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (!_mobStateQuery.Resolve(target, ref component, false))
		{
			return false;
		}
		return component.CurrentState == MobState.Alive;
	}

	public bool IsCritical(EntityUid target, MobStateComponent? component = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (!_mobStateQuery.Resolve(target, ref component, false))
		{
			return false;
		}
		return component.CurrentState == MobState.Critical;
	}

	public bool IsDead(EntityUid target, MobStateComponent? component = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (!_mobStateQuery.Resolve(target, ref component, false))
		{
			return false;
		}
		return component.CurrentState == MobState.Dead;
	}

	public bool IsIncapacitated(EntityUid target, MobStateComponent? component = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (!_mobStateQuery.Resolve(target, ref component, false))
		{
			return false;
		}
		MobState currentState = component.CurrentState;
		if (currentState - 2 <= MobState.Alive)
		{
			return true;
		}
		return false;
	}

	public bool IsInvalidState(EntityUid target, MobStateComponent? component = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (!_mobStateQuery.Resolve(target, ref component, false))
		{
			return false;
		}
		return component.CurrentState == MobState.Invalid;
	}

	public bool HasState(EntityUid entity, MobState mobState, MobStateComponent? component = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (_mobStateQuery.Resolve(entity, ref component, false))
		{
			return component.AllowedStates.Contains(mobState);
		}
		return false;
	}

	public void UpdateMobState(EntityUid entity, MobStateComponent? component = null, EntityUid? origin = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (_mobStateQuery.Resolve(entity, ref component, true))
		{
			UpdateMobStateEvent ev = new UpdateMobStateEvent
			{
				Target = entity,
				Component = component,
				Origin = origin
			};
			((EntitySystem)this).RaiseLocalEvent<UpdateMobStateEvent>(entity, ref ev, false);
			ChangeState(entity, component, ev.State, origin);
		}
	}

	public void ChangeMobState(EntityUid entity, MobState mobState, MobStateComponent? component = null, EntityUid? origin = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (_mobStateQuery.Resolve(entity, ref component, true))
		{
			ChangeState(entity, component, mobState, origin);
		}
	}

	protected virtual void OnEnterState(EntityUid entity, MobStateComponent component, MobState state)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnStateEnteredSubscribers(entity, component, state);
	}

	protected virtual void OnStateChanged(EntityUid entity, MobStateComponent component, MobState oldState, MobState newState)
	{
	}

	protected virtual void OnExitState(EntityUid entity, MobStateComponent component, MobState state)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnStateExitSubscribers(entity, component, state);
	}

	private void ChangeState(EntityUid target, MobStateComponent component, MobState newState, EntityUid? origin = null)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		MobState oldState = component.CurrentState;
		if (oldState != newState && component.AllowedStates.Contains(newState))
		{
			OnExitState(target, component, oldState);
			component.CurrentState = newState;
			OnEnterState(target, component, newState);
			MobStateChangedEvent ev = new MobStateChangedEvent(target, component, oldState, newState, origin);
			OnStateChanged(target, component, oldState, newState);
			((EntitySystem)this).RaiseLocalEvent<MobStateChangedEvent>(target, ev, true);
			((EntitySystem)this).Dirty(target, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	private void SubscribeEvents()
	{
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, BeforeGettingStrippedEvent>((ComponentEventHandler<MobStateComponent, BeforeGettingStrippedEvent>)OnGettingStripped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, ChangeDirectionAttemptEvent>((ComponentEventHandler<MobStateComponent, ChangeDirectionAttemptEvent>)CheckAct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, UseAttemptEvent>((ComponentEventHandler<MobStateComponent, UseAttemptEvent>)CheckAct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, AttackAttemptEvent>((ComponentEventHandler<MobStateComponent, AttackAttemptEvent>)CheckAct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, ConsciousAttemptEvent>((EntityEventRefHandler<MobStateComponent, ConsciousAttemptEvent>)CheckConcious, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, ThrowAttemptEvent>((ComponentEventHandler<MobStateComponent, ThrowAttemptEvent>)CheckAct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, SpeakAttemptEvent>((ComponentEventHandler<MobStateComponent, SpeakAttemptEvent>)OnSpeakAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, IsEquippingAttemptEvent>((ComponentEventHandler<MobStateComponent, IsEquippingAttemptEvent>)OnEquipAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, EmoteAttemptEvent>((ComponentEventHandler<MobStateComponent, EmoteAttemptEvent>)CheckAct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, IsUnequippingAttemptEvent>((ComponentEventHandler<MobStateComponent, IsUnequippingAttemptEvent>)OnUnequipAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, DropAttemptEvent>((ComponentEventHandler<MobStateComponent, DropAttemptEvent>)CheckAct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, PickupAttemptEvent>((ComponentEventHandler<MobStateComponent, PickupAttemptEvent>)CheckAct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, StartPullAttemptEvent>((ComponentEventHandler<MobStateComponent, StartPullAttemptEvent>)CheckAct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, UpdateCanMoveEvent>((ComponentEventHandler<MobStateComponent, UpdateCanMoveEvent>)CheckAct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, StandAttemptEvent>((ComponentEventHandler<MobStateComponent, StandAttemptEvent>)CheckAct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, PointAttemptEvent>((ComponentEventHandler<MobStateComponent, PointAttemptEvent>)CheckAct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, TryingToSleepEvent>((ComponentEventRefHandler<MobStateComponent, TryingToSleepEvent>)OnSleepAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, CombatModeShouldHandInteractEvent>((ComponentEventRefHandler<MobStateComponent, CombatModeShouldHandInteractEvent>)OnCombatModeShouldHandInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, AttemptPacifiedAttackEvent>((EntityEventRefHandler<MobStateComponent, AttemptPacifiedAttackEvent>)OnAttemptPacifiedAttack, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, DamageModifyEvent>((EntityEventRefHandler<MobStateComponent, DamageModifyEvent>)OnDamageModify, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, UnbuckleAttemptEvent>((EntityEventRefHandler<MobStateComponent, UnbuckleAttemptEvent>)OnUnbuckleAttempt, (Type[])null, (Type[])null);
	}

	private void OnUnbuckleAttempt(Entity<MobStateComponent> ent, ref UnbuckleAttemptEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? user = args.User;
		EntityUid owner = ent.Owner;
		if (user.HasValue && user.GetValueOrDefault() == owner && IsIncapacitated(Entity<MobStateComponent>.op_Implicit(ent)))
		{
			args.Cancelled = true;
		}
	}

	private void CheckConcious(Entity<MobStateComponent> ent, ref ConsciousAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		MobState currentState = ent.Comp.CurrentState;
		if (currentState - 2 <= MobState.Alive)
		{
			args.Cancelled = true;
		}
	}

	private void OnStateExitSubscribers(EntityUid target, MobStateComponent component, MobState state)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		switch (state)
		{
		case MobState.Critical:
			_standing.Stand(target);
			break;
		case MobState.Dead:
			((EntitySystem)this).RemComp<CollisionWakeComponent>(target);
			_standing.Stand(target);
			break;
		default:
			throw new NotImplementedException();
		case MobState.Invalid:
		case MobState.Alive:
			break;
		}
	}

	private void OnStateEnteredSubscribers(EntityUid target, MobStateComponent component, MobState state)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			_blocker.UpdateCanMove(target);
			switch (state)
			{
			case MobState.Alive:
				_standing.Stand(target);
				_appearance.SetData(target, (Enum)MobStateVisuals.State, (object)MobState.Alive, (AppearanceComponent)null);
				break;
			case MobState.Critical:
				_standing.Down(target);
				_appearance.SetData(target, (Enum)MobStateVisuals.State, (object)MobState.Critical, (AppearanceComponent)null);
				break;
			case MobState.Dead:
				((EntitySystem)this).EnsureComp<CollisionWakeComponent>(target);
				_standing.Down(target);
				_appearance.SetData(target, (Enum)MobStateVisuals.State, (object)MobState.Dead, (AppearanceComponent)null);
				break;
			default:
				throw new NotImplementedException();
			case MobState.Invalid:
				break;
			}
		}
	}

	private void OnSleepAttempt(EntityUid target, MobStateComponent component, ref TryingToSleepEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (IsDead(target, component))
		{
			args.Cancelled = true;
		}
	}

	private void OnGettingStripped(EntityUid target, MobStateComponent component, BeforeGettingStrippedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (IsDead(target, component))
		{
			args.Multiplier /= 3f;
		}
		else if (IsCritical(target, component))
		{
			args.Multiplier /= 2f;
		}
	}

	private void OnSpeakAttempt(EntityUid uid, MobStateComponent component, SpeakAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<AllowNextCritSpeechComponent>(uid))
		{
			((EntitySystem)this).RemCompDeferred<AllowNextCritSpeechComponent>(uid);
		}
		else
		{
			CheckAct(uid, component, (CancellableEntityEventArgs)(object)args);
		}
	}

	private void CheckAct(EntityUid target, MobStateComponent component, CancellableEntityEventArgs args)
	{
		MobState currentState = component.CurrentState;
		if (currentState - 2 <= MobState.Alive)
		{
			args.Cancel();
		}
	}

	private void OnEquipAttempt(EntityUid target, MobStateComponent component, IsEquippingAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Equipee == target)
		{
			CheckAct(target, component, (CancellableEntityEventArgs)(object)args);
		}
	}

	private void OnUnequipAttempt(EntityUid target, MobStateComponent component, IsUnequippingAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Unequipee == target)
		{
			CheckAct(target, component, (CancellableEntityEventArgs)(object)args);
		}
	}

	private void OnCombatModeShouldHandInteract(EntityUid uid, MobStateComponent component, ref CombatModeShouldHandInteractEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!IsDead(uid, component))
		{
			args.Cancelled = true;
		}
	}

	private void OnAttemptPacifiedAttack(Entity<MobStateComponent> ent, ref AttemptPacifiedAttackEvent args)
	{
		args.Cancelled = true;
	}

	private void OnDamageModify(Entity<MobStateComponent> ent, ref DamageModifyEvent args)
	{
		args.Damage *= _damageable.UniversalMobDamageModifier;
	}
}
