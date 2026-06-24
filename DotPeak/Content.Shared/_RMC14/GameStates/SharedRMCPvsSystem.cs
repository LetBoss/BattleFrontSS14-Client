// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.GameStates.SharedRMCPvsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;

#nullable enable
namespace Content.Shared._RMC14.GameStates;

public abstract class SharedRMCPvsSystem : EntitySystem
{
  [Dependency]
  private ISharedPlayerManager _player;

  public virtual void AddGlobalOverride(EntityUid ent)
  {
  }

  public virtual void RemoveGlobalOverride(EntityUid ent)
  {
  }

  public virtual void AddForceSend(EntityUid ent)
  {
  }

  public virtual void AddSessionOverride(EntityUid ent, ICommonSession session)
  {
  }

  public virtual void AddSessionOverride(EntityUid ent, NetUserId sessionId)
  {
    ICommonSession session;
    if (!this._player.TryGetSessionById(new NetUserId?(sessionId), out session))
      return;
    this.AddSessionOverride(ent, session);
  }
}
