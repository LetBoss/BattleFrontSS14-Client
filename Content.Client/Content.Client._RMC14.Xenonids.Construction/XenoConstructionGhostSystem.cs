using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.IconSmoothing;
using Content.Client.UserInterface.Systems.Actions;
using Content.Shared._RMC14.Sentry;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Construction.Events;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Construction.ResinWhisper;
using Content.Shared._RMC14.Xenonids.Construction.Tunnel;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Eye;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Atmos;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Tag;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Xenonids.Construction;

public sealed class XenoConstructionGhostSystem : EntitySystem
{
	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IUserInterfaceManager _uiManager;

	[Dependency]
	private ITileDefinitionManager _tile;

	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private SpriteSystem _sprite;

	private SharedTransformSystem _transform;

	private SharedMapSystem _mapSystem;

	private SharedXenoConstructionSystem _xenoConstruction;

	private SharedXenoWeedsSystem _xenoWeeds;

	private SharedXenoHiveSystem _hive;

	private TurfSystem _turf;

	private TagSystem _tags;

	private XenoNestSystem _xenoNest;

	private QueenEyeSystem _queenEye;

	private ExamineSystemShared _examineSystem;

	private SharedInteractionSystem _interaction;

	private EntityQuery<BlockXenoConstructionComponent> _blockXenoConstructionQuery;

	private EntityQuery<XenoConstructionSupportComponent> _constructionSupportQuery;

	private EntityQuery<HiveConstructionNodeComponent> _hiveConstructionNodeQuery;

	private EntityQuery<SentryComponent> _sentryQuery;

	private EntityQuery<XenoConstructComponent> _xenoConstructQuery;

	private EntityQuery<XenoEggComponent> _xenoEggQuery;

	private EntityQuery<XenoTunnelComponent> _xenoTunnelQuery;

	private EntityUid? _currentGhost;

	private string? _currentGhostStructure;

	private EntityCoordinates _lastPosition = EntityCoordinates.Invalid;

	private static readonly ProtoId<TagPrototype> AirlockTag = ProtoId<TagPrototype>.op_Implicit("Airlock");

	private static readonly ProtoId<TagPrototype> StructureTag = ProtoId<TagPrototype>.op_Implicit("Structure");

	public override void Initialize()
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		((EntitySystem)this).Initialize();
		((EntitySystem)this).UpdatesOutsidePrediction = true;
		_transform = base.EntityManager.System<SharedTransformSystem>();
		_mapSystem = base.EntityManager.System<SharedMapSystem>();
		_xenoConstruction = base.EntityManager.System<SharedXenoConstructionSystem>();
		_xenoWeeds = base.EntityManager.System<SharedXenoWeedsSystem>();
		_hive = base.EntityManager.System<SharedXenoHiveSystem>();
		_turf = base.EntityManager.System<TurfSystem>();
		_tags = base.EntityManager.System<TagSystem>();
		_xenoNest = base.EntityManager.System<XenoNestSystem>();
		_queenEye = base.EntityManager.System<QueenEyeSystem>();
		_examineSystem = base.EntityManager.System<ExamineSystemShared>();
		_interaction = base.EntityManager.System<SharedInteractionSystem>();
		_blockXenoConstructionQuery = ((EntitySystem)this).GetEntityQuery<BlockXenoConstructionComponent>();
		_constructionSupportQuery = ((EntitySystem)this).GetEntityQuery<XenoConstructionSupportComponent>();
		_hiveConstructionNodeQuery = ((EntitySystem)this).GetEntityQuery<HiveConstructionNodeComponent>();
		_sentryQuery = ((EntitySystem)this).GetEntityQuery<SentryComponent>();
		_xenoConstructQuery = ((EntitySystem)this).GetEntityQuery<XenoConstructComponent>();
		_xenoEggQuery = ((EntitySystem)this).GetEntityQuery<XenoEggComponent>();
		_xenoTunnelQuery = ((EntitySystem)this).GetEntityQuery<XenoTunnelComponent>();
		CommandBinds.Builder.Bind(EngineKeyFunctions.Use, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleUse), true, true)).Bind(EngineKeyFunctions.UseSecondary, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleRightClick), true, true)).Register<XenoConstructionGhostSystem>();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<XenoConstructionGhostSystem>();
	}

	private bool HandleUse(in PointerInputCmdArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if ((int)args.State != 1)
		{
			return false;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		XenoConstructionComponent xenoConstructionComponent = default(XenoConstructionComponent);
		if (!localEntity.HasValue || !((EntitySystem)this).TryComp<XenoConstructionComponent>(localEntity.Value, ref xenoConstructionComponent))
		{
			return false;
		}
		if (xenoConstructionComponent.OrderConstructionTargeting && xenoConstructionComponent.OrderConstructionChoice.HasValue)
		{
			ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
			EntityCoordinates val = SnapToGrid(mouseScreenPosition);
			if (!((EntityCoordinates)(ref val)).IsValid((IEntityManager)(object)base.EntityManager))
			{
				return false;
			}
			if (!IsValidConstructionLocation(localEntity.Value, val))
			{
				return false;
			}
			XenoOrderConstructionClickEvent xenoOrderConstructionClickEvent = new XenoOrderConstructionClickEvent(((EntitySystem)this).GetNetCoordinates(val, (MetaDataComponent)null), xenoConstructionComponent.OrderConstructionChoice.Value);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)xenoOrderConstructionClickEvent);
			return true;
		}
		return false;
	}

	private bool HandleRightClick(in PointerInputCmdArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if ((int)args.State != 1)
		{
			return false;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		XenoConstructionComponent xenoConstructionComponent = default(XenoConstructionComponent);
		if (!localEntity.HasValue || !((EntitySystem)this).TryComp<XenoConstructionComponent>(localEntity.Value, ref xenoConstructionComponent))
		{
			return false;
		}
		if (xenoConstructionComponent.OrderConstructionTargeting)
		{
			ClearCurrentGhost();
			XenoOrderConstructionCancelEvent xenoOrderConstructionCancelEvent = new XenoOrderConstructionCancelEvent();
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)xenoOrderConstructionCancelEvent);
			return true;
		}
		return false;
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue)
		{
			ClearCurrentGhost();
			return;
		}
		(string? buildChoice, bool isActive) constructionState = GetConstructionState(localEntity.Value);
		string item = constructionState.buildChoice;
		bool item2 = constructionState.isActive;
		bool flag = IsBuilding(localEntity.Value);
		if (item2 && !string.IsNullOrEmpty(item) && !flag)
		{
			string actualBuildPrototype = GetActualBuildPrototype(localEntity.Value, item);
			if (!_currentGhost.HasValue || _currentGhostStructure != item || GetActualBuildPrototype(localEntity.Value, _currentGhostStructure ?? "") != actualBuildPrototype)
			{
				ClearCurrentGhost();
				CreateGhost(localEntity.Value, item);
			}
			UpdateGhostPosition();
		}
		else
		{
			ClearCurrentGhost();
		}
	}

	private (string? buildChoice, bool isActive) GetConstructionState(EntityUid player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		XenoConstructionComponent xenoConstructionComponent = default(XenoConstructionComponent);
		EntProtoId val;
		if (((EntitySystem)this).TryComp<XenoConstructionComponent>(player, ref xenoConstructionComponent) && xenoConstructionComponent.OrderConstructionTargeting && xenoConstructionComponent.OrderConstructionChoice.HasValue)
		{
			val = xenoConstructionComponent.OrderConstructionChoice.Value;
			return (buildChoice: ((EntProtoId)(ref val)).Id, isActive: true);
		}
		EntityUid? selectingTargetFor = _uiManager.GetUIController<ActionUIController>().SelectingTargetFor;
		if (selectingTargetFor.HasValue)
		{
			EntityUid valueOrDefault = selectingTargetFor.GetValueOrDefault();
			XenoConstructionActionComponent xenoConstructionActionComponent = default(XenoConstructionActionComponent);
			if (((EntitySystem)this).TryComp<XenoConstructionActionComponent>(valueOrDefault, ref xenoConstructionActionComponent) && xenoConstructionComponent != null)
			{
				ref EntProtoId? buildChoice = ref xenoConstructionComponent.BuildChoice;
				object item;
				if (!buildChoice.HasValue)
				{
					item = null;
				}
				else
				{
					val = buildChoice.GetValueOrDefault();
					item = ((EntProtoId)(ref val)).Id;
				}
				return (buildChoice: (string)item, isActive: true);
			}
			return (buildChoice: null, isActive: false);
		}
		return (buildChoice: null, isActive: false);
	}

	private bool IsBuilding(EntityUid player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DoAfterComponent doAfterComponent = default(DoAfterComponent);
		if (!((EntitySystem)this).TryComp<DoAfterComponent>(player, ref doAfterComponent))
		{
			return false;
		}
		return doAfterComponent.DoAfters.Values.Any(delegate(Content.Shared.DoAfter.DoAfter activeDoAfter)
		{
			DoAfterEvent doAfterEvent = activeDoAfter.Args.Event;
			return (doAfterEvent is XenoSecreteStructureDoAfterEvent || doAfterEvent is XenoOrderConstructionDoAfterEvent) ? true : false;
		});
	}

	private void CreateGhost(EntityUid player, string structurePrototype)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coordinates = ((EntitySystem)this).Comp<TransformComponent>(player).Coordinates;
		EntityUid val = ((EntitySystem)this).Spawn("XenoConstructionGhost", coordinates);
		string actualBuildPrototype = GetActualBuildPrototype(player, structurePrototype);
		ConfigureGhostSprite(val, actualBuildPrototype);
		_currentGhost = val;
		_currentGhostStructure = structurePrototype;
		_lastPosition = EntityCoordinates.Invalid;
	}

	private string GetActualBuildPrototype(EntityUid player, string originalPrototype)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<QueenBuildingBoostComponent>(player))
		{
			string queenVariant = GetQueenVariant(originalPrototype);
			if (_prototypeManager.HasIndex(EntProtoId.op_Implicit(queenVariant)))
			{
				return queenVariant;
			}
		}
		return originalPrototype;
	}

	private string GetQueenVariant(string originalId)
	{
		return originalId switch
		{
			"WallXenoResin" => "WallXenoResinQueen", 
			"WallXenoMembrane" => "WallXenoMembraneQueen", 
			"DoorXenoResin" => "DoorXenoResinQueen", 
			_ => originalId, 
		};
	}

	private void ConfigureGhostSprite(EntityUid ghost, string structurePrototype)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(ghost, ref val))
		{
			return;
		}
		_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((ghost, val)), new Color((byte)48, byte.MaxValue, (byte)48, (byte)128));
		_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((ghost, val)), 9);
		_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((ghost, val)), true);
		EntityPrototype val2 = default(EntityPrototype);
		SpriteComponent item = default(SpriteComponent);
		if (_prototypeManager.TryIndex<EntityPrototype>(structurePrototype, ref val2) && !TryConfigureIconSmoothSprite(ghost, val, val2) && val2.TryGetComponent<SpriteComponent>(ref item, base.EntityManager.ComponentFactory))
		{
			_sprite.CopySprite(Entity<SpriteComponent>.op_Implicit((EntityUid.Invalid, item)), Entity<SpriteComponent>.op_Implicit((ghost, val)));
			_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((ghost, val)), new Color((byte)48, byte.MaxValue, (byte)48, (byte)128));
			_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((ghost, val)), 9);
			for (int i = 0; i < val.AllLayers.Count(); i++)
			{
				val.LayerSetShader(i, "unshaded");
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ghost, val)), i, true);
			}
		}
	}

	private bool TryConfigureIconSmoothSprite(EntityUid ghost, SpriteComponent sprite, EntityPrototype prototype)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		IconSmoothComponent iconSmoothComponent = default(IconSmoothComponent);
		SpriteComponent val = default(SpriteComponent);
		if (!prototype.TryGetComponent<IconSmoothComponent>(ref iconSmoothComponent, base.EntityManager.ComponentFactory) || !prototype.TryGetComponent<SpriteComponent>(ref val, base.EntityManager.ComponentFactory) || string.IsNullOrEmpty(iconSmoothComponent.StateBase))
		{
			return false;
		}
		try
		{
			if (!_sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), 0))
			{
				_sprite.AddBlankLayer(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), (int?)0);
			}
			_sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), 0, val.BaseRSI, (StateId?)null);
			RSI baseRSI = val.BaseRSI;
			State val2 = default(State);
			if (baseRSI != null && baseRSI.TryGetState(StateId.op_Implicit(iconSmoothComponent.StateBase), ref val2))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), 0, StateId.op_Implicit(iconSmoothComponent.StateBase));
				sprite.LayerSetShader(0, "unshaded");
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), 0, true);
				_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((ghost, sprite)), new Color((byte)48, byte.MaxValue, (byte)48, (byte)128));
				return true;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	private void UpdateGhostPosition()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue || !_currentGhost.HasValue || !((EntitySystem)this).Exists(_currentGhost.Value))
		{
			return;
		}
		ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
		EntityCoordinates val = SnapToGrid(mouseScreenPosition);
		if (((EntityCoordinates)(ref val)).IsValid((IEntityManager)(object)base.EntityManager))
		{
			if (!((EntityCoordinates)(ref val)).Equals(_lastPosition))
			{
				TransformComponent val2 = ((EntitySystem)this).Comp<TransformComponent>(_currentGhost.Value);
				_transform.SetCoordinates(_currentGhost.Value, val2, val, (Angle?)null, true, (TransformComponent)null, (TransformComponent)null);
				_lastPosition = val;
			}
			SpriteComponent item = default(SpriteComponent);
			if (((EntitySystem)this).TryComp<SpriteComponent>(_currentGhost.Value, ref item))
			{
				_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((_currentGhost.Value, item)), IsValidConstructionLocation(localEntity.Value, val) ? new Color((byte)48, byte.MaxValue, (byte)48, (byte)128) : new Color(byte.MaxValue, (byte)48, (byte)48, (byte)128));
			}
		}
	}

	private EntityCoordinates SnapToGrid(ScreenCoordinates screenCoords)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates val = _eyeManager.PixelToMap(screenCoords.Position);
		if (val.MapId == MapId.Nullspace)
		{
			EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
			if (!localEntity.HasValue)
			{
				return EntityCoordinates.Invalid;
			}
			return _transform.GetMoverCoordinates(localEntity.Value);
		}
		EntityUid val2 = default(EntityUid);
		MapGridComponent val3 = default(MapGridComponent);
		if (!_mapManager.TryFindGridAt(val, ref val2, ref val3))
		{
			return _transform.ToCoordinates(val);
		}
		EntityCoordinates val4 = _transform.ToCoordinates(Entity<TransformComponent>.op_Implicit(val2), val);
		Vector2i val5 = _mapSystem.CoordinatesToTile(val2, val3, val4);
		return _mapSystem.GridTileToLocal(val2, val3, val5);
	}

	private bool IsValidConstructionLocation(EntityUid player, EntityCoordinates coords)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		XenoConstructionComponent xenoConstructionComponent = default(XenoConstructionComponent);
		if (!((EntitySystem)this).TryComp<XenoConstructionComponent>(player, ref xenoConstructionComponent))
		{
			return false;
		}
		try
		{
			if (xenoConstructionComponent.OrderConstructionTargeting && xenoConstructionComponent.OrderConstructionChoice.HasValue)
			{
				return CanOrderConstruction(Entity<XenoConstructionComponent>.op_Implicit((player, xenoConstructionComponent)), coords, xenoConstructionComponent.OrderConstructionChoice);
			}
			if (xenoConstructionComponent.BuildChoice.HasValue)
			{
				return CanSecreteOnTile(Entity<XenoConstructionComponent>.op_Implicit((player, xenoConstructionComponent)), xenoConstructionComponent.BuildChoice, coords, checkStructureSelected: true, checkWeeds: true);
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	private bool CanSecreteOnTile(Entity<XenoConstructionComponent> xeno, EntProtoId? buildChoice, EntityCoordinates target, bool checkStructureSelected, bool checkWeeds)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		if (checkStructureSelected && !buildChoice.HasValue)
		{
			return false;
		}
		EntityUid? grid = _transform.GetGrid(target);
		if (grid.HasValue)
		{
			EntityUid valueOrDefault = grid.GetValueOrDefault();
			MapGridComponent val = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(valueOrDefault, ref val))
			{
				target = target.SnapToGrid((IEntityManager?)(object)base.EntityManager, _mapManager);
				if (checkWeeds && !_queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner)) && !_xenoWeeds.IsOnWeeds(Entity<MapGridComponent>.op_Implicit((valueOrDefault, val)), target))
				{
					return false;
				}
				if (!_queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner)))
				{
					EntityCoordinates moverCoordinates = _transform.GetMoverCoordinates(xeno.Owner);
					var (num, flag) = GetEffectiveBuildRange(xeno, target);
					if (num > 0f && !_transform.InRange(moverCoordinates, target, num))
					{
						return false;
					}
					if (_transform.InRange(moverCoordinates, target, _xenoConstruction.GetStructureMinRange(buildChoice).Float()))
					{
						return false;
					}
					if (flag && !CanDoRemoteConstruction(xeno, target))
					{
						return false;
					}
				}
				if (!_queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner)) && !TileSolidAndNotBlocked(target))
				{
					return false;
				}
				Vector2i val2 = _mapSystem.CoordinatesToTile(valueOrDefault, val, target);
				AnchoredEntitiesEnumerator anchoredEntitiesEnumerator = _mapSystem.GetAnchoredEntitiesEnumerator(valueOrDefault, val, val2);
				EntityUid? val3 = default(EntityUid?);
				while (((AnchoredEntitiesEnumerator)(ref anchoredEntitiesEnumerator)).MoveNext(ref val3))
				{
					if (_xenoConstructQuery.HasComp(val3) || _xenoEggQuery.HasComp(val3) || _xenoTunnelQuery.HasComp(val3) || _sentryQuery.HasComp(val3) || _blockXenoConstructionQuery.HasComp(val3))
					{
						return false;
					}
				}
				if (checkStructureSelected && buildChoice.HasValue && !((EntitySystem)this).HasComp<QueenBuildingBoostComponent>(xeno.Owner))
				{
					FixedPoint2? structurePlasmaCost = _xenoConstruction.GetStructurePlasmaCost(buildChoice.Value);
					if (structurePlasmaCost.HasValue)
					{
						FixedPoint2 valueOrDefault2 = structurePlasmaCost.GetValueOrDefault();
						XenoPlasmaComponent xenoPlasmaComponent = default(XenoPlasmaComponent);
						if (!((EntitySystem)this).TryComp<XenoPlasmaComponent>(xeno.Owner, ref xenoPlasmaComponent) || xenoPlasmaComponent.Plasma < valueOrDefault2)
						{
							return false;
						}
					}
				}
				if (checkStructureSelected && buildChoice.HasValue)
				{
					EntProtoId valueOrDefault3 = buildChoice.GetValueOrDefault();
					EntityPrototype val4 = default(EntityPrototype);
					XenoConstructionRequiresSupportComponent xenoConstructionRequiresSupportComponent = default(XenoConstructionRequiresSupportComponent);
					if (_prototypeManager.TryIndex(valueOrDefault3, ref val4) && val4.TryGetComponent<XenoConstructionRequiresSupportComponent>(ref xenoConstructionRequiresSupportComponent, _compFactory) && !IsSupported(Entity<MapGridComponent>.op_Implicit((valueOrDefault, val)), target))
					{
						return false;
					}
				}
				return true;
			}
		}
		return false;
	}

	private bool CanOrderConstruction(Entity<XenoConstructionComponent> xeno, EntityCoordinates target, EntProtoId? choice)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		if (!CanSecreteOnTile(xeno, choice, target, checkStructureSelected: false, checkWeeds: false))
		{
			return false;
		}
		EntityUid? grid = _transform.GetGrid(target);
		if (grid.HasValue)
		{
			EntityUid valueOrDefault = grid.GetValueOrDefault();
			MapGridComponent val = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(valueOrDefault, ref val))
			{
				Vector2i val2 = _mapSystem.TileIndicesFor(valueOrDefault, val, target);
				Direction[] array = new Direction[4];
				RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
				Direction[] array2 = (Direction[])(object)array;
				EntityUid? val4 = default(EntityUid?);
				HiveConstructionNodeComponent hiveConstructionNodeComponent = default(HiveConstructionNodeComponent);
				foreach (Direction val3 in array2)
				{
					Vector2i direction = SharedMapSystem.GetDirection(val2, val3, 1);
					AnchoredEntitiesEnumerator anchoredEntitiesEnumerator = _mapSystem.GetAnchoredEntitiesEnumerator(valueOrDefault, val, direction);
					while (((AnchoredEntitiesEnumerator)(ref anchoredEntitiesEnumerator)).MoveNext(ref val4))
					{
						if (_hiveConstructionNodeQuery.TryGetComponent(val4, ref hiveConstructionNodeComponent) && hiveConstructionNodeComponent.BlockOtherNodes)
						{
							return false;
						}
					}
				}
				EntityPrototype val5 = default(EntityPrototype);
				if (choice.HasValue && _prototypeManager.TryIndex(choice, ref val5))
				{
					HiveConstructionRequiresWeedableSurfaceComponent hiveConstructionRequiresWeedableSurfaceComponent = default(HiveConstructionRequiresWeedableSurfaceComponent);
					TileRef val6 = default(TileRef);
					ITileDefinition val7 = default(ITileDefinition);
					if (val5.TryGetComponent<HiveConstructionRequiresWeedableSurfaceComponent>(ref hiveConstructionRequiresWeedableSurfaceComponent, _compFactory) && (!_mapSystem.TryGetTileRef(valueOrDefault, val, val2, ref val6) || !_tile.TryGetDefinition(val6.Tile.TypeId, ref val7) || ((IPrototype)val7).ID == "Space" || val7 is ContentTileDefinition { WeedsSpreadable: false }))
					{
						return false;
					}
					HiveConstructionRequiresHiveWeedsComponent hiveConstructionRequiresHiveWeedsComponent = default(HiveConstructionRequiresHiveWeedsComponent);
					if (val5.TryGetComponent<HiveConstructionRequiresHiveWeedsComponent>(ref hiveConstructionRequiresHiveWeedsComponent, _compFactory) && !_xenoWeeds.IsOnHiveWeeds(Entity<MapGridComponent>.op_Implicit((valueOrDefault, val)), target))
					{
						return false;
					}
					HiveConstructionRequiresSpaceComponent hiveConstructionRequiresSpaceComponent = default(HiveConstructionRequiresSpaceComponent);
					if (val5.TryGetComponent<HiveConstructionRequiresSpaceComponent>(ref hiveConstructionRequiresSpaceComponent, _compFactory) && !CanPlaceSpaceRequiringStructure(_transform.ToMapCoordinates(target, true), Entity<MapGridComponent>.op_Implicit((valueOrDefault, val))))
					{
						return false;
					}
					HiveConstructionLimitedComponent comp = default(HiveConstructionLimitedComponent);
					if (val5.TryGetComponent<HiveConstructionLimitedComponent>(ref comp, _compFactory) && !CanPlaceLimitedHiveStructure(xeno.Owner, comp))
					{
						return false;
					}
				}
				return true;
			}
		}
		return false;
	}

	private bool TileSolidAndNotBlocked(EntityCoordinates target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		TileRef? tileRef = _turf.GetTileRef(target);
		if (tileRef.HasValue)
		{
			TileRef valueOrDefault = tileRef.GetValueOrDefault();
			if (!_turf.IsSpace(valueOrDefault) && _turf.GetContentTileDefinition(valueOrDefault).Sturdy && !_turf.IsTileBlocked(valueOrDefault, CollisionGroup.Impassable))
			{
				return !_xenoNest.HasAdjacentNestFacing(target);
			}
		}
		return false;
	}

	private bool IsSupported(Entity<MapGridComponent> grid, EntityCoordinates coordinates)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Vector2i tile = _mapSystem.TileIndicesFor(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), coordinates);
		return IsSupported(grid, tile);
	}

	private bool IsSupported(Entity<MapGridComponent> grid, Vector2i tile)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val2 = default(EntityUid?);
		for (int i = 0; i < 4; i++)
		{
			AtmosDirection dir = (AtmosDirection)(1 << i);
			Vector2i val = tile.Offset(dir);
			AnchoredEntitiesEnumerator anchoredEntitiesEnumerator = _mapSystem.GetAnchoredEntitiesEnumerator(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), val);
			while (((AnchoredEntitiesEnumerator)(ref anchoredEntitiesEnumerator)).MoveNext(ref val2))
			{
				if (!((EntitySystem)this).TerminatingOrDeleted(val2.Value, (MetaDataComponent)null) && !base.EntityManager.IsQueuedForDeletion(val2.Value) && _constructionSupportQuery.HasComp(val2))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool CanPlaceSpaceRequiringStructure(MapCoordinates mapCoords, Entity<MapGridComponent> map)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		TileRef tileRef = _mapSystem.GetTileRef(map, mapCoords);
		Vector2i indices = default(Vector2i);
		for (int i = ((TileRef)(ref tileRef)).X - 1; i <= ((TileRef)(ref tileRef)).X + 1; i++)
		{
			for (int j = ((TileRef)(ref tileRef)).Y - 1; j <= ((TileRef)(ref tileRef)).Y + 1; j++)
			{
				if (i != 0 || j != 0)
				{
					((Vector2i)(ref indices))._002Ector(i, j);
					if (_turf.IsTileBlocked(Entity<MapGridComponent>.op_Implicit(map), indices, CollisionGroup.MobMask, map.Comp))
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private bool CanPlaceLimitedHiveStructure(EntityUid hiveMember, HiveConstructionLimitedComponent comp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		EntProtoId id = comp.Id;
		Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(hiveMember));
		if (hive.HasValue)
		{
			Entity<HiveComponent> valueOrDefault = hive.GetValueOrDefault();
			if (_hive.TryGetStructureLimit(valueOrDefault, id, out var limit))
			{
				int num = 0;
				EntityQueryEnumerator<HiveConstructionLimitedComponent, HiveMemberComponent> val = ((EntitySystem)this).EntityQueryEnumerator<HiveConstructionLimitedComponent, HiveMemberComponent>();
				HiveConstructionLimitedComponent hiveConstructionLimitedComponent = default(HiveConstructionLimitedComponent);
				HiveMemberComponent hiveMemberComponent = default(HiveMemberComponent);
				while (val.MoveNext(ref hiveConstructionLimitedComponent, ref hiveMemberComponent))
				{
					if (hiveConstructionLimitedComponent.Id == id)
					{
						num++;
					}
				}
				return limit > num;
			}
		}
		return false;
	}

	private (float range, bool isRemote) GetEffectiveBuildRange(Entity<XenoConstructionComponent> xeno, EntityCoordinates target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		FixedPoint2 buildRange = xeno.Comp.BuildRange;
		if (_queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner)))
		{
			return (range: float.MaxValue, isRemote: false);
		}
		ResinWhispererComponent resinWhispererComponent = default(ResinWhispererComponent);
		if (!((EntitySystem)this).TryComp<ResinWhispererComponent>(xeno.Owner, ref resinWhispererComponent))
		{
			return (range: buildRange.Float(), isRemote: false);
		}
		float num = resinWhispererComponent.MaxConstructDistance?.Float() ?? buildRange.Float();
		if (_interaction.InRangeUnobstructed(xeno.Owner, target, num))
		{
			return (range: num, isRemote: false);
		}
		return (range: resinWhispererComponent.MaxRemoteConstructDistance, isRemote: true);
	}

	private bool CanDoRemoteConstruction(Entity<XenoConstructionComponent> xeno, EntityCoordinates target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		ResinWhispererComponent resinWhispererComponent = default(ResinWhispererComponent);
		if (!((EntitySystem)this).TryComp<ResinWhispererComponent>(xeno.Owner, ref resinWhispererComponent))
		{
			return false;
		}
		if (_queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner)))
		{
			return true;
		}
		if (!_xenoWeeds.IsOnFriendlyWeeds(Entity<TransformComponent>.op_Implicit(xeno.Owner)))
		{
			return false;
		}
		if (!TileIsVisible(xeno.Owner, target, resinWhispererComponent.MaxRemoteConstructDistance))
		{
			return false;
		}
		return true;
	}

	private bool TileIsVisible(EntityUid user, EntityCoordinates targetCoordinates, float maxDistance)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates other = _transform.ToMapCoordinates(targetCoordinates, true);
		for (int i = 0; i < 9; i++)
		{
			switch (i)
			{
			case 1:
			case 7:
			case 8:
				other = ((MapCoordinates)(ref other)).Offset(0.499f, 0f);
				break;
			case 2:
				other = ((MapCoordinates)(ref other)).Offset(0f, -0.499f);
				break;
			case 3:
			case 4:
				other = ((MapCoordinates)(ref other)).Offset(-0.499f, 0f);
				break;
			case 5:
			case 6:
				other = ((MapCoordinates)(ref other)).Offset(0f, 0.499f);
				break;
			}
			if (_examineSystem.InRangeUnOccluded(user, other, maxDistance))
			{
				return true;
			}
		}
		return false;
	}

	private void ClearCurrentGhost()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (_currentGhost.HasValue && ((EntitySystem)this).Exists(_currentGhost.Value))
		{
			((EntitySystem)this).QueueDel((EntityUid?)_currentGhost.Value);
		}
		_currentGhost = null;
		_currentGhostStructure = null;
		_lastPosition = EntityCoordinates.Invalid;
	}
}
