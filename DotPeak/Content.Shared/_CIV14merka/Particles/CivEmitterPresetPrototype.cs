// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Particles.CivEmitterPresetPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System.Numerics;

#nullable enable
namespace Content.Shared._CIV14merka.Particles;

[Prototype(null, 1)]
public sealed class CivEmitterPresetPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public ResPath? Texture;
  [DataField(null, false, 1, false, false, null)]
  public float Rate;
  [DataField(null, false, 1, false, false, null)]
  public int BurstCount;
  [DataField(null, false, 1, false, false, null)]
  public float Radius;
  [DataField(null, false, 1, false, false, null)]
  public float Life = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float LifeVariance = 0.3f;
  [DataField(null, false, 1, false, false, null)]
  public float Speed = 2f;
  [DataField(null, false, 1, false, false, null)]
  public float SpeedVariance = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float Direction = 90f;
  [DataField(null, false, 1, false, false, null)]
  public float Spread = 360f;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 Gravity = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  public float Drag = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  public float SizeStart = 0.3f;
  [DataField(null, false, 1, false, false, null)]
  public float SizeEnd = 0.8f;
  [DataField(null, false, 1, false, false, null)]
  public float AlphaStart = 0.6f;
  [DataField(null, false, 1, false, false, null)]
  public float AlphaEnd;
  [DataField(null, false, 1, false, false, null)]
  public Color Color = Color.White;
  [DataField(null, false, 1, false, false, null)]
  public float WindResponse = 0.3f;
  [DataField(null, false, 1, false, false, null)]
  public float Stretch = 1f;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
