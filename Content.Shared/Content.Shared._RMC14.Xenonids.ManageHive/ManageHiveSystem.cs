using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Commendations;
using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.ManageHive.Boons;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Watch;
using Content.Shared.Administration.Logs;
using Content.Shared.Chat;
using Content.Shared.Database;
using Content.Shared.Dataset;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Mobs.Systems;
using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.Popups;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.ManageHive;

public sealed class ManageHiveSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private SharedCommendationSystem _commendation;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private DialogSystem _dialog;

	[Dependency]
	private SharedGameTicker _gameTicker;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private HiveBoonSystem _hiveBoon;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private ISharedPlaytimeManager _playtime;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private SharedCMChatSystem _rmcChat;

	[Dependency]
	private SharedXenoWatchSystem _xenoWatch;

	[Dependency]
	private XenoEvolutionSystem _xenoEvolution;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	private static readonly ProtoId<LocalizedDatasetPrototype> JelliesDatasetId = ProtoId<LocalizedDatasetPrototype>.op_Implicit("RMCXenoJellies");

	private LocalizedDatasetPrototype _jelliesDataset;

	private int _jelliesPerQueen;

	private TimeSpan _burrowedLarvaSacrificeTime;

	private int _burrowedLarvaEvolutionPointsPer;

	public override void Initialize()
	{
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).SubscribeLocalEvent<ManageHiveComponent, ManageHiveActionEvent>((EntityEventRefHandler<ManageHiveComponent, ManageHiveActionEvent>)OnManageHiveAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ManageHiveComponent, ManageHiveDevolveEvent>((EntityEventRefHandler<ManageHiveComponent, ManageHiveDevolveEvent>)OnManageHiveDevolve, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ManageHiveComponent, ManageHiveJellyEvent>((EntityEventRefHandler<ManageHiveComponent, ManageHiveJellyEvent>)OnManageHiveJelly, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ManageHiveComponent, ManageHiveSacrificeBurrowedEvent>((EntityEventRefHandler<ManageHiveComponent, ManageHiveSacrificeBurrowedEvent>)OnSacrificeBurrowed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ManageHiveComponent, ManageHiveSacrificeBurrowedTargetEvent>((EntityEventRefHandler<ManageHiveComponent, ManageHiveSacrificeBurrowedTargetEvent>)OnSacrificeBurrowedTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ManageHiveComponent, ManageHiveActivateBoonsEvent>((EntityEventRefHandler<ManageHiveComponent, ManageHiveActivateBoonsEvent>)OnPurchaseBoons, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ManageHiveComponent, ManageHiveActivateBoonsChosenEvent>((EntityEventRefHandler<ManageHiveComponent, ManageHiveActivateBoonsChosenEvent>)OnPurchaseBoonsChosen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ManageHiveComponent, ManageHiveJellyXenoEvent>((EntityEventRefHandler<ManageHiveComponent, ManageHiveJellyXenoEvent>)OnManageHiveJellyXeno, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ManageHiveComponent, ManageHiveJellyNameEvent>((EntityEventRefHandler<ManageHiveComponent, ManageHiveJellyNameEvent>)OnManageHiveJellyType, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ManageHiveComponent, ManageHiveJellyMessageEvent>((EntityEventRefHandler<ManageHiveComponent, ManageHiveJellyMessageEvent>)OnManageHiveJellyMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ManageHiveComponent, ManageHiveDevolveConfirmEvent>((EntityEventRefHandler<ManageHiveComponent, ManageHiveDevolveConfirmEvent>)OnManageHiveDevolveConfirm, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ManageHiveComponent, ManageHiveDevolveMessageEvent>((EntityEventRefHandler<ManageHiveComponent, ManageHiveDevolveMessageEvent>)OnManageHiveDevolveMessage, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCJelliesPerQueen, (Action<int>)delegate(int v)
		{
			_jelliesPerQueen = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCBurrowedLarvaSacrificeTimeMinutes, (Action<int>)delegate(int v)
		{
			_burrowedLarvaSacrificeTime = TimeSpan.FromMinutes(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCBurrowedLarvaEvolutionPointsPer, (Action<int>)delegate(int v)
		{
			_burrowedLarvaEvolutionPointsPer = v;
		}, true);
		_jelliesDataset = _prototype.Index<LocalizedDatasetPrototype>(JelliesDatasetId);
	}

	private void OnManageHiveAction(Entity<ManageHiveComponent> manage, ref ManageHiveActionEvent args)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		List<DialogOption> options = new List<DialogOption>
		{
			new DialogOption(base.Loc.GetString("rmc-hivemanagement-deevolve"), default(ManageHiveDevolveEvent))
		};
		CommendationGiverComponent giver = default(CommendationGiverComponent);
		if (((EntitySystem)this).TryComp<CommendationGiverComponent>(Entity<ManageHiveComponent>.op_Implicit(manage), ref giver) && giver.Given < _jelliesPerQueen)
		{
			options.Add(new DialogOption(base.Loc.GetString("rmc-hivemanagement-reward"), new ManageHiveJellyEvent()));
		}
		options.Add(new DialogOption(base.Loc.GetString("rmc-hivemanagement-exchange-larva"), new ManageHiveSacrificeBurrowedEvent()));
		options.Add(new DialogOption(base.Loc.GetString("rmc-boon-activate"), new ManageHiveActivateBoonsEvent()));
		_dialog.OpenOptions(Entity<ManageHiveComponent>.op_Implicit(manage), base.Loc.GetString("rmc-hivemanagement-hive-management"), options, base.Loc.GetString("rmc-hivemanagement-manage-the-hive"));
	}

	private void OnManageHiveDevolve(Entity<ManageHiveComponent> manage, ref ManageHiveDevolveEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || !CanDevolveTargetPopup(manage, out Entity<XenoDevolveComponent> watched))
		{
			return;
		}
		EntProtoId[] devolutions = watched.Comp.DevolvesTo;
		if (devolutions.Length == 1)
		{
			string name = ((EntitySystem)this).Name(Entity<XenoDevolveComponent>.op_Implicit(watched), (MetaDataComponent)null);
			string protoName = null;
			EntityPrototype obj = ((EntitySystem)this).Prototype(Entity<XenoDevolveComponent>.op_Implicit(watched), (MetaDataComponent)null);
			string n = ((obj != null) ? obj.Name : null);
			if (n != null)
			{
				protoName = n;
			}
			bool hasFrom = protoName != null;
			EntityPrototype devolveProto = default(EntityPrototype);
			bool hasTo = _prototype.TryIndex(devolutions[0], ref devolveProto);
			string msg = ((hasFrom && hasTo) ? base.Loc.GetString("rmc-hivemanagement-are-you-sure-deevolve-from-to", new(string, object)[3]
			{
				("name", name),
				("from", protoName ?? ""),
				("to", ((devolveProto != null) ? devolveProto.Name : null) ?? "")
			}) : ((!hasFrom) ? base.Loc.GetString("rmc-hivemanagement-are-you-sure-deevolve", (ValueTuple<string, object>)("name", name)) : base.Loc.GetString("rmc-hivemanagement-are-you-sure-deevolve-from", (ValueTuple<string, object>)("name", name), (ValueTuple<string, object>)("from", protoName ?? ""))));
			_dialog.OpenConfirmation(Entity<ManageHiveComponent>.op_Implicit(manage), base.Loc.GetString("rmc-hivemanagement-deevolution"), msg, new ManageHiveDevolveConfirmEvent(devolutions[0]));
			return;
		}
		List<DialogOption> choices = new List<DialogOption>();
		EntProtoId[] array = devolutions;
		EntityPrototype choiceProto = default(EntityPrototype);
		for (int i = 0; i < array.Length; i++)
		{
			EntProtoId choice = array[i];
			string name2 = ((EntProtoId)(ref choice)).Id;
			if (_prototype.TryIndex(choice, ref choiceProto))
			{
				name2 = choiceProto.Name;
			}
			choices.Add(new DialogOption(name2, new ManageHiveDevolveConfirmEvent(choice)));
		}
		_dialog.OpenOptions(Entity<ManageHiveComponent>.op_Implicit(manage), base.Loc.GetString("rmc-hivemanagement-choose-caste"), choices);
	}

	private void OnManageHiveJelly(Entity<ManageHiveComponent> ent, ref ManageHiveJellyEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		CommendationGiverComponent giver = default(CommendationGiverComponent);
		ActorComponent giverActor = default(ActorComponent);
		if (_net.IsClient || !((EntitySystem)this).TryComp<CommendationGiverComponent>(Entity<ManageHiveComponent>.op_Implicit(ent), ref giver) || !((EntitySystem)this).TryComp<ActorComponent>(Entity<ManageHiveComponent>.op_Implicit(ent), ref giverActor))
		{
			return;
		}
		try
		{
			if (!_playtime.GetPlayTimes(giverActor.PlayerSession).TryGetValue(ProtoId<PlayTimeTrackerPrototype>.op_Implicit(ent.Comp.PlayTime), out var time) || time < ent.Comp.JellyRequiredTime)
			{
				_popup.PopupCursor(base.Loc.GetString("rmc-jelly-error-not-enough-playtime", (ValueTuple<string, object>)("requiredHours", (int)ent.Comp.JellyRequiredTime.TotalHours)), Entity<ManageHiveComponent>.op_Implicit(ent), PopupType.LargeCaution);
				return;
			}
		}
		catch
		{
		}
		if (!_xenoPlasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(ent.Owner), ent.Comp.JellyPlasmaCost, predicted: false))
		{
			return;
		}
		List<DialogOption> choices = new List<DialogOption>();
		HiveMemberComponent manageMemberComp = ((EntitySystem)this).CompOrNull<HiveMemberComponent>(Entity<ManageHiveComponent>.op_Implicit(ent));
		Entity<ManageHiveComponent, CommendationGiverComponent, HiveMemberComponent, ActorComponent> manageMember = default(Entity<ManageHiveComponent, CommendationGiverComponent, HiveMemberComponent, ActorComponent>);
		manageMember._002Ector(Entity<ManageHiveComponent>.op_Implicit(ent), Entity<ManageHiveComponent>.op_Implicit(ent), giver, manageMemberComp, giverActor);
		EntityQueryEnumerator<CommendationReceiverComponent, HiveMemberComponent> receivers = ((EntitySystem)this).EntityQueryEnumerator<CommendationReceiverComponent, HiveMemberComponent>();
		EntityUid uid = default(EntityUid);
		CommendationReceiverComponent commendationReceiverComponent = default(CommendationReceiverComponent);
		HiveMemberComponent member = default(HiveMemberComponent);
		while (receivers.MoveNext(ref uid, ref commendationReceiverComponent, ref member))
		{
			if (CanAwardJellyPopup(manageMember, Entity<HiveMemberComponent>.op_Implicit((uid, member)), popup: false))
			{
				choices.Add(new DialogOption(((EntitySystem)this).Name(uid, (MetaDataComponent)null), new ManageHiveJellyXenoEvent(((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null))));
			}
		}
		_dialog.OpenOptions(Entity<ManageHiveComponent>.op_Implicit(ent), base.Loc.GetString("rmc-jelly-recipient"), choices, base.Loc.GetString("rmc-jelly-recipient-prompt"));
	}

	private void OnSacrificeBurrowed(Entity<ManageHiveComponent> ent, ref ManageHiveSacrificeBurrowedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || !CanSacrificeBurrowedPopup(ent, out Entity<HiveComponent> _))
		{
			return;
		}
		List<DialogOption> choices = new List<DialogOption>();
		EntityQueryEnumerator<ActorComponent, XenoComponent, XenoEvolutionComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ActorComponent, XenoComponent, XenoEvolutionComponent>();
		EntityUid target = default(EntityUid);
		ActorComponent val = default(ActorComponent);
		XenoComponent xenoComponent = default(XenoComponent);
		XenoEvolutionComponent evolution = default(XenoEvolutionComponent);
		while (query.MoveNext(ref target, ref val, ref xenoComponent, ref evolution))
		{
			if (!(target == ent.Owner) && !_mobState.IsIncapacitated(target))
			{
				FixedPoint2 points = evolution.Points;
				FixedPoint2 max = evolution.Max;
				if (!(evolution.Points >= evolution.Max) && _hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(target)))
				{
					string targetName = $"{((EntitySystem)this).Name(target, (MetaDataComponent)null)} ({points.Int()}/{max.Int()})";
					ManageHiveSacrificeBurrowedTargetEvent ev = new ManageHiveSacrificeBurrowedTargetEvent(((EntitySystem)this).GetNetEntity(target, (MetaDataComponent)null));
					choices.Add(new DialogOption(targetName, ev));
				}
			}
		}
		_dialog.OpenOptions(Entity<ManageHiveComponent>.op_Implicit(ent), base.Loc.GetString("rmc-hivemanagement-exchange-larva-title"), choices, base.Loc.GetString("rmc-hivemanagement-exchange-larva-description", (ValueTuple<string, object>)("points", _burrowedLarvaEvolutionPointsPer)));
	}

	private void OnSacrificeBurrowedTarget(Entity<ManageHiveComponent> ent, ref ManageHiveSacrificeBurrowedTargetEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			EntityUid target = ((EntitySystem)this).GetEntity(args.Target);
			if (((EntityUid)(ref target)).Valid && !(ent.Owner == target) && _hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(target)) && !_mobState.IsIncapacitated(target) && CanSacrificeBurrowedPopup(ent, out Entity<HiveComponent> hive))
			{
				_hive.IncreaseBurrowedLarva(hive, -1);
				FixedPoint2 given = _xenoEvolution.AddPointsCapped(Entity<XenoEvolutionComponent>.op_Implicit(target), _burrowedLarvaEvolutionPointsPer);
				_popup.PopupCursor(base.Loc.GetString("rmc-hivemanagement-exchange-larva-given-user", (ValueTuple<string, object>)("target", ent), (ValueTuple<string, object>)("points", given)), Entity<ManageHiveComponent>.op_Implicit(ent));
				_popup.PopupCursor(base.Loc.GetString("rmc-hivemanagement-exchange-larva-given-target", (ValueTuple<string, object>)("user", ent), (ValueTuple<string, object>)("points", given)), Entity<ManageHiveComponent>.op_Implicit(ent));
			}
		}
	}

	private void OnPurchaseBoons(Entity<ManageHiveComponent> ent, ref ManageHiveActivateBoonsEvent args)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			List<DialogOption> choices = new List<DialogOption>();
			ImmutableArray<(EntityPrototype, HiveBoonDefinitionComponent)>.Enumerator enumerator = _hiveBoon.Boons.GetEnumerator();
			while (enumerator.MoveNext())
			{
				(EntityPrototype, HiveBoonDefinitionComponent) boon = enumerator.Current;
				string text = base.Loc.GetString("rmc-boon-name-cost", new(string, object)[3]
				{
					("boon", boon.Item1.Name),
					("cost", boon.Item2.Cost),
					("pylons", boon.Item2.Pylons)
				});
				ManageHiveActivateBoonsChosenEvent ev = new ManageHiveActivateBoonsChosenEvent(EntProtoId<HiveBoonDefinitionComponent>.op_Implicit(boon.Item1.ID));
				choices.Add(new DialogOption(text, ev));
			}
			int resin = 0;
			Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner));
			if (hive.HasValue)
			{
				Entity<HiveComponent> hive2 = hive.GetValueOrDefault();
				resin = _hiveBoon.EnsureBoons(hive2).Comp.RoyalResin;
			}
			_dialog.OpenOptions(Entity<ManageHiveComponent>.op_Implicit(ent), base.Loc.GetString("rmc-boon-activate"), choices, base.Loc.GetString("rmc-boon-message", (ValueTuple<string, object>)("current", resin)));
		}
	}

	private void OnPurchaseBoonsChosen(Entity<ManageHiveComponent> ent, ref ManageHiveActivateBoonsChosenEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			_hiveBoon.TryActivateBoon(ent, args.Boon);
		}
	}

	private void OnManageHiveJellyXeno(Entity<ManageHiveComponent> ent, ref ManageHiveJellyXenoEvent args)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		List<DialogOption> options = new List<DialogOption>();
		foreach (string name in _jelliesDataset.Values)
		{
			options.Add(new DialogOption(base.Loc.GetString(name), new ManageHiveJellyNameEvent(args.Xeno, base.Loc.GetString(name))));
		}
		_dialog.OpenOptions(Entity<ManageHiveComponent>.op_Implicit(ent), base.Loc.GetString("rmc-jelly-type"), options, base.Loc.GetString("rmc-jelly-type-prompt"));
	}

	private void OnManageHiveJellyType(Entity<ManageHiveComponent> ent, ref ManageHiveJellyNameEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			ManageHiveJellyMessageEvent ev = new ManageHiveJellyMessageEvent(args.Xeno, args.Name);
			_dialog.OpenInput(Entity<ManageHiveComponent>.op_Implicit(ent), base.Loc.GetString("rmc-jelly-citation-prompt"), ev, largeInput: true, _commendation.CharacterLimit);
		}
	}

	private void OnManageHiveJellyMessage(Entity<ManageHiveComponent> ent, ref ManageHiveJellyMessageEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? xeno = default(EntityUid?);
		if (!_net.IsClient && ((EntitySystem)this).TryGetEntity(args.Xeno, ref xeno) && CanAwardJellyPopup(Entity<ManageHiveComponent, CommendationGiverComponent, HiveMemberComponent, ActorComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(xeno.Value)) && _commendation.ValidCommendation(Entity<CommendationGiverComponent, ActorComponent>.op_Implicit(ent.Owner), Entity<CommendationReceiverComponent>.op_Implicit(xeno.Value), args.Message) && _xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(ent.Owner), ent.Comp.JellyPlasmaCost))
		{
			_commendation.GiveCommendation(Entity<CommendationGiverComponent, ActorComponent>.op_Implicit(ent.Owner), Entity<CommendationReceiverComponent>.op_Implicit(xeno.Value), base.Loc.GetString(args.Name), args.Message, CommendationType.Jelly);
			_popup.PopupCursor(base.Loc.GetString("rmc-jelly-awarded"), Entity<ManageHiveComponent>.op_Implicit(ent), PopupType.Large);
		}
	}

	private void OnManageHiveDevolveConfirm(Entity<ManageHiveComponent> manage, ref ManageHiveDevolveConfirmEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && CanDevolveTargetPopup(manage, out Entity<XenoDevolveComponent> watched))
		{
			EntProtoId[] devolvesTo = watched.Comp.DevolvesTo;
			EntProtoId choice = args.Choice;
			if (Enumerable.Contains(devolvesTo, EntProtoId.op_Implicit(((EntProtoId)(ref choice)).Id)))
			{
				_dialog.OpenInput(Entity<ManageHiveComponent>.op_Implicit(manage), base.Loc.GetString("rmc-hivemanagement-provide-reason", (ValueTuple<string, object>)("name", ((EntitySystem)this).Name(Entity<XenoDevolveComponent>.op_Implicit(watched), (MetaDataComponent)null))), new ManageHiveDevolveMessageEvent(args.Choice));
			}
		}
	}

	private void OnManageHiveDevolveMessage(Entity<ManageHiveComponent> manage, ref ManageHiveDevolveMessageEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || !CanDevolveTargetPopup(manage, out Entity<XenoDevolveComponent> watched) || !Enumerable.Contains(watched.Comp.DevolvesTo, args.Choice) || !_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(manage.Owner), manage.Comp.DevolvePlasmaCost))
		{
			return;
		}
		EntityStringRepresentation? oldString = ((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoDevolveComponent>.op_Implicit(watched), (MetaDataComponent)null);
		EntityUid? val = _xenoEvolution.Devolve(watched, args.Choice);
		if (val.HasValue)
		{
			EntityUid devolution = val.GetValueOrDefault();
			ActorComponent watchedActor = default(ActorComponent);
			if (((EntitySystem)this).TryComp<ActorComponent>(devolution, ref watchedActor))
			{
				string msg = base.Loc.GetString("rmc-hivemanagement-queen-deevolving", (ValueTuple<string, object>)("reason", args.Message));
				_rmcChat.ChatMessageToOne(ChatChannel.Local, msg, msg, default(EntityUid), hideChat: false, watchedActor.PlayerSession.Channel);
				_popup.PopupEntity(msg, devolution, PopupType.LargeCaution);
			}
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(14, 3);
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ManageHiveComponent>.op_Implicit(manage), (MetaDataComponent)null), "ToPrettyString(manage)");
			handler.AppendLiteral(" devolved ");
			handler.AppendFormatted(oldString, "oldString");
			handler.AppendLiteral(" to ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(devolution)), "ToPrettyString(devolution)");
			adminLog.Add(LogType.RMCDevolve, ref handler);
		}
	}

	private bool CanDevolveTargetPopup(Entity<ManageHiveComponent> manage, out Entity<XenoDevolveComponent> watched)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		watched = default(Entity<XenoDevolveComponent>);
		if (!_xenoWatch.TryGetWatched(Entity<XenoWatchingComponent>.op_Implicit(manage.Owner), out var watchedId) || watchedId == manage.Owner)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-hivemanagement-must-overwatch"), Entity<ManageHiveComponent>.op_Implicit(manage), Entity<ManageHiveComponent>.op_Implicit(manage), PopupType.MediumCaution);
			return false;
		}
		XenoDevolveComponent devolve = default(XenoDevolveComponent);
		if (!((EntitySystem)this).TryComp<XenoDevolveComponent>(watchedId, ref devolve) || devolve.DevolvesTo.Length == 0)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-hivemanagement-cant-be-devolved", (ValueTuple<string, object>)("name", ((EntitySystem)this).Name(watchedId, (MetaDataComponent)null))), watchedId, Entity<ManageHiveComponent>.op_Implicit(manage), PopupType.MediumCaution);
			return false;
		}
		if (!devolve.CanBeDevolvedByOther)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-hivemanagement-cant-deevolve-larva"), watchedId, Entity<ManageHiveComponent>.op_Implicit(manage), PopupType.MediumCaution);
			return false;
		}
		if (!_xenoPlasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(manage.Owner), manage.Comp.DevolvePlasmaCost, predicted: false))
		{
			return false;
		}
		if (!_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(manage.Owner), Entity<HiveMemberComponent>.op_Implicit(watchedId)))
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-hivemanagement-cant-deevolve-other-hive"), watchedId, Entity<ManageHiveComponent>.op_Implicit(manage), PopupType.MediumCaution);
			return false;
		}
		watched = Entity<XenoDevolveComponent>.op_Implicit((watchedId, devolve));
		return true;
	}

	private bool CanAwardJellyPopup(Entity<ManageHiveComponent?, CommendationGiverComponent?, HiveMemberComponent?, ActorComponent?> manage, Entity<HiveMemberComponent?> target, bool popup = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ManageHiveComponent, CommendationGiverComponent, HiveMemberComponent, ActorComponent>(Entity<ManageHiveComponent, CommendationGiverComponent, HiveMemberComponent, ActorComponent>.op_Implicit(manage), ref manage.Comp1, ref manage.Comp2, ref manage.Comp3, ref manage.Comp4, false))
		{
			return false;
		}
		CommendationReceiverComponent receiver = default(CommendationReceiverComponent);
		if (!((EntitySystem)this).Resolve<HiveMemberComponent>(Entity<HiveMemberComponent>.op_Implicit(target), ref target.Comp, false) || !_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(manage.Owner), target) || !((EntitySystem)this).TryComp<CommendationReceiverComponent>(Entity<HiveMemberComponent>.op_Implicit(target), ref receiver) || receiver.LastPlayerId == null || manage.Owner == target.Owner || Guid.Parse(receiver.LastPlayerId) == NetUserId.op_Implicit(manage.Comp4.PlayerSession.UserId))
		{
			if (popup)
			{
				_popup.PopupCursor(base.Loc.GetString("rmc-jelly-error-cant-give"), Entity<ManageHiveComponent, CommendationGiverComponent, HiveMemberComponent, ActorComponent>.op_Implicit(manage), PopupType.MediumCaution);
			}
			return false;
		}
		if (manage.Comp2.Given >= _jelliesPerQueen)
		{
			if (popup)
			{
				_popup.PopupCursor(base.Loc.GetString("rmc-jelly-error-limit-reached", (ValueTuple<string, object>)("given", manage.Comp2.Given), (ValueTuple<string, object>)("limit", _jelliesPerQueen)), Entity<ManageHiveComponent, CommendationGiverComponent, HiveMemberComponent, ActorComponent>.op_Implicit(manage), PopupType.MediumCaution);
			}
			return false;
		}
		return true;
	}

	private bool CanSacrificeBurrowedPopup(Entity<ManageHiveComponent> user, out Entity<HiveComponent> hive)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		hive = default(Entity<HiveComponent>);
		Entity<HiveComponent>? hive2 = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(user.Owner));
		if (hive2.HasValue)
		{
			Entity<HiveComponent> userHive = hive2.GetValueOrDefault();
			hive = userHive;
			if (hive.Comp.BurrowedLarva <= 0)
			{
				_popup.PopupCursor(base.Loc.GetString("rmc-hivemanagement-exchange-larva-not-enough"), Entity<ManageHiveComponent>.op_Implicit(user), PopupType.MediumCaution);
				return false;
			}
			TimeSpan time = _burrowedLarvaSacrificeTime - _gameTicker.RoundDuration();
			if (time > TimeSpan.Zero)
			{
				string msg = base.Loc.GetString("rmc-hivemanagement-exchange-larva-need-minutes", (ValueTuple<string, object>)("minutes", (int)time.TotalMinutes));
				_popup.PopupCursor(msg, Entity<ManageHiveComponent>.op_Implicit(user), PopupType.MediumCaution);
				return false;
			}
			if (!_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(user.Owner), user.Comp.SacrificeBurrowedLarvaForEvolutionCost, predicted: false))
			{
				return false;
			}
			return true;
		}
		return false;
	}
}
