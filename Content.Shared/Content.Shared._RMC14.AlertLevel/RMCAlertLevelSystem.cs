using System;
using System.Collections.Generic;
using Content.Shared._RMC14.ARES;
using Content.Shared._RMC14.Doors;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Ghost;
using Content.Shared.Lock;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.AlertLevel;

public sealed class RMCAlertLevelSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private ARESSystem _ares;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoorSystem _door;

	[Dependency]
	private SharedEntityStorageSystem _entityStorage;

	[Dependency]
	private LockSystem _lock;

	[Dependency]
	private SharedMarineAnnounceSystem _marineAnnounce;

	[Dependency]
	private INetManager _net;

	private EntityQuery<GhostComponent> _ghostQuery;

	public override void Initialize()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).SubscribeLocalEvent<DropshipHijackLandedEvent>((EntityEventRefHandler<DropshipHijackLandedEvent>)OnDropshipHijackLanded, (Type[])null, (Type[])null);
		_ghostQuery = ((EntitySystem)this).GetEntityQuery<GhostComponent>();
	}

	private void OnDropshipHijackLanded(ref DropshipHijackLandedEvent ev)
	{
	}

	private bool TryGetAlertLevel(out Entity<RMCAlertLevelComponent> alert)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = default(EntityUid);
		RMCAlertLevelComponent comp = default(RMCAlertLevelComponent);
		if (((EntitySystem)this).EntityQueryEnumerator<RMCAlertLevelComponent>().MoveNext(ref uid, ref comp))
		{
			alert = Entity<RMCAlertLevelComponent>.op_Implicit((uid, comp));
			return true;
		}
		alert = default(Entity<RMCAlertLevelComponent>);
		return false;
	}

	private Entity<RMCAlertLevelComponent> EnsureAlertLevel()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetAlertLevel(out Entity<RMCAlertLevelComponent> alert))
		{
			return alert;
		}
		EntityUid uid = ((EntitySystem)this).Spawn((string)null, (ComponentRegistry)null, true);
		RMCAlertLevelComponent comp = ((EntitySystem)this).EnsureComp<RMCAlertLevelComponent>(uid);
		return Entity<RMCAlertLevelComponent>.op_Implicit((uid, comp));
	}

	public RMCAlertLevels? Get()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetAlertLevel(out Entity<RMCAlertLevelComponent> alert))
		{
			return null;
		}
		return alert.Comp.Level;
	}

	public bool IsRedOrDeltaAlert()
	{
		if (Get() != RMCAlertLevels.Red)
		{
			return Get() == RMCAlertLevels.Delta;
		}
		return true;
	}

	public void Set(RMCAlertLevels level, EntityUid? user, bool playSound = true, bool sendAnnouncement = true)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		Entity<RMCAlertLevelComponent> ent = EnsureAlertLevel();
		if (ent.Comp.Level == level)
		{
			return;
		}
		(SoundSpecifier, LocId?, LocId?) tuple;
		switch (level)
		{
		case RMCAlertLevels.Green:
			tuple = (ent.Comp.GreenSound, ent.Comp.GreenMessage, null);
			break;
		case RMCAlertLevels.Blue:
			if (ent.Comp.Level < RMCAlertLevels.Blue)
			{
				tuple = (ent.Comp.BlueElevatedSound, ent.Comp.BlueElevatedMessage, null);
				break;
			}
			if (ent.Comp.Level > RMCAlertLevels.Blue)
			{
				tuple = (ent.Comp.BlueLoweredSound, ent.Comp.BlueLoweredMessage, null);
				break;
			}
			goto default;
		case RMCAlertLevels.Red:
			if (ent.Comp.Level < RMCAlertLevels.Red)
			{
				tuple = (ent.Comp.RedElevatedSound, ent.Comp.RedElevatedMessage, null);
				break;
			}
			if (ent.Comp.Level > RMCAlertLevels.Red)
			{
				tuple = (ent.Comp.RedLoweredSound, ent.Comp.RedLoweredMessage, null);
				break;
			}
			goto default;
		case RMCAlertLevels.Delta:
			tuple = (ent.Comp.DeltaSound, ent.Comp.DeltaAnnouncement, ent.Comp.DeltaAnnouncement);
			break;
		default:
			tuple = (null, null, null);
			break;
		}
		var (sound, message, announcement) = tuple;
		ent.Comp.Level = level;
		((EntitySystem)this).Dirty<RMCAlertLevelComponent>(ent, (MetaDataComponent)null);
		ISharedAdminLogManager adminLog = _adminLog;
		LogStringHandler handler = new LogStringHandler(20, 2);
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString(user, (MetaDataComponent)null), "ToPrettyString(user)");
		handler.AppendLiteral(" set alert level to ");
		handler.AppendFormatted(level, "level");
		adminLog.Add(LogType.RMCAlertLevel, ref handler);
		HashSet<EntityUid> almayers = new HashSet<EntityUid>();
		EntityQueryEnumerator<AlmayerComponent> almayerQuery = ((EntitySystem)this).EntityQueryEnumerator<AlmayerComponent>();
		EntityUid uid = default(EntityUid);
		AlmayerComponent almayerComponent = default(AlmayerComponent);
		while (almayerQuery.MoveNext(ref uid, ref almayerComponent))
		{
			almayers.Add(uid);
		}
		EntityQuery<TransformComponent> transformQuery = base.EntityManager.TransformQuery;
		Filter filter = Filter.Empty().AddWhereAttachedEntity((Predicate<EntityUid>)delegate(EntityUid entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			TransformComponent obj = transformQuery.CompOrNull(entity);
			EntityUid? val2 = ((obj != null) ? obj.MapUid : ((EntityUid?)null));
			if (val2.HasValue)
			{
				EntityUid valueOrDefault = val2.GetValueOrDefault();
				if (almayers.Contains(valueOrDefault))
				{
					return true;
				}
			}
			return _ghostQuery.HasComp(entity) ? true : false;
		});
		if (playSound && _net.IsServer)
		{
			_audio.PlayGlobal(sound, filter, true, (AudioParams?)null);
		}
		if (sendAnnouncement)
		{
			if (announcement.HasValue)
			{
				SharedMarineAnnounceSystem marineAnnounce = _marineAnnounce;
				ILocalizationManager loc = base.Loc;
				LocId? val = announcement;
				marineAnnounce.AnnounceToMarines(loc.GetString(val.HasValue ? LocId.op_Implicit(val.GetValueOrDefault()) : null));
			}
			else if (message.HasValue)
			{
				Entity<ARESComponent> ares = _ares.EnsureARES();
				_marineAnnounce.AnnounceRadio(Entity<ARESComponent>.op_Implicit(ares), base.Loc.GetString(LocId.op_Implicit(message.Value)), ent.Comp.RadioChannel);
			}
		}
		EntityQueryEnumerator<RMCUnlockOnAlertLevelComponent, LockComponent> unlockQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCUnlockOnAlertLevelComponent, LockComponent>();
		EntityUid uid2 = default(EntityUid);
		RMCUnlockOnAlertLevelComponent unlock = default(RMCUnlockOnAlertLevelComponent);
		LockComponent lockComp = default(LockComponent);
		while (unlockQuery.MoveNext(ref uid2, ref unlock, ref lockComp))
		{
			if (unlock.Level <= level)
			{
				_lock.Unlock(uid2, null, lockComp);
				continue;
			}
			SharedEntityStorageComponent entityStorageComp = null;
			if (_entityStorage.ResolveStorage(uid2, ref entityStorageComp))
			{
				_entityStorage.CloseStorage(uid2, entityStorageComp);
			}
			_lock.Lock(uid2, null, lockComp);
		}
		EntityQueryEnumerator<RMCOpenOnAlertLevelComponent, DoorComponent, RMCPodDoorComponent> openQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCOpenOnAlertLevelComponent, DoorComponent, RMCPodDoorComponent>();
		EntityUid uid3 = default(EntityUid);
		RMCOpenOnAlertLevelComponent unlock2 = default(RMCOpenOnAlertLevelComponent);
		DoorComponent door = default(DoorComponent);
		RMCPodDoorComponent podDoor = default(RMCPodDoorComponent);
		while (openQuery.MoveNext(ref uid3, ref unlock2, ref door, ref podDoor))
		{
			if (!(unlock2.Id != podDoor.Id))
			{
				if (unlock2.Level <= level)
				{
					_door.TryOpen(uid3, door);
				}
				else
				{
					_door.TryClose(uid3, door);
				}
			}
		}
		EntityQueryEnumerator<RMCAlertLevelDisplayComponent> displays = ((EntitySystem)this).EntityQueryEnumerator<RMCAlertLevelDisplayComponent>();
		EntityUid uid4 = default(EntityUid);
		RMCAlertLevelDisplayComponent rMCAlertLevelDisplayComponent = default(RMCAlertLevelDisplayComponent);
		while (displays.MoveNext(ref uid4, ref rMCAlertLevelDisplayComponent))
		{
			_appearance.SetData(uid4, (Enum)RMCAlertLevelsVisuals.Alert, (object)level, (AppearanceComponent)null);
		}
		RMCAlertLevelChangedEvent ev = new RMCAlertLevelChangedEvent(ent.Comp.Level);
		((EntitySystem)this).RaiseLocalEvent<RMCAlertLevelChangedEvent>(Entity<RMCAlertLevelComponent>.op_Implicit(ent), ref ev, true);
	}
}
