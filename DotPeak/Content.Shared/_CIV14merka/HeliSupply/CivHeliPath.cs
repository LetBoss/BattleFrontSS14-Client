// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.HeliSupply.CivHeliPath
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._CIV14merka.HeliSupply;

public sealed class CivHeliPath
{
  public readonly List<CivHeliPathSegment> Segments = new List<CivHeliPathSegment>();
  public float[] PointDist = Array.Empty<float>();
  public float Total;
  public float TotalCost;

  public void SampleByCost(float cost, out Vector2 pos, out Vector2 tangent)
  {
    pos = Vector2.Zero;
    tangent = Vector2.UnitX;
    if (this.Segments.Count == 0)
      return;
    float num = 0.0f;
    for (int index = 0; index < this.Segments.Count; ++index)
    {
      CivHeliPathSegment segment = this.Segments[index];
      if ((double) cost <= (double) num + (double) segment.Cost || index == this.Segments.Count - 1)
      {
        float amount = (double) segment.Cost > 0.0 ? Math.Clamp((cost - num) / segment.Cost, 0.0f, 1f) : 1f;
        pos = Vector2.Lerp(segment.A, segment.B, amount);
        tangent = segment.B - segment.A;
        break;
      }
      num += segment.Cost;
    }
  }

  public float DistAtCost(float cost)
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    foreach (CivHeliPathSegment segment in this.Segments)
    {
      if ((double) cost <= (double) num1 + (double) segment.Cost)
      {
        float num3 = (double) segment.Cost > 0.0 ? Math.Clamp((cost - num1) / segment.Cost, 0.0f, 1f) : 1f;
        return num2 + segment.Length * num3;
      }
      num1 += segment.Cost;
      num2 += segment.Length;
    }
    return num2;
  }

  public float CostAtDist(float dist)
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    foreach (CivHeliPathSegment segment in this.Segments)
    {
      if ((double) dist <= (double) num1 + (double) segment.Length)
      {
        float num3 = (double) segment.Length > 0.0 ? Math.Clamp((dist - num1) / segment.Length, 0.0f, 1f) : 1f;
        return num2 + segment.Cost * num3;
      }
      num1 += segment.Length;
      num2 += segment.Cost;
    }
    return num2;
  }
}
