// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Ladder.SharedLadderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Teleporter;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.GameTicking;
using Content.Shared.Ghost;
using Content.Shared.Interaction;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Ladder;

public abstract class SharedLadderSystem : EntitySystem
{
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedEyeSystem _eye;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private ISharedPlayerManager _player;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCTeleporterSystem _rmcTeleporter;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  private readonly HashSet<Entity<LadderComponent>> _toUpdate = new HashSet<Entity<LadderComponent>>();
  private readonly Dictionary<string, HashSet<Entity<LadderComponent>>> _toUpdateIds = new Dictionary<string, HashSet<Entity<LadderComponent>>>();
  private Robust.Shared.GameObjects.EntityQuery<ActorComponent> _actorQuery;
  private Robust.Shared.GameObjects.EntityQuery<LadderComponent> _ladderQuery;

  public override void Initialize()
  {
    this._actorQuery = this.GetEntityQuery<ActorComponent>();
    this._ladderQuery = this.GetEntityQuery<LadderComponent>();
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestartCleanup));
    this.SubscribeLocalEvent<LadderComponent, MapInitEvent>(new EntityEventRefHandler<LadderComponent, MapInitEvent>(this.OnLadderMapInit));
    this.SubscribeLocalEvent<LadderComponent, ComponentRemove>(new EntityEventRefHandler<LadderComponent, ComponentRemove>(this.OnLadderRemove<ComponentRemove>));
    this.SubscribeLocalEvent<LadderComponent, EntityTerminatingEvent>(new EntityEventRefHandler<LadderComponent, EntityTerminatingEvent>(this.OnLadderRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<LadderComponent, ActivateInWorldEvent>(new EntityEventRefHandler<LadderComponent, ActivateInWorldEvent>(this.OnLadderActivateInWorld));
    this.SubscribeLocalEvent<LadderComponent, DoAfterAttemptEvent<LadderDoAfterEvent>>(new EntityEventRefHandler<LadderComponent, DoAfterAttemptEvent<LadderDoAfterEvent>>(this.OnLadderDoAfterAttempt));
    this.SubscribeLocalEvent<LadderComponent, LadderDoAfterEvent>(new EntityEventRefHandler<LadderComponent, LadderDoAfterEvent>(this.OnLadderDoAfter));
    this.SubscribeLocalEvent<LadderComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<LadderComponent, GetVerbsEvent<AlternativeVerb>>(this.OnLadderGetAltVerbs));
    this.SubscribeLocalEvent<LadderComponent, CanDropDraggedEvent>(new EntityEventRefHandler<LadderComponent, CanDropDraggedEvent>(this.OnLadderCanDropDragged));
    this.SubscribeLocalEvent<LadderComponent, CanDragEvent>(new EntityEventRefHandler<LadderComponent, CanDragEvent>(this.OnLadderCanDrag));
    this.SubscribeLocalEvent<LadderComponent, DragDropDraggedEvent>(new EntityEventRefHandler<LadderComponent, DragDropDraggedEvent>(this.OnLadderDragDropDragged));
    this.SubscribeLocalEvent<LadderWatchingComponent, MoveInputEvent>(new EntityEventRefHandler<LadderWatchingComponent, MoveInputEvent>(this.OnWatchingMoveInput));
  }

  private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
  {
    this._toUpdate.Clear();
    this._toUpdateIds.Clear();
  }

  private void OnLadderMapInit(Entity<LadderComponent> ent, ref MapInitEvent args)
  {
    this._toUpdate.Add(ent);
  }

  public bool LadderIdInUse(string id) => this._toUpdateIds.ContainsKey(id);

  public void ReassignLadderId(Entity<LadderComponent> ent, string? newId)
  {
    LadderComponent comp;
    if (ent.Comp.Other.HasValue && this.TryComp<LadderComponent>(ent.Comp.Other, out comp))
    {
      EntityUid uid = ent.Comp.Other.Value;
      ent.Comp.Other = new EntityUid?();
      comp.Id = (string) null;
      comp.Other = new EntityUid?();
      this.Dirty(uid, (IComponent) comp);
    }
    if (ent.Comp.Id != null)
      this._toUpdateIds.Remove(ent.Comp.Id);
    ent.Comp.Id = newId;
    this.Dirty<LadderComponent>(ent);
    this._toUpdate.Add(ent);
  }

  private void OnLadderRemove<T>(Entity<LadderComponent> ent, ref T args)
  {
    foreach (EntityUid uid in ent.Comp.Watching)
    {
      if (!this.TerminatingOrDeleted(uid))
        this.RemCompDeferred<LadderWatchingComponent>(uid);
    }
    LadderComponent component;
    if (!this.TerminatingOrDeleted(ent.Comp.Other) && this._ladderQuery.TryComp(ent.Comp.Other, out component))
    {
      component.Other = new EntityUid?();
      this.Dirty(ent.Comp.Other.Value, (IComponent) component);
    }
    ent.Comp.Other = new EntityUid?();
  }

  private void OnLadderActivateInWorld(Entity<LadderComponent> ent, ref ActivateInWorldEvent args)
  {
    EntityUid user = args.User;
    if (!ent.Comp.Other.HasValue)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-ladder-leads-nowhere"), (EntityUid) ent, new EntityUid?(user), PopupType.SmallCaution);
    }
    else
    {
      TimeSpan curTime = this._timing.CurTime;
      EntityUid? lastDoAfterEnt = ent.Comp.LastDoAfterEnt;
      if (lastDoAfterEnt.HasValue)
      {
        EntityUid valueOrDefault1 = lastDoAfterEnt.GetValueOrDefault();
        ushort? lastDoAfterId = ent.Comp.LastDoAfterId;
        if (lastDoAfterId.HasValue)
        {
          ushort valueOrDefault2 = lastDoAfterId.GetValueOrDefault();
          if (curTime - ent.Comp.LastDoAfterTime < ent.Comp.Delay * 5.0 && this._doAfter.GetStatus(new DoAfterId?(new DoAfterId(valueOrDefault1, valueOrDefault2))) == DoAfterStatus.Running)
          {
            lastDoAfterEnt = ent.Comp.LastDoAfterEnt;
            EntityUid entityUid = user;
            if ((lastDoAfterEnt.HasValue ? (lastDoAfterEnt.GetValueOrDefault() != entityUid ? 1 : 0) : 1) == 0)
              return;
            this._popup.PopupClient(this.Loc.GetString("rmc-ladder-someone-else-climbing"), (EntityUid) ent, new EntityUid?(user), PopupType.SmallCaution);
            return;
          }
        }
      }
      LadderDoAfterEvent @event = new LadderDoAfterEvent();
      TimeSpan delay = ent.Comp.Delay;
      if (this.HasComp<GhostComponent>(args.User))
        delay = TimeSpan.Zero;
      DoAfterId? id;
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent))
      {
        AttemptFrequency = delay == TimeSpan.Zero ? AttemptFrequency.Never : AttemptFrequency.EveryTick
      }, out id))
        return;
      ent.Comp.LastDoAfterEnt = new EntityUid?(id.Value.Uid);
      ent.Comp.LastDoAfterId = new ushort?(id.Value.Index);
      ent.Comp.LastDoAfterTime = curTime;
      this.Dirty<LadderComponent>(ent);
      if (ent.Comp.Delay > TimeSpan.Zero)
        this._popup.PopupPredicted(this.Loc.GetString("rmc-ladder-start-climbing-self"), this.Loc.GetString("rmc-ladder-start-climbing-others", ("user", (object) user)), user, new EntityUid?(user));
      ActorComponent component;
      if (!this._actorQuery.TryComp(user, out component))
        return;
      this.AddViewer(ent, component.PlayerSession);
    }
  }

  private void OnLadderDoAfterAttempt(
    Entity<LadderComponent> ent,
    ref DoAfterAttemptEvent<LadderDoAfterEvent> args)
  {
    if (args.Cancelled)
      return;
    EntityUid user = args.DoAfter.Args.User;
    EntityCoordinates coordinates = ent.Owner.ToCoordinates();
    float distance;
    if (!user.ToCoordinates().TryDistance((IEntityManager) this.EntityManager, this._transform, coordinates, out distance) || (double) distance <= (double) ent.Comp.Range)
      return;
    args.Cancel();
  }

  private void OnLadderDoAfter(Entity<LadderComponent> ent, ref LadderDoAfterEvent args)
  {
    EntityUid user = args.User;
    if (this._net.IsClient)
    {
      EntityUid entityUid = user;
      EntityUid? localEntity = this._player.LocalEntity;
      if ((localEntity.HasValue ? (entityUid != localEntity.GetValueOrDefault() ? 1 : 0) : 1) != 0)
        return;
    }
    ActorComponent component;
    if (this._actorQuery.TryComp(user, out component))
      this.RemoveViewer(ent, component.PlayerSession);
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    EntityUid? other = ent.Comp.Other;
    if (!other.HasValue)
      return;
    EntityUid valueOrDefault = other.GetValueOrDefault();
    if (this.TerminatingOrDeleted(ent.Comp.Other))
      return;
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(valueOrDefault);
    if (mapCoordinates.MapId == MapId.Nullspace)
      return;
    this._transform.SetMapCoordinates(user, mapCoordinates);
    this._popup.PopupPredicted(this.Loc.GetString("rmc-ladder-finish-climbing-self"), this.Loc.GetString("rmc-ladder-finish-climbing-others", ("user", (object) user)), user, new EntityUid?(user));
    ent.Comp.LastDoAfterEnt = new EntityUid?();
    ent.Comp.LastDoAfterId = new ushort?();
    this.Dirty<LadderComponent>(ent);
    this._rmcTeleporter.HandlePulling(user, mapCoordinates);
  }

  private void OnLadderGetAltVerbs(
    Entity<LadderComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    EntityUid? other1 = ent.Comp.Other;
    if (!other1.HasValue)
      return;
    EntityUid other = other1.GetValueOrDefault();
    EntityUid user = args.User;
    if (!this.CanWatchPopup(ent, user))
      return;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Priority = 100;
    alternativeVerb.Act = (Action) (() =>
    {
      if (!this.CanWatchPopup(ent, user))
        return;
      this.Watch((Entity<ActorComponent, EyeComponent>) user, (Entity<LadderComponent>) other);
    });
    alternativeVerb.Text = this.Loc.GetString("rmc-ladder-look-through");
    verbs.Add(alternativeVerb);
  }

  private void OnLadderCanDropDragged(Entity<LadderComponent> ent, ref CanDropDraggedEvent args)
  {
    if (args.User != args.Target)
      return;
    args.Handled = true;
    args.CanDrop = true;
  }

  private void OnLadderCanDrag(Entity<LadderComponent> ent, ref CanDragEvent args)
  {
    args.Handled = true;
  }

  private void OnLadderDragDropDragged(Entity<LadderComponent> ent, ref DragDropDraggedEvent args)
  {
    EntityUid user = args.User;
    EntityUid? other = ent.Comp.Other;
    if (!other.HasValue)
      return;
    EntityUid valueOrDefault = other.GetValueOrDefault();
    if (user != args.Target || !this.CanWatchPopup(ent, user))
      return;
    args.Handled = true;
    this.Watch((Entity<ActorComponent, EyeComponent>) user, (Entity<LadderComponent>) valueOrDefault);
  }

  private void OnWatchingMoveInput(Entity<LadderWatchingComponent> ent, ref MoveInputEvent args)
  {
    if (!args.HasDirectionalMovement)
      return;
    if (this._net.IsClient)
    {
      EntityUid? localEntity = this._player.LocalEntity;
      EntityUid owner = ent.Owner;
      if ((localEntity.HasValue ? (localEntity.GetValueOrDefault() == owner ? 1 : 0) : 0) != 0 && this._player.LocalSession != null)
      {
        this.Unwatch((Entity<EyeComponent>) ent.Owner, this._player.LocalSession);
        return;
      }
    }
    ActorComponent comp;
    if (!this.TryComp<ActorComponent>((EntityUid) ent, out comp))
      return;
    this.Unwatch((Entity<EyeComponent>) ent.Owner, comp.PlayerSession);
  }

  protected virtual void AddViewer(Entity<LadderComponent> ent, ICommonSession player)
  {
  }

  protected virtual void RemoveViewer(Entity<LadderComponent> ent, ICommonSession player)
  {
  }

  protected virtual void Watch(
    Entity<ActorComponent?, EyeComponent?> watcher,
    Entity<LadderComponent?> toWatch)
  {
  }

  protected virtual void Unwatch(Entity<EyeComponent?> watcher, ICommonSession player)
  {
    if (!this.Resolve<EyeComponent>((EntityUid) watcher, ref watcher.Comp))
      return;
    this._eye.SetTarget((EntityUid) watcher, new EntityUid?());
  }

  protected bool CanWatchPopup(Entity<LadderComponent> ladder, EntityUid user)
  {
    return this._interaction.InRangeUnobstructed((Entity<TransformComponent>) user, (Entity<TransformComponent>) ladder.Owner, popup: true);
  }

  public override void Update(float frameTime)
  {
    if (this._toUpdate.Count == 0)
      return;
    if (this._net.IsClient)
    {
      this._toUpdateIds.Clear();
      this._toUpdate.Clear();
    }
    else
    {
      this._toUpdateIds.Clear();
      foreach (Entity<LadderComponent> entity in this._toUpdate)
      {
        string id = entity.Comp.Id;
        if (id != null)
        {
          HashSet<Entity<LadderComponent>> orNew = this._toUpdateIds.GetOrNew<string, HashSet<Entity<LadderComponent>>>(id);
          if (orNew.Count > 2)
          {
            string str = string.Join<EntityStringRepresentation?>(",", orNew.Select<Entity<LadderComponent>, EntityStringRepresentation?>((Func<Entity<LadderComponent>, EntityStringRepresentation?>) (e => this.ToPrettyString(new EntityUid?((EntityUid) e)))));
            this.Log.Error($"Found more than 2 ladders with id {id}, current ladder: {this.ToPrettyString(new EntityUid?((EntityUid) entity))}, previous ladders: {str}");
          }
          orNew.Add(entity);
        }
      }
      this._toUpdate.Clear();
      Robust.Shared.GameObjects.EntityQueryEnumerator<LadderComponent> entityQueryEnumerator = this.EntityQueryEnumerator<LadderComponent>();
      while (true)
      {
        LadderComponent comp1;
        Entity<LadderComponent> valueOrDefault1;
        EntityUid uid;
        do
        {
          Entity<LadderComponent>? nullable;
          do
          {
            HashSet<Entity<LadderComponent>> source;
            do
            {
              if (!entityQueryEnumerator.MoveNext(out uid, out comp1))
                goto label_1;
            }
            while (comp1.Id == null || !this._toUpdateIds.TryGetValue(comp1.Id, out source));
            nullable = source.FirstOrNull<Entity<LadderComponent>>((Func<Entity<LadderComponent>, bool>) (e => e.Owner != uid));
          }
          while (!nullable.HasValue);
          valueOrDefault1 = nullable.GetValueOrDefault();
        }
        while (valueOrDefault1.Owner == uid);
        EntityUid? other = comp1.Other;
        if (other.HasValue)
        {
          EntityUid valueOrDefault2 = other.GetValueOrDefault();
          if (valueOrDefault2 != valueOrDefault1.Owner)
            this.Log.Error($"Found {this.ToPrettyString(new EntityUid?((EntityUid) valueOrDefault1))} with duplicate ID {valueOrDefault1.Comp.Id}, previous ladder: {this.ToPrettyString((Entity<MetaDataComponent>) valueOrDefault2)}");
        }
        comp1.Other = new EntityUid?((EntityUid) valueOrDefault1);
        this.Dirty(uid, (IComponent) comp1);
        valueOrDefault1.Comp.Other = new EntityUid?(uid);
        this.Dirty<LadderComponent>(valueOrDefault1);
      }
label_1:;
    }
  }
}
