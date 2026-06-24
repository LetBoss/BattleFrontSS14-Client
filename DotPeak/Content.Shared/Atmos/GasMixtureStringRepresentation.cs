// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.GasMixtureStringRepresentation
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Atmos;

public readonly record struct GasMixtureStringRepresentation(
  float TotalMoles,
  float Temperature,
  float Pressure,
  Dictionary<string, float> MolesPerGas) : IFormattable
{
  public override string ToString() => $"{this.Temperature}K {this.Pressure} kPa";

  public string ToString(string? format, IFormatProvider? formatProvider) => this.ToString();

  public static implicit operator string(GasMixtureStringRepresentation rep) => rep.ToString();
}
