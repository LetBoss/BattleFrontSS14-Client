// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.NetProbe.PubgNetProbeAckEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.NetProbe;

[NetSerializable]
[Serializable]
public sealed class PubgNetProbeAckEvent : EntityEventArgs
{
  public int RequestId { get; }

  public PubgNetProbeAckStatus Status { get; }

  public string LocKey { get; }

  public int Kb { get; }

  public int ReceivedBytes { get; }

  public int ExpectedBytes { get; }

  public PubgNetProbeAckEvent(
    int requestId,
    PubgNetProbeAckStatus status,
    string locKey,
    int kb,
    int receivedBytes,
    int expectedBytes)
  {
    this.RequestId = requestId;
    this.Status = status;
    this.LocKey = locKey;
    this.Kb = kb;
    this.ReceivedBytes = receivedBytes;
    this.ExpectedBytes = expectedBytes;
  }
}
