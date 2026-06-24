// Decompiled with JetBrains decompiler
// Type: Content.Client.Maps.GridDraggingSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Maps;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Maps;

public sealed class GridDraggingSystem : SharedGridDraggingSystem
{
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private InputSystem _inputSystem;
  [Dependency]
  private SharedTransformSystem _transformSystem;
  private EntityUid? _dragging;
  private Vector2 _localPosition;
  private MapCoordinates? _lastMousePosition;

  public bool Enabled { get; set; }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<GridDragToggleMessage>(new EntityEventHandler<GridDragToggleMessage>(this.OnToggleMessage), (Type[]) null, (Type[]) null);
  }

  private void OnToggleMessage(GridDragToggleMessage ev)
  {
    if (this.Enabled == ev.Enabled)
      return;
    this.Enabled = ev.Enabled;
    if (this.Enabled)
      return;
    this.StopDragging();
  }

  private void StartDragging(EntityUid grid, Vector2 localPosition)
  {
    this._dragging = new EntityUid?(grid);
    this._localPosition = localPosition;
    if (!this.HasComp<PhysicsComponent>(grid))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new GridDragVelocityRequest()
    {
      Grid = this.GetNetEntity(grid, (MetaDataComponent) null),
      LinearVelocity = Vector2.Zero
    });
  }

  private void StopDragging()
  {
    if (!this._dragging.HasValue)
      return;
    TransformComponent transformComponent;
    PhysicsComponent physicsComponent;
    if (this._lastMousePosition.HasValue && this.TryComp(this._dragging.Value, ref transformComponent) && this.TryComp<PhysicsComponent>(this._dragging.Value, ref physicsComponent) && MapId.op_Equality(transformComponent.MapID, this._lastMousePosition.Value.MapId))
    {
      TimeSpan tickPeriod = this._gameTiming.TickPeriod;
      Vector2 vector2 = this._lastMousePosition.Value.Position - this._transformSystem.GetWorldPosition(transformComponent);
      this.RaiseNetworkEvent((EntityEventArgs) new GridDragVelocityRequest()
      {
        Grid = this.GetNetEntity(this._dragging.Value, (MetaDataComponent) null),
        LinearVelocity = ((double) vector2.LengthSquared() > 0.0 ? vector2 / (float) tickPeriod.TotalSeconds * 0.25f : Vector2.Zero)
      });
    }
    this._dragging = new EntityUid?();
    this._localPosition = Vector2.Zero;
    this._lastMousePosition = new MapCoordinates?();
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this.Enabled || !this._gameTiming.IsFirstTimePredicted)
      return;
    if (this._inputSystem.CmdStates.GetState(EngineKeyFunctions.Use) != 1)
    {
      this.StopDragging();
    }
    else
    {
      MapCoordinates map = this._eyeManager.PixelToMap(this._inputManager.MouseScreenPosition);
      if (!this._dragging.HasValue)
      {
        EntityUid grid;
        MapGridComponent mapGridComponent;
        if (!this._mapManager.TryFindGridAt(map, ref grid, ref mapGridComponent))
          return;
        this.StartDragging(grid, Vector2.Transform(map.Position, this._transformSystem.GetInvWorldMatrix(grid)));
      }
      TransformComponent transformComponent;
      if (!this.TryComp(this._dragging, ref transformComponent))
        this.StopDragging();
      else if (MapId.op_Inequality(transformComponent.MapID, map.MapId))
      {
        this.StopDragging();
      }
      else
      {
        if (Vector2Helpers.EqualsApprox(Vector2.Transform(this._localPosition, this._transformSystem.GetWorldMatrix(transformComponent)), map.Position, 0.0099999997764825821))
          return;
        Vector2 position = map.Position;
        Angle worldRotation = this._transformSystem.GetWorldRotation(transformComponent);
        Vector2 vector2_1 = ((Angle) ref worldRotation).RotateVec(ref this._localPosition);
        Vector2 vector2_2 = position - vector2_1;
        this._lastMousePosition = new MapCoordinates?(new MapCoordinates(vector2_2, map.MapId));
        this.RaiseNetworkEvent((EntityEventArgs) new GridDragRequestPosition()
        {
          Grid = this.GetNetEntity(this._dragging.Value, (MetaDataComponent) null),
          WorldPosition = vector2_2
        });
      }
    }
  }
}
