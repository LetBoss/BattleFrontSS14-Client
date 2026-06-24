// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.StationAi.StationAiCustomizationPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Silicons.StationAi;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class StationAiCustomizationPrototype : IPrototype, IInheritingPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public LocId Name;
  [DataField(null, false, 1, true, false, null)]
  public Dictionary<string, PrototypeLayerData> LayerData = new Dictionary<string, PrototypeLayerData>();
  [DataField(null, false, 1, false, false, null)]
  public string PreviewKey = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier? PreviewBackground;

  [IdDataField(1, null)]
  public string ID { get; private set; } = string.Empty;

  [Robust.Shared.ViewVariables.ViewVariables]
  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<StationAiCustomizationPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }
}
