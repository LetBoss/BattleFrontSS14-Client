// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Inventory.InventoryUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.UniformAccessories;
using Content.Client._RMC14.Webbing;
using Content.Client.Gameplay;
using Content.Client.Hands.Systems;
using Content.Client.Inventory;
using Content.Client.Storage.Systems;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Client.UserInterface.Systems.Inventory.Controls;
using Content.Client.UserInterface.Systems.Inventory.Widgets;
using Content.Client.UserInterface.Systems.Inventory.Windows;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Hands.Components;
using Content.Shared.Input;
using Content.Shared.Inventory;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Systems.Inventory;

public sealed class InventoryUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>,
  IOnSystemChanged<ClientInventorySystem>,
  IOnSystemLoaded<ClientInventorySystem>,
  IOnSystemUnloaded<ClientInventorySystem>,
  IOnSystemChanged<HandsSystem>,
  IOnSystemLoaded<HandsSystem>,
  IOnSystemUnloaded<HandsSystem>,
  IOnSystemChanged<WebbingSystem>,
  IOnSystemLoaded<WebbingSystem>,
  IOnSystemUnloaded<WebbingSystem>,
  IOnSystemChanged<UniformAccessorySystem>,
  IOnSystemLoaded<UniformAccessorySystem>,
  IOnSystemUnloaded<UniformAccessorySystem>
{
  [Dependency]
  private IEntityManager _entities;
  [UISystemDependency]
  private readonly ClientInventorySystem _inventorySystem;
  [UISystemDependency]
  private readonly HandsSystem _handsSystem;
  [UISystemDependency]
  private readonly ContainerSystem _container;
  [UISystemDependency]
  private readonly SpriteSystem _sprite;
  private EntityUid? _playerUid;
  private InventorySlotsComponent? _playerInventory;
  private readonly Dictionary<string, ItemSlotButtonContainer> _slotGroups = new Dictionary<string, ItemSlotButtonContainer>();
  private StrippingWindow? _strippingWindow;
  private ItemSlotButtonContainer? _inventoryHotbar;
  private SlotButton? _inventoryButton;
  private SlotControl? _lastHovered;
  private const string PubgInventoryTemplateId = "pubgHuman";

  public virtual void Initialize()
  {
    base.Initialize();
    this.UIManager.GetUIController<GameplayStateLoadController>().OnScreenLoad += new Action(this.OnScreenLoad);
  }

  private void OnScreenLoad()
  {
    if (this.UIManager.ActiveScreen == null)
      return;
    InventoryGui activeUiWidgetOrNull = this.UIManager.GetActiveUIWidgetOrNull<InventoryGui>();
    if (activeUiWidgetOrNull == null)
      return;
    this.RegisterInventoryButton(activeUiWidgetOrNull.InventoryButton);
  }

  public void OnStateEntered(GameplayState state)
  {
    this._strippingWindow = this.UIManager.CreateWindow<StrippingWindow>();
    LayoutContainer.SetAnchorPreset((Control) this._strippingWindow, (LayoutContainer.LayoutPreset) 8, false);
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(ContentKeyFunctions.OpenInventoryMenu, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003COnStateEntered\u003Eb__15_0)), (StateInputCmdDelegate) null, true, true)).Register<ClientInventorySystem>();
  }

  public void OnStateExited(GameplayState state)
  {
    if (this._strippingWindow != null)
    {
      if (!((Control) this._strippingWindow).Disposed)
        ((Control) this._strippingWindow).Orphan();
      this._strippingWindow = (StrippingWindow) null;
    }
    if (this._inventoryHotbar != null)
      ((Control) this._inventoryHotbar).Visible = false;
    CommandBinds.Unregister<ClientInventorySystem>();
  }

  private SlotButton CreateSlotButton(ClientInventorySystem.SlotData data)
  {
    SlotButton slotButton = new SlotButton(data);
    slotButton.Pressed += new Action<GUIBoundKeyEventArgs, SlotControl>(this.ItemPressed);
    slotButton.StoragePressed += new Action<GUIBoundKeyEventArgs, SlotControl>(this.StoragePressed);
    slotButton.Hover += new Action<GUIMouseHoverEventArgs, SlotControl>(this.SlotButtonHovered);
    return slotButton;
  }

  public void RegisterInventoryBarContainer(ItemSlotButtonContainer inventoryHotbar)
  {
    this._inventoryHotbar = inventoryHotbar;
  }

  public void RegisterInventoryButton(SlotButton? button)
  {
    if (this._inventoryButton != null)
      this._inventoryButton.Pressed -= new Action<GUIBoundKeyEventArgs, SlotControl>(this.InventoryButtonPressed);
    if (button == null)
      return;
    this._inventoryButton = button;
    this._inventoryButton.Pressed += new Action<GUIBoundKeyEventArgs, SlotControl>(this.InventoryButtonPressed);
  }

  private void InventoryButtonPressed(GUIBoundKeyEventArgs args, SlotControl control)
  {
    if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
      return;
    this.ToggleInventoryBar();
  }

  private void UpdateInventoryHotbar(InventorySlotsComponent? clientInv)
  {
    if (clientInv == null)
    {
      this._inventoryHotbar?.ClearButtons();
      if (this._inventoryButton == null)
        return;
      this._inventoryButton.Visible = false;
    }
    else
    {
      foreach ((string _, ClientInventorySystem.SlotData data) in clientInv.SlotData)
      {
        if (!data.ShowInWindow)
        {
          this.RemoveSlotFromOtherGroups(data.SlotName);
        }
        else
        {
          this.RemoveSlotFromOtherGroups(data.SlotName, data.SlotGroup);
          ItemSlotButtonContainer slotButtonContainer;
          if (this._slotGroups.TryGetValue(data.SlotGroup, out slotButtonContainer))
          {
            SlotControl button;
            if (!slotButtonContainer.TryGetButton(data.SlotName, out button))
            {
              button = (SlotControl) this.CreateSlotButton(data);
              slotButtonContainer.AddButton(button);
            }
            bool ShowStorage = this._entities.HasComponent<StorageComponent>(data.HeldEntity);
            this.SpriteUpdated(new ClientInventorySystem.SlotSpriteUpdate(data.HeldEntity, data.SlotGroup, data.SlotName, ShowStorage));
          }
        }
      }
      InventoryComponent inventoryComponent;
      ItemSlotButtonContainer slotButtonContainer1;
      if (this._playerUid.HasValue && this._entities.TryGetComponent<InventoryComponent>(this._playerUid.Value, ref inventoryComponent) && inventoryComponent.TemplateId == "pubgHuman" && this._slotGroups.TryGetValue("MainHotbar", out slotButtonContainer1))
        slotButtonContainer1.ReorderPubgMainHotbar();
      if (this._inventoryHotbar == null)
        return;
      List<KeyValuePair<string, ClientInventorySystem.SlotData>> list = clientInv.SlotData.Where<KeyValuePair<string, ClientInventorySystem.SlotData>>((Func<KeyValuePair<string, ClientInventorySystem.SlotData>, bool>) (p => !p.Value.HasSlotGroup)).ToList<KeyValuePair<string, ClientInventorySystem.SlotData>>();
      if (this._inventoryButton != null)
        this._inventoryButton.Visible = list.Count != 0;
      if (list.Count == 0)
        return;
      foreach (Control control in new List<Control>((IEnumerable<Control>) ((Control) this._inventoryHotbar).Children))
      {
        if (!(control is SlotControl))
          ((Control) this._inventoryHotbar).RemoveChild(control);
      }
      int maxWidth = list.Max<KeyValuePair<string, ClientInventorySystem.SlotData>>((Func<KeyValuePair<string, ClientInventorySystem.SlotData>, int>) (p => p.Value.ButtonOffset.X)) + 1;
      int num = list.Select<KeyValuePair<string, ClientInventorySystem.SlotData>, int>((Func<KeyValuePair<string, ClientInventorySystem.SlotData>, int>) (p => GetIndex(p.Value.ButtonOffset))).Max();
      this._inventoryHotbar.MaxColumns = new int?(maxWidth);
      this._inventoryHotbar.Columns = maxWidth;
      for (int index1 = 0; index1 <= num; ++index1)
      {
        int index = index1;
        KeyValuePair<string, ClientInventorySystem.SlotData>? nullable = Extensions.FirstOrNull<KeyValuePair<string, ClientInventorySystem.SlotData>>((IEnumerable<KeyValuePair<string, ClientInventorySystem.SlotData>>) list, (Func<KeyValuePair<string, ClientInventorySystem.SlotData>, bool>) (p => GetIndex(p.Value.ButtonOffset) == index));
        if (nullable.HasValue)
        {
          SlotControl button;
          if (this._inventoryHotbar.TryGetButton(nullable.GetValueOrDefault().Key, out button))
            button.SetPositionLast();
        }
        else
          ((Control) this._inventoryHotbar).AddChild(new Control()
          {
            MinSize = new Vector2(64f, 64f)
          });
      }
    }
    int maxWidth1;

    int GetIndex(Vector2i position) => position.Y * maxWidth1 + position.X;
  }

  private void UpdateStrippingWindow(InventorySlotsComponent? clientInv)
  {
    if (clientInv == null)
    {
      this._strippingWindow.InventoryButtons.ClearButtons();
    }
    else
    {
      foreach ((string _, ClientInventorySystem.SlotData data) in clientInv.SlotData)
      {
        if (data.ShowInWindow)
        {
          SlotControl button;
          if (!this._strippingWindow.InventoryButtons.TryGetButton(data.SlotName, out button))
          {
            button = (SlotControl) this.CreateSlotButton(data);
            this._strippingWindow.InventoryButtons.AddButton(button, data.ButtonOffset);
          }
          bool ShowStorage = this._entities.HasComponent<StorageComponent>(data.HeldEntity);
          this.SpriteUpdated(new ClientInventorySystem.SlotSpriteUpdate(data.HeldEntity, data.SlotGroup, data.SlotName, ShowStorage));
        }
      }
    }
  }

  public void ToggleStrippingMenu()
  {
    this.UpdateStrippingWindow(this._playerInventory);
    if (this._strippingWindow.IsOpen)
      this._strippingWindow.Close();
    else
      this._strippingWindow.Open();
  }

  public void ToggleInventoryBar()
  {
    if (this._inventoryHotbar == null)
    {
      this.Log.Warning("Tried to toggle inventory bar when none are assigned");
    }
    else
    {
      this.UpdateInventoryHotbar(this._playerInventory);
      ((Control) this._inventoryHotbar).Visible = !((Control) this._inventoryHotbar).Visible;
    }
  }

  public void OnSystemLoaded(ClientInventorySystem system)
  {
    this._inventorySystem.OnSlotAdded += new Action<ClientInventorySystem.SlotData>(this.AddSlot);
    this._inventorySystem.OnSlotRemoved += new Action<ClientInventorySystem.SlotData>(this.RemoveSlot);
    this._inventorySystem.OnLinkInventorySlots += new Action<EntityUid, InventorySlotsComponent>(this.LoadSlots);
    this._inventorySystem.OnUnlinkInventory += new Action(this.UnloadSlots);
    this._inventorySystem.OnSpriteUpdate += new Action<ClientInventorySystem.SlotSpriteUpdate>(this.SpriteUpdated);
  }

  public void OnSystemUnloaded(ClientInventorySystem system)
  {
    this._inventorySystem.OnSlotAdded -= new Action<ClientInventorySystem.SlotData>(this.AddSlot);
    this._inventorySystem.OnSlotRemoved -= new Action<ClientInventorySystem.SlotData>(this.RemoveSlot);
    this._inventorySystem.OnLinkInventorySlots -= new Action<EntityUid, InventorySlotsComponent>(this.LoadSlots);
    this._inventorySystem.OnUnlinkInventory -= new Action(this.UnloadSlots);
    this._inventorySystem.OnSpriteUpdate -= new Action<ClientInventorySystem.SlotSpriteUpdate>(this.SpriteUpdated);
  }

  private void ItemPressed(GUIBoundKeyEventArgs args, SlotControl control)
  {
    string slotName = control.SlotName;
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
    {
      this._inventorySystem.UIInventoryActivate(control.SlotName);
      ((BoundKeyEventArgs) args).Handle();
    }
    else
    {
      if (this._playerInventory == null || !this._playerUid.HasValue)
        return;
      if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ExamineEntity))
        this._inventorySystem.UIInventoryExamine(slotName, this._playerUid.Value);
      else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UseSecondary))
        this._inventorySystem.UIInventoryOpenContextMenu(slotName, this._playerUid.Value);
      else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ActivateItemInWorld))
      {
        this._inventorySystem.UIInventoryActivateItem(slotName, this._playerUid.Value);
      }
      else
      {
        if (!BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.AltActivateItemInWorld))
          return;
        this._inventorySystem.UIInventoryAltActivateItem(slotName, this._playerUid.Value);
      }
      ((BoundKeyEventArgs) args).Handle();
    }
  }

  private void StoragePressed(GUIBoundKeyEventArgs args, SlotControl control)
  {
    this._inventorySystem.UIInventoryStorageActivate(control.SlotName);
  }

  private void SlotButtonHovered(GUIMouseHoverEventArgs args, SlotControl control)
  {
    this.UpdateHover(control);
    this._lastHovered = control;
  }

  public void UpdateHover(SlotControl control)
  {
    EntityUid? playerUid = this._playerUid;
    EntityUid? nullable;
    SpriteComponent spriteComponent;
    ContainerSlot containerSlot;
    SlotDefinition slotDefinition;
    if (!control.MouseIsHovering || !playerUid.HasValue || !this._handsSystem.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(playerUid.Value), out nullable) || !this._entities.TryGetComponent<SpriteComponent>(nullable, ref spriteComponent) || !this._inventorySystem.TryGetSlotContainer(playerUid.Value, control.SlotName, out containerSlot, out slotDefinition))
    {
      control.ClearHover();
    }
    else
    {
      EntityUid entityUid = this._entities.SpawnEntity("hoverentity", MapCoordinates.Nullspace, (ComponentRegistry) null);
      SpriteComponent component = this._entities.GetComponent<SpriteComponent>(entityUid);
      string reason;
      bool flag = this._inventorySystem.CanEquip(playerUid.Value, nullable.Value, control.SlotName, out reason, slotDefinition) && ((SharedContainerSystem) this._container).CanInsert(nullable.Value, (BaseContainer) containerSlot, false, (TransformComponent) null);
      StorageComponent storageComp;
      if (!flag && this._entities.TryGetComponent<StorageComponent>(containerSlot.ContainedEntity, ref storageComp))
      {
        flag = this._entities.System<StorageSystem>().CanInsert(containerSlot.ContainedEntity.Value, nullable.Value, new EntityUid?(playerUid.Value), out reason, storageComp);
      }
      else
      {
        ItemSlotsComponent itemSlotsComponent;
        if (!flag && this._entities.TryGetComponent<ItemSlotsComponent>(containerSlot.ContainedEntity, ref itemSlotsComponent))
        {
          ItemSlotsSystem itemSlotsSystem = this._entities.System<ItemSlotsSystem>();
          foreach (ItemSlot slot in itemSlotsComponent.Slots.Values)
          {
            if (slot.InsertOnInteract && itemSlotsSystem.CanInsert(containerSlot.ContainedEntity.Value, nullable.Value, new EntityUid?(), slot))
            {
              flag = true;
              break;
            }
          }
        }
      }
      this._sprite.CopySprite(Entity<SpriteComponent>.op_Implicit((nullable.Value, spriteComponent)), Entity<SpriteComponent>.op_Implicit((entityUid, component)));
      this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((entityUid, component)), flag ? new Color((byte) 0, byte.MaxValue, (byte) 0, (byte) 127 /*0x7F*/) : new Color(byte.MaxValue, (byte) 0, (byte) 0, (byte) 127 /*0x7F*/));
      control.HoverSpriteView.SetEntity(new EntityUid?(entityUid));
    }
  }

  private void AddSlot(ClientInventorySystem.SlotData data)
  {
    this.RemoveSlotFromOtherGroups(data.SlotName, data.SlotGroup);
    ItemSlotButtonContainer slotButtonContainer;
    if (!this._slotGroups.TryGetValue(data.SlotGroup, out slotButtonContainer) || slotButtonContainer.TryGetButton(data.SlotName, out SlotControl _))
      return;
    SlotButton slotButton = this.CreateSlotButton(data);
    slotButtonContainer.AddButton((SlotControl) slotButton);
  }

  private void RemoveSlot(ClientInventorySystem.SlotData data)
  {
    foreach (ItemSlotUIContainer<SlotControl> itemSlotUiContainer in this._slotGroups.Values)
      itemSlotUiContainer.RemoveButton(data.SlotName);
  }

  public void ReloadSlots() => this._inventorySystem.ReloadInventory();

  private void LoadSlots(EntityUid clientUid, InventorySlotsComponent clientInv)
  {
    this.UnloadSlots();
    this._playerUid = new EntityUid?(clientUid);
    this._playerInventory = clientInv;
    foreach (ClientInventorySystem.SlotData data in clientInv.SlotData.Values)
    {
      this.AddSlot(data);
      if (this._inventoryButton != null)
        this._inventoryButton.Visible = true;
    }
    this.UpdateInventoryHotbar(this._playerInventory);
  }

  private void UnloadSlots()
  {
    if (this._inventoryButton != null)
      this._inventoryButton.Visible = false;
    this._playerUid = new EntityUid?();
    this._playerInventory = (InventorySlotsComponent) null;
    foreach (ItemSlotUIContainer<SlotControl> itemSlotUiContainer in this._slotGroups.Values)
      itemSlotUiContainer.ClearButtons();
    this.UpdateInventoryHotbar((InventorySlotsComponent) null);
  }

  private void SpriteUpdated(ClientInventorySystem.SlotSpriteUpdate update)
  {
    (EntityUid? nullable, string str1, string str2, bool ShowStorage) = update;
    SlotControl button1 = this._strippingWindow?.InventoryButtons.GetButton(update.Name);
    if (button1 != null)
    {
      button1.SetEntity(nullable);
      ((Control) button1.StorageButton).Visible = ShowStorage;
    }
    SlotControl button2 = this._slotGroups.GetValueOrDefault<string, ItemSlotButtonContainer>(str1)?.GetButton(str2);
    if (button2 == null)
      return;
    VirtualItemComponent virtualItemComponent;
    if (this._entities.TryGetComponent<VirtualItemComponent>(nullable, ref virtualItemComponent))
    {
      button2.SetEntity(new EntityUid?(virtualItemComponent.BlockingEntity));
      button2.Blocked = true;
    }
    else
    {
      button2.SetEntity(nullable);
      button2.Blocked = false;
      ((Control) button2.StorageButton).Visible = ShowStorage;
    }
  }

  public bool RegisterSlotGroupContainer(ItemSlotButtonContainer slotContainer)
  {
    return this._slotGroups.TryAdd(slotContainer.SlotGroup, slotContainer);
  }

  public void RemoveSlotGroup(string slotGroupName) => this._slotGroups.Remove(slotGroupName);

  private void RemoveSlotFromOtherGroups(string slotName, string? keepGroup = null)
  {
    foreach ((string key, ItemSlotButtonContainer slotButtonContainer) in this._slotGroups)
    {
      string str = keepGroup;
      if (!(key == str))
        slotButtonContainer.RemoveButton(slotName);
    }
  }

  public void OnSystemLoaded(HandsSystem system)
  {
    this._handsSystem.OnPlayerItemAdded += new Action<string, EntityUid>(this.OnItemAdded);
    this._handsSystem.OnPlayerItemRemoved += new Action<string, EntityUid>(this.OnItemRemoved);
    this._handsSystem.OnPlayerSetActiveHand += new Action<string>(this.SetActiveHand);
  }

  public void OnSystemUnloaded(HandsSystem system)
  {
    this._handsSystem.OnPlayerItemAdded -= new Action<string, EntityUid>(this.OnItemAdded);
    this._handsSystem.OnPlayerItemRemoved -= new Action<string, EntityUid>(this.OnItemRemoved);
    this._handsSystem.OnPlayerSetActiveHand -= new Action<string>(this.SetActiveHand);
  }

  private void OnItemAdded(string name, EntityUid entity)
  {
    if (this._lastHovered == null)
      return;
    this.UpdateHover(this._lastHovered);
  }

  private void OnItemRemoved(string name, EntityUid entity)
  {
    if (this._lastHovered == null)
      return;
    this.UpdateHover(this._lastHovered);
  }

  private void SetActiveHand(string? handName)
  {
    if (this._lastHovered == null)
      return;
    this.UpdateHover(this._lastHovered);
  }

  public void OnSystemLoaded(WebbingSystem system)
  {
    system.PlayerWebbingUpdated += new Action(this.InventoryUpdated);
  }

  public void OnSystemUnloaded(WebbingSystem system)
  {
    system.PlayerWebbingUpdated -= new Action(this.InventoryUpdated);
  }

  public void OnSystemLoaded(UniformAccessorySystem system)
  {
    system.PlayerMedalUpdated += new Action(this.InventoryUpdated);
  }

  public void OnSystemUnloaded(UniformAccessorySystem system)
  {
    system.PlayerMedalUpdated -= new Action(this.InventoryUpdated);
  }

  private void InventoryUpdated() => this.UpdateInventoryHotbar(this._playerInventory);
}
