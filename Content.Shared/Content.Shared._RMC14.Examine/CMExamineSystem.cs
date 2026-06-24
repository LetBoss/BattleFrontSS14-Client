using System;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.HealthExaminable;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared._RMC14.Examine;

public sealed class CMExamineSystem : EntitySystem
{
	[Dependency]
	private SkillsSystem _skillsSystem;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private HealthExaminableSystem _healthExaminable;

	[Dependency]
	private IdExaminableSystem _idExaminable;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCGenericExamineComponent, ExaminedEvent>((EntityEventRefHandler<RMCGenericExamineComponent, ExaminedEvent>)OnGenericExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ShortExamineComponent, GetVerbsEvent<ExamineVerb>>((EntityEventRefHandler<ShortExamineComponent, GetVerbsEvent<ExamineVerb>>)OnGetExaminedVerbs, (Type[])null, new Type[2]
		{
			typeof(HealthExaminableSystem),
			typeof(IdExaminableSystem)
		});
		((EntitySystem)this).SubscribeLocalEvent<IdExaminableComponent, ExaminedEvent>((EntityEventRefHandler<IdExaminableComponent, ExaminedEvent>)OnIdExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HealthExaminableComponent, ExaminedEvent>((EntityEventRefHandler<HealthExaminableComponent, ExaminedEvent>)OnHealthExamined, (Type[])null, (Type[])null);
	}

	private void OnGenericExamined(Entity<RMCGenericExamineComponent> ent, ref ExaminedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.Examiner;
		SkillWhitelist skillsRequired = ent.Comp.SkillsRequired;
		if ((skillsRequired != null && !_skillsSystem.HasSkills(Entity<SkillsComponent>.op_Implicit(user), skillsRequired)) || !_entityWhitelist.CheckBoth(user, ent.Comp.Blacklist, ent.Comp.Whitelist))
		{
			return;
		}
		using (args.PushGroup("CMExamineSystem", ent.Comp.ExaminePriority))
		{
			args.PushMarkup(base.Loc.GetString(LocId.op_Implicit(ent.Comp.MessageId)));
		}
	}

	private void OnGetExaminedVerbs(Entity<ShortExamineComponent> ent, ref GetVerbsEvent<ExamineVerb> args)
	{
		args.Verbs.RemoveWhere((ExamineVerb v) => v.Text == base.Loc.GetString("health-examinable-verb-text") || v.Text == base.Loc.GetString("id-examinable-component-verb-text"));
	}

	private void OnIdExamined(Entity<IdExaminableComponent> ent, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<BlockIdExamineComponent>(args.Examiner))
		{
			return;
		}
		using (args.PushGroup("CMExamineSystem", 1))
		{
			string info = _idExaminable.GetInfo(Entity<IdExaminableComponent>.op_Implicit(ent));
			if (info != null)
			{
				args.PushMarkup(info);
			}
		}
	}

	private void OnHealthExamined(Entity<HealthExaminableComponent> ent, ref ExaminedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("CMExamineSystem", -1))
		{
			DamageableComponent damageable = default(DamageableComponent);
			if (((EntitySystem)this).TryComp<DamageableComponent>(Entity<HealthExaminableComponent>.op_Implicit(ent), ref damageable))
			{
				args.PushMessage(_healthExaminable.CreateMarkup(Entity<HealthExaminableComponent>.op_Implicit(ent), ent.Comp, damageable));
			}
		}
	}

	public bool CanExamine(Entity<BlockExamineComponent?> block, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BlockExamineComponent>(Entity<BlockExamineComponent>.op_Implicit(block), ref block.Comp, false))
		{
			return true;
		}
		return !_entityWhitelist.IsWhitelistPass(block.Comp.Whitelist, user);
	}
}
