// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Prototypes.DamageTypePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared.Damage.Prototypes;

[Prototype(null, 1)]
public sealed class DamageTypePrototype : IPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, true, false, null)]
  private LocId Name { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string LocalizedName => Loc.GetString(LocId.op_Implicit(this.Name));

  [DataField("armorCoefficientPrice", false, 1, false, false, null)]
  public double ArmorPriceCoefficient { get; set; }

  [DataField("armorFlatPrice", false, 1, false, false, null)]
  public double ArmorPriceFlat { get; set; }
}
