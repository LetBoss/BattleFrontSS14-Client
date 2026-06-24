using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Weapons.Ranged.Battery;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.MotionDetector;

public sealed class MotionDetectorSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private GunIFFSystem _gunIFF;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private MotionDetectorSystem _motionDetector;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private RMCGunBatterySystem _rmcGunBattery;

	[Dependency]
	private SharedCMInventorySystem _rmcInventory;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	private EntityQuery<MotionDetectorComponent> _detectorQuery;

	private EntityQuery<StorageComponent> _storageQuery;

	private readonly HashSet<Entity<MotionDetectorTrackedComponent>> _toUpdate = new HashSet<Entity<MotionDetectorTrackedComponent>>();

	private readonly HashSet<Entity<MotionDetectorTrackedComponent>> _tracked = new HashSet<Entity<MotionDetectorTrackedComponent>>();

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_detectorQuery = ((EntitySystem)this).GetEntityQuery<MotionDetectorComponent>();
		_storageQuery = ((EntitySystem)this).GetEntityQuery<StorageComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteInfectEvent>((EntityEventHandler<XenoParasiteInfectEvent>)OnXenoInfect, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateChangedEvent>((EntityEventHandler<MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDevouredEvent>((EntityEventRefHandler<XenoDevouredEvent>)OnMotionDetectorDevoured, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MotionDetectorComponent, UseInHandEvent>((EntityEventRefHandler<MotionDetectorComponent, UseInHandEvent>)OnMotionDetectorUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MotionDetectorComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<MotionDetectorComponent, GetVerbsEvent<AlternativeVerb>>)OnMotionDetectorGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MotionDetectorComponent, DroppedEvent>((EntityEventRefHandler<MotionDetectorComponent, DroppedEvent>)OnMotionDetectorDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MotionDetectorComponent, RMCDroppedEvent>((EntityEventRefHandler<MotionDetectorComponent, RMCDroppedEvent>)OnMotionDetectorDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MotionDetectorComponent, ExaminedEvent>((EntityEventRefHandler<MotionDetectorComponent, ExaminedEvent>)OnMotionDetectorExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleableMotionDetectorComponent, GetItemActionsEvent>((EntityEventRefHandler<ToggleableMotionDetectorComponent, GetItemActionsEvent>)OnGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleableMotionDetectorComponent, ToggleableMotionDetectorActionEvent>((EntityEventRefHandler<ToggleableMotionDetectorComponent, ToggleableMotionDetectorActionEvent>)OnToggleAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleableMotionDetectorComponent, GunGetBatteryDrainEvent>((EntityEventRefHandler<ToggleableMotionDetectorComponent, GunGetBatteryDrainEvent>)OnGetBatteryDrain, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleableMotionDetectorComponent, GunUnpoweredEvent>((EntityEventRefHandler<ToggleableMotionDetectorComponent, GunUnpoweredEvent>)OnGunUnpowered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleableMotionDetectorComponent, MotionDetectorUpdatedEvent>((EntityEventRefHandler<ToggleableMotionDetectorComponent, MotionDetectorUpdatedEvent>)OnMotionDetectorUpdated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MotionDetectorTrackedComponent, MoveEvent>((EntityEventRefHandler<MotionDetectorTrackedComponent, MoveEvent>)OnMotionDetectorTracked, (Type[])null, (Type[])null);
	}

	private void OnXenoInfect(XenoParasiteInfectEvent ev)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		DisableDetectorsOnMob(ev.Target);
	}

	private void OnMobStateChanged(MobStateChangedEvent ev)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (ev.NewMobState == MobState.Dead)
		{
			DisableDetectorsOnMob(ev.Target);
		}
	}

	private void OnMotionDetectorUseInHand(Entity<MotionDetectorComponent> ent, ref UseInHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.HandToggleable && _hands.IsHolding(Entity<HandsComponent>.op_Implicit(args.User), Entity<MotionDetectorComponent>.op_Implicit(ent)))
		{
			((HandledEntityEventArgs)args).Handled = true;
			Toggle(ent);
			EntityUid user = args.User;
			ent.Comp.LastUser = user;
			((EntitySystem)this).Dirty<MotionDetectorComponent>(ent, (MetaDataComponent)null);
			_audio.PlayPredicted(ent.Comp.ToggleSound, Entity<MotionDetectorComponent>.op_Implicit(ent), (EntityUid?)user, (AudioParams?)null);
		}
	}

	private void OnMotionDetectorGetVerbs(Entity<MotionDetectorComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && ent.Comp.CanToggleRange)
		{
			EntityUid user = args.User;
			args.Verbs.Add(new AlternativeVerb
			{
				Text = (ent.Comp.Short ? "Change to long range mode" : "Change to short range mode"),
				Act = delegate
				{
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0051: Unknown result type (might be due to invalid IL or missing references)
					//IL_0056: Unknown result type (might be due to invalid IL or missing references)
					//IL_005c: Unknown result type (might be due to invalid IL or missing references)
					//IL_009f: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
					//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
					//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
					//IL_0104: Unknown result type (might be due to invalid IL or missing references)
					ent.Comp.Short = !ent.Comp.Short;
					((EntitySystem)this).Dirty<MotionDetectorComponent>(ent, (MetaDataComponent)null);
					_audio.PlayPredicted(ent.Comp.ToggleSound, Entity<MotionDetectorComponent>.op_Implicit(ent), (EntityUid?)user, (AudioParams?)null);
					_popup.PopupClient($"You change the {((EntitySystem)this).Name(Entity<MotionDetectorComponent>.op_Implicit(ent), (MetaDataComponent)null)} to {(ent.Comp.Short ? "short" : "long")} range mode", Entity<MotionDetectorComponent>.op_Implicit(ent), user);
				}
			});
		}
	}

	private void OnMotionDetectorDropped<T>(Entity<MotionDetectorComponent> ent, ref T args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.DeactivateOnDrop)
		{
			ent.Comp.Enabled = false;
			((EntitySystem)this).Dirty<MotionDetectorComponent>(ent, (MetaDataComponent)null);
			UpdateAppearance(ent);
			MotionDetectorUpdated(ent);
		}
	}

	private void OnMotionDetectorDevoured(ref XenoDevouredEvent ent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		DisableDetectorsOnMob(ent.Target);
	}

	private void OnMotionDetectorExamined(Entity<MotionDetectorComponent> ent, ref ExaminedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("MotionDetectorComponent"))
		{
			string mode = (ent.Comp.Short ? "short" : "long");
			args.PushMarkup("The motion detector is in [color=cyan]" + mode + "[/color] scanning mode.");
		}
	}

	private void OnGetItemActions(Entity<ToggleableMotionDetectorComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Slots == SlotFlags.All || (!args.InHands && (args.SlotFlags & ent.Comp.Slots) != 0))
		{
			args.AddAction(ref ent.Comp.Action, EntProtoId.op_Implicit(ent.Comp.ActionId));
			((EntitySystem)this).Dirty<ToggleableMotionDetectorComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnToggleAction(Entity<ToggleableMotionDetectorComponent> ent, ref ToggleableMotionDetectorActionEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.Performer;
		MotionDetectorComponent detector = default(MotionDetectorComponent);
		if (((EntitySystem)this).TryComp<MotionDetectorComponent>(Entity<ToggleableMotionDetectorComponent>.op_Implicit(ent), ref detector))
		{
			_motionDetector.Toggle(Entity<MotionDetectorComponent>.op_Implicit((Entity<ToggleableMotionDetectorComponent>.op_Implicit(ent), detector)));
			detector.LastUser = user;
			((EntitySystem)this).Dirty<ToggleableMotionDetectorComponent>(ent, (MetaDataComponent)null);
		}
		_audio.PlayPredicted(ent.Comp.ToggleSound, Entity<ToggleableMotionDetectorComponent>.op_Implicit(ent), (EntityUid?)user, (AudioParams?)null);
		DetectorUpdated(ent);
		string popup = (_motionDetector.IsEnabled(Entity<MotionDetectorComponent>.op_Implicit((Entity<ToggleableMotionDetectorComponent>.op_Implicit(ent), detector))) ? base.Loc.GetString("rmc-toggleable-motion-detector-on", (ValueTuple<string, object>)("gun", ent)) : base.Loc.GetString("rmc-toggleable-motion-detector-off", (ValueTuple<string, object>)("gun", ent)));
		_popup.PopupClient(popup, user, user, PopupType.Large);
	}

	private void OnGetBatteryDrain(Entity<ToggleableMotionDetectorComponent> ent, ref GunGetBatteryDrainEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (_motionDetector.IsEnabled(Entity<MotionDetectorComponent>.op_Implicit(ent.Owner)))
		{
			args.Drain += ent.Comp.BatteryDrain;
		}
	}

	private void OnGunUnpowered(Entity<ToggleableMotionDetectorComponent> ent, ref GunUnpoweredEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		MotionDetectorComponent detector = default(MotionDetectorComponent);
		if (((EntitySystem)this).TryComp<MotionDetectorComponent>(Entity<ToggleableMotionDetectorComponent>.op_Implicit(ent), ref detector))
		{
			_motionDetector.Disable(Entity<MotionDetectorComponent>.op_Implicit((Entity<ToggleableMotionDetectorComponent>.op_Implicit(ent), detector)));
			DetectorUpdated(ent);
		}
	}

	private void OnMotionDetectorUpdated(Entity<ToggleableMotionDetectorComponent> ent, ref MotionDetectorUpdatedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DetectorUpdated(ent);
	}

	private void DetectorUpdated(Entity<ToggleableMotionDetectorComponent> ent)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = false;
		MotionDetectorComponent detector = default(MotionDetectorComponent);
		if (((EntitySystem)this).TryComp<MotionDetectorComponent>(Entity<ToggleableMotionDetectorComponent>.op_Implicit(ent), ref detector))
		{
			enabled = _motionDetector.IsEnabled(Entity<MotionDetectorComponent>.op_Implicit((Entity<ToggleableMotionDetectorComponent>.op_Implicit(ent), detector)));
		}
		_rmcGunBattery.RefreshBatteryDrain(Entity<GunDrainBatteryOnShootComponent>.op_Implicit(ent.Owner));
		SharedActionsSystem actions = _actions;
		EntityUid? action = ent.Comp.Action;
		actions.SetToggled(action.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), enabled);
	}

	private void OnMotionDetectorTracked(Entity<MotionDetectorTrackedComponent> ent, ref MoveEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.OldPosition == args.NewPosition))
		{
			_toUpdate.Add(ent);
		}
	}

	private void UpdateAppearance(Entity<MotionDetectorComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<MotionDetectorComponent>.op_Implicit(ent), (Enum)MotionDetectorLayer.Setting, (object)((!ent.Comp.Short) ? MotionDetectorSetting.Long : MotionDetectorSetting.Short), (AppearanceComponent)null);
		int count = Math.Min(ent.Comp.Blips.Count, 9);
		if (!ent.Comp.Enabled)
		{
			count = -1;
		}
		_appearance.SetData(Entity<MotionDetectorComponent>.op_Implicit(ent), (Enum)MotionDetectorLayer.Number, (object)count, (AppearanceComponent)null);
	}

	private void DisableMotionDetectors(EntityUid ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		MotionDetectorComponent detector = default(MotionDetectorComponent);
		if (_detectorQuery.TryComp(ent, ref detector))
		{
			detector.Enabled = false;
			((EntitySystem)this).Dirty(ent, (IComponent)(object)detector, (MetaDataComponent)null);
			UpdateAppearance(Entity<MotionDetectorComponent>.op_Implicit((ent, detector)));
			MotionDetectorUpdated(Entity<MotionDetectorComponent>.op_Implicit((ent, detector)));
		}
		StorageComponent storage = default(StorageComponent);
		if (!_storageQuery.TryComp(ent, ref storage))
		{
			return;
		}
		foreach (EntityUid stored in storage.StoredItems.Keys)
		{
			DisableMotionDetectors(stored);
		}
	}

	private void DisableDetectorsOnMob(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid held in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(uid)))
		{
			DisableMotionDetectors(held);
		}
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(uid));
		ContainerSlot slot;
		while (slots.MoveNext(out slot))
		{
			EntityUid? containedEntity = slot.ContainedEntity;
			if (containedEntity.HasValue)
			{
				EntityUid contained = containedEntity.GetValueOrDefault();
				DisableMotionDetectors(contained);
			}
		}
	}

	private TimeSpan GetRefreshRate(Entity<MotionDetectorComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Short)
		{
			return ent.Comp.LongRefresh;
		}
		return ent.Comp.ShortRefresh;
	}

	private void MotionDetectorUpdated(Entity<MotionDetectorComponent> ent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		MotionDetectorUpdatedEvent ev = new MotionDetectorUpdatedEvent(ent.Comp.Enabled);
		((EntitySystem)this).RaiseLocalEvent<MotionDetectorUpdatedEvent>(Entity<MotionDetectorComponent>.op_Implicit(ent), ref ev, false);
	}

	public void Toggle(Entity<MotionDetectorComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ref bool enabled = ref ent.Comp.Enabled;
		enabled = !enabled;
		if (enabled)
		{
			ent.Comp.NextScanAt = _timing.CurTime + GetRefreshRate(ent);
		}
		ent.Comp.Blips.Clear();
		((EntitySystem)this).Dirty<MotionDetectorComponent>(ent, (MetaDataComponent)null);
		UpdateAppearance(ent);
		MotionDetectorUpdated(ent);
	}

	public void Disable(Entity<MotionDetectorComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Enabled)
		{
			Toggle(ent);
		}
	}

	public bool IsEnabled(Entity<MotionDetectorComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MotionDetectorComponent>(Entity<MotionDetectorComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return ent.Comp.Enabled;
		}
		return false;
	}

	public override void Update(float frameTime)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		try
		{
			foreach (Entity<MotionDetectorTrackedComponent> item in _toUpdate)
			{
				item.Comp.LastMove = time;
			}
		}
		finally
		{
			_toUpdate.Clear();
		}
		EntityQueryEnumerator<MotionDetectorComponent> detectors = ((EntitySystem)this).EntityQueryEnumerator<MotionDetectorComponent>();
		EntityUid uid = default(EntityUid);
		MotionDetectorComponent detector = default(MotionDetectorComponent);
		while (detectors.MoveNext(ref uid, ref detector))
		{
			if (!detector.Enabled || time < detector.NextScanAt)
			{
				continue;
			}
			detector.LastScan = time;
			detector.NextScanAt = time + GetRefreshRate(Entity<MotionDetectorComponent>.op_Implicit((uid, detector)));
			((EntitySystem)this).Dirty(uid, (IComponent)(object)detector, (MetaDataComponent)null);
			int range = (detector.Short ? detector.ShortRange : detector.LongRange);
			_tracked.Clear();
			_entityLookup.GetEntitiesInRange<MotionDetectorTrackedComponent>(uid.ToCoordinates(), (float)range, _tracked, (LookupFlags)78);
			detector.Blips.Clear();
			foreach (Entity<MotionDetectorTrackedComponent> tracked in _tracked)
			{
				EntityUid owner = tracked.Owner;
				EntityUid? lastUser = detector.LastUser;
				if ((lastUser.HasValue && owner == lastUser.GetValueOrDefault()) || tracked.Comp.LastMove < time - detector.MoveTime)
				{
					continue;
				}
				lastUser = detector.LastUser;
				if (lastUser.HasValue)
				{
					EntityUid lastUser2 = lastUser.GetValueOrDefault();
					if (_gunIFF.TryGetFaction(Entity<UserIFFComponent>.op_Implicit(lastUser2), out EntProtoId<IFFFactionComponent> userFaction) && _gunIFF.IsInFaction(tracked.Owner, userFaction))
					{
						continue;
					}
				}
				detector.Blips.Add(new Blip(_transform.GetMapCoordinates(Entity<MotionDetectorTrackedComponent>.op_Implicit(tracked), (TransformComponent)null), tracked.Comp.IsQueenEye));
			}
			UpdateAppearance(Entity<MotionDetectorComponent>.op_Implicit((uid, detector)));
			if (detector.Blips.Count == 0)
			{
				if (_rmcInventory.TryGetUserHoldingOrStoringItem(uid, out var user))
				{
					_audio.PlayEntity(detector.ScanEmptySound, user, uid, (AudioParams?)null);
				}
			}
			else
			{
				_audio.PlayPvs(detector.ScanSound, uid, (AudioParams?)null);
			}
		}
	}
}
