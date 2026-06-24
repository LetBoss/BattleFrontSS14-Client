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

public sealed class DistanceJoint : Joint, IEquatable<DistanceJoint>, ISerializationGenerated<DistanceJoint>, ISerializationGenerated
{
	private float _bias;

	private float _gamma;

	private float _impulse;

	private float _lowerImpulse;

	private float _upperImpulse;

	private int _indexA;

	private int _indexB;

	private Vector2 _u;

	private Vector2 _rA;

	private Vector2 _rB;

	private Vector2 _localCenterA;

	private Vector2 _localCenterB;

	private float _invMassA;

	private float _invMassB;

	private float _invIA;

	private float _invIB;

	private float _mass;

	private float _currentLength;

	private float _softMass;

	private float _length;

	private float _maxLength;

	private float _minLength;

	private float _stiffness;

	private float _damping;

	public override JointType JointType => JointType.Distance;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("length", false, 1, false, false, null)]
	public float Length
	{
		get
		{
			return _length;
		}
		set
		{
			if (!MathHelper.CloseTo(value, _length, 1E-07f))
			{
				_impulse = 0f;
				_length = MathF.Max(value, 0.005f);
				Dirty();
			}
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("maxLength", false, 1, false, false, null)]
	public float MaxLength
	{
		get
		{
			return _maxLength;
		}
		set
		{
			if (!MathHelper.CloseTo(value, _maxLength, 1E-07f))
			{
				_upperImpulse = 0f;
				_maxLength = MathF.Max(value, _minLength);
				Dirty();
			}
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("minLength", false, 1, false, false, null)]
	public float MinLength
	{
		get
		{
			return _minLength;
		}
		set
		{
			if (!MathHelper.CloseTo(value, _minLength, 1E-07f))
			{
				_lowerImpulse = 0f;
				_minLength = Math.Clamp(value, 0.005f, MaxLength);
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

	public DistanceJoint()
	{
	}

	public DistanceJoint(EntityUid uidA, EntityUid uidB, Vector2 anchorA, Vector2 anchorB, float length)
		: base(uidA, uidB)
	{
		Length = MathF.Max(0.005f, length);
		_minLength = _length;
		_maxLength = _length;
		base.LocalAnchorA = anchorA;
		base.LocalAnchorB = anchorB;
	}

	internal DistanceJoint(DistanceJointState state, IEntityManager entManager, EntityUid owner)
		: base(state, entManager, owner)
	{
		_damping = state.Damping;
		_length = state.Length;
		_maxLength = state.MaxLength;
		_minLength = state.MinLength;
		_stiffness = state.Stiffness;
	}

	public override Vector2 GetReactionForce(float invDt)
	{
		return _u * invDt * (_impulse + _lowerImpulse - _upperImpulse);
	}

	public override JointState GetState(IEntityManager entManager)
	{
		DistanceJointState distanceJointState = new DistanceJointState
		{
			Damping = _damping,
			Length = _length,
			MinLength = _minLength,
			MaxLength = _maxLength,
			Stiffness = _stiffness,
			LocalAnchorA = base.LocalAnchorA,
			LocalAnchorB = base.LocalAnchorB
		};
		GetState(distanceJointState, entManager);
		return distanceJointState;
	}

	internal override void ApplyState(JointState state)
	{
		base.ApplyState(state);
		if (state is DistanceJointState distanceJointState)
		{
			_damping = distanceJointState.Damping;
			_length = distanceJointState.Length;
			_minLength = distanceJointState.MinLength;
			_maxLength = distanceJointState.MaxLength;
			_stiffness = distanceJointState.Stiffness;
		}
	}

	public override float GetReactionTorque(float invDt)
	{
		return 0f;
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
		Vector2 vector = positions[_indexA];
		float angle = angles[_indexA];
		Vector2 vector2 = linearVelocities[offset + _indexA];
		float num = angularVelocities[offset + _indexA];
		Vector2 vector3 = positions[_indexB];
		float angle2 = angles[_indexB];
		Vector2 vector4 = linearVelocities[offset + _indexB];
		float num2 = angularVelocities[offset + _indexB];
		Quaternion2D quaternion2D = new Quaternion2D(angle);
		Quaternion2D quaternion2D2 = new Quaternion2D(angle2);
		_rA = Transform.Mul(in quaternion2D, base.LocalAnchorA - _localCenterA);
		_rB = Transform.Mul(in quaternion2D2, base.LocalAnchorB - _localCenterB);
		_u = vector3 + _rB - vector - _rA;
		_currentLength = _u.Length();
		if (_currentLength > 0.005f)
		{
			_u *= 1f / _currentLength;
		}
		else
		{
			_u = Vector2.Zero;
			_mass = 0f;
			_impulse = 0f;
			_lowerImpulse = 0f;
			_upperImpulse = 0f;
		}
		float num3 = Vector2Helpers.Cross(_rA, _u);
		float num4 = Vector2Helpers.Cross(_rB, _u);
		float num5 = _invMassA + _invIA * num3 * num3 + _invMassB + _invIB * num4 * num4;
		_mass = ((num5 != 0f) ? (1f / num5) : 0f);
		if (Stiffness > 0f && _minLength < _maxLength)
		{
			float num6 = _currentLength - _length;
			float damping = Damping;
			float stiffness = Stiffness;
			float frameTime = data.FrameTime;
			_gamma = frameTime * (damping + frameTime * stiffness);
			_gamma = ((_gamma != 0f) ? (1f / _gamma) : 0f);
			_bias = num6 * frameTime * stiffness * _gamma;
			num5 += _gamma;
			_softMass = ((num5 != 0f) ? (1f / num5) : 0f);
		}
		else
		{
			_gamma = 0f;
			_bias = 0f;
			_softMass = _mass;
		}
		if (data.WarmStarting)
		{
			_impulse *= data.DtRatio;
			_lowerImpulse *= data.DtRatio;
			_upperImpulse *= data.DtRatio;
			Vector2 vector5 = _u * (_impulse + _lowerImpulse - _upperImpulse);
			vector2 -= vector5 * _invMassA;
			num -= _invIA * Vector2Helpers.Cross(_rA, vector5);
			vector4 += vector5 * _invMassB;
			num2 += _invIB * Vector2Helpers.Cross(_rB, vector5);
		}
		else
		{
			_impulse = 0f;
		}
		linearVelocities[offset + _indexA] = vector2;
		angularVelocities[offset + _indexA] = num;
		linearVelocities[offset + _indexB] = vector4;
		angularVelocities[offset + _indexB] = num2;
	}

	internal override void SolveVelocityConstraints(in SolverData data, in SharedPhysicsSystem.IslandData island, Vector2[] linearVelocities, float[] angularVelocities)
	{
		int offset = island.Offset;
		Vector2 vector = linearVelocities[offset + _indexA];
		float num = angularVelocities[offset + _indexA];
		Vector2 vector2 = linearVelocities[offset + _indexB];
		float num2 = angularVelocities[offset + _indexB];
		if (_minLength < _maxLength)
		{
			if (Stiffness > 0f)
			{
				Vector2 vector3 = vector + Vector2Helpers.Cross(num, ref _rA);
				Vector2 vector4 = vector2 + Vector2Helpers.Cross(num2, ref _rB);
				float num3 = Vector2.Dot(_u, vector4 - vector3);
				float num4 = (0f - _softMass) * (num3 + _bias + _gamma * _impulse);
				_impulse += num4;
				Vector2 vector5 = _u * num4;
				vector -= vector5 * _invMassA;
				num -= _invIA * Vector2Helpers.Cross(_rA, vector5);
				vector2 += vector5 * _invMassB;
				num2 += _invIB * Vector2Helpers.Cross(_rB, vector5);
			}
			float y = _currentLength - _minLength;
			float num5 = MathF.Max(0f, y) * data.InvDt;
			Vector2 vector6 = vector + Vector2Helpers.Cross(num, ref _rA);
			Vector2 vector7 = vector2 + Vector2Helpers.Cross(num2, ref _rB);
			float num6 = Vector2.Dot(_u, vector7 - vector6);
			float num7 = (0f - _mass) * (num6 + num5);
			float lowerImpulse = _lowerImpulse;
			_lowerImpulse = MathF.Max(0f, _lowerImpulse + num7);
			num7 = _lowerImpulse - lowerImpulse;
			Vector2 vector8 = _u * num7;
			vector -= vector8 * _invMassA;
			num -= _invIA * Vector2Helpers.Cross(_rA, vector8);
			vector2 += vector8 * _invMassB;
			num2 += _invIB * Vector2Helpers.Cross(_rB, vector8);
			float y2 = _maxLength - _currentLength;
			float num8 = MathF.Max(0f, y2) * data.InvDt;
			Vector2 vector9 = vector + Vector2Helpers.Cross(num, ref _rA);
			Vector2 vector10 = vector2 + Vector2Helpers.Cross(num2, ref _rB);
			float num9 = Vector2.Dot(_u, vector9 - vector10);
			float num10 = (0f - _mass) * (num9 + num8);
			float upperImpulse = _upperImpulse;
			_upperImpulse = MathF.Max(0f, _upperImpulse + num10);
			num10 = _upperImpulse - upperImpulse;
			Vector2 vector11 = _u * (0f - num10);
			vector -= vector11 * _invMassA;
			num -= _invIA * Vector2Helpers.Cross(_rA, vector11);
			vector2 += vector11 * _invMassB;
			num2 += _invIB * Vector2Helpers.Cross(_rB, vector11);
		}
		else
		{
			Vector2 vector12 = vector + Vector2Helpers.Cross(num, ref _rA);
			Vector2 vector13 = vector2 + Vector2Helpers.Cross(num2, ref _rB);
			float num11 = Vector2.Dot(_u, vector13 - vector12);
			float num12 = (0f - _mass) * num11;
			_impulse += num12;
			Vector2 vector14 = _u * num12;
			vector -= vector14 * _invMassA;
			num -= _invIA * Vector2Helpers.Cross(_rA, vector14);
			vector2 += vector14 * _invMassB;
			num2 += _invIB * Vector2Helpers.Cross(_rB, vector14);
		}
		linearVelocities[offset + _indexA] = vector;
		angularVelocities[offset + _indexA] = num;
		linearVelocities[offset + _indexB] = vector2;
		angularVelocities[offset + _indexB] = num2;
	}

	internal override bool SolvePositionConstraints(in SolverData data, Vector2[] positions, float[] angles)
	{
		Vector2 vector = positions[_indexA];
		float num = angles[_indexA];
		Vector2 vector2 = positions[_indexB];
		float num2 = angles[_indexB];
		Quaternion2D quaternion2D = new Quaternion2D(num);
		Quaternion2D quaternion2D2 = new Quaternion2D(num2);
		Vector2 vector3 = Transform.Mul(in quaternion2D, base.LocalAnchorA - _localCenterA);
		Vector2 vector4 = Transform.Mul(in quaternion2D2, base.LocalAnchorB - _localCenterB);
		Vector2 vector5 = vector2 + vector4 - vector - vector3;
		float num3 = vector5.Length();
		vector5 = Vector2Helpers.Normalized(vector5);
		float num4;
		if (MathHelper.CloseTo(_minLength, _maxLength, 1E-07f))
		{
			num4 = num3 - _minLength;
		}
		else if (num3 < _minLength)
		{
			num4 = num3 - _minLength;
		}
		else
		{
			if (!(_maxLength < num3))
			{
				return true;
			}
			num4 = num3 - _maxLength;
		}
		float num5 = (0f - _mass) * num4;
		Vector2 vector6 = vector5 * num5;
		vector -= vector6 * _invMassA;
		num -= _invIA * Vector2Helpers.Cross(vector3, vector6);
		vector2 += vector6 * _invMassB;
		num2 += _invIB * Vector2Helpers.Cross(vector4, vector6);
		positions[_indexA] = vector;
		angles[_indexA] = num;
		positions[_indexB] = vector2;
		angles[_indexB] = num2;
		return MathF.Abs(num4) < 0.005f;
	}

	public override Joint Clone(EntityUid uidA, EntityUid uidB)
	{
		return new DistanceJoint(uidA, uidB, base.LocalAnchorA, base.LocalAnchorB, Length)
		{
			Enabled = base.Enabled,
			MinLength = MinLength,
			MaxLength = MaxLength,
			Stiffness = Stiffness,
			Damping = Damping,
			_lowerImpulse = _lowerImpulse,
			_upperImpulse = _upperImpulse,
			_impulse = _impulse,
			Breakpoint = base.Breakpoint
		};
	}

	public override void CopyTo(Joint original)
	{
		if (original is DistanceJoint distanceJoint)
		{
			distanceJoint.Enabled = base.Enabled;
			distanceJoint.MinLength = MinLength;
			distanceJoint.MaxLength = MaxLength;
			distanceJoint.Length = Length;
			distanceJoint.Stiffness = Stiffness;
			distanceJoint.Damping = Damping;
			distanceJoint._lowerImpulse = _lowerImpulse;
			distanceJoint._upperImpulse = _upperImpulse;
			distanceJoint._impulse = _impulse;
			distanceJoint.Breakpoint = base.Breakpoint;
		}
	}

	public bool Equals(DistanceJoint? other)
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
		if (MathHelper.CloseTo(_length, other._length, 1E-07f) && MathHelper.CloseTo(_minLength, other._minLength, 1E-07f) && MathHelper.CloseTo(_maxLength, other._maxLength, 1E-07f) && MathHelper.CloseTo(_stiffness, other._stiffness, 1E-07f))
		{
			return MathHelper.CloseTo(_damping, other._damping, 1E-07f);
		}
		return false;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DistanceJoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Joint target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (DistanceJoint)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			float target3 = 0f;
			if (!serialization.TryCustomCopy(Length, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = Length;
			}
			target.Length = target3;
			float target4 = 0f;
			if (!serialization.TryCustomCopy(MaxLength, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = MaxLength;
			}
			target.MaxLength = target4;
			float target5 = 0f;
			if (!serialization.TryCustomCopy(MinLength, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = MinLength;
			}
			target.MinLength = target5;
			float target6 = 0f;
			if (!serialization.TryCustomCopy(Stiffness, ref target6, hookCtx, hasHooks: false, context))
			{
				target6 = Stiffness;
			}
			target.Stiffness = target6;
			float target7 = 0f;
			if (!serialization.TryCustomCopy(Damping, ref target7, hookCtx, hasHooks: false, context))
			{
				target7 = Damping;
			}
			target.Damping = target7;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DistanceJoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Joint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DistanceJoint target2 = (DistanceJoint)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DistanceJoint target2 = (DistanceJoint)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DistanceJoint Instantiate()
	{
		return new DistanceJoint();
	}
}
