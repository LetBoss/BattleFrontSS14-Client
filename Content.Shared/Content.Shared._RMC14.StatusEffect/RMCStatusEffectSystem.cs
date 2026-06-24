using System;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.StatusEffect;

public sealed class RMCStatusEffectSystem : EntitySystem
{
	[Dependency]
	private SkillsSystem _skills;

	private static readonly EntProtoId<SkillDefinitionComponent> EnduranceSkill = EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillEndurance");

	private static readonly ProtoId<StatusEffectPrototype> Knockdown = ProtoId<StatusEffectPrototype>.op_Implicit("KnockedDown");

	private static readonly ProtoId<StatusEffectPrototype> Stun = ProtoId<StatusEffectPrototype>.op_Implicit("Stun");

	private static readonly ProtoId<StatusEffectPrototype> Unconscious = ProtoId<StatusEffectPrototype>.op_Implicit("Unconscious");

	private static readonly ProtoId<StatusEffectPrototype> Dazed = ProtoId<StatusEffectPrototype>.op_Implicit("Dazed");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SkillsComponent, RMCStatusEffectTimeEvent>((EntityEventRefHandler<SkillsComponent, RMCStatusEffectTimeEvent>)OnSkillsStatusEffectTime, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, RMCStatusEffectTimeEvent>((EntityEventRefHandler<XenoComponent, RMCStatusEffectTimeEvent>)OnXenoStatusEffectTime, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStunResistanceComponent, RMCStatusEffectTimeEvent>((EntityEventRefHandler<RMCStunResistanceComponent, RMCStatusEffectTimeEvent>)OnStunResistanceStatusEffectTime, (Type[])null, (Type[])null);
	}

	private void OnSkillsStatusEffectTime(Entity<SkillsComponent> ent, ref RMCStatusEffectTimeEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!(ProtoId<StatusEffectPrototype>.op_Implicit(args.Key) != Knockdown) || !(ProtoId<StatusEffectPrototype>.op_Implicit(args.Key) != Stun) || !(ProtoId<StatusEffectPrototype>.op_Implicit(args.Key) != Unconscious) || !(ProtoId<StatusEffectPrototype>.op_Implicit(args.Key) != Dazed))
		{
			int endurance = _skills.GetSkill(Entity<SkillsComponent>.op_Implicit((Entity<SkillsComponent>.op_Implicit(ent), Entity<SkillsComponent>.op_Implicit(ent))), EnduranceSkill);
			if (endurance >= 1)
			{
				double skill = (double)(endurance - 1) * 0.08;
				double multiplier = 1.0 - skill;
				args.Duration *= multiplier;
			}
		}
	}

	private void OnXenoStatusEffectTime(Entity<XenoComponent> ent, ref RMCStatusEffectTimeEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!(ProtoId<StatusEffectPrototype>.op_Implicit(args.Key) != Knockdown) || !(ProtoId<StatusEffectPrototype>.op_Implicit(args.Key) != Stun) || !(ProtoId<StatusEffectPrototype>.op_Implicit(args.Key) != Unconscious) || !(ProtoId<StatusEffectPrototype>.op_Implicit(args.Key) != Dazed))
		{
			args.Duration *= 0.667;
		}
	}

	private void OnStunResistanceStatusEffectTime(Entity<RMCStunResistanceComponent> ent, ref RMCStatusEffectTimeEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!(ProtoId<StatusEffectPrototype>.op_Implicit(args.Key) != Knockdown) || !(ProtoId<StatusEffectPrototype>.op_Implicit(args.Key) != Stun) || !(ProtoId<StatusEffectPrototype>.op_Implicit(args.Key) != Unconscious) || !(ProtoId<StatusEffectPrototype>.op_Implicit(args.Key) != Dazed))
		{
			args.Duration /= (double)ent.Comp.Resistance;
		}
	}

	public void GiveStunResistance(EntityUid target, float resistance)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		RMCStunResistanceComponent resistanceComp = ((EntitySystem)this).EnsureComp<RMCStunResistanceComponent>(target);
		resistanceComp.Resistance = resistance;
		((EntitySystem)this).Dirty(target, (IComponent)(object)resistanceComp, (MetaDataComponent)null);
	}
}
