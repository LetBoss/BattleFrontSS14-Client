// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.SelectableSounds.RMCSelectableSoundsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Sound;
using Content.Shared.Popups;
using Content.Shared.Sound.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.SelectableSounds;

public sealed class RMCSelectableSoundsSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCSelectableSoundsComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<RMCSelectableSoundsComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetAltVerbs));
  }

  private void OnGetAltVerbs(
    Entity<RMCSelectableSoundsComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null)
      return;
    EntityUid user = args.User;
    ValueList<AlternativeVerb> other = new ValueList<AlternativeVerb>();
    foreach (KeyValuePair<LocId, SoundSpecifier> sound1 in ent.Comp.Sounds)
    {
      string name = this.Loc.GetString((string) sound1.Key);
      SoundSpecifier sound = sound1.Value;
      AlternativeVerb alternativeVerb1 = new AlternativeVerb();
      alternativeVerb1.Text = name;
      alternativeVerb1.IconEntity = new NetEntity?(this.GetNetEntity(ent.Owner));
      alternativeVerb1.Category = VerbCategory.SelectType;
      alternativeVerb1.Act = (Action) (() =>
      {
        EmitSoundOnUseComponent comp1;
        if (this.TryComp<EmitSoundOnUseComponent>(ent.Owner, out comp1))
          comp1.Sound = sound;
        EmitSoundOnActionComponent comp2;
        if (this.TryComp<EmitSoundOnActionComponent>(ent.Owner, out comp2))
          comp2.Sound = sound;
        this._popup.PopupClient(this.Loc.GetString("rmc-sound-select", ("sound", (object) name)), user, new EntityUid?(user));
      });
      AlternativeVerb alternativeVerb2 = alternativeVerb1;
      other.Add(alternativeVerb2);
    }
    args.Verbs.UnionWith((IEnumerable<AlternativeVerb>) other);
  }
}
