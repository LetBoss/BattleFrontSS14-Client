// Decompiled with JetBrains decompiler
// Type: Content.Shared.Salvage.Expeditions.Modifiers.SalvageWeatherMod
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

#nullable enable
namespace Content.Shared.Salvage.Expeditions.Modifiers;

[Robust.Shared.Prototypes.Prototype("salvageWeatherMod", 1)]
public sealed class SalvageWeatherMod : IPrototype, IBiomeSpecificMod, ISalvageMod
{
  [DataField("weather", false, 1, true, false, typeof (PrototypeIdSerializer<Content.Shared.Weather.WeatherPrototype>))]
  public string WeatherPrototype = string.Empty;

  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("desc", false, 1, false, false, null)]
  public LocId Description { get; private set; } = (LocId) string.Empty;

  [DataField("cost", false, 1, false, false, null)]
  public float Cost { get; private set; }

  [DataField("biomes", false, 1, false, false, typeof (PrototypeIdListSerializer<SalvageBiomeModPrototype>))]
  public System.Collections.Generic.List<string>? Biomes { get; private set; }
}
