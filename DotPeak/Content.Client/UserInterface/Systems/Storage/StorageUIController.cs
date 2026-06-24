// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Storage.StorageUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Examine;
using Content.Client.Hands.Systems;
using Content.Client.Interaction;
using Content.Client.Storage;
using Content.Client.Storage.Systems;
using Content.Client.UserInterface.Systems.Hotbar.Widgets;
using Content.Client.UserInterface.Systems.Info;
using Content.Client.UserInterface.Systems.Storage.Controls;
using Content.Client.Verbs.UI;
using Content.Shared.CCVar;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Systems.Storage;

public sealed class StorageUIController : 
  UIController,
  IOnSystemChanged<StorageSystem>,
  IOnSystemLoaded<StorageSystem>,
  IOnSystemUnloaded<StorageSystem>
{
  [Dependency]
  private IConfigurationManager _configuration;
  [Dependency]
  private IInputManager _input;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private CloseRecentWindowUIController _closeRecentWindowUIController;
  [UISystemDependency]
  private readonly StorageSystem _storage;
  [UISystemDependency]
  private readonly UserInterfaceSystem _ui;
  private readonly DragDropHelper<ItemGridPiece> _menuDragHelper;
  public Angle DraggingRotation = Angle.Zero;
  public bool StaticStorageUIEnabled;
  public bool OpaqueStorageWindow;
  private int _openStorageLimit = -1;

  public ItemGridPiece? DraggingGhost => this._menuDragHelper.Dragged;

  public bool IsDragging => this._menuDragHelper.IsDragging;

  public ItemGridPiece? CurrentlyDragging => this._menuDragHelper.Dragged;

  public bool WindowTitle { get; private set; }

  public StorageUIController()
  {
    this._menuDragHelper = new DragDropHelper<ItemGridPiece>(new OnBeginDrag(this.OnMenuBeginDrag), new OnContinueDrag(this.OnMenuContinueDrag), new OnEndDrag(this.OnMenuEndDrag));
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this._configuration.OnValueChanged<bool>(CCVars.StaticStorageUI, new Action<bool>(this.OnStaticStorageChanged), true);
    this._configuration.OnValueChanged<bool>(CCVars.OpaqueStorageWindow, new Action<bool>(this.OnOpaqueWindowChanged), true);
    this._configuration.OnValueChanged<bool>(CCVars.StorageWindowTitle, new Action<bool>(this.OnStorageWindowTitle), true);
    this._configuration.OnValueChanged<int>(CCVars.StorageLimit, new Action<int>(this.OnStorageLimitChanged), true);
  }

  private void OnStorageLimitChanged(int obj) => this._openStorageLimit = obj;

  private void OnStorageWindowTitle(bool obj) => this.WindowTitle = obj;

  private void OnOpaqueWindowChanged(bool obj) => this.OpaqueStorageWindow = obj;

  private void OnStaticStorageChanged(bool obj) => this.StaticStorageUIEnabled = obj;

  public StorageWindow CreateStorageWindow(StorageBoundUserInterface sBui)
  {
    StorageWindow window = new StorageWindow();
    ((Control) window).MouseFilter = (Control.MouseFilterMode) 1;
    window.OnPiecePressed += (Action<GUIBoundKeyEventArgs, ItemGridPiece>) ((args, piece) => this.OnPiecePressed(args, window, piece));
    window.OnPieceUnpressed += (Action<GUIBoundKeyEventArgs, ItemGridPiece>) ((args, piece) => this.OnPieceUnpressed(args, window, piece));
    if (this.StaticStorageUIEnabled)
    {
      HotbarGui activeUiWidgetOrNull = this.UIManager.GetActiveUIWidgetOrNull<HotbarGui>();
      Action<Control, Control> action = (Action<Control, Control>) ((parent, child) =>
      {
        if (parent == null)
          return;
        int index = ((IEnumerable<Control>) parent.Children).ToList<Control>().FindIndex((Predicate<Control>) (c => !c.Visible));
        if (index == -1)
          return;
        child.SetPositionInParent(index);
      });
      if (activeUiWidgetOrNull != null)
      {
        ((Control) activeUiWidgetOrNull.DoubleStorageContainer).Visible = this._openStorageLimit == 2;
        ((Control) activeUiWidgetOrNull.SingleStorageContainer).Visible = this._openStorageLimit != 2;
      }
      if (this._openStorageLimit == 2)
      {
        if (activeUiWidgetOrNull != null && !((IEnumerable<Control>) ((Control) activeUiWidgetOrNull.LeftStorageContainer).Children).Any<Control>((Func<Control, bool>) (c => c.Visible)))
        {
          ((Control) activeUiWidgetOrNull?.LeftStorageContainer).AddChild((Control) window);
          action((Control) activeUiWidgetOrNull?.LeftStorageContainer, (Control) window);
        }
        else
        {
          ((Control) activeUiWidgetOrNull?.RightStorageContainer).AddChild((Control) window);
          action((Control) activeUiWidgetOrNull?.RightStorageContainer, (Control) window);
        }
      }
      else
      {
        ((Control) activeUiWidgetOrNull?.SingleStorageContainer).AddChild((Control) window);
        action((Control) activeUiWidgetOrNull?.SingleStorageContainer, (Control) window);
      }
      this._closeRecentWindowUIController.SetMostRecentlyInteractedWindow((BaseWindow) window);
    }
    else
    {
      StorageBoundUserInterface boundUserInterface;
      if (((SharedUserInterfaceSystem) this._ui).TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(this.EntityManager.GetComponent<TransformComponent>(sBui.Owner).ParentUid), (Enum) StorageComponent.StorageUiKey.Key, ref boundUserInterface) && boundUserInterface.Position.HasValue)
      {
        window.Open(boundUserInterface.Position.Value);
      }
      else
      {
        Vector2 vector2;
        if (((SharedUserInterfaceSystem) this._ui).TryGetPosition(Entity<UserInterfaceComponent>.op_Implicit(sBui.Owner), (Enum) StorageComponent.StorageUiKey.Key, ref vector2))
          window.Open(vector2);
        else
          window.OpenCenteredLeft();
      }
    }
    this._ui.RegisterControl((BoundUserInterface) sBui, (Control) window);
    return window;
  }

  public void OnSystemLoaded(StorageSystem system)
  {
    // ISSUE: method pointer
    this._input.FirstChanceOnKeyEvent += new KeyEventAction((object) this, __methodptr(OnMiddleMouse));
  }

  public void OnSystemUnloaded(StorageSystem system)
  {
    // ISSUE: method pointer
    this._input.FirstChanceOnKeyEvent -= new KeyEventAction((object) this, __methodptr(OnMiddleMouse));
  }

  private void OnMiddleMouse(KeyEventArgs keyEvent, KeyEventType type)
  {
    IKeyBinding ikeyBinding;
    if (((InputEventArgs) keyEvent).Handled || type != null || !this._input.TryGetKeyBinding(ContentKeyFunctions.RotateStoredItem, ref ikeyBinding) || ikeyBinding.BaseKey != keyEvent.Key || ((ModifierInputEventArgs) keyEvent).Shift && ikeyBinding.Mod1 != 58 && ikeyBinding.Mod2 != 58 && ikeyBinding.Mod3 != 58 || ((ModifierInputEventArgs) keyEvent).Alt && ikeyBinding.Mod1 != 59 && ikeyBinding.Mod2 != 59 && ikeyBinding.Mod3 != 59 || ((ModifierInputEventArgs) keyEvent).Control && ikeyBinding.Mod1 != 57 && ikeyBinding.Mod2 != 57 && ikeyBinding.Mod3 != 57 || !this.IsDragging && !this.EntityManager.System<HandsSystem>().GetActiveHandEntity().HasValue || this.DraggingGhost == null && !(this.UIManager.CurrentlyHovered is StorageWindow))
      return;
    Angle angle = Angle.op_Addition(this.DraggingRotation, Angle.op_Implicit(Math.PI / 2.0));
    this.DraggingRotation = DirectionExtensions.ToAngle(((Angle) ref angle).GetCardinalDir());
    if (this.DraggingGhost != null)
      this.DraggingGhost.Location.Rotation = this.DraggingRotation;
    if (!this.IsDragging && !(this.UIManager.CurrentlyHovered is StorageWindow))
      return;
    ((InputEventArgs) keyEvent).Handle();
  }

  private void OnPiecePressed(
    GUIBoundKeyEventArgs args,
    StorageWindow window,
    ItemGridPiece control)
  {
    if (this.IsDragging || !window.IsOpen)
      return;
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.MoveStoredItem))
      ((BoundKeyEventArgs) args).Handle();
    else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.SaveItemLocation))
    {
      EntityUid? storageEntity = window.StorageEntity;
      if (!storageEntity.HasValue)
        return;
      EntityUid valueOrDefault = storageEntity.GetValueOrDefault();
      this.EntityManager.RaisePredictiveEvent<StorageSaveItemLocationEvent>(new StorageSaveItemLocationEvent(this.EntityManager.GetNetEntity(control.Entity, (MetaDataComponent) null), this.EntityManager.GetNetEntity(valueOrDefault, (MetaDataComponent) null)));
      ((BoundKeyEventArgs) args).Handle();
    }
    else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ExamineEntity))
    {
      this.EntityManager.System<ExamineSystem>().DoExamine(control.Entity);
      ((BoundKeyEventArgs) args).Handle();
    }
    else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UseSecondary))
    {
      this.UIManager.GetUIController<VerbMenuUIController>().OpenVerbMenu(control.Entity);
      ((BoundKeyEventArgs) args).Handle();
    }
    else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ActivateItemInWorld))
    {
      this.EntityManager.RaisePredictiveEvent<InteractInventorySlotEvent>(new InteractInventorySlotEvent(this.EntityManager.GetNetEntity(control.Entity, (MetaDataComponent) null)));
      ((BoundKeyEventArgs) args).Handle();
    }
    else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.AltActivateItemInWorld))
    {
      this.EntityManager.RaisePredictiveEvent<InteractInventorySlotEvent>(new InteractInventorySlotEvent(this.EntityManager.GetNetEntity(control.Entity, (MetaDataComponent) null), true));
      ((BoundKeyEventArgs) args).Handle();
    }
    window.FlagDirty();
  }

  private void OnPieceUnpressed(
    GUIBoundKeyEventArgs args,
    StorageWindow window,
    ItemGridPiece control)
  {
    if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.MoveStoredItem))
      return;
    control.MouseFilter = (Control.MouseFilterMode) 2;
    Control control1 = this.UIManager.MouseGetControl(((BoundKeyEventArgs) args).PointerLocation);
    StorageWindow storageWindow = control1 as StorageWindow;
    control.MouseFilter = (Control.MouseFilterMode) 1;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    window.RemoveGrid(control);
    window.FlagDirty();
    if (!(control1 is ItemGridPiece))
    {
      EntityUid? storageEntity = window.StorageEntity;
      if (storageEntity.HasValue)
      {
        EntityUid valueOrDefault = storageEntity.GetValueOrDefault();
        if (localEntity.HasValue)
        {
          if (this._menuDragHelper.IsDragging)
          {
            ItemGridPiece draggingGhost = this.DraggingGhost;
            if (draggingGhost != null)
            {
              EntityUid entity = draggingGhost.Entity;
              ItemStorageLocation location1 = draggingGhost.Location;
              if (storageWindow == window)
              {
                ItemStorageLocation location2 = new ItemStorageLocation(this.DraggingRotation, storageWindow.GetMouseGridPieceLocation(Entity<ItemComponent>.op_Implicit(entity), location1));
                if (!this._storage.ItemFitsInGridLocation(Entity<ItemComponent>.op_Implicit(entity), Entity<StorageComponent>.op_Implicit(valueOrDefault), location2))
                {
                  window.Reclaim(control.Location, control);
                }
                else
                {
                  this.EntityManager.RaisePredictiveEvent<StorageSetItemLocationEvent>(new StorageSetItemLocationEvent(this.EntityManager.GetNetEntity(draggingGhost.Entity, (MetaDataComponent) null), this.EntityManager.GetNetEntity(valueOrDefault, (MetaDataComponent) null), location2));
                  window.Reclaim(location2, control);
                }
              }
              else if (storageWindow != null && storageWindow.StorageEntity.HasValue && storageWindow != window)
              {
                ItemStorageLocation location3 = new ItemStorageLocation(this.DraggingRotation, storageWindow.GetMouseGridPieceLocation(Entity<ItemComponent>.op_Implicit(entity), location1));
                if (this._storage.ItemFitsInGridLocation(Entity<ItemComponent>.op_Implicit((entity, (ItemComponent) null)), Entity<StorageComponent>.op_Implicit((storageWindow.StorageEntity.Value, (StorageComponent) null)), location3))
                {
                  this.EntityManager.RaisePredictiveEvent<StorageTransferItemEvent>(new StorageTransferItemEvent(this.EntityManager.GetNetEntity(entity, (MetaDataComponent) null), this.EntityManager.GetNetEntity(storageWindow.StorageEntity.Value, (MetaDataComponent) null), location3));
                  storageWindow.Reclaim(location3, control);
                  this.DraggingRotation = Angle.Zero;
                }
                else
                  window.Reclaim(location1, control);
              }
              if (storageWindow != null)
              {
                storageWindow.FlagDirty();
                goto label_18;
              }
              goto label_18;
            }
          }
          window.Reclaim(control.Location, control);
          this.EntityManager.RaisePredictiveEvent<StorageInteractWithItemEvent>(new StorageInteractWithItemEvent(this.EntityManager.GetNetEntity(control.Entity, (MetaDataComponent) null), this.EntityManager.GetNetEntity(valueOrDefault, (MetaDataComponent) null)));
label_18:
          this._menuDragHelper.EndDrag();
          ((BoundKeyEventArgs) args).Handle();
          return;
        }
      }
    }
    window.Reclaim(control.Location, control);
    ((BoundKeyEventArgs) args).Handle();
    this._menuDragHelper.EndDrag();
  }

  private bool OnMenuBeginDrag()
  {
    ItemGridPiece dragged = this._menuDragHelper.Dragged;
    if (dragged == null)
      return false;
    this.DraggingGhost.Orphan();
    this.DraggingRotation = dragged.Location.Rotation;
    ((Control) this.UIManager.PopupRoot).AddChild((Control) this.DraggingGhost);
    this.SetDraggingRotation();
    return true;
  }

  private bool OnMenuContinueDrag(float frameTime)
  {
    if (this.DraggingGhost == null)
      return false;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    BaseContainer container;
    if (!localEntity.HasValue || !this._storage.TryGetStorageLocation(Entity<ItemComponent>.op_Implicit(this.DraggingGhost.Entity), out container, out StorageComponent _, out ItemStorageLocation _) || !((SharedUserInterfaceSystem) this._ui).IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(container.Owner), (Enum) StorageComponent.StorageUiKey.Key, localEntity.Value))
    {
      this.DraggingGhost.Orphan();
      return false;
    }
    this.SetDraggingRotation();
    return true;
  }

  private void SetDraggingRotation()
  {
    BaseContainer baseContainer;
    StorageComponent storageComponent;
    if (this.DraggingGhost == null || !((SharedContainerSystem) this.EntityManager.System<ContainerSystem>()).TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((this.DraggingGhost.Entity, (TransformComponent) null)), ref baseContainer) || !this.EntityManager.TryGetComponent<StorageComponent>(baseContainer.Owner, ref storageComponent))
      return;
    LayoutContainer.SetPosition((Control) this.DraggingGhost, this.UIManager.MousePositionScaled.Position / 2f - ItemGridPiece.GetCenterOffset(Entity<StorageComponent>.op_Implicit((baseContainer.Owner, storageComponent)), Entity<ItemComponent>.op_Implicit((this.DraggingGhost.Entity, (ItemComponent) null)), new ItemStorageLocation(this.DraggingRotation, Vector2i.Zero), this.EntityManager));
  }

  private void OnMenuEndDrag()
  {
    if (this.DraggingGhost == null)
      return;
    this.DraggingRotation = Angle.Zero;
  }

  public virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    this._menuDragHelper.Update(((FrameEventArgs) ref args).DeltaSeconds);
  }
}
