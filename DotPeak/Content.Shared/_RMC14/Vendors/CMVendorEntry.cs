// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vendors.CMVendorEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.Analyzers;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vendors;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed record CMVendorEntry : ISerializationGenerated<CMVendorEntry>, ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId Id;
  [DataField(null, false, 1, false, false, null)]
  public string? Name;
  [DataField(null, false, 1, false, false, null)]
  public int? Amount;
  [DataField(null, false, 1, false, false, null)]
  public int? OnlinePerStock;
  [DataField(null, false, 1, false, false, null)]
  public int? PerPlayerQuota;
  [DataField(null, false, 1, false, false, null)]
  public int? Points;
  [DataField(null, false, 1, false, false, null)]
  public int Spawn = 1;
  [DataField(null, false, 1, false, false, null)]
  public bool Recommended;
  [DataField(null, false, 1, false, false, null)]
  public int? Multiplier;
  [DataField(null, false, 1, false, false, null)]
  public int? Max;
  [DataField(null, false, 1, false, false, null)]
  public List<EntProtoId> LinkedEntries = new List<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? Box;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? BoxAmount;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? BoxSlots;
  [DataField(null, false, 1, false, false, null)]
  public LocId? GiveSquadRoleName;
  [DataField(null, false, 1, false, false, null)]
  public bool IsAppendSquadRoleName;
  [DataField(null, false, 1, false, false, null)]
  public LocId? GivePrefix;
  [DataField(null, false, 1, false, false, null)]
  public bool IsAppendPrefix;
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier.Rsi? GiveIcon;
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier.Rsi? GiveMapBlip;
  [DataField(null, false, 1, false, false, null)]
  public SlotFlags? ReplaceSlot;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMVendorEntry target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<CMVendorEntry>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target1 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Id, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<EntProtoId>(this.Id, hookCtx, context);
    target.Id = target1;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Name, ref target2, hookCtx, false, context))
      target2 = this.Name;
    target.Name = target2;
    int? target3 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Amount, ref target3, hookCtx, false, context))
      target3 = this.Amount;
    target.Amount = target3;
    int? target4 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.OnlinePerStock, ref target4, hookCtx, false, context))
      target4 = this.OnlinePerStock;
    target.OnlinePerStock = target4;
    int? target5 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.PerPlayerQuota, ref target5, hookCtx, false, context))
      target5 = this.PerPlayerQuota;
    target.PerPlayerQuota = target5;
    int? target6 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Points, ref target6, hookCtx, false, context))
      target6 = this.Points;
    target.Points = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.Spawn, ref target7, hookCtx, false, context))
      target7 = this.Spawn;
    target.Spawn = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.Recommended, ref target8, hookCtx, false, context))
      target8 = this.Recommended;
    target.Recommended = target8;
    int? target9 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Multiplier, ref target9, hookCtx, false, context))
      target9 = this.Multiplier;
    target.Multiplier = target9;
    int? target10 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Max, ref target10, hookCtx, false, context))
      target10 = this.Max;
    target.Max = target10;
    List<EntProtoId> target11 = (List<EntProtoId>) null;
    if (this.LinkedEntries == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.LinkedEntries, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<List<EntProtoId>>(this.LinkedEntries, hookCtx, context);
    target.LinkedEntries = target11;
    EntProtoId? target12 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.Box, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<EntProtoId?>(this.Box, hookCtx, context);
    target.Box = target12;
    int? target13 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.BoxAmount, ref target13, hookCtx, false, context))
      target13 = this.BoxAmount;
    target.BoxAmount = target13;
    int? target14 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.BoxSlots, ref target14, hookCtx, false, context))
      target14 = this.BoxSlots;
    target.BoxSlots = target14;
    LocId? target15 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.GiveSquadRoleName, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<LocId?>(this.GiveSquadRoleName, hookCtx, context);
    target.GiveSquadRoleName = target15;
    bool target16 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsAppendSquadRoleName, ref target16, hookCtx, false, context))
      target16 = this.IsAppendSquadRoleName;
    target.IsAppendSquadRoleName = target16;
    LocId? target17 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.GivePrefix, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<LocId?>(this.GivePrefix, hookCtx, context);
    target.GivePrefix = target17;
    bool target18 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsAppendPrefix, ref target18, hookCtx, false, context))
      target18 = this.IsAppendPrefix;
    target.IsAppendPrefix = target18;
    SpriteSpecifier.Rsi target19 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.GiveIcon, ref target19, hookCtx, false, context))
    {
      if (this.GiveIcon == null)
        target19 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.GiveIcon, ref target19, hookCtx, context);
    }
    target.GiveIcon = target19;
    SpriteSpecifier.Rsi target20 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.GiveMapBlip, ref target20, hookCtx, false, context))
    {
      if (this.GiveMapBlip == null)
        target20 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.GiveMapBlip, ref target20, hookCtx, context);
    }
    target.GiveMapBlip = target20;
    SlotFlags? target21 = new SlotFlags?();
    if (!serialization.TryCustomCopy<SlotFlags?>(this.ReplaceSlot, ref target21, hookCtx, false, context))
      target21 = this.ReplaceSlot;
    target.ReplaceSlot = target21;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMVendorEntry target,
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
    CMVendorEntry target1 = (CMVendorEntry) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public CMVendorEntry Instantiate() => new CMVendorEntry();

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return ((((((((((((((((((((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<EntProtoId>.Default.GetHashCode(this.Id)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Name)) * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(this.Amount)) * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(this.OnlinePerStock)) * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(this.PerPlayerQuota)) * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(this.Points)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.Spawn)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Recommended)) * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(this.Multiplier)) * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(this.Max)) * -1521134295 + EqualityComparer<List<EntProtoId>>.Default.GetHashCode(this.LinkedEntries)) * -1521134295 + EqualityComparer<EntProtoId?>.Default.GetHashCode(this.Box)) * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(this.BoxAmount)) * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(this.BoxSlots)) * -1521134295 + EqualityComparer<LocId?>.Default.GetHashCode(this.GiveSquadRoleName)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.IsAppendSquadRoleName)) * -1521134295 + EqualityComparer<LocId?>.Default.GetHashCode(this.GivePrefix)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.IsAppendPrefix)) * -1521134295 + EqualityComparer<SpriteSpecifier.Rsi>.Default.GetHashCode(this.GiveIcon)) * -1521134295 + EqualityComparer<SpriteSpecifier.Rsi>.Default.GetHashCode(this.GiveMapBlip)) * -1521134295 + EqualityComparer<SlotFlags?>.Default.GetHashCode(this.ReplaceSlot);
  }

  [CompilerGenerated]
  public bool Equals(CMVendorEntry? other)
  {
    if ((object) this == (object) other)
      return true;
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<EntProtoId>.Default.Equals(this.Id, other.Id) && EqualityComparer<string>.Default.Equals(this.Name, other.Name) && EqualityComparer<int?>.Default.Equals(this.Amount, other.Amount) && EqualityComparer<int?>.Default.Equals(this.OnlinePerStock, other.OnlinePerStock) && EqualityComparer<int?>.Default.Equals(this.PerPlayerQuota, other.PerPlayerQuota) && EqualityComparer<int?>.Default.Equals(this.Points, other.Points) && EqualityComparer<int>.Default.Equals(this.Spawn, other.Spawn) && EqualityComparer<bool>.Default.Equals(this.Recommended, other.Recommended) && EqualityComparer<int?>.Default.Equals(this.Multiplier, other.Multiplier) && EqualityComparer<int?>.Default.Equals(this.Max, other.Max) && EqualityComparer<List<EntProtoId>>.Default.Equals(this.LinkedEntries, other.LinkedEntries) && EqualityComparer<EntProtoId?>.Default.Equals(this.Box, other.Box) && EqualityComparer<int?>.Default.Equals(this.BoxAmount, other.BoxAmount) && EqualityComparer<int?>.Default.Equals(this.BoxSlots, other.BoxSlots) && EqualityComparer<LocId?>.Default.Equals(this.GiveSquadRoleName, other.GiveSquadRoleName) && EqualityComparer<bool>.Default.Equals(this.IsAppendSquadRoleName, other.IsAppendSquadRoleName) && EqualityComparer<LocId?>.Default.Equals(this.GivePrefix, other.GivePrefix) && EqualityComparer<bool>.Default.Equals(this.IsAppendPrefix, other.IsAppendPrefix) && EqualityComparer<SpriteSpecifier.Rsi>.Default.Equals(this.GiveIcon, other.GiveIcon) && EqualityComparer<SpriteSpecifier.Rsi>.Default.Equals(this.GiveMapBlip, other.GiveMapBlip) && EqualityComparer<SlotFlags?>.Default.Equals(this.ReplaceSlot, other.ReplaceSlot);
  }
}
