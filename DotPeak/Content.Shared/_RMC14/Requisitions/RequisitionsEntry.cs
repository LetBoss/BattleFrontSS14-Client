// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Requisitions.RequisitionsEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Requisitions;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class RequisitionsEntry : 
  ISerializationGenerated<RequisitionsEntry>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string? Name;
  [DataField(null, false, 1, true, false, null)]
  public int Cost;
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId Crate;
  [DataField(null, false, 1, false, false, null)]
  public List<EntProtoId> Entities = new List<EntProtoId>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RequisitionsEntry target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RequisitionsEntry>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Name, ref target1, hookCtx, false, context))
      target1 = this.Name;
    target.Name = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Cost, ref target2, hookCtx, false, context))
      target2 = this.Cost;
    target.Cost = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Crate, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.Crate, hookCtx, context);
    target.Crate = target3;
    List<EntProtoId> target4 = (List<EntProtoId>) null;
    if (this.Entities == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.Entities, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<EntProtoId>>(this.Entities, hookCtx, context);
    target.Entities = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RequisitionsEntry target,
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
    RequisitionsEntry target1 = (RequisitionsEntry) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RequisitionsEntry Instantiate() => new RequisitionsEntry();
}
