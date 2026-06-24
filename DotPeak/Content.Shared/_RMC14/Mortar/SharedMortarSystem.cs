// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Mortar.SharedMortarSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Camera;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Extensions;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Rangefinder;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Administration.Logs;
using Content.Shared.Chat;
using Content.Shared.Construction.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.UserInterface;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Mortar;

public abstract class SharedMortarSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogs;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private FixtureSystem _fixture;
  [Dependency]
  private MetaDataSystem _metaData;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private ISharedPlayerManager _player;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedCMChatSystem _rmcChat;
  [Dependency]
  private SharedRMCExplosionSystem _rmcExplosion;
  [Dependency]
  private RMCCameraShakeSystem _rmcCameraShake;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private RMCPlanetSystem _rmcPlanet;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  private Robust.Shared.GameObjects.EntityQuery<TransformComponent> _transformQuery;

  public override void Initialize()
  {
    this._transformQuery = this.GetEntityQuery<TransformComponent>();
    this.SubscribeLocalEvent<MortarComponent, UseInHandEvent>(new EntityEventRefHandler<MortarComponent, UseInHandEvent>(this.OnMortarUseInHand), new Type[1]
    {
      typeof (ActivatableUISystem)
    });
    this.SubscribeLocalEvent<MortarComponent, DeployMortarDoAfterEvent>(new EntityEventRefHandler<MortarComponent, DeployMortarDoAfterEvent>(this.OnMortarDeployDoAfter));
    this.SubscribeLocalEvent<MortarComponent, TargetMortarDoAfterEvent>(new EntityEventRefHandler<MortarComponent, TargetMortarDoAfterEvent>(this.OnMortarTargetDoAfter));
    this.SubscribeLocalEvent<MortarComponent, DialMortarDoAfterEvent>(new EntityEventRefHandler<MortarComponent, DialMortarDoAfterEvent>(this.OnMortarDialDoAfter));
    this.SubscribeLocalEvent<MortarComponent, InteractUsingEvent>(new EntityEventRefHandler<MortarComponent, InteractUsingEvent>(this.OnMortarInteractUsing));
    this.SubscribeLocalEvent<MortarComponent, LoadMortarShellDoAfterEvent>(new EntityEventRefHandler<MortarComponent, LoadMortarShellDoAfterEvent>(this.OnMortarLoadDoAfter));
    this.SubscribeLocalEvent<MortarComponent, UnanchorAttemptEvent>(new EntityEventRefHandler<MortarComponent, UnanchorAttemptEvent>(this.OnMortarUnanchorAttempt));
    this.SubscribeLocalEvent<MortarComponent, AnchorStateChangedEvent>(new EntityEventRefHandler<MortarComponent, AnchorStateChangedEvent>(this.OnMortarAnchorStateChanged));
    this.SubscribeLocalEvent<MortarComponent, ExaminedEvent>(new EntityEventRefHandler<MortarComponent, ExaminedEvent>(this.OnMortarExamined));
    this.SubscribeLocalEvent<MortarComponent, ActivatableUIOpenAttemptEvent>(new EntityEventRefHandler<MortarComponent, ActivatableUIOpenAttemptEvent>(this.OnMortarActivatableUIOpenAttempt));
    this.SubscribeLocalEvent<MortarComponent, CombatModeShouldHandInteractEvent>(new EntityEventRefHandler<MortarComponent, CombatModeShouldHandInteractEvent>(this.OnMortarShouldInteract));
    this.SubscribeLocalEvent<MortarComponent, DestructionEventArgs>(new EntityEventRefHandler<MortarComponent, DestructionEventArgs>(this.OnMortarDestruction));
    this.SubscribeLocalEvent<MortarComponent, BeforeDamageChangedEvent>(new EntityEventRefHandler<MortarComponent, BeforeDamageChangedEvent>(this.OnMortarBeforeDamageChanged));
    this.SubscribeLocalEvent<MortarComponent, LinkMortarLaserDesignatorDoAfterEvent>(new EntityEventRefHandler<MortarComponent, LinkMortarLaserDesignatorDoAfterEvent>(this.OnMortarLinkLaserDesignatorDoAfter));
    this.SubscribeLocalEvent<MortarComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<MortarComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetMortarVerbs));
    this.SubscribeLocalEvent<MortarComponent, MortarLaserTargetUpdateDoAfterEvent>(new EntityEventRefHandler<MortarComponent, MortarLaserTargetUpdateDoAfterEvent>(this.OnMortarLaserTargetUpdateDoAfter));
    this.SubscribeLocalEvent<MortarComponent, DoAfterAttemptEvent<MortarLaserTargetUpdateDoAfterEvent>>(new EntityEventRefHandler<MortarComponent, DoAfterAttemptEvent<MortarLaserTargetUpdateDoAfterEvent>>(this.OnMortarLaserTargetUpdateAttempt));
    this.SubscribeLocalEvent<ActivateMortarShellOnSpawnComponent, MapInitEvent>(new EntityEventRefHandler<ActivateMortarShellOnSpawnComponent, MapInitEvent>(this.OnMortarExplosionOnSpawn));
    this.SubscribeLocalEvent<MortarCameraShellComponent, MortarShellLandEvent>(new EntityEventRefHandler<MortarCameraShellComponent, MortarShellLandEvent>(this.OnMortarCameraShellLand));
    this.SubscribeLocalEvent<ActiveLaserDesignatorComponent, ComponentShutdown>(new EntityEventRefHandler<ActiveLaserDesignatorComponent, ComponentShutdown>(this.OnActiveLaserDesignatorShutdown));
    this.SubscribeLocalEvent<RangefinderComponent, ComponentShutdown>(new EntityEventRefHandler<RangefinderComponent, ComponentShutdown>(this.OnRangefinderShutdown));
    this.Subs.BuiEvents<MortarComponent>((object) MortarUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<MortarComponent>) (subs =>
    {
      subs.Event<MortarTargetBuiMsg>(new EntityEventRefHandler<MortarComponent, MortarTargetBuiMsg>(this.OnMortarTargetBui));
      subs.Event<MortarDialBuiMsg>(new EntityEventRefHandler<MortarComponent, MortarDialBuiMsg>(this.OnMortarDialBui));
      subs.Event<MortarViewCamerasMsg>(new EntityEventRefHandler<MortarComponent, MortarViewCamerasMsg>(this.OnMortarViewCameras));
    }));
  }

  private void OnMortarBeforeDamageChanged(
    Entity<MortarComponent> ent,
    ref BeforeDamageChangedEvent args)
  {
    if (ent.Comp.Deployed)
      return;
    args.Cancelled = true;
  }

  private void OnMortarDestruction(Entity<MortarComponent> mortar, ref DestructionEventArgs args)
  {
    if (!mortar.Comp.Deployed || this._net.IsClient)
      return;
    this.SpawnAtPosition((string) mortar.Comp.Drop, mortar.Owner.ToCoordinates());
  }

  private void OnMortarUseInHand(Entity<MortarComponent> mortar, ref UseInHandEvent args)
  {
    args.Handled = true;
    this.DeployMortar(mortar, args.User);
  }

  private void OnMortarDeployDoAfter(
    Entity<MortarComponent> mortar,
    ref DeployMortarDoAfterEvent args)
  {
    EntityUid user = args.User;
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    if (mortar.Comp.Deployed || !this.CanDeployPopup(mortar, user))
      return;
    mortar.Comp.Deployed = true;
    this.Dirty<MortarComponent>(mortar);
    Fixture fixtureOrNull = this._fixture.GetFixtureOrNull((EntityUid) mortar, mortar.Comp.FixtureId);
    if (fixtureOrNull != null)
      this._physics.SetHard((EntityUid) mortar, fixtureOrNull, true);
    this._appearance.SetData((EntityUid) mortar, (Enum) MortarVisualLayers.State, (object) MortarVisuals.Deployed);
    TransformComponent xform = this.Transform((EntityUid) mortar);
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) mortar, xform);
    Angle localRotation = this.Transform(user).LocalRotation;
    Angle angle = DirectionExtensions.ToAngle(((Angle) ref localRotation).GetCardinalDir());
    this._transform.SetCoordinates((EntityUid) mortar, xform, moverCoordinates, new Angle?(angle));
    this._transform.AnchorEntity((Entity<TransformComponent>) ((EntityUid) mortar, xform));
    if (!this._rmcPlanet.IsOnPlanet(moverCoordinates))
      this._popup.PopupClient(this.Loc.GetString("rmc-mortar-deploy-end-not-planet"), user, new EntityUid?(user), PopupType.MediumCaution);
    this._audio.PlayPredicted(mortar.Comp.DeploySound, (EntityUid) mortar, new EntityUid?(user));
  }

  private void OnMortarTargetDoAfter(
    Entity<MortarComponent> mortar,
    ref TargetMortarDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    EntityUid user = args.User;
    this._popup.PopupPredicted(this.Loc.GetString("rmc-mortar-target-finish-self", (nameof (mortar), (object) mortar)), this.Loc.GetString("rmc-mortar-target-finish-others", ("user", (object) user), (nameof (mortar), (object) mortar)), user, new EntityUid?(user));
    if (this._net.IsClient)
      return;
    Vector2i vector = args.Vector;
    Vector2 position = this._transform.GetMapCoordinates((EntityUid) mortar).Position;
    Vector2i vector2i = vector;
    Vector2i offset;
    if (this._rmcPlanet.TryGetOffset(this._transform.GetMapCoordinates(mortar.Owner), out offset))
      vector2i = Vector2i.op_Subtraction(vector2i, offset);
    mortar.Comp.Target = vector;
    int tilesPerOffset = mortar.Comp.TilesPerOffset;
    int num1 = (int) Math.Floor((double) Math.Abs((float) vector2i.X - position.X) / (double) tilesPerOffset);
    int num2 = (int) Math.Floor((double) Math.Abs((float) vector2i.Y - position.Y) / (double) tilesPerOffset);
    mortar.Comp.Offset = Vector2i.op_Implicit((this._random.Next(-num1, num1 + 1), this._random.Next(-num2, num2 + 1)));
    this.Dirty<MortarComponent>(mortar);
  }

  private void OnMortarDialDoAfter(Entity<MortarComponent> mortar, ref DialMortarDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    mortar.Comp.Dial = args.Vector;
    this.Dirty<MortarComponent>(mortar);
    EntityUid user = args.User;
    this._popup.PopupPredicted(this.Loc.GetString("rmc-mortar-dial-finish-self", (nameof (mortar), (object) mortar)), this.Loc.GetString("rmc-mortar-dial-finish-others", ("user", (object) user), (nameof (mortar), (object) mortar)), user, new EntityUid?(user));
  }

  private void OnMortarInteractUsing(Entity<MortarComponent> mortar, ref InteractUsingEvent args)
  {
    EntityUid used = args.Used;
    EntityUid user = args.User;
    RangefinderComponent comp1;
    if (this.TryComp<RangefinderComponent>(used, out comp1) && comp1.CanDesignate && mortar.Comp.LaserTargetingMode)
    {
      args.Handled = true;
      this.TryStartLinkLaserDesignator(mortar, used, user);
    }
    else
    {
      MortarShellComponent comp2;
      if (!this.TryComp<MortarShellComponent>(used, out comp2))
        return;
      args.Handled = true;
      if (!this.HasSkillPopup(mortar, user, true) || !this.CanLoadPopup(mortar, (Entity<MortarShellComponent>) (used, comp2), user, out TimeSpan _, out MapCoordinates _))
        return;
      LoadMortarShellDoAfterEvent @event = new LoadMortarShellDoAfterEvent();
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, comp2.LoadDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) mortar), new EntityUid?((EntityUid) mortar), new EntityUid?(used))
      {
        BreakOnMove = true,
        BreakOnHandChange = true,
        ForceVisible = true
      }))
        return;
      this._popup.PopupPredicted(this.Loc.GetString("rmc-mortar-shell-load-start-self", (nameof (mortar), (object) mortar), ("shell", (object) used)), this.Loc.GetString("rmc-mortar-shell-load-start-others", ("user", (object) user), (nameof (mortar), (object) mortar), ("shell", (object) used)), (EntityUid) mortar, new EntityUid?(user));
      if (!this._net.IsServer)
        return;
      this._audio.PlayPvs(mortar.Comp.ReloadSound, (EntityUid) mortar);
    }
  }

  private void OnMortarLoadDoAfter(
    Entity<MortarComponent> mortar,
    ref LoadMortarShellDoAfterEvent args)
  {
    EntityUid user = args.User;
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? nullable = args.Used;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    args.Handled = true;
    MortarShellComponent comp;
    TimeSpan travelTime;
    MapCoordinates coordinates;
    if (this._net.IsClient || !this.TryComp<MortarShellComponent>(valueOrDefault, out comp) || !mortar.Comp.Deployed || this.HasComp<ActiveMortarShellComponent>(valueOrDefault) || !this.CanLoadPopup(mortar, (Entity<MortarShellComponent>) (valueOrDefault, comp), user, out travelTime, out coordinates))
      return;
    ISharedAdminLogManager adminLogs = this._adminLogs;
    LogStringHandler logStringHandler = new LogStringHandler(33, 4);
    logStringHandler.AppendLiteral("Mortar ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) mortar)), "ToPrettyString(mortar)");
    logStringHandler.AppendLiteral(" shell ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) valueOrDefault), "ToPrettyString(shellId)");
    logStringHandler.AppendLiteral(" shot by ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" aimed at ");
    logStringHandler.AppendFormatted<MapCoordinates>(coordinates, "coordinates");
    ref LogStringHandler local = ref logStringHandler;
    adminLogs.Add(LogType.RMCMortar, LogImpact.High, ref local);
    Container container = this._container.EnsureContainer<Container>((EntityUid) mortar, mortar.Comp.ContainerId);
    if (!this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) valueOrDefault, (BaseContainer) container))
      return;
    TimeSpan curTime = this._timing.CurTime;
    mortar.Comp.LastFiredAt = curTime;
    ActiveMortarShellComponent component = new ActiveMortarShellComponent()
    {
      Coordinates = this._transform.ToCoordinates(coordinates),
      WarnAt = curTime + travelTime,
      ImpactWarnAt = curTime + travelTime + comp.ImpactWarningDelay,
      LandAt = curTime + travelTime + comp.ImpactDelay
    };
    this.AddComp<ActiveMortarShellComponent>(valueOrDefault, component, true);
    this._popup.PopupPredicted(this.Loc.GetString("rmc-mortar-shell-load-finish-self", (nameof (mortar), (object) mortar), ("shell", (object) valueOrDefault)), this.Loc.GetString("rmc-mortar-shell-load-finish-others", ("user", (object) user), (nameof (mortar), (object) mortar), ("shell", (object) valueOrDefault)), user, new EntityUid?(user));
    this._popup.PopupEntity(this.Loc.GetString("rmc-mortar-shell-fire", (nameof (mortar), (object) mortar)), (EntityUid) mortar, PopupType.MediumCaution);
    Filter filter = Filter.Pvs((EntityUid) mortar);
    this._audio.PlayPvs(mortar.Comp.FireSound, (EntityUid) mortar);
    this.RaiseNetworkEvent((EntityEventArgs) new MortarFiredEvent(this.GetNetEntity((EntityUid) mortar)), filter);
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(mortar.Owner.ToCoordinates());
    foreach (ICommonSession recipient in Filter.Empty().AddInRange(mapCoordinates, 7f).Recipients)
    {
      nullable = recipient.AttachedEntity;
      if (nullable.HasValue)
        this._rmcCameraShake.ShakeCamera(nullable.GetValueOrDefault(), 3, 1);
    }
  }

  private void OnMortarUnanchorAttempt(
    Entity<MortarComponent> mortar,
    ref UnanchorAttemptEvent args)
  {
    if (args.Cancelled || this.HasSkillPopup(mortar, args.User, true))
      return;
    args.Cancel();
  }

  private void OnMortarAnchorStateChanged(
    Entity<MortarComponent> mortar,
    ref AnchorStateChangedEvent args)
  {
    if (args.Anchored)
      return;
    mortar.Comp.Deployed = false;
    this.Dirty<MortarComponent>(mortar);
    Fixture fixtureOrNull = this._fixture.GetFixtureOrNull((EntityUid) mortar, mortar.Comp.FixtureId);
    if (fixtureOrNull != null)
      this._physics.SetHard((EntityUid) mortar, fixtureOrNull, false);
    this._appearance.SetData((EntityUid) mortar, (Enum) MortarVisualLayers.State, (object) MortarVisuals.Item);
  }

  private void OnMortarExamined(Entity<MortarComponent> ent, ref ExaminedEvent args)
  {
    if (this.HasComp<XenoComponent>(args.Examiner))
      return;
    using (args.PushGroup("MortarComponent"))
    {
      args.PushMarkup(this.Loc.GetString("rmc-mortar-less-accurate-with-range"));
      args.PushMarkup(this.Loc.GetString(ent.Comp.LaserTargetingMode ? "rmc-mortar-in-laser-mode" : "rmc-mortar-in-coordinates-mode", ("mortar", (object) ent)));
      if (ent.Comp.Deployed && ent.Comp.LaserTargetingMode && ent.Comp.LinkedLaserDesignator.HasValue && ent.Comp.LaserTargetCoordinates.HasValue)
        args.PushMarkup(this.Loc.GetString("rmc-mortar-laser-aimed", ("mortar", (object) ent)));
      args.PushMarkup(this.Loc.GetString("rmc-mortar-toggle-mode-hint"));
    }
  }

  private void OnMortarActivatableUIOpenAttempt(
    Entity<MortarComponent> ent,
    ref ActivatableUIOpenAttemptEvent args)
  {
    if (args.Cancelled || ent.Comp.Deployed)
      return;
    args.Cancel();
  }

  private void OnMortarShouldInteract(
    Entity<MortarComponent> ent,
    ref CombatModeShouldHandInteractEvent args)
  {
    args.Cancelled = true;
  }

  private void OnMortarCameraShellLand(
    Entity<MortarCameraShellComponent> ent,
    ref MortarShellLandEvent args)
  {
    this._audio.PlayPvs(ent.Comp.Sound, args.Coordinates);
    RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcMap.GetAnchoredEntitiesEnumerator(args.Coordinates, facing: (DirectionFlag) 0);
    EntityUid uid1;
    while (entitiesEnumerator.MoveNext(out uid1))
    {
      if (this.HasComp<MortarCameraComponent>(uid1))
        this.QueueDel(new EntityUid?(uid1));
    }
    MapCoordinates coordinates = this._transform.ToMapCoordinates(args.Coordinates);
    this.Spawn((string) ent.Comp.Flare, coordinates, rotation: new Angle());
    EntityUid uid2 = this.Spawn((string) ent.Comp.Camera, coordinates, rotation: new Angle());
    Vector2i offset;
    if (this._rmcPlanet.TryGetOffset(coordinates, out offset))
      coordinates = coordinates.Offset(Vector2i.op_Implicit(offset));
    float num1;
    float num2;
    Vector2Helpers.Deconstruct(coordinates.Position, ref num1, ref num2);
    float num3 = num1;
    float num4 = num2;
    this._metaData.SetEntityName(uid2, this.Loc.GetString("rmc-mortar-camera-name", ("x", (object) (int) num3), ("y", (object) (int) num4)));
  }

  private void OnMortarTargetBui(Entity<MortarComponent> mortar, ref MortarTargetBuiMsg args)
  {
    if (mortar.Comp.LaserTargetingMode)
    {
      this._popup.PopupPredictedCursor(this.Loc.GetString("rmc-mortar-dial-coordinates", (nameof (mortar), (object) mortar)), args.Actor, PopupType.SmallCaution);
    }
    else
    {
      args.Target.X.Cap(mortar.Comp.MaxTarget);
      args.Target.Y.Cap(mortar.Comp.MaxTarget);
      EntityUid actor = args.Actor;
      TargetMortarDoAfterEvent @event = new TargetMortarDoAfterEvent(args.Target);
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, actor, mortar.Comp.TargetDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) mortar))
      {
        BreakOnMove = true
      }))
        return;
      this._popup.PopupPredicted(this.Loc.GetString("rmc-mortar-target-start-self", (nameof (mortar), (object) mortar)), this.Loc.GetString("rmc-mortar-target-start-others", ("user", (object) actor), (nameof (mortar), (object) mortar)), actor, new EntityUid?(actor));
    }
  }

  private void OnMortarDialBui(Entity<MortarComponent> mortar, ref MortarDialBuiMsg args)
  {
    if (mortar.Comp.LaserTargetingMode)
    {
      this._popup.PopupPredictedCursor(this.Loc.GetString("rmc-mortar-dial-coordinates", (nameof (mortar), (object) mortar)), args.Actor, PopupType.SmallCaution);
    }
    else
    {
      args.Target.X.Cap(mortar.Comp.MaxDial);
      args.Target.Y.Cap(mortar.Comp.MaxDial);
      EntityUid actor = args.Actor;
      DialMortarDoAfterEvent @event = new DialMortarDoAfterEvent(args.Target);
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, actor, mortar.Comp.TargetDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) mortar))
      {
        BreakOnMove = true
      }))
        return;
      this._popup.PopupPredicted(this.Loc.GetString("rmc-mortar-dial-start-self", (nameof (mortar), (object) mortar)), this.Loc.GetString("rmc-mortar-dial-start-others", ("user", (object) actor), (nameof (mortar), (object) mortar)), actor, new EntityUid?(actor));
    }
  }

  private void OnMortarViewCameras(Entity<MortarComponent> ent, ref MortarViewCamerasMsg args)
  {
    this._ui.OpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) RMCCameraUiKey.Key, new EntityUid?(args.Actor));
  }

  private void DeployMortar(Entity<MortarComponent> mortar, EntityUid user)
  {
    if (mortar.Comp.Deployed || !this.CanDeployPopup(mortar, user))
      return;
    DeployMortarDoAfterEvent @event = new DeployMortarDoAfterEvent();
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, mortar.Comp.DeployDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) mortar))
    {
      BreakOnMove = true,
      BreakOnHandChange = true
    }))
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-mortar-deploy-start", (nameof (mortar), (object) mortar)), user, new EntityUid?(user));
  }

  protected bool HasSkillPopup(Entity<MortarComponent> mortar, EntityUid user, bool predicted)
  {
    if (this._skills.HasSkills((Entity<SkillsComponent>) user, mortar.Comp.Skill))
      return true;
    string message = this.Loc.GetString("rmc-skills-no-training", ("target", (object) mortar));
    if (predicted)
      this._popup.PopupClient(message, user, new EntityUid?(user), PopupType.SmallCaution);
    else
      this._popup.PopupEntity(message, user, user, PopupType.SmallCaution);
    return false;
  }

  private bool CanDeployPopup(Entity<MortarComponent> mortar, EntityUid user)
  {
    if (!this.HasSkillPopup(mortar, user, true))
      return false;
    if (this._area.CanMortarPlacement(user.ToCoordinates()))
      return true;
    this._popup.PopupClient(this.Loc.GetString("rmc-mortar-covered", (nameof (mortar), (object) mortar)), user, new EntityUid?(user), PopupType.SmallCaution);
    return false;
  }

  protected virtual bool CanLoadPopup(
    Entity<MortarComponent> mortar,
    Entity<MortarShellComponent> shell,
    EntityUid user,
    out TimeSpan travelTime,
    out MapCoordinates coordinates)
  {
    travelTime = new TimeSpan();
    coordinates = new MapCoordinates();
    return false;
  }

  protected virtual bool ValidateTargetCoordinates(
    Entity<MortarComponent> mortar,
    Entity<MortarShellComponent>? shell,
    MapCoordinates coordinates,
    MapCoordinates mortarCoordinates,
    EntityUid? user,
    out TimeSpan travelTime)
  {
    travelTime = new TimeSpan();
    return false;
  }

  public void PopupWarning(
    MapCoordinates coordinates,
    float range,
    LocId warning,
    LocId warningAbove,
    bool chat = false)
  {
    foreach (ICommonSession networkedSession in this._player.NetworkedSessions)
    {
      EntityUid? attachedEntity = networkedSession.AttachedEntity;
      if (attachedEntity.HasValue)
      {
        EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
        TransformComponent component;
        if (this._transformQuery.TryComp(valueOrDefault, out component) && !(component.MapID != coordinates.MapId))
        {
          MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(component);
          Vector2 vector2 = coordinates.Position - mapCoordinates.Position;
          float num = vector2.Length();
          if ((double) num <= (double) range)
          {
            string upperInvariant = DirectionExtensions.GetDir(vector2).ToString().ToUpperInvariant();
            string message = (double) num < 1.0 ? this.Loc.GetString((string) warningAbove) : this.Loc.GetString((string) warning, ("direction", (object) upperInvariant));
            this._popup.PopupEntity(message, valueOrDefault, valueOrDefault, PopupType.LargeCaution);
            if (chat)
            {
              string str = $"[bold][font size=24][color=red]\n{message}\n[/color][/font][/bold]";
              this._rmcChat.ChatMessageToOne(ChatChannel.Radio, str, str, new EntityUid(), false, networkedSession.Channel);
            }
          }
        }
      }
    }
  }

  private void OnMortarExplosionOnSpawn(
    Entity<ActivateMortarShellOnSpawnComponent> explosion,
    ref MapInitEvent args)
  {
    MortarShellComponent comp;
    if (!this.TryComp<MortarShellComponent>((EntityUid) explosion, out comp))
      return;
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) explosion);
    if (mapCoordinates.MapId == MapId.Nullspace)
      return;
    TimeSpan curTime = this._timing.CurTime;
    ActiveMortarShellComponent component = new ActiveMortarShellComponent()
    {
      Coordinates = this._transform.ToCoordinates(mapCoordinates),
      WarnAt = curTime + comp.TravelDelay,
      ImpactWarnAt = curTime + comp.TravelDelay + comp.ImpactWarningDelay,
      LandAt = curTime + comp.TravelDelay + comp.ImpactDelay
    };
    this.AddComp<ActiveMortarShellComponent>((EntityUid) explosion, component, true);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveMortarShellComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveMortarShellComponent>();
    EntityUid uid;
    ActiveMortarShellComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!comp1.Warned && curTime >= comp1.WarnAt)
      {
        comp1.Warned = true;
        this.PopupWarning(this._transform.ToMapCoordinates(comp1.Coordinates), comp1.WarnRange, (LocId) "rmc-mortar-shell-warning", (LocId) "rmc-mortar-shell-warning-above");
        this._audio.PlayPvs(comp1.WarnSound, comp1.Coordinates);
      }
      if (!comp1.ImpactWarned && curTime >= comp1.ImpactWarnAt)
      {
        comp1.ImpactWarned = true;
        this.PopupWarning(this._transform.ToMapCoordinates(comp1.Coordinates), comp1.WarnRange, (LocId) "rmc-mortar-shell-impact-warning", (LocId) "rmc-mortar-shell-impact-warning-above");
      }
      if (curTime >= comp1.LandAt)
      {
        this._transform.SetCoordinates(uid, comp1.Coordinates);
        MortarShellLandEvent args = new MortarShellLandEvent(comp1.Coordinates);
        this.RaiseLocalEvent<MortarShellLandEvent>(uid, ref args);
        this._rmcExplosion.TriggerExplosive(uid);
        if (!this.EntityManager.IsQueuedForDeletion(uid))
          this.QueueDel(new EntityUid?(uid));
      }
    }
  }

  private void OnMortarLinkLaserDesignatorDoAfter(
    Entity<MortarComponent> mortar,
    ref LinkMortarLaserDesignatorDoAfterEvent args)
  {
    if (args.Handled)
      return;
    if (args.Cancelled)
    {
      mortar.Comp.IsLinking = false;
      this.Dirty<MortarComponent>(mortar);
    }
    else
    {
      args.Handled = true;
      EntityUid user = args.User;
      EntityUid entity = this.GetEntity(args.LaserDesignator);
      mortar.Comp.IsLinking = false;
      mortar.Comp.LinkedLaserDesignator = new EntityUid?(entity);
      this.Dirty<MortarComponent>(mortar);
      this._popup.PopupPredicted(this.Loc.GetString("rmc-mortar-laser-linked-self", (nameof (mortar), (object) mortar), ("laserDesignator", (object) entity)), this.Loc.GetString("rmc-mortar-laser-linked-others", ("user", (object) user), (nameof (mortar), (object) mortar), ("laserDesignator", (object) entity)), user, new EntityUid?(user));
    }
  }

  public bool TryToggleLaserTargetingMode(
    Entity<MortarComponent> mortar,
    EntityUid user,
    bool laserMode,
    bool playSound = true)
  {
    if (this.HasComp<XenoComponent>(user))
      return false;
    if (!mortar.Comp.Deployed)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-mortar-not-deployed", (nameof (mortar), (object) mortar)), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    mortar.Comp.LaserTargetingMode = laserMode;
    this.Dirty<MortarComponent>(mortar);
    this._popup.PopupPredicted(this.Loc.GetString(laserMode ? "rmc-mortar-laser-mode-switched-self" : "rmc-mortar-coordinates-mode-switched-self", (nameof (mortar), (object) mortar)), this.Loc.GetString(laserMode ? "rmc-mortar-laser-mode-switched-others" : "rmc-mortar-coordinates-mode-switched-others", (nameof (user), (object) user), (nameof (mortar), (object) mortar)), user, new EntityUid?(user));
    if (playSound)
      this._audio.PlayPredicted(mortar.Comp.ToggleSound, (EntityUid) mortar, new EntityUid?(user));
    return true;
  }

  public bool TryStartLinkLaserDesignator(
    Entity<MortarComponent> mortar,
    EntityUid laserDesignator,
    EntityUid user)
  {
    if (mortar.Comp.IsLinking)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-mortar-already-linking"), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    mortar.Comp.IsLinking = true;
    this.Dirty<MortarComponent>(mortar);
    LinkMortarLaserDesignatorDoAfterEvent @event = new LinkMortarLaserDesignatorDoAfterEvent(this.GetNetEntity(laserDesignator));
    if (this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, mortar.Comp.LaserLinkDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) mortar))
    {
      BreakOnMove = true,
      NeedHand = true,
      BreakOnHandChange = true
    }))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-mortar-linking-start", (nameof (mortar), (object) mortar), (nameof (laserDesignator), (object) laserDesignator)), (EntityUid) mortar, new EntityUid?(user));
      return true;
    }
    mortar.Comp.IsLinking = false;
    this.Dirty<MortarComponent>(mortar);
    return false;
  }

  public bool TryUpdateLaserTarget(
    Entity<MortarComponent> mortar,
    EntityCoordinates coordinates,
    bool playSound = true,
    bool laserTargetDelay = true)
  {
    EntityCoordinates? targetCoordinates = mortar.Comp.LaserTargetCoordinates;
    EntityCoordinates entityCoordinates = coordinates;
    if ((targetCoordinates.HasValue ? (targetCoordinates.GetValueOrDefault() == entityCoordinates ? 1 : 0) : 0) != 0 || coordinates == EntityCoordinates.Invalid || !mortar.Comp.LaserTargetingMode || !mortar.Comp.LinkedLaserDesignator.HasValue)
      return false;
    if (laserTargetDelay)
    {
      if (mortar.Comp.IsTargeting)
        return false;
      this.EnsureComp<DoAfterComponent>((EntityUid) mortar);
      MortarLaserTargetUpdateDoAfterEvent @event = new MortarLaserTargetUpdateDoAfterEvent(this.GetNetCoordinates(coordinates));
      if (this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) mortar, mortar.Comp.LaserTargetDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) mortar))
      {
        NeedHand = false,
        BreakOnMove = true,
        BreakOnHandChange = false,
        BreakOnDamage = false,
        RequireCanInteract = false,
        AttemptFrequency = AttemptFrequency.EveryTick
      }))
      {
        mortar.Comp.IsTargeting = true;
        this.Dirty<MortarComponent>(mortar);
        return true;
      }
    }
    else
    {
      mortar.Comp.LaserTargetCoordinates = new EntityCoordinates?(coordinates);
      mortar.Comp.NeedAnnouncement = true;
      this.Dirty<MortarComponent>(mortar);
      if (playSound)
      {
        SoundSpecifier sound = mortar.Comp.LaserTargetSound;
        if (mortar.Comp.LaserTargetCoordinates.HasValue)
          sound = this.ValidateTargetCoordinates(mortar, new Entity<MortarShellComponent>?(), this._transform.ToMapCoordinates(mortar.Comp.LaserTargetCoordinates.Value), this._transform.GetMapCoordinates((EntityUid) mortar), new EntityUid?(), out TimeSpan _) ? mortar.Comp.LaserTargetSound : mortar.Comp.LaserTargetWarningSound;
        this._audio.PlayPvs(sound, (EntityUid) mortar);
      }
    }
    return true;
  }

  private void OnMortarLaserTargetUpdateAttempt(
    Entity<MortarComponent> mortar,
    ref DoAfterAttemptEvent<MortarLaserTargetUpdateDoAfterEvent> args)
  {
    EntityUid? linkedLaserDesignator = mortar.Comp.LinkedLaserDesignator;
    if (linkedLaserDesignator.HasValue && this.HasComp<ActiveLaserDesignatorComponent>(linkedLaserDesignator.Value) && mortar.Comp.Deployed)
      return;
    args.Cancel();
  }

  private void OnGetMortarVerbs(
    Entity<MortarComponent> mortar,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    EntityUid user = args.User;
    if (this.HasComp<XenoComponent>(user))
      return;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = (Action) (() => this.TryToggleLaserTargetingMode(mortar, user, !mortar.Comp.LaserTargetingMode));
    alternativeVerb1.Text = this.Loc.GetString("rmc-mortar-toggle-mode");
    alternativeVerb1.Message = this.Loc.GetString("rmc-mortar-toggle-mode-message");
    alternativeVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/settings.svg.192dpi.png"));
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
  }

  private void OnMortarLaserTargetUpdateDoAfter(
    Entity<MortarComponent> mortar,
    ref MortarLaserTargetUpdateDoAfterEvent args)
  {
    if (args.Cancelled)
    {
      mortar.Comp.IsTargeting = false;
      mortar.Comp.NeedAnnouncement = true;
      this.Dirty<MortarComponent>(mortar);
      this._audio.PlayPvs(mortar.Comp.LaserTargetWarningSound, (EntityUid) mortar);
    }
    else
    {
      if (args.Handled)
        return;
      args.Handled = true;
      SoundSpecifier sound = this.ValidateTargetCoordinates(mortar, new Entity<MortarShellComponent>?(), this._transform.ToMapCoordinates(args.TargetCoordinates), this._transform.GetMapCoordinates((EntityUid) mortar), new EntityUid?(), out TimeSpan _) ? mortar.Comp.LaserTargetSound : mortar.Comp.LaserTargetWarningSound;
      mortar.Comp.LaserTargetCoordinates = new EntityCoordinates?(this.GetCoordinates(args.TargetCoordinates));
      mortar.Comp.IsTargeting = false;
      mortar.Comp.NeedAnnouncement = true;
      this.Dirty<MortarComponent>(mortar);
      this._audio.PlayPvs(sound, (EntityUid) mortar);
    }
  }

  private void OnActiveLaserDesignatorShutdown(
    Entity<ActiveLaserDesignatorComponent> laserDesignator,
    ref ComponentShutdown args)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<MortarComponent> entityQueryEnumerator = this.EntityQueryEnumerator<MortarComponent>();
    EntityUid uid;
    MortarComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntityUid? linkedLaserDesignator = comp1.LinkedLaserDesignator;
      EntityUid owner = laserDesignator.Owner;
      if ((linkedLaserDesignator.HasValue ? (linkedLaserDesignator.GetValueOrDefault() == owner ? 1 : 0) : 0) != 0)
      {
        comp1.LaserTargetCoordinates = new EntityCoordinates?();
        comp1.NeedAnnouncement = true;
        this.Dirty(uid, (IComponent) comp1);
        this._audio.PlayPredicted(comp1.LaserTargetWarningSound, uid, new EntityUid?());
      }
    }
  }

  private void OnRangefinderShutdown(
    Entity<RangefinderComponent> rangefinder,
    ref ComponentShutdown args)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<MortarComponent> entityQueryEnumerator = this.EntityQueryEnumerator<MortarComponent>();
    EntityUid uid;
    MortarComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntityUid? linkedLaserDesignator = comp1.LinkedLaserDesignator;
      EntityUid owner = rangefinder.Owner;
      if ((linkedLaserDesignator.HasValue ? (linkedLaserDesignator.GetValueOrDefault() == owner ? 1 : 0) : 0) != 0)
      {
        comp1.LinkedLaserDesignator = new EntityUid?();
        comp1.LaserTargetCoordinates = new EntityCoordinates?();
        comp1.NeedAnnouncement = true;
        this.Dirty(uid, (IComponent) comp1);
        this._audio.PlayPredicted(comp1.LaserTargetWarningSound, uid, new EntityUid?());
      }
    }
  }
}
