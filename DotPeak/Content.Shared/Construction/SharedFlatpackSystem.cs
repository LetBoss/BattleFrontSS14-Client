// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.SharedFlatpackSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Construction.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Materials;
using Content.Shared.Popups;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Construction;

public abstract class SharedFlatpackSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private INetManager _net;
  [Dependency]
  protected IPrototypeManager PrototypeManager;
  [Dependency]
  protected SharedAppearanceSystem Appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  protected MachinePartSystem MachinePart;
  [Dependency]
  protected SharedMaterialStorageSystem MaterialStorage;
  [Dependency]
  private MetaDataSystem _metaData;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedToolSystem _tool;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FlatpackComponent, InteractUsingEvent>(new EntityEventRefHandler<FlatpackComponent, InteractUsingEvent>((object) this, __methodptr(OnFlatpackInteractUsing)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FlatpackComponent, ExaminedEvent>(new EntityEventRefHandler<FlatpackComponent, ExaminedEvent>((object) this, __methodptr(OnFlatpackExamined)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FlatpackCreatorComponent, ItemSlotInsertAttemptEvent>(new EntityEventRefHandler<FlatpackCreatorComponent, ItemSlotInsertAttemptEvent>((object) this, __methodptr(OnInsertAttempt)), (Type[]) null, (Type[]) null);
  }

  private void OnInsertAttempt(
    Entity<FlatpackCreatorComponent> ent,
    ref ItemSlotInsertAttemptEvent args)
  {
    ComputerBoardComponent computerBoardComponent;
    if (args.Slot.ID != ent.Comp.SlotId || args.Cancelled || this.HasComp<MachineBoardComponent>(args.Item) || this.TryComp<ComputerBoardComponent>(args.Item, ref computerBoardComponent) && computerBoardComponent.Prototype != null)
      return;
    args.Cancelled = true;
  }

  private void OnFlatpackInteractUsing(Entity<FlatpackComponent> ent, ref InteractUsingEvent args)
  {
    EntityUid entityUid1;
    FlatpackComponent flatpackComponent1;
    ent.Deconstruct(ref entityUid1, ref flatpackComponent1);
    EntityUid uid = entityUid1;
    FlatpackComponent flatpackComponent2 = flatpackComponent1;
    if (!this._tool.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(flatpackComponent2.QualityNeeded)) || this._container.IsEntityInContainer(Entity<FlatpackComponent>.op_Implicit(ent), (MetaDataComponent) null))
      return;
    TransformComponent transformComponent = this.Transform(Entity<FlatpackComponent>.op_Implicit(ent));
    EntityUid? gridUid = transformComponent.GridUid;
    if (!gridUid.HasValue)
      return;
    EntityUid valueOrDefault = gridUid.GetValueOrDefault();
    MapGridComponent mapGridComponent;
    if (!this.TryComp<MapGridComponent>(valueOrDefault, ref mapGridComponent))
      return;
    args.Handled = true;
    if (!flatpackComponent2.Entity.HasValue)
    {
      this.Log.Error($"No entity prototype present for flatpack {this.ToPrettyString(new EntityUid?(Entity<FlatpackComponent>.op_Implicit(ent)), (MetaDataComponent) null)}.");
      if (!this._net.IsServer)
        return;
      this.QueueDel(new EntityUid?(Entity<FlatpackComponent>.op_Implicit(ent)));
    }
    else
    {
      Vector2i vector2i = this._map.TileIndicesFor(valueOrDefault, mapGridComponent, transformComponent.Coordinates);
      if (this._entityLookup.AnyEntitiesIntersecting(this._map.ToCenterCoordinates(valueOrDefault, vector2i, (MapGridComponent) null), (LookupFlags) 6))
      {
        if (!this._net.IsServer)
          return;
        this._popup.PopupEntity(this.Loc.GetString("flatpack-unpack-no-room"), uid, args.User);
      }
      else
      {
        if (this._net.IsServer)
        {
          EntProtoId? entity = flatpackComponent2.Entity;
          EntityUid entityUid2 = this.Spawn(entity.HasValue ? EntProtoId.op_Implicit(entity.GetValueOrDefault()) : (string) null, this._map.GridTileToLocal(valueOrDefault, mapGridComponent, vector2i));
          ISharedAdminLogManager adminLogger = this._adminLogger;
          LogStringHandler logStringHandler = new LogStringHandler(20, 4);
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "player", "ToPrettyString(args.User)");
          logStringHandler.AppendLiteral(" unpacked ");
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entityUid2)), "entity", "ToPrettyString(spawn)");
          logStringHandler.AppendLiteral(" at ");
          logStringHandler.AppendFormatted<EntityCoordinates>(transformComponent.Coordinates, "xform.Coordinates");
          logStringHandler.AppendLiteral(" from ");
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "entity", "ToPrettyString(uid)");
          ref LogStringHandler local = ref logStringHandler;
          adminLogger.Add(LogType.Construction, LogImpact.Low, ref local);
          this.QueueDel(new EntityUid?(uid));
        }
        this._audio.PlayPredicted(flatpackComponent2.UnpackSound, args.Used, new EntityUid?(args.User), new AudioParams?());
      }
    }
  }

  private void OnFlatpackExamined(Entity<FlatpackComponent> ent, ref ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    args.PushMarkup(this.Loc.GetString("flatpack-examine"));
  }

  protected void SetupFlatpack(Entity<FlatpackComponent?> ent, EntProtoId proto, EntityUid board)
  {
    if (!this.Resolve<FlatpackComponent>(Entity<FlatpackComponent>.op_Implicit(ent), ref ent.Comp, true))
      return;
    ent.Comp.Entity = new EntProtoId?(proto);
    EntityPrototype entityPrototype = this.PrototypeManager.Index<EntityPrototype>(EntProtoId.op_Implicit(proto));
    MetaDataComponent metaDataComponent = this.MetaData(Entity<FlatpackComponent>.op_Implicit(ent));
    this._metaData.SetEntityName(Entity<FlatpackComponent>.op_Implicit(ent), this.Loc.GetString("flatpack-entity-name", ("name", (object) entityPrototype.Name)), metaDataComponent, true);
    this._metaData.SetEntityDescription(Entity<FlatpackComponent>.op_Implicit(ent), this.Loc.GetString("flatpack-entity-description", ("name", (object) entityPrototype.Name)), metaDataComponent);
    this.Dirty<FlatpackComponent>(ent, metaDataComponent);
    this.Appearance.SetData(Entity<FlatpackComponent>.op_Implicit(ent), (Enum) FlatpackVisuals.Machine, (object) (this.MetaData(board).EntityPrototype?.ID ?? string.Empty), (AppearanceComponent) null);
  }

  public Dictionary<string, int> GetFlatpackCreationCost(
    Entity<FlatpackCreatorComponent> entity,
    Entity<MachineBoardComponent>? machineBoard)
  {
    Dictionary<string, int> flatpackCreationCost = new Dictionary<string, int>();
    Dictionary<ProtoId<MaterialPrototype>, int> dictionary;
    if (machineBoard.HasValue)
    {
      flatpackCreationCost = this.MachinePart.GetMachineBoardMaterialCost(machineBoard.Value, -1);
      dictionary = entity.Comp.BaseMachineCost;
    }
    else
      dictionary = entity.Comp.BaseComputerCost;
    foreach ((ProtoId<MaterialPrototype> key, int num) in dictionary)
    {
      flatpackCreationCost.TryAdd(ProtoId<MaterialPrototype>.op_Implicit(key), 0);
      flatpackCreationCost[ProtoId<MaterialPrototype>.op_Implicit(key)] -= num;
    }
    return flatpackCreationCost;
  }
}
