using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Administration.Logs;
using Content.Shared.Buckle.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Database;
using Content.Shared.Effects;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Tackle;

public sealed class TackleSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private RMCHandsSystem _rmcHands;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedGunSystem _gunSystem;

	private readonly List<EntityUid> _trackersToRemove = new List<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<TackleableComponent, CMDisarmEvent>((EntityEventRefHandler<TackleableComponent, CMDisarmEvent>)OnDisarmed, new Type[2]
		{
			typeof(SharedHandsSystem),
			typeof(SharedStaminaSystem)
		}, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDisarmableComponent, CMDisarmEvent>((EntityEventRefHandler<RMCDisarmableComponent, CMDisarmEvent>)OnDisarmed, new Type[2]
		{
			typeof(SharedHandsSystem),
			typeof(SharedStaminaSystem)
		}, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TackledRecentlyByComponent, ComponentRemove>((EntityEventRefHandler<TackledRecentlyByComponent, ComponentRemove>)OnByRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TackledRecentlyByComponent, EntityTerminatingEvent>((EntityEventRefHandler<TackledRecentlyByComponent, EntityTerminatingEvent>)OnByRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TackledRecentlyByComponent, DownedEvent>((EntityEventRefHandler<TackledRecentlyByComponent, DownedEvent>)OnDowned, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TackledRecentlyComponent, ComponentRemove>((EntityEventRefHandler<TackledRecentlyComponent, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TackledRecentlyComponent, EntityTerminatingEvent>((EntityEventRefHandler<TackledRecentlyComponent, EntityTerminatingEvent>)OnRemove, (Type[])null, (Type[])null);
	}

	private void OnDisarmed(Entity<TackleableComponent> target, ref CMDisarmEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		if (args.Handled)
		{
			return;
		}
		EntityUid user = args.User;
		TackleComponent tackle = default(TackleComponent);
		if (!((EntitySystem)this).TryComp<TackleComponent>(user, ref tackle))
		{
			return;
		}
		args.Handled = true;
		DoDisarmEffects(user, Entity<TackleableComponent>.op_Implicit(target));
		TimeSpan time = _timing.CurTime;
		TackledRecentlyComponent recently = ((EntitySystem)this).EnsureComp<TackledRecentlyComponent>(user);
		TackleTracker tracker = recently.Trackers.GetValueOrDefault(Entity<TackleableComponent>.op_Implicit(target));
		tracker.Count++;
		tracker.Last = time;
		recently.Trackers[Entity<TackleableComponent>.op_Implicit(target)] = tracker;
		((EntitySystem)this).Dirty(user, (IComponent)(object)recently, (MetaDataComponent)null);
		TackledRecentlyByComponent tackledBy = ((EntitySystem)this).EnsureComp<TackledRecentlyByComponent>(Entity<TackleableComponent>.op_Implicit(target));
		tackledBy.Tacklers.Add(user);
		((EntitySystem)this).Dirty(Entity<TackleableComponent>.op_Implicit(target), (IComponent)(object)tackledBy, (MetaDataComponent)null);
		if (_net.IsClient)
		{
			return;
		}
		float random = _random.NextFloat(0f, 1f);
		if ((tracker.Count < tackle.Min || tackle.Chance < random) && tracker.Count < tackle.Max)
		{
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(18, 2);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
			handler.AppendLiteral(" tried to tackle ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<TackleableComponent>.op_Implicit(target), (MetaDataComponent)null), "ToPrettyString(target)");
			handler.AppendLiteral(".");
			adminLog.Add(LogType.RMCTackle, ref handler);
			string selfPopup = base.Loc.GetString("cm-tackle-try-self", (ValueTuple<string, object>)("target", Identity.Name(Entity<TackleableComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager, user)));
			string targetPopup = base.Loc.GetString("cm-tackle-try-target", (ValueTuple<string, object>)("user", Identity.Name(user, (IEntityManager)(object)base.EntityManager, Entity<TackleableComponent>.op_Implicit(target))));
			DoPvsPopups(user, Entity<TackleableComponent>.op_Implicit(target), selfPopup, targetPopup, (EntityUid other) => base.Loc.GetString("cm-tackle-try-observer", (ValueTuple<string, object>)("user", Identity.Name(user, (IEntityManager)(object)base.EntityManager, other)), (ValueTuple<string, object>)("target", Identity.Name(Entity<TackleableComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager, other))));
			return;
		}
		ISharedAdminLogManager adminLog2 = _adminLog;
		LogStringHandler handler2 = new LogStringHandler(15, 2);
		handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
		handler2.AppendLiteral(" tackled down ");
		handler2.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<TackleableComponent>.op_Implicit(target), (MetaDataComponent)null), "ToPrettyString(target)");
		handler2.AppendLiteral(".");
		adminLog2.Add(LogType.RMCTackle, ref handler2);
		string selfPopup2 = base.Loc.GetString("cm-tackle-success-self", (ValueTuple<string, object>)("target", Identity.Name(Entity<TackleableComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager, user)));
		string targetPopup2 = base.Loc.GetString("cm-tackle-success-target", (ValueTuple<string, object>)("user", Identity.Name(user, (IEntityManager)(object)base.EntityManager, Entity<TackleableComponent>.op_Implicit(target))));
		DoPvsPopups(user, Entity<TackleableComponent>.op_Implicit(target), selfPopup2, targetPopup2, (EntityUid other) => base.Loc.GetString("cm-tackle-success-observer", (ValueTuple<string, object>)("user", Identity.Name(user, (IEntityManager)(object)base.EntityManager, other)), (ValueTuple<string, object>)("target", Identity.Name(Entity<TackleableComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager, other))));
		_audio.PlayPvs(target.Comp.KnockdownSound, Entity<TackleableComponent>.op_Implicit(target), (AudioParams?)null);
		if (!((EntitySystem)this).HasComp<VictimInfectedComponent>(Entity<TackleableComponent>.op_Implicit(target)))
		{
			recently.Trackers.Remove(Entity<TackleableComponent>.op_Implicit(target));
			RemoveTackledBy(Entity<TackledRecentlyByComponent>.op_Implicit(target.Owner), user);
		}
		TimeSpan stun = tackle.StunMin;
		if (tackle.StunMin < tackle.StunMax)
		{
			stun = _random.Next(tackle.StunMin, tackle.StunMax);
		}
		stun *= 2.0;
		_stun.TryParalyze(Entity<TackleableComponent>.op_Implicit(target), stun, refresh: true);
	}

	private void OnDisarmed(Entity<RMCDisarmableComponent> target, ref CMDisarmEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0915: Unknown result type (might be due to invalid IL or missing references)
		//IL_091a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_095c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0961: Unknown result type (might be due to invalid IL or missing references)
		//IL_096d: Unknown result type (might be due to invalid IL or missing references)
		//IL_099e: Unknown result type (might be due to invalid IL or missing references)
		//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_09af: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_080a: Unknown result type (might be due to invalid IL or missing references)
		//IL_080f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0828: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_083c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0841: Unknown result type (might be due to invalid IL or missing references)
		//IL_0846: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0794: Unknown result type (might be due to invalid IL or missing references)
		//IL_0799: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0871: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		if (args.Handled)
		{
			return;
		}
		EntityUid user = args.User;
		RMCDisarmComponent disarm = default(RMCDisarmComponent);
		if (!((EntitySystem)this).TryComp<RMCDisarmComponent>(user, ref disarm))
		{
			return;
		}
		args.Handled = true;
		DoDisarmEffects(user, Entity<RMCDisarmableComponent>.op_Implicit(target));
		if (_net.IsClient)
		{
			return;
		}
		bool doPopups = true;
		if (!_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(user), disarm.Skill, disarm.AccidentalDischargeSkillAmount))
		{
			bool fired = false;
			GunComponent gun = default(GunComponent);
			foreach (EntityUid item in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(target.Owner)))
			{
				if (fired)
				{
					break;
				}
				if (!((EntitySystem)this).TryComp<GunComponent>(item, ref gun) || !RandomExtensions.Prob(_random, disarm.AccidentalDischargeChance))
				{
					continue;
				}
				EntityCoordinates coords = _transform.GetMoverCoordinates(user);
				List<EntityUid>? list = _gunSystem.AttemptShoot(Entity<GunComponent>.op_Implicit((item, gun)), Entity<RMCDisarmableComponent>.op_Implicit(target), coords);
				GetAmmoCountEvent ammoCount = default(GetAmmoCountEvent);
				((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(item, ref ammoCount, false);
				if (list != null && ammoCount.Count > 0)
				{
					fired = true;
					doPopups = false;
					string selfMsg = base.Loc.GetString("rmc-disarm-discharge-self", (ValueTuple<string, object>)("targetName", Identity.Name(Entity<RMCDisarmableComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager, user)), (ValueTuple<string, object>)("gun", item));
					string targetMsg = base.Loc.GetString("rmc-disarm-discharge-target", (ValueTuple<string, object>)("performerName", Identity.Name(user, (IEntityManager)(object)base.EntityManager, Entity<RMCDisarmableComponent>.op_Implicit(target))), (ValueTuple<string, object>)("gun", item));
					DoPvsPopups(user, Entity<RMCDisarmableComponent>.op_Implicit(target), selfMsg, targetMsg, (EntityUid other) => base.Loc.GetString("rmc-disarm-discharge-others", new(string, object)[3]
					{
						("performerName", Identity.Name(user, (IEntityManager)(object)base.EntityManager, other)),
						("targetName", Identity.Name(Entity<RMCDisarmableComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager, other)),
						("gun", item)
					}), PopupType.MediumCaution);
					UpdateClientAmmoEvent ev = default(UpdateClientAmmoEvent);
					((EntitySystem)this).RaiseLocalEvent<UpdateClientAmmoEvent>(item, ref ev, false);
				}
			}
			if (fired)
			{
				ISharedAdminLogManager adminLog = _adminLog;
				LogStringHandler handler = new LogStringHandler(60, 2);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
				handler.AppendLiteral(" accidentally discharged ");
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<RMCDisarmableComponent>.op_Implicit(target), (MetaDataComponent)null), "ToPrettyString(target)");
				handler.AppendLiteral("'s gun while trying to disarm them.");
				adminLog.Add(LogType.RMCTackle, ref handler);
			}
		}
		float disarmChance = _random.NextFloat(1f, 100f);
		int attackerSkill = _skills.GetSkill(Entity<SkillsComponent>.op_Implicit(user), disarm.Skill);
		int defenderSkill = _skills.GetSkill(Entity<SkillsComponent>.op_Implicit(target.Owner), disarm.Skill);
		disarmChance -= (float)(5 * attackerSkill);
		disarmChance += (float)(5 * defenderSkill);
		if (disarmChance <= 25f)
		{
			string shoveText = base.Loc.GetString((attackerSkill > 1) ? "rmc-disarm-text-skilled" : LocId.op_Implicit(RandomExtensions.Pick<LocId>(_random, (IReadOnlyList<LocId>)disarm.RandomShoveTexts)));
			if (doPopups)
			{
				string selfMsg2 = base.Loc.GetString("rmc-disarm-shove-self", (ValueTuple<string, object>)("targetName", Identity.Name(Entity<RMCDisarmableComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager, user)), (ValueTuple<string, object>)("shoveText", shoveText));
				string targetMsg2 = base.Loc.GetString("rmc-disarm-shove-target", (ValueTuple<string, object>)("performerName", Identity.Name(user, (IEntityManager)(object)base.EntityManager, Entity<RMCDisarmableComponent>.op_Implicit(target))), (ValueTuple<string, object>)("shoveText", shoveText));
				DoPvsPopups(user, Entity<RMCDisarmableComponent>.op_Implicit(target), selfMsg2, targetMsg2, (EntityUid other) => base.Loc.GetString("rmc-disarm-shove-others", new(string, object)[3]
				{
					("performerName", Identity.Name(user, (IEntityManager)(object)base.EntityManager, other)),
					("targetName", Identity.Name(Entity<RMCDisarmableComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager, other)),
					("shoveText", shoveText)
				}));
			}
			TimeSpan strength = disarm.BaseStunTime + TimeSpan.FromSeconds(Math.Max(attackerSkill - defenderSkill, 0));
			_stun.TryParalyze(Entity<RMCDisarmableComponent>.op_Implicit(target), strength, refresh: true);
			ISharedAdminLogManager adminLog2 = _adminLog;
			LogStringHandler handler2 = new LogStringHandler(26, 2);
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
			handler2.AppendLiteral(" disarmed ");
			handler2.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<RMCDisarmableComponent>.op_Implicit(target), (MetaDataComponent)null), "ToPrettyString(target)");
			handler2.AppendLiteral(", stunning them.");
			adminLog2.Add(LogType.RMCTackle, ref handler2);
			return;
		}
		if (disarmChance <= 60f)
		{
			PullerComponent puller = default(PullerComponent);
			if (((EntitySystem)this).TryComp<PullerComponent>(Entity<RMCDisarmableComponent>.op_Implicit(target), ref puller))
			{
				EntityUid? pulling = puller.Pulling;
				if (pulling.HasValue)
				{
					EntityUid pulledObject = pulling.GetValueOrDefault();
					if (doPopups)
					{
						string selfMsg3 = base.Loc.GetString("rmc-disarm-break-pulls-self", (ValueTuple<string, object>)("targetName", Identity.Name(Entity<RMCDisarmableComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager, user)), (ValueTuple<string, object>)("object", pulledObject));
						string targetMsg3 = base.Loc.GetString("rmc-disarm-break-pulls-target", (ValueTuple<string, object>)("performerName", Identity.Name(user, (IEntityManager)(object)base.EntityManager, Entity<RMCDisarmableComponent>.op_Implicit(target))), (ValueTuple<string, object>)("object", pulledObject));
						DoPvsPopups(user, Entity<RMCDisarmableComponent>.op_Implicit(target), selfMsg3, targetMsg3, (EntityUid other) => base.Loc.GetString("rmc-disarm-break-pulls-others", new(string, object)[3]
						{
							("performerName", Identity.Name(user, (IEntityManager)(object)base.EntityManager, other)),
							("targetName", Identity.Name(Entity<RMCDisarmableComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager, other)),
							("object", Identity.Name(pulledObject, (IEntityManager)(object)base.EntityManager, other))
						}));
					}
					_rmcPulling.TryStopAllPullsFromAndOn(Entity<RMCDisarmableComponent>.op_Implicit(target));
					goto IL_0852;
				}
			}
			if (doPopups)
			{
				string selfMsg4 = base.Loc.GetString("rmc-disarm-success-self", (ValueTuple<string, object>)("targetName", Identity.Name(Entity<RMCDisarmableComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager, user)));
				string targetMsg4 = base.Loc.GetString("rmc-disarm-success-target", (ValueTuple<string, object>)("performerName", Identity.Name(user, (IEntityManager)(object)base.EntityManager, Entity<RMCDisarmableComponent>.op_Implicit(target))));
				DoPvsPopups(user, Entity<RMCDisarmableComponent>.op_Implicit(target), selfMsg4, targetMsg4, (EntityUid other) => base.Loc.GetString("rmc-disarm-success-others", (ValueTuple<string, object>)("performerName", Identity.Name(user, (IEntityManager)(object)base.EntityManager, other)), (ValueTuple<string, object>)("targetName", Identity.Name(Entity<RMCDisarmableComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager, other))));
			}
			EntityCoordinates moverCoordinates = _transform.GetMoverCoordinates(Entity<RMCDisarmableComponent>.op_Implicit(target));
			EntityCoordinates offset = ((EntityCoordinates)(ref moverCoordinates)).Offset(_random.NextVector2(1f, 1.5f));
			_rmcHands.ThrowHeldItem(Entity<RMCDisarmableComponent>.op_Implicit(target), offset);
			goto IL_0852;
		}
		ISharedAdminLogManager adminLog3 = _adminLog;
		LogStringHandler handler3 = new LogStringHandler(18, 2);
		handler3.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
		handler3.AppendLiteral(" tried to disarm ");
		handler3.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<RMCDisarmableComponent>.op_Implicit(target), (MetaDataComponent)null), "ToPrettyString(target)");
		handler3.AppendLiteral(".");
		adminLog3.Add(LogType.RMCTackle, ref handler3);
		if (doPopups)
		{
			string selfPopup = base.Loc.GetString("rmc-disarm-attempt-self", (ValueTuple<string, object>)("targetName", Identity.Name(Entity<RMCDisarmableComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager, user)));
			string targetPopup = base.Loc.GetString("rmc-disarm-attempt-target", (ValueTuple<string, object>)("performerName", Identity.Name(user, (IEntityManager)(object)base.EntityManager, Entity<RMCDisarmableComponent>.op_Implicit(target))));
			DoPvsPopups(user, Entity<RMCDisarmableComponent>.op_Implicit(target), selfPopup, targetPopup, (EntityUid other) => base.Loc.GetString("rmc-disarm-attempt-others", (ValueTuple<string, object>)("performerName", Identity.Name(other, (IEntityManager)(object)base.EntityManager, user)), (ValueTuple<string, object>)("targetName", Identity.Name(Entity<RMCDisarmableComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager, other))));
		}
		return;
		IL_0852:
		ISharedAdminLogManager adminLog4 = _adminLog;
		LogStringHandler handler4 = new LogStringHandler(11, 2);
		handler4.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
		handler4.AppendLiteral(" disarmed ");
		handler4.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<RMCDisarmableComponent>.op_Implicit(target), (MetaDataComponent)null), "ToPrettyString(target)");
		handler4.AppendLiteral(".");
		adminLog4.Add(LogType.RMCTackle, ref handler4);
	}

	private void DoDisarmEffects(EntityUid user, EntityUid target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_colorFlash.RaiseEffect(Color.Aqua, new List<EntityUid> { target }, Filter.PvsExcept(user, 2f, (IEntityManager)null));
	}

	private void DoPvsPopups(EntityUid user, EntityUid target, string selfPopup, string targetPopup, Func<EntityUid, string> othersPopup, PopupType selfPopupType = PopupType.Small)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupEntity(selfPopup, user, user, selfPopupType);
		foreach (ICommonSession recipient2 in Filter.PvsExcept(user, 2f, (IEntityManager)null).Recipients)
		{
			EntityUid? attachedEntity = recipient2.AttachedEntity;
			if (attachedEntity.HasValue)
			{
				EntityUid recipient = attachedEntity.GetValueOrDefault();
				if (recipient == target)
				{
					_popup.PopupEntity(targetPopup, user, recipient, PopupType.MediumCaution);
				}
				else
				{
					_popup.PopupEntity(othersPopup(recipient), user, recipient, PopupType.SmallCaution);
				}
			}
		}
	}

	private void OnByRemove<T>(Entity<TackledRecentlyByComponent> ent, ref T args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		TackledRecentlyComponent recently = default(TackledRecentlyComponent);
		foreach (EntityUid tackler in ent.Comp.Tacklers)
		{
			if (((EntitySystem)this).TryComp<TackledRecentlyComponent>(tackler, ref recently))
			{
				recently.Trackers.Remove(Entity<TackledRecentlyByComponent>.op_Implicit(ent));
				((EntitySystem)this).Dirty(tackler, (IComponent)(object)recently, (MetaDataComponent)null);
			}
		}
	}

	private void OnDowned(Entity<TackledRecentlyByComponent> ent, ref DownedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		BuckleComponent buckle = default(BuckleComponent);
		if (!((EntitySystem)this).HasComp<VictimInfectedComponent>(Entity<TackledRecentlyByComponent>.op_Implicit(ent)) && (!((EntitySystem)this).TryComp<BuckleComponent>(Entity<TackledRecentlyByComponent>.op_Implicit(ent), ref buckle) || !buckle.Buckled))
		{
			((EntitySystem)this).RemCompDeferred<TackledRecentlyByComponent>(Entity<TackledRecentlyByComponent>.op_Implicit(ent));
		}
	}

	private void OnRemove<T>(Entity<TackledRecentlyComponent> ent, ref T args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		TackledRecentlyByComponent tackled = default(TackledRecentlyByComponent);
		foreach (KeyValuePair<EntityUid, TackleTracker> tracker2 in ent.Comp.Trackers)
		{
			if (((EntitySystem)this).TryComp<TackledRecentlyByComponent>(tracker2.Key, ref tackled))
			{
				tackled.Tacklers.Remove(Entity<TackledRecentlyComponent>.op_Implicit(ent));
			}
		}
	}

	private void RemoveTackledBy(Entity<TackledRecentlyByComponent?> by, EntityUid tackler)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<TackledRecentlyByComponent>(Entity<TackledRecentlyByComponent>.op_Implicit(by), ref by.Comp, false))
		{
			by.Comp.Tacklers.Remove(tackler);
			((EntitySystem)this).Dirty<TackledRecentlyByComponent>(by, (MetaDataComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<TackledRecentlyComponent> query = ((EntitySystem)this).EntityQueryEnumerator<TackledRecentlyComponent>();
		EntityUid tackler = default(EntityUid);
		TackledRecentlyComponent recently = default(TackledRecentlyComponent);
		while (query.MoveNext(ref tackler, ref recently))
		{
			_trackersToRemove.Clear();
			foreach (KeyValuePair<EntityUid, TackleTracker> tracker in recently.Trackers)
			{
				if (time >= tracker.Value.Last + recently.ExpireAfter)
				{
					_trackersToRemove.Add(tracker.Key);
				}
			}
			foreach (EntityUid id in _trackersToRemove)
			{
				recently.Trackers.Remove(id);
				RemoveTackledBy(Entity<TackledRecentlyByComponent>.op_Implicit(id), tackler);
			}
			if (recently.Trackers.Count == 0)
			{
				((EntitySystem)this).RemCompDeferred<TackledRecentlyComponent>(tackler);
			}
		}
	}
}
