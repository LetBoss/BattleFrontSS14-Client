// Decompiled with JetBrains decompiler
// Type: Content.Client.Mapping.MappingSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;

#nullable enable
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

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<FillActionSlotEvent>(new EntityEventHandler<FillActionSlotEvent>(this.OnFillActionSlot), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<StartPlacementActionEvent>(new EntityEventHandler<StartPlacementActionEvent>(this.OnStartPlacementAction), (Type[]) null, (Type[]) null);
  }

  private void OnFillActionSlot(FillActionSlotEvent args)
  {
    if (!this._placementMan.IsActive || args.Action.HasValue)
      return;
    PlacementInformation currentPermission = this._placementMan.CurrentPermission;
    if (currentPermission != null)
    {
      StartPlacementActionEvent ev = new StartPlacementActionEvent()
      {
        EntityType = EntProtoId.op_Implicit(currentPermission.EntityType),
        PlacementOption = currentPermission.PlacementOption
      };
      EntityUid uid = this.Spawn(EntProtoId.op_Implicit(MappingSystem.SpawnAction), (ComponentRegistry) null, true);
      if (this._placementMan.CurrentPermission.IsTile)
      {
        if (!(this._tileMan[this._placementMan.CurrentPermission.TileType] is ContentTileDefinition contentTileDefinition))
          return;
        if (!contentTileDefinition.MapAtmosphere)
        {
          ResPath? sprite = contentTileDefinition.Sprite;
          if (sprite.HasValue)
          {
            ResPath valueOrDefault = sprite.GetValueOrDefault();
            this._actions.SetIcon(Entity<ActionComponent>.op_Implicit(uid), (SpriteSpecifier) new SpriteSpecifier.Texture(valueOrDefault));
          }
        }
        ev.TileId = ProtoId<ContentTileDefinition>.op_Implicit(contentTileDefinition.ID);
        this._metaData.SetEntityName(uid, this.Loc.GetString(contentTileDefinition.Name), (MetaDataComponent) null, true);
      }
      else
      {
        string entityType = currentPermission.EntityType;
        if (entityType != null)
        {
          this._actions.SetIcon(Entity<ActionComponent>.op_Implicit(uid), (SpriteSpecifier) new SpriteSpecifier.EntityPrototype(entityType));
          this._metaData.SetEntityName(uid, entityType, (MetaDataComponent) null, true);
        }
      }
      this._actions.SetEvent(uid, (BaseActionEvent) ev);
      args.Action = new EntityUid?(uid);
    }
    else
    {
      if (!this._placementMan.Eraser)
        return;
      args.Action = new EntityUid?(this.Spawn(EntProtoId.op_Implicit(MappingSystem.EraserAction), (ComponentRegistry) null, true));
    }
  }

  private void OnStartPlacementAction(StartPlacementActionEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    IPlacementManager placementMan = this._placementMan;
    PlacementInformation placementInformation = new PlacementInformation();
    EntProtoId? entityType = args.EntityType;
    placementInformation.EntityType = entityType.HasValue ? EntProtoId.op_Implicit(entityType.GetValueOrDefault()) : (string) null;
    placementInformation.IsTile = args.TileId.HasValue;
    int num;
    if (!args.TileId.HasValue)
    {
      num = 0;
    }
    else
    {
      ITileDefinitionManager tileMan = this._tileMan;
      ProtoId<ContentTileDefinition>? tileId = args.TileId;
      string str = tileId.HasValue ? ProtoId<ContentTileDefinition>.op_Implicit(tileId.GetValueOrDefault()) : (string) null;
      num = (int) tileMan[str].TileId;
    }
    placementInformation.TileType = num;
    placementInformation.PlacementOption = args.PlacementOption;
    placementMan.BeginPlacing(placementInformation, (PlacementHijack) null);
    if (this._placementMan.Eraser == args.Eraser)
      return;
    this._placementMan.ToggleEraser();
  }
}
