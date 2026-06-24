using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared.Interaction.Events;
using Content.Shared.Mind;
using Content.Shared.Popups;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Marines.Skills.Pamphlets;

public sealed class SkillPamphletSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedMindSystem _mind;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedJobSystem _job;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SquadSystem _squads;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SkillPamphletComponent, UseInHandEvent>((EntityEventRefHandler<SkillPamphletComponent, UseInHandEvent>)OnUse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UsedSkillPamphletComponent, GetMarineIconEvent>((EntityEventRefHandler<UsedSkillPamphletComponent, GetMarineIconEvent>)OnGetMarineIcon, (Type[])null, new Type[2]
		{
			typeof(SharedMarineSystem),
			typeof(SquadSystem)
		});
		((EntitySystem)this).SubscribeLocalEvent<UsedSkillPamphletComponent, GetMarineSquadNameEvent>((EntityEventRefHandler<UsedSkillPamphletComponent, GetMarineSquadNameEvent>)OnGetSquadTitle, (Type[])null, new Type[1] { typeof(SquadSystem) });
	}

	private void OnUse(Entity<SkillPamphletComponent> ent, ref UseInHandEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		UsedSkillPamphletComponent used = default(UsedSkillPamphletComponent);
		if (!ent.Comp.BypassLimit && ((EntitySystem)this).TryComp<UsedSkillPamphletComponent>(args.User, ref used) && used.Used)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-pamphlets-limit-reached"), Entity<SkillPamphletComponent>.op_Implicit(ent), args.User);
			return;
		}
		foreach (SkillPamphletComponent.PamphletWhitelist whitelist in ent.Comp.Whitelists)
		{
			if (_whitelistSystem.IsWhitelistFail(whitelist.Restrictions, args.User))
			{
				_popup.PopupClient(base.Loc.GetString(whitelist.Popup), Entity<SkillPamphletComponent>.op_Implicit(ent), args.User);
				return;
			}
		}
		bool failed = ent.Comp.JobWhitelists.Count > 0;
		LocId? popup = null;
		foreach (SkillPamphletComponent.JobWhitelist whitelist2 in ent.Comp.JobWhitelists)
		{
			if (_mind.TryGetMind(args.User, out EntityUid mindId, out MindComponent _) && _job.MindHasJobWithId(mindId, ProtoId<JobPrototype>.op_Implicit(whitelist2.JobProto)))
			{
				failed = false;
			}
			else
			{
				popup = whitelist2.Popup;
			}
		}
		if (failed)
		{
			if (popup.HasValue)
			{
				SharedPopupSystem popup2 = _popup;
				ILocalizationManager loc = base.Loc;
				LocId? val = popup;
				popup2.PopupClient(loc.GetString(val.HasValue ? LocId.op_Implicit(val.GetValueOrDefault()) : null), Entity<SkillPamphletComponent>.op_Implicit(ent), args.User);
			}
			return;
		}
		foreach (ComponentRegistryEntry comp in ((Dictionary<string, ComponentRegistryEntry>)(object)ent.Comp.AddComps).Values)
		{
			if (!((EntitySystem)this).HasComp(args.User, ((object)comp.Component).GetType()))
			{
				base.EntityManager.AddComponent(args.User, comp, false, (MetaDataComponent)null);
				ent.Comp.GaveSkill = true;
			}
		}
		foreach (KeyValuePair<EntProtoId<SkillDefinitionComponent>, int> skill in ent.Comp.AddSkills)
		{
			int skillCap;
			int cap = (ent.Comp.SkillCap.TryGetValue(skill.Key, out skillCap) ? skillCap : skill.Value);
			if (!_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(args.User), skill.Key, cap))
			{
				int level = _skills.GetSkill(Entity<SkillsComponent>.op_Implicit(args.User), skill.Key) + skill.Value;
				level = Math.Min(level, cap);
				_skills.SetSkill(Entity<SkillsComponent>.op_Implicit(args.User), skill.Key, level);
				ent.Comp.GaveSkill = true;
			}
		}
		if (ent.Comp.GaveSkill || ent.Comp.BypassSkill)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-pamphlets-reading"), args.User, args.User);
			UsedSkillPamphletComponent usedSkillComp = ((EntitySystem)this).EnsureComp<UsedSkillPamphletComponent>(args.User);
			if (ent.Comp.GiveIcon != null)
			{
				usedSkillComp.Icon = ent.Comp.GiveIcon;
			}
			if (ent.Comp.GiveJobTitle.HasValue)
			{
				usedSkillComp.JobTitle = ent.Comp.GiveJobTitle;
			}
			if (!ent.Comp.BypassLimit)
			{
				usedSkillComp.Used = true;
			}
			((EntitySystem)this).Dirty(args.User, (IComponent)(object)usedSkillComp, (MetaDataComponent)null);
			MapBlipIconOverrideComponent mapBlip = ((EntitySystem)this).EnsureComp<MapBlipIconOverrideComponent>(args.User);
			if (ent.Comp.GiveMapBlip != null)
			{
				mapBlip.Icon = ent.Comp.GiveMapBlip;
			}
			((EntitySystem)this).Dirty(args.User, (IComponent)(object)mapBlip, (MetaDataComponent)null);
			_squads.UpdateSquadTitle(args.User);
			if (ent.Comp.GivePrefix.HasValue)
			{
				JobPrefixComponent jobPrefix = ((EntitySystem)this).EnsureComp<JobPrefixComponent>(args.User);
				if (ent.Comp.IsAppendPrefix)
				{
					jobPrefix.AdditionalPrefix = ent.Comp.GivePrefix.Value;
				}
				else
				{
					jobPrefix.Prefix = ent.Comp.GivePrefix.Value;
				}
				((EntitySystem)this).Dirty(args.User, (IComponent)(object)jobPrefix, (MetaDataComponent)null);
			}
			if (!_net.IsClient)
			{
				((EntitySystem)this).QueueDel((EntityUid?)Entity<SkillPamphletComponent>.op_Implicit(ent));
			}
		}
		else
		{
			_popup.PopupClient(base.Loc.GetString("rmc-pamphlets-already-know"), Entity<SkillPamphletComponent>.op_Implicit(ent), args.User);
		}
	}

	private void OnGetMarineIcon(Entity<UsedSkillPamphletComponent> ent, ref GetMarineIconEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<SquadLeaderComponent>(Entity<UsedSkillPamphletComponent>.op_Implicit(ent)) && ent.Comp.Icon != null)
		{
			args.Icon = (SpriteSpecifier?)(object)ent.Comp.Icon;
		}
	}

	private void OnGetSquadTitle(Entity<UsedSkillPamphletComponent> ent, ref GetMarineSquadNameEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.JobTitle.HasValue)
		{
			ILocalizationManager loc = base.Loc;
			LocId? jobTitle = ent.Comp.JobTitle;
			args.RoleName = loc.GetString(jobTitle.HasValue ? LocId.op_Implicit(jobTitle.GetValueOrDefault()) : null);
		}
	}
}
