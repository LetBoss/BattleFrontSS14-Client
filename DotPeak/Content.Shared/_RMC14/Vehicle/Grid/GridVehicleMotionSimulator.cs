// Decompiled with JetBrains decompiler
// Type: Content.Shared.Vehicle.GridVehicleMotionSimulator
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable disable
namespace Content.Shared.Vehicle;

public static class GridVehicleMotionSimulator
{
  public static float StepIdleSpeed(float currentSpeed, float deceleration, float frameTime)
  {
    if ((double) currentSpeed > 0.0)
      return MathF.Max(0.0f, currentSpeed - deceleration * frameTime);
    return (double) currentSpeed < 0.0 ? MathF.Min(0.0f, currentSpeed + deceleration * frameTime) : 0.0f;
  }

  public static float StepPushSpeed(
    float currentSpeed,
    float maxSpeed,
    float acceleration,
    float deceleration,
    bool hasInput,
    bool isCommittedToMove,
    float frameTime)
  {
    float targetSpeed;
    float accelerateTowardTarget;
    if (!(hasInput | isCommittedToMove))
    {
      targetSpeed = 0.0f;
      accelerateTowardTarget = deceleration;
    }
    else
    {
      targetSpeed = maxSpeed;
      accelerateTowardTarget = acceleration;
    }
    return GridVehicleMotionSimulator.StepTowardsTargetSpeed(currentSpeed, targetSpeed, accelerateTowardTarget, deceleration, frameTime);
  }

  public static GridVehicleMotionSimulator.DriveSpeedResult StepDriveSpeed(
    float currentSpeed,
    GridVehicleMotionSimulator.DriveProfile profile,
    Vector2i facing,
    Vector2i inputDir,
    bool hasInput,
    bool isCommittedToMove,
    float frameTime)
  {
    bool flag = hasInput | isCommittedToMove;
    bool ReversingInput = hasInput && Vector2i.op_Inequality(facing, Vector2i.Zero) && Vector2i.op_Equality(inputDir, Vector2i.op_UnaryNegation(facing));
    float targetSpeed;
    float accelerateTowardTarget;
    if (!flag)
    {
      targetSpeed = 0.0f;
      accelerateTowardTarget = profile.Deceleration;
    }
    else if (ReversingInput)
    {
      if ((double) currentSpeed > 0.0)
      {
        targetSpeed = 0.0f;
        accelerateTowardTarget = profile.Deceleration;
      }
      else
      {
        targetSpeed = -profile.MaxReverseSpeed;
        accelerateTowardTarget = profile.ReverseAcceleration;
      }
    }
    else if ((double) currentSpeed < 0.0 & flag)
    {
      targetSpeed = 0.0f;
      accelerateTowardTarget = profile.Deceleration;
    }
    else
    {
      targetSpeed = profile.MaxSpeed;
      accelerateTowardTarget = profile.Acceleration;
    }
    float num = GridVehicleMotionSimulator.StepTowardsTargetSpeed(currentSpeed, targetSpeed, accelerateTowardTarget, profile.Deceleration, frameTime);
    bool ChangingDirection = (double) MathF.Abs(num) > 0.0099999997764825821 && (ReversingInput && (double) num > 0.0 || !ReversingInput && (double) num < 0.0);
    return new GridVehicleMotionSimulator.DriveSpeedResult(num, ReversingInput, ChangingDirection);
  }

  public static float StepFreeSpeed(
    float currentSpeed,
    int throttle,
    float maxSpeed,
    float maxReverseSpeed,
    float acceleration,
    float reverseAcceleration,
    float deceleration,
    bool allowReverse,
    float frameTime)
  {
    float targetSpeed;
    float accelerateTowardTarget;
    if (throttle > 0)
    {
      if ((double) currentSpeed < 0.0)
      {
        targetSpeed = 0.0f;
        accelerateTowardTarget = deceleration;
      }
      else
      {
        targetSpeed = maxSpeed;
        accelerateTowardTarget = acceleration;
      }
    }
    else if (throttle < 0)
    {
      if ((double) currentSpeed > 0.0)
      {
        targetSpeed = 0.0f;
        accelerateTowardTarget = deceleration;
      }
      else if (allowReverse)
      {
        targetSpeed = -maxReverseSpeed;
        accelerateTowardTarget = reverseAcceleration;
      }
      else
      {
        targetSpeed = 0.0f;
        accelerateTowardTarget = deceleration;
      }
    }
    else
    {
      targetSpeed = 0.0f;
      accelerateTowardTarget = deceleration;
    }
    return GridVehicleMotionSimulator.StepTowardsTargetSpeed(currentSpeed, targetSpeed, accelerateTowardTarget, deceleration, frameTime);
  }

  public static GridVehicleMotionSimulator.AdvanceResult AdvanceToTarget(
    Vector2 position,
    Vector2i currentTile,
    Vector2i targetTile,
    Vector2 targetPosition,
    float travelDistance)
  {
    Vector2 vector2_1 = targetPosition - position;
    float num = vector2_1.Length();
    if ((double) num <= 9.9999997473787516E-05 || (double) travelDistance >= (double) num)
      return new GridVehicleMotionSimulator.AdvanceResult(targetPosition, targetTile, MathF.Max(0.0f, travelDistance - num), true);
    Vector2 vector2_2 = vector2_1 / num;
    return new GridVehicleMotionSimulator.AdvanceResult(position + vector2_2 * travelDistance, currentTile, 0.0f, false);
  }

  private static float StepTowardsTargetSpeed(
    float currentSpeed,
    float targetSpeed,
    float accelerateTowardTarget,
    float decelerateTowardTarget,
    float frameTime)
  {
    if ((double) currentSpeed < (double) targetSpeed)
      return MathF.Min(currentSpeed + accelerateTowardTarget * frameTime, targetSpeed);
    return (double) currentSpeed > (double) targetSpeed ? MathF.Max(currentSpeed - decelerateTowardTarget * frameTime, targetSpeed) : currentSpeed;
  }

  public readonly record struct DriveProfile(
    float MaxSpeed,
    float MaxReverseSpeed,
    float Acceleration,
    float ReverseAcceleration,
    float Deceleration)
  ;

  public readonly record struct DriveSpeedResult(
    float CurrentSpeed,
    bool ReversingInput,
    bool ChangingDirection)
  ;

  public readonly record struct AdvanceResult(
    Vector2 Position,
    Vector2i CurrentTile,
    float RemainingDistance,
    bool ReachedTarget)
  ;
}
