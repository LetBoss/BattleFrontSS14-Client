// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Prototypes.GasPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

#nullable enable
namespace Content.Shared.Atmos.Prototypes;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class GasPrototype : IPrototype
{
  [DataField("gasVisbilityFactor", false, 1, false, false, null)]
  public float GasVisibilityFactor = 20f;

  [DataField("name", false, 1, false, false, null)]
  public string Name { get; set; } = "";

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("specificHeat", false, 1, false, false, null)]
  public float SpecificHeat { get; private set; }

  [DataField("heatCapacityRatio", false, 1, false, false, null)]
  public float HeatCapacityRatio { get; private set; } = 1.4f;

  [DataField("molarMass", false, 1, false, false, null)]
  public float MolarMass { get; set; } = 1f;

  [DataField("gasMolesVisible", false, 1, false, false, null)]
  public float GasMolesVisible { get; private set; } = 0.25f;

  public float GasMolesVisibleMax => this.GasMolesVisible * this.GasVisibilityFactor;

  [DataField("gasOverlayTexture", false, 1, false, false, null)]
  public string GasOverlayTexture { get; private set; } = string.Empty;

  [DataField("gasOverlayState", false, 1, false, false, null)]
  public string GasOverlayState { get; set; } = string.Empty;

  [DataField("gasOverlaySprite", false, 1, false, false, null)]
  public string GasOverlaySprite { get; set; } = string.Empty;

  [DataField("overlayPath", false, 1, false, false, null)]
  public string OverlayPath { get; private set; } = string.Empty;

  [DataField("reagent", false, 1, false, false, typeof (PrototypeIdSerializer<ReagentPrototype>))]
  public string? Reagent { get; private set; }

  [DataField("color", false, 1, false, false, null)]
  public string Color { get; private set; } = string.Empty;

  [DataField("pricePerMole", false, 1, false, false, null)]
  public float PricePerMole { get; set; }
}
