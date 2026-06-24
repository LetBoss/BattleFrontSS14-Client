// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Examine.CMExamineSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;

#nullable enable
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
    this.SubscribeLocalEvent<RMCGenericExamineComponent, ExaminedEvent>(new EntityEventRefHandler<RMCGenericExamineComponent, ExaminedEvent>(this.OnGenericExamined));
    this.SubscribeLocalEvent<ShortExamineComponent, GetVerbsEvent<ExamineVerb>>(new EntityEventRefHandler<ShortExamineComponent, GetVerbsEvent<ExamineVerb>>(this.OnGetExaminedVerbs), after: new Type[2]
    {
      typeof (HealthExaminableSystem),
      typeof (IdExaminableSystem)
    });
    this.SubscribeLocalEvent<IdExaminableComponent, ExaminedEvent>(new EntityEventRefHandler<IdExaminableComponent, ExaminedEvent>(this.OnIdExamined));
    this.SubscribeLocalEvent<HealthExaminableComponent, ExaminedEvent>(new EntityEventRefHandler<HealthExaminableComponent, ExaminedEvent>(this.OnHealthExamined));
  }

  private void OnGenericExamined(Entity<RMCGenericExamineComponent> ent, ref ExaminedEvent args)
  {
    EntityUid examiner = args.Examiner;
    SkillWhitelist skillsRequired = ent.Comp.SkillsRequired;
    if (skillsRequired != null && !this._skillsSystem.HasSkills((Entity<SkillsComponent>) examiner, skillsRequired) || !this._entityWhitelist.CheckBoth(new EntityUid?(examiner), ent.Comp.Blacklist, ent.Comp.Whitelist))
      return;
    using (args.PushGroup(nameof (CMExamineSystem), ent.Comp.ExaminePriority))
      args.PushMarkup(this.Loc.GetString((string) ent.Comp.MessageId));
  }

  private void OnGetExaminedVerbs(
    Entity<ShortExamineComponent> ent,
    ref GetVerbsEvent<ExamineVerb> args)
  {
    args.Verbs.RemoveWhere((Predicate<ExamineVerb>) (v => v.Text == this.Loc.GetString("health-examinable-verb-text") || v.Text == this.Loc.GetString("id-examinable-component-verb-text")));
  }

  private void OnIdExamined(Entity<IdExaminableComponent> ent, ref ExaminedEvent args)
  {
    if (this.HasComp<BlockIdExamineComponent>(args.Examiner))
      return;
    using (args.PushGroup(nameof (CMExamineSystem), 1))
    {
      string info = this._idExaminable.GetInfo((EntityUid) ent);
      if (info == null)
        return;
      args.PushMarkup(info);
    }
  }

  private void OnHealthExamined(Entity<HealthExaminableComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup(nameof (CMExamineSystem), -1))
    {
      DamageableComponent comp;
      if (!this.TryComp<DamageableComponent>((EntityUid) ent, out comp))
        return;
      args.PushMessage(this._healthExaminable.CreateMarkup((EntityUid) ent, ent.Comp, comp));
    }
  }

  public bool CanExamine(Entity<BlockExamineComponent?> block, EntityUid user)
  {
    return !this.Resolve<BlockExamineComponent>((EntityUid) block, ref block.Comp, false) || !this._entityWhitelist.IsWhitelistPass(block.Comp.Whitelist, user);
  }
}
