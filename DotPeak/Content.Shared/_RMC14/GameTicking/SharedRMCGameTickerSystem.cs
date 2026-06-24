// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.GameTicking.SharedRMCGameTickerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable
namespace Content.Shared._RMC14.GameTicking;

public abstract class SharedRMCGameTickerSystem : EntitySystem
{
  public virtual IReadOnlyDictionary<NetUserId, PlayerGameStatus> PlayerGameStatuses
  {
    get
    {
      return (IReadOnlyDictionary<NetUserId, PlayerGameStatus>) ImmutableDictionary<NetUserId, PlayerGameStatus>.Empty;
    }
  }

  public virtual void PlayerJoinGame(ICommonSession session, bool silent = false)
  {
  }

  public virtual bool ServerOnlyIsInRound() => false;
}
