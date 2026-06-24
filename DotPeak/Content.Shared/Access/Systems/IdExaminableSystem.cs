// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Systems.IdExaminableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.PDA;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Access.Systems;

public sealed class IdExaminableSystem : EntitySystem
{
  [Dependency]
  private ExamineSystemShared _examineSystem;
  [Dependency]
  private InventorySystem _inventorySystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<IdExaminableComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<IdExaminableComponent, GetVerbsEvent<ExamineVerb>>((object) this, __methodptr(OnGetExamineVerbs)), (Type[]) null, (Type[]) null);
  }

  private void OnGetExamineVerbs(
    EntityUid uid,
    IdExaminableComponent component,
    GetVerbsEvent<ExamineVerb> args)
  {
    bool flag = this._examineSystem.IsInDetailsRange(args.User, uid);
    string info = this.GetMessage(uid);
    ExamineVerb examineVerb1 = new ExamineVerb();
    examineVerb1.Act = (Action) (() => this._examineSystem.SendExamineTooltip(args.User, uid, FormattedMessage.FromMarkupOrThrow(info), false, false));
    examineVerb1.Text = this.Loc.GetString("id-examinable-component-verb-text");
    examineVerb1.Category = VerbCategory.Examine;
    examineVerb1.Disabled = !flag;
    examineVerb1.Message = flag ? (string) null : this.Loc.GetString("id-examinable-component-verb-disabled");
    examineVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/character.svg.192dpi.png"));
    ExamineVerb examineVerb2 = examineVerb1;
    args.Verbs.Add(examineVerb2);
  }

  public string GetMessage(EntityUid uid)
  {
    return this.GetInfo(uid) ?? this.Loc.GetString("id-examinable-component-verb-no-id");
  }

  public string? GetInfo(EntityUid uid)
  {
    EntityUid? entityUid;
    PdaComponent pdaComponent;
    IdCardComponent id;
    return this._inventorySystem.TryGetSlotEntity(uid, "id", out entityUid) && (this.TryComp<PdaComponent>(entityUid, ref pdaComponent) && this.TryComp<IdCardComponent>(pdaComponent.ContainedId, ref id) || this.TryComp<IdCardComponent>(entityUid, ref id)) ? this.GetNameAndJob(id) : (string) null;
  }

  private string GetNameAndJob(IdCardComponent id)
  {
    string str = string.IsNullOrWhiteSpace(id.LocalizedJobTitle) ? string.Empty : $" ({id.LocalizedJobTitle})";
    return !string.IsNullOrWhiteSpace(id.FullName) ? this.Loc.GetString(LocId.op_Implicit(id.FullNameLocId), ("fullName", (object) id.FullName), ("jobSuffix", (object) str)) : this.Loc.GetString(LocId.op_Implicit(id.NameLocId), ("jobSuffix", (object) str));
  }
}
