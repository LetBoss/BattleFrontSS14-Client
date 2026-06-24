// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.Components.SunShadowCycleDirection
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Light.Components;

[DataDefinition]
[NetSerializable]
[Serializable]
public record struct SunShadowCycleDirection : 
  ISerializationGenerated<SunShadowCycleDirection>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Ratio;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 Direction;
  [DataField(null, false, 1, false, false, null)]
  public float Alpha;

  public SunShadowCycleDirection(float ratio, Vector2 direction, float alpha)
  {
    this.Ratio = ratio;
    this.Direction = direction;
    this.Alpha = alpha;
  }

  public SunShadowCycleDirection()
  {
    this.Ratio = 0.0f;
    this.Direction = new Vector2();
    this.Alpha = 0.0f;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SunShadowCycleDirection target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<SunShadowCycleDirection>(this, ref target, hookCtx, false, context))
      return;
    float target1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Ratio, ref target1, hookCtx, false, context))
      target1 = this.Ratio;
    Vector2 target2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Direction, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2>(this.Direction, hookCtx, context);
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Alpha, ref target3, hookCtx, false, context))
      target3 = this.Alpha;
    target = target with
    {
      Ratio = target1,
      Direction = target2,
      Alpha = target3
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SunShadowCycleDirection target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SunShadowCycleDirection target1 = (SunShadowCycleDirection) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public SunShadowCycleDirection Instantiate() => new SunShadowCycleDirection();

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (EqualityComparer<float>.Default.GetHashCode(this.Ratio) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.Direction)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.Alpha);
  }

  [CompilerGenerated]
  public readonly bool Equals(SunShadowCycleDirection other)
  {
    return EqualityComparer<float>.Default.Equals(this.Ratio, other.Ratio) && EqualityComparer<Vector2>.Default.Equals(this.Direction, other.Direction) && EqualityComparer<float>.Default.Equals(this.Alpha, other.Alpha);
  }
}
