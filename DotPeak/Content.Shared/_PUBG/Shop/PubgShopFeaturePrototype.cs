// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Shop.PubgShopFeaturePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared._PUBG.Shop;

[Prototype(null, 1)]
public sealed class PubgShopFeaturePrototype : IPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public string Kind = string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public string TitleLocKey = string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public string DescriptionLocKey = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier? Icon;
  [DataField(null, false, 1, false, false, null)]
  public string? IconEntity;
  [DataField(null, false, 1, false, false, null)]
  public int Price;
  [DataField(null, false, 1, false, false, null)]
  public string Currency = "coins";
  [DataField(null, false, 1, false, false, null)]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  public int Order;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
