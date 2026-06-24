// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceNetwork.Events.DeviceNetworkPacketEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.DeviceNetwork.Events;

public sealed class DeviceNetworkPacketEvent : EntityEventArgs
{
  public int NetId;
  public readonly uint Frequency;
  public string? Address;
  public readonly string SenderAddress;
  public EntityUid Sender;
  public readonly NetworkPayload Data;

  public DeviceNetworkPacketEvent(
    int netId,
    string? address,
    uint frequency,
    string senderAddress,
    EntityUid sender,
    NetworkPayload data)
  {
    this.NetId = netId;
    this.Address = address;
    this.Frequency = frequency;
    this.SenderAddress = senderAddress;
    this.Sender = sender;
    this.Data = data;
  }
}
