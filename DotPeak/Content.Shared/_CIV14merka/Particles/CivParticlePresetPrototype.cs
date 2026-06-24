// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Particles.CivParticlePresetPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weather;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._CIV14merka.Particles;

[Prototype(null, 1)]
public sealed class CivParticlePresetPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<WeatherPrototype>> WeatherTypes = new List<ProtoId<WeatherPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public ResPath? Texture;
  [DataField(null, false, 1, false, false, null)]
  public float Density = 0.12f;
  [DataField(null, false, 1, false, false, null)]
  public int MaxParticles = 1200;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 Velocity = new Vector2(0.0f, -5f);
  [DataField(null, false, 1, false, false, null)]
  public Vector2 VelocityVariance = new Vector2(0.6f, 1.5f);
  [DataField(null, false, 1, false, false, null)]
  public float WindResponse = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float SizeMin = 0.06f;
  [DataField(null, false, 1, false, false, null)]
  public float SizeMax = 0.12f;
  [DataField(null, false, 1, false, false, null)]
  public float Stretch = 1f;
  [DataField(null, false, 1, false, false, null)]
  public Color Color = Color.White;
  [DataField(null, false, 1, false, false, null)]
  public float Alpha = 0.5f;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
