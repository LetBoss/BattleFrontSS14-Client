using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

namespace Robust.Shared.Physics.Systems;

public abstract class SharedJointSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	private sealed class JointRelayComponentState : ComponentState
	{
		public HashSet<NetEntity> Entities;

		public JointRelayComponentState(HashSet<NetEntity> entities)
		{
			Entities = entities;
		}
	}

	[Dependency]
	private readonly SharedContainerSystem _container;

	[Dependency]
	private readonly SharedPhysicsSystem _physics;

	[Dependency]
	private readonly IGameTiming _gameTiming;

	[Dependency]
	private readonly SharedTransformSystem _transform;

	private EntityQuery<JointComponent> _jointsQuery;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	private EntityQuery<JointRelayTargetComponent> _relayQuery;

	private readonly HashSet<Entity<JointComponent>> _dirtyJoints = new HashSet<Entity<JointComponent>>();

	protected readonly HashSet<Joint> AddedJoints = new HashSet<Joint>();

	protected readonly List<Joint> ToRemove = new List<Joint>();

	public override void Initialize()
	{
		base.Initialize();
		_jointsQuery = GetEntityQuery<JointComponent>();
		_relayQuery = GetEntityQuery<JointRelayTargetComponent>();
		_physicsQuery = GetEntityQuery<PhysicsComponent>();
		base.UpdatesOutsidePrediction = true;
		base.UpdatesBefore.Add(typeof(SharedPhysicsSystem));
		SubscribeLocalEvent<JointComponent, ComponentShutdown>(OnJointShutdown);
		SubscribeLocalEvent<JointComponent, ComponentInit>(OnJointInit);
		InitializeRelay();
	}

	private void OnJointInit(EntityUid uid, JointComponent component, ComponentInit args)
	{
		foreach (KeyValuePair<string, Joint> joint2 in component.Joints)
		{
			joint2.Deconstruct(out var key, out var value);
			string key2 = key;
			Joint joint = value;
			EntityUid uid2 = ((uid == joint.BodyAUid) ? joint.BodyBUid : joint.BodyAUid);
			if (!TryComp(joint.BodyAUid, out PhysicsComponent comp) || !TryComp(joint.BodyBUid, out PhysicsComponent comp2) || !TryComp(uid2, out JointComponent comp3))
			{
				continue;
			}
			if (!comp3.Joints.ContainsKey(key2))
			{
				if (uid == joint.BodyAUid)
				{
					InitJoint(joint, comp, comp2, component, comp3, ignoreExisting: true);
				}
				else
				{
					InitJoint(joint, comp, comp2, comp3, component, ignoreExisting: true);
				}
				continue;
			}
			_physics.WakeBody(joint.BodyAUid, force: false, null, comp);
			_physics.WakeBody(joint.BodyBUid, force: false, null, comp2);
			JointAddedEvent jointAddedEvent = new JointAddedEvent(joint, joint.BodyAUid, joint.BodyBUid, comp, comp2);
			RaiseLocalEvent(joint.BodyAUid, jointAddedEvent);
			JointAddedEvent args2 = new JointAddedEvent(joint, joint.BodyBUid, joint.BodyAUid, comp2, comp);
			RaiseLocalEvent(joint.BodyBUid, args2);
			EntityManager.EventBus.RaiseEvent(EventSource.Local, jointAddedEvent);
		}
		RefreshRelay(uid, component);
	}

	private void OnJointShutdown(EntityUid uid, JointComponent component, ComponentShutdown args)
	{
		foreach (Joint value in component.Joints.Values)
		{
			RemoveJoint(value);
		}
		if (component.Relay.HasValue && !TerminatingOrDeleted(component.Relay.Value))
		{
			SetRelay(uid, null, component);
		}
	}

	public void SetEnabled(Joint joint, bool value)
	{
		if (joint.Enabled != value)
		{
			joint.Enabled = value;
		}
	}

	public override void Update(float frameTime)
	{
		base.Update(frameTime);
		foreach (Joint addedJoint in AddedJoints)
		{
			InitJoint(addedJoint);
		}
		AddedJoints.Clear();
		foreach (Entity<JointComponent> dirtyJoint in _dirtyJoints)
		{
			if (!dirtyJoint.Comp.Deleted && dirtyJoint.Comp.JointCount == 0)
			{
				RemComp<JointComponent>(dirtyJoint);
			}
		}
		_dirtyJoints.Clear();
	}

	private void InitJoint(Joint joint, PhysicsComponent? bodyA = null, PhysicsComponent? bodyB = null, JointComponent? jointComponentA = null, JointComponent? jointComponentB = null, bool ignoreExisting = false)
	{
		EntityUid bodyAUid = joint.BodyAUid;
		EntityUid bodyBUid = joint.BodyBUid;
		if (!_physicsQuery.Resolve(bodyAUid, ref bodyA, logMissing: false) || !_physicsQuery.Resolve(bodyBUid, ref bodyB, logMissing: false))
		{
			return;
		}
		if (jointComponentA == null)
		{
			jointComponentA = EnsureComp<JointComponent>(bodyAUid);
		}
		if (jointComponentB == null)
		{
			jointComponentB = EnsureComp<JointComponent>(bodyBUid);
		}
		Dictionary<string, Joint> joints = jointComponentA.Joints;
		Dictionary<string, Joint> joints2 = jointComponentB.Joints;
		if (_gameTiming.IsFirstTimePredicted)
		{
			base.Log.Debug("Initializing joint " + joint.ID);
		}
		if (!ignoreExisting && joints.TryGetValue(joint.ID, out var value))
		{
			if (value.BodyBUid != bodyBUid)
			{
				base.Log.Error($"While adding joint {joint.ID} to entity {ToPrettyString(bodyBUid)}, the connected entity {ToPrettyString(bodyAUid)} already had a joint with the same ID connected to another entity {ToPrettyString(value.BodyBUid)}.");
				return;
			}
			if (joints2.TryGetValue(joint.ID, out var _))
			{
				return;
			}
			base.Log.Error($"While adding joint {joint.ID} to entity {ToPrettyString(bodyBUid)}, the joint already existed for the connected entity {ToPrettyString(bodyAUid)}.");
		}
		else if (!ignoreExisting && joints2.TryGetValue(joint.ID, out value))
		{
			if (value.BodyAUid != bodyAUid)
			{
				base.Log.Error($"While adding joint {joint.ID} to entity {ToPrettyString(bodyAUid)}, the connected entity {ToPrettyString(bodyBUid)} already had a joint with the same ID connected to another entity {ToPrettyString(value.BodyAUid)}.");
				return;
			}
			base.Log.Error($"While adding joint {joint.ID} to entity {ToPrettyString(bodyAUid)}, the joint already existed for the connected entity {ToPrettyString(bodyBUid)}.");
		}
		joints.TryAdd(joint.ID, joint);
		joints2.TryAdd(joint.ID, joint);
		if (!joint.CollideConnected)
		{
			FilterContactsForJoint(joint, bodyA, bodyB);
		}
		_physics.WakeBody(bodyAUid, force: false, null, bodyA);
		_physics.WakeBody(bodyBUid, force: false, null, bodyB);
		Dirty(bodyAUid, bodyA);
		Dirty(bodyBUid, bodyB);
		Dirty(bodyAUid, jointComponentA);
		Dirty(bodyBUid, jointComponentB);
		_dirtyJoints.Add((Owner: bodyAUid, Comp: jointComponentA));
		_dirtyJoints.Add((Owner: bodyBUid, Comp: jointComponentB));
		JointAddedEvent jointAddedEvent = new JointAddedEvent(joint, bodyAUid, bodyBUid, bodyA, bodyB);
		EntityManager.EventBus.RaiseLocalEvent(bodyAUid, jointAddedEvent);
		JointAddedEvent args = new JointAddedEvent(joint, bodyBUid, bodyAUid, bodyB, bodyA);
		EntityManager.EventBus.RaiseLocalEvent(bodyBUid, args);
		EntityManager.EventBus.RaiseEvent(EventSource.Local, jointAddedEvent);
	}

	private static string GetJointId(Joint joint)
	{
		string iD = joint.ID;
		if (string.IsNullOrEmpty(iD))
		{
			return joint.GetHashCode().ToString();
		}
		return iD;
	}

	public DistanceJoint CreateDistanceJoint(EntityUid bodyA, EntityUid bodyB, Vector2? anchorA = null, Vector2? anchorB = null, string? id = null, TransformComponent? xformA = null, TransformComponent? xformB = null, float? minimumDistance = null)
	{
		if (!Resolve(bodyA, ref xformA) || !Resolve(bodyB, ref xformB))
		{
			throw new InvalidOperationException();
		}
		Vector2 valueOrDefault = anchorA.GetValueOrDefault();
		if (!anchorA.HasValue)
		{
			valueOrDefault = Vector2.Zero;
			anchorA = valueOrDefault;
		}
		valueOrDefault = anchorB.GetValueOrDefault();
		if (!anchorB.HasValue)
		{
			valueOrDefault = Vector2.Zero;
			anchorB = valueOrDefault;
		}
		Vector2 vector = Vector2.Transform(anchorA.Value, _transform.GetWorldMatrix(xformA));
		Vector2 vector2 = Vector2.Transform(anchorB.Value, _transform.GetWorldMatrix(xformB));
		float num = (vector - vector2).Length();
		if (minimumDistance.HasValue)
		{
			num = Math.Max(minimumDistance.Value, num);
		}
		DistanceJoint distanceJoint = new DistanceJoint(bodyA, bodyB, anchorA.Value, anchorB.Value, num);
		if (id == null)
		{
			id = GetJointId(distanceJoint);
		}
		distanceJoint.ID = id;
		AddJoint(distanceJoint);
		return distanceJoint;
	}

	public MouseJoint CreateMouseJoint(EntityUid bodyA, EntityUid bodyB, Vector2? anchorA = null, Vector2? anchorB = null, string? id = null)
	{
		Vector2 valueOrDefault = anchorA.GetValueOrDefault();
		if (!anchorA.HasValue)
		{
			valueOrDefault = Vector2.Zero;
			anchorA = valueOrDefault;
		}
		valueOrDefault = anchorB.GetValueOrDefault();
		if (!anchorB.HasValue)
		{
			valueOrDefault = Vector2.Zero;
			anchorB = valueOrDefault;
		}
		MouseJoint mouseJoint = new MouseJoint(bodyA, bodyB, anchorA.Value, anchorB.Value);
		if (id == null)
		{
			id = GetJointId(mouseJoint);
		}
		mouseJoint.ID = id;
		AddJoint(mouseJoint);
		return mouseJoint;
	}

	public PrismaticJoint CreatePrismaticJoint(EntityUid bodyA, EntityUid bodyB, string? id = null)
	{
		PrismaticJoint prismaticJoint = new PrismaticJoint(bodyA, bodyB);
		if (id == null)
		{
			id = GetJointId(prismaticJoint);
		}
		prismaticJoint.ID = id;
		AddJoint(prismaticJoint);
		return prismaticJoint;
	}

	public PrismaticJoint CreatePrismaticJoint(EntityUid bodyA, EntityUid bodyB, Vector2 anchorA, Vector2 anchorB, Vector2 worldAxis, float referenceAngle, string? id = null)
	{
		Vector2 localVector = GetLocalVector2(bodyA, worldAxis);
		PrismaticJoint prismaticJoint = new PrismaticJoint(bodyA, bodyB, anchorA, anchorB, localVector, referenceAngle);
		if (id == null)
		{
			id = GetJointId(prismaticJoint);
		}
		prismaticJoint.ID = id;
		AddJoint(prismaticJoint);
		return prismaticJoint;
	}

	public RevoluteJoint CreateRevoluteJoint(EntityUid bodyA, EntityUid bodyB, string? id = null)
	{
		RevoluteJoint revoluteJoint = new RevoluteJoint(bodyA, bodyB);
		if (id == null)
		{
			id = GetJointId(revoluteJoint);
		}
		revoluteJoint.ID = id;
		AddJoint(revoluteJoint);
		return revoluteJoint;
	}

	public WeldJoint GetOrCreateWeldJoint(EntityUid bodyA, EntityUid bodyB, string? id = null)
	{
		if (id != null && _jointsQuery.TryComp(bodyA, out JointComponent component) && component.Joints.TryGetValue(id, out Joint value))
		{
			return (WeldJoint)value;
		}
		WeldJoint weldJoint = new WeldJoint(bodyA, bodyB);
		if (id == null)
		{
			id = GetJointId(weldJoint);
		}
		weldJoint.ID = id;
		AddJoint(weldJoint);
		return weldJoint;
	}

	public WeldJoint CreateWeldJoint(EntityUid bodyA, EntityUid bodyB, string? id = null)
	{
		WeldJoint weldJoint = new WeldJoint(bodyA, bodyB);
		if (id == null)
		{
			id = GetJointId(weldJoint);
		}
		weldJoint.ID = id;
		AddJoint(weldJoint);
		return weldJoint;
	}

	private Vector2 GetLocalVector2(EntityUid uid, Vector2 worldVector, TransformComponent? xform = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!Resolve(uid, ref xform))
		{
			return Vector2.Zero;
		}
		return Robust.Shared.Physics.Transform.MulT(new Quaternion2D((float)_transform.GetWorldRotation(xform).Theta), worldVector);
	}

	public static void LinearStiffness(float frequencyHertz, float dampingRatio, float massA, float massB, out float stiffness, out float damping)
	{
		float num = ((massA > 0f && massB > 0f) ? (massA * massB / (massA + massB)) : ((!(massA > 0f)) ? massB : massA));
		float num2 = (float)Math.PI * 2f * frequencyHertz;
		stiffness = num * num2 * num2;
		damping = 2f * num * dampingRatio * num2;
	}

	public static void AngularStiffness(float frequencyHertz, float dampingRatio, PhysicsComponent bodyA, PhysicsComponent bodyB, out float stiffness, out float damping)
	{
		float inertia = bodyA.Inertia;
		float inertia2 = bodyB.Inertia;
		float num = ((inertia > 0f && inertia2 > 0f) ? (inertia * inertia2 / (inertia + inertia2)) : ((!(inertia > 0f)) ? inertia2 : inertia));
		float num2 = (float)Math.PI * 2f * frequencyHertz;
		stiffness = num * num2 * num2;
		damping = 2f * num * dampingRatio * num2;
	}

	protected void AddJoint(Joint joint, PhysicsComponent? bodyA = null, PhysicsComponent? bodyB = null)
	{
		if (!_physicsQuery.Resolve(joint.BodyAUid, ref bodyA) || !_physicsQuery.Resolve(joint.BodyBUid, ref bodyB))
		{
			return;
		}
		if (!joint.CollideConnected)
		{
			FilterContactsForJoint(joint, bodyA, bodyB);
		}
		MapId mapID = Transform(joint.BodyAUid).MapID;
		if (mapID == MapId.Nullspace || mapID != Transform(joint.BodyBUid).MapID)
		{
			base.Log.Error("Tried to add joint to ineligible bodies");
			return;
		}
		if (string.IsNullOrEmpty(joint.ID))
		{
			base.Log.Error("Can't add a joint with no ID");
			return;
		}
		InitJoint(joint, bodyA, bodyB);
		if (_gameTiming.IsFirstTimePredicted)
		{
			base.Log.Debug($"Added {joint.JointType} Joint with ID {joint.ID} from {bodyA.BodyType} to {bodyB.BodyType} ");
		}
	}

	public void RecursiveClearJoints(EntityUid uid, TransformComponent? xform = null, JointComponent? component = null, JointRelayTargetComponent? relay = null)
	{
		if (!Resolve(uid, ref xform))
		{
			return;
		}
		_jointsQuery.Resolve(uid, ref component, logMissing: false);
		_relayQuery.Resolve(uid, ref relay, logMissing: false);
		if (relay != null)
		{
			foreach (EntityUid item in relay.Relayed)
			{
				_jointsQuery.TryGetComponent(item, out JointComponent component2);
				ClearJoints(item, component2);
			}
			RemComp(uid, relay);
		}
		if (component != null)
		{
			ClearJoints(uid, component);
		}
	}

	public void ClearJoints(EntityUid uid, JointComponent? component = null)
	{
		if (!_jointsQuery.Resolve(uid, ref component, logMissing: false))
		{
			return;
		}
		Joint[] array = component.Joints.Values.ToArray();
		foreach (Joint joint in array)
		{
			RemoveJoint(joint);
		}
		foreach (Joint addedJoint in AddedJoints)
		{
			if (addedJoint.BodyAUid == uid || addedJoint.BodyBUid == uid)
			{
				ToRemove.Add(addedJoint);
			}
		}
		AddedJoints.ExceptWith(ToRemove);
		ToRemove.Clear();
		if (_gameTiming.IsFirstTimePredicted)
		{
			base.Log.Debug($"Removed all joints from entity {ToPrettyString(uid)}");
		}
	}

	public void RemoveJoint(EntityUid uid, string id)
	{
		if (_jointsQuery.TryComp(uid, out JointComponent component) && component.Joints.TryGetValue(id, out Joint value))
		{
			RemoveJoint(value);
		}
	}

	public void RemoveJoint(Joint joint)
	{
		AddedJoints.Remove(joint);
		EntityUid bodyAUid = joint.BodyAUid;
		EntityUid bodyBUid = joint.BodyBUid;
		if (!_jointsQuery.TryComp(bodyAUid, out JointComponent component) || !_jointsQuery.TryComp(bodyBUid, out JointComponent component2) || !component.Joints.Remove(joint.ID) || !component2.Joints.Remove(joint.ID))
		{
			return;
		}
		if (_physicsQuery.TryComp(bodyAUid, out PhysicsComponent component3) && (int)MetaData(bodyAUid).EntityLifeStage < 4)
		{
			EntityUid uid = component.Relay ?? bodyAUid;
			_physics.WakeBody(uid);
		}
		if (TryComp(bodyBUid, out PhysicsComponent comp) && (int)MetaData(bodyBUid).EntityLifeStage < 4)
		{
			EntityUid uid2 = component2.Relay ?? bodyBUid;
			_physics.WakeBody(uid2);
		}
		if (!component.Deleted)
		{
			Dirty(bodyAUid, component);
		}
		if (!component2.Deleted)
		{
			Dirty(bodyBUid, component2);
		}
		if (component.Deleted && component2.Deleted)
		{
			return;
		}
		if (!joint.CollideConnected)
		{
			FilterContactsForJoint(joint);
		}
		if (component3 == null)
		{
			base.Log.Debug($"Tried to remove joint from entity {ToPrettyString(bodyAUid)} without a physics component");
		}
		else if (comp == null)
		{
			base.Log.Debug($"Tried to remove joint from entity {ToPrettyString(bodyBUid)} without a physics component");
		}
		else
		{
			JointRemovedEvent jointRemovedEvent = new JointRemovedEvent(joint, bodyAUid, bodyBUid, component3, comp);
			EntityManager.EventBus.RaiseLocalEvent(bodyAUid, jointRemovedEvent);
			JointRemovedEvent args = new JointRemovedEvent(joint, bodyBUid, bodyAUid, comp, component3);
			EntityManager.EventBus.RaiseLocalEvent(bodyBUid, args);
			EntityManager.EventBus.RaiseEvent(EventSource.Local, jointRemovedEvent);
			if (_gameTiming.IsFirstTimePredicted)
			{
				base.Log.Debug($"Removed {joint.JointType} joint with ID {joint.ID} from entity {ToPrettyString(bodyAUid)} to entity {ToPrettyString(bodyBUid)}");
			}
		}
		_dirtyJoints.Add((Owner: bodyAUid, Comp: component));
		_dirtyJoints.Add((Owner: bodyBUid, Comp: component2));
	}

	internal void FilterContactsForJoint(Joint joint, PhysicsComponent? bodyA = null, PhysicsComponent? bodyB = null)
	{
		if (!_physicsQuery.Resolve(joint.BodyBUid, ref bodyB))
		{
			return;
		}
		LinkedListNode<Contact> linkedListNode = bodyB.Contacts.First;
		while (linkedListNode != null)
		{
			Contact value = linkedListNode.Value;
			linkedListNode = linkedListNode.Next;
			if (value.EntityA == joint.BodyAUid || value.EntityB == joint.BodyAUid)
			{
				value.Flags |= ContactFlags.Filter;
			}
		}
	}

	private void InitializeRelay()
	{
		SubscribeLocalEvent<JointRelayTargetComponent, ComponentShutdown>(OnRelayShutdown);
		SubscribeLocalEvent<JointRelayTargetComponent, ComponentGetState>(OnRelayGetState);
		SubscribeLocalEvent<JointRelayTargetComponent, ComponentHandleState>(OnRelayHandleState);
	}

	private void OnRelayGetState(EntityUid uid, JointRelayTargetComponent component, ref ComponentGetState args)
	{
		args.State = new JointRelayComponentState(GetNetEntitySet(component.Relayed));
	}

	private void OnRelayHandleState(EntityUid uid, JointRelayTargetComponent component, ref ComponentHandleState args)
	{
		if (args.Current is JointRelayComponentState jointRelayComponentState)
		{
			EnsureEntitySet<JointRelayTargetComponent>(jointRelayComponentState.Entities, uid, component.Relayed);
		}
	}

	private void OnRelayShutdown(EntityUid uid, JointRelayTargetComponent component, ComponentShutdown args)
	{
		if (_gameTiming.ApplyingState)
		{
			return;
		}
		foreach (EntityUid item in component.Relayed)
		{
			if (!TerminatingOrDeleted(item) && _jointsQuery.TryGetComponent(item, out JointComponent component2))
			{
				RefreshRelay(item, component2);
			}
		}
	}

	public void RefreshRelay(EntityUid uid, JointComponent? component = null)
	{
		if (!Resolve(uid, ref component, logMissing: false))
		{
			return;
		}
		EntityUid? entityUid = null;
		if (_container.TryGetOuterContainer(uid, Transform(uid), out BaseContainer container))
		{
			entityUid = container.Owner;
			foreach (Joint value in component.Joints.Values)
			{
				if (value.GetOther(uid) == entityUid)
				{
					SetRelay(uid, null, component);
					return;
				}
			}
		}
		SetRelay(uid, entityUid, component);
	}

	public void SetRelay(EntityUid uid, EntityUid? relay, JointComponent? component = null)
	{
		if (!Resolve(uid, ref component, logMissing: false) || component.Relay == relay)
		{
			return;
		}
		if (TryComp(component.Relay, out JointRelayTargetComponent comp) && comp.Relayed.Remove(uid))
		{
			if (comp.Relayed.Count == 0)
			{
				RemCompDeferred<JointRelayTargetComponent>(component.Relay.Value);
			}
			Dirty(component.Relay.Value, comp);
		}
		component.Relay = relay;
		if (relay.HasValue)
		{
			comp = EnsureComp<JointRelayTargetComponent>(relay.Value);
			if (comp.Relayed.Add(uid))
			{
				_physics.WakeBody(relay.Value);
				Dirty(relay.Value, comp);
			}
		}
		Dirty(uid, component);
	}
}
