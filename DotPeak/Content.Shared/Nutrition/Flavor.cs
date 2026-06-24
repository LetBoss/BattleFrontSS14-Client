// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.FlavorPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared.Nutrition;

[Prototype(null, 1)]
public sealed class FlavorPrototype : IPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("flavorType", false, 1, false, false, null)]
  public FlavorType FlavorType { get; private set; }

  [DataField("description", false, 1, false, false, null)]
  public string FlavorDescription { get; private set; }
}
