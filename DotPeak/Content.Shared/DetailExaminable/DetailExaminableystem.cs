// Decompiled with JetBrains decompiler
// Type: Content.Shared.DetailExaminable.DetailExaminableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.DetailExaminable;

public sealed class DetailExaminableSystem : EntitySystem
{
  [Dependency]
  private ExamineSystemShared _examine;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DetailExaminableComponent, GetVerbsEvent<ExamineVerb>>(new EntityEventRefHandler<DetailExaminableComponent, GetVerbsEvent<ExamineVerb>>((object) this, __methodptr(OnGetExamineVerbs)), (Type[]) null, (Type[]) null);
  }

  private void OnGetExamineVerbs(
    Entity<DetailExaminableComponent> ent,
    ref GetVerbsEvent<ExamineVerb> args)
  {
    if (this.HasComp<XenoComponent>(args.User) || (string) Identity.Name(args.Target, (IEntityManager) this.EntityManager) != this.MetaData(args.Target).EntityName)
      return;
    bool flag = this._examine.IsInDetailsRange(args.User, Entity<DetailExaminableComponent>.op_Implicit(ent));
    EntityUid user = args.User;
    ExamineVerb examineVerb1 = new ExamineVerb();
    examineVerb1.Act = (Action) (() =>
    {
      FormattedMessage message = new FormattedMessage();
      message.AddMarkupPermissive(ent.Comp.Content);
      this._examine.SendExamineTooltip(user, Entity<DetailExaminableComponent>.op_Implicit(ent), message, false, false);
    });
    examineVerb1.Text = this.Loc.GetString("detail-examinable-verb-text");
    examineVerb1.Category = VerbCategory.Examine;
    examineVerb1.Disabled = !flag;
    examineVerb1.Message = flag ? (string) null : this.Loc.GetString("detail-examinable-verb-disabled");
    examineVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/examine.svg.192dpi.png"));
    ExamineVerb examineVerb2 = examineVerb1;
    args.Verbs.Add(examineVerb2);
  }
}
