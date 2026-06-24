// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Scanner.HealthScannerBuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._RMC14.Medical.Scanner;

[NetSerializable]
[Serializable]
public sealed class HealthScannerBuiState(
  NetEntity target,
  FixedPoint2 blood,
  FixedPoint2 maxBlood,
  float? temperature,
  Solution? chemicals,
  bool bleeding) : BoundUserInterfaceState
{
  public readonly NetEntity Target = target;
  public readonly FixedPoint2 Blood = blood;
  public readonly FixedPoint2 MaxBlood = maxBlood;
  public readonly float? Temperature = temperature;
  public readonly Solution? Chemicals = chemicals;
  public readonly bool Bleeding = bleeding;
}
