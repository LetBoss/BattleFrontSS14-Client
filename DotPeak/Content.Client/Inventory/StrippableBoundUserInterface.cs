// Decompiled with JetBrains decompiler
// Type: Content.Client.Inventory.StrippableBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Examine;
using Content.Client.Hands.Systems;
using Content.Client.Strip;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Hands.Controls;
using Content.Client.Verbs.UI;
using Content.Shared.Cuffs;
using Content.Shared.Cuffs.Components;
using Content.Shared.Ensnaring.Components;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Inventory;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Strip.Components;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Inventory;

public sealed class StrippableBoundUserInterface : BoundUserInterface
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IUserInterfaceManager _ui;
  private readonly ExamineSystem _examine;
  private readonly HandsSystem _hands;
  private readonly InventorySystem _inv;
  private readonly SharedCuffableSystem _cuffable;
  private readonly StrippableSystem _strippable;
  [Robust.Shared.ViewVariables.ViewVariables]
  private const int ButtonSeparation = 4;
  [Robust.Shared.ViewVariables.ViewVariables]
  public const string HiddenPocketEntityId = "StrippingHiddenEntity";
  [Robust.Shared.ViewVariables.ViewVariables]
  private StrippingMenu? _strippingMenu;
  [Robust.Shared.ViewVariables.ViewVariables]
  private readonly EntityUid _virtualHiddenEntity;
  [Robust.Shared.ViewVariables.ViewVariables]
  private int _handCount;
  [Robust.Shared.ViewVariables.ViewVariables]
  private Vector2i _inventoryDimensions;

  public StrippableBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._examine = this.EntMan.System<ExamineSystem>();
    this._hands = this.EntMan.System<HandsSystem>();
    this._inv = this.EntMan.System<InventorySystem>();
    this._cuffable = this.EntMan.System<SharedCuffableSystem>();
    this._strippable = this.EntMan.System<StrippableSystem>();
    this._virtualHiddenEntity = this.EntMan.SpawnEntity("StrippingHiddenEntity", MapCoordinates.Nullspace, (ComponentRegistry) null);
  }

  protected virtual void Open()
  {
    base.Open();
    this._strippingMenu = BoundUserInterfaceExt.CreateWindowCenteredLeft<StrippingMenu>((BoundUserInterface) this);
    this._strippingMenu.OnDirty += new Action(this.UpdateMenu);
    this._strippingMenu.Title = Loc.GetString("strippable-bound-user-interface-stripping-menu-title", new (string, object)[1]
    {
      ("ownerName", (object) Identity.Name(this.Owner, this.EntMan))
    });
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!disposing)
      return;
    if (this._strippingMenu != null)
      this._strippingMenu.OnDirty -= new Action(this.UpdateMenu);
    this.EntMan.DeleteEntity(new EntityUid?(this._virtualHiddenEntity));
    base.Dispose(disposing);
  }

  public void DirtyMenu()
  {
    if (this._strippingMenu == null)
      return;
    this._strippingMenu.Dirty = true;
  }

  public void UpdateMenu()
  {
    if (this._strippingMenu == null)
      return;
    this._strippingMenu.ClearButtons();
    this._handCount = 0;
    this._inventoryDimensions = Vector2i.Zero;
    InventoryComponent inv;
    if (this.EntMan.TryGetComponent<InventoryComponent>(this.Owner, ref inv))
    {
      foreach (SlotDefinition slot in inv.Slots)
        this.AddInventoryButton(this.Owner, slot.Name, inv);
    }
    HandsComponent handsComponent;
    if (this.EntMan.TryGetComponent<HandsComponent>(this.Owner, ref handsComponent) && handsComponent.CanBeStripped)
    {
      foreach ((string key, Hand hand5) in handsComponent.Hands)
      {
        string handId = key;
        Hand hand2 = hand5;
        if (hand2.Location == HandLocation.Right)
          this.AddHandButton(Entity<HandsComponent>.op_Implicit((this.Owner, handsComponent)), handId, hand2);
      }
      foreach ((key, hand5) in handsComponent.Hands)
      {
        string handId = key;
        Hand hand4 = hand5;
        if (hand4.Location == HandLocation.Middle)
          this.AddHandButton(Entity<HandsComponent>.op_Implicit((this.Owner, handsComponent)), handId, hand4);
      }
      foreach ((key, hand5) in handsComponent.Hands)
      {
        string handId = key;
        Hand hand6 = hand5;
        if (hand6.Location == HandLocation.Left)
          this.AddHandButton(Entity<HandsComponent>.op_Implicit((this.Owner, handsComponent)), handId, hand6);
      }
    }
    EnsnareableComponent ensnareableComponent;
    if (this.EntMan.TryGetComponent<EnsnareableComponent>(this.Owner, ref ensnareableComponent) && ensnareableComponent.IsEnsnared)
    {
      Button button1 = new Button();
      button1.Text = Loc.GetString("strippable-bound-user-interface-stripping-menu-ensnare-button");
      ((Control) button1).StyleClasses.Add("OpenRight");
      Button button2 = button1;
      ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new StrippingEnsnareButtonPressed()));
      ((Control) this._strippingMenu.SnareContainer).AddChild((Control) button2);
    }
    int x = Math.Max(200, Math.Max(this._handCount, this._inventoryDimensions.X + 1) * (SlotControl.DefaultButtonSize + 4) + 20);
    int y = Math.Max(200, (this._inventoryDimensions.Y + (this._handCount > 0 ? 2 : 1)) * (SlotControl.DefaultButtonSize + 4) + 53);
    if (ensnareableComponent != null && ensnareableComponent.IsEnsnared)
      y += 20;
    ((Control) this._strippingMenu).SetSize = new Vector2((float) x, (float) y);
  }

  private void AddHandButton(Entity<HandsComponent> ent, string handId, Hand hand)
  {
    HandButton button = new HandButton(handId, hand.Location);
    button.Pressed += new Action<GUIBoundKeyEventArgs, SlotControl>(this.SlotPressed);
    EntityUid? heldItem = this._hands.GetHeldItem(ent.AsNullable(), handId);
    VirtualItemComponent virtualItemComponent;
    if (this.EntMan.TryGetComponent<VirtualItemComponent>(heldItem, ref virtualItemComponent))
    {
      button.Blocked = true;
      CuffableComponent component;
      if (this.EntMan.TryGetComponent<CuffableComponent>(this.Owner, ref component) && this._cuffable.GetAllCuffs(component).Contains<EntityUid>(virtualItemComponent.BlockingEntity))
        ((Control) button.BlockedRect).MouseFilter = (Control.MouseFilterMode) 2;
    }
    this.UpdateEntityIcon((SlotControl) button, heldItem);
    ((Control) this._strippingMenu.HandsContainer).AddChild((Control) button);
    LayoutContainer.SetPosition((Control) button, Vector2i.op_Implicit(Vector2i.op_Multiply(new Vector2i(this._handCount, 0), SlotControl.DefaultButtonSize + 4)));
    ++this._handCount;
  }

  private void SlotPressed(GUIBoundKeyEventArgs ev, SlotControl slot)
  {
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) ev).Function, EngineKeyFunctions.Use))
    {
      this.SendPredictedMessage((BoundUserInterfaceMessage) new StrippingSlotButtonPressed(slot.SlotName, slot is HandButton));
    }
    else
    {
      if (!slot.Entity.HasValue)
        return;
      if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) ev).Function, ContentKeyFunctions.ExamineEntity))
      {
        ExamineSystem examine = this._examine;
        EntityUid? nullable = slot.Entity;
        EntityUid entity = nullable.Value;
        nullable = new EntityUid?();
        EntityUid? userOverride = nullable;
        examine.DoExamine(entity, userOverride: userOverride);
        ((BoundKeyEventArgs) ev).Handle();
      }
      else
      {
        if (!BoundKeyFunction.op_Equality(((BoundKeyEventArgs) ev).Function, EngineKeyFunctions.UseSecondary))
          return;
        this._ui.GetUIController<VerbMenuUIController>().OpenVerbMenu(slot.Entity.Value);
        ((BoundKeyEventArgs) ev).Handle();
      }
    }
  }

  private void AddInventoryButton(EntityUid invUid, string slotId, InventoryComponent inv)
  {
    ContainerSlot containerSlot;
    SlotDefinition slotDefinition;
    if (!this._inv.TryGetSlotContainer(invUid, slotId, out containerSlot, out slotDefinition, inv))
      return;
    EntityUid? entity = containerSlot.ContainedEntity;
    if (entity.HasValue && this._strippable.IsStripHidden(slotDefinition, ((ISharedPlayerManager) this._player).LocalEntity))
      entity = new EntityUid?(this._virtualHiddenEntity);
    SlotButton button = new SlotButton(new ClientInventorySystem.SlotData(slotDefinition, containerSlot));
    button.Pressed += new Action<GUIBoundKeyEventArgs, SlotControl>(this.SlotPressed);
    ((Control) this._strippingMenu.InventoryContainer).AddChild((Control) button);
    this.UpdateEntityIcon((SlotControl) button, entity);
    LayoutContainer.SetPosition((Control) button, Vector2i.op_Implicit(Vector2i.op_Multiply(slotDefinition.StrippingWindowPos, SlotControl.DefaultButtonSize + 4)));
    if (slotDefinition.StrippingWindowPos.X > this._inventoryDimensions.X)
      this._inventoryDimensions = new Vector2i(slotDefinition.StrippingWindowPos.X, this._inventoryDimensions.Y);
    if (slotDefinition.StrippingWindowPos.Y <= this._inventoryDimensions.Y)
      return;
    this._inventoryDimensions = new Vector2i(this._inventoryDimensions.X, slotDefinition.StrippingWindowPos.Y);
  }

  private void UpdateEntityIcon(SlotControl button, EntityUid? entity)
  {
    button.ClearHover();
    ((Control) button.StorageButton).Visible = false;
    if (!entity.HasValue)
    {
      button.SetEntity(new EntityUid?());
    }
    else
    {
      VirtualItemComponent virtualItemComponent;
      EntityUid? ent;
      if (this.EntMan.TryGetComponent<VirtualItemComponent>(entity, ref virtualItemComponent))
      {
        ent = this.EntMan.HasComponent<SpriteComponent>(virtualItemComponent.BlockingEntity) ? new EntityUid?(virtualItemComponent.BlockingEntity) : new EntityUid?();
      }
      else
      {
        if (!this.EntMan.HasComponent<SpriteComponent>(entity))
          return;
        ent = entity;
      }
      button.SetEntity(ent);
    }
  }
}
