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

namespace Robust.Shared.Physics.Dynamics.Joints;

public sealed class RevoluteJoint : Joint, IEquatable<RevoluteJoint>, ISerializationGenerated<RevoluteJoint>, ISerializationGenerated
{
	private Vector2 _impulse;

	private int _indexA;

	private int _indexB;

	private Vector2 _localCenterA;

	private Vector2 _localCenterB;

	private float _invMassA;

	private float _invMassB;

	private float _invIA;

	private float _invIB;

	private Vector2 _rA;

	private Vector2 _rB;

	private Matrix22 _K;

	private float _axialMass;

	private float _angle;

	private float _motorImpulse;

	private float _lowerImpulse;

	private float _upperImpulse;

	[DataField("enableLimit", false, 1, false, false, null)]
	public bool EnableLimit;

	[DataField("enableMotor", false, 1, false, false, null)]
	public bool EnableMotor;

	[DataField("referenceAngle", false, 1, false, false, null)]
	public float ReferenceAngle;

	[DataField("lowerAngle", false, 1, false, false, null)]
	public float LowerAngle;

	[DataField("upperAngle", false, 1, false, false, null)]
	public float UpperAngle;

	[DataField("motorSpeed", false, 1, false, false, null)]
	public float MotorSpeed;

	[DataField("maxMotorTorque", false, 1, false, false, null)]
	public float MaxMotorTorque;

	public override JointType JointType => JointType.Revolute;

	public RevoluteJoint()
	{
	}

	public RevoluteJoint(EntityUid uidA, EntityUid uidB, Vector2 anchorA, Vector2 anchorB, float referenceAngle)
		: base(uidA, uidB)
	{
		base.LocalAnchorA = anchorA;
		base.LocalAnchorB = anchorB;
		ReferenceAngle = referenceAngle;
	}

	public RevoluteJoint(EntityUid bodyAUid, EntityUid bodyBUid)
		: base(bodyAUid, bodyBUid)
	{
	}

	internal RevoluteJoint(RevoluteJointState state, IEntityManager entManager, EntityUid owner)
		: base(state, entManager, owner)
	{
		EnableLimit = state.EnableLimit;
		EnableMotor = state.EnableMotor;
		ReferenceAngle = state.ReferenceAngle;
		LowerAngle = state.LowerAngle;
		UpperAngle = state.UpperAngle;
		MotorSpeed = state.MotorSpeed;
		MaxMotorTorque = state.MaxMotorTorque;
	}

	public override JointState GetState(IEntityManager entManager)
	{
		RevoluteJointState revoluteJointState = new RevoluteJointState();
		GetState(revoluteJointState, entManager);
		return revoluteJointState;
	}

	internal override void ApplyState(JointState state)
	{
		base.ApplyState(state);
		if (state is RevoluteJointState revoluteJointState)
		{
			EnableLimit = revoluteJointState.EnableLimit;
			EnableMotor = revoluteJointState.EnableMotor;
			LowerAngle = revoluteJointState.LowerAngle;
			MotorSpeed = revoluteJointState.MotorSpeed;
			ReferenceAngle = revoluteJointState.ReferenceAngle;
			UpperAngle = revoluteJointState.UpperAngle;
			MaxMotorTorque = revoluteJointState.MaxMotorTorque;
		}
	}

	public override Vector2 GetReactionForce(float invDt)
	{
		return new Vector2(_impulse.X, _impulse.Y) * invDt;
	}

	public override float GetReactionTorque(float invDt)
	{
		return invDt * (_motorImpulse + _lowerImpulse - _upperImpulse);
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
		float num = angles[_indexA];
		Vector2 vector = linearVelocities[offset + _indexA];
		float num2 = angularVelocities[offset + _indexA];
		float num3 = angles[_indexB];
		Vector2 vector2 = linearVelocities[offset + _indexB];
		float num4 = angularVelocities[offset + _indexB];
		Quaternion2D quaternion2D = new Quaternion2D(num);
		Quaternion2D quaternion2D2 = new Quaternion2D(num3);
		_rA = Transform.Mul(in quaternion2D, base.LocalAnchorA - _localCenterA);
		_rB = Transform.Mul(in quaternion2D2, base.LocalAnchorB - _localCenterB);
		float invMassA = _invMassA;
		float invMassB = _invMassB;
		float invIA = _invIA;
		float invIB = _invIB;
		_K.EX.X = invMassA + invMassB + _rA.Y * _rA.Y * invIA + _rB.Y * _rB.Y * invIB;
		_K.EY.X = (0f - _rA.Y) * _rA.X * invIA - _rB.Y * _rB.X * invIB;
		_K.EX.Y = _K.EY.X;
		_K.EY.Y = invMassA + invMassB + _rA.X * _rA.X * invIA + _rB.X * _rB.X * invIB;
		_axialMass = invIA + invIB;
		bool flag;
		if (_axialMass > 0f)
		{
			_axialMass = 1f / _axialMass;
			flag = false;
		}
		else
		{
			flag = true;
		}
		_angle = num3 - num - ReferenceAngle;
		if (!EnableLimit || flag)
		{
			_lowerImpulse = 0f;
			_upperImpulse = 0f;
		}
		if (!EnableMotor || flag)
		{
			_motorImpulse = 0f;
		}
		if (data.WarmStarting)
		{
			_impulse *= data.DtRatio;
			_motorImpulse *= data.DtRatio;
			_lowerImpulse *= data.DtRatio;
			_upperImpulse *= data.DtRatio;
			float num5 = _motorImpulse + _lowerImpulse - _upperImpulse;
			Vector2 vector3 = new Vector2(_impulse.X, _impulse.Y);
			vector -= vector3 * invMassA;
			num2 -= invIA * (Vector2Helpers.Cross(_rA, vector3) + num5);
			vector2 += vector3 * invMassB;
			num4 += invIB * (Vector2Helpers.Cross(_rB, vector3) + num5);
		}
		else
		{
			_impulse = Vector2.Zero;
			_motorImpulse = 0f;
			_lowerImpulse = 0f;
			_upperImpulse = 0f;
		}
		linearVelocities[offset + _indexA] = vector;
		angularVelocities[offset + _indexA] = num2;
		linearVelocities[offset + _indexB] = vector2;
		angularVelocities[offset + _indexB] = num4;
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
		bool flag = invIA + invIB == 0f;
		if (EnableMotor && !flag)
		{
			float num3 = num2 - num - MotorSpeed;
			float num4 = (0f - _axialMass) * num3;
			float motorImpulse = _motorImpulse;
			float num5 = data.FrameTime * MaxMotorTorque;
			_motorImpulse = Math.Clamp(_motorImpulse + num4, 0f - num5, num5);
			num4 = _motorImpulse - motorImpulse;
			num -= invIA * num4;
			num2 += invIB * num4;
		}
		if (EnableLimit && !flag)
		{
			float x = _angle - LowerAngle;
			float num6 = num2 - num;
			float num7 = (0f - _axialMass) * (num6 + MathF.Max(x, 0f) * data.InvDt);
			float lowerImpulse = _lowerImpulse;
			_lowerImpulse = MathF.Max(_lowerImpulse + num7, 0f);
			num7 = _lowerImpulse - lowerImpulse;
			num -= invIA * num7;
			num2 += invIB * num7;
			float x2 = UpperAngle - _angle;
			float num8 = num - num2;
			float num9 = (0f - _axialMass) * (num8 + MathF.Max(x2, 0f) * data.InvDt);
			float upperImpulse = _upperImpulse;
			_upperImpulse = MathF.Max(_upperImpulse + num9, 0f);
			num9 = _upperImpulse - upperImpulse;
			num += invIA * num9;
			num2 -= invIB * num9;
		}
		Vector2 vector3 = vector2 + Vector2Helpers.Cross(num2, ref _rB) - vector - Vector2Helpers.Cross(num, ref _rA);
		Vector2 vector4 = ((Matrix22)(ref _K)).Solve(-vector3);
		_impulse.X += vector4.X;
		_impulse.Y += vector4.Y;
		vector -= vector4 * invMassA;
		num -= invIA * Vector2Helpers.Cross(_rA, vector4);
		vector2 += vector4 * invMassB;
		num2 += invIB * Vector2Helpers.Cross(_rB, vector4);
		linearVelocities[offset + _indexA] = vector;
		angularVelocities[offset + _indexA] = num;
		linearVelocities[offset + _indexB] = vector2;
		angularVelocities[offset + _indexB] = num2;
	}

	internal override bool SolvePositionConstraints(in SolverData data, Vector2[] positions, float[] angles)
	{
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = positions[_indexA];
		float num = angles[_indexA];
		Vector2 vector2 = positions[_indexB];
		float num2 = angles[_indexB];
		Quaternion2D quaternion2D = new Quaternion2D(num);
		Quaternion2D quaternion2D2 = new Quaternion2D(num2);
		float num3 = 0f;
		bool flag = _invIA + _invIB == 0f;
		if (EnableLimit && !flag)
		{
			float num4 = num2 - num - ReferenceAngle;
			float num5 = 0f;
			if (Math.Abs(UpperAngle - LowerAngle) < 0.06981318f)
			{
				num5 = Math.Clamp(num4 - LowerAngle, 0f - data.MaxAngularCorrection, data.MaxAngularCorrection);
			}
			else if (num4 <= LowerAngle)
			{
				num5 = Math.Clamp(num4 - LowerAngle + 0.03490659f, 0f - data.MaxAngularCorrection, 0f);
			}
			else if (num4 >= UpperAngle)
			{
				num5 = Math.Clamp(num4 - UpperAngle - 0.03490659f, 0f, data.MaxAngularCorrection);
			}
			float num6 = (0f - _axialMass) * num5;
			num -= _invIA * num6;
			num2 += _invIB * num6;
			num3 = Math.Abs(num5);
		}
		quaternion2D.Set(num);
		quaternion2D2.Set(num2);
		Vector2 vector3 = Transform.Mul(in quaternion2D, base.LocalAnchorA - _localCenterA);
		Vector2 vector4 = Transform.Mul(in quaternion2D2, base.LocalAnchorB - _localCenterB);
		Vector2 vector5 = vector2 + vector4 - vector - vector3;
		float num7 = vector5.Length();
		float invMassA = _invMassA;
		float invMassB = _invMassB;
		float invIA = _invIA;
		float invIB = _invIB;
		Unsafe.SkipInit(out Matrix22 val);
		((Matrix22)(ref val))._002Ector(invMassA + invMassB + invIA * vector3.Y * vector3.Y + invIB * vector4.Y * vector4.Y, (0f - invIA) * vector3.X * vector3.Y - invIB * vector4.X * vector4.Y, 0f, invMassA + invMassB + invIA * vector3.X * vector3.X + invIB * vector4.X * vector4.X);
		val.EY.X = val.EX.Y;
		Vector2 vector6 = -((Matrix22)(ref val)).Solve(vector5);
		vector -= vector6 * invMassA;
		num -= invIA * Vector2Helpers.Cross(vector3, vector6);
		vector2 += vector6 * invMassB;
		num2 += invIB * Vector2Helpers.Cross(vector4, vector6);
		positions[_indexA] = vector;
		angles[_indexA] = num;
		positions[_indexB] = vector2;
		angles[_indexB] = num2;
		if (num7 <= 0.005f)
		{
			return num3 <= 0.03490659f;
		}
		return false;
	}

	public bool Equals(RevoluteJoint? other)
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
		if (EnableLimit == other.EnableLimit && EnableMotor == other.EnableMotor && MathHelper.CloseTo(ReferenceAngle, other.ReferenceAngle, 1E-07f) && MathHelper.CloseTo(LowerAngle, other.LowerAngle, 1E-07f) && MathHelper.CloseTo(UpperAngle, other.UpperAngle, 1E-07f) && MathHelper.CloseTo(MotorSpeed, other.MotorSpeed, 1E-07f))
		{
			return MathHelper.CloseTo(MaxMotorTorque, other.MaxMotorTorque, 1E-07f);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((RevoluteJoint)obj);
	}

	public override int GetHashCode()
	{
		HashCode hashCode = default(HashCode);
		hashCode.Add(base.BodyAUid);
		hashCode.Add(base.BodyBUid);
		return hashCode.ToHashCode();
	}

	public override Joint Clone(EntityUid uidA, EntityUid uidB)
	{
		return new RevoluteJoint(uidA, uidB, base.LocalAnchorA, base.LocalAnchorB, ReferenceAngle)
		{
			Enabled = base.Enabled,
			EnableLimit = EnableLimit,
			EnableMotor = EnableMotor,
			LowerAngle = LowerAngle,
			UpperAngle = UpperAngle,
			MaxMotorTorque = MaxMotorTorque,
			MotorSpeed = MotorSpeed,
			_impulse = _impulse,
			_upperImpulse = _upperImpulse,
			_lowerImpulse = _lowerImpulse,
			_motorImpulse = _motorImpulse,
			Breakpoint = base.Breakpoint
		};
	}

	public override void CopyTo(Joint original)
	{
		if (original is RevoluteJoint revoluteJoint)
		{
			revoluteJoint.Enabled = base.Enabled;
			revoluteJoint.EnableLimit = EnableLimit;
			revoluteJoint.EnableMotor = EnableMotor;
			revoluteJoint.LowerAngle = LowerAngle;
			revoluteJoint.UpperAngle = UpperAngle;
			revoluteJoint.MaxMotorTorque = MaxMotorTorque;
			revoluteJoint.MotorSpeed = MotorSpeed;
			revoluteJoint._impulse = _impulse;
			revoluteJoint._upperImpulse = _upperImpulse;
			revoluteJoint._lowerImpulse = _lowerImpulse;
			revoluteJoint._motorImpulse = _motorImpulse;
			revoluteJoint.Breakpoint = base.Breakpoint;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RevoluteJoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Joint target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (RevoluteJoint)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			bool target3 = false;
			if (!serialization.TryCustomCopy(EnableLimit, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = EnableLimit;
			}
			target.EnableLimit = target3;
			bool target4 = false;
			if (!serialization.TryCustomCopy(EnableMotor, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = EnableMotor;
			}
			target.EnableMotor = target4;
			float target5 = 0f;
			if (!serialization.TryCustomCopy(ReferenceAngle, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = ReferenceAngle;
			}
			target.ReferenceAngle = target5;
			float target6 = 0f;
			if (!serialization.TryCustomCopy(LowerAngle, ref target6, hookCtx, hasHooks: false, context))
			{
				target6 = LowerAngle;
			}
			target.LowerAngle = target6;
			float target7 = 0f;
			if (!serialization.TryCustomCopy(UpperAngle, ref target7, hookCtx, hasHooks: false, context))
			{
				target7 = UpperAngle;
			}
			target.UpperAngle = target7;
			float target8 = 0f;
			if (!serialization.TryCustomCopy(MotorSpeed, ref target8, hookCtx, hasHooks: false, context))
			{
				target8 = MotorSpeed;
			}
			target.MotorSpeed = target8;
			float target9 = 0f;
			if (!serialization.TryCustomCopy(MaxMotorTorque, ref target9, hookCtx, hasHooks: false, context))
			{
				target9 = MaxMotorTorque;
			}
			target.MaxMotorTorque = target9;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RevoluteJoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Joint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RevoluteJoint target2 = (RevoluteJoint)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RevoluteJoint target2 = (RevoluteJoint)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RevoluteJoint Instantiate()
	{
		return new RevoluteJoint();
	}
}
