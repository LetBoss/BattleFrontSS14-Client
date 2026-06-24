// Decompiled with JetBrains decompiler
// Type: Content.Shared.Salvage.Expeditions.SalvageFactionPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Salvage.Expeditions;

[Prototype(null, 1)]
public sealed class SalvageFactionPrototype : IPrototype
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("entries", false, 1, true, false, null)]
  public List<SalvageMobEntry> MobGroups = new List<SalvageMobEntry>();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("configs", false, 1, false, false, null)]
  public Dictionary<string, string> Configs = new Dictionary<string, string>();

  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("desc", false, 1, false, false, null)]
  public LocId Description { get; private set; } = (LocId) string.Empty;
}
