// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Movement.RMCLagCompensationSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Movement;
using Robust.Client.Timing;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Client._RMC14.Movement;

public sealed class RMCLagCompensationSystem : SharedRMCLagCompensationSystem
{
  [Dependency]
  private IClientGameTiming _timing;

  public override GameTick GetLastRealTick(NetUserId? session) => this._timing.LastRealTick;
}
