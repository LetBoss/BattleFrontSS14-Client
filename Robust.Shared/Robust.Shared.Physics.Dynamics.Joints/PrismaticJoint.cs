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

public sealed class PrismaticJoint : Joint, IEquatable<PrismaticJoint>, ISerializationGenerated<PrismaticJoint>, ISerializationGenerated
{
	private Vector2 _localAxisA;

	[DataField("referenceANgle", false, 1, false, false, null)]
	public float ReferenceAngle;

	[DataField("enableLimit", false, 1, false, false, null)]
	public bool EnableLimit;

	[DataField("lowerTranslation", false, 1, false, false, null)]
	public float LowerTranslation;

	[DataField("upperTranslation", false, 1, false, false, null)]
	public float UpperTranslation;

	[DataField("enableMotor", false, 1, false, false, null)]
	public bool EnableMotor;

	[DataField("maxMotorForce", false, 1, false, false, null)]
	public float MaxMotorForce;

	[DataField("motorSpeed", false, 1, false, false, null)]
	public float MotorSpeed;

	internal Vector2 _localXAxisA;

	internal Vector2 _localYAxisA;

	private Vector2 _impulse;

	private float _motorImpulse;

	private float _lowerImpulse;

	private float _upperImpulse;

	private int _indexA;

	private int _indexB;

	private Vector2 _localCenterA;

	private Vector2 _localCenterB;

	private float _invMassA;

	private float _invMassB;

	private float _invIA;

	private float _invIB;

	private Vector2 _axis;

	private Vector2 _perp;

	private float _s1;

	private float _s2;

	private float _a1;

	private float _a2;

	private Matrix22 _K;

	private float _translation;

	private float _axialMass;

	[DataField("localAxisA", false, 1, false, false, null)]
	public Vector2 LocalAxisA
	{
		get
		{
			return _localAxisA;
		}
		set
		{
			_localAxisA = value;
			_localXAxisA = Vector2Helpers.Normalized(value);
			_localYAxisA = Vector2Helpers.Cross(1f, ref _localXAxisA);
		}
	}

	public override JointType JointType => JointType.Prismatic;

	public PrismaticJoint()
	{
	}

	internal PrismaticJoint(EntityUid bodyAUid, EntityUid bodyBUid)
		: base(bodyAUid, bodyBUid)
	{
		LocalAxisA = new Vector2(1f, 0f);
	}

	public PrismaticJoint(EntityUid bodyAUid, EntityUid bodyBUid, Vector2 anchorA, Vector2 anchorB, Vector2 axis, float referenceAngle)
		: base(bodyAUid, bodyBUid)
	{
		base.LocalAnchorA = anchorA;
		base.LocalAnchorB = anchorB;
		LocalAxisA = axis;
		ReferenceAngle = referenceAngle;
	}

	internal PrismaticJoint(PrismaticJointState state, IEntityManager entManager, EntityUid owner)
		: base(state, entManager, owner)
	{
		LocalAxisA = state.LocalAxisA;
		ReferenceAngle = state.ReferenceAngle;
		EnableLimit = state.EnableLimit;
		LowerTranslation = state.LowerTranslation;
		UpperTranslation = state.UpperTranslation;
		EnableMotor = state.EnableMotor;
		MaxMotorForce = state.MaxMotorForce;
		MotorSpeed = state.MotorSpeed;
	}

	public override JointState GetState(IEntityManager entManager)
	{
		PrismaticJointState prismaticJointState = new PrismaticJointState
		{
			LocalAnchorA = base.LocalAnchorA,
			LocalAnchorB = base.LocalAnchorB
		};
		GetState(prismaticJointState, entManager);
		return prismaticJointState;
	}

	public override Vector2 GetReactionForce(float invDt)
	{
		return (_perp * _impulse.X + _axis * (_motorImpulse + _lowerImpulse - _upperImpulse)) * invDt;
	}

	public override float GetReactionTorque(float invDt)
	{
		return invDt * _impulse.Y;
	}

	internal override void InitVelocityConstraints(in SolverData data, in SharedPhysicsSystem.IslandData island, PhysicsComponent bodyA, PhysicsComponent bodyB, Vector2[] positions, float[] angles, Vector2[] linearVelocities, float[] angularVelocities)
	{
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
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
		Vector2 vector5 = Transform.Mul(in quaternion2D, base.LocalAnchorA - _localCenterA);
		Vector2 vector6 = Transform.Mul(in quaternion2D2, base.LocalAnchorB - _localCenterB);
		Vector2 vector7 = vector3 - vector + vector6 - vector5;
		float invMassA = _invMassA;
		float invMassB = _invMassB;
		float invIA = _invIA;
		float invIB = _invIB;
		_axis = Transform.Mul(in quaternion2D, in _localXAxisA);
		_a1 = Vector2Helpers.Cross(vector7 + vector5, _axis);
		_a2 = Vector2Helpers.Cross(vector6, _axis);
		_axialMass = invMassA + invMassB + invIA * _a1 * _a1 + invIB * _a2 * _a2;
		if (_axialMass > 0f)
		{
			_axialMass = 1f / _axialMass;
		}
		_perp = Transform.Mul(in quaternion2D, in _localYAxisA);
		_s1 = Vector2Helpers.Cross(vector7 + vector5, _perp);
		_s2 = Vector2Helpers.Cross(vector6, _perp);
		float x = invMassA + invMassB + invIA * _s1 * _s1 + invIB * _s2 * _s2;
		float num3 = invIA * _s1 + invIB * _s2;
		float num4 = invIA + invIB;
		if (num4 == 0f)
		{
			num4 = 1f;
		}
		_K = new Matrix22(new Vector2(x, num3), new Vector2(num3, num4));
		if (EnableLimit)
		{
			_translation = Vector2.Dot(_axis, vector7);
		}
		else
		{
			_lowerImpulse = 0f;
			_upperImpulse = 0f;
		}
		if (!EnableMotor)
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
			Vector2 vector8 = _perp * _impulse.X + _axis * num5;
			float num6 = _impulse.X * _s1 + _impulse.Y + num5 * _a1;
			float num7 = _impulse.X * _s2 + _impulse.Y + num5 * _a2;
			vector2 -= vector8 * invMassA;
			num -= invIA * num6;
			vector4 += vector8 * invMassB;
			num2 += invIB * num7;
		}
		else
		{
			_impulse = Vector2.Zero;
			_motorImpulse = 0f;
			_lowerImpulse = 0f;
			_upperImpulse = 0f;
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
		float invMassA = _invMassA;
		float invMassB = _invMassB;
		float invIA = _invIA;
		float invIB = _invIB;
		if (EnableMotor)
		{
			float num3 = Vector2.Dot(_axis, vector2 - vector) + _a2 * num2 - _a1 * num;
			float num4 = _axialMass * (MotorSpeed - num3);
			float motorImpulse = _motorImpulse;
			float num5 = data.FrameTime * MaxMotorForce;
			_motorImpulse = Math.Clamp(_motorImpulse + num4, 0f - num5, num5);
			num4 = _motorImpulse - motorImpulse;
			Vector2 vector3 = _axis * num4;
			float num6 = num4 * _a1;
			float num7 = num4 * _a2;
			vector -= vector3 * invMassA;
			num -= invIA * num6;
			vector2 += vector3 * invMassB;
			num2 += invIB * num7;
		}
		if (EnableLimit)
		{
			float x = _translation - LowerTranslation;
			float num8 = Vector2.Dot(_axis, vector2 - vector) + _a2 * num2 - _a1 * num;
			float num9 = (0f - _axialMass) * (num8 + MathF.Max(x, 0f) * data.InvDt);
			float lowerImpulse = _lowerImpulse;
			_lowerImpulse = MathF.Max(_lowerImpulse + num9, 0f);
			num9 = _lowerImpulse - lowerImpulse;
			Vector2 vector4 = _axis * num9;
			float num10 = num9 * _a1;
			float num11 = num9 * _a2;
			vector -= vector4 * invMassA;
			num -= invIA * num10;
			vector2 += vector4 * invMassB;
			num2 += invIB * num11;
			float x2 = UpperTranslation - _translation;
			float num12 = Vector2.Dot(_axis, vector - vector2) + _a1 * num - _a2 * num2;
			float num13 = (0f - _axialMass) * (num12 + MathF.Max(x2, 0f) * data.InvDt);
			float upperImpulse = _upperImpulse;
			_upperImpulse = MathF.Max(_upperImpulse + num13, 0f);
			num13 = _upperImpulse - upperImpulse;
			Vector2 vector5 = _axis * num13;
			float num14 = num13 * _a1;
			float num15 = num13 * _a2;
			vector += vector5 * invMassA;
			num += invIA * num14;
			vector2 -= vector5 * invMassB;
			num2 -= invIB * num15;
		}
		Unsafe.SkipInit(out Vector2 vector6);
		vector6.X = Vector2.Dot(_perp, vector2 - vector) + _s2 * num2 - _s1 * num;
		vector6.Y = num2 - num;
		Vector2 vector7 = ((Matrix22)(ref _K)).Solve(-vector6);
		_impulse += vector7;
		Vector2 vector8 = _perp * vector7.X;
		float num16 = vector7.X * _s1 + vector7.Y;
		float num17 = vector7.X * _s2 + vector7.Y;
		vector -= vector8 * invMassA;
		num -= invIA * num16;
		vector2 += vector8 * invMassB;
		num2 += invIB * num17;
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
		float invMassA = _invMassA;
		float invMassB = _invMassB;
		float invIA = _invIA;
		float invIB = _invIB;
		Vector2 vector3 = Transform.Mul(in quaternion2D, base.LocalAnchorA - _localCenterA);
		Vector2 vector4 = Transform.Mul(in quaternion2D2, base.LocalAnchorB - _localCenterB);
		Vector2 vector5 = vector2 + vector4 - vector - vector3;
		Vector2 vector6 = Transform.Mul(in quaternion2D, in _localXAxisA);
		float num3 = Vector2Helpers.Cross(vector5 + vector3, vector6);
		float num4 = Vector2Helpers.Cross(vector4, vector6);
		Vector2 vector7 = Transform.Mul(in quaternion2D, in _localYAxisA);
		float num5 = Vector2Helpers.Cross(vector5 + vector3, vector7);
		float num6 = Vector2Helpers.Cross(vector4, vector7);
		Unsafe.SkipInit(out Vector2 vector8);
		vector8.X = Vector2.Dot(vector7, vector5);
		vector8.Y = num2 - num - ReferenceAngle;
		float num7 = MathF.Abs(vector8.X);
		float num8 = MathF.Abs(vector8.Y);
		bool flag = false;
		float z = 0f;
		if (EnableLimit)
		{
			float num9 = Vector2.Dot(vector6, vector5);
			if (MathF.Abs(UpperTranslation - LowerTranslation) < 0.01f)
			{
				z = num9;
				num7 = MathF.Max(num7, MathF.Abs(num9));
				flag = true;
			}
			else if (num9 <= LowerTranslation)
			{
				z = MathF.Min(num9 - LowerTranslation, 0f);
				num7 = MathF.Max(num7, LowerTranslation - num9);
				flag = true;
			}
			else if (num9 >= UpperTranslation)
			{
				z = MathF.Max(num9 - UpperTranslation, 0f);
				num7 = MathF.Max(num7, num9 - UpperTranslation);
				flag = true;
			}
		}
		Unsafe.SkipInit(out Vector3 vector10);
		if (flag)
		{
			float x = invMassA + invMassB + invIA * num5 * num5 + invIB * num6 * num6;
			float num10 = invIA * num5 + invIB * num6;
			float num11 = invIA * num5 * num3 + invIB * num6 * num4;
			float num12 = invIA + invIB;
			if (num12 == 0f)
			{
				num12 = 1f;
			}
			float num13 = invIA * num3 + invIB * num4;
			float z2 = invMassA + invMassB + invIA * num3 * num3 + invIB * num4 * num4;
			Unsafe.SkipInit(out Matrix33 val);
			((Matrix33)(ref val))._002Ector(new Vector3(x, num10, num11), new Vector3(num10, num12, num13), new Vector3(num11, num13, z2));
			Unsafe.SkipInit(out Vector3 vector9);
			vector9.X = vector8.X;
			vector9.Y = vector8.Y;
			vector9.Z = z;
			vector10 = ((Matrix33)(ref val)).Solve33(-vector9);
		}
		else
		{
			float num14 = invMassA + invMassB + invIA * num5 * num5 + invIB * num6 * num6;
			float num15 = invIA * num5 + invIB * num6;
			float num16 = invIA + invIB;
			if (num16 == 0f)
			{
				num16 = 1f;
			}
			Unsafe.SkipInit(out Matrix22 val2);
			((Matrix22)(ref val2))._002Ector(num14, num15, num15, num16);
			Vector2 vector11 = ((Matrix22)(ref val2)).Solve(-vector8);
			vector10.X = vector11.X;
			vector10.Y = vector11.Y;
			vector10.Z = 0f;
		}
		Vector2 vector12 = vector7 * vector10.X + vector6 * vector10.Z;
		float num17 = vector10.X * num5 + vector10.Y + vector10.Z * num3;
		float num18 = vector10.X * num6 + vector10.Y + vector10.Z * num4;
		vector -= vector12 * invMassA;
		num -= invIA * num17;
		vector2 += vector12 * invMassB;
		num2 += invIB * num18;
		positions[_indexA] = vector;
		angles[_indexA] = num;
		positions[_indexB] = vector2;
		angles[_indexB] = num2;
		if (num7 <= 0.005f)
		{
			return num8 <= 0.03490659f;
		}
		return false;
	}

	public override Joint Clone(EntityUid uidA, EntityUid uidB)
	{
		return new PrismaticJoint(uidA, uidB, base.LocalAnchorA, base.LocalAnchorB, LocalAxisA, ReferenceAngle)
		{
			EnableLimit = EnableLimit,
			LowerTranslation = LowerTranslation,
			UpperTranslation = UpperTranslation,
			EnableMotor = EnableMotor,
			MaxMotorForce = MaxMotorForce,
			MotorSpeed = MotorSpeed,
			_impulse = _impulse,
			_lowerImpulse = _lowerImpulse,
			_upperImpulse = _upperImpulse,
			_motorImpulse = _motorImpulse,
			Breakpoint = base.Breakpoint
		};
	}

	public override void CopyTo(Joint original)
	{
		if (original is PrismaticJoint prismaticJoint)
		{
			prismaticJoint.EnableLimit = EnableLimit;
			prismaticJoint.LowerTranslation = LowerTranslation;
			prismaticJoint.UpperTranslation = UpperTranslation;
			prismaticJoint.EnableMotor = EnableMotor;
			prismaticJoint.MaxMotorForce = MaxMotorForce;
			prismaticJoint.MotorSpeed = MotorSpeed;
			prismaticJoint._impulse = _impulse;
			prismaticJoint._lowerImpulse = _lowerImpulse;
			prismaticJoint._upperImpulse = _upperImpulse;
			prismaticJoint._motorImpulse = _motorImpulse;
			prismaticJoint.Breakpoint = base.Breakpoint;
		}
	}

	public bool Equals(PrismaticJoint? other)
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
		if (EnableLimit.Equals(other.EnableLimit) && EnableMotor.Equals(other.EnableMotor) && Vector2Helpers.EqualsApprox(LocalAxisA, other.LocalAxisA) && MathHelper.CloseTo(ReferenceAngle, other.ReferenceAngle, 1E-07f) && MathHelper.CloseTo(LowerTranslation, other.LowerTranslation, 1E-07f) && MathHelper.CloseTo(UpperTranslation, other.UpperTranslation, 1E-07f) && MathHelper.CloseTo(MaxMotorForce, other.MaxMotorForce, 1E-07f))
		{
			return MathHelper.CloseTo(MotorSpeed, other.MotorSpeed, 1E-07f);
		}
		return false;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PrismaticJoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Joint target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (PrismaticJoint)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			Vector2 target3 = default(Vector2);
			if (!serialization.TryCustomCopy(LocalAxisA, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = serialization.CreateCopy(LocalAxisA, hookCtx, context);
			}
			target.LocalAxisA = target3;
			float target4 = 0f;
			if (!serialization.TryCustomCopy(ReferenceAngle, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = ReferenceAngle;
			}
			target.ReferenceAngle = target4;
			bool target5 = false;
			if (!serialization.TryCustomCopy(EnableLimit, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = EnableLimit;
			}
			target.EnableLimit = target5;
			float target6 = 0f;
			if (!serialization.TryCustomCopy(LowerTranslation, ref target6, hookCtx, hasHooks: false, context))
			{
				target6 = LowerTranslation;
			}
			target.LowerTranslation = target6;
			float target7 = 0f;
			if (!serialization.TryCustomCopy(UpperTranslation, ref target7, hookCtx, hasHooks: false, context))
			{
				target7 = UpperTranslation;
			}
			target.UpperTranslation = target7;
			bool target8 = false;
			if (!serialization.TryCustomCopy(EnableMotor, ref target8, hookCtx, hasHooks: false, context))
			{
				target8 = EnableMotor;
			}
			target.EnableMotor = target8;
			float target9 = 0f;
			if (!serialization.TryCustomCopy(MaxMotorForce, ref target9, hookCtx, hasHooks: false, context))
			{
				target9 = MaxMotorForce;
			}
			target.MaxMotorForce = target9;
			float target10 = 0f;
			if (!serialization.TryCustomCopy(MotorSpeed, ref target10, hookCtx, hasHooks: false, context))
			{
				target10 = MotorSpeed;
			}
			target.MotorSpeed = target10;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PrismaticJoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Joint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PrismaticJoint target2 = (PrismaticJoint)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PrismaticJoint target2 = (PrismaticJoint)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PrismaticJoint Instantiate()
	{
		return new PrismaticJoint();
	}
}
