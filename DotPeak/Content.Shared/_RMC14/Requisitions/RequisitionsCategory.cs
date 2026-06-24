// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Requisitions.RequisitionsCategory
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
public sealed class RequisitionsCategory : 
  ISerializationGenerated<RequisitionsCategory>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string Name = string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public List<RequisitionsEntry> Entries = new List<RequisitionsEntry>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RequisitionsCategory target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RequisitionsCategory>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref target1, hookCtx, false, context))
      target1 = this.Name;
    target.Name = target1;
    List<RequisitionsEntry> target2 = (List<RequisitionsEntry>) null;
    if (this.Entries == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<RequisitionsEntry>>(this.Entries, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<RequisitionsEntry>>(this.Entries, hookCtx, context);
    target.Entries = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RequisitionsCategory target,
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
    RequisitionsCategory target1 = (RequisitionsCategory) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RequisitionsCategory Instantiate() => new RequisitionsCategory();
}
