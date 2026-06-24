// Decompiled with JetBrains decompiler
// Type: Content.Shared.Salvage.Expeditions.Modifiers.SalvageBiomeModPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Parallax.Biomes;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

#nullable enable
namespace Content.Shared.Salvage.Expeditions.Modifiers;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class SalvageBiomeModPrototype : IPrototype, ISalvageMod
{
  [DataField("weather", false, 1, false, false, null)]
  public bool Weather = true;
  [DataField("biome", false, 1, true, false, typeof (PrototypeIdSerializer<BiomeTemplatePrototype>))]
  public string? BiomePrototype;

  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("desc", false, 1, false, false, null)]
  public LocId Description { get; private set; } = (LocId) string.Empty;

  [DataField("cost", false, 1, false, false, null)]
  public float Cost { get; private set; }
}
