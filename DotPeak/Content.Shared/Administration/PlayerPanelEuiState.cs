// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.PlayerPanelEuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eui;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Administration;

[NetSerializable]
[Serializable]
public sealed class PlayerPanelEuiState(
  NetUserId guid,
  string username,
  TimeSpan playtime,
  int? totalNotes,
  int? totalBans,
  int? totalRoleBans,
  int sharedConnections,
  bool? whitelisted,
  bool canFreeze,
  bool frozen,
  bool canAhelp) : EuiStateBase
{
  public readonly NetUserId Guid = guid;
  public readonly string Username = username;
  public readonly TimeSpan Playtime = playtime;
  public readonly int? TotalNotes = totalNotes;
  public readonly int? TotalBans = totalBans;
  public readonly int? TotalRoleBans = totalRoleBans;
  public readonly int SharedConnections = sharedConnections;
  public readonly bool? Whitelisted = whitelisted;
  public readonly bool CanFreeze = canFreeze;
  public readonly bool Frozen = frozen;
  public readonly bool CanAhelp = canAhelp;
}
