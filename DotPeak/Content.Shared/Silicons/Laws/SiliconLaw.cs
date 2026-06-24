// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Laws.SiliconLaw
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;

#nullable enable
namespace Content.Shared.Silicons.Laws;

[Virtual]
[DataDefinition]
[NetSerializable]
[Serializable]
public class SiliconLaw : 
  IComparable<SiliconLaw>,
  IEquatable<SiliconLaw>,
  ISerializationGenerated<SiliconLaw>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string LawString = string.Empty;
  [DataField(null, false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public FixedPoint2 Order;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string? LawIdentifierOverride;

  public int CompareTo(SiliconLaw? other) => other == null ? -1 : this.Order.CompareTo(other.Order);

  public bool Equals(SiliconLaw? other)
  {
    return other != null && this.LawString == other.LawString && this.Order == other.Order && this.LawIdentifierOverride == other.LawIdentifierOverride;
  }

  public override bool Equals(object? obj) => obj != null && this.Equals(obj as SiliconLaw);

  public override int GetHashCode()
  {
    return HashCode.Combine<string, FixedPoint2, string>(this.LawString, this.Order, this.LawIdentifierOverride);
  }

  public SiliconLaw ShallowClone()
  {
    return new SiliconLaw()
    {
      LawString = this.LawString,
      Order = this.Order,
      LawIdentifierOverride = this.LawIdentifierOverride
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref SiliconLaw target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<SiliconLaw>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.LawString == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.LawString, ref target1, hookCtx, false, context))
      target1 = this.LawString;
    target.LawString = target1;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Order, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.Order, hookCtx, context);
    target.Order = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.LawIdentifierOverride, ref target3, hookCtx, false, context))
      target3 = this.LawIdentifierOverride;
    target.LawIdentifierOverride = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref SiliconLaw target,
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
    SiliconLaw target1 = (SiliconLaw) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual SiliconLaw Instantiate() => new SiliconLaw();
}
