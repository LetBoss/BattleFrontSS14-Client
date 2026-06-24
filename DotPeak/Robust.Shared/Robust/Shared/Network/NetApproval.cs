// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetApproval
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Network;

public struct NetApproval
{
  private readonly string? _denyReason;

  public bool IsApproved => this._denyReason == null;

  public string DenyReason
  {
    get
    {
      if (this._denyReason == null)
        throw new InvalidOperationException("This was not a denial.");
      return this._denyReason;
    }
  }

  private NetApproval(string? denyReason) => this._denyReason = denyReason;

  public static NetApproval Deny(string reason) => new NetApproval(reason);

  public static NetApproval Allow() => new NetApproval((string) null);
}
