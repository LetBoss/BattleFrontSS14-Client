// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Mortar.SharedCivMortarSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Teams;
using Content.Shared.Damage;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.UserInterface;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._CIV14merka.Mortar;

public abstract class SharedCivMortarSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private FixtureSystem _fixture;
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
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  private Robust.Shared.GameObjects.EntityQuery<TransformComponent> _transformQuery;

  public override void Initialize()
  {
    this._transformQuery = this.GetEntityQuery<TransformComponent>();
    this.SubscribeLocalEvent<CivMortarComponent, UseInHandEvent>(new EntityEventRefHandler<CivMortarComponent, UseInHandEvent>(this.OnUseInHand), new Type[1]
    {
      typeof (ActivatableUISystem)
    });
    this.SubscribeLocalEvent<CivMortarComponent, CivDeployMortarDoAfterEvent>(new EntityEventRefHandler<CivMortarComponent, CivDeployMortarDoAfterEvent>(this.OnDeployDoAfter));
    this.SubscribeLocalEvent<CivMortarComponent, CivTargetMortarDoAfterEvent>(new EntityEventRefHandler<CivMortarComponent, CivTargetMortarDoAfterEvent>(this.OnTargetDoAfter));
    this.SubscribeLocalEvent<CivMortarComponent, CivDialMortarDoAfterEvent>(new EntityEventRefHandler<CivMortarComponent, CivDialMortarDoAfterEvent>(this.OnDialDoAfter));
    this.SubscribeLocalEvent<CivMortarComponent, InteractUsingEvent>(new EntityEventRefHandler<CivMortarComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<CivMortarComponent, CivLoadMortarShellDoAfterEvent>(new EntityEventRefHandler<CivMortarComponent, CivLoadMortarShellDoAfterEvent>(this.OnLoadDoAfter));
    this.SubscribeLocalEvent<CivMortarComponent, AnchorStateChangedEvent>(new EntityEventRefHandler<CivMortarComponent, AnchorStateChangedEvent>(this.OnAnchorStateChanged));
    this.SubscribeLocalEvent<CivMortarComponent, ExaminedEvent>(new EntityEventRefHandler<CivMortarComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<CivMortarComponent, ActivatableUIOpenAttemptEvent>(new EntityEventRefHandler<CivMortarComponent, ActivatableUIOpenAttemptEvent>(this.OnActivatableUiOpenAttempt));
    this.SubscribeLocalEvent<CivMortarComponent, CombatModeShouldHandInteractEvent>(new EntityEventRefHandler<CivMortarComponent, CombatModeShouldHandInteractEvent>(this.OnShouldInteract));
    this.SubscribeLocalEvent<CivMortarComponent, DestructionEventArgs>(new EntityEventRefHandler<CivMortarComponent, DestructionEventArgs>(this.OnDestruction));
    this.SubscribeLocalEvent<CivMortarComponent, BeforeDamageChangedEvent>(new EntityEventRefHandler<CivMortarComponent, BeforeDamageChangedEvent>(this.OnBeforeDamageChanged));
    this.Subs.BuiEvents<CivMortarComponent>((object) CivMortarUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<CivMortarComponent>) (subs =>
    {
      subs.Event<CivMortarTargetBuiMsg>(new EntityEventRefHandler<CivMortarComponent, CivMortarTargetBuiMsg>(this.OnTargetBui));
      subs.Event<CivMortarDialBuiMsg>(new EntityEventRefHandler<CivMortarComponent, CivMortarDialBuiMsg>(this.OnDialBui));
    }));
  }

  private void OnBeforeDamageChanged(
    Entity<CivMortarComponent> ent,
    ref BeforeDamageChangedEvent args)
  {
    if (ent.Comp.Deployed)
      return;
    args.Cancelled = true;
  }

  private void OnDestruction(Entity<CivMortarComponent> mortar, ref DestructionEventArgs args)
  {
    if (!mortar.Comp.Deployed || this._net.IsClient)
      return;
    this.SpawnAtPosition((string) mortar.Comp.Drop, this.Transform((EntityUid) mortar).Coordinates);
  }

  private void OnUseInHand(Entity<CivMortarComponent> mortar, ref UseInHandEvent args)
  {
    args.Handled = true;
    this.UpdateMortarTeam(mortar, args.User);
    this.DeployMortar(mortar, args.User);
  }

  private void OnDeployDoAfter(
    Entity<CivMortarComponent> mortar,
    ref CivDeployMortarDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled || mortar.Comp.Deployed)
      return;
    args.Handled = true;
    mortar.Comp.Deployed = true;
    this.Dirty<CivMortarComponent>(mortar);
    Fixture fixtureOrNull = this._fixture.GetFixtureOrNull((EntityUid) mortar, mortar.Comp.FixtureId);
    if (fixtureOrNull != null)
      this._physics.SetHard((EntityUid) mortar, fixtureOrNull, true);
    this._appearance.SetData((EntityUid) mortar, (Enum) CivMortarVisualLayers.State, (object) CivMortarVisuals.Deployed);
    TransformComponent xform = this.Transform((EntityUid) mortar);
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) mortar, xform);
    Angle localRotation = this.Transform(args.User).LocalRotation;
    Angle angle = DirectionExtensions.ToAngle(((Angle) ref localRotation).GetCardinalDir());
    this._transform.SetCoordinates((EntityUid) mortar, xform, moverCoordinates, new Angle?(angle));
    this._transform.AnchorEntity((Entity<TransformComponent>) ((EntityUid) mortar, xform));
    this._audio.PlayPredicted(mortar.Comp.DeploySound, (EntityUid) mortar, new EntityUid?(args.User));
  }

  private void OnTargetDoAfter(
    Entity<CivMortarComponent> mortar,
    ref CivTargetMortarDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    if (!this.CanChangeTargeting(mortar, args.User, false))
    {
      this.PopupTargetingBlocked(mortar, args.User, false);
    }
    else
    {
      this.UpdateMortarTeam(mortar, args.User);
      args.Handled = true;
      this.ApplyTarget(mortar, args.Vector);
      this._popup.PopupPredicted(this.Loc.GetString("civ-eq-mortar-target-saved"), this.Loc.GetString("civ-eq-mortar-target-saved"), args.User, new EntityUid?(args.User));
    }
  }

  private void OnDialDoAfter(Entity<CivMortarComponent> mortar, ref CivDialMortarDoAfterEvent args)
  {
  }

  private void OnInteractUsing(Entity<CivMortarComponent> mortar, ref InteractUsingEvent args)
  {
    CivMortarShellComponent comp;
    if (!this.TryComp<CivMortarShellComponent>(args.Used, out comp))
      return;
    args.Handled = true;
    this.UpdateMortarTeam(mortar, args.User);
    if (!this.CanLoadPopup(mortar, (Entity<CivMortarShellComponent>) (args.Used, comp), args.User, out TimeSpan _, out MapCoordinates _))
      return;
    CivLoadMortarShellDoAfterEvent @event = new CivLoadMortarShellDoAfterEvent();
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, comp.LoadDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) mortar), new EntityUid?((EntityUid) mortar), new EntityUid?(args.Used))
    {
      BreakOnMove = true,
      BreakOnHandChange = true,
      ForceVisible = true
    }))
      return;
    this._popup.PopupPredicted(this.Loc.GetString("civ-eq-mortar-loading-self"), this.Loc.GetString("civ-eq-mortar-loading-others"), args.User, new EntityUid?(args.User));
    if (!this._net.IsServer)
      return;
    this._audio.PlayPvs(mortar.Comp.ReloadSound, (EntityUid) mortar);
  }

  private void OnLoadDoAfter(
    Entity<CivMortarComponent> mortar,
    ref CivLoadMortarShellDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? used = args.Used;
    if (!used.HasValue)
      return;
    EntityUid valueOrDefault = used.GetValueOrDefault();
    if (this._net.IsClient)
      return;
    args.Handled = true;
    CivMortarShellComponent comp1;
    if (!this.TryComp<CivMortarShellComponent>(valueOrDefault, out comp1) || !mortar.Comp.Deployed || this.HasComp<CivActiveMortarShellComponent>(valueOrDefault))
      return;
    this.UpdateMortarTeam(mortar, args.User);
    TimeSpan travelTime;
    MapCoordinates coordinates;
    if (!this.CanLoadPopup(mortar, (Entity<CivMortarShellComponent>) (valueOrDefault, comp1), args.User, out travelTime, out coordinates))
      return;
    bool flag = this.IsUnderRoof((EntityUid) mortar, out EntityUid? _);
    TimeSpan curTime = this._timing.CurTime;
    EntityCoordinates entityCoordinates = flag ? this.Transform((EntityUid) mortar).Coordinates : this._transform.ToCoordinates(coordinates);
    TimeSpan timeSpan1 = flag ? curTime : curTime + travelTime;
    TimeSpan timeSpan2 = flag ? curTime : curTime + travelTime + comp1.ImpactWarningDelay;
    TimeSpan timeSpan3 = flag ? curTime : curTime + travelTime + comp1.ImpactDelay;
    MortarShellOwnerComponent comp2;
    if (this.TryComp<MortarShellOwnerComponent>(valueOrDefault, out comp2))
      comp2.User = new EntityUid?(args.User);
    CivActiveMortarShellComponent component = new CivActiveMortarShellComponent()
    {
      Coordinates = entityCoordinates,
      WarnAt = timeSpan1,
      ImpactWarnAt = timeSpan2,
      LandAt = timeSpan3,
      FiredFromRoof = flag,
      MortarUid = new EntityUid?((EntityUid) mortar)
    };
    if (!flag && component.WarnSound != null)
    {
      TimeSpan audioLength = this._audio.GetAudioLength(this._audio.ResolveSound(component.WarnSound));
      TimeSpan timeSpan4 = timeSpan3 - audioLength;
      component.WarnAt = timeSpan4 > curTime ? timeSpan4 : curTime;
    }
    if (flag)
    {
      Container container = this._container.EnsureContainer<Container>((EntityUid) mortar, mortar.Comp.ContainerId);
      if (!this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) valueOrDefault, (BaseContainer) container))
        return;
      mortar.Comp.LastFiredAt = curTime;
      this.Dirty<CivMortarComponent>(mortar);
      this.AddComp<CivActiveMortarShellComponent>(valueOrDefault, component, true);
      CivMortarShellFiredEvent args1 = new CivMortarShellFiredEvent(args.User);
      this.RaiseLocalEvent<CivMortarShellFiredEvent>(valueOrDefault, ref args1);
      this.PlayFire(mortar, args.User);
    }
    else
    {
      CivMortarInterceptAttemptEvent args2 = new CivMortarInterceptAttemptEvent(this.GetMortarTeam(mortar, args.User), this._transform.GetMapCoordinates((EntityUid) mortar), coordinates, component.WarnRange);
      this.RaiseLocalEvent<CivMortarInterceptAttemptEvent>(mortar.Owner, ref args2);
      if (args2.Cancelled)
      {
        mortar.Comp.LastFiredAt = curTime;
        this.Dirty<CivMortarComponent>(mortar);
        this.QueueDel(new EntityUid?(valueOrDefault));
        this.PlayFire(mortar, args.User);
      }
      else
      {
        Container container = this._container.EnsureContainer<Container>((EntityUid) mortar, mortar.Comp.ContainerId);
        if (!this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) valueOrDefault, (BaseContainer) container))
          return;
        mortar.Comp.LastFiredAt = curTime;
        this.Dirty<CivMortarComponent>(mortar);
        this.AddComp<CivActiveMortarShellComponent>(valueOrDefault, component, true);
        CivMortarShellFiredEvent args3 = new CivMortarShellFiredEvent(args.User);
        this.RaiseLocalEvent<CivMortarShellFiredEvent>(valueOrDefault, ref args3);
        this.PlayFire(mortar, args.User);
      }
    }
  }

  private void OnAnchorStateChanged(
    Entity<CivMortarComponent> mortar,
    ref AnchorStateChangedEvent args)
  {
    if (args.Anchored)
      return;
    mortar.Comp.Deployed = false;
    this.Dirty<CivMortarComponent>(mortar);
    Fixture fixtureOrNull = this._fixture.GetFixtureOrNull((EntityUid) mortar, mortar.Comp.FixtureId);
    if (fixtureOrNull != null)
      this._physics.SetHard((EntityUid) mortar, fixtureOrNull, false);
    this._appearance.SetData((EntityUid) mortar, (Enum) CivMortarVisualLayers.State, (object) CivMortarVisuals.Item);
  }

  private void OnExamined(Entity<CivMortarComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("CivMortarComponent"))
    {
      args.PushText(ent.Comp.Deployed ? this.Loc.GetString("civ-eq-mortar-examine-deployed") : this.Loc.GetString("civ-eq-mortar-examine-stowed"));
      args.PushText(ent.Comp.HasTarget ? this.Loc.GetString("civ-eq-mortar-examine-target", ("x", (object) ent.Comp.Target.X), ("y", (object) ent.Comp.Target.Y)) : this.Loc.GetString("civ-eq-mortar-examine-no-target"));
      args.PushText(this.Loc.GetString("civ-eq-mortar-examine-dial", ("x", (object) ent.Comp.Dial.X), ("y", (object) ent.Comp.Dial.Y)));
    }
  }

  private void OnActivatableUiOpenAttempt(
    Entity<CivMortarComponent> ent,
    ref ActivatableUIOpenAttemptEvent args)
  {
    if (ent.Comp.Deployed)
      return;
    args.Cancel();
  }

  private void OnShouldInteract(
    Entity<CivMortarComponent> ent,
    ref CombatModeShouldHandInteractEvent args)
  {
    args.Cancelled = true;
  }

  private void OnTargetBui(Entity<CivMortarComponent> mortar, ref CivMortarTargetBuiMsg args)
  {
    if (!this.CanChangeTargeting(mortar, args.Actor, false))
    {
      this.PopupTargetingBlocked(mortar, args.Actor, false);
    }
    else
    {
      CivTargetMortarDoAfterEvent @event = new CivTargetMortarDoAfterEvent(args.Target);
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.Actor, mortar.Comp.TargetDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) mortar))
      {
        BreakOnMove = true
      }))
        return;
      this._popup.PopupPredictedCursor(this.Loc.GetString("civ-eq-mortar-start-target"), args.Actor, PopupType.Medium);
    }
  }

  private void OnDialBui(Entity<CivMortarComponent> mortar, ref CivMortarDialBuiMsg args)
  {
    if (!this.CanChangeTargeting(mortar, args.Actor, true))
    {
      this.PopupTargetingBlocked(mortar, args.Actor, true);
    }
    else
    {
      mortar.Comp.Dial = args.Target;
      this.Dirty<CivMortarComponent>(mortar);
      this._popup.PopupPredicted(this.Loc.GetString("civ-eq-mortar-dial-saved"), this.Loc.GetString("civ-eq-mortar-dial-saved"), args.Actor, new EntityUid?(args.Actor));
    }
  }

  private void DeployMortar(Entity<CivMortarComponent> mortar, EntityUid user)
  {
    if (mortar.Comp.Deployed)
      return;
    CivDeployMortarDoAfterEvent @event = new CivDeployMortarDoAfterEvent();
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, mortar.Comp.DeployDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) mortar))
    {
      BreakOnMove = true,
      BreakOnHandChange = true
    }))
      return;
    this._popup.PopupClient(this.Loc.GetString("civ-eq-mortar-start-deploy"), user, new EntityUid?(user));
  }

  protected void ApplyTarget(Entity<CivMortarComponent> mortar, Vector2i target)
  {
    mortar.Comp.Target = target;
    mortar.Comp.HasTarget = true;
    Vector2 position = this._transform.GetMapCoordinates((EntityUid) mortar).Position;
    int num1 = Math.Max(1, mortar.Comp.TilesPerOffset);
    int num2 = (int) Math.Floor((double) Math.Abs((float) target.X - position.X) / (double) num1);
    int num3 = (int) Math.Floor((double) Math.Abs((float) target.Y - position.Y) / (double) num1);
    mortar.Comp.SpreadOffset = Vector2i.op_Implicit((this._random.Next(-num2, num2 + 1), this._random.Next(-num3, num3 + 1)));
    this.Dirty<CivMortarComponent>(mortar);
  }

  protected virtual bool CanChangeTargeting(
    Entity<CivMortarComponent> mortar,
    EntityUid user,
    bool dial)
  {
    return true;
  }

  protected virtual void PopupTargetingBlocked(
    Entity<CivMortarComponent> mortar,
    EntityUid user,
    bool dial)
  {
  }

  protected virtual bool IsUnderRoof(EntityUid uid, out EntityUid? roofMarker)
  {
    roofMarker = new EntityUid?();
    return false;
  }

  protected bool CanLoadPopup(
    Entity<CivMortarComponent> mortar,
    Entity<CivMortarShellComponent> shell,
    EntityUid user,
    out TimeSpan travelTime,
    out MapCoordinates coordinates)
  {
    travelTime = new TimeSpan();
    coordinates = new MapCoordinates();
    if (!mortar.Comp.Deployed)
    {
      this._popup.PopupEntity(this.Loc.GetString("civ-eq-mortar-deploy-first"), user, user, PopupType.SmallCaution);
      return false;
    }
    if (!mortar.Comp.HasTarget)
    {
      this._popup.PopupEntity(this.Loc.GetString("civ-eq-mortar-set-target-first"), user, user, PopupType.SmallCaution);
      return false;
    }
    if (this._timing.CurTime < mortar.Comp.LastFiredAt + mortar.Comp.FireDelay)
    {
      this._popup.PopupEntity(this.Loc.GetString("civ-eq-mortar-not-ready"), user, user, PopupType.SmallCaution);
      return false;
    }
    BaseContainer container;
    if (this._container.TryGetContainer((EntityUid) mortar, mortar.Comp.ContainerId, out container) && !this._container.CanInsert((EntityUid) shell, container))
    {
      this._popup.PopupClient(this.Loc.GetString("civ-eq-mortar-already-loaded"), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    MapId mapId = this.Transform((EntityUid) mortar).MapID;
    if (mapId == MapId.Nullspace)
    {
      this._popup.PopupEntity(this.Loc.GetString("civ-eq-mortar-off-map"), user, user, PopupType.SmallCaution);
      return false;
    }
    Vector2i vector2i = Vector2i.op_Addition(Vector2i.op_Addition(mortar.Comp.Target, mortar.Comp.SpreadOffset), mortar.Comp.Dial);
    coordinates = new MapCoordinates(Vector2i.op_Implicit(vector2i), mapId);
    travelTime = shell.Comp.TravelDelay;
    return true;
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<CivActiveMortarShellComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CivActiveMortarShellComponent>();
    EntityUid uid;
    CivActiveMortarShellComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!comp1.Warned && curTime >= comp1.WarnAt)
      {
        comp1.Warned = true;
        this.PopupWarning(this._transform.ToMapCoordinates(comp1.Coordinates), comp1.WarnRange, this.Loc.GetString("civ-eq-mortar-incoming"));
        this._audio.PlayPvs(comp1.WarnSound, comp1.Coordinates);
        this.Dirty(uid, (IComponent) comp1);
      }
      if (!comp1.ImpactWarned && curTime >= comp1.ImpactWarnAt)
      {
        comp1.ImpactWarned = true;
        this.PopupWarning(this._transform.ToMapCoordinates(comp1.Coordinates), comp1.ImpactWarnRange, this.Loc.GetString("civ-eq-mortar-impact-soon"));
        this.Dirty(uid, (IComponent) comp1);
      }
      if (!(curTime < comp1.LandAt))
      {
        this._transform.SetCoordinates(uid, comp1.Coordinates);
        this.RaiseLocalEvent<CivMortarShellLandEvent>(uid, new CivMortarShellLandEvent(comp1.Coordinates));
      }
    }
  }

  private void PopupWarning(MapCoordinates coordinates, float range, string message)
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
          if ((double) (coordinates.Position - mapCoordinates.Position).Length() <= (double) range)
            this._popup.PopupEntity(message, valueOrDefault, valueOrDefault, PopupType.LargeCaution);
        }
      }
    }
  }

  protected void UpdateMortarTeam(Entity<CivMortarComponent> mortar, EntityUid user)
  {
    CivTeamMemberComponent comp;
    if (!this.TryComp<CivTeamMemberComponent>(user, out comp) || comp.TeamId <= 0 || mortar.Comp.TeamId == comp.TeamId)
      return;
    mortar.Comp.TeamId = comp.TeamId;
  }

  protected int GetMortarTeam(Entity<CivMortarComponent> mortar, EntityUid user)
  {
    CivTeamMemberComponent comp;
    return this.TryComp<CivTeamMemberComponent>(user, out comp) && comp.TeamId > 0 ? comp.TeamId : mortar.Comp.TeamId;
  }

  private void PlayFire(Entity<CivMortarComponent> mortar, EntityUid user)
  {
    this._popup.PopupPredicted(this.Loc.GetString("civ-eq-mortar-fire-self"), this.Loc.GetString("civ-eq-mortar-fire-others"), user, new EntityUid?(user));
    this._popup.PopupEntity(this.Loc.GetString("civ-eq-mortar-fires"), (EntityUid) mortar, PopupType.MediumCaution);
    this._audio.PlayPvs(mortar.Comp.FireSound, (EntityUid) mortar);
    this.RaiseNetworkEvent((EntityEventArgs) new CivMortarFiredEvent(this.GetNetEntity((EntityUid) mortar)), Filter.Pvs((EntityUid) mortar));
  }
}
