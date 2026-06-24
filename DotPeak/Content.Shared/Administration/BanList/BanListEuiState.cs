// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.BanList.BanListEuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eui;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Administration.BanList;

[NetSerializable]
[Serializable]
public sealed class BanListEuiState : EuiStateBase
{
  public BanListEuiState(
    string banListPlayerName,
    List<SharedServerBan> bans,
    List<SharedServerRoleBan> roleBans)
  {
    this.BanListPlayerName = banListPlayerName;
    this.Bans = bans;
    this.RoleBans = roleBans;
  }

  public string BanListPlayerName { get; }

  public List<SharedServerBan> Bans { get; }

  public List<SharedServerRoleBan> RoleBans { get; }
}
