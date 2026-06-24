using System;
using Content.Shared._RMC14.Evasion;
using Content.Shared._RMC14.Input;
using Content.Shared.Buckle.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Pulling.Events;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Standing;

public sealed class RMCStandingSystem : EntitySystem
{
	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private MobStateSystem _mob;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		((EntitySystem)this).SubscribeLocalEvent<DropItemsOnRestComponent, IsEquippingAttemptEvent>((EntityEventRefHandler<DropItemsOnRestComponent, IsEquippingAttemptEvent>)OnDropIsEquippingAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropItemsOnRestComponent, IsUnequippingAttemptEvent>((EntityEventRefHandler<DropItemsOnRestComponent, IsUnequippingAttemptEvent>)OnDropIsUnequippingAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DownOnEnterComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<DownOnEnterComponent, EntInsertedIntoContainerMessage>)OnEnterDown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DownOnEnterComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<DownOnEnterComponent, EntRemovedFromContainerMessage>)OnLeaveDown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StandingStateComponent, EvasionRefreshModifiersEvent>((EntityEventRefHandler<StandingStateComponent, EvasionRefreshModifiersEvent>)OnStandingStateEvasionRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRestComponent, StoodEvent>((EntityEventRefHandler<RMCRestComponent, StoodEvent>)OnRestStood, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRestComponent, StandAttemptEvent>((EntityEventRefHandler<RMCRestComponent, StandAttemptEvent>)OnRestStandAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRestComponent, StartPullAttemptEvent>((EntityEventRefHandler<RMCRestComponent, StartPullAttemptEvent>)OnRestStartPullAttempt, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(CMKeyFunctions.RMCRest, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
			if (val.HasValue)
			{
				EntityUid valueOrDefault = val.GetValueOrDefault();
				RMCRestComponent rMCRestComponent = default(RMCRestComponent);
				if (((EntitySystem)this).TryComp<RMCRestComponent>(valueOrDefault, ref rMCRestComponent))
				{
					TimeSpan curTime = _timing.CurTime;
					if (!(curTime < rMCRestComponent.LastToggleAt + rMCRestComponent.Cooldown) && rMCRestComponent.Resting)
					{
						SetRest(Entity<RMCRestComponent>.op_Implicit((valueOrDefault, rMCRestComponent)), resting: false);
						if (_standing.IsDown(valueOrDefault))
						{
							_popup.PopupClient(base.Loc.GetString("rmc-standing-stand-when-able"), valueOrDefault, valueOrDefault, PopupType.Medium);
						}
						rMCRestComponent.LastToggleAt = curTime;
						_movementSpeed.RefreshMovementSpeedModifiers(valueOrDefault);
					}
				}
			}
		}, (StateInputCmdDelegate)null, false, true)).Register<RMCStandingSystem>();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<RMCStandingSystem>();
	}

	private void OnDropBuckled(Entity<DropItemsOnRestComponent> drop, ref BuckledEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (!_standing.IsDown(Entity<DropItemsOnRestComponent>.op_Implicit(drop)))
		{
			return;
		}
		foreach (EntityUid held in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(drop.Owner)))
		{
			_hands.TryDrop(Entity<HandsComponent>.op_Implicit(drop.Owner), held);
		}
	}

	private void CancelIfResting<T>(Entity<DropItemsOnRestComponent> drop, ref T args) where T : CancellableEntityEventArgs
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TryCancelIfResting(drop, ref args);
	}

	private void OnDropIsEquippingAttempt(Entity<DropItemsOnRestComponent> drop, ref IsEquippingAttemptEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && args.Equipee == args.EquipTarget && TryCancelIfResting(drop, ref args))
		{
			args.Reason = "rmc-cant-while-resting";
		}
	}

	private void OnDropIsUnequippingAttempt(Entity<DropItemsOnRestComponent> drop, ref IsUnequippingAttemptEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && args.Unequipee == args.UnEquipTarget && TryCancelIfResting(drop, ref args))
		{
			args.Reason = "rmc-cant-while-resting";
		}
	}

	private bool TryCancelIfResting<T>(Entity<DropItemsOnRestComponent> drop, ref T args) where T : CancellableEntityEventArgs
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (((CancellableEntityEventArgs)args).Cancelled)
		{
			return false;
		}
		if (_standing.IsDown(Entity<DropItemsOnRestComponent>.op_Implicit(drop)))
		{
			((CancellableEntityEventArgs)args/*cast due to constrained. prefix*/).Cancel();
			return true;
		}
		return false;
	}

	private void OnEnterDown(Entity<DownOnEnterComponent> mob, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_standing.Down(((ContainerModifiedMessage)args).Entity, playSound: false, dropHeldItems: false, force: true, changeCollision: true);
	}

	private void OnLeaveDown(Entity<DownOnEnterComponent> mob, ref EntRemovedFromContainerMessage args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<KnockedDownComponent>(((ContainerModifiedMessage)args).Entity) || _mob.IsIncapacitated(((ContainerModifiedMessage)args).Entity))
		{
			_standing.Down(((ContainerModifiedMessage)args).Entity, playSound: false, dropHeldItems: true, force: true, changeCollision: true);
		}
		else
		{
			_standing.Stand(((ContainerModifiedMessage)args).Entity);
		}
	}

	private void OnStandingStateEvasionRefresh(Entity<StandingStateComponent> entity, ref EvasionRefreshModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!(entity.Owner != args.Entity.Owner) && _standing.IsDown(entity.Owner, entity.Comp))
		{
			args.Evasion += (FixedPoint2)(-15);
		}
	}

	private void OnRestStood(Entity<RMCRestComponent> ent, ref StoodEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Resting = false;
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<RMCRestComponent>.op_Implicit(ent));
		((EntitySystem)this).Dirty<RMCRestComponent>(ent, (MetaDataComponent)null);
	}

	private void OnRestStandAttempt(Entity<RMCRestComponent> ent, ref StandAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Resting)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnRestStartPullAttempt(Entity<RMCRestComponent> ent, ref StartPullAttemptEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !(args.Puller != ent.Owner) && ent.Comp.Resting)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	public void SetRest(Entity<RMCRestComponent?> rest, bool resting)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RMCRestComponent>(Entity<RMCRestComponent>.op_Implicit(rest), ref rest.Comp, false))
		{
			rest.Comp.Resting = resting;
			((EntitySystem)this).Dirty<RMCRestComponent>(rest, (MetaDataComponent)null);
			if (!resting)
			{
				_standing.Stand(Entity<RMCRestComponent>.op_Implicit(rest));
			}
		}
	}
}
