// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Admin.SharedRMCBanSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Shared._RMC14.Admin;

public abstract class SharedRMCBanSystem : EntitySystem
{
  public virtual bool IsJobBanned(NetUserId user, ProtoId<JobPrototype> job) => false;

  public bool IsJobBanned(Entity<ActorComponent?> user, ProtoId<JobPrototype> job)
  {
    return this.Resolve<ActorComponent>((EntityUid) user, ref user.Comp, false) && this.IsJobBanned(user.Comp.PlayerSession.UserId, job);
  }
}
