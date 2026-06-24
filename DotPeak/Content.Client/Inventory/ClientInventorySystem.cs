// Decompiled with JetBrains decompiler
// Type: Content.Client.Inventory.ClientInventorySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Clothing;
using Content.Client.Examine;
using Content.Client.Verbs.UI;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Storage;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Inventory;

public sealed class ClientInventorySystem : InventorySystem
{
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IUserInterfaceManager _ui;
  [Dependency]
  private ClientClothingSystem _clothingVisualsSystem;
  [Dependency]
  private ExamineSystem _examine;
  public Action<ClientInventorySystem.SlotData>? EntitySlotUpdate;
  public Action<ClientInventorySystem.SlotData>? OnSlotAdded;
  public Action<ClientInventorySystem.SlotData>? OnSlotRemoved;
  public Action<EntityUid, InventorySlotsComponent>? OnLinkInventorySlots;
  public Action? OnUnlinkInventory;
  public Action<ClientInventorySystem.SlotSpriteUpdate>? OnSpriteUpdate;
  private readonly Queue<(InventorySlotsComponent comp, EntityEventArgs args)> _equipEventsQueue = new Queue<(InventorySlotsComponent, EntityEventArgs)>();

  public override void Initialize()
  {
    this.UpdatesOutsidePrediction = true;
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InventorySlotsComponent, LocalPlayerAttachedEvent>(new ComponentEventHandler<InventorySlotsComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnPlayerAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InventorySlotsComponent, LocalPlayerDetachedEvent>(new ComponentEventHandler<InventorySlotsComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnPlayerDetached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InventoryComponent, ComponentShutdown>(new ComponentEventHandler<InventoryComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InventorySlotsComponent, DidEquipEvent>(new ComponentEventHandler<InventorySlotsComponent, DidEquipEvent>((object) this, __methodptr(\u003CInitialize\u003Eb__11_0)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InventorySlotsComponent, DidUnequipEvent>(new ComponentEventHandler<InventorySlotsComponent, DidUnequipEvent>((object) this, __methodptr(\u003CInitialize\u003Eb__11_1)), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    (InventorySlotsComponent comp, EntityEventArgs args) result;
    while (this._equipEventsQueue.TryDequeue(out result))
    {
      (InventorySlotsComponent inventorySlotsComponent, EntityEventArgs args1) = result;
      switch (args1)
      {
        case DidEquipEvent args2:
          this.OnDidEquip(inventorySlotsComponent, args2);
          continue;
        case DidUnequipEvent args3:
          this.OnDidUnequip(inventorySlotsComponent, args3);
          continue;
        default:
          throw new InvalidOperationException($"Received queued event of unknown type: {args1.GetType()}");
      }
    }
  }

  private void OnDidUnequip(InventorySlotsComponent component, DidUnequipEvent args)
  {
    this.UpdateSlot(args.Equipee, component, args.Slot);
    EntityUid equipee = args.Equipee;
    EntityUid? nullable = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((nullable.HasValue ? (EntityUid.op_Inequality(equipee, nullable.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    ClientInventorySystem.SlotSpriteUpdate slotSpriteUpdate;
    ref ClientInventorySystem.SlotSpriteUpdate local = ref slotSpriteUpdate;
    nullable = new EntityUid?();
    EntityUid? Entity = nullable;
    string slotGroup = args.SlotGroup;
    string slot = args.Slot;
    local = new ClientInventorySystem.SlotSpriteUpdate(Entity, slotGroup, slot, false);
    Action<ClientInventorySystem.SlotSpriteUpdate> onSpriteUpdate = this.OnSpriteUpdate;
    if (onSpriteUpdate == null)
      return;
    onSpriteUpdate(slotSpriteUpdate);
  }

  private void OnDidEquip(InventorySlotsComponent component, DidEquipEvent args)
  {
    this.UpdateSlot(args.Equipee, component, args.Slot);
    EntityUid equipee = args.Equipee;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(equipee, localEntity.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    ClientInventorySystem.SlotSpriteUpdate slotSpriteUpdate = new ClientInventorySystem.SlotSpriteUpdate(new EntityUid?(args.Equipment), args.SlotGroup, args.Slot, this.HasComp<StorageComponent>(args.Equipment));
    Action<ClientInventorySystem.SlotSpriteUpdate> onSpriteUpdate = this.OnSpriteUpdate;
    if (onSpriteUpdate == null)
      return;
    onSpriteUpdate(slotSpriteUpdate);
  }

  private void OnShutdown(EntityUid uid, InventoryComponent component, ComponentShutdown args)
  {
    EntityUid entityUid = uid;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
      return;
    Action onUnlinkInventory = this.OnUnlinkInventory;
    if (onUnlinkInventory == null)
      return;
    onUnlinkInventory();
  }

  private void OnPlayerDetached(
    EntityUid uid,
    InventorySlotsComponent component,
    LocalPlayerDetachedEvent args)
  {
    Action onUnlinkInventory = this.OnUnlinkInventory;
    if (onUnlinkInventory == null)
      return;
    onUnlinkInventory();
  }

  private void OnPlayerAttached(
    EntityUid uid,
    InventorySlotsComponent component,
    LocalPlayerAttachedEvent args)
  {
    SlotDefinition[] slotDefinitions;
    if (this.TryGetSlots(uid, out slotDefinitions))
    {
      foreach (SlotDefinition slotDef in slotDefinitions)
      {
        ContainerSlot containerSlot;
        if (this.TryGetSlotContainer(uid, slotDef.Name, out containerSlot, out SlotDefinition _))
        {
          ClientInventorySystem.SlotData slotData;
          if (!component.SlotData.TryGetValue(slotDef.Name, out slotData))
          {
            slotData = new ClientInventorySystem.SlotData(slotDef);
            component.SlotData[slotDef.Name] = slotData;
          }
          slotData.Container = containerSlot;
        }
      }
    }
    Action<EntityUid, InventorySlotsComponent> linkInventorySlots = this.OnLinkInventorySlots;
    if (linkInventorySlots == null)
      return;
    linkInventorySlots(uid, component);
  }

  public override void Shutdown()
  {
    CommandBinds.Unregister<ClientInventorySystem>();
    base.Shutdown();
  }

  protected override void OnInit(EntityUid uid, InventoryComponent component, ComponentInit args)
  {
    base.OnInit(uid, component, args);
    this._clothingVisualsSystem.InitClothing(uid, component);
    InventorySlotsComponent component1;
    if (!this.TryComp<InventorySlotsComponent>(uid, ref component1))
      return;
    foreach (SlotDefinition slot in component.Slots)
      this.TryAddSlotDef(uid, component1, slot);
  }

  public void ReloadInventory(InventorySlotsComponent? component = null)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue || !this.Resolve<InventorySlotsComponent>(localEntity.Value, ref component, false))
      return;
    Action onUnlinkInventory = this.OnUnlinkInventory;
    if (onUnlinkInventory != null)
      onUnlinkInventory();
    Action<EntityUid, InventorySlotsComponent> linkInventorySlots = this.OnLinkInventorySlots;
    if (linkInventorySlots == null)
      return;
    linkInventorySlots(localEntity.Value, component);
  }

  public void SetSlotHighlight(
    EntityUid owner,
    InventorySlotsComponent component,
    string slotName,
    bool state)
  {
    ClientInventorySystem.SlotData oldData = component.SlotData[slotName];
    ClientInventorySystem.SlotData slotData = component.SlotData[slotName] = new ClientInventorySystem.SlotData(oldData, state);
    EntityUid entityUid = owner;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
      return;
    Action<ClientInventorySystem.SlotData> entitySlotUpdate = this.EntitySlotUpdate;
    if (entitySlotUpdate == null)
      return;
    entitySlotUpdate(slotData);
  }

  public void UpdateSlot(
    EntityUid owner,
    InventorySlotsComponent component,
    string slotName,
    bool? blocked = null,
    bool? highlight = null)
  {
    ClientInventorySystem.SlotData slotData1 = component.SlotData[slotName];
    bool highlighted = slotData1.Highlighted;
    bool blocked1 = slotData1.Blocked;
    if (blocked.HasValue)
      blocked1 = blocked.Value;
    if (highlight.HasValue)
      highlighted = highlight.Value;
    ClientInventorySystem.SlotData slotData2 = component.SlotData[slotName] = new ClientInventorySystem.SlotData(component.SlotData[slotName], highlighted, blocked1);
    EntityUid entityUid = owner;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
      return;
    Action<ClientInventorySystem.SlotData> entitySlotUpdate = this.EntitySlotUpdate;
    if (entitySlotUpdate == null)
      return;
    entitySlotUpdate(slotData2);
  }

  public bool TryAddSlotDef(
    EntityUid owner,
    InventorySlotsComponent component,
    SlotDefinition newSlotDef)
  {
    ContainerSlot containerSlot;
    this.TryGetSlotContainer(owner, newSlotDef.Name, out containerSlot, out SlotDefinition _);
    ClientInventorySystem.SlotData slotData = new ClientInventorySystem.SlotData(newSlotDef, containerSlot);
    if (!component.SlotData.TryAdd(newSlotDef.Name, slotData))
      return false;
    EntityUid entityUid = owner;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 0) != 0)
    {
      Action<ClientInventorySystem.SlotData> onSlotAdded = this.OnSlotAdded;
      if (onSlotAdded != null)
        onSlotAdded(slotData);
    }
    return true;
  }

  public void UIInventoryActivate(string slot)
  {
    this.RaisePredictiveEvent<UseSlotNetworkMessage>(new UseSlotNetworkMessage(slot));
  }

  public void UIInventoryStorageActivate(string slot)
  {
    this.RaisePredictiveEvent<OpenSlotStorageNetworkMessage>(new OpenSlotStorageNetworkMessage(slot));
  }

  public void UIInventoryExamine(string slot, EntityUid uid)
  {
    EntityUid? entityUid;
    if (!this.TryGetSlotEntity(uid, slot, out entityUid))
      return;
    this._examine.DoExamine(entityUid.Value);
  }

  public void UIInventoryOpenContextMenu(string slot, EntityUid uid)
  {
    EntityUid? entityUid;
    if (!this.TryGetSlotEntity(uid, slot, out entityUid))
      return;
    this._ui.GetUIController<VerbMenuUIController>().OpenVerbMenu(entityUid.Value);
  }

  public void UIInventoryActivateItem(string slot, EntityUid uid)
  {
    EntityUid? entityUid;
    if (!this.TryGetSlotEntity(uid, slot, out entityUid))
      return;
    this.RaisePredictiveEvent<InteractInventorySlotEvent>(new InteractInventorySlotEvent(this.GetNetEntity(entityUid.Value, (MetaDataComponent) null)));
  }

  public void UIInventoryAltActivateItem(string slot, EntityUid uid)
  {
    EntityUid? entityUid;
    if (!this.TryGetSlotEntity(uid, slot, out entityUid))
      return;
    this.RaisePredictiveEvent<InteractInventorySlotEvent>(new InteractInventorySlotEvent(this.GetNetEntity(entityUid.Value, (MetaDataComponent) null), true));
  }

  protected override void UpdateInventoryTemplate(Entity<InventoryComponent> ent)
  {
    base.UpdateInventoryTemplate(ent);
    InventorySlotsComponent component;
    if (!this.TryComp<InventorySlotsComponent>(Entity<InventoryComponent>.op_Implicit(ent), ref component))
      return;
    HashSet<string> stringSet = new HashSet<string>(ent.Comp.Slots.Length);
    foreach (SlotDefinition slot in ent.Comp.Slots)
    {
      stringSet.Add(slot.Name);
      ContainerSlot containerSlot;
      this.TryGetSlotContainer(ent.Owner, slot.Name, out containerSlot, out SlotDefinition _);
      ClientInventorySystem.SlotData slotData;
      if (component.SlotData.TryGetValue(slot.Name, out slotData))
      {
        slotData.SlotDef = slot;
        slotData.Container = containerSlot;
      }
      else
        this.TryAddSlotDef(ent.Owner, component, slot);
    }
    List<string> stringList = new List<string>();
    foreach ((string key, ClientInventorySystem.SlotData _) in component.SlotData)
    {
      if (!stringSet.Contains(key))
        stringList.Add(key);
    }
    foreach (string key in stringList)
    {
      ClientInventorySystem.SlotData slotData;
      if (component.SlotData.TryGetValue(key, out slotData) && component.SlotData.Remove(key))
      {
        EntityUid owner = ent.Owner;
        EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
        if ((localEntity.HasValue ? (EntityUid.op_Equality(owner, localEntity.GetValueOrDefault()) ? 1 : 0) : 0) != 0)
        {
          Action<ClientInventorySystem.SlotData> onSlotRemoved = this.OnSlotRemoved;
          if (onSlotRemoved != null)
            onSlotRemoved(slotData);
        }
      }
    }
    EntityUid owner1 = ent.Owner;
    EntityUid? localEntity1 = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity1.HasValue ? (EntityUid.op_Equality(owner1, localEntity1.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
      return;
    Action<EntityUid, InventorySlotsComponent> linkInventorySlots = this.OnLinkInventorySlots;
    if (linkInventorySlots == null)
      return;
    linkInventorySlots(ent.Owner, component);
  }

  public sealed class SlotData
  {
    public SlotDefinition SlotDef;
    public bool Blocked;
    public bool Highlighted;
    [Robust.Shared.ViewVariables.ViewVariables]
    public ContainerSlot? Container;

    public EntityUid? HeldEntity => this.Container?.ContainedEntity;

    public bool HasSlotGroup => this.SlotDef.SlotGroup != "Default";

    public Vector2i ButtonOffset => this.SlotDef.UIWindowPosition;

    public string SlotName => this.SlotDef.Name;

    public bool ShowInWindow => this.SlotDef.ShowInWindow;

    public string SlotGroup => this.SlotDef.SlotGroup;

    public string SlotDisplayName => this.SlotDef.DisplayName;

    public string TextureName => "Slots/" + this.SlotDef.TextureName;

    public string FullTextureName => this.SlotDef.FullTextureName;

    public SlotData(
      SlotDefinition slotDef,
      ContainerSlot? container = null,
      bool highlighted = false,
      bool blocked = false)
    {
      this.SlotDef = slotDef;
      this.Highlighted = highlighted;
      this.Blocked = blocked;
      this.Container = container;
    }

    public SlotData(ClientInventorySystem.SlotData oldData, bool highlighted = false, bool blocked = false)
    {
      this.SlotDef = oldData.SlotDef;
      this.Highlighted = highlighted;
      this.Container = oldData.Container;
      this.Blocked = blocked;
    }

    public static implicit operator ClientInventorySystem.SlotData(SlotDefinition s)
    {
      return new ClientInventorySystem.SlotData(s);
    }

    public static implicit operator SlotDefinition(ClientInventorySystem.SlotData s) => s.SlotDef;
  }

  public readonly record struct SlotSpriteUpdate(
    EntityUid? Entity,
    string Group,
    string Name,
    bool ShowStorage)
  ;
}
