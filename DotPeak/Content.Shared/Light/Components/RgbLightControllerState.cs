// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.Components.RgbLightControllerState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Light.Components;

[NetSerializable]
[Serializable]
public sealed class RgbLightControllerState : ComponentState
{
  public readonly float CycleRate;
  public List<int>? Layers;

  public RgbLightControllerState(float cycleRate, List<int>? layers)
  {
    this.CycleRate = cycleRate;
    this.Layers = layers;
  }
}
