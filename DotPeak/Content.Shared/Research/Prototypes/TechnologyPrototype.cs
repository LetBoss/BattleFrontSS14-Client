// Decompiled with JetBrains decompiler
// Type: Content.Shared.Research.Prototypes.TechnologyPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Research.Prototypes;

[Prototype(null, 1)]
public sealed class TechnologyPrototype : IPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public LocId Name = (LocId) string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public SpriteSpecifier Icon;
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<TechDisciplinePrototype> Discipline;
  [DataField(null, false, 1, true, false, null)]
  public int Tier;
  [DataField(null, false, 1, false, false, null)]
  public bool Hidden;
  [DataField(null, false, 1, false, false, null)]
  public int Cost = 10000;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<TechnologyPrototype>> TechnologyPrerequisites = new List<ProtoId<TechnologyPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<LatheRecipePrototype>> RecipeUnlocks = new List<ProtoId<LatheRecipePrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public IReadOnlyList<GenericUnlock> GenericUnlocks = (IReadOnlyList<GenericUnlock>) new List<GenericUnlock>();

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
