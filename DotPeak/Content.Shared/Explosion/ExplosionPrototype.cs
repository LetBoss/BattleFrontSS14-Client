// Decompiled with JetBrains decompiler
// Type: Content.Shared.Explosion.ExplosionPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Explosion;

[Prototype(null, 1)]
public sealed class ExplosionPrototype : IPrototype
{
  [DataField("damagePerIntensity", false, 1, true, false, null)]
  public DamageSpecifier DamagePerIntensity;
  [DataField(null, false, 1, false, false, null)]
  public float? FireStacks;
  [DataField("tileBreakChance", false, 1, false, false, null)]
  public float[] _tileBreakChance = new float[2]{ 0.0f, 1f };
  [DataField("tileBreakIntensity", false, 1, false, false, null)]
  public float[] _tileBreakIntensity = new float[2]
  {
    0.0f,
    15f
  };
  [DataField("tileBreakRerollReduction", false, 1, false, false, null)]
  public float TileBreakRerollReduction = 10f;
  [DataField("lightColor", false, 1, false, false, null)]
  public Color LightColor = Color.Orange;
  [DataField("fireColor", false, 1, false, false, null)]
  public Color? FireColor;
  [DataField("smallSoundIterationThreshold", false, 1, false, false, null)]
  public int SmallSoundIterationThreshold = 6;
  [DataField(null, false, 1, false, false, null)]
  public float MaxCombineDistance = 1f;
  [DataField("sound", false, 1, false, false, null)]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundCollectionSpecifier("Explosion");
  [DataField("smallSound", false, 1, false, false, null)]
  public SoundSpecifier SmallSound = (SoundSpecifier) new SoundCollectionSpecifier("ExplosionSmall");
  [DataField("soundFar", false, 1, false, false, null)]
  public SoundSpecifier SoundFar = (SoundSpecifier) new SoundCollectionSpecifier("ExplosionFar", new AudioParams?(AudioParams.Default.WithVolume(2f)));
  [DataField("smallSoundFar", false, 1, false, false, null)]
  public SoundSpecifier SmallSoundFar = (SoundSpecifier) new SoundCollectionSpecifier("ExplosionSmallFar", new AudioParams?(AudioParams.Default.WithVolume(2f)));
  [DataField("texturePath", false, 1, false, false, null)]
  public ResPath TexturePath = new ResPath("/Textures/Effects/fire.rsi");
  [DataField("intensityPerState", false, 1, false, false, null)]
  public float IntensityPerState = 12f;
  [DataField("fireStates", false, 1, false, false, null)]
  public int FireStates = 3;

  [IdDataField(1, null)]
  public string ID { get; private set; }

  public float TileBreakChance(float intensity)
  {
    double num1 = (double) intensity;
    float[] tileBreakIntensity = this._tileBreakIntensity;
    double num2 = (double) tileBreakIntensity[tileBreakIntensity.Length - 1];
    if (num1 >= num2 || this._tileBreakIntensity.Length == 1)
    {
      float[] tileBreakChance = this._tileBreakChance;
      return tileBreakChance[tileBreakChance.Length - 1];
    }
    if ((double) intensity <= (double) this._tileBreakIntensity[0])
      return this._tileBreakChance[0];
    int index = Array.FindIndex<float>(this._tileBreakIntensity, (Predicate<float>) (k => (double) k >= (double) intensity));
    float num3 = (float) (((double) this._tileBreakChance[index] - (double) this._tileBreakChance[index - 1]) / ((double) this._tileBreakIntensity[index] - (double) this._tileBreakIntensity[index - 1]));
    return this._tileBreakChance[index - 1] + num3 * (intensity - this._tileBreakIntensity[index - 1]);
  }
}
