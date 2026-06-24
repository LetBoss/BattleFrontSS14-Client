// Decompiled with JetBrains decompiler
// Type: Content.Shared.Interaction.RotateToFaceSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Interaction;
using Content.Shared.ActionBlocker;
using Content.Shared.Buckle.Components;
using Content.Shared.Rotatable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Interaction;

public sealed class RotateToFaceSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlockerSystem;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private RMCInteractionSystem _rmcInteraction;

  public bool TryRotateTo(
    EntityUid uid,
    Angle goalRotation,
    float frameTime,
    Angle tolerance,
    double rotationSpeed = 3.4028234663852886E+38,
    TransformComponent? xform = null)
  {
    if (!this.Resolve(uid, ref xform))
      return true;
    this._rmcInteraction.TryCapWorldRotation((Entity<MaxRotationComponent, TransformComponent>) (uid, (MaxRotationComponent) null, xform), ref goalRotation);
    if (rotationSpeed < 3.4028234663852886E+38)
    {
      Angle worldRotation = this._transform.GetWorldRotation(xform);
      double theta = Angle.ShortestDistance(ref worldRotation, ref goalRotation).Theta;
      double num = rotationSpeed * (double) frameTime;
      if (Math.Abs(theta) > num)
      {
        Angle diffAngle = Angle.op_Addition(worldRotation, Angle.op_Implicit((double) Math.Sign(theta) * num));
        this.TryFaceAngle(uid, diffAngle, xform);
        return Math.Abs(Angle.op_Implicit(Angle.op_Subtraction(goalRotation, diffAngle))) <= Angle.op_Implicit(tolerance);
      }
      this.TryFaceAngle(uid, goalRotation, xform);
    }
    else
      this.TryFaceAngle(uid, goalRotation, xform);
    return true;
  }

  public bool TryFaceCoordinates(EntityUid user, Vector2 coordinates, TransformComponent? xform = null)
  {
    if (!this.Resolve(user, ref xform))
      return false;
    Vector2 vector2 = coordinates - this._transform.GetMapCoordinates(user, xform).Position;
    if ((double) vector2.LengthSquared() <= 0.0099999997764825821)
      return true;
    Angle diffAngle = Angle.FromWorldVec(vector2);
    return this.TryFaceAngle(user, diffAngle);
  }

  public bool TryFaceAngle(EntityUid user, Angle diffAngle, TransformComponent? xform = null)
  {
    if (!this._actionBlockerSystem.CanChangeDirection(user))
      return false;
    BuckleComponent comp1;
    if (this.TryComp<BuckleComponent>(user, out comp1))
    {
      EntityUid? buckledTo = comp1.BuckledTo;
      if (buckledTo.HasValue)
      {
        EntityUid valueOrDefault = buckledTo.GetValueOrDefault();
        RotatableComponent comp2;
        if (!this.TryComp<RotatableComponent>(valueOrDefault, out comp2) || !comp2.RotateWhileAnchored)
          return false;
        this._transform.SetWorldRotation(this.Transform(valueOrDefault), diffAngle);
        return true;
      }
    }
    if (!this.Resolve(user, ref xform))
      return false;
    this._transform.SetWorldRotation(xform, diffAngle);
    return true;
  }
}
