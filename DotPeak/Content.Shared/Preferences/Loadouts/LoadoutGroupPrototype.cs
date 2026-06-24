// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.Loadouts.LoadoutGroupPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Preferences.Loadouts;

[Prototype(null, 1)]
public sealed class LoadoutGroupPrototype : IPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public LocId Name;
  [DataField(null, false, 1, false, false, null)]
  public int MinLimit = 1;
  [DataField(null, false, 1, false, false, null)]
  public int MaxLimit = 1;
  [DataField(null, false, 1, false, false, null)]
  public bool Hidden;
  [DataField(null, false, 1, true, false, null)]
  public List<ProtoId<LoadoutPrototype>> Loadouts = new List<ProtoId<LoadoutPrototype>>();

  [IdDataField(1, null)]
  public string ID { get; private set; } = string.Empty;
}
