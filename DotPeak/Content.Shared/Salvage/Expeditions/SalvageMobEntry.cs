// Decompiled with JetBrains decompiler
// Type: Content.Shared.Salvage.Expeditions.SalvageMobEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Random;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;

#nullable enable
namespace Content.Shared.Salvage.Expeditions;

[DataDefinition]
public record struct SalvageMobEntry : 
  IBudgetEntry,
  IProbEntry,
  ISerializationGenerated<SalvageMobEntry>,
  ISerializationGenerated
{
  public SalvageMobEntry()
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CCost\u003Ek__BackingField = 1f;
    // ISSUE: reference to a compiler-generated field
    this.\u003CProb\u003Ek__BackingField = 1f;
    // ISSUE: reference to a compiler-generated field
    this.\u003CProto\u003Ek__BackingField = string.Empty;
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("cost", false, 1, false, false, null)]
  public float Cost { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("prob", false, 1, false, false, null)]
  public float Prob { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("proto", false, 1, true, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string Proto { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SalvageMobEntry target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<SalvageMobEntry>(this, ref target, hookCtx, false, context))
      return;
    float target1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Cost, ref target1, hookCtx, false, context))
      target1 = this.Cost;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Prob, ref target2, hookCtx, false, context))
      target2 = this.Prob;
    string target3 = (string) null;
    if (this.Proto == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Proto, ref target3, hookCtx, false, context))
      target3 = this.Proto;
    target = target with
    {
      Cost = target1,
      Prob = target2,
      Proto = target3
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SalvageMobEntry target,
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
    SalvageMobEntry target1 = (SalvageMobEntry) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public SalvageMobEntry Instantiate() => new SalvageMobEntry();
}
