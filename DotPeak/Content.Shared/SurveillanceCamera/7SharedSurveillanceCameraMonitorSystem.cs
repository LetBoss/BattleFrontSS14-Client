// Decompiled with JetBrains decompiler
// Type: Content.Shared.SurveillanceCamera.SurveillanceCameraSetupBoundUiState
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
public sealed class SurveillanceCameraSetupBoundUiState : BoundUserInterfaceState
{
  public string Name { get; }

  public uint Network { get; }

  public List<string> Networks { get; }

  public bool NameDisabled { get; }

  public bool NetworkDisabled { get; }

  public SurveillanceCameraSetupBoundUiState(
    string name,
    uint network,
    List<string> networks,
    bool nameDisabled,
    bool networkDisabled)
  {
    this.Name = name;
    this.Network = network;
    this.Networks = networks;
    this.NameDisabled = nameDisabled;
    this.NetworkDisabled = networkDisabled;
  }
}
