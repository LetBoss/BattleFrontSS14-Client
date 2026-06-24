// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Medical.CivReviveRules
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;

#nullable disable
namespace Content.Shared._CIV14merka.Medical;

public static class CivReviveRules
{
  public const float ReviveWindowMinutes = 4f;
  public static readonly FixedPoint2 MaxReviveDamage = FixedPoint2.New(300);
  public const float CorpseHealMultiplier = 2f;
}
