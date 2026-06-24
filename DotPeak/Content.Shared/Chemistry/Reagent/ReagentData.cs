// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reagent.ReagentData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Chemistry.Reagent;

[ImplicitDataDefinitionForInheritors]
[NetSerializable]
[Serializable]
public abstract class ReagentData : 
  IEquatable<ReagentData>,
  ISerializationGenerated<ReagentData>,
  ISerializationGenerated
{
  public virtual string ToString(string prototype, FixedPoint2 quantity)
  {
    return $"{prototype}:{this.GetType().Name}:{quantity}";
  }

  public virtual string ToString(string prototype) => $"{prototype}:{this.GetType().Name}";

  public abstract bool Equals(ReagentData? other);

  public override bool Equals(object? obj)
  {
    if (obj == null)
      return false;
    if (this == obj)
      return true;
    return !(obj.GetType() != this.GetType()) && this.Equals((ReagentData) obj);
  }

  public abstract override int GetHashCode();

  public abstract ReagentData Clone();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref ReagentData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    serialization.TryCustomCopy<ReagentData>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref ReagentData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ReagentData target1 = (ReagentData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual ReagentData Instantiate() => throw new NotImplementedException();
}
