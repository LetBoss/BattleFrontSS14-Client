// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vents.SharedVentCrawlingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Storage.Containers;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Atmos;
using Content.Shared.CCVar;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Tools.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Vents;

public abstract class SharedVentCrawlingSystem : EntitySystem
{
  [Dependency]
  private SharedDoAfterSystem _doafter;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private RMCMapSystem _rmcmap;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedJitteringSystem _jitter;
  [Dependency]
  private SharedMoverController _mover;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedEyeSystem _eye;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private MobStateSystem _mob;
  private bool _relativeMovement;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<VentEntranceComponent, ExaminedEvent>(new EntityEventRefHandler<VentEntranceComponent, ExaminedEvent>(this.OnVentEntranceExamine));
    this.SubscribeLocalEvent<VentEntranceComponent, InteractHandEvent>(new EntityEventRefHandler<VentEntranceComponent, InteractHandEvent>(this.OnVentEntranceInteract));
    this.SubscribeLocalEvent<VentEntranceComponent, VentEnterDoafterEvent>(new EntityEventRefHandler<VentEntranceComponent, VentEnterDoafterEvent>(this.OnVentEnterDoafter));
    this.SubscribeLocalEvent<VentExitComponent, VentExitDoafterEvent>(new EntityEventRefHandler<VentExitComponent, VentExitDoafterEvent>(this.OnVentExitDoafter));
    this.SubscribeLocalEvent<VentCrawlableComponent, MapInitEvent>(new EntityEventRefHandler<VentCrawlableComponent, MapInitEvent>(this.OnVentDuctInit));
    this.SubscribeLocalEvent<VentCrawlableComponent, MoveEvent>(new EntityEventRefHandler<VentCrawlableComponent, MoveEvent>(this.OnVentDuctMove));
    this.SubscribeLocalEvent<VentCrawlableComponent, AnchorStateChangedEvent>(new EntityEventRefHandler<VentCrawlableComponent, AnchorStateChangedEvent>(this.OnVentAnchorChanged));
    this.SubscribeLocalEvent<VentCrawlableComponent, RMCContainerDestructionEmptyEvent>(new EntityEventRefHandler<VentCrawlableComponent, RMCContainerDestructionEmptyEvent>(this.OnVentContainerDeletionEmpty));
    this.SubscribeLocalEvent<VentCrawlingComponent, MoveInputEvent>(new EntityEventRefHandler<VentCrawlingComponent, MoveInputEvent>(this.OnVentCrawlingInput));
    this.SubscribeLocalEvent<VentCrawlingComponent, ComponentInit>(new EntityEventRefHandler<VentCrawlingComponent, ComponentInit>(this.OnVentCrawlingStart));
    this.SubscribeLocalEvent<VentCrawlingComponent, ComponentRemove>(new EntityEventRefHandler<VentCrawlingComponent, ComponentRemove>(this.OnVentCrawlingEnd));
    this.SubscribeLocalEvent<VentCrawlingComponent, DropAttemptEvent>(new EntityEventRefHandler<VentCrawlingComponent, DropAttemptEvent>(this.OnVentCrawlingCancel<DropAttemptEvent>));
    this.SubscribeLocalEvent<VentCrawlingComponent, PickupAttemptEvent>(new EntityEventRefHandler<VentCrawlingComponent, PickupAttemptEvent>(this.OnVentCrawlingCancel<PickupAttemptEvent>));
    this.SubscribeLocalEvent<VentCrawlingComponent, UseAttemptEvent>(new EntityEventRefHandler<VentCrawlingComponent, UseAttemptEvent>(this.OnVentCrawlingCancel<UseAttemptEvent>));
    this.SubscribeLocalEvent<RMCTrayCrawlerComponent, GetVisMaskEvent>(new EntityEventRefHandler<RMCTrayCrawlerComponent, GetVisMaskEvent>(this.OnTrayGetVis));
    this.Subs.CVar<bool>(this._config, CCVars.RelativeMovement, (Action<bool>) (v => this._relativeMovement = v), true);
  }

  private void OnVentEntranceExamine(Entity<VentEntranceComponent> vent, ref ExaminedEvent args)
  {
    VentCrawlerComponent comp1;
    WeldableComponent comp2;
    if (!this.TryComp<VentCrawlerComponent>(args.Examiner, out comp1) || this.TryComp<WeldableComponent>((EntityUid) vent, out comp2) && comp2.IsWelded)
      return;
    args.PushMarkup(this.Loc.GetString((string) comp1.VentCrawlExamine));
  }

  private void OnTrayGetVis(Entity<RMCTrayCrawlerComponent> ent, ref GetVisMaskEvent args)
  {
    if (!ent.Comp.Enabled)
      return;
    args.VisibilityMask |= 4;
  }

  private void OnVentDuctInit(Entity<VentCrawlableComponent> vent, ref MapInitEvent args)
  {
    if (vent.Comp.TravelDirection == PipeDirection.Fourway)
      return;
    vent.Comp.TravelDirection = vent.Comp.TravelDirection.RotatePipeDirection(Angle.op_Implicit(this.Transform((EntityUid) vent).LocalRotation));
    this.Dirty<VentCrawlableComponent>(vent);
  }

  private void OnVentDuctMove(Entity<VentCrawlableComponent> vent, ref MoveEvent args)
  {
    if (vent.Comp.TravelDirection == PipeDirection.Fourway)
      return;
    vent.Comp.TravelDirection = vent.Comp.TravelDirection.RotatePipeDirection(Angle.op_Implicit(args.NewRotation));
    this.Dirty<VentCrawlableComponent>(vent);
  }

  private void OnVentAnchorChanged(
    Entity<VentCrawlableComponent> vent,
    ref AnchorStateChangedEvent args)
  {
    this.EmptyVent((EntityUid) vent);
  }

  private void OnVentContainerDeletionEmpty(
    Entity<VentCrawlableComponent> vent,
    ref RMCContainerDestructionEmptyEvent args)
  {
    this.EmptyVent((EntityUid) vent);
  }

  private void EmptyVent(EntityUid vent)
  {
    VentCrawlableComponent comp;
    if (!this.TryComp<VentCrawlableComponent>(vent, out comp))
      return;
    foreach (EntityUid ent in this._container.EmptyContainer((BaseContainer) this._container.EnsureContainer<Container>(vent, comp.ContainerId), true))
      this.RemoveVentCrawling(ent);
  }

  private bool TryGetVent(
    EntityUid vent,
    [NotNullWhen(true)] out VentCrawlableComponent? ventComp,
    [NotNullWhen(true)] out Container? container)
  {
    ventComp = (VentCrawlableComponent) null;
    container = (Container) null;
    if (!this.TryComp<VentCrawlableComponent>(vent, out ventComp) || !this.Transform(vent).Anchored)
      return false;
    container = this._container.EnsureContainer<Container>(vent, ventComp.ContainerId);
    return true;
  }

  private void OnVentEntranceInteract(
    Entity<VentEntranceComponent> vent,
    ref InteractHandEvent args)
  {
    if (args.Handled)
      return;
    WeldableComponent comp1;
    if (this.TryComp<WeldableComponent>((EntityUid) vent, out comp1) && comp1.IsWelded)
    {
      this._popup.PopupPredicted(this.Loc.GetString("rmc-vent-crawling-welded"), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
    }
    else
    {
      VentCrawlerComponent comp2;
      VentCrawlableComponent ventComp;
      Container container;
      if (!this.TryComp<VentCrawlerComponent>(args.User, out comp2) || !this.TryGetVent((EntityUid) vent, out ventComp, out container))
        return;
      if (ventComp.MaxEntities.HasValue)
      {
        int count = container.ContainedEntities.Count;
        int? maxEntities = ventComp.MaxEntities;
        int valueOrDefault = maxEntities.GetValueOrDefault();
        if (count > valueOrDefault & maxEntities.HasValue)
        {
          this._popup.PopupPredicted(this.Loc.GetString("rmc-vent-crawling-full"), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
          return;
        }
      }
      if (this._container.IsEntityInContainer(args.User))
        return;
      VentEnterAttemptEvent args1 = new VentEnterAttemptEvent();
      this.RaiseLocalEvent<VentEnterAttemptEvent>(args.User, args1);
      if (args1.Cancelled)
        return;
      args.Handled = true;
      VentEnterDoafterEvent @event = new VentEnterDoafterEvent();
      this._doafter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, comp2.VentEnterDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) vent), new EntityUid?((EntityUid) vent))
      {
        BreakOnMove = true,
        DuplicateCondition = DuplicateConditions.SameEvent
      });
      this._jitter.AddJitter((EntityUid) vent, 5f, 8f);
    }
  }

  private void OnVentEnterDoafter(
    Entity<VentEntranceComponent> vent,
    ref VentEnterDoafterEvent args)
  {
    this.RemCompDeferred<JitteringComponent>((EntityUid) vent);
    if (args.Handled || args.Cancelled)
      return;
    WeldableComponent comp1;
    if (this.TryComp<WeldableComponent>((EntityUid) vent, out comp1) && comp1.IsWelded)
    {
      this._popup.PopupPredicted(this.Loc.GetString("rmc-vent-crawling-welded"), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
    }
    else
    {
      VentCrawlableComponent ventComp;
      Container container;
      if (!this.TryGetVent((EntityUid) vent, out ventComp, out container))
        return;
      if (ventComp.MaxEntities.HasValue)
      {
        int count = container.ContainedEntities.Count;
        int? maxEntities = ventComp.MaxEntities;
        int valueOrDefault = maxEntities.GetValueOrDefault();
        if (count > valueOrDefault & maxEntities.HasValue)
        {
          this._popup.PopupPredicted(this.Loc.GetString("rmc-vent-crawling-full"), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
          return;
        }
      }
      VentEnterAttemptEvent args1 = new VentEnterAttemptEvent();
      this.RaiseLocalEvent<VentEnterAttemptEvent>(args.User, args1);
      if (args1.Cancelled)
        return;
      args.Handled = true;
      this._audio.PlayPredicted(vent.Comp.EnterSound, (EntityUid) vent, new EntityUid?(args.User));
      this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) args.User, (BaseContainer) container);
      this.EnsureComp<VentCrawlingComponent>(args.User);
      RMCTrayCrawlerComponent comp2;
      if (!this.TryComp<RMCTrayCrawlerComponent>(args.User, out comp2))
        return;
      comp2.Enabled = true;
      this.Dirty(args.User, (IComponent) comp2);
      this._eye.RefreshVisibilityMask((Entity<EyeComponent>) args.User);
      this.EnsureComp<VentSightComponent>(args.User);
    }
  }

  private void OnVentExitDoafter(Entity<VentExitComponent> vent, ref VentExitDoafterEvent args)
  {
    this.RemCompDeferred<JitteringComponent>((EntityUid) vent);
    Container container;
    if (args.Handled || args.Cancelled || !this.TryGetVent((EntityUid) vent, out VentCrawlableComponent _, out container) || this._rmcmap.IsTileBlocked(vent.Owner.ToCoordinates()))
      return;
    args.Handled = true;
    this._container.Remove((Entity<TransformComponent, MetaDataComponent>) args.User, (BaseContainer) container);
    this._audio.PlayPredicted(vent.Comp.ExitSound, (EntityUid) vent, new EntityUid?(args.User));
    this.RemoveVentCrawling(args.User);
    this._transform.AttachToGridOrMap(args.User);
  }

  private void RemoveVentCrawling(EntityUid ent)
  {
    this.RemCompDeferred<VentCrawlingComponent>(ent);
    RMCTrayCrawlerComponent comp;
    if (!this.TryComp<RMCTrayCrawlerComponent>(ent, out comp))
      return;
    comp.Enabled = false;
    this.Dirty(ent, (IComponent) comp);
    this._eye.RefreshVisibilityMask((Entity<EyeComponent>) ent);
    this.RemComp<VentSightComponent>(ent);
  }

  private void OnVentCrawlingInput(Entity<VentCrawlingComponent> ent, ref MoveInputEvent args)
  {
    InputMoverComponent comp;
    if (!this.TryComp<InputMoverComponent>((EntityUid) ent, out comp))
      return;
    Vector2 vector2_1 = this._mover.DirVecForButtons(SharedMoverController.GetNormalizedMovement(comp.HeldMoveButtons));
    if (vector2_1 == Vector2.Zero)
    {
      ent.Comp.TravelDirection = new Direction?();
    }
    else
    {
      Angle parentGridAngle = this._mover.GetParentGridAngle(comp);
      Vector2 vector2_2 = this._relativeMovement ? ((Angle) ref parentGridAngle).RotateVec(ref vector2_1) : vector2_1;
      ent.Comp.TravelDirection = DirectionExtensions.GetDir(vector2_2).IsCardinal() ? new Direction?(DirectionExtensions.GetDir(vector2_2)) : new Direction?();
      this.Dirty<VentCrawlingComponent>(ent);
    }
  }

  public bool AreVentsConnectedInDirection(
    Entity<VentCrawlableComponent> sourceVent,
    Entity<VentCrawlableComponent> destinationVent,
    PipeDirection direction)
  {
    return !(sourceVent.Comp.LayerId != destinationVent.Comp.LayerId) && sourceVent.Comp.TravelDirection.HasDirection(direction) && destinationVent.Comp.TravelDirection.HasDirection(direction.GetOpposite());
  }

  private void OnVentCrawlingStart(Entity<VentCrawlingComponent> ent, ref ComponentInit args)
  {
    foreach (Entity<ActionComponent> action in this._actions.GetActions((EntityUid) ent))
      this._actions.SetEnabled(new Entity<ActionComponent>?(action.AsNullable()), false);
  }

  private void OnVentCrawlingEnd(Entity<VentCrawlingComponent> ent, ref ComponentRemove args)
  {
    foreach (Entity<ActionComponent> action in this._actions.GetActions((EntityUid) ent))
      this._actions.SetEnabled(new Entity<ActionComponent>?(action.AsNullable()), true);
  }

  private void OnVentCrawlingCancel<T>(Entity<VentCrawlingComponent> ent, ref T args) where T : CancellableEntityEventArgs
  {
    args.Cancel();
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<VentCrawlingComponent, VentCrawlerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<VentCrawlingComponent, VentCrawlerComponent>();
    EntityUid uid1;
    VentCrawlingComponent comp1;
    VentCrawlerComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid1, out comp1, out comp2))
    {
      BaseContainer container1;
      VentCrawlableComponent comp3;
      if (!(curTime < comp1.NextVentMoveTime) && comp1.TravelDirection.HasValue && this._mob.IsAlive(uid1) && this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) uid1, out container1) && this.TryComp<VentCrawlableComponent>(container1.Owner, out comp3))
      {
        RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcmap.GetAnchoredEntitiesEnumerator(container1.Owner, new Direction?(comp1.TravelDirection.Value), (DirectionFlag) 0);
        bool flag = false;
        EntityUid uid2;
        while (entitiesEnumerator.MoveNext(out uid2))
        {
          VentCrawlableComponent ventComp;
          Container container2;
          if (this.TryGetVent(uid2, out ventComp, out container2) && this.AreVentsConnectedInDirection((Entity<VentCrawlableComponent>) (container1.Owner, comp3), (Entity<VentCrawlableComponent>) (uid2, ventComp), comp1.TravelDirection.Value.ToPipeDirection()))
          {
            if (ventComp.MaxEntities.HasValue)
            {
              int count = container2.ContainedEntities.Count;
              int? maxEntities = ventComp.MaxEntities;
              int valueOrDefault = maxEntities.GetValueOrDefault();
              if (count > valueOrDefault & maxEntities.HasValue)
              {
                this._popup.PopupPredicted(this.Loc.GetString("rmc-vent-crawling-full"), uid1, new EntityUid?(uid1), PopupType.SmallCaution);
                continue;
              }
            }
            this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) uid1, (BaseContainer) container2);
            comp1.NextVentMoveTime = curTime + comp2.VentCrawlDelay;
            flag = true;
            if (curTime >= comp1.NextVentCrawlSound)
            {
              this._audio.PlayPredicted(ventComp.TravelSound, uid2, new EntityUid?(uid1));
              comp1.NextVentCrawlSound = curTime + comp2.VentCrawlSoundDelay;
              this._popup.PopupPredictedCoordinates(this.Loc.GetString("rmc-vent-crawling-moving"), this._transform.GetMoverCoordinates(uid1), new EntityUid?(uid1), PopupType.SmallCaution);
            }
            this.Dirty(uid1, (IComponent) comp1);
            break;
          }
        }
        if (!flag && this.HasComp<VentExitComponent>(container1.Owner) && comp3.TravelDirection.HasDirection(comp1.TravelDirection.Value.ToPipeDirection()))
        {
          WeldableComponent comp4;
          if (this.TryComp<WeldableComponent>(container1.Owner, out comp4) && comp4.IsWelded)
            this._popup.PopupPredicted(this.Loc.GetString("rmc-vent-crawling-welded"), uid1, new EntityUid?(uid1), PopupType.SmallCaution);
          else if (!this._rmcmap.IsTileBlocked(container1.Owner.ToCoordinates()))
          {
            VentExitDoafterEvent @event = new VentExitDoafterEvent();
            this._doafter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, uid1, comp2.VentExitDelay, (DoAfterEvent) @event, new EntityUid?(container1.Owner), new EntityUid?(container1.Owner))
            {
              BreakOnMove = true,
              DuplicateCondition = DuplicateConditions.SameEvent,
              CancelDuplicate = false,
              BlockDuplicate = true,
              RequireCanInteract = false
            });
            this._jitter.AddJitter(container1.Owner, 5f, 8f);
          }
        }
      }
    }
  }
}
