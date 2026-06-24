// Decompiled with JetBrains decompiler
// Type: Content.Shared.Bed.Cryostorage.SharedCryostorageSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.DragDrop;
using Content.Shared.GameTicking;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Bed.Cryostorage;

public abstract class SharedCryostorageSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _configuration;
  [Dependency]
  private ISharedPlayerManager _player;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  protected IGameTiming Timing;
  [Dependency]
  protected ISharedAdminLogManager AdminLog;
  [Dependency]
  protected SharedMindSystem Mind;
  protected bool CryoSleepRejoiningEnabled;

  protected EntityUid? PausedMap { get; private set; }

  public virtual void Initialize()
  {
    SharedCryostorageSystem cryostorageSystem = this;
    // ISSUE: virtual method pointer
    this.SubscribeLocalEvent<CryostorageComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<CryostorageComponent, EntInsertedIntoContainerMessage>((object) cryostorageSystem, __vmethodptr(cryostorageSystem, OnInsertedContainer)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CryostorageComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<CryostorageComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnRemovedContainer)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CryostorageComponent, ContainerIsInsertingAttemptEvent>(new EntityEventRefHandler<CryostorageComponent, ContainerIsInsertingAttemptEvent>((object) this, __methodptr(OnInsertAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CryostorageComponent, ComponentShutdown>(new EntityEventRefHandler<CryostorageComponent, ComponentShutdown>((object) this, __methodptr(OnShutdownContainer)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CryostorageComponent, CanDropTargetEvent>(new EntityEventRefHandler<CryostorageComponent, CanDropTargetEvent>((object) this, __methodptr(OnCanDropTarget)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CryostorageContainedComponent, EntGotRemovedFromContainerMessage>(new EntityEventRefHandler<CryostorageContainedComponent, EntGotRemovedFromContainerMessage>((object) this, __methodptr(OnRemovedContained)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CryostorageContainedComponent, ComponentShutdown>(new EntityEventRefHandler<CryostorageContainedComponent, ComponentShutdown>((object) this, __methodptr(OnShutdownContained)), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart), (Type[]) null, (Type[]) null);
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._configuration, CCVars.GameCryoSleepRejoining, new Action<bool>(this.OnCvarChanged), true);
  }

  private void OnCvarChanged(bool value) => this.CryoSleepRejoiningEnabled = value;

  protected virtual void OnInsertedContainer(
    Entity<CryostorageComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    EntityUid mindId;
    CryostorageComponent cryostorageComponent1;
    ent.Deconstruct(ref mindId, ref cryostorageComponent1);
    CryostorageComponent cryostorageComponent2 = cryostorageComponent1;
    if (((ContainerModifiedMessage) args).Container.ID != cryostorageComponent2.ContainerId)
      return;
    this._appearance.SetData(Entity<CryostorageComponent>.op_Implicit(ent), (Enum) CryostorageVisuals.Full, (object) true, (AppearanceComponent) null);
    if (!this.Timing.IsFirstTimePredicted)
      return;
    CryostorageContainedComponent containedComponent = this.EnsureComp<CryostorageContainedComponent>(((ContainerModifiedMessage) args).Entity);
    TimeSpan timeSpan = this.Mind.TryGetMind(((ContainerModifiedMessage) args).Entity, out mindId, out MindComponent _) ? cryostorageComponent2.GracePeriod : cryostorageComponent2.NoMindGracePeriod;
    containedComponent.GracePeriodEndTime = new TimeSpan?(this.Timing.CurTime + timeSpan);
    containedComponent.Cryostorage = new EntityUid?(Entity<CryostorageComponent>.op_Implicit(ent));
    this.Dirty(((ContainerModifiedMessage) args).Entity, (IComponent) containedComponent, (MetaDataComponent) null);
  }

  private void OnRemovedContainer(
    Entity<CryostorageComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    EntityUid entityUid;
    CryostorageComponent cryostorageComponent1;
    ent.Deconstruct(ref entityUid, ref cryostorageComponent1);
    CryostorageComponent cryostorageComponent2 = cryostorageComponent1;
    if (((ContainerModifiedMessage) args).Container.ID != cryostorageComponent2.ContainerId)
      return;
    this._appearance.SetData(Entity<CryostorageComponent>.op_Implicit(ent), (Enum) CryostorageVisuals.Full, (object) (((ContainerModifiedMessage) args).Container.ContainedEntities.Count > 0), (AppearanceComponent) null);
  }

  private void OnInsertAttempt(
    Entity<CryostorageComponent> ent,
    ref ContainerIsInsertingAttemptEvent args)
  {
    EntityUid mindId;
    CryostorageComponent cryostorageComponent1;
    ent.Deconstruct(ref mindId, ref cryostorageComponent1);
    CryostorageComponent cryostorageComponent2 = cryostorageComponent1;
    if (((ContainerAttemptEventBase) args).Container.ID != cryostorageComponent2.ContainerId)
      return;
    if (this._mobState.IsIncapacitated(((ContainerAttemptEventBase) args).EntityUid))
    {
      ((CancellableEntityEventArgs) args).Cancel();
    }
    else
    {
      MindContainerComponent container;
      if (!this.HasComp<CanEnterCryostorageComponent>(((ContainerAttemptEventBase) args).EntityUid) || !this.TryComp<MindContainerComponent>(((ContainerAttemptEventBase) args).EntityUid, ref container))
      {
        ((CancellableEntityEventArgs) args).Cancel();
      }
      else
      {
        MindComponent mind;
        if (!this.Mind.TryGetMind(((ContainerAttemptEventBase) args).EntityUid, out mindId, out mind, container) || !mind.PreventSuicide && !mind.PreventGhosting)
          return;
        ((CancellableEntityEventArgs) args).Cancel();
      }
    }
  }

  private void OnShutdownContainer(Entity<CryostorageComponent> ent, ref ComponentShutdown args)
  {
    CryostorageComponent comp = ent.Comp;
    foreach (EntityUid storedPlayer in comp.StoredPlayers)
    {
      CryostorageContainedComponent containedComponent;
      if (this.TryComp<CryostorageContainedComponent>(storedPlayer, ref containedComponent))
      {
        containedComponent.Cryostorage = new EntityUid?();
        this.Dirty(storedPlayer, (IComponent) containedComponent, (MetaDataComponent) null);
      }
    }
    comp.StoredPlayers.Clear();
    this.Dirty(Entity<CryostorageComponent>.op_Implicit(ent), (IComponent) comp, (MetaDataComponent) null);
  }

  private void OnCanDropTarget(Entity<CryostorageComponent> ent, ref CanDropTargetEvent args)
  {
    ICommonSession icommonSession;
    if (EntityUid.op_Equality(args.Dragged, args.User) || !this._player.TryGetSessionByEntity(args.Dragged, ref icommonSession))
      return;
    EntityUid? attachedEntity = icommonSession.AttachedEntity;
    EntityUid dragged = args.Dragged;
    if ((attachedEntity.HasValue ? (EntityUid.op_Inequality(attachedEntity.GetValueOrDefault(), dragged) ? 1 : 0) : 1) != 0)
      return;
    args.CanDrop = false;
    args.Handled = true;
  }

  private void OnRemovedContained(
    Entity<CryostorageContainedComponent> ent,
    ref EntGotRemovedFromContainerMessage args)
  {
    EntityUid entityUid1;
    CryostorageContainedComponent containedComponent1;
    ent.Deconstruct(ref entityUid1, ref containedComponent1);
    EntityUid entityUid2 = entityUid1;
    CryostorageContainedComponent containedComponent2 = containedComponent1;
    if (this.IsInPausedMap(Entity<TransformComponent>.op_Implicit(entityUid2)))
      return;
    this.RemCompDeferred(Entity<CryostorageContainedComponent>.op_Implicit(ent), (IComponent) containedComponent2);
  }

  private void OnShutdownContained(
    Entity<CryostorageContainedComponent> ent,
    ref ComponentShutdown args)
  {
    CryostorageContainedComponent comp = ent.Comp;
    this.CompOrNull<CryostorageComponent>(comp.Cryostorage)?.StoredPlayers.Remove(Entity<CryostorageContainedComponent>.op_Implicit(ent));
    ent.Comp.Cryostorage = new EntityUid?();
    this.Dirty(Entity<CryostorageContainedComponent>.op_Implicit(ent), (IComponent) comp, (MetaDataComponent) null);
  }

  private void OnRoundRestart(RoundRestartCleanupEvent _) => this.DeletePausedMap();

  private void DeletePausedMap()
  {
    if (!this.PausedMap.HasValue || !this.Exists(this.PausedMap))
      return;
    this.Del(new EntityUid?(this.PausedMap.Value));
    this.PausedMap = new EntityUid?();
  }

  protected void EnsurePausedMap()
  {
    if (this.PausedMap.HasValue && this.Exists(this.PausedMap))
      return;
    this.PausedMap = new EntityUid?(this._map.CreateMap(true));
    this._map.SetPaused(Entity<MapComponent>.op_Implicit(this.PausedMap.Value), true);
  }

  public bool IsInPausedMap(Entity<TransformComponent?> entity)
  {
    EntityUid entityUid;
    TransformComponent transformComponent1;
    entity.Deconstruct(ref entityUid, ref transformComponent1);
    TransformComponent transformComponent2 = transformComponent1 ?? this.Transform(Entity<TransformComponent>.op_Implicit(entity));
    if (!transformComponent2.MapUid.HasValue)
      return false;
    EntityUid? mapUid = transformComponent2.MapUid;
    EntityUid? pausedMap = this.PausedMap;
    if (mapUid.HasValue != pausedMap.HasValue)
      return false;
    return !mapUid.HasValue || EntityUid.op_Equality(mapUid.GetValueOrDefault(), pausedMap.GetValueOrDefault());
  }
}
