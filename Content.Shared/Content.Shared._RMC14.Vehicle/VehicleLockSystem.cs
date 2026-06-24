using System;
using System.Collections.Generic;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Popups;
using Content.Shared.Vehicle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleLockSystem : EntitySystem
{
	[Dependency]
	private readonly SharedActionsSystem _actions;

	[Dependency]
	private readonly INetManager _net;

	[Dependency]
	private readonly SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VehicleEnterComponent, MapInitEvent>((EntityEventRefHandler<VehicleEnterComponent, MapInitEvent>)OnVehicleMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleLockActionComponent, VehicleLockActionEvent>((EntityEventRefHandler<VehicleLockActionComponent, VehicleLockActionEvent>)OnLockAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleLockActionComponent, ComponentShutdown>((EntityEventRefHandler<VehicleLockActionComponent, ComponentShutdown>)OnLockActionShutdown, (Type[])null, (Type[])null);
	}

	private void OnVehicleMapInit(Entity<VehicleEnterComponent> ent, ref MapInitEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			((EntitySystem)this).EnsureComp<VehicleLockComponent>(ent.Owner);
		}
	}

	public void EnableLockAction(EntityUid user, EntityUid vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		VehicleLockActionComponent actionComp = ((EntitySystem)this).EnsureComp<VehicleLockActionComponent>(user);
		actionComp.Sources.Add(vehicle);
		actionComp.Vehicle = vehicle;
		VehicleLockComponent lockComp = ((EntitySystem)this).EnsureComp<VehicleLockComponent>(vehicle);
		if (!actionComp.Action.HasValue || ((EntitySystem)this).TerminatingOrDeleted(actionComp.Action.Value, (MetaDataComponent)null))
		{
			actionComp.Action = _actions.AddAction(user, EntProtoId.op_Implicit(actionComp.ActionId));
		}
		EntityUid? action = actionComp.Action;
		if (action.HasValue)
		{
			EntityUid actionUid = action.GetValueOrDefault();
			_actions.SetEnabled(Entity<ActionComponent>.op_Implicit(actionUid), enabled: true);
			_actions.SetToggled(Entity<ActionComponent>.op_Implicit(actionUid), lockComp.Locked);
		}
		((EntitySystem)this).Dirty(user, (IComponent)(object)actionComp, (MetaDataComponent)null);
	}

	public void DisableLockAction(EntityUid user, EntityUid vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		VehicleLockActionComponent actionComp = default(VehicleLockActionComponent);
		if (!((EntitySystem)this).TryComp<VehicleLockActionComponent>(user, ref actionComp))
		{
			return;
		}
		actionComp.Sources.Remove(vehicle);
		if (actionComp.Sources.Count > 0)
		{
			using (HashSet<EntityUid>.Enumerator enumerator = actionComp.Sources.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					EntityUid remaining = enumerator.Current;
					actionComp.Vehicle = remaining;
				}
			}
			EntityUid? action = actionComp.Action;
			if (action.HasValue)
			{
				EntityUid actionUid = action.GetValueOrDefault();
				action = actionComp.Vehicle;
				if (action.HasValue)
				{
					EntityUid actionVehicle = action.GetValueOrDefault();
					VehicleLockComponent lockComp = default(VehicleLockComponent);
					if (((EntitySystem)this).TryComp<VehicleLockComponent>(actionVehicle, ref lockComp))
					{
						_actions.SetToggled(Entity<ActionComponent>.op_Implicit(actionUid), lockComp.Locked);
					}
				}
			}
			((EntitySystem)this).Dirty(user, (IComponent)(object)actionComp, (MetaDataComponent)null);
		}
		else
		{
			if (actionComp.Action.HasValue)
			{
				_actions.RemoveAction(Entity<ActionsComponent>.op_Implicit(user), Entity<ActionComponent>.op_Implicit(actionComp.Action.Value));
				actionComp.Action = null;
			}
			((EntitySystem)this).RemCompDeferred<VehicleLockActionComponent>(user);
		}
	}

	private void OnLockActionShutdown(Entity<VehicleLockActionComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? action = ent.Comp.Action;
		if (action.HasValue)
		{
			EntityUid action2 = action.GetValueOrDefault();
			_actions.RemoveAction(Entity<ActionComponent>.op_Implicit(action2));
		}
	}

	private void OnLockAction(Entity<VehicleLockActionComponent> ent, ref VehicleLockActionEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || ((HandledEntityEventArgs)args).Handled || args.Performer != ent.Owner)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid? vehicle = ent.Comp.Vehicle;
		if (!vehicle.HasValue)
		{
			return;
		}
		EntityUid vehicle2 = vehicle.GetValueOrDefault();
		if (((EntitySystem)this).Deleted(vehicle2, (MetaDataComponent)null))
		{
			return;
		}
		VehicleComponent vehicleComp = default(VehicleComponent);
		if (((EntitySystem)this).TryComp<VehicleComponent>(vehicle2, ref vehicleComp))
		{
			vehicle = vehicleComp.Operator;
			EntityUid owner = ent.Owner;
			if (vehicle.HasValue && !(vehicle.GetValueOrDefault() != owner))
			{
				VehicleLockComponent lockComp = ((EntitySystem)this).EnsureComp<VehicleLockComponent>(vehicle2);
				lockComp.Locked = !lockComp.Locked;
				vehicle = ent.Comp.Action;
				if (vehicle.HasValue)
				{
					EntityUid actionUid = vehicle.GetValueOrDefault();
					_actions.SetToggled(Entity<ActionComponent>.op_Implicit(actionUid), lockComp.Locked);
				}
				_popup.PopupEntity(base.Loc.GetString(lockComp.Locked ? "rmc-vehicle-lock-set-locked" : "rmc-vehicle-lock-set-unlocked"), ent.Owner, ent.Owner);
				return;
			}
		}
		_popup.PopupEntity(base.Loc.GetString("rmc-vehicle-lock-not-driver"), ent.Owner, ent.Owner, PopupType.SmallCaution);
	}
}
