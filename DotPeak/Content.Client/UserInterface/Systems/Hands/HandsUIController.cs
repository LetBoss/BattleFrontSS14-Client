// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Hands.HandsUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Gameplay;
using Content.Client.Hands.Systems;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Hands.Controls;
using Content.Client.UserInterface.Systems.Hotbar.Widgets;
using Content.Shared.Hands.Components;
using Content.Shared.Input;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Timing;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.UserInterface.Systems.Hands;

public sealed class HandsUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnSystemChanged<HandsSystem>,
  IOnSystemLoaded<HandsSystem>,
  IOnSystemUnloaded<HandsSystem>
{
  [Dependency]
  private IEntityManager _entities;
  [Dependency]
  private IPlayerManager _player;
  [UISystemDependency]
  private readonly HandsSystem _handsSystem;
  [UISystemDependency]
  private readonly UseDelaySystem _useDelay;
  private readonly List<HandsContainer> _handsContainers = new List<HandsContainer>();
  private readonly Dictionary<string, int> _handContainerIndices = new Dictionary<string, int>();
  private readonly Dictionary<string, HandButton> _handLookup = new Dictionary<string, HandButton>();
  private HandsComponent? _playerHandsComponent;
  private HandButton? _activeHand;
  private HandButton? _statusHandLeft;
  private HandButton? _statusHandRight;
  private int _backupSuffix;

  private HotbarGui? HandsGui => this.UIManager.GetActiveUIWidgetOrNull<HotbarGui>();

  public void OnSystemLoaded(HandsSystem system)
  {
    this._handsSystem.OnPlayerAddHand += new Action<Entity<HandsComponent>, string, HandLocation>(this.OnAddHand);
    this._handsSystem.OnPlayerItemAdded += new Action<string, EntityUid>(this.OnItemAdded);
    this._handsSystem.OnPlayerItemRemoved += new Action<string, EntityUid>(this.OnItemRemoved);
    this._handsSystem.OnPlayerSetActiveHand += new Action<string>(this.SetActiveHand);
    this._handsSystem.OnPlayerRemoveHand += new Action<Entity<HandsComponent>, string>(this.OnRemoveHand);
    this._handsSystem.OnPlayerHandsAdded += new Action<Entity<HandsComponent>>(this.LoadPlayerHands);
    this._handsSystem.OnPlayerHandsRemoved += new Action(this.UnloadPlayerHands);
    this._handsSystem.OnPlayerHandBlocked += new Action<string>(this.HandBlocked);
    this._handsSystem.OnPlayerHandUnblocked += new Action<string>(this.HandUnblocked);
  }

  public void OnSystemUnloaded(HandsSystem system)
  {
    this._handsSystem.OnPlayerAddHand -= new Action<Entity<HandsComponent>, string, HandLocation>(this.OnAddHand);
    this._handsSystem.OnPlayerItemAdded -= new Action<string, EntityUid>(this.OnItemAdded);
    this._handsSystem.OnPlayerItemRemoved -= new Action<string, EntityUid>(this.OnItemRemoved);
    this._handsSystem.OnPlayerSetActiveHand -= new Action<string>(this.SetActiveHand);
    this._handsSystem.OnPlayerRemoveHand -= new Action<Entity<HandsComponent>, string>(this.OnRemoveHand);
    this._handsSystem.OnPlayerHandsAdded -= new Action<Entity<HandsComponent>>(this.LoadPlayerHands);
    this._handsSystem.OnPlayerHandsRemoved -= new Action(this.UnloadPlayerHands);
    this._handsSystem.OnPlayerHandBlocked -= new Action<string>(this.HandBlocked);
    this._handsSystem.OnPlayerHandUnblocked -= new Action<string>(this.HandUnblocked);
  }

  private void OnAddHand(Entity<HandsComponent> entity, string name, HandLocation location)
  {
    EntityUid owner = entity.Owner;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(owner, localEntity.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    this.AddHand(name, location);
  }

  private void OnRemoveHand(Entity<HandsComponent> entity, string name)
  {
    EntityUid owner = entity.Owner;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(owner, localEntity.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    this.RemoveHand(name);
  }

  private void HandPressed(GUIBoundKeyEventArgs args, SlotControl hand)
  {
    Entity<HandsComponent>? hands;
    if (!this._handsSystem.TryGetPlayerHands(out hands))
      return;
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
    {
      this._handsSystem.UIHandClick(hands.Value, hand.SlotName);
      ((BoundKeyEventArgs) args).Handle();
    }
    else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UseSecondary))
    {
      this._handsSystem.UIHandOpenContextMenu(hand.SlotName);
      ((BoundKeyEventArgs) args).Handle();
    }
    else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ActivateItemInWorld))
    {
      this._handsSystem.UIHandActivate(hand.SlotName);
      ((BoundKeyEventArgs) args).Handle();
    }
    else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.AltActivateItemInWorld))
    {
      this._handsSystem.UIHandAltActivateItem(hand.SlotName);
      ((BoundKeyEventArgs) args).Handle();
    }
    else
    {
      if (!BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ExamineEntity))
        return;
      this._handsSystem.UIInventoryExamine(hand.SlotName);
      ((BoundKeyEventArgs) args).Handle();
    }
  }

  private void UnloadPlayerHands()
  {
    if (this.HandsGui != null)
      ((Control) this.HandsGui).Visible = false;
    this._handContainerIndices.Clear();
    this._handLookup.Clear();
    this._playerHandsComponent = (HandsComponent) null;
    foreach (HandsContainer handsContainer in this._handsContainers)
      handsContainer.Clear();
  }

  private void LoadPlayerHands(Entity<HandsComponent> handsComp)
  {
    if (this.HandsGui != null)
      ((Control) this.HandsGui).Visible = true;
    this._playerHandsComponent = Entity<HandsComponent>.op_Implicit(handsComp);
    foreach ((string str, Hand hand) in handsComp.Comp.Hands)
    {
      HandButton handButton = this.AddHand(str, hand.Location);
      EntityUid? held;
      VirtualItemComponent virtualItemComponent;
      if (this._handsSystem.TryGetHeldItem(handsComp.AsNullable(), str, out held) && this._entities.TryGetComponent<VirtualItemComponent>(held, ref virtualItemComponent))
      {
        handButton.SetEntity(new EntityUid?(virtualItemComponent.BlockingEntity));
        handButton.Blocked = true;
      }
      else
      {
        handButton.SetEntity(held);
        handButton.Blocked = false;
      }
    }
    if (handsComp.Comp.ActiveHandId == null)
      return;
    this.SetActiveHand(handsComp.Comp.ActiveHandId);
  }

  private void HandBlocked(string handName)
  {
    HandButton handButton;
    if (!this._handLookup.TryGetValue(handName, out handButton))
      return;
    handButton.Blocked = true;
  }

  private void HandUnblocked(string handName)
  {
    HandButton handButton;
    if (!this._handLookup.TryGetValue(handName, out handButton))
      return;
    handButton.Blocked = false;
  }

  private int GetHandContainerIndex(string containerName)
  {
    int num;
    return !this._handContainerIndices.TryGetValue(containerName, out num) ? -1 : num;
  }

  private void OnItemAdded(string name, EntityUid entity)
  {
    HandButton hand = this.GetHand(name);
    if (hand == null)
      return;
    VirtualItemComponent virtualItemComponent;
    if (this._entities.TryGetComponent<VirtualItemComponent>(entity, ref virtualItemComponent))
    {
      hand.SetEntity(new EntityUid?(virtualItemComponent.BlockingEntity));
      hand.Blocked = true;
    }
    else
    {
      hand.SetEntity(new EntityUid?(entity));
      hand.Blocked = false;
    }
    this.UpdateHandStatus(hand, new EntityUid?(entity));
  }

  private void OnItemRemoved(string name, EntityUid entity)
  {
    HandButton hand = this.GetHand(name);
    if (hand == null)
      return;
    hand.SetEntity(new EntityUid?());
    this.UpdateHandStatus(hand, new EntityUid?());
  }

  private HandsContainer GetFirstAvailableContainer()
  {
    if (this._handsContainers.Count == 0)
      throw new Exception("Could not find an attached hand hud container");
    foreach (HandsContainer handsContainer in this._handsContainers)
    {
      if (!handsContainer.IsFull)
        return handsContainer;
    }
    throw new Exception("All attached hand hud containers were full!");
  }

  public bool TryGetHandContainer(string containerName, out HandsContainer? container)
  {
    container = (HandsContainer) null;
    int handContainerIndex = this.GetHandContainerIndex(containerName);
    if (handContainerIndex == -1)
      return false;
    container = this._handsContainers[handContainerIndex];
    return true;
  }

  private void StorageActivate(GUIBoundKeyEventArgs args, SlotControl handControl)
  {
    this._handsSystem.UIHandActivate(handControl.SlotName);
  }

  private void SetActiveHand(string? handName)
  {
    if (handName == null)
    {
      if (this._activeHand == null)
        return;
      this._activeHand.Highlight = false;
    }
    else
    {
      HandButton handButton;
      if (!this._handLookup.TryGetValue(handName, out handButton) || handButton == this._activeHand)
        return;
      if (this._activeHand != null)
        this._activeHand.Highlight = false;
      handButton.Highlight = true;
      this._activeHand = handButton;
      if (this.HandsGui == null || this._playerHandsComponent == null)
        return;
      EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._player).LocalSession?.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
      Hand? hand;
      if (!this._handsSystem.TryGetHand(Entity<HandsComponent>.op_Implicit((valueOrDefault, this._playerHandsComponent)), handName, out hand))
        return;
      EntityUid? heldItem = this._handsSystem.GetHeldItem(Entity<HandsComponent>.op_Implicit((valueOrDefault, this._playerHandsComponent)), handName);
      HandUILocation uiLocation = hand.Value.Location.GetUILocation();
      if (uiLocation == HandUILocation.Left)
      {
        this._statusHandLeft = handButton;
        this.HandsGui.UpdatePanelEntityLeft(heldItem);
      }
      else
      {
        this._statusHandRight = handButton;
        this.HandsGui.UpdatePanelEntityRight(heldItem);
      }
      this.HandsGui.SetHighlightHand(new HandUILocation?(uiLocation));
    }
  }

  private HandButton? GetHand(string handName)
  {
    HandButton hand;
    this._handLookup.TryGetValue(handName, out hand);
    return hand;
  }

  private HandButton AddHand(string handName, HandLocation location)
  {
    HandButton newButton = new HandButton(handName, location);
    newButton.StoragePressed += new Action<GUIBoundKeyEventArgs, SlotControl>(this.StorageActivate);
    newButton.Pressed += new Action<GUIBoundKeyEventArgs, SlotControl>(this.HandPressed);
    if (!this._handLookup.TryAdd(handName, newButton))
      return this._handLookup[handName];
    if (this.HandsGui != null)
      this.HandsGui.HandContainer.AddButton(newButton);
    else
      this.GetFirstAvailableContainer().AddButton(newButton);
    if (location.GetUILocation() == HandUILocation.Left)
    {
      if (this._statusHandLeft == null)
        this._statusHandLeft = newButton;
    }
    else if (this._statusHandRight == null)
      this._statusHandRight = newButton;
    this.UpdateVisibleStatusPanels();
    return newButton;
  }

  public void ReloadHands()
  {
    this.UnloadPlayerHands();
    this._handsSystem.ReloadHandButtons();
  }

  public void SwapHands(HandsContainer other, HandsContainer? source = null)
  {
    if (this.HandsGui == null && source == null)
      throw new ArgumentException("Cannot swap hands if no source hand container exists!");
    if (source == null)
      source = this.HandsGui.HandContainer;
    List<Control> controlList = new List<Control>();
    foreach (Control child in ((Control) source).Children)
    {
      if (child is HandButton)
        controlList.Add(child);
    }
    foreach (Control control in controlList)
    {
      ((Control) source).RemoveChild(control);
      ((Control) other).AddChild(control);
    }
  }

  private void RemoveHand(string handName) => this.RemoveHand(handName, out HandButton _);

  private bool RemoveHand(string handName, out HandButton? handButton)
  {
    if (!this._handLookup.TryGetValue(handName, out handButton))
      return false;
    if (handButton.Parent is HandsContainer parent)
      parent.RemoveButton(handButton);
    if (this._statusHandLeft == handButton)
      this._statusHandLeft = (HandButton) null;
    if (this._statusHandRight == handButton)
      this._statusHandRight = (HandButton) null;
    this._handLookup.Remove(handName);
    handButton.Orphan();
    this.UpdateVisibleStatusPanels();
    return true;
  }

  private void UpdateVisibleStatusPanels()
  {
    bool left = false;
    bool right = false;
    foreach (HandButton handButton in this._handLookup.Values)
    {
      if (handButton.HandLocation.GetUILocation() == HandUILocation.Left)
        left = true;
      else
        right = true;
    }
    this.HandsGui?.UpdateStatusVisibility(left, right);
  }

  public string RegisterHandContainer(HandsContainer handContainer)
  {
    string key = "HandContainer_" + this._backupSuffix.ToString();
    if (handContainer.Indexer == null)
    {
      handContainer.Indexer = key;
      ++this._backupSuffix;
    }
    else
      key = handContainer.Indexer;
    this._handContainerIndices.Add(key, this._handsContainers.Count);
    this._handsContainers.Add(handContainer);
    return key;
  }

  public bool RemoveHandContainer(string handContainerName)
  {
    int handContainerIndex = this.GetHandContainerIndex(handContainerName);
    if (handContainerIndex == -1)
      return false;
    this._handContainerIndices.Remove(handContainerName);
    this._handsContainers.RemoveAt(handContainerIndex);
    return true;
  }

  public bool RemoveHandContainer(string handContainerName, out HandsContainer? container)
  {
    int index;
    int num = this._handContainerIndices.TryGetValue(handContainerName, out index) ? 1 : 0;
    container = this._handsContainers[index];
    this._handContainerIndices.Remove(handContainerName);
    this._handsContainers.RemoveAt(index);
    return num != 0;
  }

  public void OnStateEntered(GameplayState state)
  {
    if (this.HandsGui == null)
      return;
    ((Control) this.HandsGui).Visible = this._playerHandsComponent != null;
  }

  public virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    foreach (HandsContainer handsContainer in this._handsContainers)
    {
      foreach (HandButton button in handsContainer.GetButtons())
      {
        UseDelayComponent useDelayComponent;
        if (!this._entities.TryGetComponent<UseDelayComponent>(button.Entity, ref useDelayComponent))
        {
          button.CooldownDisplay.Visible = false;
        }
        else
        {
          UseDelayInfo lastEndingDelay = this._useDelay.GetLastEndingDelay(Entity<UseDelayComponent>.op_Implicit((button.Entity.Value, useDelayComponent)));
          button.CooldownDisplay.Visible = true;
          button.CooldownDisplay.FromTime(lastEndingDelay.StartTime, lastEndingDelay.EndTime);
        }
      }
    }
  }

  private void UpdateHandStatus(HandButton hand, EntityUid? entity)
  {
    if (hand == this._statusHandLeft)
      this.HandsGui?.UpdatePanelEntityLeft(entity);
    if (hand != this._statusHandRight)
      return;
    this.HandsGui?.UpdatePanelEntityRight(entity);
  }
}
