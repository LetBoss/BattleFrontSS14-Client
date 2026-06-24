// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.SpawnExplosionEuiMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eui;
using Content.Shared.Explosion.Components;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Administration;

public static class SpawnExplosionEuiMsg
{
  [NetSerializable]
  [Serializable]
  public sealed class PreviewRequest : EuiMessageBase
  {
    public readonly MapCoordinates Epicenter;
    public readonly string TypeId;
    public readonly float TotalIntensity;
    public readonly float IntensitySlope;
    public readonly float MaxIntensity;

    public PreviewRequest(
      MapCoordinates epicenter,
      string typeId,
      float totalIntensity,
      float intensitySlope,
      float maxIntensity)
    {
      this.Epicenter = epicenter;
      this.TypeId = typeId;
      this.TotalIntensity = totalIntensity;
      this.IntensitySlope = intensitySlope;
      this.MaxIntensity = maxIntensity;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class PreviewData : EuiMessageBase
  {
    public readonly float Slope;
    public readonly float TotalIntensity;
    public readonly ExplosionVisualsState Explosion;

    public PreviewData(ExplosionVisualsState explosion, float slope, float totalIntensity)
    {
      this.Slope = slope;
      this.TotalIntensity = totalIntensity;
      this.Explosion = explosion;
    }
  }
}
