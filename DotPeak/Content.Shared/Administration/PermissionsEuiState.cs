// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.PermissionsEuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eui;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Administration;

[NetSerializable]
[Serializable]
public sealed class PermissionsEuiState : EuiStateBase
{
  public bool IsLoading;
  public PermissionsEuiState.AdminData[] Admins = Array.Empty<PermissionsEuiState.AdminData>();
  public Dictionary<int, PermissionsEuiState.AdminRankData> AdminRanks = new Dictionary<int, PermissionsEuiState.AdminRankData>();

  [NetSerializable]
  [Serializable]
  public struct AdminData
  {
    public NetUserId UserId;
    public string? UserName;
    public string? Title;
    public bool Suspended;
    public AdminFlags PosFlags;
    public AdminFlags NegFlags;
    public int? RankId;
  }

  [NetSerializable]
  [Serializable]
  public struct AdminRankData
  {
    public string Name;
    public AdminFlags Flags;
  }
}
