// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Player.CommonSession
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Player;

internal sealed class CommonSession : ICommonSessionInternal, ICommonSession
{
  private short _ping;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool InitialPlayerListReqDone;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool InitialResourcesDone;

  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? AttachedEntity { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public NetUserId UserId { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string Name { get; set; } = "<Unknown>";

  [Robust.Shared.ViewVariables.ViewVariables]
  public short Ping
  {
    get
    {
      INetChannel channel = this.Channel;
      return channel == null ? this._ping : channel.Ping;
    }
    set => this._ping = value;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public DateTime ConnectedTime { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public SessionState State { get; } = new SessionState();

  [Robust.Shared.ViewVariables.ViewVariables]
  public SessionStatus Status { get; set; } = SessionStatus.Connecting;

  [Robust.Shared.ViewVariables.ViewVariables]
  public SessionData Data { get; }

  public bool ClientSide { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public INetChannel Channel { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public HashSet<EntityUid> ViewSubscriptions { get; } = new HashSet<EntityUid>();

  [Robust.Shared.ViewVariables.ViewVariables]
  public LoginType AuthType
  {
    get
    {
      INetChannel channel = this.Channel;
      return channel == null ? LoginType.Guest : channel.AuthType;
    }
  }

  public override string ToString() => this.Name;

  public CommonSession(NetUserId user, string name, SessionData data)
  {
    this.UserId = user;
    this.Name = name;
    this.Data = data;
  }

  public void SetStatus(SessionStatus status) => this.Status = status;

  public void SetAttachedEntity(EntityUid? uid) => this.AttachedEntity = uid;

  public void SetPing(short ping) => this.Ping = ping;

  public void SetName(string name) => this.Name = name;

  public void SetChannel(INetChannel channel) => this.Channel = channel;
}
