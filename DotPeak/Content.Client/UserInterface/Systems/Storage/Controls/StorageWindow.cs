// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Storage.Controls.StorageWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Hands.Systems;
using Content.Client.Items.Systems;
using Content.Client.Storage;
using Content.Client.Storage.Systems;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Item;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Item;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Collections;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Systems.Storage.Controls;

public sealed class StorageWindow : BaseWindow
{
  [Dependency]
  private IEntityManager _entity;
  [Dependency]
  private IPlayerManager _player;
  private readonly StorageUIController _storageController;
  public EntityUid? StorageEntity;
  private readonly GridContainer _pieceGrid;
  private readonly GridContainer _backgroundGrid;
  private readonly GridContainer _sidebar;
  private Control _titleContainer;
  private Label _titleLabel;
  private readonly Dictionary<EntityUid, (ItemStorageLocation? Loc, ItemGridPiece Control)> _pieces = new Dictionary<EntityUid, (ItemStorageLocation?, ItemGridPiece)>();
  private readonly List<Control> _controlGrid = new List<Control>();
  private ValueList<EntityUid> _contained;
  private ValueList<EntityUid> _toRemove;
  private Vector2i _pieceGridSize;
  private TextureButton? _backButton;
  private bool _isDirty;
  private readonly string _emptyTexturePath = "Storage/cm_tile_empty";
  private Texture? _emptyTexture;
  private readonly string _blockedTexturePath = "Storage/tile_blocked";
  private Texture? _blockedTexture;
  private readonly string _emptyOpaqueTexturePath = "Storage/cm_tile_empty_opaque";
  private Texture? _emptyOpaqueTexture;
  private readonly string _blockedOpaqueTexturePath = "Storage/tile_blocked_opaque";
  private Texture? _blockedOpaqueTexture;
  private readonly string _exitTexturePath = "Storage/exit";
  private Texture? _exitTexture;
  private readonly string _backTexturePath = "Storage/back";
  private Texture? _backTexture;
  private readonly string _sidebarTopTexturePath = "Storage/sidebar_top";
  private Texture? _sidebarTopTexture;
  private readonly string _sidebarMiddleTexturePath = "Storage/sidebar_mid";
  private Texture? _sidebarMiddleTexture;
  private readonly string _sidebarBottomTexturePath = "Storage/sidebar_bottom";
  private Texture? _sidebarBottomTexture;
  private readonly string _sidebarFatTexturePath = "Storage/sidebar_fat";
  private Texture? _sidebarFatTexture;
  private (Box2i Grid, EntityUid[] Contained, Dictionary<EntityUid, ItemStorageLocation> Stored) _lastUpdate = (new Box2i(), Array.Empty<EntityUid>(), new Dictionary<EntityUid, ItemStorageLocation>());

  public event Action<GUIBoundKeyEventArgs, ItemGridPiece>? OnPiecePressed;

  public event Action<GUIBoundKeyEventArgs, ItemGridPiece>? OnPieceUnpressed;

  public StorageWindow()
  {
    IoCManager.InjectDependencies<StorageWindow>(this);
    this.Resizable = false;
    this._storageController = ((Control) this).UserInterfaceManager.GetUIController<StorageUIController>();
    ((Control) this).OnThemeUpdated();
    ((Control) this).MouseFilter = (Control.MouseFilterMode) 0;
    GridContainer gridContainer1 = new GridContainer();
    ((Control) gridContainer1).Name = "SideBar";
    gridContainer1.HSeparationOverride = new int?(0);
    gridContainer1.VSeparationOverride = new int?(0);
    gridContainer1.Columns = 1;
    this._sidebar = gridContainer1;
    GridContainer gridContainer2 = new GridContainer();
    ((Control) gridContainer2).Name = "PieceGrid";
    gridContainer2.HSeparationOverride = new int?(0);
    gridContainer2.VSeparationOverride = new int?(0);
    this._pieceGrid = gridContainer2;
    GridContainer gridContainer3 = new GridContainer();
    ((Control) gridContainer3).Name = "BackgroundGrid";
    gridContainer3.HSeparationOverride = new int?(0);
    gridContainer3.VSeparationOverride = new int?(0);
    this._backgroundGrid = gridContainer3;
    Label label = new Label();
    ((Control) label).HorizontalExpand = true;
    ((Control) label).Name = "StorageLabel";
    label.ClipText = true;
    label.Text = "Dummy";
    ((Control) label).StyleClasses.Add("FancyWindowTitle");
    this._titleLabel = label;
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).StyleClasses.Add("WindowHeadingBackground");
    ((Control) panelContainer1).Children.Add((Control) this._titleLabel);
    this._titleContainer = (Control) panelContainer1;
    PanelContainer panelContainer2 = new PanelContainer();
    panelContainer2.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BorderColor = Color.Black,
      BorderThickness = new Thickness(2f)
    };
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).Children.Add(this._titleContainer);
    Control.OrderedChildCollection children1 = ((Control) boxContainer1).Children;
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer2).Children.Add((Control) this._sidebar);
    Control.OrderedChildCollection children2 = ((Control) boxContainer2).Children;
    Control control = new Control();
    control.Children.Add((Control) this._backgroundGrid);
    control.Children.Add((Control) this._pieceGrid);
    control.Children.Add((Control) panelContainer2);
    children2.Add(control);
    children1.Add((Control) boxContainer2);
    ((Control) this).AddChild((Control) boxContainer1);
  }

  protected virtual void OnThemeUpdated()
  {
    ((Control) this).OnThemeUpdated();
    this._emptyTexture = ((Control) this).Theme.ResolveTextureOrNull(this._emptyTexturePath)?.Texture;
    this._blockedTexture = ((Control) this).Theme.ResolveTextureOrNull(this._blockedTexturePath)?.Texture;
    this._emptyOpaqueTexture = ((Control) this).Theme.ResolveTextureOrNull(this._emptyOpaqueTexturePath)?.Texture;
    this._blockedOpaqueTexture = ((Control) this).Theme.ResolveTextureOrNull(this._blockedOpaqueTexturePath)?.Texture;
    this._exitTexture = ((Control) this).Theme.ResolveTextureOrNull(this._exitTexturePath)?.Texture;
    this._backTexture = ((Control) this).Theme.ResolveTextureOrNull(this._backTexturePath)?.Texture;
    this._sidebarTopTexture = ((Control) this).Theme.ResolveTextureOrNull(this._sidebarTopTexturePath)?.Texture;
    this._sidebarMiddleTexture = ((Control) this).Theme.ResolveTextureOrNull(this._sidebarMiddleTexturePath)?.Texture;
    this._sidebarBottomTexture = ((Control) this).Theme.ResolveTextureOrNull(this._sidebarBottomTexturePath)?.Texture;
    this._sidebarFatTexture = ((Control) this).Theme.ResolveTextureOrNull(this._sidebarFatTexturePath)?.Texture;
  }

  public void UpdateContainer(Entity<StorageComponent>? entity)
  {
    ((Control) this).Visible = entity.HasValue;
    Entity<StorageComponent>? nullable = entity;
    this.StorageEntity = nullable.HasValue ? new EntityUid?(Entity<StorageComponent>.op_Implicit(nullable.GetValueOrDefault())) : new EntityUid?();
    if (!entity.HasValue)
      return;
    if (((Control) this).UserInterfaceManager.GetUIController<StorageUIController>().WindowTitle)
    {
      this._titleLabel.Text = (string) Identity.Name(Entity<StorageComponent>.op_Implicit(entity.Value), this._entity);
      this._titleContainer.Visible = true;
    }
    else
      this._titleContainer.Visible = false;
    this.BuildGridRepresentation();
  }

  private void CloseParent()
  {
    if (!this.StorageEntity.HasValue)
      return;
    SharedContainerSystem sharedContainerSystem = this._entity.System<SharedContainerSystem>();
    UserInterfaceSystem userInterfaceSystem = this._entity.System<UserInterfaceSystem>();
    Entity<TransformComponent, MetaDataComponent> entity = Entity<TransformComponent, MetaDataComponent>.op_Implicit(this.StorageEntity.Value);
    BaseContainer baseContainer;
    ref BaseContainer local = ref baseContainer;
    StorageComponent storageComponent;
    StorageBoundUserInterface boundUserInterface;
    if (!sharedContainerSystem.TryGetContainingContainer(entity, ref local) || !this._entity.TryGetComponent<StorageComponent>(baseContainer.Owner, ref storageComponent) || !((BaseContainer) storageComponent.Container).Contains(this.StorageEntity.Value) || !((SharedUserInterfaceSystem) userInterfaceSystem).TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(baseContainer.Owner), (Enum) StorageComponent.StorageUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.CloseWindow(((Control) this).Position);
  }

  private void BuildGridRepresentation()
  {
    StorageComponent storageComponent1;
    if (!this._entity.TryGetComponent<StorageComponent>(this.StorageEntity, ref storageComponent1) || storageComponent1.Grid.Count == 0)
      return;
    Box2i boundingBox = storageComponent1.Grid.GetBoundingBox();
    this.BuildBackground();
    ((Control) this._sidebar).Children.Clear();
    int num1 = ((Box2i) ref boundingBox).Height + 1;
    this._sidebar.Rows = num1;
    TextureButton textureButton1 = new TextureButton();
    ((Control) textureButton1).Name = "ExitButton";
    textureButton1.TextureNormal = this._exitTexture;
    textureButton1.Scale = new Vector2(2f, 2f);
    TextureButton textureButton2 = textureButton1;
    ((BaseButton) textureButton2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.Close();
      this.CloseParent();
    });
    ((Control) textureButton2).OnKeyBindDown += (Action<GUIBoundKeyEventArgs>) (args =>
    {
      if (((BoundKeyEventArgs) args).Handled || !BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ActivateItemInWorld))
        return;
      this.Close();
      this.CloseParent();
      ((BoundKeyEventArgs) args).Handle();
    });
    BoxContainer boxContainer1 = new BoxContainer();
    ((Control) boxContainer1).Name = "ExitContainer";
    Control.OrderedChildCollection children1 = ((Control) boxContainer1).Children;
    TextureRect textureRect1 = new TextureRect();
    textureRect1.Texture = ((Box2i) ref boundingBox).Height != 0 ? this._sidebarTopTexture : this._sidebarFatTexture;
    textureRect1.TextureScale = new Vector2(2f, 2f);
    ((Control) textureRect1).Children.Add((Control) textureButton2);
    children1.Add((Control) textureRect1);
    ((Control) this._sidebar).AddChild((Control) boxContainer1);
    int num2 = 2;
    if (this._entity.System<StorageSystem>().NestedStorage && num1 > 0)
    {
      this._backButton = new TextureButton()
      {
        TextureNormal = this._backTexture,
        Scale = new Vector2(2f, 2f)
      };
      ((BaseButton) this._backButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        BaseContainer baseContainer;
        StorageComponent storageComponent2;
        if (!this._entity.System<SharedContainerSystem>().TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(this.StorageEntity.Value), ref baseContainer) || !this._entity.TryGetComponent<StorageComponent>(baseContainer.Owner, ref storageComponent2) || !((BaseContainer) storageComponent2.Container).Contains(this.StorageEntity.Value))
          return;
        this.Close();
        StorageBoundUserInterface boundUserInterface;
        if (!this._entity.System<SharedUserInterfaceSystem>().TryGetOpenUi<StorageBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(baseContainer.Owner), (Enum) StorageComponent.StorageUiKey.Key, ref boundUserInterface))
          return;
        boundUserInterface.Show(((Control) this).Position);
      });
      BoxContainer boxContainer2 = new BoxContainer();
      ((Control) boxContainer2).Name = "ExitContainer";
      Control.OrderedChildCollection children2 = ((Control) boxContainer2).Children;
      TextureRect textureRect2 = new TextureRect();
      textureRect2.Texture = num1 > 2 ? this._sidebarMiddleTexture : this._sidebarBottomTexture;
      textureRect2.TextureScale = new Vector2(2f, 2f);
      ((Control) textureRect2).Children.Add((Control) this._backButton);
      children2.Add((Control) textureRect2);
      ((Control) this._sidebar).AddChild((Control) boxContainer2);
    }
    int num3 = num1 - num2;
    for (int index = 0; index < num3; ++index)
      ((Control) this._sidebar).AddChild((Control) new TextureRect()
      {
        Texture = (index != num3 - 1 ? this._sidebarMiddleTexture : this._sidebarBottomTexture),
        TextureScale = new Vector2(2f, 2f)
      });
    this.FlagDirty();
  }

  public void BuildBackground()
  {
    StorageComponent storageComponent;
    if (!this._entity.TryGetComponent<StorageComponent>(this.StorageEntity, ref storageComponent) || !storageComponent.Grid.Any<Box2i>())
      return;
    Box2i boundingBox = storageComponent.Grid.GetBoundingBox();
    Texture texture1 = this._storageController.OpaqueStorageWindow ? this._emptyOpaqueTexture : this._emptyTexture;
    Texture texture2 = this._storageController.OpaqueStorageWindow ? this._blockedOpaqueTexture : this._blockedTexture;
    ((Control) this._backgroundGrid).Children.Clear();
    this._backgroundGrid.Rows = ((Box2i) ref boundingBox).Height + 1;
    this._backgroundGrid.Columns = ((Box2i) ref boundingBox).Width + 1;
    int? x = EntityManagerExt.GetComponentOrNull<FixedItemSizeStorageComponent>(this._entity, this.StorageEntity)?.Size.X;
    for (int bottom = boundingBox.Bottom; bottom <= boundingBox.Top; ++bottom)
    {
      for (int left = boundingBox.Left; left <= boundingBox.Right; ++left)
      {
        Texture texture3 = storageComponent.Grid.Contains(left, bottom) ? texture1 : texture2;
        TextureRect textureRect = new TextureRect()
        {
          Texture = texture3,
          TextureScale = new Vector2(2f, 2f)
        };
        PanelContainer panelContainer = this.WrapBorders((Control) textureRect, x, left, boundingBox.Right);
        if (panelContainer != null)
          ((Control) this._backgroundGrid).AddChild((Control) panelContainer);
        else
          ((Control) this._backgroundGrid).AddChild((Control) textureRect);
      }
    }
  }

  public void Reclaim(ItemStorageLocation location, ItemGridPiece draggingGhost)
  {
    draggingGhost.OnPiecePressed += this.OnPiecePressed;
    draggingGhost.OnPieceUnpressed += this.OnPieceUnpressed;
    this._pieces[draggingGhost.Entity] = (new ItemStorageLocation?(location), draggingGhost);
    draggingGhost.Location = location;
    this._controlGrid[this.GetGridIndex(draggingGhost)].AddChild((Control) draggingGhost);
  }

  private int GetGridIndex(ItemGridPiece piece)
  {
    return piece.Location.Position.X + piece.Location.Position.Y * this._pieceGrid.Columns;
  }

  public void FlagDirty() => this._isDirty = true;

  public void RemoveGrid(ItemGridPiece control)
  {
    control.Orphan();
    this._pieces.Remove(control.Entity);
    control.OnPiecePressed -= this.OnPiecePressed;
    control.OnPieceUnpressed -= this.OnPieceUnpressed;
  }

  public void BuildItemPieces()
  {
    StorageComponent storageComp;
    if (!this._entity.TryGetComponent<StorageComponent>(this.StorageEntity, ref storageComp) || storageComp.Grid.Count == 0)
      return;
    Box2i boundingBox = storageComp.Grid.GetBoundingBox();
    Vector2i vector2i = Vector2i.op_Multiply(this._emptyTexture.Size, 2);
    EntityUid[] array = ((BaseContainer) storageComp.Container).ContainedEntities.Reverse<EntityUid>().ToArray<EntityUid>();
    ItemStorageLocation itemStorageLocation1;
    // ISSUE: explicit reference operation
    if (((Box2i) @this._lastUpdate.Grid).Equals(boundingBox) && ((IEnumerable<EntityUid>) this._lastUpdate.Contained).SequenceEqual<EntityUid>((IEnumerable<EntityUid>) array) && this._lastUpdate.Stored.Count == storageComp.StoredItems.Count && this._lastUpdate.Stored.All<KeyValuePair<EntityUid, ItemStorageLocation>>((Func<KeyValuePair<EntityUid, ItemStorageLocation>, bool>) (kvp => storageComp.StoredItems.TryGetValue(kvp.Key, out itemStorageLocation1) && kvp.Value == itemStorageLocation1)))
      return;
    this._lastUpdate = (boundingBox, array, storageComp.StoredItems);
    this._contained.Clear();
    this._contained.AddRange(((BaseContainer) storageComp.Container).ContainedEntities.Reverse<EntityUid>());
    int num1 = ((Box2i) ref boundingBox).Width + 1;
    int num2 = ((Box2i) ref boundingBox).Height + 1;
    if (this._pieceGrid.Rows != this._pieceGridSize.Y || this._pieceGrid.Columns != this._pieceGridSize.X)
    {
      this._pieceGrid.Rows = num2;
      this._pieceGrid.Columns = num1;
      this._controlGrid.Clear();
      int? x = EntityManagerExt.GetComponentOrNull<FixedItemSizeStorageComponent>(this._entity, this.StorageEntity)?.Size.X;
      for (int bottom = boundingBox.Bottom; bottom <= boundingBox.Top; ++bottom)
      {
        for (int left = boundingBox.Left; left <= boundingBox.Right; ++left)
        {
          Control control = new Control()
          {
            MinSize = Vector2i.op_Implicit(vector2i)
          };
          PanelContainer panelContainer = this.WrapBorders(control, x, left, boundingBox.Right);
          if (panelContainer != null)
            ((Control) this._pieceGrid).AddChild((Control) panelContainer);
          else
            ((Control) this._pieceGrid).AddChild(control);
          this._controlGrid.Add(control);
        }
      }
    }
    this._pieceGridSize = new Vector2i(num1, num2);
    this._toRemove.Clear();
    foreach ((EntityUid key, (ItemStorageLocation? Loc, ItemGridPiece Control) tuple1) in this._pieces)
    {
      EntityUid entityUid = key;
      (ItemStorageLocation? Loc, ItemGridPiece Control) tuple2 = tuple1;
      ItemStorageLocation itemStorageLocation2;
      if (storageComp.StoredItems.TryGetValue(entityUid, out itemStorageLocation2))
      {
        tuple2.Control.Marked = this.IsMarked(entityUid);
        if (!tuple2.Loc.Equals((object) itemStorageLocation2))
        {
          tuple2.Control.Location = itemStorageLocation2;
          int gridIndex = this.GetGridIndex(tuple2.Control);
          tuple2.Control.Orphan();
          this._controlGrid[gridIndex].AddChild((Control) tuple2.Control);
          this._pieces[entityUid] = (new ItemStorageLocation?(itemStorageLocation2), tuple2.Control);
        }
      }
      else
        this._toRemove.Add(entityUid);
    }
    foreach (EntityUid key in this._toRemove)
    {
      (ItemStorageLocation? Loc, ItemGridPiece Control) tuple;
      this._pieces.Remove(key, out tuple);
      tuple.Control.Orphan();
    }
    ItemStorageLocation itemStorageLocation4;
    foreach ((key, itemStorageLocation4) in storageComp.StoredItems)
    {
      EntityUid entityUid = key;
      ItemStorageLocation location = itemStorageLocation4;
      ItemComponent itemComponent;
      if (!this._pieces.TryGetValue(entityUid, out (ItemStorageLocation?, ItemGridPiece) _) && this._entity.TryGetComponent<ItemComponent>(entityUid, ref itemComponent))
      {
        ItemGridPiece itemGridPiece1 = new ItemGridPiece(Entity<ItemComponent>.op_Implicit((entityUid, itemComponent)), location, this._entity);
        itemGridPiece1.MinSize = Vector2i.op_Implicit(vector2i);
        itemGridPiece1.Marked = this.IsMarked(entityUid);
        ItemGridPiece itemGridPiece2 = itemGridPiece1;
        itemGridPiece2.OnPiecePressed += this.OnPiecePressed;
        itemGridPiece2.OnPieceUnpressed += this.OnPieceUnpressed;
        this._controlGrid[location.Position.X + location.Position.Y * (((Box2i) ref boundingBox).Width + 1)].AddChild((Control) itemGridPiece2);
        this._pieces[entityUid] = (new ItemStorageLocation?(location), itemGridPiece2);
      }
    }
  }

  private ItemGridPieceMarks? IsMarked(EntityUid uid)
  {
    ItemGridPieceMarks? nullable;
    switch (this._contained.IndexOf(uid))
    {
      case 0:
        nullable = new ItemGridPieceMarks?(ItemGridPieceMarks.First);
        break;
      case 1:
        nullable = new ItemGridPieceMarks?(ItemGridPieceMarks.Second);
        break;
      default:
        nullable = new ItemGridPieceMarks?();
        break;
    }
    return nullable;
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    ((Control) this).FrameUpdate(args);
    if (!this.IsOpen)
      return;
    if (this._isDirty)
    {
      this._isDirty = false;
      this.BuildItemPieces();
    }
    SharedContainerSystem sharedContainerSystem = this._entity.System<SharedContainerSystem>();
    if (this._backButton != null)
    {
      if (this.StorageEntity.HasValue && this._entity.System<StorageSystem>().NestedStorage)
      {
        BaseContainer baseContainer;
        StorageComponent storageComponent;
        if (sharedContainerSystem.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(this.StorageEntity.Value), ref baseContainer) && this._entity.TryGetComponent<StorageComponent>(baseContainer.Owner, ref storageComponent) && ((BaseContainer) storageComponent.Container).Contains(this.StorageEntity.Value))
          ((Control) this._backButton).Visible = true;
        else
          ((Control) this._backButton).Visible = false;
      }
      else
        ((Control) this._backButton).Visible = false;
    }
    ItemSystem itemSystem = this._entity.System<ItemSystem>();
    StorageSystem storageSystem = this._entity.System<StorageSystem>();
    HandsSystem handsSystem = this._entity.System<HandsSystem>();
    foreach (Control child in ((Control) this._backgroundGrid).Children)
      child.ModulateSelfOverride = new Color?();
    StorageComponent storageComp;
    if (((Control) this).UserInterfaceManager.CurrentlyHovered is StorageWindow currentlyHovered && currentlyHovered != this || !this._entity.TryGetComponent<StorageComponent>(this.StorageEntity, ref storageComp))
      return;
    bool flag1 = false;
    EntityUid entityUid;
    ItemStorageLocation location1;
    if (this._storageController.IsDragging)
    {
      ItemGridPiece draggingGhost = this._storageController.DraggingGhost;
      if (draggingGhost != null)
      {
        entityUid = draggingGhost.Entity;
        location1 = draggingGhost.Location;
        goto label_24;
      }
    }
    EntityUid? activeHandEntity = handsSystem.GetActiveHandEntity();
    if (!activeHandEntity.HasValue)
      return;
    EntityUid valueOrDefault = activeHandEntity.GetValueOrDefault();
    if (!storageSystem.CanInsert(this.StorageEntity.Value, valueOrDefault, ((ISharedPlayerManager) this._player).LocalEntity, out string _, storageComp, ignoreLocation: true))
      return;
    entityUid = valueOrDefault;
    location1 = new ItemStorageLocation(this._storageController.DraggingRotation, Vector2i.Zero);
    flag1 = true;
label_24:
    ItemComponent itemComponent;
    if (!this._entity.TryGetComponent<ItemComponent>(entityUid, ref itemComponent))
      return;
    Vector2i gridPieceLocation = this.GetMouseGridPieceLocation(Entity<ItemComponent>.op_Implicit((entityUid, itemComponent)), location1);
    IReadOnlyList<Box2i> adjustedItemShape1 = itemSystem.GetAdjustedItemShape(Entity<StorageComponent>.op_Implicit((this.StorageEntity.Value, storageComp)), Entity<ItemComponent>.op_Implicit((entityUid, itemComponent)), location1.Rotation, gridPieceLocation);
    Box2i boundingBox1 = adjustedItemShape1.GetBoundingBox();
    storageSystem.ItemFitsInGridLocation(Entity<ItemComponent>.op_Implicit((entityUid, itemComponent)), Entity<StorageComponent>.op_Implicit((this.StorageEntity.Value, storageComp)), gridPieceLocation, location1.Rotation);
    foreach (KeyValuePair<string, List<ItemStorageLocation>> savedLocation in storageComp.SavedLocations)
    {
      MetaDataComponent metaDataComponent;
      if (this._entity.TryGetComponent<MetaDataComponent>(entityUid, ref metaDataComponent) && !(metaDataComponent.EntityName != savedLocation.Key))
      {
        float num = 0.0f;
        ValueList<Control> valueList = new ValueList<Control>();
        foreach (ItemStorageLocation location2 in savedLocation.Value)
        {
          IReadOnlyList<Box2i> adjustedItemShape2 = itemSystem.GetAdjustedItemShape(Entity<StorageComponent>.op_Implicit((this.StorageEntity.Value, storageComp)), Entity<ItemComponent>.op_Implicit(entityUid), location2);
          Box2i boundingBox2 = adjustedItemShape2.GetBoundingBox();
          bool flag2 = storageSystem.ItemFitsInGridLocation(Entity<ItemComponent>.op_Implicit(entityUid), Entity<StorageComponent>.op_Implicit(this.StorageEntity.Value), location2);
          if (flag2)
            ++num;
          for (int bottom = boundingBox2.Bottom; bottom <= boundingBox2.Top; ++bottom)
          {
            for (int left = boundingBox2.Left; left <= boundingBox2.Right; ++left)
            {
              Control cell;
              if (this.TryGetBackgroundCell(left, bottom, out cell) && adjustedItemShape2.Contains(left, bottom) && !valueList.Contains(cell))
              {
                valueList.Add(cell);
                cell.ModulateSelfOverride = new Color?(flag2 ? Color.FromHsv(new Vector4(0.18f, 1f / num, (float) (0.5 / (double) num + 0.5), 1f)) : Color.FromHex((ReadOnlySpan<char>) "#2222CC", new Color?()));
              }
            }
          }
        }
      }
    }
    if (!flag1)
    {
      Color.FromHex((ReadOnlySpan<char>) "#1E8000", new Color?());
    }
    else
    {
      Color goldenrod = Color.Goldenrod;
    }
    for (int bottom = boundingBox1.Bottom; bottom <= boundingBox1.Top; ++bottom)
    {
      for (int left = boundingBox1.Left; left <= boundingBox1.Right; ++left)
      {
        if (this.TryGetBackgroundCell(left, bottom, out Control _))
          adjustedItemShape1.Contains(left, bottom);
      }
    }
  }

  protected virtual BaseWindow.DragMode GetDragModeFor(Vector2 relativeMousePos)
  {
    if (this._storageController.StaticStorageUIEnabled)
      return (BaseWindow.DragMode) 0;
    UIBox2 sizeBox = ((Control) this._sidebar).SizeBox;
    return ((UIBox2) ref sizeBox).Contains(relativeMousePos - ((Control) this._sidebar).Position, true) ? (BaseWindow.DragMode) 1 : (BaseWindow.DragMode) 0;
  }

  public Vector2i GetMouseGridPieceLocation(
    Entity<ItemComponent?> entity,
    ItemStorageLocation location)
  {
    Vector2i zero = Vector2i.Zero;
    StorageComponent storageComponent;
    if (!this._entity.TryGetComponent<StorageComponent>(this.StorageEntity, ref storageComponent))
      return zero;
    Vector2i bottomLeft = storageComponent.Grid.GetBoundingBox().BottomLeft;
    Vector2 vector2 = Vector2i.op_Implicit(this._emptyTexture.Size) * 2f;
    return Vector2i.op_Addition(Vector2Helpers.Floored((((Control) this).UserInterfaceManager.MousePositionScaled.Position - ((Control) this._backgroundGrid).GlobalPosition - ItemGridPiece.GetCenterOffset(Entity<StorageComponent>.op_Implicit((this.StorageEntity.Value, storageComponent)), entity, location, this._entity) * 2f + vector2 / 2f) / vector2), bottomLeft);
  }

  public bool TryGetBackgroundCell(int x, int y, [NotNullWhen(true)] out Control? cell)
  {
    cell = (Control) null;
    StorageComponent storageComponent;
    if (!this._entity.TryGetComponent<StorageComponent>(this.StorageEntity, ref storageComponent))
      return false;
    Box2i boundingBox = storageComponent.Grid.GetBoundingBox();
    x -= boundingBox.Left;
    y -= boundingBox.Bottom;
    if (x < 0 || x >= this._backgroundGrid.Columns || y < 0 || y >= this._backgroundGrid.Rows)
      return false;
    cell = ((Control) this._backgroundGrid).GetChild(y * this._backgroundGrid.Columns + x);
    return true;
  }

  protected virtual void KeyBindDown(GUIBoundKeyEventArgs args)
  {
    base.KeyBindDown(args);
    if (!this.IsOpen)
      return;
    StorageSystem storageSystem = this._entity.System<StorageSystem>();
    HandsSystem handsSystem = this._entity.System<HandsSystem>();
    if (!BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.MoveStoredItem) || !this.StorageEntity.HasValue)
      return;
    EntityUid? activeHandEntity = handsSystem.GetActiveHandEntity();
    if (!activeHandEntity.HasValue)
      return;
    EntityUid valueOrDefault = activeHandEntity.GetValueOrDefault();
    ItemStorageLocation location;
    if (!storageSystem.CanInsert(this.StorageEntity.Value, valueOrDefault, ((ISharedPlayerManager) this._player).LocalEntity, out string _) || !CMInventoryExtensions.TryGetFirst(this.StorageEntity.Value, valueOrDefault, out location) || !storageSystem.ItemFitsInGridLocation(Entity<ItemComponent>.op_Implicit((valueOrDefault, (ItemComponent) null)), Entity<StorageComponent>.op_Implicit((this.StorageEntity.Value, (StorageComponent) null)), location))
      return;
    this._entity.RaisePredictiveEvent<StorageInsertItemIntoLocationEvent>(new StorageInsertItemIntoLocationEvent(this._entity.GetNetEntity(valueOrDefault, (MetaDataComponent) null), this._entity.GetNetEntity(this.StorageEntity.Value, (MetaDataComponent) null), location));
    this._storageController.DraggingRotation = Angle.Zero;
    ((BoundKeyEventArgs) args).Handle();
  }

  private PanelContainer? WrapBorders(Control control, int? fixedSizeX, int x, int right)
  {
    if (fixedSizeX.HasValue && x != 0)
    {
      int? nullable1;
      int? nullable2;
      if (x == right)
      {
        int num1 = x;
        nullable1 = fixedSizeX;
        nullable2 = nullable1.HasValue ? new int?(num1 % nullable1.GetValueOrDefault()) : new int?();
        int num2 = 0;
        if (!(nullable2.GetValueOrDefault() == num2 & nullable2.HasValue))
          goto label_4;
      }
      int num3 = x;
      nullable1 = fixedSizeX;
      nullable2 = nullable1.HasValue ? new int?(num3 % nullable1.GetValueOrDefault()) : new int?();
      int num4 = 0;
      Thickness thickness = nullable2.GetValueOrDefault() == num4 & nullable2.HasValue ? new Thickness(1f, 0.0f, 0.0f, 0.0f) : new Thickness(0.0f, 0.0f, 1f, 0.0f);
      PanelContainer panelContainer = new PanelContainer();
      panelContainer.PanelOverride = (StyleBox) new StyleBoxFlat()
      {
        BackgroundColor = Color.Transparent,
        BorderColor = Color.Black,
        BorderThickness = thickness
      };
      ((Control) panelContainer).AddChild(control);
      return panelContainer;
    }
label_4:
    return (PanelContainer) null;
  }
}
