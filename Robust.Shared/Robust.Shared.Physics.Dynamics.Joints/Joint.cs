using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Physics.Dynamics.Joints;

[ImplicitDataDefinitionForInheritors]
public abstract class Joint : IEquatable<Joint>, ISerializationGenerated<Joint>, ISerializationGenerated
{
	internal bool IslandFlag;

	[DataField("localAnchorA", false, 1, false, false, null)]
	private Vector2 _localAnchorA;

	[DataField("localAnchorB", false, 1, false, false, null)]
	private Vector2 _localAnchorB;

	[DataField("collideConnected", false, 1, false, false, null)]
	protected bool _collideConnected = true;

	[DataField("breakpoint", false, 1, false, false, null)]
	private float _breakpoint = float.MaxValue;

	private double _breakpointSquared = double.MaxValue;

	[DataField("id", false, 1, false, false, null)]
	public string ID { get; set; } = string.Empty;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("enabled", false, 1, false, false, null)]
	public bool Enabled { get; internal set; } = true;

	public abstract JointType JointType { get; }

	[DataField("bodyA", false, 1, false, false, null)]
	public EntityUid BodyAUid { get; private set; }

	[DataField("bodyB", false, 1, false, false, null)]
	public EntityUid BodyBUid { get; private set; }

	[ViewVariables(VVAccess.ReadWrite)]
	public Vector2 LocalAnchorA
	{
		get
		{
			return _localAnchorA;
		}
		set
		{
			if (!Vector2Helpers.EqualsApprox(_localAnchorA, value))
			{
				_localAnchorA = value;
				Dirty();
			}
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	public Vector2 LocalAnchorB
	{
		get
		{
			return _localAnchorB;
		}
		set
		{
			if (!Vector2Helpers.EqualsApprox(_localAnchorB, value))
			{
				_localAnchorB = value;
				Dirty();
			}
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	public bool CollideConnected
	{
		get
		{
			return _collideConnected;
		}
		set
		{
			if (_collideConnected != value)
			{
				_collideConnected = value;
				if (!_collideConnected)
				{
					IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<SharedJointSystem>().FilterContactsForJoint(this);
				}
				Dirty();
			}
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	public float Breakpoint
	{
		get
		{
			return _breakpoint;
		}
		set
		{
			if (!MathHelper.CloseToPercent(_breakpoint, value, 1E-05))
			{
				_breakpoint = value;
				_breakpointSquared = _breakpoint * _breakpoint;
				Dirty();
			}
		}
	}

	public EntityUid GetOther(EntityUid uid)
	{
		if (BodyAUid == uid)
		{
			return BodyBUid;
		}
		if (BodyBUid == uid)
		{
			return BodyAUid;
		}
		throw new ArgumentOutOfRangeException($"EntityUid {uid} unrelated to joint");
	}

	protected internal void Dirty(IEntityManager? entMan = null)
	{
		IoCManager.Resolve(ref entMan);
		if (entMan.TryGetComponent<PhysicsComponent>(BodyAUid, out PhysicsComponent component))
		{
			entMan.Dirty(BodyAUid, component);
		}
		if (entMan.TryGetComponent<PhysicsComponent>(BodyBUid, out component))
		{
			entMan.Dirty(BodyBUid, component);
		}
	}

	protected Joint()
	{
	}

	protected Joint(EntityUid bodyAUid, EntityUid bodyBUid)
	{
		BodyAUid = bodyAUid;
		BodyBUid = bodyBUid;
	}

	protected Joint(JointState state, IEntityManager entManager, EntityUid owner)
	{
		ID = state.ID;
		BodyAUid = entManager.EnsureEntity<JointComponent>(state.UidA, owner);
		BodyBUid = entManager.EnsureEntity<JointComponent>(state.UidB, owner);
		Enabled = state.Enabled;
		_collideConnected = state.CollideConnected;
		_localAnchorA = state.LocalAnchorA;
		_localAnchorB = state.LocalAnchorB;
		_breakpoint = state.Breakpoint;
	}

	protected void GetState(JointState state, IEntityManager entManager)
	{
		state.ID = ID;
		state.CollideConnected = _collideConnected;
		state.Enabled = Enabled;
		state.UidA = entManager.GetNetEntity(BodyAUid);
		state.UidB = entManager.GetNetEntity(BodyBUid);
		state.Breakpoint = _breakpoint;
	}

	public abstract JointState GetState(IEntityManager entManager);

	internal virtual void ApplyState(JointState state)
	{
		ID = state.ID;
		CollideConnected = state.CollideConnected;
		Enabled = state.Enabled;
		Breakpoint = state.Breakpoint;
		_breakpointSquared = Breakpoint * Breakpoint;
		_localAnchorA = state.LocalAnchorA;
		_localAnchorB = state.LocalAnchorB;
	}

	public abstract Vector2 GetReactionForce(float invDt);

	public abstract float GetReactionTorque(float invDt);

	internal abstract void InitVelocityConstraints(in SolverData data, in SharedPhysicsSystem.IslandData island, PhysicsComponent bodyA, PhysicsComponent bodyB, Vector2[] positions, float[] angles, Vector2[] linearVelocities, float[] angularVelocities);

	internal float Validate(float invDt)
	{
		if (!Enabled)
		{
			return 0f;
		}
		float num = GetReactionForce(invDt).LengthSquared();
		if ((double)MathF.Abs(num) <= _breakpointSquared)
		{
			return 0f;
		}
		Enabled = false;
		return num;
	}

	internal abstract void SolveVelocityConstraints(in SolverData data, in SharedPhysicsSystem.IslandData island, Vector2[] linearVelocities, float[] angularVelocities);

	internal abstract bool SolvePositionConstraints(in SolverData data, Vector2[] positions, float[] angles);

	public bool Equals(Joint? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (Enabled == other.Enabled && JointType == other.JointType && BodyAUid.Equals(other.BodyAUid) && BodyBUid.Equals(other.BodyBUid) && CollideConnected == other.CollideConnected && MathHelper.CloseTo(_breakpoint, other._breakpoint, 1E-07f) && Vector2Helpers.EqualsApprox(_localAnchorA, other._localAnchorA))
		{
			return Vector2Helpers.EqualsApprox(_localAnchorB, other._localAnchorB);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((BodyAUid.GetHashCode() * 397) ^ BodyBUid.GetHashCode()) * 397) ^ JointType.GetHashCode();
	}

	public abstract Joint Clone(EntityUid uidA, EntityUid uidB);

	public Joint Clone()
	{
		return Clone(BodyAUid, BodyBUid);
	}

	public abstract void CopyTo(Joint original);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref Joint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			string target2 = null;
			if (ID == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy(ID, ref target2, hookCtx, hasHooks: false, context))
			{
				target2 = ID;
			}
			target.ID = target2;
			bool target3 = false;
			if (!serialization.TryCustomCopy(Enabled, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = Enabled;
			}
			target.Enabled = target3;
			EntityUid target4 = default(EntityUid);
			if (!serialization.TryCustomCopy(BodyAUid, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = serialization.CreateCopy(BodyAUid, hookCtx, context);
			}
			target.BodyAUid = target4;
			EntityUid target5 = default(EntityUid);
			if (!serialization.TryCustomCopy(BodyBUid, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = serialization.CreateCopy(BodyBUid, hookCtx, context);
			}
			target.BodyBUid = target5;
			Vector2 target6 = default(Vector2);
			if (!serialization.TryCustomCopy(_localAnchorA, ref target6, hookCtx, hasHooks: false, context))
			{
				target6 = serialization.CreateCopy(_localAnchorA, hookCtx, context);
			}
			target._localAnchorA = target6;
			Vector2 target7 = default(Vector2);
			if (!serialization.TryCustomCopy(_localAnchorB, ref target7, hookCtx, hasHooks: false, context))
			{
				target7 = serialization.CreateCopy(_localAnchorB, hookCtx, context);
			}
			target._localAnchorB = target7;
			bool target8 = false;
			if (!serialization.TryCustomCopy(_collideConnected, ref target8, hookCtx, hasHooks: false, context))
			{
				target8 = _collideConnected;
			}
			target._collideConnected = target8;
			float target9 = 0f;
			if (!serialization.TryCustomCopy(_breakpoint, ref target9, hookCtx, hasHooks: false, context))
			{
				target9 = _breakpoint;
			}
			target._breakpoint = target9;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref Joint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Joint target2 = (Joint)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual Joint Instantiate()
	{
		throw new NotImplementedException();
	}
}
