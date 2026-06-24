// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.EntitySystems.SharedAtmosDebugOverlaySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedAtmosDebugOverlaySystem : EntitySystem
{
  public const int LocalViewRange = 16 /*0x10*/;
  protected float AccumulatedFrameTime;

  [NetSerializable]
  [Serializable]
  public readonly record struct AtmosDebugOverlayData(
    Vector2 Indices,
    float Temperature,
    float[]? Moles,
    AtmosDirection PressureDirection,
    AtmosDirection LastPressureDirection,
    AtmosDirection BlockDirection,
    int? InExcitedGroup,
    bool IsSpace,
    bool MapAtmosphere,
    bool NoGrid,
    bool Immutable)
  ;

  [NetSerializable]
  [Serializable]
  public sealed class AtmosDebugOverlayMessage : EntityEventArgs
  {
    public NetEntity GridId { get; }

    public Vector2i BaseIdx { get; }

    public SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData?[] OverlayData { get; }

    public AtmosDebugOverlayMessage(
      NetEntity gridIndices,
      Vector2i baseIdx,
      SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData?[] overlayData)
    {
      this.GridId = gridIndices;
      this.BaseIdx = baseIdx;
      this.OverlayData = overlayData;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class AtmosDebugOverlayDisableMessage : EntityEventArgs
  {
  }
}
