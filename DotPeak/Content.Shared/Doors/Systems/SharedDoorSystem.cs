// Decompiled with JetBrains decompiler
// Type: Content.Shared.Doors.Systems.SharedDoorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
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
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Doors.Systems;

public abstract class SharedDoorSystem : EntitySystem
{
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
  public static readonly ProtoId<TagPrototype> DoorBumpTag = (ProtoId<TagPrototype>) "DoorBumpOpener";
  private readonly HashSet<Entity<DoorComponent>> _activeDoors = new HashSet<Entity<DoorComponent>>();
  private readonly HashSet<Entity<PhysicsComponent>> _doorIntersecting = new HashSet<Entity<PhysicsComponent>>();
  public SharedDoorSystem.AccessTypes AccessType;

  public void InitializeBolts()
  {
    base.Initialize();
    this.SubscribeLocalEvent<DoorBoltComponent, BeforeDoorOpenedEvent>(new ComponentEventHandler<DoorBoltComponent, BeforeDoorOpenedEvent>(this.OnBeforeDoorOpened));
    this.SubscribeLocalEvent<DoorBoltComponent, BeforeDoorClosedEvent>(new ComponentEventHandler<DoorBoltComponent, BeforeDoorClosedEvent>(this.OnBeforeDoorClosed));
    this.SubscribeLocalEvent<DoorBoltComponent, BeforeDoorDeniedEvent>(new ComponentEventHandler<DoorBoltComponent, BeforeDoorDeniedEvent>(this.OnBeforeDoorDenied));
    this.SubscribeLocalEvent<DoorBoltComponent, BeforePryEvent>(new ComponentEventRefHandler<DoorBoltComponent, BeforePryEvent>(this.OnDoorPry));
    this.SubscribeLocalEvent<DoorBoltComponent, DoorStateChangedEvent>(new EntityEventRefHandler<DoorBoltComponent, DoorStateChangedEvent>(this.OnStateChanged));
  }

  private void OnDoorPry(EntityUid uid, DoorBoltComponent component, ref BeforePryEvent args)
  {
    if (args.Cancelled || !component.BoltsDown || args.Force)
      return;
    args.Message = "airlock-component-cannot-pry-is-bolted-message";
    args.Cancelled = true;
  }

  private void OnBeforeDoorOpened(
    EntityUid uid,
    DoorBoltComponent component,
    BeforeDoorOpenedEvent args)
  {
    if (!component.BoltsDown)
      return;
    args.Cancel();
  }

  private void OnBeforeDoorClosed(
    EntityUid uid,
    DoorBoltComponent component,
    BeforeDoorClosedEvent args)
  {
    if (!component.BoltsDown)
      return;
    args.Cancel();
  }

  private void OnBeforeDoorDenied(
    EntityUid uid,
    DoorBoltComponent component,
    BeforeDoorDeniedEvent args)
  {
    if (!component.BoltsDown)
      return;
    args.Cancel();
  }

  public void SetBoltWireCut(Entity<DoorBoltComponent> ent, bool value)
  {
    ent.Comp.BoltWireCut = value;
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
  }

  public void UpdateBoltLightStatus(Entity<DoorBoltComponent> ent)
  {
    this.AppearanceSystem.SetData((EntityUid) ent, (Enum) DoorVisuals.BoltLights, (object) this.GetBoltLightsVisible(ent));
  }

  public bool GetBoltLightsVisible(Entity<DoorBoltComponent> ent)
  {
    return ent.Comp.BoltLightsEnabled && ent.Comp.BoltsDown && ent.Comp.Powered;
  }

  public void SetBoltLightsEnabled(Entity<DoorBoltComponent> ent, bool value)
  {
    if (ent.Comp.BoltLightsEnabled == value)
      return;
    ent.Comp.BoltLightsEnabled = value;
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
    this.UpdateBoltLightStatus(ent);
  }

  public void SetBoltsDown(
    Entity<DoorBoltComponent> ent,
    bool value,
    EntityUid? user = null,
    bool predicted = false)
  {
    this.TrySetBoltDown(ent, value, user, predicted);
  }

  public bool TrySetBoltDown(
    Entity<DoorBoltComponent> ent,
    bool value,
    EntityUid? user = null,
    bool predicted = false)
  {
    if (!this._powerReceiver.IsPowered((Entity<SharedApcPowerReceiverComponent>) ent.Owner) || ent.Comp.BoltsDown == value)
      return false;
    ent.Comp.BoltsDown = value;
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
    this.UpdateBoltLightStatus(ent);
    DoorBoltsChangedEvent args = new DoorBoltsChangedEvent(value);
    this.RaiseLocalEvent<DoorBoltsChangedEvent>(ent.Owner, args);
    SoundSpecifier sound = value ? ent.Comp.BoltDownSound : ent.Comp.BoltUpSound;
    if (predicted)
      this.Audio.PlayPredicted(sound, (EntityUid) ent, user);
    else
      this.Audio.PlayPvs(sound, (EntityUid) ent);
    return true;
  }

  private void OnStateChanged(Entity<DoorBoltComponent> entity, ref DoorStateChangedEvent args)
  {
    this.UpdateBoltLightStatus(entity);
  }

  public bool IsBolted(EntityUid uid, DoorBoltComponent? component = null)
  {
    return this.Resolve<DoorBoltComponent>(uid, ref component, false) && component.BoltsDown;
  }

  public override void Initialize()
  {
    base.Initialize();
    this.InitializeBolts();
    this.SubscribeLocalEvent<DoorComponent, ComponentInit>(new EntityEventRefHandler<DoorComponent, ComponentInit>(this.OnComponentInit));
    this.SubscribeLocalEvent<DoorComponent, ComponentRemove>(new EntityEventRefHandler<DoorComponent, ComponentRemove>(this.OnRemove));
    this.SubscribeLocalEvent<DoorComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<DoorComponent, AfterAutoHandleStateEvent>(this.OnHandleState));
    this.SubscribeLocalEvent<DoorComponent, ActivateInWorldEvent>(new ComponentEventHandler<DoorComponent, ActivateInWorldEvent>(this.OnActivate));
    this.SubscribeLocalEvent<DoorComponent, StartCollideEvent>(new ComponentEventRefHandler<DoorComponent, StartCollideEvent>(this.HandleCollide));
    this.SubscribeLocalEvent<DoorComponent, PreventCollideEvent>(new ComponentEventRefHandler<DoorComponent, PreventCollideEvent>(this.PreventCollision));
    this.SubscribeLocalEvent<DoorComponent, BeforePryEvent>(new ComponentEventRefHandler<DoorComponent, BeforePryEvent>(this.OnBeforePry));
    this.SubscribeLocalEvent<DoorComponent, PriedEvent>(new ComponentEventRefHandler<DoorComponent, PriedEvent>(this.OnAfterPry));
    this.SubscribeLocalEvent<DoorComponent, WeldableAttemptEvent>(new ComponentEventHandler<DoorComponent, WeldableAttemptEvent>(this.OnWeldAttempt));
    this.SubscribeLocalEvent<DoorComponent, WeldableChangedEvent>(new ComponentEventRefHandler<DoorComponent, WeldableChangedEvent>(this.OnWeldChanged));
    this.SubscribeLocalEvent<DoorComponent, GetPryTimeModifierEvent>(new ComponentEventRefHandler<DoorComponent, GetPryTimeModifierEvent>(this.OnPryTimeModifier));
    this.SubscribeLocalEvent<DoorComponent, GotEmaggedEvent>(new ComponentEventRefHandler<DoorComponent, GotEmaggedEvent>(this.OnEmagged));
  }

  protected virtual void OnComponentInit(Entity<DoorComponent> ent, ref ComponentInit args)
  {
    DoorComponent comp = ent.Comp;
    if (comp.NextStateChange.HasValue)
    {
      this._activeDoors.Add(ent);
    }
    else
    {
      if (comp.State == DoorState.Opening)
      {
        comp.State = DoorState.Open;
        comp.Partial = false;
      }
      if (comp.State == DoorState.Closing)
      {
        comp.State = DoorState.Closed;
        comp.Partial = false;
      }
    }
    bool collidable = comp.State == DoorState.Closed || comp.State == DoorState.Closing && comp.Partial || comp.State == DoorState.Opening && !comp.Partial;
    this.SetCollidable((EntityUid) ent, collidable, comp);
    this.AppearanceSystem.SetData((EntityUid) ent, (Enum) DoorVisuals.State, (object) comp.State);
  }

  private void OnRemove(Entity<DoorComponent> door, ref ComponentRemove args)
  {
    this._activeDoors.Remove(door);
  }

  private void OnEmagged(EntityUid uid, DoorComponent door, ref GotEmaggedEvent args)
  {
    AirlockComponent comp;
    if (!this._emag.CompareFlag(args.Type, EmagType.Access) || !this.TryComp<AirlockComponent>(uid, out comp) || this.IsBolted(uid) || !comp.Powered || door.State != DoorState.Closed || !this.SetState(uid, DoorState.Emagging, door))
      return;
    args.Repeatable = true;
    args.Handled = true;
  }

  private void OnHandleState(Entity<DoorComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    DoorComponent comp = ent.Comp;
    if (!comp.NextStateChange.HasValue)
      this._activeDoors.Remove(ent);
    else
      this._activeDoors.Add(ent);
    this.RaiseLocalEvent<DoorStateChangedEvent>((EntityUid) ent, new DoorStateChangedEvent(comp.State));
  }

  public bool SetState(EntityUid uid, DoorState state, DoorComponent? door = null)
  {
    if (!this.Resolve<DoorComponent>(uid, ref door) || state == door.State)
      return false;
    switch (state)
    {
      case DoorState.Closed:
        door.Partial = false;
        break;
      case DoorState.Closing:
        this._activeDoors.Add((Entity<DoorComponent>) (uid, door));
        door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.CloseTimeOne);
        break;
      case DoorState.Open:
        door.Partial = false;
        if (!door.NextStateChange.HasValue)
        {
          this._activeDoors.Remove((Entity<DoorComponent>) (uid, door));
          break;
        }
        break;
      case DoorState.Opening:
        this._activeDoors.Add((Entity<DoorComponent>) (uid, door));
        door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.OpenTimeOne);
        break;
      case DoorState.Denying:
        this._activeDoors.Add((Entity<DoorComponent>) (uid, door));
        door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.DenyDuration);
        break;
      case DoorState.Emagging:
        this._activeDoors.Add((Entity<DoorComponent>) (uid, door));
        door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.EmagDuration);
        break;
    }
    door.State = state;
    this.Dirty(uid, (IComponent) door);
    this.RaiseLocalEvent<DoorStateChangedEvent>(uid, new DoorStateChangedEvent(state));
    this.AppearanceSystem.SetData(uid, (Enum) DoorVisuals.State, (object) door.State);
    return true;
  }

  protected void OnActivate(EntityUid uid, DoorComponent door, ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex || !door.ClickOpen)
      return;
    if (!this.TryToggleDoor(uid, door, new EntityUid?(args.User), true))
      this._pryingSystem.TryPry(uid, args.User, out DoAfterId? _);
    args.Handled = true;
  }

  private void OnPryTimeModifier(
    EntityUid uid,
    DoorComponent door,
    ref GetPryTimeModifierEvent args)
  {
    args.BaseTime = door.PryTime;
  }

  private void OnBeforePry(EntityUid uid, DoorComponent door, ref BeforePryEvent args)
  {
    if (door.State == DoorState.Welded || !door.CanPry)
      args.Cancelled = true;
    RMCBeforePryEvent args1 = new RMCBeforePryEvent(args.User);
    this.RaiseLocalEvent<RMCBeforePryEvent>(uid, ref args1);
    if (!args1.Cancelled)
      return;
    args.Cancelled = true;
  }

  private void OnAfterPry(EntityUid uid, DoorComponent door, ref PriedEvent args)
  {
    if (door.State == DoorState.Closed)
    {
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(12, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "ToPrettyString(args.User)");
      logStringHandler.AppendLiteral(" pried ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "ToPrettyString(uid)");
      logStringHandler.AppendLiteral(" open");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.Action, LogImpact.Medium, ref local);
      this.StartOpening(uid, door, new EntityUid?(args.User), true);
    }
    else
    {
      if (door.State != DoorState.Open)
        return;
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(14, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "ToPrettyString(args.User)");
      logStringHandler.AppendLiteral(" pried ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "ToPrettyString(uid)");
      logStringHandler.AppendLiteral(" closed");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.Action, LogImpact.Medium, ref local);
      this.StartClosing(uid, door, new EntityUid?(args.User), true);
    }
  }

  private void OnWeldAttempt(EntityUid uid, DoorComponent component, WeldableAttemptEvent args)
  {
    if (component.CurrentlyCrushing.Count > 0)
    {
      args.Cancel();
    }
    else
    {
      if (component.State == DoorState.Closed || component.State == DoorState.Welded)
        return;
      args.Cancel();
    }
  }

  private void OnWeldChanged(EntityUid uid, DoorComponent component, ref WeldableChangedEvent args)
  {
    if (component.State == DoorState.Closed)
    {
      this.SetState(uid, DoorState.Welded, component);
    }
    else
    {
      if (component.State != DoorState.Welded)
        return;
      this.SetState(uid, DoorState.Closed, component);
    }
  }

  public void Deny(EntityUid uid, DoorComponent? door = null, EntityUid? user = null, bool predicted = false)
  {
    if (!this.Resolve<DoorComponent>(uid, ref door) || door.State != DoorState.Closed)
      return;
    BeforeDoorDeniedEvent args = new BeforeDoorDeniedEvent();
    this.RaiseLocalEvent<BeforeDoorDeniedEvent>(uid, args);
    if (args.Cancelled || !this.SetState(uid, DoorState.Denying, door))
      return;
    if (predicted)
    {
      this.Audio.PlayPredicted(door.DenySound, uid, user, new AudioParams?(AudioParams.Default.WithVolume(-3f)));
    }
    else
    {
      if (!this._net.IsServer)
        return;
      this.Audio.PlayPvs(door.DenySound, uid, new AudioParams?(AudioParams.Default.WithVolume(-3f)));
    }
  }

  public bool TryToggleDoor(EntityUid uid, DoorComponent? door = null, EntityUid? user = null, bool predicted = false)
  {
    if (!this.Resolve<DoorComponent>(uid, ref door))
      return false;
    bool flag;
    switch (door.State)
    {
      case DoorState.Closed:
      case DoorState.Denying:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    if (flag)
      return this.TryOpen(uid, door, user, predicted, door.State == DoorState.Denying);
    return door.State == DoorState.Open && this.TryClose(uid, door, user, predicted);
  }

  public bool TryOpen(
    EntityUid uid,
    DoorComponent? door = null,
    EntityUid? user = null,
    bool predicted = false,
    bool quiet = false)
  {
    if (!this.Resolve<DoorComponent>(uid, ref door) || !this.CanOpen(uid, door, user, quiet))
      return false;
    this.StartOpening(uid, door, user, predicted);
    return true;
  }

  public bool CanOpen(EntityUid uid, DoorComponent? door = null, EntityUid? user = null, bool quiet = true)
  {
    if (!this.Resolve<DoorComponent>(uid, ref door) || door.State == DoorState.Welded)
      return false;
    BeforeDoorOpenedEvent args = new BeforeDoorOpenedEvent()
    {
      User = user
    };
    this.RaiseLocalEvent<BeforeDoorOpenedEvent>(uid, args);
    if (args.Cancelled)
      return false;
    if (this.HasAccess(uid, user, door))
      return true;
    if (!quiet)
      this.Deny(uid, door, user, true);
    return false;
  }

  public void StartOpening(EntityUid uid, DoorComponent? door = null, EntityUid? user = null, bool predicted = false)
  {
    if (!this.Resolve<DoorComponent>(uid, ref door))
      return;
    DoorState state = door.State;
    if (!this.SetState(uid, DoorState.Opening, door))
      return;
    if (predicted)
      this.Audio.PlayPredicted(door.OpenSound, uid, user, new AudioParams?(AudioParams.Default.WithVolume(-5f)));
    else if (this._net.IsServer)
      this.Audio.PlayPvs(door.OpenSound, uid, new AudioParams?(AudioParams.Default.WithVolume(-5f)));
    DoorBoltComponent comp;
    if (state != DoorState.Emagging || !this.TryComp<DoorBoltComponent>(uid, out comp))
      return;
    this.SetBoltsDown((Entity<DoorBoltComponent>) (uid, comp), !comp.BoltsDown, user, true);
  }

  public void OnPartialOpen(EntityUid uid, DoorComponent? door = null)
  {
    if (!this.Resolve<DoorComponent>(uid, ref door))
      return;
    this.SetCollidable(uid, false, door);
    door.Partial = true;
    door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.CloseTimeTwo);
    this._activeDoors.Add((Entity<DoorComponent>) (uid, door));
    this.Dirty(uid, (IComponent) door);
  }

  public bool TryOpenAndBolt(EntityUid uid, DoorComponent? door = null, AirlockComponent? airlock = null)
  {
    if (!this.Resolve<DoorComponent, AirlockComponent>(uid, ref door, ref airlock) || this.IsBolted(uid) || !airlock.Powered || door.State != DoorState.Closed)
      return false;
    this.SetState(uid, DoorState.Emagging, door);
    return true;
  }

  public bool TryClose(EntityUid uid, DoorComponent? door = null, EntityUid? user = null, bool predicted = false)
  {
    if (!this.Resolve<DoorComponent>(uid, ref door) || !this.CanClose(uid, door, user))
      return false;
    this.StartClosing(uid, door, user, predicted);
    return true;
  }

  public bool CanClose(EntityUid uid, DoorComponent? door = null, EntityUid? user = null, bool partial = false)
  {
    if (!this.Resolve<DoorComponent>(uid, ref door))
      return false;
    bool flag;
    switch (door.State)
    {
      case DoorState.Closed:
      case DoorState.Welded:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    if (flag)
      return false;
    BeforeDoorClosedEvent args = new BeforeDoorClosedEvent(door.PerformCollisionCheck, partial);
    this.RaiseLocalEvent<BeforeDoorClosedEvent>(uid, args);
    if (args.Cancelled || !this.HasAccess(uid, user, door))
      return false;
    return !args.PerformCollisionCheck || !this.GetColliding(uid).Any<EntityUid>();
  }

  public void StartClosing(EntityUid uid, DoorComponent? door = null, EntityUid? user = null, bool predicted = false)
  {
    if (!this.Resolve<DoorComponent>(uid, ref door) || !this.SetState(uid, DoorState.Closing, door))
      return;
    if (predicted)
    {
      this.Audio.PlayPredicted(door.CloseSound, uid, user, new AudioParams?(AudioParams.Default.WithVolume(-5f)));
    }
    else
    {
      if (!this._net.IsServer)
        return;
      this.Audio.PlayPvs(door.CloseSound, uid, new AudioParams?(AudioParams.Default.WithVolume(-5f)));
    }
  }

  public bool OnPartialClose(EntityUid uid, DoorComponent? door = null, PhysicsComponent? physics = null)
  {
    if (!this.Resolve<DoorComponent, PhysicsComponent>(uid, ref door, ref physics))
      return false;
    if (!this.CanClose(uid, door, partial: true))
    {
      door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.OpenTimeTwo);
      door.State = DoorState.Open;
      this.AppearanceSystem.SetData(uid, (Enum) DoorVisuals.State, (object) DoorState.Open);
      this.Dirty(uid, (IComponent) door);
      return false;
    }
    door.Partial = true;
    this.SetCollidable(uid, true, door, physics);
    door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.CloseTimeTwo);
    this.Dirty(uid, (IComponent) door);
    this._activeDoors.Add((Entity<DoorComponent>) (uid, door));
    this.Crush(uid, door, physics);
    return true;
  }

  protected virtual void SetCollidable(
    EntityUid uid,
    bool collidable,
    DoorComponent? door = null,
    PhysicsComponent? physics = null,
    OccluderComponent? occluder = null)
  {
    if (!this.Resolve<DoorComponent>(uid, ref door))
      return;
    if (this.Resolve<PhysicsComponent>(uid, ref physics, false))
      this.PhysicsSystem.SetCanCollide(uid, collidable, body: physics);
    if (!collidable)
      door.CurrentlyCrushing.Clear();
    if (!door.Occludes)
      return;
    this._occluder.SetEnabled(uid, collidable, occluder);
  }

  public void Crush(EntityUid uid, DoorComponent? door = null, PhysicsComponent? physics = null)
  {
    if (!this.Resolve<DoorComponent>(uid, ref door) || !door.CanCrush)
      return;
    TimeSpan time = door.DoorStunTime + door.OpenTimeOne;
    foreach (EntityUid uid1 in this.GetColliding(uid, physics))
    {
      door.CurrentlyCrushing.Add(uid1);
      if (door.CrushDamage != null)
        this._damageableSystem.TryChangeDamage(new EntityUid?(uid1), door.CrushDamage, origin: new EntityUid?(uid));
      this._stunSystem.TryParalyze(uid1, time, true);
    }
    if (door.CurrentlyCrushing.Count == 0)
      return;
    door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.DoorStunTime);
    door.Partial = false;
  }

  public IEnumerable<EntityUid> GetColliding(EntityUid uid, PhysicsComponent? physics = null)
  {
    SharedDoorSystem sharedDoorSystem = this;
    if (sharedDoorSystem.Resolve<PhysicsComponent>(uid, ref physics))
    {
      TransformComponent transformComponent = sharedDoorSystem.Transform(uid);
      MapGridComponent comp;
      if (sharedDoorSystem.TryComp<MapGridComponent>(transformComponent.GridUid, out comp))
      {
        SharedMapSystem mapSystem = sharedDoorSystem._mapSystem;
        EntityUid? gridUid1 = transformComponent.GridUid;
        EntityUid uid1 = gridUid1.Value;
        MapGridComponent grid = comp;
        EntityCoordinates coordinates = transformComponent.Coordinates;
        TileRef tileRef = mapSystem.GetTileRef(uid1, grid, coordinates);
        sharedDoorSystem._doorIntersecting.Clear();
        EntityLookupSystem entityLookup = sharedDoorSystem._entityLookup;
        gridUid1 = transformComponent.GridUid;
        EntityUid gridUid2 = gridUid1.Value;
        Vector2i gridIndices = tileRef.GridIndices;
        HashSet<Entity<PhysicsComponent>> doorIntersecting = sharedDoorSystem._doorIntersecting;
        MapGridComponent gridComp = comp;
        entityLookup.GetLocalEntitiesIntersecting<PhysicsComponent>(gridUid2, gridIndices, doorIntersecting, flags: LookupFlags.StaticSundries | LookupFlags.Dynamic | LookupFlags.Contained, gridComp: gridComp);
        foreach (Entity<PhysicsComponent> entity in sharedDoorSystem._doorIntersecting)
        {
          if (entity.Comp != physics && entity.Comp.CanCollide && entity.Comp.CollisionLayer != 222 && entity.Comp.CollisionLayer != 204 && entity.Comp.CollisionLayer != 4 && (entity.Comp.CollisionMask & 16 /*0x10*/) != 0 && entity.Comp.CollisionLayer != 278 && ((physics.CollisionMask & entity.Comp.CollisionLayer) != 0 || (entity.Comp.CollisionMask & physics.CollisionLayer) != 0) && entity.Comp.CollisionLayer != 268435456 /*0x10000000*/)
            yield return entity.Owner;
        }
      }
    }
  }

  private void PreventCollision(
    EntityUid uid,
    DoorComponent component,
    ref PreventCollideEvent args)
  {
    if (!component.CurrentlyCrushing.Contains(args.OtherEntity))
      return;
    args.Cancelled = true;
  }

  private void HandleCollide(EntityUid uid, DoorComponent door, ref StartCollideEvent args)
  {
    if (!door.BumpOpen)
      return;
    bool flag;
    switch (door.State)
    {
      case DoorState.Closed:
      case DoorState.Denying:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    if (!flag)
      return;
    EntityUid otherEntity = args.OtherEntity;
    if (!this.Tags.HasTag(otherEntity, SharedDoorSystem.DoorBumpTag))
      return;
    this.TryOpen(uid, door, new EntityUid?(otherEntity), true, door.State == DoorState.Denying);
  }

  public bool HasAccess(
    EntityUid uid,
    EntityUid? user = null,
    DoorComponent? door = null,
    AccessReaderComponent? access = null)
  {
    AirlockComponent comp;
    if (!user.HasValue || this.AccessType == SharedDoorSystem.AccessTypes.AllowAll || this.TryComp<AirlockComponent>(uid, out comp) && comp.EmergencyAccess || this.Resolve<DoorComponent>(uid, ref door) && door.State == DoorState.Closed && this.TryComp<FirelockComponent>(uid, out FirelockComponent _) || !this.Resolve<AccessReaderComponent>(uid, ref access, false))
      return true;
    bool flag1 = access.AccessLists.Any<HashSet<ProtoId<AccessLevelPrototype>>>((Func<HashSet<ProtoId<AccessLevelPrototype>>, bool>) (list => list.Contains((ProtoId<AccessLevelPrototype>) "External")));
    bool flag2;
    switch (this.AccessType)
    {
      case SharedDoorSystem.AccessTypes.AllowAllIdExternal:
        flag2 = !flag1 || this._accessReaderSystem.IsAllowed(user.Value, uid, access);
        break;
      case SharedDoorSystem.AccessTypes.AllowAllNoExternal:
        flag2 = !flag1;
        break;
      default:
        flag2 = this._accessReaderSystem.IsAllowed(user.Value, uid, access);
        break;
    }
    return flag2;
  }

  public void SetNextStateChange(EntityUid uid, TimeSpan? delay, DoorComponent? door = null)
  {
    if (!this.Resolve<DoorComponent>(uid, ref door, false) || door.State != DoorState.Open && door.State != DoorState.Closed)
      return;
    if (!delay.HasValue || delay.Value <= TimeSpan.Zero)
    {
      door.NextStateChange = new TimeSpan?();
      this._activeDoors.Remove((Entity<DoorComponent>) (uid, door));
    }
    else
    {
      door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + delay.Value);
      this.Dirty(uid, (IComponent) door);
      this._activeDoors.Add((Entity<DoorComponent>) (uid, door));
    }
  }

  protected void CheckDoorBump(Entity<DoorComponent, PhysicsComponent> ent)
  {
    (EntityUid entityUid, DoorComponent doorComponent, PhysicsComponent physicsComponent) = ent;
    if (!doorComponent.BumpOpen)
      return;
    foreach (EntityUid contactingEntity in this.PhysicsSystem.GetContactingEntities(entityUid, physicsComponent))
    {
      if (this.Tags.HasTag(contactingEntity, SharedDoorSystem.DoorBumpTag) && this.TryOpen(entityUid, doorComponent, new EntityUid?(contactingEntity), quiet: true))
        break;
    }
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this.GameTiming.CurTime;
    foreach (Entity<DoorComponent> entity in this._activeDoors.ToList<Entity<DoorComponent>>())
    {
      DoorComponent comp1 = entity.Comp;
      if (comp1.Deleted || !comp1.NextStateChange.HasValue)
        this._activeDoors.Remove(entity);
      else if (!this.Paused((EntityUid) entity))
      {
        if (comp1.NextStateChange.Value < curTime)
          this.NextState(entity, curTime);
        PhysicsComponent comp2;
        if (comp1.State == DoorState.Closed && this.TryComp<PhysicsComponent>((EntityUid) entity, out comp2))
        {
          this._activeDoors.Remove(entity);
          this.CheckDoorBump((Entity<DoorComponent, PhysicsComponent>) ((EntityUid) entity, comp1, comp2));
        }
      }
    }
  }

  private void NextState(Entity<DoorComponent> ent, TimeSpan time)
  {
    DoorComponent comp = ent.Comp;
    comp.NextStateChange = new TimeSpan?();
    if (comp.CurrentlyCrushing.Count > 0 && comp.State != DoorState.Opening)
    {
      this.StartOpening((EntityUid) ent, comp);
    }
    else
    {
      switch (comp.State)
      {
        case DoorState.Closing:
          if (comp.Partial)
          {
            this.SetState((EntityUid) ent, DoorState.Closed, comp);
            break;
          }
          this.OnPartialClose((EntityUid) ent, comp);
          break;
        case DoorState.Open:
          if (this.TryClose((EntityUid) ent, comp))
            break;
          comp.NextStateChange = new TimeSpan?(time + TimeSpan.FromSeconds(1L));
          break;
        case DoorState.Opening:
          if (comp.Partial)
          {
            this.SetState((EntityUid) ent, DoorState.Open, comp);
            break;
          }
          this.OnPartialOpen((EntityUid) ent, comp);
          break;
        case DoorState.Welded:
          this.Log.Error($"Welded door was in the list of active doors. Door: {this.ToPrettyString(new EntityUid?((EntityUid) ent))}");
          break;
        case DoorState.Denying:
          this.SetState((EntityUid) ent, DoorState.Closed, comp);
          break;
        case DoorState.Emagging:
          this.StartOpening((EntityUid) ent, comp);
          break;
      }
    }
  }

  public enum AccessTypes
  {
    Id,
    AllowAllIdExternal,
    AllowAllNoExternal,
    AllowAll,
  }
}
