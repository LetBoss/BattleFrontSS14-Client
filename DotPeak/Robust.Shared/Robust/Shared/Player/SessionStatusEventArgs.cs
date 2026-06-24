// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Player.SessionStatusEventArgs
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Enums;
using System;

#nullable enable
namespace Robust.Shared.Player;

public sealed class SessionStatusEventArgs : EventArgs
{
  public readonly ICommonSession Session;
  public readonly SessionStatus OldStatus;
  public readonly SessionStatus NewStatus;

  public SessionStatusEventArgs(
    ICommonSession session,
    SessionStatus oldStatus,
    SessionStatus newStatus)
  {
    this.Session = session;
    this.OldStatus = oldStatus;
    this.NewStatus = newStatus;
  }
}
