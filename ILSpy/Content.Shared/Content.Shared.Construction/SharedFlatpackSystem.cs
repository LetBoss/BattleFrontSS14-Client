using System;
using System.Collections.Generic;
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

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<FlatpackComponent, InteractUsingEvent>((EntityEventRefHandler<FlatpackComponent, InteractUsingEvent>)OnFlatpackInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlatpackComponent, ExaminedEvent>((EntityEventRefHandler<FlatpackComponent, ExaminedEvent>)OnFlatpackExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlatpackCreatorComponent, ItemSlotInsertAttemptEvent>((EntityEventRefHandler<FlatpackCreatorComponent, ItemSlotInsertAttemptEvent>)OnInsertAttempt, (Type[])null, (Type[])null);
	}

	private void OnInsertAttempt(Entity<FlatpackCreatorComponent> ent, ref ItemSlotInsertAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		ComputerBoardComponent computer = default(ComputerBoardComponent);
		if (!(args.Slot.ID != ent.Comp.SlotId) && !args.Cancelled && !((EntitySystem)this).HasComp<MachineBoardComponent>(args.Item) && (!((EntitySystem)this).TryComp<ComputerBoardComponent>(args.Item, ref computer) || computer.Prototype == null))
		{
			args.Cancelled = true;
		}
	}

	private void OnFlatpackInteractUsing(Entity<FlatpackComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		Entity<FlatpackComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		FlatpackComponent flatpackComponent = default(FlatpackComponent);
		val.Deconstruct(ref val2, ref flatpackComponent);
		EntityUid uid = val2;
		FlatpackComponent comp = flatpackComponent;
		if (!_tool.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(comp.QualityNeeded)) || _container.IsEntityInContainer(Entity<FlatpackComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			return;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(Entity<FlatpackComponent>.op_Implicit(ent));
		EntityUid? gridUid = xform.GridUid;
		if (!gridUid.HasValue)
		{
			return;
		}
		EntityUid grid = gridUid.GetValueOrDefault();
		MapGridComponent gridComp = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(grid, ref gridComp))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (!comp.Entity.HasValue)
		{
			((EntitySystem)this).Log.Error($"No entity prototype present for flatpack {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<FlatpackComponent>.op_Implicit(ent), (MetaDataComponent)null)}.");
			if (_net.IsServer)
			{
				((EntitySystem)this).QueueDel((EntityUid?)Entity<FlatpackComponent>.op_Implicit(ent));
			}
			return;
		}
		Vector2i buildPos = _map.TileIndicesFor(grid, gridComp, xform.Coordinates);
		EntityCoordinates coords = _map.ToCenterCoordinates(grid, buildPos, (MapGridComponent)null);
		if (_entityLookup.AnyEntitiesIntersecting(coords, (LookupFlags)6))
		{
			if (_net.IsServer)
			{
				_popup.PopupEntity(base.Loc.GetString("flatpack-unpack-no-room"), uid, args.User);
			}
			return;
		}
		if (_net.IsServer)
		{
			EntProtoId? entity = comp.Entity;
			EntityUid spawn = ((EntitySystem)this).Spawn(entity.HasValue ? EntProtoId.op_Implicit(entity.GetValueOrDefault()) : null, _map.GridTileToLocal(grid, gridComp, buildPos));
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(20, 4);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "player", "ToPrettyString(args.User)");
			handler.AppendLiteral(" unpacked ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(spawn)), "entity", "ToPrettyString(spawn)");
			handler.AppendLiteral(" at ");
			handler.AppendFormatted<EntityCoordinates>(xform.Coordinates, "xform.Coordinates");
			handler.AppendLiteral(" from ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "entity", "ToPrettyString(uid)");
			adminLogger.Add(LogType.Construction, LogImpact.Low, ref handler);
			((EntitySystem)this).QueueDel((EntityUid?)uid);
		}
		_audio.PlayPredicted(comp.UnpackSound, args.Used, (EntityUid?)args.User, (AudioParams?)null);
	}

	private void OnFlatpackExamined(Entity<FlatpackComponent> ent, ref ExaminedEvent args)
	{
		if (args.IsInDetailsRange)
		{
			args.PushMarkup(base.Loc.GetString("flatpack-examine"));
		}
	}

	protected void SetupFlatpack(Entity<FlatpackComponent?> ent, EntProtoId proto, EntityUid board)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<FlatpackComponent>(Entity<FlatpackComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			ent.Comp.Entity = proto;
			EntityPrototype machinePrototype = PrototypeManager.Index<EntityPrototype>(EntProtoId.op_Implicit(proto));
			MetaDataComponent meta = ((EntitySystem)this).MetaData(Entity<FlatpackComponent>.op_Implicit(ent));
			_metaData.SetEntityName(Entity<FlatpackComponent>.op_Implicit(ent), base.Loc.GetString("flatpack-entity-name", (ValueTuple<string, object>)("name", machinePrototype.Name)), meta, true);
			_metaData.SetEntityDescription(Entity<FlatpackComponent>.op_Implicit(ent), base.Loc.GetString("flatpack-entity-description", (ValueTuple<string, object>)("name", machinePrototype.Name)), meta);
			((EntitySystem)this).Dirty<FlatpackComponent>(ent, meta);
			SharedAppearanceSystem appearance = Appearance;
			EntityUid val = Entity<FlatpackComponent>.op_Implicit(ent);
			object obj = FlatpackVisuals.Machine;
			EntityPrototype entityPrototype = ((EntitySystem)this).MetaData(board).EntityPrototype;
			appearance.SetData(val, (Enum)obj, (object)(((entityPrototype != null) ? entityPrototype.ID : null) ?? string.Empty), (AppearanceComponent)null);
		}
	}

	public Dictionary<string, int> GetFlatpackCreationCost(Entity<FlatpackCreatorComponent> entity, Entity<MachineBoardComponent>? machineBoard)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, int> cost = new Dictionary<string, int>();
		Dictionary<ProtoId<MaterialPrototype>, int> baseCost;
		if (machineBoard.HasValue)
		{
			cost = MachinePart.GetMachineBoardMaterialCost(machineBoard.Value, -1);
			baseCost = entity.Comp.BaseMachineCost;
		}
		else
		{
			baseCost = entity.Comp.BaseComputerCost;
		}
		foreach (var (mat, amount) in baseCost)
		{
			cost.TryAdd(ProtoId<MaterialPrototype>.op_Implicit(mat), 0);
			cost[ProtoId<MaterialPrototype>.op_Implicit(mat)] -= amount;
		}
		return cost;
	}
}
