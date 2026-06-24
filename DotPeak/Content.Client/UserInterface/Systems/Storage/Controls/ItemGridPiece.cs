// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Storage.Controls.ItemGridPiece
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Storage;
using Content.Client.Items.Systems;
using Content.Shared.Item;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Systems.Storage.Controls;

public sealed class ItemGridPiece : Control, IEntityControl
{
  private readonly IEntityManager _entityManager;
  private readonly StorageUIController _storageController;
  private readonly List<(Texture, Vector2)> _texturesPositions = new List<(Texture, Vector2)>();
  public readonly EntityUid Entity;
  public ItemStorageLocation Location;
  public ItemGridPieceMarks? Marked;
  private readonly string _centerTexturePath = "Storage/piece_center";
  private Texture? _centerTexture;
  private readonly string _topTexturePath = "Storage/piece_top";
  private Texture? _topTexture;
  private readonly string _bottomTexturePath = "Storage/piece_bottom";
  private Texture? _bottomTexture;
  private readonly string _leftTexturePath = "Storage/piece_left";
  private Texture? _leftTexture;
  private readonly string _rightTexturePath = "Storage/piece_right";
  private Texture? _rightTexture;
  private readonly string _topLeftTexturePath = "Storage/piece_topLeft";
  private Texture? _topLeftTexture;
  private readonly string _topRightTexturePath = "Storage/piece_topRight";
  private Texture? _topRightTexture;
  private readonly string _bottomLeftTexturePath = "Storage/piece_bottomLeft";
  private Texture? _bottomLeftTexture;
  private readonly string _bottomRightTexturePath = "Storage/piece_bottomRight";
  private Texture? _bottomRightTexture;
  private readonly string _markedFirstTexturePath = "Storage/marked_first";
  private Texture? _markedFirstTexture;
  private readonly string _markedSecondTexturePath = "Storage/marked_second";
  private Texture? _markedSecondTexture;

  public event Action<GUIBoundKeyEventArgs, ItemGridPiece>? OnPiecePressed;

  public event Action<GUIBoundKeyEventArgs, ItemGridPiece>? OnPieceUnpressed;

  public ItemGridPiece(
    Robust.Shared.GameObjects.Entity<ItemComponent> entity,
    ItemStorageLocation location,
    IEntityManager entityManager)
  {
    IoCManager.InjectDependencies<ItemGridPiece>(this);
    this._entityManager = entityManager;
    this._storageController = this.UserInterfaceManager.GetUIController<StorageUIController>();
    this.Entity = entity.Owner;
    this.Location = location;
    this.Visible = true;
    this.MouseFilter = (Control.MouseFilterMode) 0;
    // ISSUE: method pointer
    this.TooltipSupplier = new TooltipSupplier((object) this, __methodptr(SupplyTooltip));
    base.OnThemeUpdated();
  }

  private Control? SupplyTooltip(Control sender)
  {
    if (this._storageController.IsDragging)
      return (Control) null;
    return (Control) new Tooltip()
    {
      Text = this._entityManager.GetComponent<MetaDataComponent>(this.Entity).EntityName
    };
  }

  protected virtual void OnThemeUpdated()
  {
    base.OnThemeUpdated();
    this._centerTexture = this.Theme.ResolveTextureOrNull(this._centerTexturePath)?.Texture;
    this._topTexture = this.Theme.ResolveTextureOrNull(this._topTexturePath)?.Texture;
    this._bottomTexture = this.Theme.ResolveTextureOrNull(this._bottomTexturePath)?.Texture;
    this._leftTexture = this.Theme.ResolveTextureOrNull(this._leftTexturePath)?.Texture;
    this._rightTexture = this.Theme.ResolveTextureOrNull(this._rightTexturePath)?.Texture;
    this._topLeftTexture = this.Theme.ResolveTextureOrNull(this._topLeftTexturePath)?.Texture;
    this._topRightTexture = this.Theme.ResolveTextureOrNull(this._topRightTexturePath)?.Texture;
    this._bottomLeftTexture = this.Theme.ResolveTextureOrNull(this._bottomLeftTexturePath)?.Texture;
    this._bottomRightTexture = this.Theme.ResolveTextureOrNull(this._bottomRightTexturePath)?.Texture;
    this._markedFirstTexture = this.Theme.ResolveTextureOrNull(this._markedFirstTexturePath)?.Texture;
    this._markedSecondTexture = this.Theme.ResolveTextureOrNull(this._markedSecondTexturePath)?.Texture;
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    base.Draw(handle);
    ItemComponent itemComponent;
    if (!this._entityManager.EntityExists(this.Entity) || !this._entityManager.TryGetComponent<ItemComponent>(this.Entity, ref itemComponent))
    {
      this.Orphan();
    }
    else
    {
      if (this._storageController.IsDragging)
      {
        EntityUid? entity1 = this._storageController.DraggingGhost?.Entity;
        EntityUid entity2 = this.Entity;
        if ((entity1.HasValue ? (EntityUid.op_Equality(entity1.GetValueOrDefault(), entity2) ? 1 : 0) : 0) != 0 && this._storageController.DraggingGhost != this)
          return;
      }
      BaseContainer baseContainer;
      StorageComponent storageComponent;
      if (!((SharedContainerSystem) this._entityManager.System<ContainerSystem>()).TryGetContainingContainer(Robust.Shared.GameObjects.Entity<TransformComponent, MetaDataComponent>.op_Implicit((this.Entity, (TransformComponent) null)), ref baseContainer) || !this._entityManager.TryGetComponent<StorageComponent>(baseContainer.Owner, ref storageComponent))
        return;
      IReadOnlyList<Box2i> adjustedItemShape = this._entityManager.System<ItemSystem>().GetAdjustedItemShape(Robust.Shared.GameObjects.Entity<StorageComponent>.op_Implicit((baseContainer.Owner, storageComponent)), Robust.Shared.GameObjects.Entity<ItemComponent>.op_Implicit((this.Entity, itemComponent)), this.Location.Rotation, Vector2i.Zero);
      Box2i boundingBox = adjustedItemShape.GetBoundingBox();
      Vector2 vector2_1 = Vector2i.op_Multiply(Vector2i.op_Multiply(this._centerTexture.Size, 2), this.UIScale);
      Color? nullable1 = (this._storageController.IsDragging ? 0 : (this.UserInterfaceManager.CurrentlyHovered == this ? 1 : 0)) != 0 ? new Color?() : new Color?(Color.FromHex((ReadOnlySpan<char>) "#a8a8a8", new Color?()));
      bool flag = this.Marked.HasValue;
      Vector2i? nullable2 = new Vector2i?();
      this._texturesPositions.Clear();
      for (int bottom = boundingBox.Bottom; bottom <= boundingBox.Top; ++bottom)
      {
        for (int left = boundingBox.Left; left <= boundingBox.Right; ++left)
        {
          if (adjustedItemShape.Contains(left, bottom))
          {
            Vector2 vector2_2 = vector2_1 * 2f * new Vector2((float) (left - boundingBox.Left), (float) (bottom - boundingBox.Bottom));
            Vector2i vector2i = Vector2i.op_Addition(this.PixelPosition, Vector2Helpers.Floored(vector2_2));
            Texture texture1 = this.GetTexture(adjustedItemShape, new Vector2i(left, bottom), (Direction) 3);
            if (texture1 != null)
            {
              Vector2 vector2_3 = new Vector2(vector2_1.X, 0.0f);
              handle.DrawTextureRect(texture1, new UIBox2(Vector2i.op_Implicit(vector2i) + vector2_3, Vector2i.op_Implicit(vector2i) + vector2_3 + vector2_1), nullable1);
            }
            Texture texture2 = this.GetTexture(adjustedItemShape, new Vector2i(left, bottom), (Direction) 5);
            if (texture2 != null)
            {
              this._texturesPositions.Add((texture2, this.Position + vector2_2 / this.UIScale));
              handle.DrawTextureRect(texture2, new UIBox2(Vector2i.op_Implicit(vector2i), Vector2i.op_Implicit(vector2i) + vector2_1), nullable1);
              if (flag && texture2 == this._topLeftTexture)
              {
                nullable2 = new Vector2i?(vector2i);
                flag = false;
              }
            }
            Texture texture3 = this.GetTexture(adjustedItemShape, new Vector2i(left, bottom), (Direction) 1);
            if (texture3 != null)
            {
              Vector2 vector2_4 = vector2_1;
              handle.DrawTextureRect(texture3, new UIBox2(Vector2i.op_Implicit(vector2i) + vector2_4, Vector2i.op_Implicit(vector2i) + vector2_4 + vector2_1), nullable1);
            }
            Texture texture4 = this.GetTexture(adjustedItemShape, new Vector2i(left, bottom), (Direction) 7);
            if (texture4 != null)
            {
              Vector2 vector2_5 = new Vector2(0.0f, vector2_1.Y);
              handle.DrawTextureRect(texture4, new UIBox2(Vector2i.op_Implicit(vector2i) + vector2_5, Vector2i.op_Implicit(vector2i) + vector2_5 + vector2_1), nullable1);
            }
          }
        }
      }
      Angle rotation = this.Location.Rotation;
      ref Angle local1 = ref rotation;
      Vector2 vector2_6 = Vector2i.op_Implicit(itemComponent.StoredOffset);
      ref Vector2 local2 = ref vector2_6;
      Vector2 vector2_7 = ((Angle) ref local1).RotateVec(ref local2) * 2f * this.UIScale;
      Vector2 iconPosition = new Vector2((float) (((Box2i) ref boundingBox).Width + 1) * vector2_1.X + vector2_7.X, (float) (((Box2i) ref boundingBox).Height + 1) * vector2_1.Y + vector2_7.Y);
      Angle angle = Angle.op_Addition(this.Location.Rotation, Angle.FromDegrees((double) itemComponent.StoredRotation));
      SpriteSpecifier storedSprite = itemComponent.StoredSprite;
      if (storedSprite != null)
      {
        float num = 2f * this.UIScale;
        Texture texture = this._entityManager.System<SpriteSystem>().Frame0(storedSprite);
        Vector2 vector2_8 = Vector2i.op_Multiply(Vector2i.op_Subtraction(Vector2i.op_Multiply(Vector2i.op_Multiply(Vector2i.op_Addition(((Box2i) ref boundingBox).Size, Vector2i.One), this._centerTexture.Size), 2), texture.Size), this.UIScale);
        Box2Rotated box2Rotated;
        // ISSUE: explicit constructor call
        ((Box2Rotated) ref box2Rotated).\u002Ector(new Box2(0.0f, (float) texture.Height * num, (float) texture.Width * num, 0.0f), Angle.op_UnaryNegation(angle), Vector2.Zero);
        Vector2 bottomLeft = ((Box2Rotated) ref box2Rotated).CalcBoundingBox().BottomLeft;
        Vector2 vector2_9 = Vector2i.op_Implicit(Vector2i.op_Multiply(this.PixelPosition, 2));
        Vector2i? globalPixelPosition = this.Parent?.GlobalPixelPosition;
        Vector2 vector2_10 = globalPixelPosition.HasValue ? Vector2i.op_Implicit(globalPixelPosition.GetValueOrDefault()) : Vector2.Zero;
        Vector2 vector2_11 = vector2_9 + vector2_10 + vector2_8 + vector2_7;
        ((DrawingHandleBase) handle).SetTransform(ref vector2_11, ref angle);
        UIBox2 uiBox2;
        // ISSUE: explicit constructor call
        ((UIBox2) ref uiBox2).\u002Ector(bottomLeft, bottomLeft + Vector2i.op_Multiply(texture.Size, num));
        handle.DrawTextureRect(texture, uiBox2, new Color?());
        DrawingHandleScreen drawingHandleScreen = handle;
        Vector2 vector2_12 = Vector2i.op_Implicit(this.GlobalPixelPosition);
        ref Vector2 local3 = ref vector2_12;
        Angle zero = Angle.Zero;
        ref Angle local4 = ref zero;
        ((DrawingHandleBase) drawingHandleScreen).SetTransform(ref local3, ref local4);
      }
      else
      {
        this._entityManager.System<SpriteSystem>().ForceUpdate(this.Entity);
        handle.DrawEntity(this.Entity, Vector2i.op_Implicit(this.PixelPosition) + iconPosition, Vector2.One * 2f * this.UIScale, new Angle?(Angle.Zero), angle, new Direction?((Direction) 0), (SpriteComponent) null, (TransformComponent) null, (SharedTransformSystem) null);
      }
      if (nullable2.HasValue)
      {
        Vector2i valueOrDefault = nullable2.GetValueOrDefault();
        ItemGridPieceMarks? marked = this.Marked;
        Texture texture5;
        if (marked.HasValue)
        {
          switch (marked.GetValueOrDefault())
          {
            case ItemGridPieceMarks.First:
              texture5 = this._markedFirstTexture;
              goto label_32;
            case ItemGridPieceMarks.Second:
              texture5 = this._markedSecondTexture;
              goto label_32;
          }
        }
        texture5 = (Texture) null;
label_32:
        Texture texture6 = texture5;
        if (texture6 != null)
          handle.DrawTextureRect(texture6, new UIBox2(Vector2i.op_Implicit(valueOrDefault), Vector2i.op_Implicit(valueOrDefault) + vector2_1), new Color?());
      }
      this._entityManager.System<RMCIconLabelSystem>().DrawStorage(this.Entity, this.UIScale, iconPosition, handle);
    }
  }

  protected virtual bool HasPoint(Vector2 point)
  {
    foreach ((Texture texture, Vector2 vector2) in this._texturesPositions)
    {
      Box2 box2 = new Box2(vector2, vector2 + Vector2i.op_Implicit(Vector2i.op_Multiply(texture.Size, 4)));
      if (((Box2) ref box2).Contains(point, true))
        return true;
    }
    return false;
  }

  public bool Has(Vector2 point) => base.HasPoint(point);

  protected virtual void KeyBindDown(GUIBoundKeyEventArgs args)
  {
    base.KeyBindDown(args);
    Action<GUIBoundKeyEventArgs, ItemGridPiece> onPiecePressed = this.OnPiecePressed;
    if (onPiecePressed == null)
      return;
    onPiecePressed(args, this);
  }

  protected virtual void KeyBindUp(GUIBoundKeyEventArgs args)
  {
    base.KeyBindUp(args);
    Action<GUIBoundKeyEventArgs, ItemGridPiece> onPieceUnpressed = this.OnPieceUnpressed;
    if (onPieceUnpressed == null)
      return;
    onPieceUnpressed(args, this);
  }

  private Texture? GetTexture(IReadOnlyList<Box2i> boxes, Vector2i position, Direction corner)
  {
    bool flag1 = !boxes.Contains(Vector2i.op_Subtraction(position, Vector2i.Up));
    bool flag2 = !boxes.Contains(Vector2i.op_Subtraction(position, Vector2i.Down));
    bool flag3 = !boxes.Contains(Vector2i.op_Addition(position, Vector2i.Left));
    bool flag4 = !boxes.Contains(Vector2i.op_Addition(position, Vector2i.Right));
    switch (corner - 1)
    {
      case 0:
        if (flag2 & flag4)
          return this._bottomRightTexture;
        if (flag2)
          return this._bottomTexture;
        return flag4 ? this._rightTexture : this._centerTexture;
      case 2:
        if (flag1 & flag4)
          return this._topRightTexture;
        if (flag1)
          return this._topTexture;
        return flag4 ? this._rightTexture : this._centerTexture;
      case 4:
        if (flag1 & flag3)
          return this._topLeftTexture;
        if (flag1)
          return this._topTexture;
        return flag3 ? this._leftTexture : this._centerTexture;
      case 6:
        if (flag2 & flag3)
          return this._bottomLeftTexture;
        if (flag2)
          return this._bottomTexture;
        return flag3 ? this._leftTexture : this._centerTexture;
      default:
        return (Texture) null;
    }
  }

  public static Vector2 GetCenterOffset(
    Robust.Shared.GameObjects.Entity<StorageComponent?> storage,
    Robust.Shared.GameObjects.Entity<ItemComponent?> entity,
    ItemStorageLocation location,
    IEntityManager entMan)
  {
    Box2i boundingBox = entMan.System<ItemSystem>().GetAdjustedItemShape(storage, entity, location).GetBoundingBox();
    Vector2i size = ((Box2i) ref boundingBox).Size;
    return new Vector2((float) (size.X + 1), (float) (size.Y + 1)) * Vector2i.op_Implicit(new Vector2i(8, 8));
  }

  public EntityUid? UiEntity => new EntityUid?(this.Entity);
}
