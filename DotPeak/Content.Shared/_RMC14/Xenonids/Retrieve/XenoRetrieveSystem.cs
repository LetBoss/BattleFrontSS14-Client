// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Retrieve.XenoRetrieveSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Line;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Retrieve;

public sealed class XenoRetrieveSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private LineSystem _line;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private SharedRMCEmoteSystem _rmcEmote;
  [Dependency]
  private RMCSizeStunSystem _rmcSize;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private RMCPullingSystem _rmcPulling;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoRetrieveComponent, XenoRetrieveActionEvent>(new EntityEventRefHandler<XenoRetrieveComponent, XenoRetrieveActionEvent>(this.OnXenoRetrieveAction));
    this.SubscribeLocalEvent<XenoRetrieveComponent, XenoRetrieveDoAfterEvent>(new EntityEventRefHandler<XenoRetrieveComponent, XenoRetrieveDoAfterEvent>(this.OnXenoRetrieveDoAfter));
    this.SubscribeLocalEvent<XenoRetrieveComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoRetrieveComponent, EntityTerminatingEvent>(this.OnXenoRetrieveTerminating));
  }

  private void OnXenoRetrieveAction(
    Entity<XenoRetrieveComponent> xeno,
    ref XenoRetrieveActionEvent args)
  {
    EntityUid target1 = args.Target;
    if (!this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) target1))
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-not-same-hive"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    else if (xeno.Owner == target1)
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-retrieve-self"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    else if (this.Transform(target1).Anchored)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-retrieve-anchored"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    }
    else
    {
      RMCSizes size;
      if (this._rmcSize.TryGetSize(target1, out size) && size > xeno.Comp.SizeLimit && this._mobState.IsAlive(target1) && !this.HasComp<XenoRestingComponent>(target1) && !this._standing.IsDown(target1))
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-retrieve-too-big", ("target", (object) target1)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
      else if (!this._interaction.InRangeUnobstructed((Entity<TransformComponent>) xeno.Owner, (Entity<TransformComponent>) target1, xeno.Comp.Range, CollisionGroup.Impassable))
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-retrieve-blocked", ("target", (object) target1)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
      }
      else
      {
        args.Handled = true;
        XenoRetrieveDoAfterEvent retrieveDoAfterEvent = new XenoRetrieveDoAfterEvent(this.GetNetEntity((EntityUid) args.Action));
        EntityManager entityManager = this.EntityManager;
        EntityUid user = (EntityUid) xeno;
        TimeSpan delay = xeno.Comp.Delay;
        XenoRetrieveDoAfterEvent @event = retrieveDoAfterEvent;
        EntityUid? eventTarget = new EntityUid?((EntityUid) xeno);
        EntityUid? target2 = new EntityUid?(target1);
        EntityUid? blocker = new EntityUid?();
        EntityUid? used = blocker;
        if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user, delay, (DoAfterEvent) @event, eventTarget, target2, used)
        {
          BreakOnMove = true,
          DistanceThreshold = new float?(xeno.Comp.Range),
          DuplicateCondition = DuplicateConditions.SameEvent
        }))
          return;
        this._popup.PopupPredicted(this.Loc.GetString("rmc-xeno-retrieve-start-self", ("target", (object) target1)), this.Loc.GetString("rmc-xeno-retrieve-start-others", ("user", (object) xeno), ("target", (object) target1)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
        foreach (EntityUid visual in xeno.Comp.Visuals)
          this.QueueDel(new EntityUid?(visual));
        xeno.Comp.Visuals.Clear();
        foreach (LineTile lineTile in this._line.DrawLine(xeno.Owner.ToCoordinates(), target1.ToCoordinates(), TimeSpan.Zero, new float?(xeno.Comp.Range), out blocker))
          xeno.Comp.Visuals.Add(this.Spawn((string) xeno.Comp.Visual, lineTile.Coordinates, rotation: new Angle()));
        ProtoId<EmotePrototype>? emote = xeno.Comp.Emote;
        if (!emote.HasValue)
          return;
        ProtoId<EmotePrototype> valueOrDefault = emote.GetValueOrDefault();
        this._rmcEmote.TryEmoteWithChat((EntityUid) xeno, valueOrDefault);
      }
    }
  }

  private void OnXenoRetrieveDoAfter(
    Entity<XenoRetrieveComponent> xeno,
    ref XenoRetrieveDoAfterEvent args)
  {
    foreach (EntityUid visual in xeno.Comp.Visuals)
      this.QueueDel(new EntityUid?(visual));
    xeno.Comp.Visuals.Clear();
    if (args.Handled || args.Cancelled)
      return;
    EntityUid? nullable = args.Target;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    args.Handled = true;
    EntityUid? entity;
    if (!this.TryGetEntity(args.Action, out entity))
      return;
    SharedInteractionSystem interaction = this._interaction;
    Entity<TransformComponent> owner = (Entity<TransformComponent>) xeno.Owner;
    Entity<TransformComponent> other = (Entity<TransformComponent>) valueOrDefault;
    double range = (double) xeno.Comp.Range;
    nullable = new EntityUid?();
    EntityUid? user = nullable;
    if (!interaction.InRangeUnobstructed(owner, other, (float) range, CollisionGroup.Impassable, user: user))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-retrieve-blocked", ("target", (object) valueOrDefault)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    }
    else
    {
      MapCoordinates mapCoordinates1 = this._transform.GetMapCoordinates((EntityUid) xeno);
      MapCoordinates mapCoordinates2 = this._transform.GetMapCoordinates(valueOrDefault);
      if (mapCoordinates1.MapId != mapCoordinates2.MapId)
        return;
      Vector2 vector2_1 = mapCoordinates1.Position - mapCoordinates2.Position;
      Vector2 vector2_2 = vector2_1 + Vector2Helpers.Normalized(vector2_1);
      PhysicsComponent comp;
      if (vector2_2 == Vector2.Zero || !this.TryComp<PhysicsComponent>(valueOrDefault, out comp) || !this._rmcActions.TryUseAction((EntityUid) xeno, entity.Value, valueOrDefault))
        return;
      this._rmcPulling.TryStopAllPullsFromAndOn(valueOrDefault);
      float num1 = vector2_2.Length();
      float num2 = Math.Clamp(num1, 0.1f, xeno.Comp.Range);
      vector2_2 *= num2 / num1;
      Vector2 impulse = Vector2Helpers.Normalized(vector2_2) * xeno.Comp.Force * comp.Mass;
      XenoBeingRetrievedComponent retrievedComponent = this.EnsureComp<XenoBeingRetrievedComponent>(valueOrDefault);
      retrievedComponent.EndTime = this._timing.CurTime + TimeSpan.FromSeconds((double) vector2_2.Length() / (double) xeno.Comp.Force);
      this.Dirty(valueOrDefault, (IComponent) retrievedComponent);
      this._physics.ApplyLinearImpulse(valueOrDefault, impulse, body: comp);
      this._physics.SetBodyStatus(valueOrDefault, comp, BodyStatus.InAir);
      this._popup.PopupPredicted(this.Loc.GetString("rmc-xeno-retrieve-finish-user", ("target", (object) valueOrDefault)), this.Loc.GetString("rmc-xeno-retrieve-finish-others", ("user", (object) xeno), ("target", (object) valueOrDefault)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
      this._audio.PlayPredicted(xeno.Comp.Sound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    }
  }

  private void OnXenoRetrieveTerminating(
    Entity<XenoRetrieveComponent> ent,
    ref EntityTerminatingEvent args)
  {
    foreach (EntityUid visual in ent.Comp.Visuals)
    {
      if (!this.TerminatingOrDeleted(visual) && !this.EntityManager.IsQueuedForDeletion(visual))
        this.QueueDel(new EntityUid?(visual));
    }
    ent.Comp.Visuals.Clear();
  }

  private void StopRetrieve(Entity<XenoBeingRetrievedComponent> retrieved)
  {
    PhysicsComponent comp;
    if (this.TryComp<PhysicsComponent>((EntityUid) retrieved, out comp))
    {
      this._physics.SetLinearVelocity((EntityUid) retrieved, Vector2.Zero, body: comp);
      this._physics.SetBodyStatus((EntityUid) retrieved, comp, BodyStatus.OnGround);
    }
    this.RemCompDeferred<XenoBeingRetrievedComponent>((EntityUid) retrieved);
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoBeingRetrievedComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoBeingRetrievedComponent>();
    EntityUid uid;
    XenoBeingRetrievedComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(curTime < comp1.EndTime))
        this.StopRetrieve((Entity<XenoBeingRetrievedComponent>) (uid, comp1));
    }
  }
}
