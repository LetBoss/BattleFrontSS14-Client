using System;
using System.Numerics;
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

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<GridDragToggleMessage>((EntityEventHandler<GridDragToggleMessage>)OnToggleMessage, (Type[])null, (Type[])null);
	}

	private void OnToggleMessage(GridDragToggleMessage ev)
	{
		if (Enabled != ev.Enabled)
		{
			Enabled = ev.Enabled;
			if (!Enabled)
			{
				StopDragging();
			}
		}
	}

	private void StartDragging(EntityUid grid, Vector2 localPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		_dragging = grid;
		_localPosition = localPosition;
		if (((EntitySystem)this).HasComp<PhysicsComponent>(grid))
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new GridDragVelocityRequest
			{
				Grid = ((EntitySystem)this).GetNetEntity(grid, (MetaDataComponent)null),
				LinearVelocity = Vector2.Zero
			});
		}
	}

	private void StopDragging()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if (_dragging.HasValue)
		{
			TransformComponent val = default(TransformComponent);
			PhysicsComponent val2 = default(PhysicsComponent);
			if (_lastMousePosition.HasValue && ((EntitySystem)this).TryComp(_dragging.Value, ref val) && ((EntitySystem)this).TryComp<PhysicsComponent>(_dragging.Value, ref val2) && val.MapID == _lastMousePosition.Value.MapId)
			{
				TimeSpan tickPeriod = _gameTiming.TickPeriod;
				Vector2 vector = _lastMousePosition.Value.Position - _transformSystem.GetWorldPosition(val);
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new GridDragVelocityRequest
				{
					Grid = ((EntitySystem)this).GetNetEntity(_dragging.Value, (MetaDataComponent)null),
					LinearVelocity = ((vector.LengthSquared() > 0f) ? (vector / (float)tickPeriod.TotalSeconds * 0.25f) : Vector2.Zero)
				});
			}
			_dragging = null;
			_localPosition = Vector2.Zero;
			_lastMousePosition = null;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (!Enabled || !_gameTiming.IsFirstTimePredicted)
		{
			return;
		}
		if ((int)_inputSystem.CmdStates.GetState(EngineKeyFunctions.Use) != 1)
		{
			StopDragging();
			return;
		}
		ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
		MapCoordinates val = _eyeManager.PixelToMap(mouseScreenPosition);
		if (!_dragging.HasValue)
		{
			EntityUid val2 = default(EntityUid);
			MapGridComponent val3 = default(MapGridComponent);
			if (!_mapManager.TryFindGridAt(val, ref val2, ref val3))
			{
				return;
			}
			StartDragging(val2, Vector2.Transform(val.Position, _transformSystem.GetInvWorldMatrix(val2)));
		}
		TransformComponent val4 = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(_dragging, ref val4))
		{
			StopDragging();
		}
		else if (val4.MapID != val.MapId)
		{
			StopDragging();
		}
		else if (!Vector2Helpers.EqualsApprox(Vector2.Transform(_localPosition, _transformSystem.GetWorldMatrix(val4)), val.Position, 0.009999999776482582))
		{
			Vector2 position = val.Position;
			Angle worldRotation = _transformSystem.GetWorldRotation(val4);
			Vector2 vector = position - ((Angle)(ref worldRotation)).RotateVec(ref _localPosition);
			_lastMousePosition = new MapCoordinates(vector, val.MapId);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new GridDragRequestPosition
			{
				Grid = ((EntitySystem)this).GetNetEntity(_dragging.Value, (MetaDataComponent)null),
				WorldPosition = vector
			});
		}
	}
}
