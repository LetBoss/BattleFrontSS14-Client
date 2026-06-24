// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.ChemMasterReagentAmountToFixedPoint
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;

#nullable disable
namespace Content.Shared.Chemistry;

public static class ChemMasterReagentAmountToFixedPoint
{
  public static FixedPoint2 GetFixedPoint(this ChemMasterReagentAmount amount)
  {
    return amount == ChemMasterReagentAmount.All ? FixedPoint2.MaxValue : FixedPoint2.New((int) amount);
  }
}
