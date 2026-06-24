// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Piping.Trinary.Components.GasMixerBoundUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Atmos.Piping.Trinary.Components;

[NetSerializable]
[Serializable]
public sealed class GasMixerBoundUserInterfaceState : BoundUserInterfaceState
{
  public string MixerLabel { get; }

  public float OutputPressure { get; }

  public bool Enabled { get; }

  public float NodeOne { get; }

  public GasMixerBoundUserInterfaceState(
    string mixerLabel,
    float outputPressure,
    bool enabled,
    float nodeOne)
  {
    this.MixerLabel = mixerLabel;
    this.OutputPressure = outputPressure;
    this.Enabled = enabled;
    this.NodeOne = nodeOne;
  }
}
