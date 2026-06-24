// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vendors.CMVendorSection
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Vendors;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class CMVendorSection : 
  ISerializationGenerated<CMVendorSection>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string Name = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public (string Id, int Amount)? Choices;
  [DataField(null, false, 1, false, false, null)]
  public string? TakeAll;
  [DataField(null, false, 1, false, false, null)]
  public string? TakeOne;
  [DataField(null, false, 1, true, false, null)]
  public List<CMVendorEntry> Entries = new List<CMVendorEntry>();
  [DataField(null, false, 1, false, false, null)]
  public int? SharedSpecLimit;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<JobPrototype>> Jobs = new List<ProtoId<JobPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<RankPrototype>> Ranks = new List<ProtoId<RankPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public List<string> Holidays = new List<string>();
  [DataField(null, false, 1, false, false, null)]
  public bool HasBoxes;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMVendorSection target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<CMVendorSection>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref target1, hookCtx, false, context))
      target1 = this.Name;
    target.Name = target1;
    (string, int)? target2 = new (string, int)?();
    if (!serialization.TryCustomCopy<(string, int)?>(this.Choices, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<(string, int)?>(this.Choices, hookCtx, context);
    target.Choices = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.TakeAll, ref target3, hookCtx, false, context))
      target3 = this.TakeAll;
    target.TakeAll = target3;
    string target4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.TakeOne, ref target4, hookCtx, false, context))
      target4 = this.TakeOne;
    target.TakeOne = target4;
    List<CMVendorEntry> target5 = (List<CMVendorEntry>) null;
    if (this.Entries == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<CMVendorEntry>>(this.Entries, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<CMVendorEntry>>(this.Entries, hookCtx, context);
    target.Entries = target5;
    int? target6 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.SharedSpecLimit, ref target6, hookCtx, false, context))
      target6 = this.SharedSpecLimit;
    target.SharedSpecLimit = target6;
    List<ProtoId<JobPrototype>> target7 = (List<ProtoId<JobPrototype>>) null;
    if (this.Jobs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<JobPrototype>>>(this.Jobs, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<List<ProtoId<JobPrototype>>>(this.Jobs, hookCtx, context);
    target.Jobs = target7;
    List<ProtoId<RankPrototype>> target8 = (List<ProtoId<RankPrototype>>) null;
    if (this.Ranks == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<RankPrototype>>>(this.Ranks, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<List<ProtoId<RankPrototype>>>(this.Ranks, hookCtx, context);
    target.Ranks = target8;
    List<string> target9 = (List<string>) null;
    if (this.Holidays == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.Holidays, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<List<string>>(this.Holidays, hookCtx, context);
    target.Holidays = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.HasBoxes, ref target10, hookCtx, false, context))
      target10 = this.HasBoxes;
    target.HasBoxes = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMVendorSection target,
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
    CMVendorSection target1 = (CMVendorSection) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public CMVendorSection Instantiate() => new CMVendorSection();
}
