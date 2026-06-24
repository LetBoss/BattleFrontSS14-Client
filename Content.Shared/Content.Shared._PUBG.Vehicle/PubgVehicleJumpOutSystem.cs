using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Shared._PUBG.Vehicle;

public sealed class PubgVehicleJumpOutSystem : EntitySystem
{
	private const string ActionId = "ActionPubgVehicleJumpOut";

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private VehicleSystem _vehicles;

	private static readonly DamageSpecifier JumpDamage = new DamageSpecifier
	{
		DamageDict = new Dictionary<string, FixedPoint2> { ["Blunt"] = 20 }
	};

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCVehicleInteriorOccupantComponent, ComponentStartup>((EntityEventRefHandler<RMCVehicleInteriorOccupantComponent, ComponentStartup>)OnOccupantAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCVehicleInteriorOccupantComponent, ComponentShutdown>((EntityEventRefHandler<RMCVehicleInteriorOccupantComponent, ComponentShutdown>)OnOccupantRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgVehicleJumpOutOccupantComponent, PubgVehicleJumpOutActionEvent>((EntityEventRefHandler<PubgVehicleJumpOutOccupantComponent, PubgVehicleJumpOutActionEvent>)OnJumpOutAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgVehicleJumpOutOccupantComponent, PubgVehicleJumpOutDoAfterEvent>((EntityEventRefHandler<PubgVehicleJumpOutOccupantComponent, PubgVehicleJumpOutDoAfterEvent>)OnJumpOutDoAfter, (Type[])null, (Type[])null);
	}

	private void OnOccupantAdded(Entity<RMCVehicleInteriorOccupantComponent> ent, ref ComponentStartup args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			PubgVehicleJumpOutOccupantComponent pubgVehicleJumpOutOccupantComponent = ((EntitySystem)this).EnsureComp<PubgVehicleJumpOutOccupantComponent>(ent.Owner);
			EntityUid? action = null;
			_actions.AddAction(ent.Owner, ref action, "ActionPubgVehicleJumpOut");
			pubgVehicleJumpOutOccupantComponent.ActionEntity = action;
		}
	}

	private void OnOccupantRemoved(Entity<RMCVehicleInteriorOccupantComponent> ent, ref ComponentShutdown args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		PubgVehicleJumpOutOccupantComponent comp = default(PubgVehicleJumpOutOccupantComponent);
		if (!_net.IsClient && ((EntitySystem)this).TryComp<PubgVehicleJumpOutOccupantComponent>(ent.Owner, ref comp))
		{
			EntityUid? actionEntity = comp.ActionEntity;
			if (actionEntity.HasValue)
			{
				EntityUid action = actionEntity.GetValueOrDefault();
				_actions.RemoveAction(Entity<ActionsComponent>.op_Implicit(ent.Owner), Entity<ActionComponent>.op_Implicit(action));
			}
			((EntitySystem)this).RemCompDeferred<PubgVehicleJumpOutOccupantComponent>(ent.Owner);
		}
	}

	private void OnJumpOutAction(Entity<PubgVehicleJumpOutOccupantComponent> ent, ref PubgVehicleJumpOutActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		RMCVehicleInteriorOccupantComponent occupant = default(RMCVehicleInteriorOccupantComponent);
		VehicleEnterComponent enter = default(VehicleEnterComponent);
		if (((EntitySystem)this).TryComp<RMCVehicleInteriorOccupantComponent>(ent.Owner, ref occupant) && occupant.Vehicle.HasValue && ((EntitySystem)this).TryComp<VehicleEnterComponent>(occupant.Vehicle.Value, ref enter))
		{
			float delay = Math.Max(0f, enter.ExitDoAfter / 2f);
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, ent.Owner, delay, new PubgVehicleJumpOutDoAfterEvent(), ent.Owner)
			{
				BreakOnMove = false
			};
			_doAfter.TryStartDoAfter(doAfter);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnJumpOutDoAfter(Entity<PubgVehicleJumpOutOccupantComponent> ent, ref PubgVehicleJumpOutDoAfterEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		RMCVehicleInteriorOccupantComponent occupant = default(RMCVehicleInteriorOccupantComponent);
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<RMCVehicleInteriorOccupantComponent>(ent.Owner, ref occupant) && occupant.Vehicle.HasValue)
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (_vehicles.TryExitFromInterior(ent.Owner, occupant.Vehicle.Value))
			{
				_damageable.TryChangeDamage(ent.Owner, JumpDamage);
			}
		}
	}
}
