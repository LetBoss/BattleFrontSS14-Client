using System;
using System.Numerics;
using Robust.Shared.Maths;

namespace Content.Shared.Vehicle;

public static class GridVehicleMotionSimulator
{
	public readonly record struct DriveProfile(float MaxSpeed, float MaxReverseSpeed, float Acceleration, float ReverseAcceleration, float Deceleration);

	public readonly record struct DriveSpeedResult(float CurrentSpeed, bool ReversingInput, bool ChangingDirection);

	public readonly record struct AdvanceResult(Vector2 Position, Vector2i CurrentTile, float RemainingDistance, bool ReachedTarget);

	public static float StepIdleSpeed(float currentSpeed, float deceleration, float frameTime)
	{
		if (currentSpeed > 0f)
		{
			return MathF.Max(0f, currentSpeed - deceleration * frameTime);
		}
		if (currentSpeed < 0f)
		{
			return MathF.Min(0f, currentSpeed + deceleration * frameTime);
		}
		return 0f;
	}

	public static float StepPushSpeed(float currentSpeed, float maxSpeed, float acceleration, float deceleration, bool hasInput, bool isCommittedToMove, float frameTime)
	{
		float targetSpeed;
		float accel;
		if (!(hasInput || isCommittedToMove))
		{
			targetSpeed = 0f;
			accel = deceleration;
		}
		else
		{
			targetSpeed = maxSpeed;
			accel = acceleration;
		}
		return StepTowardsTargetSpeed(currentSpeed, targetSpeed, accel, deceleration, frameTime);
	}

	public static DriveSpeedResult StepDriveSpeed(float currentSpeed, DriveProfile profile, Vector2i facing, Vector2i inputDir, bool hasInput, bool isCommittedToMove, float frameTime)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		bool hasInputForSpeed = hasInput || isCommittedToMove;
		bool reversing = hasInput && facing != Vector2i.Zero && inputDir == -facing;
		float targetSpeed;
		float accel;
		if (!hasInputForSpeed)
		{
			targetSpeed = 0f;
			accel = profile.Deceleration;
		}
		else if (reversing)
		{
			if (currentSpeed > 0f)
			{
				targetSpeed = 0f;
				accel = profile.Deceleration;
			}
			else
			{
				targetSpeed = 0f - profile.MaxReverseSpeed;
				accel = profile.ReverseAcceleration;
			}
		}
		else if (currentSpeed < 0f && hasInputForSpeed)
		{
			targetSpeed = 0f;
			accel = profile.Deceleration;
		}
		else
		{
			targetSpeed = profile.MaxSpeed;
			accel = profile.Acceleration;
		}
		float steppedSpeed = StepTowardsTargetSpeed(currentSpeed, targetSpeed, accel, profile.Deceleration, frameTime);
		bool changingDirection = MathF.Abs(steppedSpeed) > 0.01f && ((reversing && steppedSpeed > 0f) || (!reversing && steppedSpeed < 0f));
		return new DriveSpeedResult(steppedSpeed, reversing, changingDirection);
	}

	public static float StepFreeSpeed(float currentSpeed, int throttle, float maxSpeed, float maxReverseSpeed, float acceleration, float reverseAcceleration, float deceleration, bool allowReverse, float frameTime)
	{
		float targetSpeed;
		float accel;
		if (throttle > 0)
		{
			if (currentSpeed < 0f)
			{
				targetSpeed = 0f;
				accel = deceleration;
			}
			else
			{
				targetSpeed = maxSpeed;
				accel = acceleration;
			}
		}
		else if (throttle < 0)
		{
			if (currentSpeed > 0f)
			{
				targetSpeed = 0f;
				accel = deceleration;
			}
			else if (allowReverse)
			{
				targetSpeed = 0f - maxReverseSpeed;
				accel = reverseAcceleration;
			}
			else
			{
				targetSpeed = 0f;
				accel = deceleration;
			}
		}
		else
		{
			targetSpeed = 0f;
			accel = deceleration;
		}
		return StepTowardsTargetSpeed(currentSpeed, targetSpeed, accel, deceleration, frameTime);
	}

	public static AdvanceResult AdvanceToTarget(Vector2 position, Vector2i currentTile, Vector2i targetTile, Vector2 targetPosition, float travelDistance)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Vector2 toTarget = targetPosition - position;
		float distToTarget = toTarget.Length();
		if (distToTarget <= 0.0001f || travelDistance >= distToTarget)
		{
			return new AdvanceResult(targetPosition, targetTile, MathF.Max(0f, travelDistance - distToTarget), ReachedTarget: true);
		}
		Vector2 dir = toTarget / distToTarget;
		return new AdvanceResult(position + dir * travelDistance, currentTile, 0f, ReachedTarget: false);
	}

	private static float StepTowardsTargetSpeed(float currentSpeed, float targetSpeed, float accelerateTowardTarget, float decelerateTowardTarget, float frameTime)
	{
		if (currentSpeed < targetSpeed)
		{
			return MathF.Min(currentSpeed + accelerateTowardTarget * frameTime, targetSpeed);
		}
		if (currentSpeed > targetSpeed)
		{
			return MathF.Max(currentSpeed - decelerateTowardTarget * frameTime, targetSpeed);
		}
		return currentSpeed;
	}
}
