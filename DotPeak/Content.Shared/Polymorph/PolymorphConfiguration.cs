// Decompiled with JetBrains decompiler
// Type: Content.Shared.Polymorph.PolymorphConfiguration
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Polymorph;

[DataDefinition]
public sealed record PolymorphConfiguration : 
  ISerializationGenerated<PolymorphConfiguration>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, true, null)]
  public EntProtoId Entity;
  [DataField(null, false, 1, false, true, null)]
  public EntProtoId? EffectProto;
  [DataField(null, false, 1, false, true, null)]
  public int Delay = 60;
  [DataField(null, false, 1, false, true, null)]
  public int? Duration;
  [DataField(null, false, 1, false, true, null)]
  public bool Forced;
  [DataField(null, false, 1, false, true, null)]
  public bool TransferDamage = true;
  [DataField(null, false, 1, false, true, null)]
  public bool TransferName;
  [DataField(null, false, 1, false, true, null)]
  public bool TransferHumanoidAppearance;
  [DataField(null, false, 1, false, true, null)]
  public PolymorphInventoryChange Inventory;
  [DataField(null, false, 1, false, true, null)]
  public bool RevertOnCrit = true;
  [DataField(null, false, 1, false, true, null)]
  public bool RevertOnDeath = true;
  [DataField(null, false, 1, false, true, null)]
  public bool RevertOnEat;
  [DataField(null, false, 1, false, true, null)]
  public bool AllowRepeatedMorphs;
  [DataField(null, false, 1, false, true, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan Cooldown = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? PolymorphSound;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? ExitPolymorphSound;
  [DataField(null, false, 1, false, false, null)]
  public LocId? PolymorphPopup = (LocId?) "polymorph-popup-generic";
  [DataField(null, false, 1, false, false, null)]
  public LocId? ExitPolymorphPopup = (LocId?) "polymorph-revert-popup-generic";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PolymorphConfiguration target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<PolymorphConfiguration>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target1 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Entity, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<EntProtoId>(this.Entity, hookCtx, context);
    target.Entity = target1;
    EntProtoId? target2 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.EffectProto, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId?>(this.EffectProto, hookCtx, context);
    target.EffectProto = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Delay, ref target3, hookCtx, false, context))
      target3 = this.Delay;
    target.Delay = target3;
    int? target4 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Duration, ref target4, hookCtx, false, context))
      target4 = this.Duration;
    target.Duration = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Forced, ref target5, hookCtx, false, context))
      target5 = this.Forced;
    target.Forced = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.TransferDamage, ref target6, hookCtx, false, context))
      target6 = this.TransferDamage;
    target.TransferDamage = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.TransferName, ref target7, hookCtx, false, context))
      target7 = this.TransferName;
    target.TransferName = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.TransferHumanoidAppearance, ref target8, hookCtx, false, context))
      target8 = this.TransferHumanoidAppearance;
    target.TransferHumanoidAppearance = target8;
    PolymorphInventoryChange target9 = PolymorphInventoryChange.None;
    if (!serialization.TryCustomCopy<PolymorphInventoryChange>(this.Inventory, ref target9, hookCtx, false, context))
      target9 = this.Inventory;
    target.Inventory = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.RevertOnCrit, ref target10, hookCtx, false, context))
      target10 = this.RevertOnCrit;
    target.RevertOnCrit = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.RevertOnDeath, ref target11, hookCtx, false, context))
      target11 = this.RevertOnDeath;
    target.RevertOnDeath = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.RevertOnEat, ref target12, hookCtx, false, context))
      target12 = this.RevertOnEat;
    target.RevertOnEat = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowRepeatedMorphs, ref target13, hookCtx, false, context))
      target13 = this.AllowRepeatedMorphs;
    target.AllowRepeatedMorphs = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target14;
    SoundSpecifier target15 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PolymorphSound, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<SoundSpecifier>(this.PolymorphSound, hookCtx, context);
    target.PolymorphSound = target15;
    SoundSpecifier target16 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ExitPolymorphSound, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<SoundSpecifier>(this.ExitPolymorphSound, hookCtx, context);
    target.ExitPolymorphSound = target16;
    LocId? target17 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.PolymorphPopup, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<LocId?>(this.PolymorphPopup, hookCtx, context);
    target.PolymorphPopup = target17;
    LocId? target18 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.ExitPolymorphPopup, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<LocId?>(this.ExitPolymorphPopup, hookCtx, context);
    target.ExitPolymorphPopup = target18;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PolymorphConfiguration target,
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
    PolymorphConfiguration target1 = (PolymorphConfiguration) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public PolymorphConfiguration Instantiate() => new PolymorphConfiguration();

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return (((((((((((((((((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<EntProtoId>.Default.GetHashCode(this.Entity)) * -1521134295 + EqualityComparer<EntProtoId?>.Default.GetHashCode(this.EffectProto)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.Delay)) * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(this.Duration)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Forced)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.TransferDamage)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.TransferName)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.TransferHumanoidAppearance)) * -1521134295 + EqualityComparer<PolymorphInventoryChange>.Default.GetHashCode(this.Inventory)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.RevertOnCrit)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.RevertOnDeath)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.RevertOnEat)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.AllowRepeatedMorphs)) * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(this.Cooldown)) * -1521134295 + EqualityComparer<SoundSpecifier>.Default.GetHashCode(this.PolymorphSound)) * -1521134295 + EqualityComparer<SoundSpecifier>.Default.GetHashCode(this.ExitPolymorphSound)) * -1521134295 + EqualityComparer<LocId?>.Default.GetHashCode(this.PolymorphPopup)) * -1521134295 + EqualityComparer<LocId?>.Default.GetHashCode(this.ExitPolymorphPopup);
  }

  [CompilerGenerated]
  public bool Equals(PolymorphConfiguration? other)
  {
    if ((object) this == (object) other)
      return true;
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<EntProtoId>.Default.Equals(this.Entity, other.Entity) && EqualityComparer<EntProtoId?>.Default.Equals(this.EffectProto, other.EffectProto) && EqualityComparer<int>.Default.Equals(this.Delay, other.Delay) && EqualityComparer<int?>.Default.Equals(this.Duration, other.Duration) && EqualityComparer<bool>.Default.Equals(this.Forced, other.Forced) && EqualityComparer<bool>.Default.Equals(this.TransferDamage, other.TransferDamage) && EqualityComparer<bool>.Default.Equals(this.TransferName, other.TransferName) && EqualityComparer<bool>.Default.Equals(this.TransferHumanoidAppearance, other.TransferHumanoidAppearance) && EqualityComparer<PolymorphInventoryChange>.Default.Equals(this.Inventory, other.Inventory) && EqualityComparer<bool>.Default.Equals(this.RevertOnCrit, other.RevertOnCrit) && EqualityComparer<bool>.Default.Equals(this.RevertOnDeath, other.RevertOnDeath) && EqualityComparer<bool>.Default.Equals(this.RevertOnEat, other.RevertOnEat) && EqualityComparer<bool>.Default.Equals(this.AllowRepeatedMorphs, other.AllowRepeatedMorphs) && EqualityComparer<TimeSpan>.Default.Equals(this.Cooldown, other.Cooldown) && EqualityComparer<SoundSpecifier>.Default.Equals(this.PolymorphSound, other.PolymorphSound) && EqualityComparer<SoundSpecifier>.Default.Equals(this.ExitPolymorphSound, other.ExitPolymorphSound) && EqualityComparer<LocId?>.Default.Equals(this.PolymorphPopup, other.PolymorphPopup) && EqualityComparer<LocId?>.Default.Equals(this.ExitPolymorphPopup, other.ExitPolymorphPopup);
  }
}
