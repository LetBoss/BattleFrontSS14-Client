using System;
using System.Numerics;
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
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		base.Initialize();
		((EntitySystem)this).UpdatesOutsidePrediction = true;
		CommandBinds.Builder.Bind(EngineKeyFunctions.Use, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(OnUse), false, true)).Bind(EngineKeyFunctions.UseSecondary, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(OnUseSecondary), true, true)).Register<TabletopSystem>();
		((EntitySystem)this).SubscribeNetworkEvent<TabletopPlayEvent>((EntityEventHandler<TabletopPlayEvent>)OnTabletopPlay, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TabletopDraggableComponent, ComponentRemove>((ComponentEventHandler<TabletopDraggableComponent, ComponentRemove>)HandleDraggableRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TabletopDraggableComponent, AppearanceChangeEvent>((ComponentEventRefHandler<TabletopDraggableComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void HandleDraggableRemoved(EntityUid uid, TabletopDraggableComponent component, ComponentRemove args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? draggedEntity = _draggedEntity;
		if (draggedEntity.HasValue && draggedEntity.GetValueOrDefault() == uid)
		{
			StopDragging(broadcast: false);
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		if (!CanSeeTable(valueOrDefault, _table))
		{
			StopDragging();
			DefaultWindow? window = _window;
			if (window != null)
			{
				((BaseWindow)window).Close();
			}
		}
		else
		{
			if (!_draggedEntity.HasValue || _viewport == null)
			{
				return;
			}
			if (!CanDrag(valueOrDefault, _draggedEntity.Value, out TabletopDraggableComponent draggable))
			{
				StopDragging();
				return;
			}
			if (draggable.DraggingPlayer.HasValue)
			{
				NetUserId? draggingPlayer = draggable.DraggingPlayer;
				NetUserId userId = ((ISharedPlayerManager)_playerManager).LocalSession.UserId;
				if (!draggingPlayer.HasValue || draggingPlayer.GetValueOrDefault() != userId)
				{
					StopDragging(broadcast: false);
					return;
				}
			}
			MapCoordinates val = ClampPositionToViewport(_viewport.PixelToMap(_inputManager.MouseScreenPosition.Position), _viewport);
			if (!((MapCoordinates)(ref val)).Equals(MapCoordinates.Nullspace))
			{
				_transformSystem.SetWorldPosition(_draggedEntity.Value, val.Position);
				_timePassed += frameTime;
				if (_timePassed >= 0.1f && _table.HasValue)
				{
					((EntitySystem)this).RaisePredictiveEvent<TabletopMoveEvent>(new TabletopMoveEvent(((EntitySystem)this).GetNetEntity(_draggedEntity.Value, (MetaDataComponent)null), val, ((EntitySystem)this).GetNetEntity(_table.Value, (MetaDataComponent)null)));
					_timePassed -= 0.1f;
				}
			}
		}
	}

	private void OnTabletopPlay(TabletopPlayEvent msg)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		DefaultWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
		_table = ((EntitySystem)this).GetEntity(msg.TableUid);
		EntityUid entity = ((EntitySystem)this).GetEntity(msg.CameraUid);
		EyeComponent val = default(EyeComponent);
		if (!((EntitySystem)this).TryComp<EyeComponent>(entity, ref val))
		{
			((EntitySystem)this).Log.Error("Camera entity does not have eye component!");
			return;
		}
		TabletopWindow tabletopWindow = new TabletopWindow((IEye?)(object)val.Eye, Vector2i.op_Implicit((msg.Size.X, msg.Size.Y)));
		((Control)tabletopWindow).MinWidth = 500f;
		((Control)tabletopWindow).MinHeight = 436f;
		((DefaultWindow)tabletopWindow).Title = msg.Title;
		_window = (DefaultWindow?)(object)tabletopWindow;
		((BaseWindow)_window).OnClose += OnWindowClose;
	}

	private void OnWindowClose()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (_table.HasValue)
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new TabletopStopPlayingEvent(((EntitySystem)this).GetNetEntity(_table.Value, (MetaDataComponent)null)));
		}
		StopDragging();
		_window = null;
	}

	private bool OnUse(in PointerInputCmdArgs args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		if (!_gameTiming.IsFirstTimePredicted)
		{
			return false;
		}
		BoundKeyState state = args.State;
		if ((int)state != 0)
		{
			if ((int)state == 1)
			{
				return OnMouseDown(in args);
			}
			return false;
		}
		return OnMouseUp(in args);
	}

	private bool OnUseSecondary(in PointerInputCmdArgs args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (_draggedEntity.HasValue && _table.HasValue)
		{
			TabletopRequestTakeOut tabletopRequestTakeOut = new TabletopRequestTakeOut
			{
				Entity = ((EntitySystem)this).GetNetEntity(_draggedEntity.Value, (MetaDataComponent)null),
				TableUid = ((EntitySystem)this).GetNetEntity(_table.Value, (MetaDataComponent)null)
			};
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)tabletopRequestTakeOut);
		}
		return false;
	}

	private bool OnMouseDown(in PointerInputCmdArgs args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			EntityUid entityUid = args.EntityUid;
			if (!CanSeeTable(valueOrDefault, _table) || !CanDrag(valueOrDefault, entityUid, out TabletopDraggableComponent _))
			{
				return false;
			}
			if (!(_uiManger.MouseGetControl(args.ScreenCoordinates) is ScalingViewport viewport))
			{
				return false;
			}
			StartDragging(entityUid, viewport);
			return true;
		}
		return false;
	}

	private bool OnMouseUp(in PointerInputCmdArgs args)
	{
		StopDragging();
		return false;
	}

	private void OnAppearanceChange(EntityUid uid, TabletopDraggableComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			Vector2 vector = default(Vector2);
			if (((SharedAppearanceSystem)_appearance).TryGetData<Vector2>(uid, (Enum)TabletopItemVisuals.Scale, ref vector, args.Component))
			{
				_sprite.SetScale(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), vector);
			}
			int num = default(int);
			if (((SharedAppearanceSystem)_appearance).TryGetData<int>(uid, (Enum)TabletopItemVisuals.DrawDepth, ref num, args.Component))
			{
				_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num);
			}
		}
	}

	private void StartDragging(EntityUid draggedEntity, ScalingViewport viewport)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaisePredictiveEvent<TabletopDraggingPlayerChangedEvent>(new TabletopDraggingPlayerChangedEvent(((EntitySystem)this).GetNetEntity(draggedEntity, (MetaDataComponent)null), isDragging: true));
		_draggedEntity = draggedEntity;
		_viewport = viewport;
	}

	private void StopDragging(bool broadcast = true)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (broadcast && _draggedEntity.HasValue && ((EntitySystem)this).HasComp<TabletopDraggableComponent>(_draggedEntity.Value))
		{
			((EntitySystem)this).RaisePredictiveEvent<TabletopMoveEvent>(new TabletopMoveEvent(((EntitySystem)this).GetNetEntity(_draggedEntity.Value, (MetaDataComponent)null), Transforms.GetMapCoordinates(_draggedEntity.Value, (TransformComponent)null), ((EntitySystem)this).GetNetEntity(_table.Value, (MetaDataComponent)null)));
			((EntitySystem)this).RaisePredictiveEvent<TabletopDraggingPlayerChangedEvent>(new TabletopDraggingPlayerChangedEvent(((EntitySystem)this).GetNetEntity(_draggedEntity.Value, (MetaDataComponent)null), isDragging: false));
		}
		_draggedEntity = null;
		_viewport = null;
	}

	private static MapCoordinates ClampPositionToViewport(MapCoordinates coordinates, ScalingViewport viewport)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		if (coordinates == MapCoordinates.Nullspace)
		{
			return MapCoordinates.Nullspace;
		}
		IEye eye = viewport.Eye;
		if (eye == null)
		{
			return MapCoordinates.Nullspace;
		}
		Vector2 vector = Vector2i.op_Implicit(viewport.ViewportSize) / 32f;
		Vector2 position = eye.Position.Position;
		Angle rotation = eye.Rotation;
		Vector2 scale = eye.Scale;
		Vector2 min = (position - vector / 2f) / scale;
		Vector2 max = (position + vector / 2f) / scale;
		if (MathHelper.CloseToPercent(((Angle)(ref rotation)).Degrees % 180.0, 90.0, 1E-05) || MathHelper.CloseToPercent(((Angle)(ref rotation)).Degrees % 180.0, -90.0, 1E-05))
		{
			ref float y = ref min.Y;
			ref float x = ref min.X;
			float x2 = min.X;
			float y2 = min.Y;
			y = x2;
			x = y2;
			y = ref max.Y;
			ref float x3 = ref max.X;
			y2 = max.X;
			x2 = max.Y;
			y = y2;
			x3 = x2;
		}
		return new MapCoordinates(Vector2.Clamp(coordinates.Position, min, max), eye.Position.MapId);
	}
}
