// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Player.SessionData
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Network;

#nullable enable
namespace Robust.Shared.Player;

public sealed class SessionData
{
  public SessionData(NetUserId userId, string userName)
  {
    this.UserId = userId;
    this.UserName = userName;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public NetUserId UserId { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string UserName { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public object? ContentDataUncast { get; set; }
}
