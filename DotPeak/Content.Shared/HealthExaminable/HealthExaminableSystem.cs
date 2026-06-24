// Decompiled with JetBrains decompiler
// Type: Content.Shared.HealthExaminable.HealthExaminableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.HealthExaminable;

public sealed class HealthExaminableSystem : EntitySystem
{
  [Dependency]
  private ExamineSystemShared _examineSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<HealthExaminableComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<HealthExaminableComponent, GetVerbsEvent<ExamineVerb>>(this.OnGetExamineVerbs));
  }

  private void OnGetExamineVerbs(
    EntityUid uid,
    HealthExaminableComponent component,
    GetVerbsEvent<ExamineVerb> args)
  {
    DamageableComponent damage;
    if (!this.TryComp<DamageableComponent>(uid, out damage))
      return;
    bool flag = this._examineSystem.IsInDetailsRange(args.User, uid);
    ExamineVerb examineVerb1 = new ExamineVerb();
    examineVerb1.Act = (Action) (() => this._examineSystem.SendExamineTooltip(args.User, uid, this.CreateMarkup(uid, component, damage), false, false));
    examineVerb1.Text = this.Loc.GetString("health-examinable-verb-text");
    examineVerb1.Category = VerbCategory.Examine;
    examineVerb1.Disabled = !flag;
    examineVerb1.Message = flag ? (string) null : this.Loc.GetString("health-examinable-verb-disabled");
    examineVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/rejuvenate.svg.192dpi.png"));
    ExamineVerb examineVerb2 = examineVerb1;
    args.Verbs.Add(examineVerb2);
  }

  public FormattedMessage CreateMarkup(
    EntityUid uid,
    HealthExaminableComponent component,
    DamageableComponent damage)
  {
    FormattedMessage message = new FormattedMessage();
    bool flag = true;
    foreach (ProtoId<DamageTypePrototype> examinableType in component.ExaminableTypes)
    {
      FixedPoint2 fixedPoint2_1;
      if (damage.Damage.DamageDict.TryGetValue((string) examinableType, out fixedPoint2_1) && !(fixedPoint2_1 == FixedPoint2.Zero))
      {
        FixedPoint2 fixedPoint2_2 = FixedPoint2.Zero;
        string markup = string.Empty;
        foreach (FixedPoint2 threshold in component.Thresholds)
        {
          string str1 = $"health-examinable-{component.LocPrefix}-{examinableType}-{threshold}";
          string str2 = this.Loc.GetString($"health-examinable-{component.LocPrefix}-{examinableType}-{threshold}", ("target", (object) Identity.Entity(uid, (IEntityManager) this.EntityManager)));
          if (!(str2 == str1) && fixedPoint2_1 > threshold && threshold > fixedPoint2_2)
          {
            markup = str2;
            fixedPoint2_2 = threshold;
          }
        }
        if (!(fixedPoint2_2 == FixedPoint2.Zero))
        {
          if (!flag)
            message.PushNewline();
          else
            flag = false;
          message.AddMarkupOrThrow(markup);
        }
      }
    }
    if (message.IsEmpty && component.ExamineShowEmpty)
      message.AddMarkupOrThrow(this.Loc.GetString($"health-examinable-{component.LocPrefix}-none"));
    this.RaiseLocalEvent<HealthBeingExaminedEvent>(uid, new HealthBeingExaminedEvent(message), true);
    return message;
  }
}
