// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.Systems.AdminVerbSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Administration;
using Content.Shared.Administration.Managers;
using Content.Shared.Mind.Components;
using Content.Shared.Verbs;
using Robust.Client.Console;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client.Administration.Systems;

internal sealed class AdminVerbSystem : EntitySystem
{
  [Dependency]
  private IClientConGroupController _clientConGroupController;
  [Dependency]
  private IClientConsoleHost _clientConsoleHost;
  [Dependency]
  private ISharedAdminManager _admin;

  public virtual void Initialize()
  {
    this.SubscribeLocalEvent<GetVerbsEvent<Verb>>(new EntityEventHandler<GetVerbsEvent<Verb>>(this.AddAdminVerbs), (Type[]) null, (Type[]) null);
  }

  private void AddAdminVerbs(GetVerbsEvent<Verb> args)
  {
    if (((IClientConGroupImplementation) this._clientConGroupController).CanViewVar())
    {
      VvVerb vvVerb1 = new VvVerb();
      vvVerb1.Text = this.Loc.GetString("view-variables");
      vvVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/vv.svg.192dpi.png"));
      vvVerb1.Act = (Action) (() => ((IConsoleHost) this._clientConsoleHost).ExecuteCommand($"vv {this.GetNetEntity(args.Target, (MetaDataComponent) null)}"));
      vvVerb1.ClientExclusive = true;
      VvVerb vvVerb2 = vvVerb1;
      args.Verbs.Add((Verb) vvVerb2);
    }
    if (!this._admin.IsAdmin(args.User))
      return;
    if (this._admin.HasAdminFlag(args.User, AdminFlags.Admin))
      args.ExtraCategories.Add(VerbCategory.Admin);
    if (this._admin.HasAdminFlag(args.User, AdminFlags.Fun) && this.HasComp<MindContainerComponent>(args.Target))
      args.ExtraCategories.Add(VerbCategory.Antag);
    if (this._admin.HasAdminFlag(args.User, AdminFlags.Debug))
      args.ExtraCategories.Add(VerbCategory.Debug);
    if (this._admin.HasAdminFlag(args.User, AdminFlags.Fun))
      args.ExtraCategories.Add(VerbCategory.Smite);
    if (!this._admin.HasAdminFlag(args.User, AdminFlags.Admin))
      return;
    args.ExtraCategories.Add(VerbCategory.Tricks);
  }
}
