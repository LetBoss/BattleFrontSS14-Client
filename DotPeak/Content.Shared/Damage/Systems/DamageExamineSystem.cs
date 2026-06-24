// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Systems.DamageExamineSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Damage.Systems;

public sealed class DamageExamineSystem : EntitySystem
{
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private IPrototypeManager _prototype;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageExaminableComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<DamageExaminableComponent, GetVerbsEvent<ExamineVerb>>((object) this, __methodptr(OnGetExamineVerbs)), (Type[]) null, (Type[]) null);
  }

  private void OnGetExamineVerbs(
    EntityUid uid,
    DamageExaminableComponent component,
    GetVerbsEvent<ExamineVerb> args)
  {
    if (!args.CanInteract || !args.CanAccess)
      return;
    DamageExamineEvent damageExamineEvent = new DamageExamineEvent(new FormattedMessage(), args.User);
    this.RaiseLocalEvent<DamageExamineEvent>(uid, ref damageExamineEvent, false);
    if (damageExamineEvent.Message.IsEmpty)
      return;
    this._examine.AddDetailedExamineVerb(args, (Component) component, damageExamineEvent.Message, this.Loc.GetString("damage-examinable-verb-text"), "/Textures/Interface/VerbIcons/smite.svg.192dpi.png", this.Loc.GetString("damage-examinable-verb-message"));
  }

  public void AddDamageExamine(
    FormattedMessage message,
    DamageSpecifier damageSpecifier,
    string? type = null)
  {
    FormattedMessage damageExamine = this.GetDamageExamine(damageSpecifier, type);
    if (!message.IsEmpty)
      message.PushNewline();
    message.AddMessage(damageExamine);
  }

  private FormattedMessage GetDamageExamine(DamageSpecifier damageSpecifier, string? type = null)
  {
    FormattedMessage damageExamine = new FormattedMessage();
    if (string.IsNullOrEmpty(type))
    {
      damageExamine.AddMarkupOrThrow(this.Loc.GetString("damage-examine"));
    }
    else
    {
      if (damageSpecifier.GetTotal() == FixedPoint2.Zero && !damageSpecifier.AnyPositive())
      {
        damageExamine.AddMarkupOrThrow(this.Loc.GetString("damage-none"));
        return damageExamine;
      }
      damageExamine.AddMarkupOrThrow(this.Loc.GetString("damage-examine-type", (nameof (type), (object) type)));
    }
    foreach (KeyValuePair<string, FixedPoint2> keyValuePair in damageSpecifier.DamageDict)
    {
      if (keyValuePair.Value != FixedPoint2.Zero)
      {
        damageExamine.PushNewline();
        damageExamine.AddMarkupOrThrow(this.Loc.GetString("damage-value", (nameof (type), (object) this._prototype.Index<DamageTypePrototype>(keyValuePair.Key).LocalizedName), ("amount", (object) keyValuePair.Value)));
      }
    }
    return damageExamine;
  }
}
