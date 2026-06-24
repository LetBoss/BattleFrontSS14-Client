// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reagent.ReagentGuideEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Prototypes;
using Content.Shared.EntityEffects;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Chemistry.Reagent;

[NetSerializable]
[Serializable]
public struct ReagentGuideEntry
{
  public string ReagentPrototype;
  public Dictionary<ProtoId<MetabolismGroupPrototype>, ReagentEffectsGuideEntry>? GuideEntries;
  public List<string>? PlantMetabolisms;

  public ReagentGuideEntry(
    Content.Shared.Chemistry.Reagent.ReagentPrototype proto,
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    this.PlantMetabolisms = (List<string>) null;
    this.ReagentPrototype = proto.ID;
    FrozenDictionary<ProtoId<MetabolismGroupPrototype>, ReagentEffectsEntry> metabolisms = proto.Metabolisms;
    this.GuideEntries = metabolisms != null ? metabolisms.Select<KeyValuePair<ProtoId<MetabolismGroupPrototype>, ReagentEffectsEntry>, (ProtoId<MetabolismGroupPrototype>, ReagentEffectsGuideEntry)>((Func<KeyValuePair<ProtoId<MetabolismGroupPrototype>, ReagentEffectsEntry>, (ProtoId<MetabolismGroupPrototype>, ReagentEffectsGuideEntry)>) (x => (x.Key, x.Value.MakeGuideEntry(prototype, entSys)))).ToDictionary<(ProtoId<MetabolismGroupPrototype>, ReagentEffectsGuideEntry), ProtoId<MetabolismGroupPrototype>, ReagentEffectsGuideEntry>((Func<(ProtoId<MetabolismGroupPrototype>, ReagentEffectsGuideEntry), ProtoId<MetabolismGroupPrototype>>) (x => x.Key), (Func<(ProtoId<MetabolismGroupPrototype>, ReagentEffectsGuideEntry), ReagentEffectsGuideEntry>) (x => x.Item2)) : (Dictionary<ProtoId<MetabolismGroupPrototype>, ReagentEffectsGuideEntry>) null;
    if (proto.PlantMetabolisms.Count <= 0)
      return;
    this.PlantMetabolisms = new List<string>((IEnumerable<string>) proto.PlantMetabolisms.Select<EntityEffect, string>((Func<EntityEffect, string>) (x => x.GuidebookEffectDescription(prototype, entSys))).Where<string>((Func<string, bool>) (x => x != null)).Select<string, string>((Func<string, string>) (x => x)).ToArray<string>());
  }
}
