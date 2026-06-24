// Decompiled with JetBrains decompiler
// Type: Content.Shared.Audio.AudioHelpers
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using System;

#nullable enable
namespace Content.Shared.Audio;

public static class AudioHelpers
{
  private static readonly float[] SemitoneMultipliers = new float[25]
  {
    0.5f,
    0.5297273f,
    0.561227262f,
    0.5946137f,
    0.6299545f,
    0.6674091f,
    0.7071136f,
    0.7491591f,
    0.793704569f,
    0.840886354f,
    0.8909091f,
    0.94386363f,
    1f,
    1.05945456f,
    1.12245452f,
    1.18920457f,
    1.2599318f,
    1.33484089f,
    1.4142046f,
    1.4983182f,
    1.58740914f,
    1.68179548f,
    1.78179538f,
    1.88774991f,
    2f
  };

  [Obsolete("Use AudioParams.Variation data-field")]
  public static AudioParams WithVariation(float amplitude)
  {
    return AudioHelpers.WithVariation(amplitude, (IRobustRandom) null);
  }

  [Obsolete("Use AudioParams.Variation data-field")]
  public static AudioParams WithVariation(float amplitude, IRobustRandom? rand)
  {
    IoCManager.Resolve<IRobustRandom>(ref rand);
    float num = (float) RandomExtensions.NextGaussian(rand, 1.0, (double) amplitude);
    return ((AudioParams) ref AudioParams.Default).WithPitchScale(num);
  }

  public static AudioParams ShiftSemitone(AudioParams @params, int shift)
  {
    shift = MathHelper.Clamp(shift, -12, 12);
    float semitoneMultiplier = AudioHelpers.SemitoneMultipliers[shift + 12];
    return ((AudioParams) ref @params).WithPitchScale(semitoneMultiplier);
  }

  public static AudioParams WithSemitoneVariation(
    AudioParams @params,
    int variation,
    IRobustRandom rand)
  {
    IoCManager.Resolve<IRobustRandom>(ref rand);
    variation = Math.Clamp(variation, 0, 12);
    return AudioHelpers.ShiftSemitone(@params, rand.Next(-variation, variation));
  }
}
