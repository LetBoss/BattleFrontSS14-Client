using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.Explosion.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Vehicle;
using Content.Shared.Vehicle.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared._CIV14merka.Aircraft;

public sealed class AircraftSystem : EntitySystem
{
	[Dependency]
	private readonly SharedActionsSystem _actions;

	[Dependency]
	private readonly INetManager _net;

	[Dependency]
	private readonly SharedPhysicsSystem _physics;

	[Dependency]
	private readonly DamageableSystem _damageable;

	[Dependency]
	private readonly SharedAudioSystem _audio;

	[Dependency]
	private readonly SharedPopupSystem _popup;

	[Dependency]
	private readonly SharedContentEyeSystem _contentEye;

	[Dependency]
	private readonly SharedEyeSystem _eye;

	[Dependency]
	private readonly AlertsSystem _alerts;

	[Dependency]
	private readonly VehicleTopologySystem _topology;

	[Dependency]
	private readonly GridVehicleMoverSystem _mover;

	private const int WallMask = 67108894;

	private const string AltitudeAlertId = "AircraftAltitude";

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<AircraftComponent, VehicleOperatorSetEvent>((EntityEventRefHandler<AircraftComponent, VehicleOperatorSetEvent>)OnOperatorSet, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AircraftPilotActionComponent, AircraftAscendActionEvent>((EntityEventRefHandler<AircraftPilotActionComponent, AircraftAscendActionEvent>)OnAscend, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AircraftPilotActionComponent, AircraftDescendActionEvent>((EntityEventRefHandler<AircraftPilotActionComponent, AircraftDescendActionEvent>)OnDescend, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AircraftPilotActionComponent, AircraftBombActionEvent>((EntityEventRefHandler<AircraftPilotActionComponent, AircraftBombActionEvent>)OnBomb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AircraftPilotActionComponent, ComponentShutdown>((EntityEventRefHandler<AircraftPilotActionComponent, ComponentShutdown>)OnPilotShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AircraftCannonComponent, AmmoShotEvent>((EntityEventRefHandler<AircraftCannonComponent, AmmoShotEvent>)OnCannonShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AircraftComponent, BeforeDamageChangedEvent>((EntityEventRefHandler<AircraftComponent, BeforeDamageChangedEvent>)OnBeforeDamage, new Type[1] { typeof(Content.Shared.Vehicle.VehicleSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AircraftComponent, ComponentShutdown>((EntityEventRefHandler<AircraftComponent, ComponentShutdown>)OnAircraftShutdown, (Type[])null, (Type[])null);
	}

	private void OnOperatorSet(Entity<AircraftComponent> ent, ref VehicleOperatorSetEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityUid? oldOperator = args.OldOperator;
		if (oldOperator.HasValue)
		{
			EntityUid oldOp = oldOperator.GetValueOrDefault();
			EntityUid val = oldOp;
			oldOperator = args.NewOperator;
			if (!oldOperator.HasValue || val != oldOperator.GetValueOrDefault())
			{
				ClearOperatorView(oldOp);
				((EntitySystem)this).RemCompDeferred<AircraftPilotActionComponent>(oldOp);
			}
		}
		oldOperator = args.NewOperator;
		if (oldOperator.HasValue)
		{
			EntityUid newOp = oldOperator.GetValueOrDefault();
			AircraftPilotActionComponent comp = ((EntitySystem)this).EnsureComp<AircraftPilotActionComponent>(newOp);
			comp.Vehicle = ent.Owner;
			AircraftPilotActionComponent aircraftPilotActionComponent = comp;
			oldOperator = aircraftPilotActionComponent.AscendAction;
			if (!oldOperator.HasValue)
			{
				aircraftPilotActionComponent.AscendAction = _actions.AddAction(newOp, EntProtoId.op_Implicit(ent.Comp.AscendActionId));
			}
			aircraftPilotActionComponent = comp;
			oldOperator = aircraftPilotActionComponent.DescendAction;
			if (!oldOperator.HasValue)
			{
				aircraftPilotActionComponent.DescendAction = _actions.AddAction(newOp, EntProtoId.op_Implicit(ent.Comp.DescendActionId));
			}
			aircraftPilotActionComponent = comp;
			oldOperator = aircraftPilotActionComponent.BombAction;
			if (!oldOperator.HasValue)
			{
				aircraftPilotActionComponent.BombAction = _actions.AddAction(newOp, EntProtoId.op_Implicit(ent.Comp.BombActionId));
			}
			((EntitySystem)this).Dirty(newOp, (IComponent)(object)comp, (MetaDataComponent)null);
			ApplyOperatorView(ent.Owner, ent.Comp);
			RefreshActionStates(ent.Owner);
		}
	}

	private void OnPilotShutdown(Entity<AircraftPilotActionComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? ascendAction = ent.Comp.AscendAction;
		if (ascendAction.HasValue)
		{
			EntityUid asc = ascendAction.GetValueOrDefault();
			_actions.RemoveAction(Entity<ActionsComponent>.op_Implicit(ent.Owner), Entity<ActionComponent>.op_Implicit(asc));
		}
		ascendAction = ent.Comp.DescendAction;
		if (ascendAction.HasValue)
		{
			EntityUid desc = ascendAction.GetValueOrDefault();
			_actions.RemoveAction(Entity<ActionsComponent>.op_Implicit(ent.Owner), Entity<ActionComponent>.op_Implicit(desc));
		}
		ascendAction = ent.Comp.BombAction;
		if (ascendAction.HasValue)
		{
			EntityUid bomb = ascendAction.GetValueOrDefault();
			_actions.RemoveAction(Entity<ActionsComponent>.op_Implicit(ent.Owner), Entity<ActionComponent>.op_Implicit(bomb));
		}
		ClearOperatorView(ent.Owner);
	}

	private void OnAscend(Entity<AircraftPilotActionComponent> ent, ref AircraftAscendActionEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
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
		AircraftComponent aircraft = default(AircraftComponent);
		if (((EntitySystem)this).TryComp<AircraftComponent>(vehicle2, ref aircraft) && aircraft.Altitude < aircraft.MaxAltitude)
		{
			GridVehicleMoverComponent mover = default(GridVehicleMoverComponent);
			if (aircraft.Altitude == 0 && (!((EntitySystem)this).TryComp<GridVehicleMoverComponent>(vehicle2, ref mover) || mover.CurrentSpeed < aircraft.TakeoffSpeed))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-aircraft-need-speed"), ent.Owner, ent.Owner, PopupType.SmallCaution);
			}
			else
			{
				SetAltitude(vehicle2, aircraft, aircraft.Altitude + 1);
			}
		}
	}

	private void OnDescend(Entity<AircraftPilotActionComponent> ent, ref AircraftDescendActionEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
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
		AircraftComponent aircraft = default(AircraftComponent);
		if (!((EntitySystem)this).TryComp<AircraftComponent>(vehicle2, ref aircraft) || aircraft.Altitude <= 0)
		{
			return;
		}
		if (aircraft.Altitude > 1)
		{
			SetAltitude(vehicle2, aircraft, aircraft.Altitude - 1);
			return;
		}
		_physics.SetCanCollide(vehicle2, true, true, true, (FixturesComponent)null, (PhysicsComponent)null);
		if (_mover.CanOccupyCurrent(vehicle2))
		{
			SetAltitude(vehicle2, aircraft, 0);
		}
		else
		{
			CrashLand(vehicle2, aircraft);
		}
	}

	private void OnBomb(Entity<AircraftPilotActionComponent> ent, ref AircraftBombActionEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || ((HandledEntityEventArgs)args).Handled || args.Performer != ent.Owner)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid? vehicle = ent.Comp.Vehicle;
		if (vehicle.HasValue)
		{
			EntityUid vehicle2 = vehicle.GetValueOrDefault();
			AircraftComponent aircraft = default(AircraftComponent);
			if (((EntitySystem)this).TryComp<AircraftComponent>(vehicle2, ref aircraft) && aircraft.Altitude > 0)
			{
				EntityUid bomb = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(aircraft.BombProto), ((EntitySystem)this).Transform(vehicle2).Coordinates);
				((EntitySystem)this).EnsureComp<ActiveTimerTriggerComponent>(bomb).TimeRemaining = (float)aircraft.Altitude * aircraft.BombFallTimePerLevel;
			}
		}
	}

	private void OnCannonShot(Entity<AircraftCannonComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		AircraftComponent aircraft = default(AircraftComponent);
		if (!_topology.TryGetVehicle(ent.Owner, out var vehicle) || !((EntitySystem)this).TryComp<AircraftComponent>(vehicle, ref aircraft) || aircraft.Altitude < ent.Comp.IgnoreWallsFromAltitude)
		{
			return;
		}
		FixturesComponent fixtures = default(FixturesComponent);
		foreach (EntityUid proj in args.FiredProjectiles)
		{
			((EntitySystem)this).EnsureComp<HighAltitudeProjectileComponent>(proj);
			if (!((EntitySystem)this).TryComp<FixturesComponent>(proj, ref fixtures))
			{
				continue;
			}
			foreach (KeyValuePair<string, Fixture> fixture2 in fixtures.Fixtures)
			{
				fixture2.Deconstruct(out var key, out var value);
				string id = key;
				Fixture fixture = value;
				int newMask = fixture.CollisionMask & -67108895;
				if (newMask != fixture.CollisionMask)
				{
					_physics.SetCollisionMask(proj, id, fixture, newMask, fixtures, (PhysicsComponent)null);
				}
			}
		}
	}

	private void SetAltitude(EntityUid vehicle, AircraftComponent aircraft, int newAltitude)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		aircraft.Altitude = Math.Clamp(newAltitude, 0, aircraft.MaxAltitude);
		bool airborne = aircraft.Altitude > 0;
		_physics.SetCanCollide(vehicle, !airborne, true, true, (FixturesComponent)null, (PhysicsComponent)null);
		PhysicsComponent body = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(vehicle, ref body))
		{
			_physics.SetBodyStatus(vehicle, body, (BodyStatus)(airborne ? 1 : 0), true);
		}
		((EntitySystem)this).Dirty(vehicle, (IComponent)(object)aircraft, (MetaDataComponent)null);
		ApplyOperatorView(vehicle, aircraft);
		RefreshActionStates(vehicle);
	}

	private void ApplyOperatorView(EntityUid vehicle, AircraftComponent aircraft)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		VehicleComponent vc = default(VehicleComponent);
		if (!((EntitySystem)this).TryComp<VehicleComponent>(vehicle, ref vc))
		{
			return;
		}
		EntityUid? val = vc.Operator;
		if (val.HasValue)
		{
			EntityUid op = val.GetValueOrDefault();
			_alerts.ShowAlert(op, aircraft.AltitudeAlert, (short)aircraft.Altitude);
			bool airborne = aircraft.Altitude > 0;
			_eye.SetDrawFov(op, !airborne, (EyeComponent)null);
			if (airborne)
			{
				float z = 1f + (float)aircraft.Altitude * aircraft.ZoomPerLevel;
				Vector2 zoom = new Vector2(z, z);
				_contentEye.SetMaxZoom(op, zoom);
				_contentEye.SetZoom(op, zoom);
			}
			else
			{
				_contentEye.ResetZoom(op);
			}
		}
	}

	private void ClearOperatorView(EntityUid op)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		_contentEye.ResetZoom(op);
		_eye.SetDrawFov(op, true, (EyeComponent)null);
		_alerts.ClearAlert(op, ProtoId<AlertPrototype>.op_Implicit("AircraftAltitude"));
	}

	private void CrashLand(EntityUid vehicle, AircraftComponent aircraft)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		aircraft.Altitude = 0;
		_physics.SetCanCollide(vehicle, true, true, true, (FixturesComponent)null, (PhysicsComponent)null);
		PhysicsComponent body = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(vehicle, ref body))
		{
			_physics.SetBodyStatus(vehicle, body, (BodyStatus)0, true);
		}
		((EntitySystem)this).Dirty(vehicle, (IComponent)(object)aircraft, (MetaDataComponent)null);
		if (aircraft.CrashSound != null)
		{
			_audio.PlayPvs(aircraft.CrashSound, vehicle, (AudioParams?)null);
		}
		VehicleComponent vc = default(VehicleComponent);
		if (((EntitySystem)this).TryComp<VehicleComponent>(vehicle, ref vc))
		{
			EntityUid? val = vc.Operator;
			if (val.HasValue)
			{
				EntityUid op = val.GetValueOrDefault();
				DamageSpecifier dmg = new DamageSpecifier
				{
					DamageDict = { ["Blunt"] = 500 }
				};
				_damageable.TryChangeDamage(op, dmg, ignoreResistances: true, interruptsDoAfters: true, null, vehicle);
			}
		}
		((EntitySystem)this).RaiseLocalEvent<RMCVehicleFrameDestroyedEvent>(vehicle, new RMCVehicleFrameDestroyedEvent(vehicle), false);
		ApplyOperatorView(vehicle, aircraft);
		RefreshActionStates(vehicle);
	}

	private void OnBeforeDamage(Entity<AircraftComponent> ent, ref BeforeDamageChangedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Airborne && !args.Cancelled)
		{
			args.Cancelled = true;
		}
	}

	private void OnAircraftShutdown(Entity<AircraftComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Airborne)
		{
			_physics.SetCanCollide(ent.Owner, true, true, true, (FixturesComponent)null, (PhysicsComponent)null);
		}
	}

	private void RefreshActionStates(EntityUid vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		AircraftComponent aircraft = default(AircraftComponent);
		VehicleComponent vc = default(VehicleComponent);
		if (!((EntitySystem)this).TryComp<AircraftComponent>(vehicle, ref aircraft) || !((EntitySystem)this).TryComp<VehicleComponent>(vehicle, ref vc))
		{
			return;
		}
		EntityUid? val = vc.Operator;
		if (!val.HasValue)
		{
			return;
		}
		EntityUid op = val.GetValueOrDefault();
		AircraftPilotActionComponent pilot = default(AircraftPilotActionComponent);
		if (((EntitySystem)this).TryComp<AircraftPilotActionComponent>(op, ref pilot))
		{
			val = pilot.AscendAction;
			if (val.HasValue)
			{
				EntityUid asc = val.GetValueOrDefault();
				_actions.SetToggled(Entity<ActionComponent>.op_Implicit(asc), aircraft.Altitude > 0);
			}
			val = pilot.DescendAction;
			if (val.HasValue)
			{
				EntityUid desc = val.GetValueOrDefault();
				_actions.SetToggled(Entity<ActionComponent>.op_Implicit(desc), aircraft.Altitude > 0);
			}
		}
	}
}
