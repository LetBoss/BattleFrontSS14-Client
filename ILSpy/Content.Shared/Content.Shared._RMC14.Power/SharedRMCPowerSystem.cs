using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Sprite;
using Content.Shared._RMC14.Tools;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Access.Components;
using Content.Shared.Damage;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.PowerCell;
using Content.Shared.Toggleable;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Content.Shared.UserInterface;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Power;

public abstract class SharedRMCPowerSystem : EntitySystem
{
	private static readonly bool DisableApcChannelButtons = true;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private MetaDataSystem _metaData;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPointLightSystem _pointLight;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedPowerReceiverSystem _powerReceiver;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SharedRMCSpriteSystem _sprite;

	[Dependency]
	private SharedToolSystem _tool;

	[Dependency]
	private SharedTransformSystem _transform;

	protected readonly HashSet<EntityUid> ToUpdate = new HashSet<EntityUid>();

	private readonly Dictionary<MapId, List<EntityUid>> _reactorPoweredLights = new Dictionary<MapId, List<EntityUid>>();

	private readonly HashSet<MapId> _reactorsUpdated = new HashSet<MapId>();

	private bool _recalculate;

	private EntityQuery<RMCApcComponent> _apcQuery;

	private EntityQuery<AppearanceComponent> _appearanceQuery;

	private EntityQuery<RMCAreaPowerComponent> _areaPowerQuery;

	private EntityQuery<AreaComponent> _areaQuery;

	private EntityQuery<RMCPowerReceiverComponent> _powerReceiverQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		_apcQuery = ((EntitySystem)this).GetEntityQuery<RMCApcComponent>();
		_appearanceQuery = ((EntitySystem)this).GetEntityQuery<AppearanceComponent>();
		_areaPowerQuery = ((EntitySystem)this).GetEntityQuery<RMCAreaPowerComponent>();
		_areaQuery = ((EntitySystem)this).GetEntityQuery<AreaComponent>();
		_powerReceiverQuery = ((EntitySystem)this).GetEntityQuery<RMCPowerReceiverComponent>();
		((EntitySystem)this).SubscribeLocalEvent<RMCApcComponent, ComponentStartup>((EntityEventRefHandler<RMCApcComponent, ComponentStartup>)OnApcStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCApcComponent, MapInitEvent>((EntityEventRefHandler<RMCApcComponent, MapInitEvent>)OnApcUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCApcComponent, EntParentChangedMessage>((EntityEventRefHandler<RMCApcComponent, EntParentChangedMessage>)OnApcUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCApcComponent, ComponentRemove>((EntityEventRefHandler<RMCApcComponent, ComponentRemove>)OnApcRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCApcComponent, EntityTerminatingEvent>((EntityEventRefHandler<RMCApcComponent, EntityTerminatingEvent>)OnApcRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCApcComponent, BreakageEventArgs>((EntityEventRefHandler<RMCApcComponent, BreakageEventArgs>)OnApcBreakage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCApcComponent, InteractUsingEvent>((EntityEventRefHandler<RMCApcComponent, InteractUsingEvent>)OnApcInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCApcComponent, InteractHandEvent>((EntityEventRefHandler<RMCApcComponent, InteractHandEvent>)OnApcInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCApcComponent, ActivatableUIOpenAttemptEvent>((EntityEventRefHandler<RMCApcComponent, ActivatableUIOpenAttemptEvent>)OnApcActivatableUIOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCApcComponent, ExaminedEvent>((EntityEventRefHandler<RMCApcComponent, ExaminedEvent>)OnApcExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCPowerReceiverComponent, MapInitEvent>((EntityEventRefHandler<RMCPowerReceiverComponent, MapInitEvent>)OnReceiverMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCPowerReceiverComponent, EntParentChangedMessage>((EntityEventRefHandler<RMCPowerReceiverComponent, EntParentChangedMessage>)OnReceiverUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCPowerReceiverComponent, ComponentRemove>((EntityEventRefHandler<RMCPowerReceiverComponent, ComponentRemove>)OnReceiverRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCPowerReceiverComponent, EntityTerminatingEvent>((EntityEventRefHandler<RMCPowerReceiverComponent, EntityTerminatingEvent>)OnReceiverRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFusionReactorComponent, MapInitEvent>((EntityEventRefHandler<RMCFusionReactorComponent, MapInitEvent>)OnFusionReactorMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFusionReactorComponent, InteractUsingEvent>((EntityEventRefHandler<RMCFusionReactorComponent, InteractUsingEvent>)OnFusionReactorInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFusionReactorComponent, RMCFusionReactorCellDoAfterEvent>((EntityEventRefHandler<RMCFusionReactorComponent, RMCFusionReactorCellDoAfterEvent>)OnFusionReactorCellDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFusionReactorComponent, RMCFusionReactorRemoveCellDoAfterEvent>((EntityEventRefHandler<RMCFusionReactorComponent, RMCFusionReactorRemoveCellDoAfterEvent>)OnFusionReactorRemoveCellDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFusionReactorComponent, RMCFusionReactorRepairDoAfterEvent>((EntityEventRefHandler<RMCFusionReactorComponent, RMCFusionReactorRepairDoAfterEvent>)OnFusionReactorRepairWeldingDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFusionReactorComponent, InteractHandEvent>((EntityEventRefHandler<RMCFusionReactorComponent, InteractHandEvent>)OnFusionReactorInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFusionReactorComponent, RMCFusionReactorDestroyDoAfterEvent>((EntityEventRefHandler<RMCFusionReactorComponent, RMCFusionReactorDestroyDoAfterEvent>)OnFusionReactorDestroyDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFusionReactorComponent, ExaminedEvent>((EntityEventRefHandler<RMCFusionReactorComponent, ExaminedEvent>)OnFusionReactorExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCReactorPoweredLightComponent, MapInitEvent>((EntityEventRefHandler<RMCReactorPoweredLightComponent, MapInitEvent>)OnReactorPoweredLightMapInit, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<RMCApcComponent>(((EntitySystem)this).Subs, (object)RMCApcUiKey.Key, (BuiEventSubscriber<RMCApcComponent>)delegate(Subscriber<RMCApcComponent> subs)
		{
			subs.Event<RMCApcSetChannelBuiMsg>((EntityEventRefHandler<RMCApcComponent, RMCApcSetChannelBuiMsg>)OnApcSetChannelBuiMsg);
			subs.Event<RMCApcCoverBuiMsg>((EntityEventRefHandler<RMCApcComponent, RMCApcCoverBuiMsg>)OnApcCover);
		});
	}

	private void OnApcStartup(Entity<RMCApcComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OffsetApc(ent);
	}

	private void OnApcUpdate<T>(Entity<RMCApcComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		MetaDataComponent metaData = default(MetaDataComponent);
		if (!((EntitySystem)this).TryComp(Entity<RMCApcComponent>.op_Implicit(ent), ref metaData) || (int)metaData.EntityLifeStage < 3)
		{
			return;
		}
		ToUpdate.Add(Entity<RMCApcComponent>.op_Implicit(ent));
		if (!_net.IsClient && !((EntitySystem)this).TerminatingOrDeleted(Entity<RMCApcComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			if (_area.TryGetArea(Entity<RMCApcComponent>.op_Implicit(ent), out Entity<AreaComponent>? _, out EntityPrototype areaProto))
			{
				_metaData.SetEntityName(Entity<RMCApcComponent>.op_Implicit(ent), areaProto.Name + " APC", (MetaDataComponent)null, true);
			}
			_container.EnsureContainer<ContainerSlot>(Entity<RMCApcComponent>.op_Implicit(ent), ent.Comp.CellContainerSlot, (ContainerManagerComponent)null);
			EntProtoId<PowerCellComponent>? startingCell = ent.Comp.StartingCell;
			if (startingCell.HasValue)
			{
				EntProtoId<PowerCellComponent> startingCell2 = startingCell.GetValueOrDefault();
				EntityUid? val = default(EntityUid?);
				((EntitySystem)this).TrySpawnInContainer(EntProtoId<PowerCellComponent>.op_Implicit(startingCell2), Entity<RMCApcComponent>.op_Implicit(ent), ent.Comp.CellContainerSlot, ref val, (ContainerManagerComponent)null, (ComponentRegistry)null);
			}
			OffsetApc(ent);
		}
	}

	private void OnApcRemove<T>(Entity<RMCApcComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		RMCAreaPowerComponent map = default(RMCAreaPowerComponent);
		if (!((EntitySystem)this).TerminatingOrDeleted(ent.Comp.Area, (MetaDataComponent)null) && _areaPowerQuery.TryComp(ent.Comp.Area, ref map))
		{
			map.Apcs.Remove(Entity<RMCApcComponent>.op_Implicit(ent));
			((EntitySystem)this).Dirty(ent.Comp.Area.Value, (IComponent)(object)map, (MetaDataComponent)null);
		}
	}

	private void OnApcBreakage(Entity<RMCApcComponent> ent, ref BreakageEventArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.State = RMCApcState.WiresExposed;
		ent.Comp.Broken = true;
		((EntitySystem)this).Dirty<RMCApcComponent>(ent, (MetaDataComponent)null);
		_appearance.SetData(Entity<RMCApcComponent>.op_Implicit(ent), (Enum)RMCApcVisualsLayers.Layer, (object)RMCApcState.WiresExposed, (AppearanceComponent)null);
	}

	private void OnApcInteractUsing(Entity<RMCApcComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (!_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(user), ent.Comp.Skill, ent.Comp.SkillLevel))
		{
			_popup.PopupClient("You don't know how to use the " + ((EntitySystem)this).Name(Entity<RMCApcComponent>.op_Implicit(ent), (MetaDataComponent)null) + "'s interface.", Entity<RMCApcComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
			return;
		}
		EntityUid used = args.Used;
		if (_tool.HasQuality(used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.CrowbarTool)))
		{
			switch (ent.Comp.State)
			{
			case RMCApcState.Working:
			case RMCApcState.WiresExposed:
			{
				if (ent.Comp.CoverLockedButton)
				{
					_popup.PopupClient("The cover is locked and cannot be opened.", user, user, PopupType.MediumCaution);
					return;
				}
				BaseContainer container = default(BaseContainer);
				ent.Comp.State = ((_container.TryGetContainer(Entity<RMCApcComponent>.op_Implicit(ent), ent.Comp.CellContainerSlot, ref container, (ContainerManagerComponent)null) && container.ContainedEntities.Count > 0) ? RMCApcState.CoverOpenBattery : RMCApcState.CoverOpenNoBattery);
				((EntitySystem)this).Dirty<RMCApcComponent>(ent, (MetaDataComponent)null);
				_appearance.SetData(Entity<RMCApcComponent>.op_Implicit(ent), (Enum)RMCApcVisualsLayers.Layer, (object)ent.Comp.State, (AppearanceComponent)null);
				break;
			}
			case RMCApcState.CoverOpenBattery:
			case RMCApcState.CoverOpenNoBattery:
				ent.Comp.State = RMCApcState.Working;
				((EntitySystem)this).Dirty<RMCApcComponent>(ent, (MetaDataComponent)null);
				_appearance.SetData(Entity<RMCApcComponent>.op_Implicit(ent), (Enum)RMCApcVisualsLayers.Layer, (object)ent.Comp.State, (AppearanceComponent)null);
				break;
			}
		}
		if (((EntitySystem)this).HasComp<PowerCellComponent>(used) && ent.Comp.State == RMCApcState.CoverOpenNoBattery)
		{
			ContainerSlot container2 = _container.EnsureContainer<ContainerSlot>(Entity<RMCApcComponent>.op_Implicit(ent), ent.Comp.CellContainerSlot, (ContainerManagerComponent)null);
			_hands.TryDropIntoContainer(Entity<HandsComponent>.op_Implicit(user), used, (BaseContainer)(object)container2);
			if (((BaseContainer)container2).ContainedEntities.Count > 0)
			{
				ent.Comp.State = RMCApcState.CoverOpenBattery;
				((EntitySystem)this).Dirty<RMCApcComponent>(ent, (MetaDataComponent)null);
				_appearance.SetData(Entity<RMCApcComponent>.op_Implicit(ent), (Enum)RMCApcVisualsLayers.Layer, (object)ent.Comp.State, (AppearanceComponent)null);
				ToUpdate.Add(Entity<RMCApcComponent>.op_Implicit(ent));
			}
			return;
		}
		AccessComponent access = default(AccessComponent);
		if (((EntitySystem)this).TryComp<AccessComponent>(used, ref access))
		{
			ent.Comp.Locked = !ent.Comp.Locked;
			((EntitySystem)this).Dirty<RMCApcComponent>(ent, (MetaDataComponent)null);
		}
		if (_tool.HasQuality(used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.RepairTool)))
		{
			RMCApcComponent comp = ent.Comp;
			comp.State = ent.Comp.State switch
			{
				RMCApcState.Working => RMCApcState.WiresExposed, 
				RMCApcState.WiresExposed => RMCApcState.Working, 
				_ => ent.Comp.State, 
			};
			ent.Comp.Broken = false;
			((EntitySystem)this).Dirty<RMCApcComponent>(ent, (MetaDataComponent)null);
			_appearance.SetData(Entity<RMCApcComponent>.op_Implicit(ent), (Enum)RMCApcVisualsLayers.Layer, (object)ent.Comp.State, (AppearanceComponent)null);
			DamageableComponent damageable = default(DamageableComponent);
			if (((EntitySystem)this).TryComp<DamageableComponent>(Entity<RMCApcComponent>.op_Implicit(ent), ref damageable))
			{
				_damageable.SetAllDamage(Entity<RMCApcComponent>.op_Implicit(ent), damageable, FixedPoint2.Zero);
			}
		}
	}

	private void OnApcInteractHand(Entity<RMCApcComponent> ent, ref InteractHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (ent.Comp.State != RMCApcState.CoverOpenBattery || !_container.TryGetContainer(Entity<RMCApcComponent>.op_Implicit(ent), ent.Comp.CellContainerSlot, ref container, (ContainerManagerComponent)null))
		{
			return;
		}
		foreach (EntityUid contained in container.ContainedEntities)
		{
			if (_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(contained), container, true, false, (EntityCoordinates?)null, (Angle?)null))
			{
				_hands.TryPickupAnyHand(args.User, contained);
				ent.Comp.State = RMCApcState.CoverOpenNoBattery;
				ent.Comp.ChargePercentage = 0f;
				((EntitySystem)this).Dirty<RMCApcComponent>(ent, (MetaDataComponent)null);
				_appearance.SetData(Entity<RMCApcComponent>.op_Implicit(ent), (Enum)RMCApcVisualsLayers.Layer, (object)ent.Comp.State, (AppearanceComponent)null);
				ToUpdate.Add(Entity<RMCApcComponent>.op_Implicit(ent));
				break;
			}
		}
	}

	private void OnApcActivatableUIOpenAttempt(Entity<RMCApcComponent> ent, ref ActivatableUIOpenAttemptEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled)
		{
			if (!_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.Skill, ent.Comp.SkillLevel))
			{
				((CancellableEntityEventArgs)args).Cancel();
				_popup.PopupClient("You don't know how to use the " + ((EntitySystem)this).Name(Entity<RMCApcComponent>.op_Implicit(ent), (MetaDataComponent)null) + "'s interface.", Entity<RMCApcComponent>.op_Implicit(ent), args.User, PopupType.SmallCaution);
			}
			else if (ent.Comp.State != RMCApcState.Working)
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	private void OnApcExamined(Entity<RMCApcComponent> ent, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			return;
		}
		using (args.PushGroup("RMCApcComponent"))
		{
			string markup = ent.Comp.State switch
			{
				RMCApcState.Working => "Use:\n- An [color=cyan]engineering ID[/color] to lock or unlock the interface.\n- A [color=cyan]crowbar[/color] to open the cover.\n- A [color=cyan]screwdriver[/color] to expose the wires.", 
				RMCApcState.WiresExposed => "Use a [color=cyan]screwdriver[/color] to unexpose the wires or a [color=cyan]crowbar[/color] to open the cover!", 
				RMCApcState.CoverOpenBattery => "Use an [color=cyan]empty hand[/color] to remove the battery or a [color=cyan]crowbar[/color] to close the cover!", 
				RMCApcState.CoverOpenNoBattery => "Use a [color=cyan]battery[/color] to put in a battery!", 
				_ => null, 
			};
			if (markup != null)
			{
				args.PushMarkup(markup);
			}
		}
	}

	protected virtual void OnReceiverMapInit(Entity<RMCPowerReceiverComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnReceiverUpdate(ent, ref args);
	}

	private void OnReceiverUpdate<T>(Entity<RMCPowerReceiverComponent> ent, ref T args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ToUpdate.Add(Entity<RMCPowerReceiverComponent>.op_Implicit(ent));
	}

	private void OnReceiverRemove<T>(Entity<RMCPowerReceiverComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetPowerArea(Entity<RMCPowerReceiverComponent>.op_Implicit(ent), out Entity<RMCAreaPowerComponent> area) && !((EntitySystem)this).TerminatingOrDeleted(Entity<RMCAreaPowerComponent>.op_Implicit(area), (MetaDataComponent)null))
		{
			GetAreaReceivers(area, ent.Comp.Channel).Remove(Entity<RMCPowerReceiverComponent>.op_Implicit(ent));
		}
	}

	private void OnFusionReactorMapInit(Entity<RMCFusionReactorComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		_container.EnsureContainer<ContainerSlot>(Entity<RMCFusionReactorComponent>.op_Implicit(ent), ent.Comp.CellContainerSlot, (ContainerManagerComponent)null);
		EntProtoId<RMCFusionCellComponent>? startingCell = ent.Comp.StartingCell;
		if (startingCell.HasValue)
		{
			EntProtoId<RMCFusionCellComponent> startingCell2 = startingCell.GetValueOrDefault();
			EntityUid? val = default(EntityUid?);
			((EntitySystem)this).TrySpawnInContainer(EntProtoId<RMCFusionCellComponent>.op_Implicit(startingCell2), Entity<RMCFusionReactorComponent>.op_Implicit(ent), ent.Comp.CellContainerSlot, ref val, (ContainerManagerComponent)null, (ComponentRegistry)null);
		}
		if (ent.Comp.RandomizeDamage)
		{
			double random = _random.NextDouble();
			if (random < 0.5)
			{
				ent.Comp.State = RMCFusionReactorState.Weld;
			}
			else if (random < 0.85)
			{
				ent.Comp.State = RMCFusionReactorState.Wire;
			}
			else
			{
				ent.Comp.State = RMCFusionReactorState.Wrench;
			}
			((EntitySystem)this).Dirty<RMCFusionReactorComponent>(ent, (MetaDataComponent)null);
		}
		UpdateAppearance(ent);
		ReactorUpdated(ent);
	}

	private void OnFusionReactorInteractUsing(Entity<RMCFusionReactorComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		EntityUid used = args.Used;
		((HandledEntityEventArgs)args).Handled = true;
		ContainerSlot container = _container.EnsureContainer<ContainerSlot>(Entity<RMCFusionReactorComponent>.op_Implicit(ent), ent.Comp.CellContainerSlot, (ContainerManagerComponent)null);
		RMCDeviceBreakerComponent breaker = default(RMCDeviceBreakerComponent);
		if (((EntitySystem)this).HasComp<RMCFusionCellComponent>(used))
		{
			if (container.ContainedEntity.HasValue)
			{
				string msg = base.Loc.GetString("rmc-fusion-reactor-insert-already-has-cell", (ValueTuple<string, object>)("reactor", ent));
				_popup.PopupClient(msg, Entity<RMCFusionReactorComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
				return;
			}
			RMCFusionReactorCellDoAfterEvent ev = new RMCFusionReactorCellDoAfterEvent();
			TimeSpan delay = ent.Comp.CellDelay * _skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(user), ent.Comp.Skill);
			EntityManager entityManager = base.EntityManager;
			EntityUid? eventTarget = Entity<RMCFusionReactorComponent>.op_Implicit(ent);
			EntityUid? used2 = used;
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)entityManager, user, delay, ev, eventTarget, null, used2)
			{
				BreakOnMove = true,
				DuplicateCondition = DuplicateConditions.SameEvent
			};
			if (_doAfter.TryStartDoAfter(doAfter))
			{
				string msg2 = base.Loc.GetString("rmc-fusion-reactor-insert-start-self", (ValueTuple<string, object>)("cell", used), (ValueTuple<string, object>)("reactor", ent));
				_popup.PopupClient(msg2, Entity<RMCFusionReactorComponent>.op_Implicit(ent), user);
			}
		}
		else if (_tool.HasQuality(used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.CrowbarQuality)))
		{
			if (!container.ContainedEntity.HasValue)
			{
				string msg3 = base.Loc.GetString("rmc-fusion-reactor-remove-none", (ValueTuple<string, object>)("reactor", ent));
				_popup.PopupClient(msg3, Entity<RMCFusionReactorComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
				return;
			}
			RMCFusionReactorRemoveCellDoAfterEvent ev2 = new RMCFusionReactorRemoveCellDoAfterEvent();
			TimeSpan delay2 = ent.Comp.CellDelay * _skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(user), ent.Comp.Skill);
			EntityManager entityManager2 = base.EntityManager;
			EntityUid? eventTarget2 = Entity<RMCFusionReactorComponent>.op_Implicit(ent);
			EntityUid? used2 = used;
			DoAfterArgs doAfter2 = new DoAfterArgs((IEntityManager)(object)entityManager2, user, delay2, ev2, eventTarget2, null, used2)
			{
				BreakOnMove = true,
				DuplicateCondition = DuplicateConditions.SameEvent
			};
			if (_doAfter.TryStartDoAfter(doAfter2))
			{
				string msg4 = base.Loc.GetString("rmc-fusion-reactor-remove-start-self", (ValueTuple<string, object>)("cell", container.ContainedEntity.Value), (ValueTuple<string, object>)("reactor", ent));
				_popup.PopupClient(msg4, Entity<RMCFusionReactorComponent>.op_Implicit(ent), user);
			}
		}
		else if (_tool.HasQuality(used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.WeldingQuality)))
		{
			TryRepair(ent, user, used, RMCFusionReactorState.Weld);
		}
		else if (_tool.HasQuality(used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.CuttingQuality)))
		{
			TryRepair(ent, user, used, RMCFusionReactorState.Wire);
		}
		else if (_tool.HasQuality(used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.WrenchQuality)))
		{
			TryRepair(ent, user, used, RMCFusionReactorState.Wrench);
		}
		else if (((EntitySystem)this).TryComp<RMCDeviceBreakerComponent>(used, ref breaker) && ent.Comp.State != RMCFusionReactorState.Weld)
		{
			DoAfterArgs doafter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, breaker.DoAfterTime, new RMCDeviceBreakerDoAfterEvent(), args.Used, args.Target, args.Used)
			{
				BreakOnMove = true,
				RequireCanInteract = true,
				BreakOnHandChange = true,
				DuplicateCondition = DuplicateConditions.SameTool
			};
			_doAfter.TryStartDoAfter(doafter);
		}
	}

	private void OnFusionReactorCellDoAfter(Entity<RMCFusionReactorComponent> ent, ref RMCFusionReactorCellDoAfterEvent args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? used = args.Used;
		if (used.HasValue)
		{
			EntityUid used2 = used.GetValueOrDefault();
			((HandledEntityEventArgs)args).Handled = true;
			EntityUid user = args.User;
			ContainerSlot container = _container.EnsureContainer<ContainerSlot>(Entity<RMCFusionReactorComponent>.op_Implicit(ent), ent.Comp.CellContainerSlot, (ContainerManagerComponent)null);
			if (!_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(used2), (BaseContainer)(object)container, (TransformComponent)null, false))
			{
				string msg = base.Loc.GetString("rmc-fusion-reactor-insert-fail-self", (ValueTuple<string, object>)("cell", used2), (ValueTuple<string, object>)("reactor", ent));
				_popup.PopupClient(msg, Entity<RMCFusionReactorComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
			}
			else
			{
				string msg = base.Loc.GetString("rmc-fusion-reactor-insert-finish-self", (ValueTuple<string, object>)("cell", used2), (ValueTuple<string, object>)("reactor", ent));
				_popup.PopupClient(msg, Entity<RMCFusionReactorComponent>.op_Implicit(ent), user);
				UpdateAppearance(ent);
			}
		}
	}

	private void OnFusionReactorRemoveCellDoAfter(Entity<RMCFusionReactorComponent> ent, ref RMCFusionReactorRemoveCellDoAfterEvent args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid user = args.User;
		ContainerSlot container = _container.EnsureContainer<ContainerSlot>(Entity<RMCFusionReactorComponent>.op_Implicit(ent), ent.Comp.CellContainerSlot, (ContainerManagerComponent)null);
		EntityUid? containedEntity = container.ContainedEntity;
		if (containedEntity.HasValue)
		{
			EntityUid cell = containedEntity.GetValueOrDefault();
			if (_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(cell), (BaseContainer)(object)container, true, false, (EntityCoordinates?)null, (Angle?)null))
			{
				_hands.TryPickupAnyHand(user, cell);
			}
			string msg = base.Loc.GetString("rmc-fusion-reactor-remove-finish-self", (ValueTuple<string, object>)("cell", cell), (ValueTuple<string, object>)("reactor", ent));
			_popup.PopupClient(msg, Entity<RMCFusionReactorComponent>.op_Implicit(ent), user);
			UpdateAppearance(ent);
		}
		else
		{
			string msg = base.Loc.GetString("rmc-fusion-reactor-remove-none", (ValueTuple<string, object>)("reactor", ent));
			_popup.PopupClient(msg, Entity<RMCFusionReactorComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
		}
	}

	private void OnFusionReactorRepairWeldingDoAfter(Entity<RMCFusionReactorComponent> ent, ref RMCFusionReactorRepairDoAfterEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (ent.Comp.State == args.State)
			{
				RMCFusionReactorComponent comp = ent.Comp;
				comp.State = args.State switch
				{
					RMCFusionReactorState.Wrench => RMCFusionReactorState.Working, 
					RMCFusionReactorState.Wire => RMCFusionReactorState.Wrench, 
					RMCFusionReactorState.Weld => RMCFusionReactorState.Wire, 
					_ => throw new ArgumentOutOfRangeException(), 
				};
				((EntitySystem)this).Dirty<RMCFusionReactorComponent>(ent, (MetaDataComponent)null);
				UpdateAppearance(ent);
				ReactorUpdated(ent);
			}
		}
	}

	private void OnFusionReactorInteractHand(Entity<RMCFusionReactorComponent> ent, ref InteractHandEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (((EntitySystem)this).HasComp<XenoComponent>(user) && ((EntitySystem)this).HasComp<MeleeWeaponComponent>(user))
		{
			if (ent.Comp.State == RMCFusionReactorState.Weld)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-fusion-reactor-already-destroyed", (ValueTuple<string, object>)("reactor", ent)), Entity<RMCFusionReactorComponent>.op_Implicit(ent), user);
				return;
			}
			RMCFusionReactorDestroyDoAfterEvent ev = new RMCFusionReactorDestroyDoAfterEvent();
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, ent.Comp.DestroyDelay, ev, Entity<RMCFusionReactorComponent>.op_Implicit(ent), Entity<RMCFusionReactorComponent>.op_Implicit(ent))
			{
				BreakOnMove = true,
				DuplicateCondition = DuplicateConditions.SameEvent
			};
			_doAfter.TryStartDoAfter(doAfter);
		}
	}

	private void OnFusionReactorDestroyDoAfter(Entity<RMCFusionReactorComponent> ent, ref RMCFusionReactorDestroyDoAfterEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		if (ent.Comp.State == RMCFusionReactorState.Weld)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-fusion-reactor-already-destroyed", (ValueTuple<string, object>)("reactor", ent)), Entity<RMCFusionReactorComponent>.op_Implicit(ent), user);
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		DestroyReactor(ent, args.User);
		if (ent.Comp.State != RMCFusionReactorState.Weld)
		{
			args.Repeat = true;
		}
	}

	public void DestroyReactor(Entity<RMCFusionReactorComponent> ent, EntityUid? user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		RMCFusionReactorComponent comp = ent.Comp;
		comp.State = ent.Comp.State switch
		{
			RMCFusionReactorState.Working => RMCFusionReactorState.Wrench, 
			RMCFusionReactorState.Wrench => RMCFusionReactorState.Wire, 
			RMCFusionReactorState.Wire => RMCFusionReactorState.Weld, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		((EntitySystem)this).Dirty<RMCFusionReactorComponent>(ent, (MetaDataComponent)null);
		UpdateAppearance(ent);
		_popup.PopupClient(base.Loc.GetString("rmc-fusion-reactor-destroyed", (ValueTuple<string, object>)("reactor", ent)), Entity<RMCFusionReactorComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
		ReactorUpdated(ent);
	}

	private void OnFusionReactorExamined(Entity<RMCFusionReactorComponent> ent, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			return;
		}
		using (args.PushGroup("RMCFusionReactorComponent"))
		{
			if (ent.Comp.State != RMCFusionReactorState.Working)
			{
				string tool = ent.Comp.State switch
				{
					RMCFusionReactorState.Wrench => "a [color=cyan]Wrench[/color]", 
					RMCFusionReactorState.Wire => "[color=cyan]Wirecutters[/color]", 
					RMCFusionReactorState.Weld => "a [color=cyan]Welder[/color]", 
					_ => throw new ArgumentOutOfRangeException(), 
				};
				args.PushMarkup("Use " + tool + " to repair it!");
			}
			BaseContainer container = default(BaseContainer);
			if (!_container.TryGetContainer(Entity<RMCFusionReactorComponent>.op_Implicit(ent), ent.Comp.CellContainerSlot, ref container, (ContainerManagerComponent)null) || container.ContainedEntities.Count == 0)
			{
				args.PushMarkup("It needs a [color=cyan]fuel cell[/color]!");
			}
		}
	}

	private void OnReactorPoweredLightMapInit(Entity<RMCReactorPoweredLightComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (((EntitySystem)this).TryComp(Entity<RMCReactorPoweredLightComponent>.op_Implicit(ent), ref xform))
		{
			Extensions.GetOrNew<MapId, List<EntityUid>>(_reactorPoweredLights, xform.MapID).Add(Entity<RMCReactorPoweredLightComponent>.op_Implicit(ent));
		}
	}

	private void OnApcSetChannelBuiMsg(Entity<RMCApcComponent> ent, ref RMCApcSetChannelBuiMsg args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (!DisableApcChannelButtons)
		{
			int channel = (int)args.Channel;
			if (args.Channel >= RMCPowerChannel.Equipment && channel < ent.Comp.Channels.Length)
			{
				ent.Comp.Channels[channel].Button = args.State;
				((EntitySystem)this).Dirty<RMCApcComponent>(ent, (MetaDataComponent)null);
			}
		}
	}

	private void OnApcCover(Entity<RMCApcComponent> ent, ref RMCApcCoverBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.State == RMCApcState.Working && !ent.Comp.Locked)
		{
			ent.Comp.CoverLockedButton = !ent.Comp.CoverLockedButton;
			((EntitySystem)this).Dirty<RMCApcComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void UpdateAppearance(Entity<RMCFusionReactorComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		switch (ent.Comp.State)
		{
		case RMCFusionReactorState.Weld:
			_appearance.SetData(Entity<RMCFusionReactorComponent>.op_Implicit(ent), (Enum)RMCFusionReactorLayers.Layer, (object)RMCFusionReactorVisuals.Weld, (AppearanceComponent)null);
			return;
		case RMCFusionReactorState.Wire:
			_appearance.SetData(Entity<RMCFusionReactorComponent>.op_Implicit(ent), (Enum)RMCFusionReactorLayers.Layer, (object)RMCFusionReactorVisuals.Wire, (AppearanceComponent)null);
			return;
		case RMCFusionReactorState.Wrench:
			_appearance.SetData(Entity<RMCFusionReactorComponent>.op_Implicit(ent), (Enum)RMCFusionReactorLayers.Layer, (object)RMCFusionReactorVisuals.Wrench, (AppearanceComponent)null);
			return;
		}
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<RMCFusionReactorComponent>.op_Implicit(ent), ent.Comp.CellContainerSlot, ref container, (ContainerManagerComponent)null) || container.ContainedEntities.Count == 0)
		{
			_appearance.SetData(Entity<RMCFusionReactorComponent>.op_Implicit(ent), (Enum)RMCFusionReactorLayers.Layer, (object)RMCFusionReactorVisuals.Empty, (AppearanceComponent)null);
		}
		else
		{
			_appearance.SetData(Entity<RMCFusionReactorComponent>.op_Implicit(ent), (Enum)RMCFusionReactorLayers.Layer, (object)RMCFusionReactorVisuals.Hundred, (AppearanceComponent)null);
		}
	}

	private void TryRepair(Entity<RMCFusionReactorComponent> ent, EntityUid user, EntityUid used, RMCFusionReactorState state)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.State == RMCFusionReactorState.Working)
		{
			string msg = base.Loc.GetString("rmc-fusion-reactor-repair-not-needed", (ValueTuple<string, object>)("reactor", ent));
			_popup.PopupClient(msg, Entity<RMCFusionReactorComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
			return;
		}
		if (ent.Comp.State != state)
		{
			string msg = base.Loc.GetString("rmc-fusion-reactor-repair-different-tool", (ValueTuple<string, object>)("reactor", ent));
			_popup.PopupClient(msg, Entity<RMCFusionReactorComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
			return;
		}
		ProtoId<ToolQualityPrototype> quality = (ProtoId<ToolQualityPrototype>)(state switch
		{
			RMCFusionReactorState.Wrench => ent.Comp.WrenchQuality, 
			RMCFusionReactorState.Wire => ent.Comp.CuttingQuality, 
			RMCFusionReactorState.Weld => ent.Comp.WeldingQuality, 
			_ => throw new ArgumentOutOfRangeException("state", state, null), 
		});
		if (_tool.UseTool(used, user, Entity<RMCFusionReactorComponent>.op_Implicit(ent), (float)ent.Comp.RepairDelay.TotalSeconds, ProtoId<ToolQualityPrototype>.op_Implicit(quality), new RMCFusionReactorRepairDoAfterEvent(state), ent.Comp.WeldingCost, null, DuplicateConditions.SameTool))
		{
			string msg = base.Loc.GetString("rmc-fusion-reactor-repair-start-self", (ValueTuple<string, object>)("reactor", ent), (ValueTuple<string, object>)("tool", used));
			_popup.PopupClient(msg, Entity<RMCFusionReactorComponent>.op_Implicit(ent), user);
		}
	}

	private bool TryGetPowerArea(EntityUid ent, out Entity<RMCAreaPowerComponent> areaPower)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		areaPower = default(Entity<RMCAreaPowerComponent>);
		if (!_area.TryGetArea(ent, out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			return false;
		}
		RMCAreaPowerComponent areaPowerComp = ((EntitySystem)this).EnsureComp<RMCAreaPowerComponent>(Entity<AreaComponent>.op_Implicit(area.Value));
		areaPower = Entity<RMCAreaPowerComponent>.op_Implicit((Entity<AreaComponent>.op_Implicit(area.Value), areaPowerComp));
		return true;
	}

	private int GetNewPowerLoad(Entity<RMCPowerReceiverComponent> receiver)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		return receiver.Comp.Mode switch
		{
			RMCPowerMode.Off => 0, 
			RMCPowerMode.Idle => receiver.Comp.IdleLoad, 
			RMCPowerMode.Active => receiver.Comp.ActiveLoad, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	protected HashSet<EntityUid> GetAreaReceivers(Entity<RMCAreaPowerComponent> area, RMCPowerChannel channel)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return channel switch
		{
			RMCPowerChannel.Equipment => area.Comp.EquipmentReceivers, 
			RMCPowerChannel.Lighting => area.Comp.LightingReceivers, 
			RMCPowerChannel.Environment => area.Comp.EnvironmentReceivers, 
			_ => throw new ArgumentOutOfRangeException("channel", channel, null), 
		};
	}

	protected void UpdateApcChannel(Entity<RMCApcComponent> apc, Entity<RMCAreaPowerComponent> area, RMCPowerChannel channel, bool on)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		ref RMCApcChannel apcChannel = ref apc.Comp.Channels[(int)channel];
		if (apcChannel.On != on)
		{
			if (apcChannel.Button == RMCApcButtonState.Auto || (apcChannel.Button == RMCApcButtonState.On && on) || (apcChannel.Button == RMCApcButtonState.Off && !on))
			{
				apcChannel.On = on;
			}
			PowerUpdated(area, channel, on);
		}
	}

	protected virtual void PowerUpdated(Entity<RMCAreaPowerComponent> area, RMCPowerChannel channel, bool on)
	{
	}

	public bool IsAreaPowered(Entity<RMCAreaPowerComponent?> area, RMCPowerChannel channel)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (!_areaPowerQuery.Resolve(Entity<RMCAreaPowerComponent>.op_Implicit(area), ref area.Comp, false))
		{
			return false;
		}
		AreaComponent areaComponent = default(AreaComponent);
		if (_areaQuery.TryComp(Entity<RMCAreaPowerComponent>.op_Implicit(area), ref areaComponent) && areaComponent.AlwaysPowered)
		{
			return true;
		}
		RMCApcComponent apc = default(RMCApcComponent);
		foreach (EntityUid apcId in area.Comp.Apcs)
		{
			if (_apcQuery.TryComp(apcId, ref apc) && apc.Channels[(int)channel].On)
			{
				return true;
			}
		}
		return false;
	}

	public abstract bool IsPowered(EntityUid ent);

	private bool AnyReactorsOn(MapId map)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<RMCFusionReactorComponent, TransformComponent> reactors = ((EntitySystem)this).EntityQueryEnumerator<RMCFusionReactorComponent, TransformComponent>();
		RMCFusionReactorComponent comp = default(RMCFusionReactorComponent);
		TransformComponent xform = default(TransformComponent);
		while (reactors.MoveNext(ref comp, ref xform))
		{
			if (comp.State == RMCFusionReactorState.Working && xform.MapID == map)
			{
				return true;
			}
		}
		return false;
	}

	private void ReactorUpdated(Entity<RMCFusionReactorComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		MapId mapId = _transform.GetMapId(Entity<TransformComponent>.op_Implicit(ent.Owner));
		_reactorsUpdated.Add(mapId);
	}

	protected void UpdateReceiverPower(EntityUid receiver, ref PowerChangedEvent ev)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		SharedApcPowerReceiverComponent receiverComp = null;
		if (_powerReceiver.ResolveApc(receiver, ref receiverComp) && receiverComp.Powered != ev.Powered && receiverComp.NeedsPower)
		{
			receiverComp.Powered = ev.Powered;
			((EntitySystem)this).Dirty(receiver, (IComponent)(object)receiverComp, (MetaDataComponent)null);
			((EntitySystem)this).RaiseLocalEvent<PowerChangedEvent>(receiver, ref ev, false);
			AppearanceComponent appearance = default(AppearanceComponent);
			if (_appearanceQuery.TryComp(receiver, ref appearance))
			{
				_appearance.SetData(receiver, (Enum)PowerDeviceVisuals.Powered, (object)ev.Powered, appearance);
			}
		}
	}

	public void RecalculatePower()
	{
		_recalculate = true;
	}

	private void OffsetApc(Entity<RMCApcComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected I4, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		SpriteSetRenderOrderComponent sprite = ((EntitySystem)this).EnsureComp<SpriteSetRenderOrderComponent>(Entity<RMCApcComponent>.op_Implicit(ent));
		Angle localRotation = ((EntitySystem)this).Transform(Entity<RMCApcComponent>.op_Implicit(ent)).LocalRotation;
		Direction dir = ((Angle)(ref localRotation)).GetDir();
		switch ((int)dir)
		{
		case 0:
			_sprite.SetOffset(Entity<RMCApcComponent>.op_Implicit(ent), new Vector2(0.45f, -0.32f));
			break;
		case 2:
			_sprite.SetOffset(Entity<RMCApcComponent>.op_Implicit(ent), new Vector2(0.7f, -1.45f));
			break;
		case 4:
			_sprite.SetOffset(Entity<RMCApcComponent>.op_Implicit(ent), new Vector2(-0.5f, -1.5f));
			break;
		case 6:
			_sprite.SetOffset(Entity<RMCApcComponent>.op_Implicit(ent), new Vector2(-0.7f, -0.4f));
			break;
		}
		((EntitySystem)this).Dirty(Entity<RMCApcComponent>.op_Implicit(ent), (IComponent)(object)sprite, (MetaDataComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		if (_recalculate)
		{
			_recalculate = false;
			EntityQueryEnumerator<RMCApcComponent> apcQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCApcComponent>();
			EntityUid uid = default(EntityUid);
			RMCApcComponent rMCApcComponent = default(RMCApcComponent);
			while (apcQuery.MoveNext(ref uid, ref rMCApcComponent))
			{
				ToUpdate.Add(uid);
			}
			EntityQueryEnumerator<RMCPowerReceiverComponent> receiverQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCPowerReceiverComponent>();
			EntityUid uid2 = default(EntityUid);
			RMCPowerReceiverComponent rMCPowerReceiverComponent = default(RMCPowerReceiverComponent);
			while (receiverQuery.MoveNext(ref uid2, ref rMCPowerReceiverComponent))
			{
				ToUpdate.Add(uid2);
			}
			EntityQueryEnumerator<RMCFusionReactorComponent> reactorQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCFusionReactorComponent>();
			EntityUid uid3 = default(EntityUid);
			RMCFusionReactorComponent rMCFusionReactorComponent = default(RMCFusionReactorComponent);
			while (reactorQuery.MoveNext(ref uid3, ref rMCFusionReactorComponent))
			{
				_reactorsUpdated.Add(((EntitySystem)this).Transform(uid3).MapID);
			}
			EntityQueryEnumerator<RMCReactorPoweredLightComponent> lightQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCReactorPoweredLightComponent>();
			EntityUid uid4 = default(EntityUid);
			RMCReactorPoweredLightComponent comp = default(RMCReactorPoweredLightComponent);
			while (lightQuery.MoveNext(ref uid4, ref comp))
			{
				Extensions.GetOrNew<MapId, List<EntityUid>>(_reactorPoweredLights, ((EntitySystem)this).Transform(uid4).MapID).Add(uid4);
			}
		}
		if (_net.IsClient)
		{
			ToUpdate.Clear();
			_reactorPoweredLights.Clear();
			_reactorsUpdated.Clear();
			return;
		}
		try
		{
			EntityUid uid5 = default(EntityUid);
			RMCReactorPoweredLightComponent rMCReactorPoweredLightComponent = default(RMCReactorPoweredLightComponent);
			TransformComponent xform = default(TransformComponent);
			foreach (MapId map in _reactorsUpdated)
			{
				bool powered = AnyReactorsOn(map);
				EntityQueryEnumerator<RMCReactorPoweredLightComponent, TransformComponent> lights = ((EntitySystem)this).EntityQueryEnumerator<RMCReactorPoweredLightComponent, TransformComponent>();
				while (lights.MoveNext(ref uid5, ref rMCReactorPoweredLightComponent, ref xform))
				{
					if (xform.MapID == map)
					{
						_appearance.SetData(uid5, (Enum)ToggleableVisuals.Enabled, (object)powered, (AppearanceComponent)null);
						_pointLight.SetEnabled(uid5, powered, (SharedPointLightComponent)null, (MetaDataComponent)null);
					}
				}
			}
		}
		finally
		{
			_reactorsUpdated.Clear();
		}
		try
		{
			RMCApcComponent apc = default(RMCApcComponent);
			RMCAreaPowerComponent oldArea = default(RMCAreaPowerComponent);
			RMCPowerReceiverComponent receiver = default(RMCPowerReceiverComponent);
			RMCAreaPowerComponent oldArea2 = default(RMCAreaPowerComponent);
			foreach (EntityUid update in ToUpdate)
			{
				if (((EntitySystem)this).TerminatingOrDeleted(update, (MetaDataComponent)null))
				{
					continue;
				}
				if (_apcQuery.TryComp(update, ref apc) && _areaPowerQuery.TryComp(apc.Area, ref oldArea))
				{
					oldArea.Apcs.Remove(update);
					((EntitySystem)this).Dirty(update, (IComponent)(object)apc, (MetaDataComponent)null);
				}
				if (_powerReceiverQuery.TryComp(update, ref receiver) && _areaPowerQuery.TryComp(receiver.Area, ref oldArea2))
				{
					GetAreaReceivers(Entity<RMCAreaPowerComponent>.op_Implicit((receiver.Area.Value, oldArea2)), receiver.Channel).Remove(update);
					oldArea2.Load[(int)receiver.Channel] -= receiver.LastLoad;
					((EntitySystem)this).Dirty(update, (IComponent)(object)receiver, (MetaDataComponent)null);
				}
				if (!TryGetPowerArea(update, out Entity<RMCAreaPowerComponent> area))
				{
					continue;
				}
				if (apc != null)
				{
					if (area.Comp.Apcs.Add(update))
					{
						((EntitySystem)this).Dirty<RMCAreaPowerComponent>(area, (MetaDataComponent)null);
					}
					apc.Area = Entity<RMCAreaPowerComponent>.op_Implicit(area);
					((EntitySystem)this).Dirty(update, (IComponent)(object)apc, (MetaDataComponent)null);
				}
				if (receiver != null)
				{
					receiver.Area = Entity<RMCAreaPowerComponent>.op_Implicit(area);
					((EntitySystem)this).Dirty(update, (IComponent)(object)receiver, (MetaDataComponent)null);
					PowerChangedEvent ev = new PowerChangedEvent(IsAreaPowered(Entity<RMCAreaPowerComponent>.op_Implicit((Entity<RMCAreaPowerComponent>.op_Implicit(area), Entity<RMCAreaPowerComponent>.op_Implicit(area))), receiver.Channel), 0f);
					UpdateReceiverPower(update, ref ev);
					if (GetAreaReceivers(area, receiver.Channel).Add(update))
					{
						receiver.LastLoad = GetNewPowerLoad(Entity<RMCPowerReceiverComponent>.op_Implicit((update, receiver)));
						area.Comp.Load[(int)receiver.Channel] += receiver.LastLoad;
						((EntitySystem)this).Dirty<RMCAreaPowerComponent>(area, (MetaDataComponent)null);
					}
				}
			}
		}
		finally
		{
			ToUpdate.Clear();
		}
	}
}
