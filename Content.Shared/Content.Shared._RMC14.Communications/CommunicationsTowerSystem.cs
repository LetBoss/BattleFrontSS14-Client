using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.Intel;
using Content.Shared._RMC14.Power;
using Content.Shared._RMC14.Tools;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared._RMC14.Xenonids.ManageHive.Boons;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Power;
using Content.Shared.Radio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Communications;

public sealed class CommunicationsTowerSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private DialogSystem _dialog;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private HiveBoonSystem _hiveBoon;

	[Dependency]
	private IntelSystem _intel;

	[Dependency]
	private GunIFFSystem _gunIFF;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedRMCPowerSystem _rmcPower;

	[Dependency]
	private SharedTransformSystem _transform;

	private readonly Dictionary<EntProtoId, List<Entity<CommunicationsTowerSpawnerComponent>>> _spawners = new Dictionary<EntProtoId, List<Entity<CommunicationsTowerSpawnerComponent>>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<CommunicationsTowerComponent, MapInitEvent>((EntityEventRefHandler<CommunicationsTowerComponent, MapInitEvent>)OnTowerMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CommunicationsTowerComponent, DamageChangedEvent>((EntityEventRefHandler<CommunicationsTowerComponent, DamageChangedEvent>)OnTowerDamageChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CommunicationsTowerComponent, BreakageEventArgs>((EntityEventRefHandler<CommunicationsTowerComponent, BreakageEventArgs>)OnTowerBreakage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CommunicationsTowerComponent, ExaminedEvent>((EntityEventRefHandler<CommunicationsTowerComponent, ExaminedEvent>)OnTowerExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CommunicationsTowerComponent, InteractUsingEvent>((EntityEventRefHandler<CommunicationsTowerComponent, InteractUsingEvent>)OnTowerInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CommunicationsTowerComponent, DialogChosenEvent>((EntityEventRefHandler<CommunicationsTowerComponent, DialogChosenEvent>)OnTowerDialogChosen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CommunicationsTowerComponent, CommunicationsTowerWipeDoAfterEvent>((EntityEventRefHandler<CommunicationsTowerComponent, CommunicationsTowerWipeDoAfterEvent>)OnTowerDialogWipeDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CommunicationsTowerComponent, CommunicationsTowerAddDoAfterEvent>((EntityEventRefHandler<CommunicationsTowerComponent, CommunicationsTowerAddDoAfterEvent>)OnTowerDialogAddDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CommunicationsTowerComponent, InteractHandEvent>((EntityEventRefHandler<CommunicationsTowerComponent, InteractHandEvent>)OnTowerInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CommunicationsTowerComponent, PowerChangedEvent>((EntityEventRefHandler<CommunicationsTowerComponent, PowerChangedEvent>)OnTowerPowerChangedEvent, (Type[])null, (Type[])null);
	}

	private void OnTowerMapInit(Entity<CommunicationsTowerComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(ent);
	}

	private void OnTowerDamageChanged(Entity<CommunicationsTowerComponent> ent, ref DamageChangedEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.Damageable.TotalDamage > FixedPoint2.Zero) && ent.Comp.State == CommunicationsTowerState.Broken)
		{
			ChangeState(ent, CommunicationsTowerState.Off);
		}
	}

	private void OnTowerBreakage(Entity<CommunicationsTowerComponent> ent, ref BreakageEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ChangeState(ent, CommunicationsTowerState.Broken);
	}

	private void OnTowerExamined(Entity<CommunicationsTowerComponent> ent, ref ExaminedEvent args)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("CommunicationsTowerComponent"))
		{
			string msg = $"[color=cyan]If placed {(int)_hiveBoon.CommunicationTowerXenoTakeoverTime.TotalMinutes} minutes into the round, a hive cluster will turn into a hive pylon when its weeds take over this![/color]";
			args.PushMarkup(msg);
			if (ent.Comp.State == CommunicationsTowerState.Broken)
			{
				args.PushMarkup("[color=red]It is damaged and needs a welder for repairs![/color]");
			}
		}
	}

	private void OnTowerInteractUsing(Entity<CommunicationsTowerComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.State != CommunicationsTowerState.Broken)
		{
			RMCDeviceBreakerComponent breaker = default(RMCDeviceBreakerComponent);
			if (((EntitySystem)this).TryComp<RMCDeviceBreakerComponent>(args.Used, ref breaker) && ent.Comp.State != CommunicationsTowerState.Broken)
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
			else if (((EntitySystem)this).HasComp<MultitoolComponent>(args.Used))
			{
				List<DialogOption> options = new List<DialogOption>
				{
					new DialogOption("Wipe communication frequencies"),
					new DialogOption("Add your faction's frequencies")
				};
				_dialog.OpenOptions(Entity<CommunicationsTowerComponent>.op_Implicit(ent), args.User, "TC-3T comms tower", options);
			}
		}
	}

	private void OnTowerDialogChosen(Entity<CommunicationsTowerComponent> ent, ref DialogChosenEvent args)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan delay = TimeSpan.Zero;
		DoAfterEvent ev;
		if (args.Index == 0)
		{
			ev = new CommunicationsTowerWipeDoAfterEvent();
		}
		else
		{
			ev = new CommunicationsTowerAddDoAfterEvent();
			delay = TimeSpan.FromSeconds(1L);
		}
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.Actor, delay, ev, Entity<CommunicationsTowerComponent>.op_Implicit(ent))
		{
			BreakOnMove = true
		};
		_doAfter.TryStartDoAfter(doAfter);
	}

	private void OnTowerDialogWipeDoAfter(Entity<CommunicationsTowerComponent> ent, ref CommunicationsTowerWipeDoAfterEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled && ent.Comp.State != CommunicationsTowerState.Broken)
		{
			((HandledEntityEventArgs)args).Handled = true;
			string msg = "You wipe the preexisting frequencies from the " + ((EntitySystem)this).Name(Entity<CommunicationsTowerComponent>.op_Implicit(ent), (MetaDataComponent)null) + ".";
			_popup.PopupClient(msg, Entity<CommunicationsTowerComponent>.op_Implicit(ent), args.User, PopupType.Medium);
		}
	}

	private void OnTowerDialogAddDoAfter(Entity<CommunicationsTowerComponent> ent, ref CommunicationsTowerAddDoAfterEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled && ent.Comp.State != CommunicationsTowerState.Broken)
		{
			EntityPrototype factionProto = default(EntityPrototype);
			FactionFrequenciesComponent frequencies = default(FactionFrequenciesComponent);
			if (_gunIFF.TryGetFaction(Entity<UserIFFComponent>.op_Implicit(args.User), out EntProtoId<IFFFactionComponent> faction) && _prototypes.TryIndex(EntProtoId<IFFFactionComponent>.op_Implicit(faction), ref factionProto) && factionProto.TryGetComponent<FactionFrequenciesComponent>(ref frequencies, _compFactory))
			{
				ent.Comp.Channels.UnionWith(frequencies.Channels);
				((EntitySystem)this).Dirty<CommunicationsTowerComponent>(ent, (MetaDataComponent)null);
			}
			((HandledEntityEventArgs)args).Handled = true;
			string msg = "You add your faction's communication frequencies to the " + ((EntitySystem)this).Name(Entity<CommunicationsTowerComponent>.op_Implicit(ent), (MetaDataComponent)null) + "'s comm list.";
			_popup.PopupClient(msg, Entity<CommunicationsTowerComponent>.op_Implicit(ent), args.User, PopupType.Medium);
		}
	}

	private void OnTowerInteractHand(Entity<CommunicationsTowerComponent> ent, ref InteractHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.State == CommunicationsTowerState.Broken)
		{
			_popup.PopupClient(((EntitySystem)this).Name(Entity<CommunicationsTowerComponent>.op_Implicit(ent), (MetaDataComponent)null) + " needs repairs to be turned back on!", Entity<CommunicationsTowerComponent>.op_Implicit(ent), args.User, PopupType.MediumCaution);
			return;
		}
		if (!_rmcPower.IsPowered(Entity<CommunicationsTowerComponent>.op_Implicit(ent)))
		{
			_popup.PopupClient(((EntitySystem)this).Name(Entity<CommunicationsTowerComponent>.op_Implicit(ent), (MetaDataComponent)null) + " makes a small plaintful beep, and nothing happens. It seems to be out of power.", Entity<CommunicationsTowerComponent>.op_Implicit(ent), args.User, PopupType.MediumCaution);
			return;
		}
		CommunicationsTowerState state = ent.Comp.State switch
		{
			CommunicationsTowerState.Off => CommunicationsTowerState.On, 
			CommunicationsTowerState.On => CommunicationsTowerState.Off, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		ISharedAdminLogManager adminLog = _adminLog;
		LogStringHandler handler = new LogStringHandler(10, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
		handler.AppendLiteral(" turned ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<CommunicationsTowerComponent>.op_Implicit(ent), (MetaDataComponent)null), "tower", "ToPrettyString(ent)");
		handler.AppendLiteral(" ");
		handler.AppendFormatted(state, "state");
		handler.AppendLiteral(".");
		adminLog.Add(LogType.RMCCommunicationsTower, ref handler);
		ChangeState(ent, state);
		if (ent.Comp.State == CommunicationsTowerState.On)
		{
			_intel.RestoreColonyCommunications();
		}
	}

	private void OnTowerPowerChangedEvent(Entity<CommunicationsTowerComponent> ent, ref PowerChangedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.State == CommunicationsTowerState.On)
		{
			if (args.Powered)
			{
				_intel.RestoreColonyCommunications();
			}
			else
			{
				ChangeState(ent, CommunicationsTowerState.Off);
			}
		}
	}

	private void ChangeState(Entity<CommunicationsTowerComponent> tower, CommunicationsTowerState newState)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		tower.Comp.State = newState;
		((EntitySystem)this).Dirty<CommunicationsTowerComponent>(tower, (MetaDataComponent)null);
		CommunicationsTowerStateChangedEvent ev = new CommunicationsTowerStateChangedEvent(tower);
		((EntitySystem)this).RaiseLocalEvent<CommunicationsTowerStateChangedEvent>(Entity<CommunicationsTowerComponent>.op_Implicit(tower), ev, false);
		UpdateAppearance(tower);
	}

	public bool CanTransmit(ProtoId<RadioChannelPrototype> channel)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<CommunicationsTowerComponent> towers = ((EntitySystem)this).EntityQueryEnumerator<CommunicationsTowerComponent>();
		CommunicationsTowerComponent tower = default(CommunicationsTowerComponent);
		while (towers.MoveNext(ref tower))
		{
			if (tower.State == CommunicationsTowerState.On && tower.Channels.Contains(channel))
			{
				return true;
			}
		}
		return false;
	}

	public void UpdateAppearance(Entity<CommunicationsTowerComponent> tower)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<CommunicationsTowerComponent>.op_Implicit(tower), (Enum)CommunicationsTowerLayers.Layer, (object)tower.Comp.State, (AppearanceComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		_spawners.Clear();
		EntityQueryEnumerator<CommunicationsTowerSpawnerComponent> spawnersQuery = ((EntitySystem)this).EntityQueryEnumerator<CommunicationsTowerSpawnerComponent>();
		EntityUid uid = default(EntityUid);
		CommunicationsTowerSpawnerComponent spawner = default(CommunicationsTowerSpawnerComponent);
		while (spawnersQuery.MoveNext(ref uid, ref spawner))
		{
			if (!((EntitySystem)this).TerminatingOrDeleted(uid, (MetaDataComponent)null) && !base.EntityManager.IsQueuedForDeletion(uid))
			{
				((EntitySystem)this).QueueDel((EntityUid?)uid);
				Extensions.GetOrNew<EntProtoId, List<Entity<CommunicationsTowerSpawnerComponent>>>(_spawners, spawner.Group).Add(Entity<CommunicationsTowerSpawnerComponent>.op_Implicit((uid, spawner)));
			}
		}
		foreach (List<Entity<CommunicationsTowerSpawnerComponent>> spawners in _spawners.Values)
		{
			if (spawners.Count != 0)
			{
				Entity<CommunicationsTowerSpawnerComponent> spawner2 = RandomExtensions.Pick<Entity<CommunicationsTowerSpawnerComponent>>(_random, (IReadOnlyList<Entity<CommunicationsTowerSpawnerComponent>>)spawners);
				((EntitySystem)this).Spawn(EntProtoId.op_Implicit(spawner2.Comp.Spawn), _transform.GetMoverCoordinates(Entity<CommunicationsTowerSpawnerComponent>.op_Implicit(spawner2)));
			}
		}
	}
}
