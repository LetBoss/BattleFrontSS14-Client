// Decompiled with JetBrains decompiler
// Type: Content.Shared.Follower.FollowerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Teams;
using Content.Shared.Administration.Managers;
using Content.Shared.Database;
using Content.Shared.Follower.Components;
using Content.Shared.Ghost;
using Content.Shared.Hands;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Polymorph;
using Content.Shared.Silicons.StationAi;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Events;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Follower;

public sealed class FollowerSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private TagSystem _tagSystem;
  [Dependency]
  private SharedContainerSystem _containerSystem;
  [Dependency]
  private SharedJointSystem _jointSystem;
  [Dependency]
  private SharedPhysicsSystem _physicsSystem;
  [Dependency]
  private INetManager _netMan;
  [Dependency]
  private ISharedAdminManager _adminManager;
  private static readonly ProtoId<TagPrototype> ForceableFollowTag = (ProtoId<TagPrototype>) "ForceableFollow";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<GetVerbsEvent<AlternativeVerb>>(new EntityEventHandler<GetVerbsEvent<AlternativeVerb>>(this.OnGetAlternativeVerbs));
    this.SubscribeLocalEvent<FollowerComponent, MoveInputEvent>(new ComponentEventRefHandler<FollowerComponent, MoveInputEvent>(this.OnFollowerMove));
    this.SubscribeLocalEvent<FollowerComponent, PullStartedMessage>(new ComponentEventHandler<FollowerComponent, PullStartedMessage>(this.OnPullStarted));
    this.SubscribeLocalEvent<FollowerComponent, EntityTerminatingEvent>(new ComponentEventRefHandler<FollowerComponent, EntityTerminatingEvent>(this.OnFollowerTerminating));
    this.SubscribeLocalEvent<FollowerComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<FollowerComponent, AfterAutoHandleStateEvent>(this.OnAfterHandleState));
    this.SubscribeLocalEvent<FollowedComponent, ComponentGetStateAttemptEvent>(new EntityEventRefHandler<FollowedComponent, ComponentGetStateAttemptEvent>(this.OnFollowedAttempt));
    this.SubscribeLocalEvent<FollowerComponent, GotEquippedHandEvent>(new ComponentEventHandler<FollowerComponent, GotEquippedHandEvent>(this.OnGotEquippedHand));
    this.SubscribeLocalEvent<FollowedComponent, EntityTerminatingEvent>(new ComponentEventRefHandler<FollowedComponent, EntityTerminatingEvent>(this.OnFollowedTerminating));
    this.SubscribeLocalEvent<BeforeSerializationEvent>(new EntityEventHandler<BeforeSerializationEvent>(this.OnBeforeSave));
    this.SubscribeLocalEvent<FollowedComponent, PolymorphedEvent>(new EntityEventRefHandler<FollowedComponent, PolymorphedEvent>(this.OnFollowedPolymorphed));
    this.SubscribeLocalEvent<FollowedComponent, StationAiRemoteEntityReplacementEvent>(new EntityEventRefHandler<FollowedComponent, StationAiRemoteEntityReplacementEvent>(this.OnFollowedStationAiRemoteEntityReplaced));
  }

  private void OnFollowedAttempt(
    Entity<FollowedComponent> ent,
    ref ComponentGetStateAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    EntityUid? attachedEntity = (EntityUid?) args.Player?.AttachedEntity;
    if (attachedEntity.HasValue && (ent.Comp.Following.Contains(attachedEntity.Value) || this.HasComp<GhostComponent>(attachedEntity.Value)))
      return;
    args.Cancelled = true;
  }

  private void OnBeforeSave(BeforeSerializationEvent ev)
  {
    HashSet<EntityUid?> hashSet = ev.Entities.Select<EntityUid, EntityUid?>((Func<EntityUid, EntityUid?>) (x => this.Transform(x).MapUid)).ToHashSet<EntityUid?>();
    AllEntityQueryEnumerator<FollowerComponent, TransformComponent, MetaDataComponent> entityQueryEnumerator = this.AllEntityQuery<FollowerComponent, TransformComponent, MetaDataComponent>();
    EntityUid uid;
    FollowerComponent comp1;
    TransformComponent comp2;
    MetaDataComponent comp3;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2, out comp3))
    {
      if (comp3.EntityPrototype != null && !comp3.EntityPrototype.MapSavable && hashSet.Contains(comp2.MapUid))
        this.StopFollowingEntity(uid, comp1.Following);
    }
  }

  private void OnGetAlternativeVerbs(GetVerbsEvent<AlternativeVerb> ev)
  {
    if (ev.User == ev.Target || this.IsClientSide(ev.Target))
      return;
    if (this.HasComp<GhostComponent>(ev.User) && this.CanStartFollowingEntity(ev.User, ev.Target))
    {
      AlternativeVerb alternativeVerb1 = new AlternativeVerb();
      alternativeVerb1.Priority = 10;
      alternativeVerb1.Act = (Action) (() => this.StartFollowingEntity(ev.User, ev.Target));
      alternativeVerb1.Impact = LogImpact.Low;
      alternativeVerb1.Text = this.Loc.GetString("verb-follow-text");
      alternativeVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/open.svg.192dpi.png"));
      AlternativeVerb alternativeVerb2 = alternativeVerb1;
      ev.Verbs.Add(alternativeVerb2);
    }
    if (!this._tagSystem.HasTag(ev.Target, FollowerSystem.ForceableFollowTag) || !ev.CanAccess || !ev.CanInteract)
      return;
    AlternativeVerb alternativeVerb3 = new AlternativeVerb();
    alternativeVerb3.Priority = 10;
    alternativeVerb3.Act = (Action) (() => this.StartFollowingEntity(ev.Target, ev.User));
    alternativeVerb3.Impact = LogImpact.Low;
    alternativeVerb3.Text = this.Loc.GetString("verb-follow-me-text");
    alternativeVerb3.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/close.svg.192dpi.png"));
    AlternativeVerb alternativeVerb4 = alternativeVerb3;
    ev.Verbs.Add(alternativeVerb4);
  }

  private void OnFollowerMove(EntityUid uid, FollowerComponent component, ref MoveInputEvent args)
  {
    if (!args.HasDirectionalMovement)
      return;
    this.StopFollowingEntity(uid, component.Following);
  }

  private void OnPullStarted(EntityUid uid, FollowerComponent component, PullStartedMessage args)
  {
    this.StopFollowingEntity(uid, component.Following);
  }

  private void OnGotEquippedHand(
    EntityUid uid,
    FollowerComponent component,
    GotEquippedHandEvent args)
  {
    this.StopFollowingEntity(uid, component.Following, deparent: false);
  }

  private void OnFollowerTerminating(
    EntityUid uid,
    FollowerComponent component,
    ref EntityTerminatingEvent args)
  {
    this.StopFollowingEntity(uid, component.Following, deparent: false);
  }

  private void OnAfterHandleState(
    Entity<FollowerComponent> entity,
    ref AfterAutoHandleStateEvent args)
  {
    this.StartFollowingEntity((EntityUid) entity, entity.Comp.Following);
  }

  private void OnFollowedTerminating(
    EntityUid uid,
    FollowedComponent component,
    ref EntityTerminatingEvent args)
  {
    this.StopAllFollowers(uid, component);
  }

  private void OnFollowedPolymorphed(Entity<FollowedComponent> entity, ref PolymorphedEvent args)
  {
    foreach (EntityUid follower in entity.Comp.Following)
      this.StartFollowingEntity(follower, args.NewEntity);
  }

  private void OnFollowedStationAiRemoteEntityReplaced(
    Entity<FollowedComponent> entity,
    ref StationAiRemoteEntityReplacementEvent args)
  {
    if (!args.NewRemoteEntity.HasValue)
      return;
    foreach (EntityUid follower in entity.Comp.Following)
      this.StartFollowingEntity(follower, args.NewRemoteEntity.Value);
  }

  public void StartFollowingEntity(EntityUid follower, EntityUid entity)
  {
    if (!this.CanStartFollowingEntity(follower, entity))
      return;
    for (TransformComponent transformComponent = this.Transform(entity); transformComponent.ParentUid.IsValid(); transformComponent = this.Transform(transformComponent.ParentUid))
    {
      if (transformComponent.ParentUid == follower)
        return;
    }
    FollowerComponent comp1;
    if (this.TryComp<FollowerComponent>(follower, out comp1))
    {
      if (comp1.Following == entity)
        return;
      this.StopFollowingEntity(follower, comp1.Following, deparent: false, removeComp: false);
    }
    else
      comp1 = this.AddComp<FollowerComponent>(follower);
    comp1.Following = entity;
    FollowedComponent followedComponent = this.EnsureComp<FollowedComponent>(entity);
    if (!followedComponent.Following.Add(follower))
      return;
    JointComponent comp2;
    if (this.TryComp<JointComponent>(follower, out comp2))
      this._jointSystem.ClearJoints(follower, comp2);
    TransformComponent xform = this.Transform(follower);
    this._containerSystem.AttachParentToContainerOrGrid((Entity<TransformComponent>) (follower, xform));
    if (xform.ParentUid != this.Transform(xform.ParentUid).ParentUid)
      this._transform.SetCoordinates(follower, xform, new EntityCoordinates(entity, Vector2.Zero), new Angle?(Angle.Zero));
    this._physicsSystem.SetLinearVelocity(follower, Vector2.Zero);
    this.EnsureComp<OrbitVisualsComponent>(follower);
    StartedFollowingEntityEvent args1 = new StartedFollowingEntityEvent(entity, follower);
    EntityStartedFollowingEvent args2 = new EntityStartedFollowingEvent(entity, follower);
    this.RaiseLocalEvent<StartedFollowingEntityEvent>(follower, args1);
    this.RaiseLocalEvent<EntityStartedFollowingEvent>(entity, args2);
    this.Dirty(entity, (IComponent) followedComponent);
    this.Dirty(follower, (IComponent) comp1);
  }

  private bool CanStartFollowingEntity(EntityUid follower, EntityUid entity)
  {
    if (!this.Exists(follower) || !this.Exists(entity))
      return false;
    CivTeamMemberComponent comp1;
    if (!this.TryComp<CivTeamMemberComponent>(follower, out comp1) || !comp1.IsCommander)
      return true;
    CivTeamMemberComponent comp2;
    return this.TryComp<CivTeamMemberComponent>(entity, out comp2) && comp2.TeamId > 0 && comp2.TeamId == comp1.TeamId && !comp2.IsCommander;
  }

  public void StopFollowingEntity(
    EntityUid uid,
    EntityUid target,
    FollowedComponent? followed = null,
    bool deparent = true,
    bool removeComp = true)
  {
    FollowerComponent comp1;
    if (!this.Resolve<FollowedComponent>(target, ref followed, false) || !this.TryComp<FollowerComponent>(uid, out comp1) || comp1.Following != target)
      return;
    followed.Following.Remove(uid);
    if (followed.Following.Count == 0)
      this.RemComp<FollowedComponent>(target);
    if (removeComp)
    {
      this.RemComp<FollowerComponent>(uid);
      this.RemComp<OrbitVisualsComponent>(uid);
    }
    StoppedFollowingEntityEvent args1 = new StoppedFollowingEntityEvent(target, uid);
    EntityStoppedFollowingEvent args2 = new EntityStoppedFollowingEvent(target, uid);
    this.RaiseLocalEvent<StoppedFollowingEntityEvent>(uid, args1, true);
    this.RaiseLocalEvent<EntityStoppedFollowingEvent>(target, args2);
    this.Dirty(target, (IComponent) followed);
    this.RaiseLocalEvent<StoppedFollowingEntityEvent>(uid, args1);
    this.RaiseLocalEvent<EntityStoppedFollowingEvent>(target, args2);
    TransformComponent comp2;
    if (!deparent || !this.TryComp(uid, out comp2))
      return;
    this._transform.AttachToGridOrMap(uid, comp2);
    if (comp2.MapUid.HasValue)
      return;
    if (this._netMan.IsClient)
    {
      this._transform.DetachEntity(uid, comp2);
    }
    else
    {
      this.Log.Warning($"A follower has been detached to null-space and will be deleted. Follower: {this.ToPrettyString((Entity<MetaDataComponent>) uid)}. Followed: {this.ToPrettyString((Entity<MetaDataComponent>) target)}");
      this.QueueDel(new EntityUid?(uid));
    }
  }

  public void StopAllFollowers(EntityUid uid, FollowedComponent? followed = null)
  {
    if (!this.Resolve<FollowedComponent>(uid, ref followed))
      return;
    foreach (EntityUid uid1 in followed.Following)
      this.StopFollowingEntity(uid1, uid, followed);
  }

  public EntityUid? GetMostGhostFollowed()
  {
    EntityUid? mostGhostFollowed = new EntityUid?();
    int num1 = 0;
    Dictionary<EntityUid, int> dictionary = new Dictionary<EntityUid, int>();
    Robust.Shared.GameObjects.EntityQueryEnumerator<FollowerComponent, GhostComponent, ActorComponent> entityQueryEnumerator = this.EntityQueryEnumerator<FollowerComponent, GhostComponent, ActorComponent>();
    FollowerComponent comp1;
    ActorComponent comp3;
    while (entityQueryEnumerator.MoveNext(out EntityUid _, out comp1, out GhostComponent _, out comp3))
    {
      if (!this._adminManager.IsAdmin(comp3.PlayerSession))
      {
        EntityUid following = comp1.Following;
        int num2;
        dictionary.TryGetValue(following, out num2);
        dictionary[following] = num2 + 1;
        if (dictionary[following] > num1)
        {
          mostGhostFollowed = new EntityUid?(following);
          num1 = dictionary[following];
        }
      }
    }
    return mostGhostFollowed;
  }
}
