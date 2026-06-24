using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.Tracker;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Content.Shared._RMC14.Xenonids.Watch;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Xenonids.HiveLeader;

public sealed class HiveLeaderSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private DialogSystem _dialog;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedCMChatSystem _rmcChat;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedXenoWatchSystem _xenoWatch;

	private EntityQuery<XenoAttachedOvipositorComponent> _attachedOvipositorQuery;

	private EntityQuery<HiveLeaderComponent> _hiveLeaderQuery;

	private EntityQuery<HiveLeaderGranterComponent> _hiveLeaderGranterQuery;

	private EntityQuery<XenoActivePheromonesComponent> _activePheromonesQuery;

	private EntityQuery<XenoPheromonesComponent> _pheromonesQuery;

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
		_attachedOvipositorQuery = ((EntitySystem)this).GetEntityQuery<XenoAttachedOvipositorComponent>();
		_hiveLeaderQuery = ((EntitySystem)this).GetEntityQuery<HiveLeaderComponent>();
		_hiveLeaderGranterQuery = ((EntitySystem)this).GetEntityQuery<HiveLeaderGranterComponent>();
		_activePheromonesQuery = ((EntitySystem)this).GetEntityQuery<XenoActivePheromonesComponent>();
		_pheromonesQuery = ((EntitySystem)this).GetEntityQuery<XenoPheromonesComponent>();
		((EntitySystem)this).SubscribeLocalEvent<NewXenoEvolvedEvent>((EntityEventRefHandler<NewXenoEvolvedEvent>)OnLeaderNewXenoEvolved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDevolvedEvent>((EntityEventRefHandler<XenoDevolvedEvent>)OnLeaderXenoDevolved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveLeaderComponent, ComponentRemove>((EntityEventRefHandler<HiveLeaderComponent, ComponentRemove>)OnLeaderRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveLeaderComponent, EntityTerminatingEvent>((EntityEventRefHandler<HiveLeaderComponent, EntityTerminatingEvent>)OnLeaderRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveLeaderComponent, MobStateChangedEvent>((EntityEventRefHandler<HiveLeaderComponent, MobStateChangedEvent>)OnLeaderMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveLeaderGranterComponent, ComponentRemove>((EntityEventRefHandler<HiveLeaderGranterComponent, ComponentRemove>)OnGranterRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveLeaderGranterComponent, EntityTerminatingEvent>((EntityEventRefHandler<HiveLeaderGranterComponent, EntityTerminatingEvent>)OnGranterRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveLeaderGranterComponent, MobStateChangedEvent>((EntityEventRefHandler<HiveLeaderGranterComponent, MobStateChangedEvent>)OnGranterMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveLeaderGranterComponent, HiveLeaderActionEvent>((EntityEventRefHandler<HiveLeaderGranterComponent, HiveLeaderActionEvent>)OnGranterAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveLeaderGranterComponent, HiveLeaderWatchEvent>((EntityEventRefHandler<HiveLeaderGranterComponent, HiveLeaderWatchEvent>)OnGranterWatch, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveLeaderGranterComponent, XenoPheromonesActivatedEvent>((EntityEventRefHandler<HiveLeaderGranterComponent, XenoPheromonesActivatedEvent>)OnGranterPheromonesActivated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveLeaderGranterComponent, XenoPheromonesDeactivatedEvent>((EntityEventRefHandler<HiveLeaderGranterComponent, XenoPheromonesDeactivatedEvent>)OnGranterPheromonesDeactivated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveLeaderGranterComponent, XenoOvipositorChangedEvent>((EntityEventRefHandler<HiveLeaderGranterComponent, XenoOvipositorChangedEvent>)OnGranterOvipositorChanged, (Type[])null, (Type[])null);
	}

	private void OnLeaderRemove<T>(Entity<HiveLeaderComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RemoveLeader(ent);
	}

	private void OnLeaderMobStateChanged(Entity<HiveLeaderComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState == MobState.Dead)
		{
			RemoveLeader(ent);
		}
	}

	private void OnLeaderNewXenoEvolved(ref NewXenoEvolvedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Transfer(Entity<XenoEvolutionComponent>.op_Implicit(args.OldXeno), args.NewXeno);
	}

	private void OnLeaderXenoDevolved(ref XenoDevolvedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Transfer(args.OldXeno, args.NewXeno);
	}

	private void OnGranterRemove<T>(Entity<HiveLeaderGranterComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RemoveLeaders(ent);
	}

	private void OnGranterMobStateChanged(Entity<HiveLeaderGranterComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState == MobState.Dead)
		{
			RemoveLeaders(ent);
		}
	}

	private void OnGranterAction(Entity<HiveLeaderGranterComponent> ent, ref HiveLeaderActionEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		List<EntityUid> leaders = ent.Comp.Leaders;
		int max = ent.Comp.MaxLeaders;
		if (!_xenoWatch.TryGetWatched(Entity<XenoWatchingComponent>.op_Implicit(ent.Owner), out var watching))
		{
			if (leaders.Count == 0)
			{
				string msg = "There are no Xenonid leaders. Overwatch a Xenonid to make it a leader.";
				_popup.PopupClient(msg, Entity<HiveLeaderGranterComponent>.op_Implicit(ent), Entity<HiveLeaderGranterComponent>.op_Implicit(ent), PopupType.MediumCaution);
				return;
			}
			List<DialogOption> options = new List<DialogOption>();
			foreach (EntityUid leader in leaders)
			{
				options.Add(new DialogOption(((EntitySystem)this).Name(leader, (MetaDataComponent)null), new HiveLeaderWatchEvent(((EntitySystem)this).GetNetEntity(leader, (MetaDataComponent)null))));
			}
			_dialog.OpenOptions(Entity<HiveLeaderGranterComponent>.op_Implicit(ent), "Watch with leader?", options, "Target");
		}
		else if (!(ent.Owner == watching))
		{
			HiveLeaderComponent leaderComp = default(HiveLeaderComponent);
			if (!((EntitySystem)this).HasComp<HiveLeaderComponent>(watching) && leaders.Count >= max)
			{
				string msg = $"You can't have more than {max} promoted leaders.";
				_popup.PopupClient(msg, watching, Entity<HiveLeaderGranterComponent>.op_Implicit(ent), PopupType.MediumCaution);
			}
			else if (((EntitySystem)this).EnsureComp<HiveLeaderComponent>(watching, ref leaderComp))
			{
				((EntitySystem)this).RemCompDeferred<HiveLeaderComponent>(watching);
				((EntitySystem)this).RemComp<RMCTrackableComponent>(watching);
				ent.Comp.Leaders.Remove(watching);
				string msg = "You've demoted " + ((EntitySystem)this).Name(watching, (MetaDataComponent)null) + " from Hive Leader.";
				_popup.PopupClient(msg, watching, Entity<HiveLeaderGranterComponent>.op_Implicit(ent), PopupType.MediumCaution);
				msg = ((EntitySystem)this).Name(Entity<HiveLeaderGranterComponent>.op_Implicit(ent), (MetaDataComponent)null) + " has demoted you from Hive Leader. Your leadership rights and abilities have waned.";
				_popup.PopupEntity(msg, watching, watching, PopupType.MediumCaution);
				_rmcChat.ChatMessageToOne(msg, watching);
				HiveLeaderStatusChangedEvent evn = new HiveLeaderStatusChangedEvent(BecameLeader: false);
				((EntitySystem)this).RaiseLocalEvent<HiveLeaderStatusChangedEvent>(watching, ref evn, false);
			}
			else
			{
				((EntitySystem)this).EnsureComp<RMCTrackableComponent>(watching);
				leaderComp.Granter = Entity<HiveLeaderGranterComponent>.op_Implicit(ent);
				((EntitySystem)this).Dirty(watching, (IComponent)(object)leaderComp, (MetaDataComponent)null);
				ent.Comp.Leaders.Add(watching);
				((EntitySystem)this).Dirty<HiveLeaderGranterComponent>(ent, (MetaDataComponent)null);
				string msg = "You've selected " + ((EntitySystem)this).Name(watching, (MetaDataComponent)null) + " as a Hive Leader.";
				_popup.PopupClient(msg, watching, Entity<HiveLeaderGranterComponent>.op_Implicit(ent), PopupType.Medium);
				msg = ((EntitySystem)this).Name(Entity<HiveLeaderGranterComponent>.op_Implicit(ent), (MetaDataComponent)null) + " has selected you as a Hive Leader. The other Xenonids must listen to you. You will also act as a beacon for the Queen's pheromones.";
				_popup.PopupClient(msg, watching, watching, PopupType.Medium);
				_rmcChat.ChatMessageToOne(msg, watching);
				HiveLeaderStatusChangedEvent ev = new HiveLeaderStatusChangedEvent(BecameLeader: true);
				((EntitySystem)this).RaiseLocalEvent<HiveLeaderStatusChangedEvent>(watching, ref ev, false);
				SyncPheromones(ent);
			}
		}
	}

	private void OnGranterWatch(Entity<HiveLeaderGranterComponent> ent, ref HiveLeaderWatchEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? leader = default(EntityUid?);
		if (((EntitySystem)this).TryGetEntity(args.Leader, ref leader) && _hiveLeaderQuery.HasComp(leader))
		{
			_xenoWatch.Watch(Entity<HiveMemberComponent, ActorComponent, EyeComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(leader.Value));
		}
	}

	private void OnGranterPheromonesActivated(Entity<HiveLeaderGranterComponent> ent, ref XenoPheromonesActivatedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SyncPheromones(ent);
	}

	private void OnGranterPheromonesDeactivated(Entity<HiveLeaderGranterComponent> ent, ref XenoPheromonesDeactivatedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SyncPheromones(ent, forceDisable: true);
	}

	private void OnGranterOvipositorChanged(Entity<HiveLeaderGranterComponent> ent, ref XenoOvipositorChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SyncPheromones(ent);
	}

	private void RemoveLeaders(Entity<HiveLeaderGranterComponent> ent)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.ApplyingState)
		{
			return;
		}
		SyncPheromones(ent, forceDisable: true);
		foreach (EntityUid leader in ent.Comp.Leaders)
		{
			((EntitySystem)this).RemCompDeferred<HiveLeaderComponent>(leader);
			HiveLeaderStatusChangedEvent ev = new HiveLeaderStatusChangedEvent(BecameLeader: false);
			((EntitySystem)this).RaiseLocalEvent<HiveLeaderStatusChangedEvent>(leader, ref ev, false);
		}
		ent.Comp.Leaders.Clear();
	}

	private void SyncPheromones(Entity<HiveLeaderGranterComponent> ent, bool forceDisable = false)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		XenoPheromonesComponent pheromone = default(XenoPheromonesComponent);
		if (_timing.ApplyingState || !_pheromonesQuery.TryComp(Entity<HiveLeaderGranterComponent>.op_Implicit(ent), ref pheromone))
		{
			return;
		}
		XenoActivePheromonesComponent active = default(XenoActivePheromonesComponent);
		bool hasPheromones = _activePheromonesQuery.TryComp(Entity<HiveLeaderGranterComponent>.op_Implicit(ent), ref active) && _attachedOvipositorQuery.HasComp(Entity<HiveLeaderGranterComponent>.op_Implicit(ent)) && !_mobState.IsDead(Entity<HiveLeaderGranterComponent>.op_Implicit(ent)) && !forceDisable;
		HiveLeaderComponent leaderComp = default(HiveLeaderComponent);
		BaseContainer container = default(BaseContainer);
		EntityUid? first = default(EntityUid?);
		EntityUid? spawnedRelayEnt = default(EntityUid?);
		foreach (EntityUid leader in ent.Comp.Leaders)
		{
			if (!_hiveLeaderQuery.TryComp(leader, ref leaderComp))
			{
				continue;
			}
			if (!hasPheromones)
			{
				if (_container.TryGetContainer(leader, leaderComp.PheromonesContainerId, ref container, (ContainerManagerComponent)null) && Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)container.ContainedEntities, ref first))
				{
					((EntitySystem)this).RemComp<XenoActivePheromonesComponent>(first.Value);
				}
				continue;
			}
			ContainerSlot slot = _container.EnsureContainer<ContainerSlot>(leader, leaderComp.PheromonesContainerId, (ContainerManagerComponent)null);
			EntityUid relayEnt;
			if (!slot.ContainedEntity.HasValue)
			{
				if (!((EntitySystem)this).TrySpawnInContainer(EntProtoId.op_Implicit(ent.Comp.PheromoneRelayId), leader, leaderComp.PheromonesContainerId, ref spawnedRelayEnt, (ContainerManagerComponent)null, (ComponentRegistry)null))
				{
					continue;
				}
				relayEnt = spawnedRelayEnt.Value;
			}
			else
			{
				relayEnt = slot.ContainedEntity.Value;
			}
			XenoPheromonesComponent relayPheromones = ((EntitySystem)this).EnsureComp<XenoPheromonesComponent>(relayEnt);
			relayPheromones.PheromonesPlasmaCost = 0;
			relayPheromones.PheromonesPlasmaUpkeep = 0;
			relayPheromones.PheromonesRange = pheromone.PheromonesRange;
			relayPheromones.PheromonesMultiplier = pheromone.PheromonesMultiplier;
			((EntitySystem)this).Dirty(relayEnt, (IComponent)(object)relayPheromones, (MetaDataComponent)null);
			XenoActivePheromonesComponent relayActive = ((EntitySystem)this).EnsureComp<XenoActivePheromonesComponent>(relayEnt);
			if (active != null)
			{
				relayActive.Pheromones = active.Pheromones;
			}
		}
	}

	private void RemoveLeader(Entity<HiveLeaderComponent> leader)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			BaseContainer container = default(BaseContainer);
			EntityUid? first = default(EntityUid?);
			if (_container.TryGetContainer(Entity<HiveLeaderComponent>.op_Implicit(leader), leader.Comp.PheromonesContainerId, ref container, (ContainerManagerComponent)null) && Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)container.ContainedEntities, ref first))
			{
				((EntitySystem)this).RemComp<XenoActivePheromonesComponent>(first.Value);
			}
			((EntitySystem)this).RemCompDeferred<HiveLeaderComponent>(Entity<HiveLeaderComponent>.op_Implicit(leader));
			HiveLeaderGranterComponent granter = default(HiveLeaderGranterComponent);
			if (((EntitySystem)this).TryComp<HiveLeaderGranterComponent>(leader.Comp.Granter, ref granter))
			{
				granter.Leaders.Remove(Entity<HiveLeaderComponent>.op_Implicit(leader));
				((EntitySystem)this).Dirty(leader.Comp.Granter.Value, (IComponent)(object)granter, (MetaDataComponent)null);
				SyncPheromones(Entity<HiveLeaderGranterComponent>.op_Implicit((leader.Comp.Granter.Value, granter)));
			}
		}
	}

	private void Transfer(EntityUid oldXeno, EntityUid newXeno)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		HiveLeaderComponent oldLeader = default(HiveLeaderComponent);
		HiveLeaderGranterComponent granter = default(HiveLeaderGranterComponent);
		if (_hiveLeaderQuery.TryComp(oldXeno, ref oldLeader) && _hiveLeaderGranterQuery.TryComp(oldLeader.Granter, ref granter) && !_hiveLeaderGranterQuery.HasComp(newXeno))
		{
			((EntitySystem)this).EnsureComp<RMCTrackableComponent>(newXeno);
			((EntitySystem)this).EnsureComp<HiveLeaderComponent>(newXeno).Granter = oldLeader.Granter;
			granter.Leaders.Remove(oldXeno);
			granter.Leaders.Add(newXeno);
			SyncPheromones(Entity<HiveLeaderGranterComponent>.op_Implicit((oldLeader.Granter.Value, granter)));
		}
	}

	public bool IsLeader(EntityUid leader, [NotNullWhen(true)] out HiveLeaderComponent? leaderComp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ((EntitySystem)this).TryComp<HiveLeaderComponent>(leader, ref leaderComp);
	}
}
