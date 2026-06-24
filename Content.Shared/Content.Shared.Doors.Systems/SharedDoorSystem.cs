using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Doors.Components;
using Content.Shared.Emag.Systems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Prying.Components;
using Content.Shared.Prying.Systems;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Tools.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Doors.Systems;

public abstract class SharedDoorSystem : EntitySystem
{
	public enum AccessTypes
	{
		Id,
		AllowAllIdExternal,
		AllowAllNoExternal,
		AllowAll
	}

	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	protected IGameTiming GameTiming;

	[Dependency]
	private INetManager _net;

	[Dependency]
	protected SharedPhysicsSystem PhysicsSystem;

	[Dependency]
	private DamageableSystem _damageableSystem;

	[Dependency]
	private EmagSystem _emag;

	[Dependency]
	private SharedStunSystem _stunSystem;

	[Dependency]
	protected TagSystem Tags;

	[Dependency]
	protected SharedAudioSystem Audio;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	protected SharedAppearanceSystem AppearanceSystem;

	[Dependency]
	private OccluderSystem _occluder;

	[Dependency]
	private AccessReaderSystem _accessReaderSystem;

	[Dependency]
	private PryingSystem _pryingSystem;

	[Dependency]
	protected SharedPopupSystem Popup;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private SharedPowerReceiverSystem _powerReceiver;

	public static readonly ProtoId<TagPrototype> DoorBumpTag = ProtoId<TagPrototype>.op_Implicit("DoorBumpOpener");

	private readonly HashSet<Entity<DoorComponent>> _activeDoors = new HashSet<Entity<DoorComponent>>();

	private readonly HashSet<Entity<PhysicsComponent>> _doorIntersecting = new HashSet<Entity<PhysicsComponent>>();

	public AccessTypes AccessType;

	public void InitializeBolts()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DoorBoltComponent, BeforeDoorOpenedEvent>((ComponentEventHandler<DoorBoltComponent, BeforeDoorOpenedEvent>)OnBeforeDoorOpened, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorBoltComponent, BeforeDoorClosedEvent>((ComponentEventHandler<DoorBoltComponent, BeforeDoorClosedEvent>)OnBeforeDoorClosed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorBoltComponent, BeforeDoorDeniedEvent>((ComponentEventHandler<DoorBoltComponent, BeforeDoorDeniedEvent>)OnBeforeDoorDenied, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorBoltComponent, BeforePryEvent>((ComponentEventRefHandler<DoorBoltComponent, BeforePryEvent>)OnDoorPry, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorBoltComponent, DoorStateChangedEvent>((EntityEventRefHandler<DoorBoltComponent, DoorStateChangedEvent>)OnStateChanged, (Type[])null, (Type[])null);
	}

	private void OnDoorPry(EntityUid uid, DoorBoltComponent component, ref BeforePryEvent args)
	{
		if (!args.Cancelled && component.BoltsDown && !args.Force)
		{
			args.Message = "airlock-component-cannot-pry-is-bolted-message";
			args.Cancelled = true;
		}
	}

	private void OnBeforeDoorOpened(EntityUid uid, DoorBoltComponent component, BeforeDoorOpenedEvent args)
	{
		if (component.BoltsDown)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnBeforeDoorClosed(EntityUid uid, DoorBoltComponent component, BeforeDoorClosedEvent args)
	{
		if (component.BoltsDown)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnBeforeDoorDenied(EntityUid uid, DoorBoltComponent component, BeforeDoorDeniedEvent args)
	{
		if (component.BoltsDown)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	public void SetBoltWireCut(Entity<DoorBoltComponent> ent, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.BoltWireCut = value;
		((EntitySystem)this).Dirty(Entity<DoorBoltComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
	}

	public void UpdateBoltLightStatus(Entity<DoorBoltComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		AppearanceSystem.SetData(Entity<DoorBoltComponent>.op_Implicit(ent), (Enum)DoorVisuals.BoltLights, (object)GetBoltLightsVisible(ent), (AppearanceComponent)null);
	}

	public bool GetBoltLightsVisible(Entity<DoorBoltComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.BoltLightsEnabled && ent.Comp.BoltsDown)
		{
			return ent.Comp.Powered;
		}
		return false;
	}

	public void SetBoltLightsEnabled(Entity<DoorBoltComponent> ent, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.BoltLightsEnabled != value)
		{
			ent.Comp.BoltLightsEnabled = value;
			((EntitySystem)this).Dirty(Entity<DoorBoltComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
			UpdateBoltLightStatus(ent);
		}
	}

	public void SetBoltsDown(Entity<DoorBoltComponent> ent, bool value, EntityUid? user = null, bool predicted = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TrySetBoltDown(ent, value, user, predicted);
	}

	public bool TrySetBoltDown(Entity<DoorBoltComponent> ent, bool value, EntityUid? user = null, bool predicted = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (!_powerReceiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(ent.Owner)))
		{
			return false;
		}
		if (ent.Comp.BoltsDown == value)
		{
			return false;
		}
		ent.Comp.BoltsDown = value;
		((EntitySystem)this).Dirty(Entity<DoorBoltComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		UpdateBoltLightStatus(ent);
		DoorBoltsChangedEvent ev = new DoorBoltsChangedEvent(value);
		((EntitySystem)this).RaiseLocalEvent<DoorBoltsChangedEvent>(ent.Owner, ev, false);
		SoundSpecifier sound = (value ? ent.Comp.BoltDownSound : ent.Comp.BoltUpSound);
		if (predicted)
		{
			Audio.PlayPredicted(sound, Entity<DoorBoltComponent>.op_Implicit(ent), user, (AudioParams?)null);
		}
		else
		{
			Audio.PlayPvs(sound, Entity<DoorBoltComponent>.op_Implicit(ent), (AudioParams?)null);
		}
		return true;
	}

	private void OnStateChanged(Entity<DoorBoltComponent> entity, ref DoorStateChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateBoltLightStatus(entity);
	}

	public bool IsBolted(EntityUid uid, DoorBoltComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DoorBoltComponent>(uid, ref component, false))
		{
			return false;
		}
		return component.BoltsDown;
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		InitializeBolts();
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, ComponentInit>((EntityEventRefHandler<DoorComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, ComponentRemove>((EntityEventRefHandler<DoorComponent, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<DoorComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, ActivateInWorldEvent>((ComponentEventHandler<DoorComponent, ActivateInWorldEvent>)OnActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, StartCollideEvent>((ComponentEventRefHandler<DoorComponent, StartCollideEvent>)HandleCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, PreventCollideEvent>((ComponentEventRefHandler<DoorComponent, PreventCollideEvent>)PreventCollision, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, BeforePryEvent>((ComponentEventRefHandler<DoorComponent, BeforePryEvent>)OnBeforePry, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, PriedEvent>((ComponentEventRefHandler<DoorComponent, PriedEvent>)OnAfterPry, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, WeldableAttemptEvent>((ComponentEventHandler<DoorComponent, WeldableAttemptEvent>)OnWeldAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, WeldableChangedEvent>((ComponentEventRefHandler<DoorComponent, WeldableChangedEvent>)OnWeldChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, GetPryTimeModifierEvent>((ComponentEventRefHandler<DoorComponent, GetPryTimeModifierEvent>)OnPryTimeModifier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, GotEmaggedEvent>((ComponentEventRefHandler<DoorComponent, GotEmaggedEvent>)OnEmagged, (Type[])null, (Type[])null);
	}

	protected virtual void OnComponentInit(Entity<DoorComponent> ent, ref ComponentInit args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		DoorComponent door = ent.Comp;
		if (door.NextStateChange.HasValue)
		{
			_activeDoors.Add(ent);
		}
		else
		{
			if (door.State == DoorState.Opening)
			{
				door.State = DoorState.Open;
				door.Partial = false;
			}
			if (door.State == DoorState.Closing)
			{
				door.State = DoorState.Closed;
				door.Partial = false;
			}
		}
		bool collidable = door.State == DoorState.Closed || (door.State == DoorState.Closing && door.Partial) || (door.State == DoorState.Opening && !door.Partial);
		SetCollidable(Entity<DoorComponent>.op_Implicit(ent), collidable, door);
		AppearanceSystem.SetData(Entity<DoorComponent>.op_Implicit(ent), (Enum)DoorVisuals.State, (object)door.State, (AppearanceComponent)null);
	}

	private void OnRemove(Entity<DoorComponent> door, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_activeDoors.Remove(door);
	}

	private void OnEmagged(EntityUid uid, DoorComponent door, ref GotEmaggedEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		AirlockComponent airlock = default(AirlockComponent);
		if (_emag.CompareFlag(args.Type, EmagType.Access) && ((EntitySystem)this).TryComp<AirlockComponent>(uid, ref airlock) && !IsBolted(uid) && airlock.Powered && door.State == DoorState.Closed && SetState(uid, DoorState.Emagging, door))
		{
			args.Repeatable = true;
			args.Handled = true;
		}
	}

	private void OnHandleState(Entity<DoorComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		DoorComponent door = ent.Comp;
		if (!door.NextStateChange.HasValue)
		{
			_activeDoors.Remove(ent);
		}
		else
		{
			_activeDoors.Add(ent);
		}
		((EntitySystem)this).RaiseLocalEvent<DoorStateChangedEvent>(Entity<DoorComponent>.op_Implicit(ent), new DoorStateChangedEvent(door.State), false);
	}

	public bool SetState(EntityUid uid, DoorState state, DoorComponent? door = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DoorComponent>(uid, ref door, true))
		{
			return false;
		}
		if (state == door.State)
		{
			return false;
		}
		switch (state)
		{
		case DoorState.Opening:
			_activeDoors.Add(Entity<DoorComponent>.op_Implicit((uid, door)));
			door.NextStateChange = GameTiming.CurTime + door.OpenTimeOne;
			break;
		case DoorState.Closing:
			_activeDoors.Add(Entity<DoorComponent>.op_Implicit((uid, door)));
			door.NextStateChange = GameTiming.CurTime + door.CloseTimeOne;
			break;
		case DoorState.Denying:
			_activeDoors.Add(Entity<DoorComponent>.op_Implicit((uid, door)));
			door.NextStateChange = GameTiming.CurTime + door.DenyDuration;
			break;
		case DoorState.Emagging:
			_activeDoors.Add(Entity<DoorComponent>.op_Implicit((uid, door)));
			door.NextStateChange = GameTiming.CurTime + door.EmagDuration;
			break;
		case DoorState.Open:
			door.Partial = false;
			if (!door.NextStateChange.HasValue)
			{
				_activeDoors.Remove(Entity<DoorComponent>.op_Implicit((uid, door)));
			}
			break;
		case DoorState.Closed:
			door.Partial = false;
			break;
		}
		door.State = state;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)door, (MetaDataComponent)null);
		((EntitySystem)this).RaiseLocalEvent<DoorStateChangedEvent>(uid, new DoorStateChangedEvent(state), false);
		AppearanceSystem.SetData(uid, (Enum)DoorVisuals.State, (object)door.State, (AppearanceComponent)null);
		return true;
	}

	protected void OnActivate(EntityUid uid, DoorComponent door, ActivateInWorldEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex && door.ClickOpen)
		{
			if (!TryToggleDoor(uid, door, args.User, predicted: true))
			{
				_pryingSystem.TryPry(uid, args.User, out var _);
			}
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnPryTimeModifier(EntityUid uid, DoorComponent door, ref GetPryTimeModifierEvent args)
	{
		args.BaseTime = door.PryTime;
	}

	private void OnBeforePry(EntityUid uid, DoorComponent door, ref BeforePryEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (door.State == DoorState.Welded || !door.CanPry)
		{
			args.Cancelled = true;
		}
		RMCBeforePryEvent beforepry = new RMCBeforePryEvent(args.User);
		((EntitySystem)this).RaiseLocalEvent<RMCBeforePryEvent>(uid, ref beforepry, false);
		if (beforepry.Cancelled)
		{
			args.Cancelled = true;
		}
	}

	private void OnAfterPry(EntityUid uid, DoorComponent door, ref PriedEvent args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		if (door.State == DoorState.Closed)
		{
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(12, 2);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "ToPrettyString(args.User)");
			handler.AppendLiteral(" pried ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
			handler.AppendLiteral(" open");
			adminLog.Add(LogType.Action, LogImpact.Medium, ref handler);
			StartOpening(uid, door, args.User, predicted: true);
		}
		else if (door.State == DoorState.Open)
		{
			ISharedAdminLogManager adminLog2 = _adminLog;
			LogStringHandler handler2 = new LogStringHandler(14, 2);
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "ToPrettyString(args.User)");
			handler2.AppendLiteral(" pried ");
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
			handler2.AppendLiteral(" closed");
			adminLog2.Add(LogType.Action, LogImpact.Medium, ref handler2);
			StartClosing(uid, door, args.User, predicted: true);
		}
	}

	private void OnWeldAttempt(EntityUid uid, DoorComponent component, WeldableAttemptEvent args)
	{
		if (component.CurrentlyCrushing.Count > 0)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
		else if (component.State != DoorState.Closed && component.State != DoorState.Welded)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnWeldChanged(EntityUid uid, DoorComponent component, ref WeldableChangedEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (component.State == DoorState.Closed)
		{
			SetState(uid, DoorState.Welded, component);
		}
		else if (component.State == DoorState.Welded)
		{
			SetState(uid, DoorState.Closed, component);
		}
	}

	public void Deny(EntityUid uid, DoorComponent? door = null, EntityUid? user = null, bool predicted = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DoorComponent>(uid, ref door, true) || door.State != DoorState.Closed)
		{
			return;
		}
		BeforeDoorDeniedEvent ev = new BeforeDoorDeniedEvent();
		((EntitySystem)this).RaiseLocalEvent<BeforeDoorDeniedEvent>(uid, ev, false);
		if (!((CancellableEntityEventArgs)ev).Cancelled && SetState(uid, DoorState.Denying, door))
		{
			if (predicted)
			{
				Audio.PlayPredicted(door.DenySound, uid, user, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(-3f));
			}
			else if (_net.IsServer)
			{
				Audio.PlayPvs(door.DenySound, uid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(-3f));
			}
		}
	}

	public bool TryToggleDoor(EntityUid uid, DoorComponent? door = null, EntityUid? user = null, bool predicted = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DoorComponent>(uid, ref door, true))
		{
			return false;
		}
		DoorState state = door.State;
		if ((state == DoorState.Closed || state == DoorState.Denying) ? true : false)
		{
			return TryOpen(uid, door, user, predicted, door.State == DoorState.Denying);
		}
		if (door.State == DoorState.Open)
		{
			return TryClose(uid, door, user, predicted);
		}
		return false;
	}

	public bool TryOpen(EntityUid uid, DoorComponent? door = null, EntityUid? user = null, bool predicted = false, bool quiet = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DoorComponent>(uid, ref door, true))
		{
			return false;
		}
		if (!CanOpen(uid, door, user, quiet))
		{
			return false;
		}
		StartOpening(uid, door, user, predicted);
		return true;
	}

	public bool CanOpen(EntityUid uid, DoorComponent? door = null, EntityUid? user = null, bool quiet = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DoorComponent>(uid, ref door, true))
		{
			return false;
		}
		if (door.State == DoorState.Welded)
		{
			return false;
		}
		BeforeDoorOpenedEvent ev = new BeforeDoorOpenedEvent
		{
			User = user
		};
		((EntitySystem)this).RaiseLocalEvent<BeforeDoorOpenedEvent>(uid, ev, false);
		if (((CancellableEntityEventArgs)ev).Cancelled)
		{
			return false;
		}
		if (!HasAccess(uid, user, door))
		{
			if (!quiet)
			{
				Deny(uid, door, user, predicted: true);
			}
			return false;
		}
		return true;
	}

	public void StartOpening(EntityUid uid, DoorComponent? door = null, EntityUid? user = null, bool predicted = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DoorComponent>(uid, ref door, true))
		{
			return;
		}
		DoorState lastState = door.State;
		if (SetState(uid, DoorState.Opening, door))
		{
			if (predicted)
			{
				Audio.PlayPredicted(door.OpenSound, uid, user, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(-5f));
			}
			else if (_net.IsServer)
			{
				Audio.PlayPvs(door.OpenSound, uid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(-5f));
			}
			DoorBoltComponent doorBoltComponent = default(DoorBoltComponent);
			if (lastState == DoorState.Emagging && ((EntitySystem)this).TryComp<DoorBoltComponent>(uid, ref doorBoltComponent))
			{
				SetBoltsDown(Entity<DoorBoltComponent>.op_Implicit((uid, doorBoltComponent)), !doorBoltComponent.BoltsDown, user, predicted: true);
			}
		}
	}

	public void OnPartialOpen(EntityUid uid, DoorComponent? door = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<DoorComponent>(uid, ref door, true))
		{
			SetCollidable(uid, collidable: false, door);
			door.Partial = true;
			door.NextStateChange = GameTiming.CurTime + door.CloseTimeTwo;
			_activeDoors.Add(Entity<DoorComponent>.op_Implicit((uid, door)));
			((EntitySystem)this).Dirty(uid, (IComponent)(object)door, (MetaDataComponent)null);
		}
	}

	public bool TryOpenAndBolt(EntityUid uid, DoorComponent? door = null, AirlockComponent? airlock = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DoorComponent, AirlockComponent>(uid, ref door, ref airlock, true))
		{
			return false;
		}
		if (IsBolted(uid) || !airlock.Powered || door.State != DoorState.Closed)
		{
			return false;
		}
		SetState(uid, DoorState.Emagging, door);
		return true;
	}

	public bool TryClose(EntityUid uid, DoorComponent? door = null, EntityUid? user = null, bool predicted = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DoorComponent>(uid, ref door, true))
		{
			return false;
		}
		if (!CanClose(uid, door, user))
		{
			return false;
		}
		StartClosing(uid, door, user, predicted);
		return true;
	}

	public bool CanClose(EntityUid uid, DoorComponent? door = null, EntityUid? user = null, bool partial = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DoorComponent>(uid, ref door, true))
		{
			return false;
		}
		DoorState state = door.State;
		if ((state == DoorState.Closed || state == DoorState.Welded) ? true : false)
		{
			return false;
		}
		BeforeDoorClosedEvent ev = new BeforeDoorClosedEvent(door.PerformCollisionCheck, partial);
		((EntitySystem)this).RaiseLocalEvent<BeforeDoorClosedEvent>(uid, ev, false);
		if (((CancellableEntityEventArgs)ev).Cancelled)
		{
			return false;
		}
		if (!HasAccess(uid, user, door))
		{
			return false;
		}
		if (ev.PerformCollisionCheck)
		{
			return !GetColliding(uid).Any();
		}
		return true;
	}

	public void StartClosing(EntityUid uid, DoorComponent? door = null, EntityUid? user = null, bool predicted = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<DoorComponent>(uid, ref door, true) && SetState(uid, DoorState.Closing, door))
		{
			if (predicted)
			{
				Audio.PlayPredicted(door.CloseSound, uid, user, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(-5f));
			}
			else if (_net.IsServer)
			{
				Audio.PlayPvs(door.CloseSound, uid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(-5f));
			}
		}
	}

	public bool OnPartialClose(EntityUid uid, DoorComponent? door = null, PhysicsComponent? physics = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DoorComponent, PhysicsComponent>(uid, ref door, ref physics, true))
		{
			return false;
		}
		if (!CanClose(uid, door, null, partial: true))
		{
			door.NextStateChange = GameTiming.CurTime + door.OpenTimeTwo;
			door.State = DoorState.Open;
			AppearanceSystem.SetData(uid, (Enum)DoorVisuals.State, (object)DoorState.Open, (AppearanceComponent)null);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)door, (MetaDataComponent)null);
			return false;
		}
		door.Partial = true;
		SetCollidable(uid, collidable: true, door, physics);
		door.NextStateChange = GameTiming.CurTime + door.CloseTimeTwo;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)door, (MetaDataComponent)null);
		_activeDoors.Add(Entity<DoorComponent>.op_Implicit((uid, door)));
		Crush(uid, door, physics);
		return true;
	}

	protected virtual void SetCollidable(EntityUid uid, bool collidable, DoorComponent? door = null, PhysicsComponent? physics = null, OccluderComponent? occluder = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<DoorComponent>(uid, ref door, true))
		{
			if (((EntitySystem)this).Resolve<PhysicsComponent>(uid, ref physics, false))
			{
				PhysicsSystem.SetCanCollide(uid, collidable, true, false, (FixturesComponent)null, physics);
			}
			if (!collidable)
			{
				door.CurrentlyCrushing.Clear();
			}
			if (door.Occludes)
			{
				_occluder.SetEnabled(uid, collidable, occluder, (MetaDataComponent)null);
			}
		}
	}

	public void Crush(EntityUid uid, DoorComponent? door = null, PhysicsComponent? physics = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DoorComponent>(uid, ref door, true) || !door.CanCrush)
		{
			return;
		}
		TimeSpan stunTime = door.DoorStunTime + door.OpenTimeOne;
		foreach (EntityUid entity in GetColliding(uid, physics))
		{
			door.CurrentlyCrushing.Add(entity);
			if (door.CrushDamage != null)
			{
				_damageableSystem.TryChangeDamage(entity, door.CrushDamage, ignoreResistances: false, interruptsDoAfters: true, null, uid);
			}
			_stunSystem.TryParalyze(entity, stunTime, refresh: true);
		}
		if (door.CurrentlyCrushing.Count != 0)
		{
			door.NextStateChange = GameTiming.CurTime + door.DoorStunTime;
			door.Partial = false;
		}
	}

	public IEnumerable<EntityUid> GetColliding(EntityUid uid, PhysicsComponent? physics = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PhysicsComponent>(uid, ref physics, true))
		{
			yield break;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(uid);
		MapGridComponent mapGridComp = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(xform.GridUid, ref mapGridComp))
		{
			yield break;
		}
		TileRef tileRef = _mapSystem.GetTileRef(xform.GridUid.Value, mapGridComp, xform.Coordinates);
		_doorIntersecting.Clear();
		_entityLookup.GetLocalEntitiesIntersecting<PhysicsComponent>(xform.GridUid.Value, tileRef.GridIndices, _doorIntersecting, -0.04f, (LookupFlags)46, mapGridComp);
		foreach (Entity<PhysicsComponent> otherPhysics in _doorIntersecting)
		{
			if (otherPhysics.Comp != physics && otherPhysics.Comp.CanCollide && otherPhysics.Comp.CollisionLayer != 222 && otherPhysics.Comp.CollisionLayer != 204 && otherPhysics.Comp.CollisionLayer != 4 && (otherPhysics.Comp.CollisionMask & 0x10) != 0 && otherPhysics.Comp.CollisionLayer != 278 && ((physics.CollisionMask & otherPhysics.Comp.CollisionLayer) != 0 || (otherPhysics.Comp.CollisionMask & physics.CollisionLayer) != 0) && otherPhysics.Comp.CollisionLayer != 268435456)
			{
				yield return otherPhysics.Owner;
			}
		}
	}

	private void PreventCollision(EntityUid uid, DoorComponent component, ref PreventCollideEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (component.CurrentlyCrushing.Contains(args.OtherEntity))
		{
			args.Cancelled = true;
		}
	}

	private void HandleCollide(EntityUid uid, DoorComponent door, ref StartCollideEvent args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!door.BumpOpen)
		{
			return;
		}
		DoorState state = door.State;
		if ((state == DoorState.Closed || state == DoorState.Denying) ? true : false)
		{
			EntityUid otherUid = args.OtherEntity;
			if (Tags.HasTag(otherUid, DoorBumpTag))
			{
				TryOpen(uid, door, otherUid, predicted: true, door.State == DoorState.Denying);
			}
		}
	}

	public bool HasAccess(EntityUid uid, EntityUid? user = null, DoorComponent? door = null, AccessReaderComponent? access = null)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		if (!user.HasValue || AccessType == AccessTypes.AllowAll)
		{
			return true;
		}
		AirlockComponent airlock = default(AirlockComponent);
		if (((EntitySystem)this).TryComp<AirlockComponent>(uid, ref airlock) && airlock.EmergencyAccess)
		{
			return true;
		}
		FirelockComponent firelock = default(FirelockComponent);
		if (((EntitySystem)this).Resolve<DoorComponent>(uid, ref door, true) && door.State == DoorState.Closed && ((EntitySystem)this).TryComp<FirelockComponent>(uid, ref firelock))
		{
			return true;
		}
		if (!((EntitySystem)this).Resolve<AccessReaderComponent>(uid, ref access, false))
		{
			return true;
		}
		bool isExternal = access.AccessLists.Any((HashSet<ProtoId<AccessLevelPrototype>> list) => list.Contains(ProtoId<AccessLevelPrototype>.op_Implicit("External")));
		return AccessType switch
		{
			AccessTypes.AllowAllIdExternal => !isExternal || _accessReaderSystem.IsAllowed(user.Value, uid, access), 
			AccessTypes.AllowAllNoExternal => !isExternal, 
			_ => _accessReaderSystem.IsAllowed(user.Value, uid, access), 
		};
	}

	public void SetNextStateChange(EntityUid uid, TimeSpan? delay, DoorComponent? door = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<DoorComponent>(uid, ref door, false) && (door.State == DoorState.Open || door.State == DoorState.Closed))
		{
			if (!delay.HasValue || delay.Value <= TimeSpan.Zero)
			{
				door.NextStateChange = null;
				_activeDoors.Remove(Entity<DoorComponent>.op_Implicit((uid, door)));
			}
			else
			{
				door.NextStateChange = GameTiming.CurTime + delay.Value;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)door, (MetaDataComponent)null);
				_activeDoors.Add(Entity<DoorComponent>.op_Implicit((uid, door)));
			}
		}
	}

	protected void CheckDoorBump(Entity<DoorComponent, PhysicsComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		Entity<DoorComponent, PhysicsComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		DoorComponent doorComponent = default(DoorComponent);
		PhysicsComponent val3 = default(PhysicsComponent);
		val.Deconstruct(ref val2, ref doorComponent, ref val3);
		EntityUid uid = val2;
		DoorComponent door = doorComponent;
		PhysicsComponent physics = val3;
		if (!door.BumpOpen)
		{
			return;
		}
		foreach (EntityUid other in PhysicsSystem.GetContactingEntities(uid, physics, false))
		{
			if (Tags.HasTag(other, DoorBumpTag) && TryOpen(uid, door, other, predicted: false, quiet: true))
			{
				break;
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = GameTiming.CurTime;
		PhysicsComponent doorBody = default(PhysicsComponent);
		foreach (Entity<DoorComponent> ent in _activeDoors.ToList())
		{
			DoorComponent door = ent.Comp;
			if (((Component)door).Deleted || !door.NextStateChange.HasValue)
			{
				_activeDoors.Remove(ent);
			}
			else if (!((EntitySystem)this).Paused(Entity<DoorComponent>.op_Implicit(ent), (MetaDataComponent)null))
			{
				if (door.NextStateChange.Value < time)
				{
					NextState(ent, time);
				}
				if (door.State == DoorState.Closed && ((EntitySystem)this).TryComp<PhysicsComponent>(Entity<DoorComponent>.op_Implicit(ent), ref doorBody))
				{
					_activeDoors.Remove(ent);
					CheckDoorBump(Entity<DoorComponent, PhysicsComponent>.op_Implicit((Entity<DoorComponent>.op_Implicit(ent), door, doorBody)));
				}
			}
		}
	}

	private void NextState(Entity<DoorComponent> ent, TimeSpan time)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		DoorComponent door = ent.Comp;
		door.NextStateChange = null;
		if (door.CurrentlyCrushing.Count > 0 && door.State != DoorState.Opening)
		{
			StartOpening(Entity<DoorComponent>.op_Implicit(ent), door);
			return;
		}
		switch (door.State)
		{
		case DoorState.Opening:
			if (door.Partial)
			{
				SetState(Entity<DoorComponent>.op_Implicit(ent), DoorState.Open, door);
			}
			else
			{
				OnPartialOpen(Entity<DoorComponent>.op_Implicit(ent), door);
			}
			break;
		case DoorState.Closing:
			if (door.Partial)
			{
				SetState(Entity<DoorComponent>.op_Implicit(ent), DoorState.Closed, door);
			}
			else
			{
				OnPartialClose(Entity<DoorComponent>.op_Implicit(ent), door);
			}
			break;
		case DoorState.Denying:
			SetState(Entity<DoorComponent>.op_Implicit(ent), DoorState.Closed, door);
			break;
		case DoorState.Emagging:
			StartOpening(Entity<DoorComponent>.op_Implicit(ent), door);
			break;
		case DoorState.Open:
			if (!TryClose(Entity<DoorComponent>.op_Implicit(ent), door))
			{
				door.NextStateChange = time + TimeSpan.FromSeconds(1L);
			}
			break;
		case DoorState.Welded:
			((EntitySystem)this).Log.Error($"Welded door was in the list of active doors. Door: {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<DoorComponent>.op_Implicit(ent), (MetaDataComponent)null)}");
			break;
		}
	}
}
