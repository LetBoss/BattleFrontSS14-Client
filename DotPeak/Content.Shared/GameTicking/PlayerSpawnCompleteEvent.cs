// Decompiled with JetBrains decompiler
// Type: Content.Shared.GameTicking.PlayerSpawnCompleteEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;

#nullable enable
namespace Content.Shared.GameTicking;

public sealed class PlayerSpawnCompleteEvent : EntityEventArgs
{
  public EntityUid Mob { get; }

  public ICommonSession Player { get; }

  public string? JobId { get; }

  public bool LateJoin { get; }

  public bool Silent { get; }

  public EntityUid Station { get; }

  public HumanoidCharacterProfile Profile { get; }

  public int JoinOrder { get; }

  public PlayerSpawnCompleteEvent(
    EntityUid mob,
    ICommonSession player,
    string? jobId,
    bool lateJoin,
    bool silent,
    int joinOrder,
    EntityUid station,
    HumanoidCharacterProfile profile)
  {
    this.Mob = mob;
    this.Player = player;
    this.JobId = jobId;
    this.LateJoin = lateJoin;
    this.Silent = silent;
    this.Station = station;
    this.Profile = profile;
    this.JoinOrder = joinOrder;
  }
}
