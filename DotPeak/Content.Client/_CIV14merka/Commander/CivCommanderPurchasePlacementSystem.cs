// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivCommanderPurchasePlacementSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.GlobalMap;
using Content.Client.ContextMenu.UI;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderPurchasePlacementSystem : EntitySystem
{
  [Dependency]
  private IEyeManager _eye;
  [Dependency]
  private IInputManager _input;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IUserInterfaceManager _ui;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private CivGlobalMapSystem _globalMap;
  private string? _entryId;
  private string? _displayName;
  private EntProtoId? _prototypeId;
  private int _price;
  private bool _keepPlacing;
  private EntityUid? _preview;
  private bool _validPlacement;
  private EntityCoordinates _previewCoordinates = EntityCoordinates.Invalid;
  private MapCoordinates _previewMapCoordinates = MapCoordinates.Nullspace;

  public bool IsActive => this._entryId != null && this._prototypeId.HasValue;

  public string? ActiveName => this._displayName;

  public virtual void Initialize()
  {
    base.Initialize();
    this.UpdatesOutsidePrediction = true;
    // ISSUE: method pointer
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(EngineKeyFunctions.Use, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleUse)), true, true)).BindBefore(EngineKeyFunctions.UseSecondary, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleCancel)), true, true), new Type[1]
    {
      typeof (EntityMenuUIController)
    }).Register<CivCommanderPurchasePlacementSystem>();
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<CivCommanderPurchasePlacementSystem>();
    this.CancelPlacement();
  }

  public void BeginPlacement(
    string entryId,
    string displayName,
    EntProtoId prototypeId,
    int price,
    bool keepPlacing)
  {
    EntityPrototype entityPrototype;
    if (!this._prototypeManager.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(prototypeId), ref entityPrototype))
      return;
    this.CancelPlacement();
    this._entryId = entryId;
    this._displayName = displayName;
    this._prototypeId = new EntProtoId?(prototypeId);
    this._price = price;
    this._keepPlacing = keepPlacing;
    this.EnsurePreview();
    this.UpdatePreview();
  }

  public void CancelPlacement()
  {
    this._entryId = (string) null;
    this._displayName = (string) null;
    this._prototypeId = new EntProtoId?();
    this._price = 0;
    this._keepPlacing = false;
    this._validPlacement = false;
    this._previewCoordinates = EntityCoordinates.Invalid;
    this._previewMapCoordinates = MapCoordinates.Nullspace;
    EntityUid? preview = this._preview;
    if (preview.HasValue)
    {
      EntityUid valueOrDefault = preview.GetValueOrDefault();
      if (this.Exists(valueOrDefault))
        this.QueueDel(new EntityUid?(valueOrDefault));
    }
    this._preview = new EntityUid?();
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this.IsActive)
      return;
    if (!this.IsCommander())
    {
      this.CancelPlacement();
    }
    else
    {
      this.EnsurePreview();
      this.UpdatePreview();
    }
  }

  private bool HandleUse(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (!this.IsActive || args.State != 1 || this._ui.CurrentlyHovered != null && !(this._ui.CurrentlyHovered is IViewportControl))
      return false;
    this.UpdatePreview();
    if (!this._validPlacement || this._entryId == null)
      return true;
    this._globalMap.RequestCommanderPlacePurchase(this._entryId, this._previewMapCoordinates.MapId, this._previewMapCoordinates.Position);
    if (!this._keepPlacing)
      this.CancelPlacement();
    return true;
  }

  private bool HandleCancel(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (!this.IsActive || args.State != 1)
      return false;
    this.CancelPlacement();
    return true;
  }

  private void EnsurePreview()
  {
    if (this._preview.HasValue || !this._prototypeId.HasValue)
      return;
    EntProtoId? prototypeId = this._prototypeId;
    this._preview = new EntityUid?(this.Spawn(prototypeId.HasValue ? EntProtoId.op_Implicit(prototypeId.GetValueOrDefault()) : (string) null, MapCoordinates.Nullspace, (ComponentRegistry) null, new Angle()));
  }

  private void UpdatePreview()
  {
    EntityUid? preview = this._preview;
    if (!preview.HasValue)
      return;
    EntityUid valueOrDefault = preview.GetValueOrDefault();
    if (!this.Exists(valueOrDefault) || !this.IsActive)
      return;
    this._validPlacement = false;
    this._previewCoordinates = EntityCoordinates.Invalid;
    this._previewMapCoordinates = MapCoordinates.Nullspace;
    ScreenCoordinates mouseScreenPosition = this._input.MouseScreenPosition;
    if (!((ScreenCoordinates) ref mouseScreenPosition).IsValid)
    {
      this._transform.SetCoordinates(valueOrDefault, EntityCoordinates.Invalid);
    }
    else
    {
      MapCoordinates map = this._eye.PixelToMap(mouseScreenPosition);
      EntityUid entityUid;
      MapGridComponent mapGridComponent;
      if (MapId.op_Equality(map.MapId, MapId.Nullspace) || !this._mapManager.TryFindGridAt(map, ref entityUid, ref mapGridComponent))
      {
        this._transform.SetCoordinates(valueOrDefault, EntityCoordinates.Invalid);
      }
      else
      {
        Vector2i vector2i = Vector2Helpers.Floored(Vector2.Transform(map.Position, this._transform.GetInvWorldMatrix(entityUid)));
        EntityCoordinates entityCoordinates;
        // ISSUE: explicit constructor call
        ((EntityCoordinates) ref entityCoordinates).\u002Ector(entityUid, Vector2i.op_Implicit(vector2i) + new Vector2(0.5f, 0.5f));
        MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(entityCoordinates, true);
        this._previewCoordinates = entityCoordinates;
        this._previewMapCoordinates = mapCoordinates;
        this._validPlacement = this.IsValidPlacement(mapCoordinates);
        this._transform.SetCoordinates(valueOrDefault, entityCoordinates);
        this.SetPreviewColor(valueOrDefault, this._validPlacement);
      }
    }
  }

  private bool IsValidPlacement(MapCoordinates coordinates)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CivTeamMemberComponent teamMemberComponent;
    if (!localEntity.HasValue || !this.TryComp<CivTeamMemberComponent>(localEntity.GetValueOrDefault(), ref teamMemberComponent) || !teamMemberComponent.IsCommander)
      return false;
    int price;
    if (this._entryId != null && this._globalMap.TryGetCommanderShopEntryPrice(this._entryId, out price))
      this._price = price;
    if (this._globalMap.GetCommanderCurrency() < this._price)
      return false;
    EntityQueryEnumerator<CivCommanderPurchaseAnchorComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CivCommanderPurchaseAnchorComponent, TransformComponent>();
    EntityUid entityUid;
    CivCommanderPurchaseAnchorComponent purchaseAnchorComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref purchaseAnchorComponent, ref transformComponent))
    {
      if (string.Equals(purchaseAnchorComponent.RequiredSideId, teamMemberComponent.SideId, StringComparison.OrdinalIgnoreCase) && !MapId.op_Inequality(transformComponent.MapID, coordinates.MapId) && (double) Vector2.Distance(this._transform.ToMapCoordinates(transformComponent.Coordinates, true).Position, coordinates.Position) <= (double) purchaseAnchorComponent.Range)
        return true;
    }
    return false;
  }

  private void SetPreviewColor(EntityUid uid, bool valid)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), valid ? new Color((byte) 90, (byte) 220, (byte) 120, (byte) 190) : new Color((byte) 220, (byte) 90, (byte) 90, (byte) 170));
  }

  private bool IsCommander()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CivTeamMemberComponent teamMemberComponent;
    return localEntity.HasValue && this.TryComp<CivTeamMemberComponent>(localEntity.GetValueOrDefault(), ref teamMemberComponent) && teamMemberComponent.IsCommander;
  }
}
