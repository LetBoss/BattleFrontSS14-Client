// Decompiled with JetBrains decompiler
// Type: Content.Shared.Climbing.Systems.ClimbSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CombatMode;
using Content.Shared._RMC14.Movement;
using Content.Shared.ActionBlocker;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.CCVar;
using Content.Shared.Climbing.Components;
using Content.Shared.Climbing.Events;
using Content.Shared.CombatMode;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Climbing.Systems;

public sealed class ClimbSystem : VirtualController
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private ActionBlockerSystem _actionBlockerSystem;
  [Dependency]
  private DamageableSystem _damageableSystem;
  [Dependency]
  private FixtureSystem _fixtureSystem;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Dependency]
  private SharedContainerSystem _containers;
  [Dependency]
  private SharedInteractionSystem _interactionSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedStunSystem _stunSystem;
  [Dependency]
  private SharedTransformSystem _xformSystem;
  [Dependency]
  private RMCMovementSystem _rmcMovement;
  [Dependency]
  private INetConfigurationManager _netConfig;
  private const string ClimbingFixtureName = "climb";
  private const int ClimbingCollisionGroup = 67108884 /*0x04000014*/;
  private EntityQuery<ClimbableComponent> _climbableQuery;
  private EntityQuery<FixturesComponent> _fixturesQuery;
  private EntityQuery<TransformComponent> _xformQuery;

  public virtual void Initialize()
  {
    base.Initialize();
    this._climbableQuery = ((EntitySystem) this).GetEntityQuery<ClimbableComponent>();
    this._fixturesQuery = ((EntitySystem) this).GetEntityQuery<FixturesComponent>();
    this._xformQuery = ((EntitySystem) this).GetEntityQuery<TransformComponent>();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ClimbingComponent, UpdateCanMoveEvent>(new ComponentEventHandler<ClimbingComponent, UpdateCanMoveEvent>((object) this, __methodptr(OnMoveAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ClimbingComponent, EntParentChangedMessage>(new ComponentEventRefHandler<ClimbingComponent, EntParentChangedMessage>((object) this, __methodptr(OnParentChange)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ClimbingComponent, ClimbSystem.ClimbDoAfterEvent>(new ComponentEventHandler<ClimbingComponent, ClimbSystem.ClimbDoAfterEvent>((object) this, __methodptr(OnDoAfter)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ClimbingComponent, EndCollideEvent>(new ComponentEventRefHandler<ClimbingComponent, EndCollideEvent>((object) this, __methodptr(OnClimbEndCollide)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ClimbingComponent, BuckledEvent>(new ComponentEventRefHandler<ClimbingComponent, BuckledEvent>((object) this, __methodptr(OnBuckled)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ClimbingComponent, EntGotInsertedIntoContainerMessage>(new ComponentEventRefHandler<ClimbingComponent, EntGotInsertedIntoContainerMessage>((object) this, __methodptr(OnStored)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ClimbingComponent, RMCCombatModeInteractOverrideUserEvent>(new ComponentEventRefHandler<ClimbingComponent, RMCCombatModeInteractOverrideUserEvent>((object) this, __methodptr(OnCombatModeInteractOverride)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ClimbableComponent, CanDropTargetEvent>(new ComponentEventRefHandler<ClimbableComponent, CanDropTargetEvent>((object) this, __methodptr(OnCanDragDropOn)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ClimbableComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<ClimbableComponent, GetVerbsEvent<AlternativeVerb>>((object) this, __methodptr(AddClimbableVerb)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ComponentEventRefHandler<ClimbableComponent, InteractHandEvent> componentEventRefHandler = new ComponentEventRefHandler<ClimbableComponent, InteractHandEvent>((object) this, __methodptr(OnClimbableInteractHand));
    Type[] typeArray1 = new Type[1]
    {
      typeof (SharedBuckleSystem)
    };
    Type[] typeArray2 = new Type[1]
    {
      typeof (InteractionPopupSystem)
    };
    Type[] typeArray3 = typeArray1;
    ((EntitySystem) this).SubscribeLocalEvent<ClimbableComponent, InteractHandEvent>(componentEventRefHandler, typeArray2, typeArray3);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ClimbableComponent, DragDropTargetEvent>(new ComponentEventRefHandler<ClimbableComponent, DragDropTargetEvent>((object) this, __methodptr(OnClimbableDragDrop)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ClimbableComponent, StartCollideEvent>(new ComponentEventRefHandler<ClimbableComponent, StartCollideEvent>((object) this, __methodptr(OnClimbableStartCollide)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<GlassTableComponent, ClimbedOnEvent>(new ComponentEventRefHandler<GlassTableComponent, ClimbedOnEvent>((object) this, __methodptr(OnGlassClimbed)), (Type[]) null, (Type[]) null);
  }

  public virtual void UpdateBeforeSolve(bool prediction, float frameTime)
  {
    base.UpdateBeforeSolve(prediction, frameTime);
    EntityQueryEnumerator<ClimbingComponent> entityQueryEnumerator = ((EntitySystem) this).EntityQueryEnumerator<ClimbingComponent>();
    TimeSpan curTime = this._timing.CurTime;
    EntityUid uid;
    ClimbingComponent comp;
    while (entityQueryEnumerator.MoveNext(ref uid, ref comp))
    {
      if (comp.NextTransition.HasValue)
      {
        TimeSpan? nextTransition = comp.NextTransition;
        TimeSpan timeSpan = curTime;
        if ((nextTransition.HasValue ? (nextTransition.GetValueOrDefault() < timeSpan ? 1 : 0) : 0) != 0)
        {
          this.FinishTransition(uid, comp);
        }
        else
        {
          TransformComponent component = this._xformQuery.GetComponent(uid);
          this._xformSystem.SetLocalPosition(uid, component.LocalPosition + comp.Direction * frameTime, component);
        }
      }
    }
  }

  private void FinishTransition(EntityUid uid, ClimbingComponent comp)
  {
    comp.NextTransition = new TimeSpan?();
    this._actionBlockerSystem.UpdateCanMove(uid);
    ((EntitySystem) this).Dirty(uid, (IComponent) comp, (MetaDataComponent) null);
    FixturesComponent fixturesComp;
    if (this._fixturesQuery.TryGetComponent(uid, ref fixturesComp) && this.IsClimbing(uid, fixturesComp))
      return;
    this.StopClimb(uid, comp);
  }

  private bool IsClimbing(EntityUid uid, FixturesComponent? fixturesComp = null)
  {
    Fixture fixture;
    if (!this._fixturesQuery.Resolve(uid, ref fixturesComp, true) || !fixturesComp.Fixtures.TryGetValue("climb", out fixture))
      return false;
    foreach (Contact contact in fixture.Contacts.Values)
    {
      if (((EntitySystem) this).HasComp<ClimbableComponent>(EntityUid.op_Equality(uid, contact.EntityA) ? contact.EntityB : contact.EntityA))
        return true;
    }
    return false;
  }

  private void OnMoveAttempt(EntityUid uid, ClimbingComponent component, UpdateCanMoveEvent args)
  {
    if (!component.NextTransition.HasValue)
      return;
    args.Cancel();
  }

  private void OnParentChange(
    EntityUid uid,
    ClimbingComponent component,
    ref EntParentChangedMessage args)
  {
    if (!component.NextTransition.HasValue)
      return;
    this.FinishTransition(uid, component);
  }

  private void OnCanDragDropOn(
    EntityUid uid,
    ClimbableComponent component,
    ref CanDropTargetEvent args)
  {
    ClimbingComponent climbingComponent;
    if (args.Handled || !component.Vaultable || ((EntitySystem) this).TryComp<ClimbingComponent>(args.Dragged, ref climbingComponent) && climbingComponent.IsClimbing)
      return;
    string reason;
    bool flag = EntityUid.op_Equality(args.User, args.Dragged) ? this.CanVault(component, args.User, uid, out reason) : this.CanVault(component, args.User, args.Dragged, uid, out reason);
    args.CanDrop = flag;
    if (!((EntitySystem) this).HasComp<HandsComponent>(args.User))
      args.CanDrop = false;
    args.Handled = true;
  }

  private void AddClimbableVerb(
    EntityUid uid,
    ClimbableComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    ClimbingComponent climbingComponent;
    if (!args.CanAccess || !args.CanInteract || !this._actionBlockerSystem.CanMove(args.User) || !component.Vaultable || !((EntitySystem) this).TryComp<ClimbingComponent>(args.User, ref climbingComponent) || climbingComponent.IsClimbing || !climbingComponent.CanClimb)
      return;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Act = (Action) (() => this.TryClimb(args.User, args.User, args.Target, out DoAfterId? _, component));
    alternativeVerb.Text = ((EntitySystem) this).Loc.GetString("comp-climbable-verb-climb");
    verbs.Add(alternativeVerb);
  }

  private void OnCombatModeInteractOverride(
    EntityUid uid,
    ClimbingComponent component,
    ref RMCCombatModeInteractOverrideUserEvent args)
  {
    if (!args.Target.HasValue)
      return;
    ref EntityQuery<ClimbableComponent> local1 = ref this._climbableQuery;
    EntityUid? target = args.Target;
    EntityUid entityUid = target.Value;
    ClimbableComponent climbableComponent1;
    ref ClimbableComponent local2 = ref climbableComponent1;
    if (!local1.TryComp(entityUid, ref local2))
      return;
    EntityUid user = uid;
    target = args.Target;
    EntityUid climbable = target.Value;
    ClimbableComponent climbableComponent2 = climbableComponent1;
    ClimbingComponent climbingComponent = component;
    if (!this.CanTryManualClimb(user, climbable, climbableComponent2, climbingComponent, true))
      return;
    args.CanInteract = true;
    args.Handled = true;
  }

  private void OnClimbableInteractHand(
    EntityUid uid,
    ClimbableComponent component,
    ref InteractHandEvent args)
  {
    if (args.Handled || !this.CanTryManualClimb(args.User, uid, component))
      return;
    args.Handled = this.TryClimb(args.User, args.User, uid, out DoAfterId? _, component);
  }

  private void OnClimbableDragDrop(
    EntityUid uid,
    ClimbableComponent component,
    ref DragDropTargetEvent args)
  {
    if (args.Handled)
      return;
    this.TryClimb(args.User, args.Dragged, uid, out DoAfterId? _, component);
  }

  private void OnClimbableStartCollide(
    EntityUid uid,
    ClimbableComponent component,
    ref StartCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    ActorComponent actorComponent;
    ClimbingComponent climbing;
    CombatModeComponent combatModeComponent;
    PullerComponent pullerComponent;
    PullableComponent pullableComponent;
    InputMoverComponent inputMoverComponent;
    if (EntityUid.op_Equality(otherEntity, uid) || !((EntitySystem) this).TryComp<ActorComponent>(otherEntity, ref actorComponent) || !this._netConfig.GetClientCVar<bool>(actorComponent.PlayerSession.Channel, CCVars.PubgAutoClimbEnabled) || !((EntitySystem) this).TryComp<ClimbingComponent>(otherEntity, ref climbing) || !climbing.CanClimb || climbing.IsClimbing || climbing.DoAfter.HasValue || ((EntitySystem) this).TryComp<CombatModeComponent>(otherEntity, ref combatModeComponent) && combatModeComponent.IsInCombatMode || ((EntitySystem) this).TryComp<PullerComponent>(otherEntity, ref pullerComponent) && pullerComponent.Pulling.HasValue || ((EntitySystem) this).TryComp<PullableComponent>(otherEntity, ref pullableComponent) && pullableComponent.BeingPulled || !((EntitySystem) this).TryComp<InputMoverComponent>(otherEntity, ref inputMoverComponent) || !inputMoverComponent.HasDirectionalMovement)
      return;
    Vector2 wishDir = inputMoverComponent.WishDir;
    if ((double) wishDir.LengthSquared() < 1.0 / 1000.0)
      return;
    MapCoordinates mapCoordinates = this._xformSystem.GetMapCoordinates(otherEntity, (TransformComponent) null);
    Vector2 vector2 = this._xformSystem.GetMapCoordinates(uid, (TransformComponent) null).Position - mapCoordinates.Position;
    if ((double) vector2.LengthSquared() < 9.9999997473787516E-05 || (double) Vector2.Dot(Vector2.Normalize(wishDir), Vector2.Normalize(vector2)) < 0.60000002384185791 || !this.CanVault(component, otherEntity, uid, out string _) || !this._rmcMovement.CanClimbOver(new EntityUid?(otherEntity), otherEntity, uid, popup: false))
      return;
    this.TryClimb(otherEntity, otherEntity, uid, out DoAfterId? _, component, climbing);
  }

  private bool CanTryManualClimb(
    EntityUid user,
    EntityUid climbable,
    ClimbableComponent? climbableComponent = null,
    ClimbingComponent? climbingComponent = null,
    bool ignoreCombatMode = false)
  {
    CombatModeComponent combatModeComponent;
    PullerComponent pullerComponent;
    PullableComponent pullableComponent;
    return ((EntitySystem) this).Resolve<ClimbableComponent>(climbable, ref climbableComponent, false) && ((EntitySystem) this).Resolve<ClimbingComponent>(user, ref climbingComponent, false) && climbableComponent.Vaultable && !climbingComponent.IsClimbing && !climbingComponent.DoAfter.HasValue && climbingComponent.CanClimb && this._actionBlockerSystem.CanMove(user) && !this._interactionSystem.TryGetUsedEntity(user, out EntityUid? _, false) && (ignoreCombatMode || !((EntitySystem) this).TryComp<CombatModeComponent>(user, ref combatModeComponent) || !combatModeComponent.IsInCombatMode) && (!((EntitySystem) this).TryComp<PullerComponent>(user, ref pullerComponent) || !pullerComponent.Pulling.HasValue) && (!((EntitySystem) this).TryComp<PullableComponent>(user, ref pullableComponent) || !pullableComponent.BeingPulled);
  }

  public bool TryClimb(
    EntityUid user,
    EntityUid entityToMove,
    EntityUid climbable,
    out DoAfterId? id,
    ClimbableComponent? comp = null,
    ClimbingComponent? climbing = null)
  {
    id = new DoAfterId?();
    if (!((EntitySystem) this).Resolve<ClimbableComponent>(climbable, ref comp, true) || !((EntitySystem) this).Resolve<ClimbingComponent>(entityToMove, ref climbing, false))
      return false;
    string reason;
    if ((EntityUid.op_Equality(user, entityToMove) ? (this.CanVault(comp, user, climbable, out reason) ? 1 : 0) : (this.CanVault(comp, user, entityToMove, climbable, out reason) ? 1 : 0)) == 0)
    {
      this._popupSystem.PopupClient(reason, user, new EntityUid?(user));
      return false;
    }
    if (climbing.IsClimbing)
      return true;
    if (!this._rmcMovement.CanClimbOver(new EntityUid?(user), entityToMove, climbable))
      return false;
    DoAfterArgs args = new DoAfterArgs((IEntityManager) ((EntitySystem) this).EntityManager, user, comp.ClimbDelay, (DoAfterEvent) new ClimbSystem.ClimbDoAfterEvent(), new EntityUid?(entityToMove), new EntityUid?(climbable), new EntityUid?(entityToMove))
    {
      BreakOnMove = true,
      DuplicateCondition = DuplicateConditions.SameTool | DuplicateConditions.SameTarget
    };
    this._audio.PlayPredicted(comp.StartClimbSound, climbable, new EntityUid?(user), new AudioParams?());
    int num = this._doAfterSystem.TryStartDoAfter(args, out id) ? 1 : 0;
    if (num == 0)
      return num != 0;
    climbing.DoAfter = id;
    return num != 0;
  }

  private void OnDoAfter(
    EntityUid uid,
    ClimbingComponent component,
    ClimbSystem.ClimbDoAfterEvent args)
  {
    component.DoAfter = new DoAfterId?();
    if (args.Handled || args.Cancelled || !args.Args.Target.HasValue || !args.Args.Used.HasValue)
      return;
    if (this._containers.IsEntityInContainer(uid, (MetaDataComponent) null))
    {
      args.Handled = true;
    }
    else
    {
      this.Climb(uid, args.Args.User, args.Args.Target.Value, climbing: component);
      args.Handled = true;
    }
  }

  public void Climb(
    EntityUid uid,
    EntityUid user,
    EntityUid climbable,
    bool silent = false,
    ClimbingComponent? climbing = null,
    PhysicsComponent? physics = null,
    FixturesComponent? fixtures = null,
    ClimbableComponent? comp = null)
  {
    if (!((EntitySystem) this).Resolve<ClimbingComponent, PhysicsComponent, FixturesComponent>(uid, ref climbing, ref physics, ref fixtures, false) || !((EntitySystem) this).Resolve<ClimbableComponent>(climbable, ref comp, false))
      return;
    SelfBeforeClimbEvent beforeClimbEvent1 = new SelfBeforeClimbEvent(uid, user, Entity<ClimbableComponent>.op_Implicit((climbable, comp)));
    ((EntitySystem) this).RaiseLocalEvent<SelfBeforeClimbEvent>(uid, beforeClimbEvent1, false);
    if (beforeClimbEvent1.Cancelled)
      return;
    TargetBeforeClimbEvent beforeClimbEvent2 = new TargetBeforeClimbEvent(uid, user, Entity<ClimbableComponent>.op_Implicit((climbable, comp)));
    ((EntitySystem) this).RaiseLocalEvent<TargetBeforeClimbEvent>(climbable, beforeClimbEvent2, false);
    if (beforeClimbEvent2.Cancelled || !this.ReplaceFixtures(uid, climbing, fixtures))
      return;
    TransformComponent component = this._xformQuery.GetComponent(uid);
    (Vector2 vector2_1, Angle angle1) = this._xformSystem.GetWorldPositionRotation(component);
    Vector2 vector2_2 = this._xformSystem.GetWorldPosition(climbable) - vector2_1;
    float num = vector2_2.Length();
    Angle localRotation = component.LocalRotation;
    Angle angle2 = Angle.op_UnaryNegation(Angle.op_Subtraction(angle1, localRotation));
    Vector2 vector2_3 = ((Angle) ref angle2).RotateVec(ref vector2_2);
    if ((double) vector2_3.LengthSquared() < 0.0099999997764825821)
    {
      climbing.NextTransition = new TimeSpan?();
    }
    else
    {
      TimeSpan timeSpan = TimeSpan.FromSeconds(((double) num + (double) comp.VaultPastDistance) / (double) climbing.TransitionRate);
      climbing.NextTransition = new TimeSpan?(this._timing.CurTime + timeSpan);
      climbing.Direction = Vector2Helpers.Normalized(vector2_3) * climbing.TransitionRate;
      this._actionBlockerSystem.UpdateCanMove(uid);
    }
    climbing.IsClimbing = true;
    ((EntitySystem) this).Dirty(uid, (IComponent) climbing, (MetaDataComponent) null);
    this._audio.PlayPredicted(comp.FinishClimbSound, climbable, new EntityUid?(user), new AudioParams?());
    StartClimbEvent startClimbEvent = new StartClimbEvent(climbable);
    ClimbedOnEvent climbedOnEvent = new ClimbedOnEvent(uid, user);
    ((EntitySystem) this).RaiseLocalEvent<StartClimbEvent>(uid, ref startClimbEvent, false);
    ((EntitySystem) this).RaiseLocalEvent<ClimbedOnEvent>(climbable, ref climbedOnEvent, false);
    if (silent)
      return;
    string othersMessage;
    string recipientMessage;
    if (EntityUid.op_Equality(user, uid))
    {
      othersMessage = ((EntitySystem) this).Loc.GetString("comp-climbable-user-climbs-other", (nameof (user), (object) Identity.Entity(uid, (IEntityManager) ((EntitySystem) this).EntityManager)), (nameof (climbable), (object) climbable));
      recipientMessage = ((EntitySystem) this).Loc.GetString("comp-climbable-user-climbs", (nameof (climbable), (object) climbable));
    }
    else
    {
      othersMessage = ((EntitySystem) this).Loc.GetString("comp-climbable-user-climbs-force-other", new (string, object)[3]
      {
        (nameof (user), (object) Identity.Entity(user, (IEntityManager) ((EntitySystem) this).EntityManager)),
        ("moved-user", (object) Identity.Entity(uid, (IEntityManager) ((EntitySystem) this).EntityManager)),
        (nameof (climbable), (object) climbable)
      });
      recipientMessage = ((EntitySystem) this).Loc.GetString("comp-climbable-user-climbs-force", ("moved-user", (object) Identity.Entity(uid, (IEntityManager) ((EntitySystem) this).EntityManager)), (nameof (climbable), (object) climbable));
    }
    this._popupSystem.PopupPredicted(recipientMessage, othersMessage, uid, new EntityUid?(user));
  }

  private bool ReplaceFixtures(
    EntityUid uid,
    ClimbingComponent climbingComp,
    FixturesComponent fixturesComp)
  {
    foreach ((string key, Fixture fixture) in fixturesComp.Fixtures)
    {
      if (!climbingComp.DisabledFixtureMasks.ContainsKey(key) && fixture.Hard && (fixture.CollisionMask & 67108884 /*0x04000014*/) != 0)
      {
        climbingComp.DisabledFixtureMasks.Add(key, fixture.CollisionMask & 67108884 /*0x04000014*/);
        this._physics.SetCollisionMask(uid, key, fixture, fixture.CollisionMask & -67108885, fixturesComp, (PhysicsComponent) null);
      }
    }
    return this._fixtureSystem.TryCreateFixture(uid, (IPhysShape) new PhysShapeCircle(0.35f), "climb", 1f, false, 0, 67108884 /*0x04000014*/, 0.4f, 0.0f, true, fixturesComp, (PhysicsComponent) null, (TransformComponent) null);
  }

  private void OnClimbEndCollide(
    EntityUid uid,
    ClimbingComponent component,
    ref EndCollideEvent args)
  {
    if (args.OurFixtureId != "climb" || !component.IsClimbing || component.NextTransition.HasValue)
      return;
    foreach (Contact contact in args.OurFixture.Contacts.Values)
    {
      if (contact.IsTouching)
      {
        EntityUid entityUid = contact.OtherEnt(uid);
        (string str, Fixture fixture) = contact.OtherFixture(uid);
        if ((!EntityUid.op_Equality(args.OtherEntity, entityUid) || !(args.OtherFixtureId == str)) && fixture != null && fixture.Hard && this._climbableQuery.HasComp(entityUid))
          return;
      }
    }
    foreach (Fixture key in args.OurFixture.Contacts.Keys)
    {
      if (key != args.OtherFixture && ((EntitySystem) this).HasComp<ClimbableComponent>(key.Owner))
        return;
    }
    this.StopClimb(uid, component);
  }

  private void StopClimb(EntityUid uid, ClimbingComponent? climbing = null, FixturesComponent? fixtures = null)
  {
    if (!((EntitySystem) this).Resolve<ClimbingComponent, FixturesComponent>(uid, ref climbing, ref fixtures, false))
      return;
    foreach ((string key, int num) in climbing.DisabledFixtureMasks)
    {
      Fixture fixture;
      if (fixtures.Fixtures.TryGetValue(key, out fixture))
        this._physics.SetCollisionMask(uid, key, fixture, fixture.CollisionMask | num, fixtures, (PhysicsComponent) null);
    }
    climbing.DisabledFixtureMasks.Clear();
    this._fixtureSystem.DestroyFixture(uid, "climb", true, (PhysicsComponent) null, fixtures, (TransformComponent) null);
    climbing.IsClimbing = false;
    climbing.NextTransition = new TimeSpan?();
    EndClimbEvent endClimbEvent = new EndClimbEvent();
    ((EntitySystem) this).RaiseLocalEvent<EndClimbEvent>(uid, ref endClimbEvent, false);
    ((EntitySystem) this).Dirty(uid, (IComponent) climbing, (MetaDataComponent) null);
  }

  public bool CanVault(
    ClimbableComponent component,
    EntityUid user,
    EntityUid target,
    out string reason)
  {
    if (!component.Vaultable)
    {
      reason = string.Empty;
      return false;
    }
    if (!this._actionBlockerSystem.CanInteract(user, new EntityUid?(target)))
    {
      reason = ((EntitySystem) this).Loc.GetString("comp-climbable-cant-interact");
      return false;
    }
    ClimbingComponent climbingComponent;
    if (!((EntitySystem) this).TryComp<ClimbingComponent>(user, ref climbingComponent) || !climbingComponent.CanClimb)
    {
      reason = ((EntitySystem) this).Loc.GetString("comp-climbable-cant-climb");
      return false;
    }
    if (!this._interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target), component.Range))
    {
      reason = ((EntitySystem) this).Loc.GetString("comp-climbable-cant-reach");
      return false;
    }
    if (this._containers.IsEntityInContainer(user, (MetaDataComponent) null))
    {
      reason = ((EntitySystem) this).Loc.GetString("comp-climbable-cant-reach");
      return false;
    }
    reason = string.Empty;
    return true;
  }

  public bool CanVault(
    ClimbableComponent component,
    EntityUid user,
    EntityUid dragged,
    EntityUid target,
    out string reason)
  {
    if (!this._actionBlockerSystem.CanInteract(user, new EntityUid?(dragged)) || !this._actionBlockerSystem.CanInteract(user, new EntityUid?(target)))
    {
      reason = ((EntitySystem) this).Loc.GetString("comp-climbable-cant-interact");
      return false;
    }
    if (!((EntitySystem) this).HasComp<ClimbingComponent>(dragged))
    {
      reason = ((EntitySystem) this).Loc.GetString("comp-climbable-target-cant-climb", ("moved-user", (object) Identity.Entity(dragged, (IEntityManager) ((EntitySystem) this).EntityManager)));
      return false;
    }
    if (!this._interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target), component.Range, predicate: new SharedInteractionSystem.Ignored(Ignored)) || !this._interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(dragged), component.Range, predicate: new SharedInteractionSystem.Ignored(Ignored)))
    {
      reason = ((EntitySystem) this).Loc.GetString("comp-climbable-cant-reach");
      return false;
    }
    if (this._containers.IsEntityInContainer(user, (MetaDataComponent) null) || this._containers.IsEntityInContainer(dragged, (MetaDataComponent) null))
    {
      reason = ((EntitySystem) this).Loc.GetString("comp-climbable-cant-reach");
      return false;
    }
    reason = string.Empty;
    return true;

    bool Ignored(EntityUid entity)
    {
      return EntityUid.op_Equality(entity, target) || EntityUid.op_Equality(entity, user) || EntityUid.op_Equality(entity, dragged);
    }
  }

  public void ForciblySetClimbing(EntityUid uid, EntityUid climbable, ClimbingComponent? component = null)
  {
    this.Climb(uid, uid, climbable, true, component);
  }

  private void OnBuckled(EntityUid uid, ClimbingComponent component, ref BuckledEvent args)
  {
    this.StopOrCancelClimb(uid, component);
  }

  private void OnStored(
    EntityUid uid,
    ClimbingComponent component,
    ref EntGotInsertedIntoContainerMessage args)
  {
    this.StopOrCancelClimb(uid, component);
  }

  private void StopOrCancelClimb(EntityUid uid, ClimbingComponent component)
  {
    if (component.IsClimbing)
    {
      this.StopClimb(uid, component);
    }
    else
    {
      if (!component.DoAfter.HasValue)
        return;
      this._doAfterSystem.Cancel(component.DoAfter);
      component.DoAfter = new DoAfterId?();
    }
  }

  private void OnGlassClimbed(
    EntityUid uid,
    GlassTableComponent component,
    ref ClimbedOnEvent args)
  {
    PhysicsComponent physicsComponent;
    if (((EntitySystem) this).TryComp<PhysicsComponent>(args.Climber, ref physicsComponent) && (double) physicsComponent.Mass <= (double) component.MassLimit)
      return;
    this._damageableSystem.TryChangeDamage(new EntityUid?(args.Climber), component.ClimberDamage, origin: new EntityUid?(args.Climber));
    this._damageableSystem.TryChangeDamage(new EntityUid?(uid), component.TableDamage, origin: new EntityUid?(args.Climber));
    this._stunSystem.TryParalyze(args.Climber, TimeSpan.FromSeconds((double) component.StunTime), true);
    this._popupSystem.PopupEntity(((EntitySystem) this).Loc.GetString("glass-table-shattered-others", ("table", (object) uid), ("climber", (object) Identity.Entity(args.Climber, (IEntityManager) ((EntitySystem) this).EntityManager))), args.Climber, Filter.PvsExcept(args.Climber, 2f, (IEntityManager) null), true);
  }

  [NetSerializable]
  [Serializable]
  private sealed class ClimbDoAfterEvent : 
    SimpleDoAfterEvent,
    ISerializationGenerated<ClimbSystem.ClimbDoAfterEvent>,
    ISerializationGenerated
  {
    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref ClimbSystem.ClimbDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (ClimbSystem.ClimbDoAfterEvent) target1;
      serialization.TryCustomCopy<ClimbSystem.ClimbDoAfterEvent>(this, ref target, hookCtx, false, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref ClimbSystem.ClimbDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref SimpleDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      ClimbSystem.ClimbDoAfterEvent target1 = (ClimbSystem.ClimbDoAfterEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (SimpleDoAfterEvent) target1;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      ClimbSystem.ClimbDoAfterEvent target1 = (ClimbSystem.ClimbDoAfterEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual ClimbSystem.ClimbDoAfterEvent SimpleDoAfterEvent.Instantiate()
    {
      return new ClimbSystem.ClimbDoAfterEvent();
    }
  }
}
