// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.GlobalMap.CivGlobalMapVerbSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.GlobalMap;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._CIV14merka.GlobalMap;

public sealed class CivGlobalMapVerbSystem : EntitySystem
{
  [Dependency]
  private CivGlobalMapSystem _globalMap;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CivGlobalMapAlertComponent, GetVerbsEvent<ActivationVerb>>(new EntityEventRefHandler<CivGlobalMapAlertComponent, GetVerbsEvent<ActivationVerb>>((object) this, __methodptr(OnGetVerbs)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CivTeamMemberComponent, GetVerbsEvent<ActivationVerb>>(new EntityEventRefHandler<CivTeamMemberComponent, GetVerbsEvent<ActivationVerb>>((object) this, __methodptr(OnGetCommanderVerbs)), (Type[]) null, (Type[]) null);
  }

  private void OnGetVerbs(
    Entity<CivGlobalMapAlertComponent> ent,
    ref GetVerbsEvent<ActivationVerb> args)
  {
    if (EntityUid.op_Inequality(args.Target, ent.Owner) || EntityUid.op_Inequality(args.User, ent.Owner))
      return;
    SortedSet<ActivationVerb> verbs = args.Verbs;
    ActivationVerb activationVerb = new ActivationVerb();
    activationVerb.Text = this.Loc.GetString("civ-gmap-verb-open-map");
    activationVerb.ClientExclusive = true;
    activationVerb.CloseMenu = new bool?(true);
    activationVerb.Act = (Action) (() => this.RaiseLocalEvent<OpenCivGlobalMapAlertEvent>(ent.Owner, new OpenCivGlobalMapAlertEvent(), false));
    verbs.Add(activationVerb);
  }

  private void OnGetCommanderVerbs(
    Entity<CivTeamMemberComponent> ent,
    ref GetVerbsEvent<ActivationVerb> args)
  {
    if (EntityUid.op_Inequality(args.Target, ent.Owner) || EntityUid.op_Inequality(args.User, ent.Owner) || !ent.Comp.IsCommander)
      return;
    SortedSet<ActivationVerb> verbs = args.Verbs;
    ActivationVerb activationVerb = new ActivationVerb();
    activationVerb.Text = this.Loc.GetString("civ-gmap-verb-open-hq");
    activationVerb.ClientExclusive = true;
    activationVerb.CloseMenu = new bool?(true);
    activationVerb.Act = (Action) (() => this._globalMap.OpenCommanderWindow());
    verbs.Add(activationVerb);
  }
}
