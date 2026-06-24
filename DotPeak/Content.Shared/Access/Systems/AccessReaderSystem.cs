// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Systems.AccessReaderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.DeviceLinking.Events;
using Content.Shared.Emag.Systems;
using Content.Shared.GameTicking;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Inventory;
using Content.Shared.NameIdentifier;
using Content.Shared.PDA;
using Content.Shared.StationRecords;
using Content.Shared.Tag;
using Robust.Shared.Collections;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared.Access.Systems;

public sealed class AccessReaderSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private InventorySystem _inventorySystem;
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private EmagSystem _emag;
  [Dependency]
  private TagSystem _tag;
  [Dependency]
  private SharedGameTicker _gameTicker;
  [Dependency]
  private SharedHandsSystem _handsSystem;
  [Dependency]
  private SharedContainerSystem _containerSystem;
  [Dependency]
  private SharedStationRecordsSystem _recordsSystem;
  private static readonly ProtoId<TagPrototype> PreventAccessLoggingTag = ProtoId<TagPrototype>.op_Implicit("PreventAccessLogging");

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AccessReaderComponent, GotEmaggedEvent>(new ComponentEventRefHandler<AccessReaderComponent, GotEmaggedEvent>((object) this, __methodptr(OnEmagged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AccessReaderComponent, LinkAttemptEvent>(new ComponentEventHandler<AccessReaderComponent, LinkAttemptEvent>((object) this, __methodptr(OnLinkAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AccessReaderComponent, ComponentGetState>(new ComponentEventRefHandler<AccessReaderComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AccessReaderComponent, ComponentHandleState>(new ComponentEventRefHandler<AccessReaderComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnGetState(
    EntityUid uid,
    AccessReaderComponent component,
    ref ComponentGetState args)
  {
    ((ComponentGetState) ref args).State = (IComponentState) new AccessReaderComponentState(component.Enabled, component.DenyTags, component.AccessLists, this._recordsSystem.Convert((ICollection<StationRecordKey>) component.AccessKeys), component.AccessLog, component.AccessLogLimit);
  }

  private void OnHandleState(
    EntityUid uid,
    AccessReaderComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is AccessReaderComponentState current))
      return;
    component.Enabled = current.Enabled;
    component.AccessKeys.Clear();
    foreach ((NetEntity, uint) accessKey in current.AccessKeys)
    {
      EntityUid originStation = this.EnsureEntity<AccessReaderComponent>(accessKey.Item1, uid);
      if (((EntityUid) ref originStation).IsValid())
        component.AccessKeys.Add(new StationRecordKey(accessKey.Item2, originStation));
    }
    component.AccessLists = new List<HashSet<ProtoId<AccessLevelPrototype>>>((IEnumerable<HashSet<ProtoId<AccessLevelPrototype>>>) current.AccessLists);
    component.DenyTags = new HashSet<ProtoId<AccessLevelPrototype>>((IEnumerable<ProtoId<AccessLevelPrototype>>) current.DenyTags);
    component.AccessLog = new Queue<AccessRecord>((IEnumerable<AccessRecord>) current.AccessLog);
    component.AccessLogLimit = current.AccessLogLimit;
  }

  private void OnLinkAttempt(EntityUid uid, AccessReaderComponent component, LinkAttemptEvent args)
  {
    if (!args.User.HasValue || this.IsAllowed(args.User.Value, uid, component))
      return;
    args.Cancel();
  }

  private void OnEmagged(EntityUid uid, AccessReaderComponent reader, ref GotEmaggedEvent args)
  {
    Entity<AccessReaderComponent>? ent;
    if (!this._emag.CompareFlag(args.Type, EmagType.Access) || !reader.BreakOnAccessBreaker || !this.GetMainAccessReader(uid, out ent) || ent.Value.Comp.AccessLists.Count < 1)
      return;
    args.Repeatable = true;
    args.Handled = true;
    ent.Value.Comp.AccessLists.Clear();
    ent.Value.Comp.AccessLog.Clear();
    this.Dirty(uid, (IComponent) reader, (MetaDataComponent) null);
  }

  public bool IsAllowed(EntityUid user, EntityUid target, AccessReaderComponent? reader = null)
  {
    if (!this.Resolve<AccessReaderComponent>(target, ref reader, false) || !reader.Enabled)
      return true;
    HashSet<EntityUid> potentialAccessItems = this.FindPotentialAccessItems(user);
    ICollection<ProtoId<AccessLevelPrototype>> accessTags = this.FindAccessTags(user, potentialAccessItems);
    ICollection<StationRecordKey> recordKeys;
    this.FindStationRecordKeys(user, out recordKeys, potentialAccessItems);
    if (!this.IsAllowed(accessTags, recordKeys, target, reader))
      return false;
    if (!this._tag.HasTag(user, AccessReaderSystem.PreventAccessLoggingTag))
      this.LogAccess(Entity<AccessReaderComponent>.op_Implicit((target, reader)), user);
    return true;
  }

  public bool GetMainAccessReader(EntityUid uid, [NotNullWhen(true)] out Entity<AccessReaderComponent>? ent)
  {
    ent = new Entity<AccessReaderComponent>?();
    AccessReaderComponent accessReaderComponent1;
    if (!this.TryComp<AccessReaderComponent>(uid, ref accessReaderComponent1))
      return false;
    ent = new Entity<AccessReaderComponent>?(Entity<AccessReaderComponent>.op_Implicit((uid, accessReaderComponent1)));
    BaseContainer baseContainer;
    if (ent.Value.Comp.ContainerAccessProvider == null || !this._containerSystem.TryGetContainer(uid, ent.Value.Comp.ContainerAccessProvider, ref baseContainer, (ContainerManagerComponent) null))
      return true;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) baseContainer.ContainedEntities)
    {
      AccessReaderComponent accessReaderComponent2;
      if (this.TryComp<AccessReaderComponent>(containedEntity, ref accessReaderComponent2))
      {
        ent = new Entity<AccessReaderComponent>?(Entity<AccessReaderComponent>.op_Implicit((containedEntity, accessReaderComponent2)));
        return true;
      }
    }
    return true;
  }

  public bool IsAllowed(
    ICollection<ProtoId<AccessLevelPrototype>> access,
    ICollection<StationRecordKey> stationKeys,
    EntityUid target,
    AccessReaderComponent reader)
  {
    if (!reader.Enabled)
      return true;
    if (reader.ContainerAccessProvider == null)
      return this.IsAllowedInternal(access, stationKeys, reader);
    BaseContainer baseContainer;
    if (!this._containerSystem.TryGetContainer(target, reader.ContainerAccessProvider, ref baseContainer, (ContainerManagerComponent) null))
      return false;
    if (this.Paused(target, (MetaDataComponent) null))
      return true;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) baseContainer.ContainedEntities)
    {
      AccessReaderComponent reader1;
      if (this.TryComp<AccessReaderComponent>(containedEntity, ref reader1) && this.IsAllowed(access, stationKeys, containedEntity, reader1))
        return true;
    }
    return false;
  }

  private bool IsAllowedInternal(
    ICollection<ProtoId<AccessLevelPrototype>> access,
    ICollection<StationRecordKey> stationKeys,
    AccessReaderComponent reader)
  {
    return !reader.Enabled || this.AreAccessTagsAllowed(access, reader) || this.AreStationRecordKeysAllowed(stationKeys, reader);
  }

  public bool AreAccessTagsAllowed(
    ICollection<ProtoId<AccessLevelPrototype>> accessTags,
    AccessReaderComponent reader)
  {
    if (reader.DenyTags.Overlaps((IEnumerable<ProtoId<AccessLevelPrototype>>) accessTags))
      return false;
    if (reader.AccessLists.Count == 0)
      return true;
    foreach (HashSet<ProtoId<AccessLevelPrototype>> accessList in reader.AccessLists)
    {
      if (accessList.IsSubsetOf((IEnumerable<ProtoId<AccessLevelPrototype>>) accessTags))
        return true;
    }
    return false;
  }

  public bool AreStationRecordKeysAllowed(
    ICollection<StationRecordKey> keys,
    AccessReaderComponent reader)
  {
    foreach (StationRecordKey accessKey in reader.AccessKeys)
    {
      if (keys.Contains(accessKey))
        return true;
    }
    return false;
  }

  public HashSet<EntityUid> FindPotentialAccessItems(EntityUid uid)
  {
    HashSet<EntityUid> items;
    this.FindAccessItemsInventory(uid, out items);
    GetAdditionalAccessEvent additionalAccessEvent = new GetAdditionalAccessEvent()
    {
      Entities = items
    };
    this.RaiseLocalEvent<GetAdditionalAccessEvent>(uid, ref additionalAccessEvent, false);
    foreach (EntityUid uid1 in new ValueList<EntityUid>((IReadOnlyCollection<EntityUid>) items))
      items.UnionWith((IEnumerable<EntityUid>) this.FindPotentialAccessItems(uid1));
    items.Add(uid);
    return items;
  }

  public ICollection<ProtoId<AccessLevelPrototype>> FindAccessTags(
    EntityUid uid,
    HashSet<EntityUid>? items = null)
  {
    HashSet<ProtoId<AccessLevelPrototype>> tags = (HashSet<ProtoId<AccessLevelPrototype>>) null;
    bool owned = false;
    if (items == null)
      items = this.FindPotentialAccessItems(uid);
    foreach (EntityUid uid1 in items)
      this.FindAccessTagsItem(uid1, ref tags, ref owned);
    return (ICollection<ProtoId<AccessLevelPrototype>>) tags ?? (ICollection<ProtoId<AccessLevelPrototype>>) Array.Empty<ProtoId<AccessLevelPrototype>>();
  }

  public bool FindStationRecordKeys(
    EntityUid uid,
    out ICollection<StationRecordKey> recordKeys,
    HashSet<EntityUid>? items = null)
  {
    recordKeys = (ICollection<StationRecordKey>) new HashSet<StationRecordKey>();
    if (items == null)
      items = this.FindPotentialAccessItems(uid);
    foreach (EntityUid uid1 in items)
    {
      StationRecordKey? key;
      if (this.FindStationRecordKeyItem(uid1, out key))
        recordKeys.Add(key.Value);
    }
    return recordKeys.Any<StationRecordKey>();
  }

  private void FindAccessTagsItem(
    EntityUid uid,
    ref HashSet<ProtoId<AccessLevelPrototype>>? tags,
    ref bool owned)
  {
    HashSet<ProtoId<AccessLevelPrototype>> tags1;
    if (!this.FindAccessTagsItem(uid, out tags1))
      return;
    if (tags != null)
    {
      if (!owned)
      {
        tags = new HashSet<ProtoId<AccessLevelPrototype>>((IEnumerable<ProtoId<AccessLevelPrototype>>) tags);
        owned = true;
      }
      tags.UnionWith((IEnumerable<ProtoId<AccessLevelPrototype>>) tags1);
    }
    else
    {
      tags = tags1;
      owned = false;
    }
  }

  public void ClearAccesses(Entity<AccessReaderComponent> ent)
  {
    ent.Comp.AccessLists.Clear();
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
    this.RaiseLocalEvent<AccessReaderConfigurationChangedEvent>(Entity<AccessReaderComponent>.op_Implicit(ent), new AccessReaderConfigurationChangedEvent(), false);
  }

  public void SetAccesses(
    Entity<AccessReaderComponent> ent,
    List<HashSet<ProtoId<AccessLevelPrototype>>> accesses)
  {
    ent.Comp.AccessLists.Clear();
    this.AddAccesses(ent, accesses);
  }

  public void SetAccesses(
    Entity<AccessReaderComponent> ent,
    List<ProtoId<AccessLevelPrototype>> accesses)
  {
    ent.Comp.AccessLists.Clear();
    this.AddAccesses(ent, accesses);
  }

  public void AddAccesses(
    Entity<AccessReaderComponent> ent,
    List<HashSet<ProtoId<AccessLevelPrototype>>> accesses)
  {
    foreach (HashSet<ProtoId<AccessLevelPrototype>> access in accesses)
      this.AddAccess(ent, access, false);
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
    this.RaiseLocalEvent<AccessReaderConfigurationChangedEvent>(Entity<AccessReaderComponent>.op_Implicit(ent), new AccessReaderConfigurationChangedEvent(), false);
  }

  public void AddAccesses(
    Entity<AccessReaderComponent> ent,
    List<ProtoId<AccessLevelPrototype>> accesses)
  {
    foreach (ProtoId<AccessLevelPrototype> access in accesses)
      this.AddAccess(ent, access, false);
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
    this.RaiseLocalEvent<AccessReaderConfigurationChangedEvent>(Entity<AccessReaderComponent>.op_Implicit(ent), new AccessReaderConfigurationChangedEvent(), false);
  }

  public void AddAccess(
    Entity<AccessReaderComponent> ent,
    HashSet<ProtoId<AccessLevelPrototype>> access,
    bool dirty = true)
  {
    ent.Comp.AccessLists.Add(access);
    if (!dirty)
      return;
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
    this.RaiseLocalEvent<AccessReaderConfigurationChangedEvent>(Entity<AccessReaderComponent>.op_Implicit(ent), new AccessReaderConfigurationChangedEvent(), false);
  }

  public void AddAccess(
    Entity<AccessReaderComponent> ent,
    ProtoId<AccessLevelPrototype> access,
    bool dirty = true)
  {
    Entity<AccessReaderComponent> ent1 = ent;
    HashSet<ProtoId<AccessLevelPrototype>> access1 = new HashSet<ProtoId<AccessLevelPrototype>>();
    access1.Add(access);
    int num = dirty ? 1 : 0;
    this.AddAccess(ent1, access1, num != 0);
  }

  public void RemoveAccesses(
    Entity<AccessReaderComponent> ent,
    List<HashSet<ProtoId<AccessLevelPrototype>>> accesses)
  {
    foreach (HashSet<ProtoId<AccessLevelPrototype>> access in accesses)
      this.RemoveAccess(ent, access, false);
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
    this.RaiseLocalEvent<AccessReaderConfigurationChangedEvent>(Entity<AccessReaderComponent>.op_Implicit(ent), new AccessReaderConfigurationChangedEvent(), false);
  }

  public void RemoveAccesses(
    Entity<AccessReaderComponent> ent,
    List<ProtoId<AccessLevelPrototype>> accesses)
  {
    foreach (ProtoId<AccessLevelPrototype> access in accesses)
      this.RemoveAccess(ent, access, false);
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
    this.RaiseLocalEvent<AccessReaderConfigurationChangedEvent>(Entity<AccessReaderComponent>.op_Implicit(ent), new AccessReaderConfigurationChangedEvent(), false);
  }

  public void RemoveAccess(
    Entity<AccessReaderComponent> ent,
    HashSet<ProtoId<AccessLevelPrototype>> access,
    bool dirty = true)
  {
    for (int index = ent.Comp.AccessLists.Count - 1; index >= 0; --index)
    {
      if (ent.Comp.AccessLists[index].SetEquals((IEnumerable<ProtoId<AccessLevelPrototype>>) access))
        ent.Comp.AccessLists.RemoveAt(index);
    }
    if (!dirty)
      return;
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
    this.RaiseLocalEvent<AccessReaderConfigurationChangedEvent>(Entity<AccessReaderComponent>.op_Implicit(ent), new AccessReaderConfigurationChangedEvent(), false);
  }

  public void RemoveAccess(
    Entity<AccessReaderComponent> ent,
    ProtoId<AccessLevelPrototype> access,
    bool dirty = true)
  {
    Entity<AccessReaderComponent> ent1 = ent;
    HashSet<ProtoId<AccessLevelPrototype>> access1 = new HashSet<ProtoId<AccessLevelPrototype>>();
    access1.Add(access);
    int num = dirty ? 1 : 0;
    this.RemoveAccess(ent1, access1, num != 0);
  }

  public void ClearAccessKeys(Entity<AccessReaderComponent> ent)
  {
    ent.Comp.AccessKeys.Clear();
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
  }

  public void SetAccessKeys(Entity<AccessReaderComponent> ent, HashSet<StationRecordKey> keys)
  {
    ent.Comp.AccessKeys.Clear();
    foreach (StationRecordKey key in keys)
      ent.Comp.AccessKeys.Add(key);
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
  }

  public void AddAccessKey(Entity<AccessReaderComponent> ent, StationRecordKey key)
  {
    ent.Comp.AccessKeys.Add(key);
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
  }

  public void RemoveAccessKey(Entity<AccessReaderComponent> ent, StationRecordKey key)
  {
    ent.Comp.AccessKeys.Remove(key);
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
  }

  public void ClearDenyTags(Entity<AccessReaderComponent> ent)
  {
    ent.Comp.DenyTags.Clear();
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
  }

  public void SetDenyTags(
    Entity<AccessReaderComponent> ent,
    HashSet<ProtoId<AccessLevelPrototype>> tags)
  {
    ent.Comp.DenyTags.Clear();
    foreach (ProtoId<AccessLevelPrototype> tag in tags)
      ent.Comp.DenyTags.Add(tag);
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
  }

  public void AddDenyTag(Entity<AccessReaderComponent> ent, ProtoId<AccessLevelPrototype> tag)
  {
    ent.Comp.DenyTags.Add(tag);
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
  }

  public void RemoveDenyTag(Entity<AccessReaderComponent> ent, ProtoId<AccessLevelPrototype> tag)
  {
    ent.Comp.DenyTags.Remove(tag);
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
  }

  public void SetActive(Entity<AccessReaderComponent> ent, bool enabled)
  {
    ent.Comp.Enabled = enabled;
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
  }

  public void SetLoggingActive(Entity<AccessReaderComponent> ent, bool enabled)
  {
    ent.Comp.LoggingDisabled = !enabled;
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
  }

  public bool FindAccessItemsInventory(EntityUid uid, out HashSet<EntityUid> items)
  {
    items = new HashSet<EntityUid>(this._handsSystem.EnumerateHeld(Entity<HandsComponent>.op_Implicit(uid)));
    EntityUid? entityUid;
    if (this._inventorySystem.TryGetSlotEntity(uid, "id", out entityUid))
      items.Add(entityUid.Value);
    return items.Any<EntityUid>();
  }

  private bool FindAccessTagsItem(EntityUid uid, out HashSet<ProtoId<AccessLevelPrototype>> tags)
  {
    tags = new HashSet<ProtoId<AccessLevelPrototype>>();
    GetAccessTagsEvent getAccessTagsEvent = new GetAccessTagsEvent(tags, this._prototype);
    this.RaiseLocalEvent<GetAccessTagsEvent>(uid, ref getAccessTagsEvent, false);
    return tags.Count != 0;
  }

  private bool FindStationRecordKeyItem(EntityUid uid, [NotNullWhen(true)] out StationRecordKey? key)
  {
    StationRecordKeyStorageComponent storageComponent1;
    if (this.TryComp<StationRecordKeyStorageComponent>(uid, ref storageComponent1) && storageComponent1.Key.HasValue)
    {
      key = storageComponent1.Key;
      return true;
    }
    PdaComponent pdaComponent;
    if (this.TryComp<PdaComponent>(uid, ref pdaComponent))
    {
      EntityUid? containedId = pdaComponent.ContainedId;
      if (containedId.HasValue)
      {
        EntityUid valueOrDefault = containedId.GetValueOrDefault();
        StationRecordKeyStorageComponent storageComponent2;
        if (((EntityUid) ref valueOrDefault).Valid && this.TryComp<StationRecordKeyStorageComponent>(valueOrDefault, ref storageComponent2) && storageComponent2.Key.HasValue)
        {
          key = storageComponent2.Key;
          return true;
        }
      }
    }
    key = new StationRecordKey?();
    return false;
  }

  public void LogAccess(Entity<AccessReaderComponent> ent, EntityUid accessor)
  {
    if (this.IsPaused(new EntityUid?(Entity<AccessReaderComponent>.op_Implicit(ent)), (MetaDataComponent) null) || ent.Comp.LoggingDisabled)
      return;
    string str = (string) null;
    NameIdentifierComponent identifierComponent;
    if (this.TryComp<NameIdentifierComponent>(accessor, ref identifierComponent))
      str = identifierComponent.FullIdentifier;
    TryGetIdentityShortInfoEvent identityShortInfoEvent = new TryGetIdentityShortInfoEvent(new EntityUid?(Entity<AccessReaderComponent>.op_Implicit(ent)), accessor, true);
    this.RaiseLocalEvent<TryGetIdentityShortInfoEvent>(identityShortInfoEvent);
    if (identityShortInfoEvent.Title != null)
      str = identityShortInfoEvent.Title;
    this.LogAccess(ent, str ?? this.Loc.GetString("access-reader-unknown-id"));
  }

  public void LogAccess(
    Entity<AccessReaderComponent> ent,
    string name,
    TimeSpan? accessTime = null,
    bool force = false)
  {
    if (!force)
    {
      if (this.IsPaused(new EntityUid?(Entity<AccessReaderComponent>.op_Implicit(ent)), (MetaDataComponent) null) || ent.Comp.LoggingDisabled)
        return;
      if (ent.Comp.AccessLog.Count >= ent.Comp.AccessLogLimit)
        ent.Comp.AccessLog.Dequeue();
    }
    TimeSpan AccessTime = accessTime ?? this._gameTiming.CurTime.Subtract(this._gameTicker.RoundStartTimeSpan);
    ent.Comp.AccessLog.Enqueue(new AccessRecord(AccessTime, name));
    this.Dirty<AccessReaderComponent>(ent, (MetaDataComponent) null);
  }
}
