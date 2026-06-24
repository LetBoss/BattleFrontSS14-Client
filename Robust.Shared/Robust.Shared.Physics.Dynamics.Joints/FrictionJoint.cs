using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Physics.Dynamics.Joints;

public sealed class FrictionJoint : Joint, IEquatable<FrictionJoint>, ISerializationGenerated<FrictionJoint>, ISerializationGenerated
{
	private Vector2 _linearImpulse;

	private float _angularImpulse;

	private int _indexA;

	private int _indexB;

	private Vector2 _rA;

	private Vector2 _rB;

	private Vector2 _localCenterA;

	private Vector2 _localCenterB;

	private float _invMassA;

	private float _invMassB;

	private float _invIA;

	private float _invIB;

	private float _angularMass;

	private Vector2[] _linearMass = new Vector2[2];

	public override JointType JointType => JointType.Friction;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("maxForce", false, 1, false, false, null)]
	public float MaxForce { get; set; }

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("maxTorque", false, 1, false, false, null)]
	public float MaxTorque { get; set; }

	public FrictionJoint()
	{
	}

	public FrictionJoint(EntityUid uidA, EntityUid uidB, Vector2 anchorA, Vector2 anchorB)
		: base(uidA, uidB)
	{
		base.LocalAnchorA = anchorA;
		base.LocalAnchorB = anchorB;
	}

	internal FrictionJoint(FrictionJointState state, IEntityManager entManager, EntityUid owner)
		: base(state, entManager, owner)
	{
		MaxForce = state.MaxForce;
		MaxTorque = state.MaxTorque;
	}

	public override JointState GetState(IEntityManager entManager)
	{
		FrictionJointState frictionJointState = new FrictionJointState();
		GetState(frictionJointState, entManager);
		return frictionJointState;
	}

	internal override void ApplyState(JointState state)
	{
		base.ApplyState(state);
		if (state is FrictionJointState frictionJointState)
		{
			MaxForce = frictionJointState.MaxForce;
			MaxTorque = frictionJointState.MaxTorque;
		}
	}

	public override Vector2 GetReactionForce(float invDt)
	{
		return _linearImpulse * invDt;
	}

	public override float GetReactionTorque(float invDt)
	{
		return invDt * _angularImpulse;
	}

	internal override void InitVelocityConstraints(in SolverData data, in SharedPhysicsSystem.IslandData island, PhysicsComponent bodyA, PhysicsComponent bodyB, Vector2[] positions, float[] angles, Vector2[] linearVelocities, float[] angularVelocities)
	{
		int offset = island.Offset;
		_indexA = bodyA.IslandIndex[island.Index];
		_indexB = bodyB.IslandIndex[island.Index];
		_localCenterA = bodyA.LocalCenter;
		_localCenterB = bodyB.LocalCenter;
		_invMassA = bodyA.InvMass;
		_invMassB = bodyB.InvMass;
		_invIA = bodyA.InvI;
		_invIB = bodyB.InvI;
		float angle = angles[_indexA];
		Vector2 vector = linearVelocities[offset + _indexA];
		float num = angularVelocities[offset + _indexA];
		float angle2 = angles[_indexB];
		Vector2 vector2 = linearVelocities[offset + _indexB];
		float num2 = angularVelocities[offset + _indexB];
		Quaternion2D quaternion2D = new Quaternion2D(angle);
		Quaternion2D quaternion2D2 = new Quaternion2D(angle2);
		_rA = Transform.Mul(in quaternion2D, base.LocalAnchorA - _localCenterA);
		_rB = Transform.Mul(in quaternion2D2, base.LocalAnchorB - _localCenterB);
		float invMassA = _invMassA;
		float invMassB = _invMassB;
		float invIA = _invIA;
		float invIB = _invIB;
		Span<Vector2> matrix = stackalloc Vector2[2];
		matrix[0].X = invMassA + invMassB + invIA * _rA.Y * _rA.Y + invIB * _rB.Y * _rB.Y;
		matrix[0].Y = (0f - invIA) * _rA.X * _rA.Y - invIB * _rB.X * _rB.Y;
		matrix[1].X = matrix[0].Y;
		matrix[1].Y = invMassA + invMassB + invIA * _rA.X * _rA.X + invIB * _rB.X * _rB.X;
		Vector4Helpers.Inverse(matrix);
		_angularMass = invIA + invIB;
		if (_angularMass > 0f)
		{
			_angularMass = 1f / _angularMass;
		}
		if (data.WarmStarting)
		{
			_linearImpulse *= data.DtRatio;
			_angularImpulse *= data.DtRatio;
			Vector2 vector3 = new Vector2(_linearImpulse.X, _linearImpulse.Y);
			vector -= vector3 * invMassA;
			num -= invIA * (Vector2Helpers.Cross(_rA, vector3) + _angularImpulse);
			vector2 += vector3 * invMassB;
			num2 += invIB * (Vector2Helpers.Cross(_rB, vector3) + _angularImpulse);
		}
		else
		{
			_linearImpulse = Vector2.Zero;
			_angularImpulse = 0f;
		}
		linearVelocities[offset + _indexA] = vector;
		angularVelocities[offset + _indexA] = num;
		linearVelocities[offset + _indexB] = vector2;
		angularVelocities[offset + _indexB] = num2;
	}

	internal override void SolveVelocityConstraints(in SolverData data, in SharedPhysicsSystem.IslandData island, Vector2[] linearVelocities, float[] angularVelocities)
	{
		int offset = island.Offset;
		Vector2 vector = linearVelocities[offset + _indexA];
		float num = angularVelocities[offset + _indexA];
		Vector2 vector2 = linearVelocities[offset + _indexB];
		float num2 = angularVelocities[offset + _indexB];
		float invMassA = _invMassA;
		float invMassB = _invMassB;
		float invIA = _invIA;
		float invIB = _invIB;
		float frameTime = data.FrameTime;
		float num3 = num2 - num;
		float num4 = (0f - _angularMass) * num3;
		float angularImpulse = _angularImpulse;
		float num5 = frameTime * MaxTorque;
		_angularImpulse = Math.Clamp(_angularImpulse + num4, 0f - num5, num5);
		num4 = _angularImpulse - angularImpulse;
		num -= invIA * num4;
		num2 += invIB * num4;
		Vector2 v = vector2 + Vector2Helpers.Cross(num2, ref _rB) - vector - Vector2Helpers.Cross(num, ref _rA);
		Vector2 vector3 = -Transform.Mul(in _linearMass, in v);
		Vector2 linearImpulse = _linearImpulse;
		_linearImpulse += vector3;
		float num6 = frameTime * MaxForce;
		if (_linearImpulse.LengthSquared() > num6 * num6)
		{
			_linearImpulse = Vector2Helpers.Normalized(_linearImpulse);
			_linearImpulse *= num6;
		}
		vector3 = _linearImpulse - linearImpulse;
		vector -= vector3 * invMassA;
		num -= invIA * Vector2Helpers.Cross(_rA, vector3);
		vector2 += vector3 * invMassB;
		num2 += invIB * Vector2Helpers.Cross(_rB, vector3);
		linearVelocities[offset + _indexA] = vector;
		angularVelocities[offset + _indexA] = num;
		linearVelocities[offset + _indexB] = vector2;
		angularVelocities[offset + _indexB] = num2;
	}

	internal override bool SolvePositionConstraints(in SolverData data, Vector2[] positions, float[] angles)
	{
		return true;
	}

	public override Joint Clone(EntityUid uidA, EntityUid uidB)
	{
		return new FrictionJoint(uidA, uidB, base.LocalAnchorA, base.LocalAnchorB)
		{
			Enabled = base.Enabled,
			MaxTorque = MaxTorque,
			MaxForce = MaxForce,
			_linearImpulse = _linearImpulse,
			_angularImpulse = _angularImpulse,
			Breakpoint = base.Breakpoint
		};
	}

	public override void CopyTo(Joint original)
	{
		if (original is FrictionJoint frictionJoint)
		{
			frictionJoint.Enabled = base.Enabled;
			frictionJoint.MaxTorque = MaxTorque;
			frictionJoint.MaxForce = MaxForce;
			frictionJoint._linearImpulse = _linearImpulse;
			frictionJoint._angularImpulse = _angularImpulse;
			frictionJoint.Breakpoint = base.Breakpoint;
		}
	}

	public bool Equals(FrictionJoint? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (!Equals((Joint?)other))
		{
			return false;
		}
		if (MathHelper.CloseTo(MaxForce, other.MaxForce, 1E-07f))
		{
			return MathHelper.CloseTo(MaxTorque, other.MaxTorque, 1E-07f);
		}
		return false;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FrictionJoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Joint target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (FrictionJoint)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			float target3 = 0f;
			if (!serialization.TryCustomCopy(MaxForce, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = MaxForce;
			}
			target.MaxForce = target3;
			float target4 = 0f;
			if (!serialization.TryCustomCopy(MaxTorque, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = MaxTorque;
			}
			target.MaxTorque = target4;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FrictionJoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Joint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FrictionJoint target2 = (FrictionJoint)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FrictionJoint target2 = (FrictionJoint)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FrictionJoint Instantiate()
	{
		return new FrictionJoint();
	}
}
