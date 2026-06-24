// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Parasite.SharedXenoParasiteSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Gibbing;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared._RMC14.NPC;
using Content.Shared._RMC14.Sprite;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Construction.EggMorpher;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Construction.ResinHole;
using Content.Shared._RMC14.Xenonids.Construction.ResinWhisper;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Hide;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Content.Shared._RMC14.Xenonids.Projectile.Parasite;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.DragDrop;
using Content.Shared.Examine;
using Content.Shared.Ghost;
using Content.Shared.Humanoid;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Jittering;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Rejuvenate;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Throwing;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Parasite;

public abstract class SharedXenoParasiteSystem : EntitySystem
{
  [Dependency]
  private SharedRMCNPCSystem _rmcNpc;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  private const string RipOffOnInfectionTag = "RipOffOnInfection";
  [Dependency]
  private SharedActionsSystem _action;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private RMCHandsSystem _rmcHands;
  [Dependency]
  private SharedRMCSpriteSystem _rmcSprite;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedJitteringSystem _jitter;
  [Dependency]
  private DamageableSystem _damage;
  [Dependency]
  private StatusEffectsSystem _status;
  [Dependency]
  private SharedRottingSystem _rotting;
  [Dependency]
  private TagSystem _tagSystem;
  [Dependency]
  private RMCSizeStunSystem _size;
  [Dependency]
  private RMCUnrevivableSystem _unrevivable;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  private const CollisionGroup LeapCollisionGroup = CollisionGroup.InteractImpassable;
  private const CollisionGroup ThrownCollisionGroup = CollisionGroup.InteractImpassable | CollisionGroup.BarricadeImpassable;
  protected readonly ProtoId<TagPrototype> ParasiteIsPreparingLeapProtoID = new ProtoId<TagPrototype>("RMCXenoParasitePreparingLeap");

  public void IntializeAI()
  {
    this.SubscribeLocalEvent<XenoParasiteComponent, PlayerAttachedEvent>(new EntityEventRefHandler<XenoParasiteComponent, PlayerAttachedEvent>(this.OnPlayerAdded));
    this.SubscribeLocalEvent<XenoParasiteComponent, PlayerDetachedEvent>(new EntityEventRefHandler<XenoParasiteComponent, PlayerDetachedEvent>(this.OnPlayerRemoved));
    this.SubscribeLocalEvent<ParasiteAIDelayAddComponent, ComponentStartup>(new EntityEventRefHandler<ParasiteAIDelayAddComponent, ComponentStartup>(this.OnAIDelayAdded));
    this.SubscribeLocalEvent<ParasiteAIComponent, MapInitEvent>(new EntityEventRefHandler<ParasiteAIComponent, MapInitEvent>(this.OnAIAdded));
    this.SubscribeLocalEvent<ParasiteAIComponent, ExaminedEvent>(new EntityEventRefHandler<ParasiteAIComponent, ExaminedEvent>(this.OnAIExamined));
    this.SubscribeLocalEvent<ParasiteAIComponent, DroppedEvent>(new EntityEventRefHandler<ParasiteAIComponent, DroppedEvent>(this.OnAIDropPickup<DroppedEvent>));
    this.SubscribeLocalEvent<ParasiteAIComponent, EntGotInsertedIntoContainerMessage>(new EntityEventRefHandler<ParasiteAIComponent, EntGotInsertedIntoContainerMessage>(this.OnAIDropPickup<EntGotInsertedIntoContainerMessage>));
    this.SubscribeLocalEvent<ParasiteAIComponent, GetVerbsEvent<ActivationVerb>>(new EntityEventRefHandler<ParasiteAIComponent, GetVerbsEvent<ActivationVerb>>(this.OnGetVerbs));
    this.SubscribeLocalEvent<TrapParasiteComponent, ComponentStartup>(new EntityEventRefHandler<TrapParasiteComponent, ComponentStartup>(this.OnTrapAdded));
    this.SubscribeLocalEvent<TrapParasiteComponent, PlayerAttachedEvent>(new EntityEventRefHandler<TrapParasiteComponent, PlayerAttachedEvent>(this.OnStopTrap<PlayerAttachedEvent>));
    this.SubscribeLocalEvent<TrapParasiteComponent, EntGotInsertedIntoContainerMessage>(new EntityEventRefHandler<TrapParasiteComponent, EntGotInsertedIntoContainerMessage>(this.OnStopTrap<EntGotInsertedIntoContainerMessage>));
    this.SubscribeLocalEvent<TrapParasiteComponent, XenoLeapHitEvent>(new EntityEventRefHandler<TrapParasiteComponent, XenoLeapHitEvent>(this.OnLeapEndTrap));
    this.SubscribeLocalEvent<TrapParasiteComponent, ComponentShutdown>(new EntityEventRefHandler<TrapParasiteComponent, ComponentShutdown>(this.OnTrapEnd));
    this.SubscribeLocalEvent<ParasiteTiredOutComponent, MapInitEvent>(new EntityEventRefHandler<ParasiteTiredOutComponent, MapInitEvent>(this.OnParasiteAIMapInit));
    this.SubscribeLocalEvent<ParasiteTiredOutComponent, UpdateMobStateEvent>(new EntityEventRefHandler<ParasiteTiredOutComponent, UpdateMobStateEvent>(this.OnParasiteAIUpdateMobState), after: new Type[2]
    {
      typeof (MobThresholdSystem),
      typeof (SharedXenoPheromonesSystem)
    });
  }

  private void OnTrapAdded(Entity<TrapParasiteComponent> para, ref ComponentStartup args)
  {
    para.Comp.LeapAt = new TimeSpan?(this._timing.CurTime + para.Comp.JumpTime);
    TrapParasiteComponent comp1 = para.Comp;
    TimeSpan? leapAt = para.Comp.LeapAt;
    TimeSpan disableTime = para.Comp.DisableTime;
    TimeSpan? nullable = leapAt.HasValue ? new TimeSpan?(leapAt.GetValueOrDefault() + disableTime) : new TimeSpan?();
    comp1.DisableAt = nullable;
    XenoLeapComponent comp2;
    if (!this.TryComp<XenoLeapComponent>((EntityUid) para, out comp2))
      return;
    para.Comp.NormalLeapDelay = comp2.Delay;
    comp2.Delay = TimeSpan.Zero;
    this._rmcNpc.SleepNPC((EntityUid) para);
  }

  private void OnStopTrap<T>(Entity<TrapParasiteComponent> para, ref T args) where T : EntityEventArgs
  {
    this.RemCompDeferred<TrapParasiteComponent>((EntityUid) para);
  }

  private void OnTrapEnd(Entity<TrapParasiteComponent> para, ref ComponentShutdown args)
  {
    XenoLeapComponent comp;
    if (!this.TryComp<XenoLeapComponent>((EntityUid) para, out comp))
      return;
    comp.Delay = para.Comp.NormalLeapDelay;
    this._rmcNpc.SleepNPC((EntityUid) para);
  }

  private void OnLeapEndTrap(Entity<TrapParasiteComponent> para, ref XenoLeapHitEvent args)
  {
    this.ResetTrapState(para);
  }

  public void ResetTrapState(Entity<TrapParasiteComponent> para)
  {
    XenoLeapComponent comp;
    if (!this.TryComp<XenoLeapComponent>((EntityUid) para, out comp))
      return;
    if (this.TryComp<ParasiteAIComponent>((EntityUid) para, out ParasiteAIComponent _))
      this._rmcNpc.SleepNPC((EntityUid) para);
    comp.Delay = para.Comp.NormalLeapDelay;
    this.RemCompDeferred<TrapParasiteComponent>((EntityUid) para);
  }

  private void OnPlayerAdded(Entity<XenoParasiteComponent> para, ref PlayerAttachedEvent args)
  {
    this.RemCompDeferred<ParasiteAIComponent>((EntityUid) para);
    this.RemCompDeferred<ParasiteAIDelayAddComponent>((EntityUid) para);
  }

  private void OnPlayerRemoved(Entity<XenoParasiteComponent> para, ref PlayerDetachedEvent args)
  {
    if (this.TerminatingOrDeleted((EntityUid) para))
      return;
    this.EnsureComp<ParasiteAIDelayAddComponent>((EntityUid) para);
  }

  private void OnAIDelayAdded(Entity<ParasiteAIDelayAddComponent> para, ref ComponentStartup args)
  {
    para.Comp.TimeToAI = this._timing.CurTime + para.Comp.DelayTime;
    this._rmcNpc.SleepNPC((EntityUid) para);
  }

  private void OnAIAdded(Entity<ParasiteAIComponent> para, ref MapInitEvent args)
  {
    if (this._mobState.IsDead((EntityUid) para))
      return;
    this.HandleDeathTimer(para);
    this.GoActive(para);
  }

  private void OnAIExamined(Entity<ParasiteAIComponent> para, ref ExaminedEvent args)
  {
    if (this._mobState.IsDead((EntityUid) para) || !this.HasComp<XenoComponent>(args.Examiner))
      return;
    switch (para.Comp.Mode)
    {
      case ParasiteMode.Idle:
        args.PushMarkup(this.Loc.GetString("rmc-xeno-parasite-ai-idle", ("parasite", (object) para)) ?? "");
        break;
      case ParasiteMode.Active:
        args.PushMarkup(this.Loc.GetString("rmc-xeno-parasite-ai-active", ("parasite", (object) para)) ?? "");
        break;
      case ParasiteMode.Dying:
        args.PushMarkup(this.Loc.GetString("rmc-xeno-parasite-ai-dying", ("parasite", (object) para)) ?? "");
        break;
    }
  }

  private void OnAIDropPickup<T>(Entity<ParasiteAIComponent> para, ref T args) where T : EntityEventArgs
  {
    this.HandleDeathTimer(para);
    this.GoIdle(para);
  }

  public void HandleDeathTimer(Entity<ParasiteAIComponent> para)
  {
    BaseContainer container;
    if (this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) para, (TransformComponent) null, (MetaDataComponent) null), out container) && this.HasComp<XenoNurturingComponent>(container.Owner))
    {
      para.Comp.DeathTime = new TimeSpan?();
      if (para.Comp.Mode != ParasiteMode.Dying)
        return;
      para.Comp.Mode = ParasiteMode.Active;
      this.GoIdle(para);
    }
    else
    {
      if (para.Comp.DeathTime.HasValue)
        return;
      para.Comp.DeathTime = new TimeSpan?(this._timing.CurTime + para.Comp.LifeTime);
    }
  }

  public void UpdateAI(Entity<ParasiteAIComponent> para, TimeSpan currentTime)
  {
    this.CheckCannibalize(para);
    if (para.Comp.DeathTime.HasValue)
    {
      TimeSpan timeSpan = currentTime;
      TimeSpan? deathTime = para.Comp.DeathTime;
      if ((deathTime.HasValue ? (timeSpan > deathTime.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        goto label_3;
    }
    if (para.Comp.JumpsLeft > 0)
    {
      if (para.Comp.Mode == ParasiteMode.Active)
      {
        TimeSpan timeSpan = currentTime;
        TimeSpan? nextJump = para.Comp.NextJump;
        if ((nextJump.HasValue ? (timeSpan >= nextJump.GetValueOrDefault() ? 1 : 0) : 0) != 0 && !this._container.IsEntityInContainer((EntityUid) para))
        {
          if (!this.HasComp<StunnedComponent>((EntityUid) para))
            this.EnsureComp<TrapParasiteComponent>((EntityUid) para).JumpTime = TimeSpan.Zero;
          para.Comp.NextJump = new TimeSpan?(currentTime + para.Comp.JumpTime);
        }
      }
      if (para.Comp.Mode != ParasiteMode.Idle || !(currentTime > para.Comp.NextActiveTime))
        return;
      this.GoActive(para);
      return;
    }
label_3:
    if (para.Comp.Mode != ParasiteMode.Dying)
    {
      para.Comp.Mode = ParasiteMode.Dying;
      if (this.HasComp<XenoRestingComponent>((EntityUid) para))
        this.DoRestAction(para);
      this.ChangeHTN((EntityUid) para, ParasiteMode.Dying);
      this._rmcNpc.WakeNPC((EntityUid) para);
      this.Dirty<ParasiteAIComponent>(para);
    }
    this.CheckDeath(para);
  }

  public void GoIdle(Entity<ParasiteAIComponent> para)
  {
    para.Comp.JumpsLeft = para.Comp.InitialJumps;
    if (para.Comp.Mode != ParasiteMode.Active)
      return;
    if (!this.HasComp<XenoRestingComponent>((EntityUid) para))
      this.DoRestAction(para);
    this._rmcNpc.SleepNPC((EntityUid) para);
    para.Comp.Mode = ParasiteMode.Idle;
    if (this.HasComp<TrapParasiteComponent>((EntityUid) para))
      this.RemCompDeferred<TrapParasiteComponent>((EntityUid) para);
    para.Comp.NextActiveTime = this._timing.CurTime + TimeSpan.FromSeconds((long) this._random.Next(para.Comp.MinIdleTime, para.Comp.MaxIdleTime + 1));
    this.Dirty<ParasiteAIComponent>(para);
  }

  public void GoActive(Entity<ParasiteAIComponent> para)
  {
    if (para.Comp.Mode == ParasiteMode.Dying)
      return;
    if (this.HasComp<XenoRestingComponent>((EntityUid) para))
      this.DoRestAction(para);
    this.ChangeHTN((EntityUid) para, ParasiteMode.Active);
    para.Comp.Mode = ParasiteMode.Active;
    this._rmcNpc.SleepNPC((EntityUid) para);
    if (this.HasComp<TrapParasiteComponent>((EntityUid) para))
      this.RemCompDeferred<TrapParasiteComponent>((EntityUid) para);
    para.Comp.NextJump = new TimeSpan?(this._timing.CurTime + para.Comp.JumpTime);
    this.Dirty<ParasiteAIComponent>(para);
  }

  private void DoRestAction(Entity<ParasiteAIComponent> para)
  {
    XenoComponent comp1;
    EntityUid uid;
    ActionComponent comp2;
    if (!this.TryComp<XenoComponent>((EntityUid) para, out comp1) || !comp1.Initialized || !comp1.Actions.TryGetValue((EntProtoId) para.Comp.RestAction, out uid) || !this.TryComp<ActionComponent>(uid, out comp2))
      return;
    BaseActionEvent actionEvent = this._action.GetEvent(uid);
    this._action.PerformAction((Entity<ActionsComponent>) para.Owner, (Entity<ActionComponent>) (uid, comp2), actionEvent);
  }

  protected virtual void ChangeHTN(EntityUid parasite, ParasiteMode mode)
  {
  }

  private void CheckCannibalize(Entity<ParasiteAIComponent> para)
  {
    EntityUid user;
    if (this._rmcHands.TryGetHolder((EntityUid) para, out user) || this.HasComp<ThrownItemComponent>((EntityUid) para))
      return;
    int num = 0;
    foreach (Entity<ParasiteAIComponent> entity in this._entityLookup.GetEntitiesInRange<ParasiteAIComponent>(this._transform.GetMapCoordinates((EntityUid) para), para.Comp.CannibalizeCheck))
    {
      if (!(entity == para) && !this.TerminatingOrDeleted((EntityUid) entity) && !this.EntityManager.IsQueuedForDeletion((EntityUid) entity) && !this._mobState.IsDead((EntityUid) entity) && entity.Comp.Mode == ParasiteMode.Active && !this._rmcHands.TryGetHolder((EntityUid) entity, out user) && !this.HasComp<ThrownItemComponent>((EntityUid) entity) && !this.HasComp<StunnedComponent>((EntityUid) entity))
        ++num;
    }
    if (num <= para.Comp.MaxSurroundingParas)
      return;
    this._popup.PopupCoordinates(this.Loc.GetString("rmc-xeno-parasite-ai-eaten", ("parasite", (object) para)), this._transform.GetMoverCoordinates((EntityUid) para), PopupType.SmallCaution);
    this.QueueDel(new EntityUid?((EntityUid) para));
  }

  private void CheckDeath(Entity<ParasiteAIComponent> para)
  {
    foreach (Entity<XenoEggComponent> entity in this._entityLookup.GetEntitiesInRange<XenoEggComponent>(this._transform.GetMoverCoordinates((EntityUid) para), para.Comp.RangeCheck))
    {
      if (entity.Comp.State == XenoEggState.Opened)
        return;
    }
    foreach (Entity<XenoResinHoleComponent> entity in this._entityLookup.GetEntitiesInRange<XenoResinHoleComponent>(this._transform.GetMoverCoordinates((EntityUid) para), para.Comp.RangeCheck))
    {
      if (!entity.Comp.TrapPrototype.HasValue)
        return;
    }
    foreach (Entity<EggMorpherComponent> entity in this._entityLookup.GetEntitiesInRange<EggMorpherComponent>(this._transform.GetMoverCoordinates((EntityUid) para), para.Comp.RangeCheck))
    {
      if (entity.Comp.CurParasites < entity.Comp.MaxParasites)
        return;
    }
    this.EnsureComp<ParasiteTiredOutComponent>((EntityUid) para);
  }

  private void OnParasiteAIMapInit(Entity<ParasiteTiredOutComponent> dead, ref MapInitEvent args)
  {
    MobStateComponent comp;
    if (!this.TryComp<MobStateComponent>((EntityUid) dead, out comp))
      return;
    this._mobState.UpdateMobState((EntityUid) dead, comp);
  }

  private void OnParasiteAIUpdateMobState(
    Entity<ParasiteTiredOutComponent> dead,
    ref UpdateMobStateEvent args)
  {
    args.State = MobState.Dead;
  }

  private void OnGetVerbs(Entity<ParasiteAIComponent> ent, ref GetVerbsEvent<ActivationVerb> args)
  {
    EntityUid uid = args.User;
    if (!this.HasComp<ActorComponent>(uid) || !this.HasComp<GhostComponent>(uid) || !this._mobState.IsAlive((EntityUid) ent))
      return;
    ActivationVerb activationVerb1 = new ActivationVerb();
    activationVerb1.Text = this.Loc.GetString("rmc-xeno-egg-ghost-verb");
    activationVerb1.Act = (Action) (() => this._ui.TryOpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) XenoParasiteGhostUI.Key, uid));
    activationVerb1.Impact = LogImpact.High;
    ActivationVerb activationVerb2 = activationVerb1;
    args.Verbs.Add(activationVerb2);
  }

  public override void Initialize()
  {
    this.SubscribeLocalEvent<InfectableComponent, ActivateInWorldEvent>(new EntityEventRefHandler<InfectableComponent, ActivateInWorldEvent>(this.OnInfectableActivate));
    this.SubscribeLocalEvent<InfectableComponent, CanDropTargetEvent>(new EntityEventRefHandler<InfectableComponent, CanDropTargetEvent>(this.OnInfectableCanDropTarget));
    this.SubscribeLocalEvent<XenoParasiteComponent, XenoLeapHitEvent>(new EntityEventRefHandler<XenoParasiteComponent, XenoLeapHitEvent>(this.OnParasiteLeapHit));
    this.SubscribeLocalEvent<XenoParasiteComponent, AfterInteractEvent>(new EntityEventRefHandler<XenoParasiteComponent, AfterInteractEvent>(this.OnParasiteAfterInteract));
    this.SubscribeLocalEvent<XenoParasiteComponent, BeforeInteractHandEvent>(new EntityEventRefHandler<XenoParasiteComponent, BeforeInteractHandEvent>(this.OnParasiteInteractHand));
    this.SubscribeLocalEvent<XenoParasiteComponent, DoAfterAttemptEvent<AttachParasiteDoAfterEvent>>(new EntityEventRefHandler<XenoParasiteComponent, DoAfterAttemptEvent<AttachParasiteDoAfterEvent>>(this.OnParasiteAttachDoAfterAttempt));
    this.SubscribeLocalEvent<XenoParasiteComponent, AttachParasiteDoAfterEvent>(new EntityEventRefHandler<XenoParasiteComponent, AttachParasiteDoAfterEvent>(this.OnParasiteAttachDoAfter));
    this.SubscribeLocalEvent<XenoParasiteComponent, CanDragEvent>(new EntityEventRefHandler<XenoParasiteComponent, CanDragEvent>(this.OnParasiteCanDrag));
    this.SubscribeLocalEvent<XenoParasiteComponent, CanDropDraggedEvent>(new EntityEventRefHandler<XenoParasiteComponent, CanDropDraggedEvent>(this.OnParasiteCanDropDragged));
    this.SubscribeLocalEvent<XenoParasiteComponent, DragDropDraggedEvent>(new EntityEventRefHandler<XenoParasiteComponent, DragDropDraggedEvent>(this.OnParasiteDragDropDragged));
    this.SubscribeLocalEvent<XenoParasiteComponent, ThrowItemAttemptEvent>(new EntityEventRefHandler<XenoParasiteComponent, ThrowItemAttemptEvent>(this.OnParasiteThrowAttempt));
    this.SubscribeLocalEvent<XenoParasiteComponent, PullAttemptEvent>(new EntityEventRefHandler<XenoParasiteComponent, PullAttemptEvent>(this.OnParasiteTryPull));
    this.SubscribeLocalEvent<XenoParasiteComponent, GettingPickedUpAttemptEvent>(new EntityEventRefHandler<XenoParasiteComponent, GettingPickedUpAttemptEvent>(this.OnParasiteTryPickup));
    this.SubscribeLocalEvent<XenoParasiteComponent, BeforeDamageChangedEvent>(new EntityEventRefHandler<XenoParasiteComponent, BeforeDamageChangedEvent>(this.OnParasiteBeforeDamageChanged));
    this.SubscribeLocalEvent<XenoParasiteComponent, XenoLeapActionEvent>(new EntityEventRefHandler<XenoParasiteComponent, XenoLeapActionEvent>(this.OnParasiteLeap));
    this.SubscribeLocalEvent<XenoParasiteComponent, XenoLeapAttemptEvent>(new EntityEventRefHandler<XenoParasiteComponent, XenoLeapAttemptEvent>(this.OnParasiteLeapAttempt));
    this.SubscribeLocalEvent<XenoParasiteComponent, XenoLeapDoAfterEvent>(new EntityEventRefHandler<XenoParasiteComponent, XenoLeapDoAfterEvent>(this.OnParasiteLeapDoAfter));
    this.SubscribeLocalEvent<XenoParasiteComponent, XenoLeapStoppedEvent>(new EntityEventRefHandler<XenoParasiteComponent, XenoLeapStoppedEvent>(this.OnParasiteLeapStopped));
    this.SubscribeLocalEvent<XenoParasiteComponent, ThrownEvent>(new EntityEventRefHandler<XenoParasiteComponent, ThrownEvent>(this.OnParasiteThrown));
    this.SubscribeLocalEvent<XenoParasiteComponent, LandEvent>(new EntityEventRefHandler<XenoParasiteComponent, LandEvent>(this.OnParasiteLand));
    this.SubscribeLocalEvent<ParasiteSpentComponent, MapInitEvent>(new EntityEventRefHandler<ParasiteSpentComponent, MapInitEvent>(this.OnParasiteSpentMapInit));
    this.SubscribeLocalEvent<ParasiteSpentComponent, UpdateMobStateEvent>(new EntityEventRefHandler<ParasiteSpentComponent, UpdateMobStateEvent>(this.OnParasiteSpentUpdateMobState), after: new Type[2]
    {
      typeof (MobThresholdSystem),
      typeof (SharedXenoPheromonesSystem)
    });
    this.SubscribeLocalEvent<ParasiteSpentComponent, ExaminedEvent>(new EntityEventRefHandler<ParasiteSpentComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<VictimInfectedComponent, MapInitEvent>(new EntityEventRefHandler<VictimInfectedComponent, MapInitEvent>(this.OnVictimInfectedMapInit));
    this.SubscribeLocalEvent<VictimInfectedComponent, ComponentRemove>(new EntityEventRefHandler<VictimInfectedComponent, ComponentRemove>(this.OnVictimInfectedRemoved));
    this.SubscribeLocalEvent<VictimInfectedComponent, ExaminedEvent>(new EntityEventRefHandler<VictimInfectedComponent, ExaminedEvent>(this.OnVictimInfectedExamined));
    this.SubscribeLocalEvent<VictimInfectedComponent, RejuvenateEvent>(new EntityEventRefHandler<VictimInfectedComponent, RejuvenateEvent>(this.OnVictimInfectedRejuvenate));
    this.SubscribeLocalEvent<VictimInfectedComponent, LarvaBurstDoAfterEvent>(new EntityEventRefHandler<VictimInfectedComponent, LarvaBurstDoAfterEvent>(this.OnBurst));
    this.SubscribeLocalEvent<VictimBurstComponent, MapInitEvent>(new EntityEventRefHandler<VictimBurstComponent, MapInitEvent>(this.OnVictimBurstMapInit));
    this.SubscribeLocalEvent<VictimBurstComponent, UpdateMobStateEvent>(new EntityEventRefHandler<VictimBurstComponent, UpdateMobStateEvent>(this.OnVictimUpdateMobState), after: new Type[2]
    {
      typeof (MobThresholdSystem),
      typeof (SharedXenoPheromonesSystem)
    });
    this.SubscribeLocalEvent<VictimBurstComponent, RejuvenateEvent>(new EntityEventRefHandler<VictimBurstComponent, RejuvenateEvent>(this.OnVictimBurstRejuvenate));
    this.SubscribeLocalEvent<VictimBurstComponent, ExaminedEvent>(new EntityEventRefHandler<VictimBurstComponent, ExaminedEvent>(this.OnVictimBurstExamine));
    this.SubscribeLocalEvent<BursterComponent, MoveInputEvent>(new EntityEventRefHandler<BursterComponent, MoveInputEvent>(this.OnTryMove));
    this.IntializeAI();
  }

  private void OnInfectableActivate(Entity<InfectableComponent> ent, ref ActivateInWorldEvent args)
  {
    XenoParasiteComponent comp;
    if (!this.TryComp<XenoParasiteComponent>(args.User, out comp) || !this.StartInfect((Entity<XenoParasiteComponent>) (args.User, comp), args.Target, args.User))
      return;
    args.Handled = true;
  }

  private void OnInfectableCanDropTarget(
    Entity<InfectableComponent> ent,
    ref CanDropTargetEvent args)
  {
    XenoParasiteComponent comp;
    if (!this.TryComp<XenoParasiteComponent>(args.Dragged, out comp) || !this.CanInfectPopup((Entity<XenoParasiteComponent>) (args.Dragged, comp), (EntityUid) ent, args.User, false))
      return;
    args.CanDrop = true;
    args.Handled = true;
  }

  private void OnParasiteLeapHit(Entity<XenoParasiteComponent> parasite, ref XenoLeapHitEvent args)
  {
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) parasite);
    ParasiteAIComponent comp;
    float range = this.TryComp<ParasiteAIComponent>((EntityUid) parasite, out comp) ? (float) comp.MaxInfectRange : parasite.Comp.InfectRange;
    if (!this._transform.InRange(moverCoordinates, args.Leaping.Origin, range))
      return;
    this.Infect(parasite, args.Hit, false);
  }

  private void OnParasiteAfterInteract(
    Entity<XenoParasiteComponent> ent,
    ref AfterInteractEvent args)
  {
    if (!args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue || args.Handled || !this._rmcHands.IsPickupByAllowed((Entity<WhitelistPickupByComponent>) ent.Owner, (Entity<WhitelistPickupComponent>) args.User))
      return;
    Entity<XenoParasiteComponent> parasite = ent;
    target = args.Target;
    EntityUid victim = target.Value;
    EntityUid user = args.User;
    if (!this.StartInfect(parasite, victim, user))
      return;
    args.Handled = true;
  }

  private void OnParasiteInteractHand(
    Entity<XenoParasiteComponent> ent,
    ref BeforeInteractHandEvent args)
  {
    if (!this.IsInfectable(ent, args.Target))
      return;
    this.StartInfect(ent, args.Target, (EntityUid) ent);
    args.Handled = true;
  }

  private void OnParasiteAttachDoAfterAttempt(
    Entity<XenoParasiteComponent> ent,
    ref DoAfterAttemptEvent<AttachParasiteDoAfterEvent> args)
  {
    EntityUid? target = args.DoAfter.Args.Target;
    if (target.HasValue)
    {
      EntityUid valueOrDefault = target.GetValueOrDefault();
      if (this.CanInfectPopup(ent, valueOrDefault, (EntityUid) ent))
        return;
      args.Cancel();
    }
    else
      args.Cancel();
  }

  private void OnParasiteAttachDoAfter(
    Entity<XenoParasiteComponent> ent,
    ref AttachParasiteDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    Entity<XenoParasiteComponent> parasite = ent;
    target = args.Target;
    EntityUid victim = target.Value;
    if (!this.Infect(parasite, victim))
      return;
    args.Handled = true;
  }

  private void OnParasiteCanDrag(Entity<XenoParasiteComponent> ent, ref CanDragEvent args)
  {
    args.Handled = true;
  }

  private void OnParasiteCanDropDragged(
    Entity<XenoParasiteComponent> ent,
    ref CanDropDraggedEvent args)
  {
    if (args.User != ent.Owner && !this._rmcHands.IsPickupByAllowed((Entity<WhitelistPickupByComponent>) ent.Owner, (Entity<WhitelistPickupComponent>) args.User) || !this.CanInfectPopup(ent, args.Target, args.User, false))
      return;
    args.CanDrop = true;
    args.Handled = true;
  }

  private void OnParasiteDragDropDragged(
    Entity<XenoParasiteComponent> ent,
    ref DragDropDraggedEvent args)
  {
    if (args.User != ent.Owner && !this._rmcHands.IsPickupByAllowed((Entity<WhitelistPickupByComponent>) ent.Owner, (Entity<WhitelistPickupComponent>) args.User))
      return;
    this.StartInfect(ent, args.Target, args.User);
    args.Handled = true;
  }

  private void OnParasiteThrowAttempt(
    Entity<XenoParasiteComponent> ent,
    ref ThrowItemAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    args.Cancelled = true;
    if (this._net.IsClient)
      return;
    EntityUid user = args.User;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-cant-throw", ("target", (object) ent)), user, user, PopupType.SmallCaution);
  }

  private void OnParasiteTryPull(Entity<XenoParasiteComponent> ent, ref PullAttemptEvent args)
  {
    if (!this.HasComp<ParasiteAIComponent>((EntityUid) ent) || this.HasComp<InfectableComponent>(args.PullerUid))
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-parasite-nonplayer-pull", ("parasite", (object) ent)), (EntityUid) ent, new EntityUid?(args.PullerUid), PopupType.SmallCaution);
    args.Cancelled = true;
  }

  private void OnParasiteTryPickup(
    Entity<XenoParasiteComponent> ent,
    ref GettingPickedUpAttemptEvent args)
  {
    if (!this.HasComp<ParasiteAIComponent>((EntityUid) ent))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-parasite-player-pickup", ("parasite", (object) ent)), (EntityUid) ent, new EntityUid?(args.User), PopupType.SmallCaution);
      args.Cancel();
    }
    else
    {
      if (!this.HasComp<OnFireComponent>(args.User))
        return;
      this._popup.PopupClient("Touching the parasite while you're on fire would burn it!", (EntityUid) ent, new EntityUid?(args.User), PopupType.MediumCaution);
      args.Cancel();
    }
  }

  private void OnParasiteBeforeDamageChanged(
    Entity<XenoParasiteComponent> ent,
    ref BeforeDamageChangedEvent args)
  {
    if (!ent.Comp.InfectedVictim.HasValue || ent.Comp.FellOff)
      return;
    args.Cancelled = true;
  }

  private void OnParasiteLeap(Entity<XenoParasiteComponent> ent, ref XenoLeapActionEvent args)
  {
    this._tagSystem.AddTag((EntityUid) ent, this.ParasiteIsPreparingLeapProtoID);
    int num = (int) this._rmcSprite.UpdateDrawDepth((EntityUid) ent);
    XenoHideComponent comp;
    if (!this.TryComp<XenoHideComponent>((EntityUid) ent, out comp) || !comp.Hiding)
      return;
    XenoHideActionEvent args1 = new XenoHideActionEvent();
    args1.Performer = (EntityUid) ent;
    args1.Toggle = false;
    this.RaiseLocalEvent<XenoHideActionEvent>((EntityUid) ent, args1);
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoHideActionEvent>((EntityUid) ent))
      this._action.SetEnabled(new Entity<ActionComponent>?(entity.AsNullable()), false);
  }

  private void OnParasiteLeapAttempt(
    Entity<XenoParasiteComponent> ent,
    ref XenoLeapAttemptEvent args)
  {
    if (args.Cancelled)
    {
      this._tagSystem.RemoveTag((EntityUid) ent, this.ParasiteIsPreparingLeapProtoID);
      int num = (int) this._rmcSprite.UpdateDrawDepth((EntityUid) ent);
      foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoHideActionEvent>((EntityUid) ent))
        this._action.SetEnabled(new Entity<ActionComponent>?(entity.AsNullable()), false);
    }
    else
    {
      foreach (EntityUid contactingEntity in this._physics.GetContactingEntities((EntityUid) ent))
      {
        if (this.HasComp<DoorComponent>(contactingEntity) && !this.HasComp<ResinDoorComponent>(contactingEntity))
        {
          this._popup.PopupClient(this.Loc.GetString("cm-xeno-leap-blocked"), this.Transform((EntityUid) ent).Coordinates, new EntityUid?((EntityUid) ent));
          args.Cancelled = true;
          break;
        }
      }
    }
  }

  private void OnParasiteLeapDoAfter(
    Entity<XenoParasiteComponent> ent,
    ref XenoLeapDoAfterEvent args)
  {
    this._tagSystem.RemoveTag((EntityUid) ent, this.ParasiteIsPreparingLeapProtoID);
    int num1 = (int) this._rmcSprite.UpdateDrawDepth((EntityUid) ent);
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoHideActionEvent>((EntityUid) ent))
      this._action.SetEnabled(new Entity<ActionComponent>?(entity.AsNullable()), true);
    if (args.Cancelled)
      return;
    HashSet<EntityUid> contactingEntities = this._physics.GetContactingEntities((EntityUid) ent);
    EntityUid? nullable1 = new EntityUid?();
    float? nullable2 = new float?();
    foreach (EntityUid entityUid in contactingEntities)
    {
      float distance;
      if (this.HasComp<DoorComponent>(entityUid) && this.HasComp<ResinDoorComponent>(entityUid) && this._physics.TryGetDistance((EntityUid) ent, entityUid, out distance))
      {
        if (nullable2.HasValue)
        {
          float? nullable3 = nullable2;
          float num2 = distance;
          if (!((double) nullable3.GetValueOrDefault() > (double) num2 & nullable3.HasValue))
            continue;
        }
        nullable1 = new EntityUid?(entityUid);
        nullable2 = new float?(distance);
      }
    }
    if (nullable1.HasValue)
      this.AddComp<PreventCollideComponent>((EntityUid) ent, new PreventCollideComponent()
      {
        Uid = nullable1.Value
      });
    FixturesComponent comp;
    if (!this.TryComp<FixturesComponent>((EntityUid) ent, out comp))
      return;
    KeyValuePair<string, Fixture> keyValuePair = comp.Fixtures.First<KeyValuePair<string, Fixture>>();
    this._physics.SetCollisionMask((EntityUid) ent, keyValuePair.Key, keyValuePair.Value, keyValuePair.Value.CollisionMask | 128 /*0x80*/);
  }

  private void OnParasiteLeapStopped(
    Entity<XenoParasiteComponent> ent,
    ref XenoLeapStoppedEvent args)
  {
    this.RemCompDeferred<PreventCollideComponent>((EntityUid) ent);
    FixturesComponent comp;
    if (!this.TryComp<FixturesComponent>((EntityUid) ent, out comp))
      return;
    KeyValuePair<string, Fixture> keyValuePair = comp.Fixtures.First<KeyValuePair<string, Fixture>>();
    if ((keyValuePair.Value.CollisionMask & 205) == 0)
      return;
    this._physics.SetCollisionMask((EntityUid) ent, keyValuePair.Key, keyValuePair.Value, keyValuePair.Value.CollisionMask ^ 128 /*0x80*/);
  }

  private void OnParasiteThrown(Entity<XenoParasiteComponent> ent, ref ThrownEvent args)
  {
    FixturesComponent comp;
    if (!this.TryComp<FixturesComponent>((EntityUid) ent, out comp))
      return;
    KeyValuePair<string, Fixture> keyValuePair = comp.Fixtures.First<KeyValuePair<string, Fixture>>();
    this._physics.SetCollisionMask((EntityUid) ent, keyValuePair.Key, keyValuePair.Value, keyValuePair.Value.CollisionMask | 67108992 /*0x04000080*/);
  }

  private void OnParasiteLand(Entity<XenoParasiteComponent> ent, ref LandEvent args)
  {
    FixturesComponent comp;
    if (!this.TryComp<FixturesComponent>((EntityUid) ent, out comp))
      return;
    KeyValuePair<string, Fixture> keyValuePair = comp.Fixtures.First<KeyValuePair<string, Fixture>>();
    if ((keyValuePair.Value.CollisionMask & 205 & 67108864 /*0x04000000*/) != 0)
      return;
    this._physics.SetCollisionMask((EntityUid) ent, keyValuePair.Key, keyValuePair.Value, keyValuePair.Value.CollisionMask ^ 67108992 /*0x04000080*/);
  }

  protected virtual void ParasiteLeapHit(Entity<XenoParasiteComponent> parasite)
  {
  }

  private void OnParasiteSpentMapInit(Entity<ParasiteSpentComponent> spent, ref MapInitEvent args)
  {
    MobStateComponent comp;
    if (!this.TryComp<MobStateComponent>((EntityUid) spent, out comp))
      return;
    this._mobState.UpdateMobState((EntityUid) spent, comp);
  }

  private void OnParasiteSpentUpdateMobState(
    Entity<ParasiteSpentComponent> spent,
    ref UpdateMobStateEvent args)
  {
    args.State = MobState.Dead;
  }

  private void OnExamined(Entity<ParasiteSpentComponent> spent, ref ExaminedEvent args)
  {
    args.PushMarkup($"[italic]{this.Loc.GetString("rmc-xeno-parasite-dead", ("parasite", (object) spent))}[/italic]");
  }

  private void OnVictimInfectedMapInit(
    Entity<VictimInfectedComponent> victim,
    ref MapInitEvent args)
  {
    victim.Comp.BurstAt = this._timing.CurTime + victim.Comp.BurstDelay;
  }

  private void OnVictimInfectedRemoved(
    Entity<VictimInfectedComponent> victim,
    ref ComponentRemove args)
  {
    if (this._status.HasStatusEffect((EntityUid) victim, "Unconscious"))
      this._status.TryRemoveStatusEffect((EntityUid) victim, "Unconscious");
    this._standing.Stand((EntityUid) victim);
  }

  private void OnVictimInfectedCancel<T>(Entity<VictimInfectedComponent> victim, ref T args) where T : CancellableEntityEventArgs
  {
    if (victim.Comp.LifeStage > ComponentLifeStage.Running)
      return;
    args.Cancel();
  }

  private void OnVictimInfectedExamined(
    Entity<VictimInfectedComponent> victim,
    ref ExaminedEvent args)
  {
    if (this.HasComp<XenoComponent>(args.Examiner))
    {
      args.PushMarkup("This one is hosting a sister! She will emerge in time.");
    }
    else
    {
      if (!this.HasComp<GhostComponent>(args.Examiner))
        return;
      args.PushMarkup("This creature is infected.");
    }
  }

  private void OnVictimInfectedRejuvenate(
    Entity<VictimInfectedComponent> victim,
    ref RejuvenateEvent args)
  {
    this.RemCompDeferred<VictimInfectedComponent>((EntityUid) victim);
  }

  private void OnVictimBurstMapInit(Entity<VictimBurstComponent> burst, ref MapInitEvent args)
  {
    this._appearance.SetData((EntityUid) burst, (Enum) BurstVisuals.Visuals, (object) VictimBurstState.Burst);
    this._unrevivable.MakeUnrevivable((Entity<RMCRevivableComponent>) burst.Owner);
  }

  private void OnVictimUpdateMobState(
    Entity<VictimBurstComponent> burst,
    ref UpdateMobStateEvent args)
  {
    args.State = MobState.Dead;
  }

  private void OnVictimBurstRejuvenate(Entity<VictimBurstComponent> burst, ref RejuvenateEvent args)
  {
    this.RemCompDeferred<VictimBurstComponent>((EntityUid) burst);
  }

  private void OnVictimBurstExamine(Entity<VictimBurstComponent> burst, ref ExaminedEvent args)
  {
    using (args.PushGroup("VictimBurstComponent"))
      args.PushMarkup($"[color=red][bold]{this.Loc.GetString("rmc-xeno-infected-bursted", ("victim", (object) burst))}[/bold][/color]");
  }

  private bool StartInfect(
    Entity<XenoParasiteComponent> parasite,
    EntityUid victim,
    EntityUid user)
  {
    if (!this.CanInfectPopup(parasite, victim, user))
      return false;
    AttachParasiteDoAfterEvent @event = new AttachParasiteDoAfterEvent();
    TimeSpan delay = parasite.Comp.ManualAttachDelay;
    if (parasite.Owner == user)
      delay = parasite.Comp.SelfAttachDelay;
    if (this.HasComp<TrapParasiteComponent>((EntityUid) parasite))
      delay = TimeSpan.Zero;
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) parasite), new EntityUid?(victim))
    {
      BreakOnMove = true,
      BlockDuplicate = true,
      DuplicateCondition = DuplicateConditions.SameEvent,
      AttemptFrequency = AttemptFrequency.EveryTick
    });
    return true;
  }

  private bool IsInfectable(Entity<XenoParasiteComponent> parasite, EntityUid victim)
  {
    InfectableComponent comp;
    return this.TryComp<InfectableComponent>(victim, out comp) && !parasite.Comp.InfectedVictim.HasValue && !comp.BeingInfected && !this.HasComp<ParasiteSpentComponent>((EntityUid) parasite) && !this.HasComp<VictimInfectedComponent>(victim);
  }

  private bool CanInfectPopup(
    Entity<XenoParasiteComponent> parasite,
    EntityUid victim,
    EntityUid user,
    bool popup = true,
    bool force = false)
  {
    if (!this.IsInfectable(parasite, victim))
    {
      if (popup)
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-failed-cant-infect", ("target", (object) victim)), victim, new EntityUid?(user), PopupType.MediumCaution);
      return false;
    }
    StandingStateComponent comp;
    if (!force && !this.HasComp<XenoNestedComponent>(victim) && this.TryComp<StandingStateComponent>(victim, out comp) && !this._standing.IsDown(victim, comp))
    {
      if (popup)
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-failed-cant-reach", ("target", (object) victim)), victim, new EntityUid?(user), PopupType.MediumCaution);
      return false;
    }
    if (this._mobState.IsDead(victim))
    {
      if (popup)
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-failed-target-dead"), victim, new EntityUid?(user), PopupType.MediumCaution);
      return false;
    }
    if (!this._mobState.IsDead((EntityUid) parasite))
      return true;
    if (popup)
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-failed-parasite-dead"), victim, new EntityUid?(user), PopupType.MediumCaution);
    return false;
  }

  public bool Infect(
    Entity<XenoParasiteComponent> parasite,
    EntityUid victim,
    bool popup = true,
    bool force = false)
  {
    InfectableComponent comp1;
    if (!this.CanInfectPopup(parasite, victim, (EntityUid) parasite, popup, force) || !this.TryComp<InfectableComponent>(victim, out comp1))
      return false;
    if (this._net.IsServer)
    {
      Vector2 worldPosition = this._transform.GetWorldPosition(victim);
      this._transform.SetWorldPosition((EntityUid) parasite, worldPosition);
      ParasiteAIComponent comp2;
      if (this.TryComp<ParasiteAIComponent>((EntityUid) parasite, out comp2))
      {
        --comp2.JumpsLeft;
        if ((double) this._random.NextFloat() < (double) comp2.IdleChance)
          this.GoIdle((Entity<ParasiteAIComponent>) ((EntityUid) parasite, comp2));
      }
      TrapParasiteComponent comp3;
      if (this.TryComp<TrapParasiteComponent>((EntityUid) parasite, out comp3))
        this.ResetTrapState((Entity<TrapParasiteComponent>) (parasite.Owner, comp3));
    }
    if (!this.TryRipOffClothing(victim, SlotFlags.HEAD) || !this.TryRipOffClothing(victim, SlotFlags.MASK, false))
      return false;
    HumanoidAppearanceComponent comp4;
    SoundSpecifier sound;
    if (this._net.IsServer && this.TryComp<HumanoidAppearanceComponent>(victim, out comp4) && comp1.Sound.TryGetValue(comp4.Sex, out sound))
      this._audio.PlayPvs(sound, victim);
    comp1.BeingInfected = true;
    this.Dirty(victim, (IComponent) comp1);
    this._size.TryKnockOut(victim, parasite.Comp.ParalyzeTime);
    this.RefreshIncubationMultipliers((Entity<VictimInfectedComponent>) victim);
    this._inventory.TryEquip(victim, parasite.Owner, "mask", true, true, true);
    this.EnsureComp<UnremoveableComponent>((EntityUid) parasite).DeleteOnDrop = false;
    parasite.Comp.InfectedVictim = new EntityUid?(victim);
    parasite.Comp.FallOffAt = new TimeSpan?(this._timing.CurTime + parasite.Comp.FallOffDelay);
    this.Dirty<XenoParasiteComponent>(parasite);
    this.RemCompDeferred<RMCGibOnDeathComponent>((EntityUid) parasite);
    this.RemCompDeferred<ParasiteAIComponent>((EntityUid) parasite);
    XenoParasiteInfectEvent args = new XenoParasiteInfectEvent(victim, parasite.Owner);
    this.RaiseLocalEvent<XenoParasiteInfectEvent>(victim, args, true);
    this.ParasiteLeapHit(parasite);
    return true;
  }

  public void RefreshIncubationMultipliers(Entity<VictimInfectedComponent?> ent)
  {
    if (!this.Resolve<VictimInfectedComponent>((EntityUid) ent, ref ent.Comp, false))
      return;
    GetInfectedIncubationMultiplierEvent args = new GetInfectedIncubationMultiplierEvent(ent.Comp.CurrentStage);
    this.RaiseLocalEvent<GetInfectedIncubationMultiplierEvent>((EntityUid) ent, ref args);
    float num = 1f;
    foreach (float addition in args.Additions)
      num += addition;
    foreach (float multiplier in args.Multipliers)
      num *= multiplier;
    ent.Comp.IncubationMultiplier = num;
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    if (this._net.IsServer)
    {
      Robust.Shared.GameObjects.EntityQueryEnumerator<ParasiteAIComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<ParasiteAIComponent>();
      EntityUid uid1;
      ParasiteAIComponent comp1_1;
      while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
      {
        if (!this._mobState.IsDead(uid1) && !this.TerminatingOrDeleted(uid1))
          this.UpdateAI((Entity<ParasiteAIComponent>) (uid1, comp1_1), curTime);
      }
      Robust.Shared.GameObjects.EntityQueryEnumerator<TrapParasiteComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<TrapParasiteComponent>();
      EntityUid uid2;
      TrapParasiteComponent comp1_2;
      while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
      {
        TimeSpan? leapAt = comp1_2.LeapAt;
        TimeSpan timeSpan1 = curTime;
        if ((leapAt.HasValue ? (leapAt.GetValueOrDefault() > timeSpan1 ? 1 : 0) : 0) == 0 && !this._mobState.IsDead(uid2) && !this.TerminatingOrDeleted(uid2))
        {
          this._rmcNpc.WakeNPC(uid2);
          TimeSpan? disableAt = comp1_2.DisableAt;
          TimeSpan timeSpan2 = curTime;
          if ((disableAt.HasValue ? (disableAt.GetValueOrDefault() > timeSpan2 ? 1 : 0) : 0) == 0)
            this.RemCompDeferred<TrapParasiteComponent>(uid2);
        }
      }
      Robust.Shared.GameObjects.EntityQueryEnumerator<ParasiteAIDelayAddComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<ParasiteAIDelayAddComponent>();
      EntityUid uid3;
      ParasiteAIDelayAddComponent comp1_3;
      while (entityQueryEnumerator3.MoveNext(out uid3, out comp1_3))
      {
        if (curTime > comp1_3.TimeToAI)
        {
          this.EnsureComp<ParasiteAIComponent>(uid3);
          this.RemCompDeferred<ParasiteAIDelayAddComponent>(uid3);
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoParasiteComponent> entityQueryEnumerator4 = this.EntityQueryEnumerator<XenoParasiteComponent>();
    EntityUid uid4;
    XenoParasiteComponent comp1_4;
    while (entityQueryEnumerator4.MoveNext(out uid4, out comp1_4))
    {
      TimeSpan? fallOffAt = comp1_4.FallOffAt;
      TimeSpan timeSpan = curTime;
      if ((fallOffAt.HasValue ? (fallOffAt.GetValueOrDefault() < timeSpan ? 1 : 0) : 0) != 0 && !comp1_4.FellOff && comp1_4.InfectedVictim.HasValue)
      {
        EntityUid uid5 = comp1_4.InfectedVictim.Value;
        InfectableComponent comp;
        if (this.TryComp<InfectableComponent>(uid5, out comp))
        {
          comp1_4.FellOff = true;
          this.Dirty(uid4, (IComponent) comp1_4);
          this._inventory.TryUnequip(uid5, "mask", true, true, true);
          VictimInfectedComponent infectedComponent = this.EnsureComp<VictimInfectedComponent>(uid5);
          Entity<VictimInfectedComponent> burst = (Entity<VictimInfectedComponent>) (uid5, infectedComponent);
          Entity<HiveComponent>? hive1 = this._hive.GetHive((Entity<HiveMemberComponent>) uid4);
          ref Entity<HiveComponent>? local = ref hive1;
          EntityUid? hive2 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Owner) : new EntityUid?();
          this.SetHive(burst, hive2);
          this.EnsureComp<ParasiteSpentComponent>(uid4);
          comp.BeingInfected = false;
          this.Dirty(uid5, (IComponent) comp);
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<VictimInfectedComponent, TransformComponent> entityQueryEnumerator5 = this.EntityQueryEnumerator<VictimInfectedComponent, TransformComponent>();
    EntityUid uid6;
    VictimInfectedComponent comp1_5;
    while (entityQueryEnumerator5.MoveNext(out uid6, out comp1_5, out TransformComponent _))
    {
      if (!this._net.IsClient)
      {
        if (comp1_5.BurstAt + comp1_5.AutoBurstTime <= curTime && comp1_5.SpawnedLarva.HasValue)
          this.TryBurst((Entity<VictimInfectedComponent>) (uid6, comp1_5));
        else if (this._mobState.IsDead(uid6) && (this.HasComp<InfectStopOnDeathComponent>(uid6) || this._rotting.IsRotten(uid6) || this._unrevivable.IsUnrevivable(uid6) && this._unrevivable.DoesKillLarvaOnUnrevivable((Entity<RMCRevivableComponent>) uid6)))
        {
          if (comp1_5.SpawnedLarva.HasValue)
            this.TryBurst((Entity<VictimInfectedComponent>) (uid6, comp1_5));
          else
            this.RemCompDeferred<VictimInfectedComponent>(uid6);
        }
        else
        {
          if ((double) comp1_5.IncubationMultiplier != 1.0)
            comp1_5.BurstAt += TimeSpan.FromSeconds(1.0 - (double) comp1_5.IncubationMultiplier) * (double) frameTime;
          if (comp1_5.BurstAt <= curTime && !comp1_5.SpawnedLarva.HasValue)
            this.SpawnLarva((Entity<VictimInfectedComponent>) (uid6, comp1_5), out EntityUid _);
          int num = Math.Max((int) ((comp1_5.BurstDelay - (comp1_5.BurstAt - curTime)) / comp1_5.BurstDelay * (double) comp1_5.FinalStage), comp1_5.CurrentStage);
          if (num != comp1_5.CurrentStage)
          {
            comp1_5.CurrentStage = num;
            this.Dirty(uid6, (IComponent) comp1_5);
            this.RefreshIncubationMultipliers((Entity<VictimInfectedComponent>) uid6);
          }
          if (!comp1_5.DidBurstWarning && num == comp1_5.BurstWarningStart)
          {
            this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-infection-burst-soon-self"), uid6, uid6, PopupType.MediumCaution);
            TimeSpan knockdownTime = comp1_5.BaseKnockdownTime * 75.0;
            this.InfectionShakes(uid6, comp1_5, knockdownTime, comp1_5.JitterTime, false);
            comp1_5.DidBurstWarning = true;
          }
          else if (num >= comp1_5.BurstWarningStart)
          {
            if (this._random.Prob(comp1_5.InsanePainChance * frameTime))
            {
              this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-infection-insanepain-" + RandomExtensions.Pick<string>(this._random, (IReadOnlyList<string>) new List<string>()
              {
                "one",
                "two",
                "three",
                "four",
                "five"
              })), uid6, uid6, PopupType.LargeCaution);
              TimeSpan knockdownTime = comp1_5.BaseKnockdownTime * 2.0;
              TimeSpan jitterTime = comp1_5.JitterTime * 0.0;
              this.InfectionShakes(uid6, comp1_5, knockdownTime, jitterTime, false);
            }
          }
          else if (num >= comp1_5.FinalSymptomsStart)
          {
            if (this._random.Prob(comp1_5.MajorPainChance * frameTime))
            {
              this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-infection-majorpain-" + RandomExtensions.Pick<string>(this._random, (IReadOnlyList<string>) new List<string>()
              {
                "chest",
                "breathing",
                "heart"
              })), uid6, uid6, PopupType.SmallCaution);
              if (this._random.Prob(0.5f))
              {
                VictimInfectedEmoteEvent args = new VictimInfectedEmoteEvent(comp1_5.ScreamId);
                this.RaiseLocalEvent<VictimInfectedEmoteEvent>(uid6, ref args);
              }
            }
            if (this._random.Prob(comp1_5.ShakesChance * frameTime))
              this.InfectionShakes(uid6, comp1_5, comp1_5.BaseKnockdownTime * 4.0, comp1_5.JitterTime * 4.0);
          }
          else if (num >= comp1_5.MiddlingSymptomsStart)
          {
            if (this._random.Prob(comp1_5.ThroatPainChance * frameTime))
              this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-infection-throat-" + RandomExtensions.Pick<string>(this._random, (IReadOnlyList<string>) new List<string>()
              {
                "sore",
                "mucous"
              })), uid6, uid6, PopupType.SmallCaution);
            else if (this._random.Prob(comp1_5.MuscleAcheChance * frameTime))
            {
              this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-infection-muscle-ache"), uid6, uid6, PopupType.SmallCaution);
              if (this._random.Prob(0.2f))
                this._damage.TryChangeDamage(new EntityUid?(uid6), comp1_5.InfectionDamage, true, false);
            }
            else if (this._random.Prob(comp1_5.SneezeCoughChance * frameTime))
            {
              VictimInfectedEmoteEvent args = new VictimInfectedEmoteEvent(RandomExtensions.Pick<ProtoId<EmotePrototype>>(this._random, (IReadOnlyList<ProtoId<EmotePrototype>>) new List<ProtoId<EmotePrototype>>()
              {
                comp1_5.SneezeId,
                comp1_5.CoughId
              }));
              this.RaiseLocalEvent<VictimInfectedEmoteEvent>(uid6, ref args);
            }
            if (this._random.Prob((float) ((double) comp1_5.ShakesChance * 5.0 / 6.0) * frameTime))
              this.InfectionShakes(uid6, comp1_5, comp1_5.BaseKnockdownTime * 2.0, comp1_5.JitterTime * 2.0);
          }
          else if (num >= comp1_5.InitialSymptomsStart)
          {
            if (this._random.Prob(comp1_5.MinorPainChance * frameTime))
              this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-infection-minorpain-" + RandomExtensions.Pick<string>(this._random, (IReadOnlyList<string>) new List<string>()
              {
                "stomach",
                "chest"
              })), uid6, uid6, PopupType.SmallCaution);
            if (this._random.Prob((float) ((double) comp1_5.ShakesChance * 2.0 / 3.0) * frameTime))
              this.InfectionShakes(uid6, comp1_5, comp1_5.BaseKnockdownTime, comp1_5.JitterTime);
          }
        }
      }
    }
  }

  private void InfectionShakes(
    EntityUid victim,
    VictimInfectedComponent infected,
    TimeSpan knockdownTime,
    TimeSpan jitterTime,
    bool popups = true)
  {
    if (this._mobState.IsIncapacitated(victim))
      return;
    this._size.TryKnockOut(victim, knockdownTime);
    this._jitter.DoJitter(victim, jitterTime, false);
    this._damage.TryChangeDamage(new EntityUid?(victim), infected.InfectionDamage, true, false);
    if (!popups)
      return;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-infection-shakes-self"), victim, victim, PopupType.MediumCaution);
    BaseContainer container;
    if (this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) victim, out container) && this.HasComp<RMCHideParasiteInfectionContainerPopupComponent>(container.Owner))
      return;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-infection-shakes", (nameof (victim), (object) victim)), victim, Filter.PvsExcept(victim), true, PopupType.MediumCaution);
  }

  private void OnTryMove(Entity<BursterComponent> burster, ref MoveInputEvent args)
  {
    VictimInfectedComponent comp;
    if (!args.HasDirectionalMovement || !this.TryComp<VictimInfectedComponent>(burster.Comp.BurstFrom, out comp) || comp.IsBursting)
      return;
    this.TryBurst((Entity<VictimInfectedComponent>) (burster.Comp.BurstFrom, comp));
  }

  private void TryBurst(Entity<VictimInfectedComponent> burstFrom)
  {
    EntityUid owner = burstFrom.Owner;
    VictimInfectedComponent comp = burstFrom.Comp;
    if (!comp.SpawnedLarva.HasValue || comp.IsBursting)
      return;
    comp.IsBursting = true;
    this.Dirty(owner, (IComponent) comp);
    EntityUid entityUid = comp.SpawnedLarva.Value;
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, entityUid, comp.BurstDoAfterDelay, (DoAfterEvent) new LarvaBurstDoAfterEvent(), new EntityUid?(owner), new EntityUid?(owner))
    {
      NeedHand = false,
      BreakOnDamage = false,
      BreakOnMove = false,
      BreakOnRest = false,
      Hidden = true,
      CancelDuplicate = true,
      BlockDuplicate = true,
      DuplicateCondition = DuplicateConditions.SameEvent
    }))
      return;
    this.EnsureComp<VictimBurstComponent>((EntityUid) burstFrom);
    this._appearance.SetData(burstFrom.Owner, (Enum) BurstVisuals.Visuals, (object) VictimBurstState.Bursting);
    Filter filter = Filter.PvsExcept(owner);
    filter.RemoveWhereAttachedEntity(new Predicate<EntityUid>(((EntitySystem) this).HasComp<BursterComponent>));
    if (this._net.IsServer)
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-infection-burst-now-victim"), owner, owner, PopupType.MediumCaution);
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-infection-burst-soon", ("victim", (object) owner)), owner, filter, true, PopupType.LargeCaution);
      this._jitter.DoJitter(owner, comp.JitterTime / 1.2, true, 14f, 5f, true);
    }
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-infection-burst-now-xeno", ("victim", (object) Identity.Entity(owner, (IEntityManager) this.EntityManager))), entityUid, new EntityUid?(entityUid), PopupType.MediumCaution);
  }

  private void OnBurst(Entity<VictimInfectedComponent> ent, ref LarvaBurstDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled || this._net.IsClient)
      return;
    this.EnsureComp<VictimBurstComponent>(ent.Owner);
    this._appearance.SetData(ent.Owner, (Enum) BurstVisuals.Visuals, (object) VictimBurstState.Burst);
    MobStateComponent comp;
    if (this.TryComp<MobStateComponent>(ent.Owner, out comp))
      this._mobState.UpdateMobState(ent.Owner, comp);
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) ent);
    BaseContainer container;
    if (this._container.TryGetContainer((EntityUid) ent, ent.Comp.LarvaContainerId, out container))
    {
      foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
      {
        this.RemCompDeferred<BursterComponent>(containedEntity);
        RMCTemporaryInvincibilityComponent invincibilityComponent = this.EnsureComp<RMCTemporaryInvincibilityComponent>(containedEntity);
        invincibilityComponent.ExpiresAt = this._timing.CurTime + ent.Comp.LarvaInvincibilityTime;
        this.Dirty(containedEntity, (IComponent) invincibilityComponent);
      }
      this._container.EmptyContainer(container, destination: new EntityCoordinates?(moverCoordinates));
    }
    this.Dirty<VictimInfectedComponent>(ent);
    this.RemCompDeferred<VictimInfectedComponent>((EntityUid) ent);
    this._audio.PlayPvs(ent.Comp.BurstSound, args.User);
  }

  private bool TryRipOffClothing(EntityUid victim, SlotFlags slotFlags, bool doPopup = true)
  {
    InventorySystem.InventorySlotEnumerator containerSlotEnumerator;
    if (!this._inventory.TryGetContainerSlotEnumerator((Entity<InventoryComponent>) victim, out containerSlotEnumerator))
      return true;
    EntityUid? nullable = new EntityUid?();
    EntityUid entityUid;
    SlotDefinition slot;
    while (containerSlotEnumerator.NextItem(out entityUid, out slot))
    {
      if ((slot.SlotFlags & slotFlags) != SlotFlags.NONE || this._tagSystem.HasTag(entityUid, (ProtoId<TagPrototype>) "RipOffOnInfection"))
      {
        ParasiteResistanceComponent comp;
        this.TryComp<ParasiteResistanceComponent>(entityUid, out comp);
        if (comp != null && (double) comp.Count < (double) comp.MaxCount)
        {
          ++comp.Count;
          this.Dirty(entityUid, (IComponent) comp);
          if (this._net.IsServer & doPopup)
            this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-infect-fail", ("target", (object) victim), ("clothing", (object) entityUid)), victim, PopupType.SmallCaution);
          return false;
        }
        this._inventory.TryUnequip(victim, victim, slot.Name, force: true);
        nullable = new EntityUid?(entityUid);
      }
    }
    if (this._net.IsServer & doPopup && nullable.HasValue)
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-infect-success", ("target", (object) victim), ("clothing", (object) nullable)), victim, PopupType.MediumCaution);
    return true;
  }

  public void SetBurstSpawn(Entity<VictimInfectedComponent> burst, EntProtoId spawn)
  {
    burst.Comp.BurstSpawn = spawn;
    this.Dirty<VictimInfectedComponent>(burst);
  }

  public void SetBurstSound(Entity<VictimInfectedComponent> burst, SoundSpecifier sound)
  {
    burst.Comp.BurstSound = sound;
    this.Dirty<VictimInfectedComponent>(burst);
  }

  public void TryStartBurst(Entity<VictimInfectedComponent> burst)
  {
    this.SetBurstDelay(burst, TimeSpan.Zero);
    this.TryBurst(burst);
  }

  public void SetBurstDelay(Entity<VictimInfectedComponent> burst, TimeSpan time)
  {
    burst.Comp.BurstAt = this._timing.CurTime + time;
    this.Dirty<VictimInfectedComponent>(burst);
  }

  public void SetHive(Entity<VictimInfectedComponent> burst, EntityUid? hive)
  {
    burst.Comp.Hive = hive;
    this.Dirty<VictimInfectedComponent>(burst);
  }

  public void SpawnLarva(Entity<VictimInfectedComponent> victim, out EntityUid spawned)
  {
    ContainerSlot containerSlot = this._container.EnsureContainer<ContainerSlot>(victim.Owner, victim.Comp.LarvaContainerId);
    spawned = this.SpawnInContainerOrDrop((string) victim.Comp.BurstSpawn, victim.Owner, containerSlot.ID);
    this.LinkLarvaToVictim(victim, spawned);
  }

  public void InsertLarva(Entity<VictimInfectedComponent> victim, EntityUid spawned)
  {
    ContainerSlot container = this._container.EnsureContainer<ContainerSlot>(victim.Owner, victim.Comp.LarvaContainerId);
    this._container.InsertOrDrop((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) spawned, (BaseContainer) container);
    this.LinkLarvaToVictim(victim, spawned);
  }

  private void LinkLarvaToVictim(Entity<VictimInfectedComponent> victim, EntityUid spawned)
  {
    if (this.HasComp<XenoComponent>(spawned))
      this._hive.SetHive((Entity<HiveMemberComponent>) spawned, victim.Comp.Hive);
    victim.Comp.CurrentStage = 6;
    victim.Comp.SpawnedLarva = new EntityUid?(spawned);
    this.Dirty<VictimInfectedComponent>(victim);
    BursterComponent comp;
    this.EnsureComp<BursterComponent>(spawned, out comp);
    comp.BurstFrom = victim.Owner;
    this.Dirty(spawned, (IComponent) comp);
  }
}
