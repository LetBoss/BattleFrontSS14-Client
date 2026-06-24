using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Power;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Access.Systems;
using Content.Shared.Directions;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.GameTicking;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Prying.Components;
using Content.Shared.Tools.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Doors;

public sealed class CMDoorSystem : EntitySystem
{
	[Dependency]
	private AccessReaderSystem _accessReader;

	[Dependency]
	private SharedMarineAnnounceSystem _announce;

	[Dependency]
	private SharedDoorSystem _doors;

	[Dependency]
	private SharedGameTicker _gameTicker;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedAudioSystem _audioSystem;

	[Dependency]
	private SharedRMCPowerSystem _rmcPower;

	[Dependency]
	private IGameTiming _timing;

	private EntityQuery<DoorComponent> _doorQuery;

	private EntityQuery<CMDoubleDoorComponent> _doubleQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_doorQuery = ((EntitySystem)this).GetEntityQuery<DoorComponent>();
		_doubleQuery = ((EntitySystem)this).GetEntityQuery<CMDoubleDoorComponent>();
		((EntitySystem)this).SubscribeLocalEvent<CMDoubleDoorComponent, DoorStateChangedEvent>((EntityEventRefHandler<CMDoubleDoorComponent, DoorStateChangedEvent>)OnDoorStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDoorButtonComponent, ActivateInWorldEvent>((EntityEventRefHandler<RMCDoorButtonComponent, ActivateInWorldEvent>)OnButtonActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, RMCDoorPryEvent>((EntityEventRefHandler<DoorComponent, RMCDoorPryEvent>)OnDoorPry, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, RMCBeforePryEvent>((EntityEventRefHandler<DoorComponent, RMCBeforePryEvent>)OnBeforePry, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCPodDoorComponent, GetPryTimeModifierEvent>((EntityEventRefHandler<RMCPodDoorComponent, GetPryTimeModifierEvent>)OnPodDoorGetPryTimeModifier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LayerChangeOnWeldComponent, DoorBoltsChangedEvent>((EntityEventRefHandler<LayerChangeOnWeldComponent, DoorBoltsChangedEvent>)OnDoorBoltStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCOpenOnlyWhenUnanchoredComponent, BeforeDoorClosedEvent>((EntityEventRefHandler<RMCOpenOnlyWhenUnanchoredComponent, BeforeDoorClosedEvent>)OnOpenOnlyWhenUnanchoredBeforeClosed, (Type[])null, (Type[])null);
	}

	private void OnDoorStateChanged(Entity<CMDoubleDoorComponent> door, ref DoorStateChangedEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		switch (args.State)
		{
		case DoorState.Opening:
			Open(door);
			break;
		case DoorState.Closing:
			Close(door);
			break;
		}
	}

	private void OnButtonActivateInWorld(Entity<RMCDoorButtonComponent> button, ref ActivateInWorldEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (((EntitySystem)this).HasComp<XenoComponent>(user))
		{
			return;
		}
		if (!_rmcPower.IsPowered(Entity<RMCDoorButtonComponent>.op_Implicit(button)))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-machines-unpowered"), Entity<RMCDoorButtonComponent>.op_Implicit(button), args.User, PopupType.SmallCaution);
			return;
		}
		TimeSpan? minimumRoundTimeToPress = button.Comp.MinimumRoundTimeToPress;
		if (minimumRoundTimeToPress.HasValue)
		{
			TimeSpan minimumTime = minimumRoundTimeToPress.GetValueOrDefault();
			if (_gameTicker.RoundDuration() <= minimumTime)
			{
				int minutesLeft = (int)(minimumTime.TotalMinutes - _gameTicker.RoundDuration().TotalMinutes);
				string timeMessage = base.Loc.GetString(LocId.op_Implicit(button.Comp.NoTimeMessage), (ValueTuple<string, object>)("minutes", minutesLeft));
				_popup.PopupClient(timeMessage, user, user, PopupType.SmallCaution);
				return;
			}
		}
		if (button.Comp.Used && button.Comp.UseOnlyOnce)
		{
			_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(button.Comp.AlreadyUsedMessage)), Entity<RMCDoorButtonComponent>.op_Implicit(button), user, PopupType.SmallCaution);
			return;
		}
		if (!_accessReader.IsAllowed(user, Entity<RMCDoorButtonComponent>.op_Implicit(button)))
		{
			_popup.PopupClient(base.Loc.GetString("cm-vending-machine-access-denied"), Entity<RMCDoorButtonComponent>.op_Implicit(button), user, PopupType.SmallCaution);
			DoPodDoorButtonAnimation(Entity<RMCDoorButtonComponent>.op_Implicit(button), button.Comp.DeniedState);
			return;
		}
		TimeSpan time = _timing.CurTime;
		if (time < button.Comp.LastUse + button.Comp.Cooldown)
		{
			return;
		}
		button.Comp.LastUse = time;
		button.Comp.Used = true;
		((EntitySystem)this).Dirty<RMCDoorButtonComponent>(button, (MetaDataComponent)null);
		string buttonName = button.Comp.Id ?? ((EntitySystem)this).Name(Entity<RMCDoorButtonComponent>.op_Implicit(button), (MetaDataComponent)null);
		TransformComponent buttonTransform = ((EntitySystem)this).Transform(Entity<RMCDoorButtonComponent>.op_Implicit(button));
		EntityQueryEnumerator<RMCPodDoorComponent, DoorComponent, TransformComponent, MetaDataComponent> doors = ((EntitySystem)this).EntityQueryEnumerator<RMCPodDoorComponent, DoorComponent, TransformComponent, MetaDataComponent>();
		EntityUid door = default(EntityUid);
		RMCPodDoorComponent podDoor = default(RMCPodDoorComponent);
		DoorComponent doorComp = default(DoorComponent);
		TransformComponent doorTransform = default(TransformComponent);
		MetaDataComponent metaData = default(MetaDataComponent);
		while (doors.MoveNext(ref door, ref podDoor, ref doorComp, ref doorTransform, ref metaData))
		{
			if (((EntitySystem)this).TerminatingOrDeleted(door, (MetaDataComponent)null) || buttonTransform.MapID != doorTransform.MapID)
			{
				continue;
			}
			string id = podDoor.Id ?? metaData.EntityName;
			if (!(buttonName != id))
			{
				if (doorComp.State == DoorState.Open)
				{
					_doors.StartClosing(door);
				}
				else
				{
					_doors.TryOpen(door, doorComp);
				}
			}
		}
		string selfMsg = base.Loc.GetString("rmc-door-button-pressed-self", (ValueTuple<string, object>)("button", button));
		string othersMsg = base.Loc.GetString("rmc-door-button-pressed-others", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("button", button));
		_popup.PopupPredicted(selfMsg, othersMsg, user, user);
		DoPodDoorButtonAnimation(Entity<RMCDoorButtonComponent>.op_Implicit(button), button.Comp.OnState);
		if (button.Comp.MarineAnnouncement.HasValue)
		{
			ILocalizationManager loc = base.Loc;
			LocId? marineAnnouncement = button.Comp.MarineAnnouncement;
			string announceText = loc.GetString(marineAnnouncement.HasValue ? LocId.op_Implicit(marineAnnouncement.GetValueOrDefault()) : null);
			string author = base.Loc.GetString(LocId.op_Implicit(button.Comp.MarineAnnouncementAuthor));
			_announce.AnnounceHighCommand(announceText, author);
		}
	}

	public void DoPodDoorButtonAnimation(EntityUid button, string animState)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCPodDoorButtonPressedEvent(((EntitySystem)this).GetNetEntity(button, (MetaDataComponent)null), animState), Filter.PvsExcept(button, 2f, (IEntityManager)null), true);
		}
	}

	private void OnBeforePry(Entity<DoorComponent> ent, ref RMCBeforePryEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		DoorComponent door = default(DoorComponent);
		if (((EntitySystem)this).TryComp<DoorComponent>(Entity<DoorComponent>.op_Implicit(ent), ref door) && door.State != DoorState.Closed && (((EntitySystem)this).HasComp<RMCPodDoorComponent>(Entity<DoorComponent>.op_Implicit(ent)) || ((EntitySystem)this).HasComp<XenoComponent>(args.User)))
		{
			args.Cancelled = true;
		}
		if ((!((EntitySystem)this).HasComp<XenoComponent>(args.User) || !((EntitySystem)this).HasComp<AirlockComponent>(Entity<DoorComponent>.op_Implicit(ent))) && _rmcPower.IsPowered(Entity<DoorComponent>.op_Implicit(ent)))
		{
			args.Cancelled = true;
		}
	}

	private void OnDoorPry(Entity<DoorComponent> ent, ref RMCDoorPryEvent args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			_audioSystem.Stop(ent.Comp.SoundEntity, (AudioComponent)null);
		}
		if (((EntitySystem)this).HasComp<XenoComponent>(args.User) && _net.IsServer && !args.Cancelled)
		{
			if (((EntitySystem)this).HasComp<RMCPodDoorComponent>(ent.Owner))
			{
				ent.Comp.SoundEntity = _audioSystem.PlayPredicted(ent.Comp.XenoPodDoorPrySound, ent.Owner, (EntityUid?)ent.Owner, (AudioParams?)null)?.Item1;
			}
			else
			{
				ent.Comp.SoundEntity = _audioSystem.PlayPredicted(ent.Comp.XenoPrySound, ent.Owner, (EntityUid?)ent.Owner, (AudioParams?)null)?.Item1;
			}
		}
	}

	private void OnPodDoorGetPryTimeModifier(Entity<RMCPodDoorComponent> ent, ref GetPryTimeModifierEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.User))
		{
			args.PryTimeModifier *= ent.Comp.XenoPodlockPryMultiplier;
		}
	}

	private void OnDoorBoltStateChanged(Entity<LayerChangeOnWeldComponent> ent, ref DoorBoltsChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		FixturesComponent fixtureComp = default(FixturesComponent);
		DoorComponent door = default(DoorComponent);
		if (!((EntitySystem)this).TryComp<FixturesComponent>(Entity<LayerChangeOnWeldComponent>.op_Implicit(ent), ref fixtureComp) || !((EntitySystem)this).TryComp<DoorComponent>(Entity<LayerChangeOnWeldComponent>.op_Implicit(ent), ref door))
		{
			return;
		}
		foreach (KeyValuePair<string, Fixture> fixture in fixtureComp.Fixtures)
		{
			if (args.BoltsDown)
			{
				if (fixture.Value.CollisionLayer == (int)ent.Comp.UnWeldedLayer && door.State == DoorState.Closed)
				{
					_physics.SetCollisionLayer(Entity<LayerChangeOnWeldComponent>.op_Implicit(ent), fixture.Key, fixture.Value, (int)ent.Comp.WeldedLayer, (FixturesComponent)null, (PhysicsComponent)null);
				}
			}
			else if (fixture.Value.CollisionLayer == (int)ent.Comp.WeldedLayer)
			{
				_physics.SetCollisionLayer(Entity<LayerChangeOnWeldComponent>.op_Implicit(ent), fixture.Key, fixture.Value, (int)ent.Comp.UnWeldedLayer, (FixturesComponent)null, (PhysicsComponent)null);
			}
		}
	}

	private void OnOpenOnlyWhenUnanchoredBeforeClosed(Entity<RMCOpenOnlyWhenUnanchoredComponent> ent, ref BeforeDoorClosedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(Entity<RMCOpenOnlyWhenUnanchoredComponent>.op_Implicit(ent), ref xform) || !xform.Anchored)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private AnchoredEntitiesEnumerator? GetAdjacentEnumerator(Entity<CMDoubleDoorComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transform = default(TransformComponent);
		MapGridComponent grid = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp(Entity<CMDoubleDoorComponent>.op_Implicit(ent), ref transform) || !((EntitySystem)this).TryComp<MapGridComponent>(transform.GridUid, ref grid))
		{
			return null;
		}
		EntityCoordinates coordinates = transform.Coordinates;
		Angle localRotation = transform.LocalRotation;
		EntityCoordinates adjacent = coordinates.Offset(((Angle)(ref localRotation)).GetCardinalDir());
		Vector2i position = _map.LocalToTile(transform.GridUid.Value, grid, adjacent);
		return _map.GetAnchoredEntitiesEnumerator(transform.GridUid.Value, grid, position);
	}

	private bool AreFacing(EntityUid one, EntityUid two)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transformOne = default(TransformComponent);
		TransformComponent transformTwo = default(TransformComponent);
		if (((EntitySystem)this).TryComp(one, ref transformOne) && ((EntitySystem)this).TryComp(two, ref transformTwo))
		{
			Angle localRotation = transformOne.LocalRotation;
			Direction opposite = DirectionExtensions.GetOpposite(((Angle)(ref localRotation)).GetCardinalDir());
			localRotation = transformTwo.LocalRotation;
			return opposite == ((Angle)(ref localRotation)).GetCardinalDir();
		}
		return false;
	}

	private void Open(Entity<CMDoubleDoorComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		AnchoredEntitiesEnumerator? adjacentEnumerator = GetAdjacentEnumerator(ent);
		if (!adjacentEnumerator.HasValue)
		{
			return;
		}
		AnchoredEntitiesEnumerator enumerator = adjacentEnumerator.GetValueOrDefault();
		TimeSpan time = _timing.CurTime;
		ent.Comp.LastOpeningAt = time;
		((EntitySystem)this).Dirty<CMDoubleDoorComponent>(ent, (MetaDataComponent)null);
		EntityUid? anchored = default(EntityUid?);
		CMDoubleDoorComponent doubleDoor = default(CMDoubleDoorComponent);
		DoorComponent door = default(DoorComponent);
		while (((AnchoredEntitiesEnumerator)(ref enumerator)).MoveNext(ref anchored))
		{
			if (_doubleQuery.TryGetComponent(anchored, ref doubleDoor) && doubleDoor.LastOpeningAt != time && AreFacing(Entity<CMDoubleDoorComponent>.op_Implicit(ent), anchored.Value) && _doorQuery.TryGetComponent(anchored, ref door) && door.State != DoorState.Opening)
			{
				doubleDoor.LastOpeningAt = time;
				((EntitySystem)this).Dirty(anchored.Value, (IComponent)(object)doubleDoor, (MetaDataComponent)null);
				SoundSpecifier sound = door.OpenSound;
				door.OpenSound = null;
				door.Partial = false;
				_doors.StartOpening(anchored.Value, door);
				door.OpenSound = sound;
			}
		}
	}

	private void Close(Entity<CMDoubleDoorComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		AnchoredEntitiesEnumerator? adjacentEnumerator = GetAdjacentEnumerator(ent);
		if (!adjacentEnumerator.HasValue)
		{
			return;
		}
		AnchoredEntitiesEnumerator enumerator = adjacentEnumerator.GetValueOrDefault();
		TimeSpan time = _timing.CurTime;
		ent.Comp.LastClosingAt = time;
		((EntitySystem)this).Dirty<CMDoubleDoorComponent>(ent, (MetaDataComponent)null);
		EntityUid? anchored = default(EntityUid?);
		CMDoubleDoorComponent doubleDoor = default(CMDoubleDoorComponent);
		DoorComponent door = default(DoorComponent);
		while (((AnchoredEntitiesEnumerator)(ref enumerator)).MoveNext(ref anchored))
		{
			if (_doubleQuery.TryGetComponent(anchored, ref doubleDoor) && doubleDoor.LastClosingAt != time && AreFacing(Entity<CMDoubleDoorComponent>.op_Implicit(ent), anchored.Value) && _doorQuery.TryGetComponent(anchored, ref door) && door.State != DoorState.Closing)
			{
				doubleDoor.LastClosingAt = time;
				((EntitySystem)this).Dirty(anchored.Value, (IComponent)(object)doubleDoor, (MetaDataComponent)null);
				SoundSpecifier sound = door.CloseSound;
				door.CloseSound = null;
				door.Partial = false;
				_doors.StartClosing(anchored.Value, door);
				door.CloseSound = sound;
			}
		}
	}
}
