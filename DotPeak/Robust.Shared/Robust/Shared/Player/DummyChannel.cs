// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Player.DummyChannel
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using System;
using System.Net;

#nullable enable
namespace Robust.Shared.Player;

internal sealed class DummyChannel(DummySession session) : INetChannel
{
  public readonly DummySession Session = session;

  public NetUserData UserData => this.Session.UserData;

  public short Ping => this.Session.Ping;

  public string UserName => this.Session.Name;

  public LoginType AuthType => this.Session.AuthType;

  public NetUserId UserId => this.Session.UserId;

  public int CurrentMtu { get; set; }

  public long ConnectionId { get; set; }

  public TimeSpan RemoteTimeOffset { get; set; }

  public TimeSpan RemoteTime { get; set; }

  public bool IsConnected { get; set; } = true;

  public bool IsHandshakeComplete { get; set; } = true;

  public IPEndPoint RemoteEndPoint { get; } = new IPEndPoint(IPAddress.Loopback, 1212);

  public NetEncryption? Encryption { get; set; }

  public INetManager NetPeer => throw new NotImplementedException();

  public T CreateNetMessage<T>() where T : NetMessage, new() => throw new NotImplementedException();

  public void SendMessage(NetMessage message) => throw new NotImplementedException();

  public void Disconnect(string reason) => throw new NotImplementedException();

  public void Disconnect(string reason, bool sendBye) => throw new NotImplementedException();

  public bool CanSendImmediately(NetDeliveryMethod method, int sequenceChannel) => true;
}
