// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.HiveLeader.HiveLeaderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.Tracker;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Content.Shared._RMC14.Xenonids.Watch;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.HiveLeader;

public sealed class HiveLeaderSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private DialogSystem _dialog;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedCMChatSystem _rmcChat;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedXenoWatchSystem _xenoWatch;
  private Robust.Shared.GameObjects.EntityQuery<XenoAttachedOvipositorComponent> _attachedOvipositorQuery;
  private Robust.Shared.GameObjects.EntityQuery<HiveLeaderComponent> _hiveLeaderQuery;
  private Robust.Shared.GameObjects.EntityQuery<HiveLeaderGranterComponent> _hiveLeaderGranterQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoActivePheromonesComponent> _activePheromonesQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoPheromonesComponent> _pheromonesQuery;

  public override void Initialize()
  {
    this._attachedOvipositorQuery = this.GetEntityQuery<XenoAttachedOvipositorComponent>();
    this._hiveLeaderQuery = this.GetEntityQuery<HiveLeaderComponent>();
    this._hiveLeaderGranterQuery = this.GetEntityQuery<HiveLeaderGranterComponent>();
    this._activePheromonesQuery = this.GetEntityQuery<XenoActivePheromonesComponent>();
    this._pheromonesQuery = this.GetEntityQuery<XenoPheromonesComponent>();
    this.SubscribeLocalEvent<NewXenoEvolvedEvent>(new EntityEventRefHandler<NewXenoEvolvedEvent>(this.OnLeaderNewXenoEvolved));
    this.SubscribeLocalEvent<XenoDevolvedEvent>(new EntityEventRefHandler<XenoDevolvedEvent>(this.OnLeaderXenoDevolved));
    this.SubscribeLocalEvent<HiveLeaderComponent, ComponentRemove>(new EntityEventRefHandler<HiveLeaderComponent, ComponentRemove>(this.OnLeaderRemove<ComponentRemove>));
    this.SubscribeLocalEvent<HiveLeaderComponent, EntityTerminatingEvent>(new EntityEventRefHandler<HiveLeaderComponent, EntityTerminatingEvent>(this.OnLeaderRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<HiveLeaderComponent, MobStateChangedEvent>(new EntityEventRefHandler<HiveLeaderComponent, MobStateChangedEvent>(this.OnLeaderMobStateChanged));
    this.SubscribeLocalEvent<HiveLeaderGranterComponent, ComponentRemove>(new EntityEventRefHandler<HiveLeaderGranterComponent, ComponentRemove>(this.OnGranterRemove<ComponentRemove>));
    this.SubscribeLocalEvent<HiveLeaderGranterComponent, EntityTerminatingEvent>(new EntityEventRefHandler<HiveLeaderGranterComponent, EntityTerminatingEvent>(this.OnGranterRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<HiveLeaderGranterComponent, MobStateChangedEvent>(new EntityEventRefHandler<HiveLeaderGranterComponent, MobStateChangedEvent>(this.OnGranterMobStateChanged));
    this.SubscribeLocalEvent<HiveLeaderGranterComponent, HiveLeaderActionEvent>(new EntityEventRefHandler<HiveLeaderGranterComponent, HiveLeaderActionEvent>(this.OnGranterAction));
    this.SubscribeLocalEvent<HiveLeaderGranterComponent, HiveLeaderWatchEvent>(new EntityEventRefHandler<HiveLeaderGranterComponent, HiveLeaderWatchEvent>(this.OnGranterWatch));
    this.SubscribeLocalEvent<HiveLeaderGranterComponent, XenoPheromonesActivatedEvent>(new EntityEventRefHandler<HiveLeaderGranterComponent, XenoPheromonesActivatedEvent>(this.OnGranterPheromonesActivated));
    this.SubscribeLocalEvent<HiveLeaderGranterComponent, XenoPheromonesDeactivatedEvent>(new EntityEventRefHandler<HiveLeaderGranterComponent, XenoPheromonesDeactivatedEvent>(this.OnGranterPheromonesDeactivated));
    this.SubscribeLocalEvent<HiveLeaderGranterComponent, XenoOvipositorChangedEvent>(new EntityEventRefHandler<HiveLeaderGranterComponent, XenoOvipositorChangedEvent>(this.OnGranterOvipositorChanged));
  }

  private void OnLeaderRemove<T>(Entity<HiveLeaderComponent> ent, ref T args)
  {
    this.RemoveLeader(ent);
  }

  private void OnLeaderMobStateChanged(
    Entity<HiveLeaderComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState != MobState.Dead)
      return;
    this.RemoveLeader(ent);
  }

  private void OnLeaderNewXenoEvolved(ref NewXenoEvolvedEvent args)
  {
    this.Transfer((EntityUid) args.OldXeno, args.NewXeno);
  }

  private void OnLeaderXenoDevolved(ref XenoDevolvedEvent args)
  {
    this.Transfer(args.OldXeno, args.NewXeno);
  }

  private void OnGranterRemove<T>(Entity<HiveLeaderGranterComponent> ent, ref T args)
  {
    this.RemoveLeaders(ent);
  }

  private void OnGranterMobStateChanged(
    Entity<HiveLeaderGranterComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState != MobState.Dead)
      return;
    this.RemoveLeaders(ent);
  }

  private void OnGranterAction(
    Entity<HiveLeaderGranterComponent> ent,
    ref HiveLeaderActionEvent args)
  {
    List<EntityUid> leaders = ent.Comp.Leaders;
    int maxLeaders = ent.Comp.MaxLeaders;
    EntityUid watched;
    if (!this._xenoWatch.TryGetWatched((Entity<XenoWatchingComponent>) ent.Owner, out watched))
    {
      if (leaders.Count == 0)
      {
        this._popup.PopupClient("There are no Xenonid leaders. Overwatch a Xenonid to make it a leader.", (EntityUid) ent, new EntityUid?((EntityUid) ent), PopupType.MediumCaution);
      }
      else
      {
        List<DialogOption> options = new List<DialogOption>();
        foreach (EntityUid uid in leaders)
          options.Add(new DialogOption(this.Name(uid), (object) new HiveLeaderWatchEvent(this.GetNetEntity(uid))));
        this._dialog.OpenOptions((EntityUid) ent, "Watch with leader?", options, "Target");
      }
    }
    else
    {
      if (ent.Owner == watched)
        return;
      if (!this.HasComp<HiveLeaderComponent>(watched) && leaders.Count >= maxLeaders)
      {
        this._popup.PopupClient($"You can't have more than {maxLeaders} promoted leaders.", watched, new EntityUid?((EntityUid) ent), PopupType.MediumCaution);
      }
      else
      {
        HiveLeaderComponent comp;
        if (this.EnsureComp<HiveLeaderComponent>(watched, out comp))
        {
          this.RemCompDeferred<HiveLeaderComponent>(watched);
          this.RemComp<RMCTrackableComponent>(watched);
          ent.Comp.Leaders.Remove(watched);
          this._popup.PopupClient($"You've demoted {this.Name(watched)} from Hive Leader.", watched, new EntityUid?((EntityUid) ent), PopupType.MediumCaution);
          string message = this.Name((EntityUid) ent) + " has demoted you from Hive Leader. Your leadership rights and abilities have waned.";
          this._popup.PopupEntity(message, watched, watched, PopupType.MediumCaution);
          this._rmcChat.ChatMessageToOne(message, watched);
          HiveLeaderStatusChangedEvent args1 = new HiveLeaderStatusChangedEvent(false);
          this.RaiseLocalEvent<HiveLeaderStatusChangedEvent>(watched, ref args1);
        }
        else
        {
          this.EnsureComp<RMCTrackableComponent>(watched);
          comp.Granter = new EntityUid?((EntityUid) ent);
          this.Dirty(watched, (IComponent) comp);
          ent.Comp.Leaders.Add(watched);
          this.Dirty<HiveLeaderGranterComponent>(ent);
          this._popup.PopupClient($"You've selected {this.Name(watched)} as a Hive Leader.", watched, new EntityUid?((EntityUid) ent), PopupType.Medium);
          string message = this.Name((EntityUid) ent) + " has selected you as a Hive Leader. The other Xenonids must listen to you. You will also act as a beacon for the Queen's pheromones.";
          this._popup.PopupClient(message, watched, new EntityUid?(watched), PopupType.Medium);
          this._rmcChat.ChatMessageToOne(message, watched);
          HiveLeaderStatusChangedEvent args2 = new HiveLeaderStatusChangedEvent(true);
          this.RaiseLocalEvent<HiveLeaderStatusChangedEvent>(watched, ref args2);
          this.SyncPheromones(ent);
        }
      }
    }
  }

  private void OnGranterWatch(Entity<HiveLeaderGranterComponent> ent, ref HiveLeaderWatchEvent args)
  {
    EntityUid? entity;
    if (!this.TryGetEntity(args.Leader, out entity) || !this._hiveLeaderQuery.HasComp(entity))
      return;
    this._xenoWatch.Watch((Entity<HiveMemberComponent, ActorComponent, EyeComponent>) ent.Owner, (Entity<HiveMemberComponent>) entity.Value);
  }

  private void OnGranterPheromonesActivated(
    Entity<HiveLeaderGranterComponent> ent,
    ref XenoPheromonesActivatedEvent args)
  {
    this.SyncPheromones(ent);
  }

  private void OnGranterPheromonesDeactivated(
    Entity<HiveLeaderGranterComponent> ent,
    ref XenoPheromonesDeactivatedEvent args)
  {
    this.SyncPheromones(ent, true);
  }

  private void OnGranterOvipositorChanged(
    Entity<HiveLeaderGranterComponent> ent,
    ref XenoOvipositorChangedEvent args)
  {
    this.SyncPheromones(ent);
  }

  private void RemoveLeaders(Entity<HiveLeaderGranterComponent> ent)
  {
    if (this._timing.ApplyingState)
      return;
    this.SyncPheromones(ent, true);
    foreach (EntityUid leader in ent.Comp.Leaders)
    {
      this.RemCompDeferred<HiveLeaderComponent>(leader);
      HiveLeaderStatusChangedEvent args = new HiveLeaderStatusChangedEvent(false);
      this.RaiseLocalEvent<HiveLeaderStatusChangedEvent>(leader, ref args);
    }
    ent.Comp.Leaders.Clear();
  }

  private void SyncPheromones(Entity<HiveLeaderGranterComponent> ent, bool forceDisable = false)
  {
    XenoPheromonesComponent component1;
    if (this._timing.ApplyingState || !this._pheromonesQuery.TryComp((EntityUid) ent, out component1))
      return;
    XenoActivePheromonesComponent component2;
    bool flag = this._activePheromonesQuery.TryComp((EntityUid) ent, out component2) && this._attachedOvipositorQuery.HasComp((EntityUid) ent) && !this._mobState.IsDead((EntityUid) ent) && !forceDisable;
    foreach (EntityUid leader in ent.Comp.Leaders)
    {
      HiveLeaderComponent component3;
      if (this._hiveLeaderQuery.TryComp(leader, out component3))
      {
        if (!flag)
        {
          BaseContainer container;
          EntityUid? element;
          if (this._container.TryGetContainer(leader, component3.PheromonesContainerId, out container) && container.ContainedEntities.TryFirstOrNull<EntityUid>(out element))
            this.RemComp<XenoActivePheromonesComponent>(element.Value);
        }
        else
        {
          ContainerSlot containerSlot = this._container.EnsureContainer<ContainerSlot>(leader, component3.PheromonesContainerId);
          EntityUid? containedEntity = containerSlot.ContainedEntity;
          EntityUid uid1;
          if (!containedEntity.HasValue)
          {
            EntityUid? uid2;
            if (this.TrySpawnInContainer((string) ent.Comp.PheromoneRelayId, leader, component3.PheromonesContainerId, out uid2))
              uid1 = uid2.Value;
            else
              continue;
          }
          else
          {
            containedEntity = containerSlot.ContainedEntity;
            uid1 = containedEntity.Value;
          }
          XenoPheromonesComponent pheromonesComponent1 = this.EnsureComp<XenoPheromonesComponent>(uid1);
          pheromonesComponent1.PheromonesPlasmaCost = 0;
          pheromonesComponent1.PheromonesPlasmaUpkeep = (FixedPoint2) 0;
          pheromonesComponent1.PheromonesRange = component1.PheromonesRange;
          pheromonesComponent1.PheromonesMultiplier = component1.PheromonesMultiplier;
          this.Dirty(uid1, (IComponent) pheromonesComponent1);
          XenoActivePheromonesComponent pheromonesComponent2 = this.EnsureComp<XenoActivePheromonesComponent>(uid1);
          if (component2 != null)
            pheromonesComponent2.Pheromones = component2.Pheromones;
        }
      }
    }
  }

  private void RemoveLeader(Entity<HiveLeaderComponent> leader)
  {
    if (this._timing.ApplyingState)
      return;
    BaseContainer container;
    EntityUid? element;
    if (this._container.TryGetContainer((EntityUid) leader, leader.Comp.PheromonesContainerId, out container) && container.ContainedEntities.TryFirstOrNull<EntityUid>(out element))
      this.RemComp<XenoActivePheromonesComponent>(element.Value);
    this.RemCompDeferred<HiveLeaderComponent>((EntityUid) leader);
    HiveLeaderGranterComponent comp;
    if (!this.TryComp<HiveLeaderGranterComponent>(leader.Comp.Granter, out comp))
      return;
    comp.Leaders.Remove((EntityUid) leader);
    this.Dirty(leader.Comp.Granter.Value, (IComponent) comp);
    this.SyncPheromones((Entity<HiveLeaderGranterComponent>) (leader.Comp.Granter.Value, comp));
  }

  private void Transfer(EntityUid oldXeno, EntityUid newXeno)
  {
    HiveLeaderComponent component1;
    HiveLeaderGranterComponent component2;
    if (!this._hiveLeaderQuery.TryComp(oldXeno, out component1) || !this._hiveLeaderGranterQuery.TryComp(component1.Granter, out component2) || this._hiveLeaderGranterQuery.HasComp(newXeno))
      return;
    this.EnsureComp<RMCTrackableComponent>(newXeno);
    this.EnsureComp<HiveLeaderComponent>(newXeno).Granter = component1.Granter;
    component2.Leaders.Remove(oldXeno);
    component2.Leaders.Add(newXeno);
    this.SyncPheromones((Entity<HiveLeaderGranterComponent>) (component1.Granter.Value, component2));
  }

  public bool IsLeader(EntityUid leader, [NotNullWhen(true)] out HiveLeaderComponent? leaderComp)
  {
    return this.TryComp<HiveLeaderComponent>(leader, out leaderComp);
  }
}
