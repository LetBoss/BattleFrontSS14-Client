// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Admin.SharedRMCAdminSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration;
using Content.Shared.Administration.Managers;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

#nullable enable
namespace Content.Shared._RMC14.Admin;

public abstract class SharedRMCAdminSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminManager _admin;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<GetVerbsEvent<RMCAdminVerb>>(new EntityEventHandler<GetVerbsEvent<RMCAdminVerb>>(this.OnXenoGetVerbs));
  }

  private void OnXenoGetVerbs(GetVerbsEvent<RMCAdminVerb> args)
  {
    ActorComponent comp;
    if (!this.TryComp<ActorComponent>(args.User, out comp))
      return;
    this.CanUse(comp.PlayerSession);
  }

  protected bool CanUse(ICommonSession player)
  {
    return this._admin.HasAdminFlag(player, AdminFlags.Debug);
  }

  protected virtual void OpenBui(ICommonSession player, EntityUid target)
  {
  }
}
