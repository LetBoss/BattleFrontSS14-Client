// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.BanPanelEuiStateMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Shared.Serialization;
using System;
using System.Net;

#nullable enable
namespace Content.Shared.Administration;

public static class BanPanelEuiStateMsg
{
  [NetSerializable]
  [Serializable]
  public sealed class CreateBanRequest : EuiMessageBase
  {
    public string? Player { get; set; }

    public string? IpAddress { get; set; }

    public ImmutableTypedHwid? Hwid { get; set; }

    public uint Minutes { get; set; }

    public string Reason { get; set; }

    public NoteSeverity Severity { get; set; }

    public string[]? Roles { get; set; }

    public bool UseLastIp { get; set; }

    public bool UseLastHwid { get; set; }

    public bool Erase { get; set; }

    public CreateBanRequest(
      string? player,
      (IPAddress, int)? ipAddress,
      bool useLastIp,
      ImmutableTypedHwid? hwid,
      bool useLastHwid,
      uint minutes,
      string reason,
      NoteSeverity severity,
      string[]? roles,
      bool erase)
    {
      this.Player = player;
      string str;
      if (ipAddress.HasValue)
        str = $"{ipAddress.Value.Item1}/{ipAddress.Value.Item2}";
      else
        str = (string) null;
      this.IpAddress = str;
      this.UseLastIp = useLastIp;
      this.Hwid = hwid;
      this.UseLastHwid = useLastHwid;
      this.Minutes = minutes;
      this.Reason = reason;
      this.Severity = severity;
      this.Roles = roles;
      this.Erase = erase;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class GetPlayerInfoRequest : EuiMessageBase
  {
    public string PlayerUsername { get; set; }

    public GetPlayerInfoRequest(string username) => this.PlayerUsername = username;
  }
}
