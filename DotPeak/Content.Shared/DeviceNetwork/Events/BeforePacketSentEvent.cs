// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceNetwork.Events.BeforePacketSentEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Numerics;

#nullable enable
namespace Content.Shared.DeviceNetwork.Events;

public sealed class BeforePacketSentEvent : CancellableEntityEventArgs
{
  public readonly EntityUid Sender;
  public readonly TransformComponent SenderTransform;
  public readonly Vector2 SenderPosition;
  public readonly string NetworkId;

  public BeforePacketSentEvent(
    EntityUid sender,
    TransformComponent xform,
    Vector2 senderPosition,
    string networkId)
  {
    this.Sender = sender;
    this.SenderTransform = xform;
    this.SenderPosition = senderPosition;
    this.NetworkId = networkId;
  }
}
