// Decompiled with JetBrains decompiler
// Type: Content.Shared.SurveillanceCamera.SurveillanceCameraMonitorUiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.SurveillanceCamera;

[NetSerializable]
[Serializable]
public sealed class SurveillanceCameraMonitorUiState : BoundUserInterfaceState
{
  public string ActiveAddress;

  public NetEntity? ActiveCamera { get; }

  public HashSet<string> Subnets { get; }

  public string ActiveSubnet { get; }

  public Dictionary<string, string> Cameras { get; }

  public SurveillanceCameraMonitorUiState(
    NetEntity? activeCamera,
    HashSet<string> subnets,
    string activeAddress,
    string activeSubnet,
    Dictionary<string, string> cameras)
  {
    this.ActiveCamera = activeCamera;
    this.Subnets = subnets;
    this.ActiveAddress = activeAddress;
    this.ActiveSubnet = activeSubnet;
    this.Cameras = cameras;
  }
}
