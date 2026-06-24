using System;
using Content.Client.Actions;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Mapping;
using Content.Shared.Maps;
using Robust.Client.Placement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Mapping;

public sealed class MappingSystem : EntitySystem
{
	[Dependency]
	private IPlacementManager _placementMan;

	[Dependency]
	private ITileDefinitionManager _tileMan;

	[Dependency]
	private MetaDataSystem _metaData;

	[Dependency]
	private SharedActionsSystem _actions;

	public static readonly EntProtoId SpawnAction = EntProtoId.op_Implicit("BaseMappingSpawnAction");

	public static readonly EntProtoId EraserAction = EntProtoId.op_Implicit("ActionMappingEraser");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FillActionSlotEvent>((EntityEventHandler<FillActionSlotEvent>)OnFillActionSlot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StartPlacementActionEvent>((EntityEventHandler<StartPlacementActionEvent>)OnStartPlacementAction, (Type[])null, (Type[])null);
	}

	private void OnFillActionSlot(FillActionSlotEvent args)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		if (!_placementMan.IsActive || args.Action.HasValue)
		{
			return;
		}
		PlacementInformation currentPermission = _placementMan.CurrentPermission;
		if (currentPermission != null)
		{
			StartPlacementActionEvent startPlacementActionEvent = new StartPlacementActionEvent
			{
				EntityType = EntProtoId.op_Implicit(currentPermission.EntityType),
				PlacementOption = currentPermission.PlacementOption
			};
			EntityUid val = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(SpawnAction), (ComponentRegistry)null, true);
			if (_placementMan.CurrentPermission.IsTile)
			{
				if (!(_tileMan[_placementMan.CurrentPermission.TileType] is ContentTileDefinition contentTileDefinition))
				{
					return;
				}
				if (!contentTileDefinition.MapAtmosphere)
				{
					ResPath? sprite = contentTileDefinition.Sprite;
					if (sprite.HasValue)
					{
						ResPath valueOrDefault = sprite.GetValueOrDefault();
						_actions.SetIcon(Entity<ActionComponent>.op_Implicit(val), (SpriteSpecifier?)new Texture(valueOrDefault));
					}
				}
				startPlacementActionEvent.TileId = ProtoId<ContentTileDefinition>.op_Implicit(contentTileDefinition.ID);
				_metaData.SetEntityName(val, base.Loc.GetString(contentTileDefinition.Name), (MetaDataComponent)null, true);
			}
			else
			{
				string entityType = currentPermission.EntityType;
				if (entityType != null)
				{
					_actions.SetIcon(Entity<ActionComponent>.op_Implicit(val), (SpriteSpecifier?)new EntityPrototype(entityType));
					_metaData.SetEntityName(val, entityType, (MetaDataComponent)null, true);
				}
			}
			_actions.SetEvent(val, startPlacementActionEvent);
			args.Action = val;
		}
		else if (_placementMan.Eraser)
		{
			args.Action = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(EraserAction), (ComponentRegistry)null, true);
		}
	}

	private void OnStartPlacementAction(StartPlacementActionEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			IPlacementManager placementMan = _placementMan;
			PlacementInformation val = new PlacementInformation();
			EntProtoId? entityType = args.EntityType;
			val.EntityType = (entityType.HasValue ? EntProtoId.op_Implicit(entityType.GetValueOrDefault()) : null);
			val.IsTile = args.TileId.HasValue;
			int tileType;
			if (!args.TileId.HasValue)
			{
				tileType = 0;
			}
			else
			{
				ITileDefinitionManager tileMan = _tileMan;
				ProtoId<ContentTileDefinition>? tileId = args.TileId;
				tileType = tileMan[tileId.HasValue ? ProtoId<ContentTileDefinition>.op_Implicit(tileId.GetValueOrDefault()) : null].TileId;
			}
			val.TileType = tileType;
			val.PlacementOption = args.PlacementOption;
			placementMan.BeginPlacing(val, (PlacementHijack)null);
			if (_placementMan.Eraser != args.Eraser)
			{
				_placementMan.ToggleEraser();
			}
		}
	}
}
