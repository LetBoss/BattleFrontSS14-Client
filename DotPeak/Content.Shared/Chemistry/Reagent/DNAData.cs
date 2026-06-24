// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reagent.DnaData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chemistry.Reagent;

[ImplicitDataDefinitionForInheritors]
[NetSerializable]
[Serializable]
public sealed class DnaData : ReagentData, ISerializationGenerated<DnaData>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string DNA = string.Empty;

  public override ReagentData Clone() => (ReagentData) this;

  public override bool Equals(ReagentData? other)
  {
    return other != null && ((DnaData) other).DNA == this.DNA;
  }

  public override int GetHashCode() => this.DNA.GetHashCode();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DnaData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ReagentData target1 = (ReagentData) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DnaData) target1;
    if (serialization.TryCustomCopy<DnaData>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.DNA == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DNA, ref str, hookCtx, false, context))
      str = this.DNA;
    target.DNA = str;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DnaData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref ReagentData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DnaData target1 = (DnaData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (ReagentData) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DnaData target1 = (DnaData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DnaData ReagentData.Instantiate() => new DnaData();
}
