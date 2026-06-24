using System;
using Content.Shared._RMC14.Hands;
using Content.Shared.Body.Events;
using Content.Shared.Emoting;
using Content.Shared.Hands;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Speech;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.ActionBlocker;

public sealed class ActionBlockerSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	private EntityQuery<ComplexInteractionComponent> _complexInteractionQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_complexInteractionQuery = ((EntitySystem)this).GetEntityQuery<ComplexInteractionComponent>();
		((EntitySystem)this).SubscribeLocalEvent<InputMoverComponent, ComponentStartup>((ComponentEventHandler<InputMoverComponent, ComponentStartup>)OnMoverStartup, (Type[])null, (Type[])null);
	}

	private void OnMoverStartup(EntityUid uid, InputMoverComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateCanMove(uid, component);
	}

	public bool CanMove(EntityUid uid, InputMoverComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<InputMoverComponent>(uid, ref component, false))
		{
			return component.CanMove;
		}
		return false;
	}

	public bool UpdateCanMove(EntityUid uid, InputMoverComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<InputMoverComponent>(uid, ref component, false))
		{
			return false;
		}
		UpdateCanMoveEvent ev = new UpdateCanMoveEvent(uid);
		((EntitySystem)this).RaiseLocalEvent<UpdateCanMoveEvent>(uid, ev, false);
		if (component.CanMove == ((CancellableEntityEventArgs)ev).Cancelled)
		{
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
		component.CanMove = !((CancellableEntityEventArgs)ev).Cancelled;
		return !((CancellableEntityEventArgs)ev).Cancelled;
	}

	public bool CanComplexInteract(EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _complexInteractionQuery.HasComp(user);
	}

	public bool CanInteract(EntityUid user, EntityUid? target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (!CanConsciouslyPerformAction(user))
		{
			return false;
		}
		InteractionAttemptEvent ev = new InteractionAttemptEvent(user, target);
		((EntitySystem)this).RaiseLocalEvent<InteractionAttemptEvent>(user, ref ev, false);
		if (ev.Cancelled)
		{
			return false;
		}
		if (target.HasValue)
		{
			EntityUid? val = target;
			if (!val.HasValue || !(val.GetValueOrDefault() == user))
			{
				GettingInteractedWithAttemptEvent targetEv = new GettingInteractedWithAttemptEvent(user, target);
				((EntitySystem)this).RaiseLocalEvent<GettingInteractedWithAttemptEvent>(target.Value, ref targetEv, false);
				return !targetEv.Cancelled;
			}
		}
		return true;
	}

	public bool CanUseHeldEntity(EntityUid user, EntityUid used)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		UseAttemptEvent useEv = new UseAttemptEvent(user, used);
		((EntitySystem)this).RaiseLocalEvent<UseAttemptEvent>(user, useEv, false);
		if (((CancellableEntityEventArgs)useEv).Cancelled)
		{
			return false;
		}
		GettingUsedAttemptEvent usedEv = new GettingUsedAttemptEvent(user);
		((EntitySystem)this).RaiseLocalEvent<GettingUsedAttemptEvent>(used, usedEv, false);
		return !((CancellableEntityEventArgs)usedEv).Cancelled;
	}

	public bool CanConsciouslyPerformAction(EntityUid user)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		ConsciousAttemptEvent ev = new ConsciousAttemptEvent(user);
		((EntitySystem)this).RaiseLocalEvent<ConsciousAttemptEvent>(user, ref ev, false);
		return !ev.Cancelled;
	}

	public bool CanThrow(EntityUid user, EntityUid itemUid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		ThrowAttemptEvent ev = new ThrowAttemptEvent(user, itemUid);
		((EntitySystem)this).RaiseLocalEvent<ThrowAttemptEvent>(user, ev, false);
		if (((CancellableEntityEventArgs)ev).Cancelled)
		{
			return false;
		}
		ThrowItemAttemptEvent itemEv = new ThrowItemAttemptEvent(user);
		((EntitySystem)this).RaiseLocalEvent<ThrowItemAttemptEvent>(itemUid, ref itemEv, false);
		return !itemEv.Cancelled;
	}

	public bool CanSpeak(EntityUid uid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		SpeakAttemptEvent ev = new SpeakAttemptEvent(uid);
		((EntitySystem)this).RaiseLocalEvent<SpeakAttemptEvent>(uid, ev, true);
		return !((CancellableEntityEventArgs)ev).Cancelled;
	}

	public bool CanDrop(EntityUid uid, EntityUid? held = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		DropAttemptEvent ev = new DropAttemptEvent();
		((EntitySystem)this).RaiseLocalEvent<DropAttemptEvent>(uid, ev, false);
		if (held.HasValue)
		{
			RMCItemDropAttemptEvent rmcEv = default(RMCItemDropAttemptEvent);
			((EntitySystem)this).RaiseLocalEvent<RMCItemDropAttemptEvent>(held.Value, ref rmcEv, false);
			if (rmcEv.Cancelled)
			{
				return false;
			}
		}
		return !((CancellableEntityEventArgs)ev).Cancelled;
	}

	public bool CanPickup(EntityUid user, EntityUid item)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		PickupAttemptEvent userEv = new PickupAttemptEvent(user, item);
		((EntitySystem)this).RaiseLocalEvent<PickupAttemptEvent>(user, userEv, false);
		if (((CancellableEntityEventArgs)userEv).Cancelled)
		{
			return false;
		}
		GettingPickedUpAttemptEvent itemEv = new GettingPickedUpAttemptEvent(user, item);
		((EntitySystem)this).RaiseLocalEvent<GettingPickedUpAttemptEvent>(item, itemEv, false);
		return !((CancellableEntityEventArgs)itemEv).Cancelled;
	}

	public bool CanEmote(EntityUid uid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		EmoteAttemptEvent ev = new EmoteAttemptEvent(uid);
		((EntitySystem)this).RaiseLocalEvent<EmoteAttemptEvent>(uid, ev, true);
		return !((CancellableEntityEventArgs)ev).Cancelled;
	}

	public bool CanAttack(EntityUid uid, EntityUid? target = null, Entity<MeleeWeaponComponent>? weapon = null, bool disarm = false)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (target.HasValue && _container.IsEntityInContainer(target.Value, (MetaDataComponent)null))
		{
			return false;
		}
		BaseContainer outerContainer = default(BaseContainer);
		_container.TryGetOuterContainer(uid, ((EntitySystem)this).Transform(uid), ref outerContainer);
		if (target.HasValue)
		{
			EntityUid? val = target;
			EntityUid? val2 = ((outerContainer != null) ? new EntityUid?(outerContainer.Owner) : ((EntityUid?)null));
			if ((val.HasValue != val2.HasValue || (val.HasValue && val.GetValueOrDefault() != val2.GetValueOrDefault())) && _container.IsEntityInContainer(uid, (MetaDataComponent)null))
			{
				CanAttackFromContainerEvent containerEv = new CanAttackFromContainerEvent(uid, target);
				((EntitySystem)this).RaiseLocalEvent<CanAttackFromContainerEvent>(uid, containerEv, false);
				if (!containerEv.CanAttack)
				{
					return false;
				}
			}
		}
		AttackAttemptEvent ev = new AttackAttemptEvent(uid, target, weapon, disarm);
		((EntitySystem)this).RaiseLocalEvent<AttackAttemptEvent>(uid, ev, false);
		if (((CancellableEntityEventArgs)ev).Cancelled)
		{
			return false;
		}
		if (!target.HasValue)
		{
			return true;
		}
		Entity<MeleeWeaponComponent>? val3 = weapon;
		GettingAttackedAttemptEvent tev = new GettingAttackedAttemptEvent(uid, val3.HasValue ? new EntityUid?(Entity<MeleeWeaponComponent>.op_Implicit(val3.GetValueOrDefault())) : ((EntityUid?)null), disarm);
		((EntitySystem)this).RaiseLocalEvent<GettingAttackedAttemptEvent>(target.Value, ref tev, false);
		return !tev.Cancelled;
	}

	public bool CanChangeDirection(EntityUid uid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		ChangeDirectionAttemptEvent ev = new ChangeDirectionAttemptEvent(uid);
		((EntitySystem)this).RaiseLocalEvent<ChangeDirectionAttemptEvent>(uid, ev, false);
		return !((CancellableEntityEventArgs)ev).Cancelled;
	}

	public bool CanShiver(EntityUid uid)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		ShiverAttemptEvent ev = new ShiverAttemptEvent(uid);
		((EntitySystem)this).RaiseLocalEvent<ShiverAttemptEvent>(uid, ref ev, false);
		return !ev.Cancelled;
	}

	public bool CanSweat(EntityUid uid)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		SweatAttemptEvent ev = new SweatAttemptEvent(uid);
		((EntitySystem)this).RaiseLocalEvent<SweatAttemptEvent>(uid, ref ev, false);
		return !ev.Cancelled;
	}
}
