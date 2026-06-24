// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.Prototypes.CargoBountyItemEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared.Cargo.Prototypes;

[DataDefinition]
[NetSerializable]
[Serializable]
public readonly record struct CargoBountyItemEntry : 
  ISerializationGenerated<CargoBountyItemEntry>,
  ISerializationGenerated
{
  public CargoBountyItemEntry()
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CWhitelist\u003Ek__BackingField = (EntityWhitelist) null;
    // ISSUE: reference to a compiler-generated field
    this.\u003CBlacklist\u003Ek__BackingField = (EntityWhitelist) null;
    // ISSUE: reference to a compiler-generated field
    this.\u003CAmount\u003Ek__BackingField = 1;
    // ISSUE: reference to a compiler-generated field
    this.\u003CName\u003Ek__BackingField = LocId.op_Implicit(string.Empty);
  }

  [DataField(null, false, 1, true, false, null)]
  public EntityWhitelist Whitelist { get; init; }

  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist { get; init; }

  [DataField(null, false, 1, false, false, null)]
  public int Amount { get; init; }

  [DataField(null, false, 1, false, false, null)]
  public LocId Name { get; init; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CargoBountyItemEntry target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<CargoBountyItemEntry>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist entityWhitelist1 = (EntityWhitelist) null;
    if (this.Whitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref entityWhitelist1, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        entityWhitelist1 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref entityWhitelist1, hookCtx, context, true);
    }
    EntityWhitelist entityWhitelist2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref entityWhitelist2, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        entityWhitelist2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref entityWhitelist2, hookCtx, context, false);
    }
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.Amount, ref num, hookCtx, false, context))
      num = this.Amount;
    LocId locId = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Name, ref locId, hookCtx, false, context))
      locId = serialization.CreateCopy<LocId>(this.Name, hookCtx, context, false);
    target = target with
    {
      Whitelist = entityWhitelist1,
      Blacklist = entityWhitelist2,
      Amount = num,
      Name = locId
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CargoBountyItemEntry target,
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
    CargoBountyItemEntry target1 = (CargoBountyItemEntry) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public CargoBountyItemEntry Instantiate() => new CargoBountyItemEntry();
}
