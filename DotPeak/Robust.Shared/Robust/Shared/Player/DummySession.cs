// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Player.DummySession
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable
namespace Robust.Shared.Player;

internal sealed class DummySession : ICommonSessionInternal, ICommonSession
{
  public DummyChannel DummyChannel;

  public EntityUid? AttachedEntity { get; set; }

  public SessionStatus Status { get; set; } = SessionStatus.Connecting;

  public NetUserId UserId => this.UserData.UserId;

  public string Name => this.UserData.UserName;

  public short Ping { get; set; }

  public INetChannel Channel
  {
    get => (INetChannel) this.DummyChannel;
    [Obsolete] set => throw new NotSupportedException();
  }

  public LoginType AuthType { get; set; } = LoginType.GuestAssigned;

  public HashSet<EntityUid> ViewSubscriptions { get; } = new HashSet<EntityUid>();

  public DateTime ConnectedTime { get; set; }

  public SessionState State { get; set; } = new SessionState();

  public SessionData Data { get; set; }

  public bool ClientSide { get; set; }

  public NetUserData UserData { get; set; }

  public DummySession(NetUserId userId, string userName, SessionData data)
  {
    this.Data = data;
    this.UserData = new NetUserData(userId, userName)
    {
      HWId = ImmutableArray<byte>.Empty
    };
    this.DummyChannel = new DummyChannel(this);
  }

  public void SetStatus(SessionStatus status) => this.Status = status;

  public void SetAttachedEntity(EntityUid? uid) => this.AttachedEntity = uid;

  public void SetPing(short ping) => this.Ping = ping;

  public void SetName(string name)
  {
    this.UserData = new NetUserData(this.UserData.UserId, name)
    {
      HWId = this.UserData.HWId
    };
  }

  public void SetChannel(INetChannel channel) => throw new NotSupportedException();
}
