// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.AuthManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Network;

internal sealed class AuthManager : IAuthManager
{
  public const string DefaultAuthServer = "https://auth.spacestation14.com/";

  public NetUserId? UserId { get; set; }

  public string? Server { get; set; } = "https://auth.spacestation14.com/";

  public string? Token { get; set; }

  public string? PubKey { get; set; }

  public bool AllowHwid { get; set; } = true;

  public void LoadFromEnv()
  {
    string val1;
    if (TryGetVar("ROBUST_AUTH_SERVER", out val1))
      this.Server = val1;
    string val2;
    if (TryGetVar("ROBUST_AUTH_USERID", out val2))
      this.UserId = new NetUserId?(new NetUserId(Guid.Parse(val2)));
    string val3;
    if (TryGetVar("ROBUST_AUTH_PUBKEY", out val3))
      this.PubKey = val3;
    string val4;
    if (TryGetVar("ROBUST_AUTH_TOKEN", out val4))
      this.Token = val4;
    string val5;
    if (!TryGetVar("ROBUST_AUTH_ALLOW_HWID", out val5))
      return;
    this.AllowHwid = val5.Trim() == "1";

    static bool TryGetVar(string var, [NotNullWhen(true)] out string? val)
    {
      val = Environment.GetEnvironmentVariable(var);
      return val != null;
    }
  }
}
