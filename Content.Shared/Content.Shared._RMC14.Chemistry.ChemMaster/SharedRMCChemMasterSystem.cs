using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._RMC14.Chemistry.SmartFridge;
using Content.Shared._RMC14.IconLabel;
using Content.Shared._RMC14.Storage;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Coordinates;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Labels.Components;
using Content.Shared.Labels.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Chemistry.ChemMaster;

public abstract class SharedRMCChemMasterSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private ItemSlotsSystem _itemSlots;

	[Dependency]
	private LabelSystem _label;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCIconLabelSystem _rmcIconLabel;

	[Dependency]
	private SharedRMCSmartFridgeSystem _rmcSmartFridge;

	[Dependency]
	private RMCStorageSystem _rmcStorage;

	[Dependency]
	private SharedSolutionContainerSystem _solution;

	[Dependency]
	private SolutionTransferSystem _solutionTransfer;

	[Dependency]
	private SharedStorageSystem _storage;

	private readonly List<EntityUid> _toFill = new List<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCChemMasterComponent, InteractUsingEvent>((EntityEventRefHandler<RMCChemMasterComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCChemMasterComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<RMCChemMasterComponent, EntInsertedIntoContainerMessage>)OnEntInsertedIntoContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCChemMasterComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<RMCChemMasterComponent, EntRemovedFromContainerMessage>)OnEntRemovedFromContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCChemMasterComponent, RMCChemMasterPillBottleTransferDoAfterEvent>((EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterPillBottleTransferDoAfterEvent>)OnPillBottleBoxTransferDoAfter, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<RMCChemMasterComponent>(((EntitySystem)this).Subs, (object)RMCChemMasterUI.Key, (BuiEventSubscriber<RMCChemMasterComponent>)delegate(Subscriber<RMCChemMasterComponent> subs)
		{
			subs.Event<RMCChemMasterPillBottleLabelMsg>((EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterPillBottleLabelMsg>)OnPillBottleLabelMsg);
			subs.Event<RMCChemMasterPillBottleColorMsg>((EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterPillBottleColorMsg>)OnPillBottleColorMsg);
			subs.Event<RMCChemMasterPillBottleFillMsg>((EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterPillBottleFillMsg>)OnPillBottleFillMsg);
			subs.Event<RMCChemMasterPillBottleTransferMsg>((EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterPillBottleTransferMsg>)OnPillBottleTransferMsg);
			subs.Event<RMCChemMasterPillBottleEjectMsg>((EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterPillBottleEjectMsg>)OnPillBottleEjectMsg);
			subs.Event<RMCChemMasterBeakerEjectMsg>((EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterBeakerEjectMsg>)OnBeakerEjectMsg);
			subs.Event<RMCChemMasterBeakerTransferMsg>((EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterBeakerTransferMsg>)OnBeakerTransferMsg);
			subs.Event<RMCChemMasterBeakerTransferAllMsg>((EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterBeakerTransferAllMsg>)OnBeakerTransferAllMsg);
			subs.Event<RMCChemMasterBufferModeMsg>((EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterBufferModeMsg>)OnBufferModeMsg);
			subs.Event<RMCChemMasterBufferTransferMsg>((EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterBufferTransferMsg>)OnBufferTransferMsg);
			subs.Event<RMCChemMasterBufferTransferAllMsg>((EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterBufferTransferAllMsg>)OnBufferTransferAllMsg);
			subs.Event<RMCChemMasterSetPillAmountMsg>((EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterSetPillAmountMsg>)OnSetPillAmountMsg);
			subs.Event<RMCChemMasterSetPillTypeMsg>((EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterSetPillTypeMsg>)OnSetPillTypeMsg);
			subs.Event<RMCChemMasterCreatePillsMsg>((EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterCreatePillsMsg>)OnCreatePillsMsg);
		});
	}

	private void OnInteractUsing(Entity<RMCChemMasterComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		RMCPillBottleTransferComponent boxComp = default(RMCPillBottleTransferComponent);
		StorageComponent boxStorage = default(StorageComponent);
		if (((EntitySystem)this).TryComp<RMCPillBottleTransferComponent>(args.Used, ref boxComp) && ((EntitySystem)this).TryComp<StorageComponent>(args.Used, ref boxStorage))
		{
			((HandledEntityEventArgs)args).Handled = true;
			Container pillBottleSlot = _container.EnsureContainer<Container>(Entity<RMCChemMasterComponent>.op_Implicit(ent), ent.Comp.PillBottleContainer, (ContainerManagerComponent)null);
			int availableSpace = ent.Comp.MaxPillBottles - ((BaseContainer)pillBottleSlot).Count;
			if (availableSpace <= 0)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-chem-master-full-pill-bottles"), Entity<RMCChemMasterComponent>.op_Implicit(ent), args.User);
				return;
			}
			if (boxStorage.StoredItems.Count == 0)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-chem-master-pill-bottle-box-empty", (ValueTuple<string, object>)("box", args.Used)), Entity<RMCChemMasterComponent>.op_Implicit(ent), args.User);
				return;
			}
			TimeSpan transferTime = TimeSpan.FromSeconds((float)Math.Min(boxStorage.StoredItems.Count, availableSpace) * boxComp.TimePerBottle);
			RMCChemMasterPillBottleTransferDoAfterEvent ev = new RMCChemMasterPillBottleTransferDoAfterEvent();
			DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, transferTime, ev, ent.Owner, ent.Owner, args.Used)
			{
				BreakOnMove = true,
				NeedHand = true
			};
			if (_doAfter.TryStartDoAfter(doAfterArgs))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-chem-master-pill-bottle-box-start", (ValueTuple<string, object>)("box", args.Used), (ValueTuple<string, object>)("target", ent)), args.User, args.User);
			}
		}
		else if (_entityWhitelist.IsWhitelistPass(ent.Comp.PillBottleWhitelist, args.Used))
		{
			((HandledEntityEventArgs)args).Handled = true;
			Container slot = _container.EnsureContainer<Container>(Entity<RMCChemMasterComponent>.op_Implicit(ent), ent.Comp.PillBottleContainer, (ContainerManagerComponent)null);
			if (((BaseContainer)slot).Count >= ent.Comp.MaxPillBottles)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-chem-master-full-pill-bottles"), Entity<RMCChemMasterComponent>.op_Implicit(ent), args.User);
				return;
			}
			_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(args.Used), (BaseContainer)(object)slot, (TransformComponent)null, false);
			_audio.PlayPredicted(ent.Comp.PillBottleInsertSound, Entity<RMCChemMasterComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
		}
	}

	protected virtual void OnEntInsertedIntoContainer(Entity<RMCChemMasterComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ContainerModifiedMessage)args).Container.ID != ent.Comp.BufferSolutionId))
		{
			((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
		}
	}

	protected virtual void OnEntRemovedFromContainer(Entity<RMCChemMasterComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ContainerModifiedMessage)args).Container.ID != ent.Comp.BufferSolutionId))
		{
			ent.Comp.SelectedBottles.Remove(((ContainerModifiedMessage)args).Entity);
			((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnPillBottleLabelMsg(Entity<RMCChemMasterComponent> ent, ref RMCChemMasterPillBottleLabelMsg args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		string label = args.Label;
		if (label.Length > ent.Comp.MaxLabelLength)
		{
			label = label.Substring(0, ent.Comp.MaxLabelLength);
		}
		BaseContainer container = default(BaseContainer);
		foreach (EntityUid bottle in ent.Comp.SelectedBottles)
		{
			if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(bottle, null)), ref container) && !(container.Owner != ent.Owner))
			{
				_label.Label(bottle, label);
				_rmcIconLabel.Label(Entity<IconLabelComponent>.op_Implicit(bottle), LocId.op_Implicit("rmc-custom-container-label-text"), ("customLabel", label));
			}
		}
		((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
	}

	private void OnPillBottleColorMsg(Entity<RMCChemMasterComponent> ent, ref RMCChemMasterPillBottleColorMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		foreach (EntityUid bottle in ent.Comp.SelectedBottles)
		{
			if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(bottle, null)), ref container) && !(container.Owner != ent.Owner))
			{
				_appearance.SetData(bottle, (Enum)RMCPillBottleVisuals.Color, (object)args.Color, (AppearanceComponent)null);
			}
		}
		((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
	}

	private void OnPillBottleFillMsg(Entity<RMCChemMasterComponent> ent, ref RMCChemMasterPillBottleFillMsg args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? bottle = default(EntityUid?);
		BaseContainer container = default(BaseContainer);
		if (((EntitySystem)this).TryGetEntity(args.Bottle, ref bottle) && _container.TryGetContainer(Entity<RMCChemMasterComponent>.op_Implicit(ent), ent.Comp.PillBottleContainer, ref container, (ContainerManagerComponent)null) && container.ContainedEntities.Contains(bottle.Value))
		{
			if (args.Fill)
			{
				ent.Comp.SelectedBottles.Add(bottle.Value);
			}
			else
			{
				ent.Comp.SelectedBottles.Remove(bottle.Value);
			}
			((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
			RefreshUIs(ent);
		}
	}

	private void OnPillBottleTransferMsg(Entity<RMCChemMasterComponent> ent, ref RMCChemMasterPillBottleTransferMsg args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? bottle = default(EntityUid?);
		BaseContainer container = default(BaseContainer);
		if (((EntitySystem)this).TryGetEntity(args.Bottle, ref bottle) && _container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(bottle.Value, null)), ref container) && !(container.Owner != ent.Owner) && !(container.ID != ent.Comp.PillBottleContainer))
		{
			_rmcSmartFridge.TransferToNearby(ent.Owner.ToCoordinates(), ent.Comp.LinkRange, bottle.Value);
			((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnPillBottleEjectMsg(Entity<RMCChemMasterComponent> ent, ref RMCChemMasterPillBottleEjectMsg args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? bottle = default(EntityUid?);
		BaseContainer container = default(BaseContainer);
		if (((EntitySystem)this).TryGetEntity(args.Bottle, ref bottle) && _container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(bottle.Value, null)), ref container) && !(container.Owner != ent.Owner) && !(container.ID != ent.Comp.PillBottleContainer))
		{
			if (_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(bottle.Value), container, true, false, (EntityCoordinates?)null, (Angle?)null))
			{
				_hands.TryPickupAnyHand(((BaseBoundUserInterfaceEvent)args).Actor, bottle.Value);
				_audio.PlayPredicted(ent.Comp.PillBottleEjectSound, Entity<RMCChemMasterComponent>.op_Implicit(ent), (EntityUid?)((BaseBoundUserInterfaceEvent)args).Actor, (AudioParams?)null);
			}
			((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnBeakerEjectMsg(Entity<RMCChemMasterComponent> ent, ref RMCChemMasterBeakerEjectMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetBeaker(ent, out Entity<FitsInDispenserComponent> _, out ItemSlot slot, out Entity<SolutionComponent> _))
		{
			_itemSlots.TryEjectToHands(Entity<RMCChemMasterComponent>.op_Implicit(ent), slot, ((BaseBoundUserInterfaceEvent)args).Actor, excludeUserAudio: true);
			((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnBeakerTransferMsg(Entity<RMCChemMasterComponent> ent, ref RMCChemMasterBeakerTransferMsg args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.Amount < FixedPoint2.Zero) && TryGetBeaker(ent, out Entity<FitsInDispenserComponent> _, out ItemSlot _, out Entity<SolutionComponent> beakerSolution) && _solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BufferSolutionId, out Entity<SolutionComponent>? buffer))
		{
			FixedPoint2 removed = beakerSolution.Comp.Solution.RemoveReagent(new ReagentQuantity(ProtoId<ReagentPrototype>.op_Implicit(args.Reagent), args.Amount), preserveOrder: true);
			_solution.TryAddReagent(buffer.Value, ProtoId<ReagentPrototype>.op_Implicit(args.Reagent), removed, out var accepted);
			removed -= accepted;
			if (removed > FixedPoint2.Zero)
			{
				_solution.TryAddReagent(beakerSolution, ProtoId<ReagentPrototype>.op_Implicit(args.Reagent), removed);
			}
			_solution.UpdateChemicals(buffer.Value);
			_solution.UpdateChemicals(beakerSolution);
			((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
			RefreshUIs(ent);
		}
	}

	private void OnBeakerTransferAllMsg(Entity<RMCChemMasterComponent> ent, ref RMCChemMasterBeakerTransferAllMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetBeaker(ent, out Entity<FitsInDispenserComponent> beaker, out ItemSlot _, out Entity<SolutionComponent> beakerSolution) && _solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BufferSolutionId, out Entity<SolutionComponent>? buffer))
		{
			_solutionTransfer.Transfer(((BaseBoundUserInterfaceEvent)args).Actor, Entity<FitsInDispenserComponent>.op_Implicit(beaker), beakerSolution, Entity<RMCChemMasterComponent>.op_Implicit(ent), buffer.Value, beakerSolution.Comp.Solution.Volume);
			((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
			RefreshUIs(ent);
		}
	}

	private void OnBufferModeMsg(Entity<RMCChemMasterComponent> ent, ref RMCChemMasterBufferModeMsg args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (Enum.IsDefined(args.Mode))
		{
			ent.Comp.BufferTransferMode = args.Mode;
			((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
			RefreshUIs(ent);
		}
	}

	private void OnBufferTransferMsg(Entity<RMCChemMasterComponent> ent, ref RMCChemMasterBufferTransferMsg args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if (args.Amount < FixedPoint2.Zero || !_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BufferSolutionId, out Entity<SolutionComponent>? buffer))
		{
			return;
		}
		FixedPoint2 removed = buffer.Value.Comp.Solution.RemoveReagent(new ReagentQuantity(ProtoId<ReagentPrototype>.op_Implicit(args.Reagent), args.Amount), preserveOrder: true);
		Entity<FitsInDispenserComponent> beaker;
		ItemSlot slot;
		Entity<SolutionComponent> beakerSolution;
		if (ent.Comp.BufferTransferMode == RMCChemMasterBufferMode.ToDisposal)
		{
			_solution.UpdateChemicals(buffer.Value);
			((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
			RefreshUIs(ent);
		}
		else if (TryGetBeaker(ent, out beaker, out slot, out beakerSolution))
		{
			_solution.TryAddReagent(beakerSolution, ProtoId<ReagentPrototype>.op_Implicit(args.Reagent), removed, out var accepted);
			removed -= accepted;
			if (removed > FixedPoint2.Zero)
			{
				_solution.TryAddReagent(buffer.Value, ProtoId<ReagentPrototype>.op_Implicit(args.Reagent), removed);
			}
			_solution.UpdateChemicals(buffer.Value);
			_solution.UpdateChemicals(beakerSolution);
			((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
			RefreshUIs(ent);
		}
	}

	private void OnBufferTransferAllMsg(Entity<RMCChemMasterComponent> ent, ref RMCChemMasterBufferTransferAllMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BufferSolutionId, out Entity<SolutionComponent>? buffer))
		{
			Entity<FitsInDispenserComponent> beaker;
			ItemSlot slot;
			Entity<SolutionComponent> beakerSolution;
			if (ent.Comp.BufferTransferMode == RMCChemMasterBufferMode.ToDisposal)
			{
				_solution.RemoveAllSolution(buffer.Value);
			}
			else if (TryGetBeaker(ent, out beaker, out slot, out beakerSolution))
			{
				_solutionTransfer.Transfer(((BaseBoundUserInterfaceEvent)args).Actor, Entity<RMCChemMasterComponent>.op_Implicit(ent), buffer.Value, Entity<FitsInDispenserComponent>.op_Implicit(beaker), beakerSolution, buffer.Value.Comp.Solution.Volume);
			}
			((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
			RefreshUIs(ent);
		}
	}

	private void OnSetPillAmountMsg(Entity<RMCChemMasterComponent> ent, ref RMCChemMasterSetPillAmountMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.PillAmount = Math.Clamp(args.Amount, 1, ent.Comp.MaxPillAmount);
		((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
	}

	private void OnSetPillTypeMsg(Entity<RMCChemMasterComponent> ent, ref RMCChemMasterSetPillTypeMsg args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (args.Type != 0 && args.Type <= ent.Comp.PillTypes)
		{
			ent.Comp.SelectedType = args.Type;
			((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
			RefreshUIs(ent);
		}
	}

	private void OnCreatePillsMsg(Entity<RMCChemMasterComponent> ent, ref RMCChemMasterCreatePillsMsg args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<RMCChemMasterComponent>.op_Implicit(ent), ent.Comp.PillBottleContainer, ref container, (ContainerManagerComponent)null))
		{
			return;
		}
		_toFill.Clear();
		StorageComponent storage = default(StorageComponent);
		foreach (EntityUid bottle in ent.Comp.SelectedBottles)
		{
			if (container.Contains(bottle) && ((EntitySystem)this).TryComp<StorageComponent>(bottle, ref storage))
			{
				if (_rmcStorage.EstimateFreeColumns(Entity<StorageComponent>.op_Implicit((bottle, storage))) < ent.Comp.PillAmount)
				{
					string msg = base.Loc.GetString("rmc-chem-master-pills-not-enough-space");
					_popup.PopupClient(msg, ((BaseBoundUserInterfaceEvent)args).Actor, PopupType.MediumCaution);
					return;
				}
				_toFill.Add(bottle);
			}
		}
		if (!_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.BufferSolutionId, out Entity<SolutionComponent>? buffer))
		{
			string msg2 = base.Loc.GetString("rmc-chem-master-not-enough-space-solution");
			_popup.PopupClient(msg2, ((BaseBoundUserInterfaceEvent)args).Actor, PopupType.MediumCaution);
			return;
		}
		FixedPoint2 solution = buffer.Value.Comp.Solution.Volume;
		int divider = _toFill.Count * ent.Comp.PillAmount;
		if (divider == 0)
		{
			return;
		}
		FixedPoint2 perPill = solution / divider;
		if (solution <= FixedPoint2.Zero || perPill <= FixedPoint2.Zero)
		{
			string msg3 = base.Loc.GetString("rmc-chem-master-not-enough-space-solution");
			_popup.PopupClient(msg3, ((BaseBoundUserInterfaceEvent)args).Actor, PopupType.MediumCaution);
		}
		else
		{
			if (_net.IsClient)
			{
				return;
			}
			string originalSolution = string.Join(", ", buffer.Value.Comp.Solution.Contents.Select((ReagentQuantity c) => $"{c.Quantity}u {c.Reagent.Prototype}"));
			EntityCoordinates coords = ((EntitySystem)this).Transform(Entity<RMCChemMasterComponent>.op_Implicit(ent)).Coordinates;
			List<(string, FixedPoint2)> reagentsPerPill = buffer.Value.Comp.Solution.Contents.Select((ReagentQuantity c) => (Prototype: c.Reagent.Prototype, Amount: c.Quantity / divider)).ToList();
			SolutionSpikerComponent spiker = default(SolutionSpikerComponent);
			foreach (EntityUid fill in _toFill)
			{
				string label = ((EntitySystem)this).CompOrNull<LabelComponent>(fill)?.CurrentLabel;
				for (int i = 0; i < ent.Comp.PillAmount; i++)
				{
					EntityUid pill = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(ent.Comp.PillProto), coords);
					if (!_storage.Insert(fill, pill, out var _, ((BaseBoundUserInterfaceEvent)args).Actor, null, playSound: false))
					{
						((EntitySystem)this).QueueDel((EntityUid?)pill);
						continue;
					}
					PillComponent pillComp = ((EntitySystem)this).EnsureComp<PillComponent>(pill);
					pillComp.PillType = ent.Comp.SelectedType - 1;
					((EntitySystem)this).Dirty(pill, (IComponent)(object)pillComp, (MetaDataComponent)null);
					if (label != null)
					{
						_label.Label(pill, label);
					}
					if (!((EntitySystem)this).TryComp<SolutionSpikerComponent>(pill, ref spiker) || !_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(pill), spiker.SourceSolution, out Entity<SolutionComponent>? pillSolution))
					{
						continue;
					}
					foreach (var item in reagentsPerPill)
					{
						string reagentProto = item.Item1;
						FixedPoint2 amount = item.Item2;
						FixedPoint2 removed = buffer.Value.Comp.Solution.RemoveReagent(reagentProto, amount);
						_solution.TryAddReagent(pillSolution.Value, reagentProto, removed);
					}
					ISharedAdminLogManager adminLog = _adminLog;
					LogStringHandler handler = new LogStringHandler(38, 4);
					handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "player", "ToPrettyString(args.Actor)");
					handler.AppendLiteral(" transferred ");
					handler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(pillSolution.Value.Comp.Solution));
					handler.AppendLiteral(" to ");
					handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(pill)), "target", "ToPrettyString(pill)");
					handler.AppendLiteral(", which now contains ");
					handler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(pillSolution.Value.Comp.Solution));
					adminLog.Add(LogType.Action, LogImpact.Medium, ref handler);
				}
			}
			_solution.UpdateChemicals(buffer.Value);
			ISharedAdminLogManager adminLog2 = _adminLog;
			LogStringHandler handler2 = new LogStringHandler(73, 7);
			handler2.AppendLiteral("");
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "user", "ToPrettyString(args.Actor)");
			handler2.AppendLiteral(" created ");
			handler2.AppendFormatted(ent.Comp.PillAmount, "pillAmount", "ent.Comp.PillAmount");
			handler2.AppendLiteral(" ");
			handler2.AppendFormatted(perPill, "pillUnits", "perPill");
			handler2.AppendLiteral("u pills in ");
			handler2.AppendFormatted(ent.Comp.SelectedBottles.Count, "bottleAmount", "ent.Comp.SelectedBottles.Count");
			handler2.AppendLiteral(" pill bottles using ");
			handler2.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<RMCChemMasterComponent>.op_Implicit(ent), (MetaDataComponent)null), "chemMaster", "ToPrettyString(ent)");
			handler2.AppendLiteral(".\r\nSolution: ");
			handler2.AppendFormatted(originalSolution, 0, "solution");
			handler2.AppendLiteral("\r\nPill bottle IDs: ");
			handler2.AppendFormatted(string.Join(", ", ent.Comp.SelectedBottles), 0, "bottleIds");
			adminLog2.Add(LogType.RMCChemMaster, ref handler2);
			((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnPillBottleBoxTransferDoAfter(Entity<RMCChemMasterComponent> ent, ref RMCChemMasterPillBottleTransferDoAfterEvent args)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		RMCPillBottleTransferComponent boxComp = default(RMCPillBottleTransferComponent);
		StorageComponent boxStorage = default(StorageComponent);
		if (!args.Used.HasValue || !((EntitySystem)this).Exists(args.Used) || !((EntitySystem)this).TryComp<RMCPillBottleTransferComponent>(args.Used, ref boxComp) || !((EntitySystem)this).TryComp<StorageComponent>(args.Used, ref boxStorage))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-chem-master-pill-bottle-box-failed"), args.User, args.User);
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		Container slot = _container.EnsureContainer<Container>(Entity<RMCChemMasterComponent>.op_Implicit(ent), ent.Comp.PillBottleContainer, (ContainerManagerComponent)null);
		int availableSpace = ent.Comp.MaxPillBottles - ((BaseContainer)slot).Count;
		if (availableSpace <= 0)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-chem-master-full-pill-bottles"), Entity<RMCChemMasterComponent>.op_Implicit(ent), args.User);
		}
		else
		{
			BaseContainer boxContainer = default(BaseContainer);
			if (!_container.TryGetContainer(args.Used.Value, ((BaseContainer)boxStorage.Container).ID, ref boxContainer, (ContainerManagerComponent)null))
			{
				return;
			}
			int transferred = 0;
			foreach (EntityUid bottle in boxContainer.ContainedEntities.ToList())
			{
				if (transferred >= availableSpace)
				{
					break;
				}
				if (((EntitySystem)this).Exists(bottle) && _entityWhitelist.IsWhitelistPass(ent.Comp.PillBottleWhitelist, bottle) && _container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(bottle), boxContainer, true, false, (EntityCoordinates?)null, (Angle?)null))
				{
					if (_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(bottle), (BaseContainer)(object)slot, (TransformComponent)null, false))
					{
						transferred++;
					}
					else if (!_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(bottle), boxContainer, (TransformComponent)null, false))
					{
						_hands.TryPickupAnyHand(args.User, bottle);
					}
				}
			}
			if (transferred > 0)
			{
				_audio.PlayPredicted(boxComp.InsertPillBottleSound, Entity<RMCChemMasterComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
				_popup.PopupClient(base.Loc.GetString("rmc-chem-master-pill-bottle-box-complete", (ValueTuple<string, object>)("count", transferred), (ValueTuple<string, object>)("target", ent)), args.User, args.User);
			}
			else
			{
				_popup.PopupClient(base.Loc.GetString("rmc-chem-master-pill-bottle-box-failed"), args.User, args.User);
			}
			((EntitySystem)this).Dirty<RMCChemMasterComponent>(ent, (MetaDataComponent)null);
		}
	}

	private bool TryGetBeaker(Entity<RMCChemMasterComponent> chemMaster, out Entity<FitsInDispenserComponent> beaker, [NotNullWhen(true)] out ItemSlot? slot, out Entity<SolutionComponent> solution)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		beaker = default(Entity<FitsInDispenserComponent>);
		solution = default(Entity<SolutionComponent>);
		if (_itemSlots.TryGetSlot(Entity<RMCChemMasterComponent>.op_Implicit(chemMaster), chemMaster.Comp.BeakerSlot, out slot))
		{
			ContainerSlot? containerSlot = slot.ContainerSlot;
			EntityUid? val = ((containerSlot != null) ? containerSlot.ContainedEntity : ((EntityUid?)null));
			if (val.HasValue)
			{
				EntityUid contained = val.GetValueOrDefault();
				FitsInDispenserComponent fits = default(FitsInDispenserComponent);
				if (!((EntitySystem)this).TryComp<FitsInDispenserComponent>(contained, ref fits))
				{
					return false;
				}
				if (!_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(contained), fits.Solution, out Entity<SolutionComponent>? solutionNullable))
				{
					return false;
				}
				beaker = Entity<FitsInDispenserComponent>.op_Implicit((contained, fits));
				solution = solutionNullable.Value;
				return true;
			}
		}
		return false;
	}

	protected virtual void RefreshUIs(Entity<RMCChemMasterComponent> ent)
	{
	}
}
