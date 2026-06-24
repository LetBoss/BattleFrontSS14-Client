using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client._RMC14.Interaction;
using Content.Client.Clickable;
using Content.Client.Sprite;
using Content.Client.UserInterface;
using Content.Client.Viewport;
using Content.Shared.Input;
using Robust.Client.ComponentTrees;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Analyzers;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Client.Gameplay;

[Virtual]
public class GameplayStateBase : State, IEntityEventSubscriber
{
	private sealed class ClickableEntityComparer : IComparer<(EntityUid clicked, int depth, uint renderOrder, float bottom)>
	{
		public int Compare((EntityUid clicked, int depth, uint renderOrder, float bottom) x, (EntityUid clicked, int depth, uint renderOrder, float bottom) y)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			int num = y.depth.CompareTo(x.depth);
			if (num != 0)
			{
				return num;
			}
			num = y.renderOrder.CompareTo(x.renderOrder);
			if (num != 0)
			{
				return num;
			}
			num = -y.bottom.CompareTo(x.bottom);
			if (num != 0)
			{
				return num;
			}
			return ((EntityUid)(ref y.clicked)).CompareTo(x.clicked);
		}
	}

	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IEntitySystemManager _entitySystemManager;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	protected IUserInterfaceManager UserInterfaceManager;

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IViewVariablesManager _vvm;

	[Dependency]
	private IConsoleHost _conHost;

	private ClickableEntityComparer _comparer;

	private (ViewVariablesPath? path, string[] segments) ResolveVvHoverObject(string path)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		string[] item = path.Split('/');
		EntityUid? val = RecursivelyFindUiEntity(UserInterfaceManager.CurrentlyHovered);
		NetEntity? netEntity = _entityManager.GetNetEntity(val, (MetaDataComponent)null);
		return (path: (ViewVariablesPath)((!netEntity.HasValue) ? ((ViewVariablesInstancePath)null) : new ViewVariablesInstancePath((object)netEntity)), segments: item);
	}

	private EntityUid? RecursivelyFindUiEntity(Control? control)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (control == null)
		{
			return null;
		}
		IViewportControl val = (IViewportControl)(object)((control is IViewportControl) ? control : null);
		if (val == null)
		{
			SpriteView val2 = (SpriteView)(object)((control is SpriteView) ? control : null);
			if (val2 == null)
			{
				if (control is IEntityControl entityControl)
				{
					return entityControl.UiEntity;
				}
				return RecursivelyFindUiEntity(control.Parent);
			}
			Entity<SpriteComponent, TransformComponent>? entity = val2.Entity;
			if (!entity.HasValue)
			{
				return null;
			}
			return Entity<SpriteComponent, TransformComponent>.op_Implicit(entity.GetValueOrDefault());
		}
		ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
		if (((ScreenCoordinates)(ref mouseScreenPosition)).IsValid)
		{
			return GetClickedEntity(val.PixelToMap(_inputManager.MouseScreenPosition.Position));
		}
		return null;
	}

	private IEnumerable<string>? ListVVHoverPaths(string[] segments)
	{
		return null;
	}

	protected override void Startup()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0028: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		_vvm.RegisterDomain("enthover", new DomainResolveObject(ResolveVvHoverObject), new DomainListPaths(ListVVHoverPaths));
		_inputManager.KeyBindStateChanged += OnKeyBindStateChanged;
		_comparer = new ClickableEntityComparer();
		CommandBinds.Builder.Bind(ContentKeyFunctions.InspectEntity, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate(HandleInspect), true, true)).Register<GameplayStateBase>();
	}

	protected override void Shutdown()
	{
		_vvm.UnregisterDomain("enthover");
		_inputManager.KeyBindStateChanged -= OnKeyBindStateChanged;
		CommandBinds.Unregister<GameplayStateBase>();
	}

	private bool HandleInspect(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
	{
		_conHost.ExecuteCommand("vv /c/enthover");
		return true;
	}

	public EntityUid? GetClickedEntity(MapCoordinates coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetClickedEntity(coordinates, _eyeManager.CurrentEye);
	}

	public EntityUid? GetClickedEntity(MapCoordinates coordinates, IEye? eye)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (eye == null)
		{
			return null;
		}
		EntityUid value = GetClickableEntities(coordinates, eye).FirstOrDefault();
		if (!((EntityUid)(ref value)).IsValid())
		{
			return null;
		}
		return value;
	}

	public IEnumerable<EntityUid> GetClickableEntities(EntityCoordinates coordinates, bool excludeFaded = true)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		SharedTransformSystem entitySystem = _entitySystemManager.GetEntitySystem<SharedTransformSystem>();
		return GetClickableEntities(entitySystem.ToMapCoordinates(coordinates, true), excludeFaded);
	}

	public IEnumerable<EntityUid> GetClickableEntities(MapCoordinates coordinates, bool excludeFaded = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetClickableEntities(coordinates, _eyeManager.CurrentEye, excludeFaded);
	}

	public IEnumerable<EntityUid> GetClickableEntities(MapCoordinates coordinates, IEye? eye, bool excludeFaded = true, bool ignoreInteractionTransparency = false)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if (eye == null)
		{
			return Array.Empty<EntityUid>();
		}
		HashSet<ComponentTreeEntry<SpriteComponent>> hashSet = ((ComponentTreeSystem<SpriteTreeComponent, SpriteComponent>)(object)_entityManager.EntitySysManager.GetEntitySystem<SpriteTreeSystem>()).QueryAabb(coordinates.MapId, Box2.CenteredAround(coordinates.Position, new Vector2(1f, 1f)), true);
		List<(EntityUid, int, uint, float)> list = new List<(EntityUid, int, uint, float)>(hashSet.Count);
		EntityQuery<ClickableComponent> entityQuery = _entityManager.GetEntityQuery<ClickableComponent>();
		ClickableSystem clickableSystem = _entityManager.System<ClickableSystem>();
		ClickableComponent item = default(ClickableComponent);
		foreach (ComponentTreeEntry<SpriteComponent> item2 in hashSet)
		{
			if ((ignoreInteractionTransparency || !_entitySystemManager.GetEntitySystem<RMCClientInteractionSystem>().IsInteractionTransparency(item2.Uid, ((ISharedPlayerManager)_playerManager).LocalEntity, eye)) && entityQuery.TryGetComponent(item2.Uid, ref item) && clickableSystem.CheckClick(Entity<ClickableComponent, SpriteComponent, TransformComponent, FadingSpriteComponent>.op_Implicit((item2.Uid, item, item2.Component, item2.Transform)), coordinates.Position, eye, excludeFaded, out var drawDepth, out var renderOrder, out var bottom))
			{
				list.Add((item2.Uid, drawDepth, renderOrder, bottom));
			}
		}
		if (list.Count == 0)
		{
			return Array.Empty<EntityUid>();
		}
		list.Sort(_comparer);
		return list.Select(((EntityUid, int, uint, float) a) => a.Item1);
	}

	protected virtual void OnKeyBindStateChanged(ViewportBoundKeyEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		InputSystem val = default(InputSystem);
		if (!_entitySystemManager.TryGetEntitySystem<InputSystem>(ref val))
		{
			return;
		}
		BoundKeyEventArgs keyEventArgs = args.KeyEventArgs;
		BoundKeyFunction function = keyEventArgs.Function;
		KeyFunctionId val2 = _inputManager.NetworkBindMap.KeyFunctionID(function);
		EntityCoordinates val3 = default(EntityCoordinates);
		EntityUid? val4 = null;
		Control viewport = args.Viewport;
		IViewportControl val5 = (IViewportControl)(object)((viewport is IViewportControl) ? viewport : null);
		if (val5 != null)
		{
			ScreenCoordinates pointerLocation = keyEventArgs.PointerLocation;
			if (((ScreenCoordinates)(ref pointerLocation)).IsValid)
			{
				MapCoordinates val6 = val5.PixelToMap(keyEventArgs.PointerLocation.Position);
				val4 = ((!(val5 is ScalingViewport scalingViewport)) ? GetClickedEntity(val6) : GetClickedEntity(val6, scalingViewport.Eye));
				SharedTransformSystem entitySystem = _entitySystemManager.GetEntitySystem<SharedTransformSystem>();
				MapSystem entitySystem2 = _entitySystemManager.GetEntitySystem<MapSystem>();
				EntityUid val7 = default(EntityUid);
				MapGridComponent val8 = default(MapGridComponent);
				val3 = ((!((SharedMapSystem)entitySystem2).MapExists((MapId?)val6.MapId)) ? EntityCoordinates.Invalid : (_mapManager.TryFindGridAt(val6, ref val7, ref val8) ? ((SharedMapSystem)entitySystem2).MapToGrid(val7, val6) : entitySystem.ToCoordinates(val6)));
				goto IL_0110;
			}
		}
		val3 = EntityCoordinates.Invalid;
		goto IL_0110;
		IL_0110:
		ClientFullInputCmdMessage val9 = new ClientFullInputCmdMessage(_timing.CurTick, _timing.TickFraction, val2);
		val9.set_State(keyEventArgs.State);
		val9.set_Coordinates(val3);
		val9.set_ScreenCoordinates(keyEventArgs.PointerLocation);
		val9.set_Uid(val4.GetValueOrDefault());
		ClientFullInputCmdMessage val10 = val9;
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		if (val.HandleInputCommand(localSession, function, (IFullInputCmdMessage)(object)val10, false))
		{
			keyEventArgs.Handle();
		}
	}
}
