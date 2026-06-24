// Decompiled with JetBrains decompiler
// Type: Content.Shared.RMCLoreExaminable.DetailExaminableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.RMCLoreExaminable;

public sealed class DetailExaminableSystem : EntitySystem
{
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private NpcFactionSystem _npcFaction;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCLoreExaminableComponent, GetVerbsEvent<ExamineVerb>>(new EntityEventRefHandler<RMCLoreExaminableComponent, GetVerbsEvent<ExamineVerb>>(this.OnGetExamineVerbs));
  }

  private void OnGetExamineVerbs(
    Entity<RMCLoreExaminableComponent> ent,
    ref GetVerbsEvent<ExamineVerb> args)
  {
    if (this.HasComp<XenoComponent>(args.User) || (string) Identity.Name(args.Target, (IEntityManager) this.EntityManager) != this.MetaData(args.Target).EntityName || ent.Comp.Factions != null && ent.Comp.Factions.Count > 0 && !this._npcFaction.IsMemberOfAny((Entity<NpcFactionMemberComponent>) args.User, (IEnumerable<ProtoId<NpcFactionPrototype>>) ent.Comp.Factions))
      return;
    bool flag = this._examine.IsInDetailsRange(args.User, (EntityUid) ent);
    EntityUid user = args.User;
    ExamineVerb examineVerb1 = new ExamineVerb();
    examineVerb1.Act = (Action) (() =>
    {
      FormattedMessage message = new FormattedMessage();
      message.AddMarkupPermissive(this.Loc.GetString((string) ent.Comp.Content));
      this._examine.SendExamineTooltip(user, (EntityUid) ent, message, false, false);
    });
    examineVerb1.Text = this.Loc.GetString("lore-examinable-verb-text");
    examineVerb1.Category = VerbCategory.Examine;
    examineVerb1.Disabled = !flag;
    examineVerb1.Message = flag ? (string) null : this.Loc.GetString("lore-examinable-verb-disabled");
    examineVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/examine.svg.192dpi.png"));
    ExamineVerb examineVerb2 = examineVerb1;
    args.Verbs.Add(examineVerb2);
  }
}
