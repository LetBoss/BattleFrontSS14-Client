// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.GasTileOverlayState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Atmos.Components;

[NetSerializable]
[Serializable]
public sealed class GasTileOverlayState : ComponentState
{
  public readonly Dictionary<Vector2i, GasOverlayChunk> Chunks;

  public GasTileOverlayState(Dictionary<Vector2i, GasOverlayChunk> chunks)
  {
    this.Chunks = chunks;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }
}
