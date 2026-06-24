// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.Generator.SharedGeneratorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.Power.Generator;

public abstract class SharedGeneratorSystem : EntitySystem
{
  public static float CalcFuelEfficiency(
    float targetPower,
    float optimalPower,
    FuelGeneratorComponent component)
  {
    return MathF.Pow(optimalPower / targetPower, component.FuelEfficiencyConstant);
  }
}
