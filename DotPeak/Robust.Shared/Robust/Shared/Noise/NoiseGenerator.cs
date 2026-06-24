// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Noise.NoiseGenerator
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Noise;

[Obsolete("Use FastNoiseLite")]
public sealed class NoiseGenerator
{
  private readonly FastNoise _fastNoiseInstance;
  private float _periodX;
  private float _periodY;

  public NoiseGenerator(NoiseGenerator.NoiseType type)
  {
    this._fastNoiseInstance = new FastNoise();
    this._fastNoiseInstance.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
    this._fastNoiseInstance.SetFractalLacunarity(2.09439516f);
    if (type != NoiseGenerator.NoiseType.Fbm)
    {
      if (type != NoiseGenerator.NoiseType.Ridged)
        throw new ArgumentOutOfRangeException(nameof (type), (object) type, (string) null);
      this._fastNoiseInstance.SetFractalType(FastNoise.FractalType.RigidMulti);
    }
    else
      this._fastNoiseInstance.SetFractalType(FastNoise.FractalType.FBM);
  }

  public void SetFrequency(float frequency) => this._fastNoiseInstance.SetFrequency(frequency);

  public void SetLacunarity(float lacunarity)
  {
    this._fastNoiseInstance.SetFractalLacunarity(lacunarity);
  }

  public void SetPersistence(float persistence)
  {
    this._fastNoiseInstance.SetFractalGain(persistence);
  }

  public void SetPeriodX(float periodX) => this._periodX = periodX;

  public void SetPeriodY(float periodY) => this._periodY = periodY;

  public void SetOctaves(uint octaves) => this._fastNoiseInstance.SetFractalOctaves((int) octaves);

  public void SetSeed(uint seed) => this._fastNoiseInstance.SetSeed((int) seed);

  public float GetNoiseTiled(float x, float y) => this.GetNoiseTiled(new Vector2(x, y));

  public float GetNoiseTiled(Vector2 vec)
  {
    float num1 = vec.X / this._periodX;
    float num2 = vec.Y / this._periodY;
    return this.GetNoise((float) (0.0 + Math.Cos((double) num1 * 6.2831854820251465) * 0.15915493667125702), (float) (0.0 + Math.Cos((double) num2 * 6.2831854820251465) * 0.15915493667125702), (float) (0.0 + Math.Sin((double) num1 * 6.2831854820251465) * 0.15915493667125702), (float) (0.0 + Math.Sin((double) num2 * 6.2831854820251465) * 0.15915493667125702));
  }

  public float GetNoise(float x) => this.GetNoise(new Vector2(x, 0.0f));

  public float GetNoise(float x, float y) => this._fastNoiseInstance.GetSimplexFractal(x, y);

  public float GetNoise(Vector2 vector) => this.GetNoise(vector.X, vector.Y);

  public float GetNoise(float x, float y, float z)
  {
    return this._fastNoiseInstance.GetSimplexFractal(x, y, z);
  }

  public float GetNoise(Vector3 vector) => this.GetNoise(vector.X, vector.Y, vector.Z);

  public float GetNoise(float x, float y, float z, float w)
  {
    return this._fastNoiseInstance.GetSimplex(x, y, z, w);
  }

  public float GetNoise(Vector4 vector) => this.GetNoise(vector.X, vector.Y, vector.Z, vector.W);

  public enum NoiseType : byte
  {
    Fbm,
    Ridged,
  }
}
