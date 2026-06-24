// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.StatusEffect.RMCStatusEffectSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Shared._RMC14.StatusEffect;

public sealed class RMCStatusEffectSystem : EntitySystem
{
  [Dependency]
  private SkillsSystem _skills;
  private static readonly EntProtoId<SkillDefinitionComponent> EnduranceSkill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillEndurance";
  private static readonly ProtoId<StatusEffectPrototype> Knockdown = (ProtoId<StatusEffectPrototype>) "KnockedDown";
  private static readonly ProtoId<StatusEffectPrototype> Stun = (ProtoId<StatusEffectPrototype>) nameof (Stun);
  private static readonly ProtoId<StatusEffectPrototype> Unconscious = (ProtoId<StatusEffectPrototype>) nameof (Unconscious);
  private static readonly ProtoId<StatusEffectPrototype> Dazed = (ProtoId<StatusEffectPrototype>) nameof (Dazed);

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SkillsComponent, RMCStatusEffectTimeEvent>(new EntityEventRefHandler<SkillsComponent, RMCStatusEffectTimeEvent>(this.OnSkillsStatusEffectTime));
    this.SubscribeLocalEvent<XenoComponent, RMCStatusEffectTimeEvent>(new EntityEventRefHandler<XenoComponent, RMCStatusEffectTimeEvent>(this.OnXenoStatusEffectTime));
    this.SubscribeLocalEvent<RMCStunResistanceComponent, RMCStatusEffectTimeEvent>(new EntityEventRefHandler<RMCStunResistanceComponent, RMCStatusEffectTimeEvent>(this.OnStunResistanceStatusEffectTime));
  }

  private void OnSkillsStatusEffectTime(
    Entity<SkillsComponent> ent,
    ref RMCStatusEffectTimeEvent args)
  {
    if ((ProtoId<StatusEffectPrototype>) args.Key != RMCStatusEffectSystem.Knockdown && (ProtoId<StatusEffectPrototype>) args.Key != RMCStatusEffectSystem.Stun && (ProtoId<StatusEffectPrototype>) args.Key != RMCStatusEffectSystem.Unconscious && (ProtoId<StatusEffectPrototype>) args.Key != RMCStatusEffectSystem.Dazed)
      return;
    int skill = this._skills.GetSkill((Entity<SkillsComponent>) ((EntityUid) ent, (SkillsComponent) ent), RMCStatusEffectSystem.EnduranceSkill);
    if (skill < 1)
      return;
    double num = 1.0 - (double) (skill - 1) * 0.08;
    args.Duration *= num;
  }

  private void OnXenoStatusEffectTime(Entity<XenoComponent> ent, ref RMCStatusEffectTimeEvent args)
  {
    if ((ProtoId<StatusEffectPrototype>) args.Key != RMCStatusEffectSystem.Knockdown && (ProtoId<StatusEffectPrototype>) args.Key != RMCStatusEffectSystem.Stun && (ProtoId<StatusEffectPrototype>) args.Key != RMCStatusEffectSystem.Unconscious && (ProtoId<StatusEffectPrototype>) args.Key != RMCStatusEffectSystem.Dazed)
      return;
    args.Duration *= 0.667;
  }

  private void OnStunResistanceStatusEffectTime(
    Entity<RMCStunResistanceComponent> ent,
    ref RMCStatusEffectTimeEvent args)
  {
    if ((ProtoId<StatusEffectPrototype>) args.Key != RMCStatusEffectSystem.Knockdown && (ProtoId<StatusEffectPrototype>) args.Key != RMCStatusEffectSystem.Stun && (ProtoId<StatusEffectPrototype>) args.Key != RMCStatusEffectSystem.Unconscious && (ProtoId<StatusEffectPrototype>) args.Key != RMCStatusEffectSystem.Dazed)
      return;
    args.Duration /= (double) ent.Comp.Resistance;
  }

  public void GiveStunResistance(EntityUid target, float resistance)
  {
    RMCStunResistanceComponent resistanceComponent = this.EnsureComp<RMCStunResistanceComponent>(target);
    resistanceComponent.Resistance = resistance;
    this.Dirty(target, (IComponent) resistanceComponent);
  }
}
