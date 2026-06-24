using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.MotionDetector;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Storage;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Intel.Detector;

public sealed class IntelDetectorSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedCMInventorySystem _rmcInventory;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private InventorySystem _inventory;

	private EntityQuery<IntelDetectorComponent> _detectorQuery;

	private EntityQuery<StorageComponent> _storageQuery;

	private readonly HashSet<Entity<IntelDetectorTrackedComponent>> _tracked = new HashSet<Entity<IntelDetectorTrackedComponent>>();

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_detectorQuery = ((EntitySystem)this).GetEntityQuery<IntelDetectorComponent>();
		_storageQuery = ((EntitySystem)this).GetEntityQuery<StorageComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteInfectEvent>((EntityEventHandler<XenoParasiteInfectEvent>)OnXenoInfect, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateChangedEvent>((EntityEventHandler<MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelDetectorComponent, UseInHandEvent>((EntityEventRefHandler<IntelDetectorComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelDetectorComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<IntelDetectorComponent, GetVerbsEvent<AlternativeVerb>>)OnGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelDetectorComponent, DroppedEvent>((EntityEventRefHandler<IntelDetectorComponent, DroppedEvent>)OnDisable, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelDetectorComponent, RMCDroppedEvent>((EntityEventRefHandler<IntelDetectorComponent, RMCDroppedEvent>)OnDisable, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntelDetectorComponent, ExaminedEvent>((EntityEventRefHandler<IntelDetectorComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
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

	private void OnUseInHand(Entity<IntelDetectorComponent> ent, ref UseInHandEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		Toggle(ent);
		_audio.PlayPredicted(ent.Comp.ToggleSound, Entity<IntelDetectorComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
	}

	private void OnGetVerbs(Entity<IntelDetectorComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract)
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
					ent.Comp.Short = !ent.Comp.Short;
					((EntitySystem)this).Dirty<IntelDetectorComponent>(ent, (MetaDataComponent)null);
					_audio.PlayPredicted(ent.Comp.ToggleSound, Entity<IntelDetectorComponent>.op_Implicit(ent), (EntityUid?)user, (AudioParams?)null);
				}
			});
		}
	}

	private void OnDisable<T>(Entity<IntelDetectorComponent> ent, ref T args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Enabled = false;
		((EntitySystem)this).Dirty<IntelDetectorComponent>(ent, (MetaDataComponent)null);
		UpdateAppearance(ent);
	}

	private void DisableIntelDetectors(EntityUid ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		IntelDetectorComponent detector = default(IntelDetectorComponent);
		if (_detectorQuery.TryComp(ent, ref detector))
		{
			detector.Enabled = false;
			((EntitySystem)this).Dirty(ent, (IComponent)(object)detector, (MetaDataComponent)null);
			UpdateAppearance(Entity<IntelDetectorComponent>.op_Implicit((ent, detector)));
		}
		StorageComponent storage = default(StorageComponent);
		if (!_storageQuery.TryComp(ent, ref storage))
		{
			return;
		}
		foreach (EntityUid stored in storage.StoredItems.Keys)
		{
			DisableIntelDetectors(stored);
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
			DisableIntelDetectors(held);
		}
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(uid));
		ContainerSlot slot;
		while (slots.MoveNext(out slot))
		{
			EntityUid? containedEntity = slot.ContainedEntity;
			if (containedEntity.HasValue)
			{
				EntityUid contained = containedEntity.GetValueOrDefault();
				DisableIntelDetectors(contained);
			}
		}
	}

	private void OnExamined(Entity<IntelDetectorComponent> ent, ref ExaminedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("IntelDetectorComponent"))
		{
			string mode = (ent.Comp.Short ? "short" : "long");
			args.PushMarkup("The motion detector is in [color=cyan]" + mode + "[/color] scanning mode.");
		}
	}

	private void Toggle(Entity<IntelDetectorComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ref bool enabled = ref ent.Comp.Enabled;
		enabled = !enabled;
		if (enabled)
		{
			ent.Comp.NextScanAt = _timing.CurTime + GetRefreshRate(ent);
		}
		ent.Comp.Blips.Clear();
		((EntitySystem)this).Dirty<IntelDetectorComponent>(ent, (MetaDataComponent)null);
		UpdateAppearance(ent);
	}

	private TimeSpan GetRefreshRate(Entity<IntelDetectorComponent> ent)
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

	private void UpdateAppearance(Entity<IntelDetectorComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<IntelDetectorComponent>.op_Implicit(ent), (Enum)IntelDetectorLayer.State, (object)ent.Comp.Enabled, (AppearanceComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<IntelDetectorComponent> detectors = ((EntitySystem)this).EntityQueryEnumerator<IntelDetectorComponent>();
		EntityUid uid = default(EntityUid);
		IntelDetectorComponent detector = default(IntelDetectorComponent);
		while (detectors.MoveNext(ref uid, ref detector))
		{
			if (!detector.Enabled || time < detector.NextScanAt)
			{
				continue;
			}
			detector.LastScan = time;
			detector.NextScanAt = time + GetRefreshRate(Entity<IntelDetectorComponent>.op_Implicit((uid, detector)));
			((EntitySystem)this).Dirty(uid, (IComponent)(object)detector, (MetaDataComponent)null);
			int range = (detector.Short ? detector.ShortRange : detector.LongRange);
			_tracked.Clear();
			_entityLookup.GetEntitiesInRange<IntelDetectorTrackedComponent>(uid.ToCoordinates(), (float)range, _tracked, (LookupFlags)110);
			detector.Blips.Clear();
			foreach (Entity<IntelDetectorTrackedComponent> tracked in _tracked)
			{
				detector.Blips.Add(new Blip(_transform.GetMapCoordinates(Entity<IntelDetectorTrackedComponent>.op_Implicit(tracked), (TransformComponent)null), QueenEye: false));
			}
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
