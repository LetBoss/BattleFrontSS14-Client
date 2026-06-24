// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fax.FaxUiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Fax;

[NetSerializable]
[Serializable]
public sealed class FaxUiState : BoundUserInterfaceState
{
  public string DeviceName { get; }

  public Dictionary<string, string> AvailablePeers { get; }

  public string? DestinationAddress { get; }

  public bool IsPaperInserted { get; }

  public bool CanSend { get; }

  public bool CanCopy { get; }

  public FaxUiState(
    string deviceName,
    Dictionary<string, string> peers,
    bool canSend,
    bool canCopy,
    bool isPaperInserted,
    string? destAddress)
  {
    this.DeviceName = deviceName;
    this.AvailablePeers = peers;
    this.IsPaperInserted = isPaperInserted;
    this.CanSend = canSend;
    this.CanCopy = canCopy;
    this.DestinationAddress = destAddress;
  }
}
