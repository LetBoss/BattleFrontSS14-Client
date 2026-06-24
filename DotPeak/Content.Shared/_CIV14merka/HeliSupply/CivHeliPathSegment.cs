// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.HeliSupply.CivHeliPathSegment
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System.Numerics;

#nullable disable
namespace Content.Shared._CIV14merka.HeliSupply;

public sealed class CivHeliPathSegment
{
  public Vector2 A;
  public Vector2 B;
  public float Length;
  public float SpeedFactor = 1f;

  public float Cost
  {
    get => (double) this.SpeedFactor <= 0.0 ? this.Length : this.Length / this.SpeedFactor;
  }
}
