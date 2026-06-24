// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.StationAi.StationAiCustomizationGroupPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Silicons.StationAi;

[Prototype(null, 1)]
public sealed class StationAiCustomizationGroupPrototype : IPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public LocId Name;
  [DataField(null, false, 1, false, false, null)]
  public StationAiCustomizationType Category;
  [DataField(null, false, 1, true, false, null)]
  public List<ProtoId<StationAiCustomizationPrototype>> ProtoIds = new List<ProtoId<StationAiCustomizationPrototype>>();

  [IdDataField(1, null)]
  public string ID { get; private set; } = string.Empty;
}
