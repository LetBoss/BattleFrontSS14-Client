using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Physics.Dynamics.Joints;

public sealed class MouseJoint : Joint, IEquatable<MouseJoint>, ISerializationGenerated<MouseJoint>, ISerializationGenerated
{
	private float _maxForce;

	private float _stiffness;

	private float _damping;

	private float _invMassB;

	private float _invIB;

	private Vector2 _rB;

	private Vector2 _C;

	private Matrix22 _mass;

	private Vector2 _impulse;

	private float _beta;

	private float _gamma;

	private int _indexB;

	private Vector2 _localCenterB;

	public override JointType JointType => JointType.Mouse;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("maxForce", false, 1, false, false, null)]
	public float MaxForce
	{
		get
		{
			return _maxForce;
		}
		set
		{
			if (!MathHelper.CloseTo(_maxForce, value, 1E-07f))
			{
				_maxForce = value;
				Dirty();
			}
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("stiffness", false, 1, false, false, null)]
	public float Stiffness
	{
		get
		{
			return _stiffness;
		}
		set
		{
			if (!MathHelper.CloseTo(_stiffness, value, 1E-07f))
			{
				_stiffness = value;
				Dirty();
			}
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("damping", false, 1, false, false, null)]
	public float Damping
	{
		get
		{
			return _damping;
		}
		set
		{
			if (!MathHelper.CloseTo(_damping, value, 1E-07f))
			{
				_damping = value;
				Dirty();
			}
		}
	}

	public MouseJoint()
	{
	}

	public MouseJoint(EntityUid uidA, EntityUid uidB, Vector2 localAnchorA, Vector2 localAnchorB)
		: base(uidA, uidB)
	{
		base.LocalAnchorA = localAnchorA;
		base.LocalAnchorB = localAnchorB;
	}

	internal MouseJoint(MouseJointState state, IEntityManager entManager, EntityUid owner)
		: base(state, entManager, owner)
	{
		Damping = state.Damping;
		Stiffness = state.Stiffness;
		MaxForce = state.MaxForce;
	}

	public override JointState GetState(IEntityManager entManager)
	{
		MouseJointState mouseJointState = new MouseJointState
		{
			Damping = _damping,
			Stiffness = _stiffness,
			MaxForce = _maxForce,
			LocalAnchorA = base.LocalAnchorA,
			LocalAnchorB = base.LocalAnchorB
		};
		GetState(mouseJointState, entManager);
		return mouseJointState;
	}

	public override Vector2 GetReactionForce(float invDt)
	{
		return _impulse * invDt;
	}

	public override float GetReactionTorque(float invDt)
	{
		return invDt * 0f;
	}

	internal override void InitVelocityConstraints(in SolverData data, in SharedPhysicsSystem.IslandData island, PhysicsComponent bodyA, PhysicsComponent bodyB, Vector2[] positions, float[] angles, Vector2[] linearVelocities, float[] angularVelocities)
	{
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		int offset = island.Offset;
		_indexB = bodyB.IslandIndex[island.Index];
		_localCenterB = bodyB.LocalCenter;
		_invMassB = bodyB.InvMass;
		_invIB = bodyB.InvI;
		Vector2 vector = positions[_indexB];
		float angle = angles[_indexB];
		Vector2 vector2 = linearVelocities[offset + _indexB];
		float num = angularVelocities[offset + _indexB];
		Quaternion2D quaternion2D = new Quaternion2D(angle);
		float damping = _damping;
		float stiffness = _stiffness;
		float frameTime = data.FrameTime;
		_gamma = frameTime * (damping + frameTime * stiffness);
		if (_gamma != 0f)
		{
			_gamma = 1f / _gamma;
		}
		_beta = frameTime * stiffness * _gamma;
		_rB = Transform.Mul(in quaternion2D, base.LocalAnchorB - _localCenterB);
		Unsafe.SkipInit(out Matrix22 val);
		val.EX.X = _invMassB + _invIB * _rB.Y * _rB.Y + _gamma;
		val.EX.Y = (0f - _invIB) * _rB.X * _rB.Y;
		val.EY.X = val.EX.Y;
		val.EY.Y = _invMassB + _invIB * _rB.X * _rB.X + _gamma;
		_mass = ((Matrix22)(ref val)).GetInverse();
		Vector2 vector3 = positions[bodyA.IslandIndex[island.Index]];
		_C = vector + _rB - vector3;
		_C *= _beta;
		num *= MathF.Max(0f, 1f - 0.02f * (60f * data.FrameTime));
		if (data.WarmStarting)
		{
			_impulse *= data.DtRatio;
			vector2 += _impulse * _invMassB;
			num += _invIB * Vector2Helpers.Cross(_rB, _impulse);
		}
		else
		{
			_impulse = Vector2.Zero;
		}
		linearVelocities[offset + _indexB] = vector2;
		angularVelocities[offset + _indexB] = num;
	}

	internal override void SolveVelocityConstraints(in SolverData data, in SharedPhysicsSystem.IslandData island, Vector2[] linearVelocities, float[] angularVelocities)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		int offset = island.Offset;
		Vector2 vector = linearVelocities[offset + _indexB];
		float num = angularVelocities[offset + _indexB];
		Vector2 vector2 = vector + Vector2Helpers.Cross(num, ref _rB);
		Vector2 vector3 = Transform.Mul(_mass, -(vector2 + _C + _impulse * _gamma));
		Vector2 impulse = _impulse;
		_impulse += vector3;
		float num2 = data.FrameTime * _maxForce;
		if (_impulse.LengthSquared() > num2 * num2)
		{
			_impulse *= num2 / _impulse.Length();
		}
		vector3 = _impulse - impulse;
		vector += vector3 * _invMassB;
		num += _invIB * Vector2Helpers.Cross(_rB, vector3);
		linearVelocities[offset + _indexB] = vector;
		angularVelocities[offset + _indexB] = num;
	}

	internal override bool SolvePositionConstraints(in SolverData data, Vector2[] positions, float[] angles)
	{
		return true;
	}

	public override Joint Clone(EntityUid uidA, EntityUid uidB)
	{
		return new MouseJoint(uidA, uidB, base.LocalAnchorA, base.LocalAnchorB)
		{
			Enabled = base.Enabled,
			MaxForce = MaxForce,
			Damping = Damping,
			Stiffness = Stiffness,
			_impulse = _impulse,
			Breakpoint = base.Breakpoint
		};
	}

	public override void CopyTo(Joint original)
	{
		if (original is MouseJoint mouseJoint)
		{
			mouseJoint.Enabled = base.Enabled;
			mouseJoint.MaxForce = MaxForce;
			mouseJoint.Damping = Damping;
			mouseJoint.Stiffness = Stiffness;
			mouseJoint._impulse = _impulse;
			mouseJoint.Breakpoint = base.Breakpoint;
		}
	}

	public bool Equals(MouseJoint? other)
	{
		if (other == null)
		{
			return false;
		}
		if (base.BodyAUid == other.BodyAUid && base.BodyBUid == other.BodyBUid && _damping.Equals(other.Damping) && _stiffness.Equals(other.Stiffness))
		{
			return _maxForce.Equals(other.MaxForce);
		}
		return false;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MouseJoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Joint target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (MouseJoint)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			float target3 = 0f;
			if (!serialization.TryCustomCopy(MaxForce, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = MaxForce;
			}
			target.MaxForce = target3;
			float target4 = 0f;
			if (!serialization.TryCustomCopy(Stiffness, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = Stiffness;
			}
			target.Stiffness = target4;
			float target5 = 0f;
			if (!serialization.TryCustomCopy(Damping, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = Damping;
			}
			target.Damping = target5;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MouseJoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Joint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MouseJoint target2 = (MouseJoint)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MouseJoint target2 = (MouseJoint)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MouseJoint Instantiate()
	{
		return new MouseJoint();
	}
}
