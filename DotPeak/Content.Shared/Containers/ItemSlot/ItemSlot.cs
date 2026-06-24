// Decompiled with JetBrains decompiler
// Type: Content.Shared.Containers.ItemSlots.ItemSlot
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;

#nullable enable
namespace Content.Shared.Containers.ItemSlots;

[DataDefinition]
[Access(new Type[] {typeof (ItemSlotsSystem)})]
[NetSerializable]
[Serializable]
public sealed class ItemSlot : ISerializationGenerated<ItemSlot>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (ItemSlotsSystem)})]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? InsertSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/MagIn/revolver_magin.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? EjectSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/MagOut/revolver_magout.ogg", new AudioParams?());
  [DataField(null, true, 1, false, false, null)]
  [Access(new Type[] {typeof (ItemSlotsSystem)})]
  public string Name = string.Empty;
  [DataField(null, true, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  [Access(new Type[] {typeof (ItemSlotsSystem)})]
  [NonSerialized]
  public string? StartingItem;
  [DataField(null, true, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Locked;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool DisableEject;
  [DataField(null, false, 1, false, false, null)]
  public bool InsertOnInteract = true;
  [DataField(null, false, 1, false, false, null)]
  public bool EjectOnInteract;
  [DataField(null, false, 1, false, false, null)]
  public bool EjectOnUse;
  [DataField(null, false, 1, false, false, null)]
  public string? InsertVerbText;
  [DataField(null, false, 1, false, false, null)]
  public string? EjectVerbText;
  [Robust.Shared.ViewVariables.ViewVariables]
  [NonSerialized]
  public ContainerSlot? ContainerSlot;
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (ItemSlotsSystem)})]
  [NonSerialized]
  public bool EjectOnDeconstruct = true;
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (ItemSlotsSystem)})]
  [NonSerialized]
  public bool EjectOnBreak;
  [DataField(null, false, 1, false, false, null)]
  public LocId? WhitelistFailPopup;
  [DataField(null, false, 1, false, false, null)]
  public LocId? LockedFailPopup;
  [DataField(null, false, 1, false, false, null)]
  public LocId? InsertSuccessPopup;
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (ItemSlotsSystem)})]
  public bool Swap = true;
  [DataField(null, false, 1, false, false, null)]
  public int Priority;
  [NonSerialized]
  public bool Local = true;

  public ItemSlot()
  {
  }

  public ItemSlot(ItemSlot other) => this.CopyFrom(other);

  public string? ID => ((BaseContainer) this.ContainerSlot)?.ID;

  public bool HasItem
  {
    get
    {
      ContainerSlot containerSlot = this.ContainerSlot;
      return containerSlot != null && containerSlot.ContainedEntity.HasValue;
    }
  }

  public EntityUid? Item => this.ContainerSlot?.ContainedEntity;

  public void CopyFrom(ItemSlot other)
  {
    this.Whitelist = other.Whitelist;
    this.InsertSound = other.InsertSound;
    this.EjectSound = other.EjectSound;
    this.Name = other.Name;
    this.Locked = other.Locked;
    this.InsertOnInteract = other.InsertOnInteract;
    this.EjectOnInteract = other.EjectOnInteract;
    this.EjectOnUse = other.EjectOnUse;
    this.InsertVerbText = other.InsertVerbText;
    this.EjectVerbText = other.EjectVerbText;
    this.WhitelistFailPopup = other.WhitelistFailPopup;
    this.LockedFailPopup = other.LockedFailPopup;
    this.InsertSuccessPopup = other.InsertSuccessPopup;
    this.Swap = other.Swap;
    this.Priority = other.Priority;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ItemSlot target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ItemSlot>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist entityWhitelist1 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref entityWhitelist1, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        entityWhitelist1 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref entityWhitelist1, hookCtx, context, false);
    }
    target.Whitelist = entityWhitelist1;
    EntityWhitelist entityWhitelist2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref entityWhitelist2, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        entityWhitelist2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref entityWhitelist2, hookCtx, context, false);
    }
    target.Blacklist = entityWhitelist2;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InsertSound, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.InsertSound, hookCtx, context, false);
    target.InsertSound = soundSpecifier1;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EjectSound, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.EjectSound, hookCtx, context, false);
    target.EjectSound = soundSpecifier2;
    string str1 = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref str1, hookCtx, false, context))
      str1 = this.Name;
    target.Name = str1;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.StartingItem, ref str2, hookCtx, false, context))
      str2 = this.StartingItem;
    target.StartingItem = str2;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Locked, ref flag1, hookCtx, false, context))
      flag1 = this.Locked;
    target.Locked = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.DisableEject, ref flag2, hookCtx, false, context))
      flag2 = this.DisableEject;
    target.DisableEject = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.InsertOnInteract, ref flag3, hookCtx, false, context))
      flag3 = this.InsertOnInteract;
    target.InsertOnInteract = flag3;
    bool flag4 = false;
    if (!serialization.TryCustomCopy<bool>(this.EjectOnInteract, ref flag4, hookCtx, false, context))
      flag4 = this.EjectOnInteract;
    target.EjectOnInteract = flag4;
    bool flag5 = false;
    if (!serialization.TryCustomCopy<bool>(this.EjectOnUse, ref flag5, hookCtx, false, context))
      flag5 = this.EjectOnUse;
    target.EjectOnUse = flag5;
    string str3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.InsertVerbText, ref str3, hookCtx, false, context))
      str3 = this.InsertVerbText;
    target.InsertVerbText = str3;
    string str4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.EjectVerbText, ref str4, hookCtx, false, context))
      str4 = this.EjectVerbText;
    target.EjectVerbText = str4;
    bool flag6 = false;
    if (!serialization.TryCustomCopy<bool>(this.EjectOnDeconstruct, ref flag6, hookCtx, false, context))
      flag6 = this.EjectOnDeconstruct;
    target.EjectOnDeconstruct = flag6;
    bool flag7 = false;
    if (!serialization.TryCustomCopy<bool>(this.EjectOnBreak, ref flag7, hookCtx, false, context))
      flag7 = this.EjectOnBreak;
    target.EjectOnBreak = flag7;
    LocId? nullable1 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.WhitelistFailPopup, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<LocId?>(this.WhitelistFailPopup, hookCtx, context, false);
    target.WhitelistFailPopup = nullable1;
    LocId? nullable2 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.LockedFailPopup, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<LocId?>(this.LockedFailPopup, hookCtx, context, false);
    target.LockedFailPopup = nullable2;
    LocId? nullable3 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.InsertSuccessPopup, ref nullable3, hookCtx, false, context))
      nullable3 = serialization.CreateCopy<LocId?>(this.InsertSuccessPopup, hookCtx, context, false);
    target.InsertSuccessPopup = nullable3;
    bool flag8 = false;
    if (!serialization.TryCustomCopy<bool>(this.Swap, ref flag8, hookCtx, false, context))
      flag8 = this.Swap;
    target.Swap = flag8;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.Priority, ref num, hookCtx, false, context))
      num = this.Priority;
    target.Priority = num;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ItemSlot target,
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
    ItemSlot target1 = (ItemSlot) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ItemSlot Instantiate() => new ItemSlot();
}
