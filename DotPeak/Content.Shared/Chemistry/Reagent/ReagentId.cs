// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reagent.ReagentId
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Chemistry.Reagent;

[NetSerializable]
[DataDefinition]
[Serializable]
public struct ReagentId : 
  IEquatable<ReagentId>,
  ISerializationGenerated<ReagentId>,
  ISerializationGenerated
{
  [DataField("ReagentId", false, 1, true, false, typeof (PrototypeIdSerializer<ReagentPrototype>))]
  public string Prototype { get; private set; }

  [DataField("data", false, 1, false, false, null)]
  public List<ReagentData>? Data { get; private set; }

  public ReagentId(string prototype, List<ReagentData>? data)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CData\u003Ek__BackingField = new List<ReagentData>();
    this.Prototype = prototype;
    this.Data = data ?? new List<ReagentData>();
  }

  public ReagentId()
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CData\u003Ek__BackingField = new List<ReagentData>();
    this.Prototype = (string) null;
    this.Data = new List<ReagentData>();
  }

  public List<ReagentData> EnsureReagentData()
  {
    return this.Data == null ? new List<ReagentData>() : this.Data;
  }

  public bool Equals(ReagentId other)
  {
    if (this.Prototype != other.Prototype)
      return false;
    if (this.Data == null)
      return other.Data == null;
    return other.Data != null && !this.Data.Except<ReagentData>((IEnumerable<ReagentData>) other.Data).Any<ReagentData>() && !other.Data.Except<ReagentData>((IEnumerable<ReagentData>) this.Data).Any<ReagentData>() && this.Data.Count == other.Data.Count;
  }

  public bool Equals(string prototype, List<ReagentData>? otherData = null)
  {
    if (this.Prototype != prototype)
      return false;
    return this.Data == null ? otherData == null : this.Data.Equals((object) otherData);
  }

  public override bool Equals(object? obj) => obj is ReagentId other && this.Equals(other);

  public override int GetHashCode()
  {
    return HashCode.Combine<string, List<ReagentData>>(this.Prototype, this.Data);
  }

  public string ToString(FixedPoint2 quantity) => $"{this.Prototype}:{quantity}";

  public override string ToString() => this.Prototype ?? "";

  public static bool operator ==(ReagentId left, ReagentId right) => left.Equals(right);

  public static bool operator !=(ReagentId left, ReagentId right) => !(left == right);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReagentId target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ReagentId>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.Prototype == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Prototype, ref str, hookCtx, false, context))
      str = this.Prototype;
    List<ReagentData> reagentDataList = (List<ReagentData>) null;
    if (!serialization.TryCustomCopy<List<ReagentData>>(this.Data, ref reagentDataList, hookCtx, true, context))
      reagentDataList = serialization.CreateCopy<List<ReagentData>>(this.Data, hookCtx, context, false);
    target = target with
    {
      Prototype = str,
      Data = reagentDataList
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReagentId target,
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
    ReagentId target1 = (ReagentId) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ReagentId Instantiate() => new ReagentId();
}
