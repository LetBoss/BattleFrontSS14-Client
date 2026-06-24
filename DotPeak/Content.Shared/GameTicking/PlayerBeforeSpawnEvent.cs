// Decompiled with JetBrains decompiler
// Type: Content.Shared.GameTicking.PlayerBeforeSpawnEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;

#nullable enable
namespace Content.Shared.GameTicking;

public sealed class PlayerBeforeSpawnEvent : HandledEntityEventArgs
{
  public ICommonSession Player { get; }

  public HumanoidCharacterProfile Profile { get; }

  public string? JobId { get; }

  public bool LateJoin { get; }

  public EntityUid Station { get; }

  public PlayerBeforeSpawnEvent(
    ICommonSession player,
    HumanoidCharacterProfile profile,
    string? jobId,
    bool lateJoin,
    EntityUid station)
  {
    this.Player = player;
    this.Profile = profile;
    this.JobId = jobId;
    this.LateJoin = lateJoin;
    this.Station = station;
  }
}
