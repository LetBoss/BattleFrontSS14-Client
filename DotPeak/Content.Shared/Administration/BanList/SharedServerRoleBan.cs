// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.BanList.SharedServerRoleBan
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Administration.BanList;

[NetSerializable]
[Serializable]
public sealed record SharedServerRoleBan(
  int? Id,
  NetUserId? UserId,
  (string address, int cidrMask)? Address,
  string? HWId,
  DateTime BanTime,
  DateTime? ExpirationTime,
  string Reason,
  string? BanningAdminName,
  SharedServerUnban? Unban,
  string Role) : SharedServerBan(Id, UserId, Address, HWId, BanTime, ExpirationTime, Reason, BanningAdminName, Unban)
{
  public string Role { get; init; } = Role;

  [CompilerGenerated]
  public sealed override bool Equals(SharedServerBan? other) => this.Equals((object) other);

  [CompilerGenerated]
  public void Deconstruct(
    out int? Id,
    out NetUserId? UserId,
    out (string address, int cidrMask)? Address,
    out string? HWId,
    out DateTime BanTime,
    out DateTime? ExpirationTime,
    out string Reason,
    out string? BanningAdminName,
    out SharedServerUnban? Unban,
    out string Role)
  {
    Id = this.Id;
    UserId = this.UserId;
    Address = this.Address;
    HWId = this.HWId;
    BanTime = this.BanTime;
    ExpirationTime = this.ExpirationTime;
    Reason = this.Reason;
    BanningAdminName = this.BanningAdminName;
    Unban = this.Unban;
    Role = this.Role;
  }
}
