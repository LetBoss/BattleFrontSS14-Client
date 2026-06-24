// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.PlayerInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mind;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Administration;

[NetSerializable]
[Serializable]
public sealed record PlayerInfo(
  string Username,
  string CharacterName,
  string IdentityName,
  string StartingJob,
  bool Antag,
  ProtoId<RoleTypePrototype>? RoleProto,
  LocId? Subtype,
  int SortWeight,
  Robust.Shared.GameObjects.NetEntity? NetEntity,
  NetUserId SessionId,
  bool Connected,
  bool ActiveThisRound,
  TimeSpan? OverallPlaytime)
{
  private string? _playtimeString;

  public bool IsPinned { get; set; }

  public string PlaytimeString
  {
    get
    {
      string playtimeString = this._playtimeString;
      if (playtimeString != null)
        return playtimeString;
      TimeSpan? overallPlaytime = this.OverallPlaytime;
      ref TimeSpan? local = ref overallPlaytime;
      return this._playtimeString = (local.HasValue ? local.GetValueOrDefault().ToString("%d':'hh':'mm") : (string) null) ?? Loc.GetString("generic-unknown-title");
    }
  }

  public bool Equals(PlayerInfo? other)
  {
    NetUserId? sessionId1 = other?.SessionId;
    NetUserId sessionId2 = this.SessionId;
    return sessionId1.HasValue && NetUserId.op_Equality(sessionId1.GetValueOrDefault(), sessionId2);
  }

  public override int GetHashCode() => this.SessionId.GetHashCode();
}
