// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameStates.SessionState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Robust.Shared.GameStates;

[NetSerializable]
[Serializable]
public sealed class SessionState
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public NetUserId UserId { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string Name { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public SessionStatus Status { get; set; }

  [Obsolete("Ping data is not currently networked")]
  [Robust.Shared.ViewVariables.ViewVariables]
  public short Ping { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public NetEntity? ControlledEntity { get; set; }

  public SessionState Clone()
  {
    return new SessionState()
    {
      UserId = this.UserId,
      Name = this.Name,
      Status = this.Status,
      ControlledEntity = this.ControlledEntity
    };
  }
}
