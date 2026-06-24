// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.WorldEdit.WorldEditPreviewSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.WorldEdit;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.WorldEdit;

public sealed class WorldEditPreviewSystem : EntitySystem
{
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private IPrototypeManager _protoManager;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SpriteSystem _sprite;
  private bool _isPlacing;
  private Angle _rotation = Angle.Zero;
  private readonly List<EntityUid> _previewEntities = new List<EntityUid>();
  private readonly List<WorldEditPreviewEntityData> _entityData = new List<WorldEditPreviewEntityData>();
  private int _buildingWidth;
  private int _buildingHeight;

  public bool IsPlacing => this._isPlacing;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<WorldEditPreviewDataEvent>(new EntityEventHandler<WorldEditPreviewDataEvent>(this.OnPreviewData), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(EngineKeyFunctions.Use, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleUse)), true, true)).Bind(EngineKeyFunctions.UseSecondary, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleCancel)), true, true)).Register<WorldEditPreviewSystem>();
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<WorldEditPreviewSystem>();
    this.ClearPreview();
  }

  private void OnPreviewData(WorldEditPreviewDataEvent msg)
  {
    this.ClearPreview();
    this._entityData.AddRange((IEnumerable<WorldEditPreviewEntityData>) msg.Entities);
    this._buildingWidth = msg.Width;
    this._buildingHeight = msg.Height;
    this._rotation = Angle.FromDegrees((double) msg.Degrees);
    this._isPlacing = true;
    this.SpawnPreviewEntities();
  }

  private void SpawnPreviewEntities()
  {
    this.ClearPreviewEntities();
    foreach (WorldEditPreviewEntityData previewEntityData in this._entityData)
    {
      if (this._protoManager.HasIndex<EntityPrototype>(previewEntityData.PrototypeId))
      {
        EntityUid entityUid = this.Spawn(previewEntityData.PrototypeId, MapCoordinates.Nullspace, (ComponentRegistry) null, new Angle());
        this._previewEntities.Add(entityUid);
        SpriteComponent spriteComponent;
        if (this.TryComp<SpriteComponent>(entityUid, ref spriteComponent))
          this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), new Color((byte) 150, byte.MaxValue, (byte) 150, (byte) 200));
      }
    }
  }

  private void ClearPreviewEntities()
  {
    foreach (EntityUid previewEntity in this._previewEntities)
    {
      if (this.Exists(previewEntity))
        this.QueueDel(new EntityUid?(previewEntity));
    }
    this._previewEntities.Clear();
  }

  public void ClearPreview()
  {
    this.ClearPreviewEntities();
    this._entityData.Clear();
    this._isPlacing = false;
  }

  private bool HandleUse(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (!this._isPlacing || args.State != 1)
      return false;
    MapCoordinates map = this._eyeManager.PixelToMap(this._inputManager.MouseScreenPosition);
    EntityUid entityUid;
    MapGridComponent mapGridComponent;
    if (MapId.op_Equality(map.MapId, MapId.Nullspace) || !this._mapManager.TryFindGridAt(map, ref entityUid, ref mapGridComponent))
      return false;
    EntityCoordinates entityCoordinates;
    // ISSUE: explicit constructor call
    ((EntityCoordinates) ref entityCoordinates).\u002Ector(entityUid, Vector2.Transform(map.Position, this._transform.GetInvWorldMatrix(entityUid)));
    this.RaiseNetworkEvent((EntityEventArgs) new WorldEditPlacePreviewEvent(this.GetNetCoordinates(entityCoordinates, (MetaDataComponent) null), this._rotation));
    this.ClearPreview();
    return true;
  }

  private bool HandleCancel(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (!this._isPlacing || args.State != 1)
      return false;
    this.RaiseNetworkEvent((EntityEventArgs) new WorldEditCancelPreviewEvent());
    this.ClearPreview();
    return true;
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._isPlacing || this._previewEntities.Count == 0)
      return;
    ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
    if (!((ScreenCoordinates) ref mouseScreenPosition).IsValid)
      return;
    MapCoordinates map = this._eyeManager.PixelToMap(mouseScreenPosition);
    EntityUid entityUid;
    MapGridComponent mapGridComponent;
    if (MapId.op_Equality(map.MapId, MapId.Nullspace) || !this._mapManager.TryFindGridAt(map, ref entityUid, ref mapGridComponent))
      return;
    Matrix3x2 invWorldMatrix = this._transform.GetInvWorldMatrix(entityUid);
    Vector2i vector2i = Vector2Helpers.Floored(Vector2.Transform(map.Position, invWorldMatrix));
    Vector2 vector2_1 = new Vector2((float) vector2i.X, (float) vector2i.Y);
    for (int index = 0; index < this._previewEntities.Count && index < this._entityData.Count; ++index)
    {
      EntityUid previewEntity = this._previewEntities[index];
      WorldEditPreviewEntityData previewEntityData = this._entityData[index];
      if (this.Exists(previewEntity))
      {
        Vector2 vector2_2 = WorldEditPreviewSystem.RotatePosition(previewEntityData.RelativePosition, this._buildingWidth, this._buildingHeight, this._rotation) + vector2_1;
        this._transform.SetCoordinates(previewEntity, new EntityCoordinates(entityUid, vector2_2));
        this._transform.SetLocalRotation(previewEntity, Angle.op_Addition(previewEntityData.Rotation, this._rotation), (TransformComponent) null);
      }
    }
  }

  private static Vector2 RotatePosition(Vector2 pos, int width, int height, Angle rotation)
  {
    int num = (int) MathF.Round((float) ((Angle) ref rotation).Degrees) % 360;
    if (num < 0)
      num += 360;
    Vector2 vector2;
    switch (num)
    {
      case 0:
        vector2 = pos;
        break;
      case 90:
        vector2 = new Vector2((float) height - pos.Y, pos.X);
        break;
      case 180:
        vector2 = new Vector2((float) width - pos.X, (float) height - pos.Y);
        break;
      case 270:
        vector2 = new Vector2(pos.Y, (float) width - pos.X);
        break;
      default:
        vector2 = pos;
        break;
    }
    return vector2;
  }
}
