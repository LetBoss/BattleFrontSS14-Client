// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.StorageComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Item;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Storage;

[RegisterComponent]
[NetworkedComponent]
public sealed class StorageComponent : 
  Component,
  ISerializationGenerated<StorageComponent>,
  ISerializationGenerated
{
  public static string ContainerId = "storagebase";
  public const byte ChunkSize = 8;
  public Dictionary<Vector2i, ulong> OccupiedGrid = new Dictionary<Vector2i, ulong>();
  [Robust.Shared.ViewVariables.ViewVariables]
  public Container Container;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Dictionary<EntityUid, ItemStorageLocation> StoredItems = new Dictionary<EntityUid, ItemStorageLocation>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, List<ItemStorageLocation>> SavedLocations = new Dictionary<string, List<ItemStorageLocation>>();
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public List<Box2i> Grid = new List<Box2i>();
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [Access(new Type[] {typeof (SharedStorageSystem)})]
  public ProtoId<ItemSizePrototype>? MaxItemSize;
  [DataField(null, false, 1, false, false, null)]
  public bool QuickInsert;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan QuickInsertCooldown = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan OpenUiCooldown = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public bool ClickInsert = true;
  [DataField(null, false, 1, false, false, null)]
  public bool AllowStorageTransfer = true;
  [DataField(null, false, 1, false, false, null)]
  public bool OpenOnActivate = true;
  public const int AreaPickupLimit = 10;
  [DataField(null, false, 1, false, false, null)]
  public bool AreaInsert;
  [DataField(null, false, 1, false, false, null)]
  public int AreaInsertRadius = 1;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? StorageInsertSound = (SoundSpecifier) new SoundCollectionSpecifier("storageRustle");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? StorageRemoveSound;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? StorageOpenSound = (SoundSpecifier) new SoundCollectionSpecifier("storageRustle");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? StorageCloseSound;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public StorageDefaultOrientation? DefaultStorageOrientation;
  [DataField(null, false, 1, false, false, null)]
  public bool HideStackVisualsWhenClosed = true;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<TagPrototype> SilentStorageUserTag = (ProtoId<TagPrototype>) "SilentStorageUser";
  [DataField(null, false, 1, false, false, null)]
  public bool ShowVerb = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StorageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StorageComponent) target1;
    if (serialization.TryCustomCopy<StorageComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<EntityUid, ItemStorageLocation> target2 = (Dictionary<EntityUid, ItemStorageLocation>) null;
    if (this.StoredItems == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntityUid, ItemStorageLocation>>(this.StoredItems, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<EntityUid, ItemStorageLocation>>(this.StoredItems, hookCtx, context);
    target.StoredItems = target2;
    Dictionary<string, List<ItemStorageLocation>> target3 = (Dictionary<string, List<ItemStorageLocation>>) null;
    if (this.SavedLocations == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, List<ItemStorageLocation>>>(this.SavedLocations, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<string, List<ItemStorageLocation>>>(this.SavedLocations, hookCtx, context);
    target.SavedLocations = target3;
    List<Box2i> target4 = (List<Box2i>) null;
    if (this.Grid == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<Box2i>>(this.Grid, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<Box2i>>(this.Grid, hookCtx, context);
    target.Grid = target4;
    ProtoId<ItemSizePrototype>? target5 = new ProtoId<ItemSizePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<ItemSizePrototype>?>(this.MaxItemSize, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<ItemSizePrototype>?>(this.MaxItemSize, hookCtx, context);
    target.MaxItemSize = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.QuickInsert, ref target6, hookCtx, false, context))
      target6 = this.QuickInsert;
    target.QuickInsert = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.QuickInsertCooldown, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.QuickInsertCooldown, hookCtx, context);
    target.QuickInsertCooldown = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.OpenUiCooldown, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.OpenUiCooldown, hookCtx, context);
    target.OpenUiCooldown = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.ClickInsert, ref target9, hookCtx, false, context))
      target9 = this.ClickInsert;
    target.ClickInsert = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowStorageTransfer, ref target10, hookCtx, false, context))
      target10 = this.AllowStorageTransfer;
    target.AllowStorageTransfer = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.OpenOnActivate, ref target11, hookCtx, false, context))
      target11 = this.OpenOnActivate;
    target.OpenOnActivate = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.AreaInsert, ref target12, hookCtx, false, context))
      target12 = this.AreaInsert;
    target.AreaInsert = target12;
    int target13 = 0;
    if (!serialization.TryCustomCopy<int>(this.AreaInsertRadius, ref target13, hookCtx, false, context))
      target13 = this.AreaInsertRadius;
    target.AreaInsertRadius = target13;
    EntityWhitelist target14 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target14, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target14 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target14, hookCtx, context);
    }
    target.Whitelist = target14;
    EntityWhitelist target15 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target15, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target15 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target15, hookCtx, context);
    }
    target.Blacklist = target15;
    SoundSpecifier target16 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.StorageInsertSound, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<SoundSpecifier>(this.StorageInsertSound, hookCtx, context);
    target.StorageInsertSound = target16;
    SoundSpecifier target17 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.StorageRemoveSound, ref target17, hookCtx, true, context))
      target17 = serialization.CreateCopy<SoundSpecifier>(this.StorageRemoveSound, hookCtx, context);
    target.StorageRemoveSound = target17;
    SoundSpecifier target18 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.StorageOpenSound, ref target18, hookCtx, true, context))
      target18 = serialization.CreateCopy<SoundSpecifier>(this.StorageOpenSound, hookCtx, context);
    target.StorageOpenSound = target18;
    SoundSpecifier target19 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.StorageCloseSound, ref target19, hookCtx, true, context))
      target19 = serialization.CreateCopy<SoundSpecifier>(this.StorageCloseSound, hookCtx, context);
    target.StorageCloseSound = target19;
    StorageDefaultOrientation? target20 = new StorageDefaultOrientation?();
    if (!serialization.TryCustomCopy<StorageDefaultOrientation?>(this.DefaultStorageOrientation, ref target20, hookCtx, false, context))
      target20 = this.DefaultStorageOrientation;
    target.DefaultStorageOrientation = target20;
    bool target21 = false;
    if (!serialization.TryCustomCopy<bool>(this.HideStackVisualsWhenClosed, ref target21, hookCtx, false, context))
      target21 = this.HideStackVisualsWhenClosed;
    target.HideStackVisualsWhenClosed = target21;
    ProtoId<TagPrototype> target22 = new ProtoId<TagPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<TagPrototype>>(this.SilentStorageUserTag, ref target22, hookCtx, false, context))
      target22 = serialization.CreateCopy<ProtoId<TagPrototype>>(this.SilentStorageUserTag, hookCtx, context);
    target.SilentStorageUserTag = target22;
    bool target23 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowVerb, ref target23, hookCtx, false, context))
      target23 = this.ShowVerb;
    target.ShowVerb = target23;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StorageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StorageComponent target1 = (StorageComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StorageComponent target1 = (StorageComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StorageComponent target1 = (StorageComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual StorageComponent Component.Instantiate() => new StorageComponent();

  [NetSerializable]
  [Serializable]
  public enum StorageUiKey : byte
  {
    Key,
  }
}
