using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Physics.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class PhysicsComponent : Component, IComponentDelta, IComponent, ISerializationGenerated<IComponent>, ISerializationGenerated, ISerializationGenerated<IComponentDelta>, ISerializationGenerated<PhysicsComponent>
{
	[Access(new Type[] { typeof(SharedPhysicsSystem) })]
	public bool Island;

	[Access(new Type[] { typeof(SharedPhysicsSystem) })]
	public Dictionary<int, int> IslandIndex = new Dictionary<int, int>();

	internal readonly LinkedList<Contact> Contacts = new LinkedList<Contact>();

	[DataField(null, false, 1, false, false, null)]
	public bool IgnorePaused;

	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public BodyType BodyType = BodyType.Static;

	[ViewVariables(VVAccess.ReadWrite)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public bool Awake;

	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public bool SleepingAllowed = true;

	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public float SleepTime;

	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public bool CanCollide = true;

	internal float _mass;

	internal float _invMass;

	[ViewVariables(VVAccess.ReadWrite)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	internal float _inertia;

	[ViewVariables(VVAccess.ReadWrite)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public float InvI;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public bool FixedRotation = true;

	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	internal Vector2 _localCenter = Vector2.Zero;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public Vector2 Force;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public float Torque;

	internal float _friction;

	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public float LinearDamping = 0.2f;

	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public float AngularDamping = 0.2f;

	[ViewVariables(VVAccess.ReadWrite)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.ReadExecute)]
	public Vector2 LinearVelocity;

	[ViewVariables(VVAccess.ReadWrite)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.ReadExecute)]
	public float AngularVelocity;

	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) })]
	public bool IgnoreGravity;

	[ViewVariables]
	[Access(new Type[] { typeof(SharedPhysicsSystem) })]
	public bool Predict;

	public GameTick LastFieldUpdate { get; set; }

	public GameTick[] LastModifiedFields { get; set; }

	[ViewVariables]
	public int ContactCount => Contacts.Count;

	[ViewVariables]
	[Access(new Type[]
	{
		typeof(SharedPhysicsSystem),
		typeof(FixtureSystem)
	}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public bool Hard { get; internal set; }

	[ViewVariables]
	[Access(new Type[]
	{
		typeof(SharedPhysicsSystem),
		typeof(FixtureSystem)
	}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public int CollisionLayer { get; internal set; }

	[ViewVariables]
	[Access(new Type[]
	{
		typeof(SharedPhysicsSystem),
		typeof(FixtureSystem)
	}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public int CollisionMask { get; internal set; }

	[ViewVariables]
	public float FixturesMass => _mass;

	[ViewVariables(VVAccess.ReadOnly)]
	public float Mass
	{
		get
		{
			if ((BodyType & (BodyType.KinematicController | BodyType.Dynamic)) == 0)
			{
				return 0f;
			}
			return _mass;
		}
	}

	[ViewVariables]
	public float InvMass
	{
		get
		{
			if ((BodyType & (BodyType.KinematicController | BodyType.Dynamic)) == 0)
			{
				return 0f;
			}
			return _invMass;
		}
	}

	[ViewVariables]
	public float Inertia => _inertia + _mass * Vector2.Dot(_localCenter, _localCenter);

	[ViewVariables]
	public Vector2 LocalCenter => _localCenter;

	[ViewVariables]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public float Friction => _friction;

	[ViewVariables]
	public Vector2 Momentum => LinearVelocity * Mass;

	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPhysicsSystem) }, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
	public BodyStatus BodyStatus { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PhysicsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (PhysicsComponent)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			bool target3 = false;
			if (!serialization.TryCustomCopy(IgnorePaused, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = IgnorePaused;
			}
			target.IgnorePaused = target3;
			BodyType target4 = BodyType.Kinematic;
			if (!serialization.TryCustomCopy(BodyType, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = BodyType;
			}
			target.BodyType = target4;
			bool target5 = false;
			if (!serialization.TryCustomCopy(SleepingAllowed, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = SleepingAllowed;
			}
			target.SleepingAllowed = target5;
			float target6 = 0f;
			if (!serialization.TryCustomCopy(SleepTime, ref target6, hookCtx, hasHooks: false, context))
			{
				target6 = SleepTime;
			}
			target.SleepTime = target6;
			bool target7 = false;
			if (!serialization.TryCustomCopy(CanCollide, ref target7, hookCtx, hasHooks: false, context))
			{
				target7 = CanCollide;
			}
			target.CanCollide = target7;
			bool target8 = false;
			if (!serialization.TryCustomCopy(FixedRotation, ref target8, hookCtx, hasHooks: false, context))
			{
				target8 = FixedRotation;
			}
			target.FixedRotation = target8;
			Vector2 target9 = default(Vector2);
			if (!serialization.TryCustomCopy(Force, ref target9, hookCtx, hasHooks: false, context))
			{
				target9 = serialization.CreateCopy(Force, hookCtx, context);
			}
			target.Force = target9;
			float target10 = 0f;
			if (!serialization.TryCustomCopy(Torque, ref target10, hookCtx, hasHooks: false, context))
			{
				target10 = Torque;
			}
			target.Torque = target10;
			float target11 = 0f;
			if (!serialization.TryCustomCopy(LinearDamping, ref target11, hookCtx, hasHooks: false, context))
			{
				target11 = LinearDamping;
			}
			target.LinearDamping = target11;
			float target12 = 0f;
			if (!serialization.TryCustomCopy(AngularDamping, ref target12, hookCtx, hasHooks: false, context))
			{
				target12 = AngularDamping;
			}
			target.AngularDamping = target12;
			BodyStatus target13 = BodyStatus.OnGround;
			if (!serialization.TryCustomCopy(BodyStatus, ref target13, hookCtx, hasHooks: false, context))
			{
				target13 = BodyStatus;
			}
			target.BodyStatus = target13;
			bool target14 = false;
			if (!serialization.TryCustomCopy(IgnoreGravity, ref target14, hookCtx, hasHooks: false, context))
			{
				target14 = IgnoreGravity;
			}
			target.IgnoreGravity = target14;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PhysicsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PhysicsComponent target2 = (PhysicsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PhysicsComponent target2 = (PhysicsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IComponentDelta target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PhysicsComponent target2 = (PhysicsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IComponentDelta target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PhysicsComponent target2 = (PhysicsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PhysicsComponent Instantiate()
	{
		return new PhysicsComponent();
	}

	IComponentDelta IComponentDelta.Instantiate()
	{
		return Instantiate();
	}

	IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
	{
		return Instantiate();
	}
}
