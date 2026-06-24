// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Egg.XenoEggSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Construction.Tunnel;
using Content.Shared._RMC14.Xenonids.Egg.EggRetriever;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Ghost;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Jittering;
using Content.Shared.Maps;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.StepTrigger.Components;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Egg;

public sealed class XenoEggSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private FixtureSystem _fixture;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private SharedXenoParasiteSystem _parasite;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private XenoPlasmaSystem _plasma;
  [Dependency]
  private SharedXenoWeedsSystem _weeds;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private EntityManager _entities;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private RMCHandsSystem _rmcHands;
  [Dependency]
  private TagSystem _tags;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedJitteringSystem _jitter;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDestructibleSystem _destruction;
  [Dependency]
  private NameModifierSystem _nameModifier;
  private static readonly ProtoId<TagPrototype> AirlockTag = (ProtoId<TagPrototype>) "Airlock";
  private static readonly ProtoId<TagPrototype> StructureTag = (ProtoId<TagPrototype>) "Structure";
  private Robust.Shared.GameObjects.EntityQuery<StepTriggerComponent> _stepTriggerQuery;

  public override void Initialize()
  {
    this._stepTriggerQuery = this.GetEntityQuery<StepTriggerComponent>();
    this.SubscribeLocalEvent<DropshipHijackStartEvent>(new EntityEventRefHandler<DropshipHijackStartEvent>(this.OnDropshipHijackStart));
    this.SubscribeLocalEvent<XenoComponent, XenoGrowOvipositorActionEvent>(new EntityEventRefHandler<XenoComponent, XenoGrowOvipositorActionEvent>(this.OnXenoGrowOvipositorAction));
    this.SubscribeLocalEvent<XenoComponent, XenoGrowOvipositorDoAfterEvent>(new EntityEventRefHandler<XenoComponent, XenoGrowOvipositorDoAfterEvent>(this.OnXenoGrowOvipositorDoAfter));
    this.SubscribeLocalEvent<XenoAttachedOvipositorComponent, MapInitEvent>(new EntityEventRefHandler<XenoAttachedOvipositorComponent, MapInitEvent>(this.OnXenoAttachedMapInit));
    this.SubscribeLocalEvent<XenoAttachedOvipositorComponent, ComponentRemove>(new EntityEventRefHandler<XenoAttachedOvipositorComponent, ComponentRemove>(this.OnXenoAttachedRemove));
    this.SubscribeLocalEvent<XenoAttachedOvipositorComponent, MobStateChangedEvent>(new EntityEventRefHandler<XenoAttachedOvipositorComponent, MobStateChangedEvent>(this.OnXenoMobStateChanged));
    this.SubscribeLocalEvent<XenoAttachedOvipositorComponent, XenoConstructionRangeEvent>(new EntityEventRefHandler<XenoAttachedOvipositorComponent, XenoConstructionRangeEvent>(this.OnXenoConstructionRange));
    this.SubscribeLocalEvent<XenoEggComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<XenoEggComponent, AfterAutoHandleStateEvent>(this.OnXenoEggAfterState));
    this.SubscribeLocalEvent<XenoEggComponent, GettingPickedUpAttemptEvent>(new EntityEventRefHandler<XenoEggComponent, GettingPickedUpAttemptEvent>(this.OnXenoEggPickedUpAttempt));
    this.SubscribeLocalEvent<XenoEggComponent, UseInHandEvent>(new EntityEventRefHandler<XenoEggComponent, UseInHandEvent>(this.OnXenoEggUseInHand));
    this.SubscribeLocalEvent<XenoEggComponent, InteractUsingEvent>(new EntityEventRefHandler<XenoEggComponent, InteractUsingEvent>(this.OnXenoEggInteractUsing));
    this.SubscribeLocalEvent<XenoEggComponent, XenoEggReturnParasiteDoAfterEvent>(new EntityEventRefHandler<XenoEggComponent, XenoEggReturnParasiteDoAfterEvent>(this.OnXenoEggReturnParasiteDoAfter));
    this.SubscribeLocalEvent<XenoEggComponent, AfterInteractEvent>(new EntityEventRefHandler<XenoEggComponent, AfterInteractEvent>(this.OnXenoEggAfterInteract));
    this.SubscribeLocalEvent<XenoEggComponent, XenoEggPlaceDoAfterEvent>(new EntityEventRefHandler<XenoEggComponent, XenoEggPlaceDoAfterEvent>(this.OnXenoEggPlaceDoAfter));
    this.SubscribeLocalEvent<XenoEggComponent, ActivateInWorldEvent>(new EntityEventRefHandler<XenoEggComponent, ActivateInWorldEvent>(this.OnXenoEggActivateInWorld));
    this.SubscribeLocalEvent<XenoEggComponent, StepTriggerAttemptEvent>(new EntityEventRefHandler<XenoEggComponent, StepTriggerAttemptEvent>(this.OnXenoEggStepTriggerAttempt));
    this.SubscribeLocalEvent<XenoEggComponent, StepTriggeredOffEvent>(new EntityEventRefHandler<XenoEggComponent, StepTriggeredOffEvent>(this.OnXenoEggStepTriggered));
    this.SubscribeLocalEvent<XenoEggComponent, BeforeDamageChangedEvent>(new EntityEventRefHandler<XenoEggComponent, BeforeDamageChangedEvent>(this.OnXenoEggBeforeDamageChanged));
    this.SubscribeLocalEvent<XenoEggComponent, GetVerbsEvent<ActivationVerb>>(new EntityEventRefHandler<XenoEggComponent, GetVerbsEvent<ActivationVerb>>(this.OnGetVerbs));
    this.SubscribeLocalEvent<XenoEggComponent, DestructionEventArgs>(new EntityEventRefHandler<XenoEggComponent, DestructionEventArgs>(this.OnDestruction));
    this.SubscribeLocalEvent<XenoFragileEggComponent, ComponentShutdown>(new EntityEventRefHandler<XenoFragileEggComponent, ComponentShutdown>(this.OnFragileConvert));
    this.SubscribeLocalEvent<XenoFragileEggComponent, RefreshNameModifiersEvent>(new EntityEventRefHandler<XenoFragileEggComponent, RefreshNameModifiersEvent>(this.OnFragileRefreshModifier));
    this.SubscribeLocalEvent<XenoFragileEggComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoFragileEggComponent, EntityTerminatingEvent>(this.OnFragileDelete));
    this.SubscribeLocalEvent<XenoEggSustainerComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoEggSustainerComponent, EntityTerminatingEvent>(this.OnEggSustainerDelete));
    this.SubscribeLocalEvent<XenoEggSustainerComponent, MobStateChangedEvent>(new EntityEventRefHandler<XenoEggSustainerComponent, MobStateChangedEvent>(this.OnEggSustainerDeath));
  }

  private void OnDropshipHijackStart(ref DropshipHijackStartEvent ev)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoOvipositorCapableComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoOvipositorCapableComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out XenoOvipositorCapableComponent _))
    {
      foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoGrowOvipositorActionEvent>(uid))
        this._actions.ClearCooldown(new Entity<ActionComponent>?(entity.AsNullable()));
    }
  }

  private void OnXenoGrowOvipositorAction(
    Entity<XenoComponent> xeno,
    ref XenoGrowOvipositorActionEvent args)
  {
    if (args.Handled)
      return;
    bool flag = this.HasComp<XenoAttachedOvipositorComponent>((EntityUid) xeno);
    if (!flag && !this._plasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, (FixedPoint2) args.AttachPlasmaCost))
      return;
    args.Handled = true;
    XenoGrowOvipositorDoAfterEvent @event = new XenoGrowOvipositorDoAfterEvent()
    {
      PlasmaCost = (FixedPoint2) args.AttachPlasmaCost
    };
    TimeSpan delay = args.AttachDoAfter;
    LocId messageId = new LocId("cm-xeno-ovipositor-attach");
    PopupType type = PopupType.Medium;
    if (flag)
    {
      @event.PlasmaCost = FixedPoint2.Zero;
      delay = args.DetachDoAfter;
      messageId = (LocId) "cm-xeno-ovipositor-detach";
      type = PopupType.MediumCaution;
    }
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) xeno))
    {
      BreakOnMove = true,
      MovementThreshold = 1f / 1000f,
      BreakOnRest = !flag
    }))
      return;
    this._popup.PopupClient(this.Loc.GetString((string) messageId), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), type);
  }

  private void OnXenoGrowOvipositorDoAfter(
    Entity<XenoComponent> xeno,
    ref XenoGrowOvipositorDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled || !this._plasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, args.PlasmaCost))
      return;
    args.Handled = true;
    XenoAttachedOvipositorComponent comp;
    if (this.TryComp<XenoAttachedOvipositorComponent>((EntityUid) xeno, out comp))
      this.DetachOvipositor((Entity<XenoAttachedOvipositorComponent>) ((EntityUid) xeno, comp));
    else
      this.AttachOvipositor((Entity<XenoAttachedOvipositorComponent>) xeno.Owner);
  }

  private void OnXenoAttachedMapInit(
    Entity<XenoAttachedOvipositorComponent> attached,
    ref MapInitEvent args)
  {
    TransformComponent comp;
    if (this.TryComp((EntityUid) attached, out comp))
      this._transform.AnchorEntity((EntityUid) attached, comp);
    XenoOvipositorChangedEvent args1 = new XenoOvipositorChangedEvent(true);
    this.RaiseLocalEvent<XenoOvipositorChangedEvent>((EntityUid) attached, ref args1, true);
  }

  private void OnXenoAttachedRemove(
    Entity<XenoAttachedOvipositorComponent> attached,
    ref ComponentRemove args)
  {
    TransformComponent comp;
    if (!this.TerminatingOrDeleted((EntityUid) attached) && this.TryComp((EntityUid) attached, out comp))
    {
      this._transform.Unanchor((EntityUid) attached, comp);
      this._physics.TrySetBodyType((EntityUid) attached, BodyType.KinematicController);
    }
    XenoOvipositorChangedEvent args1 = new XenoOvipositorChangedEvent(false);
    this.RaiseLocalEvent<XenoOvipositorChangedEvent>((EntityUid) attached, ref args1, true);
  }

  private void OnXenoMobStateChanged(
    Entity<XenoAttachedOvipositorComponent> ent,
    ref MobStateChangedEvent args)
  {
    this.DetachOvipositor(ent);
  }

  private void OnXenoConstructionRange(
    Entity<XenoAttachedOvipositorComponent> ent,
    ref XenoConstructionRangeEvent args)
  {
    args.Range = (FixedPoint2) 0;
  }

  private void OnXenoEggAfterState(Entity<XenoEggComponent> egg, ref AfterAutoHandleStateEvent args)
  {
    XenoEggStateChangedEvent args1 = new XenoEggStateChangedEvent();
    this.RaiseLocalEvent<XenoEggStateChangedEvent>((EntityUid) egg, ref args1);
  }

  private void OnXenoEggPickedUpAttempt(
    Entity<XenoEggComponent> egg,
    ref GettingPickedUpAttemptEvent args)
  {
    if (egg.Comp.State == XenoEggState.Item)
      return;
    args.Cancel();
  }

  private void OnXenoEggUseInHand(Entity<XenoEggComponent> egg, ref UseInHandEvent args)
  {
    XenoEggUseInHandEvent args1 = new XenoEggUseInHandEvent(this._entities.GetNetEntity(egg.Owner, (MetaDataComponent) null));
    this.RaiseLocalEvent<XenoEggUseInHandEvent>(args.User, args1);
    args.Handled = args1.Handled;
  }

  private void OnXenoEggAfterInteract(Entity<XenoEggComponent> egg, ref AfterInteractEvent args)
  {
    if (args.Handled || egg.Comp.State != XenoEggState.Item || !this.HasComp<TransformComponent>((EntityUid) egg))
      return;
    EggPlantingDistanceComponent comp1;
    if (!this.HasComp<EggPlantingDistanceComponent>(args.User) && !args.CanReach || this.TryComp<EggPlantingDistanceComponent>(args.User, out comp1) && !this._interaction.InRangeUnobstructed(args.User, args.ClickLocation, comp1.Distance))
    {
      if (!this._timing.IsFirstTimePredicted)
        return;
      this._popup.PopupCoordinates(this.Loc.GetString("cm-xeno-cant-reach-there"), args.ClickLocation, Filter.Local(), true);
    }
    else if (!this.CanPlaceEggPopup(args.User, egg, args.ClickLocation, args.Handled, out bool _))
    {
      args.Handled = true;
    }
    else
    {
      args.Handled = true;
      TimeSpan delay = TimeSpan.FromSeconds(3.5);
      EggPlantTimeComponent comp2;
      if (this.TryComp<EggPlantTimeComponent>(args.User, out comp2))
        delay = comp2.PlantTime;
      XenoEggPlaceDoAfterEvent @event = new XenoEggPlaceDoAfterEvent(this.GetNetCoordinates(args.ClickLocation));
      DoAfterArgs args1 = new DoAfterArgs((IEntityManager) this.EntityManager, args.User, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) egg))
      {
        BreakOnMove = true,
        BlockDuplicate = true,
        DuplicateCondition = DuplicateConditions.SameEvent,
        RootEntity = true
      };
      this._popup.PopupPredicted(this.Loc.GetString("rmc-xeno-egg-plant-self"), this.Loc.GetString("rmc-xeno-egg-plant", ("user", (object) args.User)), (EntityUid) egg, new EntityUid?(args.User));
      this._doAfter.TryStartDoAfter(args1);
    }
  }

  private void OnXenoEggPlaceDoAfter(
    Entity<XenoEggComponent> egg,
    ref XenoEggPlaceDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    args.Handled = true;
    EntityCoordinates coordinates = this.GetCoordinates(args.Coordinates);
    bool hasHiveWeeds;
    if (!this.CanPlaceEggPopup(args.User, egg, coordinates, false, out hasHiveWeeds) || !this._plasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) args.User, (FixedPoint2) 30))
      return;
    if (!hasHiveWeeds)
      this.EggsacSustain(args.User, egg);
    this._transform.SetCoordinates((EntityUid) egg, this.GetCoordinates(args.Coordinates));
    this._transform.SetLocalRotation((EntityUid) egg, Angle.op_Implicit(0.0f));
    this.SetEggState(egg, XenoEggState.Growing);
    this._transform.AnchorEntity((EntityUid) egg, this.Transform((EntityUid) egg));
    this._audio.PlayPredicted(egg.Comp.PlantSound, (EntityUid) egg, new EntityUid?(args.User));
  }

  private void EggsacSustain(EntityUid user, Entity<XenoEggComponent> egg)
  {
    this.SetEggSprite(egg, egg.Comp.SustainedSprite);
    XenoEggSustainerComponent comp1;
    if (this._net.IsClient || !this.TryComp<XenoEggSustainerComponent>(user, out comp1))
      return;
    XenoFragileEggComponent fragileEggComponent = this.EnsureComp<XenoFragileEggComponent>((EntityUid) egg);
    fragileEggComponent.SustainedBy = new EntityUid?(user);
    fragileEggComponent.SustainRange = (float) comp1.SustainedEggsRange;
    fragileEggComponent.ExpireAt = new TimeSpan?(this._timing.CurTime + comp1.SustainedEggMaxTime);
    fragileEggComponent.ShortExpireAt = new TimeSpan?(this._timing.CurTime + fragileEggComponent.SustainDuration);
    fragileEggComponent.CheckSustainAt = new TimeSpan?(this._timing.CurTime + fragileEggComponent.SustainCheckEvery);
    this._nameModifier.RefreshNameModifiers((Entity<NameModifierComponent>) egg.Owner);
    this.Dirty((EntityUid) egg, (IComponent) fragileEggComponent);
    comp1.SustainedEggs.Add((EntityUid) egg);
    if (comp1.SustainedEggs.Count <= comp1.MaxSustainedEggs)
      return;
    EntityUid sustainedEgg = comp1.SustainedEggs[0];
    comp1.SustainedEggs.Remove(sustainedEgg);
    XenoFragileEggComponent comp2;
    XenoEggComponent comp3;
    if (!this.TryComp<XenoFragileEggComponent>(sustainedEgg, out comp2) || !this.TryComp<XenoEggComponent>(sustainedEgg, out comp3))
      return;
    this.UnsustainEgg(sustainedEgg, comp3, comp2, true);
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-sustain-egg-decaying", ("max", (object) comp1.MaxSustainedEggs)), user, user, PopupType.SmallCaution);
  }

  private void OnXenoEggActivateInWorld(Entity<XenoEggComponent> egg, ref ActivateInWorldEvent args)
  {
    TransformComponent comp;
    if (!this.TryComp(egg.Owner, out comp) || !comp.Anchored || !this.HasComp<XenoParasiteComponent>(args.User) && (!this.HasComp<XenoComponent>(args.User) || !this.HasComp<HandsComponent>(args.User)) || !this.Open(egg, new EntityUid?(args.User), out EntityUid? _))
      return;
    args.Handled = true;
  }

  private void OnXenoEggInteractUsing(Entity<XenoEggComponent> egg, ref InteractUsingEvent args)
  {
    EntityUid user = args.User;
    EntityUid used = args.Used;
    if (!this.HasComp<XenoParasiteComponent>(used) || !this._rmcHands.IsPickupByAllowed((Entity<WhitelistPickupByComponent>) args.Used, (Entity<WhitelistPickupComponent>) user))
      return;
    args.Handled = true;
    if (this._net.IsClient || !this.CanReturnParasitePopup(user, used, egg))
      return;
    TimeSpan delay = TimeSpan.FromSeconds(3.5);
    EggPlantTimeComponent comp;
    if (this.TryComp<EggPlantTimeComponent>(args.User, out comp))
      delay = comp.PlantTime;
    XenoEggReturnParasiteDoAfterEvent @event = new XenoEggReturnParasiteDoAfterEvent();
    DoAfterArgs args1 = new DoAfterArgs((IEntityManager) this.EntityManager, user, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) egg), new EntityUid?((EntityUid) egg), new EntityUid?(used))
    {
      BreakOnMove = true,
      BlockDuplicate = true,
      DuplicateCondition = DuplicateConditions.SameEvent
    };
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-egg-return-start"), args.User, args.User);
    this._doAfter.TryStartDoAfter(args1);
  }

  private void OnXenoEggReturnParasiteDoAfter(
    Entity<XenoEggComponent> egg,
    ref XenoEggReturnParasiteDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? used = args.Used;
    if (!used.HasValue)
      return;
    EntityUid valueOrDefault = used.GetValueOrDefault();
    args.Handled = true;
    if (this._net.IsClient || !this.CanReturnParasitePopup(args.User, valueOrDefault, egg))
      return;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-egg-return-user"), args.User, args.User);
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-egg-return", ("user", (object) args.User), ("parasite", (object) args.Used)), (EntityUid) egg, Filter.PvsExcept(args.User), true);
    this.SetEggState(egg, XenoEggState.Grown);
    this.QueueDel(args.Used);
  }

  private void OnXenoEggStepTriggerAttempt(
    Entity<XenoEggComponent> egg,
    ref StepTriggerAttemptEvent args)
  {
    if (!this.CanTrigger(args.Tripper))
      return;
    args.Continue = true;
  }

  private void OnXenoEggStepTriggered(Entity<XenoEggComponent> egg, ref StepTriggeredOffEvent args)
  {
    this.TryTrigger(egg, args.Tripper);
  }

  private void OnGetVerbs(Entity<XenoEggComponent> ent, ref GetVerbsEvent<ActivationVerb> args)
  {
    EntityUid uid = args.User;
    XenoFragileEggComponent comp;
    if (!this.HasComp<ActorComponent>(uid) || !this.HasComp<GhostComponent>(uid) || ent.Comp.State != XenoEggState.Grown || this.TryComp<XenoFragileEggComponent>((EntityUid) ent, out comp) && comp.SustainedBy.HasValue)
      return;
    ActivationVerb activationVerb1 = new ActivationVerb();
    activationVerb1.Text = this.Loc.GetString("rmc-xeno-egg-ghost-verb");
    activationVerb1.Act = (Action) (() => this._ui.TryOpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) XenoParasiteGhostUI.Key, uid));
    activationVerb1.Impact = LogImpact.High;
    ActivationVerb activationVerb2 = activationVerb1;
    args.Verbs.Add(activationVerb2);
  }

  private bool CanTrigger(EntityUid user)
  {
    InfectableComponent comp;
    return this.TryComp<InfectableComponent>(user, out comp) && !comp.BeingInfected && !this._mobState.IsDead(user) && !this.HasComp<VictimInfectedComponent>(user);
  }

  public bool Open(Entity<XenoEggComponent> egg, EntityUid? user, out EntityUid? spawned)
  {
    spawned = new EntityUid?();
    if (egg.Comp.State == XenoEggState.Opening)
      return false;
    if (egg.Comp.State == XenoEggState.Opened)
    {
      if (this.HasComp<XenoParasiteComponent>(user))
      {
        if (this._mobState.IsDead(user.Value))
          return true;
        this.SetEggState(egg, XenoEggState.Grown);
        if (this._net.IsClient)
          return true;
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-egg-return-self", ("parasite", (object) user)), (EntityUid) egg);
        this.QueueDel(user);
        return true;
      }
      if (user.HasValue)
        this._popup.PopupClient(this.Loc.GetString("cm-xeno-egg-clear"), (EntityUid) egg, new EntityUid?(user.Value));
      if (this._net.IsClient)
        return true;
      this.QueueDel(new EntityUid?((EntityUid) egg));
      return true;
    }
    if (this.HasComp<XenoParasiteComponent>(user))
    {
      if (egg.Comp.State == XenoEggState.Grown || egg.Comp.State == XenoEggState.Growing)
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-egg-has-child"), new EntityUid?(user.Value));
      return true;
    }
    if (egg.Comp.State != XenoEggState.Grown)
    {
      if (user.HasValue)
        this._popup.PopupClient(this.Loc.GetString("cm-xeno-egg-not-developed"), (EntityUid) egg, new EntityUid?(user.Value));
      return false;
    }
    this.SetEggState(egg, XenoEggState.Opening);
    if (this._timing.IsFirstTimePredicted)
      this._audio.PlayPredicted(egg.Comp.OpenSound, (EntityUid) egg, user);
    if (this._net.IsClient)
      return true;
    ContainerSlot containerSlot = this._container.EnsureContainer<ContainerSlot>(egg.Owner, egg.Comp.CreatureContainerId);
    spawned = new EntityUid?(this.SpawnInContainerOrDrop((string) egg.Comp.Spawn, egg.Owner, containerSlot.ID));
    this._hive.SetSameHive((Entity<HiveMemberComponent>) egg.Owner, (Entity<HiveMemberComponent>) spawned.Value);
    egg.Comp.SpawnedCreature = spawned;
    this.Dirty<XenoEggComponent>(egg);
    ParasiteAIComponent comp;
    if (this.TryComp<ParasiteAIComponent>(spawned, out comp))
      this._parasite.GoIdle((Entity<ParasiteAIComponent>) (spawned.Value, comp));
    return true;
  }

  private void SetEggState(Entity<XenoEggComponent> egg, XenoEggState state)
  {
    egg.Comp.State = state;
    this.Dirty<XenoEggComponent>(egg);
    if (state == XenoEggState.Opened)
      this.RemCompDeferred<XenoFriendlyComponent>((EntityUid) egg);
    this.UpdateEggSprite(egg);
    if (state != XenoEggState.Item)
    {
      if ((uint) (state - 1) > 5U || egg.Comp.GrownFixtures)
        return;
      egg.Comp.GrownFixtures = true;
      this.Dirty<XenoEggComponent>(egg);
      this._fixture.TryCreateFixture((EntityUid) egg, egg.Comp.GrowingMaskShape, egg.Comp.GrowingMaskFixture, hard: false, collisionMask: (int) egg.Comp.GrowingMask);
      Fixture fixtureOrNull = this._fixture.GetFixtureOrNull((EntityUid) egg, egg.Comp.GrowingLayerFixture);
      if (fixtureOrNull == null)
        return;
      this._physics.SetCollisionLayer((EntityUid) egg, egg.Comp.GrowingLayerFixture, fixtureOrNull, (int) egg.Comp.GrowingLayer);
    }
    else
    {
      if (!egg.Comp.GrownFixtures)
        return;
      egg.Comp.GrownFixtures = false;
      this.Dirty<XenoEggComponent>(egg);
      Fixture fixtureOrNull = this._fixture.GetFixtureOrNull((EntityUid) egg, egg.Comp.GrowingLayerFixture);
      if (fixtureOrNull != null)
        this._physics.SetCollisionLayer((EntityUid) egg, egg.Comp.GrowingLayerFixture, fixtureOrNull, 0);
      this._fixture.DestroyFixture((EntityUid) egg, egg.Comp.GrowingMaskFixture);
    }
  }

  private void SetEggSprite(Entity<XenoEggComponent> egg, string sprite)
  {
    egg.Comp.CurrentSprite = sprite;
    this.Dirty<XenoEggComponent>(egg);
    this.UpdateEggSprite(egg);
  }

  private void UpdateEggSprite(Entity<XenoEggComponent> egg)
  {
    XenoEggStateChangedEvent args = new XenoEggStateChangedEvent();
    this.RaiseLocalEvent<XenoEggStateChangedEvent>((EntityUid) egg, ref args);
  }

  private void AttachOvipositor(Entity<XenoAttachedOvipositorComponent?> xeno)
  {
    XenoAttachedOvipositorComponent comp1;
    if (this.EnsureComp<XenoAttachedOvipositorComponent>((EntityUid) xeno, out comp1))
      return;
    xeno.Comp = comp1;
    foreach ((EntityUid entityUid, ActionComponent _) in this._actions.GetActions((EntityUid) xeno))
    {
      XenoGrowOvipositorActionComponent comp2;
      if (this.TryComp<XenoGrowOvipositorActionComponent>(entityUid, out comp2))
      {
        this._actions.SetCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) entityUid), comp2.AttachCooldown);
        this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) entityUid), true);
      }
    }
    XenoOvipositorCapableComponent comp3;
    if (this.TryComp<XenoOvipositorCapableComponent>((EntityUid) xeno, out comp3))
    {
      this.RemoveOvipositorActions((Entity<XenoOvipositorCapableComponent>) (xeno.Owner, comp3));
      foreach (EntProtoId actionId in comp3.ActionIds)
      {
        EntityUid? nullable = this._actions.AddAction((EntityUid) xeno, (string) actionId);
        if (nullable.HasValue)
        {
          EntityUid valueOrDefault = nullable.GetValueOrDefault();
          comp3.Actions[actionId] = valueOrDefault;
        }
      }
    }
    this.EnsureComp<EggPlantingDistanceComponent>((EntityUid) xeno).Distance = 3.5f;
  }

  private void DetachOvipositor(Entity<XenoAttachedOvipositorComponent> xeno)
  {
    if (!this.RemCompDeferred<XenoAttachedOvipositorComponent>((EntityUid) xeno))
      return;
    foreach ((EntityUid entityUid, ActionComponent _) in this._actions.GetActions((EntityUid) xeno))
    {
      XenoGrowOvipositorActionComponent comp;
      if (this.TryComp<XenoGrowOvipositorActionComponent>(entityUid, out comp))
      {
        this._actions.SetCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) entityUid), comp.DetachCooldown);
        this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) entityUid), false);
      }
    }
    this.RemoveOvipositorActions((Entity<XenoOvipositorCapableComponent>) xeno.Owner);
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-ovipositor-detach"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    this.RemCompDeferred<EggPlantingDistanceComponent>((EntityUid) xeno);
  }

  private bool TryTrigger(Entity<XenoEggComponent> egg, EntityUid tripper)
  {
    EntityUid? spawned;
    if (egg.Comp.State != XenoEggState.Grown || !this.CanTrigger(tripper) || !this._interaction.InRangeUnobstructed((Entity<TransformComponent>) egg.Owner, (Entity<TransformComponent>) tripper) || !this.Open(egg, new EntityUid?(tripper), out spawned) || !this.TryComp<XenoParasiteComponent>(spawned, out XenoParasiteComponent _))
      return false;
    egg.Comp.InfectTarget = new EntityUid?(tripper);
    this.Dirty<XenoEggComponent>(egg);
    return true;
  }

  private bool CanPlaceEggPopup(
    EntityUid user,
    Entity<XenoEggComponent> egg,
    EntityCoordinates coordinates,
    bool handled,
    out bool hasHiveWeeds)
  {
    hasHiveWeeds = false;
    if (this.HasComp<MarineComponent>(user))
    {
      if (!handled)
      {
        this._hands.TryDrop((Entity<HandsComponent>) user, (EntityUid) egg, new EntityCoordinates?(coordinates));
        this._popup.PopupClient(this.Loc.GetString("cm-xeno-egg-failed-plant-outside"), user, new EntityUid?(user));
      }
      return false;
    }
    EntityUid? grid = this._transform.GetGrid(coordinates);
    if (grid.HasValue)
    {
      EntityUid valueOrDefault = grid.GetValueOrDefault();
      MapGridComponent comp;
      if (this.TryComp<MapGridComponent>(valueOrDefault, out comp))
      {
        Vector2i vector2i = this._map.TileIndicesFor(valueOrDefault, comp, coordinates);
        AnchoredEntitiesEnumerator entitiesEnumerator = this._map.GetAnchoredEntitiesEnumerator(valueOrDefault, comp, vector2i);
        hasHiveWeeds = this._weeds.IsOnHiveWeeds((Entity<MapGridComponent>) (valueOrDefault, comp), coordinates);
        EntityUid? uid;
        while (entitiesEnumerator.MoveNext(out uid))
        {
          if (this.HasComp<XenoEggComponent>(uid))
          {
            this._popup.PopupClient(this.Loc.GetString("cm-xeno-egg-failed-already-there"), uid.Value, new EntityUid?(user), PopupType.SmallCaution);
            return false;
          }
          if (!this.HasComp<XenoConstructComponent>(uid))
          {
            if (!this._tags.HasAnyTag(uid.Value, XenoEggSystem.StructureTag, XenoEggSystem.AirlockTag) && !this.HasComp<StrapComponent>(uid) && !this.HasComp<XenoTunnelComponent>(uid))
              continue;
          }
          this._popup.PopupClient(this.Loc.GetString("cm-xeno-egg-blocked"), uid.Value, new EntityUid?(user), PopupType.SmallCaution);
          return false;
        }
        if (this._turf.IsTileBlocked(valueOrDefault, vector2i, CollisionGroup.FlyingMobMask | CollisionGroup.MidImpassable, comp))
        {
          this._popup.PopupClient(this.Loc.GetString("cm-xeno-egg-blocked"), coordinates, new EntityUid?(user), PopupType.SmallCaution);
          return false;
        }
        if (hasHiveWeeds)
          return true;
        if (!this.HasComp<XenoEggSustainerComponent>(user))
        {
          this._popup.PopupClient(this.Loc.GetString("cm-xeno-egg-failed-must-hive-weeds"), user, new EntityUid?(user));
        }
        else
        {
          if (this._weeds.IsOnWeeds((Entity<MapGridComponent>) (valueOrDefault, comp), coordinates))
            return true;
          this._popup.PopupClient(this.Loc.GetString("cm-xeno-egg-failed-must-weeds"), user, new EntityUid?(user));
        }
        return false;
      }
    }
    return false;
  }

  private bool CanReturnParasitePopup(EntityUid user, EntityUid used, Entity<XenoEggComponent> egg)
  {
    if (this._mobState.IsDead(used))
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-egg-dead-child"), user, user, PopupType.SmallCaution);
      return false;
    }
    if (egg.Comp.State == XenoEggState.Growing || egg.Comp.State == XenoEggState.Grown)
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-egg-has-child"), user, user, PopupType.SmallCaution);
      return false;
    }
    if (egg.Comp.State != XenoEggState.Opened)
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-egg-fail-return"), user, user, PopupType.SmallCaution);
      return false;
    }
    if (this.HasComp<ParasiteAIComponent>(used))
      return true;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-egg-awake-child", ("parasite", (object) used)), user, user, PopupType.SmallCaution);
    return false;
  }

  private void OnXenoEggBeforeDamageChanged(
    Entity<XenoEggComponent> ent,
    ref BeforeDamageChangedEvent args)
  {
    if (ent.Comp.State != XenoEggState.Item)
      return;
    args.Cancelled = true;
  }

  private void RemoveOvipositorActions(Entity<XenoOvipositorCapableComponent?> capable)
  {
    if (!this.Resolve<XenoOvipositorCapableComponent>((EntityUid) capable, ref capable.Comp, false))
      return;
    foreach (KeyValuePair<EntProtoId, EntityUid> action in capable.Comp.Actions)
      this._actions.RemoveAction(new Entity<ActionComponent>?((Entity<ActionComponent>) action.Value));
    capable.Comp.Actions.Clear();
  }

  private void OnDestruction(Entity<XenoEggComponent> ent, ref DestructionEventArgs args)
  {
    if (this._net.IsClient)
      return;
    string prototype = (string) ent.Comp.EggDestroyed;
    if (ent.Comp.CurrentSprite == ent.Comp.FragileSprite)
      prototype = (string) ent.Comp.EggDestroyedFragile;
    else if (ent.Comp.CurrentSprite == ent.Comp.SustainedSprite)
      prototype = (string) ent.Comp.EggDestroyedSustained;
    EntityUid uid = this.SpawnAtPosition(prototype, ent.Owner.ToCoordinates());
    this._audio.PlayPvs(ent.Comp.BurstSound, uid);
  }

  private void OnFragileConvert(Entity<XenoFragileEggComponent> ent, ref ComponentShutdown args)
  {
    XenoEggSustainerComponent comp1;
    if (ent.Comp.SustainedBy.HasValue && this.TryComp<XenoEggSustainerComponent>(ent.Comp.SustainedBy, out comp1))
      comp1.SustainedEggs.Remove((EntityUid) ent);
    XenoEggComponent comp2;
    if (this.TryComp<XenoEggComponent>((EntityUid) ent, out comp2))
      this.SetEggSprite((Entity<XenoEggComponent>) ((EntityUid) ent, comp2), comp2.NormalSprite);
    this._nameModifier.RefreshNameModifiers((Entity<NameModifierComponent>) ent.Owner);
  }

  private void OnFragileDelete(Entity<XenoFragileEggComponent> ent, ref EntityTerminatingEvent args)
  {
    XenoEggSustainerComponent comp;
    if (!ent.Comp.SustainedBy.HasValue || !this.TryComp<XenoEggSustainerComponent>(ent.Comp.SustainedBy, out comp))
      return;
    comp.SustainedEggs.Remove((EntityUid) ent);
  }

  private void OnFragileRefreshModifier(
    Entity<XenoFragileEggComponent> ent,
    ref RefreshNameModifiersEvent args)
  {
    if (this.TerminatingOrDeleted((EntityUid) ent))
      return;
    args.AddModifier((LocId) "rmc-xeno-fragile-egg-prefix");
  }

  private void UnsustainEgg(
    EntityUid egg,
    XenoEggComponent eggComp,
    XenoFragileEggComponent fragile,
    bool decay = false)
  {
    if (this._net.IsClient)
      return;
    this.SetEggSprite((Entity<XenoEggComponent>) (egg, eggComp), eggComp.FragileSprite);
    fragile.SustainedBy = new EntityUid?();
    this.Dirty(egg, (IComponent) fragile);
    if (!decay || fragile.BurstAt.HasValue)
      return;
    fragile.BurstAt = new TimeSpan?(this._timing.CurTime + fragile.BurstDelay);
    this._jitter.DoJitter(egg, fragile.BurstDelay / 2.0, true, 40f, 8f, true);
  }

  private void OnEggSustainerDelete(
    Entity<XenoEggSustainerComponent> ent,
    ref EntityTerminatingEvent args)
  {
    foreach (EntityUid sustainedEgg in ent.Comp.SustainedEggs)
    {
      XenoFragileEggComponent comp1;
      XenoEggComponent comp2;
      if (this.TryComp<XenoFragileEggComponent>(sustainedEgg, out comp1) && this.TryComp<XenoEggComponent>(sustainedEgg, out comp2))
        this.UnsustainEgg(sustainedEgg, comp2, comp1);
    }
  }

  private void OnEggSustainerDeath(
    Entity<XenoEggSustainerComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (this._timing.ApplyingState || args.NewMobState != MobState.Dead)
      return;
    foreach (EntityUid sustainedEgg in ent.Comp.SustainedEggs)
    {
      XenoFragileEggComponent comp1;
      XenoEggComponent comp2;
      if (this.TryComp<XenoFragileEggComponent>(sustainedEgg, out comp1) && this.TryComp<XenoEggComponent>(sustainedEgg, out comp2))
        this.UnsustainEgg(sustainedEgg, comp2, comp1, true);
    }
    ent.Comp.SustainedEggs.Clear();
    if (!this._timing.IsFirstTimePredicted)
      return;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-sustain-death", ("xeno", (object) ent)), (EntityUid) ent, PopupType.MediumCaution);
    this._audio.PlayPredicted(ent.Comp.DeathSound, (EntityUid) ent, new EntityUid?((EntityUid) ent));
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoOvipositorCapableComponent, XenoAttachedOvipositorComponent, TransformComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<XenoOvipositorCapableComponent, XenoAttachedOvipositorComponent, TransformComponent>();
    EntityUid uid1;
    XenoOvipositorCapableComponent comp1_1;
    XenoAttachedOvipositorComponent comp2_1;
    TransformComponent comp3;
    TimeSpan? nullable;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1, out comp2_1, out comp3))
    {
      if (!comp2_1.NextEgg.HasValue)
      {
        comp2_1.NextEgg = new TimeSpan?(curTime + comp1_1.Cooldown);
      }
      else
      {
        TimeSpan timeSpan = curTime;
        nullable = comp2_1.NextEgg;
        MobStateComponent comp;
        if ((nullable.HasValue ? (timeSpan < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0 && (!this.TryComp<MobStateComponent>(uid1, out comp) || !this._mobState.IsIncapacitated(uid1, comp)))
        {
          comp2_1.NextEgg = new TimeSpan?(curTime + comp1_1.Cooldown);
          this.Dirty(uid1, (IComponent) comp2_1);
          EntityUid entityUid = this.SpawnAtPosition((string) comp1_1.Spawn, comp3.Coordinates.Offset(comp1_1.Offset));
          this._hive.SetSameHive((Entity<HiveMemberComponent>) uid1, (Entity<HiveMemberComponent>) entityUid);
          this._transform.SetLocalRotation(entityUid, Angle.Zero);
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoEggComponent, TransformComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<XenoEggComponent, TransformComponent>();
    EntityUid uid2;
    XenoEggComponent comp1_2;
    TransformComponent comp2_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2, out comp2_2))
    {
      StepTriggerComponent component;
      if (comp1_2.State == XenoEggState.Grown && this._stepTriggerQuery.TryComp(uid2, out component) && component.CurrentlySteppedOn.Count > 0)
      {
        foreach (EntityUid tripper in component.CurrentlySteppedOn)
        {
          if (this.TryTrigger((Entity<XenoEggComponent>) (uid2, comp1_2), tripper))
            break;
        }
      }
      if (comp2_2.Anchored)
      {
        if (curTime >= comp1_2.CheckWeedsAt)
        {
          comp1_2.CheckWeedsAt = curTime + comp1_2.CheckWeedsDelay;
          EntityUid? grid = this._transform.GetGrid(uid2.ToCoordinates());
          if (grid.HasValue)
          {
            EntityUid valueOrDefault = grid.GetValueOrDefault();
            MapGridComponent comp1;
            if (this.TryComp<MapGridComponent>(valueOrDefault, out comp1))
            {
              if (this._weeds.IsOnHiveWeeds((Entity<MapGridComponent>) (valueOrDefault, comp1), uid2.ToCoordinates()))
              {
                if (this.HasComp<XenoFragileEggComponent>(uid2))
                  this.RemCompDeferred<XenoFragileEggComponent>(uid2);
              }
              else
              {
                XenoFragileEggComponent comp2;
                if (!this.EnsureComp<XenoFragileEggComponent>(uid2, out comp2))
                {
                  comp2.ExpireAt = new TimeSpan?(curTime + comp1_2.FragileEggDuration);
                  this.SetEggSprite((Entity<XenoEggComponent>) (uid2, comp1_2), comp1_2.FragileSprite);
                  this._nameModifier.RefreshNameModifiers((Entity<NameModifierComponent>) uid2);
                }
              }
            }
            else
              continue;
          }
          else
            continue;
        }
        TimeSpan valueOrDefault1;
        if (comp1_2.State == XenoEggState.Growing)
        {
          XenoEggComponent xenoEggComponent = comp1_2;
          valueOrDefault1 = xenoEggComponent.GrowAt.GetValueOrDefault();
          if (!xenoEggComponent.GrowAt.HasValue)
          {
            TimeSpan timeSpan = curTime + this._random.Next(comp1_2.MinTime, comp1_2.MaxTime);
            xenoEggComponent.GrowAt = new TimeSpan?(timeSpan);
          }
          TimeSpan timeSpan1 = curTime;
          nullable = comp1_2.GrowAt;
          if ((nullable.HasValue ? (timeSpan1 < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0 && comp1_2.State == XenoEggState.Growing)
            this.SetEggState((Entity<XenoEggComponent>) (uid2, comp1_2), XenoEggState.Grown);
          else
            continue;
        }
        if (comp1_2.State == XenoEggState.Opening)
        {
          XenoEggComponent xenoEggComponent = comp1_2;
          valueOrDefault1 = xenoEggComponent.OpenAt.GetValueOrDefault();
          if (!xenoEggComponent.OpenAt.HasValue)
          {
            TimeSpan timeSpan = curTime + comp1_2.EggOpenTime;
            xenoEggComponent.OpenAt = new TimeSpan?(timeSpan);
          }
          TimeSpan timeSpan2 = curTime;
          nullable = comp1_2.OpenAt;
          if ((nullable.HasValue ? (timeSpan2 < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0 && comp1_2.State == XenoEggState.Opening)
          {
            this.SetEggState((Entity<XenoEggComponent>) (uid2, comp1_2), XenoEggState.Opened);
            EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(uid2);
            BaseContainer container;
            if (this._container.TryGetContainer(uid2, comp1_2.CreatureContainerId, out container))
              this._container.EmptyContainer(container, destination: new EntityCoordinates?(moverCoordinates));
            if (comp1_2.SpawnedCreature.HasValue)
            {
              this._jitter.DoJitter(comp1_2.SpawnedCreature.Value, comp1_2.CreatureExitEggJitterDuration, true, 80f, 8f, true);
              XenoParasiteComponent comp;
              if (comp1_2.InfectTarget.HasValue && this.TryComp<XenoParasiteComponent>(comp1_2.SpawnedCreature, out comp))
              {
                this._parasite.Infect((Entity<XenoParasiteComponent>) (comp1_2.SpawnedCreature.Value, comp), comp1_2.InfectTarget.Value, force: true);
                this._stun.TryParalyze(comp1_2.InfectTarget.Value, comp1_2.KnockdownTime, true);
              }
              comp1_2.InfectTarget = new EntityUid?();
              comp1_2.SpawnedCreature = new EntityUid?();
              comp1_2.OpenAt = new TimeSpan?();
              this.Dirty(uid2, (IComponent) comp1_2);
            }
          }
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoFragileEggComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<XenoFragileEggComponent>();
    EntityUid uid3;
    XenoFragileEggComponent comp1_3;
    while (entityQueryEnumerator3.MoveNext(out uid3, out comp1_3))
    {
      if (comp1_3.BurstAt.HasValue)
      {
        TimeSpan timeSpan = curTime;
        nullable = comp1_3.BurstAt;
        if ((nullable.HasValue ? (timeSpan < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
          this._destruction.DestroyEntity(uid3);
      }
      else
      {
        if (comp1_3.SustainedBy.HasValue)
        {
          if (!comp1_3.InRange)
          {
            TimeSpan timeSpan = curTime;
            nullable = comp1_3.ShortExpireAt;
            if ((nullable.HasValue ? (timeSpan >= nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
            {
              comp1_3.BurstAt = new TimeSpan?(curTime + comp1_3.BurstDelay);
              this._jitter.DoJitter(uid3, comp1_3.BurstDelay / 2.0, true, 40f, 8f, true);
              continue;
            }
          }
          TimeSpan timeSpan3 = curTime;
          nullable = comp1_3.CheckSustainAt;
          if ((nullable.HasValue ? (timeSpan3 >= nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          {
            comp1_3.CheckSustainAt = new TimeSpan?(curTime + comp1_3.SustainCheckEvery);
            float distance;
            if (uid3.ToCoordinates().TryDistance((IEntityManager) this.EntityManager, comp1_3.SustainedBy.Value.ToCoordinates(), out distance))
            {
              if ((double) distance > (double) comp1_3.SustainRange)
              {
                comp1_3.InRange = false;
              }
              else
              {
                comp1_3.InRange = true;
                comp1_3.ShortExpireAt = new TimeSpan?(curTime + comp1_3.SustainDuration);
              }
            }
            else
              continue;
          }
        }
        TimeSpan timeSpan4 = curTime;
        nullable = comp1_3.ExpireAt;
        if ((nullable.HasValue ? (timeSpan4 < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
        {
          comp1_3.BurstAt = new TimeSpan?(curTime + comp1_3.BurstDelay);
          this._jitter.DoJitter(uid3, comp1_3.BurstDelay / 2.0, true, 40f, 8f, true);
        }
      }
    }
  }
}
