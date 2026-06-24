// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.SharedJointSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics.Systems;

public abstract class SharedJointSystem : EntitySystem
{
  [Dependency]
  private readonly SharedContainerSystem _container;
  [Dependency]
  private readonly SharedPhysicsSystem _physics;
  [Dependency]
  private readonly IGameTiming _gameTiming;
  [Dependency]
  private readonly SharedTransformSystem _transform;
  private Robust.Shared.GameObjects.EntityQuery<JointComponent> _jointsQuery;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;
  private Robust.Shared.GameObjects.EntityQuery<JointRelayTargetComponent> _relayQuery;
  private readonly HashSet<Entity<JointComponent>> _dirtyJoints = new HashSet<Entity<JointComponent>>();
  protected readonly HashSet<Joint> AddedJoints = new HashSet<Joint>();
  protected readonly List<Joint> ToRemove = new List<Joint>();

  public override void Initialize()
  {
    base.Initialize();
    this._jointsQuery = this.GetEntityQuery<JointComponent>();
    this._relayQuery = this.GetEntityQuery<JointRelayTargetComponent>();
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this.UpdatesOutsidePrediction = true;
    this.UpdatesBefore.Add(typeof (SharedPhysicsSystem));
    this.SubscribeLocalEvent<JointComponent, ComponentShutdown>(new ComponentEventHandler<JointComponent, ComponentShutdown>(this.OnJointShutdown));
    this.SubscribeLocalEvent<JointComponent, ComponentInit>(new ComponentEventHandler<JointComponent, ComponentInit>(this.OnJointInit));
    this.InitializeRelay();
  }

  private void OnJointInit(EntityUid uid, JointComponent component, ComponentInit args)
  {
    foreach ((string key, Joint joint) in component.Joints)
    {
      EntityUid uid1 = uid == joint.BodyAUid ? joint.BodyBUid : joint.BodyAUid;
      PhysicsComponent comp1;
      PhysicsComponent comp2;
      JointComponent comp3;
      if (this.TryComp<PhysicsComponent>(joint.BodyAUid, out comp1) && this.TryComp<PhysicsComponent>(joint.BodyBUid, out comp2) && this.TryComp<JointComponent>(uid1, out comp3))
      {
        if (!comp3.Joints.ContainsKey(key))
        {
          if (uid == joint.BodyAUid)
            this.InitJoint(joint, comp1, comp2, component, comp3, true);
          else
            this.InitJoint(joint, comp1, comp2, comp3, component, true);
        }
        else
        {
          this._physics.WakeBody(joint.BodyAUid, body: comp1);
          this._physics.WakeBody(joint.BodyBUid, body: comp2);
          JointAddedEvent jointAddedEvent = new JointAddedEvent(joint, joint.BodyAUid, joint.BodyBUid, comp1, comp2);
          this.RaiseLocalEvent<JointAddedEvent>(joint.BodyAUid, jointAddedEvent);
          JointAddedEvent args1 = new JointAddedEvent(joint, joint.BodyBUid, joint.BodyAUid, comp2, comp1);
          this.RaiseLocalEvent<JointAddedEvent>(joint.BodyBUid, args1);
          this.EntityManager.EventBus.RaiseEvent<JointAddedEvent>(EventSource.Local, jointAddedEvent);
        }
      }
    }
    this.RefreshRelay(uid, component);
  }

  private void OnJointShutdown(EntityUid uid, JointComponent component, ComponentShutdown args)
  {
    foreach (Joint joint in component.Joints.Values)
      this.RemoveJoint(joint);
    if (!component.Relay.HasValue || this.TerminatingOrDeleted(component.Relay.Value))
      return;
    this.SetRelay(uid, new EntityUid?(), component);
  }

  public void SetEnabled(Joint joint, bool value)
  {
    if (joint.Enabled == value)
      return;
    joint.Enabled = value;
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    foreach (Joint addedJoint in this.AddedJoints)
      this.InitJoint(addedJoint);
    this.AddedJoints.Clear();
    foreach (Entity<JointComponent> dirtyJoint in this._dirtyJoints)
    {
      if (!dirtyJoint.Comp.Deleted && dirtyJoint.Comp.JointCount == 0)
        this.RemComp<JointComponent>((EntityUid) dirtyJoint);
    }
    this._dirtyJoints.Clear();
  }

  private void InitJoint(
    Joint joint,
    PhysicsComponent? bodyA = null,
    PhysicsComponent? bodyB = null,
    JointComponent? jointComponentA = null,
    JointComponent? jointComponentB = null,
    bool ignoreExisting = false)
  {
    EntityUid bodyAuid = joint.BodyAUid;
    EntityUid bodyBuid = joint.BodyBUid;
    if (!this._physicsQuery.Resolve(bodyAuid, ref bodyA, false) || !this._physicsQuery.Resolve(bodyBuid, ref bodyB, false))
      return;
    if (jointComponentA == null)
      jointComponentA = this.EnsureComp<JointComponent>(bodyAuid);
    if (jointComponentB == null)
      jointComponentB = this.EnsureComp<JointComponent>(bodyBuid);
    Dictionary<string, Joint> joints1 = jointComponentA.Joints;
    Dictionary<string, Joint> joints2 = jointComponentB.Joints;
    if (this._gameTiming.IsFirstTimePredicted)
      this.Log.Debug("Initializing joint " + joint.ID);
    Joint joint1;
    if (!ignoreExisting && joints1.TryGetValue(joint.ID, out joint1))
    {
      if (joint1.BodyBUid != bodyBuid)
      {
        this.Log.Error($"While adding joint {joint.ID} to entity {this.ToPrettyString((Entity<MetaDataComponent>) bodyBuid)}, the connected entity {this.ToPrettyString((Entity<MetaDataComponent>) bodyAuid)} already had a joint with the same ID connected to another entity {this.ToPrettyString((Entity<MetaDataComponent>) joint1.BodyBUid)}.");
        return;
      }
      if (joints2.TryGetValue(joint.ID, out Joint _))
        return;
      this.Log.Error($"While adding joint {joint.ID} to entity {this.ToPrettyString((Entity<MetaDataComponent>) bodyBuid)}, the joint already existed for the connected entity {this.ToPrettyString((Entity<MetaDataComponent>) bodyAuid)}.");
    }
    else if (!ignoreExisting && joints2.TryGetValue(joint.ID, out joint1))
    {
      if (joint1.BodyAUid != bodyAuid)
      {
        this.Log.Error($"While adding joint {joint.ID} to entity {this.ToPrettyString((Entity<MetaDataComponent>) bodyAuid)}, the connected entity {this.ToPrettyString((Entity<MetaDataComponent>) bodyBuid)} already had a joint with the same ID connected to another entity {this.ToPrettyString((Entity<MetaDataComponent>) joint1.BodyAUid)}.");
        return;
      }
      this.Log.Error($"While adding joint {joint.ID} to entity {this.ToPrettyString((Entity<MetaDataComponent>) bodyAuid)}, the joint already existed for the connected entity {this.ToPrettyString((Entity<MetaDataComponent>) bodyBuid)}.");
    }
    joints1.TryAdd(joint.ID, joint);
    joints2.TryAdd(joint.ID, joint);
    if (!joint.CollideConnected)
      this.FilterContactsForJoint(joint, bodyA, bodyB);
    this._physics.WakeBody(bodyAuid, body: bodyA);
    this._physics.WakeBody(bodyBuid, body: bodyB);
    this.Dirty(bodyAuid, (IComponent) bodyA);
    this.Dirty(bodyBuid, (IComponent) bodyB);
    this.Dirty(bodyAuid, (IComponent) jointComponentA);
    this.Dirty(bodyBuid, (IComponent) jointComponentB);
    this._dirtyJoints.Add((Entity<JointComponent>) (bodyAuid, jointComponentA));
    this._dirtyJoints.Add((Entity<JointComponent>) (bodyBuid, jointComponentB));
    JointAddedEvent jointAddedEvent = new JointAddedEvent(joint, bodyAuid, bodyBuid, bodyA, bodyB);
    this.EntityManager.EventBus.RaiseLocalEvent<JointAddedEvent>(bodyAuid, jointAddedEvent);
    JointAddedEvent args = new JointAddedEvent(joint, bodyBuid, bodyAuid, bodyB, bodyA);
    this.EntityManager.EventBus.RaiseLocalEvent<JointAddedEvent>(bodyBuid, args);
    this.EntityManager.EventBus.RaiseEvent<JointAddedEvent>(EventSource.Local, jointAddedEvent);
  }

  private static string GetJointId(Joint joint)
  {
    string id = joint.ID;
    return string.IsNullOrEmpty(id) ? joint.GetHashCode().ToString() : id;
  }

  public DistanceJoint CreateDistanceJoint(
    EntityUid bodyA,
    EntityUid bodyB,
    Vector2? anchorA = null,
    Vector2? anchorB = null,
    string? id = null,
    TransformComponent? xformA = null,
    TransformComponent? xformB = null,
    float? minimumDistance = null)
  {
    if (!this.Resolve(bodyA, ref xformA) || !this.Resolve(bodyB, ref xformB))
      throw new InvalidOperationException();
    Vector2 valueOrDefault = anchorA.GetValueOrDefault();
    if (!anchorA.HasValue)
      anchorA = new Vector2?(Vector2.Zero);
    valueOrDefault = anchorB.GetValueOrDefault();
    if (!anchorB.HasValue)
      anchorB = new Vector2?(Vector2.Zero);
    float num = (Vector2.Transform(anchorA.Value, this._transform.GetWorldMatrix(xformA)) - Vector2.Transform(anchorB.Value, this._transform.GetWorldMatrix(xformB))).Length();
    if (minimumDistance.HasValue)
      num = Math.Max(minimumDistance.Value, num);
    DistanceJoint distanceJoint = new DistanceJoint(bodyA, bodyB, anchorA.Value, anchorB.Value, num);
    if (id == null)
      id = SharedJointSystem.GetJointId((Joint) distanceJoint);
    distanceJoint.ID = id;
    this.AddJoint((Joint) distanceJoint);
    return distanceJoint;
  }

  public MouseJoint CreateMouseJoint(
    EntityUid bodyA,
    EntityUid bodyB,
    Vector2? anchorA = null,
    Vector2? anchorB = null,
    string? id = null)
  {
    Vector2 valueOrDefault = anchorA.GetValueOrDefault();
    if (!anchorA.HasValue)
      anchorA = new Vector2?(Vector2.Zero);
    valueOrDefault = anchorB.GetValueOrDefault();
    if (!anchorB.HasValue)
      anchorB = new Vector2?(Vector2.Zero);
    MouseJoint mouseJoint = new MouseJoint(bodyA, bodyB, anchorA.Value, anchorB.Value);
    if (id == null)
      id = SharedJointSystem.GetJointId((Joint) mouseJoint);
    mouseJoint.ID = id;
    this.AddJoint((Joint) mouseJoint);
    return mouseJoint;
  }

  public PrismaticJoint CreatePrismaticJoint(EntityUid bodyA, EntityUid bodyB, string? id = null)
  {
    PrismaticJoint prismaticJoint = new PrismaticJoint(bodyA, bodyB);
    if (id == null)
      id = SharedJointSystem.GetJointId((Joint) prismaticJoint);
    prismaticJoint.ID = id;
    this.AddJoint((Joint) prismaticJoint);
    return prismaticJoint;
  }

  public PrismaticJoint CreatePrismaticJoint(
    EntityUid bodyA,
    EntityUid bodyB,
    Vector2 anchorA,
    Vector2 anchorB,
    Vector2 worldAxis,
    float referenceAngle,
    string? id = null)
  {
    Vector2 localVector2 = this.GetLocalVector2(bodyA, worldAxis);
    PrismaticJoint prismaticJoint = new PrismaticJoint(bodyA, bodyB, anchorA, anchorB, localVector2, referenceAngle);
    if (id == null)
      id = SharedJointSystem.GetJointId((Joint) prismaticJoint);
    prismaticJoint.ID = id;
    this.AddJoint((Joint) prismaticJoint);
    return prismaticJoint;
  }

  public RevoluteJoint CreateRevoluteJoint(EntityUid bodyA, EntityUid bodyB, string? id = null)
  {
    RevoluteJoint revoluteJoint = new RevoluteJoint(bodyA, bodyB);
    if (id == null)
      id = SharedJointSystem.GetJointId((Joint) revoluteJoint);
    revoluteJoint.ID = id;
    this.AddJoint((Joint) revoluteJoint);
    return revoluteJoint;
  }

  public WeldJoint GetOrCreateWeldJoint(EntityUid bodyA, EntityUid bodyB, string? id = null)
  {
    JointComponent component;
    Joint weldJoint1;
    if (id != null && this._jointsQuery.TryComp(bodyA, out component) && component.Joints.TryGetValue(id, out weldJoint1))
      return (WeldJoint) weldJoint1;
    WeldJoint weldJoint2 = new WeldJoint(bodyA, bodyB);
    if (id == null)
      id = SharedJointSystem.GetJointId((Joint) weldJoint2);
    weldJoint2.ID = id;
    this.AddJoint((Joint) weldJoint2);
    return weldJoint2;
  }

  public WeldJoint CreateWeldJoint(EntityUid bodyA, EntityUid bodyB, string? id = null)
  {
    WeldJoint weldJoint = new WeldJoint(bodyA, bodyB);
    if (id == null)
      id = SharedJointSystem.GetJointId((Joint) weldJoint);
    weldJoint.ID = id;
    this.AddJoint((Joint) weldJoint);
    return weldJoint;
  }

  private Vector2 GetLocalVector2(EntityUid uid, Vector2 worldVector, TransformComponent? xform = null)
  {
    return !this.Resolve(uid, ref xform) ? Vector2.Zero : Robust.Shared.Physics.Transform.MulT(new Quaternion2D((float) this._transform.GetWorldRotation(xform).Theta), worldVector);
  }

  public static void LinearStiffness(
    float frequencyHertz,
    float dampingRatio,
    float massA,
    float massB,
    out float stiffness,
    out float damping)
  {
    float num1 = (double) massA <= 0.0 || (double) massB <= 0.0 ? ((double) massA <= 0.0 ? massB : massA) : (float) ((double) massA * (double) massB / ((double) massA + (double) massB));
    float num2 = 6.28318548f * frequencyHertz;
    stiffness = num1 * num2 * num2;
    damping = 2f * num1 * dampingRatio * num2;
  }

  public static void AngularStiffness(
    float frequencyHertz,
    float dampingRatio,
    PhysicsComponent bodyA,
    PhysicsComponent bodyB,
    out float stiffness,
    out float damping)
  {
    float inertia1 = bodyA.Inertia;
    float inertia2 = bodyB.Inertia;
    float num1 = (double) inertia1 <= 0.0 || (double) inertia2 <= 0.0 ? ((double) inertia1 <= 0.0 ? inertia2 : inertia1) : (float) ((double) inertia1 * (double) inertia2 / ((double) inertia1 + (double) inertia2));
    float num2 = 6.28318548f * frequencyHertz;
    stiffness = num1 * num2 * num2;
    damping = 2f * num1 * dampingRatio * num2;
  }

  protected void AddJoint(Joint joint, PhysicsComponent? bodyA = null, PhysicsComponent? bodyB = null)
  {
    if (!this._physicsQuery.Resolve(joint.BodyAUid, ref bodyA) || !this._physicsQuery.Resolve(joint.BodyBUid, ref bodyB))
      return;
    if (!joint.CollideConnected)
      this.FilterContactsForJoint(joint, bodyA, bodyB);
    MapId mapId = this.Transform(joint.BodyAUid).MapID;
    if (mapId == MapId.Nullspace || mapId != this.Transform(joint.BodyBUid).MapID)
      this.Log.Error("Tried to add joint to ineligible bodies");
    else if (string.IsNullOrEmpty(joint.ID))
    {
      this.Log.Error("Can't add a joint with no ID");
    }
    else
    {
      this.InitJoint(joint, bodyA, bodyB);
      if (!this._gameTiming.IsFirstTimePredicted)
        return;
      this.Log.Debug($"Added {joint.JointType} Joint with ID {joint.ID} from {bodyA.BodyType} to {bodyB.BodyType} ");
    }
  }

  public void RecursiveClearJoints(
    EntityUid uid,
    TransformComponent? xform = null,
    JointComponent? component = null,
    JointRelayTargetComponent? relay = null)
  {
    if (!this.Resolve(uid, ref xform))
      return;
    this._jointsQuery.Resolve(uid, ref component, false);
    this._relayQuery.Resolve(uid, ref relay, false);
    if (relay != null)
    {
      foreach (EntityUid uid1 in relay.Relayed)
      {
        JointComponent component1;
        this._jointsQuery.TryGetComponent(uid1, out component1);
        this.ClearJoints(uid1, component1);
      }
      this.RemComp(uid, (IComponent) relay);
    }
    if (component == null)
      return;
    this.ClearJoints(uid, component);
  }

  public void ClearJoints(EntityUid uid, JointComponent? component = null)
  {
    if (!this._jointsQuery.Resolve(uid, ref component, false))
      return;
    foreach (Joint joint in component.Joints.Values.ToArray<Joint>())
      this.RemoveJoint(joint);
    foreach (Joint addedJoint in this.AddedJoints)
    {
      if (addedJoint.BodyAUid == uid || addedJoint.BodyBUid == uid)
        this.ToRemove.Add(addedJoint);
    }
    this.AddedJoints.ExceptWith((IEnumerable<Joint>) this.ToRemove);
    this.ToRemove.Clear();
    if (!this._gameTiming.IsFirstTimePredicted)
      return;
    this.Log.Debug($"Removed all joints from entity {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
  }

  public void RemoveJoint(EntityUid uid, string id)
  {
    JointComponent component;
    Joint joint;
    if (!this._jointsQuery.TryComp(uid, out component) || !component.Joints.TryGetValue(id, out joint))
      return;
    this.RemoveJoint(joint);
  }

  public void RemoveJoint(Joint joint)
  {
    this.AddedJoints.Remove(joint);
    EntityUid bodyAuid = joint.BodyAUid;
    EntityUid bodyBuid = joint.BodyBUid;
    JointComponent component1;
    JointComponent component2;
    if (!this._jointsQuery.TryComp(bodyAuid, out component1) || !this._jointsQuery.TryComp(bodyBuid, out component2) || !component1.Joints.Remove(joint.ID) || !component2.Joints.Remove(joint.ID))
      return;
    PhysicsComponent component3;
    if (this._physicsQuery.TryComp(bodyAuid, out component3) && this.MetaData(bodyAuid).EntityLifeStage < EntityLifeStage.Terminating)
      this._physics.WakeBody(component1.Relay ?? bodyAuid);
    PhysicsComponent comp;
    if (this.TryComp<PhysicsComponent>(bodyBuid, out comp) && this.MetaData(bodyBuid).EntityLifeStage < EntityLifeStage.Terminating)
      this._physics.WakeBody(component2.Relay ?? bodyBuid);
    if (!component1.Deleted)
      this.Dirty(bodyAuid, (IComponent) component1);
    if (!component2.Deleted)
      this.Dirty(bodyBuid, (IComponent) component2);
    if (component1.Deleted && component2.Deleted)
      return;
    if (!joint.CollideConnected)
      this.FilterContactsForJoint(joint);
    if (component3 == null)
      this.Log.Debug($"Tried to remove joint from entity {this.ToPrettyString((Entity<MetaDataComponent>) bodyAuid)} without a physics component");
    else if (comp == null)
    {
      this.Log.Debug($"Tried to remove joint from entity {this.ToPrettyString((Entity<MetaDataComponent>) bodyBuid)} without a physics component");
    }
    else
    {
      JointRemovedEvent jointRemovedEvent = new JointRemovedEvent(joint, bodyAuid, bodyBuid, component3, comp);
      this.EntityManager.EventBus.RaiseLocalEvent<JointRemovedEvent>(bodyAuid, jointRemovedEvent);
      JointRemovedEvent args = new JointRemovedEvent(joint, bodyBuid, bodyAuid, comp, component3);
      this.EntityManager.EventBus.RaiseLocalEvent<JointRemovedEvent>(bodyBuid, args);
      this.EntityManager.EventBus.RaiseEvent<JointRemovedEvent>(EventSource.Local, jointRemovedEvent);
      if (this._gameTiming.IsFirstTimePredicted)
        this.Log.Debug($"Removed {joint.JointType} joint with ID {joint.ID} from entity {this.ToPrettyString((Entity<MetaDataComponent>) bodyAuid)} to entity {this.ToPrettyString((Entity<MetaDataComponent>) bodyBuid)}");
    }
    this._dirtyJoints.Add((Entity<JointComponent>) (bodyAuid, component1));
    this._dirtyJoints.Add((Entity<JointComponent>) (bodyBuid, component2));
  }

  internal void FilterContactsForJoint(Joint joint, PhysicsComponent? bodyA = null, PhysicsComponent? bodyB = null)
  {
    if (!this._physicsQuery.Resolve(joint.BodyBUid, ref bodyB))
      return;
    LinkedListNode<Contact> linkedListNode = bodyB.Contacts.First;
    while (linkedListNode != null)
    {
      Contact contact = linkedListNode.Value;
      linkedListNode = linkedListNode.Next;
      if (contact.EntityA == joint.BodyAUid || contact.EntityB == joint.BodyAUid)
        contact.Flags |= ContactFlags.Filter;
    }
  }

  private void InitializeRelay()
  {
    this.SubscribeLocalEvent<JointRelayTargetComponent, ComponentShutdown>(new ComponentEventHandler<JointRelayTargetComponent, ComponentShutdown>(this.OnRelayShutdown));
    this.SubscribeLocalEvent<JointRelayTargetComponent, ComponentGetState>(new ComponentEventRefHandler<JointRelayTargetComponent, ComponentGetState>(this.OnRelayGetState));
    this.SubscribeLocalEvent<JointRelayTargetComponent, ComponentHandleState>(new ComponentEventRefHandler<JointRelayTargetComponent, ComponentHandleState>(this.OnRelayHandleState));
  }

  private void OnRelayGetState(
    EntityUid uid,
    JointRelayTargetComponent component,
    ref ComponentGetState args)
  {
    args.State = (IComponentState) new SharedJointSystem.JointRelayComponentState(this.GetNetEntitySet(component.Relayed));
  }

  private void OnRelayHandleState(
    EntityUid uid,
    JointRelayTargetComponent component,
    ref ComponentHandleState args)
  {
    if (!(args.Current is SharedJointSystem.JointRelayComponentState current))
      return;
    this.EnsureEntitySet<JointRelayTargetComponent>(current.Entities, uid, component.Relayed);
  }

  private void OnRelayShutdown(
    EntityUid uid,
    JointRelayTargetComponent component,
    ComponentShutdown args)
  {
    if (this._gameTiming.ApplyingState)
      return;
    foreach (EntityUid uid1 in component.Relayed)
    {
      JointComponent component1;
      if (!this.TerminatingOrDeleted(uid1) && this._jointsQuery.TryGetComponent(uid1, out component1))
        this.RefreshRelay(uid1, component1);
    }
  }

  public void RefreshRelay(EntityUid uid, JointComponent? component = null)
  {
    if (!this.Resolve<JointComponent>(uid, ref component, false))
      return;
    EntityUid? relay1 = new EntityUid?();
    BaseContainer container;
    if (this._container.TryGetOuterContainer(uid, this.Transform(uid), out container))
    {
      relay1 = new EntityUid?(container.Owner);
      foreach (Joint joint in component.Joints.Values)
      {
        EntityUid other = joint.GetOther(uid);
        EntityUid? nullable = relay1;
        if ((nullable.HasValue ? (other == nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        {
          EntityUid uid1 = uid;
          nullable = new EntityUid?();
          EntityUid? relay2 = nullable;
          JointComponent component1 = component;
          this.SetRelay(uid1, relay2, component1);
          return;
        }
      }
    }
    this.SetRelay(uid, relay1, component);
  }

  public void SetRelay(EntityUid uid, EntityUid? relay, JointComponent? component = null)
  {
    if (!this.Resolve<JointComponent>(uid, ref component, false))
      return;
    EntityUid? relay1 = component.Relay;
    EntityUid? nullable = relay;
    if ((relay1.HasValue == nullable.HasValue ? (relay1.HasValue ? (relay1.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      return;
    JointRelayTargetComponent comp;
    if (this.TryComp<JointRelayTargetComponent>(component.Relay, out comp) && comp.Relayed.Remove(uid))
    {
      if (comp.Relayed.Count == 0)
        this.RemCompDeferred<JointRelayTargetComponent>(component.Relay.Value);
      this.Dirty(component.Relay.Value, (IComponent) comp);
    }
    component.Relay = relay;
    if (relay.HasValue)
    {
      JointRelayTargetComponent relayTargetComponent = this.EnsureComp<JointRelayTargetComponent>(relay.Value);
      if (relayTargetComponent.Relayed.Add(uid))
      {
        this._physics.WakeBody(relay.Value);
        this.Dirty(relay.Value, (IComponent) relayTargetComponent);
      }
    }
    this.Dirty(uid, (IComponent) component);
  }

  [NetSerializable]
  [Serializable]
  private sealed class JointRelayComponentState : ComponentState
  {
    public HashSet<NetEntity> Entities;

    public JointRelayComponentState(HashSet<NetEntity> entities) => this.Entities = entities;
  }
}
