// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.SalvageDifficultyPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

#nullable enable
namespace Content.Shared.Procedural;

[Prototype(null, 1)]
public sealed class SalvageDifficultyPrototype : IPrototype
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("color", false, 1, false, false, null)]
  public Color Color = Color.White;
  [DataField("lootBudget", false, 1, true, false, null)]
  public float LootBudget;
  [DataField("mobBudget", false, 1, true, false, null)]
  public float MobBudget;
  [DataField("modifierBudget", false, 1, false, false, null)]
  public float ModifierBudget;
  [DataField("recommendedPlayers", false, 1, true, false, null)]
  public int RecommendedPlayers;

  [IdDataField(1, null)]
  public string ID { get; private set; } = string.Empty;
}
