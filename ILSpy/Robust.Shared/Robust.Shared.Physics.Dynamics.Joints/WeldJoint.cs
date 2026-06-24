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

public sealed class WeldJoint : Joint, IEquatable<WeldJoint>, ISerializationGenerated<WeldJoint>, ISerializationGenerated
{
	private float _gamma;

	private Vector3 _impulse;

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

	private Matrix33 _mass;

	[DataField("stiffness", false, 1, false, false, null)]
	public float Stiffness;

	[DataField("damping", false, 1, false, false, null)]
	public float Damping;

	[DataField("bias", false, 1, false, false, null)]
	public float Bias;

	[DataField("referenceAngle", false, 1, false, false, null)]
	public float ReferenceAngle;

	public override JointType JointType => JointType.Weld;

	public WeldJoint()
	{
	}

	internal WeldJoint(EntityUid bodyA, EntityUid bodyB, Vector2 anchorA, Vector2 anchorB, float referenceAngle)
		: base(bodyA, bodyB)
	{
		base.LocalAnchorA = anchorA;
		base.LocalAnchorB = anchorB;
		ReferenceAngle = referenceAngle;
	}

	internal WeldJoint(EntityUid bodyAUid, EntityUid bodyBUid)
		: base(bodyAUid, bodyBUid)
	{
	}

	internal WeldJoint(WeldJointState state, IEntityManager entManager, EntityUid owner)
		: base(state, entManager, owner)
	{
		Stiffness = state.Stiffness;
		Damping = state.Damping;
		Bias = state.Bias;
	}

	public override JointState GetState(IEntityManager entManager)
	{
		WeldJointState weldJointState = new WeldJointState();
		GetState(weldJointState, entManager);
		return weldJointState;
	}

	public override Vector2 GetReactionForce(float invDt)
	{
		return new Vector2(_impulse.X, _impulse.Y) * invDt;
	}

	public override float GetReactionTorque(float invDt)
	{
		return invDt * _impulse.Z;
	}

	internal override void InitVelocityConstraints(in SolverData data, in SharedPhysicsSystem.IslandData island, PhysicsComponent bodyA, PhysicsComponent bodyB, Vector2[] positions, float[] angles, Vector2[] linearVelocities, float[] angularVelocities)
	{
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
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
		Unsafe.SkipInit(out Matrix33 val);
		val.EX.X = invMassA + invMassB + _rA.Y * _rA.Y * invIA + _rB.Y * _rB.Y * invIB;
		val.EY.X = (0f - _rA.Y) * _rA.X * invIA - _rB.Y * _rB.X * invIB;
		val.EZ.X = (0f - _rA.Y) * invIA - _rB.Y * invIB;
		val.EX.Y = val.EY.X;
		val.EY.Y = invMassA + invMassB + _rA.X * _rA.X * invIA + _rB.X * _rB.X * invIB;
		val.EZ.Y = _rA.X * invIA + _rB.X * invIB;
		val.EX.Z = val.EZ.X;
		val.EY.Z = val.EZ.Y;
		val.EZ.Z = invIA + invIB;
		if (Stiffness > 0f)
		{
			((Matrix33)(ref val)).GetInverse22(ref _mass);
			float num5 = invIA + invIB;
			float num6 = num3 - num - ReferenceAngle;
			float damping = Damping;
			float stiffness = Stiffness;
			float frameTime = data.FrameTime;
			_gamma = frameTime * (damping + frameTime * stiffness);
			_gamma = ((_gamma != 0f) ? (1f / _gamma) : 0f);
			Bias = num6 * frameTime * stiffness * _gamma;
			num5 += _gamma;
			_mass.EZ.Z = ((num5 != 0f) ? (1f / num5) : 0f);
		}
		else if (val.EZ.Z == 0f)
		{
			((Matrix33)(ref val)).GetInverse22(ref _mass);
			_gamma = 0f;
			Bias = 0f;
		}
		else
		{
			((Matrix33)(ref val)).GetSymInverse33(ref _mass);
			_gamma = 0f;
			Bias = 0f;
		}
		if (data.WarmStarting)
		{
			_impulse *= data.DtRatio;
			Vector2 vector3 = new Vector2(_impulse.X, _impulse.Y);
			vector -= vector3 * invMassA;
			num2 -= invIA * (Vector2Helpers.Cross(_rA, vector3) + _impulse.Z);
			vector2 += vector3 * invMassB;
			num4 += invIB * (Vector2Helpers.Cross(_rB, vector3) + _impulse.Z);
		}
		else
		{
			_impulse = Vector3.Zero;
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
		if (Stiffness > 0f)
		{
			float num3 = num2 - num;
			float num4 = (0f - _mass.EZ.Z) * (num3 + Bias + _gamma * _impulse.Z);
			_impulse.Z += num4;
			num -= invIA * num4;
			num2 += invIB * num4;
			Vector2 vector3 = vector2 + Vector2Helpers.Cross(num2, ref _rB) - vector - Vector2Helpers.Cross(num, ref _rA);
			Vector2 vector4 = -((Matrix33)(ref _mass)).Mul22(vector3);
			_impulse.X += vector4.X;
			_impulse.Y += vector4.Y;
			Vector2 vector5 = vector4;
			vector -= vector5 * invMassA;
			num -= invIA * Vector2Helpers.Cross(_rA, vector5);
			vector2 += vector5 * invMassB;
			num2 += invIB * Vector2Helpers.Cross(_rB, vector5);
		}
		else
		{
			Vector2 vector6 = vector2 + Vector2Helpers.Cross(num2, ref _rB) - vector - Vector2Helpers.Cross(num, ref _rA);
			float z = num2 - num;
			Vector3 vector7 = new Vector3(vector6.X, vector6.Y, z);
			Vector3 vector8 = -((Matrix33)(ref _mass)).Mul(vector7);
			_impulse += vector8;
			Vector2 vector9 = new Vector2(vector8.X, vector8.Y);
			vector -= vector9 * invMassA;
			num -= invIA * (Vector2Helpers.Cross(_rA, vector9) + vector8.Z);
			vector2 += vector9 * invMassB;
			num2 += invIB * (Vector2Helpers.Cross(_rB, vector9) + vector8.Z);
		}
		linearVelocities[offset + _indexA] = vector;
		angularVelocities[offset + _indexA] = num;
		linearVelocities[offset + _indexB] = vector2;
		angularVelocities[offset + _indexB] = num2;
	}

	internal override bool SolvePositionConstraints(in SolverData data, Vector2[] positions, float[] angles)
	{
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
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
		Unsafe.SkipInit(out Matrix33 val);
		val.EX.X = invMassA + invMassB + vector3.Y * vector3.Y * invIA + vector4.Y * vector4.Y * invIB;
		val.EY.X = (0f - vector3.Y) * vector3.X * invIA - vector4.Y * vector4.X * invIB;
		val.EZ.X = (0f - vector3.Y) * invIA - vector4.Y * invIB;
		val.EX.Y = val.EY.X;
		val.EY.Y = invMassA + invMassB + vector3.X * vector3.X * invIA + vector4.X * vector4.X * invIB;
		val.EZ.Y = vector3.X * invIA + vector4.X * invIB;
		val.EX.Z = val.EZ.X;
		val.EY.Z = val.EZ.Y;
		val.EZ.Z = invIA + invIB;
		float num3;
		float num4;
		if (Stiffness > 0f)
		{
			Vector2 vector5 = vector2 + vector4 - vector - vector3;
			num3 = vector5.Length();
			num4 = 0f;
			Vector2 vector6 = -((Matrix33)(ref val)).Solve22(vector5);
			vector -= vector6 * invMassA;
			num -= invIA * Vector2Helpers.Cross(vector3, vector6);
			vector2 += vector6 * invMassB;
			num2 += invIB * Vector2Helpers.Cross(vector4, vector6);
		}
		else
		{
			Vector2 vector7 = vector2 + vector4 - vector - vector3;
			float num5 = num2 - num - ReferenceAngle;
			num3 = vector7.Length();
			num4 = Math.Abs(num5);
			Vector3 vector8 = new Vector3(vector7.X, vector7.Y, num5);
			Vector3 vector9;
			if (val.EZ.Z > 0f)
			{
				vector9 = -((Matrix33)(ref val)).Solve33(vector8);
			}
			else
			{
				Vector2 vector10 = -((Matrix33)(ref val)).Solve22(vector7);
				vector9 = new Vector3(vector10.X, vector10.Y, 0f);
			}
			Vector2 vector11 = new Vector2(vector9.X, vector9.Y);
			vector -= vector11 * invMassA;
			num -= invIA * (Vector2Helpers.Cross(vector3, vector11) + vector9.Z);
			vector2 += vector11 * invMassB;
			num2 += invIB * (Vector2Helpers.Cross(vector4, vector11) + vector9.Z);
		}
		positions[_indexA] = vector;
		angles[_indexA] = num;
		positions[_indexB] = vector2;
		angles[_indexB] = num2;
		if (num3 <= 0.005f)
		{
			return num4 <= 0.03490659f;
		}
		return false;
	}

	public override Joint Clone(EntityUid uidA, EntityUid uidB)
	{
		return new WeldJoint(uidA, uidB, base.LocalAnchorA, base.LocalAnchorB, ReferenceAngle)
		{
			Enabled = base.Enabled,
			Bias = Bias,
			Damping = Damping,
			Stiffness = Stiffness,
			_impulse = _impulse,
			Breakpoint = base.Breakpoint
		};
	}

	public override void CopyTo(Joint original)
	{
		if (original is WeldJoint weldJoint)
		{
			weldJoint.Enabled = base.Enabled;
			weldJoint.Bias = Bias;
			weldJoint.Damping = Damping;
			weldJoint.Stiffness = Stiffness;
			weldJoint._impulse = _impulse;
			weldJoint.Breakpoint = base.Breakpoint;
		}
	}

	public bool Equals(WeldJoint? other)
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
		if (Stiffness.Equals(other.Stiffness) && Damping.Equals(other.Damping))
		{
			return Bias.Equals(other.Bias);
		}
		return false;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref WeldJoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Joint target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (WeldJoint)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			float target3 = 0f;
			if (!serialization.TryCustomCopy(Stiffness, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = Stiffness;
			}
			target.Stiffness = target3;
			float target4 = 0f;
			if (!serialization.TryCustomCopy(Damping, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = Damping;
			}
			target.Damping = target4;
			float target5 = 0f;
			if (!serialization.TryCustomCopy(Bias, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = Bias;
			}
			target.Bias = target5;
			float target6 = 0f;
			if (!serialization.TryCustomCopy(ReferenceAngle, ref target6, hookCtx, hasHooks: false, context))
			{
				target6 = ReferenceAngle;
			}
			target.ReferenceAngle = target6;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref WeldJoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Joint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WeldJoint target2 = (WeldJoint)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WeldJoint target2 = (WeldJoint)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override WeldJoint Instantiate()
	{
		return new WeldJoint();
	}
}
