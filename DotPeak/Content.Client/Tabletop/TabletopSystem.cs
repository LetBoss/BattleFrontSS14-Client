// Decompiled with JetBrains decompiler
// Type: Content.Client.Tabletop.TabletopSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Tabletop.UI;
using Content.Client.Viewport;
using Content.Shared.Tabletop;
using Content.Shared.Tabletop.Components;
using Content.Shared.Tabletop.Events;
using Robust.Client.GameObjects;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Tabletop;

public sealed class TabletopSystem : SharedTabletopSystem
{
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IUserInterfaceManager _uiManger;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private AppearanceSystem _appearance;
  [Dependency]
  private SharedTransformSystem _transformSystem;
  [Dependency]
  private SpriteSystem _sprite;
  private const float Delay = 0.1f;
  private float _timePassed;
  private EntityUid? _draggedEntity;
  private ScalingViewport? _viewport;
  private DefaultWindow? _window;
  private EntityUid? _table;

  public override void Initialize()
  {
    base.Initialize();
    this.UpdatesOutsidePrediction = true;
    // ISSUE: method pointer
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(EngineKeyFunctions.Use, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(OnUse)), false, true)).Bind(EngineKeyFunctions.UseSecondary, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(OnUseSecondary)), true, true)).Register<TabletopSystem>();
    this.SubscribeNetworkEvent<TabletopPlayEvent>(new EntityEventHandler<TabletopPlayEvent>(this.OnTabletopPlay), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TabletopDraggableComponent, ComponentRemove>(new ComponentEventHandler<TabletopDraggableComponent, ComponentRemove>((object) this, __methodptr(HandleDraggableRemoved)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TabletopDraggableComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<TabletopDraggableComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void HandleDraggableRemoved(
    EntityUid uid,
    TabletopDraggableComponent component,
    ComponentRemove args)
  {
    EntityUid? draggedEntity = this._draggedEntity;
    EntityUid entityUid = uid;
    if ((draggedEntity.HasValue ? (EntityUid.op_Equality(draggedEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this.StopDragging(false);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    if (this._window == null)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    if (!this.CanSeeTable(valueOrDefault, this._table))
    {
      this.StopDragging();
      ((BaseWindow) this._window)?.Close();
    }
    else
    {
      if (!this._draggedEntity.HasValue || this._viewport == null)
        return;
      TabletopDraggableComponent draggable;
      if (!this.CanDrag(valueOrDefault, this._draggedEntity.Value, out draggable))
      {
        this.StopDragging();
      }
      else
      {
        if (draggable.DraggingPlayer.HasValue)
        {
          NetUserId? draggingPlayer = draggable.DraggingPlayer;
          NetUserId userId = ((ISharedPlayerManager) this._playerManager).LocalSession.UserId;
          if ((draggingPlayer.HasValue ? (NetUserId.op_Inequality(draggingPlayer.GetValueOrDefault(), userId) ? 1 : 0) : 1) != 0)
          {
            this.StopDragging(false);
            return;
          }
        }
        MapCoordinates viewport = TabletopSystem.ClampPositionToViewport(this._viewport.PixelToMap(this._inputManager.MouseScreenPosition.Position), this._viewport);
        if (((MapCoordinates) ref viewport).Equals(MapCoordinates.Nullspace))
          return;
        this._transformSystem.SetWorldPosition(this._draggedEntity.Value, viewport.Position);
        this._timePassed += frameTime;
        if ((double) this._timePassed < 0.10000000149011612 || !this._table.HasValue)
          return;
        this.RaisePredictiveEvent<TabletopMoveEvent>(new TabletopMoveEvent(this.GetNetEntity(this._draggedEntity.Value, (MetaDataComponent) null), viewport, this.GetNetEntity(this._table.Value, (MetaDataComponent) null)));
        this._timePassed -= 0.1f;
      }
    }
  }

  private void OnTabletopPlay(TabletopPlayEvent msg)
  {
    ((BaseWindow) this._window)?.Close();
    this._table = new EntityUid?(this.GetEntity(msg.TableUid));
    EyeComponent eyeComponent;
    if (!this.TryComp<EyeComponent>(this.GetEntity(msg.CameraUid), ref eyeComponent))
    {
      this.Log.Error("Camera entity does not have eye component!");
    }
    else
    {
      TabletopWindow tabletopWindow = new TabletopWindow((IEye) eyeComponent.Eye, Vector2i.op_Implicit((msg.Size.X, msg.Size.Y)));
      ((Control) tabletopWindow).MinWidth = 500f;
      ((Control) tabletopWindow).MinHeight = 436f;
      tabletopWindow.Title = msg.Title;
      this._window = (DefaultWindow) tabletopWindow;
      ((BaseWindow) this._window).OnClose += new Action(this.OnWindowClose);
    }
  }

  private void OnWindowClose()
  {
    if (this._table.HasValue)
      this.RaiseNetworkEvent((EntityEventArgs) new TabletopStopPlayingEvent(this.GetNetEntity(this._table.Value, (MetaDataComponent) null)));
    this.StopDragging();
    this._window = (DefaultWindow) null;
  }

  private bool OnUse(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (!this._gameTiming.IsFirstTimePredicted)
      return false;
    BoundKeyState state = args.State;
    return state == null ? this.OnMouseUp(in args) : state == 1 && this.OnMouseDown(in args);
  }

  private bool OnUseSecondary(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (this._draggedEntity.HasValue && this._table.HasValue)
      this.RaiseNetworkEvent((EntityEventArgs) new SharedTabletopSystem.TabletopRequestTakeOut()
      {
        Entity = this.GetNetEntity(this._draggedEntity.Value, (MetaDataComponent) null),
        TableUid = this.GetNetEntity(this._table.Value, (MetaDataComponent) null)
      });
    return false;
  }

  private bool OnMouseDown(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
      return false;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    EntityUid entityUid = args.EntityUid;
    if (!this.CanSeeTable(valueOrDefault, this._table) || !this.CanDrag(valueOrDefault, entityUid, out TabletopDraggableComponent _) || !(this._uiManger.MouseGetControl(args.ScreenCoordinates) is ScalingViewport control))
      return false;
    this.StartDragging(entityUid, control);
    return true;
  }

  private bool OnMouseUp(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    this.StopDragging();
    return false;
  }

  private void OnAppearanceChange(
    EntityUid uid,
    TabletopDraggableComponent comp,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    Vector2 vector2;
    if (((SharedAppearanceSystem) this._appearance).TryGetData<Vector2>(uid, (Enum) TabletopItemVisuals.Scale, ref vector2, args.Component))
      this._sprite.SetScale(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), vector2);
    int num;
    if (!((SharedAppearanceSystem) this._appearance).TryGetData<int>(uid, (Enum) TabletopItemVisuals.DrawDepth, ref num, args.Component))
      return;
    this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num);
  }

  private void StartDragging(EntityUid draggedEntity, ScalingViewport viewport)
  {
    this.RaisePredictiveEvent<TabletopDraggingPlayerChangedEvent>(new TabletopDraggingPlayerChangedEvent(this.GetNetEntity(draggedEntity, (MetaDataComponent) null), true));
    this._draggedEntity = new EntityUid?(draggedEntity);
    this._viewport = viewport;
  }

  private void StopDragging(bool broadcast = true)
  {
    if (broadcast && this._draggedEntity.HasValue && this.HasComp<TabletopDraggableComponent>(this._draggedEntity.Value))
    {
      this.RaisePredictiveEvent<TabletopMoveEvent>(new TabletopMoveEvent(this.GetNetEntity(this._draggedEntity.Value, (MetaDataComponent) null), this.Transforms.GetMapCoordinates(this._draggedEntity.Value, (TransformComponent) null), this.GetNetEntity(this._table.Value, (MetaDataComponent) null)));
      this.RaisePredictiveEvent<TabletopDraggingPlayerChangedEvent>(new TabletopDraggingPlayerChangedEvent(this.GetNetEntity(this._draggedEntity.Value, (MetaDataComponent) null), false));
    }
    this._draggedEntity = new EntityUid?();
    this._viewport = (ScalingViewport) null;
  }

  private static MapCoordinates ClampPositionToViewport(
    MapCoordinates coordinates,
    ScalingViewport viewport)
  {
    if (MapCoordinates.op_Equality(coordinates, MapCoordinates.Nullspace))
      return MapCoordinates.Nullspace;
    IEye eye = viewport.Eye;
    if (eye == null)
      return MapCoordinates.Nullspace;
    Vector2 vector2 = Vector2i.op_Implicit(viewport.ViewportSize) / 32f;
    Vector2 position = eye.Position.Position;
    Angle rotation = eye.Rotation;
    Vector2 scale = eye.Scale;
    Vector2 min = (position - vector2 / 2f) / scale;
    Vector2 max = (position + vector2 / 2f) / scale;
    if (MathHelper.CloseToPercent(((Angle) ref rotation).Degrees % 180.0, 90.0, 1E-05) || MathHelper.CloseToPercent(((Angle) ref rotation).Degrees % 180.0, -90.0, 1E-05))
    {
      ref float local1 = ref min.Y;
      ref float local2 = ref min.X;
      float x1 = min.X;
      float y1 = min.Y;
      local1 = x1;
      double num1 = (double) y1;
      local2 = (float) num1;
      ref float local3 = ref max.Y;
      ref float local4 = ref max.X;
      float x2 = max.X;
      float y2 = max.Y;
      local3 = x2;
      double num2 = (double) y2;
      local4 = (float) num2;
    }
    return new MapCoordinates(Vector2.Clamp(coordinates.Position, min, max), eye.Position.MapId);
  }
}
